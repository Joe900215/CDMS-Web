//流程工作台控制器
Ext.define('CDMSWeb.controller._WorkflowPlace', {
    extend: 'Ext.app.Controller',
    //视图需要用到操作模型，分别为目录模型，目录树模型，目录combo模型，文章内容模型
    //这里的models不引用，后面再view里面调用会出现问题(reader.read is not a function错误)
    models: [
        'Message', 'MessageTree', 'MessageContent', 'Audit', 'WorkFlowPage', 'WorkflowPlace', 'WorkflowPlaceTree'
    ],

    //视图需要用到目录树及文章内容的Store(存储器)
    //列在这里的store,加载view的时候都会自动访问一次url
    stores: [
        'Messages'//, 'MessagesContent', 'ProjectsTree', 'Audits', 'WorkFlowPages' ,'MessagesTree'
    ],

    //引用视图：文章内容视图，分类修改视图，内容修改视图
    views: [
        '_WorkflowPlace.View', 'Message.contextMenu'
    ],

    //添加一些引用来获取视图和按钮,就可以通过get方法获得各控件
    refs: [
          //ref配置项生成引用的方法，可通过getUserPanel获取面板
          //而selector配置项就是面板的选择器，在这里使用它的id选择
          //视图路径：\scripts\app\view\mainpanel.js
         { ref: "_WorkflowPlacePanel", selector: "#_workflowplacePanel" },
         { ref: "AttaGrid", selector: "#attaGrid" }

    ],

    //加载init进入显示
    init: function () {
        var me = this,
            panel = me.get_WorkflowPlacePanel();  //获取ContentPanel实例

        me.view = Ext.widget("_workflowplaceview");//视图路径：\Scripts\app\view\_WorkflowPlace\View.js
        panel.add(me.view);
        
        ////添加目录树节点选择事件
        //me.view.down("treepanel").on("selectionchange", me.onTreeSelect, me);

        ////消息列表item选择事件
        //me.view.down("gridpanel").on("itemclick", me.onContentItemclick, me);

        //消息的附件列表，右键菜单事件
        //me.getAttaGrid().on("itemcontextmenu", me.contextMenu, me);

        //me.control({
        //    'contextmenu menuitem[action=submenu1]': {
        //        click: this.submenu1OnClick
        //    }

        //});

    },



    //点击目录树节点事件
    onTreeSelect: function (model, sels) {
    var me = this,
    id = sels[0].data.id;
    text = sels[0].data.text;

    if (sels.length > 0) {
        id = sels[0].data.id;

        var store = me.getMessagesStore();//路径：\simplecdms\scripts\app\store\contents.js
        store.proxy.extraParams.MessageType = id;//把参数传给C#MVC,路径：\simplecdms\controllers\projectcontroller.cs 下的 GetChildProject()
        store.proxy.extraParams.sid = localStorage.getItem("sid");
        //store.loadPage(1);
        store.load({
            callback: function (records, options, success) {//添加回调，获取子目录的文件数量
                if (records.length > 0) {
                    //  Ext.Msg.alert("错误", "1");
                    var record = records[0];
                    me.displayMessAttr(record);
                }
            }
        });
    }

    },

    //点击消息列表事件
    onContentItemclick: function (view, record, item, index, e) {
        var me = this;
        if (typeof (record.raw) != 'undefined' || typeof (record.data) != 'undefined') {
            me.displayMessAttr( record);

        }
    },

    //显示第一条消息
    loadFirstMessAttr: function () {
        var me = this;
        if (records.length > 0) {
            var record = records[0];
            me.displayMessAttr(record);
        }
    },

    //显示选中的消息
    displayMessAttr: function (record) {
        var me = this;
        var Keyword = record.data.Keyword;
        var sender = record.data.Sender;
        var title = record.data.Title;

        var MessKeyword = record.data.Keyword;

        var storeMess = me.getMessagesContentStore();//路径：\simplecdms\scripts\app\store\MessagesContent.js
        storeMess.proxy.extraParams.MessageKeyword = MessKeyword;//把参数传给C#MVC,路径：\simplecdms\controllers\projectcontroller.cs 下的 GetChildProject()
        storeMess.proxy.extraParams.sid = localStorage.getItem("sid");
        storeMess.load({
            callback: function (records, options, success) {
                Ext.getCmp('_textareaMessageContent').setValue(records[0].data.Content);
                Ext.getCmp('_displayfieldRecUsers').setValue(records[0].data.RecUsers);
                Ext.getCmp('_displayfieldSender').setValue(records[0].data.Sender);
                Ext.getCmp('_displayfieldTitle').setValue(records[0].data.Title);

                //当文档没有流程时，隐藏TAB页
                if (records[0] !== null && records[0] !== undefined && records[0].data.HasWorkFlow === "true") {

                    //加载流程意见页
                    ////获取流程TAB
                    var tabWf = me.view.down('_workFlowPage');
                    //tab.loadAttrPage();
                    tabWf.loadWorkflowAuditPage("WorkFlow", records[0].data.WorkFlowKeyword);
                }
                else {
                    //Ext.getCmp('messageWorkflowTab').hide();
                }
            }
        });
        //storeMess.loadPage(1);
    },

    //转到源目录
    submenu1OnClick: function (widget, event) {
        
        var me = this;
        var mpanel = Ext.getCmp('mainPanel');
        mpanel.setActiveTab(0);
        var store = me.getMessagesContentStore();//路径：\simplecdms\scripts\app\store\MessagesContent.js
        store.each(function (record) {
            if (record.get('Attachment') !== "") {

                //展开目录
                    Ext.getCmp('contentPanel').down('_mainProjectTree').ExpendProject(record.get('AttaKeyword'));
                //})
            }
        });


    },

	    //显示右键菜单方法
    contextMenu: function (tree, record, item, index, e, eOpts) {
        //阻止浏览器默认右键事件
        e.preventDefault();
        e.stopEvent();
        //显示右键菜单
        var view = Ext.widget('contextmenu');
        view.showAt(e.getXY());
    },

});
