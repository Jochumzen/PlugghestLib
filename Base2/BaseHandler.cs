using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Services.Localization;
using Latex2MathML;
using Plugghest.DNN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;

namespace Plugghest.Base2
{
    public class BaseHandler
    {
        private int PortalID = 0;  //Global. As for now - only one Portal: 0

        BaseRepository rep = new BaseRepository();

        #region Plugg/PluggContainer

        public void SavePlugg(PluggContainer p, List<object> cs)
        {
            if (p.ThePlugg.CreatedInCultureCode != null && p.CultureCode != p.ThePlugg.CreatedInCultureCode)
                throw new Exception("Cannot use SavePlugg unless you are saving it in the creation language.");

            p.ThePlugg.CreatedInCultureCode = p.CultureCode;

            try
            {
                bool isNew = p.ThePlugg.PluggId == 0;

                //Temporary - to avoid login - remove soon
                p.ThePlugg.CreatedByUserId = 1;
                p.ThePlugg.ModifiedByUserId = 1;

                //Save Plugg entity
                p.ThePlugg.ModifiedOnDate = DateTime.Now;

                if (isNew)
                {
                    p.ThePlugg.CreatedOnDate = DateTime.Now;
                    rep.CreatePlugg(p.ThePlugg);
                }
                else
                    rep.UpdatePlugg(p.ThePlugg);

                //Save Title
                if (p.TheTitle == null || p.TheTitle.Text == null)
                    throw new Exception("Cannot Save Plugg. TheTitle cannot be null");

                if (p.TheTitle.TextId == 0)
                {
                    p.TheTitle.ItemId = p.ThePlugg.PluggId;
                    p.TheTitle.ItemType = ETextItemType.PluggTitle;
                    p.TheTitle.CultureCodeStatus = ECultureCodeStatus.InCreationLanguage;
                    p.TheTitle.CreatedByUserId = p.ThePlugg.CreatedByUserId;
                    p.TheTitle.ModifiedByUserId = p.ThePlugg.ModifiedByUserId;
                }
                SavePhTextInAllCc(p.TheTitle);  //Save or Update

                if (p.TheDescription != null && p.TheDescription.Text != null)
                {
                    if (p.TheDescription.TextId == 0)
                    {
                        p.TheDescription.ItemId = p.ThePlugg.PluggId;
                        p.TheDescription.ItemType = ETextItemType.PluggDescription;
                        p.TheDescription.CultureCodeStatus = ECultureCodeStatus.InCreationLanguage;
                        p.TheDescription.CreatedByUserId = p.ThePlugg.CreatedByUserId;
                        p.TheDescription.ModifiedByUserId = p.ThePlugg.ModifiedByUserId;
                    }
                    SavePhTextInAllCc(p.TheDescription);                    
                }

                int cmpOrder = 1;
                PluggComponent pc = new PluggComponent();
                pc.PluggId = p.ThePlugg.PluggId;
                foreach (object cmp in cs)
                {
                    pc.ComponentOrder = cmpOrder;
                    switch (cmp.GetType().Name)
                    {
                        case "PHText":
                            PHText theText = (PHText)cmp;
                            switch (theText.ItemType)
                            {
                                case ETextItemType.PluggComponentRichRichText:
                                    pc.ComponentType = EComponentType.RichRichText;
                                    break;
                                case ETextItemType.PluggComponentRichText:
                                    pc.ComponentType = EComponentType.RichText;
                                    break;
                                case ETextItemType.PluggComponentLabel:
                                    pc.ComponentType = EComponentType.Label;
                                    break;
                            }
                            rep.CreatePluggComponent(pc);
                            theText.ItemId = pc.PluggComponentId;
                            theText.CultureCode = p.ThePlugg.CreatedInCultureCode;
                            theText.CreatedByUserId = p.ThePlugg.CreatedByUserId;
                            SavePhTextInAllCc(theText);
                            break;
                        case "PHLatex":
                            PHLatex theLatex = (PHLatex)cmp;
                            pc.ComponentType = EComponentType.Latex;
                            rep.CreatePluggComponent(pc);
                            theLatex.ItemId = pc.PluggComponentId;
                            theLatex.CultureCode = p.ThePlugg.CreatedInCultureCode;
                            theLatex.CreatedByUserId = p.ThePlugg.CreatedByUserId;
                            SaveLatexTextInAllCc(theLatex);
                            break;
                        case "YouTube":
                            pc.ComponentType = EComponentType.YouTube;
                            rep.CreatePluggComponent(pc);
                            YouTube theVideo = (YouTube)cmp;
                            theVideo.CreatedByUserId = p.ThePlugg.CreatedByUserId;
                            theVideo.PluggComponentId = pc.PluggComponentId;
                            SaveYouTube(theVideo);
                            break;
                    }
                    cmpOrder++;
                }

                //Create PluggPage
                DNNHelper d = new DNNHelper();
                string pageUrl = p.ThePlugg.PluggId.ToString();
                string pageName = pageUrl + ": " + p.TheTitle.Text;
                TabInfo newTab = d.AddPluggPage(pageName, pageUrl);
                p.ThePlugg.TabId = newTab.TabID;
                rep.UpdatePlugg(p.ThePlugg);
            }
            catch (Exception)
            {
                //Todo: Update
                //DeletePlugg(p.ThePlugg);
                throw;
            }
        }

        public void DeletePlugg(Plugg p)
        {
        //    // Todo: Don't delete Plugg if: It has comments or ratings, Its included in a course.
        //    // Todo: Soft delete of Plugg
        //    if (p == null)
        //    {
        //        throw new Exception("Cannot delete: Plugg not initialized");
        //        return;
        //    }

        //    TabController tabController = new TabController();
        //    TabInfo getTab = tabController.GetTab(p.TabId);

        //    if (getTab != null)
        //    {
        //        DNNHelper h = new DNNHelper();
        //        h.DeleteTab(getTab);
        //    }

        //    rep.DeleteAllPhTextForItem(p.PluggId, (int)ETextItemType.PluggTitle);
        //    rep.DeleteAllPhTextForItem(p.PluggId, (int)ETextItemType.PluggHtml);
        //    rep.DeleteAllLatexForItem(p.PluggId, (int)ELatexType.Plugg);

        //    rep.DeletePlugg(p);
        }

        /// <summary>
        /// Add a new component to a Plugg
        /// Existing PluggComponents must be in p.TheComponents
        /// newComponentData is of type PHText, PHLatex or YouTube
        /// newComponent must have ComponentOrder and ComponentType set
        /// </summary>
        /// <param name="p"></param>
        /// <param name="newComponent"></param>
        /// <param name="newComponentData"></param>
        public void AddComponent(PluggContainer p, PluggComponent newComponent, object newComponentData = null)
        {
            List<PluggComponent> cmps = p.GetComponentList();
            if (newComponent.ComponentOrder < 1 || newComponent.ComponentOrder > cmps.Count + 1)
                throw new Exception("ComponentOrder is out of range");
            if (newComponent.ComponentType == EComponentType.NotSet)
                throw new Exception("You must set ComponentType");

            PluggComponent pcToUpdate;
            for (int order = newComponent.ComponentOrder; order <= cmps.Count; order++ )
            {
                pcToUpdate = rep.GetPluggComponent(cmps[order].PluggComponentId);
                pcToUpdate.ComponentOrder += 1;
                rep.UpdatePluggComponent(pcToUpdate);
            }

            newComponent.PluggId = p.ThePlugg.PluggId;
            rep.CreatePluggComponent(newComponent);           
        }

        /// <summary>
        /// Deletes the PluggComponent at position "delOrder".
        /// Existing PluggComponents must be in p.TheComponents
        /// Deletes ComponenData as well if exist
        /// </summary>
        /// <param name="p"></param>
        /// <param name="order"></param>
        public void DeleteComponent(PluggContainer p, int delOrder)
        {
            List<PluggComponent> cmps = p.GetComponentList();
            if (delOrder < 1 || delOrder > cmps.Count + 1)
                throw new Exception("order is out of range");

            PluggComponent pcToDelete = cmps[delOrder];
            if(pcToDelete.PluggComponentId != 0)
            {
                switch(pcToDelete.ComponentType)
                {
                    case EComponentType.Label:
                        rep.DeleteAllPhTextForItem(pcToDelete.PluggComponentId, ETextItemType.PluggComponentLabel);
                        break;
                    case EComponentType.RichText:
                        rep.DeleteAllPhTextForItem(pcToDelete.PluggComponentId, ETextItemType.PluggComponentRichText);
                        break;
                    case EComponentType.RichRichText:
                        rep.DeleteAllPhTextForItem(pcToDelete.PluggComponentId, ETextItemType.PluggComponentRichRichText);
                        break;
                    case EComponentType.Latex:
                        rep.DeleteAllLatexForItem(pcToDelete.PluggComponentId, ELatexItemType.PluggComponentLatex);
                        break;
                    case EComponentType.YouTube:
                        rep.DeleteYouTube(new YouTube() { YouTubeId = pcToDelete.PluggComponentId });
                        break;
                }
            }

            PluggComponent pcToUpdate;
            for (int order = delOrder+1; order <= cmps.Count; order++)
            {
                pcToUpdate = rep.GetPluggComponent(cmps[order].PluggComponentId);
                pcToUpdate.ComponentOrder -= 1;
                rep.UpdatePluggComponent(pcToUpdate);
            }
        }

        #endregion

        #region Course

        //public void CreateCourse(Course c, List<CourseItemEntity> cis)
        //{
        //    rep.CreateCourse(c);

        //    try
        //    {
        //        foreach (CourseItemEntity ci in cis)
        //        {
        //            ci.CourseId = c.CourseId;
        //            rep.CreateCourseItem(ci);
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        DeleteCourse(c);
        //        throw;
        //    }

        //    //Create CoursePage
        //    DNNHelper d = new DNNHelper();
        //    string pageUrl = "C" + c.CourseId.ToString();
        //    string pageName = pageUrl + ": " + c.Title;
        //    try
        //    {
        //        TabInfo newTab = d.AddCoursePage(pageName, pageUrl);
        //        c.TabId = newTab.TabID;
        //        rep.UpdateCourse(c);
        //    }
        //    catch (Exception)
        //    {
        //        DeleteCourse(c);
        //        throw;
        //    }
        //}

        //public Course GetCourse(int CourseID)
        //{
        //    return rep.GetCourse(CourseID);
        //}

        //public void DeleteCourse(Course c)
        //{
        //    // Todo: Don't delete course if: It has comments or ratings
        //    // Todo: Soft delete of Course
        //    if (c == null)
        //    {
        //        throw new Exception("Cannot delete: Course not initialized");
        //        return;
        //    }

        //    TabController tabController = new TabController();
        //    TabInfo getTab = tabController.GetTab(c.TabId);

        //    if (getTab != null)
        //    {
        //        DNNHelper h = new DNNHelper();
        //        h.DeleteTab(getTab);
        //    }

        //    var cis = rep.GetItemsInCourse(c.CourseId);
        //    foreach (CourseItem ciDelete in cis)
        //    {
        //        rep.DeleteCourseItem(ciDelete);
        //    }

        //    rep.DeleteCourse(c);
        //}

        #endregion

        #region CourseItem

        //public IEnumerable<CourseItem> GetCourseItems(int CourseID, int ItemID)
        //{
        //    return rep.GetCourseItems(CourseID, ItemID);
        //}

        //public List<CourseItem> GetItemsInCourse(int courseId)
        //{
        //    return rep.GetItemsInCourse(courseId);
        //}

        //public IList<CourseItem> FlatToHierarchy(IEnumerable<CourseItem> list, int motherId = 0)
        //{
        //    return (from i in list
        //            where i.MotherId == motherId
        //            select new CourseItem
        //            {
        //                CourseItemId = i.CourseItemId,
        //                CourseId = i.CourseId,
        //                ItemId = i.ItemId,
        //                CIOrder = i.CIOrder,
        //                ItemType = i.ItemType,
        //                MotherId = i.MotherId,
        //                //Mother = i,
        //                label = i.label,
        //                name = i.name,
        //                children = FlatToHierarchy(list, i.CourseItemId)
        //            }).ToList();
        //}

        //public IList<CourseItem> GetCourseItemsAsTree(int courseId)
        //{
        //    List<CourseItem> source = GetItemsInCourse(courseId);
        //    return FlatToHierarchy(source);
        //}

        ////It is assumed that all CourseItems are in the same course
        //public void SaveCourseItems(IList<CourseItem> cis, int courseId, int motherId = 0)
        //{
        //    CourseItemEntity cie = new CourseItemEntity();
        //    int ciOrder = 1;
        //    foreach (CourseItem ci in cis)
        //    {
        //        DeleteCourseItem(ci); //Deletes heading as well if item is a heading

        //        if (ci.ItemType == ECourseItemType.Heading)
        //        {
        //            CourseMenuHeadings ch = new CourseMenuHeadings();
        //            ch.Title = ci.name;
        //            rep.CreateHeading(ch);
        //            cie.ItemId = ch.HeadingID;
        //        }
        //        else
        //            cie.ItemId = ci.ItemId; 
                
        //        cie.CourseId = courseId;
        //        cie.CIOrder = ciOrder;
        //        cie.ItemType = ci.ItemType;
        //        cie.MotherId = motherId;
        //        rep.CreateCourseItem(cie);
        //        ciOrder += 1;
        //        if (ci.children != null)
        //            SaveCourseItems(ci.children, courseId, cie.CourseItemId);
        //    }
        //}

        //public void CreateCourseItem(CourseItem ci)
        //{
        //    rep.CreateCourseItem(ci);
        //}

        //public void UpdateCourseItem(CourseItem ci)
        //{
        //    rep.UpdateCourseItem(ci);
        //}

        //public void DeleteCourseItem(CourseItem ci)
        //{
        //    if (ci.ItemType == ECourseItemType.Heading)
        //        rep.DeleteHeading(new CourseMenuHeadings() {HeadingID =ci.ItemId});
        //    rep.DeleteCourseItem(ci);
        //}

        #endregion

        #region PHTextAndLatex

        /// <summary>
        /// This method will save the text in all Culture Codes
        /// It expects the text to be created in t.CultureCode
        /// It will call SavePhText(t)
        /// It then translates t into all languages and calls SavePhText on each text
        /// </summary>
        /// <param name="t"></param>
        public void SavePhTextInAllCc(PHText t)
        {
            t.CultureCodeStatus = ECultureCodeStatus.InCreationLanguage;
            SavePhText(t);  //Save Text in created language
            LocaleController lc = new LocaleController();
            var locales = lc.GetLocales(PortalID);
            PHText translatedText;
            foreach (var locale in locales)
            {
                if (locale.Key != t.CultureCode)
                {
                    translatedText = GetCurrentVersionText(locale.Key, t.ItemId, t.ItemType);
                    translatedText.Text = TranslateText(t.CultureCode.Substring(0, 2), locale.Key.Substring(0, 2), t.Text);
                    translatedText.CultureCodeStatus = ECultureCodeStatus.GoogleTranslated;
                    SavePhText(translatedText);
                }
            }
        }

        /// <summary>
        /// Must set Text, ItemId, ItemType, CultureCode, CultureCodeStatus and CreatedByUserId or it will not save anything
        /// If text is versioned, it creates a new version
        /// If text is not versioned, it creates new text or updates text depending on TextId.
        /// </summary>
        /// <param name="t"></param>
        public void SavePhText(PHText t)
        {
            if (t.Text == null || t.ItemId == 0 || t.ItemType == ETextItemType.NotSet || t.CultureCode == null || t.CultureCodeStatus == ECultureCodeStatus.NotSet || t.CreatedByUserId == 0)
                return;  //Nothing to save

            if (t.ModifiedByUserId == 0)
                t.ModifiedByUserId = t.CreatedByUserId;
            t.ModifiedOnDate = DateTime.Now;
            t.CurrentVersion = true;
            bool isVersioned = (t.ItemType == ETextItemType.PluggComponentRichRichText || t.ItemType == ETextItemType.PluggComponentRichText || t.ItemType == ETextItemType.CoursePluggText || t.ItemType == ETextItemType.CourseRichRichText);

            if (isVersioned)
            {
                var prevText = rep.GetCurrentVersionText(t.CultureCode, t.ItemId, t.ItemType);
                if (prevText == null)
                {
                    t.Version = 1;
                }
                else
                {
                    t.Version = prevText.Version++;
                    prevText.CurrentVersion = false;
                    rep.UpdatePhText(prevText);
                }
                t.CreatedOnDate = DateTime.Now;
                rep.CreatePhText(t);
            }
            else
            {
                t.Version = 0;
                if (t.TextId == 0)
                {
                    t.CreatedOnDate = DateTime.Now;
                    rep.CreatePhText(t);
                }
                else
                    rep.UpdatePhText(t);
            }
        }

        /// <summary>
        /// This method will save the LatexText in all Culture Codes
        /// It expects the text to be created in t.CultureCode
        /// It will call SaveLatexText(t)
        /// It then translates t into all languages and calls SaveLatexText on each text
        /// </summary>
        /// <param name="t"></param>
        public void SaveLatexTextInAllCc(PHLatex t)
        {
            SaveLatexText(t);  //Save LatexText in created language
            LocaleController lc = new LocaleController();
            var locales = lc.GetLocales(PortalID);
            foreach (var locale in locales)
            {
                if (locale.Key != t.CultureCode)
                {
                    t.Text = TranslateText(t.CultureCode.Substring(0, 2), locale.Key.Substring(0, 2), t.Text);

                    t.LatexId = 0;
                    t.CultureCode = locale.Key;
                    t.CultureCodeStatus = ECultureCodeStatus.GoogleTranslated;
                    SaveLatexText(t);
                }
            }
        }

        /// <summary>
        /// Must set Text, ItemId, ItemType, CultureCode, CultureCodeStatus and CreatedByUserId or it will not save anything
        /// If text is versioned, it creates a new version
        /// If text is not versioned, it creates new text or updates text depending on TextId.        
        /// </summary>
        /// <param name="t"></param>
        public void SaveLatexText(PHLatex t)
        {
            if (t.Text == null || t.ItemId == 0 || t.ItemType == ELatexItemType.NotSet || t.CultureCode == null ||
                t.CultureCodeStatus == ECultureCodeStatus.NotSet || t.CreatedByUserId == 0)
                return; //Nothing to save

            if (t.ModifiedByUserId == 0)
                t.ModifiedByUserId = t.CreatedByUserId;
            t.ModifiedOnDate = DateTime.Now;
            t.CurrentVersion = true;
            if (t.HtmlText==null || t.HtmlText == "")
            {
                LatexToMathMLConverter myConverter = new LatexToMathMLConverter(t.Text);
                myConverter.Convert();
                t.HtmlText = myConverter.HTMLOutput;
            }
            bool isVersioned = (t.ItemType == ELatexItemType.PluggComponentLatex || t.ItemType == ELatexItemType.CourseLatexText);

            if (isVersioned)
            {
                var prevText = rep.GetCurrentVersionLatexText(t.CultureCode, t.ItemId, t.ItemType);
                if (prevText == null)
                {
                    t.Version = 1;
                }
                else
                {
                    t.Version = prevText.Version++;
                    prevText.CurrentVersion = false;
                    rep.UpdateLatexText(prevText);
                }
                t.CreatedOnDate = DateTime.Now;
                rep.CreateLatexText(t);
            }
            else
            {
                t.Version = 0;
                if (t.LatexId == 0)
                {
                    t.CreatedOnDate = DateTime.Now;
                    rep.CreateLatexText(t);
                }
                else
                    rep.UpdateLatexText(t);
            }
        }

        /// <summary>
        /// Will get the latest version of text in language cultureCode for itemType/itemId
        /// If the text does not exist, it creates a PHText where TextId=0
        /// </summary>
        /// <param name="cultureCode"></param>
        /// <param name="itemId"></param>
        /// <param name="itemType"></param>
        /// <returns></returns>
        public PHText GetCurrentVersionText(string cultureCode, int itemId, ETextItemType itemType)
        {
            PHText txt = rep.GetCurrentVersionText(cultureCode, itemId, itemType);
            if(txt == null)
            {
                txt = new PHText();
                txt.Text = "(No text)";
                txt.CultureCode = cultureCode;
                txt.ItemId = itemId;
                txt.ItemType = itemType;
            }
            return txt;
        }

        /// <summary>
        /// Will return all versions of text in language cultureCode for itemType/itemId
        /// May be null if no versions exist
        /// </summary>
        /// <param name="cultureCode"></param>
        /// <param name="itemId"></param>
        /// <param name="itemType"></param>
        /// <returns></returns>
        public IEnumerable<PHText> GetAllVersionsText(string cultureCode, int itemId, ETextItemType itemType)
        {
            return rep.GetAllVersionsText(cultureCode, itemId, itemType);
        }

        /// <summary>
        /// Will get the latest version of LatexText in language cultureCode for itemType/itemId
        /// If the text does not exist, it creates a LatexText where LatexId=0
        /// </summary>
        /// <param name="cultureCode"></param>
        /// <param name="itemId"></param>
        /// <param name="itemType"></param>
        /// <returns></returns>
        public PHLatex GetCurrentVersionLatexText(string cultureCode, int itemId, ELatexItemType itemType)
        {
            PHLatex txt = rep.GetCurrentVersionLatexText(cultureCode, itemId, itemType);
            if (txt == null)
            {
                txt = new PHLatex();
                txt.Text = "(No text)";
                txt.HtmlText = "(No text)";
                txt.CultureCode = cultureCode;
                txt.ItemId = itemId;
                txt.ItemType = itemType;
            }
            return txt;
        }

        /// <summary>
        /// Will return all versions of LatexText in language cultureCode for itemType/itemId
        /// May be null if no versions exist
        /// </summary>
        /// <param name="cultureCode"></param>
        /// <param name="itemId"></param>
        /// <param name="itemType"></param>
        /// <returns></returns>
        public IEnumerable<PHLatex> GetAllVersionsLatexText(string cultureCode, int itemId, ELatexItemType itemType)
        {
            return rep.GetAllVersionsLatexText(cultureCode, itemId, itemType);
        }    

        #endregion

        #region YouTube

        public void SaveYouTube(YouTube y)
        {
            if (y == null || y.YouTubeCode == null || y.YouTubeCode == "")
                return;  //Nothing to save

            if (y.YouTubeId == 0)
            {
                y.CreatedOn = DateTime.Now;
                rep.CreateYouTube(y);
            }
            else
                rep.UpdateYouTube(y);
        }

        public YouTube GetYouTubeByComponentId(int pluggComponentId)
        {
            return rep.GetYouTubeByComponentId(pluggComponentId);
        }

        #endregion

        #region Other

        //public IEnumerable<PluggInfoForDNNGrid> GetPluggListForGrid(string cultureCode)
        //{
        //    return rep.GetPluggRecords(cultureCode);
        //}

        //public List<CourseInfoForDNNGrid> GetCoursesForDNN()
        //{
        //    return rep.GetCoursesForDNN();
        //}

        ////public List<CourseItem> GetCourseItemsForTree(int CourseID)
        ////{
        ////    return rep.GetCourseItemsForTree(CourseID);
        ////}

        #endregion

        #region CourseMenuHeading

        //public CourseMenuHeadings CreateHeading(CourseMenuHeadings h)
        //{
        //    return rep.CreateHeading(h);
        //}

        //public void UpdateHeading(CourseMenuHeadings h)
        //{
        //    rep.UpdateHeading(h);
        //}

        #endregion

        #region CourseItemComment
        //public IEnumerable<CourseItemComment> GetCourseItemComment(int CourseID, int ItemID)
        //{
        //    return rep.GetCourseItemComment(CourseID, ItemID);
        //}
        //public void UpdateCourseItemComment(CourseItemComment CIC)
        //{
        //    rep.UpdateCourseItemComment(CIC);
        //}


        #endregion

        #region Translate Function and class
        private string TranslateText(string strFromLanguage, string strToLanguage, string strTextToTranslate)
        {
            string url = "https://www.googleapis.com/language/translate/v2?key=AIzaSyBJHrFbepkPej62Q1o0GUiDuL2ceYuFcW8&format=html&source=" + strFromLanguage + "&target=" + strToLanguage + "&q=" + strTextToTranslate;

            WebRequest request = HttpWebRequest.Create(url);

            try
            {
                WebResponse response = request.GetResponse();

                StreamReader reader = new StreamReader(response.GetResponseStream());

                string urlText = reader.ReadToEnd();

                JavaScriptSerializer TheSerializer = new JavaScriptSerializer();

                TranslaterResult ObjResult = (TranslaterResult)new JavaScriptSerializer().Deserialize(urlText, typeof(TranslaterResult));
                string strtranslatedText = ObjResult.data.translations[0].translatedText;
                return strtranslatedText;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        private class TranslaterResult
        {
            public Data data { get; set; }

            public class Translation
            {
                public string translatedText { get; set; }
            }
            public class Data
            {
                public List<Translation> translations { get; set; }
            }
        }
        #endregion
    }
}
