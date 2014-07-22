using System.Runtime.InteropServices;
using System.Web.Configuration;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Definitions;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Security.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotNetNuke.Common.Utilities;

namespace Plugghest.DNN
{

    public class DNNHelper
    {
        public enum ModuleType
        {
            DisplayPlugg = 0,
            DisplayCourse,
            CourseMenu,
            Rating,
            Comments,
            CoursePluggTitle,
            CoursePluggComment
        }

        public void DeleteTab(int tabId)
        {
            PortalSettings portalSettings = new PortalSettings();
            int portalId = portalSettings.PortalId;

            TabController tc = new TabController();
            if (tc != null)
            {
                tc.DeleteTab(tabId, portalId);
                tc.DeleteTabSettings(tabId);
                DataCache.ClearModuleCache(tabId);
            }
        }

        public void RenameTab(int tabId, string tabName)
        {
            PortalSettings portalSettings = new PortalSettings();
            int portalId = portalSettings.PortalId;

            TabController tabController = new TabController();
            TabInfo tab = tabController.GetTab(tabId, portalId);
            tab.TabName = tabName;
            tabController.UpdateTab(tab);
        }

        public TabInfo AddPluggPage(string tabName, string tabTitle, ref int ratingModuleId)
        {
            PortalSettings portalSettings = new PortalSettings();
            int portalId = portalSettings.PortalId;

            TabController tabController = new TabController();
            TabInfo getTab = tabController.GetTabByName(tabName, portalId);
            if (getTab != null)
                throw new Exception("Cannot create Page. Page with this PageName already exists");

            TabInfo newTab = new TabInfo();
            newTab.PortalID = portalId;
            newTab.TabName = tabName;
            newTab.Title = tabTitle;
            newTab.SkinSrc = "[G]Skins/20047-UnlimitedColorPack-033/PluggPage.ascx";
            newTab.ContainerSrc = "[G]Containers/20047-UnlimitedColorPack-033/No Title.ascx";
            CommonTabSettings(newTab);
            AddViewPermissions(newTab);

            int tabId = tabController.AddTab(newTab, true);
            DotNetNuke.Common.Utilities.DataCache.ClearModuleCache(tabId);

            AddTabURL(newTab); //Makes the URL of Page /4 or /C4

            // add modules to new page

            AddModuleToPage(newTab, ModuleType.CoursePluggTitle, 1);

            AddModuleToPage(newTab, ModuleType.CoursePluggComment, 1);

            AddModuleToPage(newTab, ModuleType.DisplayPlugg, 2);

            AddModuleToPage(newTab, ModuleType.CourseMenu, 1);

            ratingModuleId = AddModuleToPage(newTab, ModuleType.Rating, 2);

            AddModuleToPage(newTab, ModuleType.Comments, 3);

            return newTab;
        }

        public TabInfo AddCoursePage(string tabName, string tabTitle, ref int ratingModuleId)
        {
            PortalSettings portalSettings = new PortalSettings();
            int portalId = portalSettings.PortalId;

            TabController tabController = new TabController();
            TabInfo getTab = tabController.GetTabByName(tabName, portalId);
            if (getTab != null)
                throw new Exception("Cannot create Page. Page with this PageName already exists");

            TabInfo newTab = new TabInfo();
            newTab.PortalID = portalId;
            newTab.TabName = tabName;
            newTab.Title = tabTitle;
            newTab.SkinSrc = "[G]Skins/20047-UnlimitedColorPack-033/CoursePage.ascx";
            newTab.ContainerSrc = portalSettings.DefaultPortalContainer;
            CommonTabSettings(newTab);
            AddViewPermissions(newTab);

            int tabId = tabController.AddTab(newTab, true);
            DotNetNuke.Common.Utilities.DataCache.ClearModuleCache(tabId);

            AddTabURL(newTab); //Makes the URL of Page /4 or /C4

            // add modules to new page

            AddModuleToPage(newTab, ModuleType.DisplayCourse, 1);

            ratingModuleId = AddModuleToPage(newTab, ModuleType.Rating, 1);

            AddModuleToPage(newTab, ModuleType.Comments, 2);

            AddModuleToPage(newTab, ModuleType.CoursePluggTitle, 1);

            return newTab;
        }

        protected void CommonTabSettings(TabInfo t)
        {
            t.Description = "";
            t.KeyWords = "";
            t.IsDeleted = false;
            t.IsSuperTab = false;
            t.IsVisible = false;//for menu...
            t.DisableLink = false;
            t.IconFile = "";
            t.Url = "";
        }

        protected void AddViewPermissions(TabInfo t)
        {
            foreach (PermissionInfo p in PermissionController.GetPermissionsByTab())
            {
                if (p.PermissionKey == "VIEW")
                {
                    TabPermissionInfo tpi = new TabPermissionInfo();
                    tpi.PermissionID = p.PermissionID;
                    tpi.PermissionKey = p.PermissionKey;
                    tpi.PermissionName = p.PermissionName;
                    tpi.AllowAccess = true;
                    tpi.RoleID = -1; //ID of all users
                    t.TabPermissions.Add(tpi);
                }
            }
        }

        protected void AddTabURL(TabInfo t)
        {
            //Temporary solution: Writing directly to table TabUrls
            //Dont know how to do this in DNN

            TabUrl tu = new TabUrl();
            TabUrlController tc = new TabUrlController();
            tu.TabId = t.TabID;
            tu.SeqNum = 0;
            tu.Url = "/" + t.Title;
            tu.QueryString = "";
            tu.HttpStatus = "200";
            tu.IsSystem = true;
            tu.PortalAliasUsage = 0;
            tu.CreatedByUserID = 1;
            tu.CreatedOnDate = DateTime.Now;
            tu.LastModifiedByUserID = 1;
            tu.LastModifiedOnDate = DateTime.Now;
            tc.CreateTabUrl(tu);
        }

        public int AddModuleToPage(TabInfo t, ModuleType type, int moduleOrder)
        {
            string DesktopModuleFriendlyName="";
            string ModuleDefFriendlyName="";

            ModuleDefinitionInfo moduleDefinitionInfo = new ModuleDefinitionInfo();
            ModuleInfo moduleInfo = new ModuleInfo();
            moduleInfo.PortalID = t.PortalID;
            moduleInfo.TabID = t.TabID;
            moduleInfo.ModuleOrder = moduleOrder;
            moduleInfo.ModuleTitle = "";
            moduleInfo.DisplayPrint = false;
            moduleInfo.IsShareable = true;
            moduleInfo.IsShareableViewOnly = true;

            switch (type)
            {
                case ModuleType.DisplayPlugg:
                    moduleInfo.PaneName = "RowTwo_Grid8_Pane";
                    DesktopModuleFriendlyName = "DisplayPlugg";
                    ModuleDefFriendlyName = "DisplayPlugg";
                    break;
                case ModuleType.DisplayCourse:
                    moduleInfo.PaneName = "RowTwo_Grid8_Pane";
                    DesktopModuleFriendlyName = "DisplayCourse";
                    ModuleDefFriendlyName = "DisplayCourse";
                    break;
                case ModuleType.CourseMenu:
                    moduleInfo.PaneName = "RowTwo_Grid4_Pane";
                    DesktopModuleFriendlyName = "CourseMenu";
                    ModuleDefFriendlyName = "CourseMenu";
                    break;
                case ModuleType.Rating:
                    moduleInfo.PaneName = "RowTwo_Grid4_Pane";
                    DesktopModuleFriendlyName = "DNNCentric RnC";
                    ModuleDefFriendlyName = "DNNCentric.RatingAndComments";
                    break;
                case ModuleType.Comments:
                    moduleInfo.PaneName = "RowTwo_Grid8_Pane";
                    DesktopModuleFriendlyName = "DNNCentric RnC";
                    ModuleDefFriendlyName = "DNNCentric.RatingAndComments";
                    break;
                case ModuleType.CoursePluggTitle:
                    moduleInfo.PaneName = "breadcrumb_Pane";
                    DesktopModuleFriendlyName = "CPTitle";
                    ModuleDefFriendlyName = "CPTitle";
                    break;
                case ModuleType.CoursePluggComment:
                    moduleInfo.PaneName = "RowTwo_Grid8_Pane";
                    DesktopModuleFriendlyName = "CoursePluggComment";
                    ModuleDefFriendlyName = "CoursePluggComment";
                    break;   
           }

            DesktopModuleInfo myModule = null;
            foreach (KeyValuePair<int, DesktopModuleInfo> kvp in DesktopModuleController.GetDesktopModules(t.PortalID))
            {
                DesktopModuleInfo mod = kvp.Value;
                if (mod != null)
                    if (mod.FriendlyName == DesktopModuleFriendlyName)
                    {
                        myModule = mod;
                        break;
                    }
            }

            int moduleId=0;
            if (myModule != null)
            {
                var mc = new ModuleDefinitionController();
                var mInfo = new ModuleDefinitionInfo();
                mInfo = ModuleDefinitionController.GetModuleDefinitionByFriendlyName(ModuleDefFriendlyName,
                    myModule.DesktopModuleID);
                moduleInfo.ModuleDefID = mInfo.ModuleDefID;
                moduleInfo.CacheTime = moduleDefinitionInfo.DefaultCacheTime;//Default Cache Time is 0
                moduleInfo.InheritViewPermissions = true;  //Inherit View Permissions from Tab
                moduleInfo.AllTabs = false;
                //moduleInfo.Alignment = "Top"; Will screw upp Settings

                ModuleController moduleController = new ModuleController();
                moduleId = moduleController.AddModule(moduleInfo);
            }

            DotNetNuke.Common.Utilities.DataCache.ClearModuleCache(t.TabID);
            DotNetNuke.Common.Utilities.DataCache.ClearTabsCache(t.PortalID);
            DotNetNuke.Common.Utilities.DataCache.ClearPortalCache(t.PortalID, false);

            //Add settings for RnC
            ModuleController m = new ModuleController();
            if (type == ModuleType.Rating)
            {
                AddModuleSettingsRnCCommon(moduleId);
                m.UpdateModuleSetting(moduleId, "PRC_settingCommentObject", "tabid:" + t.TabID);
                m.UpdateModuleSetting(moduleId, "PRC_settingShow", "OnlyRatings");
                m.UpdateModuleSetting(moduleId, "PRC_settingRncWidth", "357");
            }
            if (type == ModuleType.Comments)
            {
                AddModuleSettingsRnCCommon(moduleId);
                m.UpdateModuleSetting(moduleId, "PRC_settingCommentObject", "tabid:" + t.TabID);
                m.UpdateModuleSetting(moduleId, "PRC_settingShow", "OnlyComments");
                m.UpdateModuleSetting(moduleId, "PRC_settingRncWidth", "744");
            }

            if (type == ModuleType.Rating || type == ModuleType.Comments)
            {
                RnCController c = new RnCController();
                DCC_PRC_CommentObject myCO = new DCC_PRC_CommentObject();
                myCO.CommentObject = "tabid:" + t.TabID;
                myCO.CommentObjectViewCount = 0;
                myCO.PortalID = t.PortalID;
                c.CreateCommentObject(myCO);
            }
            return moduleId;
        }

        protected void AddModuleSettingsRnCCommon(int moduleId)
        {
            ModuleController m = new ModuleController();
            m.UpdateModuleSetting(moduleId, "PRC_settingCommentLength", "1000");
            m.UpdateModuleSetting(moduleId, "PRC_settingCommentsPerPage", "10");
            m.UpdateModuleSetting(moduleId, "PRC_settingCaptchaForAnonyComments", "True");
            m.UpdateModuleSetting(moduleId, "PRC_settingDisplayedName", "DN");
            m.UpdateModuleSetting(moduleId, "PRC_settingPersistComObjInSession", "false");
            m.UpdateModuleSetting(moduleId, "PRC_settingEnabledNotifications", "1,2,3,4,5,6,7");
            m.UpdateModuleSetting(moduleId, "PRC_settingNewCommentNotificationForReplies", "true");
            m.UpdateModuleSetting(moduleId, "PRC_settingCommentModRoleID", "1");
            m.UpdateModuleSetting(moduleId, "PRC_settingOwnerRoleID", "1");
            m.UpdateModuleSetting(moduleId, "PRC_settingCommentModeration", "ModNone");
            m.UpdateModuleSetting(moduleId, "PRC_settingModUserID", "1");
            m.UpdateModuleSetting(moduleId, "PRC_settingDisplayTemplate", "Classic");
            m.UpdateModuleSetting(moduleId, "PRC_settingOwnerUserID", "1");
            m.UpdateModuleSetting(moduleId, "PRC_settingAnonyComments", "False");
            m.UpdateModuleSetting(moduleId, "PRC_settingAnonyRatings", "False");
            m.UpdateModuleSetting(moduleId, "PRC_settingShowHideCommentPoint", "False");
            m.UpdateModuleSetting(moduleId, "PRC_settingShowHideNameEmailWebsite", "True");
            m.UpdateModuleSetting(moduleId, "PRC_settingShowHideRatingPoints", "False");
            m.UpdateModuleSetting(moduleId, "PRC_settingShowHideViews", "False");
            m.UpdateModuleSetting(moduleId, "PRC_settingTheme", "Facebook_theme");
            m.UpdateModuleSetting(moduleId, "PRC_settingRatingChangeAllowed", "True");
            m.UpdateModuleSetting(moduleId, "PRC_settingHTMLAllowed", "False");
            m.UpdateModuleSetting(moduleId, "PRC_settingHideCommentOnReport", "True");
            m.UpdateModuleSetting(moduleId, "PRC_settingRoleAllowedPostingComment", "all");
            m.UpdateModuleSetting(moduleId, "PRC_settingRoleAllowedToRate", "all");
            m.UpdateModuleSetting(moduleId, "PRC_settingProfileLink", "/Activity-Feed/userId/[UserID]");
            m.UpdateModuleSetting(moduleId, "PRC_settingProfileLinkAnonymous", "[WebSite]");
            m.UpdateModuleSetting(moduleId, "PRC_settingLinkTarget", "_blank");
            m.UpdateModuleSetting(moduleId, "PRC_settingProfileLinkNoFollow", "False");
            m.UpdateModuleSetting(moduleId, "PRC_settingShowProfileImage", "True");
            m.UpdateModuleSetting(moduleId, "PRC_settingImageSourceType", "DNNProfileImage");
            m.UpdateModuleSetting(moduleId, "PRC_settingDNNProfileImageWidth", "80");
            m.UpdateModuleSetting(moduleId, "PRC_settingImageWidth", "80");
            m.UpdateModuleSetting(moduleId, "PRC_settingImageMaxRated", "pg");
            m.UpdateModuleSetting(moduleId, "PRC_settingCustomImageUrl", "");
            //Ugly way of building http://mywebsite/DesktopModules/DNNCentric-RatingAndComments/images/noProfile.jpg
            string s = DotNetNuke.Common.Globals.NavigateURL();
            s = s.Replace("http://", "");
            s = "http://" + s.Substring(0, s.IndexOf('/')) + DotNetNuke.Common.Globals.DesktopModulePath + "DNNCentric-RatingAndComments/images/noProfile.jpg";
            m.UpdateModuleSetting(moduleId, "PRC_settingDefaultImage", s);
            m.UpdateModuleSetting(moduleId, "PRC_settingRncAlignment", "left");
            m.UpdateModuleSetting(moduleId, "PRC_settingPermaLinkEnabled", "False");
            m.UpdateModuleSetting(moduleId, "PRC_settingSortingOrderValue", "5");
            m.UpdateModuleSetting(moduleId, "PRC_settingPostCommentsAnonymously", "False");
        }

        //protected void AddModuleSettingsRnCCommon(int moduleId)
        //{
        //    ModuleController m = new ModuleController();
        //    m.UpdateModuleSetting(moduleId, "PRC_settingCommentLength", "1000");
        //    m.UpdateModuleSetting(moduleId, "PRC_settingCommentsPerPage", "10");
        //    m.UpdateModuleSetting(moduleId, "PRC_settingCaptchaForAnonyComments", "True");
        //    m.UpdateModuleSetting(moduleId, "PRC_settingDisplayedName", "U");
        //    m.UpdateModuleSetting(moduleId, "PRC_settingPersistComObjInSession", "false");
        //    m.UpdateModuleSetting(moduleId, "PRC_settingEnabledNotifications", "1,2,3,4,5,6,7");
        //    m.UpdateModuleSetting(moduleId, "PRC_settingNewCommentNotificationForReplies", "true");
        //    m.UpdateModuleSetting(moduleId, "PRC_settingCommentModRoleID", "1");
        //    m.UpdateModuleSetting(moduleId, "PRC_settingOwnerRoleID", "1");
        //    m.UpdateModuleSetting(moduleId, "PRC_settingCommentModeration", "ModNone");
        //    m.UpdateModuleSetting(moduleId, "PRC_settingModUserID", "1");
        //    m.UpdateModuleSetting(moduleId, "PRC_settingDisplayTemplate", "Classic");
        //    m.UpdateModuleSetting(moduleId, "PRC_settingOwnerUserID", "1");
        //    m.UpdateModuleSetting(moduleId, "PRC_settingAnonyComments", "True");
        //    m.UpdateModuleSetting(moduleId, "PRC_settingAnonyRatings", "True");
        //    m.UpdateModuleSetting(moduleId, "PRC_settingShowHideCommentPoint", "False");
        //    m.UpdateModuleSetting(moduleId, "PRC_settingShowHideNameEmailWebsite", "True");
        //    m.UpdateModuleSetting(moduleId, "PRC_settingShowHideRatingPoints", "False");
        //    m.UpdateModuleSetting(moduleId, "PRC_settingShowHideViews", "False");
        //    m.UpdateModuleSetting(moduleId, "PRC_settingTheme", "Smart");
        //    m.UpdateModuleSetting(moduleId, "PRC_settingRatingChangeAllowed", "True");
        //    m.UpdateModuleSetting(moduleId, "PRC_settingHTMLAllowed", "False");
        //    m.UpdateModuleSetting(moduleId, "PRC_settingHideCommentOnReport", "True");
        //    m.UpdateModuleSetting(moduleId, "PRC_settingRoleAllowedPostingComment", "all");
        //    m.UpdateModuleSetting(moduleId, "PRC_settingRoleAllowedToRate", "all");
        //    m.UpdateModuleSetting(moduleId, "PRC_settingProfileLink", "[WebSite]");
        //    m.UpdateModuleSetting(moduleId, "PRC_settingProfileLinkAnonymous", "[WebSite]");
        //    m.UpdateModuleSetting(moduleId, "PRC_settingLinkTarget", "_blank");
        //    m.UpdateModuleSetting(moduleId, "PRC_settingProfileLinkNoFollow", "False");
        //    m.UpdateModuleSetting(moduleId, "PRC_settingShowProfileImage", "True");
        //    m.UpdateModuleSetting(moduleId, "PRC_settingImageSourceType", "gravatar");
        //    m.UpdateModuleSetting(moduleId, "PRC_settingDNNProfileImageWidth", "80");
        //    m.UpdateModuleSetting(moduleId, "PRC_settingImageWidth", "80");
        //    m.UpdateModuleSetting(moduleId, "PRC_settingImageMaxRated", "pg");
        //    m.UpdateModuleSetting(moduleId, "PRC_settingCustomImageUrl", "");
        //    //Ugly way of building http://mywebsite/DesktopModules/DNNCentric-RatingAndComments/images/noProfile.jpg
        //    string s = DotNetNuke.Common.Globals.NavigateURL();
        //    s = s.Replace("http://", "");
        //    s = "http://" + s.Substring(0, s.IndexOf('/')) + DotNetNuke.Common.Globals.DesktopModulePath + "DNNCentric-RatingAndComments/images/noProfile.jpg";
        //    m.UpdateModuleSetting(moduleId, "PRC_settingDefaultImage", s);
        //    m.UpdateModuleSetting(moduleId, "PRC_settingRncWidth", "500");
        //    m.UpdateModuleSetting(moduleId, "PRC_settingRncAlignment", "left");
        //    m.UpdateModuleSetting(moduleId, "PRC_settingPermaLinkEnabled", "False");
        //    m.UpdateModuleSetting(moduleId, "PRC_settingSortingOrderValue", "2");
        //    m.UpdateModuleSetting(moduleId, "PRC_settingPostCommentsAnonymously", "False");
        //}
    }
}
