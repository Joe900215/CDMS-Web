using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using AVEVA.CDMS.Server;
using System.Runtime.Serialization;
using System.Text;
using System.Net;
using System.Text.RegularExpressions;


namespace AVEVA.CDMS.WebApi
{
    //目录或者文档的属性(在模板中定义) 
    public class AttrDataController : Controller
    {
        
        /// <summary>
        /// 获取属性列表
        /// </summary>
        /// <param name="attrDataList">属性列表</param>
        /// <param name="objAttrEditRight">DOC或Project是否有编辑属性权限</param>
        /// <returns>返回一个JSON对象JObject</returns>
        internal static JArray GetAttrDataListJson(AttrDataList attrDataList,bool objAttrEditRight)
        {
            JArray jaResult = new JArray();
            try
            {
                if (attrDataList != null && attrDataList.Count() > 0)
                {
                    foreach (AttrData ad in attrDataList)
                    {
                        JArray jaShowData = new JArray();
                        List<String> adsdList = ad.ShowDataList;
                        if (adsdList.Count > 0)
                        {
                            foreach (string dataItem in adsdList)
                            {
                                jaShowData.Add(new JObject 
                                { 
                                      new JProperty("text",dataItem),
                                      new JProperty("value",dataItem),
                                 });
                            }
                        }

                        jaResult.Add(new JObject 
                        { 
                             new JProperty("AttrCode", ad.TempDefn.Code),
                             new JProperty("TempAttrType",ad.TempDefn.Attr_type),//属性类型
                             new JProperty("DataType",ad.TempDefn.Data_Type),//属性类型
                             new JProperty("DefaultCode",ad.TempDefn.DefaultCode),//编辑属性的默认值
                             new JProperty("ShowData",jaShowData),//编辑属性的代码 
                             new JProperty("AttrName", string.IsNullOrEmpty(ad.TempDefn.Description) ? ad.TempDefn.Code : ad.TempDefn.Description),
                             new JProperty("AttrValue", ad.ToString ),
                             new JProperty("AttrType","AddiAttr"),
                             new JProperty("CanEdit",ad.CanEdit && objAttrEditRight ?"True":"False"),
                             new JProperty("Visible","True"),
                        });
                    }
                }
            }
            catch (Exception e)
            {
                CommonController.WebWriteLog(e.Message);
            }
            return jaResult;
        }

        //修改目录或文档的附加属性
        //参数：attrDataList:属性所在的属性列表
        //TempDefnCode:属性模板代码
        //attrDataValue：新的属性值
        internal static bool UpdateAttrData(AttrDataList attrDataList, string TempDefnCode, string attrDataValue)
        {
            JArray jaResult = new JArray();
            bool reBool = false;
            try
            {
                //修改附加属性
                AttrData attrData = null;
                if (attrDataList != null && attrDataList.Count > 0)
                {
                    foreach (AttrData attr in attrDataList)
                    {
                        if (attr.TempDefn != null && (attr.TempDefn.Code) == TempDefnCode)
                        {
                            attrData = attr;
                            break;
                        }
                    }
                }

                #region 格式化输入的值
                    #region 如果是用户属性，且不能多选，就去掉多出来的用户
                if (attrData.TempDefn.Attr_type == enAttrType.User && attrData.TempDefn.MultiValue == false && attrDataValue.IndexOf(",") > 0)
                {
                    string[] attrDataValueAry=attrDataValue.Split(new char[] { ',' });
                    attrDataValue = attrDataValueAry[0];
                }
                    #endregion
                #endregion

                if (attrData != null)
                {
                    attrData.SetCodeDesc(attrDataValue);

                    if (attrDataList != null)
                        attrDataList.SaveData();
                    reBool = true;
                }
            }
            catch (Exception e)
            {
                CommonController.WebWriteLog(e.Message);
            }
            return reBool;
        }
    }
}
