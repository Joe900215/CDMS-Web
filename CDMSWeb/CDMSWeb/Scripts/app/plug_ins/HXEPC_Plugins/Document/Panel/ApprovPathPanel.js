Ext.define('Ext.plug_ins.HXEPC_Plugins.Document.Panel.ApprovPathPanel', {
    //extend: 'Ext.container.Container',
    extend: 'Ext.panel.Panel',
    alias: 'widget.approvPathPanel', // 此类的xtype类型为buttontransparent
    layout: {
        type: 'vbox',
        pack: 'start',
        align: 'stretch'
    },
    projectKeyword: '',
    baseCls: 'my-panel-no-border',//隐藏边框
    //height: 80,
    initComponent: function () {
        var me = this;
        me.renderTo = me.el;

        //当前用户的用户组类型（org或project）
        me.curUserGroupType = "";
        //当前用户的用户组关键字 
        me.curUserGroupKeyword = "";

        //下一流程状态用户
        me.nextStateUserList = "";

        me.defaultAuditorDesc = "";
        me.defaultAuditorList = "";

        //审批路径combo初始数据
        me.approvpathdata = [{ text: "二级-编批", value: "二级-编批" }, { text: "三级-编审批", value: "三级-编审批" },
            { text: "四级-编审定批", value: "四级-编审定批" }, { text: "五级-编校审定批", value: "五级-编校审定批" }];

        //定义下一流程状态用户Text
        me.nextStateUserText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "审核人", anchor: "80%", labelWidth: 60, labelAlign: "right", labelPad: 8, width: "40%",//width: 230, 
            margin: '10 5 0 10', fieldStyle: ' background-image: none;'//红色边框//flex: 1
        });

        //添加审批路径combo
        Ext.define("approvpathModel", {
            extend: 'Ext.data.Model',
            fields: ["text", "value"]
        });
        me.approvpathProxy = Ext.create("Ext.data.proxy.Memory", {
            data: me.approvpathdata,
            model: "approvpathModel"
        });

        me.approvpathStore = Ext.create("Ext.data.Store", {
            model: approvpathModel,
            proxy: me.approvpathProxy
        });


        me.approvpathCombo = Ext.create("Ext.form.field.ComboBox",
        {
            //xtype: "combo",
            fieldLabel: '审批路径', labelWidth: 60,
            triggerAction: "all", store: me.approvpathStore,
            valueField: 'value', editable: false,//不可输入
            displayField: 'text', margin: '10 5 0 10',// 
            anchor: "80%", labelAlign: "right", labelPad: 8, width: '50%',//width: 120,//
            emptyText: "--请选择--", autoLoadOnValue: true, value: "四级-编审定批",
            //fieldStyle: 'border-color: red; background-image: none;',//红色边框
            listeners:
            {
                select: function (combo, records, eOpts) {
                    //第一次改变时，保存默认的审核人
                    if (me.defaultAuditorList === "") {
                        me.defaultAuditorDesc = me.nextStateUserText.value;
                        me.defaultAuditorList = me.nextStateUserList;
                    }

                    var approvpath = me.approvpathCombo.value;
                    if (approvpath === "二级-编批") {
                        me.nextStateUserText.setFieldLabel("批准人");
                        me.nextStateUserText.setVisible(false);
                        me.nextStateUserButton.setVisible(false);
                        //me.defaultAuditorDesc = me.nextStateUserText.value;
                        //me.defaultAuditorList = me.nextStateUserList;
                    } else {
                        me.nextStateUserText.setVisible(true);
                        me.nextStateUserButton.setVisible(true);
                    }
                    if (approvpath === "五级-编校审定批") {
                        me.nextStateUserText.setFieldLabel("校核人");
 
                        me.nextStateUserText.setValue("");
                        me.nextStateUserList = "";

                    } else if (approvpath === "三级-编审批" || approvpath === "四级-编审定批") {
                        me.nextStateUserText.setFieldLabel("审核人");
                        me.nextStateUserText.setValue(me.defaultAuditorDesc);
                        me.nextStateUserList = me.defaultAuditorList;
                    }

                }
            }
        });


        me.nextStateUserButton = Ext.create("Ext.button.Button", {
            text: "选择...", margins: "10 0 0 5",
            listeners: {
                "click": function (btn, e, eOpts) {//添加点击按钮事件
                    Ext.require('Ext.ux.Common.comm', function () {
                        showSelectUserWin("getUser", "", "", function () {
                            me.nextStateUserText.setValue(window.parent.usernamelist);
                            me.nextStateUserList = window.parent.resultvalue;
                        }, me.curUserGroupType, me.curUserGroupKeyword);

                    })
                }
            }
        });

        me.items = [
            {

                layout: "hbox",
                width: '100%',
                align: 'stretch',
                pack: 'start', margins: "0 0 0 0",
                baseCls: 'my-panel-no-border',//隐藏边框
                items: [
                   me.approvpathCombo,//定义审批路径
                   me.nextStateUserText,
                   me.nextStateUserButton

                ]
            }
        ];

        me.callParent(arguments);
    },
});