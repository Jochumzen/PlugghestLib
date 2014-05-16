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

namespace Plugghest.Base
{
    public class BaseHandler
    {
        private int PortalID = 0;  //Global. As for now - only one Portal: 0

        BaseRepository rep = new BaseRepository();

        #region Plugg

        public Plugg GetPlugg(int pluggId)
        {
            return rep.GetPlugg(pluggId);
        }

        public IEnumerable<Plugg> GetAllPluggs()
        {
            return rep.GetAllPluggs();
        }

        public void SavePlugg(PluggContainer p)
        {
            try
            {
                bool isNew = p.ThePlugg.PluggId == 0;

                //Temporary - remove soon
                p.ThePlugg.Title = "Title no longer here";
                p.ThePlugg.CreatedByUserId = 1;
                p.ThePlugg.ModifiedByUserId = 1;

                if (isNew)
                    rep.CreatePlugg(p.ThePlugg);
                else
                    rep.UpdatePlugg(p.ThePlugg);

                //Todo: Update..
                p.TheTitle.ItemId = p.ThePlugg.PluggId;
                p.TheTitle.ItemType = ETextItemType.PluggTitle;
                p.TheTitle.CcStatus = ECCStatus.InCreationLanguage;
                p.TheTitle.CreatedByUserId = p.ThePlugg.CreatedByUserId;
                p.TheTitle.ModifiedByUserId = p.ThePlugg.ModifiedByUserId;
                SavePhText(p.TheTitle);

                //Todo: Update..
                if (p.TheHtmlText != null)
                {
                    p.TheHtmlText.ItemId = p.ThePlugg.PluggId;
                    p.TheHtmlText.ItemType = ETextItemType.PluggHtml;
                    p.TheHtmlText.CcStatus = ECCStatus.InCreationLanguage;
                    p.TheHtmlText.CreatedByUserId = p.ThePlugg.CreatedByUserId;
                    p.TheHtmlText.ModifiedByUserId = p.ThePlugg.ModifiedByUserId;
                    SavePhText(p.TheHtmlText);
                }

                //Todo: Update..
                if (p.TheLatex != null)
                {
                    p.TheLatex.ItemId = p.ThePlugg.PluggId;
                    p.TheLatex.ItemType = ELatexType.Plugg;
                    p.TheLatex.CcStatus = ECCStatus.InCreationLanguage;
                    p.TheLatex.CreatedByUserId = p.ThePlugg.CreatedByUserId;
                    p.TheLatex.ModifiedByUserId = p.ThePlugg.ModifiedByUserId;
                    LatexToMathMLConverter myConverter = new LatexToMathMLConverter(p.TheLatex.Text);
                    myConverter.Convert();
                    p.TheLatex.HtmlText = myConverter.HTMLOutput;
                    SaveLatexText(p.TheLatex);
                }

                LocaleController lc = new LocaleController();
                var locales = lc.GetLocales(PortalID);
                foreach (var locale in locales)
                {
                    if (locale.Key != p.ThePlugg.CreatedInCultureCode)
                    {
                        GoogleTranslate(p.TheTitle, locale.Key);
                        if (p.TheHtmlText != null)
                            GoogleTranslate(p.TheHtmlText, locale.Key);
                        if (p.TheLatex != null)
                            GoogleTranslate(p.TheLatex, locale.Key);
                    }
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
                DeletePlugg(p.ThePlugg);
                throw;
            }
        }

        public void DeletePlugg(Plugg p)
        {
            // Todo: Don't delete Plugg if: It has comments or ratings, Its included in a course.
            // Todo: Soft delete of Plugg
            if (p == null)
            {
                throw new Exception("Cannot delete: Plugg not initialized");
                return;
            }

            TabController tabController = new TabController();
            TabInfo getTab = tabController.GetTab(p.TabId);

            if (getTab != null)
            {
                DNNHelper h = new DNNHelper();
                h.DeleteTab(getTab);
            }

            rep.DeleteAllPhTextForItem(p.PluggId, (int)ETextItemType.PluggTitle);
            rep.DeleteAllPhTextForItem(p.PluggId, (int)ETextItemType.PluggHtml);
            rep.DeleteAllLatexForItem(p.PluggId, (int)ELatexType.Plugg);

            rep.DeletePlugg(p);
        }

        public void UpdatePlugg(Plugg p, PluggContent pc)
        {
            //For restore if something goes wrong
            Plugg oldP = GetPlugg(p.PluggId);
            IEnumerable<PluggContent> oldPCs = GetAllContentInPlugg(p.PluggId);

            rep.UpdatePlugg(p); //No repair necessary if this fails

            //For now, remove all PluggContent and recreate in all languages from pc. Fix this when we can deal with translations
            try
            {
                foreach (PluggContent pcDelete in oldPCs)
                {
                    rep.DeletePluggContent(pcDelete);
                }

                pc.PluggId = p.PluggId;
                if (pc.LatexText != null)
                {
                    LatexToMathMLConverter myConverter = new LatexToMathMLConverter(pc.LatexText);
                    myConverter.Convert();
                    pc.LatexTextInHtml = myConverter.HTMLOutput;
                }

                LocaleController lc = new LocaleController();
                var locales = lc.GetLocales(PortalID);
                foreach (var locale in locales)
                {
                    pc.CultureCode = locale.Key;
                    rep.CreatePluggContent(pc);
                }
            }
            catch (Exception)
            {
                //recreate old Plugg/PluggContent before rethrow
                var pcs = GetAllContentInPlugg(p.PluggId);
                foreach (PluggContent pcDelete in pcs)
                {
                    rep.DeletePluggContent(pcDelete);
                }
                rep.DeletePlugg(p);

                rep.CreatePlugg(oldP);
                foreach (PluggContent oldPC in oldPCs)
                    rep.CreatePluggContent(oldPC);
                throw;
            }

        }

        public void DeleteAllPluggs()
        {
            //Todo: Business logic for DeleteAllPluggs
            var pluggs = rep.GetAllPluggs();
            foreach (Plugg p in pluggs)
                DeletePlugg(p);
        }

        //public IEnumerable<Plugg> GetPluggsInCourse(int courseId)
        //{
        //    return rep.GetPluggsInCourse(courseId);
        //}

        #endregion

        #region PluggContent

        public void CreatePluggContent(PluggContent plugcontent)
        {
            rep.CreatePluggContent(plugcontent);
        }

        public PluggContent GetPluggContent(int pluggId, string cultureCode)
        {
            return rep.GetPluggContent(pluggId, cultureCode);
        }

        public IEnumerable<PluggContent> GetAllContentInPlugg(int pluggId)
        {
            return rep.GetAllContentInPlugg(pluggId);
        }

        public void UpdatePluggContent(PluggContent pc)
        {
            rep.UpdatePluggContent(pc);
        }

        public void DeleteAllPluggContent()
        {
            rep.DeleteAllPluggContent();
        }

        #endregion

        #region Course

        public void CreateCourse(Course c, List<CourseItemEntity> cis)
        {
            rep.CreateCourse(c);

            try
            {
                foreach (CourseItemEntity ci in cis)
                {
                    ci.CourseId = c.CourseId;
                    rep.CreateCourseItem(ci);
                }
            }
            catch (Exception)
            {
                DeleteCourse(c);
                throw;
            }

            //Create CoursePage
            DNNHelper d = new DNNHelper();
            string pageUrl = "C" + c.CourseId.ToString();
            string pageName = pageUrl + ": " + c.Title;
            try
            {
                TabInfo newTab = d.AddCoursePage(pageName, pageUrl);
                c.TabId = newTab.TabID;
                rep.UpdateCourse(c);
            }
            catch (Exception)
            {
                DeleteCourse(c);
                throw;
            }
        }

        public Course GetCourse(int CourseID)
        {
            return rep.GetCourse(CourseID);
        }

        public void DeleteCourse(Course c)
        {
            // Todo: Don't delete course if: It has comments or ratings
            // Todo: Soft delete of Course
            if (c == null)
            {
                throw new Exception("Cannot delete: Course not initialized");
                return;
            }

            TabController tabController = new TabController();
            TabInfo getTab = tabController.GetTab(c.TabId);

            if (getTab != null)
            {
                DNNHelper h = new DNNHelper();
                h.DeleteTab(getTab);
            }

            var cis = rep.GetItemsInCourse(c.CourseId);
            foreach (CourseItem ciDelete in cis)
            {
                rep.DeleteCourseItem(ciDelete);
            }

            rep.DeleteCourse(c);
        }

        #endregion

        #region CourseItem

        public IEnumerable<CourseItem> GetCourseItems(int CourseID, int ItemID)
        {
            return rep.GetCourseItems(CourseID, ItemID);
        }

        public List<CourseItem> GetItemsInCourse(int courseId)
        {
            return rep.GetItemsInCourse(courseId);
        }

        public IList<CourseItem> FlatToHierarchy(IEnumerable<CourseItem> list, int motherId = 0)
        {
            return (from i in list
                    where i.MotherId == motherId
                    select new CourseItem
                    {
                        CourseItemId = i.CourseItemId,
                        CourseId = i.CourseId,
                        ItemId = i.ItemId,
                        CIOrder = i.CIOrder,
                        ItemType = i.ItemType,
                        MotherId = i.MotherId,
                        //Mother = i,
                        label = i.label,
                        name = i.name,
                        children = FlatToHierarchy(list, i.CourseItemId)
                    }).ToList();
        }

        public IList<CourseItem> GetCourseItemsAsTree(int courseId)
        {
            List<CourseItem> source = GetItemsInCourse(courseId);
            return FlatToHierarchy(source);
        }

        //It is assumed that all CourseItems are in the same course
        public void SaveCourseItems(IList<CourseItem> cis, int courseId, int motherId = 0)
        {
            CourseItemEntity cie = new CourseItemEntity();
            int ciOrder = 1;
            foreach (CourseItem ci in cis)
            {
                DeleteCourseItem(ci); //Deletes heading as well if item is a heading

                if (ci.ItemType == ECourseItemType.Heading)
                {
                    CourseMenuHeadings ch = new CourseMenuHeadings();
                    ch.Title = ci.name;
                    rep.CreateHeading(ch);
                    cie.ItemId = ch.HeadingID;
                }
                else
                    cie.ItemId = ci.ItemId;

                cie.CourseId = courseId;
                cie.CIOrder = ciOrder;
                cie.ItemType = ci.ItemType;
                cie.MotherId = motherId;
                rep.CreateCourseItem(cie);
                ciOrder += 1;
                if (ci.children != null)
                    SaveCourseItems(ci.children, courseId, cie.CourseItemId);
            }
        }

        public void CreateCourseItem(CourseItem ci)
        {
            rep.CreateCourseItem(ci);
        }

        public void UpdateCourseItem(CourseItem ci)
        {
            rep.UpdateCourseItem(ci);
        }

        public void DeleteCourseItem(CourseItem ci)
        {
            if (ci.ItemType == ECourseItemType.Heading)
                rep.DeleteHeading(new CourseMenuHeadings() { HeadingID = ci.ItemId });
            rep.DeleteCourseItem(ci);
        }

        #endregion

        #region PHText

        public void SavePhText(PHText t)
        {
            if (t.Text == null || t.ItemId == 0 || t.ItemType == ETextItemType.NotSet || t.CultureCode == null || t.CcStatus == ECCStatus.NotSet || t.CreatedByUserId == 0)
                throw new Exception("Cannot save text - need Text, ItemId, ItemType, CultureCode, CreatedByUserId and CcStatus");

            if (t.ModifiedByUserId == 0)
                t.ModifiedByUserId = t.CreatedByUserId;
            t.ModifiedOnDate = DateTime.Now;
            t.CurrentVersion = true;
            bool isVersioned = t.ItemType == ETextItemType.PluggHtml;

            if (isVersioned)
            {
                var prevText = rep.GetPhText(t.CultureCode, t.ItemId, (int)t.ItemType);
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

        public void SaveLatexText(PHLatex t)
        {
            if (t.Text == null || t.ItemId == 0 || t.ItemType == ELatexType.NotSet || t.CultureCode == null || t.CcStatus == ECCStatus.NotSet || t.CreatedByUserId == 0)
                throw new Exception("Cannot save Latex - need Text, ItemId, ItemType, CultureCode, CreatedByUserId and CcStatus");

            if (t.ModifiedByUserId == 0)
                t.ModifiedByUserId = t.CreatedByUserId;
            t.ModifiedOnDate = DateTime.Now;
            t.CurrentVersion = true;
            bool isVersioned = (t.ItemType == ELatexType.Plugg || t.ItemType == ELatexType.Course);

            if (isVersioned)
            {
                var prevText = rep.GetLatexText(t.CultureCode, t.ItemId, (int)t.ItemType);
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

        //translates t into culture code cc
        public void GoogleTranslate(PHText t, string cc)
        {
            if (t.CultureCode == cc)
                throw new Exception("Cannot translate text to the same language");

            PHText currentText = rep.GetPhText(cc, t.ItemId, (int)t.ItemType);

            if (currentText == null)
            {
                t.Text = TranslateText(t.CultureCode, cc, t.Text)??t.Text;
                //Todo: Translate. For now: same
                t.CultureCode = cc;
                //t.Text = (Translation of t.Text from t.CultureCode into cc)
                rep.CreatePhText(t);
            }
            else
            {
                currentText.Text = t.Text;
                rep.UpdatePhText(currentText);
            }
        }

        //translates t into culture code cc
        public void GoogleTranslate(PHLatex t, string cc)
        {
            if (t.CultureCode == cc)
                throw new Exception("Cannot translate text to the same language");

            PHLatex currentText = rep.GetLatexText(cc, t.ItemId, (int)t.ItemType);

            if (currentText == null)
            {
                //Todo: Translate. For now: same
                t.CultureCode = cc;
                //t.Text = (Translation of t.Text from t.CultureCode into cc)
                rep.CreateLatexText(t);
            }
            else
            {
                currentText.Text = t.Text;
                rep.UpdateLatexText(currentText);
            }
        }

        #endregion

        #region Other

        public IEnumerable<PluggInfoForDNNGrid> GetPluggListForGrid(string cultureCode)
        {
            return rep.GetPluggRecords(cultureCode);
        }

        public List<CourseInfoForDNNGrid> GetCoursesForDNN()
        {
            return rep.GetCoursesForDNN();
        }

        //public List<CourseItem> GetCourseItemsForTree(int CourseID)
        //{
        //    return rep.GetCourseItemsForTree(CourseID);
        //}

        #endregion

        #region CourseMenuHeading

        public CourseMenuHeadings CreateHeading(CourseMenuHeadings h)
        {
            return rep.CreateHeading(h);
        }

        public void UpdateHeading(CourseMenuHeadings h)
        {
            rep.UpdateHeading(h);
        }

        #endregion

        #region CourseItemComment
        public IEnumerable<CourseItemComment> GetCourseItemComment(int CourseID, int ItemID)
        {
            return rep.GetCourseItemComment(CourseID, ItemID);
        }
        public void UpdateCourseItemComment(CourseItemComment CIC)
        {
            rep.UpdateCourseItemComment(CIC);
        }


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
