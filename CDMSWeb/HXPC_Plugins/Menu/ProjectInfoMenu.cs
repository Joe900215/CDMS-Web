using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AVEVA.CDMS.WebApi;
using AVEVA.CDMS.Server;
using System.Collections;

namespace AVEVA.CDMS.HXPC_Plugins
{
    internal class ProjectInfoMenu : ExWebMenu
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
                if (((this.SelectedProject != null) && (this.SelectedProject.ParentProject == null)) && this.SelectedProject.dBSource.LoginUser.IsAdmin && string.IsNullOrEmpty(this.SelectedProject.O_projectcode))
                {
                    hide = enWebMenuState.Enabled;
                }
            }
            catch { }
            return hide;
        }

    }
}