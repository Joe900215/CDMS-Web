Ext.define('Ext.ux.YDForm._contextMenu', {
    extend: 'Ext.menu.Menu',
    //id: 'contextMenu',
    alias: 'widget._contextmenu',
    //requires: ["Ext.ux.ContextMenu.test"],
    float: true,
    items: [
    ],

    //获取一个菜单子项参数：newMenus：菜单，showRecords：要显示的菜单记录集合，menuText：菜单文本，menuFunc：菜单触发的事件
    getMenuItem:function (showRecords, menuText, menuFunc) {

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

    //显示菜单，bContainer：是否是空白处的菜单,
    //tvPosition: 菜单的位置，文档，个人工作台，逻辑目录或查询
    //Default = 0,
    //TVDocument = 1,
    //TVLogicProject = 2,
    //TVUserWorkSpace = 3,
    //TVGlobalSearch = 4,
    //TVUserSearch = 5,

    showMainPanelMenu: function (view, e, bContainer, tvPosition) {
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
            if (bContainer != true)
                Position = "TVProject";
            else
                Position = "TVContainer";
        } else {
            panel = view.up('_mainDocGrid');
            if (panel != undefined) {
                mainPanelId = panel.id;//"_DocGrid";
                if (bContainer != true)
                    Position = "LVDoc";
                else
                    Position = "LVContainer";
            }
        }

        var objList = "";

        var rs = view.getSelectionModel().getSelection();//获取选择的文档
        if (rs && Position != "TVContainer") {
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
                        objList = rs[i].data.Keyword
                    else objList = objList + "," + rs[i].data.Keyword;
                }
            }
        }

        if (view.up('contentview') != undefined) {
            tvPosition = TvMenuPositon.TVDocument
        }
        else if (view.up('_workplaceview') != undefined) {
            tvPosition = TvMenuPositon.TVUserWorkSpace
        }
        else if (view.up('_logicprojectview') != undefined) {
            tvPosition = TvMenuPositon.TVLogicProject
        }
        else if (view.up('_queryview') != undefined) {
            tvPosition = TvMenuPositon.TVGlobalSearch
        }
         
        Ext.Ajax.request({
            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.WebApi.DBSourceController", A: "GetMenuList",
                sid: localStorage.getItem("sid"), mainPanelId: mainPanelId,
                ProjectList: objList, Position: Position,
                TvPosition: tvPosition
            },
            success: function (response, options) {

                try {
                    //获取到文档数量后，更新到tree
                    var res = Ext.JSON.decode(response.responseText);
                    var state = res.success;
                    if (state === true) {
                        var recods = eval(res.data);
                        Ext.require('Ext.plug_ins.SysPlugins.EnterPoint', function () {
                            var mw2 = Ext.create('Ext.plug_ins.SysPlugins.EnterPoint');
                            //添加系统菜单
                            mw2.addMenus(me, mainPanelId, recods);
                            //添加用户菜单
                            addmenu(me, mainPanelId, recods);
                        });


                    } else {
                        //Ext.Msg.alert("错误", "获取菜单出错！！");
                    }
                } catch (e) {
                    //Ext.Msg.alert("错误", "获取菜单出错！");
                }
            },
            failure: function (response, options) {
                ////Ext.Msg.alert("错误", "连接服务器失败！<br>" + response.responseText);
                //Ext.Msg.alert("错误", "连接服务器失败！<br>" + response.responseText);
            }
        });

        function addmenu(menus, mainPanelId, recods) {
            Ext.require('Ext.plug_ins.HXEPC_Plugins.EnterPoint', function () {
                var mw_HXEPC = Ext.create('Ext.plug_ins.HXEPC_Plugins.EnterPoint');
                mw_HXEPC.addMenus_HXEPC(me, mainPanelId, recods);
            });

            Ext.require('Ext.plug_ins.HXPC_Plugins.EnterPoint', function () {
                var mw_HXPC = Ext.create('Ext.plug_ins.HXPC_Plugins.EnterPoint');
                mw_HXPC.addMenus_HXPC(me, mainPanelId, recods);
            });

            //Ext.require('Ext.plug_ins.SHEPCPlugins.EnterPoint', function () {
            //    var mw = Ext.create('Ext.plug_ins.SHEPCPlugins.EnterPoint');
            //    mw.addMenus2(me, mainPanelId, recods);
            //    me.showAt(e.getXY());
            //});

            Ext.require('Ext.plug_ins.WTMS_Plugins.EnterPoint', function () {
                var mw = Ext.create('Ext.plug_ins.WTMS_Plugins.EnterPoint');
                mw.addMenus_WTMS(me, mainPanelId, recods);
            });

            Ext.require('Ext.plug_ins.ZHEPC_Plugins.EnterPoint', function () {
                var mw = Ext.create('Ext.plug_ins.ZHEPC_Plugins.EnterPoint');
                mw.addMenus2(me, mainPanelId, recods);
                me.showAt(e.getXY());
            });



        }
    }
});