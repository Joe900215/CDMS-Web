//起草文件传递单

Ext.define('Ext.plug_ins.HXEPC_Plugins.Document.DraftTransmittalCN', {
    extend: 'Ext.container.Container',
    alias: 'widget.DraftTransmittalCN',
    //layout: "border",
    layout: 'fit',
    resultvalue: '', mainPanelId: '', projectKeyword: '',
    initComponent: function () {
        var me = this;
        
        me.docClass = "";

        //所属项目目录
        me.projectDirKeyword = "";

        me.docKeyword = "";

        //记录著录表属性
        me.cataAttrArray = [{ receiveType: "TRA" }];

        //上传文档的doc关键字列表
        me.docList = "";

        //附件文件名的前缀
        me.docCode = "";

        //附件序号
        me.docUploadIndex = 0;

        //下一流程状态用户
        me.nextStateUserList = "";

        //收发文单位列表初始数据
        me.recCompanyList = [];
        me.sendCompanyList = [];

        //定义发文单位combo初始数据
        me.sendCompanydata = [];

        //定义收文单位combo初始数据
        me.recCompanydata = [];

        //发送者分类，发送者是部门还是项目
        me.senderClass = "";
        //接收者分类，接收者是部门还是项目
        me.recverClass = "";

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
            winDraftTransmittalCN.setTitle("起草文件传递单 - 编辑附件");
            winDraftTransmittalCN.closable = false;
        };

        //保存附件按钮事件
        me.fileUploadPanel.onFileSaveButtonClick = function () {
            me.totalPagesText.setValue(me.fileUploadPanel.totalPages);
            me.editTopPanel.show();
            me.approvPathPanel.show();
            me.bottomButtonPanel.show();
            //me.filegrid.setHeight(me.gridMinHeight);
            winDraftTransmittalCN.setTitle("起草文件传递单");
            winDraftTransmittalCN.closable = true;
        };

        //保密等级combo初始数据
        me.seculeveldata = [{ text: "商业秘密", value: "商业秘密" }, { text: "受限", value: "受限" }, { text: "公开", value: "公开" }];

        //是否需要回复combo初始数据
        me.needreplydata = [{ text: "是", value: "是" }, { text: "否", value: "否" }];

        //传递方式combo初始数据
        me.transmodedata = [{ text: "邮寄", value: "邮寄" }, { text: "当面递交", value: "当面递交" },
        { text: "邮件", value: "邮件" }, { text: "OA", value: "OA" },
        { text: "CDMS", value: "CDMS" }, { text: "其他", value: "其他" } ];

        //提交目的combo初始数据
        me.purposedata = [{ text: "提交", value: "提交" }, { text: "按需求提交", value: "按需求提交" },
        { text: "审查", value: "审查" }, { text: "备案", value: "备案" },
        { text: "采购", value: "采购" }, { text: "供货", value: "供货" },
        { text: "施工", value: "施工" }, { text: "调试", value: "调试" },
        { text: "告知", value: "告知" }, { text: "交工资料", value: "交工资料" },
         { text: "其他", value: "其他" }];

     

        //定义主送Text
        me.mainFeederText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "主送", anchor: "80%", labelWidth: 60, labelAlign: "right", labelPad:8, flex: 1,//width: "50%",//width: 230, 
            margin: '10 5 0 10', fieldStyle: 'border-color: red; background-image: none;'//红色边框//flex: 1
        });

        //定义发送方Text
        me.senderText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "发自", anchor: "80%", labelWidth: 60, labelAlign: "right", labelPad:8,// width: "50%",//width: 230, 
            margin: '10 5 0 10', fieldStyle: 'border-color: red; background-image: none;', flex: 1
        });

        //定义页数Text
        me.totalPagesText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "页数", anchor: "80%", labelWidth: 60, labelAlign: "right", labelPad:8, width: "50%",//width: 230, 
            margin: '10 10 0 10', fieldStyle: ' background-image: none;',//红色边框//flex: 1
            listeners:
            {
                change: function (view, newValue, oldValue, eOpts) {
                    me.fileCodePanel.cataAttrArray[0].page = newValue;
                }
            }

        });

        //定义保密期限Text
        me.secrTermText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "期限", anchor: "80%", labelWidth: 30, labelAlign: "right", labelPad:8, width: '22%',// width: "50%",//width: 230, 
            margin: '10 10 0 10', fieldStyle: ' background-image: none;'//红色边框//flex: 1
        });

        //传递方式补充说明
        me.transmodeSuppText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", disabled: true,
            fieldLabel: "请补充", anchor: "80%", labelWidth: 60, labelAlign: "right", labelPad: 8, width: "50%",//
             margin: '10 10 0 10', fieldStyle: ' background-image: none;'//红色边框//flex: 1
        });

        //提交目的补充说明
        me.purposeSuppText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", disabled: true,
            fieldLabel: "请补充", anchor: "80%", labelWidth: 60, labelAlign: "right", labelPad: 8, width: "50%",//width: 230,
             margin: '10 10 0 10', fieldStyle: ' background-image: none;'//红色边框//flex: 1
        });

        //定义函件标题Text
        me.titleText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "函件标题", anchor: "80%", labelWidth: 60, labelAlign: "right", labelPad:8, width: "100%",//width: 230, 
            margin: '10 10 0 10', fieldStyle: 'border-color: red; background-image: none;',//红色边框//flex: 1
            listeners:
            {
                change: function (view, newValue, oldValue, eOpts) {
                    me.fileCodePanel.cataAttrArray[0].desc = newValue;
                }
            }
        });

        //定义下一流程状态用户Text
        me.nextStateUserText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "批准人", anchor: "80%", labelWidth: 60, labelAlign: "right", labelPad: 8, width: "40%",//width: 230, 
            margin: '10 5 0 10', fieldStyle: ' background-image: none;'//红色边框//flex: 1
        });

        //添加摘要Text
        me.contentText = Ext.create("Ext.form.field.TextArea", {
            xtype: "textarea", anchor: "80%", labelAlign: "right", labelPad:8, margin: '10 10 0 10', //margin: '0 5 5 0',
            width: "100%",//flex:1, //width: 460, //
            height: 70, fieldLabel: "摘要", labelWidth: 60,
        });


        //定义发文日期Text
        me.sendDateField = Ext.create("Ext.form.field.Date", {

            name: "date",
            fieldLabel: ' 发文日期', fieldStyle: ' background-image: none;',
            labelAlign: "right", labelPad:8,
            editable: true, labelWidth: 60, margin: '10 10 0 10',
            emptyText: "--请选择--", autoLoadOnValue: true,
            format: 'Y年m月d日',
            value: new Date(),
            width: '50%'//width: 230
        });

        //定义回复日期Text
        me.replyDateField = Ext.create("Ext.form.field.Date", {

            name: "date",
            fieldLabel: ' 回复日期', fieldStyle: ' background-image: none;',
            labelAlign: "right", labelPad:8,
            editable: true, labelWidth: 55, margin: '10 10 0 10',
            emptyText: "--请选择--", autoLoadOnValue: true,
            format: 'Y年m月d日',
            value: new Date(),
            width: '30%' //width: 230
        });

        

   
        //添加保密等级combo
        Ext.define("seculevelModel", {
            extend: 'Ext.data.Model',
            fields: ["text", "value"]
        });
        me.seculevelProxy = Ext.create("Ext.data.proxy.Memory", {
            data: me.seculeveldata,
            model: "seculevelModel"
        });

        me.seculevelStore = Ext.create("Ext.data.Store", {
            model: seculevelModel,
            proxy: me.seculevelProxy
        });


        me.seculevelCombo = Ext.create("Ext.form.field.ComboBox",
        {
            //xtype: "combo",
            fieldLabel: '密级', labelWidth: 60,
            triggerAction: "all", store: me.seculevelStore,
            valueField: 'value', editable: false,//不可输入
            displayField: 'text', margin: '10 5 0 10',// 
            anchor: "80%", labelAlign: "right", labelPad:8, width: '28%',//width: 120,//
            emptyText: "--请选择--", autoLoadOnValue: true, value: "受限",
            fieldStyle: 'border-color: red; background-image: none;',//红色边框
            listeners:
            {
                select: function (combo, records, eOpts) {
                    var seculevel = me.seculevelCombo.value;
                    if (seculevel === "公开") {
                        me.secrTermText.setDisabled(true);
                    } else {
                        me.secrTermText.setDisabled(false);
                    }
                    me.cataAttrArray[0].secretgrade = seculevel;
                }
            }
        });

        //添加是否需要回复combo
        Ext.define("needreplyModel", {
            extend: 'Ext.data.Model',
            fields: ["text", "value"]
        });
        me.needreplyProxy = Ext.create("Ext.data.proxy.Memory", {
            data: me.needreplydata,
            model: "needreplyModel"
        });

        me.needreplyStore = Ext.create("Ext.data.Store", {
            model: needreplyModel,
            proxy: me.needreplyProxy
        });


        me.needreplyCombo = Ext.create("Ext.form.field.ComboBox",
        {
            //xtype: "combo",
            fieldLabel: '需要回复', labelWidth: 60,
            triggerAction: "all", store: me.needreplyStore,
            valueField: 'value', editable: false,//不可输入
            displayField: 'text', margin: '10 5 0 10',// 
            anchor: "80%", labelAlign: "right", labelPad:8, width: '20%',// width: 120,//
            emptyText: "--请选择--", autoLoadOnValue: true, value: "是",
            fieldStyle: 'border-color: red; background-image: none;',//红色边框
            listeners:
            {
                select: function (combo, records, eOpts) {
                    var seculevel = me.needreplyCombo.value;
                    if (seculevel === "否") {
                        me.replyDateField.setDisabled(true);
                    } else {
                        me.replyDateField.setDisabled(false);
                    }
                }
            }
        });


        //添加传递方式combo
        Ext.define("transmodeModel", {
            extend: 'Ext.data.Model',
            fields: ["text", "value"]
        });
        me.transmodeProxy = Ext.create("Ext.data.proxy.Memory", {
            data: me.transmodedata,
            model: "transmodeModel"
        });

        me.transmodeStore = Ext.create("Ext.data.Store", {
            model: transmodeModel,
            proxy: me.transmodeProxy
        });

        me.transmodeCombo = Ext.create("Ext.form.field.ComboBox",
        {
            //xtype: "combo",
            fieldLabel: '传递方式', labelWidth: 60,
            triggerAction: "all", store: me.transmodeStore,
            valueField: 'value', editable: false,//不可输入
            displayField: 'text', margin: '10 5 0 10',// 
            anchor: "80%", labelAlign: "right", labelPad: 8, width: '50%',//width: 120,//
            emptyText: "--请选择--", autoLoadOnValue: true, //value: "邮寄",
            fieldStyle: 'border-color: red; background-image: none;',//红色边框
            listeners:
            {
                select: function (combo, records, eOpts) {
                    var transmode = me.transmodeCombo.value;
                    if (transmode === "其他") {
                        me.transmodeSuppText.setDisabled(false);
                    } else {
                        me.transmodeSuppText.setDisabled(true);
                    }
                }
            }
        });

        //添加提交目的combo
        Ext.define("purposeModel", {
            extend: 'Ext.data.Model',
            fields: ["text", "value"]
        });
        me.purposeProxy = Ext.create("Ext.data.proxy.Memory", {
            data: me.purposedata,
            model: "purposeModel"
        });

        me.purposeStore = Ext.create("Ext.data.Store", {
            model: purposeModel,
            proxy: me.purposeProxy
        });

        me.purposeCombo = Ext.create("Ext.form.field.ComboBox",
        {
            //xtype: "combo",
            fieldLabel: '提交目的', labelWidth: 60,
            triggerAction: "all", store: me.purposeStore,
            valueField: 'value', editable: false,//不可输入
            displayField: 'text', margin: '10 5 0 10',// 
            anchor: "80%", labelAlign: "right", labelPad: 8, width: '50%',//width: 120,//
            emptyText: "--请选择--", autoLoadOnValue: true, //value: "邮寄",
            fieldStyle: 'border-color: red; background-image: none;',//红色边框
            listeners:
            {
                select: function (combo, records, eOpts) {
                    var purpose = me.purposeCombo.value;
                    if (purpose === "其他") {
                        me.purposeSuppText.setDisabled(false);
                    } else {
                        me.purposeSuppText.setDisabled(true);
                    }
                }
            }
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
                            {
                                layout: "hbox",
                                width: '50%',
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
                                           
                                              
                                          }
                                      }
                                  }
                                    //me.sendCodeText //发文编码
                                ], flex: 1
                            },
                            {
                                layout: "hbox",
                                width: '50%',
                                align: 'stretch',
                                pack: 'start',
                                baseCls: 'my-panel-no-border',//隐藏边框
                                items: [
                            me.senderText,//发自
                            {
                                xtype: "button",
                                text: "选择...", margins: "10 10 0 0",
                                listeners: {
                                    "click": function (btn, e, eOpts) {//添加点击按钮事件
                                        me.fileCodePanel.sendCompanyButton.fireEvent('click');
                                      
                                    }
                                }
                            }]
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
                            me.sendDateField,//发文日期
                            me.totalPagesText   //页数

                         ], flex: 1
                     },
                     {
                         layout: "hbox",
                         width: '100%',
                         align: 'stretch',
                         pack: 'start',
                         baseCls: 'my-panel-no-border',//隐藏边框
                         items: [
                            me.seculevelCombo,//保密等级
                            me.secrTermText,   //保密期限
                            me.needreplyCombo,  //是否需要回复
                            me.replyDateField  //回复日期 
                         ], flex: 1
                     },
                     {
                         layout: "hbox",
                         width: '100%',
                         align: 'stretch',
                         pack: 'start',
                         baseCls: 'my-panel-no-border',//隐藏边框
                         items: [
                            me.transmodeCombo,//传递方式
                            me.transmodeSuppText
                         ], flex: 1
                     },
                     {
                         layout: "hbox",
                         width: '100%',
                         align: 'stretch',
                         pack: 'start',
                         baseCls: 'my-panel-no-border',//隐藏边框
                         items: [
                            me.purposeCombo,//提交目的
                            me.purposeSuppText
                         ], flex: 1
                     },
                     {
                         layout: "hbox",
                         width: '100%',
                         align: 'stretch',
                         pack: 'start',
                         baseCls: 'my-panel-no-border',//隐藏边框
                         items: [
                            me.titleText//标题

                         ], flex: 1
                     }, {

                         layout: "hbox",
                         width: '100%',
                         align: 'stretch',
                         pack: 'start',
                         height: 80, margin: '0 0 0 0',
                         baseCls: 'my-panel-no-border',//隐藏边框
                         items: [
                              me.contentText  // 函件正文
                         ]//, flex: 1
                     }
            ]
        });

       

        //底部按钮区域
        me.bottomButtonPanel = Ext.create("Ext.panel.Panel", {
            //xtype: "panel",
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

                            winDraftTransmittalCN.close();
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

        //获取打开表单时的默认参数
        //me.sendGetDraftTransmittalCNDefault();

        me.callParent(arguments);

    },

    //获取起草信函表单默认参数
    sendGetDraftTransmittalCNDefault: function (funCallback) {
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
                C: "AVEVA.CDMS.HXEPC_Plugins.Document", A: "GetDraftTransmittalCNDefault",
                sid: localStorage.getItem("sid"), ProjectKeyword: me.projectKeyword,
                DraftOnProject: draftOnProject
            },
            success: function (response, options) {
                me.sendGetDraftTransmittalCNDefault_callback(response, options, funCallback);

            }
        });
    },

    //处理获取发文处理表单默认参数的返回
    sendGetDraftTransmittalCNDefault_callback: function (response, options, funCallback) {
        var me = this;

        //获取数据后，更新窗口
        var res = Ext.JSON.decode(response.responseText, true);
        var state = res.success;
        if (state === true) {
            var recod = eval(res.data[0]);

            var strRootProjectCode = recod.RootProjectCode;
            var strRootProjectDesc = recod.RootProjectDesc;
            var strProjectDesc = recod.ProjectDesc;

            var strFormClassCode = "TRA"

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

            //默认设置为不新建文件编码
            me.fileCodePanel.setNeedNewFileCode(false);

            //设置发起目录和项目所在目录
            me.fileCodePanel.projectKeyword = me.projectKeyword;
            me.fileCodePanel.projectDirKeyword = me.projectKeyword;

            //设置收发文单位的单位类型
            me.fileCodePanel.setFormClass(strFormClassCode, strProjectDesc);
            //设置项目管理类文件里项目的代码和描述
            me.fileCodePanel.setRootProject(strRootProjectCode, strRootProjectDesc);

            if (strRootProjectCode === undefined || strRootProjectCode === "") {
                //运营信函

                me.fileCodePanel.setDocUnitClass("部门", "部门");
                //隐藏发给项目单选框
                //me.fileCodePanel.toProjectCheckBox.setVisible(false);


            } else {
                //项目（非运营）信函
                me.fileCodePanel.setDocUnitClass("项目", "项目");

            }

            //设置文件编码Panel的各个按钮的用户事件
            me.fileCodePanel.AfterSelectRecCompany = function (code, desc) {
                me.mainFeederText.setValue(desc);
            }

            me.fileCodePanel.AfterSelectSendCompany = function (code, desc) {
                me.senderText.setValue(desc);
            }

            //不需要强制输入页数（页数自动生成）
            me.fileCodePanel.needInputPage = false;

            //设置发文单位代码
            me.fileCodePanel.setSendCompany(sourceCompany, sourceCompanyDesc);


            me.fileUploadPanel.setSendCompany(sourceCompany, sourceCompanyDesc);

            //设置项目管理类文件里项目的代码和描述
            me.fileUploadPanel.setRootProject(strRootProjectCode, strRootProjectDesc);

            //设置文件上传表格的模式
            me.fileUploadPanel.setFileGridMode("TRA");


            if (me.isReply === true) {
                //如果是回复信函

                me.mainFeederText.setValue(recUnitDesc);

                me.fileCodePanel.recCompanyText.setValue(recUnitCode);

                me.fileCodePanel.departmentText.setValue(recUnitCode);

                me.fileCodePanel.cataAttrArray[0].department = recUnitCode;

                me.fileCodePanel.sendGetFileId();

            }

            funCallback();
        }
    },


    selectProject: function () {
        var me = this;

        var fmSelectProject = Ext.create('Ext.plug_ins.HXEPC_Plugins.Document.SelectProject', { title: "", mainPanelId: me.Id, projectKeyword: me.projectKeyword });

        winSelectProject = Ext.widget('window', {
            title: '选择项目',
            width: 738,
            height: 558,
            minWidth: 738,
            minHeight: 558,
            layout: 'fit',
            resizable: true,
            modal: true,
            closeAction: 'close', //close 关闭  hide  隐藏  
            items: fmSelectProject,
            defaultFocus: 'firstName'
        });

        fmSelectProject.projectKeyword = me.projectKeyword;

        winSelectProject.show();


        //监听子窗口关闭事件
        winSelectProject.on('close', function () {
            //if (window.parent.resultvalue != null && window.parent.resultvalue !== "") {

            var projectCode = "";
            var projectDesc = "";
            var projectValue = "";

            projectCode = window.parent.resultvalue;
            projectDesc = window.parent.projectdesclist;
            projectValue = window.parent.projectvaluelist;

            if (projectCode.indexOf(",") > 0) {
                // var words = projectCode.split(',')
                projectCode = projectCode.substring(0, projectCode.indexOf(","));
                projectDesc = projectDesc.substring(0, projectDesc.indexOf(";"));
                projectValue = projectValue.substring(0, projectValue.indexOf(","));
            }



            me.projectCodeText.setValue(projectCode);

            me.projectDirKeyword = projectValue;

            me.cataAttrArray[0].procode = projectCode;
            me.cataAttrArray[0].proname = projectDesc;

        });
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

  
        //获取主送Text
        var mainFeeder = me.mainFeederText.value;

        //获取发送方Text
        var sender = me.senderText.value;

        //获取发文日期
        var sendDate = me.sendDateField.value;

        //获取页数Text
        var totalPages = me.totalPagesText.value;

        //获取保密等级Combo
        var seculevel = me.seculevelCombo.value;

        //获取保密期限Text
        var secrTerm = me.secrTermText.value;

        //获取是否需要回复Combo
        var needreply = me.needreplyCombo.value;

        //获取回复日期
        var replyDate = me.replyDateField.value;
        //if (needreply === "否") replyDate = "";

        //获取传递方式combo
        var transmode = me.transmodeCombo.value;

        //获取传递方式补充说明
        var transmodeSupp= me.transmodeSuppText.value;

        //获取提交目的combo
        var purpose = me.purposeCombo.value;

        //获取提交目的补充说明
        var purposeSupp = me.purposeSuppText.value;

        //获取信函主题Text
        var title = me.titleText.value;

        //获取正文内容Text
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
            var rc = record.get('receiptcode');
            var os = record.get('originalshare');
            var cs = record.get('copyshare');
            var ss = record.get('scanshare');
            var es = record.get('elecshare');
            var fr = record.get('note');

            var fa =
                {
                    fn: fn, fc: fc, fd: fd, fp: fp, fe: fe, rc: rc,
                    os: os, cs: cs, ss: ss, es: es, fr: fr
                };

            fileArray.push(fa);

        }

        //获取表单数据，转换成JSON字符串
        var projectAttr =
        [
            { name: 'fileCode', value: fileCode },

            { name: 'documentCode', value: sendCode },
            { name: 'mainFeeder', value: mainFeeder },
            { name: 'sender', value: sender },

            { name: 'sendDate', value: sendDate },
            { name: 'totalPages', value: totalPages },

            { name: 'seculevel', value: seculevel },
            { name: 'secrTerm', value: secrTerm },

            { name: 'needreply', value: needreply },
            { name: 'replyDate', value: replyDate },

            { name: 'transmode', value: transmode },
            { name: 'transmodeSupp', value: transmodeSupp },

            { name: 'purpose', value: purpose },
            { name: 'purposeSupp', value: purposeSupp },

            { name: 'title', value: title },
            { name: 'content', value: content },

             { name: 'fileId', value: fileId },

            { name: 'approvpath', value: approvpath },
            { name: 'nextStateUserList', value: me.approvPathPanel.nextStateUserList },

  
        ];

        var projectAttrJson = Ext.JSON.encode(projectAttr);
        var fileListJson = Ext.JSON.encode(fileArray);
        var cataAttrJson = Ext.JSON.encode(me.fileCodePanel.cataAttrArray);

        Ext.MessageBox.wait("正在生成文件传递单，请稍候...", "等待");

        Ext.Ajax.request({

            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.HXEPC_Plugins.Document", A: "DraftTransmittalCN",
                sid: localStorage.getItem("sid"), ProjectKeyword: me.projectKeyword,
                DocAttrJson: projectAttrJson, CataAttrJson: cataAttrJson,
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
                        
                    }
                } else {
                    var errmsg = res.msg;
                    Ext.Msg.alert("错误信息", errmsg);
                }
            },
            failure: function (response, options) {
                ////Ext.Msg.alert("错误", "连接服务器失败！<br>" + response.responseText);
            }
        })
    },

   
    //处理发送起草函件后的返回事件
    draft_document_callback:function (response, options) {
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
                    winDraftTransmittalCN.close();
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
                    winDraftTransmittalCN.close();

                //展开目录
                Ext.require('Ext.ux.Common.comm', function () {
                    Ext.getCmp('contentPanel').down('_mainProjectTree').ExpendProject(parentKeyword);
                });
            }
        });
    }
});