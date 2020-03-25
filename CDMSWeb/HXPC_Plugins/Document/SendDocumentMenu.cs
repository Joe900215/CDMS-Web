using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AVEVA.CDMS.WebApi;
using AVEVA.CDMS.Server;
using System.Collections;

namespace AVEVA.CDMS.HXPC_Plugins
{
    /// <summary>
    /// 专业发文，只有：专业主设或者专业设计人才可以发文
    /// </summary>
    internal class SendDocumentMenu : ExWebMenu
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



                    //查找阶段和专业
                    bool flag = false;
                    while (parentProject != null)
                    {
                        if (parentProject.Code == "收发文")
                        {
                            flag = true;
                        }
                        if ((parentProject.TempDefn != null) && (parentProject.TempDefn.Code == "PROFESSION"))
                        {
                            ProfessionProject = parentProject;
                            break;
                        }
                        parentProject = parentProject.ParentProject;
                    }
                    if (ProfessionProject == null || flag == false)
                    {
                        return enWebMenuState.Hide;
                    }


                    //判断是否为主设或者专业设计人
                    flag = false;
                    string valueByKeyWord = ProfessionProject.GetValueByKeyWord("PROFESSIONOWNER");  //主设
                    if (!string.IsNullOrEmpty(valueByKeyWord) && (valueByKeyWord.ToLower() == project.dBSource.LoginUser.Code.ToLower()))
                    {
                        flag = true;
                    }
                    else
                    {
                        valueByKeyWord = ProfessionProject.GetValueByKeyWord("PROFESSIONDESIGN");      //专业设计人
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