
Ext.define('Ext.plug_ins.WTMS_Plugins.EnterPoint', {
    extend: 'Ext.menu.Item',
    addMenus_WTMS: function (menus, mainPanelId, showRecords) {//参数：menus:菜单，mainPanelId:父控件ID,showRec:需要显示的菜单项
        var me = this;
        var newMenus = menus;

        var menu1 = menus.getMenuItem(showRecords, "发文（新建工作任务）", function () { me.MenuCreateWorkTask(mainPanelId); });
        if (menu1 !== null)
        {
            newMenus.add('-');
            newMenus.add(menu1);
        }

        var menuEditCompany = menus.getMenuItem(showRecords, "编辑参建单位..", function () { me.MenuEditCompany(mainPanelId); });
        if (menuEditCompany !== null) {
            newMenus.add('-');
            newMenus.add(menuEditCompany);
        }

        return newMenus;
    },

    MenuCreateWorkTask: function (mainPanelId) {
        var me = this;
        me.viewTree = Ext.getCmp(mainPanelId).down('treepanel');//获取目录树控件ID
        var nodes = me.viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {

            var fmCreateWT = Ext.create('Ext.plug_ins.WTMS_Plugins.WorkTask.winCreateWorkTask', { title: "", mainPanelId: mainPanelId, projectKeyword: nodes[0].data.Keyword, projectDirKeyword: nodes[0].data.Keyword });
            winCreateWT = Ext.widget('window', {
                title: '发文（新建工作任务）',
                closeAction: 'hide',
                width: fmCreateWT.winWidth,
                height: fmCreateWT.winHeight,
                minWidth: fmCreateWT.winWidth,
                minHeight: fmCreateWT.winHeight,
                layout: 'fit',
                resizable: true,
                modal: true,
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: fmCreateWT,
                defaultFocus: 'firstName'
            });

            winCreateWT.projectDirKeyword = nodes[0].data.Keyword;

            winCreateWT.show();
            //监听子窗口关闭事件
            winCreateWT.on('close', function () {
            });
            
        }
    },

    //编辑参建单位
    MenuEditCompany: function (mainPanelId) {
        var me = this;
        //Ext.Msg.alert("", "Hello World !");

        var viewTree = Ext.getCmp(mainPanelId).down('treepanel');//获取目录树控件ID

        var nodes = viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {
            var projectKeyword = nodes[0].data.Keyword;

            var fmEditCompany = Ext.create('Ext.plug_ins.WTMS_Plugins.Company.EditCompany',// + plugins + '.' + state,
                { title: "", mainPanelId: mainPanelId, projectKeyword: projectKeyword });

            winEditCompany = Ext.widget('window', {
                title: '编辑参建单位',
                closeAction: 'hide',
                width: 788,
                height: 538,
                minWidth: 788,
                minHeight: 538,
                layout: 'fit',
                resizable: false,
                modal: true,
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: fmEditCompany,
                defaultFocus: 'firstName'
            });

            fmEditCompany.createProjectButton.setVisible(false);

            winEditCompany.show();

            //监听子窗口关闭事件
            winEditCompany.on('close', function () {
                //var tree = Ext.getCmp(me.mainPanelId).up('_mainSourceView').down('_mainProjectTree').down('treepanel');
                var viewTreeStore = viewTree.store;

                viewTreeStore.load({
                    callback: function (records, options, success) {//添加回调，获取子目录的文件数量

                        //展开目录
                        Ext.require('Ext.ux.Common.comm', function () {
                            Ext.getCmp('contentPanel').down('_mainProjectTree').ExpendProject(projectKeyword);
                        });

                    }
                });
            });

        }
    },
});
Ext.onReady;



