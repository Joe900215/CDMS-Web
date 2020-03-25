//起草红头文 DraftDocument
Ext.define('Ext.plug_ins.HXEPC_Plugins.Document.DraftReport', {
    extend: 'Ext.container.Container',
    alias: 'widget.DraftReport',
    layout: {
        type: 'vbox',
        pack: 'start',
        align: 'stretch'
    },
    width: 400,
    height: '100%',
    GroupType: 'Org', Filter: '',
    baseCls: 'my-panel-no-border',//隐藏边框
    initComponent: function () {
        var me = this;
        //me.renderTo = Ext.getBody();
        me.renderTo = me.el;

        //所属项目目录
        me.projectDirKeyword = "";

        //表单正常标题
        me.normalTitle = "";

        //记录著录表属性
        me.cataAttrArray = [{ receiveType: "LET" }];

        //当前用户的用户组类型（org或project）
        me.curUserGroupType = "";
        //当前用户的用户组关键字 
        me.curUserGroupKeyword = "";

        //是否回复函件
        me.isReply = false;
        me.replyCallbackFun = function () { }

        //上传文档的doc关键字列表
        me.docList = "";

        //附件文件名的前缀
        me.docCode = "";

        //附件序号
        me.docUploadIndex = 0;

        //下一流程状态用户
        me.nextStateUserList = "";

        //定义发文单位combo初始数据
        //me.sendCompanydata = [];

        //定义收文单位combo初始数据
        //me.recCompanydata = [];

        //定义文件编码Panel
        me.fileCodePanel = Ext.create('Ext.plug_ins.HXEPC_Plugins.Document.Panel.FileCodePanel');

        //定义审批路径Panel
        me.approvPathPanel = Ext.create('Ext.plug_ins.HXEPC_Plugins.Document.Panel.ApprovPathPanel', {
            projectKeyword: me.projectKeyword, projectDirKeyword: me.projectKeyword
        });


        //定义文件上传Panel
        me.fileUploadPanel = Ext.create('Ext.plug_ins.HXEPC_Plugins.Document.Panel.FileUploadPanel', {
            projectKeyword: me.projectKeyword, projectDirKeyword: me.projectKeyword
        });

        //设置上传控件为附件模式
        me.fileUploadPanel.setAttaMode();

        //me.fileUploadPanel.gridMaxHeight = me.container.lastBox.height - 40;
        me.fileUploadPanel.setGridMinHeight(100);

        me.fileUploadPanel.onFileEditButtonClick = function () {
            me.editTopPanel.hide();
            me.approvPathPanel.hide();
            me.bottomButtonPanel.hide();
            me.fileUploadPanel.setHeight(me.container.lastBox.height - 40);
            me.fileUploadPanel.filegrid.setHeight(me.container.lastBox.height - 40);
            winDraftReport.setTitle("起草 - 编辑附件");
            winDraftReport.closable = false;
        };

        //保存附件按钮事件
        me.fileUploadPanel.onFileSaveButtonClick = function () {

            //me.totalPagesText.setValue(me.fileUploadPanel.totalPages);

            me.editTopPanel.show();
            me.approvPathPanel.show();
            me.bottomButtonPanel.show();
            //me.filegrid.setHeight(me.gridMinHeight);
            winDraftReport.setTitle("起草");
            winDraftReport.closable = true;
        };

        /////////////////发文编码/////////////////


        //定义函件编码Text
        me.ReportCodeText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "函件编码", anchor: "80%", labelWidth: 60, labelAlign: "left", width: "100%", //width: 230, 
            margin: '10 10 0 10', fieldStyle: 'border-color: red; background-image: none;'//红色边框//flex: 1
        });


        //定义函件标题Text
        me.titleText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "标题", anchor: "80%", labelWidth: 60, labelAlign: "right", width: "100%",//width: 230, 
            margin: '10 10 0 10', fieldStyle: 'border-color: red; background-image: none;'//红色边框//flex: 1
        });

        //定义主送单位Text
        me.deliveryUnitText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "主送", anchor: "80%", labelWidth: 60, labelAlign: "right", width: "100%",//width: 230, 
            margin: '10 10 0 10', fieldStyle: 'border-color: red; background-image: none;'//红色边框//flex: 1
        });

        //添加函件正文Text
        me.contentText = Ext.create("Ext.form.field.TextArea", {
            xtype: "textarea", anchor: "80%", labelAlign: "right", margin: '10 10 0 10', //margin: '0 5 5 0',
            width: "100%",//flex:1, //width: 460, //
            height: 150, fieldLabel: "正文", labelWidth: 60,
        });


      
        //编辑区域头部
        me.editTopPanel = Ext.create("Ext.panel.Panel", {
            baseCls: 'my-panel-no-border',//隐藏边框
            layout: {
                type: 'vbox',
                pack: 'start',
                align: 'stretch'
            },
            margin: '0 0 0 0',// 
            items: [
                 me.fileCodePanel,
                 
                     {
                         layout: "hbox",
                         width: '100%',
                         align: 'stretch',
                         pack: 'start',
                         baseCls: 'my-panel-no-border',//隐藏边框
                         items: [
                            me.deliveryUnitText//主送

                         ], flex: 1
                     }
                     ,
                     {
                         layout: "hbox",
                         width: '100%',
                         align: 'stretch',
                         pack: 'start',
                         baseCls: 'my-panel-no-border',//隐藏边框
                         items: [
                            me.titleText,//标题

                         ], flex: 1
                     },
                   
                     {

                         layout: "hbox",
                         width: '100%',
                         align: 'stretch',
                         pack: 'start',
                         height: 155, margin: '0 0 0 0',
                         baseCls: 'my-panel-no-border',//隐藏边框
                         items: [
                              me.contentText  // 函件正文
                         ]//, flex: 1
                     }
            ]
        });


        //底部按钮区域
        me.bottomButtonPanel = Ext.create("Ext.panel.Panel", {
            layout: "hbox",
            baseCls: 'my-panel-no-border',//隐藏边框
            //align: 'right',
            //pack: 'end',//组件在容器右边
            items: [{
                flex: 1, baseCls: 'my-panel-no-border'//隐藏边框
            },
                {
                    xtype: "button",
                    text: "确定", width: 60, margins: "10 5 10 5",
                    listeners: {
                        "click": function (btn, e, eOpts) {//添加点击按钮事件
                            me.send_draft_report();
                        }
                    }
                },
                {
                    xtype: "button",
                    text: "取消", width: 60, margins: "10 15 10 5",
                    listeners: {
                        "click": function (btn, e, eOpts) {//添加点击按钮事件

                            winDraftReport.close();
                        }
                    }
                }
            ]
        });

        //添加列表
        me.items = [
           
            
          Ext.widget('form', {
              baseCls: 'my-panel-no-border',//隐藏边框
              layout: {
                  type: 'vbox',
                  align: 'stretch',
                  pack: 'start'
              },
              items: [{//上部容器
                  baseCls: 'my-panel-no-border',//隐藏边框
                  layout: {
                      type: 'vbox',
                      pack: 'start',
                      align: 'stretch'
                  },
                  margin: '10 0 0 0',// 
                  items: [
                     
                      me.editTopPanel,
                      me.fileUploadPanel,
                      me.approvPathPanel


                  ], flex: 1
              },
               me.bottomButtonPanel

              ]
          })

        ];


        me.callParent(arguments);

    },


    //获取起草通知请示报告默认参数
    sendGetDraftReportDefault: function (funCallback) {
        var me = this;

        var draftOnProject = "false";
        if (me.projectKeyword === me.projectDirKeyword) {
            draftOnProject = "true";
        }
        //通过extjs的ajax获取操作全部名称
        Ext.Ajax.request({
            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.HXEPC_Plugins.Document", A: "GetDraftReportDefault",
                sid: localStorage.getItem("sid"), ProjectKeyword: me.projectKeyword,
                DraftOnProject: draftOnProject
                //ProjectDirKeyword: me.projectDirKeyword
            },
            success: function (response, options) {
                me.sendGetDraftReportDefault_callback(response, options, funCallback);

            }
        });
    },

    //获取起草信函表单默认参数
    sendGetReplyReportDefault: function (funCallback, closeWinCallBack) {
        var me = this;

        me.isReply = true;
        me.replyCallbackFun = closeWinCallBack;

        //通过extjs的ajax获取操作全部名称
        Ext.Ajax.request({
            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.HXEPC_Plugins.Document", A: "GetReplyReportDefault",
                sid: localStorage.getItem("sid"), DocKeyword: me.docKeyword
            },
            success: function (response, options) {
                me.sendGetDraftReportDefault_callback(response, options, funCallback);

            }
        });
    },

    //处理获取发文处理表单默认参数的返回
    sendGetDraftReportDefault_callback: function (response, options, funCallback) {
        var me = this;

        //获取数据后，更新窗口
        var res = Ext.JSON.decode(response.responseText, true);
        var state = res.success;
        if (state === true) {
            var recod = eval(res.data[0]);

            var strRootProjectCode = recod.RootProjectCode;
            var strRootProjectDesc = recod.RootProjectDesc;
            var strProjectDesc = recod.ProjectDesc;
            var strFormClassCode=""
            if (strProjectDesc === "通知") {
                strFormClassCode = "NOT";
            } else if (strProjectDesc === "请示") {
                strFormClassCode = "AFA";
            } else if (strProjectDesc === "报告") {
                strFormClassCode = "REP";
            }
           
            //设置表单标题
            me.setFormTitle("起草" + strProjectDesc);


            var strDocNumber = recod.DocNumber;
            me.recCompanyList = eval(recod.RecCompanyList);
            me.sendCompanyList = eval(recod.SendCompanyList);
            var sourceCompany = recod.SourceCompany;//项目所属公司
            var sourceCompanyDesc = recod.SourceCompanyDesc;

            //当前用户的用户组类型
            me.approvPathPanel.curUserGroupType = recod.GroupType;
            //当前用户的用户组关键字 
            me.approvPathPanel.curUserGroupKeyword = recod.GroupKeyword;

            //下一流程状态用户的默认值
            me.approvPathPanel.nextStateUserText.setValue(recod.AuditorDesc);
            me.approvPathPanel.nextStateUserList = recod.AuditorList;

  
            var recUnitCode = "";
            var recUnitDesc = "";

            if (me.isReply === true) {
                me.projectKeyword = recod.SendProjectKeyword;//如果是回复信函，就记录发文目录
                recUnitCode = recod.RecUnitCode;
                recUnitDesc = recod.RecUnitDesc;
                me.recCodeText.setValue(recod.RecCode);
            }

            //默认设置为不新建文件编码
            me.fileCodePanel.setNeedNewFileCode(true);

            //设置发起目录和项目所在目录
            me.fileCodePanel.projectKeyword = me.projectKeyword;
            me.fileCodePanel.projectDirKeyword = me.projectKeyword;

            //设置收发文单位的单位类型
            me.fileCodePanel.setFormClass(strFormClassCode, strProjectDesc);
            //设置项目管理类文件里项目的代码和描述
            me.fileCodePanel.setRootProject(strRootProjectCode, strRootProjectDesc);

            if (strRootProjectCode === undefined || strRootProjectCode === "") {

                me.fileCodePanel.setDocUnitClass("部门", "部门");
                //隐藏发给项目单选框
                me.fileCodePanel.toProjectCheckBox.setVisible(false);

                //运营信函

            } else {
                //项目（非运营）信函
                me.fileCodePanel.setDocUnitClass("项目", "项目");

            }
            
            //设置发文单位代码
            me.fileCodePanel.setSendCompany(sourceCompany, sourceCompanyDesc);

            //设置文件编码Panel的各个按钮的用户事件
            me.fileCodePanel.AfterSelectRecCompany = function (code,desc) {
                me.deliveryUnitText.setValue(desc);
            }

            //不需要强制输入页数（页数自动生成）
            me.fileCodePanel.needInputPage = false;

            me.fileUploadPanel.setSendCompany(sourceCompany, sourceCompanyDesc);

            //设置项目管理类文件里项目的代码和描述
            me.fileUploadPanel.setRootProject(strRootProjectCode, strRootProjectDesc);

            //设置文件上传表格的模式
            me.fileUploadPanel.setFileGridMode("NOT");

            if (me.isReply === true) {
                //如果是回复信函

                me.mainFeederText.setValue(recUnitDesc);

            }

            funCallback();
        }
    },

    setFormTitle: function (desc) {
        var me = this;
        me.normalTitle = desc;
        winDraftReport.setTitle(desc);
    },

    //向服务器发送起草红头公文请求
    send_draft_report: function () {
        var me = this;

        //检查文件编码
        var checkResult = me.fileCodePanel.checkFileCodeFill();
        if (checkResult != "true") {
            Ext.Msg.alert("错误信息", checkResult);
            return;
        }

        //获取文件编码
        var reportCode = me.fileCodePanel.getFileCode();

        //获取文件ID
        var fileId = me.fileCodePanel.getFileId();

        //获取文件类型代码
        var docIdentifier = me.fileCodePanel.getDocIdentifier();

        //获取标题Text
        var title = me.titleText.value;

        //获取主送单位Text
        var deliveryUnit = me.deliveryUnitText.value;

        //获取正文内容Text
        var content = me.contentText.value;

        ////获取责任单位Text
        //var accountUnit = me.accountUnitText.value;



        //获取审批路径Combo
        var approvpath = me.approvPathPanel.approvpathCombo.value;

    

        //获取文件列表
        var fileArray = [];
        for (var i = 0; i < me.fileUploadPanel.filestore.getCount() ; i++) {
            var record = me.fileUploadPanel.filestore.getAt(i);

            var fn = record.get('name');
            var fc = record.get('code');
            var fd = record.get('desc');
            var fp = record.get('page');
            var fe = record.get('edition');
            var sl = record.get('seculevel');
            var fs = record.get('status');
            var fr = record.get('remark');

            var fa =
                { fn: fn, fc: fc, fd: fd, fp: fp, fe: fe, sl: sl, fs: fs, fr: fr };

            fileArray.push(fa);
        }
        
        //获取表单数据，转换成JSON字符串
        var projectAttr =
        [
            { name: 'docIdentifier', value: docIdentifier },//文档所属类型（ARA，NOT，REP）
            { name: 'fileCode', value: reportCode },
            { name: 'title', value: title },
            { name: 'mainFeeder', value: deliveryUnit },
            { name: 'content', value: content },//正文内容
            { name: 'approvpath', value: approvpath },
            { name: 'nextStateUserList', value: me.approvPathPanel.nextStateUserList },
            { name: 'fileId', value: fileId }
        ];

        var projectAttrJson = Ext.JSON.encode(projectAttr);
        var fileListJson = Ext.JSON.encode(fileArray);
        var cataAttrJson = Ext.JSON.encode(me.fileCodePanel.cataAttrArray);

        Ext.MessageBox.wait("正在生成函件，请稍候...", "等待");

        Ext.Ajax.request({

            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.HXEPC_Plugins.Document", A: "DraftReport",
                sid: localStorage.getItem("sid"), ProjectKeyword: me.projectKeyword,
                DocAttrJson: projectAttrJson,CataAttrJson: cataAttrJson,
                FileListJson: fileListJson
            },
            success: function (response, options) {
                //me.draft_document_callback(response, options, "");//, me.projectKeyword, closeWin);

                //获取数据后，更新窗口
                var res = Ext.JSON.decode(response.responseText, true);
                var state = res.success;
                if (state === true) {

                    Ext.MessageBox.close();//关闭等待对话框

                    var recod = eval(res.data[0]);

                    me.docKeyword = recod.DocKeyword;//获取联系单文档id
                    me.fileUploadPanel.docList = recod.DocList;//获取流程文档列表
                    me.newProjectKeyword = recod.ProjectKeyword;//获取新建的目录id
                    //获取附件文件名的前缀
                    me.docCode = recod.DocCode;

                    me.fileUploadPanel.docKeyword = me.docKeyword;

                    if (me.fileUploadPanel.FileUploadButton.uploader.uploader.files.length > 0) {
                        //上传完所有文件后，刷新表单
                        me.fileUploadPanel.afterUploadAllFile = function () {

                            me.draft_document_callback(response, options, "");
                        };

                        me.fileUploadPanel.send_upload_file();
                    } else {
                        //当没有附件时，处理返回事件
                        me.draft_document_callback(response, options, "");//, me.projectKeyword, closeWin);
                        //me.send_create_doc_callback(projectKeyword, docKeyword, me.docList, true);
                    }
                } else {
                    var errmsg = res.msg;
                    Ext.Msg.alert("错误信息", errmsg);
                }
            },
            failure: function (response, options) {
                //////Ext.Msg.alert("错误", "连接服务器失败！<br>" + response.responseText);
            }
        })
    },

    //发送起草函件后的返回事件
    draft_report_callback: function (response, options, parentKeyword) {
        var me = this;
        var res = Ext.JSON.decode(response.responseText, true);
        var state = res.success;
        if (state === false) {
            var errmsg = res.msg;
            Ext.Msg.alert("错误信息", errmsg);
        }
        else {

            Ext.MessageBox.close();//关闭等待对话框

            Ext.require('Ext.ux.Common.comm', function () {

                winDraftReport.close();

                var projectKeyword = res.data[0].ProjectKeyword;

                me.refreshWin(projectKeyword, false);

            });
        }
    },


    //处理发送起草函件后的返回事件
    draft_document_callback: function (response, options) {
        var me = this;

        //获取审批路径Combo
        var approvpath = me.approvPathPanel.approvpathCombo.value;

        var sendUnitCode = me.fileCodePanel.getSendCompanyCode();

        Ext.MessageBox.wait("正在启动流程，请稍候...", "等待");

        Ext.Ajax.request({

            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.HXEPC_Plugins.Document", A: "LetterStartWorkFlow",
                sid: localStorage.getItem("sid"), docKeyword: me.docKeyword,
                docList: me.fileUploadPanel.docList, ApprovPath: approvpath,
                UserList: me.approvPathPanel.nextStateUserList, SendUnitCode: sendUnitCode
            },
            success: function (response, options) {

                //获取数据后，更新窗口
                var res = Ext.JSON.decode(response.responseText, true);
                var state = res.success;
                if (state === true) {

                    Ext.MessageBox.close();//关闭等待对话框

                    var recod = eval(res.data[0]);
                    //me.refreshWin(recod.ProjectKeyword, true);
                    me.refreshWin(me.docKeyword, true);


                } else {
                    var errmsg = res.msg;
                    Ext.Msg.alert("错误信息", errmsg);
                    winDraftLetterCN.close();
                }
            }

        })
    },


  
    //刷新表单，参数:parentKeyword:新建的联系单目录
    refreshWin: function (parentKeyword, closeWin) {
        var me = this;
        //var tree = Ext.getCmp(me.mainPanelId);//.down('treepanel')
        var tree = Ext.getCmp(me.mainPanelId).up('_mainSourceView').down('_mainProjectTree').down('treepanel');
        var viewTreeStore = tree.store;

        viewTreeStore.load({
            callback: function (records, options, success) {//添加回调，获取子目录的文件数量
                if (closeWin)
                    winDraftReport.close();

                //展开目录
                Ext.require('Ext.ux.Common.comm', function () {
                    Ext.getCmp('contentPanel').down('_mainProjectTree').ExpendProject(parentKeyword);
                });
            }
        });
    }
});