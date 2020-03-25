/*定义消息内容容器*/
Ext.define('Ext.ux.YDForm.WorkflowPlace._MainWorkflowPlaceContent', {
    //extend: 'Ext.container.Container',
    extend: 'Ext.panel.Panel',
    alias: 'widget._mainWorkflowPlaceContent', // 此类的xtype类型为buttontransparent  
    activeTab: 0, region: "east", width: "30%", minWidth: 100, split: true, collapsible: true,
    layout: 'fit',
    initComponent: function () {
        var me = this;
        me.renderTo = me.el;

        //定义消息内容store
        me._WorkflowsContentStore = Ext.create("Ext.data.Store", {
            
            model: 'CDMSWeb.model.MessageContent',//模型路径：\simplecdms\scripts\app\model\WorkflowContent.js
            batchActions: false,
            //文章的Store需要支持远程排序和搜索
            remoteFilter: true,
            remoteSort: true,
            //无限滚动需要//
            sorters: {
                property: 'lastpost',
                direction: 'DESC'
            },

            //每50条记录为一页
            pageSize: 50,

            proxy: {
                type: "ajax",
                url: 'WebApi/Get',
                extraParams: {
                    C: "AVEVA.CDMS.WebApi.WorkFlowController", A: "GetWorkFlowPagesData",
                    Keyword: 1, total: 50000, sid: localStorage.getItem("sid")
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

        //定义流程TAB页
        me.msgWorkFlowTabPage = Ext.create('Ext.ux.YDForm.WorkFlow._WorkFlowPage');

        me.msgWorkFlowTabPage.mainPanelType = "Workflow";

        //附件表格
        me.attaGrid = Ext.widget("grid", {
            store: me._WorkflowsContentStore,
            layout: 'fit',
            hideHeaders: true,
            height: 80,
            columns: [
                {//添加图标
                    menuDisabled: true,
                    sortable: false,
                    xtype: 'actioncolumn',
                    enableColumnResize: false,
                    width: 18,
                    items: [ {
                        getClass: function (v, metaData, record) {

                            var attatype = record.get('AttaType');
                            if (attatype === "Proj") {
                                return 'myfolder';
                            }
                            else {
                                return 'docunknown';
                            }

                        },
                        tooltip: ''
                    }]
                },
                {
                    sortable: false,
                    resizable: false,
                    dataIndex: "Attachment",flex:1
                    //width: 300 //'100%'

                },
                {
                    sortable: false,
                    resizable: false,
                    dataIndex: "FileSize",
                    width: 80 //'100%'

                }
            ],
            viewConfig: {
                getRowClass: function (record, rowIndex, p, store) {//CSS class name to add to the row.获得一行的css样式  
                    if (store.getAt(rowIndex).get('Attachment') === "")
                        return "hide-store-row";
                }
            },
            listeners: {
                "itemcontextmenu": function (view, record, item, index, e, eOpts) {//添加右键菜单事件
                    me._showContextMenu(view, record, item, index, e);
                },
                //鼠标双击表格行事件
                "itemdblclick": function (view, record, item, index, e, eOpts ) {
                    me.onAttaGridDbClick(view, record, item, index, e);
                }
            },flex:1
        });

        me.attaFieldSet = Ext.create('Ext.form.FieldSet', {
            layout: "hbox",
            width: '100%', baseCls: 'my-panel-no-border',//隐藏边框
            align: 'stretch', margin: '5 5 0 5', padding: '0 0 0 0',
            pack: 'start',
            items: [                                        

                     {
                         xtype: "label",
                         text: "附件:",
                         margin: '0 5 0 16'
                     }, me.attaGrid]

        });


        me.contextmenu = Ext.create('Ext.menu.Menu', {
            float: true,
            items: [{
                text: '转到源目录',
                action: 'submenu1',
                iconCls: 'leaf', handler: function () {
                    me.submenu1OnClick();
                }
            }]
        });

        me.mainmessagepanel = Ext.create('Ext.Panel', {
            xtype: 'panel',
            activeTab: 0,
            defaults: {
                border: false,
                bodyPadding: 5, bodyStyle: "background:#DFE9F6"
            },
            items: [
          
            me.msgWorkFlowTabPage
            ]
        });

        //添加消息内容页面到容器
        me.items = [
                    me.mainmessagepanel

        ];

        me.callParent(arguments);
    },

    //显示选中的消息
    displayMessAttr: function (record) {
    var me = this;

    me.msgWorkFlowTabPage.loadWorkflowAuditPage("WorkFlow", record.data.Keyword);

    },

    //清空内容页面
    clearMessAttr: function () {
        var me=this;

        //清空附件store
        me._WorkflowsContentStore.removeAll();

        //清空流程页面
        me.msgWorkFlowTabPage.clearWorkflowAuditPage();
    },


    //右键菜单转到源目录
    submenu1OnClick: function () {//widget, event) {

        var me = this;

        var nodes = me.attaGrid.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {
            me.mainPanelExpendProject(nodes[0]);
        }
    },

    //双击附件行跳转到源目录
    onAttaGridDbClick:function(view, record, item, index, e){
        var me = this;
        me.mainPanelExpendProject(record);
    },

    //跳转到源目录
    mainPanelExpendProject: function (record) {
        var me = this;
        var mpanel = Ext.getCmp('mainPanel');
        mpanel.setActiveTab(0);

        if (record.get('Attachment') !== "") {

            //展开目录
            Ext.getCmp('contentPanel').down('_mainProjectTree').ExpendProject(record.get('AttaKeyword'));

        }
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
});