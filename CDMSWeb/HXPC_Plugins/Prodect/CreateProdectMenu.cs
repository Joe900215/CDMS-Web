using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AVEVA.CDMS.WebApi;
using AVEVA.CDMS.Server;
using System.Collections;

namespace AVEVA.CDMS.HXPC_Plugins
{
    internal class CreateProdectMenu : ExWebMenu
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

                Project project = base.SelProjectList[0];

                if (((project != null) && project.Code == "设计成品" && project.ParentProject != null && (project.ParentProject.TempDefn != null)) && (project.ParentProject.TempDefn.Code == "PROFESSION"))
                {

                    //判断是否为主设或者专业设计人
                    bool flag = false;
                    string valueByKeyWord = project.GetValueByKeyWord("PROFESSIONOWNER");  //主设
                    if (!string.IsNullOrEmpty(valueByKeyWord) && (valueByKeyWord.ToLower() == project.dBSource.LoginUser.Code.ToLower()))
                    {
                        flag = true;
                    }
                    else
                    {
                        valueByKeyWord = project.GetValueByKeyWord("PROFESSIONDESIGN");      //专业设计人
                        if (!string.IsNullOrEmpty(valueByKeyWord) && (valueByKeyWord.ToLower() == project.dBSource.LoginUser.Code.ToLower()))
                        {
                            flag = true;
                        }
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