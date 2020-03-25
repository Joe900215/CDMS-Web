Ext.define('Ext.ux.gcz._contextMenu', {
    extend: 'Ext.menu.Menu',
    //id: 'contextMenu',
    alias: 'widget._contextmenu',
    //requires: ["Ext.ux.ContextMenu.test"],
    float: true,
    items: [
    ],

    //获取一个菜单子项参数：newMenus：菜单，showRecords：要显示的菜单记录集合，menuText：菜单文本，menuFunc：菜单触发的事件
    getMenuItem:function (showRecords, menuText, menuFunc) {
        //var menuText = "生成A.1工程开工报审表";
        //var menuFunc = function () { me.MenuCreateWinA1(); }
        var menu1=null;
        for (var i = 0; i < showRecords.length; i++) {  //从节点中取出子节点依次遍历
            var record = showRecords[i];
            if (record.Name === menuText) {
                if (record.State === "Enabled") {
                    menu1 = new Ext.menu.Item({
                        text: menuText, handler: menuFunc 
                    });
                    //newMenus.add(menu1);
                } else if (record.State === "Disabled") {//禁用菜单
                    menu1 = new Ext.menu.Item({
                        text: menuText, disabled: true, handler: menuFunc 
                    });
                    //newMenus.add(menu1);
                }
            }
        }
        return menu1;
    },

    showMainPanelMenu: function (view,e) {
        var me = this;

        //阻止浏览器默认右键事件
        e.preventDefault();
        e.stopEvent();

        var Position = "";//传递菜单位置
        //获取父控件ID
        var panel = view.up('_mainProjectTree');
        var mainPanelId = "";
        if (panel != undefined) {
            mainPanelId = panel.id;//"_projectsTree";
            Position = "TVProject";
        } else {
            panel = view.up('_mainDocGrid');
            if (panel != undefined) {
                mainPanelId = panel.id;//"_DocGrid";
                Position = "LVDoc";
            }
        }

        var objList = "";

        var rs = view.getSelectionModel().getSelection();//获取选择的文档
        if (rs) {
            //获取选取文档关键字
            var objList = "";
            for (var i = 0; i < rs.length ; i++) {
                //遍历每一行
                if (mainPanelId.substr(0, 12) === "_mainDocGrid") {//如果是文档列表控件
                    if (i === 0)
                        objList = rs[i].data.Keyword;
                    else objList = objList + "," + rs[i].data.Keyword;
                } else {//如果是目录树控件
                    if (i === 0)
                        objList = rs[i].data.Keyword;
                    else objList = objList + "," + rs[i].data.Keyword;
                }
            }
        }


        Ext.Ajax.request({
            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.WebApi.DBSourceController", A: "GetMenuList",
                sid: localStorage.getItem("sid"), mainPanelId: mainPanelId,
                ProjectList: objList, Position: Position
            },
            success: function (response, options) {

                try {
                    //获取到文档数量后，更新到tree
                    var res = Ext.JSON.decode(response.responseText);
                    var state = res.success;
                    if (state === true) {
                        var recods = eval(res.data);
                        Ext.require('Ext.plug_ins.m.SysPlugins.EnterPoint', function () {
                            var mw2 = Ext.create('Ext.plug_ins.m.SysPlugins.EnterPoint');
                            mw2.addMenus(me, mainPanelId, recods);
                            addmenu(me, mainPanelId, recods);
                        });


                    }
                } catch (e) { }
            }
        });

        function addmenu(menus, mainPanelId, recods) {
            Ext.require('Ext.plug_ins.m.HXPC_Plugins.EnterPoint', function () {
                var mw_HXPC = Ext.create('Ext.plug_ins.m.HXPC_Plugins.EnterPoint');
                mw_HXPC.addMenus_HXPC(me, mainPanelId, recods);
            });

            Ext.require('Ext.plug_ins.m.SHEPCPlugins.EnterPoint', function () {
                var mw = Ext.create('Ext.plug_ins.m.SHEPCPlugins.EnterPoint');
                mw.addMenus2(me, mainPanelId, recods);
                me.showAt(e.getXY());
            });

        }
    }
});