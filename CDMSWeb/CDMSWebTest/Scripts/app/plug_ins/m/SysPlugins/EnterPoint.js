

Ext.define('Ext.plug_ins.m.SysPlugins.EnterPoint', {
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

            var menuTv6 = menus.getMenuItem(showRecords, "复制", function () { me.MenuDeleteProject(mainPanelId); });
            if (menuTv6 !== null) {
                newMenus.add(menuTv6);
            }

            var menuTv6 = menus.getMenuItem(showRecords, "删除", function () { me.MenuDeleteProject(mainPanelId); });
            if (menuTv6 !== null) {
                newMenus.add(menuTv6);
            }

        }
        else if (mainPanelId.substr(0, 12) === "_mainDocGrid")//把子菜单添加到文档表格右键菜单
        {
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

            var menuReferCAD = menus.getMenuItem(showRecords, "参照", function () { me.MenuReferCAD(mainPanelId); });//上传替换文件
            if (menuReferCAD !== null) {
                newMenus.add(menuReferCAD);
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
    //addMenus: function (menus, mainPanelId) {
    //    var me = this;
    //    var newMenus = menus;
        //var strsub = mainPanelId.substr(0, 14);
        if (1===2 && mainPanelId.substr(0, 16) === "_mainProjectTree") {//把子菜单添加到目录树右键菜单
            //var menu = new Ext.menu.Item({
            //    text: "新建",
            //    menu: [
            //        { text: "新建目录", handler: function () { me.MenuCreateNewProject(); } },
            //        { text: "复制创建目录", handler: function () { me.OnMenuNewfolder(); } },
            //        { text: "新建文档", handler: function () { me.MenuCreateNewDoc(); } },
            //        { text: "复制创建文档" }
            //    ]
            //});
            //newMenus.add(menu);

            //var menu = new Ext.menu.Item({
            //    text: "新建目录", handler: function () { me.MenuCreateNewProject(mainPanelId); }
            //});
            //newMenus.add(menu);

            //var menu2 = new Ext.menu.Item({
            //    text: "复制创建目录", handler: function () { me.OnMenuNewfolder(mainPanelId); }
            //});
            //newMenus.add(menu2);

            //var menu3 = new Ext.menu.Item({
            //    text: "新建文档", handler: function () { me.MenuCreateNewDoc(mainPanelId); }
            //});
            //newMenus.add(menu3);

            //var menu4= new Ext.menu.Item({
            //    text: "批量创建文档", handler: function () { me.MenuBatchCreateDoc(mainPanelId); }
            //});
            //newMenus.add(menu4);
            //newMenus.add('-');

            //var menu6 = new Ext.menu.Item({
            //    text: "属性编辑", handler: function () { me.MenuModiProjAttr(mainPanelId); }
            //});
            //newMenus.add(menu6);

            //var menu7 = new Ext.menu.Item({
            //    text: "删除", handler: function () { me.MenuDeleteProject(mainPanelId); }
            //});
            //newMenus.add(menu7);
        }
        else if (1===2 && mainPanelId.substr(0, 12) === "_mainDocGrid")//把子菜单添加到文档表格右键菜单
        {
            //var menu = new Ext.menu.Item({
            //    text: "属性编辑", handler: function () { me.MenuModiDocAttr(mainPanelId); }
            //});
            //newMenus.add(menu);

            ////var menu2 = new Ext.menu.Item({
            ////    text: "启动流程", handler: function () { me._startProcess(mainPanelId); }
            ////});
            ////newMenus.add(menu2);

            //newMenus.add('-');
            //var menu3 = new Ext.menu.Item({
            //    text: "删除", handler: function () { me.MenuDeleteDoc(mainPanelId); }
            //});
            //newMenus.add(menu3);
        }
        return newMenus;
    },


    //新建目录菜单
    MenuCreateNewProject: function (mainPanelId) {
        var me = this;
        Ext.getCmp(mainPanelId).CreateNewProject();
    },

    //复制创建目录菜单
    MenuCreateProjByDef: function (mainPanelId) {
        var me = this;
        Ext.getCmp(mainPanelId).CreateProjByDef();
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

    //修改文档属性
    MenuModiDocAttr: function (mainPanelId) {
        var me = this;
        Ext.getCmp(mainPanelId).up('_mainSourceView').down('_mainDocGrid').ModiDocAttr();
    },

    //预览文档
    MenuPreviewDoc: function (mainPanelId) {
        var me = this;
        Ext.getCmp(mainPanelId).up('_mainSourceView').down('_mainDocGrid').PreviewDoc("false", "false", "", "");
    },

    //组件里面调用CAD外部文件参照功能
    MenuReferCAD: function (mainPanelId) {
        var me = this;

        //判断浏览器的类型，是否使用cefSharp浏览
        if (_browserName == "cefSharp") {
            //Ext.Msg.alert("错误信息", "调用参照！");
            var gridView = Ext.getCmp(mainPanelId).up('_mainProjectView').down('_mainDocGrid');
            var grid = gridView.maindocgrid;
            var rs = grid.getSelectionModel().getSelection();//获取选择的文档

            if (rs !== null && rs.length > 0) {

                var Keyword = "";
                var i = 0;
                //for (var i = 0; i < rs.length ; i++) {

                    Keyword = rs[i].data.Keyword;//获取文档关键字

                    //判断浏览器的类型，是否使用cefSharp浏览
                    if (_browserName == "cefSharp") {
                        //调用C#定义事件
                        jsObj.OnDocGridReferCAD(localStorage.getItem("sid"), Keyword);
                        return;
                        //continue;
                    }
                //}
            }

        } else {
            var gridView = Ext.getCmp(mainPanelId).up('_mainProjectView').down('_mainDocGrid');
            var grid = gridView.maindocgrid;
            var rs = grid.getSelectionModel().getSelection();//获取选择的文档

            if (rs !== null && rs.length > 0) {

                var Keyword = "";
                var i = 0;
                //for (var i = 0; i < rs.length ; i++) {

                Keyword = rs[i].data.Keyword;//获取文档关键字

                //判断浏览器的类型，是否使用cefSharp浏览
                //if (_browserName == "cefSharp") {
                    //调用C#定义事件
                    //Ext.Msg.alert("错误信息", "请在CAD里调用参照！");
                    //window.external.MyMessageBox(_browserName)
                    window.external.OnDocGridReferCAD(localStorage.getItem("sid"), Keyword)
                    return;
                    //continue;
                //}
                //}
            }

        }
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


