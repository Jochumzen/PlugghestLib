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
using System.Collections.Generic;
using DotNetNuke.Data;
using System.Data;

namespace Plugghest.Modules.Plugghest_Subjects.Components
{
    class ItemController
    {
        

        public void DeleteItem(int itemId, int moduleId)
        {
            var t = GetItem(itemId, moduleId);
            DeleteItem(t);
        }

        public void DeleteItem(SubjectItem t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<SubjectItem>();
                rep.Delete(t);
            }
        }

        public IEnumerable<SubjectItem> GetItems(int moduleId)
        {
            IEnumerable<SubjectItem> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<SubjectItem>();
                t = rep.Get(moduleId);
            }
            return t;
        }

        public SubjectItem GetItem(int itemId, int moduleId)
        {
            SubjectItem t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<SubjectItem>();
                t = rep.GetById(itemId, moduleId);
            }
            return t;
        }

        public void UpdateItem(SubjectItem t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<SubjectItem>();
                rep.Update(t);
            }
        }

        public DataTable GetNodeItems()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("NodeID");
            dt.Columns.Add("NodeTitle");
            dt.Columns.Add("Mother");
            dt.Columns.Add("Order");
            IEnumerable<SubjectItem> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<SubjectItem>();
                t = rep.Get();
                foreach (SubjectItem val in t)
                {
                    dt.Rows.Add(val.SubjectID, val.Subject, val.Mother, val.Order);
                }
            }

            return dt;
        }


       


        public List<Subject_Item> GetSubject_Item()
        {
            List<Subject_Item> objsubjectitem = new List<Subject_Item>();
            using (IDataContext ctx = DataContext.Instance())
            {
                var rec = ctx.ExecuteQuery<Subject_Item>(CommandType.TableDirect, "select * from SubjectItems order by [Order]");
                foreach (var val in rec)
                {
                    objsubjectitem.Add(new Subject_Item { SubjectID = val.SubjectID, Subject = val.Subject, label = val.Subject, Mother = val.Mother, Order = val.Order });
                }

            }

            return objsubjectitem;
        }



        //insert on subject
        public void CreateSubject(SubjectItem t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<SubjectItem>();
                rep.Insert(t);
            }
        }


        public SubjectItem GetSubject(int SubjectId)
        {
            SubjectItem subitem = new SubjectItem();
            using (IDataContext ctx = DataContext.Instance())
            {
                var rec = ctx.ExecuteQuery<SubjectItem>(CommandType.TableDirect, "select * from SubjectItems where subjectid="+SubjectId);
                foreach (var val in rec)
                {
                   subitem.Subject = val.Subject; subitem.Mother = val.Mother; subitem.Order = val.Order ;
                }
            }

            return subitem;
        }


        public List<SubjectItem> GetSubjectFromMother(int? MotherName, int order)
        {
            List<SubjectItem> sublist = new List<SubjectItem>();
            using (IDataContext ctx = DataContext.Instance())
            {
                var rec = ctx.ExecuteQuery<SubjectItem>(CommandType.TableDirect, "select * from SubjectItems where Mother=" + MotherName + "AND [ORDER] >="+order+" order by [order]");
                foreach (var val in rec)
                {
                    sublist.Add(new SubjectItem { SubjectID=val.SubjectID,Subject=val.Subject,Mother=val.Mother});
                }
            }
            return sublist;
        }


        public void UpdateSubjectOrder(int SubjectId,int Order)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                ctx.Execute(CommandType.Text, "update SubjectItems set [Order]=" + Order + " where Subjectid=" + SubjectId);
            }
        }
    }
}
