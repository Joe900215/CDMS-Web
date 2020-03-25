using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AVEVA.CDMS.Server;

namespace AVEVA.CDMS.WebApi
{
    /// <summary>
    /// 
    /// </summary>
    public class Global_ProjectFuns
    {
        //判断curProject是否是preProject的子目录或者是更下一层的孙目录等。
        //bool IsProjectUnderAnother(Project preProject , Project curProject)
        public static bool IsProjectUnderAnother(Project preProject, Project curProject)
        {
            try
            {
                if (preProject == null || curProject == null)
                    return false;

                if (preProject.ChildProjectList != null && preProject.ChildProjectList.Count > 0)
                {
                    bool isIn = false;
                    foreach (Project subProject in preProject.ChildProjectList)

                    {
                        if (subProject == curProject)
                        {
                            isIn = true;
                            break;
                        }
                    }

                    if (isIn == true)
                    {
                        return true;
                    }
                    else
                    {

                        foreach (Project subProject in preProject.ChildProjectList)

                        {
                            if (IsProjectUnderAnother(subProject, curProject))
                            {
                                isIn = true;
                                break;
                            }

                        }

                        return isIn;
                    }
                }
            }
            catch (Exception e)
            {
                //2008-11-27
                //::ErrorMessage(e.Message, e.StackTrace);
            }

            return false;
        }

        //判断文档是否在某个Project下
        public static bool IsDocUnderProject(Project project, Doc doc)
        {
            try
            {
                if (project == null || doc == null)
                    return false;
                if (project.DocList != null && project.DocList.Count > 0)
                {
                    foreach (Doc d in project.DocList)

                    {
                        if (d == doc)
                        {
                            return true;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                //2008-11-27
                //::ErrorMessage(e.Message, e.StackTrace);
            }
            return false;
        }

        //在parentProject根据某个Doc对象,创造一个能够用户parentProject的子文档的名字
        public static string CreateCopyDocName(Project parentProject, Doc doc)
        {
            string newName = "";

            try
            {
                if (doc != null && parentProject != null)
                {
                    int count = 2;

                    string tempName = "复件 " + doc.O_itemname;

                    if (parentProject.DocList != null && parentProject.DocList.Count > 0)
                    {
                        while (true)
                        {
                            //判断tempName是否存在parentProject的子文档里面
                            bool bExisted = false;

                            foreach (Doc sDoc in parentProject.DocList)

                            {
                                if (sDoc.O_itemname.Trim().ToUpper() == tempName.Trim().ToUpper())
                                {
                                    bExisted = true;
                                    break;
                                }
                            }

                            //如果已经存在,则需要在换一个名字
                            if (bExisted)
                            {
                                tempName = string.Format("复件({0}) {1}", count++, doc.O_itemname);
                            }
                            else
                            {
                                //该名字可用,退出
                                break;
                            }

                        }
                    }

                    newName = tempName;
                }
            }
            catch (Exception e)
            {
                //2008-11-27
                //::ErrorMessage(e.Message, e.StackTrace);
            }

            return newName;


        }

        public static string CreateCopyDocFileName(Project project, Doc doc)
        {

            string newName="";

            try
            {
                if (doc != null && project != null && !string.IsNullOrEmpty(doc.O_filename))
                {
                    int count = 2;

                    string tempName = "复件 " + doc.O_filename;

                    if (project.DocList != null && project.DocList.Count > 0)
                    {
                        while (true)
                        {
                            //判断tempName是否已经存在
                            bool bExisted = false;

                            foreach (Doc sDoc in project.DocList)

                            {
                                if (sDoc.O_filename.Trim().ToUpper() == tempName.Trim().ToUpper())
                                {

                                    bExisted = true;
                                    break;

                                }
                            }

                            if (bExisted)
                            {
                                tempName = string.Format("复件({0}) {1}", count++, doc.O_filename);
                            }
                            else
                            {
                                //名字可用,退出
                                break;
                            }
                        }

                    }

                    newName = tempName;
                }
            }
            catch (Exception e)
            {
                //2008-11-27
                //::ErrorMessage(e.Message, e.StackTrace);
            }

            return newName;
        }

        //根据name,判断在parentProject下是否有同名Doc的,有则生成新的Doc名字
        public static string CreateNewDocName(Project project, string name)
        {
            string newName = name;

            try
            {
                if (project != null)
                {
                    int count = 2;

                    string tempName = name;

                    if (project.DocList != null && project.DocList.Count > 0)
                    {
                        while (true)
                        {
                            //判断是否已被用
                            bool bExisted = false;

                            foreach (Doc doc in project.DocList)

                            {
                                if (doc.O_itemname.Trim().ToUpper() == tempName.Trim().ToUpper())
                                {
                                    bExisted = true;
                                    break;
                                }
                            }

                            if (bExisted)
                            {
                                tempName = string.Format("{0} ({1})", name, count++);
                            }
                            else
                            {
                                break;
                            }
                        }

                    }

                    newName = tempName;
                }
            }
            catch (Exception e)
            {
                //2008-11-27
                // ::ErrorMessage(e.Message, e.StackTrace);
            }

            return newName;

        }

        //把sourceDoc对应的文件复制一个给destinationDoc对象
        public static bool CopyDocFile(Doc sourceDoc, Doc destinationDoc)
        {
            try
            {
                if (sourceDoc == null || destinationDoc == null || sourceDoc.Storage == null || destinationDoc.Storage == null)
                    return false;

                if (string.IsNullOrEmpty(sourceDoc.O_filename) && string.IsNullOrEmpty(destinationDoc.O_filename))
                    return true;

                //2008-11-26记录文件大小
                destinationDoc.O_size = sourceDoc.O_size;
                destinationDoc.Modify();

                //System.IO.File.Copy(DataFileName, ServerFullFileName);
                string localFilePath = destinationDoc.FullPathFile;
                string remoteFilePath = sourceDoc.FullPathFile;

                if (System.IO.File.Exists(remoteFilePath))
                {
                    if (!System.IO.File.Exists(localFilePath))
                    {
                        //创建目录
                        try
                        {
                            string FullFileName = localFilePath;
                            //取得要保存文件的路径
                            String FileDir = FullFileName.Substring(0, FullFileName.LastIndexOf("\\"));

                            //查看路径是否存在,创建路径
                            if (!System.IO.Directory.Exists(FileDir)) System.IO.Directory.CreateDirectory(FileDir);
                        }
                        catch { }

                        System.IO.File.Copy(remoteFilePath, localFilePath);
                    }
                }

                //string destinationDocPath = (string)Global::localTempPath + ::GetDocPathInCDMS(destinationDoc);

                //string localFilePath = destinationDocPath;
                //string remoteFilePath = CDoc::GetRemoteFilePath(sourceDoc);

                ////获取Doc相应的FTP对象
                //FTPFactory ^ sourceFtp;

                //if (Global::bIsUseRemoting)
                //{
                //    sourceFtp = gcnew FTPFactory(sourceDoc.Storage/*.O_node*/);
                //}
                //else
                //{
                //    if (sourceDoc.Storage.FTP)
                //        sourceFtp = sourceDoc.Storage.FTP;
                //    else
                //        sourceFtp = gcnew FTPFactory(sourceDoc.Storage/*.O_node*/);
                //}

                //FTPFactory ^ destinationFtp;

                //if (Global::bIsUseRemoting)
                //{
                //    destinationFtp = gcnew FTPFactory(destinationDoc.Storage/*.O_node*/);
                //}
                //else
                //{
                //    if (destinationDoc.Storage.FTP)
                //        destinationFtp = destinationDoc.Storage.FTP;
                //    else
                //        destinationFtp = gcnew FTPFactory(destinationDoc.Storage/*.O_node*/);
                //}

                //if (sourceFtp && destinationFtp)
                //{
                //    //先把文件下载到本地
                //    if (sourceFtp.CheckFileIsExit(remoteFilePath))
                //    {
                //        sourceFtp.download(remoteFilePath, localFilePath, false);

                //        //再把文件上传到相应服务器上的目录
                //        remoteFilePath = CDoc::GetRemoteFilePath(destinationDoc);

                //        if (::PathFileExists((string)localFilePath))
                //        {


                //            if (destinationFtp)
                //            {
                //                destinationFtp.upload(localFilePath, remoteFilePath);
                //            }

                //            DeleteFile((string)localFilePath);
                //        }
                //    }

                //    TCHAR tempDir[MAX_PATH];
                //    string strTempDir;

                //    if (::GetTempPath(MAX_PATH, tempDir) > 0)
                //    {
                //        strTempDir = gcnew String(tempDir);
                //    }
                //    else
                //    {
                //        strTempDir = "C:\\";
                //    }

                //    //同时,如果存在附加文件,则也需把附加文件进行复制
                //    if (sourceDoc.AttachFileList!=null && sourceDoc.AttachFileList.Count > 0)
                //    {
                //        foreach(string  sAttachFile in sourceDoc.AttachFileList)


                //{
                //            //先下载,再上传
                //            string sRemoteAttachFilePath = sourceDoc.Project.FullPath + "AttachFiles\\" + sAttachFile;
                //            string localAttachFilePath = strTempDir + sAttachFile;
                //            string dRemoteAttachFilePath = destinationDoc.Project.FullPath + "AttachFiles\\" + sAttachFile;

                //            if (sourceFtp != null && sourceFtp.CheckFileIsExit(sRemoteAttachFilePath))
                //            {
                //                sourceFtp.download(sRemoteAttachFilePath, localAttachFilePath, false);

                //                if (File::Exists(localAttachFilePath) && destinationFtp)
                //                {
                //                    //上传附件到新Doc中
                //                    destinationFtp.upload(localAttachFilePath, dRemoteAttachFilePath, false);

                //                    //上传没有出异常则再把该文件挂到destinationDoc.AttachFileList中去
                //                    destinationDoc.AddAttachFile(sAttachFile);

                //                }

                //            }
                //        }
                //    }


                //    //TIM 2009-07-27  拷贝文件时,需要拷贝参考关系
                //    if (sourceDoc.RefDocList && sourceDoc.RefDocList.Count > 0)
                //    {
                //        foreach(Doc ^ refDoc in sourceDoc.RefDocList)


                //{
                //            destinationDoc.AddRefDoc(refDoc);
                //        }
                //    }


                //    //小黎 2012-7-12 把pdf文件也拷贝下来
                //    string sourcePdfName = sourceDoc.FullPathFile.Substring(0, sourceDoc.FullPathFile.LastIndexOf(".")) + ".pdf";
                //    string destPdfName = destinationDoc.FullPathFile.Substring(0, destinationDoc.FullPathFile.LastIndexOf(".")) + ".pdf";
                //    if (destinationFtp.CheckFileIsExit(sourcePdfName))
                //    {
                //        destinationFtp.RemoteCopyFile(sourcePdfName, destPdfName);
                //    }




                //}

            }
            catch (Exception e)
            {
                //2008-11-27
                //::ErrorMessage(e.Message, e.StackTrace);
                return false;
            }

            return true;
        }

        public static bool CopyProject(Project sourceProject, Project destinationProject, bool bCopyFile, bool bCopySubProject, bool bCopyTempDefn, bool bCopyProperty)
        {
            try
            {

                if (sourceProject == null || destinationProject == null)
                    return false;

                //if (bCopyTempDefn)
                //    /*destinationProject.TempDefn = sourceProject.TempDefn */
                //    ;

                ////2008-11-25
                //if (Global::ydMainFrame)
                //{
                //    Global::ydMainFrame.m_wndStatusBar.SetPaneText(0, (CString)("正在复制 " + sourceProject.ToString + " ..."));

                //    MSG message;
                //    if (::PeekMessage(&message, Global::ydMainFrame.m_hWnd, 0, 0, PM_REMOVE))
                //    {
                // ::TranslateMessage(&message);
                // ::DispatchMessage(&message);
                //    }

                //}


                //在状态栏中显示正在复制那个文件或者目录


                //把用户字段的数据也复制进去
                try
                {
                    destinationProject.O_iuser1 = sourceProject.O_iuser1;
                    destinationProject.O_iuser2 = sourceProject.O_iuser2;
                    destinationProject.O_iuser3 = sourceProject.O_iuser3;
                    destinationProject.O_iuser4 = sourceProject.O_iuser4;
                    destinationProject.O_iuser5 = sourceProject.O_iuser5;
                    destinationProject.O_suser1 = sourceProject.O_suser1;
                    destinationProject.O_suser2 = sourceProject.O_suser2;
                    destinationProject.O_suser3 = sourceProject.O_suser3;
                    destinationProject.O_suser4 = sourceProject.O_suser4;
                    destinationProject.O_suser5 = sourceProject.O_suser5;

                    destinationProject.Modify();
                }
                catch (Exception)
                { }

                //小黎 2011-7-5 修改了destinationProject.TempDefn == sourceProject.TempDefn 为
                // destinationProject.TempDefn.Code == sourceProject.TempDefn.Code

                if (bCopyProperty)
                {

                    if (destinationProject.TempDefn != null && destinationProject.TempDefn.Code == sourceProject.TempDefn.Code)
                    {
                        if (destinationProject.AttrDataList != null && destinationProject.AttrDataList.Count > 0 && sourceProject.AttrDataList != null && sourceProject.AttrDataList.Count > 0)
                        {
                            foreach (AttrData attrData in destinationProject.AttrDataList)

                            {
                                if (attrData.TempDefn == null)
                                    continue;

                                AttrData cAttr = null;

                                foreach (AttrData attr in sourceProject.AttrDataList)

                                {
                                    if (attr.TempDefn != null && attr.TempDefn.Attr_Name == attrData.TempDefn.Attr_Name && attr.TempDefn.Attr_type == attrData.TempDefn.Attr_type && attrData.TempDefn.Data_Type == attr.TempDefn.Data_Type)
                                    {
                                        /*				CString str;
                                                        str.Format( L"属性:%s 值:%s 目录:%s",CString(attr.TempDefn.Attr_Name),CString(attr.ToString),CString(destinationProject.ToString ) ) ;
                                                        AfxMessageBox(str); */

                                        cAttr = attr;
                                        break;
                                    }
                                }

                                if (cAttr != null)
                                {
                                    string str = "";

                                    if (cAttr.TempDefn.MultiValue)
                                    {

                                        if (cAttr.ToStringList != null && cAttr.ToStringList.Count > 0)
                                        {
                                            foreach (string s in cAttr.ToStringList)

                                            {
                                                if (string.IsNullOrEmpty(str))
                                                    str = s;
                                                else
                                                    str = str + "," + s;
                                            }
                                        }
                                        else
                                        {
                                            str = cAttr.ToString;
                                        }
                                    }
                                    else
                                    {
                                        str = cAttr.ToString;
                                    }

                                    attrData.SetCodeDesc(str);
                                }

                            }

                            destinationProject.AttrDataList.SaveData();


                        }
                    }
                }


                // TIM 2009-08-03 加入到TreeView中
                // 复制完自身,则需要把自己加树中,或者更新树中的数据
                //::AddObjFinishedParam * pParam = new ::AddObjFinishedParam();
                //        pParam.bIsDoc = false;
                //        pParam.dBToString = destinationProject.dBSource.ToString;
                //        pParam.iDocID = 0;
                //        pParam.iProID = destinationProject.O_projectno;

                //        if (Global::ydTreeView)
                //            Global::ydTreeView.PostMessage(WM_ADD_OBJECT_FINISHED, (WPARAM)pParam, null);


                if (sourceProject.O_subprojects.ToUpper() == "Y") //sourceProject.ChildProjectList&& sourceProject.ChildProjectList.Count>0)
                {
                    if (bCopySubProject)
                    {
                        foreach (Project childPrj in sourceProject.ChildProjectList)

                        {
                            //在destinationProject中创建新的子Project 并把childPrj复制到新建的project中
                            Project newChildPrj = null;
                            if (bCopyTempDefn != null && childPrj.TempDefn != null)
                                newChildPrj = destinationProject.NewProject(childPrj.O_projectname, childPrj.O_projectdesc, destinationProject.Storage, childPrj.TempDefn, false);
                            else
                                newChildPrj = destinationProject.NewProject(childPrj.O_projectname, childPrj.O_projectdesc, null, null, false);

                            if (newChildPrj != null)
                            {
                                if (!CopyProject(childPrj, newChildPrj, bCopyFile, bCopySubProject, bCopyTempDefn, bCopyProperty))
                                {
                                    /*AfxMessageBox((CString)("复制目录 " + childPrj.ToString + " 时,出现错误,部分对象可能没复制成功。")) ; */
                                    return false;
                                }
                            }

                            newChildPrj.Modify();
                        }
                    }
                }

                if (!bCopyFile)
                    return true;

                if (sourceProject.DocList != null && sourceProject.DocList.Count > 0)
                {
                    foreach (Doc doc in sourceProject.DocList)
                    {
                        if (!CopyDocOnToProject(doc, destinationProject, bCopyTempDefn, bCopyProperty))
                        {
                            //AfxMessageBox((CString)("复制文档 " + doc.ToString + " 时发生错误,复制过程退出!"));
                            return false;
                        }
                    }
                }

            }
            catch (Exception e)
            {       //2008-11-27
                    //  ::ErrorMessage(e.Message, e.StackTrace);
                return false;
            }

            return true;
        }


        public static bool CopyDocOnToProject(Doc sourceDoc, Project project, bool bCopyTempDefn, bool bCopyProperty)
        {
            try
            {
                if (sourceDoc == null || project == null)
                    return false;

                //在状态栏中显示正在复制那个文件或者目录

                //if (Global::ydMainFrame)
                //{
                //    Global::ydMainFrame.m_wndStatusBar.SetPaneText(0, (CString)("正在复制 " + sourceDoc.ToString + " 到 " + project.ToString + "..."));

                //    MSG message;
                //    if (::PeekMessage(&message, Global::ydMainFrame.m_hWnd, 0, 0, PM_REMOVE))
                //    {
                //     ::TranslateMessage(&message);
                //     ::DispatchMessage(&message);
                //    }

                //}

                Doc copyDoc;
                string copyDocName = sourceDoc.O_itemname;
                string copyFileName = sourceDoc.O_filename;

                if (IsDocUnderProject(project, sourceDoc))
                {
                    string newName = CreateCopyDocName(project, sourceDoc);

                    copyDocName = newName;

                    copyFileName = CreateCopyDocFileName(project, sourceDoc);
                }
                else
                {
                    string newName = CreateNewDocName(project, copyDocName);

                    copyDocName = newName;

                }

                if (bCopyTempDefn != null && sourceDoc.TempDefn != null)
                    copyDoc = project.NewDoc(copyFileName, copyDocName, sourceDoc.O_itemdesc, sourceDoc.TempDefn);
                else
                    copyDoc = project.NewDoc(copyFileName, copyDocName, sourceDoc.O_itemdesc);


                if (copyDoc != null)
                {

                    //小黎 2012-7-9 把流程和状态都复制过来 
                    copyDoc.O_dmsstatus = sourceDoc.O_dmsstatus;
                    if (sourceDoc.WorkFlow != null)
                    {
                        copyDoc.WorkFlow = sourceDoc.WorkFlow;
                    }
                    //END


                    //TIM 2010-01-07
                    //if (AVEVA::CDMS::Common::ExploreEvent::OnAfterCreateNewObject)
                    //{
                    //    AVEVA::CDMS::Common::ExploreEvent::OnAfterCreateNewObject(copyDoc);
                    //}

                    //复制的时候,需要复制文档的类型,类型保存在Doc.o_suser1中  2008-11-29
                    if (!string.IsNullOrEmpty(sourceDoc.O_suser1))
                    {
                        copyDoc.O_suser1 = sourceDoc.O_suser1;

                        copyDoc.Modify();
                    }

                    //TIM 2009-12-10
                    try
                    {
                        copyDoc.O_iuser1 = sourceDoc.O_iuser1;
                        copyDoc.O_iuser2 = sourceDoc.O_iuser2;
                        copyDoc.O_iuser3 = sourceDoc.O_iuser3;
                        copyDoc.O_iuser4 = sourceDoc.O_iuser4;
                        copyDoc.O_iuser5 = sourceDoc.O_iuser5;
                        copyDoc.O_suser1 = sourceDoc.O_suser1;
                        copyDoc.O_suser2 = sourceDoc.O_suser2;
                        copyDoc.O_suser3 = sourceDoc.O_suser3;
                        copyDoc.O_suser4 = sourceDoc.O_suser4;
                        copyDoc.O_suser5 = sourceDoc.O_suser5;

                        copyDoc.O_size = sourceDoc.O_size;

                        copyDoc.Modify();

                    }
                    catch (Exception)
                    { }

                    //小黎 修改了copyDoc.TempDefn == sourceDoc.TempDefn 为copyDoc.TempDefn.Code == sourceDoc.TempDefn.Code  2011-7-5
                    //小黎 2012-12-18 增加支持没有模板的文件 && sourceDoc.TempDefn
                    if (bCopyProperty && copyDoc.TempDefn != null && sourceDoc.TempDefn != null)
                    {
                        if (copyDoc.TempDefn.Code == sourceDoc.TempDefn.Code)
                        {
                            if (copyDoc.AttrDataList != null && copyDoc.AttrDataList.Count > 0 && sourceDoc.AttrDataList != null && sourceDoc.AttrDataList.Count > 0)
                            {
                                foreach (AttrData attrData in copyDoc.AttrDataList)

                                {
                                    if (attrData.TempDefn == null)
                                        continue;

                                    AttrData cAttr = null;

                                    foreach (AttrData attr in sourceDoc.AttrDataList)

                                    {
                                        if (attr.TempDefn != null && attr.TempDefn.Attr_Name == attrData.TempDefn.Attr_Name && attr.TempDefn.Attr_type == attrData.TempDefn.Attr_type && attrData.TempDefn.Data_Type == attr.TempDefn.Data_Type)
                                        {
                                            cAttr = attr;
                                            break;
                                        }
                                    }

                                    if (cAttr != null)
                                    {
                                        string str = "";

                                        if (cAttr.TempDefn.MultiValue)
                                        {

                                            if (cAttr.ToStringList != null && cAttr.ToStringList.Count > 0)
                                            {
                                                foreach (string s in cAttr.ToStringList)

                                                {
                                                    if (string.IsNullOrEmpty(str))
                                                        str = s;
                                                    else
                                                        str = str + "," + s;
                                                }
                                            }
                                            else
                                            {
                                                str = cAttr.ToString;
                                            }
                                        }
                                        else
                                        {
                                            str = cAttr.ToString;
                                        }

                                        attrData.SetCodeDesc(str);
                                    }

                                }

                                copyDoc.AttrDataList.SaveData();

                            }
                        }

                    }

                    if (!CopyDocFile(sourceDoc, copyDoc))
                    {
                        //if (Global::ydMainFrame)
                        //    Global::ydMainFrame.m_wndStatusBar.SetPaneText(0, L"");
                        return false;
                    }

                    copyDoc.Modify();

                    //如果粘贴的文档在当前ListView中,则要加到ListView中去
                    //    HTREEITEM hItem = ::GetProjectHTREEITEM(project);
                    //    if (Global::ydTreeView && Global::ydTreeView.m_hCurItem == hItem && hItem)
                    //    {

                    //::ListView_InsertDoc(Global::ydListView, copyDoc);
                    //    }

                }

            }
            catch (Exception e)
            {
                string estr = e.Message;
                //        /*AfxMessageBox(estr) ;*/
                //        if (Global::ydMainFrame)
                //            Global::ydMainFrame.m_wndStatusBar.SetPaneText(0, L"就绪");

                ////2008-11-27
                //::ErrorMessage(e.Message, e.StackTrace);

                return false;
            }

            //if (Global::ydMainFrame)
            //    Global::ydMainFrame.m_wndStatusBar.SetPaneText(0, L"就绪");

            return true;
        }
    }
}