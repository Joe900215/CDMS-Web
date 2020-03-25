Ext.define('Ext.plug_ins.HXPC_Plugins.EnterPoint', {
    extend: 'Ext.menu.Item',
    addMenus_HXPC: function (menus, mainPanelId, showRecords) {//参数：menus:菜单，mainPanelId:父控件ID,showRec:需要显示的菜单项
        var me = this;
        var hxpcMenus = menus;
			
        //定义菜单项
        // 这里的”HelloWorld”是菜单名，MenuNameA1是对应的函数名, mainPanelId用于传递发起控件的ID
        var menuBIMView = menus.getMenuItem(showRecords, "查看BIM模型", function () { me.MenuBIMView(mainPanelId); });
        if (menuBIMView !== null)
        {
            hxpcMenus.add('-');
            hxpcMenus.add(menuBIMView);
        }

        var menuSelectProfession = menus.getMenuItem(showRecords, "创建设计阶段与选择专业...", function () { me.MenuSelectProfession(mainPanelId); });
        if (menuSelectProfession !== null) {
            hxpcMenus.add('-');
            hxpcMenus.add(menuSelectProfession);
        }

        var menuExchangeDoc = menus.getMenuItem(showRecords, "生成提资单...", function () { me.MenuExchangeDoc(mainPanelId); });
        if (menuExchangeDoc !== null) {
            hxpcMenus.add('-');
            hxpcMenus.add(menuExchangeDoc);
        }

        var menuExchangeDocUpEdition = menus.getMenuItem(showRecords, "提资升版...", function () { me.MenuExchangeDocUpEdition(mainPanelId); });
        if (menuExchangeDocUpEdition !== null) {
            hxpcMenus.add('-');
            hxpcMenus.add(menuExchangeDocUpEdition);
        }

        var menuExchangeDocContinue = menus.getMenuItem(showRecords, "继续提资...", function () { me.MenuExchangeDocContinue(mainPanelId); });
        if (menuExchangeDocContinue !== null) {
            hxpcMenus.add('-');
            hxpcMenus.add(menuExchangeDocContinue);
        }

        var menuCompany = menus.getMenuItem(showRecords, "新建厂家资料...", function () { me.MenuCompany(mainPanelId); });
        if (menuCompany !== null) {
            hxpcMenus.add('-');
            hxpcMenus.add(menuCompany);
        }

        var menuSendDocument = menus.getMenuItem(showRecords, "创建发文...", function () { me.MenuSendDocument(mainPanelId); });
        if (menuSendDocument !== null) {
            hxpcMenus.add('-');
            hxpcMenus.add(menuSendDocument);
        }

        var menuCreateProdect = menus.getMenuItem(showRecords, "创建成品...", function () { me.MenuCreateProdect(mainPanelId); });
        if (menuCreateProdect !== null) {
            hxpcMenus.add('-');
            hxpcMenus.add(menuCreateProdect);
        }

        var menuWorkTask = menus.getMenuItem(showRecords, "创建任务...", function () { me.MenuWorkTask(mainPanelId); });
        if (menuWorkTask !== null) {
            hxpcMenus.add('-');
            hxpcMenus.add(menuWorkTask);
        }

        var menuTaskReport = menus.getMenuItem(showRecords, "工日填报...", function () { me.MenuTaskReport(mainPanelId); });
        if (menuTaskReport !== null) {
            hxpcMenus.add('-');
            hxpcMenus.add(menuTaskReport);
        }

        var menuProjectInfo = menus.getMenuItem(showRecords, "批量立项...", function () { me.MenuProjectInfo(mainPanelId); });
        if (menuProjectInfo !== null) {
            hxpcMenus.add('-');
            hxpcMenus.add(menuProjectInfo);
        }

        var menuProjectExport = menus.getMenuItem(showRecords, "导出项目...", function () { me.MenuProjectExport(mainPanelId); });
        if (menuProjectExport !== null) {
            hxpcMenus.add('-');
            hxpcMenus.add(menuProjectExport);
        }

        return hxpcMenus;
    },



    //定义菜单项对应的函数   
    //BIM预览
    MenuBIMView: function (mainPanelId) {
        var me = this;
        //Ext.Msg.alert("", "Hello World !");

        me.viewGrid = Ext.getCmp(mainPanelId).down('gridpanel');//获取目录树控件ID

        var nodes = me.viewGrid.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {

            //获取选取的文档keyword
            var docKeyword = nodes[0].data.Keyword;
            var docDesc= nodes[0].data.Title;

            window.open("BIMBrowse?docKeyword=" + docKeyword + "&docDesc=" + docDesc, '_blank');//新窗口打开链接

            //var fmBIMView = Ext.create('Ext.plug_ins.HXPC_Plugins.winBIMView', { title: "", mainPanelId: mainPanelId, docKeyword: docKeyword });
            //winBIMView = Ext.widget('window', {
            //    title: '查看BIM模型',
            //    closeAction: 'hide',
            //    width: '95%',//780,
            //    height: '95%',//580,
            //    minWidth: 500,
            //    minHeight: 580,
            //    layout: 'fit',
            //    resizable: true,
            //    modal: true,
            //    closeAction: 'close', //close 关闭  hide  隐藏  
            //    items: fmBIMView,
            //    defaultFocus: 'firstName'
            //});

            //winBIMView.show();
            ////监听子窗口关闭事件
            //winBIMView.on('close', function () {
            //});

        }
    },

    MenuSelectProfession: function (mainPanelId){
        var me = this;
        //Ext.Msg.alert("", "Hello World !");

        me.viewTree = Ext.getCmp(mainPanelId).down('treepanel');//获取目录树控件ID

        var nodes = me.viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {
            var projectKeyword = nodes[0].data.Keyword;

            var plugins = "HXPC_Plugins";
            var state = "selectProfession";

            //项目阶段的值
            var projectDesc = "";

            var fmSelectProfession = Ext.create('Ext.plug_ins.' + plugins + '.' + state,
                { title: "", mainPanelId: mainPanelId, projectDesc: projectDesc, projectKeyword: projectKeyword, reSelect:'true' });

            winSelectProfession = Ext.widget('window', {
                title: '选择参与专业',
                closeAction: 'hide',
                width: 550,
                height: 450,
                minWidth: 550,
                minHeight: 450,
                layout: 'fit',
                resizable: true,
                modal: true,
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: fmSelectProfession,
                defaultFocus: 'firstName'
            });

            fmSelectProfession.send_get_profession_list(function () {
                //获取已创建的专业
                fmSelectProfession.send_get_created_profession(function () {
                    winSelectProfession.show();
                });
            });



            //监听子窗口关闭事件
            winSelectProfession.on('close', function () {

            });
        }
    },

    //生成提资单...
    MenuExchangeDoc: function (mainPanelId) {
        var me = this;
        //Ext.Msg.alert("", "Hello World !");

        me.viewTree = Ext.getCmp(mainPanelId).down('treepanel');//获取目录树控件ID

        var nodes = me.viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {
            var projectKeyword = nodes[0].data.Keyword;

            var plugins = "HXPC_Plugins";
            var state = "exchangeDoc";

            var fmExchangeDoc = Ext.create('Ext.plug_ins.' + plugins + '.' + state,
                { title: "", mainPanelId: mainPanelId, projectKeyword: projectKeyword, exchangeType: 'Create' });

            winExchangeDoc = Ext.widget('window', {
                title: '互提资料单填写',
                closeAction: 'hide',
                width: 618,
                height: 603,
                minWidth: 618,
                minHeight: 603,
                layout: 'fit',
                resizable: false,
                modal: true,
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: fmExchangeDoc,
                defaultFocus: 'firstName'
            });

            //fmSelectProfession.send_get_profession_list(function () {
            //    //获取已创建的专业
            //    fmSelectProfession.send_get_created_profession(function () {
            //        winExchangeDoc.show();
            //    });
            //});

            winExchangeDoc.show();

            //监听子窗口关闭事件
            winExchangeDoc.on('close', function () {

            });
        }
    },

    //提资升版...
    MenuExchangeDocUpEdition: function (mainPanelId) {
        var me = this;
        //Ext.Msg.alert("", "Hello World !");

        me.viewTree = Ext.getCmp(mainPanelId).down('treepanel');//获取目录树控件ID

        var nodes = me.viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {
            var projectKeyword = nodes[0].data.Keyword;

            var plugins = "HXPC_Plugins";
            var state = "exchangeDoc";

            var fmExchangeDoc = Ext.create('Ext.plug_ins.' + plugins + '.' + state,
                { title: "", mainPanelId: mainPanelId, projectKeyword: projectKeyword, exchangeType: 'UpEdition' });

            winExchangeDoc = Ext.widget('window', {
                title: '互提资料单填写',
                closeAction: 'hide',
                width: 618,
                height: 603,
                minWidth: 618,
                minHeight: 603,
                layout: 'fit',
                resizable: false,
                modal: true,
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: fmExchangeDoc,
                defaultFocus: 'firstName'
            });

            //fmSelectProfession.send_get_profession_list(function () {
            //    //获取已创建的专业
            //    fmSelectProfession.send_get_created_profession(function () {
            //        winExchangeDoc.show();
            //    });
            //});

            winExchangeDoc.show();

            //监听子窗口关闭事件
            winExchangeDoc.on('close', function () {

            });
        }
    },

    //继续提资...
    MenuExchangeDocContinue: function (mainPanelId) {
        var me = this;
        Ext.Msg.alert("", "Hello World !");

        me.viewTree = Ext.getCmp(mainPanelId).down('treepanel');//获取目录树控件ID

        var nodes = me.viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {
            var projectKeyword = nodes[0].data.Keyword;
        }
    },

    //新建厂家资料...
    MenuCompany: function (mainPanelId) {
        var me = this;
        //Ext.Msg.alert("", "Hello World !");

        me.viewTree = Ext.getCmp(mainPanelId).down('treepanel');//获取目录树控件ID

        var nodes = me.viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {
            var projectKeyword = nodes[0].data.Keyword;

            var plugins = "HXPC_Plugins";
            var state = "editCompany";

            var fmEditCompany = Ext.create('Ext.plug_ins.' + plugins + '.' + state,
                { title: "", mainPanelId: mainPanelId, projectKeyword: projectKeyword });

            winEditCompany = Ext.widget('window', {
                title: '新建厂家资料目录',
                closeAction: 'hide',
                width: 765,
                height: 518,
                minWidth: 765,
                minHeight: 518,
                layout: 'fit',
                resizable: false,
                modal: true,
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: fmEditCompany,
                defaultFocus: 'firstName'
            });

            winEditCompany.show();

            //监听子窗口关闭事件
            winEditCompany.on('close', function () {

            });
        }
    },

    //创建发文...
    MenuSendDocument: function (mainPanelId) {
        var me = this;
        //Ext.Msg.alert("", "Hello World !");

        me.viewTree = Ext.getCmp(mainPanelId).down('treepanel');//获取目录树控件ID

        var nodes = me.viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {
            var projectKeyword = nodes[0].data.Keyword;

            var plugins = "HXPC_Plugins";
            var state = "sendDocument";

            var fmSendDocument = Ext.create('Ext.plug_ins.' + plugins + '.' + state,
                { title: "", mainPanelId: mainPanelId, projectKeyword: projectKeyword });

            winSendDocument = Ext.widget('window', {
                title: '发文处理',
                closeAction: 'hide',
                width: 553,
                height: 640,
                minWidth: 553,
                minHeight: 640,
                layout: 'fit',
                resizable: false,
                modal: true,
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: fmSendDocument,
                defaultFocus: 'firstName'
            });


            winSendDocument.show();

            //监听子窗口关闭事件
            winSendDocument.on('close', function () {

            });

        }
    },

    //创建成品...
    MenuCreateProdect: function (mainPanelId) {
        var me = this;
        //Ext.Msg.alert("", "Hello World !");

        me.viewTree = Ext.getCmp(mainPanelId).down('treepanel');//获取目录树控件ID

        var nodes = me.viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {
            var projectKeyword = nodes[0].data.Keyword;

            var plugins = "HXPC_Plugins";
            var state = "createProdect";

            var fmCreateProdect = Ext.create('Ext.plug_ins.' + plugins + '.' + state,
                { title: "", mainPanelId: mainPanelId, projectKeyword: projectKeyword });

            winCreateProdect = Ext.widget('window', {
                title: '新建成品单目录',
                closeAction: 'hide',
                width: 493,
                height: 236,
                minWidth: 493,
                minHeight: 236,
                layout: 'fit',
                resizable: false,
                modal: true,
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: fmCreateProdect,
                defaultFocus: 'firstName'
            });


            winCreateProdect.show();

            //监听子窗口关闭事件
            winCreateProdect.on('close', function () {

            });
        }
    },

    //创建任务...
    MenuWorkTask: function (mainPanelId) {
        var me = this;
        //Ext.Msg.alert("", "Hello World !");

        me.viewTree = Ext.getCmp(mainPanelId).down('treepanel');//获取目录树控件ID

        var nodes = me.viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {
            var projectKeyword = nodes[0].data.Keyword;

            var plugins = "HXPC_Plugins";
            var state = "createTask";

            var fmCreateTask = Ext.create('Ext.plug_ins.' + plugins + '.' + state,
                { title: "", mainPanelId: mainPanelId, projectKeyword: projectKeyword });

            winCreateTask = Ext.widget('window', {
                title: '创建工作任务',
                closeAction: 'hide',
                width: 541,
                height: 373,
                minWidth: 541,
                minHeight: 373,
                layout: 'fit',
                resizable: false,
                modal: true,
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: fmCreateTask,
                defaultFocus: 'firstName'
            });


            winCreateTask.show();

            //监听子窗口关闭事件
            winCreateTask.on('close', function () {

            });
        }
    },

    //工日填报...
    MenuTaskReport: function (mainPanelId) {
        var me = this;
        Ext.Msg.alert("", "Hello World !");

        me.viewTree = Ext.getCmp(mainPanelId).down('treepanel');//获取目录树控件ID

        var nodes = me.viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {
            var projectKeyword = nodes[0].data.Keyword;
        }
    },

    //批量立项...
    MenuProjectInfo: function (mainPanelId) {
        var me = this;
        Ext.Msg.alert("", "Hello World !");

        me.viewTree = Ext.getCmp(mainPanelId).down('treepanel');//获取目录树控件ID

        var nodes = me.viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {
            var projectKeyword = nodes[0].data.Keyword;
        }
    },

    //导出项目...
    MenuProjectExport: function (mainPanelId) {
        var me = this;
        Ext.Msg.alert("", "Hello World !");

        me.viewTree = Ext.getCmp(mainPanelId).down('treepanel');//获取目录树控件ID

        var nodes = me.viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {
            var projectKeyword = nodes[0].data.Keyword;
        }
    }
});
Ext.onReady;

