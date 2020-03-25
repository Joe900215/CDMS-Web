using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AVEVA.CDMS.WebApi;
using AVEVA.CDMS.Server;
using System.Collections;

namespace AVEVA.CDMS.HXEPC_Plugins
{
    /// <summary>
    /// 起草认质认价单
    /// </summary>
    internal class DraftVisaMenu : ExWebMenu
    {
        private Project SelectedProject;

        /// <summary>
        /// 决定菜单的状态
        /// </summary>
        /// <returns></returns>
        public override enWebMenuState MeasureMenuState()
        {
            try
            {
                if (base.SelProjectList.Count <= 0)
                {
                    return enWebMenuState.Hide;
                }

                Project project = base.SelProjectList[0];   //选择目录
                Project ProfessionProject = null;           //专业

                if (project != null)
                {
                    Project parentProject = project;

                    bool flag = false;

                    //需要在项目下起草签证
                    Project prjProject = CommonFunction.getParentProjectByTempDefn(project, "HXNY_DOCUMENTSYSTEM");

                    if (prjProject == null)
                    {
                        return enWebMenuState.Hide;
                    }

                    //while (parentProject != null)
                    //{
                    if (project.Code == "签证" || project.Description == "签证")
                        {
                            flag = true;
                        }
                    if (flag)
                    {
                        return enWebMenuState.Enabled;
                    }


                }
            }
            catch { }
            return enWebMenuState.Hide;
        }

    }
}