/*定义消息列表grid*/
Ext.define('Ext.ux.gcz.Message._MainMessageGrid', {
    //extend: 'Ext.container.Container',
    extend: 'Ext.panel.Panel',
    alias: 'widget._mainMessageGrid', // 此类的xtype类型为buttontransparent  
    //requires: ['Ext.ux.Common.m.comm'],
    //region: "center",
    //layout: 'fit',
    layout: {
        type: 'vbox',
        align: 'stretch',
        pack: 'start'
    },
    //flex: 1,
    height:145,
    initComponent: function () {
        var me = this;
        me.renderTo = me.el;

        //me._MessagesStore = Ext.create("Ext.data.Store", {
        me._MessagesStore = Ext.create("Ext.data.BufferedStore", {
            
            model: 'CDMSWap.model.Message',//模型路径：\simplecdms\scripts\app\model\content.js
            //batchActions: false,
            //文章的Store需要支持远程排序和搜索
            //remoteFilter: true,
            //remoteSort: true,
            //data: [["普通用户"], ["系统管理员"]],
            //每50条记录为一页
            remoteGroup: true,
            leadingBufferZone: 100,
            pageSize: 100,//视图路径：\simplecdms\scripts\app\view\content\view.js

            //proxy: {
            //    type: "ajax",
            //    url: 'WebApi/Get',
            //    extraParams: {
            //        C: "AVEVA.CDMS.WebApi.MessageController", A: "GetMessageList",
            //        MessageType: "/_1", total: 50000, sid: localStorage.getItem("sid")
            //    },
            //    reader: {
            //        type: 'json',
            //        totalProperty: 'total',//10000,//
            //        root: "data",//从C#MVC获取数据\simplecdms\controllers\ProjectController.cs .GetDocList.data  ，获取到的数据传送到model里面
            //        messageProperty: "Msg"
            //    },
            //    writer: {
            //        type: "json",
            //        encode: true,
            //        root: "data",
            //        allowSingle: false
            //    }
            //},
            proxy: {
                // load using script tags for cross domain, if the data in on the same domain as
                // this page, an Ajax proxy would be better
                type: 'jsonp',
                url: 'WebApi/GetJsonp',
                extraParams: {
                    C: "AVEVA.CDMS.WebApi.MessageController", A: "GetMessageList",
                    MessageType: "/_1", total: 50000, sid: localStorage.getItem("sid")
                },
                reader: {
                    rootProperty: 'data',
                    totalProperty: 'total',
                    //root: "data",//从C#MVC获取数据\simplecdms\controllers\ProjectController.cs .GetDocList.data  ，获取到的数据传送到model里面
                    messageProperty: "Msg"
                },
                // sends single sort as multi parameter
                simpleSortMode: true,
                // sends single group as multi parameter
                simpleGroupMode: true,

                // This particular service cannot sort on more than one field, so grouping === sorting.
                groupParam: 'sort',
                groupDirectionParam: 'dir'
            },
            listeners: {

                // This particular service cannot sort on more than one field, so if grouped, disable sorting
                groupchange: function (store, groupers) {
                    var sortable = !store.isGrouped(),
                        headers = grid.headerCt.getVisibleGridColumns(),
                        i, len = headers.length;

                    for (i = 0; i < len; i++) {
                        headers[i].sortable = (headers[i].sortable !== undefined) ? headers[i].sortable : sortable;
                    }
                },

                // This particular service cannot sort on more than one field, so if grouped, disable sorting
                beforeprefetch: function (store, operation) {
                    if (operation.getGrouper()) {
                        operation.setSorters(null);
                    }
                }
            }
            //sorters: [{
            //    property: 'threadid',
            //    direction: 'ASC'
            //}],
            //simpleSortMode: true
        });

        //定义消息树按钮
        me._messageGridTbar = Ext.create('Ext.toolbar.Toolbar', {
            xtype: 'toolbar',
            dock: 'top',
            items: [
                {
                    iconCls: "refresh", scope: me, tooltip: '刷新', listeners: {
                        "click": function (btn, e, eOpts) {
                            me.refreshMessagePage();
                        }
                    }
                },
                "->",
                {
                    iconCls: "ellipsis", scope: me, tooltip: '显示菜单',
                    listeners: {
                    "click": function (view, e, eOpts) {
                        me._showContextMenu(view, e, eOpts);
                    }

                }
                }
            ]
        });

        ////定义grid消息列表
        me.mainmessagegrid = Ext.create('Ext.grid.Panel',  {

            //title: "消息列表",// region: "center",
            store: me._MessagesStore,
            hideHeaders: true,

            loadMask: true,
            selModel: {
                pruneRemoved: false
            },
            multiSelect: true,
            viewConfig: {
                trackOver: false
            },
            features: [{
                ftype: 'grouping',
                hideGroupedHeader: false
            }],

            //autoLoad: true,
            tbar: me._messageGridTbar,
            layout: 'fit',
            width:'100%',
            height: 145,
            columns: [{
                xtype: 'rownumberer',
                width: 30,
                sortable: false
            },
                //{ text: '发送者', dataIndex: 'Sender', width: 80 },
                {
                    text: '标题', dataIndex: 'Title', flex: 1,
                    //renderer: renderTopic,
                    sortable: true,
                    groupable: false,
                    cellWrap: true
                }//,
                //{ text: '发送时间', dataIndex: 'SendDatetime', width: 80 }
                //{ text: '所属流程', dataIndex: 'Workflow_DESC', width: 200 },
                //{ text: '重要程度', dataIndex: 'SignificantType', width: 90 },
                //{ text: '延迟天数', dataIndex: 'DelayDay', width: 90 }
            ],
            listeners: {
                //选中某条记录的事件
                "select": function (rowModel, record, index, eOpts) {
                    me.OnMessagesGridSelect(rowModel, record, index, eOpts);
                }
            }
        });

        me.mainmessagepanel = Ext.create('Ext.Panel', {
            xtype: 'panel',
            activeTab: 0,
            defaults: {
                border: false,
                bodyPadding: 5,
                bodyStyle: "background:#DFE9F6"
            },
            baseCls: 'my-panel-no-border',//隐藏边框
            items: [               
                me.mainmessagegrid

        ]

        });

        me.items = [
                            me.mainmessagepanel
        ];

        //加载消息列表并定位到第一条记录
        me.refreshMessagePage();
        ////加载消息列表并定位到第一条记录
        //me.mainmessagegrid.store.load(function (records, operation, success) {
        //    if (records.length>0)
        //        me.mainmessagegrid.getSelectionModel().select(0, true);
        //});


        me.callParent(arguments);
    },

    //选中某条记录的事件
    OnMessagesGridSelect: function (rowModel, record, index, eOpts) {
        var me = this;

            //获取消息内容页面
            var contentView = me.up("_mainMessageView").down("_mainMessageContent");

            contentView.displayMessAttr(record);
    },

    //显示菜单方法
    _showContextMenu: function (view, e, eOpts) {
        var me = this;

        var menus = new Ext.menu.Menu({
            items: [
            {
                xtype: "",
                text: "退出", iconCls: "logout_16", handler: function () {
                    window.location = "DBSource/Logout?UserName=" + localStorage.getItem("username") + "&WebViewType=mobile";
                }
            }
            ]
        });
        menus.showAt(e.getXY());  
    },

    refreshMessagePage: function () {
        var me = this;
        //加载消息列表并定位到第一条记录
        me.mainmessagegrid.store.load(function (records, operation, success) {
            if (records.length > 0)
                me.mainmessagegrid.getSelectionModel().select(0, true);
        });
    }
});