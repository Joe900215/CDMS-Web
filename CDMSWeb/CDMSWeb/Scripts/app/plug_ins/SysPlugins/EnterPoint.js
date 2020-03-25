

Ext.define('Ext.plug_ins.SysPlugins.EnterPoint', {
    extend: 'Ext.menu.Item',
    addMenus: function (menus, mainPanelId, showRecords) {//参数：menus:菜单，mainPanelId:父控件ID,showRec:需要显示的菜单项
        var me = this;
        var newMenus = menus;

        //Ext.require('Ext.ux.Common.comm', function () {
        if (mainPanelId.substr(0, 16) === "_mainProjectTree") {//把子菜单添加到目录树右键菜单


            var menuTv1 = menus.getMenuItem(showRecords, "新建目录", function () { me.MenuCreateNewProject(mainPanelId); });
            if (menuTv1 !== null) {
                newMenus.add(menuTv1);
            }

            var menuTv1a = menus.getMenuItem(showRecords, "新建根目录", function () { me.MenuCreateNewRootProject(mainPanelId); });
            if (menuTv1a !== null) {
                newMenus.add(menuTv1a);
            }

            //空白处的新建根目录
            var menuTv1b = menus.getMenuItem(showRecords, "新建根目录...", function () { me.MenuCreateNewRootProject(mainPanelId); });
            if (menuTv1b !== null) {
                newMenus.add(menuTv1b);
            }

            //空白处的新建个人工作台
            var menuTvUserCustom = menus.getMenuItem(showRecords, "新建个人工作台...", function () { me.MenuCreateUserCustom(mainPanelId); });
            if (menuTvUserCustom !== null) {
                newMenus.add(menuTvUserCustom);
            }

            //新建逻辑目录
            var menuTvGlobCustom = menus.getMenuItem(showRecords, "新建逻辑目录", function () { me.MenuCreateGlobCustom(mainPanelId); });
            if (menuTvGlobCustom !== null) {
                newMenus.add(menuTvGlobCustom);
            }

            //空白处的新建逻辑目录
            var menuTvGlobCustomC = menus.getMenuItem(showRecords, "新建逻辑目录...", function () { me.MenuCreateRootGlobCustom(mainPanelId); });
            if (menuTvGlobCustomC !== null) {
                newMenus.add(menuTvGlobCustomC);
            }

            //新建查询
            var menuTvGlobSearch = menus.getMenuItem(showRecords, "新建查询", function () { me.MenuCreateGlobSearch(mainPanelId); });
            if (menuTvGlobSearch !== null) {
                newMenus.add(menuTvGlobSearch);
            }

            //空白处的新建查询
            var menuTvGlobSearchC = menus.getMenuItem(showRecords, "新建查询...", function () { me.MenuCreateRootGlobSearch(mainPanelId); });
            if (menuTvGlobSearchC !== null) {
                newMenus.add(menuTvGlobSearchC);
            }

            //编辑查询条件
            var menuTvMdsqlGlobaloruserProject = menus.getMenuItem(showRecords, "编辑查询条件", function () { me.MenuMdsqlGlobaloruserProject(mainPanelId); });
            if (menuTvMdsqlGlobaloruserProject !== null) {
                newMenus.add(menuTvMdsqlGlobaloruserProject);
            }

            var menuTv2 = menus.getMenuItem(showRecords, "复制创建目录", function () { me.MenuCreateProjByDef(mainPanelId); });
            if (menuTv2 !== null) {
                newMenus.add(menuTv2);
            }

            var menuTv3 = menus.getMenuItem(showRecords, "新建文档", function () { me.MenuCreateNewDoc(mainPanelId); });
            if (menuTv3 !== null) {
                newMenus.add(menuTv3);
            }

            var menuTv4 = menus.getMenuItem(showRecords, "批量创建文档", function () { me.MenuBatchCreateDoc(mainPanelId); });
            if (menuTv4 !== null) {
                newMenus.add(menuTv4);
            }

            newMenus.add('-');

            var menuStartFlow = menus.getMenuItem(showRecords, "启动流程", function () { me._startProjFlow(mainPanelId); });
            if (menuStartFlow !== null) {
                newMenus.add(menuStartFlow);
            }

            var menuTv5 = menus.getMenuItem(showRecords, "属性编辑", function () { me.MenuModiProjAttr(mainPanelId); });
            if (menuTv5 !== null) {
                newMenus.add(menuTv5);
            }

            var menuTvSearch = menus.getMenuItem(showRecords, "搜索...", function () { me.MenuSearchProject(mainPanelId); });
            if (menuTvSearch !== null) {
                newMenus.add(menuTvSearch);
            }

            var menuAddFavorites = menus.getMenuItem(showRecords, "添加到收藏夹", function () { me.MenuAddFavorites(mainPanelId); });
            if (menuAddFavorites !== null) {
                newMenus.add(menuAddFavorites);
            }

            var menu_PROJECT_COPY = menus.getMenuItem(showRecords, "复制", function () { me.MenuCopyProject(mainPanelId); });
            if (menu_PROJECT_COPY !== null) {
                newMenus.add('-');
                newMenus.add(menu_PROJECT_COPY);
            }

            var menu_PROJECT_PASTE = menus.getMenuItem(showRecords, "粘贴", function () { me.MenuProjectPaste(mainPanelId); });
            if (menu_PROJECT_PASTE !== null) {
                //newMenus.add('-');
                newMenus.add(menu_PROJECT_PASTE);
            } 

            var menu_PROJECT_PASTE_SHORTCUT = menus.getMenuItem(showRecords, "粘贴快捷方式", function () { me.MenuProjectPasteShortcut(mainPanelId); });
            if (menu_PROJECT_PASTE_SHORTCUT !== null) {
                //newMenus.add('-');
                newMenus.add(menu_PROJECT_PASTE_SHORTCUT);
            }

            var menuTv6 = menus.getMenuItem(showRecords, "删除", function () { me.MenuDeleteProject(mainPanelId); });
            if (menuTv6 !== null) {
                newMenus.add(menuTv6);
            }

        }
        else if (mainPanelId.substr(0, 12) === "_mainDocGrid")//把子菜单添加到文档表格右键菜单
        {
            var menuTvOpenShortcutProject = menus.getMenuItem(showRecords, "转到源目录", function () { me.MenuOpenShortcutProject(mainPanelId); });
            if (menuTvOpenShortcutProject !== null) {
                newMenus.add(menuTvOpenShortcutProject);
            }

            var menuLv1 = menus.getMenuItem(showRecords, "属性编辑", function () { me.MenuModiDocAttr(mainPanelId); });
            if (menuLv1 !== null) {
                newMenus.add(menuLv1);
            }

            var menuLvUpfile = menus.getMenuItem(showRecords, "替换文件", function () { me.MenuReplaceDoc(mainPanelId); });//上传替换文件
            if (menuLvUpfile !== null) {
                newMenus.add(menuLvUpfile);
            }

            var menuPreviewDoc = menus.getMenuItem(showRecords, "预览文件", function () { me.MenuPreviewDoc(mainPanelId); });//上传替换文件
            if (menuPreviewDoc !== null) {
                newMenus.add(menuPreviewDoc);
            }
            
            var menuLv2 = menus.getMenuItem(showRecords, "启动流程", function () { me._startProcess(mainPanelId); });
            if (menuLv2 !== null) {
                newMenus.add(menuLv2);
            }

            var menu_DOC_COPY = menus.getMenuItem(showRecords, "复制", function () { me.MenuCopyDoc(mainPanelId); });
            if (menu_DOC_COPY !== null) {
                newMenus.add('-');
                newMenus.add(menu_DOC_COPY);
            }

            var menu_DOC_MOVE = menus.getMenuItem(showRecords, "剪切", function () { me.MenuMoveDoc(mainPanelId); });
            if (menu_DOC_MOVE !== null) {
               // newMenus.add('-');
                newMenus.add(menu_DOC_MOVE);
            }

            var menuLv3 = menus.getMenuItem(showRecords, "删除", function () { me.MenuDeleteDoc(mainPanelId); });
            if (menuLv3 !== null) {
                //newMenus.add('-');
                newMenus.add(menuLv3);
            }

            var menu_DOC_GIVE_UP = menus.getMenuItem(showRecords, "放弃", function () { me.MenuFreeDoc(mainPanelId); });
            if (menu_DOC_GIVE_UP !== null) {
                // newMenus.add('-');
                newMenus.add(menu_DOC_GIVE_UP);
            }
       }
     
        return newMenus;
    },


    //新建目录菜单
    MenuCreateNewProject: function (mainPanelId) {
        var me = this;
        Ext.getCmp(mainPanelId).CreateNewProject();
    },

    MenuCreateNewRootProject: function (mainPanelId) {
        var me = this;
        Ext.getCmp(mainPanelId).CreateNewProject('true');
    },

    //新建个人工作台
    MenuCreateUserCustom: function (mainPanelId) {
        var me = this;
        Ext.getCmp(mainPanelId).CreateUserCustom('true');
    },

    //新建逻辑目录
    MenuCreateGlobCustom: function (mainPanelId) {
        var me = this;
        Ext.getCmp(mainPanelId).CreateGlobCustom('false');
    },
    
    //新建根逻辑目录
    MenuCreateRootGlobCustom: function (mainPanelId) {
        var me = this;
        Ext.getCmp(mainPanelId).CreateGlobCustom('true');
    },

    //新建根查询
    MenuCreateRootGlobSearch: function (mainPanelId) {
        var me = this;
        Ext.getCmp(mainPanelId).CreateGlobSearch('true');
    },

    //新建查询
    MenuCreateGlobSearch: function (mainPanelId) {
        var me = this;
        Ext.getCmp(mainPanelId).CreateGlobSearch('false');
    },

    //编辑查询条件
    MenuMdsqlGlobaloruserProject: function (mainPanelId) {
        var me = this;
        Ext.getCmp(mainPanelId).MdsqlGlobaloruserProject('false');
    },

    //复制创建目录菜单
    MenuCreateProjByDef: function (mainPanelId) {
        var me = this;
        Ext.getCmp(mainPanelId).CreateProjByDef();
     },

    //复制目录菜单
    MenuCopyProject: function (mainPanelId) {
        var me = this;
        Ext.getCmp(mainPanelId).CopyProject();
    },

    //粘贴到目录菜单
    MenuProjectPaste: function (mainPanelId) {
        var me = this;
        Ext.getCmp(mainPanelId).ProjectPaste();
    },

    //粘贴快捷方式到目录菜单
    MenuProjectPasteShortcut: function (mainPanelId) {
        var me = this;
        Ext.getCmp(mainPanelId).ProjectPasteShortcut();
    },

    //删除目录菜单
    MenuDeleteProject: function (mainPanelId) {
        var me = this;
        Ext.getCmp(mainPanelId).DelProject("false");
    },


    //新建文档菜单
    MenuCreateNewDoc: function (mainPanelId) {
        var me = this;
        Ext.getCmp(mainPanelId).up('_mainSourceView').down('_mainDocGrid').CreateNewDoc();
    },



    //修改目录属性
    MenuModiProjAttr: function (mainPanelId) {
        var me = this;
        Ext.getCmp(mainPanelId).ModiProjAttr();
    },

    //搜索目录
    MenuSearchProject: function (mainPanelId) {
        var me = this;
        Ext.getCmp(mainPanelId).SearchProject();
    },

    //添加到收藏夹
    MenuAddFavorites: function (mainPanelId) {
        var me = this;
        Ext.getCmp(mainPanelId).AddFavorites();
    },

    //修改文档属性
    MenuModiDocAttr: function (mainPanelId) {
        var me = this;
        Ext.getCmp(mainPanelId).up('_mainSourceView').down('_mainDocGrid').ModiDocAttr();
    },

    //转到源目录
    MenuOpenShortcutProject: function (mainPanelId) {
        var me = this;
        Ext.getCmp(mainPanelId).up('_mainSourceView').down('_mainDocGrid').OpenShortcutProject();
    },

    //预览文档
    MenuPreviewDoc: function (mainPanelId) {
        var me = this;
        Ext.getCmp(mainPanelId).up('_mainSourceView').down('_mainDocGrid').PreviewDoc("false", "false", "", "");
    },

    //上传替换文件
    MenuReplaceDoc: function (mainPanelId) {
        var me = this;
        Ext.getCmp(mainPanelId).up('_mainSourceView').down('_mainDocGrid').ReplaceDoc();
    },

    //复制文档菜单
    MenuCopyDoc: function (mainPanelId) {
        var me = this;
        Ext.getCmp(mainPanelId).up('_mainSourceView').down('_mainDocGrid').CopyDoc();
    },

    //复制文档菜单
    MenuMoveDoc: function (mainPanelId) {
        var me = this;
        Ext.getCmp(mainPanelId).up('_mainSourceView').down('_mainDocGrid').MoveDoc();
    },

    //删除文档菜单
    MenuDeleteDoc: function (mainPanelId) {
        var me = this;
        Ext.getCmp(mainPanelId).up('_mainSourceView').down('_mainDocGrid').DeleteDoc("false");
    },

    //放弃文档菜单
    MenuFreeDoc: function (mainPanelId) {
        var me = this;
        Ext.getCmp(mainPanelId).up('_mainSourceView').down('_mainDocGrid').FreeDoc();
    },

    //批量创建文档菜单
    MenuBatchCreateDoc: function (mainPanelId) {
        var me = this;
        Ext.getCmp(mainPanelId).up('_mainSourceView').down('_mainDocGrid').BatchCreateDoc();

    },

    //启动文档流程菜单
    _startProcess: function (mainPanelId) {
        Ext.getCmp('contentPanel').down('_mainDocGrid')._StartProcess();
    },

    //启动目录流程菜单
    _startProjFlow: function (mainPanelId) {
        Ext.getCmp('contentPanel').down('_mainProjectTree')._StartProjFlow();
    },

});
Ext.onReady;


