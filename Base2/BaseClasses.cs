﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Web.UI.WebControls;

namespace Plugghest.Base2
{
    ///<summary>
    /// The Plugg container simplifies the work with a Plugg.
    /// It contains the Plugg entity, the Title, the Description and a list of components
    /// It also contains helper functions to load and save these things
    ///</summary>
    public class PluggContainer
    {
        ///<summary>
        /// The Plugg entity.
        ///</summary>
        public Plugg ThePlugg;

        ///<summary>
        /// The Language we are dealing with at the moment. 
        /// The language in which the Plugg was created is in ThePlugg.
        ///</summary>
        public string CultureCode;

        ///<summary>
        /// The Title of the Plugg (in the CultureCode language)
        ///</summary>
        public PHText TheTitle;

        ///<summary>
        /// The description of the Plugg (in the CultureCode language)
        ///</summary>
        public PHText TheDescription;

        ///<summary>
        /// A list of all component in the Plugg in the correct order
        ///</summary>
        public IEnumerable<PluggComponent> TheComponents;

        /// <summary>
        /// Base constructor. Creates a new Plugg in the language cultureCode.
        /// </summary>
        /// <param name="cultureCode"></param>
        public PluggContainer(string cultureCode)
        {
            ThePlugg = new Plugg();
            CultureCode = cultureCode;
        }

        /// <summary>
        /// Loads an existing Plugg in the language cultureCode
        /// </summary>
        /// <param name="cultureCode"></param>
        /// <param name="pluggId"></param>
        public PluggContainer(string cultureCode, int pluggId)
        {
            BaseRepository rep = new BaseRepository();
            CultureCode = cultureCode;
            ThePlugg = rep.GetPlugg(pluggId);
        }

        ///<summary>
        /// Creates a Title Object (a PHText). 
        /// Sets its Text to htmlText, 
        /// its CultureCode to the CultureCode of the PluggContainer,
        /// its ETextItemType to PluggTitle.
        /// To save it, use BaseHandler.SavePhText(PHText t)
        ///</summary>
        public void SetTitle(string htmlText)
        {
            TheTitle = new PHText(htmlText, CultureCode, ETextItemType.PluggTitle);
        }

        ///<summary>
        /// Load the title in the CultureCode language from DB. You must set PluggId and CultureCode to get the Title
        ///</summary>
        public void LoadTitle()
        {
            if (ThePlugg == null || ThePlugg.PluggId == 0)
                throw new Exception("Cannot load title. Need PluggId");
            BaseRepository rep = new BaseRepository();
            TheTitle = rep.GetCurrentVersionText(CultureCode, ThePlugg.PluggId, ETextItemType.PluggTitle);
        }

        ///<summary>
        /// Create a Description Object (a PHText).
        /// Sets its Text to htmlText, 
        /// its CultureCode to the CultureCode of the PluggContainer,
        /// its ETextItemType to PluggDescription.
        /// To save it, use BaseHandler.SavePhText(PHText t)
        ///</summary>
        public void SetDescription(string htmlText)
        {
            TheDescription = new PHText(htmlText, CultureCode, ETextItemType.PluggDescription);
        }

        ///<summary>
        /// Load the Description in the CultureCode language from DB. You must set PluggId and CultureCode to get the Description
        ///</summary>
        public void LoadDescription()
        {
            if (ThePlugg == null || ThePlugg.PluggId == 0 || CultureCode == null)
                throw new Exception("Cannot load Description. Need PluggId and CultureCode");
            BaseRepository rep = new BaseRepository();
            TheDescription = rep.GetCurrentVersionText(CultureCode, ThePlugg.PluggId, ETextItemType.PluggDescription);
        }

        ///<summary>
        /// Loads all the components of a Plugg into TheComponents.
        /// Note: If the actual content of the component has not yet been created, Load will create an empty object with the correct ItemType
        ///</summary>
        /// <returns> returns nothing.</returns>
        public void LoadComponents()
        {
            BaseRepository rep = new BaseRepository();
            TheComponents = rep.GetAllComponentsInPlugg(ThePlugg.PluggId);
        }

        public List<PluggComponent> GetComponentList()
        {
            if (TheComponents == null)
                LoadComponents();
            return TheComponents.ToList();
        }
 
    }

    ///<summary>
    /// The Course container simplifies the work with a Course.
    /// It contains the Course entity, the Title, the Description and a list of Courses
    /// It also contains helper functions to load and save these things
    ///</summary>
    public class CourseContainer
    {
        ///<summary>
        /// The Course entity.
        ///</summary>
        public Course TheCourse;

        ///<summary>
        /// The Language we are dealing with at the moment. 
        /// The language in which the Course was created is in TheCourse.
        ///</summary>
        public string CultureCode;

        ///<summary>
        /// The Title of the Course (in the CultureCode language)
        ///</summary>
        public PHText TheTitle;

        ///<summary>
        /// The description of the Course (in the CultureCode language)
        ///</summary>
        public PHText TheDescription;

        /////<summary>
        ///// A list of all Pluggs in the Course in the correct order
        /////</summary>
        //public IEnumerable<CourseComponent> TheComponents;

        /// <summary>
        /// Base constructor. Creates a new Course in the language cultureCode.
        /// </summary>
        /// <param name="cultureCode"></param>
        public CourseContainer(string cultureCode)
        {
            TheCourse = new Course();
            CultureCode = cultureCode;
        }

        /// <summary>
        /// Loads an existing Course in the language cultureCode
        /// </summary>
        /// <param name="cultureCode"></param>
        /// <param name="CourseId"></param>
        public CourseContainer(string cultureCode, int CourseId)
        {
            BaseRepository rep = new BaseRepository();
            CultureCode = cultureCode;
            TheCourse = rep.GetCourse(CourseId);
        }

        ///<summary>
        /// Creates a Title Object (a PHText). 
        /// Sets its Text to htmlText, 
        /// its CultureCode to the CultureCode of the CourseContainer,
        /// its ETextItemType to CourseTitle.
        /// To save it, use BaseHandler.SavePhText(PHText t)
        ///</summary>
        public void SetTitle(string htmlText)
        {
            TheTitle = new PHText(htmlText, CultureCode, ETextItemType.CourseTitle);
        }

        ///<summary>
        /// Load the title in the CultureCode language from DB. You must set CourseId and CultureCode to get the Title
        ///</summary>
        public void LoadTitle()
        {
            if (TheCourse == null || TheCourse.CourseId == 0)
                throw new Exception("Cannot load title. Need CourseId");
            BaseRepository rep = new BaseRepository();
            TheTitle = rep.GetCurrentVersionText(CultureCode, TheCourse.CourseId, ETextItemType.CourseTitle);
        }

        ///<summary>
        /// Create a Description Object (a PHText).
        /// Sets its Text to htmlText, 
        /// its CultureCode to the CultureCode of the CourseContainer,
        /// its ETextItemType to CourseDescription.
        /// To save it, use BaseHandler.SavePhText(PHText t)
        ///</summary>
        public void SetDescription(string htmlText)
        {
            TheDescription = new PHText(htmlText, CultureCode, ETextItemType.CourseDescription);
        }

        ///<summary>
        /// Load the Description in the CultureCode language from DB. You must set CourseId and CultureCode to get the Description
        ///</summary>
        public void LoadDescription()
        {
            if (TheCourse == null || TheCourse.CourseId == 0 || CultureCode == null)
                throw new Exception("Cannot load Description. Need CourseId and CultureCode");
            BaseRepository rep = new BaseRepository();
            TheTitle = rep.GetCurrentVersionText(CultureCode, TheCourse.CourseId, ETextItemType.CourseDescription);
        }
    }

    public class CoursePlugg : CoursePluggEntity
    {
        /// <summary>
        /// The Title of the Plugg. Located in PHText table.
        /// </summary>
        public string label { get; set; }

        /// <summary>
        /// Mother of a CoursePlugg as an object
        /// </summary>
        public CoursePlugg Mother { get; set; }

        /// <summary>
        /// List of a children to a CoursePlugg
        /// </summary>
        public IList<CoursePlugg> children { get; set; }
    }

    public class Subject : SubjectEntity 
    {
        /// <summary>
        /// The name of the subject. Located in PHText table.
        /// </summary>
        public string label { get; set; }

        /// <summary>
        /// Mother of Subject as an object
        /// </summary>
        public Subject Mother { get; set; }

        /// <summary>
        /// List of a children to a Subject
        /// </summary>
        public IList<Subject> children { get; set; }
    }    


    public class PluggInfoForDnnGrid
    {
        public int PluggId { get; set; }
        public string Text { get; set; }
        public string UserName { get; set; }
    }

    public class CourseInfoForDnnGrid
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public string UserName { get; set; }
    }
}
