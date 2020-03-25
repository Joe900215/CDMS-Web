Ext.define('Ext.plug_ins.HXPC_Plugins.selectUserEx', {
    extend: 'Ext.container.Container',
    alias: 'widget.selectUserEx',
    layout: 'fit',
    resultvalue: '', mainPanelId: '',
    projectKeyword: '', WfKeyword: '',
    initComponent: function () {
        var me = this;

        me.dragUser = "";
        me.mouseDownGrid = "";

        //定义选择校核审核人员表格
        //定义选择校核审核人员表格的model
        Ext.define("selectusermodel", {
            extend: "Ext.data.Model",
            fields: [
                "workflowKeyword",
                "projectName",
                "taskName",
                "workflowName",
                "checker",
                "auditor"
            ],
            url: "_blank",
        });

        //定义选择校核审核人员表格的store
        me.selectuserstore = Ext.create("Ext.data.Store", {
            model: "selectusermodel"
        });
        //定义选择校核审核人员表格的view
        me.selectusergrid = Ext.widget("grid", {
            region: "center",
            height: 68,
            stateful: true,
            multiSelect: true,
            //hideHeaders: true,//隐藏表头
            flex: 1,
            store: me.selectuserstore,
            viewConfig: {
                stripeRows: true,
                enableTextSelection: false
                //getRowClass: function () {
                //    // 在这里添加自定样式 改变这个表格的行高
                //    return 'x-grid-row upload-file-grid-row';
                //}
            },
            columns: [
                { text: '项目名称', dataIndex: 'projectName', width: 120 },
                { text: '任务名称', dataIndex: 'taskName', width: 100 },
                { text: '流程名称', dataIndex: 'workflowName', width: 100 },
                { text: '校核人', dataIndex: 'checker', width: 120 },
                { text: '审核人', dataIndex: 'auditor', width: 120 }
            ],
            listeners: {
                 itemmousedown: function (view, record, item, index, e, eOpts) {
                    //Ext.Msg.alert("错误信息", record.data.userName);
                    me.mouseDownGrid = "selectUserGrid";
                    //me.dragUser = record.data.userName;
                 },
                 itemmouseup:function (view, record, item, index, e, eOpts) {
                     if (me.mouseDownGrid === "userListGrid")
                     {
                         me.mouseDownGrid = "";

                         //获取列的索引
                         var colIndex = e.target.parentNode.cellIndex;
                         //var storeData = me.selectusergrid.getStore().data;
                         
                         var mData = me.selectusergrid.getStore().data.items[index];//.data;

                         if (colIndex === 3)
                         {
                             //当拖放到校核人单元格

                             //mData.checker = me.dragUser;
                             mData.set("checker", me.dragUser);
                             mData.commit();
                             //me.selectusergrid.getStore().data.items[index].data.checker=me.dragUser;
                         } else if (colIndex === 4) {
                             //当拖放到审核人单元格
                             //me.selectusergrid.getStore().data.items[index].data.auditor = me.dragUser;
                             mData.set("auditor", me.dragUser);
                             mData.commit();
                         }

                     }
                 }
            }
        });


        //定义选择校核审核人员表格的model
        Ext.define("userlistmodel", {
            extend: "Ext.data.Model",
            fields: ["userName",
                "professionOwner",
                "designer",
                "exchanger",
                "checker",
                "auditor"
            ],
            url: "_blank",
        });

        //定义选择校核审核人员表格的store
        me.userliststore = Ext.create("Ext.data.Store", {
            model: "userlistmodel"
        });
        //定义选择校核审核人员表格的view
        me.userlistgrid = Ext.widget("grid", {
            region: "center",
            height: 68,
            //hideHeaders: true,//隐藏表头
            flex: 1,
            store: me.userliststore,
            columns: [
                { text: '姓名', dataIndex: 'userName', width: 120 },
                { text: '主设', dataIndex: 'professionOwner', width: 60 },
                { text: '设计', dataIndex: 'designer', width: 60 },
                { text: '互提', dataIndex: 'exchanger', width: 60 },
                { text: '校核', dataIndex: 'checker', width: 60 },
                { text: '审核', dataIndex: 'auditor', width: 60 }
            ],
            listeners: {
                itemmousedown: function (view, record, item, index, e, eOpts)
                {
                    //Ext.Msg.alert("错误信息", record.data.userName);
                    me.mouseDownGrid = "userListGrid";
                    me.dragUser = record.data.userName;
                }
            }
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
                          //{
                          //    layout: "hbox",
                          //    width: '100%', baseCls: 'my-panel-no-border',//隐藏边框
                          //    items: [{ baseCls: 'my-panel-no-border', flex: 1 },
                          //    {
                          //        xtype: 'label',
                          //        cls: 'classDiv2',
                          //        itemId: 'label1',
                          //        text: '专 业 间 互 提 资 料 单', margins: '0 0 0 10'
                          //    }, { baseCls: 'my-panel-no-border', flex: 1 }]
                          //},
                          {
                              //xtype: "fieldset", margin: '8 16 0 16',
                              xtype: "panel",
                              baseCls: 'my-panel-no-border',//隐藏边框
                              layout: {
                                  type: 'hbox',
                                  pack: 'start',
                                  align: 'stretch'
                              },
                              height:550,
                              items: [
                                 {
                                     xtype: "fieldset",// margin: '8 16 0 16',
                                     //baseCls: 'my-panel-no-border',//隐藏边框
                                     margin: '8', title: "选择校核审核人员",
                                     layout: {
                                         type: 'hbox',
                                         pack: 'start',
                                         align: 'stretch'
                                     },
                                     items: [
                                         me.selectusergrid
                                     ],flex:1
                                 }, {
                                     xtype: "fieldset",// margin: '8 16 0 16',
                                     //baseCls: 'my-panel-no-border',//隐藏边框
                                     margin: '8', title: "科室人员列表",
                                     layout: {
                                         type: 'hbox',
                                         pack: 'start',
                                         align: 'stretch'
                                     },width: 460,
                                     items: [
                                         me.userlistgrid
                                     ]
                                 }
                              ],
                              flex: 1
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
                                            me.sendSelectUserEx();

                                        }
                                    }
                                },
                                {
                                    xtype: "button",
                                    text: "取消", width: 60, margins: "0 15 10 5",
                                    listeners: {
                                        "click": function (btn, e, eOpts) {//添加点击按钮事件
                                            //Ext.Msg.alert("您展开了目录树节点！！！", "您展开了目录树节点！节点ID:" + me.tempDefnId);
                                            winSelectUserEx.close();
                                        }
                                    }
                                }
                            ]
                        }
                      ]
                  }
              ]
          })];
        me.sendGetSelectUserExDefault();
        me.callParent(arguments);
    },

    //获取主设人选择校审核人员的默认参数
    sendGetSelectUserExDefault: function(response, options) {
        var me = this;
        //通过extjs的ajax获取操作全部名称
        Ext.Ajax.request({
            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.HXPC_Plugins.SelectUser", A: "GetSelectUserExDefault",
                sid: localStorage.getItem("sid"), WorkFlowKeyword: me.WfKeyword
            },
            success: function (response, options) {
                me.sendGetSelectUserExDefault_callback(response, options);//, funCallback);
            }
        });
    },

    //处置获取主设人选择校审核人员的默认参数的返回
    sendGetSelectUserExDefault_callback: function (response, options) {
        var me = this;

        //获取数据后，更新窗口
        var res = Ext.JSON.decode(response.responseText, true);
        var state = res.success;
        if (state === true) {
            var recod = eval(res.data[0]);

            var workFlowList = eval(recod.WorkflowList);
            var userList = eval(recod.UserList);

            var str = "";
            for (var itemKey in workFlowList) {
                me.projectKeyword = workFlowList[itemKey].projectKeyword;
                if (workFlowList[itemKey].projectName != undefined) {
                    str = str + workFlowList[itemKey].projectName;
                    //插入行到文件selectUserGrid
                    var r = Ext.create('selectusermodel', {
                        //name: files[i].name
                        workflowKeyword: workFlowList[itemKey].workflowKeyword,
                        projectName: workFlowList[itemKey].projectName,
                        taskName: workFlowList[itemKey].docName,
                        workflowName: workFlowList[itemKey].workflowName,
                        checker: "",
                        auditor: ""
                    });

                    var rowlength = me.selectusergrid.getStore().data.length;
                    me.selectusergrid.getStore().insert(rowlength, r);
                }
            }

            for (var itemKey in userList) {
                if (userList[itemKey].userName != undefined) {
                    str = str + userList[itemKey].userName;
                    //插入行到文件selectUserGrid
                    var r = Ext.create('userlistmodel', {
                        //name: files[i].name

                        userName: userList[itemKey].userName,
                        professionOwner: userList[itemKey].ProfessionOwnerCount,
                        designer: userList[itemKey].DesignerCount,
                        exchanger:"",
                        checker: userList[itemKey].CheckerCount,
                        auditor: userList[itemKey].AuditorCount
                    });

                    var rowlength = me.userlistgrid.getStore().data.length;
                    me.userlistgrid.getStore().insert(rowlength, r);
                }
            }
        }
    },

    //响应主设人选择校审核人员的确定按钮
    sendSelectUserEx: function (response, options) {
        var me = this;

        //获取表单数据，转换成JSON字符串
        var workflowAttr = [];

        //var store = grid.getStore();

        for (var i = 0; i < me.selectuserstore.getCount() ; i++) {
            var dataItem = me.selectuserstore.getAt(i).data; //遍历每一行
            if (dataItem.checker != undefined && dataItem.checker != "" && dataItem.auditor != undefined && dataItem.auditor != "") {
                workflowAttr.push({
                    workflowKeyword: dataItem.workflowKeyword,
                    checker: dataItem.checker,
                    auditor: dataItem.auditor
                });//在数组里添加新的元素  
            }
        }
        ////获取项目开发阶段Combo
        //var developmentPhase = me.developmentPhaseCombo.value;

        ////获取表单数据，转换成JSON字符串
        //var workflowAttr =
        //[
        //    { name: 'projectCode', value: projectCode }

        //];

        var workflowAttrJson = Ext.JSON.encode(workflowAttr);

        Ext.MessageBox.wait("正在提交设定校审核人员，请稍候...", "等待");

        Ext.Ajax.request({

            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.HXPC_Plugins.SelectUser", A: "SelectUserEx",
                sid: localStorage.getItem("sid"),
                workflowAttrJson: workflowAttrJson
            },
            success: function (response, options) {
                //me.create_root_project_callback(response, options, "");//, me.projectKeyword, closeWin);


                //获取数据后，更新窗口
                var res = Ext.JSON.decode(response.responseText, true);
                var state = res.success;
                if (state === true) {

                    Ext.MessageBox.close();//关闭等待对话框

                    var recod = eval(res.data[0]);
                    
                    me.refreshWin(recod.projectKeyword, true);
                } else {
                    var errmsg = res.msg;
                    Ext.Msg.alert("错误信息", errmsg);
                    winExchangeDoc.close();
                }
            }
        })

    },

    //刷新表单，参数:parentKeyword:要转到的的目录
    refreshWin: function (projKeyword, closeWin) {
        var me = this;

        //调用流程页事件，刷新父控件内容
        Ext.getCmp(me.mainPanelId).refreshMainPanle(projKeyword, function () {
            if (closeWin)
                winSelectUserEx.close();
        });
    }

    ////刷新表单，参数:parentKeyword:新建的目录
    //refreshWin: function (projKeyword, closeWin) {
    //    var me = this;
    //    var tree = Ext.getCmp(me.mainPanelId).up('_mainSourceView').down('_mainProjectTree').down('treepanel');//;
    //    var viewTreeStore = tree.store;

    //    viewTreeStore.load({
    //        callback: function (records, options, success) {//添加回调，获取子目录的文件数量
    //            if (closeWin)
    //                winSelectUserEx.close();

    //            //展开目录
    //            Ext.require('Ext.ux.Common.comm', function () {
    //                Ext.getCmp('contentPanel').down('_mainProjectTree').ExpendProject(projKeyword);
    //            });
    //        }
    //    });
    //}
});