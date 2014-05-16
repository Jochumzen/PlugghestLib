using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Caching;
using Plugghest.Helpers;


namespace Plugghest.Base
{
    public enum EWhoCanEdit
    {
        NotSet = 0,
        Anyone,
        OnlyMe
    }

    public enum ECourseItemType
    {
        NotSet = 0,
        Plugg,
        Heading
    }

    public enum ETextItemType
    {
        NotSet = 0,
        PluggTitle,
        PluggHtml
    }

    public enum ELatexType
    {
        NotSet = 0,
        Plugg,
        Course
    }

    public enum ECCStatus
    {
        NotSet = 0,
        InCreationLanguage,
        GoogleTranslated,
        HumanTranslated,
        NotTranslated
    }

    // https://github.com/Jochumzen/Plugghest/wiki/Plugg
    [TableName("Pluggs")]
    //setup the primary key for table
    [PrimaryKey("PluggId", AutoIncrement = true)]
    public class Plugg
    {
        public int PluggId { get; set; }
        public string Title { get; set; }
        public string CreatedInCultureCode { get; set; }
        public string YouTubeCode { get; set; }
        public EWhoCanEdit WhoCanEdit { get; set; }
        public int TabId { get; set; }
        public int? SubjectId { get; set; }
        public DateTime CreatedOnDate { get; set; }
        public int CreatedByUserId { get; set; }
        public DateTime ModifiedOnDate { get; set; }
        public int ModifiedByUserId { get; set; }
    }

    // https://github.com/Jochumzen/Plugghest/wiki/PluggContent
    [TableName("PluggContents")]
    [PrimaryKey("PluggContentId", AutoIncrement = true)]
    public class PluggContent
    {
        public int PluggContentId { get; set; }
        public int PluggId { get; set; }
        public string CultureCode { get; set; }
        public string Title { get; set; }
        public string HtmlText { get; set; }
        public string LatexText { get; set; }
        public string LatexTextInHtml { get; set; }
    }

    public class PluggContainer
    {
        public Plugg ThePlugg;
        public string CultureCode;
        public IEnumerable<PluggContent> TheContent;
        public Youtube TheVideo;
        public PHText TheTitle;
        public PHText TheHtmlText;
        public PHLatex TheLatex;

        public PluggContainer()
        {
            ThePlugg = new Plugg();
        }

        public void SetTitle(string htmlText)
        {
            TheTitle = new PHText(htmlText, ThePlugg.CreatedInCultureCode, ETextItemType.PluggTitle);
        }

        public void LoadAllText()
        {
            LoadTitle();
            LoadHtmlText();
            LoadLatexText();
        }

        public void LoadTitle()
        {
            if (ThePlugg == null || ThePlugg.PluggId == 0 || CultureCode == null)
                throw new Exception("Cannot load title. Need PluggId and CultureCode");
            BaseRepository rep = new BaseRepository();
            TheTitle = rep.GetPhText(CultureCode, ThePlugg.PluggId, (int)ETextItemType.PluggTitle);
        }

        public void LoadHtmlText()
        {
            if (ThePlugg == null || ThePlugg.PluggId == 0 || CultureCode == null)
                throw new Exception("Cannot laod HtmlText. Need PluggId and CultureCode");
            BaseRepository rep = new BaseRepository();
            TheHtmlText = rep.GetPhText(CultureCode, ThePlugg.PluggId, (int)ETextItemType.PluggHtml);
        }

        public void LoadLatexText()
        {
            if (ThePlugg == null || ThePlugg.PluggId == 0 || CultureCode == null)
                throw new Exception("Cannot load Latex. Need PluggId and CultureCode");
            BaseRepository rep = new BaseRepository();
            TheLatex = rep.GetLatexText(CultureCode, ThePlugg.PluggId, (int)ELatexType.Plugg);
        }

        public void SetHtmlText(string htmlText)
        {
            TheHtmlText = new PHText(htmlText, ThePlugg.CreatedInCultureCode, ETextItemType.PluggHtml);
        }

        public void SetLatexText(string htmlText)
        {
            TheLatex = new PHLatex(htmlText, ThePlugg.CreatedInCultureCode, ELatexType.Plugg);
        }
    }

    [TableName("Courses")]
    [PrimaryKey("CourseId", AutoIncrement = true)]
    public class Course
    {
        public int CourseId { get; set; }
        public string Title { get; set; }
        public string CreatedInCultureCode { get; set; }
        public EWhoCanEdit WhoCanEdit { get; set; }
        public int TabId { get; set; }
        public DateTime CreatedOnDate { get; set; }
        public int CreatedByUserId { get; set; }
        public DateTime ModifiedOnDate { get; set; }
        public int ModifiedByUserId { get; set; }
        public string Description { get; set; }
    }

    [TableName("CourseItems")]
    [PrimaryKey("CourseItemId", AutoIncrement = true)]
    [Cacheable("CourseItems", CacheItemPriority.Normal, 20)]
    public class CourseItemEntity
    {
        public int CourseItemId { get; set; }
        public int CourseId { get; set; }
        public int ItemId { get; set; }
        public int CIOrder { get; set; }
        public ECourseItemType ItemType { get; set; }
        public int MotherId { get; set; }
    }

    public class CourseItem : CourseItemEntity
    {
        public CourseItem Mother { get; set; }
        public IList<CourseItem> children { get; set; }
        public string label { get; set; }
        public string name { get; set; }
    }

    [TableName("PHTexts")]
    //setup the primary key for table
    [PrimaryKey("TextId", AutoIncrement = true)]
    public class PHText
    {
        public int TextId { get; set; }
        public string CultureCode { get; set; }
        public string Text { get; set; }
        public int ItemId { get; set; }
        public ETextItemType ItemType { get; set; }
        public ECCStatus CcStatus { get; set; }
        public int Version { get; set; }
        public bool CurrentVersion { get; set; }
        public DateTime CreatedOnDate { get; set; }
        public int CreatedByUserId { get; set; }
        public DateTime ModifiedOnDate { get; set; }
        public int ModifiedByUserId { get; set; }

        public PHText()
        { }

        public PHText(string htmlText, string cultureCode, ETextItemType itemType)
        {
            Text = htmlText;
            CultureCode = cultureCode;
            ItemType = itemType;
        }
    }

    [TableName("PHLatex")]
    //setup the primary key for table
    [PrimaryKey("LatexId", AutoIncrement = true)]
    public class PHLatex
    {
        public int LatexId { get; set; }
        public string CultureCode { get; set; }
        public string Text { get; set; }
        public string HtmlText { get; set; }
        public int ItemId { get; set; }
        public ELatexType ItemType { get; set; }
        public ECCStatus CcStatus { get; set; }
        public int Version { get; set; }
        public bool CurrentVersion { get; set; }
        public DateTime CreatedOnDate { get; set; }
        public int CreatedByUserId { get; set; }
        public DateTime ModifiedOnDate { get; set; }
        public int ModifiedByUserId { get; set; }

        public PHLatex()
        { }

        public PHLatex(string text, string cultureCode, ELatexType itemType)
        {
            Text = text;
            CultureCode = cultureCode;
            ItemType = itemType;
        }
    }

    #region TemporaryDNN

    public class PluggInfoForDNNGrid
    {
        public int PluggId { get; set; }
        public string Text { get; set; }
        public string UserName { get; set; }
    }

    public class CourseInfoForDNNGrid
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public string UserName { get; set; }
    }
    #endregion

    //class for Create Course tree...


    #region  CourseHeading
    [TableName("CourseMenuHeadings")]
    [PrimaryKey("HeadingID", AutoIncrement = true)]
    public class CourseMenuHeadings
    {
        public int HeadingID { get; set; }

        public string Title { get; set; }
    }
    #endregion

    #region CourseItemComments
    [TableName("CourseItemComment")]
    [PrimaryKey("CourseItemCommentID", AutoIncrement = true)]
    public class CourseItemComment
    {
        public int CourseItemCommentID { get; set; }
        public int CourseID { get; set; }
        public int ItemId { get; set; }
        public int ItemType { get; set; }
        public string HtmlText { get; set; }
    }

    #endregion
}
