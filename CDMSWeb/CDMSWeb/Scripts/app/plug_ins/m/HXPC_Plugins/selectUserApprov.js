Ext.define('Ext.plug_ins.m.HXPC_Plugins.selectUserApprov', {
    extend: 'Ext.container.Container',
    alias: 'widget.selectUserApprov',
    layout: 'fit', 
    resultvalue: '', mainPanelId: '',
    projectKeyword: '', WfKeyword: '',
    initComponent: function () {
        var me = this;

        me.dragUser = "";
        me.mouseDownGrid = "";

        //显示的列数 
        me.selectusergridColsCount = 4;

        //定义选择校核审核人员表格
        //定义选择校核审核人员表格的model
        Ext.define("selectusermodel", {
            extend: "Ext.data.Model",
            fields: [
                "workflowKeyword",
                "projectName",
                "taskName",
                "workflowName",
                "checkerName",
                "auditorName",
                "approverName",
                "checker",
                "auditor",
                "approver"
            ],
            url: "_blank",
        });

        //定义选择校核审核人员表格的store
        me.selectuserstore = Ext.create("Ext.data.Store", {
            model: "selectusermodel"
        });


        me.btnright = Ext.create('Ext.button.Button', {
            iconCls: "arrow-right", text: "查看流程名称", scope: me, tooltip: '', listeners: {
                "click": function (btn, e, eOpts) {
                    //  me.CreateNewDoc();
                    if (me.selectusergrid.columns[0].hidden === false) {
                        me.selectusergrid.columns[0].hide();
                        me.selectusergrid.columns[1].show();
                        me.btnright.setText("查看流程名称");
                    } else if (me.selectusergrid.columns[1].hidden === false) {
                        me.selectusergrid.columns[1].hide();
                        me.selectusergrid.columns[2].show();
                        me.btnright.setText("查看项目名称");
                    } else if (me.selectusergrid.columns[2].hidden === false) {
                        me.selectusergrid.columns[2].hide();
                        me.selectusergrid.columns[0].show();
                        me.btnright.setText("查看任务名称");
                    }
                }
            }
        });

        //定义目录树按钮
        me.selusergridTbar = Ext.create('Ext.toolbar.Toolbar', {
            xtype: 'toolbar',
            dock: 'top',
            items: [
                me.btnright
            ]
        });

        //定义选择校核审核人员表格的view
        me.selectusergrid = Ext.widget("grid", {
            region: "center",
            height: 68,
            stateful: true,
            multiSelect: true,
            //hideHeaders: true,//隐藏表头
            tbar: me.selusergridTbar,
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
                { text: '项目名称', dataIndex: 'projectName', hidden: true, flex: 1 },// width: 120 },
                { text: '任务名称', dataIndex: 'taskName', flex: 1 },//width: 100 },
                { text: '流程名称', dataIndex: 'workflowName', hidden: true, flex: 1 },//width: 100 },
                { text: '校核人', dataIndex: 'checkerName', width: 60 },
                { text: '审核人', dataIndex: 'auditorName', width: 60 },
                { text: '批准人', dataIndex: 'approverName', width: 60 },
                { text: '校核人', dataIndex: 'checker', hidden: true, width: 60 },
                { text: '审核人', dataIndex: 'auditor', hidden: true, width: 60 },
                { text: '批准人', dataIndex: 'approver', hidden: true, width: 60 }
            ],
            listeners: {
                'itemmousedown': function (view, record, item, index, e, eOpts) {

                    me.mouseDownGrid = "selectUserGrid";
                    //me.dragUser = record.data.userName;
                },
                'itemmouseup': function (view, record, item, index, e, eOpts) {
                    //if (me.mouseDownGrid === "userListGrid") {
                    if (me.dragUser != "")
                    {
                        me.mouseDownGrid = "";

                        //获取列的索引
                        var colIndex = e.target.parentNode.cellIndex;
                        //var storeData = me.selectusergrid.getStore().data;

                        var mData = me.selectusergrid.getStore().data.items[index];//.data;

                        //if (colIndex === 3) {
                        if ((me.selectusergridColsCount === 6 && colIndex === 3) || (me.selectusergridColsCount === 4 && colIndex === 1)){
                            //当拖放到校核人单元格

                            var cNameIndex = me.dragUser.indexOf("__");
                            var strCheckerName = (cNameIndex >= 0) ? me.dragUser.substring(cNameIndex + 2) : me.dragUser;

                            mData.set("checkerName", strCheckerName);

                            mData.set("checker", me.dragUser);
                            mData.commit();
                            //me.selectusergrid.getStore().data.items[index].data.checker=me.dragUser;
                        //} else if (colIndex === 4) {
                        } else if ((me.selectusergridColsCount === 6 && colIndex === 4) || (me.selectusergridColsCount === 4 && colIndex === 2)) {
                            //当拖放到审核人单元格
                            var aNameIndex = me.dragUser.indexOf("__");
                            var strAuditorName = (aNameIndex >= 0) ? me.dragUser.substring(aNameIndex + 2) : me.dragUser;

                            mData.set("auditorName", strAuditorName);
                            mData.set("auditor", me.dragUser);
                            mData.commit();
                        //} else if (colIndex === 5) {
                        } else if ((me.selectusergridColsCount === 6 && colIndex === 5) || (me.selectusergridColsCount === 4 && colIndex === 3)) {
                            //当拖放到审核人单元格
                            var cNameIndex = me.dragUser.indexOf("__");
                            var strApproverName = (cNameIndex >= 0) ? me.dragUser.substring(cNameIndex + 2) : me.dragUser;

                            mData.set("approverName", strApproverName);

                            mData.set("approver", me.dragUser);
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
                "auditor",
                "approver"
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
                { text: '姓名', dataIndex: 'userName', width: 110 },
                { text: '主设', dataIndex: 'professionOwner', width: 60 },
                { text: '设计', dataIndex: 'designer', width: 60 },
                { text: '互提', dataIndex: 'exchanger', width: 60 },
                { text: '校核', dataIndex: 'checker', width: 60 },
                { text: '审核', dataIndex: 'auditor', width: 60 },
                { text: '批准', dataIndex: 'approver', width: 60 }
            ],
            listeners: {
                'itemmousedown': function (view, record, item, index, e, eOpts) {
                    //Ext.Msg.alert("错误信息", record.data.userName);
                    me.mouseDownGrid = "userListGrid";
                    me.dragUser = record.data.userName;
                },
                'itemmouseup': function (view, record, item, index, e, eOpts) {
                    me.mouseDownGrid = "userListGrid";
                }
            }
        });

        //添加列表
        me.items = [

                  {
                      xtype: "panel", margins: "2 2 2 2", padding: 5,
                      baseCls: 'my-panel-no-border',//隐藏边框
                      layout: {
                          type: 'vbox',
                          pack: 'start',
                          align: 'stretch'
                      }, flex: 1,
                      items: [
                          {
                              //xtype: "fieldset", margin: '8 16 0 16',
                              xtype: "panel",
                              baseCls: 'my-panel-no-border',//隐藏边框
                              layout: {
                                  type: 'vbox',
                                  pack: 'start',
                                  align: 'stretch'
                              },
                              height: '100%',
                              items: [
                                 {
                                     xtype: "fieldset",// margin: '8 16 0 16',
                                     //baseCls: 'my-panel-no-border',//隐藏边框
                                     margin: '8', title: "选择校核审核人员",
                                     layout: {
                                         type: 'hbox',
                                         pack: 'start',
                                         align: 'stretch'
                                     }, padding: '2 5 2 5',
                                     items: [
                                         me.selectusergrid
                                     ], flex: 1
                                 }, {
                                     xtype: "fieldset",// margin: '8 16 0 16',
                                     //baseCls: 'my-panel-no-border',//隐藏边框
                                     margin: '8', title: "科室人员列表",
                                     //layout: {
                                     //    type: 'hbox',
                                     //    pack: 'start',
                                     //    align: 'stretch'
                                    // },
                                     layout: 'fit', padding: '2 5 2 5',
                                     items: [
                                         me.userlistgrid
                                     ], flex: 1
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
                                    text: "确定", width: 60, margin: "0 5 10 5",
                                    listeners: {
                                        "click": function (btn, e, eOpts) {//添加点击按钮事件
                                            me.sendSelectUserApprov();

                                        }
                                    }
                                },
                                {
                                    xtype: "button",
                                    text: "取消", width: 60, margin: "0 15 10 5",
                                    listeners: {
                                        "click": function (btn, e, eOpts) {//添加点击按钮事件
                                            //Ext.Msg.alert("您展开了目录树节点！！！", "您展开了目录树节点！节点ID:" + me.tempDefnId);
                                            winSelectUserApprov.close();
                                        }
                                    }
                                }
                            ]
                        }
                      ]
                  }
        ]
        ///    })]
        ;
        me.sendGetSelectUserApprovDefault();
        me.callParent(arguments);
    },

    //获取主设人选择校审核人员的默认参数
    sendGetSelectUserApprovDefault: function (response, options) {
        var me = this;
        //通过extjs的ajax获取操作全部名称
        Ext.Ajax.request({
            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.HXPC_Plugins.SelectUser", A: "GetSelectUserApprovDefault",
                sid: localStorage.getItem("sid"), WorkFlowKeyword: me.WfKeyword
            },
            success: function (response, options) {
                me.sendGetSelectUserApprovDefault_callback(response, options);//, funCallback);
            }
        });
    },

    //处置获取主设人选择校审核人员的默认参数的返回
    sendGetSelectUserApprovDefault_callback: function (response, options) {
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
                        auditor: "",
                        approver: ""
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
                        exchanger: "",
                        checker: userList[itemKey].CheckerCount,
                        auditor: userList[itemKey].AuditorCount,
                        approver: userList[itemKey].ApprovCount
                    });

                    var rowlength = me.userlistgrid.getStore().data.length;
                    me.userlistgrid.getStore().insert(rowlength, r);
                }
            }
        }
    },

    //响应主设人选择校审核人员的确定按钮
    sendSelectUserApprov: function (response, options) {
        var me = this;

        //获取表单数据，转换成JSON字符串
        var workflowAttr = [];

        //var store = grid.getStore();

        for (var i = 0; i < me.selectuserstore.getCount() ; i++) {
            var dataItem = me.selectuserstore.getAt(i).data; //遍历每一行
            if (dataItem.checker != undefined && dataItem.checker != "" && dataItem.auditor != undefined && dataItem.auditor != "" && dataItem.approver != undefined && dataItem.approver != "") {
                workflowAttr.push({
                    workflowKeyword: dataItem.workflowKeyword,
                    checker: dataItem.checker,
                    auditor: dataItem.auditor,
                    approver: dataItem.approver
                });//在数组里添加新的元素  
            }
        }

        var workflowAttrJson = Ext.JSON.encode(workflowAttr);

        Ext.MessageBox.wait("正在提交设定校审核人员，请稍候...", "等待");

        Ext.Ajax.request({

            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.HXPC_Plugins.SelectUser", A: "SelectUserApprov",
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
                winSelectUserApprov.close();
        });
    }


});