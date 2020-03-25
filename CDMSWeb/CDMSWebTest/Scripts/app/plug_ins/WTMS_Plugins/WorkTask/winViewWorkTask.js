//新建目录
Ext.define('Ext.plug_ins.WTMS_Plugins.WorkTask.winViewWorkTask', {
    extend: 'Ext.container.Container',
    alias: 'widget.winViewWorkTask',
    //layout: "border",
    //requires: ["Ext.plug_ins.WTMS_Plugins.Api.workTaskApi"],
    layout: 'fit',
    resultvalue: '', mainPanelId: '',
    winWidth: 780,
    winHeight: 390,
    initComponent: function () {
        var me = this;

        //定义workTaskApi网络访问接口
        me.workTaskApi = Ext.create('Ext.plug_ins.WTMS_Plugins.Api.workTaskApi', {});

        //记录著录表属性
        me.cataAttrArray = [{ receiveType: "LET", needNewFileCode: false }];

        //添加事由text
        me.titleText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "发文主题", readOnly: true,
            anchor: "80%", labelWidth: 60, labelAlign: "left", margin: '8 0 8 0', width: 360//flex: 1
        });
        //添加内容Text
        me.contentText = Ext.create("Ext.form.field.TextArea", {
            xtype: "textarea", anchor: "80%", labelWidth: 60, labelAlign: "left", margin: '8 0 8 0', width: 360, readOnly: true,
            fieldLabel: "发文内容", height: 120
        });


        //定义计划开始时间Text
        me.requestResDateText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "要求回复时间", readOnly: true,
            anchor: "80%", labelWidth: 90, labelAlign: "left", margin: '8 0 8 0', width: 360//flex: 1
        });
        //me.requestResDateField = Ext.create("Ext.form.field.Date", {

        //    name: "date",
        //    fieldLabel: ' 要求回复时间', fieldStyle: ' background-image: none;',
        //    editable: false,//禁止手工修改
        //    readOnly: true,
        //    labelWidth: 90, margin: '8 0 8 0',
        //    emptyText: "--请选择--", autoLoadOnValue: true,
        //    format: 'Y年m月d日',
        //    value: new Date(),
        //    width: 280
        //});

        //添加收 文单位代码text
        me.recCompanyText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "收文单位", labelWidth: 60, readOnly: true, emptyText: "收文单位",
            margin: '10 0 0 0', anchor: "80%", labelAlign: "left", flex: 1//width: 280//
        });

        //选择收文单位按钮
        me.recCompanyButton = Ext.create("Ext.button.Button", {
            text: "..", margins: "10 0 0 0",
            listeners: {
                "click": function (btn, e, eOpts) {//添加点击按钮事件

                    me.selectRecUnit();

                }
            }
        });

        me.workTaskPanel = Ext.create("Ext.panel.Panel", {
            xtype: "fieldset", margin: '8',
            layout: {
                type: 'vbox',
                pack: 'start',
                align: 'stretch'
            },
            items: [
                me.fileCodePanel,
                {
                    layout: "hbox",
                    width: '100%',
                    baseCls: 'my-panel-no-border',//隐藏边框
                    align: 'stretch', margin: '8 0 8 0', padding: '0 0 0 0',
                    pack: 'start',
                    items: [me.titleText,
                        me.requestResDateText
                    ]
                },
                me.contentText,
                {

                    baseCls: 'my-panel-no-border',//隐藏边框
                    width: '100%',
                    layout: {
                        type: 'hbox',
                        pack: 'start',
                        align: 'stretch'
                    },
                    items: [
                        me.recCompanyText //, me.recCompanyButton
                    ],
                    flex: 1


                }


            ]
        });

        me.AfterSelectRecCompany = function () { }

        //============================================================
        //添加列表
        me.items = [
          Ext.widget('form', {
              layout: "form",
              items: [{
                  xtype: "panel",
                  baseCls: 'my-panel-no-border',//隐藏边框
                  layout: {
                      type: 'vbox',
                      pack: 'start',
                      align: 'stretch'
                  },
                  items: [
                      {
                          layout: "hbox",
                          width: '100%', baseCls: 'my-panel-no-border',//隐藏边框
                          items: [{ baseCls: 'my-panel-no-border', flex: 1 },
                      {
                          xtype: 'label',
                          cls: 'classDiv2',
                          itemId: 'label1',
                          text: '查看工作任务', margins: '0 0 0 10'
                      }, { baseCls: 'my-panel-no-border', flex: 1 }]
                      }
                  ,
                  me.workTaskPanel,
  
              {
                  xtype: "panel",
                  layout: "hbox",
                  baseCls: 'my-panel-no-border',//隐藏边框
                  //align: 'right',
                  //pack: 'end',//组件在容器右边
                  items: [{
                      flex: 1, baseCls: 'my-panel-no-border'//隐藏边框
                  },
                      {
                          xtype: "button",
                          text: "回复", width: 60, margins: "10 5 10 5",
                          listeners: {
                              "click": function (btn, e, eOpts) {//添加点击按钮事件
                                  //me.workTaskApi.canSay();
                                  //me.send_create_worktask();
                                  me.OnBtnReplyClick();
                              }
                          }
                      },
                      {
                          xtype: "button",
                          text: "取消", width: 60, margins: "10 15 10 5",
                          listeners: {
                              "click": function (btn, e, eOpts) {//添加点击按钮事件
                                  winViewWT.close();
                              }
                          }
                      }
                  ]
              }
                  ]
              }]
          }
          )];


        me.callParent(arguments);
    },


    //选择收文单位
    selectRecUnit: function () {
        var me = this;

        var fmSelectUnit = Ext.create('Ext.plug_ins.WTMS_Plugins.Dist.SelectUnit', { title: "", mainPanelId: me.Id, projectKeyword: me.projectDirKeyword });

        winSelectUnit = Ext.widget('window', {
            title: '选择收文单位',
            width: 738,
            height: 558,
            minWidth: 738,
            minHeight: 580,
            layout: 'fit',
            resizable: true,
            modal: true,
            closeAction: 'close', //close 关闭  hide  隐藏  
            items: fmSelectUnit,
            defaultFocus: 'firstName'
        });

        fmSelectUnit.projectKeyword = me.projectDirKeyword;

        winSelectUnit.show();


        //监听子窗口关闭事件
        winSelectUnit.on('close', function () {
            if (window.parent.resultvalue != null && window.parent.resultvalue !== "") {

                var unitCode = "";
                var unitDesc = "";
                var unitValue = "";

                unitCode = window.parent.resultvalue;
                unitDesc = window.parent.unitdesclist;
                unitValue = window.parent.unitvaluelist;

                ////只选取第一个选项
                //if (unitCode.indexOf(",") > 0) {
                //    // var words = unitCode.split(',')
                //    unitCode = unitCode.substring(0, unitCode.indexOf(","));
                //    unitDesc = unitDesc.substring(0, unitDesc.indexOf(";"));
                //}


                //me.mainFeederText.setValue(unitDesc);

                me.recCompanyText.setValue(unitDesc);//unitCode);
                me.cataAttrArray[0].recUnitCode = unitCode;

                me.recCompanyDesc = unitDesc;

                //me.sendGetFileId();

                me.AfterSelectRecCompany(unitCode, unitDesc);
            }
        });
    },

    //创建D4联系单
    send_create_worktask: function () {
        var me = this;

        var title = me.titleText.value === undefined ? "" : me.titleText.value;
        if (title === "") {
            Ext.Msg.alert("错误信息", "请填写发文标题！");
            return;
        }

        if (title.indexOf("&") > 0) {
            Ext.Msg.alert("系统提示", "文件名称中包含非法字符‘&’！");
            return;
        }

        var content = me.contentText.value === undefined ? "" : me.contentText.value;
        if (content === "") {
            Ext.Msg.alert("错误信息", "请填写发文内容！");
            return;
        }

        //获取发送日期
        var reqResDate = me.requestResDateField.value;

        var projKeyword = me.projectKeyword;

        var docAttr =
            [
                { name: 'TEXT1', value: title },
                { name: 'TEXT2', value: content },
                { name: 'TEXT3', value: reqResDate },
                { name: 'TEXT4', value: me.cataAttrArray[0].recUnitCode }

            ];
        var docAttrJson = Ext.JSON.encode(docAttr);

        //Ext.Msg.alert("错误信息", docAttrJson);

        me.createTask(me, projKeyword, title, docAttrJson);
    },

    createTask: function (me, projKeyword, title, docAttrJson) {

        var self = this;

        Ext.MessageBox.wait("正在生成工作任务，请稍候...", "等待");
        Ext.Ajax.request({
            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.WTMS_Plugins.WorkTask", A: "CreateWorkTask",
                ProjectKeyword: projKeyword,
                title: title, docAttrJson: docAttrJson,
                sid: localStorage.getItem("sid")
            },
            success: function (response, options) {
                //Ext.MessageBox.close();//关闭等待对话框
                //获取数据后，更新窗口
                var res = Ext.JSON.decode(response.responseText, true);
                var state = res.success;
                if (state === false) {
                    var errmsg = res.msg;
                    Ext.Msg.alert("错误信息", errmsg);
                }
                else {
                    Ext.MessageBox.close();//关闭等待对话框

                    var recod = eval(res.data[0]);

                    me.newProjectKeyword = recod.ProjectKeyword;//获取新建的目录id

                    //当没有附件时，处理返回事件
                    self.create_task_callback(response, me.newProjectKeyword);

                }
            },
            failure: function (response, options) {
                //Ext.MessageBox.close();//关闭等待对话框
                Ext.Msg.alert("系统提示", "连接服务器失败，请尝试重新提交！");
            }
        });
    },

    //处理发送起草函件后的返回事件
    create_task_callback: function (response, projectKeyword) {
        var me = this;

        me.refreshWin(projectKeyword, true);

    },

    getViewTaskDefault: function () {

        var me = this;

        //me.projKeyword

        Ext.MessageBox.wait("正在查询工作任务，请稍候...", "等待");
        Ext.Ajax.request({
            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.WTMS_Plugins.WorkTask", A: "GetWorkTaskByKeyword",
                ProjectKeyword: me.projectKeyword,
                sid: localStorage.getItem("sid")
            },
            success: function (response, options) {
                //Ext.MessageBox.close();//关闭等待对话框
                //获取数据后，更新窗口
                var res = Ext.JSON.decode(response.responseText, true);
                var state = res.success;
                if (state === false) {
                    var errmsg = res.msg;
                    Ext.Msg.alert("错误信息", errmsg);
                }
                else {
                    Ext.MessageBox.close();//关闭等待对话框

                    var recod = eval(res.data[0]);


                    me.titleText.setValue(recod.Title);
                    //内容Text
                    me.contentText.setValue( recod.TaskDesc);


                    //计划开始时间Text
                    me.requestResDateText.setValue(recod.ReqResTime);

                    //收文单位代码text
                    me.recCompanyText.setValue(recod.RecUnit);

                }
            },
            failure: function (response, options) {
                //Ext.MessageBox.close();//关闭等待对话框
                Ext.Msg.alert("系统提示", "连接服务器失败，请尝试重新提交！");
            }
        });
    },

    OnBtnReplyClick: function (view, record, item, index, e, eOpts) {
        var me = this;

            var mpanel = Ext.getCmp('mainPanel');

            var keyword = me.projectKeyword;


            var fmReplyWT = Ext.create('Ext.plug_ins.WTMS_Plugins.WorkTask.winReplyWorkTask', { title: "", mainPanelId: "", projectKeyword: keyword, projectDirKeyword: keyword });
            winReplyWT = Ext.widget('window', {
                title: '回复工作任务',
                closeAction: 'hide',
                width: fmReplyWT.winWidth,
                height: fmReplyWT.winHeight,
                minWidth: fmReplyWT.winWidth,
                minHeight: fmReplyWT.winHeight,
                layout: 'fit',
                resizable: true,
                modal: true,
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: fmReplyWT,
                defaultFocus: 'firstName'
            });

            winReplyWT.projectKeyword = keyword;

            //fmReplyWT.getReplyTaskDefault();

            winReplyWT.show();
            //监听子窗口关闭事件
            winReplyWT.on('close', function () {
            });
            
            winViewWT.close();

    },


    //刷新表单，参数:parentKeyword:新建的联系单目录
    refreshWin: function (parentKeyword, closeWin) {
        var me = this;
        var tree = Ext.getCmp(me.mainPanelId).down('treepanel');
        var viewTreeStore = tree.store;

        viewTreeStore.load({
            callback: function (records, options, success) {//添加回调，获取子目录的文件数量
                if (closeWin)
                    winViewWT.close();

                //展开目录
                Ext.require('Ext.ux.Common.comm', function () {
                    Ext.getCmp('contentPanel').down('_mainProjectTree').ExpendProject(parentKeyword);
                });
            }
        });
    }

});