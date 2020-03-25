//新建目录
Ext.define('Ext.plug_ins.HXPC_Plugins.winBIMView', {
    extend: 'Ext.container.Container',
    alias: 'widget.winBIMView',
    layout: 'border',
    //defaults: {
    //    collapsible: true,
    //    split: true,
    //    bodyStyle: 'padding:15px'
    //},
    //layout: 'fit',
    resultvalue: '', mainPanelId: '',docKeyword:'',
    modelName : "建筑",
    modelUrl : "/Interface/CDMS0119792/建筑.thm",
    initComponent: function () {
        var me = this;
 
        //me.docKeyword = "";
        me.sqlFilePath = "";

        //定义组织机构树的model
        me.BIMTreeModel = Ext.define("_BIMTree", {
            extend: 'Ext.data.Model',
            //parentId用来记录父目录
            fields: [{ name: 'id', type: 'string' },
                { name: 'text', type: 'string' },
                { name: 'type', type: 'string' },
                { name: 'ParentId', type: 'string' }
            ],

            idProperty: "id"
        });

        me.BIMStoreLoadLock = false;

        me.BIMTreeStore = Ext.create("Ext.data.TreeStore", {

            batchActions: false,
            remoteFilter: false,
            remoteSort: false,
            autoLoad: false,//禁止自动加载
            model: me.BIMTreeModel,
            //root: { id: "Root", text: "所有模型", expanded: true },

            ////代理定义
            proxy: {
                type: 'ajax',
                //代理的API定义了操作的提交路径

                url: 'WebApi/Get',
                extraParams: {
                    C: "AVEVA.CDMS.HXPC_Plugins.EnterPoint", A: "GetBIMTreeListJson",
                    sid: localStorage.getItem("sid"), type: "", sqlFilePath: "", node: "Root"
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
                    //这里为了兼容IE修改成这样，原来是 exception: CDMSWeb.ProxyException 
                    //exception: function () { CDMSWeb.ProxyException }
                    exception: CDMSWeb.ProxyException 
                }
            },
        });
        
        //定义显示模型的panel
        me._DocPreviewPanel = Ext.create('Ext.panel.Panel', {
            width:"100%",
            height: 500,
            flex: 1,
            //html: '<div id="mapPic">' + '<iframe src="Scripts/PDFJSInNet/web/viewer.html?file=/download/N4LRTNRJ2XV0TFB4/%E8%BD%AF%E4%BB%B6%E4%BD%BF%E7%94%A8%20%E8%AF%B4%E6%98%8E%E4%B9%A6.pdf"  scrolling="no" style="width:100%;height:600px" frameborder="0"></iframe> </div>'
            html: "<p><--点击按钮预览模型</p>"//<div style='width:100%;height:100%;'  class='webgl-content'><div id='gameContainer'></div></div>"
        });
        
        //var gameInstance = null;//UnityLoader.instantiate("gameContainer", "Scripts/WebGL/Build/WebGL.json", { onProgress: UnityProgress });
        //////模型名称
        //var modelName = "建筑";
        //////模型文件下载地址
        //var modelUrl = "/Interface/CDMS0119792/建筑.thm";
        //me.modelUrl = "/Interface/CDMS0119792/建筑.thm";

        me._DocPreviewMainPanel = {
            xtype: 'panel',
            layout: "hbox",
            width: '100%',
            baseCls: 'my-panel-no-border',//隐藏边框
            margins: "5 5 5 5",
            items: [
                //me._RarListViewPanel,
                me._RarListView,

                //me._DocPreviewPanel
            ], flex: 1
        }

        //定义目录树按钮
        me._BIMTreeTbar = Ext.create('Ext.toolbar.Toolbar', {
            xtype: 'toolbar',
            dock: 'top',
            items: [
                "->",
                {
                    iconCls: "edit-add", scope: me, tooltip: '查看模型', listeners: {
                        "click": function (btn, e, eOpts) {
                            me.sendGetBIMTHMFilePath()

                            //me._DocPreviewPanel.update("<div style='width:100%;height:100%;'  class='webgl-content'><div id='gameContainer2'></div></div>", false, function () {
                            //});
        
                            //gameInstance = UnityLoader.instantiate("gameContainer2", "/Scripts/WebGL/Build/WebGL.json", { onProgress: UnityProgress });


                        }
                    }
                }
            ]
        });

        //创建树控件  
        // Ext.create('Ext.tree.Panel', {
        me.BIMTree = Ext.widget("treepanel", {
            store: me.BIMTreeStore,
            rootVisible: false,
            width: 300, minWidth: 100, split: true,
            tbar: me._BIMTreeTbar,
            viewConfig: {
                stripeRows: true
            },
            listeners: {
                //点击目录树+前 给root参数赋值  
                "beforeitemexpand": function (record, eOpts) {
                    //判断是第几级目录
                    me.BIMTreeStore.proxy.extraParams.type = record.data.type;
                },

                "selectionchange": function (model, sels) {//处理点击选择目录节点后事件
                    me.onTreeItemSelect(model, sels);
                },

                "itemcontextmenu": function (view, rec, node, index, e) {//添加鼠标右键菜单事件
                    me._showContextMenu(view, rec, node, index, e);
                }
            },
            root: { id: "Root", text: "所有模板", expanded: true }
        });

        //============================================================
        //定义属性表格

        me.ParameteModel = Ext.define('Paramete', {
            extend: 'Ext.data.Model',
            fields: ['name', 'value', 'group']
        });

        me.ParametesGridStore = Ext.create('Ext.data.Store', {
            storeId: 'restaraunts',
            model: me.ParameteModel,
            groupField: 'group',
            sorters: ['group', 'name'], 
            proxy: {
                type: "ajax",
                url: 'WebApi/Get',
                extraParams: {
                    C: "AVEVA.CDMS.HXPC_Plugins.EnterPoint", A: "GetBIMNodeParams",
                    sid: localStorage.getItem("sid"), sqlFilePath: me.sqlFilePath,
                    nodeId: ""
                },
                reader: {
                    type: 'json',
                    totalProperty: 'total',
                    root: "data",
                    messageProperty: "Msg"
                },
                writer: {
                    type: "json",
                    encode: true,
                    root: "data",
                    allowSingle: false
                },
                listeners: {
                    exception: CDMSWeb.ProxyException
                }
            }
        });

        me.groupingFeature = Ext.create('Ext.grid.feature.Grouping', {
            groupHeaderTpl: ' {name} ',//格式化分组标题
            hideGroupedHeader: true,
            startCollapsed: true
        });

        me.ParameteColumns=[{
            text: '属性名',
            flex: 1,
            dataIndex: 'name'
        }, {
            text: '属性值',
            flex: 1,
            dataIndex: 'value'
        }, {
            text: '',
            flex: 1,
            dataIndex: 'group'
        }],

        //定义grid属性列表
        me.ParametesGrid = Ext.widget("grid", {
            //extend: 'Ext.grid.Panel',
            //border: true,
            xtype: 'groupGrid',
            store: me.ParametesGridStore,
            features: [me.groupingFeature],
            scroll: 'both',
            columns: me.ParameteColumns, 
            listeners: {
                "afterrender": function () {
                  
                    //me.groupingFeature.expandAll();
                }
            }
        });
       
        //============================================================
        //添加列表
        me.items = [
            {
                extend: 'Ext.panel.Panel',
                alias: 'widget._mainProjectTree', // 此类的xtype类型为buttontransparent  
                title: '模型结构', 
                region: 'west', collapsible: true, rootVisible: false,
                width: 250, minWidth: 100, split: true,
                //margins: '5 0 0 0',
                layout: 'fit',
                items: [me.BIMTree]
            },{
                title: '模型视图',
                collapsible: false,
                region: 'center',
                layout: 'fit',
                items: [me._DocPreviewPanel]
            }, {
                title: '模型属性',
                region: 'east',
                cmargins: '5 5 0 0',
                width: 250,
                minSize: 100,
                maxSize: 350, collapsible: true, split: true,
                layout: 'fit',
                items: [me.ParametesGrid]
            }

        ];
        //me.ParametesGrid.expandAllGroups();

        ////加入菜单
        //me.BIMTree().on("itemcontextmenu", me.contextMenu, me);

        //me.control({
        //    'contextmenu menuitem[action=submenu1]': {
        //        click: this.submenu1OnClick
        //    }

        //});

        me.sendGetBIMSQLFilePath();

        me.callParent(arguments);
    },

    //获取BIM模型数据库文件的服务端绝对路径
    sendGetBIMSQLFilePath: function () {
        var me = this;

        if (me.docKeyword != ""){
            //获取数据库文件路径
            Ext.Ajax.request({

                url: 'WebApi/Get',
                method: "Get",
                params: {
                    C: "AVEVA.CDMS.HXPC_Plugins.EnterPoint", A: "GetBIMSQLFilePath",
                    sid: localStorage.getItem("sid"), docKeyword: me.docKeyword
                },
                success: function (response, options) {

                    me.getBIMSQLFilePath_callback(response, options);

                }
            })
        }
    },

    getBIMSQLFilePath_callback: function (response, options) {

        var me = this;
        var res = Ext.JSON.decode(response.responseText, true);
        var state = res.success;
        if (state === false) {
            var errmsg = res.msg;
            Ext.Msg.alert("错误信息", errmsg);
        }
        else {
            me.sqlFilePath = res.msg;

            me.BIMTreeStore.proxy.extraParams.sqlFilePath = me.sqlFilePath;

            me.BIMTreeStore.load();

        }
    },

    //获取BIM模型thm文件的网络路径
    sendGetBIMTHMFilePath: function () {
        var me = this;

        if (me.docKeyword != "") {
            //获取数据库文件路径
            Ext.Ajax.request({

                url: 'WebApi/Get',
                method: "Get",
                params: {
                    C: "AVEVA.CDMS.HXPC_Plugins.EnterPoint", A: "GetBIMTHMFilePath",
                    sid: localStorage.getItem("sid"), docKeyword: me.docKeyword
                },
                success: function (response, options) {

                    me.getBIMTHMFilePath_callback(response, options);

                }
            })
        }
    },

    getBIMTHMFilePath_callback: function (response, options) {

        var me = this;
        var res = Ext.JSON.decode(response.responseText, true);
        var state = res.success;
        if (state === false) {
            var errmsg = res.msg;
            Ext.Msg.alert("错误信息", errmsg);
        }
        else {
            me.modelUrl = res.msg;

            modelUrl = res.msg;

            me._DocPreviewPanel.update("<div style='width:100%;height:100%;'  class='webgl-content'><div id='gameContainer2'></div></div>", false, function () {
            });

            gameInstance = UnityLoader.instantiate("gameContainer2", "/Scripts/WebGL/Build/WebGL.json", { onProgress: UnityProgress });

            //me.BIMTreeStore.proxy.extraParams.sqlFilePath = me.sqlFilePath;

            //me.BIMTreeStore.load();

        }
    },

    onTreeItemSelect: function (model, sels) {
        var me = this;
        var nodeId = sels[0].data.id;
        var Type = sels[0].data.type;

        if (Type === "node")
        {
            //GetBIMNodeParams(string sid, string sqlFilePath, string nodeId)
            
            me.ParametesGridStore.proxy.extraParams.sqlFilePath = me.sqlFilePath;
            me.ParametesGridStore.proxy.extraParams.nodeId = nodeId;
            me.ParametesGridStore.load();

            ////获取节点的参数列表
            //Ext.Ajax.request({

            //    url: 'WebApi/Get',
            //    method: "Get",
            //    params: {
            //        C: "AVEVA.CDMS.HXPC_Plugins.EnterPoint", A: "GetBIMNodeParams",
            //        sid: localStorage.getItem("sid"), sqlFilePath: me.sqlFilePath,
            //        nodeId: nodeId
            //    },
            //    success: function (response, options) {
            //        var res = Ext.JSON.decode(response.responseText, true);
            //        var state = res.success;
            //        if (state === false) {
            //            var errmsg = res.msg;
            //            Ext.Msg.alert("错误信息", errmsg);
            //        }
            //        else {
            //            var recods = res.data;
            //            me.ParameteData = res.data;
            //            //me.ParametesStore.data = me.ParameteData;
            //            //me.ParametesStore.reload();
            //            //me.ParametesGrid.reconfigure(me.ParametesStore, me.ParameteColumns
            //            //    [{
            //            //    flex: 1,
            //            //    text: 'City',
            //            //    dataIndex: 'city'
            //            //}, {
            //            //    text: 'Total Employees',
            //            //    dataIndex: 'totalEmployees',
            //            //    width: 140
            //            //}, {
            //            //    text: 'Manager',
            //            //    dataIndex: 'manager',
            //            //    width: 120
            //            //}]
            //            //);
            //        }
            //    }
            //})
        }
    },

    ////转到源目录
    //submenu1OnClick: function (widget, event) {

    //    var me = this;
    //    Ext.Msg.alert("错误信息", "errmsg");


    //},
    //显示右键菜单方法
    _showContextMenu: function (tree, record, item, index, e, eOpts) {
        var me = this;
        //阻止浏览器默认右键事件
        e.preventDefault();
        e.stopEvent();

        me.ShowAllAction = Ext.create('Ext.Action', {
            iconCls: 'buy-button',
            text: '显示整个模型',
            disabled: false,
            handler: function (widget, event) {
                var rec = tree.getSelectionModel().getSelection()[0];
                if (rec) {
                    me.ShowAll();
                } else {
                }
            }
        });

        me.ShowAction = Ext.create('Ext.Action', {
            iconCls: 'buy-button',
            text: '显示',
            disabled: false,
            handler: function (widget, event) {
                var rec = tree.getSelectionModel().getSelection()[0];
                if (rec) {
                    me.Show();
                } else {
                }
            }
        });

        me.HideAction = Ext.create('Ext.Action', {
            iconCls: 'buy-button',
            text: '隐藏',
            disabled: false,
            handler: function (widget, event) {
                var rec = tree.getSelectionModel().getSelection()[0];
                if (rec) {
                    me.Hide();
                } else {
                }
            }
        });

        me.HighLightAction = Ext.create('Ext.Action', {
            iconCls: 'buy-button',
            text: '高亮',
            disabled: false,
            handler: function (widget, event) {
                var rec = tree.getSelectionModel().getSelection()[0];
                if (rec) {
                    me.HighLight();
                } else {
                }
            }
        });

        var BIMContextMenu = Ext.create('Ext.menu.Menu', {
            items: [
                me.ShowAllAction, '-',
                me.ShowAction,
                me.HideAction,
                me.HighLightAction
            ]
        });

        //显示右键菜单
        BIMContextMenu.showAt(e.getXY());
    },


    Show: function () {
        var me = this;
        if (typeof (gameInstance) == undefined || gameInstance == null)
            return;

        var node = me.BIMTree.getSelectionModel().getSelection()[0];
        var str = "";
        //HighlightElement(node.data.ParentId, node.data.id);

        if (node.data.type == "category") {
            ShowCategory(node.data.id + '');
        }
        else if (node.data.type == "floor") {
            //请求获取所选一级目录的所有二级目录
            Ext.Ajax.request({
                url: 'WebApi/Post',
                method: "POST",
                params: {
                    C: "AVEVA.CDMS.HXPC_Plugins.EnterPoint", A: "GetBIMTreeListJson",
                    sid: localStorage.getItem("sid"), type: "floor",
                    sqlFilePath: me.sqlFilePath, node: node.data.id
                },
                success: function (response, options) {
                    var res = Ext.JSON.decode(response.responseText);
                    var state = res.success;
                    if (state === false) {
                        var errmsg = res.msg;
                        Ext.Msg.alert("错误信息", errmsg);
                    }
                    else {

                        for (var i = 0; i < res.data.length; i++) {
                            str += res.data[i].id + ";";
                        }
                        ShowCategory(str.substr(0, str.length - 1));
                    }
                }
            });

        } else {
            ShowElement(node.data.ParentId, node.data.id);
        }
    },

    Hide: function () {
        var me = this;
        if (typeof (gameInstance) == undefined || gameInstance == null)
            return;

        var node = me.BIMTree.getSelectionModel().getSelection()[0];
        var str = "";
        if (node.data.type == "category") {
            HideCategory(node.data.id + '');
        }
        else if (node.data.type == "floor") {
            //请求获取所选一级目录的所有二级目录
            Ext.Ajax.request({
                url: 'WebApi/Post',
                method: "POST",
                params: {
                    C: "AVEVA.CDMS.HXPC_Plugins.EnterPoint", A: "GetBIMTreeListJson",
                    sid: localStorage.getItem("sid"), type: "floor",
                    sqlFilePath: me.sqlFilePath, node: node.data.id
                },
                success: function (response, options) {
                    var res = Ext.JSON.decode(response.responseText);
                    var state = res.success;
                    if (state === false) {
                        var errmsg = res.msg;
                        Ext.Msg.alert("错误信息", errmsg);
                    }
                    else {
                        
                        for (var i = 0; i < res.data.length; i++) {
                            str += res.data[i].id + ";";
                        }
                        HideCategory(str.substr(0, str.length - 1));
                    }
                }
            });

        } else {
            HideElement(node.data.ParentId, node.data.id);
        }

    },


    ShowAll: function () {
        var me = this;
        if (typeof (gameInstance) == undefined || gameInstance == null)
            return;
        ClearHide();
    },

    HighLight: function () {
        var me = this;
        if (typeof (gameInstance) == undefined || gameInstance == null)
            return;

        var node = me.BIMTree.getSelectionModel().getSelection()[0];
        if (typeof (node.data.ParentId) != undefined)
            HighlightElement(node.data.ParentId, node.data.id);
    }


////高亮元素,nodeid,elementid为字符串
//function HighlightElement(nodeid, elementid) {
//    gameInstance.SendMessage('JavaScriptInterface', 'HighlightElement', nodeid + ";" + elementid);
//}

////隐藏元素,elementid为字符串
//function HideElement(nodeid, elementid) {
//    gameInstance.SendMessage('JavaScriptInterface', 'HideElement', nodeid + ";" + elementid);
//}

////隐藏某一个二级子节点(若要隐藏多个则用';'号隔开)
//function HideCategory(nodeids) {
//    gameInstance.SendMessage('JavaScriptInterface', 'HideCategory', nodeids);
//}

////清理隐藏
//function ClearHide() {
//    gameInstance.SendMessage('JavaScriptInterface', 'ClearHide');
//}

////当Unity高亮一个元素的时候会调用这个函数
//function OnSelectItemChanged(elementid) {
//    //这里代码你自己填
//    console.log(elementid);
//}
    
});