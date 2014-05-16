using DotNetNuke.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Plugghest.DNN
{
    public class TabUrlController
    {
        public void CreateTabUrl(TabUrl t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<TabUrl>();
                rep.Insert(t);
            }
        }
    }

    public class RnCController
    {
        public void CreateCommentObject(DCC_PRC_CommentObject t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<DCC_PRC_CommentObject>();
                rep.Insert(t);
            }
        }
    }
}
