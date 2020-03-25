Ext.define('Ext.plug_ins.WTMS_Plugins.Dist.NavWorkTaskGrid', {
    //extend: 'Ext.container.Container',
    extend: 'Ext.panel.Panel',
    alias: 'widget.navWorkTaskGrid', // 此类的xtype类型为buttontransparent
    region: "center",
    layout: 'fit', flex: 1,// width: 400,//
    initComponent: function () {
        var me = this;
        me.renderTo = me.el;
        //me.renderTo = Ext.getBody();

        Ext.define("NavNewDocModel", {
            extend: 'Ext.data.Model',
            fields: [
                "Keyword",
    	        "Title",        //标题
                "Creater",      //创建人
                "ReqResTime",   //要求回复时间
                "ProjectName",  //项目名称
                "CreateTime",   //创建时间
                "MountState",   //关闭状态
                "Tags"
            ]
        });

        me.NavNewDocStore = Ext.create("Ext.data.Store", {
            model: NavNewDocModel,//模型路径：\simplecdms\scripts\app\model\content.js
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
                    //C: "AVEVA.CDMS.WebApi.UserController", A: "GetFavoritesList",
                    C: "AVEVA.CDMS.WTMS_Plugins.WorkTask", A: "GetUserWorkTaskList",
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
                sorters: { property: 'due', direction: 'ASC' },
                groupField: 'MountState',
            simpleSortMode: true
        });


        ////定义grid消息列表
        me.navNewDocgrid = Ext.widget("grid", {
            title: "最新文档", region: "center",
            _id: "NavNewDocGrid",
            renderTo: Ext.getBody(),
            store: me.NavNewDocStore,//"Favoritess",
            features: [{
                id: 'group',
                ftype: 'grouping',
                groupHeaderTpl: '{name}',
                hideGroupedHeader: true,
                enableGroupingMenu: false
            }],
            selModel: { checkOnly: false, mode: "MULTI" },
            colModel: me.cm,
            columns: [
                { text: '项目', dataIndex: 'ProjectName', width: 150 },
                {
                    text: '工作任务', dataIndex: 'Title', flex: 1
                    //,
                    //renderer: function (value) {
                    //    return "<span style='font-size:14px;'>" + value + "</span>";
                    //}
                },
                { text: '要求回复时间', dataIndex: 'ReqResTime', width: 120 },
                //{ text: '创建者', dataIndex: 'Creater', width: 80 },
                { text: '创建时间', dataIndex: 'CreateTime', width: 120 }
            ],
            bbar: new Ext.PagingToolbar({
                store: me.NavNewDocStore,//"Contents",//
                displayInfo: true,
                displayMsg: '当前显示{0} - {1}条，共{2}条数据',
                emptyMsg: "没有记录"
            }),
            viewConfig: {

                getRowClass: function () {
                    // 在这里添加自定样式 改变这个表格的行高
                    return 'x-grid-row custom-grid-row';
                }
            },
            listeners: {
                //"itemcontextmenu": function (view, record, item, index, e, eOpts) {//添加右键菜单事件
                //    me._showContextMenu(view, record, item, index, e);
                //},
                //"itemclick": function (view, record, item, index, e, eOpts) {
                //    me.OnNewDocGridItemClick(view, record, item, index, e, eOpts);
                //}
                "itemdblclick": function (view, record, item, index, e, eOpts) {
                    me.OnNewDocGridItemDbClick(view, record, item, index, e, eOpts);
                }

            }
        });

        me.items = [
             me.navNewDocgrid
           // {
               

                //baseCls: 'my-panel-no-border',//隐藏边框
                //layout: {
                //    type: 'vbox',
                //    pack: 'start',
                //    align: 'stretch'
                //},
                ////region: "center",
                //items: [
                //    me.navNewDocgrid
                //    //{
                //    //    xtype:"button",
                //    //    text: "我的按钮"
                //    //}
                //],
                //flex: 1
           // }

        ];

        //me.refreshGrid();

        me.callParent(arguments);
    },

    refreshGrid: function () {
        var me = this;

        if (me.navNewDocgrid == undefined) return;

        var store = me.navNewDocgrid.store;
        store.proxy.extraParams.sid = localStorage.getItem("sid");

        store.load({
            callback: function (records, options, success) {//添加回调，获取子目录的文件数量

                // var gridView = me.navfavoritesgrid;

            }
        });
    },

    OnNewDocGridItemDbClick: function (view, record, item, index, e, eOpts) {
        var me = this;
        if (typeof (record.raw) != 'undefined' || typeof (record.data) != 'undefined') {
            var mpanel = Ext.getCmp('mainPanel');
            //mpanel.setActiveTab(0);

            var keyword = "";
			if (typeof (record.raw) != 'undefined'){
				keyword = record.raw.Keyword;
			}else{
				keyword = record.data.Keyword;
			}


            var fmViewWT = Ext.create('Ext.plug_ins.WTMS_Plugins.WorkTask.winViewWorkTask', { title: "", mainPanelId: "", projectKeyword: keyword, projectDirKeyword: keyword });
            winViewWT = Ext.widget('window', {
                title: '查看工作任务',
                closeAction: 'hide',
                width: fmViewWT.winWidth,
                height: fmViewWT.winHeight,
                minWidth: fmViewWT.winWidth,
                minHeight: fmViewWT.winHeight,
                layout: 'fit',
                resizable: true,
                modal: true,
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: fmViewWT,
                defaultFocus: 'firstName'
            });

            winViewWT.projectKeyword = keyword;

            fmViewWT.getViewTaskDefault();

            winViewWT.show();
            //监听子窗口关闭事件
            winViewWT.on('close', function () {
            });


        }
    }
});