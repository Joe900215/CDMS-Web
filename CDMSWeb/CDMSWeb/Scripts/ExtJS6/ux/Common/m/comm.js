//展开指定Project,
function ExpendProject(ProjectId) {
    var me = this;
    var viewTree = me.view.up('_mainProjectView').down('_mainProjectTree');
    var viewTreeStore = viewTree.store;
    Ext.Ajax.request({
        url: 'WebApi/Post',
        method: "POST",
        params: {
            C: "AVEVA.CDMS.WebApi.ProjectController", A: "GetProjectPath",
            Keyword: ProjectId, sid: localStorage.getItem("sid")
        },
        success: function (response, options) {
            var res = Ext.JSON.decode(response.responseText);

            var record = res.data[0];
            //viewTree.collapseAll();//收缩所有子节点
            viewTree.expandPath(record.ProjectPath);
            //等待上一个函数的执行结果，查找点击树节点
            var count = 0, is_true = false;
            var node = viewTree.store.getNodeById(record.NodeId);

            setInterval(function () {
                if (!count) {
                    node = viewTree.store.getNodeById(record.NodeId);
                    if (Ext.isObject(node)) {
                        is_true = true;
                    }
                    count++;
                }

                if (is_true) {
                    viewTree.getSelectionModel().select(node);
                    viewTree.fireEvent('click', node);
                    is_true = false;
                }
            }, 1000);
        }
    });
};



//显示可用流程选择
//parentFuctionName:上一级函数名称
function showSelectDefWorkFlowWin(view, parentFuctionName, doclist,dwfList, callback_fun) {
    var me = this;
    window.parent.resultvalue="";
        var DwfSelector = Ext.create('Ext.ux.m.WorkFlow._winSelDefWorkFlow', { title: "" });

        winSelDef = Ext.widget('window', {
            title: '选择流程模板',
            closeAction: 'hide',
            width: '95%',
            height: 400,
            minWidth: '95%',
            minHeight: 400,
            layout: 'fit',
            resizable: true,
            modal: true,
            closeAction: 'close', //close 关闭  hide  隐藏  
            //items: form,
            items: DwfSelector,
            defaultFocus: 'firstName'
        });
        winSelDef.show();
        //监听子窗口关闭事件
        winSelDef.on('close', function () {

            var wfKeyword = window.parent.resultvalue;
            if (wfKeyword != "") {
                //me.StartNewWorkFlow(doclist, wfKeyword, "", callback_fun)
                me.StartNewWorkFlow(doclist, wfKeyword, "", callback_fun)
            }

            //// 启动新流程
            //Ext.require('Ext.ux.Common.comm', function () {
            //    StartNewWorkFlow(doclist, wfKeyword, "");
            //})

        });
        if (dwfList === "") {
            DwfSelector._seldefworkflowstore.proxy.extraParams.DocList = doclist;
            DwfSelector._seldefworkflowstore.proxy.extraParams.sid = localStorage.getItem("sid");
            DwfSelector._seldefworkflowstore.load();
        } else {
            for (var i = 0; i < dwfList.length; i++) {
                DwfSelector._seldefworkflowstore.add({ id: dwfList[i].id, text: dwfList[i].text, o_code: dwfList[i].o_code });
            }
            
        }

};
//显示选择用户窗口
//parentFuctionName:上一级函数名称
function showSelectUserWin(parentFuctionName, doclist, wfKeyword, callback_fun) {
    var me = this;
    var UserSelector = Ext.create('Ext.ux.m.User._UserSelector', { title: "" });
        
    _winUserSelector = Ext.widget('window', {
        title: '选择用户或用户组',
        closeAction: 'hide',
        width: '100%',
        height: '100%',
        minWidth: '100%',
        minHeight: '100%',
        layout: 'fit',
        resizable: true,
        modal: true,
        closeAction: 'close', //close 关闭  hide  隐藏  
        //items: form,
        items: UserSelector,
        defaultFocus: 'firstName'
    });
    window.parent.resultvalue = "";
    window.parent.selUser_doclist = doclist;
    window.parent.selUser_wfKeyword = wfKeyword;
    _winUserSelector.show();
    //监听子窗口关闭事件
    _winUserSelector.on('close', function () {
        var seleState = 0;//1:选择了校审人员启动下面的流程，2：没选到人员，选择再次选人，3：没选到人员，不再选人
        if (window.parent.resultvalue != null && window.parent.resultvalue !== "") {
            //选择了校审人员启动下面的流程
            if (parentFuctionName === "GotoNextWorkflowState") {
                //通过流程状态
                me.GotoNextWorkflowState(window.parent.nextWf_btnText, wfKeyword, window.parent.resultvalue, callback_fun);
            }
            else if (parentFuctionName === "StartNewWorkFlow") {
                //启动流程
                Ext.MessageBox.wait("正在提交到下一流程状态，请稍候...", "等待");
                StartNewWorkFlow(window.parent.selUser_doclist, window.parent.selUser_wfKeyword, window.parent.resultvalue, callback_fun);
            }
            else if (parentFuctionName === "getUser") {
                callback_fun();
            }
        }
        else {
            callback_fun();
        }

    });
};

//启动新流程
//参数：doclist:文档列表，wfKeyword：流程关键字（新建指定的流程），userlist：用户列表，callback_fun：回调函数
function StartNewWorkFlow (doclist, wfKeyword, userlist,callback_fun) {
    var me = this;
    //保存参数，在选择用户的时候调用
    //var storeAudit = me.getAuditsStore();
    //storeAudit.wfKeyword = wfKeyword;
    //storeAudit.doclist = doclist;
    window.parent.newWf_doclist = doclist;
    window.parent.newWf_wfKeyword = wfKeyword;

    Ext.MessageBox.wait("正在启动流程，请稍候...", "等待");

    Ext.Ajax.request({
        url: 'WebApi/Post',
        method: "POST",
        params: {
            C: "AVEVA.CDMS.WebApi.WorkFlowController", A: "StartNewWorkFlow",
            DocList: doclist,
            WfKeyword: wfKeyword,
            userlist: userlist, sid: localStorage.getItem("sid")
        },
        success: function (response, options) {

            var res = Ext.JSON.decode(response.responseText);
            var state = res.success;
            if (state === false) {

                var errmsg = res.msg;
                if (errmsg === "selectUser") {

                    Ext.MessageBox.close();//关闭等待对话框
                    //打开选人对话框
                    var getUser = me.showSelectUserWin("StartNewWorkFlow", doclist, wfKeyword, callback_fun);

                } else if (errmsg === "selectDefWorkFlow") {//先查询流程列表,如果流程列表不止一个，就弹出流程选择框

                    Ext.MessageBox.close();//关闭等待对话框
                    var recod = res.data[0];
                    var dwfList = recod.dwfList;
                    showSelectDefWorkFlowWin(me, "", doclist, dwfList, callback_fun);
                } else {
                    Ext.Msg.alert("错误信息", errmsg);
                }
            }
            else {

                Ext.MessageBox.close();//关闭等待对话框

                var recod = res.data[0];
                var State = recod.state;
                if (State === "Pass") {//如果流程列表只有一个，就直接启动流程
                    //成功启动流程,返回流程Keyword
                    //me.loadAttrPage();
                    callback_fun(res, recod.WorkFlowKeyword, recod.CuWorkStateCode);
                } if (State === "selectUser") {

                    //打开选人对话框
                    var getUser = me.showSelectUserWin("StartNewWorkFlow", doclist, wfKeyword, callback_fun);

                } else if (State === "RunFunc")
                {
                    //没有通过流程分支，返回让用户插件处理
                    var plugins = res.data[0].plugins;
                    var DefWorkFlow = res.data[0].DefWorkFlow;
                    var CuWorkState = res.data[0].CuWorkState;
                    var FuncName = res.data[0].FuncName;

                    if (plugins != "" && DefWorkFlow != "" && CuWorkState != "" && FuncName != "") {
                        Ext.require('Ext.plug_ins.m.' + plugins + '.common', function () {
                            //通过函数名调用函数
                            function runCommFun(mainPanelId, res) {
                                //state是需要调用的函数名
                                var CommFun = eval(FuncName);
                                new CommFun(mainPanelId, res);

                            }
                            runCommFun(me.id, res);
                        });

                    }
                    else {
                        //var winCreateA9 = Ext.create('Ext.plug_ins.'+plugins+'.winCreateA9', { title: "", mainPanelId: mainPanelId });
                        Ext.Msg.alert("错误信息", "流程提交预处理错误！错误消息：" + plugins + "," + DefWorkFlow + "," + CuWorkState + "," + state);
                    }
                }
                else {
                    callback_fun(res, "", "");
                }
            }
        }
    });
};

//转到下一流程状态
function GotoNextWorkflowState(btnText, wfKeyword, userlist, callback_fun) {
    var me = this;

    window.parent.nextWf_btnText = btnText;

    Ext.MessageBox.wait("正在提交到下一流程状态，请稍候...", "等待");

    Ext.Ajax.request({
        url: 'WebApi/Post',
        method: "POST",
        params: {
            C: "AVEVA.CDMS.WebApi.WorkFlowController", A: "GotoNextWfState",
            BrachKeyword: btnText,
            WfKeyword: wfKeyword,
            UserList: userlist,
            sid: localStorage.getItem("sid")
        },
        success: function (response, options) {
            var res = Ext.JSON.decode(response.responseText);


            var state = res.success;
            if (state === false) {
                var errmsg = res.msg;
                if (errmsg === "selectUser") {
                    Ext.MessageBox.close();//关闭等待对话框
                    //打开选人对话框
                    var getUser = me.showSelectUserWin("GotoNextWorkflowState", "", wfKeyword, callback_fun);

                } else {
                    Ext.Msg.alert("错误信息", errmsg);
                }
            }
            else {
                Ext.MessageBox.close();//关闭等待对话框
                var recod = res.data[0];
                var State = recod.state;
                if (State === "Pass") {
                    //通过流程分支
                    //var wfKeyword = storeAudit.wfKeyword;
                    //me.loadWorkflowAuditPage(wfKeyword);
                    callback_fun(res);
                }  else {
                    callback_fun(res);
                }
            }
        }
    });
};

//创建文档docCode：文件代码, docDesc：文件描述, docTemp：文件模板
function createDoc (uploadbasic, file, ProjectKeyword, docCode, docDesc, docTemp, callback_fun) {
    var me = this;

    var docAttr =
    [{ name: 'docCode', value: docCode },
        { name: 'docDesc', value: docDesc },
        { name: 'tempDefnId', value: "" }
    ];
    var docAttrJson = Ext.JSON.encode(docAttr);
    
    Ext.Ajax.request({

        url: 'WebApi/Post',
        method: "POST",
        params: {
            C: "AVEVA.CDMS.WebApi.DocController", A: "CreateDoc",
            sid: localStorage.getItem("sid"), projectKeyword: ProjectKeyword, docAttrJson: docAttrJson,
        },
        file: file,
        success: function (response, options) {
            var res = Ext.JSON.decode(response.responseText, true);
            var state = res.success;
            if (state === false) {
                var errmsg = res.msg;
                //uploadbasic.uploader.stop();

                if (res.msg.endsWith("指定的文件名与现有文档重名. 请指定另一个文档名称！")) {
                    Ext.Msg.alert("错误信息", errmsg);
                    setFileUploadError(uploadbasic, options.file)

                    //Ext.MessageBox.show({
                    //    title: '提示?',
                    //    msg: res.msg + "是否尝试继续上传文档?",
                    //    buttons: Ext.MessageBox.YESNO,
                    //    buttonText: {
                    //        yes: "是",
                    //        no: "否"
                    //    },
                    //    fn: function (btn) {
                    //        if (btn === "yes") {
                    //            createDoc(uploadbasic, options.file, ProjectKeyword, docCode + "(2)", docDesc, docTemp, callback_fun)
                    //            return;
                    //        }
                    //        else {
                    //            setFileUploadError(uploadbasic, options.file)
                    //        }
                    //    }
                    //});
                } else {
                    Ext.Msg.alert("错误信息", errmsg);
                    setFileUploadError(uploadbasic, options.file)
                }



                //return false
            }
            else {

                uploadbasic.multipart_params.DocKeyword = res.data[0].Keyword;

                var DocKeyword = res.data[0].Keyword;
                //me.docList = me.docList + "," + DocKeyword;


                callback_fun(uploadbasic, res, options, DocKeyword);

                //Ext.require('Ext.ux.Common.comm', function () {
                //    //触发服务端BeforeUploadFile事件，如果是断点续传，获取已上传的文件大小
                //    sendBeforeUploadFile(uploadbasic, options.file, DocKeyword, me);
                //});
                    

                return true;
            }
        },
        failure: function (response, options) {

        }
    });
};

//触发服务端BeforeUploadFile事件，如果是断点续传，获取已上传的文件大小
//senderWin:传送发送的窗口
function setFileUploadError(uploadbasic, file) {
    //设置文件状态，上传文件失败
    file.status = 4;
    uploadbasic.uploader.stop();
    uploadbasic.uploader.start();
    return false;
};
//触发服务端BeforeUploadFile事件，如果是断点续传，获取已上传的文件大小
//senderWin:传送发送的窗口
function sendBeforeUploadFile(uploadbasic, file, DocKeyword, senderWin) {
    var me = this;
    var ServerFileName = file.name;
    var CreateDate = "";
    var ModifyDate = file.updatetime;
    var fileSize = file.size;
    var MD5 = "";
    Ext.Ajax.request({

        url: 'WebApi/Post',
        method: "POST",
        params: {
            C: "AVEVA.CDMS.WebApi.FileController", A: "BeforeUploadFile",
            sid: localStorage.getItem("sid"), ObjectKeyword: DocKeyword,
            ServerFileName: ServerFileName, CreateDate: CreateDate,
            ModifyDate: ModifyDate, fileSize: fileSize, MD5: MD5
        },
        file: file,//ajax传递参数到回调函数
        success: function (response, options) {
            var res = Ext.JSON.decode(response.responseText, true);
            var state = res.success;
            if (state === false) {
                var errmsg = res.msg;

                Ext.MessageBox.show({
                    title: '错误提示',
                    msg: res.msg + " 是否继续上传文档?",
                    buttons: Ext.MessageBox.YESNO,
                    buttonText: {
                        yes: "继续上传",
                        no: "取消上传"
                    },
                    fn: function (btn) {
                        //设置文件为上传失败状态
                        file.status = 4;

                        uploadbasic.uploader.removeFile(file);

                        if (btn === "yes") {
                            //尝试重新开启上传进程
                            uploadbasic.uploader.stop();
                            uploadbasic.uploader.start();
                            return false;
                        }
                        else {

                            //尝试重新开启上传进程
                            uploadbasic.uploader.stop();
                            return false;
                        }
                    }
                });
            }
            else {
                //获取服务端返回的状态值
                var reValue = parseInt(res.data[0].resultValue);
                if (reValue > 0) {
                    options.file.lastloaded = reValue;
                    options.file.loaded = reValue;
                }

                //返回-1 表示服务器上存在，不需要传输文件
                if (reValue === -1) {
                    options.file.lastloaded = options.file.size;
                    options.file.loaded = options.file.size;

                    uploadbasic.multipart_params.ServerFullFileName = res.data[0].ServerFullFileName;
                    senderWin.ServerFullFileName = res.data[0].ServerFullFileName;

                    options.file.status = 4;

                    uploadbasic.uploader.stop();
                    uploadbasic.uploader.start();
                    afterUploadFile(uploadbasic, file, res.data[0].ServerFullFileName);
                    //return true;
                }

                uploadbasic.multipart_params.ServerFullFileName = res.data[0].ServerFullFileName;
                senderWin.ServerFullFileName = res.data[0].ServerFullFileName;

                //设置文件状态，开始上传文件
                options.file.status = 2;

                //return res.data[0].ServerFullFileName;
                return true;
            }
        },
        failure: function (response, options) {
            return false;
        }
    });
};

//上传文件后触发的事件，
//ServerFullFileName：文件的服务器路径
function afterUploadFile (uploader, file, ServerFullFileName) {
    //var me = this;
    var DocKeyword = uploader.multipart_params.DocKeyword;

    Ext.Ajax.request({

        url: 'WebApi/Post',
        method: "POST",
        params: {
            C: "AVEVA.CDMS.WebApi.FileController", A: "AfterUploadFile",
            sid: localStorage.getItem("sid"), DocKeyword: DocKeyword, ServerFullFileName: ServerFullFileName
        },
        success: function (response, options) {
            var res = Ext.JSON.decode(response.responseText, true);
            var state = res.success;
            if (state === false) {
                var errmsg = res.msg;
                //Ext.Msg.alert("错误信息", errmsg);
                return false
            }
            else {
                //uploadbasic.multipart_params.ServerFullFileName = res.data[0].ServerFullFileName;
                return true
            }
        }
    });
};

//创建用户组
function sendCreateUserGroup(nodes, groupCode, groupDesc,autoCode, callback_fun) {
    //var nodes = viewTree.getSelectionModel().getSelection();//获取已选取的节点
    var gCode = "";
    
    if (autoCode > 0) {
        //设置重复代码时自动增长
        gCode = groupCode + "(" + (autoCode + 1) + ")";
    } else
    {
        gCode = groupCode;
    }

    if (nodes !== null && nodes.length > 0) {
        Ext.Ajax.request({
            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.WebApi.GroupController", A: "NewGroup",
                sid: localStorage.getItem("sid"), groupCode: gCode,
                groupDesc: groupDesc, parentGroup: nodes[0].data.id
            },
            success: function (response, options) {
                var res = Ext.JSON.decode(response.responseText, true);
                var state = res.success;
                if (state === false) {
                    var errmsg = res.msg;
                    if (errmsg === "新建组的代码不能与现存的重复!" && autoCode > 0)
                    {
                        sendCreateUserGroup(nodes, groupCode, groupDesc, autoCode+1, callback_fun)
                    } else {
                        Ext.Msg.alert("错误信息", errmsg);
                    }
                }
                else {
                    var recod = res.data[0];
                    //添加节点
                    var newnode = [{ id: recod.groupKeyword, text: recod.groupName, leaf: true }];  //新节点信息  
                    var pnode = nodes[0]; //查找父节点
                    if (Ext.isEmpty(pnode)) //如果没有父节点，则pnode为根节点  
                    {
                        pnode = viewTree.store.getRootNode();
                    }
                    pnode.appendChild(newnode); //添加子节点  
                    pnode.set('leaf', false);
                    pnode.expand();

                    callback_fun(res);
                    //winCreateUserGroup.close();

                }
            }
        });
    }
};

//刷新文档预览,strHtml:预览的网址，DocKeyword，文档:keyword,viewPanel:要显示模型的视图
function updateDocPreview(parentView, viewPanel, strHtml, DocKeyword,fileName,explorerType) {//Scripts/PDFJSInNet/web/viewer.html
    //var me = this;
    if (DocKeyword!="")
        parentView.DocKeyword = DocKeyword;
    var h = "500px";

    if (parentView.el != undefined) {
        var str = parentView.el.dom.style.height;
        var str2 = str.replace(/px/, "");

        if (parentView === "true" || parentView === "false")
        {
            h = (parseInt(str2) - 70).toString() + "px";
        }else{
            h = (parseInt(str2)).toString() + "px";
        }
    }

    if (strHtml != "") {
        //把文件名转url编码
        var fileNameUrl = encodeURIComponent(fileName);
        strHtml = strHtml.replace(fileName,fileNameUrl);
        //me._DocPreviewPanel
        viewPanel.update('<div id="mapPic">' + '<iframe src="' + strHtml + '"  scrolling="no" style="width:100%;height:' + h + '" frameborder="0"></iframe> </div>', false, function () {
        });
    } else {
        //me._DocPreviewPanel
        viewPanel.update('<div id="mapPic">' + '<iframe src=""  scrolling="no" style="width:100%;height:' + h + '" frameborder="0"></iframe> </div>');
    }
};

//更新文档属性,参数closeWin:完成更新后是否关闭窗口
//AttrObject格式：{ name: attrCode, value: attrValueNew, attrtype: "attrData" }
//attrtype: "attrData"表示附加属性，attrtype: ""表示主要属性
function updateDocAttr(Keyword, AttrObject, callback_fun) {
    //var me = this;
    AttrJson = Ext.JSON.encode(AttrObject);

        Ext.Ajax.request({

            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.WebApi.DocController", A: "UpdateDocAttr",
                sid: localStorage.getItem("sid"), docKeyword: Keyword,//projectKeyword: me.projectKeyword,
                docAttrJson: AttrJson
            },
            success: function (response, options) {
                var obj = Ext.decode(response.responseText);
                callback_fun(obj,options);
                //if (obj.success == true) {
                //    var actTabTitle = me.up('_mainAttrTab').mainattrtab.activeTab.title;
                //    if (actTabTitle === "文档属性") {//修改文档附加属性
                //        me.loadAttrPage("docAttr", Keyword);
                //    } else if (actTabTitle === "文档_附加属性") {//修改文档属性
                //        me.loadAttrPage("docAttrData", Keyword);
                //    }
                //} else { Ext.Msg.alert("错误信息", obj.msg); }

            }
        })
};

//更新目录属性,参数closeWin:完成更新后是否关闭窗口
//AttrObject格式：{ name: attrCode, value: attrValueNew, attrtype: "attrData" }
//attrtype: "attrData"表示附加属性，attrtype: ""表示主要属性
function updateProjectAttr(Keyword, AttrObject, callback_fun) {
    //var me = this;

    AttrJson = Ext.JSON.encode(AttrObject);

        Ext.Ajax.request({

            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.WebApi.ProjectController", A: "UpdateProjectAttr",//action: "ModiProject",
                sid: localStorage.getItem("sid"),  projectKeyword: Keyword,
                projectAttrJson: AttrJson
            },
            success: function (response, options) {
                var obj = Ext.decode(response.responseText);
                callback_fun(obj, options);
                //if (obj.success == true) {
                //    var actTabTitle = me.up('_mainAttrTab').mainattrtab.activeTab.title;
                //    if (actTabTitle === "目录属性") {//修改文档附加属性
                //        me.loadAttrPage("projectAttr", Keyword);
                //    } else if (actTabTitle === "目录_附加属性") {//修改文档属性
                //        me.loadAttrPage("projectAttrData", Keyword);
                //    }
                //} else { Ext.Msg.alert("错误信息", obj.msg); }

            }
        })
};

////获取一个菜单子项参数：newMenus：菜单，showRecords：要显示的菜单记录集合，menuText：菜单文本，menuFunc：菜单触发的事件
//function getMenuItem(showRecords, menuText, menuFunc) {
//    //var menuText = "生成A.1工程开工报审表";
//    //var menuFunc = function () { me.MenuCreateWinA1(); }
//    var menu1=null;
//    for (var i = 0; i < showRecords.length; i++) {  //从节点中取出子节点依次遍历
//        var record = showRecords[i];
//        if (record.Name === menuText) {
//            if (record.State === "Enabled") {
//                menu1 = new Ext.menu.Item({
//                    text: menuText, handler: menuFunc 
//                });
//                //newMenus.add(menu1);
//            } else if (record.State === "Disabled") {//禁用菜单
//                menu1 = new Ext.menu.Item({
//                    text: menuText, disabled: true, handler: menuFunc 
//                });
//                //newMenus.add(menu1);
//            }
//        }
//    }
//    return menu1;
//};

//这一个定义必须保留在文件末尾
Ext.define('Ext.ux.Common.m.comm', {
});
