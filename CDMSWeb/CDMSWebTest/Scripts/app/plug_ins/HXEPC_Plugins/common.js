
//弹出消息框，填写信函收发文单位和发文编号
//调用本函数的位置在：Ext.ux.Common.comm，里面的StartNewWorkFlow的返回处理函数
function letterCNFillInfo(mainPanelId, res) {
    var plugins = res.data[0].plugins;
    var DefWorkFlow = res.data[0].DefWorkFlow;
    var CuWorkState = res.data[0].CuWorkState;
    var FuncName = res.data[0].FuncName;
    var docKeyword = res.data[0].DocKeyword;

    if (plugins === "HXEPC_Plugins" && DefWorkFlow === "COMMUNICATIONWORKFLOW" && CuWorkState === "SECRETARILMAN" && FuncName === "letterCNFillInfo") {
        //Ext.Msg.alert("返回信息", plugins + "," + DefWorkFlow + "," + CuWorkState + "," + state);
        var fmLetterCNFillInfo = Ext.create('Ext.plug_ins.' + plugins + '.Document.' + FuncName, { title: "", mainPanelId: mainPanelId, docKeyword: docKeyword });

        winLetterCNFillInfo = Ext.widget('window', {
            title: '填写发文信息',
            closeAction: 'hide',
            width: 550,
            height: 250,
            minWidth: 550,
            minHeight: 250,
            layout: 'fit',
            resizable: true,
            modal: true,
            closeAction: 'close', //close 关闭  hide  隐藏  
            items: fmLetterCNFillInfo,
            defaultFocus: 'firstName'
        });

        //fmLetterCNFillInfo.sendGetProductSignDefault(function () {
        //    winLetterCNFillInfo.show();
        //});

        winLetterCNFillInfo.show();

        //监听子窗口关闭事件
        winLetterCNFillInfo.on('close', function () {

        });
    }
}

//发文流程盖章 
function documenteSeal(mainPanelId, res) {
    var plugins = res.data[0].plugins;
    var DefWorkFlow = res.data[0].DefWorkFlow;
    var CuWorkState = res.data[0].CuWorkState;
    var FuncName = res.data[0].FuncName;
    var docKeyword = res.data[0].DocKeyword;
    var projectKeyword = res.data[0].ProjectKeyword;
    //var FileCode = res.data[0].FileCode;
    if (plugins === "HXEPC_Plugins" && DefWorkFlow === "COMMUNICATIONWORKFLOW" && CuWorkState === "APPROV" && FuncName === "documenteSeal") {
        var fmDocumenteSeal = Ext.create('Ext.plug_ins.' + plugins + '.Document.' + FuncName, {
            title: "", mainPanelId: mainPanelId, docKeyword: docKeyword, projectKeyword: projectKeyword
        });

        winDocumenteSeal = Ext.widget('window', {
            title: '是否盖章',
            closeAction: 'hide',
            width: 450,
            height: 200,
            minWidth: 450,
            minHeight: 200,
            layout: 'fit',
            resizable: true,
            modal: true,
            closeAction: 'close', //close 关闭  hide  隐藏  
            items: fmDocumenteSeal,
            defaultFocus: 'firstName'
        });

         //fmDocumenteSeal.sendCodeText.setValue(FileCode);

        winDocumenteSeal.show();

        //监听子窗口关闭事件
        winDocumenteSeal.on('close', function () {

        });
    }
};

//发文流程编制人撤回流程
function revokeWorkFlow(mainPanelId, res) {
    //Ext.Msg.alert("返回信息", "Hello! revoke.");
    //return;

    var plugins = res.data[0].plugins;
    var DefWorkFlow = res.data[0].DefWorkFlow;
    var CuWorkState = res.data[0].CuWorkState;
    var FuncName = res.data[0].FuncName;
    var projectKeyword = res.data[0].ProjectKeyword;
    var docKeyword = res.data[0].DocKeyword;
    var docType = res.data[0].DocType;

    if (plugins === "HXEPC_Plugins" && DefWorkFlow === "COMMUNICATIONWORKFLOW" && FuncName === "revokeWorkFlow") {


        Ext.MessageBox.show({
            title: '确定撤回流程',
            msg: '请选择撤回流程后文档的处理方法。',
            buttons: Ext.MessageBox.YESNOCANCEL,
            buttonText: {
                yes: "重走流程",
                no: "作废",
                cancel: "删除"//,
             //   cancel: "取消"
            },
            fn: function (btn, parentFuctionName) {
                if (btn === "yes") {

                    if (docType === "LET") {
                        ShowLetterCNWin(mainPanelId, projectKeyword, projectKeyword);
                        return;
                    }
                } else if (btn === "no") {
                } else if (btn === "cancel") {
                    Ext.Ajax.request({
                        url: 'WebApi/Post',
                        method: "POST",
                        params: {
                            C: "AVEVA.CDMS.HXEPC_Plugins.Document", A: "DeleteWorkFlowAndDoc",
                            sid: localStorage.getItem("sid"), DocKeyword: docKeyword
                        },
                        success: function (response, options) {
                            Ext.Msg.alert("信息", "成功删除了文档！");
                        }
                    });
                    
                }
            }
        });


        //var fmDocumenteFeedback = Ext.create('Ext.plug_ins.' + plugins + '.Document.' + FuncName, { title: "", mainPanelId: mainPanelId, docKeyword: docKeyword });

        //winDocumenteFeedback = Ext.widget('window', {
        //    title: '是否发送反馈意见',
        //    closeAction: 'hide',
        //    width: 450,
        //    height: 200,
        //    minWidth: 450,
        //    minHeight: 200,
        //    layout: 'fit',
        //    resizable: true,
        //    modal: true,
        //    closeAction: 'close', //close 关闭  hide  隐藏  
        //    items: fmDocumenteFeedback,
        //    defaultFocus: 'firstName'
        //});

        ////fmDocumenteFeedback.sendCodeText.setValue(FileCode);

        //winDocumenteFeedback.show();

        ////监听子窗口关闭事件
        //winDocumenteFeedback.on('close', function () {

        //});
    }
};

//发文流程选择发送反馈意见
function documenteFeedback(mainPanelId, res) {
    var plugins = res.data[0].plugins;
    var DefWorkFlow = res.data[0].DefWorkFlow;
    var CuWorkState = res.data[0].CuWorkState;
    var FuncName = res.data[0].FuncName;
    var docKeyword = res.data[0].DocKeyword;
    //var FileCode = res.data[0].FileCode;
    if (plugins === "HXEPC_Plugins" && DefWorkFlow === "COMMUNICATIONWORKFLOW" && CuWorkState === "RECUNIT2" && FuncName === "documenteFeedback") {
        var fmDocumenteFeedback = Ext.create('Ext.plug_ins.' + plugins + '.Document.' + FuncName, { title: "", mainPanelId: mainPanelId, docKeyword: docKeyword });

        winDocumenteFeedback = Ext.widget('window', {
            title: '是否发送反馈意见',
            closeAction: 'hide',
            width: 450,
            height: 200,
            minWidth: 450,
            minHeight: 200,
            layout: 'fit',
            resizable: true,
            modal: true,
            closeAction: 'close', //close 关闭  hide  隐藏  
            items: fmDocumenteFeedback,
            defaultFocus: 'firstName'
        });

        //fmDocumenteFeedback.sendCodeText.setValue(FileCode);

        winDocumenteFeedback.show();

        //监听子窗口关闭事件
        winDocumenteFeedback.on('close', function () {

        });
    }
};

//填写发文编号
function resetFileCode(mainPanelId, res) {
    var plugins = res.data[0].plugins;
    var DefWorkFlow = res.data[0].DefWorkFlow;
    var CuWorkState = res.data[0].CuWorkState;
    var FuncName = res.data[0].FuncName;
    var docKeyword = res.data[0].DocKeyword;
    var FileCode = res.data[0].FileCode;

    if (plugins === "HXEPC_Plugins" && DefWorkFlow === "COMMUNICATIONWORKFLOW" && CuWorkState === "SECRETARILMAN" && FuncName === "resetFileCode") {
        //Ext.Msg.alert("返回信息", plugins + "," + DefWorkFlow + "," + CuWorkState + "," + state);
        var fmResetFileCode = Ext.create('Ext.plug_ins.' + plugins + '.Document.' + FuncName, { title: "", mainPanelId: mainPanelId, docKeyword: docKeyword });

        winResetFileCode = Ext.widget('window', {
            title: '填写发文编号',
            closeAction: 'hide',
            width: 550,
            height: 250,
            minWidth: 550,
            minHeight: 250,
            layout: 'fit',
            resizable: true,
            modal: true,
            closeAction: 'close', //close 关闭  hide  隐藏  
            items: fmResetFileCode,
            defaultFocus: 'firstName'
        });

        //fmResetFileCode.sendGetProductSignDefault(function () {
        //    winResetFileCode.show();
        //});
        fmResetFileCode.sendCodeText.setValue(FileCode);

        winResetFileCode.show();

        //监听子窗口关闭事件
        winResetFileCode.on('close', function () {

        });
    }
}

//发文流程，收文的时候选择分发部门及本专业主办人
function distriProcess(mainPanelId, res) {
    var plugins = res.data[0].plugins;
    var DefWorkFlow = res.data[0].DefWorkFlow;
    var CuWorkState = res.data[0].CuWorkState;
    var FuncName = res.data[0].FuncName;
    var docKeyword = res.data[0].DocKeyword;
    var projectKeyword = res.data[0].ProjectKeyword;
    var groupType = res.data[0].GroupType;
    var groupKeyword = res.data[0].GroupKeyword;

    if (plugins === "HXEPC_Plugins" && DefWorkFlow === "COMMUNICATIONWORKFLOW" && CuWorkState === "RECUNIT" && FuncName === "distriProcess") {

        var fmDistriProcess = Ext.create('Ext.plug_ins.' + plugins + '.Document.' + FuncName, { title: "", mainPanelId: mainPanelId, docKeyword: docKeyword });

        winDistriProcess = Ext.widget('window', {
            title: '选择办理人员',
            closeAction: 'hide',
            width: 550,
            height: 250,
            minWidth: 550,
            minHeight: 250,
            layout: 'fit',
            resizable: true,
            modal: true,
            closeAction: 'close', //close 关闭  hide  隐藏  
            items: fmDistriProcess,
            defaultFocus: 'firstName'
        });

        //fmDistriProcess.sendGetProductSignDefault(function () {
        //    winDistriProcess.show();
        //});

        fmDistriProcess.projectKeyword = projectKeyword;
        fmDistriProcess.groupType = groupType;
        fmDistriProcess.groupKeyword = groupKeyword;
        winDistriProcess.show();

        //监听子窗口关闭事件
        winDistriProcess.on('close', function () {

        });
    }
}


//发文流程，收文的时候办理人及设计人填写处理意见
function fillRecAudit(mainPanelId, res) {
    var plugins = res.data[0].plugins;
    var DefWorkFlow = res.data[0].DefWorkFlow;
    var CuWorkState = res.data[0].CuWorkState;
    var FuncName = res.data[0].FuncName;
    var docKeyword = res.data[0].DocKeyword;
    var workflowKeyword = res.data[0].WorkflowKeyword;
    var workStateKeyword = res.data[0].WorkStateKeyword;
    var checkerKeyword = res.data[0].CheckerKeyword;
    var workStateBranchCode = res.data[0].WorkStateBranchCode;
    var projectKeyword = res.data[0].ProjectKeyword;

    if (plugins === "HXEPC_Plugins"  && FuncName === "fillRecAudit") {
        var ModiAuditDetail = Ext.create('Ext.ux.YDForm.WorkFlow._ModiAuditDetail', { title: "", AuditType: "NewProcAudit" });

        _winModiAuditDetail = Ext.widget('window', {
            title: '填写意见',
            closeAction: 'hide',
            width: 705,
            height: 450,
            minWidth: 705,
            minHeight: 450,
            layout: 'fit',
            resizable: true,
            modal: true,
            closeAction: 'close', //close 关闭  hide  隐藏  
            items: ModiAuditDetail,
            defaultFocus: 'firstName'
        });
        window.parent.resultvalue = "";
        _winModiAuditDetail.show();

        //监听子窗口关闭事件
        _winModiAuditDetail.on('close', function () {
            //跳转到下一状态
            Ext.require('Ext.ux.Common.comm', function () {
                GotoNextWorkflowState(workStateBranchCode, workflowKeyword, "", function (res) {
                    //回调函数

                    if (res === undefined) {
                       // return;
                    }
                    var fillAudiCallbackFun = function () {
                        //回调函数，通过流程分支
                        var tree = Ext.getCmp(mainPanelId).up('_mainSourceView').down('_mainProjectTree').down('treepanel');
                        var nodes = tree.getSelectionModel().getSelection();//获取已选取的节点
                        if (nodes !== null && nodes.length > 0) {
                            projectKeyword = nodes[0].data.Keyword;

                            var viewTreeStore = tree.store;

                            viewTreeStore.load({
                                callback: function (records, options, success) {//添加回调，获取子目录的文件数量
                                    //展开目录
                                    Ext.require('Ext.ux.Common.comm', function () {
                                        Ext.getCmp('contentPanel').down('_mainProjectTree').ExpendProject(projectKeyword);
                                    });

                                }
                            });
                        }
                    }
                    if (res === undefined || res.data[0].state === "Pass") {
                        fillAudiCallbackFun();
                    } else if (res.data[0].state === "RunFunc") {
                        //没有通过流程分支，返回让用户插件处理
                        doRunFunc(mainPanelId, res);//, fillAudiCallbackFun);
                    }
              
                });
            })
        });

        var mpanel = Ext.getCmp(mainPanelId);
        var msPanle = mpanel.up('_mainSourceView');
        var connPanle;
        if (msPanle != undefined) {
            connPanle = mpanel.up('_mainSourceView').down('_mainAttrTab');
        }
        else 
        { connPanle = mpanel.up("_mainMessageView").down('_mainMessageContent'); }


        ModiAuditDetail.mainAuditGridId = connPanle.down('_workFlowPage')._contentAuditsGrid.id;
        //loWin(流程Keyword, 流程状态Keyword, 审核人Keyword, 审核意见签署时间, 意见，修改意见，意见类型)
        //ModiAuditDetail.loadWin(workflowKeyword, workStateKeyword, checkerKeyword, "", "", "", "", "NewProcAudit");
        ModiAuditDetail.loadWin(workflowKeyword, "", "", "", "", "", "", "NewProcAudit");
    }
}

//认质认价流程，第二个造价员状态提交到确认人员，弹出添加流程附件窗口
function addRecognitionAtta(mainPanelId, res) {
    var plugins = res.data[0].plugins;
    var DefWorkFlow = res.data[0].DefWorkFlow;
    var CuWorkState = res.data[0].CuWorkState;
    var FuncName = res.data[0].FuncName;
    var docKeyword = res.data[0].DocKeyword;
    var workflowKeyword = res.data[0].WorkflowKeyword;
    var workStateKeyword = res.data[0].WorkStateKeyword;
    var checkerKeyword = res.data[0].CheckerKeyword;
    var workStateBranchCode = res.data[0].WorkStateBranchCode;
    var projectKeyword = res.data[0].ProjectKeyword;
    var fileCode = res.data[0].FileCode;

    //Ext.Msg.alert("返回信息", "弹出添加流程附件窗口");

    if (plugins === "HXEPC_Plugins" && FuncName === "addRecognitionAtta") {
        //var fmAddWorkFlowAttachment = Ext.create('Ext.plug_ins.HXEPC_Plugins.Document.' + 'Recognition.' + FuncName, { title: "" });

        var fmAddWorkFlowAttachment = Ext.create('Ext.plug_ins.' + plugins + '.Document.Recognition.' + FuncName, { title: "", 
            mainPanelId: mainPanelId, docKeyword: docKeyword, projectKeyword: projectKeyword, fileCode: fileCode });


        winAddWorkFlowAttachment = Ext.widget('window', {
            title: '添加流程附件',
            closeAction: 'hide',
            width: 705,
            height: 450,
            minWidth: 705,
            minHeight: 450,
            layout: 'fit',
            resizable: true,
            modal: true,
            closeAction: 'close', //close 关闭  hide  隐藏  
            items: fmAddWorkFlowAttachment,
            defaultFocus: 'firstName'
        });

        window.parent.resultvalue = "";
        winAddWorkFlowAttachment.show();

        //监听子窗口关闭事件
        winAddWorkFlowAttachment.on('close', function () {

        });

    }

}

//退回再重新提交时，选择是否添加人员
function ifAddChecker(mainPanelId, res) {
    var plugins = res.data[0].plugins;
    var DefWorkFlow = res.data[0].DefWorkFlow;
    var CuWorkState = res.data[0].CuWorkState;
    var FuncName = res.data[0].FuncName;
    var docKeyword = res.data[0].DocKeyword;
    var workflowKeyword = res.data[0].WorkflowKeyword;
    var workStateKeyword = res.data[0].WorkStateKeyword;
    var checkerKeyword = res.data[0].CheckerKeyword;
    var workStateBranchCode = res.data[0].WorkStateBranchCode;
    var projectKeyword = res.data[0].ProjectKeyword;
    var nextStateKeyword = res.data[0].NextStateKeyword;
    var nextStateDesc = res.data[0].NextStateDesc;

    Ext.MessageBox.show({
        title: '确认',
        msg: '是否重新选择'+nextStateDesc+'？',
        buttons: Ext.MessageBox.YESNO,
        buttonText: {
            yes: "是",
            no: "否"
        },
        fn: function (btn, parentFuctionName) {

            var sendAddUser = function (ifAddUser) {
                //me.secretarilManText.setValue(window.parent.usernamelist);
                var userList = "";
                if (ifAddUser === undefined|| ifAddUser === "true") {
                    userList = window.parent.resultvalue;

                    if (userList === undefined || userList === "") return;
                }

                Ext.MessageBox.wait("正在提交到下一流程状态，请稍候...", "等待");

                //添加下一校审状态人员，并提交流程到下一状态
                Ext.Ajax.request({

                    url: 'WebApi/Post',
                    method: "POST",
                    params: {
                        C: "AVEVA.CDMS.HXEPC_Plugins.Document", A: "AddCheckerAndGotoNextState",
                        sid: localStorage.getItem("sid"), DocKeyword: docKeyword,
                        WorkStateBranchCode: workStateBranchCode, NextStateKeyword: nextStateKeyword,
                        UserList: userList, IfAddUser: ifAddUser
                    },
                    success: function (response, options) {
                        //获取数据后，更新窗口
                        var res = Ext.JSON.decode(response.responseText, true);
                        var state = res.success;
                        if (state === true) {
                            Ext.MessageBox.close();//关闭等待对话框

                            //调用流程页事件，刷新父控件内容
                            Ext.getCmp(mainPanelId).refreshMainPanle(docKeyword, function () {
                             });

                            ////展开目录
                            //Ext.require('Ext.ux.Common.comm', function () {
                            //    var treeView=Ext.getCmp(mainPanelId).up('_mainSourceView').down('_mainProjectTree');
                            //    treeView.ExpendProject(docKeyword);//projectKeyword);
                            //});
                        } else {
                            var errmsg = res.msg;
                            Ext.Msg.alert("错误信息", errmsg);
                        }
                    }
                });
            }

            var addUser = "false";
            if (btn === "yes") {
                addUser = "true";
                var groupType = "";
                var groupKeyword = "";
                Ext.require('Ext.ux.Common.comm', function () {
                    showSelectUserWin("getUser", "", "", sendAddUser, groupType, groupKeyword);
                })
            }
            else if (btn === "no") {
                addUser = "false";
                var groupType = "";
                var groupKeyword = "";
                Ext.require('Ext.ux.Common.comm', function () {
                    sendAddUser(addUser);
                    //showSelectUserWin("getUser", "", "", sendAddUser(addUser), groupType, groupKeyword);
                })
            }

        }
    });
}

//收文流程弹出消息框，回复信函
//调用本函数的位置在：Ext.ux.Common.comm，里面的StartNewWorkFlow的返回处理函数
function replyLetterCN(mainPanelId, res) {
    var plugins = res.data[0].plugins;
    var DefWorkFlow = res.data[0].DefWorkFlow;
    var CuWorkState = res.data[0].CuWorkState;
    var FuncName = res.data[0].FuncName;
    var docKeyword = res.data[0].DocKeyword;

    if (plugins === "HXEPC_Plugins" && DefWorkFlow === "RECEIVED" && FuncName === "replyLetterCN") {
        //Ext.Msg.alert("返回信息", plugins + "," + DefWorkFlow + "," + CuWorkState + "," + state);
        var fmDraftLetterCN = Ext.create('Ext.plug_ins.' + plugins + '.Document.' + 'DraftLetterCN', { title: "", mainPanelId: mainPanelId, docKeyword: docKeyword });

        winDraftLetterCN = Ext.widget('window', {
            title: '回复函件',
            closeAction: 'hide',
            width: 788,
            height: 588,
            minWidth: 788,
            minHeight: 588,
            layout: 'fit',
            resizable: true,
            modal: true,
            closeAction: 'close', //close 关闭  hide  隐藏  
            items: fmDraftLetterCN,
            defaultFocus: 'firstName'
        });

        //获取打开表单时的默认参数
        fmDraftLetterCN.sendGetReplyLetterCNDefault(
        function () {
            //设置
            winDraftLetterCN.show();
        },
        function () {
            //设置回复完函件后的返回调用
            //Ext.Msg.alert("返回信息","触发了返回调用！@");
            //通过extjs的ajax获取操作全部名称
            Ext.Ajax.request({
                url: 'WebApi/Post',
                method: "POST",
                params: {
                    C: "AVEVA.CDMS.HXEPC_Plugins.Document", A: "RecWorflowPassReplyState",
                    sid: localStorage.getItem("sid"), DocKeyword: docKeyword
                },
                success: function (response, options) {

                }
            });
        });

        //监听子窗口关闭事件
        winDraftLetterCN.on('close', function () {

        });
    }
}

//处理拖拽文件到DocGrid控件的事件（收文）
//调用本函数的位置在：Ext.ux.YDForm._MainDocGrid 里面的 OnAfterCreateNewDocEvent 函数
//这里如果直接传递uploader,因为afterUpload事件延迟的关系，uploader里面的docKeyword会因为被新的文档覆盖而出现错误，所以不直接传送uploader了
function recDocument(mainPanelId, res, files) {
    var plugins = res.data[0].plugins;
    var FuncName = res.data[0].FuncName;
    var DocKeyword = res.data[0].DocKeyword;
    var ProjectKeyword = res.data[0].ProjectKeyword;

    if (plugins === "HXEPC_Plugins" && FuncName === "recDocument") {
        //Ext.Msg.alert("返回信息", plugins + "," + DefWorkFlow + "," + CuWorkState + "," + state);
        var fmRecDocument = Ext.create('Ext.plug_ins.' + plugins + '.RecDocument.' + FuncName, { title: "", mainPanelId: mainPanelId, docKeyword: DocKeyword, projectKeyword: ProjectKeyword });

        winRecDocument = Ext.widget('window', {
            title: '收文信息著录表',
            closeAction: 'hide',
            width: 668,
            height: 436,
            minWidth: 668,
            minHeight: 436,
            layout: 'fit',
            resizable: true,
            modal: true,
            closeAction: 'close', //close 关闭  hide  隐藏  
            items: fmRecDocument,
            defaultFocus: 'firstName'
        });

        //fmSelectUserEx.send_get_profession_list(function () {
        //    winSelectUserEx.show();
        //});

        winRecDocument.show();


        //监听子窗口关闭事件
        winRecDocument.on('close', function () {

        });
    }

}

//更新文件著录属性
function updateDocCataAttr(docType,docKeyword,data, callbackFun) {
    var docAttr = [
                              { name: "CA_REFERENCE", value: data.reference, attrtype: "attrData" },
                              { name: "CA_VOLUMENUMBER", value: data.volumenumber, attrtype: "attrData" },
                           { name: "CA_REFERENCE", value: data.reference, attrtype: "attrData" },
                           { name: "CA_FILECODE", value: data.code, attrtype: "attrData" },
                           //责任人
                          { name: "CA_RESPONSIBILITY", value: data.responsibility, attrtype: "attrData" },
                           { name: "CA_FILETITLE", value: data.desc, attrtype: "attrData" },
                            { name: "CA_PAGE", value: data.page, attrtype: "attrData" },
                             { name: "CA_NUMBER", value: data.share, attrtype: "attrData" },
                          { name: "CA_MEDIUM", value: data.medium, attrtype: "attrData" },
                          { name: "CA_LANGUAGES", value: data.languages, attrtype: "attrData" },
                          { name: "CA_PRONAME", value: data.proname, attrtype: "attrData" },
                          { name: "CA_PROCODE", value: data.procode, attrtype: "attrData" },
                          { name: "CA_MAJOR", value: data.major, attrtype: "attrData" },
                          { name: "CA_CREW", value: data.crew, attrtype: "attrData" },
                          { name: "CA_FACTORY", value: data.factoryCode, attrtype: "attrData" },
                          { name: "CA_FACTORYNAME", value: data.factoryname, attrtype: "attrData" },
                          { name: "CA_SYSTEM", value: data.systemcode, attrtype: "attrData" },
                          { name: "CA_SYSTEMNAME", value: data.systemname, attrtype: "attrData" },
                          { name: "CA_RELATIONFILECODE", value: data.relationfilecode, attrtype: "attrData" },
                          { name: "CA_RELATIONFILENAME", value: data.relationfilename, attrtype: "attrData" },
                          { name: "CA_FILESPEC", value: data.filespec, attrtype: "attrData" },
                          { name: "CA_FILEUNIT", value: data.fileunit, attrtype: "attrData" },
                          { name: "CA_SECRETGRADE", value: data.secretgrade, attrtype: "attrData" },
                          { name: "CA_KEEPINGTIME", value: data.keepingtime, attrtype: "attrData" },
                          { name: "CA_FILELISTCODE", value: data.filelistcode, attrtype: "attrData" },
                          { name: "CA_FILELISTTIME", value: data.filelisttime, attrtype: "attrData" },
                          { name: "CA_RACKNUMBER", value: data.racknumber, attrtype: "attrData" },
                          { name: "CA_NOTE", value: data.note, attrtype: "attrData" },
                          { name: "CA_WORKTYPE", value: data.workClass, attrtype: "attrData" },
                          { name: "CA_WORKSUBTIEM", value: data.workSub, attrtype: "attrData" },
                          { name: "CA_UNIT", value: data.department, attrtype: "attrData" },
                          { name: "CA_FILETYPE", value: data.receiveType, attrtype: "attrData" },
                          { name: "CA_FLOWNUMBER", value: data.fNumber, attrtype: "attrData" },
                          { name: "CA_EDITION", value: data.edition, attrtype: "attrData" },
                          { name: "CA_ATTRTEMP", value: "NONCOMM", attrtype: "attrData" }
    ];

    AttrJson = Ext.JSON.encode(docAttr);

    Ext.Ajax.request({

        url: 'WebApi/Post',
        method: "POST",
        params: {
            C: "AVEVA.CDMS.WebApi.DocController", A: "UpdateDocAttr",
            sid: localStorage.getItem("sid"), docKeyword: docKeyword,
            docAttrJson: AttrJson
        },
        success: function (response, options) {
            var obj = Ext.decode(response.responseText);
            if (obj.success == true) {
                callbackFun();
            }
        }
    });
   
}

function ShowLetterCNWin(mainPanelId, projectKeyword, projectDirKeyword, docKeyword, recUnitCode, recUnitDesc, sendCode, recCode) {
    var fmDraftLetterCN = Ext.create('Ext.plug_ins.HXEPC_Plugins.Document.DraftLetterCN',// + plugins + '.' + state,
          { title: "", mainPanelId: mainPanelId, projectKeyword: projectKeyword, projectDirKeyword: projectDirKeyword });

    winDraftLetterCN = Ext.widget('window', {
        title: '起草信函',
        closeAction: 'hide',
        width: 788,
        height: 588,
        minWidth: 788,
        minHeight: 588,
        layout: 'fit',
        resizable: false,
        modal: true,
        closeAction: 'close', //close 关闭  hide  隐藏  
        items: fmDraftLetterCN,
        defaultFocus: 'firstName'
    });

    fmDraftLetterCN.projectDirKeyword = projectDirKeyword;

    if (docKeyword != undefined && docKeyword != "") {
        fmDraftLetterCN.winAction = "回复信函";
        fmDraftLetterCN.preDocKeyword = docKeyword;
        fmDraftLetterCN.preRecUnitCode = recUnitCode;
        fmDraftLetterCN.preRecUnitDesc = recUnitDesc;
        //发文代码
        fmDraftLetterCN.preSendCode = sendCode;
        //收文代码
        fmDraftLetterCN.preRecCode = recCode;
    }

    //获取打开表单时的默认参数
    fmDraftLetterCN.sendGetDraftLetterCNDefault(function () {

        winDraftLetterCN.show();
    });


    //监听子窗口关闭事件
    winDraftLetterCN.on('close', function (mainPanelId) {

    });

    winDraftLetterCN.on('beforeclose', function (panel, eOpts) {
        if (panel.title != '起草信函') {
            Ext.Msg.alert("错误", "请保存附件编辑！");
            return false;
        }
    });
};

function ShowReplyLetterCNWin(mainPanelId, projectKeyword, projectDirKeyword, docKeyword, recUnitCode) {
    var fmDraftLetterCN = Ext.create('Ext.plug_ins.HXEPC_Plugins.Document.DraftLetterCN',// + plugins + '.' + state,
          { title: "", mainPanelId: mainPanelId, projectKeyword: projectKeyword, projectDirKeyword: projectDirKeyword });

    winDraftLetterCN = Ext.widget('window', {
        title: '回复信函',
        closeAction: 'hide',
        width: 788,
        height: 588,
        minWidth: 788,
        minHeight: 588,
        layout: 'fit',
        resizable: false,
        modal: true,
        closeAction: 'close', //close 关闭  hide  隐藏  
        items: fmDraftLetterCN,
        defaultFocus: 'firstName'
    });

    fmDraftLetterCN.projectDirKeyword = projectDirKeyword;

    if (docKeyword != undefined && docKeyword != "") {
        fmDraftLetterCN.winAction = "回复信函";
        fmDraftLetterCN.docKeyword = docKeyword;
        fmDraftLetterCN.recUnitCode = recUnitCode;
    }

    //获取打开表单时的默认参数
    fmDraftLetterCN.sendGetReplyLetterCNDefault(function () {

        winDraftLetterCN.show();
    });


    //监听子窗口关闭事件
    winDraftLetterCN.on('close', function (mainPanelId) {

    });

    winDraftLetterCN.on('beforeclose', function (panel, eOpts) {
        if (panel.title != '回复信函') {
            Ext.Msg.alert("错误", "请保存附件编辑！");
            return false;
        }
    });
};


//文件列表，搜索当前目录下未完成流程的文档
function searchUnfinishWFDocList(mainPanelId) {
    Ext.Msg.alert("错误", "显示未完成流程的文档列表！");
};

//这一个定义必须保留在文件末尾
Ext.define('Ext.plug_ins.HXEPC_Plugins.common', {
});
