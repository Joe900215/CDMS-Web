/*定义目录树*/
Ext.define('Ext.ux.YDForm.Message._MainMessageTree', {
    extend: 'Ext.panel.Panel',
    alias: 'widget._mainMessageTree', // 此类的xtype类型为buttontransparent  
    title: "消息类别", region: "west", collapsible: true, rootVisible: false,
    width: 150, minWidth: 100, split: true,
    layout: 'fit',
    initComponent: function () {
        var me = this;
        me.renderTo = me.el;

        //定义project属性store
        me._MessageTreeStore = Ext.create("Ext.data.TreeStore", {
            batchActions: false,
            remoteFilter: false,
            remoteSort: false,
            model: "CDMSWeb.model.MessageTree",
            root: { id: "/", text: "根目录", expanded: true },
            //代理定义
            proxy: {
                type: 'ajax',
                //代理的API定义了操作的提交路径
                //路径：\CDMSWeb\Controllers\FileController.cs
                url: "WebApi/Get",//调用路径：\simplecdms\controllers\projectcontroller.cs
                extraParams: {
                    C: "AVEVA.CDMS.WebApi.MessageController", A: "GetMessageTree",
                    sid: localStorage.getItem("sid")
                },
                //在代理定义中，reader和writer的定义可标准化数据的输入输出，
                //这个与用户中的定义是一样的
                reader: {
                    messageProperty: "Msg",
                    type: 'json',
                    root: "data"
                },
                writer: {
                    type: "json",
                    encode: true,
                    root: "data",
                    allowSingle: false
                },
                listeners: {
                    exception: CDMSWeb.ProxyException

                },
                listeners: {
                    exception: CDMSWeb.ProxyException
                }
            }

        });

        //定义消息树按钮
        me._messageTreeTbar = Ext.create('Ext.toolbar.Toolbar', {
            xtype: 'toolbar',
            dock: 'top',
            items: [
                {
                    iconCls: "refresh", scope: me, tooltip: '刷新',
                    text: "刷新",
                    listeners: {
                        "click": function (btn, e, eOpts) {
                            //me.RefreshProjTree("LastProject");
                            Ext.Ajax.request({
                                url: 'WebApi/Post',
                                method: "POST",
                                params: {
                                    C: "AVEVA.CDMS.WebApi.DBSourceController", A: "refreshDBSource",
                                    sid: localStorage.getItem("sid")
                                },
                                success: function (response, options) {
                                    me.refreshMessagePage();
                                },
                                failure: function (response, options) {
                                    ////Ext.Msg.alert("错误", "连接服务器失败！<br>" + response.responseText);
                                }
                            });
                        }
                    }
                }
            ]
        });

        //定义目录树
    //    me.mainmessagetree = Ext.widget("treepanel", {
        me.mainmessagetree =  Ext.create('Ext.tree.Panel', {
            store: me._MessageTreeStore,
            rootVisible: false,
            width: 150, minWidth: 100, split: true,
            tbar: me._messageTreeTbar,
            //renderTo: Ext.getBody(),
            
            viewConfig: {
                stripeRows: true
            },
            renderTo: me.el,
            listeners: {
                "selectionchange": function (model, sels) {//处理点击选择消息类别节点后事件
                    me.onTreeItemSelect(model, sels);
                },

                "afterrender": function (store, view) {//完成渲染后的事件

                    //这里用me.mainmessagetree.on('load', function (store,view,records,eOpts)事件触发

                }
            }

        });

        //添加属性TAB页面到容器
        me.items = [

                               me.mainmessagetree
        ];

        me.mainmessagetree.on('load', function (view, records, successful, operation, node, eOpts) {
            

            me.mainmessagetree.getSelectionModel().select(records[0]);

            //var typeId = records[0].data.id;

            ////刷新消息列表表格
            //me.refreshMessageGrid(typeId);
        });

        me.callParent(arguments);
    },

    //处理点击选择消息类别节点后事件
    onTreeItemSelect: function (model, sels) {
        var me = this;
        var Keyword = sels[0].data.id;

        text = sels[0].data.text;


        if (sels.length > 0) {
            var typeId = sels[0].data.id;

            //刷新消息列表表格
            me.refreshMessageGrid(typeId);

        }
    },

    
    //刷新整个消息页面
    refreshMessagePage: function () {
        var me = this;

        var msgTypes = me.mainmessagetree.getSelectionModel().getSelection();

        var msgTypeId = msgTypes[0].get("id");

        me._MessageTreeStore.load({
            callback: function (records, options, success) {//添加回调，获取子目录的文件数量
                
                var viewTree = me.mainmessagetree;

                //等待上一个函数的执行结果，查找点击树节点
                var count = 0, is_true = false;
                var node = viewTree.store.getNodeById(msgTypeId);

                //setInterval(function () {
                   // if (!count) {
                        node = viewTree.store.getNodeById(msgTypeId);
                        if (Ext.isObject(node)) {
                            is_true = true;
                        }
                //        count++;
                 //   }

                    if (is_true) {
                        viewTree.getSelectionModel().select(node);
                        viewTree.fireEvent('click', node);
                        is_true = false;
                    }
                //}, 1000);

                if (records.length > 0) {
                    //var typeId = records[0].data.id;
                    var typeId = node.get("id");
                    me.refreshMessageGrid(typeId);
                }
            }
        });
    },

    refreshMessageGrid: function (typeId) {
        var me = this;

        //获取消息列表表格
        var gridView = me.up("_mainMessageView").down("_mainMessageGrid");

        //获取消息列表的store
        var store = gridView._MessagesStore;

        store.proxy.extraParams.MessageType = typeId;//把参数传给C#MVC,路径：\simplecdms\controllers\projectcontroller.cs 下的 GetChildProject()
        store.proxy.extraParams.sid = localStorage.getItem("sid");
        store.proxy.extraParams.page = "1";
        store.proxy.extraParams.start = 0;
        store.currentPage = 1;

        store.load({
            callback: function (records, options, success) {//添加回调，获取子目录的文件数量

                var gridView = me.up("_mainMessageView").down("_mainMessageGrid");

                gridView.mainmessagegrid.getSelectionModel().select(records[0]);

            }
        });
    }
});