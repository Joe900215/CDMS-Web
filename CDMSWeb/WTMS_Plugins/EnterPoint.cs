using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using AVEVA.CDMS.Server;
using AVEVA.CDMS.Common;
using AVEVA.CDMS.WebApi;

namespace AVEVA.CDMS.WTMS_Plugins
{
    public class EnterPoint
    {
        public static string PluginName = "WTMS_Plugins";
        public static void Init()
        {
            //记录是否已加载
            bool isLoad = false;

            foreach (WebWorkFlowEvent.Before_WorkFlow_SelectUsers_Event_Class EventClass in WebWorkFlowEvent.ListBeforeWFSelectUsers)
            {
                if (EventClass.PluginName == PluginName)
                {
                    isLoad = true;
                    break;
                }
            }

            if (isLoad == false)
            {

                //添加流程按钮事件处理
                WebWorkFlowEvent.Before_WorkFlow_SelectUsers_Event BeforeWFSelectUsers = new WebWorkFlowEvent.Before_WorkFlow_SelectUsers_Event(AVEVA.CDMS.WTMS_Plugins.EnterPoint.BeforeWF);
                WebWorkFlowEvent.Before_WorkFlow_SelectUsers_Event_Class Before_WorkFlow_SelectUsers_Event_Class = new WebWorkFlowEvent.Before_WorkFlow_SelectUsers_Event_Class();
                Before_WorkFlow_SelectUsers_Event_Class.Event = BeforeWFSelectUsers;
                Before_WorkFlow_SelectUsers_Event_Class.PluginName = PluginName;
                WebWorkFlowEvent.ListBeforeWFSelectUsers.Add(Before_WorkFlow_SelectUsers_Event_Class);

                ////添加文件上传后的事件
                //WebFileEvent.After_Upload_File_Event AfterUploadFileEvent = new WebFileEvent.After_Upload_File_Event(AVEVA.CDMS.ZHEPC_Plugins.EnterPoint.AfterUploadFile);
                //WebFileEvent.After_Upload_File_Event_Class After_Upload_File_Event_Class = new WebFileEvent.After_Upload_File_Event_Class();
                //After_Upload_File_Event_Class.Event = AfterUploadFileEvent;
                //After_Upload_File_Event_Class.PluginName = PluginName;
                //WebFileEvent.ListAfterUploadFile.Add(After_Upload_File_Event_Class);
            }
        }

        public static ExReJObject BeforeWF(string PlugName, WorkFlow wf, WorkStateBranch wsb)
        {
            //当reJo.success=false时，中断流程分支的进行
            ExReJObject reJo = new ExReJObject();
            reJo.success = true;
            return reJo;
        }

        public static List<ExWebMenu> CreateNewExMenu()
        {
            try
            {
                List<ExWebMenu> menuList = new List<ExWebMenu>();


                CreateWorkTaskMenu cwtMenu = new CreateWorkTaskMenu();
                cwtMenu.MenuName = "发文（新建工作任务）";
                cwtMenu.MenuPosition = enWebMenuPosition.TVProject;
                cwtMenu.MenuType = enWebMenuType.Single;

                menuList.Add(cwtMenu);

                //AddCompanyMenu companyMenu = new AddCompanyMenu
                //{
                //    MenuName = "添加参建单位...",
                //    MenuType = enWebMenuType.Single,
                //    MenuPosition = enWebMenuPosition.TVProject

                //};
                //menuList.Add(companyMenu);

                EditCompanyMenu editCompanyMenu = new EditCompanyMenu
                {
                    MenuName = "编辑参建单位..",
                    MenuType = enWebMenuType.Single,
                    MenuPosition = enWebMenuPosition.TVProject

                };
                menuList.Add(editCompanyMenu);

                return menuList;
            }
            catch { }
            return null;
        }

    }
}
