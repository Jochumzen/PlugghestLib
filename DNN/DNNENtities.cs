using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Caching;

namespace Plugghest.DNN
{
    [TableName("TabUrls")]
    //setup the primary key for table
    [PrimaryKey("TabId", AutoIncrement = false)]
    //configure caching using PetaPoco
    [Cacheable("TabUrls", CacheItemPriority.Default, 20)]
    public class TabUrl
    {
        ///<summary>
        /// 
        ///</summary>
        public int TabId { get; set; }

        ///<summary>
        /// 
        ///</summary>
        public int SeqNum { get; set; }

        ///<summary>
        /// 
        ///</summary>
        public string Url { get; set; }

        ///<summary>
        /// 
        ///</summary>
        public string QueryString { get; set; }

        ///<summary>
        /// 
        ///</summary>
        public string HttpStatus { get; set; }

        ///<summary>
        /// 
        ///</summary>
        public string CultureCode { get; set; }

        ///<summary>
        /// 
        ///</summary>
        public bool IsSystem { get; set; }

        ///<summary>
        /// 
        ///</summary>
        public int? PortalAliasId { get; set; }

        ///<summary>
        /// 
        ///</summary>
        public int PortalAliasUsage { get; set; }

        ///<summary>
        /// 
        ///</summary>
        public int CreatedByUserID { get; set; }

        ///<summary>
        /// 
        ///</summary>
        public DateTime CreatedOnDate { get; set; }

        ///<summary>
        /// 
        ///</summary>
        public int LastModifiedByUserID { get; set; }

        ///<summary>
        /// 
        ///</summary>
        public DateTime LastModifiedOnDate { get; set; }

    }

    [TableName("DCC_PRC_CommentObject")]
    //setup the primary key for table
    [PrimaryKey("CommentObjectID", AutoIncrement = true)]
    //configure caching using PetaPoco
    [Cacheable("DCC_PRC_CommentObject", CacheItemPriority.Default, 20)]
    public class DCC_PRC_CommentObject
    {
        ///<summary>
        /// 
        ///</summary>
        public int CommentObjectID { get; set; }

        ///<summary>
        /// 
        ///</summary>
        public string CommentObject { get; set; }

        ///<summary>
        /// 
        ///</summary>
        public int CommentObjectViewCount { get; set; }

        ///<summary>
        /// 
        ///</summary>
        public int PortalID { get; set; }
    }
}
