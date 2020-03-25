Ext.define('Ext.ux.YDForm.Navigation._NavMessageGrid', {
    //extend: 'Ext.container.Container',
    extend: 'Ext.panel.Panel',
    alias: 'widget._navMessageGrid', // 此类的xtype类型为buttontransparent  
    //requires: ['Ext.ux.Common.comm'],
    //region: "center",
    //layout: 'fit', //width: 600,
    flex: 1,
    initComponent: function () {
        var me = this;
        me.renderTo = me.el;

        Ext.define("_NavMessagesModel", {
            extend: 'Ext.data.Model',
            fields: [
                        "Keyword",
        "Sender",
    	"Title",
        "SendDatetime",
    	"Workflow_DESC",
        "SignificantType",//重要程度
        "DelayDay",//延迟天数
        "WorkflowId",
        "Tags"//代码中添加了一个Tags字段，用来显示标签，该字段将以数组形式返回数据，这样，在后续的处理中会很方便。
            ]
        });

        me._NavMessagesStore = Ext.create("Ext.data.Store", {
            model: _NavMessagesModel,//模型路径：\simplecdms\scripts\app\model\content.js
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
                    C: "AVEVA.CDMS.WebApi.MessageController", A: "GetMessageList",
                    MessageType: "/_1", total: 10, sid: localStorage.getItem("sid")
                },
                reader: {
                    type: 'json',
                    totalProperty: 'total',
                    root: "data",//从C#MVC获取数据\simplecdms\controllers\ProjectController.cs .GetDocList.data  ，获取到的数据传送到model里面
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
            },
            simpleSortMode: true
        });

        ////定义grid消息列表
        me.navmessagegrid = Ext.widget("grid", {
            title: "新消息", region: "center",
            _id: "NavMessageGrid",
            renderTo: Ext.getBody(),
            store: me._NavMessagesStore,//"Messages",
     
            selModel: { checkOnly: false, mode: "MULTI" },
       

            columns: [
                { text: '发送者', dataIndex: 'Sender', width: 120 },
                { text: '标题', dataIndex: 'Title', flex: 1 },
                { text: '发送时间', dataIndex: 'SendDatetime', width: 120 }//,
                //{ text: '所属流程', dataIndex: 'Workflow_DESC', width: 200 },
                //{ text: '重要程度', dataIndex: 'SignificantType', width: 90 },
                //{ text: '延迟天数', dataIndex: 'DelayDay', width: 90 }
            ],
            listeners: {
                ////选中某条记录的事件
                //"select": function (rowModel, record, index, eOpts) {
                //    me.OnMessagesGridSelect(rowModel, record, index, eOpts);
                //}
                "itemclick": function (view, record, item, index, e, eOpts) {
                    me.OnMessagesGridItemClick(view, record, item, index, e, eOpts);
                }
            }
        });

        me.items = [
             me.navmessagegrid

        ];

        //var typeId = "/_1";
        //var store = me.navmessagegrid.store;
        //store.proxy.extraParams.MessageType = typeId;//把参数传给C#MVC,路径：\simplecdms\controllers\projectcontroller.cs 下的 GetChildProject()
        //store.proxy.extraParams.sid = localStorage.getItem("sid");
        //store.proxy.extraParams.limit = "15";

        //store.load({
        //    callback: function (records, options, success) {//添加回调，获取子目录的文件数量

        //        //var gridView = me.up("_mainMessageView").down("_mainMessageGrid");
        //        //var gridView = me.navmessagegrid;

        //    }
        //});

        me.callParent(arguments);
    },

    OnMessagesGridItemClick: function (view, record, item, index, e, eOpts) {
        var me = this;

        if (typeof (record.data) != 'undefined') {
            var mpanel = Ext.getCmp('mainPanel');
            mpanel.setActiveTab(1);

            var keyword = record.data.Keyword;

            //展开目录
            //Ext.getCmp('contentPanel').down('_mainProjectTree').ExpendProject(keyword);

        }
    },

    refreshGrid: function () {
        var me = this;

        if (me.navmessagegrid == undefined) return;

        var store = me.navmessagegrid.store;
        store.proxy.extraParams.sid = localStorage.getItem("sid");

        store.load({
            callback: function (records, options, success) {//添加回调，获取子目录的文件数量

                // var gridView = me.navfavoritesgrid;

            }
        });
    }
});