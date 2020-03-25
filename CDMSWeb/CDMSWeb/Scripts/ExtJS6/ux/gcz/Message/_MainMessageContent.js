/*定义消息内容容器*/
Ext.define('Ext.ux.gcz.Message._MainMessageContent', {
    extend: 'Ext.container.Container',
    //extend: 'Ext.panel.Panel',
    alias: 'widget._mainMessageContent', // 此类的xtype类型为buttontransparent  
    //region: "South", //
    //region: "east", //
    //activeTab: 0, 
    //width: "30%", minWidth: 100,
    //split: true, collapsible: true,
    //layout: 'fit',
    //flex: 1,
    layout: {
        type: 'vbox',
        align: 'stretch',
        pack: 'start'
    },
    flex: 1,
    initComponent: function () {
        var me = this;
        me.renderTo = me.el;
        //me.renderTo = Ext.getBody();

        //添加发送方text
        me._SenderText = Ext.create("Ext.form.field.Text", {
            fieldLabel: "发送方", //frame: true, 
            labelWidth: 45,
            labelAlign: "right", margin: '2 5 0 0', readOnly: true,flex:1
        });

        //添加接收方text
        me._RecUsersText = Ext.create("Ext.form.field.Text", {
            fieldLabel: "接收方", //frame: true, 
            labelWidth: 45,
            labelAlign: "right", margin: '2 0 0 0', readOnly: true, flex: 1
        });

        //添加标题text
        me._TitleText = Ext.create("Ext.form.field.Text", {
            fieldLabel: "标题", //frame: true,
            labelWidth: 45,
            labelAlign: "right", margin: '2 5 0 0', readOnly: true
        });

        //添加消息内容Text
        me._MessageContentText = Ext.create("Ext.form.field.TextArea", {
            fieldLabel: '内容',
            layout: 'fit',
            //height: '100%',
            width:'100%',
            readonly: true,
            name: 'msg',
            labelWidth: 45,
            //, minheight: 40
            grow: true,//防止高度收缩到最小的时候，最后面一行显示不出来
            //minheight: 60, //baseCls: 'my-panel-no-border',//隐藏边框
            margin: '2 5 2 0',
            labelAlign: "right",
            flex: 1
        });


        //定义消息内容store
        me._MessagesContentStore = Ext.create("Ext.data.Store", {
            
            model: 'CDMSWap.model.MessageContent',//模型路径：\simplecdms\scripts\app\model\MessageContent.js
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
                    C: "AVEVA.CDMS.WebApi.MessageController", A: "GetMessage",
                    MessageKeyword: 1, total: 50000, sid: localStorage.getItem("sid")
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
            },
            simpleSortMode: true
        });

        ////定义流程TAB页
        me.msgWorkFlowTabPage = Ext.create('Ext.ux.m.WorkFlow._WorkFlowPage');

        me.msgWorkFlowTabPage.mainPanelType = "Message";

        me.msgWorkFlowTabPagePanel = Ext.create('Ext.Panel', {
            layout: {
                type: 'vbox',
                align: 'stretch',
                pack: 'start'
            },
            flex: 1,
            items: [
                 me.msgWorkFlowTabPage
            ]
        });

        //附件表格
        me.attaGrid = Ext.widget("grid", {
            store: me._MessagesContentStore,
            layout: 'fit',
            hideHeaders: true,
            height:'100%',
            //forceFit:true,//取消显示横向滚动条
            columns: [
                {//添加图标
                    //menuDisabled: true,
                    //sortable: false,
                    xtype: 'actioncolumn',
                    //enableColumnResize: false,
                    items: [ {
                        getClass: function (v, metaData, record) {

                            var attatype = record.get('AttaType');
                            if (attatype === "Proj") {
                                return 'myfolder';
                            }
                            else {
                                return  'docunknown';
                            }

                        },
                        tooltip: ''
                    }], width: 18
                },
                {
                    sortable: false,
                    resizable: false,
                    dataIndex: "Attachment", flex: 1

                },
                {
                    sortable: false,
                    resizable: false,
                    dataIndex: "FileSize",
                    width: 70 //'100%'

                }
            ],
            viewConfig: {
                getRowClass: function (record, rowIndex, p, store) {//CSS class name to add to the row.获得一行的css样式  

                    if (record.data.Attachment === "" || record.data.Attachment === undefined)
                        return "hide-store-row";
                }
            },
            listeners: {
                "itemcontextmenu": function (view, record, item, index, e, eOpts) {//添加右键菜单事件
                    //me._showContextMenu(view, record, item, index, e);
                },
                "cellclick": function (view, td, cellIndex, record, tr, rowIndex, e, eOpts) {
                    me.onAttaGridCellClick(view, td, cellIndex, record, tr, rowIndex, e, eOpts);
                }
            },flex:1
        });

        //me.attaFieldSet = Ext.create('Ext.form.FieldSet', {
        me.attaPanel = Ext.create('Ext.Panel', {
            layout: {
                type: 'hbox',
                align: 'stretch',
                pack: 'start'
            },
                width: '100%', 
                //align: 'stretch', pack: 'start',
                margin: '0 5 2 5', padding: '0 0 0 0',

                baseCls: 'my-panel-no-border',//隐藏边框
                flex: 1,

                items: [                                        

                         {
                             xtype: "label",
                             text: "附件:",
                             margin: '0 5 0 12'
                         }, me.attaGrid]


        });


        //me.contextmenu = Ext.create('Ext.menu.Menu', {
        //    float: true,
        //    items: [{
        //        text: '转到源目录',
        //        action: 'submenu1',
        //        iconCls: 'leaf', handler: function () {
        //            me.submenu1OnClick();
        //        }
        //    }]
        //});

            me.maincontentpanel = Ext.create('Ext.Panel', {
                xtype: "panel",
                //height: 160,
                layout: {
                    type: 'vbox',
                    align: 'stretch'
                },
                flex:1,
                defaults: {
                    frame: true,
                    labelWidth: 45,
                    labelAlign: "right",
                    margin: '2 5 0 0',
                    readOnly: true
                },
                items: [
                        //        			{
            			//    xtype:"button",
            			//    text:"我的按钮"
            			//}

                    {
                    xtype: "panel",
                    layout: {
                        type: 'hbox',
                        pack: 'start',
                        align: 'stretch'
                    },
                    defaults: {
                        frame: true,
                        labelWidth: 45,
                        labelAlign: "right",
                        margin: '2 5 0 0',
                        readOnly: true
                    },
                    baseCls: 'my-panel-no-border',//隐藏边框
                    items:[
                        me._SenderText,

                        me._RecUsersText
                    ]
                },
                me._TitleText,

                me._MessageContentText,//消息内容

                me.attaPanel//附件

                ]

            }),

        me.mainmessagepanel = Ext.create('Ext.Panel', {
            xtype: 'panel',
            activeTab: 0,
            defaults: {
                border: false,
                bodyPadding: 0,
                bodyStyle: "background:#DFE9F6"
            },
            baseCls: 'my-panel-no-border',//隐藏边框
            layout: {
                type: 'vbox',
                align: 'stretch'
            },
            flex: 1,
            items: [
                {
                //title: '消息信息',
                    xtype: "panel", //border: false,
                    layout: {
                        type: 'vbox',
                        align: 'stretch'
                    },
                    flex:1,
                items: [
                    me.maincontentpanel
                    ,
                    //me.msgWorkFlowTabPage
                    me.msgWorkFlowTabPagePanel
                ]}                   
            ]
        });

        //添加消息内容页面到容器
        me.items = [
                    me.mainmessagepanel
                        //        			{
            			//    xtype:"button",
            			//    text:"我的按钮"
            			//}
        ];

        me.callParent(arguments);
    },

    //显示选中的消息
    displayMessAttr: function (record) {
    var me = this;
    var Keyword = record.data.Keyword;
    var sender = record.data.Sender;
    var title = record.data.Title;

    var MessKeyword = record.data.Keyword;

    //var storeMess = me._MessagesContentStore;

    me._MessagesContentStore.proxy.extraParams.MessageKeyword = MessKeyword;//把参数传给C#MVC,路径：\simplecdms\controllers\projectcontroller.cs 下的 GetChildProject()
    me._MessagesContentStore.proxy.extraParams.sid = localStorage.getItem("sid");

    me._MessagesContentStore.load({
        callback: function (records, options, success) {

            //设置发送方
            me._RecUsersText.setValue(records[0].data.RecUsers);

            //设置接收方
            me._SenderText.setValue(records[0].data.Sender);

            //设置标题
            me._TitleText.setValue(records[0].data.Title);

            //设置内容
            me._MessageContentText.setValue(records[0].data.Content);

            //当流程没有附件时，隐藏附件页
            if (records.length <= 1 && me.attaPanel.hidden === false) {
                me.attaPanel.hide();
            } else if (records.length > 1 && me.attaPanel.hidden === true) {
                me.attaPanel.show();
            }

            //当文档没有流程时，隐藏TAB页
            if (records[0] !== null && records[0] !== undefined && records[0].data.HasWorkFlow === "true") {
                if (me.msgWorkFlowTabPage.hidden===true)
                    me.msgWorkFlowTabPage.show();

                //加载流程意见页
                ////获取流程TAB
                me.msgWorkFlowTabPage.loadWorkflowAuditPage("WorkFlow", records[0].data.WorkFlowKeyword);
            }
            else {
                //Ext.getCmp('messageWorkflowTab').hide();
                if (me.msgWorkFlowTabPage.hidden===false)
                    me.msgWorkFlowTabPage.hide();
            }
        }
    });

    },

    ////清空内容页面
    //clearMessAttr: function () {
    //    var me=this;

    //    //设置发送方
    //    me._RecUsersText.setValue("");

    //    //设置接收方
    //    me._SenderText.setValue("");

    //    //设置标题
    //    me._TitleText.setValue("");

    //    //设置内容
    //    me._MessageContentText.setValue("");

    //    //清空附件store
    //    me._MessagesContentStore.removeAll();

    //    //清空流程页面
    //    //me.msgWorkFlowTabPage.clearWorkflowAuditPage();
    //},


    ////右键菜单转到源目录
    //submenu1OnClick: function () {//widget, event) {

    //    var me = this;

    //    var nodes = me.attaGrid.getSelectionModel().getSelection();//获取已选取的节点
    //    if (nodes !== null && nodes.length > 0) {
    //        me.mainPanelExpendProject(nodes[0]);
    //    }
    //},

    ////双击附件行跳转到源目录
    //onAttaGridDbClick:function(view, record, item, index, e){
    //    var me = this;
    //    me.mainPanelExpendProject(record);
    //},

    ////双击附件行
    //onAttaGridDbClick:function(view, record, item, index, e){
    //    var me = this;
    //    //me.mainPanelExpendProject(record);
    //    Ext.Msg.alert("错误信息", "aa");
    //},

    ////单击附件行
    //onAttaGridItemClick: function (view, record, item, index, e) {
    //    var me = this;
    //    //me.mainPanelExpendProject(record);
    //    Ext.Msg.alert("错误信息", "aa");
    //},

    //onAttaGridItemMouseUp: function (view, record, item, index, e) {
    //        var me = this;
    //        //me.mainPanelExpendProject(record);
    //        if (index === 1) {
    //            Ext.Msg.alert("错误信息", "aa");
    //        }
    //},

    onAttaGridCellClick: function (view, td, cellIndex, record, tr, rowIndex, e, eOpts){
        var me = this;

        if (cellIndex === 1) {

            ////弹出窗口预览文件
            //me.showPreviewFileWin(record.data.AttaKeyword);

            if (record.data.AttaType === "Doc") {
                var title = "";
                if (record.data.Attachment.length > 4) {
                    title = record.data.Attachment.substring(0, 4) + "..";
                } else { title = record.data.Attachment; }

                me.addTab(record.data.AttaKeyword, title);//"预览文件");
            }

        }
    },

    /**
 * 向tab中添加选项卡
 * @params myId 被添加的组件id  myTitle 创建tabpanel时需要 myurl 将要加载数据的地址
 */
    addTab: function (docKeyword, myTitle) {//, myUrl) {
        var me = this;

        var mainTabPanel = me.up('mainpanel');
        
        var myId = "previewtab" + docKeyword;

        var gid = Ext.getCmp(myId);
        if (gid != null) {
            mainTabPanel.remove(gid.id); // 如果该选项卡面板里已有选项卡，先将其移除
        }

        var fmPreviewFile = Ext.create('Ext.ux.m.File._PreviewFile',{
            ///title: "123",
            mainPanelId: me.id, DocKeyword: docKeyword
        });

        var tp = new  Ext.panel.Panel({
            //iconCls: 'tab',
            id: myId,
            //enableTabScroll: true,
            closable: true,
            title: myTitle,
            layout: 'fit',
            items: fmPreviewFile
        });
        // 向选项卡面板里添加选项卡
        mainTabPanel.add(tp).show();
    },

    //弹出预览文件窗口
    showPreviewFileWin: function (docKeyword) {
        var me = this;
        //Ext.Msg.alert("错误信息", "aa");

        var fmPreviewFile = Ext.create('Ext.ux.m.File._PreviewFile',
            {
                title: "", mainPanelId: "", DocKeyword: docKeyword
            });

        winPreviewFile = Ext.widget('window', {
            title: '预览文件',
            //closeAction: 'hide',
            width: '95%',//300,
            height: '95%',//600,
            minWidth: 300,
            minHeight: 500,
            layout: 'fit',
            resizable: false,
            modal: true,
            closeAction: 'close', //close 关闭  hide  隐藏  
            items: fmPreviewFile,
            defaultFocus: 'firstName'
        });


        winPreviewFile.show();

        //监听子窗口关闭事件
        winPreviewFile.on('close', function () {
            //me.selectReportKeyword = "";

            //me.sendGetTaskReportDefault();
        });

    }


    ////跳转到源目录
    //mainPanelExpendProject: function (record) {
    //    var me = this;
    //    var mpanel = Ext.getCmp('mainPanel');
    //    mpanel.setActiveTab(0);

    //    if (record.get('Attachment') !== "") {

    //        //展开目录
    //        Ext.getCmp('contentPanel').down('_mainProjectTree').ExpendProject(record.get('AttaKeyword'));

    //    }
    //},

    ////显示右键菜单方法
    //_showContextMenu: function (view, record, item, index, e, eOpts) {

    //    var me = this;
    //    //阻止浏览器默认右键事件
    //    e.preventDefault();
    //    e.stopEvent();

    //    //显示右键菜单
    //    me.contextmenu.showAt(e.getXY());


    //}
});