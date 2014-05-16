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
using DotNetNuke.Common.Utilities;
using DotNetNuke.Data;
using System.Data;
using DotNetNuke.Entities.Users;

namespace Plugghest.Subjects
{
    public class SubjectController
    {
        //insert on subject
        public void CreateSubject(Subject t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Subject>();
                rep.Insert(t);
            }
        }
        
        public Subject GetSubject(int subjectId)
        {
            Subject p;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Subject>();
                p = rep.GetById(subjectId);
            }
            return p;
        }
        
        public void UpdateItem(Subject t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Subject>();
                rep.Update(t);
            }
        }

        public IEnumerable<Subject> GetAllSubjects()
        {
            IEnumerable<Subject> objsubjectitem;
            using (IDataContext ctx = DataContext.Instance())
            {
                var repository = ctx.GetRepository<Subject>();
                objsubjectitem = repository.Find("ORDER BY SubjectOrder");
            }
            return objsubjectitem;
        }

        public IEnumerable<Subject> GetSubjectsFromMotherWhereOrderGreaterThan(int? mother, int order)
        {
            IEnumerable<Subject> sublist;
            using (IDataContext ctx = DataContext.Instance())
            {
                var repository = ctx.GetRepository<Subject>();
                if (mother == null)
                    sublist = repository.Find("WHERE MotherId IS NULL AND SubjectOrder >" + order + " ORDER BY SubjectOrder");
                else
                    sublist = repository.Find("WHERE MotherId=" + mother + "AND SubjectOrder >" + order + " ORDER BY SubjectOrder");
            }
            return sublist;
        }

    }
}
