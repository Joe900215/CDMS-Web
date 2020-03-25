//起草红头文 DraftMeetMinutes

Ext.define('Ext.plug_ins.HXEPC_Plugins.Document.DraftMeetMinutes', {
    extend: 'Ext.container.Container',
    alias: 'widget.DraftMeetMinutes',
    //layout: "border",
    layout: 'fit',
    resultvalue: '', mainPanelId: '', projectKeyword: '',
    initComponent: function () {
        var me = this;

        //记录著录表属性
        me.cataAttrArray = [{ receiveType: "MOM" }];

        //下一流程状态用户
        me.nextStateUserList = "";

        //定义发文单位combo初始数据
        me.sendCompanydata = [];


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
            winDraftMeetMinutesCN.setTitle("起草会议纪要 - 编辑附件");
            winDraftMeetMinutesCN.closable = false;
        };

        //保存附件按钮事件
        me.fileUploadPanel.onFileSaveButtonClick = function () {

            me.totalPagesText.setValue(me.fileUploadPanel.totalPages);

            me.editTopPanel.show();
            me.approvPathPanel.show();
            me.bottomButtonPanel.show();
            //me.filegrid.setHeight(me.gridMinHeight);
            winDraftMeetMinutesCN.setTitle("起草会议纪要");
            winDraftMeetMinutesCN.closable = true;
        };

     
        //定义函件编码Text
        me.documentCodeText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "函件编码", anchor: "80%", labelWidth: 70, labelAlign: "right", labelPad: 8,  width: "100%", 
            margin: '10 10 0 10', fieldStyle: 'border-color: red; background-image: none;'//红色边框//flex: 1
        });

        //定义页数Text
        me.totalPagesText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "页数", anchor: "80%", labelWidth: 70, labelAlign: "right", labelPad: 8, width: "50%",
            margin: '10 10 0 10', fieldStyle: ' background-image: none;',//红色边框//flex: 1
            listeners:
            {
                change: function (view, newValue, oldValue, eOpts) {
                    me.fileCodePanel.cataAttrArray[0].page = newValue;
                }
            }
        });

        //定义发送方Text
        me.senderText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "发自", anchor: "80%", labelWidth: 60, labelAlign: "right", labelPad: 8, width: "50%",
            margin: '10 5 0 10', fieldStyle: ' background-image: none;', flex: 1
        });

        //定义主送Text
        me.mainFeederText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "主送", anchor: "80%", labelWidth: 70, labelAlign: "right", labelPad: 8,// width: "100%"
            margin: '10 10 0 10', fieldStyle: ' background-image: none;', flex: 1//红色边框//
        });

        //定义抄送Text
        me.copyPartyText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", 
            fieldLabel: "抄送", anchor: "80%", labelWidth: 70, labelAlign: "right", labelPad: 8,// width: "100%",
            margin: '10 10 0 10', fieldStyle: ' background-image: none;', flex: 1//红色边框//flex: 1
        });

        //定义会议主题Text
        me.titleText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "会议主题", anchor: "80%", labelWidth: 70, labelAlign: "right", labelPad: 8, width: "100%",
            margin: '10 10 0 10', fieldStyle: 'border-color: red; background-image: none;',//红色边框//flex: 1
            listeners:
            {
                change: function (view, newValue, oldValue, eOpts) {
                    me.fileCodePanel.cataAttrArray[0].desc = newValue;
                }
            }
        });

        //定义会议时间Text
        me.meetTimeText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "会议时间", anchor: "80%", labelWidth: 70, labelAlign: "right", labelPad: 8,  width: "50%",
            margin: '10 10 0 10', fieldStyle: 'border-color: red; background-image: none;'//红色边框//flex: 1
        });

        //定义会议地点Text
        me.meetPlaceText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "会议地点", anchor: "80%", labelWidth: 70, labelAlign: "right", labelPad: 8,  width: "50%",
            margin: '10 10 0 10', fieldStyle: 'border-color: red; background-image: none;'//红色边框//flex: 1
        });

        //定义主办单位Text
        me.hostUnitText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "主办单位", anchor: "80%", labelWidth: 70, labelAlign: "right", labelPad: 8,  width: "100%",
            margin: '10 10 0 10', fieldStyle: 'border-color: red; background-image: none;'//红色边框//flex: 1
        });

        //定义主持人Text
        me.moderatorText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "主持人", anchor: "80%", labelWidth: 70, labelAlign: "right", labelPad: 8,  width: "100%",
            margin: '10 10 0 10', fieldStyle: 'border-color: red; background-image: none;'//红色边框//flex: 1
        });

        

        //定义参会单位与人员ext
        me.participantsText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",//"参会单位与人员"
            fieldLabel: "参会单位", anchor: "80%", labelWidth: 70, labelAlign: "right", labelPad: 8,  width: "100%",
            margin: '10 10 0 10', fieldStyle: 'border-color: red; background-image: none;'//红色边框//flex: 1
        });

        //添加会议内容Text
        me.contentText = Ext.create("Ext.form.field.TextArea", {
            xtype: "textarea", anchor: "80%", labelAlign: "right", labelPad: 8,  margin: '10 10 0 10', //margin: '0 5 5 0',
            width: "100%",// fieldStyle: 'border-color: red; background-image: none;',//flex:1, //width: 460, //
            height: 105, fieldLabel: "会议内容", labelWidth: 70,
        });

        //定义下一流程状态用户Text
        me.nextStateUserText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "审核人", anchor: "80%", labelWidth: 70, labelAlign: "right", labelPad: 8, width: "39%",
            margin: '10 5 0 10', fieldStyle: ' background-image: none;'//红色边框//flex: 1
        });
        

        //定义发送日期Text
        me.sendDateField = Ext.create("Ext.form.field.Date", {

            name: "date", 
            fieldLabel: ' 发送日期', fieldStyle: ' background-image: none;',
            labelAlign: "right", labelPad: 8,
            editable: true, labelWidth: 70, margin: '10 10 0 10',
            emptyText: "--请选择--", autoLoadOnValue: true,
            format: 'Y年m月d日',
            value: new Date(),
            width: '50%'//width: 230
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
                            me.sendDateField,//发送日期
                            me.totalPagesText //页数
                         ], flex: 1
                     },
                     {
                         layout: "hbox",
                         width: '100%',
                         align: 'stretch',
                         pack: 'start',
                         baseCls: 'my-panel-no-border',//隐藏边框
                         items: [
                            me.mainFeederText,//主送
                              {
                                  xtype: "button",
                                  text: "选择...", margins: "10 10 0 0",
                                  listeners: {
                                      "click": function (btn, e, eOpts) {//添加点击按钮事件
                                          me.fileCodePanel.recCompanyButton.fireEvent('click');
                                          //if (me.docClass === "operation") {
                                          //    //运营管理类 ，选择接收部门
                                          //    me.selectRecDepartment();
                                          //    return;
                                          //}
                                          //else {
                                          //    //项目管理类，选择接收单位
                                          //    me.selectRecUnit();
                                          //}
                                      }
                                  }
                              }

                         ], flex: 1
                     },
                     {
                         layout: "hbox",
                         width: '100%',
                         align: 'stretch',
                         pack: 'start',
                         baseCls: 'my-panel-no-border',//隐藏边框
                         items: [
                            me.copyPartyText,//抄送
                             {
                                 xtype: "button",
                                 text: "选择...", margins: "10 10 0 0",
                                 listeners: {
                                     "click": function (btn, e, eOpts) {//添加点击按钮事件

                                         me.fileCodePanel.selectCopyCallBackFun = function (code, desc) {
                                             me.copyPartyList = code;
                                             me.copyPartyText.setValue(desc);
                                         }
                                         me.fileCodePanel.callSelectCopyParty();

                                         //if (me.docClass === "operation") {
                                         //    //运营管理类 ，选择接收部门
                                         //    me.selectCopyDepartment();
                                         //    return;
                                         //}
                                         //else {
                                         //    //项目管理类，选择接收单位
                                         //    me.selectCopyUnit(); 
                                         //}
                                     }
                                 }
                             }
                         ], flex: 1
                     },
                     {
                         layout: "hbox",
                         width: '100%',
                         align: 'stretch',
                         pack: 'start',
                         baseCls: 'my-panel-no-border',//隐藏边框
                         items: [
                            me.titleText//会议主题

                         ], flex: 1
                     },
                     {
                         layout: "hbox",
                         width: '100%',
                         align: 'stretch',
                         pack: 'start',
                         baseCls: 'my-panel-no-border',//隐藏边框
                         items: [
                            me.meetTimeText,//会议时间
                             me.meetPlaceText//会议地点

                         ], flex: 1
                     },
                     //{
                     //    layout: "hbox",
                     //    width: '100%',
                     //    align: 'stretch',
                     //    pack: 'start',
                     //    baseCls: 'my-panel-no-border',//隐藏边框
                     //    items: [
                     //      me.meetPlaceText//会议地点

                     //    ], flex: 1
                     //},
                     {
                         layout: "hbox",
                         width: '100%',
                         align: 'stretch',
                         pack: 'start',
                         baseCls: 'my-panel-no-border',//隐藏边框
                         items: [
                           me.hostUnitText//主办单位

                         ], flex: 1
                     },
                     {
                         layout: "hbox",
                         width: '100%',
                         align: 'stretch',
                         pack: 'start',
                         baseCls: 'my-panel-no-border',//隐藏边框
                         items: [
                           me.moderatorText//主持人

                         ], flex: 1
                     },
                    {
                        layout: "hbox",
                        width: '100%',
                        align: 'stretch',
                        pack: 'start',
                        baseCls: 'my-panel-no-border',//隐藏边框
                        items: [
                          me.participantsText//定义参会单位与人员

                        ], flex: 1
                    },
                    {

                        layout: "hbox",
                        width: '100%',
                        align: 'stretch',
                        pack: 'start',
                        height: 110, margin: '0 0 0 0',
                        baseCls: 'my-panel-no-border',//隐藏边框
                        items: [
                             me.contentText  // 函件正文
                        ]//, flex: 1
                    }

            ]
        });


        ////编辑区域尾部
        //me.editBottomPanel = Ext.create("Ext.panel.Panel", {
        //    layout: "hbox",
        //    width: '100%',
        //    align: 'stretch',
        //    pack: 'start', margins: "0 0 5 0",
        //    baseCls: 'my-panel-no-border',//隐藏边框
        //    items: [
        //      me.approvpathCombo,//定义审批路径
        //      //{
        //      //    layout: "hbox",
        //      //    width: '100%', baseCls: 'my-panel-no-border',//隐藏边框
        //      //    align: 'stretch', margin: '0 0 0 0', padding: '0 0 0 0',
        //      //    pack: 'start',
        //      //    items: [
        //              me.nextStateUserText,
        //              {
        //                  xtype: "button",
        //                  text: "选择...", margins: "10 0 0 5",
        //                  listeners: {
        //                      "click": function (btn, e, eOpts) {//添加点击按钮事件
        //                          Ext.require('Ext.ux.Common.comm', function () {
        //                              showSelectUserWin("getUser", "", "", function () {
        //                                  me.nextStateUserText.setValue(window.parent.usernamelist);
        //                                  me.nextStateUserList = window.parent.resultvalue;
        //                              }, me.curUserGroupType, me.curUserGroupKeyword);
        //                          })
        //                      }
        //                  }
        //              }
        //          //]
        //      //}
        //    ]//, flex: 1
        //});

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
                            me.send_draft_document();
                        }
                    }
                },
                {
                    xtype: "button",
                    text: "取消", width: 60, margins: "10 15 10 5",
                    listeners: {
                        "click": function (btn, e, eOpts) {//添加点击按钮事件
                  
                            winDraftMeetMinutesCN.close();
                        }
                    }
                }
            ]
        }),

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


    //获取起草信函表单默认参数
    sendGetDraftMeetMinutesDefault: function (funCallback) {
        var me = this;


        //通过extjs的ajax获取操作全部名称
        Ext.Ajax.request({
            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.HXEPC_Plugins.Document", A: "GetDraftMeetMinutesCNDefault",
                sid: localStorage.getItem("sid"), ProjectKeyword: me.projectKeyword
            },
            success: function (response, options) {
                me.sendGetDraftMeetMinutesDefault_callback(response, options, funCallback);

            }
        });
    },

    //获取起草信函表单默认参数
    sendGetReplyMeetMinutesDefault: function (funCallback, closeWinCallBack) {
        var me = this;

        me.isReply = true;
        me.replyCallbackFun = closeWinCallBack;

        //通过extjs的ajax获取操作全部名称
        Ext.Ajax.request({
            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.HXEPC_Plugins.Document", A: "GetReplyMeetMinutesDefault",
                sid: localStorage.getItem("sid"), DocKeyword: me.docKeyword
            },
            success: function (response, options) {
                me.sendGetDraftMeetMinutesDefault_callback(response, options, funCallback);

            }
        });
    },

    //处理获取发文处理表单默认参数的返回
    sendGetDraftMeetMinutesDefault_callback: function (response, options, funCallback) {
        var me = this;

        //获取数据后，更新窗口
        var res = Ext.JSON.decode(response.responseText, true);
        var state = res.success;
        if (state === true) {
            var recod = eval(res.data[0]);

            var strRootProjectCode = recod.RootProjectCode;
            var strRootProjectDesc = recod.RootProjectDesc;
            var strProjectDesc = recod.ProjectDesc;

            var strFormClassCode="MOM"
  
            //当前用户的用户组类型
            me.approvPathPanel.curUserGroupType = recod.GroupType;
            //当前用户的用户组关键字 
            me.approvPathPanel.curUserGroupKeyword = recod.GroupKeyword;

            //下一流程状态用户的默认值
            me.approvPathPanel.nextStateUserText.setValue(recod.AuditorDesc);
            me.approvPathPanel.nextStateUserList = recod.AuditorList;

            var strDocNumber = recod.DocNumber;
            me.recCompanyList = eval(recod.RecCompanyList);
            me.sendCompanyList = eval(recod.SendCompanyList);
            var sourceCompany = recod.SourceCompany;//项目所属公司
            var sourceCompanyDesc = recod.SourceCompanyDesc;

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
            me.fileCodePanel.AfterSelectRecCompany = function (code, desc) {
                me.mainFeederText.setValue(desc);
            }

            //不需要强制输入页数（页数自动生成）
            me.fileCodePanel.needInputPage = false;

            me.fileUploadPanel.setSendCompany(sourceCompany, sourceCompanyDesc);

            //设置项目管理类文件里项目的代码和描述
            me.fileUploadPanel.setRootProject(strRootProjectCode, strRootProjectDesc);

            //设置文件上传表格的模式
            me.fileUploadPanel.setFileGridMode("MOM");
           
            if (me.isReply === true) {
                //如果是回复信函

                me.fileCodePanel.mainFeederText.setValue(recUnitDesc);

                me.fileCodePanel.recCompanyText.setValue(recUnitCode);

                me.fileCodePanel.departmentText.setValue(recUnitCode);

                me.fileCodePanel.cataAttrArray[0].department = recUnitCode;

                me.fileCodePanel.getRunNum();
            }

            funCallback();
        }
    },

            
    //向服务器发送起草红头公文请求
    send_draft_document: function () {
        var me = this;

        //检查文件编码
        var checkResult = me.fileCodePanel.checkFileCodeFill();
        if (checkResult != "true") {
            Ext.Msg.alert("错误信息", checkResult);
            return;
        }

        //获取文件编码
        var fileCode = me.fileCodePanel.getFileCode();

        //获取文件临时发文编码
        var sendCode = me.fileCodePanel.getFileSendCode();

        //获取文件ID
        var fileId = me.fileCodePanel.getFileId();

        //获取文件类型代码
        var docIdentifier = me.fileCodePanel.getDocIdentifier();

        

        ////获取发文日期
        var sendDate = me.sendDateField.value;

        //获取页数Text
        var totalPages = me.totalPagesText.value;

        ////获取主送Text
        var mainFeeder = me.mainFeederText.value;

        ////获取抄送Text
        var copyParty = me.copyPartyText.value;

        //获取会议主题Text
        var title = me.titleText.value;

        //获取会议时间
        var meetTime = me.meetTimeText.value;

        //获取会议地点
        var meetPlace = me.meetPlaceText.value;

        //获取主办单位
        var hostUnit = me.hostUnitText.value;

        //获取主持人
        var moderator = me.moderatorText.value;

        //获取参会单位与人员
        var participants = me.participantsText.value;

        //获取会议内容Text
        var content = me.contentText.value;

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
            var sl = record.get('secretgrade');
            var fs = record.get('status');
            var fr = record.get('note');

            var fa =
                { fn: fn, fc: fc, fd: fd, fp: fp, fe: fe, sl: sl, fs: fs, fr: fr };

            fileArray.push(fa);
            //fileArray.push(record.get('name'));
            //fields: ["no","name", "code", "desc", "page", "edition", "seculevel", "status", "remark"],
        }
        //获取表单数据，转换成JSON字符串
        var projectAttr =
        [
            { name: 'fileCode', value: fileCode },
            { name: 'sendCode', value: sendCode },
            //{ name: 'documentCode', value: documentCode },

            { name: 'sendDate', value: sendDate },
            { name: 'totalPages', value: totalPages },

            { name: 'mainFeeder', value: mainFeeder },
            { name: 'copyParty', value: copyParty },

            { name: 'title', value: title },
            { name: 'meetTime', value: meetTime },
            { name: 'meetPlace', value: meetPlace },

            { name: 'hostUnit', value: hostUnit },
            { name: 'moderator', value: moderator },

            { name: 'participants', value: participants },

            { name: 'content', value: content },

             { name: 'fileId', value: fileId },

            { name: 'approvpath', value: approvpath },
            { name: 'nextStateUserList', value: me.approvPathPanel.nextStateUserList }

        ];

        var projectAttrJson = Ext.JSON.encode(projectAttr);
        var fileListJson = Ext.JSON.encode(fileArray);
        var cataAttrJson = Ext.JSON.encode(me.fileCodePanel.cataAttrArray);

        Ext.MessageBox.wait("正在生成会议纪要，请稍候...", "等待");

        Ext.Ajax.request({

            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.HXEPC_Plugins.Document", A: "DraftMeetMinutesCN",
                sid: localStorage.getItem("sid"), ProjectKeyword: me.projectKeyword,
                DocAttrJson: projectAttrJson, CataAttrJson: cataAttrJson,
                    FileListJson: fileListJson
            },
            success: function (response, options) {
              
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
                    winDraftMeetMinutesCN.close();
                }
            }

        })
    },

          



    //刷新表单，参数:parentKeyword:新建的联系单目录
    refreshWin: function (parentKeyword, closeWin) {
        var me = this;
        var tree = Ext.getCmp(me.mainPanelId).down('treepanel');
        var viewTreeStore = tree.store;

        viewTreeStore.load({
            callback: function (records, options, success) {//添加回调，获取子目录的文件数量
                if (closeWin)
                    winDraftMeetMinutesCN.close();

                //展开目录
                Ext.require('Ext.ux.Common.comm', function () {
                    Ext.getCmp('contentPanel').down('_mainProjectTree').ExpendProject(parentKeyword);
                });
            }
        });
    }
});