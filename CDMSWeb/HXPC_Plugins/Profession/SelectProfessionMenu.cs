using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AVEVA.CDMS.WebApi;
using AVEVA.CDMS.Server;
using System.Collections;

namespace AVEVA.CDMS.HXPC_Plugins
{
    internal class SelectProfessionMenu : ExWebMenu
    {
        private Project SelectedProject;

        /// <summary>
        /// 决定菜单的状态
        /// </summary>
        /// <returns></returns>
        public override enWebMenuState MeasureMenuState()
        {
            enWebMenuState hide = enWebMenuState.Hide;
            try
            {
                if (base.SelProjectList.Count <= 0)
                {
                    return enWebMenuState.Hide;
                }

                this.SelectedProject = base.SelProjectList[0];


                if (((this.SelectedProject == null) || (this.SelectedProject.TempDefn == null)) || (!(this.SelectedProject.TempDefn.Code == "DESIGNPROJECT") && !(this.SelectedProject.TempDefn.Code == "DESIGNPHASE")))
                {
                    return hide;
                }



                //如果是设总，也可以启动该菜单
                foreach (Project p in this.SelectedProject.ChildProjectList)
                {
                    AttrData ad = p.GetAttrDataByKeyWord("PROJECTOWNER");//设总
                    if (ad != null && ad.group != null && ad.group.UserList != null && ad.group.UserList.Count > 0 && ad.group.UserList[0].O_userno == this.SelectedProject.dBSource.LoginUser.O_userno && ad.Code == p.dBSource.LoginUser.Code)
                    {
                        return enWebMenuState.Enabled;
                    }
                }


                //系统管理员也可以启动该菜单
                if (this.SelectedProject.dBSource.LoginUser.IsAdmin)
                {
                    return enWebMenuState.Enabled;
                }

                //string str2 = "CONTACTTYPE";
                //if (base.SelDocList != null && base.SelDocList.Count == 1)
                //{
                //    if (base.SelDocList[0].O_filename.ToLower().EndsWith(".thm"))
                //    {
                //        //bool hasRight = WebApi.DocController.ProjectController.GetProjectPCreateRight(base.SelProjectList[0], base.Sid);

                //        //有创建权限才可以创建目录
                //        //if (hasRight)
                //            hide = enWebMenuState.Enabled;
                //    }
                //}
            }
            catch { }
            return hide;
        }

    }
}