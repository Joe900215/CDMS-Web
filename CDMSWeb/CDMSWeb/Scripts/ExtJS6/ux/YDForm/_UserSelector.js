Ext.define('Ext.ux.YDForm._UserSelector', {
    extend: 'Ext.container.Container',
    alias: 'widget._UserSelector',
    layout: 'fit',
    resultvalue:'',

    initComponent: function () {
        var me = this;
        me.renderTo = me.el;
        var resultvalue = '';

        window.parent.resultvalue = "";
        window.parent.usernamelist = "";

        //定义已选择用户的model
        Ext.define("_UserResult", {
            extend: "Ext.data.Model",
            fields: ["text", "id"],
            url:"_blank",
        });

        //定义已选择用户的store
        me._userresultstore = Ext.create("Ext.data.Store", {
            batchActions: false,
            //文章的Store需要支持远程排序和搜索
            remoteFilter: true,
            remoteSort: true,
            //每50条记录为一页
            pageSize: 50,//视图路径：\simplecdms\scripts\app\view\content\view.js
            model: "_UserResult"

        });


        //定义用户选择Tab
        me._sourceSelectUserTab = Ext.create('Ext.ux.YDForm.User._SelectUserTab');

        function addItemToResultGrid(record) {
            //双击时，插入记录到已选择grid
            if (typeof (record.data) != 'undefined') {
                var Keyword = record.data.id;
                var Text = record.data.text;

                var flag = true;
                var resultstore = me.resultgrid.getStore();
                for (var i = 0; i < resultstore.getCount() ; i++) {//遍历每一行
                    if (resultstore.getAt(i).data.id === Keyword) {
                        flag = false;
                        break;
                    }
                }

                if (flag === true) {
                    //插入行到返回grid
                    var r = Ext.create('_UserResult', {
                        id: Keyword,
                        text: Text
                    });


                    var rowlength = me.resultgrid.getStore().data.length;
                    me.resultgrid.getStore().insert(rowlength, r);
                }

            }
        };
        //添加用户选择grid双击事件
        me._sourceSelectUserTab.usergrid.on('itemdblclick', function (view, record, item, index, e) {
            //Ext.Msg.alert("您展开了目录树节点！！！", "您展开了目录树节点！节点ID:");
            addItemToResultGrid(record);
        });


        //添加用户组选择grid双击事件
        me._sourceSelectUserTab._selectGroupUserPanel.groupgrid.on('itemdblclick', function (view, record, item, index, e) {
            //Ext.Msg.alert("您展开了目录树节点！！！", "您展开了目录树节点！节点ID:");
            addItemToResultGrid(record)

        });

        //定义已选择用户的view
        me.resultgrid = Ext.widget("grid", {
            //region: "center",
            //layout: "hbox",
            height: '100%',
            width: "100%",
            cellTip: true,
            store: me._userresultstore,

            columns: [
                //{ text: '用户代码', dataIndex: 'Sender', width: 120 },
                { text: '用户名', dataIndex: 'text', width: '100%' }
            ],
            listeners: {
                //'rowdblclick': function (grid, rowIndex, e) {
                'itemdblclick': function (view, record, item, index, e) {
                    //双击时，从已选择grid删除双击的节点
                    if (typeof (record.data) != 'undefined') {
                        var Keyword = record.data.id;
                        var Text = record.data.text;

                        //删除行
                        var sm = me.resultgrid.getSelectionModel();
                        var store = me.resultgrid.getStore();
                        store.remove(sm.getSelection());
                        if (store.getCount() > 0) {
                            sm.select(0);
                        }
                    }
                }
            }, flex: 1
        });

        //me.UserGroupCombo = Ext.create('Ext.ux.UserSelector.ComboBox.ComboTreeBox', { title: "" });

        //me.UserGroupCombo.getStore().load();
        me.btnPlan = {
            xtype: "panel",
            layout: "hbox",
            height: 40,
            baseCls: 'my-panel-no-border',//隐藏边框
            items: [{
                baseCls: 'my-panel-no-border',//隐藏边框
                flex: 1
            },
            {
                xtype: "button",
                text: "确定", width: 80, margins: "5 5 5 5",
                listeners: {
                    "click": function (btn, e, eOpts) {//添加点击按钮事件
                        //确定按钮事件
                        var strresult, strname;
                        var resultstore = me.resultgrid.getStore();
                        var userArray = new Array();
                        for (var i = 0; i < resultstore.getCount() ; i++) {
                            //strresult = strresult + ";" + resultstore.getAt(i).data.id + "," + resultstore.getAt(i).data.text; //遍历每一行
                            if (i > 0) {
                                strresult = strresult + "," + resultstore.getAt(i).data.id;
                                strname = strname + "," + resultstore.getAt(i).data.text;
                            } else {
                                strresult = resultstore.getAt(i).data.id;
                                strname = resultstore.getAt(i).data.text;
                            }

                        }

                        window.parent.resultvalue = strresult;//functionName(strresult);
                        window.parent.usernamelist = strname;//functionName(strresult);
                        _winUserSelector.close();

                    }
                }
            }, {
                xtype: "button",
                text: "取消", width: 80, margins: "5 5 5 5",
                listeners: {
                    "click": function (btn, e, eOpts) {//添加点击按钮事件
                        _winUserSelector.close();
                    }
                }
            }
            ]
        };


        me.resultgridPlan = {
            xtype: "panel",
            //width: "100%",
            layout: "hbox",
            height: "100%",
            margins: "0 0 0 5",
            items: [me.resultgrid],flex:1
        };

        me.selectPlan = {
            xtype: "panel",
            layout: "hbox",
            height: "100%", margins: "5 5 5 5",
            baseCls: 'my-panel-no-border',//隐藏边框
            //items: [me.selectTab, me.resultgridPlan],
            items: [me._sourceSelectUserTab, me.resultgridPlan],
            flex: 1
        };

        me.mainSelectPlan = {
            xtype: "panel",
            layout: {
                type: 'vbox',
                align : 'stretch',
                pack  : 'start',
                },

            baseCls: 'my-panel-no-border',//隐藏边框
            items: [me.selectPlan, me.btnPlan]
        };

        //添加列表
        me.items = [me.mainSelectPlan

        ];

        //me.usergrid.setSize(panel.getWidth(), panel.ownerCt.getHeight() - panel.getHeight() - me.userinputtext.getHeight() - 300 - 28);

        me.callParent(arguments);
    },





    functionName :function (strresult){
    //父窗口调用函数
    }
});