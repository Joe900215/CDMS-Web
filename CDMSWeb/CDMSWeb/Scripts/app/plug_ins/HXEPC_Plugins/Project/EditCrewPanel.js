Ext.define('Ext.plug_ins.HXEPC_Plugins.Project.EditCrewPanel', {
    //extend: 'Ext.container.Container',
    extend: 'Ext.panel.Panel',//'Ext.tab.Panel',//
    alias: 'widget.EditCrewPanel', // 此类的xtype类型为buttontransparent  
    title: "机组",
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

        //添加机组编码text
        me.CrewCodeText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "机组编码", labelWidth: 70, margins: '15 0 0 0',
            anchor: "80%", labelAlign: "left", width: 150//flex: 1
        });

        //添加机组名称text
        me.CrewDescText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "机组名称", labelWidth: 70, margins: '15 0 0 0',
            anchor: "80%", labelAlign: "left", width: 150//flex: 1
        });

        //添加机组名称text
        me.CrewEngDescText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "英文名称", labelWidth: 70, margins: '15 0 0 0',
            anchor: "80%", labelAlign: "left", width: 150//flex: 1
        });

        //定义选择公司表格
        //定义选择公司表格的model
        Ext.define("crewlistmodel", {
            extend: "Ext.data.Model",
            fields: [
                "crewId",
                "crewCode",
                "crewDesc",
                "crewEngDesc"
            ],
            url: "_blank",
        });

        //定义选择公司表格的store
        me.crewliststore = Ext.create("Ext.data.Store", {
            model: "crewlistmodel"
        });
        //定义选择公司表格的view
        me.crewlistgrid = Ext.widget("grid", {
            region: "center",
            height: 68,
            stateful: true,
            multiSelect: true,
            //hideHeaders: true,//隐藏表头
            flex: 1,
            store: me.crewliststore,
            viewConfig: {
                stripeRows: true,
                enableTextSelection: false
                //getRowClass: function () {
                //    // 在这里添加自定样式 改变这个表格的行高
                //    return 'x-grid-row upload-file-grid-row';
                //}
            },
            columns: [
                { text: '编码', dataIndex: 'crewCode', width: 60 },
                { text: '名称', dataIndex: 'crewDesc', width: 150 },
                { text: '英文名称', dataIndex: 'crewEngDesc', width: 150 }
            ],
            listeners: {
                itemmousedown: function (view, record, item, index, e, eOpts) {

                },
                select: function (view, record, index, eOpts) {

                    //设置编辑控件可编辑
                    me.set_edit_panel_disabled(false);

                    me.saveCrewButton.setDisabled(false);

                    me.deleteCrewButton.setDisabled(false);

                    if (me.createCrewButton.text === "确定")
                        me.createCrewButton.setText("新建");

                    if (me.cancelButton.disabled === false)
                        me.cancelButton.setDisabled(true);

                   // me.createProjectButton.setDisabled(false);

                    me.onCrewGridSelect(view, record, index, eOpts);
                },
                itemdblclick: function (view, record, item, index, e, eOpts) {
                    //me.send_createCrewProject();
                }
            }
        });

        me.batCreateCrewButton = Ext.create("Ext.button.Button", {
            text: "批量新建", width: 60, margins: "10 5 10 5",
            listeners: {
                "click": function (btn, e, eOpts) {//添加点击按钮事件
                    me.bat_new_crew();
                }
            }
        });

        me.createCrewButton = Ext.create("Ext.button.Button", {
            text: "新建", width: 60, margins: "10 5 10 5",
            listeners: {
                "click": function (btn, e, eOpts) {//添加点击按钮事件
                    me.new_crew();
                }
            }
        });

        me.deleteCrewButton = Ext.create("Ext.button.Button", {
            text: "删除", width: 60, margins: "10 5 10 5", disabled: true,
            listeners: {
                "click": function (btn, e, eOpts) {//添加点击按钮事件
                    me.send_deleteCrew(false);
                }
            }
        });

        me.saveCrewButton = Ext.create("Ext.button.Button", {
            text: "保存", width: 60, margins: "10 5 10 5", disabled: true,
            listeners: {
                "click": function (btn, e, eOpts) {//添加点击按钮事件
                    me.send_editCrew(false);
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
                                      title: '机组列表',
                                      layout: {
                                          type: 'hbox',
                                          pack: 'start',
                                          align: 'stretch'
                                      },
                                      width: 420, height: 420,
                                      items: [
                                            me.crewlistgrid
                                      ]
                                  }, {
                                      xtype: "fieldset", margin: '0 0 0 0',
                                      //baseCls: 'my-panel-no-border',//隐藏边框
                                      title: '机组资料',
                                      layout: {
                                          type: 'vbox',
                                          pack: 'start',
                                          align: 'stretch'
                                      },
                                      flex: 1,
                                      items: [
                                          me.ProjectCodeText,
                                          me.ProjectDescText,
                                          me.CrewCodeText,
                                          me.CrewDescText,
                                          me.CrewEngDescText

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
                          me.batCreateCrewButton,
                          me.createCrewButton,
                          me.deleteCrewButton,
                          me.saveCrewButton,
                          me.cancelButton
                      ]
                  }
                      ]
                  }
              ]
          })];

        //var fminfo = Ext.getCmp(me.Id).up('editProjectInfo');
        //me.projectKeyword = fminfo .projectKeyword;
        //me.sendEditCrewDefault();
        me.callParent(arguments);
    },

    //获取新建机组资料目录表单默认参数
    sendEditCrewDefault: function () {
        var me = this;

        me.set_edit_panel_disabled(true);

        //通过extjs的ajax获取操作全部名称
        Ext.Ajax.request({
            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.HXEPC_Plugins.HXProject", A: "GetEditCrewDefault",
                sid: localStorage.getItem("sid"), ProjectKeyword: me.projectKeyword
            },
            success: function (response, options) {
                me.sendGetEditCrewDefault_callback(response, options);//, funCallback);
            },
            failure: function (response, options) {
                ////Ext.Msg.alert("错误", "连接服务器失败！<br>" + response.responseText);
            }
        });
    },

    //处置获取新建机组资料目录表单默认参数的返回
    sendGetEditCrewDefault_callback: function (response, options) {
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

            var crewList = eval(recod.CrewList);

            me.crewlistgrid.store.removeAll();

            for (var itemKey in crewList) {
                //me.projectKeyword = crewList[itemKey].crewCode;
                if (crewList[itemKey].crewCode != undefined) {
                    //插入行到文件selectUserGrid
                    var r = Ext.create('crewlistmodel', {
                        crewId: crewList[itemKey].crewId,
                        crewCode: crewList[itemKey].crewCode,
                        crewDesc: crewList[itemKey].crewDesc,
                        crewEngDesc: crewList[itemKey].crewEngDesc
                    });

                    var rowlength = me.crewlistgrid.getStore().data.length;
                    me.crewlistgrid.getStore().insert(rowlength, r);
                }
            }
        }
    },


    //新建公司资料目录
    send_editCrew: function (isCreate) {
        var me = this;

        me.IsCreate = isCreate;


        //获取公司编号Text
        var crewCode = me.CrewCodeText.value;

        //获取公司名称Text
        var crewDesc = me.CrewDescText.value;

        var CrewEngDesc = me.CrewEngDescText.value;


        ////获取表单数据，转换成JSON字符串
        var crewAttr =
        [
            { name: 'crewId', value: me.curCrewId },
            { name: 'crewCode', value: crewCode },
            { name: 'crewDesc', value: crewDesc },
            { name: 'crewEngDesc', value: CrewEngDesc }
        ];

        var crewAttrJson = Ext.JSON.encode(crewAttr);

        Ext.MessageBox.wait("正在创建参建单位，请稍候...", "等待");

        var A = isCreate ? "CreateCrew" : "EDITCrew";

        Ext.Ajax.request({

            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.HXEPC_Plugins.HXProject", A: A,
                sid: localStorage.getItem("sid"), ProjectKeyword: me.projectKeyword,
                crewAttrJson: crewAttrJson
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
                    //me.send_editCrew_callback(me.newProjectKeyword, options, "");//, me.projectKeyword, closeWin);

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
    send_createCrewProject_callback: function (projectKeyword, options) {
        var me = this;



        me.refreshWin(me.newProjectKeyword, true);
    },

    //批量新建系统
    bat_new_crew: function () {
        var me = this;


        //弹出操作窗口
        var _fmBatchCreateCrew = Ext.create('Ext.plug_ins.HXEPC_Plugins.Project.BatchCreateCrew', { title: "" });

        winBatchCreateCrew = Ext.widget('window', {
            title: '批量创建机组',
            closeAction: 'hide',
            width: 800,
            height: 596,
            minWidth: 300,
            minHeight: 300,
            layout: 'fit',
            resizable: true,
            modal: true,
            closeAction: 'close', //close 关闭  hide  隐藏  
            items: _fmBatchCreateCrew,
            defaultFocus: 'firstName'
        });


        winBatchCreateCrew.show();
        //监听子窗口关闭事件
        winBatchCreateCrew.on('close', function () {
           me.sendEditCrewDefault();
        });

        _fmBatchCreateCrew.mainPanelId = me.crewlistgrid.id;
        var projectKeyword = me.projectKeyword;//[0].data.Keyword;
        _fmBatchCreateCrew.projectKeyword = projectKeyword;


    },

    new_crew: function () {
        var me = this;

        ///响应新建用户按钮
        if (me.createCrewButton.getText() === "新建") {

            var userTypeIndex = 0;
            var userStateIndex = 0;

            me.CrewCodeText.setValue("");
            me.CrewDescText.setValue("");
            me.CrewEngDescText.setValue("");

            //设置编辑控件可以编辑
            me.set_edit_panel_disabled(false);

            me.createCrewButton.setText("确定");
            //me.delUserButton.setDisabled(true);
            me.deleteCrewButton.setDisabled(true);
            me.saveCrewButton.setDisabled(true);
            me.cancelButton.setDisabled(false);

           // me.createProjectButton.setDisabled(true);
            // me.getSignPic();
        } else {
            //向服务器发送新建用户请求
            me.send_editCrew(true);
        }
    },

    cancel_select: function () {
        var me = this;

        me.curCrewId = "";
        //设置编辑控件不可编辑
        me.set_edit_panel_disabled(true);

        me.createCrewButton.setText("新建");
        //me.delUserButton.setDisabled(true);
        me.deleteCrewButton.setDisabled(true);
        me.saveCrewButton.setDisabled(true);

        me.cancelButton.setDisabled(true);

       // me.createProjectButton.setDisabled(true);

        me.crewlistgrid.getSelectionModel().clearSelections();
        me.crewlistgrid.getView().refresh();


    },

    set_edit_panel_disabled: function (flag) {
        var me = this;

        if (me.CrewCodeText.disabled != flag)
            me.CrewCodeText.setDisabled(flag);

        if (me.CrewDescText.disabled != flag)
            me.CrewDescText.setDisabled(flag);

        if (me.CrewEngDescText.disabled != flag)
            me.CrewEngDescText.setDisabled(flag);


    },

    refreshPanel: function () {
        var me = this;

        if (me.IsCreate) {

            me.cancel_select();
            me.sendEditCrewDefault();
            //me.crewlistgrid.store.loadPage(1);
        } else {
            var grid = me.crewlistgrid;
            var rs = grid.getSelectionModel().getSelection();//获取选择的文档

            if (!(rs !== null && rs.length > 0)) {
                return;
            }

            var strCrewCode = me.CrewCodeText.value.trim();
            var strCrewDesc = me.CrewDescText.value.trim();
            var strCrewEngDesc  = me.CrewEngDescText.value.trim();
            if (rs[0].data.crewCode != strCrewCode) {
                rs[0].data.crewCode = strCrewCode;//获取文档关键字

            }
            if (rs[0].data.crewDesc != strCrewDesc) {
                rs[0].data.crewDesc = strCrewDesc;//获取文档关键字

            }
            if (rs[0].data.crewEngDesc != strCrewEngDesc) {
                rs[0].data.crewEngDesc = strCrewEngDesc;//获取文档关键字
            }
            me.crewlistgrid.getView().refresh();
        }
    },

    //响应选择公司列表表格的事件
    onCrewGridSelect: function (view, record, index, eOpts) {
        var me = this;

        me.CrewCodeText.setValue(record.data.crewCode);
        me.CrewDescText.setValue(record.data.crewDesc);
        me.CrewEngDescText.setValue(record.data.crewEngDesc);

        me.curCrewId = record.data.crewId;
    },

    setProjectKeyword: function (projKeyword) {
        var me = this;
        me.projectKeyword = projKeyword;
    },

    //删除公司资料
    send_deleteCrew: function () {
        var me = this;

        //var viewTree = me._targetGroupUserPanel.down('treepanel');
        var nodes = me.crewlistgrid.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {
            Ext.MessageBox.show({
                title: '提示',
                msg: '是否删除选中的机组？' + me.projectKeyword,
                buttons: Ext.MessageBox.YESNO,
                buttonText: {
                    yes: "是",
                    no: "否"
                },
                fn: function (btn) {
                    if (btn === "yes") {

                        var crewId = nodes[0].data.crewId;
                        var crewCode = nodes[0].data.crewCode;

                        Ext.MessageBox.wait("正在删除机组，请稍候...", "等待");

                        Ext.Ajax.request({

                            url: 'WebApi/Post',
                            method: "POST",
                            params: {
                                C: "AVEVA.CDMS.HXEPC_Plugins.HXProject", A: "DeleteCrew",
                                sid: localStorage.getItem("sid"), ProjectKeyword: me.projectKeyword,
                                CrewId:crewId, CrewCode: crewCode
                            },
                            success: function (response, options) {

                                //获取数据后，更新窗口
                                var res = Ext.JSON.decode(response.responseText, true);
                                var state = res.success;
                                if (state === true) {

                                    Ext.MessageBox.close();//关闭等待对话框

                                    //var recod = eval(res.data[0]);
                                    me.crewlistgrid.store.remove(nodes[0]);

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
            Ext.Msg.alert("错误", "请选择机组！");
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
                    winEditCrew.close();

                //展开目录
                Ext.require('Ext.ux.Common.comm', function () {
                    Ext.getCmp('contentPanel').down('_mainProjectTree').ExpendProject(projKeyword);
                });
            }
        });
    }
});