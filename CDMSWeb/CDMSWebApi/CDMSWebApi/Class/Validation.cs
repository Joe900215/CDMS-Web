using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AVEVA.CDMS.Server;
using System.Collections;

namespace AVEVA.CDMS.WebApi
{
    public class Validation
    {
        /// <summary>
        /// 修改后对象的Code不能与现存的重复
        /// </summary>
        /// <param name="source"></param>
        /// <param name="obj"></param>
        /// <param name="sText"></param>
        /// <returns></returns>
        public static bool ExistsObject(DBSource source, object obj, string sText)
        {
            sText = sText.ToUpper();
            if (obj is DBSource)
            {
                foreach (DBSource s in source.dBManager.AllDBSourceList)
                {
                    if (s.ToString == (obj as DBSource).ToString)
                    {
                        continue;
                    }
                    else
                    {
                        if (s.DBSourceCode.ToUpper() == sText)
                        {
                            return true;
                        }
                    }
                }
            }
            else if (obj is Storage)
            {
                foreach (Storage storage in source.AllStorageList)
                {
                    if ((Storage)obj == storage)
                    {
                        continue;
                    }
                    else
                    {
                        if (storage.Code.ToUpper() == sText)
                        {
                            return true;
                        }
                    }
                }
            }
            else if (obj is User)
            {
                foreach (User user in source.AllUserList)
                {
                    if ((User)obj == user)
                    {
                        continue;
                    }
                    else
                    {
                        if (user.Code.ToUpper() == sText)
                        {
                            return true;
                        }
                    }
                }
            }
            else if (obj is Appl)
            {
                foreach (Appl appl in source.AllApplList)
                {
                    if ((Appl)obj == appl)
                    {
                        continue;
                    }
                    else
                    {
                        if (appl.Code.ToUpper() == sText)
                        {
                            return true;
                        }
                    }
                }
            }
            else if (obj is Group)
            {
                foreach (Group group in source.AllGroupList)
                {
                    if ((Group)obj == group)
                    {
                        continue;
                    }
                    else
                    {
                        if (group.Code.ToUpper() == sText)
                        {
                            return true;
                        }
                    }
                }
            }
            else if (obj is DefWorkFlow)
            {
                foreach (DefWorkFlow defwf in source.AllDefWorkFlowList)
                {
                    if ((DefWorkFlow)obj == defwf)
                    {
                        continue;
                    }
                    else
                    {
                        if (defwf.O_Code.ToUpper() == sText)
                        {
                            return true;
                        }
                    }
                }
            }
            else if (obj is DefWorkState)
            {
                foreach (DefWorkState defstate in source.AllDefWorkStateList)
                {
                    if ((DefWorkState)obj == defstate)
                    {
                        continue;
                    }
                    else
                    {
                        if (defstate.O_Code.ToUpper() == sText)
                        {
                            return true;
                        }
                    }
                }
            }
            else if (obj is ADOCon)
            {
                foreach (DictionaryEntry de in source.AllLocalADOConList)
                {
                    if ((ADOCon)obj == de.Value as ADOCon)
                    {
                        continue;
                    }
                    else
                    {
                        if (((ADOCon)de.Value).ADOConnectCode.ToUpper() == sText)
                        {
                            return true;
                        }
                    }
                }
            }
            else if (obj is DefLogicProject)
            {
                foreach (DefLogicProject defLogProj in source.AllDefLogicProjectList)
                {
                    if ((DefLogicProject)obj == defLogProj)
                    {
                        continue;
                    }
                    else
                    {
                        if (defLogProj.O_Code.ToUpper() == sText)
                        {
                            return true;
                        }
                    }
                }
            }
            else if (obj is DefOutFile)
            {
                foreach (DefOutFile defOutFile in source.AllDefOutFileList)
                {
                    if ((DefOutFile)obj == defOutFile)
                    {
                        continue;
                    }
                    else
                    {
                        if (defOutFile.O_Code.ToUpper() == sText)
                        {
                            return true;
                        }
                    }
                }
            }
            else if (obj is DefExcuteSQL)
            {
                foreach (DefExcuteSQL defExeSQL in source.AllDefExcuteSQLList)
                {
                    if ((DefExcuteSQL)obj == defExeSQL)
                    {
                        continue;
                    }
                    else
                    {
                        if (defExeSQL.O_Code.ToUpper() == sText)
                        {
                            return true;
                        }
                    }
                }
            }
            else if (obj is DefUserMessage)
            {
                foreach (DefUserMessage defUserMsg in source.AllDefUserMessageList)
                {
                    if ((DefUserMessage)obj == defUserMsg)
                    {
                        continue;
                    }
                    else
                    {
                        if (defUserMsg.O_Code.ToUpper() == sText)
                        {
                            return true;
                        }
                    }
                }
            }
            else if (obj is TempDefn)
            {
                foreach (TempDefn tempdefn in source.AllTempDefnList)
                {
                    if ((TempDefn)obj == tempdefn)
                    {
                        continue;
                    }
                    else
                    {
                        if (tempdefn.Code.ToUpper() == sText)
                        {
                            return true;
                        }
                    }
                }
            }
            //}
            return false;
        }
    }
}
