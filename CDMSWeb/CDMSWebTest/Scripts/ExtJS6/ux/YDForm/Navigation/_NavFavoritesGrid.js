Ext.define('Ext.ux.YDForm.Navigation._NavFavoritesGrid', {
    //extend: 'Ext.container.Container',
    extend: 'Ext.panel.Panel',
    alias: 'widget._navFavoritesGrid', // 此类的xtype类型为buttontransparent  
    //requires: ['Ext.ux.Common.comm'],
    region: "center",
    layout: 'fit',width : 300 ,//flex : 1,
    initComponent: function () {
        var me = this;
        me.renderTo = me.el;
        

        Ext.define("_NavFavoritesModel", {
            extend: 'Ext.data.Model',
            fields: [
                "Keyword",
    	        "Title",
                "Tags"
            ]
        });

        me._NavFavoritesStore = Ext.create("Ext.data.Store", {
            model: _NavFavoritesModel,//模型路径：\simplecdms\scripts\app\model\content.js
            batchActions: false,
            //文章的Store需要支持远程排序和搜索
            remoteFilter: true,
            remoteSort: true,
            //data: [["普通用户"], ["系统管理员"]],
            //每50条记录为一页
            pageSize: 50,//视图路径：\simplecdms\scripts\app\view\content\view.js
            proxy: {
                type: "ajax",
                url: 'WebApi/Get',
                extraParams: {
                    C: "AVEVA.CDMS.WebApi.UserController", A: "GetFavoritesList",
                    sid: localStorage.getItem("sid")
                },
                reader: {
                    type: 'json',
                    totalProperty: 'total',
                    root: "data",//从C#MVC获取数据\simplecdms\controllers\ProjectController.cs .GetDocList.data  ，获取到的数据传送到model里面
                    favoritesProperty: "Msg"
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
            },
            simpleSortMode: true
        });

       
        ////定义grid消息列表
        me.navfavoritesgrid = Ext.widget("grid", {
            title: "收藏夹", region: "center",
            _id: "NavFavoritesGrid",
            renderTo: Ext.getBody(),
            store: me._NavFavoritesStore,//"Favoritess",
 
            selModel: { checkOnly: false, mode: "MULTI" },
            colModel: me.cm,
            columns: [
                //{ text: '', dataIndex: 'Sender', width: 120 },
                {
                    text: '路径', dataIndex: 'Title', flex: 1,
                    renderer: function (value) {
                        return "<span style='font-size:14px;'>" + value + "</span>";
                    }
                }
                //{ text: '', dataIndex: 'SendDatetime', width: 120 }
            ],
            viewConfig:{

                getRowClass:function(){
                    // 在这里添加自定样式 改变这个表格的行高
                    return 'x-grid-row custom-grid-row';
                }
            },
            listeners: {
          
                "afterrender": function (view, eOpts) {
                    //me.refreshGrid();
                },
                "itemcontextmenu": function (view, record, item, index, e, eOpts) {//添加右键菜单事件
                    me._showContextMenu(view, record, item, index, e);
                },
                "itemclick": function (view, record, item, index, e, eOpts) {
                    me.OnFavoritessGridItemClick(view, record, item, index, e, eOpts);
                }

            }
        });

        me.contextmenu = Ext.create('Ext.menu.Menu', {
            float: true,
            items: [{
                text: '取消收藏',
                action: 'submenu1',
                iconCls: 'leaf', handler: function () {
                    me.delFavoritesOnClick();
                }
            }]
        });

        me.items = [
             me.navfavoritesgrid

        ];

        //var typeId = "/_1";
        //me.refreshGrid();
     
        me.callParent(arguments);
    },
    setSourceViewType: function (projectType) {
        //var me = this;
        //me._mainprojecttree.setSourceViewType(projectType);
    },

    //转到源目录
    //OnFavoritessGridSelect: function (rowModel, record, index, eOpts) {
    OnFavoritessGridItemClick: function (view, record, item, index, e, eOpts) {
        var me = this;

        if (typeof (record.data) != 'undefined') {
            var mpanel = Ext.getCmp('mainPanel');
            mpanel.setActiveTab(0);

            var keyword = record.data.Keyword;

            //展开目录
            Ext.getCmp('contentPanel').down('_mainProjectTree').ExpendProject(keyword);

        }
    },

    //右键菜单转到源目录
    delFavoritesOnClick: function () {//widget, event) {

        var me = this;

        var nodes = me.navfavoritesgrid.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {
            //me.mainPanelExpendProject(nodes[0]);
            //Ext.Msg.alert("错误", "取消收藏:" + nodes[0].data.Keyword);
            projkeyword = nodes[0].data.Keyword;

            Ext.Ajax.request({
                url: 'WebApi/Post',
                method: "POST",
                params: {
                    C: "AVEVA.CDMS.WebApi.UserController", A: "DelFavorites",
                    ProjectKeyword: projkeyword, sid: localStorage.getItem("sid")
                },
                success: function (response, options) {
                    var res = Ext.JSON.decode(response.responseText, true);
                    var state = res.success;
                    if (state === false) {
                        var errmsg = res.msg;
                        Ext.Msg.alert("错误信息", errmsg);
                    }
                    else {
                        var recod = res.data[0];
                        var State = recod.state;
                        if (State === "delSuccess") {
                            me.refreshGrid();
                        }
                    }
                },
                failure: function (response, options) {
                    ////Ext.Msg.alert("错误", "连接服务器失败！<br>" + response.responseText);
                }
            });
        }
    },

    refreshGrid: function () {
        var me = this;

        if (me.navfavoritesgrid == undefined) return;

        var store = me.navfavoritesgrid.store;
        store.proxy.extraParams.sid = localStorage.getItem("sid");

        store.load({
            callback: function (records, options, success) {//添加回调，获取子目录的文件数量

                // var gridView = me.navfavoritesgrid;

            }
        });
    },
    //显示右键菜单方法
    _showContextMenu: function (view, record, item, index, e, eOpts) {

        var me = this;
        //阻止浏览器默认右键事件
        e.preventDefault();
        e.stopEvent();

        //显示右键菜单
        me.contextmenu.showAt(e.getXY());


    }
}
);