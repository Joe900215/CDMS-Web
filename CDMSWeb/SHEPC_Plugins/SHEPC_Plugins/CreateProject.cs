using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AVEVA.CDMS.Server;
using AVEVA.CDMS.WebApi;

namespace AVEVA.CDMS.SHEPC_Plugins
{
    internal class CreateProjectMenu : ExWebMenu
    {
        private Project SelectedProject;

        private void GetSelectedProject()
        {
            if ((base.SelProjectList != null) && (base.SelProjectList.Count > 0))
            {
                if (base.SelProjectList[0].ShortProject != null)
                {
                    this.SelectedProject = base.SelProjectList[0].ShortProject;
                }
                else
                {
                    this.SelectedProject = base.SelProjectList[0];
                }
            }
        }

        public override enWebMenuState MeasureMenuState()
        {
            enWebMenuState hide = enWebMenuState.Hide;
            try
            {
                this.GetSelectedProject();
                if (((this.SelectedProject != null) && (this.SelectedProject.ParentProject == null)) && (string.IsNullOrEmpty(this.SelectedProject.O_projectcode) && (this.SelectedProject.dBSource.LoginUser.Code.ToUpper() == "ADMIN")))
                {
                    hide = enWebMenuState.Enabled;
                }
            }
            catch { }
            return hide;
        }
    }
}
