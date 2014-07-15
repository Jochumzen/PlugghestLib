using System.Globalization;
using DotNetNuke.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Plugghest.Base2
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

        public void UpdatePlugg(Plugg plugg)
        {
            using (IDataContext db = DataContext.Instance())
            {
                var rep = db.GetRepository<Plugg>();
                rep.Update(plugg);
            }
        }

        public void DeletePlugg(Plugg plugg)
        {
            using (IDataContext db = DataContext.Instance())
            {
                var rep = db.GetRepository<Plugg>();
                rep.Delete(plugg);
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
                ctx.ExecuteQuery<Plugg>(CommandType.TableDirect, "DELETE FROM Pluggs2 DBCC CHECKIDENT ('Pluggs',RESEED, 0)");
                //use DBCC CHECKIDENT  for start with 0 ............
            }
        }

        #endregion

        #region PluggComponent

        public void CreatePluggComponent(PluggComponent t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<PluggComponent>();
                rep.Insert(t);
            }
        }

        public PluggComponent GetPluggComponent(int pluggComponentId)
        {
            PluggComponent p;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<PluggComponent>();
                p = rep.GetById(pluggComponentId);
            }
            return p;
        }

        public void UpdatePluggComponent(PluggComponent pc)
        {
            using (IDataContext db = DataContext.Instance())
            {
                var rep = db.GetRepository<PluggComponent>();
                rep.Update(pc);
            }
        }

        public void DeletePluggComponent(PluggComponent pc)
        {
            using (IDataContext db = DataContext.Instance())
            {
                var rep = db.GetRepository<PluggComponent>();
                rep.Delete(pc);
            }
        }

        public IEnumerable<PluggComponent> GetAllComponentsInPlugg(int pluggId)
        {
            IEnumerable<PluggComponent> pc;
            using (IDataContext ctx = DataContext.Instance())
            {
                var repository = ctx.GetRepository<PluggComponent>();
                pc = repository.Find("WHERE PluggId = @0 ORDER BY ComponentOrder", pluggId);
            }
            return pc;
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

        public PHText GetCurrentVersionText(string cultureCode, int itemId, ETextItemType itemType)
        {
            IEnumerable<PHText> txt;
            PHText theText = null;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<PHText>();
                txt = rep.Find("WHERE CultureCode = @0 AND ItemId = @1 AND ItemType = @2 AND CurrentVersion = 'True'", cultureCode, itemId, (int)itemType);
            }

            if (txt.Any())
                theText = txt.First(); //Can only be at most one. 

            return theText;
        }

        public IEnumerable<PHText> GetAllVersionsText(string cultureCode, int itemId, ETextItemType itemType)
        {
            IEnumerable<PHText> txt;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<PHText>();
                txt = rep.Find("WHERE CultureCode = @0 AND ItemId = @1 AND ItemType = @2 ORDER BY Version", cultureCode, itemId, (int)itemType);
            }

            return txt;
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

        public void DeleteAllPhTextForItem(int itemId, ETextItemType itemType)
        {
            using (IDataContext context = DataContext.Instance())
            {
                var rec = context.ExecuteQuery<PHText>(CommandType.Text,
                    "DELETE FROM PHTexts WHERE ItemId=@0 AND ItemType=@1", itemId, (int)itemType);
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

        public PHLatex GetCurrentVersionLatexText(string cultureCode, int itemId, ELatexItemType itemType)
        {
            IEnumerable<PHLatex> txt;
            PHLatex theText = null;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<PHLatex>();
                txt = rep.Find("WHERE CultureCode = @0 AND ItemId = @1 AND ItemType = @2 AND CurrentVersion = 'True'", cultureCode, itemId, (int)itemType);
            }

            if (txt.Any())
                theText = txt.First(); //Can only be at most one. PetaPoco does not handle composite key

            return theText;
        }

        public IEnumerable<PHLatex> GetAllVersionsLatexText(string cultureCode, int itemId, ELatexItemType itemType)
        {
            IEnumerable<PHLatex> txt;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<PHLatex>();
                txt = rep.Find("WHERE CultureCode = @0 AND ItemId = @1 AND ItemType = @2 ORDER BY Version", cultureCode, itemId, (int)itemType);
            }

            return txt;
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

        public void DeleteAllLatexForItem(int itemId, ELatexItemType itemType)
        {
            using (IDataContext context = DataContext.Instance())
            {
                var rec = context.ExecuteQuery<PHText>(CommandType.Text,
                    "DELETE FROM PHLatex WHERE ItemId=@0 AND ItemType=@1", itemId, (int)itemType);
            }
        }

        #endregion

        #region YouTube

        public void CreateYouTube(YouTube p)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<YouTube>();
                rep.Insert(p);
            }
        }

        public YouTube GetYouTube(int youTubeId)
        {
            YouTube p;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<YouTube>();
                p = rep.GetById(youTubeId);
            }
            return p;
        }

        public YouTube GetYouTubeByComponentId(int componentId)
        {
            IEnumerable<YouTube> yt;
            YouTube theYt = null;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<YouTube>();
                yt = rep.Find("WHERE PluggComponentId = @0", componentId);
            }

            if (yt.Any())
                theYt = yt.First(); //Can only be at most one. 

            return theYt;
        }

        public void UpdateYouTube(YouTube p)
        {
            using (IDataContext db = DataContext.Instance())
            {
                var rep = db.GetRepository<YouTube>();
                rep.Update(p);
            }
        }

        public void DeleteYouTube(YouTube p)
        {
            using (IDataContext db = DataContext.Instance())
            {
                var rep = db.GetRepository<YouTube>();
                rep.Delete(p);
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

        #region CoursePlugg

        public void CreateCoursePlugg(CoursePluggEntity t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<CoursePluggEntity>();
                rep.Insert(t);
            }
        }

        public void UpdateCoursePlugg(CoursePluggEntity t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<CoursePluggEntity>();
                rep.Update(t);
            }
        }

        public void DeleteCoursePlugg(CoursePluggEntity t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<CoursePluggEntity>();
                rep.Delete(t);
            }
        }

        public CoursePluggEntity GetCoursePlugg(int CoursePluggId)
        {
            CoursePluggEntity p;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<CoursePluggEntity>();
                p = rep.GetById(CoursePluggId);
            }
            return p;
        }

        public List<CoursePlugg> GetPluggsInCourse(int courseId, string ccCode)
        {
            List<CoursePlugg> cps = new List<CoursePlugg>();

            using (IDataContext context = DataContext.Instance())
            {
                string sqlPlugg = "SELECT Text AS label, CoursePluggId, CourseId, CoursePluggs.PluggId, CPOrder, MotherId, CoursePluggs.CreatedOnDate, CoursePluggs.CreatedByUserId FROM CoursePluggs INNER JOIN Pluggs ON CoursePluggs.PluggID=Pluggs.PluggId INNER JOIN PHTexts ON Pluggs.PluggId=PHTexts.ItemId WHERE CourseId=" + courseId + " AND ItemType=" + (int)ETextItemType.PluggTitle + " AND CultureCode='" + ccCode + "' ORDER BY CPOrder";
                var rec = context.ExecuteQuery<CoursePlugg>(CommandType.Text, sqlPlugg);
                foreach (var cp in rec)
                {
                    cps.Add(new CoursePlugg { CoursePluggId = cp.CoursePluggId, CourseId = cp.CourseId, PluggId = cp.PluggId, CPOrder = cp.CPOrder, MotherId = cp.MotherId, label = cp.label, CreatedOnDate = cp.CreatedOnDate, CreatedByUserId = cp.CreatedByUserId  });
                }
            }
            return cps;
        }

        public IEnumerable<CoursePluggEntity> GetChildrenCP(int motherId)
        {
            IEnumerable<CoursePluggEntity> cps;
            using (IDataContext ctx = DataContext.Instance())
            {
                var repository = ctx.GetRepository<CoursePluggEntity>();
                cps = repository.Find("WHERE MotherId=" + motherId + " ORDER BY CPOrder");
            }
            return cps;
        }

        ////The same item may go into a course several times so collection is correct
        //public IEnumerable<CoursePlugg> GetCourseItems(int courseId, int pluggId)
        //{
        //    IEnumerable<CoursePlugg> cis;
        //    using (IDataContext ctx = DataContext.Instance())
        //    {
        //        var rep = ctx.GetRepository<CoursePlugg>();
        //        cis = rep.Find("WHERE CourseId = @0 AND PluggId = @1", courseId, pluggId);
        //    }
        //    return cis;
        //}

        #endregion

        #region Subjects

        public void CreateSubject(SubjectEntity s)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<SubjectEntity>();
                rep.Insert(s);
            }
        }

        public SubjectEntity GetSubject(int subjectId)
        {
            SubjectEntity s;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<SubjectEntity>();
                s = rep.GetById(subjectId);
            }
            return s;
        }

        public void UpdateSubject(SubjectEntity s)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<SubjectEntity>();
                rep.Update(s);
            }
        }

        public void DeleteSubject(SubjectEntity s)
        {
            using (IDataContext db = DataContext.Instance())
            {
                var rep = db.GetRepository<SubjectEntity>();
                rep.Delete(s);
            }
        }

        public List<Subject> GetAllSubjects(string ccCode)
        {
            List<Subject> cps = new List<Subject>();

            using (IDataContext context = DataContext.Instance())
            {
                string sqlPlugg = "SELECT Text AS label, SubjectId, SubjectOrder, MotherId FROM Subjects INNER JOIN PHTexts ON Subjects.SubjectId=PHTexts.ItemId WHERE ItemType=" + (int)ETextItemType.Subject + " AND CultureCode='" + ccCode + "' ORDER BY SubjectOrder";
                var rec = context.ExecuteQuery<Subject>(CommandType.Text, sqlPlugg);
                foreach (var cp in rec)
                {
                    cps.Add(new Subject { SubjectId = cp.SubjectId, SubjectOrder = cp.SubjectOrder, MotherId = cp.MotherId, label = cp.label });
                }
            }
            return cps;
        }

        public IEnumerable<SubjectEntity> GetChildrenSubjects(int motherId)
        {
            IEnumerable<SubjectEntity> sublist;
            using (IDataContext ctx = DataContext.Instance())
            {
                var repository = ctx.GetRepository<SubjectEntity>();
                sublist = repository.Find("WHERE MotherId=" + motherId + " ORDER BY SubjectOrder");
            }
            return sublist;
        }

        #endregion

        //#region Other

        ////public List<PluggInfoForDNNGrid> GetPluggRecords(string cultureCode)
        ////{
        ////    List<PluggInfoForDNNGrid> pluggs = new List<PluggInfoForDNNGrid>();
        ////    using (IDataContext ctx = DataContext.Instance())
        ////    {
        ////        var rec = ctx.ExecuteQuery<PluggInfoForDNNGrid>(CommandType.Text, "SELECT PluggId, Text, Username FROM pluggs JOIN Users ON users.UserID=Pluggs.CreatedByUserId JOIN PHTexts ON ItemId=Pluggs.PluggId WHERE ItemType=@0 AND CultureCode=@1" , (int)ETextItemType.PluggTitle , cultureCode);

        ////        foreach (var item in rec)
        ////        {
        ////            pluggs.Add(new PluggInfoForDNNGrid { PluggId = item.PluggId, PluggName = item.PluggName , UserName = item.UserName });
        ////        }
        ////    }

        ////    return pluggs;
        ////}

        //public IEnumerable<PluggInfoForDNNGrid> GetPluggRecords(string cultureCode)
        //{
        //    IEnumerable<PluggInfoForDNNGrid> pluggs;
        //    using (IDataContext ctx = DataContext.Instance())
        //    {
        //        pluggs = ctx.ExecuteQuery<PluggInfoForDNNGrid>(CommandType.Text, "SELECT PluggId, Text, Username FROM Pluggs JOIN Users ON Users.UserID=Pluggs.CreatedByUserId JOIN PHTexts ON ItemId=Pluggs.PluggId WHERE ItemType=" + (int)ETextItemType.PluggTitle + " AND CultureCode='" + cultureCode + "'");
        //    }
        //    return pluggs;
        //}

        ////CourseForDNN

        //public List<CourseInfoForDNNGrid> GetCoursesForDNN()
        //{
        //    List<CourseInfoForDNNGrid> cs = new List<CourseInfoForDNNGrid>();
        //    using (IDataContext ctx = DataContext.Instance())
        //    {
        //        var rec = ctx.ExecuteQuery<CourseInfoForDNNGrid>(CommandType.TableDirect, @"select CourseId, Title as CourseName, Username from courses join Users on Users.UserID=Courses.CreatedByUserId ");

        //        foreach (var item in rec)
        //        {
        //            cs.Add(new CourseInfoForDNNGrid() { CourseId = item.CourseId, CourseName = item.CourseName, UserName = item.UserName });
        //        }
        //    }

        //    return cs;
        //}

        //#endregion

        //#region CourseHeading
        //public CourseMenuHeadings CreateHeading(CourseMenuHeadings t)
        //{
        //    using (IDataContext ctx = DataContext.Instance())
        //    {
        //        var rep = ctx.GetRepository<CourseMenuHeadings>();
        //        rep.Insert(t);
        //    }
        //    return t;
        //}


        //public void UpdateHeading(CourseMenuHeadings t)
        //{
        //    using (IDataContext ctx = DataContext.Instance())
        //    {
        //        var rep = ctx.GetRepository<CourseMenuHeadings>();
        //        rep.Update(t);
        //    }
        //}

        //public void DeleteHeading(CourseMenuHeadings t)
        //{
        //    using (IDataContext ctx = DataContext.Instance())
        //    {
        //        var rep = ctx.GetRepository<CourseMenuHeadings>();
        //        rep.Delete(t);
        //    }
        //}
        //#endregion


        //#region CourseItemComment
        //public IEnumerable<CourseItemComment> GetCourseItemComment(int courseId, int itemId)
        //{
        //    IEnumerable<CourseItemComment> cic;
        //    using (IDataContext ctx = DataContext.Instance())
        //    {
        //        var rep = ctx.GetRepository<CourseItemComment>();
        //        cic = rep.Find("WHERE CourseId = @0 AND ItemId = @1", courseId, itemId);
        //    }
        //    return cic;
        //}


        //public void UpdateCourseItemComment(CourseItemComment CIC)
        //{
        //    using (IDataContext db = DataContext.Instance())
        //    {
        //        var rep = db.GetRepository<CourseItemComment>();
        //        rep.Update(CIC);
        //    }
        //}
        //#endregion

        public IEnumerable<PluggInfoForPluggList> GetAllPhtext(string _key, string cultureCode)
        {
            using (IDataContext context = DataContext.Instance())
            {
                return context.ExecuteQuery<PluggInfoForPluggList>(CommandType.StoredProcedure,
                    "sp_searchResult", _key, cultureCode);
            }
        }

        public IEnumerable<PluggInfoForPluggList> GetAllPluggs(string cultureCode)
        {
            using (IDataContext context = DataContext.Instance())
            {
                return context.ExecuteQuery<PluggInfoForPluggList>(CommandType.StoredProcedure,
                    "GetAll_Pluggs", cultureCode);
            }
        }
    }
}