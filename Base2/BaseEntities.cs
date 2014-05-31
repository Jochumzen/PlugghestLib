using System.Runtime.InteropServices;
using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Caching;
using Plugghest.Helpers;

//Put entity classes - classes mirroring tables here

namespace Plugghest.Base2
{
    /// <summary>
    /// The entity class for a Plugg
    /// To work with a complete Plugg with its title and components, use PluggContainer
    /// </summary>
    [TableName("Pluggs")]
    [PrimaryKey("PluggId", AutoIncrement = true)]
    [Cacheable("Pluggs", CacheItemPriority.Normal, 20)]
    public class Plugg
    {
        ///<summary>
        /// The ID of the Plugg. Key and AutoInc
        ///</summary>
        public int PluggId { get; set; }

        ///<summary>
        /// The Language in which the Plugg was created, ex "en-US". 
        /// see http://msdn.microsoft.com/en-us/library/ee825488(v=cs.20).aspx
        ///</summary>
        public string CreatedInCultureCode { get; set; }

        ///<summary>
        /// Who is allowed to edit the Plugg. Applies to everything in the Plugg as well
        ///</summary>
        public EWhoCanEdit WhoCanEdit { get; set; }

        ///<summary>
        /// The ID of tab/page where this Plugg is located
        ///</summary>
        public int TabId { get; set; }

        ///<summary>
        /// What subject this Plugg deals with. Primary key: Subjects->SubjectId
        ///</summary>
        public int? SubjectId { get; set; }

        ///<summary>
        /// True if "soft-deleted". Not actually deleted in DB but presented as deleted.
        ///</summary>
        public bool IsDeleted { get; set; }

        ///<summary>
        /// True if Plugg is to be listed and searchable. 
        /// If false, you will still see the plugg if you go to PluggPage directly.
        ///</summary>
        public bool IsListed { get; set; }
        
        ///<summary>
        /// 
        ///</summary>
        public DateTime CreatedOnDate { get; set; }

        ///<summary>
        /// The DNN UserId from dbo.Users
        ///</summary>
        public int CreatedByUserId { get; set; }

        ///<summary>
        /// 
        ///</summary>
        public DateTime ModifiedOnDate { get; set; }

        ///<summary>
        /// The DNN UserId from dbo.Users
        ///</summary>
        public int ModifiedByUserId { get; set; }
    }

    /// <summary>
    /// The entity class for a Plugg Component
    /// A PluggComponent is a part of a Plugg, for example a video or some rich text
    /// To work with a complete Plugg with its title and all its components, use PluggContainer
    /// </summary>
    [TableName("PluggComponents")]
    [PrimaryKey("PluggComponentId", AutoIncrement = true)]
    [Cacheable("PluggComponents", CacheItemPriority.Normal, 20)]
    public class PluggComponent
    {
        ///<summary>
        /// The ID of the PluggComponent. Key and AutoInc. 
        ///</summary>
        public int PluggComponentId { get; set; }

        ///<summary>
        /// The Id of the Plugg which this component is located in
        ///</summary>
        public int PluggId { get; set; }

        ///<summary>
        /// The type of component
        ///</summary>
        public EComponentType ComponentType { get; set; }

        ///<summary>
        /// The order of the Component inside the Plugg
        ///</summary>
        public int ComponentOrder { get; set; }
    }

    /// <summary>
    /// PHTexts contains all text in Plugghest - everything from PluggTitles to Rich Html text
    /// Allows for translation and versioning
    /// </summary>
    [TableName("PHTexts")]
    [PrimaryKey("TextId", AutoIncrement = true)]
    [Cacheable("PHTexts", CacheItemPriority.Normal, 20)]
    public class PHText
    {
        ///<summary>
        /// The ID of the PHText. Key and AutoInc
        ///</summary>
        public int TextId { get; set; }

        /// <summary>
        /// The Language of the text, ex "en-US". 
        /// see http://msdn.microsoft.com/en-us/library/ee825488(v=cs.20).aspx
        /// </summary>
        public string CultureCode { get; set; }

        /// <summary>
        /// The actual text. May be html text
        /// If text is html, it must be decoded (like <p>Hello</p>)
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// The Item Type of the text. Tells us what type of text this is, for example a Plugg Title
        /// </summary>
        public ETextItemType ItemType { get; set; }

        /// <summary>
        /// Foreign key. Primary key depend on ItemType.
        /// For PluggTitle, PluggDescription: PluggId
        /// For PluggComponentRichRichText, PluggComponentRichText, PluggComponentLabel: PluggComponentId
        /// For CourseTitle, CourseDescription, CourseRichRichText: CourseId
        /// For CoursePluggText: CoursePluggId
        /// For CourseMenuHeadingText: CourseItemId
        /// </summary>
        public int ItemId { get; set; }

        /// <summary>
        /// The culture code status
        /// </summary>
        public ECultureCodeStatus CultureCodeStatus { get; set; }

        /// <summary>
        /// The version number of this text. For unversioned text, Version is always 0.
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// Whether this is the current version or not (always true for unversioned text)
        /// </summary>
        public bool CurrentVersion { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime CreatedOnDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int CreatedByUserId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime ModifiedOnDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int ModifiedByUserId { get; set; }

        /// <summary>
        /// Empty constructor
        /// </summary>
        public PHText()
        { }

        /// <summary>
        /// Constructs a new PHText
        /// </summary>
        /// <param name="htmlText"></param>
        /// <param name="cultureCode"></param>
        /// <param name="itemType"></param>
        public PHText(string htmlText, string cultureCode, ETextItemType itemType)
        {
            Text = htmlText;
            CultureCode = cultureCode;
            ItemType = itemType;
        }
    }

    /// <summary>
    /// PHLatex contains all Latex text in Plugghest
    /// Allows for translation, versioning and conversion to Html.
    /// </summary>
    [TableName("PHLatex")]
    [PrimaryKey("LatexId", AutoIncrement = true)]
    [Cacheable("PHLatex", CacheItemPriority.Normal, 20)]
    public class PHLatex
    {
        ///<summary>
        /// The ID of the PHLatex. Key and AutoInc
        ///</summary>
        public int LatexId { get; set; }

        /// <summary>
        /// The Language of the text, ex "en-US". see http://msdn.microsoft.com/en-us/library/ee825488(v=cs.20).aspx
        /// </summary>
        public string CultureCode { get; set; }

        /// <summary>
        /// The actual Latex text.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// The Latex text translated into Html
        /// html text must be decoded (like <p>Hello</p>)
        /// </summary>
        public string HtmlText { get; set; }

        /// <summary>
        /// The Item Type of the Latex text. 
        /// Tells us what type of Latex text this is, for example latex text from the component PluggComponentLatex
        /// </summary>
        public ELatexItemType ItemType { get; set; }

        /// <summary>
        /// Foreign key. Primary key depend on ItemType.
        /// For PluggComponentLatex: PluggComponentId
        /// For CourseLatexText: CourseId
        /// </summary>
        public int ItemId { get; set; }

        /// <summary>
        /// The culture code status
        /// </summary>
        public ECultureCodeStatus CultureCodeStatus { get; set; }

        /// <summary>
        /// The version number of this text.
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// Whether this is the current version or not
        /// </summary>
        public bool CurrentVersion { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime CreatedOnDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int CreatedByUserId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime ModifiedOnDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int ModifiedByUserId { get; set; }

        /// <summary>
        /// Empty constructor
        /// </summary>
        public PHLatex()
        { }

        /// <summary>
        /// Constructs a PHLatex
        /// </summary>
        /// <param name="text"></param>
        /// <param name="cultureCode"></param>
        /// <param name="itemType"></param>
        public PHLatex(string text, string cultureCode, ELatexItemType itemType)
        {
            Text = text;
            CultureCode = cultureCode;
            ItemType = itemType;
        }
    }

    /// <summary>
    /// The entity class for a YouTube video.
    /// </summary>
    [TableName("YouTube")]
    [PrimaryKey("YouTubeId", AutoIncrement = true)]
    [Cacheable("YouTube", CacheItemPriority.Normal, 20)]
    public class YouTube
    {
        ///<summary>
        /// The ID of the YouTube. Key and AutoInc
        ///</summary>
        public int YouTubeId { get; set; }

        /// <summary>
        /// The 11 character YouTube code. For example "9CMrj0zuJGA"
        /// </summary>
        public string YouTubeCode { get; set; }

        /// <summary>
        /// The Id of the PluggComponent to which this video belongs
        /// </summary>
        public int PluggComponentId { get; set; }

        /// <summary>
        /// The  YouTube Title displayed directly below the clip on YouTube.com
        /// </summary>
        public string YouTubeTitle { get; set; }

        /// <summary>
        /// The  YouTube comment displayed above the comments on YouTube.com
        /// </summary>
        public string YouTubeComment { get; set; }

        /// <summary>
        /// The duration of the clip in seconds
        /// </summary>
        public int YouTubeDuration { get; set; }

        /// <summary>
        /// The Author of the clip on YouTube
        /// </summary>
        public string YouTubeAuthor { get; set; }

        /// <summary>
        /// When the clip was created on youtube.com
        /// </summary>
        public DateTime YouTubeCreatedOn { get; set; }

        /// <summary>
        /// When the video was added to Plugghest
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// The DNN UserId of the person adding video to Plugghest
        /// </summary>
        public int CreatedByUserId { get; set; }

        /// <summary>
        /// Gets the iframe string to display the video
        /// </summary>
        /// <param name="cultureCode"></param>
        /// <returns></returns>
        public string GetIframeString(string cultureCode)
        {
            cultureCode = cultureCode.Substring(0, 2);
            return "<iframe width=\"640\" height=\"390\" src=http://www.youtube.com/embed/" + YouTubeCode + "?cc_load_policy=1&amp;cc_lang_pref=" + cultureCode + "en\" frameborder=\"0\"></iframe>";
        }

    }

    [TableName("Courses")]
    [PrimaryKey("CourseId", AutoIncrement = true)]
    [Cacheable("Courses", CacheItemPriority.Normal, 20)]
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

    //[TableName("CourseMenuHeadings")]
    //[PrimaryKey("HeadingID", AutoIncrement = true)]
    //[Cacheable("CourseMenuHeadings", CacheItemPriority.Normal, 20)]
    //public class CourseMenuHeadings
    //{
    //    public int HeadingID { get; set; }

    //    public string Title { get; set; }
    //}

    [TableName("CourseItemComment")]
    [PrimaryKey("CourseItemCommentID", AutoIncrement = true)]
    [Cacheable("CourseItemComment", CacheItemPriority.Normal, 20)]
    public class CourseItemComment
    {
        public int CourseItemCommentID { get; set; }
        public int CourseID { get; set; }
        public int ItemId { get; set; }
        public int ItemType { get; set; }
        public string HtmlText { get; set; }
    }

    /// <summary>
    /// Hierarchy of subjects such as "Chemestry"
    /// Helps organizing Pluggs
    /// The actual name of the subject (in all languages) is in PHText
    /// </summary>
    [TableName("Subjects")]
    [PrimaryKey("SubjectId", AutoIncrement = true)]
    public class Subject
    {
        /// <summary>
        /// Primary key
        /// </summary>
        public int SubjectId { get; set; }

        /// <summary>
        /// SubjectId of Mother to subject
        /// </summary>
        public int? MotherId { get; set; }

        /// <summary>
        /// The order of this subject among all subjects with the same Mother
        /// </summary>
        public int SubjectOrder { get; set; }

        /// <summary>
        /// The name of the subject. Located in PHText table.
        /// </summary>
        [IgnoreColumn]
        public string label { get; set; }

        /// <summary>
        /// Mother of Subject as an object
        /// </summary>
        [IgnoreColumn]
        public Subject Mother { get; set; }

        /// <summary>
        /// List of a children to a Subject
        /// </summary>
        [IgnoreColumn]
        public IList<Subject> children { get; set; }
    }

    //public class Youtube
    //{
    //    private string _youTubeCode; //11 character code

    //    public Youtube()
    //    {
    //        IsValid = false;
    //    }

    //    public Youtube(string code)
    //    {
    //        code = code.Trim();
    //        if (code.Length == 11)
    //        {
    //            YouTubeCode = code;
    //            IsValid = true;
    //        }
    //        else
    //        {
    //            if (code.IndexOf("www.youtube.com") > -1)
    //            //Assume that code is the final 11 characters
    //            {
    //                YouTubeCode = code.Substring(code.Length - 11, 11);
    //                IsValid = true;
    //            }
    //            else
    //            {
    //                IsValid = false;
    //            }
    //        }
    //    }

    //    public string YouTubeCode
    //    {
    //        get { return _youTubeCode; }
    //        set
    //        {
    //            if (value.Length == 11)
    //            {
    //                _youTubeCode = value;
    //                IsValid = true;
    //            }
    //            else
    //                throw new Exception("Youtube code must have 11 characters");
    //        }
    //    }

    //    public bool IsValid { get; set; }

    //    public string GetIframeString(string CultureCode)
    //    {
    //        if (CultureCode.Length != 2)
    //            throw new Exception("Culture code must have 2 characters");
    //        return "<iframe width=\"640\" height=\"390\" src=http://www.youtube.com/embed/" + YouTubeCode + "?cc_load_policy=1&amp;cc_lang_pref=" + CultureCode + "en\" frameborder=\"0\"></iframe>";
    //    }

    //    //public string GetYouTubeData(string FilterBy, string videoID)
    //    //{
    //    //    int lb = 0;
    //    //    int ub = 0;
    //    //    string videoHTML = "";
    //    //    string videoData = "";
    //    //    string vidMarker = "";
    //    //    HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://gdata.youtube.com/feeds/api/videos?q=" + videoID);
    //    //    StreamReader sr = new StreamReader(request.GetResponse().GetResponseStream());
    //    //    switch (FilterBy)
    //    //    {
    //    //        case "Title":
    //    //            vidMarker = "<media:title type='plain'>";
    //    //            if (string.IsNullOrEmpty(vidMarker)) return string.Empty;
    //    //            videoHTML = sr.ReadToEnd();
    //    //            lb = videoHTML.IndexOf(vidMarker) + vidMarker.Length;
    //    //            ub = videoHTML.IndexOf("</media:title>", lb);
    //    //            videoData = videoHTML.Substring(lb, ub - lb);
    //    //            break;
    //    //        case "Views":
    //    //            vidMarker = "viewCount='";
    //    //            if (string.IsNullOrEmpty(vidMarker)) return string.Empty;
    //    //            videoHTML = sr.ReadToEnd();
    //    //            lb = videoHTML.IndexOf(vidMarker) + vidMarker.Length;
    //    //            ub = videoHTML.IndexOf("'", lb);
    //    //            videoData = videoHTML.Substring(lb, ub - lb);
    //    //            break;
    //    //        case "Length":
    //    //            vidMarker = "<yt:duration seconds='";
    //    //            if (string.IsNullOrEmpty(vidMarker)) return string.Empty;
    //    //            videoHTML = sr.ReadToEnd();
    //    //            lb = videoHTML.IndexOf(vidMarker) + vidMarker.Length;
    //    //            ub = videoHTML.IndexOf("'", lb);
    //    //            string Seconds = videoHTML.Substring(lb, ub - lb);
    //    //            TimeSpan t = TimeSpan.FromSeconds(int.Parse(Seconds));
    //    //            videoData = t.Minutes + ":" + t.Seconds;
    //    //            break;
    //    //    }
    //    //    return videoData;
    //    //}


    //}
}
