using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVEVA.CDMS.WebApi
{
    public enum enWebTVMenuPosition
    {
        Default = 0,
        TVDocument = 1,     //文档管理页
        TVLogicProject = 2, //逻辑目录页
        TVUserWorkSpace = 3,    //个人工作台页
        TVGlobalSearch = 4,     //全局查询页
        TVUserSearch = 5,       //个人查询页
    }
}
