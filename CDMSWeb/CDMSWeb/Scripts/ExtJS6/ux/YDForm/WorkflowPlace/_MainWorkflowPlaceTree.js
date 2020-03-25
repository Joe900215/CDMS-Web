/*定义目录树*/
Ext.define('Ext.ux.YDForm.WorkflowPlace._MainWorkflowPlaceTree', {
    extend: 'Ext.panel.Panel',
    alias: 'widget._mainWorkflowPlaceTree', // 此类的xtype类型为buttontransparent  
    title: "流程分类", region: "west", collapsible: true, rootVisible: false,
    width: 150, minWidth: 100, split: true,
    layout: 'fit',
    initComponent: function () {
        var me = this;
        me.renderTo = me.el;

        //定义project属性store
        me._WorkflowPlaceTreeStore = Ext.create("Ext.data.TreeStore", {
            batchActions: false,
            remoteFilter: false,
            remoteSort: false,
            model: "CDMSWeb.model.WorkflowPlaceTree",
            root: { id: "/", text: "根目录", expanded: true },
            //代理定义
            proxy: {
                type: 'ajax',
                //代理的API定义了操作的提交路径
                //路径：\CDMSWeb\Controllers\FileController.cs
                url: "WebApi/Get",//调用路径：\simplecdms\controllers\projectcontroller.cs
                extraParams: {
                    C: "AVEVA.CDMS.WebApi.WorkFlowController", A: "GetWorkFlowPlaceTree",
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
        me._workflowplaceTreeTbar = Ext.create('Ext.toolbar.Toolbar', {
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
                                    me.refreshWorkflowPlacePage();
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
    //    me.mainworkflowplacetree = Ext.widget("treepanel", {
        me.mainworkflowplacetree =  Ext.create('Ext.tree.Panel', {
            store: me._WorkflowPlaceTreeStore,
            rootVisible: false,
            width: 150, minWidth: 100, split: true,
            tbar: me._workflowplaceTreeTbar,
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

                    //这里用me.mainworkflowplacetree.on('load', function (store,view,records,eOpts)事件触发

                }
            }

        });

        //添加属性TAB页面到容器
        me.items = [

                               me.mainworkflowplacetree
        ];

        me.mainworkflowplacetree.on('load', function (view, records, successful, operation, node, eOpts) {

            me.mainworkflowplacetree.getSelectionModel().select(records[0]);

            //var typeId = records[0].data.id;

            ////刷新消息列表表格
            //me.refreshWorkflowPlaceGrid(typeId);
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
            me.refreshWorkflowPlaceGrid(typeId);

        }
    },

    
    //刷新整个消息页面
    refreshWorkflowPlacePage: function () {
        var me = this;

        var msgTypes = me.mainworkflowplacetree.getSelectionModel().getSelection();

        if (msgTypes.count <= 0) return;

        var msgTypeId = msgTypes[0].get("id");

        me._WorkflowPlaceTreeStore.load({
            callback: function (records, options, success) {//添加回调，获取子目录的文件数量
                
                //var viewTree = me.mainworkflowplacetree;

                ////等待上一个函数的执行结果，查找点击树节点
                //var count = 0, is_true = false;
                //var node = viewTree.store.getNodeById(msgTypeId);

                ////setInterval(function () {
                //   // if (!count) {
                //        node = viewTree.store.getNodeById(msgTypeId);
                //        if (Ext.isObject(node)) {
                //            is_true = true;
                //        }
                ////        count++;
                // //   }

                //    if (is_true) {
                //        viewTree.getSelectionModel().select(node);
                //        viewTree.fireEvent('click', node);
                //        is_true = false;
                //    }
                ////}, 1000);

                //if (records.length > 0) {
                //    //var typeId = records[0].data.id;
                //    var typeId = node.get("id");
                //    //me.refreshWorkflowPlaceGrid(typeId);
                //}
            }
        });
    },

    refreshWorkflowPlaceGrid: function (typeId) {
        var me = this;

        //获取消息列表表格
        var gridView = me.up("_mainWorkflowPlaceView").down("_mainWorkflowPlaceGrid");

        //获取消息列表的store
        var store = gridView._WorkflowPlacesStore;

        store.proxy.extraParams.WorkflowType = typeId;//把参数传给C#MVC,路径：\simplecdms\controllers\projectcontroller.cs 下的 GetChildProject()
        store.proxy.extraParams.sid = localStorage.getItem("sid");
        store.proxy.extraParams.page = "1";
        store.proxy.extraParams.start = 0;
        store.currentPage = 1;

        store.load({
            callback: function (records, options, success) {//添加回调，获取子目录的文件数量

                if (records.length > 0) {
                    var gridView = me.up("_mainWorkflowPlaceView").down("_mainWorkflowPlaceGrid");

                    gridView.mainworkflowplacegrid.getSelectionModel().select(records[0]);
                }

            }
        });
    }
});