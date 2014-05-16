using System.Globalization;
using DotNetNuke.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Plugghest.Base
{
    class BaseRepository
    {
        #region Plugg

        public void CreatePlugg(Plugg t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Plugg>();
                rep.Insert(t);
            }
        }

        public Plugg GetPlugg(int pluggId)
        {
            Plugg p;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Plugg>();
                p = rep.GetById(pluggId);
            }
            return p;
        }

        public void UpdatePlugg(Plugg plug)
        {
            using (IDataContext db = DataContext.Instance())
            {
                var rep = db.GetRepository<Plugg>();
                rep.Update(plug);
            }
        }

        public void DeletePlugg(Plugg plug)
        {
            using (IDataContext db = DataContext.Instance())
            {
                var rep = db.GetRepository<Plugg>();
                rep.Delete(plug);
            }
        }

        public IEnumerable<Plugg> GetAllPluggs()
        {
            IEnumerable<Plugg> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Plugg>();
                t = rep.Get();
            }
            return t;
        }

        public void DeleteAllPluggs()
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                ctx.ExecuteQuery<Plugg>(CommandType.TableDirect, "DELETE FROM Pluggs DBCC CHECKIDENT ('Pluggs',RESEED, 0)");
                //use DBCC CHECKIDENT  for start with 0 ............
            }
        }

        #endregion

        #region PluggContent

        public void CreatePluggContent(PluggContent t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<PluggContent>();
                rep.Insert(t);
            }
        }

        public PluggContent GetPluggContent(int pluggContentId)
        {
            PluggContent pc;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<PluggContent>();
                pc = rep.GetById(pluggContentId);
            }
            return pc;
        }

        public PluggContent GetPluggContent(int pluggId, string cultureCode)
        {
            PluggContent ThePC = null;
            IEnumerable<PluggContent> pcs;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<PluggContent>();
                pcs = rep.Find("Where PluggId = @0 AND CultureCode = @1", pluggId, cultureCode);
            }
            if (pcs.Any())
                ThePC = pcs.First();  //There can be only one. PetaPoco does not handle composite key
            return ThePC;
        }

        public void UpdatePluggContent(PluggContent pc)
        {
            using (IDataContext db = DataContext.Instance())
            {
                var rep = db.GetRepository<PluggContent>();
                rep.Update(pc);
            }
        }

        public void DeletePluggContent(PluggContent pc)
        {
            using (IDataContext db = DataContext.Instance())
            {
                var rep = db.GetRepository<PluggContent>();
                rep.Delete(pc);
            }
        }

        public IEnumerable<PluggContent> GetAllContentInPlugg(int pluggId)
        {
            IEnumerable<PluggContent> pc;
            using (IDataContext ctx = DataContext.Instance())
            {
                var repository = ctx.GetRepository<PluggContent>();
                pc = repository.Find("WHERE PluggId = @0", pluggId);
            }
            return pc;
        }

        public void DeleteAllPluggContent()
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                ctx.ExecuteQuery<PluggContent>(CommandType.TableDirect, "truncate table PluggsContent");
            }
        }

        #endregion

        #region Course

        public void CreateCourse(Course t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Course>();
                rep.Insert(t);
            }
        }

        public void DeleteCourse(Course t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Course>();
                rep.Delete(t);
            }
        }

        public void UpdateCourse(Course t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Course>();
                rep.Update(t);
            }
        }

        public Course GetCourse(int? courseId)
        {
            Course p;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Course>();
                p = rep.GetById(courseId);
            }
            return p;
        }

        #endregion

        #region CourseItem

        public void CreateCourseItem(CourseItemEntity t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<CourseItemEntity>();
                rep.Insert(t);
            }
        }

        public void UpdateCourseItem(CourseItemEntity t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<CourseItemEntity>();
                rep.Update(t);
            }
        }

        public void DeleteCourseItem(CourseItemEntity t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<CourseItemEntity>();
                rep.Delete(t);
            }
        }

        public CourseItemEntity GetCourseItem(int courseItemId)
        {
            CourseItemEntity p;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<CourseItemEntity>();
                p = rep.GetById(courseItemId);
            }
            return p;
        }

        public List<CourseItem> GetItemsInCourse(int courseId)
        {
            List<CourseItem> cps = new List<CourseItem>();

            using (IDataContext context = DataContext.Instance())
            {
                string sqlPlugg = "SELECT 1 AS label, CourseItemId, CourseId, ItemId, CIOrder, ItemType, MotherId FROM CourseItems INNER JOIN Pluggs ON PluggID=ItemId WHERE ItemType=" + (int)ECourseItemType.Plugg + " AND CourseId=" + courseId;
                string sqlHeading = "SELECT Title AS label, CourseItemId, CourseId, ItemId, CIOrder, ItemType, MotherId FROM CourseItems INNER JOIN CourseMenuHeadings ON HeadingID=ItemId WHERE ItemType=" + (int)ECourseItemType.Heading + " AND CourseId=" + courseId;
                var rec = context.ExecuteQuery<CourseItem>(CommandType.Text, sqlPlugg + " UNION " + sqlHeading + " ORDER BY CIOrder");

                foreach (var ci in rec)
                {
                    cps.Add(new CourseItem { CourseItemId = ci.CourseItemId, CourseId = ci.CourseId, ItemId = ci.ItemId, CIOrder = ci.CIOrder, ItemType = ci.ItemType, MotherId = ci.MotherId, label = ci.label, name = ci.label });
                }
            }
            return cps;
        }

        //The same item may go into a course several times so collection is correct
        public IEnumerable<CourseItem> GetCourseItems(int courseId, int itemId)
        {
            IEnumerable<CourseItem> cis;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<CourseItem>();
                cis = rep.Find("WHERE CourseId = @0 AND ItemId = @1", courseId, itemId);
            }
            return cis;
        }

        #endregion

        #region PHText

        public void CreatePhText(PHText p)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<PHText>();
                rep.Insert(p);
            }
        }

        public PHText GetPhText(int textId)
        {
            PHText p;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<PHText>();
                p = rep.GetById(textId);
            }
            return p;
        }

        public PHText GetPhText(string cultureCode, int itemId, int itemType)
        {
            IEnumerable<PHText> txt;
            PHText theText = null;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<PHText>();
                txt = rep.Find("WHERE CultureCode = @0 AND ItemId = @1 AND ItemType = @2", cultureCode, itemId, itemType);
            }

            if (txt.Any())
                theText = txt.First(); //Can only be at most one. PetaPoco does not handle composite key

            return theText;
        }

        public void UpdatePhText(PHText p)
        {
            using (IDataContext db = DataContext.Instance())
            {
                var rep = db.GetRepository<PHText>();
                rep.Update(p);
            }
        }

        public void DeletePhText(PHText p)
        {
            using (IDataContext db = DataContext.Instance())
            {
                var rep = db.GetRepository<PHText>();
                rep.Delete(p);
            }
        }

        public void DeleteAllPhTextForItem(int itemId, int itemType)
        {
            using (IDataContext context = DataContext.Instance())
            {
                var rec = context.ExecuteQuery<PHText>(CommandType.Text,
                    "DELETE FROM PHTexts WHERE ItemId=@0 AND ItemType=@1", itemId, itemType);
            }
        }

        #endregion

        #region LatexText

        public void CreateLatexText(PHLatex p)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<PHLatex>();
                rep.Insert(p);
            }
        }

        public PHLatex GetLatexText(int textId)
        {
            PHLatex p;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<PHLatex>();
                p = rep.GetById(textId);
            }
            return p;
        }

        public PHLatex GetLatexText(string cultureCode, int itemId, int itemType)
        {
            IEnumerable<PHLatex> txt;
            PHLatex theText = null;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<PHLatex>();
                txt = rep.Find("WHERE CultureCode = @0 AND ItemId = @1 AND ItemType = @2", cultureCode, itemId, itemType);
            }

            if (txt.Any())
                theText = txt.First(); //Can only be at most one. PetaPoco does not handle composite key

            return theText;
        }

        public void UpdateLatexText(PHLatex p)
        {
            using (IDataContext db = DataContext.Instance())
            {
                var rep = db.GetRepository<PHLatex>();
                rep.Update(p);
            }
        }

        public void DeleteLatexText(PHLatex p)
        {
            using (IDataContext db = DataContext.Instance())
            {
                var rep = db.GetRepository<PHLatex>();
                rep.Delete(p);
            }
        }

        public void DeleteAllLatexForItem(int itemId, int itemType)
        {
            using (IDataContext context = DataContext.Instance())
            {
                var rec = context.ExecuteQuery<PHText>(CommandType.Text,
                    "DELETE FROM PHLatex WHERE ItemId=@0 AND ItemType=@1", itemId, itemType);
            }
        }

        #endregion

        #region Other

        //public List<PluggInfoForDNNGrid> GetPluggRecords(string cultureCode)
        //{
        //    List<PluggInfoForDNNGrid> pluggs = new List<PluggInfoForDNNGrid>();
        //    using (IDataContext ctx = DataContext.Instance())
        //    {
        //        var rec = ctx.ExecuteQuery<PluggInfoForDNNGrid>(CommandType.Text, "SELECT PluggId, Text, Username FROM pluggs JOIN Users ON users.UserID=Pluggs.CreatedByUserId JOIN PHTexts ON ItemId=Pluggs.PluggId WHERE ItemType=@0 AND CultureCode=@1" , (int)ETextItemType.PluggTitle , cultureCode);

        //        foreach (var item in rec)
        //        {
        //            pluggs.Add(new PluggInfoForDNNGrid { PluggId = item.PluggId, PluggName = item.PluggName , UserName = item.UserName });
        //        }
        //    }

        //    return pluggs;
        //}

        public IEnumerable<PluggInfoForDNNGrid> GetPluggRecords(string cultureCode)
        {
            IEnumerable<PluggInfoForDNNGrid> pluggs;
            using (IDataContext ctx = DataContext.Instance())
            {
                pluggs = ctx.ExecuteQuery<PluggInfoForDNNGrid>(CommandType.Text, "SELECT PluggId, Text, Username FROM Pluggs JOIN Users ON Users.UserID=Pluggs.CreatedByUserId JOIN PHTexts ON ItemId=Pluggs.PluggId WHERE ItemType=" + (int)ETextItemType.PluggTitle + " AND CultureCode='" + cultureCode + "'");
            }
            return pluggs;
        }

        //CourseForDNN

        public List<CourseInfoForDNNGrid> GetCoursesForDNN()
        {
            List<CourseInfoForDNNGrid> cs = new List<CourseInfoForDNNGrid>();
            using (IDataContext ctx = DataContext.Instance())
            {
                var rec = ctx.ExecuteQuery<CourseInfoForDNNGrid>(CommandType.TableDirect, @"select CourseId, Title as CourseName, Username from courses join Users on Users.UserID=Courses.CreatedByUserId ");

                foreach (var item in rec)
                {
                    cs.Add(new CourseInfoForDNNGrid() { CourseId = item.CourseId, CourseName = item.CourseName, UserName = item.UserName });
                }
            }

            return cs;
        }

        #endregion

        #region CourseHeading
        public CourseMenuHeadings CreateHeading(CourseMenuHeadings t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<CourseMenuHeadings>();
                rep.Insert(t);
            }
            return t;
        }


        public void UpdateHeading(CourseMenuHeadings t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<CourseMenuHeadings>();
                rep.Update(t);
            }
        }

        public void DeleteHeading(CourseMenuHeadings t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<CourseMenuHeadings>();
                rep.Delete(t);
            }
        }
        #endregion


        #region CourseItemComment
        public IEnumerable<CourseItemComment> GetCourseItemComment(int courseId, int itemId)
        {
            IEnumerable<CourseItemComment> cic;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<CourseItemComment>();
                cic = rep.Find("WHERE CourseId = @0 AND ItemId = @1", courseId, itemId);
            }
            return cic;
        }


        public void UpdateCourseItemComment(CourseItemComment CIC)
        {
            using (IDataContext db = DataContext.Instance())
            {
                var rep = db.GetRepository<CourseItemComment>();
                rep.Update(CIC);
            }
        }
        #endregion
    }
}
