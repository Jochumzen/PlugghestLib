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

namespace Plugghest.Subjects
{
    [TableName("Subjects")]
    [PrimaryKey("SubjectId", AutoIncrement = true)]
    public class Subject
    {
        public int SubjectId { get; set; }

        [ColumnName("Title")]
        public string label { get; set; }

        public int? MotherId { get; set; }

        public int SubjectOrder { get; set; }

        [IgnoreColumn]
        public Subject Mother { get; set; }

        [IgnoreColumn]
        public IList<Subject> children { get; set; }

    }

}
