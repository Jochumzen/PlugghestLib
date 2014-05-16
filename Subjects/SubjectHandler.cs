using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Plugghest.Subjects
{
    public class SubjectHandler
    {
        SubjectController subjectcntr = new SubjectController();

        public IEnumerable<Subject> GetAllSubjects()
        {
            return subjectcntr.GetAllSubjects();
        }

        public void CreateSubject(Subject t)
        {
            subjectcntr.CreateSubject(t);
        }

        public void UpdateSubject(Subject t)
        {
            subjectcntr.UpdateItem(t);
        }

        public Subject GetSubject(int subjectId)
        {
            return subjectcntr.GetSubject(subjectId);
        }

        public IList<Subject> FlatToHierarchy(IEnumerable<Subject> list, int motherId = 0)
        {
            return (from i in list
                    where i.MotherId == motherId
                    select new Subject
                    {
                        SubjectId = i.SubjectId,
                        SubjectOrder = i.SubjectOrder,
                        MotherId = i.MotherId,
                        label = i.label,
                        Mother = i,
                        children = FlatToHierarchy(list, i.SubjectId)
                    }).ToList();
        }

        public IList<Subject> GetSubjectsAsTree()
        {
            IEnumerable<Subject> source = GetAllSubjects();
            return FlatToHierarchy(source);
        }

    }
}
