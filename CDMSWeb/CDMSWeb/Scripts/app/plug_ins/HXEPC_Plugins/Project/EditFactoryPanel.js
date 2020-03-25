/// <reference path="EditFactoryPanel.js" />
Ext.define('Ext.plug_ins.HXEPC_Plugins.Project.EditFactoryPanel', {
    //extend: 'Ext.container.Container',
    extend: 'Ext.panel.Panel',//'Ext.tab.Panel',//
    alias: 'widget.EditFactoryPanel', // 此类的xtype类型为buttontransparent  
    title: "厂房",
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

        //添加厂房编码text
        me.FactoryCodeText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "厂房编码", labelWidth: 70, margins: '15 0 0 0',
            anchor: "80%", labelAlign: "left", width: 150//flex: 1
        });

        //添加厂房名称text
        me.FactoryDescText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "厂房名称", labelWidth: 70, margins: '15 0 0 0',
            anchor: "80%", labelAlign: "left", width: 150//flex: 1
        });

        //添加厂房名称text
        me.FactoryEngDescText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "英文名称", labelWidth: 70, margins: '15 0 0 0',
            anchor: "80%", labelAlign: "left", width: 150//flex: 1
        });

        //定义选择公司表格
        //定义选择公司表格的model
        Ext.define("factorylistmodel", {
            extend: "Ext.data.Model",
            fields: [
                "factoryId",
                "factoryCode",
                "factoryDesc",
                "factoryEngDesc"
            ],
            url: "_blank",
        });

        //定义选择公司表格的store
        me.factoryliststore = Ext.create("Ext.data.Store", {
            model: "factorylistmodel"
        });
        //定义选择公司表格的view
        me.factorylistgrid = Ext.widget("grid", {
            region: "center",
            height: 68,
            stateful: true,
            multiSelect: true,
            //hideHeaders: true,//隐藏表头
            flex: 1,
            store: me.factoryliststore,
            viewConfig: {
                stripeRows: true,
                enableTextSelection: false
                //getRowClass: function () {
                //    // 在这里添加自定样式 改变这个表格的行高
                //    return 'x-grid-row upload-file-grid-row';
                //}
            },
            columns: [
                { text: '编码', dataIndex: 'factoryCode', width: 60 },
                { text: '名称', dataIndex: 'factoryDesc', width: 150 },
                { text: '英文名称', dataIndex: 'factoryEngDesc', width: 150 }
            ],
            listeners: {
                itemmousedown: function (view, record, item, index, e, eOpts) {

                },
                select: function (view, record, index, eOpts) {

                    //设置编辑控件可编辑
                    me.set_edit_panel_disabled(false);

                    me.deleteFactoryButton.setDisabled(false);

                    me.saveFactoryButton.setDisabled(false);

                    if (me.createFactoryButton.text === "确定")
                        me.createFactoryButton.setText("新建");

                    if (me.cancelButton.disabled === false)
                        me.cancelButton.setDisabled(true);

                    // me.createProjectButton.setDisabled(false);

                    me.onFactoryGridSelect(view, record, index, eOpts);
                },
                itemdblclick: function (view, record, item, index, e, eOpts) {
                    //me.send_createFactoryProject();
                }
            }
        });

        me.batCreateFactoryButton = Ext.create("Ext.button.Button", {
            text: "批量新建", width: 60, margins: "10 5 10 5",
            listeners: {
                "click": function (btn, e, eOpts) {//添加点击按钮事件
                    me.bat_new_factory();
                }
            }
        });

        me.createFactoryButton = Ext.create("Ext.button.Button", {
            text: "新建", width: 60, margins: "10 5 10 5",
            listeners: {
                "click": function (btn, e, eOpts) {//添加点击按钮事件
                    me.new_factory();
                }
            }
        });

        me.deleteFactoryButton = Ext.create("Ext.button.Button", {
            text: "删除", width: 60, margins: "10 5 10 5", disabled: true,
            listeners: {
                "click": function (btn, e, eOpts) {//添加点击按钮事件
                    me.send_deleteFactory(false);
                }
            }
        });

        me.saveFactoryButton = Ext.create("Ext.button.Button", {
            text: "保存", width: 60, margins: "10 5 10 5", disabled: true,
            listeners: {
                "click": function (btn, e, eOpts) {//添加点击按钮事件
                    me.send_editFactory(false);
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
                                      title: '厂房列表',
                                      layout: {
                                          type: 'hbox',
                                          pack: 'start',
                                          align: 'stretch'
                                      },
                                      width: 420, height: 420,
                                      items: [
                                            me.factorylistgrid
                                      ]
                                  }, {
                                      xtype: "fieldset", margin: '0 0 0 0',
                                      //baseCls: 'my-panel-no-border',//隐藏边框
                                      title: '厂房资料',
                                      layout: {
                                          type: 'vbox',
                                          pack: 'start',
                                          align: 'stretch'
                                      },
                                      flex: 1,
                                      items: [
                                          me.ProjectCodeText,
                                          me.ProjectDescText,
                                          me.FactoryCodeText,
                                          me.FactoryDescText,
                                          me.FactoryEngDescText

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
                          me.batCreateFactoryButton,
                          me.createFactoryButton,
                          me.deleteFactoryButton,
                          me.saveFactoryButton,
                          me.cancelButton
                      ]
                  }
                      ]
                  }
              ]
          })];

        //var fminfo = Ext.getCmp(me.Id).up('editProjectInfo');
        //me.projectKeyword = fminfo .projectKeyword;
        //me.sendEditFactoryDefault();
        me.callParent(arguments);
    },

    //获取新建厂房资料目录表单默认参数
    sendEditFactoryDefault: function () {
        var me = this;

        me.set_edit_panel_disabled(true);

        //通过extjs的ajax获取操作全部名称
        Ext.Ajax.request({
            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.HXEPC_Plugins.HXProject", A: "GetEditFactoryDefault",
                sid: localStorage.getItem("sid"), ProjectKeyword: me.projectKeyword
            },
            success: function (response, options) {
                me.sendGetEditFactoryDefault_callback(response, options);//, funCallback);
            },
            failure: function (response, options) {
                ////Ext.Msg.alert("错误", "连接服务器失败！<br>" + response.responseText);
            }
        });
    },

    //处置获取新建厂房资料目录表单默认参数的返回
    sendGetEditFactoryDefault_callback: function (response, options) {
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

            var factoryList = eval(recod.FactoryList);

            me.factorylistgrid.store.removeAll();

            for (var itemKey in factoryList) {
                //me.projectKeyword = factoryList[itemKey].factoryCode;
                if (factoryList[itemKey].factoryCode != undefined) {
                    //插入行到文件selectUserGrid
                    var r = Ext.create('factorylistmodel', {
                        factoryId: factoryList[itemKey].factoryId,
                        factoryCode: factoryList[itemKey].factoryCode,
                        factoryDesc: factoryList[itemKey].factoryDesc,
                        factoryEngDesc: factoryList[itemKey].factoryEngDesc
                    });

                    var rowlength = me.factorylistgrid.getStore().data.length;
                    me.factorylistgrid.getStore().insert(rowlength, r);
                }
            }
        }
    },


    //新建公司资料目录
    send_editFactory: function (isCreate) {
        var me = this;

        me.IsCreate = isCreate;


        //获取公司编号Text
        var factoryCode = me.FactoryCodeText.value;

        //获取公司名称Text
        var factoryDesc = me.FactoryDescText.value;

        var FactoryEngDesc = me.FactoryEngDescText.value;


        ////获取表单数据，转换成JSON字符串
        var factoryAttr =
        [
            { name: 'factoryId', value: me.curFactoryId },
            { name: 'factoryCode', value: factoryCode },
            { name: 'factoryDesc', value: factoryDesc },
            { name: 'factoryEngDesc', value: FactoryEngDesc }


        ];

        var factoryAttrJson = Ext.JSON.encode(factoryAttr);

        Ext.MessageBox.wait("正在创建参建单位，请稍候...", "等待");

        var A = isCreate ? "CreateFactory" : "EDITFactory";

        Ext.Ajax.request({

            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.HXEPC_Plugins.HXProject", A: A,
                sid: localStorage.getItem("sid"), ProjectKeyword: me.projectKeyword,
                factoryAttrJson: factoryAttrJson
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
                    //me.send_editFactory_callback(me.newProjectKeyword, options, "");//, me.projectKeyword, closeWin);

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
    send_createFactoryProject_callback: function (projectKeyword, options) {
        var me = this;



        me.refreshWin(me.newProjectKeyword, true);
    },

    //批量新建系统
    bat_new_factory: function () {
        var me = this;


        //弹出操作窗口
        var _fmBatchCreateFactory = Ext.create('Ext.plug_ins.HXEPC_Plugins.Project.BatchCreateFactory', { title: "" });

        winBatchCreateFactory = Ext.widget('window', {
            title: '批量创建厂房',
            closeAction: 'hide',
            width: 800,
            height: 596,
            minWidth: 300,
            minHeight: 300,
            layout: 'fit',
            resizable: true,
            modal: true,
            closeAction: 'close', //close 关闭  hide  隐藏  
            items: _fmBatchCreateFactory,
            defaultFocus: 'firstName'
        });


        winBatchCreateFactory.show();
        //监听子窗口关闭事件
        winBatchCreateFactory.on('close', function () {

           me.sendEditFactoryDefault();
        });

        _fmBatchCreateFactory.mainPanelId = me.factorylistgrid.id;
        var projectKeyword = me.projectKeyword;//[0].data.Keyword;
        _fmBatchCreateFactory.projectKeyword = projectKeyword;


    },

    new_factory: function () {
        var me = this;

        ///响应新建用户按钮
        if (me.createFactoryButton.getText() === "新建") {

            var userTypeIndex = 0;
            var userStateIndex = 0;

            me.FactoryCodeText.setValue("");
            me.FactoryDescText.setValue("");
            me.FactoryEngDescText.setValue("");

            //设置编辑控件可以编辑
            me.set_edit_panel_disabled(false);

            me.createFactoryButton.setText("确定");
            
            me.deleteFactoryButton.setDisabled(true);

            me.saveFactoryButton.setDisabled(true);
            me.cancelButton.setDisabled(false);

            // me.createProjectButton.setDisabled(true);
            // me.getSignPic();
        } else {
            //向服务器发送新建用户请求
            me.send_editFactory(true);
        }
    },

    cancel_select: function () {
        var me = this;

        me.curFactoryId = "";
        //设置编辑控件不可编辑
        me.set_edit_panel_disabled(true);

        me.createFactoryButton.setText("新建");
        //me.delUserButton.setDisabled(true);

        me.deleteFactoryButton.setDisabled(true);

        me.saveFactoryButton.setDisabled(true);
        me.cancelButton.setDisabled(true);

        // me.createProjectButton.setDisabled(true);

        me.factorylistgrid.getSelectionModel().clearSelections();
        me.factorylistgrid.getView().refresh();


    },

    set_edit_panel_disabled: function (flag) {
        var me = this;

        if (me.FactoryCodeText.disabled != flag)
            me.FactoryCodeText.setDisabled(flag);

        if (me.FactoryDescText.disabled != flag)
            me.FactoryDescText.setDisabled(flag);

        if (me.FactoryEngDescText.disabled != flag)
            me.FactoryEngDescText.setDisabled(flag);


    },

    refreshPanel: function () {
        var me = this;

        if (me.IsCreate) {

            me.cancel_select();
            me.sendEditFactoryDefault();
            //me.factorylistgrid.store.loadPage(1);
        } else {
            var grid = me.factorylistgrid;
            var rs = grid.getSelectionModel().getSelection();//获取选择的文档

            if (!(rs !== null && rs.length > 0)) {
                return;
            }

            var strFactoryCode = me.FactoryCodeText.value.trim();
            var strFactoryDesc = me.FactoryDescText.value.trim();
            var strFactoryEngDesc = me.FactoryEngDescText.value.trim();
            if (rs[0].data.factoryCode != strFactoryCode) {
                rs[0].data.factoryCode = strFactoryCode;//获取文档关键字

            }
            if (rs[0].data.factoryDesc != strFactoryDesc) {
                rs[0].data.factoryDesc = strFactoryDesc;//获取文档关键字

            }
            if (rs[0].data.factoryEngDesc != strFactoryEngDesc) {
                rs[0].data.factoryEngDesc = strFactoryEngDesc;//获取文档关键字
            }
            me.factorylistgrid.getView().refresh();
        }
    },

    //响应选择公司列表表格的事件
    onFactoryGridSelect: function (view, record, index, eOpts) {
        var me = this;

        me.FactoryCodeText.setValue(record.data.factoryCode);
        me.FactoryDescText.setValue(record.data.factoryDesc);
        me.FactoryEngDescText.setValue(record.data.factoryEngDesc);

        me.curFactoryId = record.data.factoryId;
    },



    setProjectKeyword: function (projKeyword) {
        var me = this;
        me.projectKeyword = projKeyword;
    },

    //删除厂房资料
    send_deleteFactory: function () {
        var me = this;

        //var viewTree = me._targetGroupUserPanel.down('treepanel');
        var nodes = me.factorylistgrid.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {
            Ext.MessageBox.show({
                title: '提示',
                msg: '是否删除选中的厂房？' + me.projectKeyword,
                buttons: Ext.MessageBox.YESNO,
                buttonText: {
                    yes: "是",
                    no: "否"
                },
                fn: function (btn) {
                    if (btn === "yes") {

                        var factoryId = nodes[0].data.factoryId;
                        var factoryCode = nodes[0].data.factoryCode;

                        Ext.MessageBox.wait("正在删除厂房，请稍候...", "等待");

                        Ext.Ajax.request({

                            url: 'WebApi/Post',
                            method: "POST",
                            params: {
                                C: "AVEVA.CDMS.HXEPC_Plugins.HXProject", A: "DeleteFactory",
                                sid: localStorage.getItem("sid"), ProjectKeyword: me.projectKeyword,
                                FactoryId: factoryId, FactoryCode: factoryCode
                            },
                            success: function (response, options) {

                                //获取数据后，更新窗口
                                var res = Ext.JSON.decode(response.responseText, true);
                                var state = res.success;
                                if (state === true) {

                                    Ext.MessageBox.close();//关闭等待对话框

                                    //var recod = eval(res.data[0]);
                                    me.factorylistgrid.store.remove(nodes[0]);

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
            Ext.Msg.alert("错误", "请选择厂房！");
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
                    winEditFactory.close();

                //展开目录
                Ext.require('Ext.ux.Common.comm', function () {
                    Ext.getCmp('contentPanel').down('_mainProjectTree').ExpendProject(projKeyword);
                });
            }
        });
    }
});