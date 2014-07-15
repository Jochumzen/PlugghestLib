using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Entities.Modules;
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
            int ratingModuleId = 0;
            TabInfo newTab = d.AddPluggPage(pageName, pageUrl, ref ratingModuleId);

            p.ThePlugg.TabId = newTab.TabID;
            p.ThePlugg.RatingsModuleId = ratingModuleId;

            rep.UpdatePlugg(p.ThePlugg);
        }

        public void DeleteAllPluggs()
        {
            //Todo: Business logic for DeleteAllPluggs
            var pluggs = rep.GetAllPluggs();
            foreach (Plugg p in pluggs)
                DeletePlugg(p);
        }

        public void DeletePlugg(Plugg p)
        {
            // Todo: Don't delete Plugg if: It has comments or ratings or its included in a course.
            // Todo: Soft delete of Plugg
            if (p == null)
            {
                throw new Exception("Cannot delete: Plugg not initialized");
            }

            //Delete Plugg page
            if (p.TabId != 0)
            {
                DNNHelper h = new DNNHelper();
                h.DeleteTab(p.TabId);
            }

            //Delete Plugg title and Plugg description
            rep.DeleteAllPhTextForItem(p.PluggId, ETextItemType.PluggTitle);
            rep.DeleteAllPhTextForItem(p.PluggId, ETextItemType.PluggDescription);

            //Delete all Pluggcomponents
            PluggContainer pc = new PluggContainer("en-us", p.PluggId);
            pc.LoadComponents();
            foreach (PluggComponent c in pc.TheComponents)
            {
                switch (c.ComponentType)
                {
                    case EComponentType.YouTube:
                        rep.DeleteYouTube(GetYouTubeByComponentId(c.PluggComponentId));
                        break;
                    case EComponentType.RichRichText:
                        rep.DeleteAllPhTextForItem(c.PluggComponentId, ETextItemType.PluggComponentRichRichText);
                        break;
                    case EComponentType.RichText:
                        rep.DeleteAllPhTextForItem(c.PluggComponentId, ETextItemType.PluggComponentRichText);
                        break;
                    case EComponentType.Label:
                        rep.DeleteAllPhTextForItem(c.PluggComponentId, ETextItemType.PluggComponentLabel);
                        break;
                    case EComponentType.Latex:
                        rep.DeleteAllLatexForItem(c.PluggComponentId, ELatexItemType.PluggComponentLatex);
                        break;
                    default:
                        break;
                }
                rep.DeletePluggComponent(c);
            }

            //Delete Pluggentity
            rep.DeletePlugg(p);
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
            //for (int order = newComponent.ComponentOrder; order <= cmps.Count; order++)
            for (int order = cmps.Count; order >= newComponent.ComponentOrder; order--)
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
            int ratingModuleId = 0;
            TabInfo newTab = d.AddCoursePage(pageName, pageUrl, ref ratingModuleId);
            c.TheCourse.TabId = newTab.TabID;
            c.TheCourse.RatingsModuleId = ratingModuleId;
            rep.UpdateCourse(c.TheCourse);
        }

        #endregion

        #region CoursePluggs

        /// <summary>
        /// Gets the CoursePluggEntity (CoursePluggId, CourseId, PluggId, CPOrder, MotherId, CreatedOnDate and CreatedByUser) for a given CoursePluggId
        /// </summary>
        /// <param name="cpId"></param>
        /// <returns></returns>
        public CoursePluggEntity GetCPEntity(int cpId)
        {
            return rep.GetCoursePlugg(cpId);
        }

        /// <summary>
        /// Gets a flat list of all Pluggs in given Course. 
        /// Sets the CoursePluggEntity as well as the title of the CoursePlugg in the language cultureCode.
        /// As it is a flat list, it does NOT set Mother or Children. Use FlatToHierarchy or GetCoursePluggsAsTree to set these.
        /// </summary>
        /// <param name="courseId"></param>
        /// <param name="ccCode"></param>
        /// <returns></returns>
        public List<CoursePlugg> GetPluggsAsFlatList(int courseId, string ccCode)
        {
            return rep.GetPluggsInCourse(courseId, ccCode);
        }

        /// <summary>
        /// Converts a flat list of all CoursePluggs into a hierarchy.
        /// Will set Mother as well as Children
        /// </summary>
        /// <param name="list"></param>
        /// <param name="Id"></param>
        /// <param name="_isChildren"></param>
        /// <returns></returns>
        public List<CoursePlugg> FlatToHierarchy(IEnumerable<CoursePlugg> list, int motherId = 0, CoursePlugg mother = null)
        {
            return (from i in list
                    where i.MotherId == motherId
                    select new CoursePlugg
                    {
                        CoursePluggId = i.CoursePluggId,
                        CourseId = i.CourseId,
                        PluggId = i.PluggId,
                        CPOrder = i.CPOrder,
                        MotherId = i.MotherId,
                        label = i.label,
                        //Mother = mother,
                        children = FlatToHierarchy(list, i.CoursePluggId, i),
                        CreatedOnDate = i.CreatedOnDate,
                        CreatedByUserId = i.CreatedByUserId
                    }).ToList();
        }

        /// <summary>
        /// Returns the root CoursePlugg. The root CoursePlugg has CoursePluggId = 0. It has all first level CoursePluggs as children.
        /// Navigate up and down hierarchy using children[] and Mother.
        /// </summary>
        /// <param name="cultureCode"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public CoursePlugg RootCoursePlugg(int courseId, string cultureCode, IEnumerable<CoursePlugg> list = null)
        {
            if (list == null)
                list = GetPluggsAsFlatList(courseId, cultureCode);
            CoursePlugg root = new CoursePlugg();
            root.children = new List<CoursePlugg>();

            Dictionary<int, CoursePlugg> dict = new Dictionary<int, CoursePlugg>();

            foreach (CoursePlugg s in list)
            {
                dict.Add(s.CoursePluggId, s);
                s.children = new List<CoursePlugg>();
            }

            foreach (CoursePlugg e in list)
            {
                if (e.MotherId == 0)
                {
                    root.children.Add(e);
                    e.Mother = root;
                }
                else
                {
                    dict[e.MotherId].children.Add(e);
                    e.Mother = dict[e.MotherId];
                }
            }
            return root;
        }

        /// <summary>
        /// Get all GetCoursePluggs as a tree hierarchy with the title of the GetCoursePlugg in the language cultureCode
        /// </summary>
        /// <param name="courseId"></param>
        /// <param name="ccCode"></param>
        /// <param name="_lastCoursePlugg"></param>
        /// <returns></returns>
        public List<CoursePlugg> GetCoursePluggsAsTree(int courseId, string cultureCode)
        {
            return FlatToHierarchy(GetPluggsAsFlatList(courseId, cultureCode));
        }

        /// <summary>
        /// Find a specific CoursePlugg in the hierarchy. Navigate up and down hierarchy using children[] and Mother.
        /// </summary>
        /// <param name="cultureCode"></param>
        /// <param name="coursePluggId"></param>
        /// <param name="root"></param>
        /// <returns></returns>
        public CoursePlugg FindCoursePlugg(string cultureCode, int courseId, int coursePluggId, CoursePlugg root = null)
        {
            if (root == null)
                root = RootCoursePlugg(courseId, cultureCode);
            foreach (CoursePlugg cp in root.children)
            {
                if (cp.CoursePluggId == coursePluggId)
                    return cp;
                if (cp.children.Count > 0)
                {
                    CoursePlugg fcp = FindCoursePlugg(cultureCode, courseId, coursePluggId, cp);
                    if (fcp != null)
                        return fcp;
                }
            }
            return null;
        }

        /// <summary>
        /// Retrieve next CoursePlugg in hierarchy (Think of an expanded CoursePlugg tree. Will get the "next row")
        /// </summary>
        /// <param name="current"></param>
        /// <param name="_lastCoursePlugg"></param>
        /// <param name="previousChildOrder"></param>
        /// <returns></returns>
        public CoursePlugg NextCoursePlugg(CoursePlugg cp)
        {
            bool SearchChildren = true;
            while (1 == 1)
            {
                if (cp.CoursePluggId == 0)
                    return null;
                if (SearchChildren && cp.children.Count > 0)
                    return cp.children[0];
                if (cp.Mother.children.Count > cp.CPOrder)
                    return cp.Mother.children[cp.CPOrder];
                cp = cp.Mother;
                SearchChildren = false;
            }
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
            CoursePluggEntity toUpdate;
            foreach (CoursePlugg s in ss)
            {
                toUpdate = rep.GetCoursePlugg(s.CoursePluggId);
                toUpdate.MotherId = motherId;
                toUpdate.CPOrder = cpOrder;
                rep.UpdateCoursePlugg(toUpdate);
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
        public List<Subject> GetSubjectsAsFlatList(string cultureCode)
        {
            List<Subject> ss = rep.GetAllSubjects(cultureCode);
            return ss;
        }

        /// <summary>
        /// Converts a flat list of all Subjects into a hierarchy.
        /// Will set Mother as well as Children
        /// </summary>
        /// <param name="list">Get list from GetSubjectsAsFlatList</param>
        /// <param name="motherId">Do not use this parameter</param>
        /// <returns></returns>
        public List<Subject> FlatToHierarchy(IEnumerable<Subject> list, int motherId = 0, Subject mother = null)
        {
            return (from i in list
                    where i.MotherId == motherId
                    select new Subject
                    {
                        SubjectId = i.SubjectId,
                        SubjectOrder = i.SubjectOrder,
                        MotherId = i.MotherId,
                        label = i.label,
                        Mother = mother,
                        children = FlatToHierarchy(list, i.SubjectId, i)
                    }).ToList();
        }

        /// <summary>
        /// Returns the root subject. The root subject has SubjectId = 0. It has all first level subjects as children.
        /// Navigate up and down hierarchy using children[] and Mother.
        /// </summary>
        /// <param name="cultureCode"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public Subject RootSubject(string cultureCode, IEnumerable<Subject> list = null)
        {
            if (list == null)
                list = GetSubjectsAsFlatList(cultureCode);
            Subject root = new Subject();
            root.children = new List<Subject>();

            Dictionary<int, Subject> dict = new Dictionary<int, Subject>();

            foreach (Subject s in list)
            {
                dict.Add(s.SubjectId, s);
                s.children = new List<Subject>();
            }

            foreach (Subject e in list)
            {
                if(e.MotherId==0)
                {
                    root.children.Add(e);
                    e.Mother = root;
                }
                else
                {
                    dict[e.MotherId].children.Add(e);
                    e.Mother = dict[e.MotherId];
                }
            }
            return root;
        }

        /// <summary>
        /// Get all Subjects as a tree hierarchy with the title of the subject in the language cultureCode
        /// </summary>
        /// <param name="cultureCode"></param>
        /// <returns></returns>
        public List<Subject> GetSubjectsAsTree(string cultureCode)
        {
            return FlatToHierarchy(GetSubjectsAsFlatList(cultureCode));
        }

        /// <summary>
        /// Find a specific subject in the hierarchy. Navigate up and down hierarchy using children[] and Mother.
        /// </summary>
        /// <param name="cultureCode"></param>
        /// <param name="subjectId"></param>
        /// <param name="root"></param>
        /// <returns></returns>
        public Subject FindSubject(string cultureCode, int subjectId, Subject root = null)
        {
            if (root == null)
                root = RootSubject(cultureCode);
            foreach (Subject s in root.children)
            {
                if (s.SubjectId == subjectId)
                    return s;
                if (s.children.Count > 0)
                {
                    Subject fs = FindSubject(cultureCode, subjectId, s);
                    if (fs != null)
                        return fs;
                }
            }
            return null;
        }

        /// <summary>
        /// Retrieve next subject in hierarchy (Think of an expanded subject tree. Will get the "next row")
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public Subject NextSubject(Subject s)
        {
            bool SearchChildren = true;
            while(1==1)
            {
                if (s.SubjectId == 0)
                    return null; 
                if (SearchChildren && s.children.Count > 0)
                    return s.children[0];
                if (s.Mother.children.Count > s.SubjectOrder)
                    return s.Mother.children[s.SubjectOrder];
                s = s.Mother;
                SearchChildren = false;
            }
        }

        /// <summary>
        /// Returns the subject in the form "Natural Science -> Physics -> Quantum Mechanics"
        /// </summary>
        /// <param name="cultureCode"></param>
        /// <param name="subjectId"></param>
        /// <param name="root"></param>
        /// <returns></returns>
        public string GetSubjectString(string cultureCode, int subjectId, Subject root = null)
        {
            if (root == null)
                root = RootSubject(cultureCode);
            Subject s = FindSubject(cultureCode, subjectId, root);
            StringBuilder theS = new StringBuilder(s.label);
            if (s == null)
                return null;
            while (s.MotherId != 0)
            {
                s = s.Mother;
                theS.Insert(0, s.label + " <span class=\"glyphicons glyph-right-arrow\"></span> ");
            }
            return theS.ToString();
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

        /// <summary>
        /// Add a new Subject to DB
        /// </summary>
        /// <param name="s"></param>
        /// <param name="userId"></param>
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

        /// <summary>
        /// Deletes a subject and reassign order of remaining subjects. Removes text in PHText.
        /// </summary>
        /// <param name="subjectId"></param>
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

        public IEnumerable<PluggInfoForPluggList> Get_SearchResult(string _key, string cultureCode)
        {
            return rep.GetAllPhtext(_key, cultureCode);
        }

        public IEnumerable<PluggInfoForPluggList> GetALL_Pluggs(string cultureCode)
        {
            var all = rep.GetAllPluggs(cultureCode);
            return all;
        }


    }
}