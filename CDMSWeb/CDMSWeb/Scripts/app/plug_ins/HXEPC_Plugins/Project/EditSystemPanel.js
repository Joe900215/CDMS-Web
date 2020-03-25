Ext.define('Ext.plug_ins.HXEPC_Plugins.Project.EditSystemPanel', {
    //extend: 'Ext.container.Container',
    extend: 'Ext.panel.Panel',//'Ext.tab.Panel',//
    alias: 'widget.EditSystemPanel', // 此类的xtype类型为buttontransparent  
    title: "系统",
    layout: {
        type: 'vbox',
        pack: 'start',
        align: 'stretch'
    },

    height: '100%', projectKeyword: '',
    //GroupType:'Org',Filter:'',
    baseCls: 'my-panel-no-border',//隐藏边框
    initComponent: function () {
        var me = this;
        me.renderTo = me.el;

        ////定义用户组选择Panel
        //  me._selectGroupUserPanel = Ext.create('Ext.ux.YDForm.User._SelectUserGroupPanel', { GroupType: 'Project' });

        //添加项目代码text
        me.ProjectCodeText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "项目代码", labelWidth: 70, margins: '15 0 0 0',
            readOnly: true, fieldStyle: ' background-color: #DFE9F6;border-color: #DFE9F6; background-image: none;',
            anchor: "80%", labelAlign: "left", width: 150//flex: 1
        });

        //添加项目名称text
        me.ProjectDescText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "项目名称", labelWidth: 70, margins: '15 0 0 0',
            readOnly: true, fieldStyle: ' background-color: #DFE9F6;border-color: #DFE9F6; background-image: none;',
            anchor: "80%", labelAlign: "left", width: 150//flex: 1
        });

        //添加系统编码text
        me.SystemCodeText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "系统编码", labelWidth: 70, margins: '15 0 0 0',
            anchor: "80%", labelAlign: "left", width: 150//flex: 1
        });

        //添加系统名称text
        me.SystemDescText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "系统名称", labelWidth: 70, margins: '15 0 0 0',
            anchor: "80%", labelAlign: "left", width: 150//flex: 1
        });

        //添加系统名称text
        me.SystemEngDescText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "英文名称", labelWidth: 70, margins: '15 0 0 0',
            anchor: "80%", labelAlign: "left", width: 150//flex: 1
        });

        //定义选择公司表格
        //定义选择公司表格的model
        Ext.define("systemlistmodel", {
            extend: "Ext.data.Model",
            fields: [
                "systemId",
                "systemCode",
                "systemDesc",
                "systemEngDesc"
            ],
            url: "_blank",
        });

        //定义选择公司表格的store
        me.systemliststore = Ext.create("Ext.data.Store", {
            model: "systemlistmodel"
        });
        //定义选择公司表格的view
        me.systemlistgrid = Ext.widget("grid", {
            region: "center",
            height: 68,
            stateful: true,
            multiSelect: true,
            //hideHeaders: true,//隐藏表头
            flex: 1,
            store: me.systemliststore,
            viewConfig: {
                stripeRows: true,
                enableTextSelection: false
                //getRowClass: function () {
                //    // 在这里添加自定样式 改变这个表格的行高
                //    return 'x-grid-row upload-file-grid-row';
                //}
            },
            columns: [
                { text: '编码', dataIndex: 'systemCode', width: 60 },
                { text: '名称', dataIndex: 'systemDesc', width: 150 },
                { text: '英文名称', dataIndex: 'systemEngDesc', width: 150 }
            ],
            listeners: {
                itemmousedown: function (view, record, item, index, e, eOpts) {

                },
                select: function (view, record, index, eOpts) {

                    //设置编辑控件可编辑
                    me.set_edit_panel_disabled(false);

                    me.deleteSystemButton.setDisabled(false);

                    me.saveSystemButton.setDisabled(false);

                    if (me.createSystemButton.text === "确定")
                        me.createSystemButton.setText("新建");

                    if (me.cancelButton.disabled === false)
                        me.cancelButton.setDisabled(true);

                    // me.createProjectButton.setDisabled(false);

                    me.onSystemGridSelect(view, record, index, eOpts);
                },
                itemdblclick: function (view, record, item, index, e, eOpts) {
                    //me.send_createSystemProject();
                }
            }
        });

        me.batCreateSystemButton = Ext.create("Ext.button.Button", {
            text: "批量新建", width: 60, margins: "10 5 10 5",
            listeners: {
                "click": function (btn, e, eOpts) {//添加点击按钮事件
                    me.bat_new_system();
                }
            }
        });

        me.createSystemButton = Ext.create("Ext.button.Button", {
            text: "新建", width: 60, margins: "10 5 10 5",
            listeners: {
                "click": function (btn, e, eOpts) {//添加点击按钮事件
                    me.new_system();
                }
            }
        });

        me.deleteSystemButton = Ext.create("Ext.button.Button", {
            text: "删除", width: 60, margins: "10 5 10 5", disabled: true,
            listeners: {
                "click": function (btn, e, eOpts) {//添加点击按钮事件
                    me.send_deleteSystem(false);
                }
            }
        });

        me.saveSystemButton = Ext.create("Ext.button.Button", {
            text: "保存", width: 60, margins: "10 5 10 5", disabled: true,
            listeners: {
                "click": function (btn, e, eOpts) {//添加点击按钮事件
                    me.send_editSystem(false);
                }
            }
        });

        me.cancelButton = Ext.create("Ext.button.Button", {
            text: "取消", width: 60, margins: "10 5 10 5", disabled: true,
            listeners: {
                "click": function (btn, e, eOpts) {//添加点击按钮事件

                    me.cancel_select();
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
                          {
                              xtype: "fieldset", margin: '8 16 8 16',
                              baseCls: 'my-panel-no-border',//隐藏边框
                              layout: {
                                  type: 'hbox',
                                  pack: 'start',
                                  align: 'stretch'
                              },
                              items: [
                                  {
                                      xtype: "fieldset", margin: '0 8 0 0',
                                      //baseCls: 'my-panel-no-border',//隐藏边框
                                      title: '系统列表',
                                      layout: {
                                          type: 'hbox',
                                          pack: 'start',
                                          align: 'stretch'
                                      },
                                      width: 420, height: 420,
                                      items: [
                                            me.systemlistgrid
                                      ]
                                  }, {
                                      xtype: "fieldset", margin: '0 0 0 0',
                                      //baseCls: 'my-panel-no-border',//隐藏边框
                                      title: '系统资料',
                                      layout: {
                                          type: 'vbox',
                                          pack: 'start',
                                          align: 'stretch'
                                      },
                                      flex: 1,
                                      items: [
                                          me.ProjectCodeText,
                                          me.ProjectDescText,
                                          me.SystemCodeText,
                                          me.SystemDescText,
                                          me.SystemEngDescText

                                      ]
                                  }
                              ]
                          },
                  {
                      xtype: "panel",
                      layout: "hbox",
                      baseCls: 'my-panel-no-border',//隐藏边框
                      //align: 'right',
                      //pack: 'end',//组件在容器右边
                      margins: "0 15 0 15",
                      items: [
                          //me.createProjectButton,
                          {
                              flex: 1, baseCls: 'my-panel-no-border'//隐藏边框
                          },
                          me.batCreateSystemButton,
                          me.createSystemButton,
                          me.deleteSystemButton,
                          me.saveSystemButton,
                          me.cancelButton
                      ]
                  }
                      ]
                  }
              ]
          })];

        //var fminfo = Ext.getCmp(me.Id).up('editProjectInfo');
        //me.projectKeyword = fminfo .projectKeyword;
        //me.sendEditSystemDefault();
        me.callParent(arguments);
    },

    //获取新建系统资料目录表单默认参数
    sendEditSystemDefault: function () {
        var me = this;

        me.set_edit_panel_disabled(true);

        //通过extjs的ajax获取操作全部名称
        Ext.Ajax.request({
            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.HXEPC_Plugins.HXProject", A: "GetEditSystemDefault",
                sid: localStorage.getItem("sid"), ProjectKeyword: me.projectKeyword
            },
            success: function (response, options) {
                me.sendGetEditSystemDefault_callback(response, options);//, funCallback);
            },
            failure: function (response, options) {
                ////Ext.Msg.alert("错误", "连接服务器失败！<br>" + response.responseText);
            }
        });
    },

    //处置获取新建系统资料目录表单默认参数的返回
    sendGetEditSystemDefault_callback: function (response, options) {
        var me = this;

        //获取数据后，更新窗口
        var res = Ext.JSON.decode(response.responseText, true);
        var state = res.success;
        if (state === true) {
            var recod = eval(res.data[0]);

            var projCode = recod.projectCode;
            var projDesc = recod.projectDesc;

            me.ProjectCodeText.setValue(projCode);
            me.ProjectDescText.setValue(projDesc);

            var systemList = eval(recod.SystemList);

            me.systemlistgrid.store.removeAll();

            for (var itemKey in systemList) {
                //me.projectKeyword = systemList[itemKey].systemCode;
                if (systemList[itemKey].systemCode != undefined) {
                    //插入行到文件selectUserGrid
                    var r = Ext.create('systemlistmodel', {
                        systemId: systemList[itemKey].systemId,
                        systemCode: systemList[itemKey].systemCode,
                        systemDesc: systemList[itemKey].systemDesc,
                        systemEngDesc: systemList[itemKey].systemEngDesc
                    });

                    var rowlength = me.systemlistgrid.getStore().data.length;
                    me.systemlistgrid.getStore().insert(rowlength, r);
                }
            }
        }
    },


    //新建公司资料目录
    send_editSystem: function (isCreate) {
        var me = this;

        me.IsCreate = isCreate;


        //获取公司编号Text
        var systemCode = me.SystemCodeText.value;

        //获取公司名称Text
        var systemDesc = me.SystemDescText.value;

        var SystemEngDesc = me.SystemEngDescText.value;


        ////获取表单数据，转换成JSON字符串
        var systemAttr =
        [
            { name: 'systemId', value: me.curSystemId },
            { name: 'systemCode', value: systemCode },
            { name: 'systemDesc', value: systemDesc },
            { name: 'systemEngDesc', value: SystemEngDesc }


        ];

        var systemAttrJson = Ext.JSON.encode(systemAttr);

        Ext.MessageBox.wait("正在创建参建单位，请稍候...", "等待");

        var A = isCreate ? "CreateSystem" : "EDITSystem";

        Ext.Ajax.request({

            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.HXEPC_Plugins.HXProject", A: A,
                sid: localStorage.getItem("sid"), ProjectKeyword: me.projectKeyword,
                systemAttrJson: systemAttrJson
            },
            success: function (response, options) {

                //获取数据后，更新窗口
                var res = Ext.JSON.decode(response.responseText, true);
                var state = res.success;
                if (state === true) {

                    Ext.MessageBox.close();//关闭等待对话框

                    var recod = eval(res.data[0]);

                    //me.newProjectKeyword = recod.ProjectKeyword;//获取新建的目录id


                    //Ext.Msg.alert("提示", "保存成功!");

                    me.refreshPanel();
                    //处理返回事件
                    //me.send_editSystem_callback(me.newProjectKeyword, options, "");//, me.projectKeyword, closeWin);

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

    //处理新建公司资料目录的返回事件
    send_createSystemProject_callback: function (projectKeyword, options) {
        var me = this;



        me.refreshWin(me.newProjectKeyword, true);
    },

    //批量新建系统
    bat_new_system: function () {
        var me = this;


        //弹出操作窗口
        var _fmBatchCreateSystem = Ext.create('Ext.plug_ins.HXEPC_Plugins.Project.BatchCreateSystem', { title: "" });

        winBatchCreateSystem = Ext.widget('window', {
            title: '批量创建系统',
            closeAction: 'hide',
            width: 800,
            height: 596,
            minWidth: 300,
            minHeight: 300,
            layout: 'fit',
            resizable: true,
            modal: true,
            closeAction: 'close', //close 关闭  hide  隐藏  
            items: _fmBatchCreateSystem,
            defaultFocus: 'firstName'
        });


        winBatchCreateSystem.show();
        //监听子窗口关闭事件
        winBatchCreateSystem.on('close', function () {

            me.sendEditSystemDefault();
        });

        _fmBatchCreateSystem.mainPanelId = me.systemlistgrid.id;
        var projectKeyword = me.projectKeyword;//[0].data.Keyword;
        _fmBatchCreateSystem.projectKeyword = projectKeyword;


    },

    //新建系统
    new_system: function () {
        var me = this;

        ///响应新建用户按钮
        if (me.createSystemButton.getText() === "新建") {

            var userTypeIndex = 0;
            var userStateIndex = 0;

            me.SystemCodeText.setValue("");
            me.SystemDescText.setValue("");
            me.SystemEngDescText.setValue("");

            //设置编辑控件可以编辑
            me.set_edit_panel_disabled(false);

            me.createSystemButton.setText("确定");
            me.deleteSystemButton.setDisabled(true);
            me.saveSystemButton.setDisabled(true);
            me.cancelButton.setDisabled(false);

            // me.createProjectButton.setDisabled(true);
            // me.getSignPic();
        } else {
            //向服务器发送新建用户请求
            me.send_editSystem(true);
        }
    },

    cancel_select: function () {
        var me = this;

        me.curSystemId = "";
        //设置编辑控件不可编辑
        me.set_edit_panel_disabled(true);

        me.createSystemButton.setText("新建");
       
        me.deleteSystemButton.setDisabled(true);

        me.saveSystemButton.setDisabled(true);
        me.cancelButton.setDisabled(true);

        // me.createProjectButton.setDisabled(true);

        me.systemlistgrid.getSelectionModel().clearSelections();
        me.systemlistgrid.getView().refresh();


    },

    set_edit_panel_disabled: function (flag) {
        var me = this;

        if (me.SystemCodeText.disabled != flag)
            me.SystemCodeText.setDisabled(flag);

        if (me.SystemDescText.disabled != flag)
            me.SystemDescText.setDisabled(flag);

        if (me.SystemEngDescText.disabled != flag)
            me.SystemEngDescText.setDisabled(flag);


    },

    refreshPanel: function () {
        var me = this;

        if (me.IsCreate) {

            me.cancel_select();
            me.sendEditSystemDefault();
            //me.systemlistgrid.store.loadPage(1);
        } else {
            var grid = me.systemlistgrid;
            var rs = grid.getSelectionModel().getSelection();//获取选择的文档

            if (!(rs !== null && rs.length > 0)) {
                return;
            }

            var strSystemCode = me.SystemCodeText.value.trim();
            var strSystemDesc = me.SystemDescText.value.trim();
            var strSystemEngDesc = me.SystemEngDescText.value.trim();
            if (rs[0].data.systemCode != strSystemCode) {
                rs[0].data.systemCode = strSystemCode;//获取文档关键字

            }
            if (rs[0].data.systemDesc != strSystemDesc) {
                rs[0].data.systemDesc = strSystemDesc;//获取文档关键字

            }
            if (rs[0].data.systemEngDesc != strSystemEngDesc) {
                rs[0].data.systemEngDesc = strSystemEngDesc;//获取文档关键字
            }
            me.systemlistgrid.getView().refresh();
        }
    },

    //响应选择公司列表表格的事件
    onSystemGridSelect: function (view, record, index, eOpts) {
        var me = this;

        me.SystemCodeText.setValue(record.data.systemCode);
        me.SystemDescText.setValue(record.data.systemDesc);
        me.SystemEngDescText.setValue(record.data.systemEngDesc);

        me.curSystemId = record.data.systemId;
    },

    setProjectKeyword: function (projKeyword) {
        var me = this;
        me.projectKeyword = projKeyword;
    },

    //删除公司资料
    send_deleteSystem: function () {
        var me = this;

        //var viewTree = me._targetGroupUserPanel.down('treepanel');
        var nodes = me.systemlistgrid.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {
            Ext.MessageBox.show({
                title: '提示',
                msg: '是否删除选中的系统？' + me.projectKeyword,
                buttons: Ext.MessageBox.YESNO,
                buttonText: {
                    yes: "是",
                    no: "否"
                },
                fn: function (btn) {
                    if (btn === "yes") {

                        var systemId = nodes[0].data.systemId;
                        var systemCode = nodes[0].data.systemCode;

                        Ext.MessageBox.wait("正在删除系统，请稍候...", "等待");

                        Ext.Ajax.request({

                            url: 'WebApi/Post',
                            method: "POST",
                            params: {
                                C: "AVEVA.CDMS.HXEPC_Plugins.HXProject", A: "DeleteSystem",
                                sid: localStorage.getItem("sid"), ProjectKeyword: me.projectKeyword,
                                SystemId: systemId, SystemCode: systemCode
                            },
                            success: function (response, options) {

                                //获取数据后，更新窗口
                                var res = Ext.JSON.decode(response.responseText, true);
                                var state = res.success;
                                if (state === true) {

                                    Ext.MessageBox.close();//关闭等待对话框

                                    //var recod = eval(res.data[0]);
                                    me.systemlistgrid.store.remove(nodes[0]);

                                    me.refreshPanel();

                                    me.cancel_select();

                                } else {
                                    var errmsg = res.msg;
                                    Ext.Msg.alert("错误信息", errmsg);
                                }
                            },
                            failure: function (response, options) {
                                Ext.MessageBox.close();//关闭等待对话框
                            }
                        });
                    }
                }

            });
        } else {
            Ext.Msg.alert("错误", "请选择系统！");
        }
    },

    //刷新表单，参数:parentKeyword:新建的联系单目录
    refreshWin: function (projKeyword, closeWin) {
        var me = this;
        var tree = Ext.getCmp(me.mainPanelId).up('_mainSourceView').down('_mainProjectTree').down('treepanel');//;
        var viewTreeStore = tree.store;

        viewTreeStore.load({
            callback: function (records, options, success) {//添加回调，获取子目录的文件数量
                if (closeWin)
                    winEditSystem.close();

                //展开目录
                Ext.require('Ext.ux.Common.comm', function () {
                    Ext.getCmp('contentPanel').down('_mainProjectTree').ExpendProject(projKeyword);
                });
            }
        });
    }
});