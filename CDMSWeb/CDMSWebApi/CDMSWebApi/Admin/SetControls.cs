using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVEVA.CDMS.WebApi
{
    public class SetControls
    {
        //非零的正整数的正则表达式(From: http://www.itpub.net/redirect.php?tid=965588&goto=lastpost)
        //邮箱的正则表达式(From: http://www.dotnetsky.net/netsave/ShowTopic-48793.html)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sValudatedText">输入的内容</param>
        /// <param name="enRegExp">正则表达式</param>
        /// <returns></returns>
        public static string CheckFormat(string sValudatedText, enRegularExpressions enRegExp)
        {
            //先判断输入的内容是否为空
            if (sValudatedText != string.Empty)
            {
                string sRegExp = "";
                string sMsgInfo = "输入格式不正确";
                if (enRegExp == enRegularExpressions.PlusNum)
                {
                    sRegExp = @"^+?[1-9][0-9]*$";
                    sMsgInfo = "输入的数应为非零的正整数!";
                }
                else if (enRegExp == enRegularExpressions.Email)
                {
                    sRegExp = @"^[\w-]+(\.[\w-]+)*@[\w-]+(\.[\w-]+)+$";
                    sMsgInfo = "输入的邮箱格式不正确!";
                }
                else if (enRegExp == enRegularExpressions.IP)
                {
                    sRegExp = @"\d{0,3}\.\d{0,3}\.\d{0,3}\.\d{0,3}";
                    sMsgInfo = "输入的IP格式不正确!";
                }

                //表示不可变的正则表达式
                System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(sRegExp);

                //表示单个正则表达式匹配的结果
                System.Text.RegularExpressions.Match match = regex.Match(sValudatedText.Trim());

                if (match.Success)
                {
                    return "true";
                }
                //MessageBox.Show(sMsgInfo, "提示", MessageBoxButtons.OK);
                return sMsgInfo;
            }
            return "true";
        }

        /// <summary>
        /// Emial 表示邮箱的正则表达式,PlusNum表示非零正整数的正则表达式

        /// </summary>
        public enum enRegularExpressions { Email, PlusNum, IP };
    }
}
