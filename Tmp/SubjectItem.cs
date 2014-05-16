/*
' Copyright (c) 2014 Plugghest.com
'  All rights reserved.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
' 
*/

using System;
using System.Web.Caching;
using DotNetNuke.Common.Utilities;
using DotNetNuke.ComponentModel.DataAnnotations;
using DotNetNuke.Entities.Content;
using System.Collections.Generic;

namespace Plugghest.Modules.Plugghest_Subjects.Components
{
    [TableName("SubjectItems")]
    //setup the primary key for table
    [PrimaryKey("SubjectID", AutoIncrement = true)]
    //configure caching using PetaPoco
   // [Cacheable("NodeTitle", CacheItemPriority.Default, 20)]
    //scope the objects to the ModuleId of a module on a page (or copy of a module on a page)
  //  [Scope("ModuleId")]

    class SubjectItem
    {
        public int SubjectID { get; set; }

        public string Subject { get; set; }

        public int? Mother { get; set; }

        public int Order { get; set; }
    }


    public class Subject_Item
    {
        public int SubjectID { get; set; }

        public string Subject { get; set; }

        public int? Mother { get; set; }

        public List<Subject_Item> children { get; set; }

        public int Order { get; set; }

        public string label { get; set; }

    }

}
