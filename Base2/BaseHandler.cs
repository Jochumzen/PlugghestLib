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

        //public void SavePlugg(PluggContainer p, List<object> cs)
        //{
        //    if (p.ThePlugg.CreatedInCultureCode != null && p.CultureCode != p.ThePlugg.CreatedInCultureCode)
        //        throw new Exception("Cannot use SavePlugg unless you are saving it in the creation language.");

        //    p.ThePlugg.CreatedInCultureCode = p.CultureCode;

        //    try
        //    {
        //        bool isNew = p.ThePlugg.PluggId == 0;

        //        //Temporary - to avoid login - remove soon
        //        p.ThePlugg.CreatedByUserId = 1;
        //        p.ThePlugg.ModifiedByUserId = 1;

        //        //Save Plugg entity
        //        p.ThePlugg.ModifiedOnDate = DateTime.Now;

        //        if (isNew)
        //        {
        //            p.ThePlugg.CreatedOnDate = DateTime.Now;
        //            rep.CreatePlugg(p.ThePlugg);
        //        }
        //        else
        //            rep.UpdatePlugg(p.ThePlugg);

        //        //Save Title
        //        if (p.TheTitle == null || p.TheTitle.Text == null)
        //            throw new Exception("Cannot Save Plugg. TheTitle cannot be null");

        //        if (p.TheTitle.TextId == 0)
        //        {
        //            p.TheTitle.ItemId = p.ThePlugg.PluggId;
        //            p.TheTitle.ItemType = ETextItemType.PluggTitle;
        //            p.TheTitle.CultureCodeStatus = ECultureCodeStatus.InCreationLanguage;
        //            p.TheTitle.CreatedByUserId = p.ThePlugg.CreatedByUserId;
        //            p.TheTitle.ModifiedByUserId = p.ThePlugg.ModifiedByUserId;
        //        }
        //        SavePhTextInAllCc(p.TheTitle);  //Save or Update

        //        if (p.TheDescription != null && p.TheDescription.Text != null)
        //        {
        //            if (p.TheDescription.TextId == 0)
        //            {
        //                p.TheDescription.ItemId = p.ThePlugg.PluggId;
        //                p.TheDescription.ItemType = ETextItemType.PluggDescription;
        //                p.TheDescription.CultureCodeStatus = ECultureCodeStatus.InCreationLanguage;
        //                p.TheDescription.CreatedByUserId = p.ThePlugg.CreatedByUserId;
        //                p.TheDescription.ModifiedByUserId = p.ThePlugg.ModifiedByUserId;
        //            }
        //            SavePhTextInAllCc(p.TheDescription);
        //        }

        //        int cmpOrder = 1;
        //        PluggComponent pc = new PluggComponent();
        //        pc.PluggId = p.ThePlugg.PluggId;
        //        foreach (object cmp in cs)
        //        {
        //            pc.ComponentOrder = cmpOrder;
        //            switch (cmp.GetType().Name)
        //            {
        //                case "PHText":
        //                    PHText theText = (PHText)cmp;
        //                    switch (theText.ItemType)
        //                    {
        //                        case ETextItemType.PluggComponentRichRichText:
        //                            pc.ComponentType = EComponentType.RichRichText;
        //                            break;
        //                        case ETextItemType.PluggComponentRichText:
        //                            pc.ComponentType = EComponentType.RichText;
        //                            break;
        //                        case ETextItemType.PluggComponentLabel:
        //                            pc.ComponentType = EComponentType.Label;
        //                            break;
        //                    }
        //                    rep.CreatePluggComponent(pc);
        //                    theText.ItemId = pc.PluggComponentId;
        //                    theText.CultureCode = p.ThePlugg.CreatedInCultureCode;
        //                    theText.CreatedByUserId = p.ThePlugg.CreatedByUserId;
        //                    SavePhTextInAllCc(theText);
        //                    break;
        //                case "PHLatex":
        //                    PHLatex theLatex = (PHLatex)cmp;
        //                    pc.ComponentType = EComponentType.Latex;
        //                    rep.CreatePluggComponent(pc);
        //                    theLatex.ItemId = pc.PluggComponentId;
        //                    theLatex.CultureCode = p.ThePlugg.CreatedInCultureCode;
        //                    theLatex.CreatedByUserId = p.ThePlugg.CreatedByUserId;
        //                    SaveLatexTextInAllCc(theLatex);
        //                    break;
        //                case "YouTube":
        //                    pc.ComponentType = EComponentType.YouTube;
        //                    rep.CreatePluggComponent(pc);
        //                    YouTube theVideo = (YouTube)cmp;
        //                    theVideo.CreatedByUserId = p.ThePlugg.CreatedByUserId;
        //                    theVideo.PluggComponentId = pc.PluggComponentId;
        //                    SaveYouTube(theVideo);
        //                    break;
        //            }
        //            cmpOrder++;
        //        }

        //        //Create PluggPage
        //        DNNHelper d = new DNNHelper();
        //        string pageUrl = p.ThePlugg.PluggId.ToString();
        //        string pageName = pageUrl + ": " + p.TheTitle.Text;
        //        TabInfo newTab = d.AddPluggPage(pageName, pageUrl);
        //        p.ThePlugg.TabId = newTab.TabID;
        //        rep.UpdatePlugg(p.ThePlugg);
        //    }
        //    catch (Exception)
        //    {
        //        //Todo: Update
        //        //DeletePlugg(p.ThePlugg);
        //        throw;
        //    }
        //}

        public void CreateBasicPlugg(PluggContainer p)
        {
            if (p.CultureCode == null || p.CultureCode == "")
                throw new Exception("Cannot Create Plugg. CutureCode cannot be null");

            if (p.TheTitle == null || p.TheTitle.Text == null || p.TheTitle.Text == "")
                throw new Exception("Cannot Save Plugg. Title cannot be null");

            p.ThePlugg.CreatedInCultureCode = p.CultureCode;
            p.ThePlugg.CreatedOnDate = DateTime.Now;
            p.ThePlugg.ModifiedByUserId = p.ThePlugg.CreatedByUserId;
            p.ThePlugg.ModifiedOnDate = p.ThePlugg.CreatedOnDate;
            p.ThePlugg.IsDeleted = false;
            p.ThePlugg.IsListed = true;
            rep.CreatePlugg(p.ThePlugg);

            //Save Title
            p.TheTitle.ItemId = p.ThePlugg.PluggId;
            p.TheTitle.ItemType = ETextItemType.PluggTitle;
            p.TheTitle.CultureCode = p.CultureCode;
            p.TheTitle.CultureCodeStatus = ECultureCodeStatus.InCreationLanguage;
            p.TheTitle.CreatedByUserId = p.ThePlugg.CreatedByUserId;
            p.TheTitle.ModifiedByUserId = p.ThePlugg.ModifiedByUserId;
            SavePhTextInAllCc(p.TheTitle);  //Save or Update

            //Save Description
            if (p.TheDescription != null && p.TheDescription.Text != null && p.TheDescription.Text != "")
            {
                p.TheDescription.ItemId = p.ThePlugg.PluggId;
                p.TheDescription.ItemType = ETextItemType.PluggDescription;
                p.TheDescription.CultureCode = p.CultureCode;
                p.TheDescription.CultureCodeStatus = ECultureCodeStatus.InCreationLanguage;
                p.TheDescription.CreatedByUserId = p.ThePlugg.CreatedByUserId;
                p.TheDescription.ModifiedByUserId = p.ThePlugg.ModifiedByUserId;
                SavePhTextInAllCc(p.TheDescription);
            }

            PluggComponent video = new PluggComponent();
            video.ComponentOrder = 1;
            video.ComponentType = EComponentType.YouTube;
            AddComponent(p, video);

            PluggComponent rrText = new PluggComponent();
            rrText.ComponentOrder = 2;
            rrText.ComponentType = EComponentType.RichRichText;
            p.TheComponents = null;
            AddComponent(p, rrText);

            //Create PluggPage
            DNNHelper d = new DNNHelper();
            string pageUrl = p.ThePlugg.PluggId.ToString();
            string pageName = pageUrl + ": " + p.TheTitle.Text;
            TabInfo newTab = d.AddPluggPage(pageName, pageUrl);
            p.ThePlugg.TabId = newTab.TabID;
            rep.UpdatePlugg(p.ThePlugg);
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
            for (int order = newComponent.ComponentOrder; order <= cmps.Count; order++)
            {
                pcToUpdate = rep.GetPluggComponent(cmps[order - 1].PluggComponentId);
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
            if (delOrder < 1 || delOrder > cmps.Count)
                throw new Exception("order is out of range");

            PluggComponent pcToDelete = cmps[delOrder - 1];
            if (pcToDelete.PluggComponentId != 0)
            {
                switch (pcToDelete.ComponentType)
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
                        rep.DeleteYouTube(GetYouTubeByComponentId(pcToDelete.PluggComponentId));
                        break;
                }
            }

            rep.DeletePluggComponent(pcToDelete);

            PluggComponent pcToUpdate;
            for (int order = delOrder + 1; order <= cmps.Count; order++)
            {
                pcToUpdate = rep.GetPluggComponent(cmps[order - 1].PluggComponentId);
                pcToUpdate.ComponentOrder -= 1;
                rep.UpdatePluggComponent(pcToUpdate);
            }
        }

        #endregion

        #region Course/CourseContainer

        public void CreateCourse(CourseContainer c)
        {
            if (c.CultureCode == null || c.CultureCode == "")
                throw new Exception("Cannot Create Course. CutureCode cannot be null");

            if (c.TheTitle == null || c.TheTitle.Text == null || c.TheTitle.Text == "")
                throw new Exception("Cannot Save Course. Title cannot be null");

            c.TheCourse.CreatedInCultureCode = c.CultureCode;
            c.TheCourse.CreatedOnDate = DateTime.Now;
            c.TheCourse.ModifiedByUserId = c.TheCourse.CreatedByUserId;
            c.TheCourse.ModifiedOnDate = c.TheCourse.CreatedOnDate;
            c.TheCourse.IsDeleted = false;
            c.TheCourse.IsListed = true;
            rep.CreateCourse(c.TheCourse);

            //Save Title
            c.TheTitle.ItemId = c.TheCourse.CourseId;
            c.TheTitle.ItemType = ETextItemType.CourseTitle;
            c.TheTitle.CultureCode = c.CultureCode;
            c.TheTitle.CultureCodeStatus = ECultureCodeStatus.InCreationLanguage;
            c.TheTitle.CreatedByUserId = c.TheCourse.CreatedByUserId;
            c.TheTitle.ModifiedByUserId = c.TheCourse.ModifiedByUserId;
            SavePhTextInAllCc(c.TheTitle);  //Save or Update

            //Save Description
            if (c.TheDescription != null && c.TheDescription.Text != null && c.TheDescription.Text != "")
            {
                c.TheDescription.ItemId = c.TheCourse.CourseId;
                c.TheDescription.ItemType = ETextItemType.CourseDescription;
                c.TheDescription.CultureCode = c.CultureCode;
                c.TheDescription.CultureCodeStatus = ECultureCodeStatus.InCreationLanguage;
                c.TheDescription.CreatedByUserId = c.TheCourse.CreatedByUserId;
                c.TheDescription.ModifiedByUserId = c.TheCourse.ModifiedByUserId;
                SavePhTextInAllCc(c.TheDescription);
            }

            //Create CoursePage
            DNNHelper d = new DNNHelper();
            string pageUrl = "C" + c.TheCourse.CourseId.ToString();
            string pageName = pageUrl + ": " + c.TheTitle.Text;
            TabInfo newTab = d.AddCoursePage(pageName, pageUrl);
            c.TheCourse.TabId = newTab.TabID;
            rep.UpdateCourse(c.TheCourse);
        }

        #endregion

        #region CoursePluggs

        public CoursePluggEntity GetCPEntity(int cpId)
        {
            return rep.GetCoursePlugg(cpId);
        }

        public List<CoursePlugg> GetPluggsInCourse(int courseId, string ccCode)
        {
            return rep.GetPluggsInCourse(courseId, ccCode);
        }

        public IList<CoursePlugg> FlatToHierarchy(IEnumerable<CoursePlugg> list, int Id = 0, bool _isChildren = true)
        {
            return (from i in list
                    where (i.MotherId == Id && _isChildren) || (i.CoursePluggId == Id && !_isChildren)
                    select new CoursePlugg
                    {
                        CoursePluggId = i.CoursePluggId,
                        CourseId = i.CourseId,
                        PluggId = i.PluggId,
                        CPOrder = i.CPOrder,
                        MotherId = i.MotherId,
                        label = i.label,
                        Mother = FlatToHierarchy(list, i.MotherId, false).FirstOrDefault(),
                        children = _isChildren ? FlatToHierarchy(list, i.CoursePluggId, true) : null
                    }).ToList();
        }

        //public IList<CoursePlugg> FlatToHierarchy(IEnumerable<CoursePlugg> list, int motherId = 0)
        //{
        //    return (from i in list
        //            where i.MotherId == motherId
        //            select new CoursePlugg
        //            {
        //                CoursePluggId = i.CoursePluggId,
        //                CourseId = i.CourseId,
        //                PluggId = i.PluggId,
        //                CPOrder = i.CPOrder,
        //                MotherId = i.MotherId,
        //                //Mother = i,
        //                label = i.label,
        //                children = FlatToHierarchy(list, i.CoursePluggId)
        //            }).ToList();
        //}

        public IList<CoursePlugg> GetCoursePluggsAsTree(int courseId, string ccCode)
        {
            List<CoursePlugg> source = GetPluggsInCourse(courseId, ccCode);
            return FlatToHierarchy(source);
        }

        public CoursePlugg FindCoursePlugg(IList<CoursePlugg> cps, int coursePluggId)
        {
            foreach (CoursePlugg cp in cps)
            {
                if (cp.CoursePluggId == coursePluggId)
                    return cp;
                if (cp.children != null)
                {
                    CoursePlugg fcp = FindCoursePlugg(cp.children, coursePluggId);
                    if (fcp != null)
                        return fcp;
                }
            }
            return null;
        }

        /// <summary>
        /// This method updates the CoursePluggs tree.
        /// It assumes that only the positions in the tree have changed.
        /// It assumes that no new Pluggs have been added and no Pluggs have been deleted.
        /// </summary>
        /// <param name="ss">A hierarchy of CoursePluggs</param>
        public void UpdateCourseTree(IList<CoursePlugg> ss, int motherId = 0)
        {
            int cpOrder = 1;
            foreach (CoursePlugg s in ss)
            {
                s.MotherId = motherId;
                s.CPOrder = cpOrder;
                rep.UpdateCoursePlugg(new CoursePluggEntity { CoursePluggId = s.CoursePluggId, CourseId = s.CourseId, PluggId = s.PluggId, CPOrder = cpOrder, MotherId = motherId });
                cpOrder += 1;
                if (s.children != null)
                    UpdateCourseTree(s.children, s.CoursePluggId);
            }
        }

        public void CreateCP(CoursePluggEntity cp)
        {
            if (cp == null || cp.CoursePluggId != 0 || cp.CPOrder == 0)
                throw new Exception("Cannot create CoursePlugg");
            IEnumerable<CoursePluggEntity> sameMother = rep.GetChildrenCP(cp.MotherId);
            foreach (CoursePluggEntity tmpCP in sameMother)
            {
                if (tmpCP.CPOrder >= cp.CPOrder)
                {
                    tmpCP.CPOrder++;
                    rep.UpdateCoursePlugg(tmpCP);
                }
            }
            cp.CreatedOnDate = DateTime.Now;
            rep.CreateCoursePlugg(cp);
        }

        public void DeleteCP(int CoursePluggId)
        {
            CoursePluggEntity cp = rep.GetCoursePlugg(CoursePluggId);
            IEnumerable<CoursePluggEntity> sameMother = rep.GetChildrenCP(cp.MotherId);
            foreach (CoursePluggEntity tmpCp in sameMother)
            {
                if (tmpCp.CPOrder > cp.CPOrder)
                {
                    tmpCp.CPOrder--;
                    rep.UpdateCoursePlugg(tmpCp);
                }
            }
            rep.DeleteCoursePlugg(cp);
        }

        #endregion

        #region PHTextAndLatex

        /// <summary>
        /// This method will save the text in all Culture Codes
        /// It expects the text to be created in t.CultureCode
        /// It expects the text to be decoded (actual html)
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
                    if (translatedText == null)
                    {
                        translatedText = new PHText();
                        translatedText.CultureCode = locale.Key;
                        translatedText.ItemId = t.ItemId;
                        translatedText.ItemType = t.ItemType;
                    }
                    translatedText.Text = TranslateText(t.CultureCode.Substring(0, 2), locale.Key.Substring(0, 2), t.Text);
                    if (translatedText.CreatedByUserId == 0)
                        translatedText.CreatedByUserId = t.CreatedByUserId;
                    translatedText.CultureCodeStatus = ECultureCodeStatus.GoogleTranslated;
                    SavePhText(translatedText);
                }
            }
        }

        /// <summary>
        /// Must set Text, ItemId, ItemType, CultureCode, CultureCodeStatus and CreatedByUserId or it will not save anything
        /// If text is html, it must be decoded (like <p>Hello</p>)
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
                    t.Version = prevText.Version + 1;
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
            t.CultureCodeStatus = ECultureCodeStatus.InCreationLanguage;
            SaveLatexText(t);  //Save LatexText in created language
            LocaleController lc = new LocaleController();
            var locales = lc.GetLocales(PortalID);
            PHLatex translatedText;
            foreach (var locale in locales)
            {
                if (locale.Key != t.CultureCode)
                {
                    translatedText = GetCurrentVersionLatexText(locale.Key, t.ItemId, t.ItemType);
                    if (translatedText == null)
                    {
                        translatedText = new PHLatex();
                        translatedText.CultureCode = locale.Key;
                        translatedText.ItemId = t.ItemId;
                        translatedText.ItemType = t.ItemType;
                    }
                    translatedText.Text = t.Text;
                    translatedText.HtmlText = TranslateText(t.CultureCode.Substring(0, 2), locale.Key.Substring(0, 2), t.HtmlText);
                    if (translatedText.CreatedByUserId == 0)
                        translatedText.CreatedByUserId = t.CreatedByUserId;
                    translatedText.CultureCodeStatus = ECultureCodeStatus.GoogleTranslated;
                    SaveLatexText(translatedText);
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
            if (t.HtmlText == null || t.HtmlText == "")
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
                    t.Version = prevText.Version + 1;
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
        /// Text will be decoded (actual html)
        /// If the text does not exist, it creates a new PHText object where TextId=0
        /// </summary>
        /// <param name="cultureCode"></param>
        /// <param name="itemId"></param>
        /// <param name="itemType"></param>
        /// <returns></returns>
        public PHText GetCurrentVersionText(string cultureCode, int itemId, ETextItemType itemType)
        {
            PHText txt = rep.GetCurrentVersionText(cultureCode, itemId, itemType);
            return txt;
        }

        /// <summary>
        /// Will return all versions of text in language cultureCode for itemType/itemId
        /// Text will be decoded (actual html)
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
        /// htmltext will be decoded (actual html)
        /// </summary>
        /// <param name="cultureCode"></param>
        /// <param name="itemId"></param>
        /// <param name="itemType"></param>
        /// <returns></returns>
        public PHLatex GetCurrentVersionLatexText(string cultureCode, int itemId, ELatexItemType itemType)
        {
            PHLatex txt = rep.GetCurrentVersionLatexText(cultureCode, itemId, itemType);
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

        public YouTube GetYouTube(int youTubeId)
        {
            return rep.GetYouTube(youTubeId);
        }

        public YouTube GetYouTubeByComponentId(int pluggComponentId)
        {
            return rep.GetYouTubeByComponentId(pluggComponentId);
        }

        #endregion

        #region Subjects

        /// <summary>
        /// Gets the SubjectEntity (SubjectId, MotherId, SubjectOrder) for a given SubjectId
        /// </summary>
        /// <param name="subjectId"></param>
        /// <returns></returns>
        public SubjectEntity GetSubjectEntity(int subjectId)
        {
            return rep.GetSubject(subjectId);
        }

        /// <summary>
        /// Gets a flat list of all Subject. 
        /// Sets the SubjectEntity as well as the title of the subject in the language cultureCode.
        /// As it is a flat list, it does NOT set Mother or Children. Use FlatToHierarchy or GetSubjectsAsTree to set these.
        /// </summary>
        /// <param name="cultureCode"></param>
        /// <returns></returns>
        public IEnumerable<Subject> GetSubjectsAsFlatList(string cultureCode)
        {
            IEnumerable<Subject> ss = rep.GetAllSubjects(cultureCode);
            return ss;
        }

        /// <summary>
        /// Converts a flat list of all Subjects into a hierarchy.
        /// Will set Mother as well as Children
        /// </summary>
        /// <param name="list">Get list from GetSubjectsAsFlatList</param>
        /// <param name="motherId">Do not use this parameter</param>
        /// <returns></returns>
        public static IList<Subject> FlatToHierarchy(IEnumerable<Subject> list, int Id = 0, bool _isChildren = true)
        {
            return (from i in list
                    where (i.MotherId == Id && _isChildren) || (i.SubjectId == Id && !_isChildren)
                    select new Subject
                    {
                        SubjectId = i.SubjectId,
                        SubjectOrder = i.SubjectOrder,
                        MotherId = i.MotherId,
                        label = i.label,
                        Mother = FlatToHierarchy(list, i.MotherId, false).FirstOrDefault(),
                        children = _isChildren ? FlatToHierarchy(list, i.SubjectId, true) : null
                    }).ToList();
        }
        
        
        //public IList<Subject> FlatToHierarchy(IEnumerable<Subject> list, int motherId = 0, Subject mother = null)
        //{
        //    return (from i in list
        //            where i.MotherId == motherId
        //            select new Subject
        //            {
        //                SubjectId = i.SubjectId,
        //                SubjectOrder = i.SubjectOrder,
        //                MotherId = i.MotherId,
        //                label = i.label,
        //                Mother = mother,
        //                children = FlatToHierarchy(list, i.SubjectId, i)
        //            }).ToList();
        //}

        /// <summary>
        /// Get all Subjects a tree hierarchy with the title of the subject in the language cultureCode
        /// </summary>
        /// <param name="cultureCode"></param>
        /// <returns></returns>
        public IList<Subject> GetSubjectsAsTree(string cultureCode)
        {
            IEnumerable<Subject> source = GetSubjectsAsFlatList(cultureCode);
            return FlatToHierarchy(source);
        }

        public Subject FindSubject(IList<Subject> ss, int subjectId)
        {
            foreach (Subject s in ss)
            {
                if (s.SubjectId == subjectId)
                    return s;
                if (s.children.Count > 0)
                {
                    Subject fs = FindSubject(s.children, subjectId);
                    if (fs != null)
                        return fs;
                }
            }
            return null;
        }

        /// <summary>
        /// This method updates the subject tree.
        /// It assumes that only the positions in the tree of the subjects have changed.
        /// It assumes that NO new subjects have been added and NO subjects have been deleted.
        /// It assumes that NO subjects have been renamed or translated.
        /// </summary>
        /// <param name="ss">A hierarchy of subjects</param>
        public void UpdateSubjectTree(IList<Subject> ss, int motherId = 0)
        {
            int subjectOrder = 1;
            foreach (Subject s in ss)
            {
                rep.UpdateSubject(new SubjectEntity { SubjectId = s.SubjectId, MotherId = motherId, SubjectOrder = subjectOrder });
                subjectOrder += 1;
                if (s.children != null)
                    UpdateSubjectTree(s.children, s.SubjectId);
            }
        }

        public void CreateSubject(Subject s, int userId)
        {
            if (s == null || s.SubjectId != 0 || s.SubjectOrder == 0)
                throw new Exception("Cannot create subject");
            IEnumerable<SubjectEntity> sameMother = rep.GetChildrenSubjects(s.MotherId);
            foreach (SubjectEntity tmpS in sameMother)
            {
                if (tmpS.SubjectOrder >= s.SubjectOrder)
                {
                    tmpS.SubjectOrder++;
                    rep.UpdateSubject(tmpS);
                }
            }
            SubjectEntity se = new SubjectEntity { SubjectOrder = s.SubjectOrder, MotherId = s.MotherId };
            rep.CreateSubject(se);
            PHText sText = new PHText(s.label, "en-US", ETextItemType.Subject);
            sText.CreatedByUserId = userId;
            sText.ItemId = se.SubjectId;
            sText.CultureCodeStatus = ECultureCodeStatus.InCreationLanguage;
            SavePhTextInAllCc(sText);
        }

        public void DeleteSubject(int subjectId)
        {
            rep.DeleteAllPhTextForItem(subjectId, ETextItemType.Subject);
            SubjectEntity s = rep.GetSubject(subjectId);
            IEnumerable<SubjectEntity> sameMother = rep.GetChildrenSubjects(s.MotherId);
            foreach (SubjectEntity tmpS in sameMother)
            {
                if (tmpS.SubjectOrder > s.SubjectOrder)
                {
                    tmpS.SubjectOrder--;
                    rep.UpdateSubject(tmpS);
                }
            }
            rep.DeleteSubject(s);
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
            string url = "https://www.googleapis.com/language/translate/v2?key=AIzaSyBJHrFbepkPej62Q1o0GUiDuL2ceYuFcW8&format=html&source=" + strFromLanguage + "&target=" + strToLanguage + "&q=" + System.Web.HttpUtility.UrlEncode(strTextToTranslate);

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

        public IEnumerable<sp_SearchResult> Get_SearchResult(string _key)
        {
            return rep.GetAllPhtext(_key);
        }

    }
}