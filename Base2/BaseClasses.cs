using System;
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
            TheTitle = rep.GetCurrentVersionText(CultureCode, ThePlugg.PluggId, ETextItemType.PluggDescription);
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

    public class CourseItem : CourseItemEntity
    {
        public CourseItem Mother { get; set; }
        public IList<CourseItem> children { get; set; }
        public string label { get; set; }
        public string name { get; set; }
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
