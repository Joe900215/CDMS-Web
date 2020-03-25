using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using AVEVA.CDMS.Server;
using AVEVA.CDMS.Common;
using AVEVA.CDMS.WebApi;
using System.Runtime.Serialization;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Data.SQLite;
//using LinqToDB;

namespace AVEVA.CDMS.HXPC_Plugins
{
    public class Company
    {
        /// <summary>
        /// 新建厂家资料目录时，获取默认值
        /// </summary>
        /// <param name="sid">链接密钥</param>
        /// <param name="ProjectKeyword">厂家文件夹关键字</param>
        /// <returns></returns>
        public static JObject GetEditCompanyDefault(string sid, string ProjectKeyword)
        {
            ExReJObject reJo = new ExReJObject();

            try
            {

                User curUser = DBSourceController.GetCurrentUser(sid);
                if (curUser == null)
                {
                    reJo.msg = "登录验证失败！请尝试重新登录！";
                    return reJo.Value;
                }

                DBSource dbsource = curUser.dBSource;
                if (dbsource == null)
                {
                    reJo.msg = "登录验证失败！请尝试重新登录！";
                    return reJo.Value;
                }

                Project m_prj = dbsource.GetProjectByKeyWord(ProjectKeyword);
                if (m_prj == null)
                {
                    reJo.msg = "参数错误，目录不存在！";
                    return reJo.Value;
                }

                //获取所有厂家信息
                TempDefn mTempDefn = GetTempDefn(dbsource,"FAXCOMPANY");
                if (mTempDefn == null)
                {
                    reJo.msg = "获取厂家信息失败，请联系管理员！";
                    return reJo.Value;
                }


                //查找所有厂家目录
                var cpList = m_prj.dBSource.SelectProject("select * from CDMS_Project where o_DefnID=" + mTempDefn.DefnID.ToString());
                List<Project> companyPList = new List<Project>();
                if (cpList != null)
                {
                    foreach (Project p in cpList)
                    {
                        var pp = companyPList.Find(ppp => ppp.O_projectname == p.O_projectname);
                        if (pp == null)
                        {
                            companyPList.Add(p);
                        }
                    }
                }

                JArray jaData = new JArray();
                //加载数据
                int row = 0;
                foreach (Project p in companyPList)
                {

                    JObject joData = new JObject();
                    //int index = dgvCompany.Rows.Add(row + 1);
                    int cell = -1;
                    foreach (AttrData ad in p.AttrDataList)
                    {
                        if (cell > -1 && cell < 9)
                        {
                            //转换传递的参数名
                            string strName = "";
                            switch (ad.TempDefn.Code)
                            {
                                case "FC_COMPANYCODE":
                                    strName = "companyCode";
                                    break;
                                case "FC_COMPANYCHINESE":
                                    strName = "companyChinese";
                                    break;
                                case "FC_ADDRESS":
                                    strName = "address";
                                    break;
                                case "FC_PROVINCE":
                                    strName = "province";
                                    break;
                                case "FC_POSTCODE":
                                    strName = "postCode";
                                    break;
                                case "FC_EMAIL":
                                    strName = "eMail";
                                    break;
                                case "FC_RECEIVER":
                                    strName = "receiver";
                                    break;
                                case "FC_FAXNO":
                                    strName = "faxNo";
                                    break;
                                case "FC_PHONE":
                                    strName = "phone";
                                    break;
                            }
                            if (!string.IsNullOrEmpty(strName))
                            {
                                joData.Add(strName, ad.Code);
                            }

                        }
                        cell++;
                    }
                    jaData.Add(joData);
                    row++;

                }
                reJo.data = new JArray(new JObject(new JProperty("CompanyList",jaData)));
                reJo.success = true;
                return reJo.Value;

            }
            catch (Exception e)
            {
                reJo.msg = e.Message;
                CommonController.WebWriteLog(reJo.msg);
            }

            return reJo.Value;
        }


        /// <summary>
        /// 修改厂家资料目录
        /// </summary>
        /// <param name="sid">链接密钥</param>
        /// <param name="ProjectKeyword">厂家文件夹关键字</param>
        /// <param name="projectAttrJson">参数字符串，详见下面例子例子</param>
        /// <returns>
        /// <para>projectAttrJson 例子：</para>
        /// <code>
        /// [
        ///     { name: 'companyCode', value: companyCode },    //厂家编码
        ///     { name: 'companyChinese', value: companyChinese },  //厂家名称
        ///     { name: 'address', value: address },    //厂家地址
        ///     { name: 'province', value: province },  //厂家省份
        ///     { name: 'postCode', value: postCode },  //厂家邮政编码
        ///     { name: 'eMail', value: eMail },    //厂家邮箱
        ///     { name: 'receiver', value: receiver },  //厂家收件人
        ///     { name: 'faxNo', value: faxNo },    //厂家传真号
        ///     { name: 'phone', value: phone },    //收件人电话
        /// ];
        /// </code>
        /// </returns>
        public static JObject EditCompany(string sid, string ProjectKeyword, string projectAttrJson) {
            ExReJObject reJo = new ExReJObject();

            try
            {

                User curUser = DBSourceController.GetCurrentUser(sid);
                if (curUser == null)
                {
                    reJo.msg = "登录验证失败！请尝试重新登录！";
                    return reJo.Value;
                }

                DBSource dbsource = curUser.dBSource;
                if (dbsource == null)
                {
                    reJo.msg = "登录验证失败！请尝试重新登录！";
                    return reJo.Value;
                }

                Project m_prj = dbsource.GetProjectByKeyWord(ProjectKeyword);
                if (m_prj == null)
                {
                    reJo.msg = "参数错误，目录不存在！";
                    return reJo.Value;
                }


                #region 获取传递过来的属性参数
                //获取传递过来的属性参数
                JArray jaAttr = (JArray)JsonConvert.DeserializeObject(projectAttrJson);

                string strCompanyCode = "", strCompanyChinese = "",
                    strAddress = "", strProvince = "",
                    strPostCode = "", strEMail = "",
                    strReceiver = "", strFaxNo = "", strPhone = "";

                foreach (JObject joAttr in jaAttr)
                {
                    string strName = joAttr["name"].ToString();
                    string strValue = joAttr["value"].ToString();

                    switch (strName)
                    {
                        case "companyCode":
                            strCompanyCode = strValue;
                            break;
                        case "companyChinese":
                            strCompanyChinese = strValue;
                            break;
                        case "address":
                            strAddress = strValue;
                            break;
                        case "province":
                            strProvince = strValue;
                            break;
                        case "postCode":
                            strPostCode = strValue;
                            break;
                        case "eMail":
                            strEMail = strValue;
                            break;
                        case "receiver":
                            strReceiver = strValue;
                            break;
                        case "faxNo":
                            strFaxNo = strValue;
                            break;
                        case "phone":
                            strPhone = strValue;
                            break;
                    }
                }
                #endregion


                if (m_prj.ChildProjectList.Find(px => px.Code == strCompanyCode) != null)
                {
                    //MessageBox.Show("已经存在相同版本的目录，请返回重试！");
                    reJo.msg = "已经存在相同版本的目录，请返回重试！";
                    return reJo.Value;
                }

                //获取所有厂家信息
                TempDefn mTempDefn = GetTempDefn(dbsource,"FAXCOMPANY");
                if (mTempDefn == null)
                {
                    //MessageBox.Show("获取厂家信息失败，请联系管理员！");
                    reJo.msg = "获取厂家信息失败，请联系管理员！";
                    return reJo.Value;
                }

                Project project = m_prj.NewProject(strCompanyCode, strCompanyChinese, m_prj.Storage, mTempDefn);
                if (project == null)
                {
                    //MessageBox.Show("新建版本目录失败，请联系管理员！");
                    reJo.msg = "新建版本目录失败，请联系管理员！";
                    return reJo.Value;
                }

                //增加附加属性
                try
                {
                    project.GetAttrDataByKeyWord("FC_COMPANYCODE").SetCodeDesc(strCompanyCode);       //厂家编码
                    project.GetAttrDataByKeyWord("FC_COMPANYCHINESE").SetCodeDesc(strCompanyChinese);    //厂家名称
                    project.GetAttrDataByKeyWord("FC_ADDRESS").SetCodeDesc(strAddress);           //厂家地址
                    project.GetAttrDataByKeyWord("FC_PROVINCE").SetCodeDesc(strProvince);          //厂家省份
                    project.GetAttrDataByKeyWord("FC_POSTCODE").SetCodeDesc(strPostCode);          //厂家邮政
                    project.GetAttrDataByKeyWord("FC_EMAIL").SetCodeDesc(strEMail);             //厂家邮箱
                    project.GetAttrDataByKeyWord("FC_RECEIVER").SetCodeDesc(strReceiver);          //厂家收件人
                    project.GetAttrDataByKeyWord("FC_FAXNO").SetCodeDesc(strFaxNo);             //厂家传真号
                    project.GetAttrDataByKeyWord("FC_PHONE").SetCodeDesc(strPhone);             //收件人电话
                    project.AttrDataList.SaveData();
                }
                catch (Exception ex)
                {
                    //MessageBox.Show("获取厂家模板失败，请联系管理员！");
                    reJo.msg = "获取厂家模板失败，请联系管理员！";
                    return reJo.Value;
                }

                //base.Close();
                //if (ExMenu.callTheApp != null)
                //{
                //    CallBackParam param = new CallBackParam
                //    {
                //        callType = enCallBackType.UpdateDBSource,
                //        dbs = this.m_dbs
                //    };
                //    CallBackResult result = null;
                //    ExMenu.callTheApp(param, out result);
                //}


                reJo.data = new JArray(new JObject(new JProperty("ProjectKeyword", project.KeyWord)));

                reJo.success = true;
                return reJo.Value;

            }
            catch (Exception e)
            {
                reJo.msg = e.Message;
                CommonController.WebWriteLog(reJo.msg);
            }

            return reJo.Value;
        }

        /// <summary>
        /// //获取所有厂家信息
        /// </summary>
        /// <param name="td"></param>
        private static TempDefn GetTempDefn(DBSource m_dbs,string keyword)
        {
            List<TempDefn> mTempDefnList = m_dbs.GetTempDefnByCode(keyword);
            if (mTempDefnList != null && mTempDefnList.Count > 0)
            {
                return mTempDefnList[0];
            }

            return null;
        }
    }


}
