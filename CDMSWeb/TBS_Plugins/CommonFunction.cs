using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AVEVA.CDMS.Server;

namespace AVEVA.CDMS.TBSBIM_Plugins
{
    public class CommonFunction
    {
        internal static string GetAttrDataValue(Doc doc,string attrName)
        {
            string result = "";
            if (string.IsNullOrEmpty(attrName)) return result;
            if (doc != null)
            {
                AttrData data;
                //项目名称
                if ((data = doc.GetAttrDataByKeyWord(attrName)) != null)
                {
                    result = data.ToString;
                }
            }
            return result;
        }



        internal static void SetAttrDataValue(Doc doc, string attrName, string attrValue)
        {
            if (doc != null)
            {
                AttrData data;
                //项目名称
                if ((data = doc.GetAttrDataByKeyWord(attrName)) != null)
                {
                    data.SetCodeDesc(attrValue);
                    //doc.AttrDataList.SaveData();
                }
            }
        }

        internal static string GetAttrDataValue(Project proj, string attrName)
        {
            string result = "";
            if (string.IsNullOrEmpty(attrName)) return result;
            if (proj != null)
            {
                AttrData data;
                //项目名称
                if ((data = proj.GetAttrDataByKeyWord(attrName)) != null)
                {
                    result = data.ToString;
                }
            }
            return result;
        }



        internal static void SetAttrDataValue(Project proj, string attrName, string attrValue)
        {
            if (proj != null)
            {
                AttrData data;
                //项目名称
                if ((data = proj.GetAttrDataByKeyWord(attrName)) != null)
                {
                    data.SetCodeDesc(attrValue);
                    //doc.AttrDataList.SaveData();
                }
            }
        }
    }
}
