
namespace Plugghest.Base2
{
    ///<summary>
    /// Who is allowed to edit this object. Admin can always edit.
    ///</summary>
    public enum EWhoCanEdit
    {
        ///<summary>
        /// Not yet set.
        ///</summary>
        NotSet = 0,

        ///<summary>
        /// Any registered user can edit
        ///</summary>
        Anyone,

        ///<summary>
        /// Only the user who created the object can edit (and admin)
        ///</summary>
        OnlyMe
    }

    /// <summary>
    /// All text is stored in PHText to allow for versioning and translation.
    /// This enum keeps track of what type of text we are dealing with.
    /// Text is also identified by an ItemId, a CultureCode and a VersionId
    /// </summary>
    public enum ETextItemType
    {
        /// <summary>
        /// Not yet set.
        /// </summary>
        NotSet = 0,

        /// <summary>
        /// A Plugg Title. 
        /// For this ItemType, ItemId = PluggId. 
        /// No Html allowed, not versioned.
        /// </summary>
        PluggTitle,

        /// <summary>
        /// A Plugg Description. 
        /// For this ItemType, ItemId = PluggId. 
        /// No Html, not versioned.
        /// </summary>
        PluggDescription,

        /// <summary>
        /// Text belonging to the Plugg Component RichRichText. 
        /// For this ItemType, ItemId = PluggComponentId
        /// Full Html allowed, versioned.
        /// </summary>
        PluggComponentRichRichText,

        /// <summary>
        /// Text belonging to the Plugg Component RichText. 
        /// For this ItemType, ItemId = PluggComponentId
        /// Limited Html allowed, versioned.
        /// </summary>
        PluggComponentRichText,

        /// <summary>
        /// Text belonging to the Plugg Component Label. 
        /// For this ItemType, ItemId = PluggComponentId
        /// No Html, not versioned.
        /// </summary>
        PluggComponentLabel,

        /// <summary>
        /// A Course Title. 
        /// For this ItemType, ItemId = CourseId. 
        /// No Html allowed, not versioned.
        /// </summary>
        CourseTitle,

        /// <summary>
        /// A Course Description. 
        /// For this ItemType, ItemId = CourseId. 
        /// No Html, not versioned.
        /// </summary>
        CourseDescription,

        /// <summary>
        /// Course Text. 
        /// For this ItemType, ItemId = CourseId
        /// Full Html allowed, versioned.
        /// </summary>
        CourseRichRichText,

        /// <summary>
        /// Text belonging to CoursePluggComments. 
        /// For this ItemType, ItemId = CoursePluggCommentId. 
        /// No Html, not versioned.
        /// </summary>
        CoursePluggText,

        /// <summary>
        /// Text belonging to CourseMenuHeading. 
        /// For this ItemType, ItemId = CourseMenuHeadingId. 
        /// No Html, not versioned.
        /// </summary>
        CourseMenuHeadingText,

        /// <summary>
        /// Text belonging to Subjects table
        /// For this ItemType, ItemId = SubjectId. 
        /// No Html, not versioned.
        /// </summary>
        Subject
    }

    /// <summary>
    /// All Latex text is stored in PHLatex to allow for versioning and translation.
    /// This enum keeps track of what type of text we are dealing with.
    /// Text is also identified by an ItemId, a CultureCode and a VersionId
    /// </summary>
    public enum ELatexItemType
    {
        /// <summary>
        /// Not yet set.
        /// </summary>
        NotSet = 0,

        /// <summary>
        /// Latex text belonging to the Plugg Component Latex
        /// For this ItemType, ItemId = PluggComponentId. 
        /// Versioned.
        /// </summary>
        PluggComponentLatex,

        /// <summary>
        /// Course Latex Text. 
        /// For this ItemType, ItemId = CourseId
        /// Versioned.
        /// </summary>
        CourseLatexText
    }

    /// <summary>
    /// The CultureCode status of some text in PHText or PHLatex
    /// </summary>
    public enum ECultureCodeStatus
    {
        /// <summary>
        /// Not yet set.
        /// </summary>
        NotSet = 0,

        /// <summary>
        /// This text is the original text. It is in the language of the text's culture Code.
        /// </summary>
        InCreationLanguage,

        /// <summary>
        /// This text has been translated by Google Translate
        /// </summary>
        GoogleTranslated,

        /// <summary>
        /// This text has been translated by a human
        /// </summary>
        HumanTranslated,

        /// <summary>
        /// This text has not been translated. 
        /// The text is still in the language it was created and not in the language of the text's culture Code
        /// </summary>
        NotTranslated
    }

    /// <summary>
    /// Components are parts of a Plugg and this enum keeps track of which type of component we are dealing with.
    /// </summary>
    public enum EComponentType
    {
        /// <summary>
        /// Not yet set
        /// </summary>
        NotSet = 0,

        /// <summary>
        /// A component allowing user to enter text in a full DNN Rich text editor with images, tables and what not.
        /// The text is stored in PHText with ETextItemType = PluggComponentRichRichText.
        /// </summary>
        RichRichText,

        /// <summary>
        /// A component allowing user to enter text in a simpler Rich Text editor with bullets, bold, italic and links.
        /// The text is stored in PHText with ETextItemType = PluggComponentRichText.
        /// </summary>
        RichText,

        /// <summary>
        /// A component allowing user to enter a single line of non-html text (to label other components in a Plugg).
        /// The text is stored in PHText with ETextItemType = PluggComponentLabel.
        /// </summary>
        Label,

        /// <summary>
        /// A component allowing user to enter Latex code which is translated into Html by L2H.
        /// The text is stored in PHLatex with ELatexItemtType = PluggComponentLatex.
        /// </summary>
        Latex,

        /// <summary>
        /// A component for displaying a YouTube video.
        /// The video is stored in YouTube.
        /// </summary>
        YouTube
    }
}
