Ext.define('Ext.plug_ins.m.HXPC_Plugins.createTask', {
    extend: 'Ext.container.Container',
    alias: 'widget.createTask',
    layout: 'fit',
    resultvalue: '', mainPanelId: '',
    projectKeyword: '', WfKeyword: '',
    initComponent: function () {
        var me = this;

        me.newProjectKeyword = "";

        //添加工作任务名称text
        me.nameText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",  labelWidth: 0,
            anchor: "80%", labelAlign: "right", width: 100//flex: 1
        });

        //添加工作任务内容Text
        me.contentText = Ext.create("Ext.form.field.TextArea", {
            xtype: "textarea", anchor: "80%", labelWidth: 0, labelAlign: "right", margin: '8 0 5 0', width: 360,
            //fieldLabel: "内容",
            height: 120
        });

        ////责任人 
        //me.taskOwnerList = "";
        //me.taskOwnerText = Ext.create("Ext.form.field.Text", {
        //    xtype: "textfield", readOnly: true, //fieldStyle: 'border-color: red; background-image: none;',
        //    fieldLabel: "责任人", anchor: "80%", labelWidth: 55, labelAlign: "left", margin: '0 0 0 16', width: 260//flex: 1
        //});

        //定义任务开始时间Text
        me.taskStartDateField = Ext.create("Ext.form.field.Date", {

            name: "date",
            fieldLabel: '任务开始时间', fieldStyle: ' background-image: none;',
            editable: true, labelWidth: 80, margin: '0 5 0 0',
            emptyText: "--请选择--", autoLoadOnValue: true,
            format: 'Y年m月d日',
            value: new Date(),
            width: 230
        });


        //定义任务结束时间Text
        me.taskEndDateField = Ext.create("Ext.form.field.Date", {

            name: "date",
            fieldLabel: ' 任务结束时间', fieldStyle: ' background-image: none;',
            editable: true, labelWidth: 80, margin: '5 5 0 0',
            emptyText: "--请选择--", autoLoadOnValue: true,
            format: 'Y年m月d日',
            value: new Date(),
            width: 230
        });

        //添加列表
        me.items = [
          Ext.widget('form', {
              layout: "form",
              items: [

                  {
                      xtype: "panel",
                      baseCls: 'my-panel-no-border',//隐藏边框
                      layout: {
                          type: 'vbox',
                          pack: 'start',
                          align: 'stretch'
                      },
                      items: [
                          {
                              xtype: "fieldset", margin: '8 5 0 5',
                              title:"工作任务名称",
                            //baseCls: 'my-panel-no-border',//隐藏边框
                            layout: {
                                type: 'vbox',
                                pack: 'start',
                                align: 'stretch'
                            },
                            items: [me.nameText]
                          }, {
                              xtype: "fieldset", margin: '8 5 0 5',
                              title: "工作任务内容",
                              //baseCls: 'my-panel-no-border',//隐藏边框
                              layout: {
                                  type: 'vbox',
                                  pack: 'start',
                                  align: 'stretch'
                              },
                              items: [me.contentText]
                          },
                        //{
                        //    layout: "hbox",
                        //    width: '100%', baseCls: 'my-panel-no-border',//隐藏边框
                        //    align: 'stretch', margin: '10 0 10 0', padding: '0 0 0 0',
                        //    pack: 'start',
                        //    items: [me.taskOwnerText, {
                        //        xtype: "button",
                        //        text: "选择...", margins: "0 0 0 10",
                        //        listeners: {
                        //            "click": function (btn, e, eOpts) {//添加点击按钮事件

                        //                Ext.require('Ext.ux.Common.m.comm', function () {
                        //                    showSelectUserWin("getUser", "", "", function () {
                        //                        me.taskOwnerText.setValue(window.parent.usernamelist);
                        //                        me.taskOwnerList = window.parent.resultvalue;
                        //                    });
                        //                })
                        //            }
                        //        }
                        //    }]
                        //},
                        {
                            xtype: "panel",
                            margin: '12 5 14 5',
                            baseCls: 'my-panel-no-border',//隐藏边框
                            layout: {
                                type: 'vbox',
                                pack: 'start',
                                align: 'stretch'
                            },
                            items: [
                                me.taskStartDateField,
                                me.taskEndDateField
                            ]
                        },
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
                                    text: "确定", width: 60, margins: "0 5 10 5",
                                    listeners: {
                                        "click": function (btn, e, eOpts) {//添加点击按钮事件
                                            me.sendCreateTask();

                                        }
                                    }
                                },
                                {
                                    xtype: "button",
                                    text: "取消", width: 60, margins: "0 15 10 5",
                                    listeners: {
                                        "click": function (btn, e, eOpts) {//添加点击按钮事件
                                            //Ext.Msg.alert("您展开了目录树节点！！！", "您展开了目录树节点！节点ID:" + me.tempDefnId);
                                            winCreateTask.close();
                                        }
                                    }
                                }
                            ]
                        }
              ]
          }
          ]
    })];

      me.callParent(arguments);
    },

    //创建工作任务
    sendCreateTask:function(){
        var me = this;

        //获取工作任务名称Text
        var taskName = me.nameText.value;

        //获取工作任务内容Text
        var taskContent = me.contentText.value;

        //获取责任人Text
        //var taskOwner = me.taskOwnerList;

        //获取任务开始时间Text
        var taskStartDate = me.taskStartDateField.value;

        //获取任务结束时间Text
        var taskEndDate = me.taskEndDateField.value;

        //获取表单数据，转换成JSON字符串
        var docAttr =
        [
            { name: 'taskName', value: taskName },
            { name: 'taskContent', value: taskContent },
            { name: 'taskStartDate', value: taskStartDate },
            { name: 'taskEndDate', value: taskEndDate }
        ];


        var docAttrJson = Ext.JSON.encode(docAttr);

        Ext.MessageBox.wait("正在创建工作任务，请稍候...", "等待");

        Ext.Ajax.request({

            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.HXPC_Plugins.WorkTask", A: "CreateTask",
                sid: localStorage.getItem("sid"), ProjectKeyword: me.projectKeyword,
                docAttrJson: docAttrJson
            },
            success: function (response, options) {
                me.createTask_callback(response, options, "");//, me.projectKeyword, closeWin);
            }
        })
    },

    //处理创建工作任务的返回事件
    createTask_callback: function (response, options) {
        var me = this;

        //获取数据后，更新窗口
        var res = Ext.JSON.decode(response.responseText, true);
        var state = res.success;
        if (state === true) {
            Ext.MessageBox.close();//关闭等待对话框
            var recod = eval(res.data[0]);
            me.newProjectKeyword = recod.ProjectKeyword;//获取新建的目录id

            me.refreshWin(me.newProjectKeyword, true);

        } else {
            var errmsg = res.msg;
            Ext.Msg.alert("错误信息", errmsg);
        }
    },

    //刷新表单，参数:parentKeyword:新建的联系单目录
    refreshWin: function (projKeyword, closeWin) {
        var me = this;
        var tree = Ext.getCmp(me.mainPanelId).up('_mainProjectView').down('_mainProjectTree').down('treepanel');//;
        var viewTreeStore = tree.store;

        viewTreeStore.load({
            callback: function (records, options, success) {//添加回调，获取子目录的文件数量
                if (closeWin)
                    winCreateTask.close();

                //展开目录
                Ext.require('Ext.ux.Common.m.comm', function () {
                    Ext.getCmp('contentPanel').down('_mainProjectTree').ExpendProject(projKeyword);
                });
            }
        });
    }
});