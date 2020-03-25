Ext.define('Ext.plug_ins.HXEPC_Plugins.Project.EditContractPanel', {
    //extend: 'Ext.container.Container',
    extend: 'Ext.panel.Panel',//'Ext.tab.Panel',//
    alias: 'widget.EditContractPanel', // 此类的xtype类型为buttontransparent  
    title: "合同",
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

        //添加合同编码text
        me.ContractCodeText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "合同编码", labelWidth: 70, margins: '15 0 0 0',
            anchor: "80%", labelAlign: "left", width: 150//flex: 1
        });

        //添加合同名称text
        me.ContractDescText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "合同名称", labelWidth: 70, margins: '15 0 0 0',
            anchor: "80%", labelAlign: "left", width: 150//flex: 1
        });

        ////添加合同名称text
        //me.ContractEngDescText = Ext.create("Ext.form.field.Text", {
        //    xtype: "textfield", fieldLabel: "英文名称", labelWidth: 70, margins: '15 0 0 0',
        //    anchor: "80%", labelAlign: "left", width: 150//flex: 1
        //});

        //定义选择公司表格
        //定义选择公司表格的model
        Ext.define("contractlistmodel", {
            extend: "Ext.data.Model",
            fields: [
                "contractId",
                "contractCode",
                "contractDesc"
                //,
                //"contractEngDesc"
            ],
            url: "_blank",
        });

        //定义选择公司表格的store
        me.contractliststore = Ext.create("Ext.data.Store", {
            model: "contractlistmodel"
        });
        //定义选择公司表格的view
        me.contractlistgrid = Ext.widget("grid", {
            region: "center",
            height: 68,
            stateful: true,
            multiSelect: true,
            //hideHeaders: true,//隐藏表头
            flex: 1,
            store: me.contractliststore,
            viewConfig: {
                stripeRows: true,
                enableTextSelection: false
                //getRowClass: function () {
                //    // 在这里添加自定样式 改变这个表格的行高
                //    return 'x-grid-row upload-file-grid-row';
                //}
            },
            columns: [
                { text: '编码', dataIndex: 'contractCode', width: 60 },
                { text: '名称', dataIndex: 'contractDesc', width: 150 }
                //,
                //{ text: '英文名称', dataIndex: 'contractEngDesc', width: 150 }
            ],
            listeners: {
                itemmousedown: function (view, record, item, index, e, eOpts) {

                },
                select: function (view, record, index, eOpts) {

                    //设置编辑控件可编辑
                    me.set_edit_panel_disabled(false);

                    me.deleteContractButton.setDisabled(false);

                    me.saveContractButton.setDisabled(false);

                    if (me.createContractButton.text === "确定")
                        me.createContractButton.setText("新建");

                    if (me.cancelButton.disabled === false)
                        me.cancelButton.setDisabled(true);

                    // me.createProjectButton.setDisabled(false);

                    me.onContractGridSelect(view, record, index, eOpts);
                },
                itemdblclick: function (view, record, item, index, e, eOpts) {
                    //me.send_createContractProject();
                }
            }
        });

        me.batCreateContractButton = Ext.create("Ext.button.Button", {
            text: "批量新建", width: 60, margins: "10 5 10 5",
            listeners: {
                "click": function (btn, e, eOpts) {//添加点击按钮事件
                    me.bat_new_contract();
                }
            }
        });

        me.createContractButton = Ext.create("Ext.button.Button", {
            text: "新建", width: 60, margins: "10 5 10 5",
            listeners: {
                "click": function (btn, e, eOpts) {//添加点击按钮事件
                    me.new_contract();
                }
            }
        });

        me.deleteContractButton = Ext.create("Ext.button.Button", {
            text: "删除", width: 60, margins: "10 5 10 5", disabled: true,
            listeners: {
                "click": function (btn, e, eOpts) {//添加点击按钮事件
                    me.send_deleteContract(false);
                }
            }
        });

        me.saveContractButton = Ext.create("Ext.button.Button", {
            text: "保存", width: 60, margins: "10 5 10 5", disabled: true,
            listeners: {
                "click": function (btn, e, eOpts) {//添加点击按钮事件
                    me.send_editContract(false);
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
                                      title: '合同列表',
                                      layout: {
                                          type: 'hbox',
                                          pack: 'start',
                                          align: 'stretch'
                                      },
                                      width: 420, height: 420,
                                      items: [
                                            me.contractlistgrid
                                      ]
                                  }, {
                                      xtype: "fieldset", margin: '0 0 0 0',
                                      //baseCls: 'my-panel-no-border',//隐藏边框
                                      title: '合同资料',
                                      layout: {
                                          type: 'vbox',
                                          pack: 'start',
                                          align: 'stretch'
                                      },
                                      flex: 1,
                                      items: [
                                          me.ProjectCodeText,
                                          me.ProjectDescText,
                                          me.ContractCodeText,
                                          me.ContractDescText
                                          //,
                                          //me.ContractEngDescText

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
                          me.batCreateContractButton,
                          me.createContractButton,
                           me.deleteContractButton,
                          me.saveContractButton,
                          me.cancelButton
                      ]
                  }
                      ]
                  }
              ]
          })];

        //var fminfo = Ext.getCmp(me.Id).up('editProjectInfo');
        //me.projectKeyword = fminfo .projectKeyword;
        //me.sendEditContractDefault();
        me.callParent(arguments);
    },

    //获取新建合同资料目录表单默认参数
    sendEditContractDefault: function () {
        var me = this;

        me.set_edit_panel_disabled(true);

        //通过extjs的ajax获取操作全部名称
        Ext.Ajax.request({
            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.HXEPC_Plugins.HXProject", A: "GetEditContractDefault",
                sid: localStorage.getItem("sid"), ProjectKeyword: me.projectKeyword
            },
            success: function (response, options) {
                me.sendGetEditContractDefault_callback(response, options);//, funCallback);
            },
            failure: function (response, options) {
                ////Ext.Msg.alert("错误", "连接服务器失败！<br>" + response.responseText);
            }
        });
    },

    //处置获取新建合同资料目录表单默认参数的返回
    sendGetEditContractDefault_callback: function (response, options) {
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

            var contractList = eval(recod.ContractList);

            me.contractlistgrid.store.removeAll();

            for (var itemKey in contractList) {
                //me.projectKeyword = contractList[itemKey].contractCode;
                if (contractList[itemKey].contractCode != undefined) {
                    //插入行到文件selectUserGrid
                    var r = Ext.create('contractlistmodel', {
                        contractId: contractList[itemKey].contractId,
                        contractCode: contractList[itemKey].contractCode,
                        contractDesc: contractList[itemKey].contractDesc
                        ////,
                        ////contractEngDesc: contractList[itemKey].contractEngDesc
                    });

                    var rowlength = me.contractlistgrid.getStore().data.length;
                    me.contractlistgrid.getStore().insert(rowlength, r);
                }
            }
        }
    },


    //新建公司资料目录
    send_editContract: function (isCreate) {
        var me = this;

        me.IsCreate = isCreate;


        //获取公司编号Text
        var contractCode = me.ContractCodeText.value;

        //获取公司名称Text
        var contractDesc = me.ContractDescText.value;

        //var ContractEngDesc = me.ContractEngDescText.value;


        ////获取表单数据，转换成JSON字符串
        var contractAttr =
        [
            { name: 'contractId', value: me.curContractId },
            { name: 'contractCode', value: contractCode },
            { name: 'contractDesc', value: contractDesc }
            ////,
            ////{ name: 'contractEngDesc', value: ContractEngDesc }


        ];

        var contractAttrJson = Ext.JSON.encode(contractAttr);

        Ext.MessageBox.wait("正在创建参建单位，请稍候...", "等待");

        var A = isCreate ? "CreateContract" : "EDITContract";

        Ext.Ajax.request({

            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.HXEPC_Plugins.HXProject", A: A,
                sid: localStorage.getItem("sid"), ProjectKeyword: me.projectKeyword,
                contractAttrJson: contractAttrJson
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
                    //me.send_editContract_callback(me.newProjectKeyword, options, "");//, me.projectKeyword, closeWin);

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
    send_createContractProject_callback: function (projectKeyword, options) {
        var me = this;



        me.refreshWin(me.newProjectKeyword, true);
    },

    //批量新建系统
    bat_new_contract: function () {
        var me = this;


        //弹出操作窗口
        var _fmBatchCreateContract = Ext.create('Ext.plug_ins.HXEPC_Plugins.Project.BatchCreateContract', { title: "" });

        winBatchCreateContract = Ext.widget('window', {
            title: '批量创建合同',
            closeAction: 'hide',
            width: 800,
            height: 596,
            minWidth: 300,
            minHeight: 300,
            layout: 'fit',
            resizable: true,
            modal: true,
            closeAction: 'close', //close 关闭  hide  隐藏  
            items: _fmBatchCreateContract,
            defaultFocus: 'firstName'
        });


        winBatchCreateContract.show();
        //监听子窗口关闭事件
        winBatchCreateContract.on('close', function () {
            me.sendEditContractDefault();
        });

        _fmBatchCreateContract.mainPanelId = me.contractlistgrid.id;
        var projectKeyword = me.projectKeyword;//[0].data.Keyword;
        _fmBatchCreateContract.projectKeyword = projectKeyword;


    },

    new_contract: function () {
        var me = this;

        ///响应新建用户按钮
        if (me.createContractButton.getText() === "新建") {

            var userTypeIndex = 0;
            var userStateIndex = 0;

            me.ContractCodeText.setValue("");
            me.ContractDescText.setValue("");
            //me.ContractEngDescText.setValue("");

            //设置编辑控件可以编辑
            me.set_edit_panel_disabled(false);

            me.createContractButton.setText("确定");
            me.deleteContractButton.setDisabled(true);
            me.saveContractButton.setDisabled(true);
            me.cancelButton.setDisabled(false);

            // me.createProjectButton.setDisabled(true);
            // me.getSignPic();
        } else {
            //向服务器发送新建用户请求
            me.send_editContract(true);
        }
    },

    cancel_select: function () {
        var me = this;

        me.curContractId = "";
        //设置编辑控件不可编辑
        me.set_edit_panel_disabled(true);

        me.createContractButton.setText("新建");
        me.deleteContractButton.setDisabled(true);
        me.saveContractButton.setDisabled(true);
        me.cancelButton.setDisabled(true);

        // me.createProjectButton.setDisabled(true);

        me.contractlistgrid.getSelectionModel().clearSelections();
        me.contractlistgrid.getView().refresh();


    },

    set_edit_panel_disabled: function (flag) {
        var me = this;

        if (me.ContractCodeText.disabled != flag)
            me.ContractCodeText.setDisabled(flag);

        if (me.ContractDescText.disabled != flag)
            me.ContractDescText.setDisabled(flag);

        //if (me.ContractEngDescText.disabled != flag)
        //    me.ContractEngDescText.setDisabled(flag);


    },

    refreshPanel: function () {
        var me = this;

        if (me.IsCreate) {

            me.cancel_select();
            me.sendEditContractDefault();
            //me.contractlistgrid.store.loadPage(1);
        } else {
            var grid = me.contractlistgrid;
            var rs = grid.getSelectionModel().getSelection();//获取选择的文档

            if (!(rs !== null && rs.length > 0)) {
                return;
            }

            var strContractCode = me.ContractCodeText.value.trim();
            var strContractDesc = me.ContractDescText.value.trim();
            //var strContractEngDesc = me.ContractEngDescText.value.trim();
            if (rs[0].data.contractCode != strContractCode) {
                rs[0].data.contractCode = strContractCode;//获取文档关键字

            }
            if (rs[0].data.contractDesc != strContractDesc) {
                rs[0].data.contractDesc = strContractDesc;//获取文档关键字

            }
            //if (rs[0].data.contractEngDesc != strContractEngDesc) {
            //    rs[0].data.contractEngDesc = strContractEngDesc;//获取文档关键字
            //}
            me.contractlistgrid.getView().refresh();
        }
    },

    //响应选择公司列表表格的事件
    onContractGridSelect: function (view, record, index, eOpts) {
        var me = this;

        me.ContractCodeText.setValue(record.data.contractCode);
        me.ContractDescText.setValue(record.data.contractDesc);
        //me.ContractEngDescText.setValue(record.data.contractEngDesc);

        me.curContractId = record.data.contractId;
    },

    setProjectKeyword: function (projKeyword) {
        var me = this;
        me.projectKeyword = projKeyword;
    },

    //删除公司资料
    send_deleteContract: function () {
        var me = this;

        //var viewTree = me._targetGroupUserPanel.down('treepanel');
        var nodes = me.contractlistgrid.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {
            Ext.MessageBox.show({
                title: '提示',
                msg: '是否删除选中的合同？' + me.projectKeyword,
                buttons: Ext.MessageBox.YESNO,
                buttonText: {
                    yes: "是",
                    no: "否"
                },
                fn: function (btn) {
                    if (btn === "yes") {

                        var contractId = nodes[0].data.contractId;
                        var contractCode = nodes[0].data.contractCode;

                        Ext.MessageBox.wait("正在删除合同，请稍候...", "等待");

                        Ext.Ajax.request({

                            url: 'WebApi/Post',
                            method: "POST",
                            params: {
                                C: "AVEVA.CDMS.HXEPC_Plugins.HXProject", A: "DeleteContract",
                                sid: localStorage.getItem("sid"), ProjectKeyword: me.projectKeyword,
                                ContractId: contractId, ContractCode: contractCode
                            },
                            success: function (response, options) {

                                //获取数据后，更新窗口
                                var res = Ext.JSON.decode(response.responseText, true);
                                var state = res.success;
                                if (state === true) {

                                    Ext.MessageBox.close();//关闭等待对话框

                                    //var recod = eval(res.data[0]);
                                    me.contractlistgrid.store.remove(nodes[0]);

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
            Ext.Msg.alert("错误", "请选择合同！");
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
                    winEditContract.close();

                //展开目录
                Ext.require('Ext.ux.Common.comm', function () {
                    Ext.getCmp('contentPanel').down('_mainProjectTree').ExpendProject(projKeyword);
                });
            }
        });
    }
});