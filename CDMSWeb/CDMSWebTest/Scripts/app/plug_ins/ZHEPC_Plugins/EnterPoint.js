

Ext.define('Ext.plug_ins.ZHEPC_Plugins.EnterPoint', {
    extend: 'Ext.menu.Item',
    addMenus2: function (menus, mainPanelId, showRecords) {//参数：menus:菜单，mainPanelId:父控件ID,showRec:需要显示的菜单项
        var me = this;
        var newMenus = menus;

        //Ext.require('Ext.ux.Common.comm', function () {
            var menu1 = menus.getMenuItem(showRecords, "生成A.1工程开工报审表", function () { me.MenuCreateWinA1(mainPanelId); });
            if (menu1 !== null)
            {
                newMenus.add('-');
                newMenus.add(menu1);
            }
        //})

            var menuBIMView = menus.getMenuItem(showRecords, "查看BIM模型", function () { me.MenuBIMView(mainPanelId); });
            if (menuBIMView !== null) {
                newMenus.add('-');
                newMenus.add(menuBIMView);
            }

        var menuA2 = menus.getMenuItem(showRecords, "生成A.2工程复工申请表", function () { me.MenuCreateWinA2(mainPanelId); });
        if (menuA2 !== null) {
            newMenus.add('-');
            newMenus.add(menuA2);
        }

        var menu2 = menus.getMenuItem(showRecords, "生成A.3施工组织设计报审表", function () { me.MenuCreateWinA3(mainPanelId); });
        if (menu2 !== null) {
            newMenus.add('-');
            newMenus.add(menu2);
        }

        var menu3 = menus.getMenuItem(showRecords, "生成A.4方案报审表", function () { me.MenuCreateWinA4(mainPanelId); });
        if (menu3 !== null) {
            newMenus.add('-');
            newMenus.add(menu3);
        }

        var menuA5 = menus.getMenuItem(showRecords, "生成A.5分包单位资格报审表", function () { me.MenuCreateWinA5(mainPanelId); });
        if (menuA5 !== null) {
            newMenus.add('-');
            newMenus.add(menuA5);
        }

        var menu4 = menus.getMenuItem(showRecords, "生成A.6单位资质报审表", function () { me.MenuCreateWinA6(mainPanelId); });
        if (menu4 !== null) {
            newMenus.add('-');
            newMenus.add(menu4);
        }

        var menu5 = menus.getMenuItem(showRecords, "生成A.7人员资质报审表", function () { me.MenuCreateWinA7(mainPanelId); });
        if (menu5 !== null) {
            newMenus.add('-');
            newMenus.add(menu5);
        }

        var menuA8 = menus.getMenuItem(showRecords, "生成A.8工程控制网测量／线路复测报审表", function () { me.MenuCreateWinA8(mainPanelId); });
        if (menuA8 !== null) {
            newMenus.add('-');
            newMenus.add(menuA8);
        }

        var menu6 = menus.getMenuItem(showRecords, "生成A.9主要施工机械／工器具／安全用具报审表", function () { me.MenuCreateWinA9(mainPanelId); });
        if (menu6 !== null) {
            newMenus.add('-');
            newMenus.add(menu6);
        }

        var menu7 = menus.getMenuItem(showRecords, "生成A.10主要测量计量器具／试验设备检验报审表", function () { me.MenuCreateWinA10(mainPanelId); });
        if (menu7 !== null) {
            newMenus.add('-');
            newMenus.add(menu7);
        }

        var menu8 = menus.getMenuItem(showRecords, "生成A.11质量验收及评定项目划分报审表", function () { me.MenuCreateWinA11(mainPanelId); });
        if (menu8 !== null) {
            newMenus.add('-');
            newMenus.add(menu8);
        }

        var menu9 = menus.getMenuItem(showRecords, "生成A.12工程材料／构配件／设备报审表", function () { me.MenuCreateWinA12(mainPanelId); });
        if (menu9 !== null) {
            newMenus.add('-');
            newMenus.add(menu9);
        }

        var menuA13 = menus.getMenuItem(showRecords, "生成A.13主要设备开箱申请表", function () { me.MenuCreateWinA13(mainPanelId); });
        if (menuA13 !== null) {
            newMenus.add('-');
            newMenus.add(menuA13);
        }

        var menuA14 = menus.getMenuItem(showRecords, "生成A.14验收申请表", function () { me.MenuCreateWinA14(mainPanelId); });
        if (menuA14 !== null) {
            newMenus.add('-');
            newMenus.add(menuA14);
        }

        var menu10 = menus.getMenuItem(showRecords, "生成A.15中间交付验收交接表", function () { me.MenuCreateWinA15(mainPanelId); });
        if (menu10 !== null) {
            newMenus.add('-');
            newMenus.add(menu10);
        }

        var menu11 = menus.getMenuItem(showRecords, "生成A.16计划／调整计划报审表", function () { me.MenuCreateWinA16(mainPanelId); });
        if (menu11 !== null) {
            newMenus.add('-');
            newMenus.add(menu11);
        }

        var menuA17 = menus.getMenuItem(showRecords, "生成A.17费用索赔申请表", function () { me.MenuCreateWinA17(mainPanelId); });
        if (menuA17 !== null) {
            newMenus.add('-');
            newMenus.add(menuA17);
        }

        var menu12 = menus.getMenuItem(showRecords, "生成A.18监理工程师通知回复单", function () { me.MenuCreateWinA18(mainPanelId); });
        if (menu12 !== null) {
            newMenus.add('-');
            newMenus.add(menu12);
        }

        var menu13 = menus.getMenuItem(showRecords, "生成A.19工程款支付申请表", function () { me.MenuCreateWinA19(mainPanelId); });
        if (menu13 !== null) {
            newMenus.add('-');
            newMenus.add(menu13);
        }

        var menuA20 = menus.getMenuItem(showRecords, "生成A.20工期变更报审表", function () { me.MenuCreateWinA20(mainPanelId); });
        if (menuA20 !== null) {
            newMenus.add('-');
            newMenus.add(menuA20);
        }

        var menu14 = menus.getMenuItem(showRecords, "生成A.21设备／材料／构配件缺陷通知单", function () { me.MenuCreateWinA21(mainPanelId); });
        if (menu14 !== null) {
            newMenus.add('-');
            newMenus.add(menu14);
        }

        var menu15 = menus.getMenuItem(showRecords, "生成A.22设备／材料／构配件缺陷处理报验表", function () { me.MenuCreateWinA22(mainPanelId); });
        if (menu15 !== null) {
            newMenus.add('-');
            newMenus.add(menu15);
        }

        var menuA23 = menus.getMenuItem(showRecords, "生成A.23单位工程验收申请表", function () { me.MenuCreateWinA23(mainPanelId); });
        if (menuA23 !== null) {
            newMenus.add('-');newMenus.add(menuA23);
        }

        var menuA24 = menus.getMenuItem(showRecords, "生成A.24工程竣工报验单", function () { me.MenuCreateWinA24(mainPanelId); });
        if (menuA24 !== null) {
            newMenus.add('-');newMenus.add(menuA24);
        }

        var menu16 = menus.getMenuItem(showRecords, "生成A.25设计变更／变更设计执行情况反馈单", function () { me.MenuCreateWinA25(mainPanelId); });
        if (menu16 !== null) {
            newMenus.add('-');newMenus.add(menu16);
        }

        var menu17 = menus.getMenuItem(showRecords, "生成A.26大中型施工机械进场／出场申报表", function () { me.MenuCreateWinA26(mainPanelId); });
        if (menu17 !== null) {
            newMenus.add('-');newMenus.add(menu17);
        }

        var menu18 = menus.getMenuItem(showRecords, "生成A.27作业指导书报审表", function () { me.MenuCreateWinA27(mainPanelId); });
        if (menu18 !== null) {
            newMenus.add('-');newMenus.add(menu18);
        }

        var menuA28 = menus.getMenuItem(showRecords, "生成A.28工程沉降观测报审表", function () { me.MenuCreateWinA28(mainPanelId); });
        if (menuA28 !== null) {
            newMenus.add('-');newMenus.add(menuA28);
        }

        var menuA29 = menus.getMenuItem(showRecords, "生成A.29工程量签证单", function () { me.MenuCreateWinA29(mainPanelId); });
        if (menuA29 !== null) {
            newMenus.add('-');newMenus.add(menuA29);
        }

        var menuA30 = menus.getMenuItem(showRecords, "生成A.30总承包单位资质报审表", function () { me.MenuCreateWinA30(mainPanelId); });
        if (menuA30 !== null) {
            newMenus.add('-');newMenus.add(menuA30);
        }

        var menuA31 = menus.getMenuItem(showRecords, "生成A.31强制性条文实施计划／细则报审表", function () { me.MenuCreateWinA31(mainPanelId); });
        if (menuA31 !== null) {
            newMenus.add('-');newMenus.add(menuA31);
        }

        //var menuA32 = menus.getMenuItem(showRecords, "生成A.32强条实施计划／细则报审表", function () { me.MenuCreateWinA32(mainPanelId); });
        //if (menuA32 !== null) {
        //    newMenus.add('-');newMenus.add(menuA32);
        //}

        var menuA33 = menus.getMenuItem(showRecords, "生成A.33工程变更费用报审表", function () { me.MenuCreateWinA33(mainPanelId); });
        if (menuA33 !== null) {
            newMenus.add('-');newMenus.add(menuA33);
        }

        var menuA34 = menus.getMenuItem(showRecords, "生成A.34工程变更签证单", function () { me.MenuCreateWinA34(mainPanelId); });
        if (menuA34 !== null) {
            newMenus.add('-');newMenus.add(menuA34);
        }

        var menuA35 = menus.getMenuItem(showRecords, "生成A.35承包单位资质报审表", function () { me.MenuCreateWinA35(mainPanelId); });
        if (menuA35 !== null) {
            newMenus.add('-');newMenus.add(menuA35);
        }

        var menuA37 = menus.getMenuItem(showRecords, "生成A.37工程竣工验收签证书", function () { me.MenuCreateWinA37(mainPanelId); });
        if (menuA37 !== null) {
            newMenus.add('-'); newMenus.add(menuA37);
        }

        var menuA38 = menus.getMenuItem(showRecords, "生成A.38危险源辨识与风险评价报审表", function () { me.MenuCreateWinA38(mainPanelId); });
        if (menuA38 !== null) {
            newMenus.add('-'); newMenus.add(menuA38);
        }

        var menuA41 = menus.getMenuItem(showRecords, "生成A.41年（月）度资金需求计划报审表", function () { me.MenuCreateWinA41(mainPanelId); });
        if (menuA41 !== null) {
            newMenus.add('-'); newMenus.add(menuA41);
        }

        var menuA53 = menus.getMenuItem(showRecords, "生成A.53安全（质量)管理体系报审表", function () { me.MenuCreateWinA53(mainPanelId); });
        if (menuA53 !== null) {
            newMenus.add('-'); newMenus.add(menuA53);
        }

        var menuA54 = menus.getMenuItem(showRecords, "生成A.54安全文明施工二次策划报审表", function () { me.MenuCreateWinA54(mainPanelId); });
        if (menuA54 !== null) {
            newMenus.add('-'); newMenus.add(menuA54);
        }

        var menuA57 = menus.getMenuItem(showRecords, "生成A.57安全问题整改回复单", function () { me.MenuCreateWinA57(mainPanelId); });
        if (menuA57 !== null) {
            newMenus.add('-'); newMenus.add(menuA57);
        }

        var menu20 = menus.getMenuItem(showRecords, "生成B.1监理工作联系单", function () { me.MenuCreateWinB1(mainPanelId); });
        if (menu20 !== null) {
            newMenus.add('-');newMenus.add(menu20);
        }

        var menu21 = menus.getMenuItem(showRecords, "生成B.2监理工程师通知单", function () { me.MenuCreateWinB2(mainPanelId); });
        if (menu21 !== null) {
            newMenus.add('-');newMenus.add(menu21);
        }

        var menu22 = menus.getMenuItem(showRecords, "生成B.3工程暂停令", function () { me.MenuCreateWinB3(mainPanelId); });
        if (menu22 !== null) {
            newMenus.add('-');newMenus.add(menu22);
        }

        var menuC1 = menus.getMenuItem(showRecords, "生成C.1图纸交付计划报审表", function () { me.MenuCreateWinC1(mainPanelId); });
        if (menuC1 !== null) {
            newMenus.add('-');newMenus.add(menuC1);
        }

        var menuC2 = menus.getMenuItem(showRecords, "生成C.2设计文件报检表", function () { me.MenuCreateWinC2(mainPanelId); });
        if (menuC2 !== null) {
            newMenus.add('-');newMenus.add(menuC2);
        }

        var menuC3 = menus.getMenuItem(showRecords, "生成C.3设计变更通知单", function () { me.MenuCreateWinC3(mainPanelId); });
        if (menuC3 !== null) {
            newMenus.add('-');newMenus.add(menuC3);
        }

        var menuC4 = menus.getMenuItem(showRecords, "生成C.4设计交底纪要", function () { me.MenuCreateWinC4(mainPanelId); });
        if (menuC4 !== null) {
            newMenus.add('-'); newMenus.add(menuC4);
        }

        var menuD1 = menus.getMenuItem(showRecords, "生成D.1工程联系单", function () { me.MenuCreateWinD1(mainPanelId); });
        if (menuD1 !== null) {
            newMenus.add('-');newMenus.add(menuD1);
        }

        var menuD2 = menus.getMenuItem(showRecords, "生成D.2工程变更申请单", function () { me.MenuCreateWinD2(mainPanelId); });
        if (menuD2 !== null) {
            newMenus.add('-');newMenus.add(menuD2);
        }

        var menu30 = menus.getMenuItem(showRecords, "生成D.3工程联系单", function () { me.MenuCreateWinD3(mainPanelId); });
        if (menu30 !== null) {
            newMenus.add('-');newMenus.add(menu30);
        }

        var menuD4 = menus.getMenuItem(showRecords, "生成D.4标准、规程、规范清单报审表", function () { me.MenuCreateWinD4(mainPanelId); });
        if (menuD4 !== null) {
            newMenus.add('-'); newMenus.add(menuD4);
        }

        var menuSQU = menus.getMenuItem(showRecords, "生成SQU工程联系单", function () { me.MenuCreateWinSQU(mainPanelId); });
        if (menuSQU !== null) {
            newMenus.add('-');
            newMenus.add(menuSQU);
        }

        var menuMount = menus.getMenuItem(showRecords, "挂接已关闭文件", function () { me.MenuMountClosedFile(mainPanelId); });
        if (menuMount !== null) {
            newMenus.add('-');
            newMenus.add(menuMount);
        }

        var menuUnMount = menus.getMenuItem(showRecords, "取消关闭状态", function () { me.MenuUnCloseFile(mainPanelId); });
        if (menuUnMount !== null) {
            newMenus.add(menuUnMount);
        }

        var menuEditUnitUser = menus.getMenuItem(showRecords, "编辑本单位用户..", function () { me.MenuEditUnitUser(mainPanelId); });
        if (menuEditUnitUser !== null) {
            newMenus.add(menuEditUnitUser);
        }
        
        return newMenus;
    },

    MenuCreateWinA1: function (mainPanelId) {
        var me = this;

        me.viewTree = Ext.getCmp(mainPanelId).down('treepanel');//获取目录树控件ID

        var nodes = me.viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {

            var winCreateA1 = Ext.create('Ext.plug_ins.ZHEPC_Plugins.Report.winCreateA1', { title: "", mainPanelId: mainPanelId, projectKeyword: nodes[0].data.Keyword });
            winA1 = Ext.widget('window', {
                title: '生成A.1工程联系单',
                closeAction: 'hide',
                width: 780,
                height: 580,
                minWidth: 500,
                minHeight: 580,
                layout: 'fit',
                resizable: true,
                modal: true,
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: winCreateA1,
                defaultFocus: 'firstName'
            });

            winA1.show();
            //监听子窗口关闭事件
            winA1.on('close', function () {
            });
            
        }
    },

    MenuCreateWinA2: function (mainPanelId) {
        var me = this;
        me.viewTree = Ext.getCmp(mainPanelId).down('treepanel');//获取目录树控件ID
        var nodes = me.viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {

            var winCreateA2 = Ext.create('Ext.plug_ins.ZHEPC_Plugins.Report.winCreateA2', { title: "", mainPanelId: mainPanelId, projectKeyword: nodes[0].data.Keyword });
            winA2 = Ext.widget('window', {
                title: '生成A.2工程复工申请表',
                closeAction: 'hide',
                width: 780,
                height: 580,
                minWidth: 500,
                minHeight: 580,
                layout: 'fit',
                resizable: true,
                modal: true,
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: winCreateA2,
                defaultFocus: 'firstName'
            });

            winA2.show();
            //监听子窗口关闭事件
            winA2.on('close', function () {
            });
            
        }
    },

    MenuCreateWinA3: function (mainPanelId) {
        var me = this;
        me.viewTree = Ext.getCmp(mainPanelId).down('treepanel');//获取目录树控件ID
        var nodes = me.viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {
            //var projectKeyword = nodes[0].data.Keyword;
            var winCreateA3 = Ext.create('Ext.plug_ins.ZHEPC_Plugins.Report.winCreateA3', { title: "", mainPanelId: mainPanelId, projectKeyword: nodes[0].data.Keyword });
            winA3 = Ext.widget('window', {
                title: '生成A.3施工组织设计报审表',
                closeAction: 'hide',
                width: winCreateA3.winWidth,
                height: winCreateA3.winHeight,
                minWidth: winCreateA3.winWidth,
                minHeight: winCreateA3.winHeight,
                layout: 'fit',
                resizable: true,
                modal: true,
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: winCreateA3,
                defaultFocus: 'firstName'
            });

            winA3.show();
            //监听子窗口关闭事件
            winA3.on('close', function () {
            });
            
        }
    },

    MenuCreateWinA4: function (mainPanelId) {
        var me = this;
        me.viewTree = Ext.getCmp(mainPanelId).down('treepanel');//获取目录树控件ID
        var nodes = me.viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {

            var winCreateA4 = Ext.create('Ext.plug_ins.ZHEPC_Plugins.Report.winCreateA4', { title: "", mainPanelId: mainPanelId, projectKeyword: nodes[0].data.Keyword });
            winA4 = Ext.widget('window', {
                title: '生成A.4方案报审表',
                closeAction: 'hide',
                width: 780,
                height: 580,
                minWidth: 500,
                minHeight: 580,
                layout: 'fit',
                resizable: true,
                modal: true,
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: winCreateA4,
                defaultFocus: 'firstName'
            });

            winA4.show();
            //监听子窗口关闭事件
            winA4.on('close', function () {
            });
            
        }
    },

    MenuCreateWinA5: function (mainPanelId) {
        var me = this;
        me.viewTree = Ext.getCmp(mainPanelId).down('treepanel');//获取目录树控件ID
        var nodes = me.viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {

            var winCreateA5 = Ext.create('Ext.plug_ins.ZHEPC_Plugins.Report.winCreateA5', { title: "", mainPanelId: mainPanelId, projectKeyword: nodes[0].data.Keyword });
            winA5 = Ext.widget('window', {
                title: '生成A.5分包单位资格报审表',
                closeAction: 'hide',
                width: winCreateA5.winWidth,
                height: winCreateA5.winHeight,
                minWidth: winCreateA5.winWidth,
                minHeight: winCreateA5.winHeight,
                layout: 'fit',
                resizable: true,
                modal: true,
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: winCreateA5,
                defaultFocus: 'firstName'
            });

            winA5.show();
            //监听子窗口关闭事件
            winA5.on('close', function () {
            });
            
        }
    },

    MenuCreateWinA6: function (mainPanelId) {
        var me = this;
        me.viewTree = Ext.getCmp(mainPanelId).down('treepanel');//获取目录树控件ID
        var nodes = me.viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {

            var winCreateA6 = Ext.create('Ext.plug_ins.ZHEPC_Plugins.Report.winCreateA6', { title: "", mainPanelId: mainPanelId, projectKeyword: nodes[0].data.Keyword });
            winA6 = Ext.widget('window', {
                title: '生成A.6单位资质报审表',
                closeAction: 'hide',
                width: winCreateA6.winWidth,
                height: winCreateA6.winHeight,
                minWidth: winCreateA6.winWidth,
                minHeight: winCreateA6.winHeight,
                layout: 'fit',
                resizable: true,
                modal: true,
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: winCreateA6,
                defaultFocus: 'firstName'
            });

            winA6.show();
            //监听子窗口关闭事件
            winA6.on('close', function () {
            });
            
        }
    },

    MenuCreateWinA7: function (mainPanelId) {
        var me = this;
        me.viewTree = Ext.getCmp(mainPanelId).down('treepanel');//获取目录树控件ID
        var nodes = me.viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {

            var winCreateA7 = Ext.create('Ext.plug_ins.ZHEPC_Plugins.Report.winCreateA7', { title: "", mainPanelId: mainPanelId, projectKeyword: nodes[0].data.Keyword });
            winA7 = Ext.widget('window', {
                title: '生成A.7人员资质报审表',
                closeAction: 'hide',
                width: winCreateA7.winWidth,
                height: winCreateA7.winHeight,
                minWidth: winCreateA7.winWidth,
                minHeight: winCreateA7.winHeight,
                layout: 'fit',
                resizable: true,
                modal: true,
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: winCreateA7,
                defaultFocus: 'firstName'
            });

            winA7.show();
            //监听子窗口关闭事件
            winA7.on('close', function () {
            });
            
        }
    },

    MenuCreateWinA8: function (mainPanelId) {
        var me = this;
        me.viewTree = Ext.getCmp(mainPanelId).down('treepanel');//获取目录树控件ID
        var nodes = me.viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {

            var winCreateA8 = Ext.create('Ext.plug_ins.ZHEPC_Plugins.Report.winCreateA8', { title: "", mainPanelId: mainPanelId, projectKeyword: nodes[0].data.Keyword });
            winA8 = Ext.widget('window', {
                title: '生成A.8工程控制网测量／线路复测报审表',
                closeAction: 'hide',
                width: 780,
                height: 580,
                minWidth: 500,
                minHeight: 580,
                layout: 'fit',
                resizable: true,
                modal: true,
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: winCreateA8,
                defaultFocus: 'firstName'
            });

            winA8.show();
            //监听子窗口关闭事件
            winA8.on('close', function () {
            });
            
        }
    },

    MenuCreateWinA9: function (mainPanelId) {
        var me = this;
        me.viewTree = Ext.getCmp(mainPanelId).down('treepanel');//获取目录树控件ID
        var nodes = me.viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {

            var winCreateA9 = Ext.create('Ext.plug_ins.ZHEPC_Plugins.Report.winCreateA9', { title: "", mainPanelId: mainPanelId, projectKeyword: nodes[0].data.Keyword });
            winA9 = Ext.widget('window', {
                title: '生成A.9主要施工机械/工器具/安全用具报审表',
                closeAction: 'hide',
                width: 780,
                height: 580,
                minWidth: 500,
                minHeight: 580,
                layout: 'fit',
                resizable: true,
                modal: true,
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: winCreateA9,
                defaultFocus: 'firstName'
            });

            winA9.show();
            //监听子窗口关闭事件
            winA9.on('close', function () {
            });
            
        }
    },

    MenuCreateWinA10: function (mainPanelId) {
        var me = this;
        me.viewTree = Ext.getCmp(mainPanelId).down('treepanel');//获取目录树控件ID
        var nodes = me.viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {

            var winCreateA10 = Ext.create('Ext.plug_ins.ZHEPC_Plugins.Report.winCreateA10', { title: "", mainPanelId: mainPanelId, projectKeyword: nodes[0].data.Keyword });
            winA10 = Ext.widget('window', {
                title: '生成A.10主要测量计量器具/试验设备检验报审表',
                closeAction: 'hide',
                width: 780,
                height: 580,
                minWidth: 500,
                minHeight: 580,
                layout: 'fit',
                resizable: true,
                modal: true,
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: winCreateA10,
                defaultFocus: 'firstName'
            });

            winA10.show();
            //监听子窗口关闭事件
            winA10.on('close', function () {
            });
            
        }
    },

    MenuCreateWinA11: function (mainPanelId) {
        var me = this;
        me.viewTree = Ext.getCmp(mainPanelId).down('treepanel');//获取目录树控件ID
        var nodes = me.viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {

            var winCreateA11 = Ext.create('Ext.plug_ins.ZHEPC_Plugins.Report.winCreateA11', { title: "", mainPanelId: mainPanelId, projectKeyword: nodes[0].data.Keyword });
            winA11 = Ext.widget('window', {
                title: '生成A.11质量验收及评定项目划分报审表',
                closeAction: 'hide',
                width: 780,
                height: 580,
                minWidth: 500,
                minHeight: 580,
                layout: 'fit',
                resizable: true,
                modal: true,
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: winCreateA11,
                defaultFocus: 'firstName'
            });

            winA11.show();
            //监听子窗口关闭事件
            winA11.on('close', function () {
            });
            
        }
    },

    MenuCreateWinA12: function (mainPanelId) {
        var me = this;
        me.viewTree = Ext.getCmp(mainPanelId).down('treepanel');//获取目录树控件ID
        var nodes = me.viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {

            var winCreateA12 = Ext.create('Ext.plug_ins.ZHEPC_Plugins.Report.winCreateA12', { title: "", mainPanelId: mainPanelId, projectKeyword: nodes[0].data.Keyword });
            winA12 = Ext.widget('window', {
                title: '生成A.12工程材料/构配件/设备报审表',
                closeAction: 'hide',
                width: 780,
                height: 580,
                minWidth: 500,
                minHeight: 580,
                layout: 'fit',
                resizable: true,
                modal: true,
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: winCreateA12,
                defaultFocus: 'firstName'
            });

            winA12.show();
            //监听子窗口关闭事件
            winA12.on('close', function () {
            });
            
        }
    },

    MenuCreateWinA13: function (mainPanelId) {
        var me = this;
        me.viewTree = Ext.getCmp(mainPanelId).down('treepanel');//获取目录树控件ID
        var nodes = me.viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {

            var winCreateA13 = Ext.create('Ext.plug_ins.ZHEPC_Plugins.Report.winCreateA13', { title: "", mainPanelId: mainPanelId, projectKeyword: nodes[0].data.Keyword });
            winA13 = Ext.widget('window', {
                title: '生成A.13主要设备开箱申请表',
                closeAction: 'hide',
                width: 780,
                height: 580,
                minWidth: 500,
                minHeight: 580,
                layout: 'fit',
                resizable: true,
                modal: true,
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: winCreateA13,
                defaultFocus: 'firstName'
            });

            winA13.show();
            //监听子窗口关闭事件
            winA13.on('close', function () {
            });
            
        }
    },

    MenuCreateWinA14: function (mainPanelId) {
        var me = this;
        me.viewTree = Ext.getCmp(mainPanelId).down('treepanel');//获取目录树控件ID
        var nodes = me.viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {

            var winCreateA14 = Ext.create('Ext.plug_ins.ZHEPC_Plugins.Report.winCreateA14', { title: "", mainPanelId: mainPanelId, projectKeyword: nodes[0].data.Keyword });
            winA14 = Ext.widget('window', {
                title: '生成A.14验收申请表',
                closeAction: 'hide',
                width: 780,
                height: 580,
                minWidth: 500,
                minHeight: 580,
                layout: 'fit',
                resizable: true,
                modal: true,
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: winCreateA14,
                defaultFocus: 'firstName'
            });

            winA14.show();
            //监听子窗口关闭事件
            winA14.on('close', function () {
            });
            
        }
    },

    MenuCreateWinA15: function (mainPanelId) {
        var me = this;
        me.viewTree = Ext.getCmp(mainPanelId).down('treepanel');//获取目录树控件ID
        var nodes = me.viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {

            var winCreateA15 = Ext.create('Ext.plug_ins.ZHEPC_Plugins.Report.winCreateA15', { title: "", mainPanelId: mainPanelId, projectKeyword: nodes[0].data.Keyword });
            winA15 = Ext.widget('window', {
                title: '生成A.15中间交付验收交接表',
                closeAction: 'hide',
                width: 780,
                height: 580,
                minWidth: 500,
                minHeight: 580,
                layout: 'fit',
                resizable: true,
                modal: true,
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: winCreateA15,
                defaultFocus: 'firstName'
            });

            winA15.show();
            //监听子窗口关闭事件
            winA15.on('close', function () {
            });
            
        }
    },

    MenuCreateWinA16: function (mainPanelId) {
        var me = this;
        me.viewTree = Ext.getCmp(mainPanelId).down('treepanel');//获取目录树控件ID
        var nodes = me.viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {

            var winCreateA16 = Ext.create('Ext.plug_ins.ZHEPC_Plugins.Report.winCreateA16', { title: "", mainPanelId: mainPanelId, projectKeyword: nodes[0].data.Keyword });
            winA16 = Ext.widget('window', {
                title: '生成A.16计划/调整计划报审表',
                closeAction: 'hide',
                width: winCreateA16.winWidth,
                height: winCreateA16.winHeight,
                minWidth: winCreateA16.winWidth,
                minHeight: winCreateA16.winHeight,
                layout: 'fit',
                resizable: true,
                modal: true,
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: winCreateA16,
                defaultFocus: 'firstName'
            });

            winA16.show();
            //监听子窗口关闭事件
            winA16.on('close', function () {
            });
            
        }
    },

    MenuCreateWinA17: function (mainPanelId) {
        var me = this;
        me.viewTree = Ext.getCmp(mainPanelId).down('treepanel');//获取目录树控件ID
        var nodes = me.viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {

            var winCreateA17 = Ext.create('Ext.plug_ins.ZHEPC_Plugins.Report.winCreateA17', { title: "", mainPanelId: mainPanelId, projectKeyword: nodes[0].data.Keyword });
            winA17 = Ext.widget('window', {
                title: '生成A.17费用索赔申请表',
                closeAction: 'hide',
                width: winCreateA17.winWidth,
                height: winCreateA17.winHeight,
                minWidth: winCreateA17.winWidth,
                minHeight: winCreateA17.winHeight,
                layout: 'fit',
                resizable: true,
                modal: true,
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: winCreateA17,
                defaultFocus: 'firstName'
            });

            winA17.show();
            //监听子窗口关闭事件
            winA17.on('close', function () {
            });
            
        }
    },

    MenuCreateWinA18: function (mainPanelId) {
        var me = this;
        me.viewTree = Ext.getCmp(mainPanelId).down('treepanel');//获取目录树控件ID
        var nodes = me.viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {

            var winCreateA18 = Ext.create('Ext.plug_ins.ZHEPC_Plugins.Report.winCreateA18', { title: "", mainPanelId: mainPanelId, projectKeyword: nodes[0].data.Keyword });
            winA18 = Ext.widget('window', {
                title: '生成A.18监理工程师通知回复单',
                closeAction: 'hide',
                width: winCreateA18.winWidth,
                height: winCreateA18.winHeight,
                minWidth: winCreateA18.winWidth,
                minHeight: winCreateA18.winHeight,
                layout: 'fit',
                resizable: true,
                modal: true,
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: winCreateA18,
                defaultFocus: 'firstName'
            });

            winA18.show();
            //监听子窗口关闭事件
            winA18.on('close', function () {
            });
            
        }
    },

    MenuCreateWinA19: function (mainPanelId) {
        var me = this;
        me.viewTree = Ext.getCmp(mainPanelId).down('treepanel');//获取目录树控件ID
        
        var nodes = me.viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {

            var winCreateA19 = Ext.create('Ext.plug_ins.ZHEPC_Plugins.Report.winCreateA19', { title: "", mainPanelId: mainPanelId, projectKeyword: nodes[0].data.Keyword });
            winA19 = Ext.widget('window', {
                title: '生成A.19工程款支付申请表',
                closeAction: 'hide',
                width: winCreateA19.winWidth,
                height: winCreateA19.winHeight,
                minWidth: winCreateA19.winWidth,
                minHeight: winCreateA19.winHeight,
                layout: 'fit',
                resizable: true,
                modal: true,
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: winCreateA19,
                defaultFocus: 'firstName'
            });

            winA19.show();
            //监听子窗口关闭事件
            winA19.on('close', function () {
            });
            
        }
    },

    MenuCreateWinA20: function (mainPanelId) {
        var me = this;
        me.viewTree = Ext.getCmp(mainPanelId).down('treepanel');//获取目录树控件ID
        var nodes = me.viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {

            var winCreateA20 = Ext.create('Ext.plug_ins.ZHEPC_Plugins.Report.winCreateA20', { title: "", mainPanelId: mainPanelId, projectKeyword: nodes[0].data.Keyword });
            winA20 = Ext.widget('window', {
                title: '生成A.20工期变更报审表',
                closeAction: 'hide',
                width: winCreateA20.winWidth,
                height: winCreateA20.winHeight,
                minWidth: winCreateA20.winWidth,
                minHeight: winCreateA20.winHeight,
                layout: 'fit',
                resizable: true,
                modal: true,
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: winCreateA20,
                defaultFocus: 'firstName'
            });

            winA20.show();
            //监听子窗口关闭事件
            winA20.on('close', function () {
            });
            
        }
    },

    MenuCreateWinA21: function (mainPanelId) {
        var me = this;
        me.viewTree = Ext.getCmp(mainPanelId).down('treepanel');//获取目录树控件ID
        var nodes = me.viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {

            var winCreateA21 = Ext.create('Ext.plug_ins.ZHEPC_Plugins.Report.winCreateA21', { title: "", mainPanelId: mainPanelId, projectKeyword: nodes[0].data.Keyword });
            winA21 = Ext.widget('window', {
                title: '生成A.21设备/材料/构配件缺陷通知单',
                closeAction: 'hide',
                width: 780,
                height: 580,
                minWidth: 500,
                minHeight: 580,
                layout: 'fit',
                resizable: true,
                modal: true,
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: winCreateA21,
                defaultFocus: 'firstName'
            });

            winA21.show();
            //监听子窗口关闭事件
            winA21.on('close', function () {
            });
            
        }
    },

  

    MenuCreateWinA22: function (mainPanelId) {
        var me = this;
        me.viewTree = Ext.getCmp(mainPanelId).down('treepanel');//获取目录树控件ID
        var nodes = me.viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {

            var winCreateA22 = Ext.create('Ext.plug_ins.ZHEPC_Plugins.Report.winCreateA22', { title: "", mainPanelId: mainPanelId, projectKeyword: nodes[0].data.Keyword });
            winA22 = Ext.widget('window', {
                title: '生成A.22设备/材料/构配件缺陷处理报验表',
                closeAction: 'hide',
                width: 780,
                height: 580,
                minWidth: 500,
                minHeight: 580,
                layout: 'fit',
                resizable: true,
                modal: true,
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: winCreateA22,
                defaultFocus: 'firstName'
            });

            winA22.show();
            //监听子窗口关闭事件
            winA22.on('close', function () {
            });
            
        }
    },

    MenuCreateWinA23: function (mainPanelId) {
        var me = this;
        me.viewTree = Ext.getCmp(mainPanelId).down('treepanel');//获取目录树控件ID
        var nodes = me.viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {

            var winCreateA23 = Ext.create('Ext.plug_ins.ZHEPC_Plugins.Report.winCreateA23', { title: "", mainPanelId: mainPanelId, projectKeyword: nodes[0].data.Keyword });
            winA23 = Ext.widget('window', {
                title: 'A.23单位工程验收申请表',
                closeAction: 'hide',
                width: 780,
                height: 580,
                minWidth: 500,
                minHeight: 580,
                layout: 'fit',
                resizable: true,
                modal: true,
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: winCreateA23,
                defaultFocus: 'firstName'
            });

            winA23.show();
            //监听子窗口关闭事件
            winA23.on('close', function () {
            });
            
        }
    },

    MenuCreateWinA24: function (mainPanelId) {
        var me = this;
        me.viewTree = Ext.getCmp(mainPanelId).down('treepanel');//获取目录树控件ID
        var nodes = me.viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {

            var winCreateA22 = Ext.create('Ext.plug_ins.ZHEPC_Plugins.Report.winCreateA24', { title: "", mainPanelId: mainPanelId, projectKeyword: nodes[0].data.Keyword });
            winA24 = Ext.widget('window', {
                title: '生成A.24工程竣工报验单',
                closeAction: 'hide',
                width: 780,
                height: 580,
                minWidth: 500,
                minHeight: 580,
                layout: 'fit',
                resizable: true,
                modal: true,
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: winCreateA22,
                defaultFocus: 'firstName'
            });

            winA24.show();
            //监听子窗口关闭事件
            winA24.on('close', function () {
            });
            
        }
    },

    MenuCreateWinA25: function (mainPanelId) {
        var me = this;
        me.viewTree = Ext.getCmp(mainPanelId).down('treepanel');//获取目录树控件ID
        var nodes = me.viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {

            var winCreateA25 = Ext.create('Ext.plug_ins.ZHEPC_Plugins.Report.winCreateA25', { title: "", mainPanelId: mainPanelId, projectKeyword: nodes[0].data.Keyword });
            winA25 = Ext.widget('window', {
                title: '生成A.25设计变更/变更设计执行情况反馈单',
                closeAction: 'hide',
                width: 780,
                height: 580,
                minWidth: 500,
                minHeight: 580,
                layout: 'fit',
                resizable: true,
                modal: true,
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: winCreateA25,
                defaultFocus: 'firstName'
            });

            winA25.show();
            //监听子窗口关闭事件
            winA25.on('close', function () {
            });
            
        }
    },

    MenuCreateWinA26: function (mainPanelId) {
        var me = this;
        me.viewTree = Ext.getCmp(mainPanelId).down('treepanel');//获取目录树控件ID
        var nodes = me.viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {

            var winCreateA26 = Ext.create('Ext.plug_ins.ZHEPC_Plugins.Report.winCreateA26', { title: "", mainPanelId: mainPanelId, projectKeyword: nodes[0].data.Keyword });
            winA26 = Ext.widget('window', {
                title: '生成A.26大中型施工机械进场、出场申报表',
                closeAction: 'hide',
                width: 780,
                height: 580,
                minWidth: 500,
                minHeight: 580,
                layout: 'fit',
                resizable: true,
                modal: true,
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: winCreateA26,
                defaultFocus: 'firstName'
            });

            winA26.show();
            //监听子窗口关闭事件
            winA26.on('close', function () {
            });
            
        }
    },

    MenuCreateWinA27: function (mainPanelId) {
        var me = this;
        me.viewTree = Ext.getCmp(mainPanelId).down('treepanel');//获取目录树控件ID
        var nodes = me.viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {

            var winCreateA27 = Ext.create('Ext.plug_ins.ZHEPC_Plugins.Report.winCreateA27', { title: "", mainPanelId: mainPanelId, projectKeyword: nodes[0].data.Keyword });
            winA27 = Ext.widget('window', {
                title: '生成A.27作业指导书报审表',
                closeAction: 'hide',
                width: 780,
                height: 580,
                minWidth: 500,
                minHeight: 580,
                layout: 'fit',
                resizable: true,
                modal: true,
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: winCreateA27,
                defaultFocus: 'firstName'
            });

            winA27.show();
            //监听子窗口关闭事件
            winA27.on('close', function () {
            });
            
        }
    },

    MenuCreateWinA28: function (mainPanelId) {
        var me = this;
        me.viewTree = Ext.getCmp(mainPanelId).down('treepanel');//获取目录树控件ID
        var nodes = me.viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {

            var winCreateA28 = Ext.create('Ext.plug_ins.ZHEPC_Plugins.Report.winCreateA28', { title: "", mainPanelId: mainPanelId, projectKeyword: nodes[0].data.Keyword });
            winA28 = Ext.widget('window', {
                title: '生成A.28工程沉降观测报审表',
                closeAction: 'hide',
                width: 780,
                height: 580,
                minWidth: 500,
                minHeight: 580,
                layout: 'fit',
                resizable: true,
                modal: true,
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: winCreateA28,
                defaultFocus: 'firstName'
            });

            winA28.show();
            //监听子窗口关闭事件
            winA28.on('close', function () {
            });
            
        }
    },

    MenuCreateWinA29: function (mainPanelId) {
        var me = this;
        me.viewTree = Ext.getCmp(mainPanelId).down('treepanel');//获取目录树控件ID
        var nodes = me.viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {

            var winCreateA29 = Ext.create('Ext.plug_ins.ZHEPC_Plugins.Report.winCreateA29', { title: "", mainPanelId: mainPanelId, projectKeyword: nodes[0].data.Keyword });
            winA29 = Ext.widget('window', {
                title: '生成A.29工程量签证单',
                closeAction: 'hide',
                width: winCreateA29.winWidth,
                height: winCreateA29.winHeight,
                minWidth: winCreateA29.winWidth,
                minHeight: winCreateA29.winHeight,
                layout: 'fit',
                resizable: true,
                modal: true,
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: winCreateA29,
                defaultFocus: 'firstName'
            });

            winA29.show();
            //监听子窗口关闭事件
            winA29.on('close', function () {
            });
            
        }
    },

    MenuCreateWinA30: function (mainPanelId) {
        var me = this;
        me.viewTree = Ext.getCmp(mainPanelId).down('treepanel');//获取目录树控件ID
        var nodes = me.viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {

            var winCreateA30 = Ext.create('Ext.plug_ins.ZHEPC_Plugins.Report.winCreateA30', { title: "", mainPanelId: mainPanelId, projectKeyword: nodes[0].data.Keyword });
            winA30 = Ext.widget('window', {
                title: '生成A.30总承包单位资质报审表',
                closeAction: 'hide',
                width: winCreateA30.winWidth,
                height: winCreateA30.winHeight,
                minWidth: winCreateA30.winWidth,
                minHeight: winCreateA30.winHeight,
                layout: 'fit',
                resizable: true,
                modal: true,
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: winCreateA30,
                defaultFocus: 'firstName'
            });

            winA30.show();
            //监听子窗口关闭事件
            winA30.on('close', function () {
            });
            
        }
    },

    MenuCreateWinA31: function (mainPanelId) {
        var me = this;
        me.viewTree = Ext.getCmp(mainPanelId).down('treepanel');//获取目录树控件ID
        var nodes = me.viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {

            var winCreateA31 = Ext.create('Ext.plug_ins.ZHEPC_Plugins.Report.winCreateA31', { title: "", mainPanelId: mainPanelId, projectKeyword: nodes[0].data.Keyword });
            winA31 = Ext.widget('window', {
                title: '生成A.31强制性条文实施计划／细则报审表',
                closeAction: 'hide',
                width: winCreateA31.winWidth,
                height: winCreateA31.winHeight,
                minWidth: winCreateA31.winWidth,
                minHeight: winCreateA31.winHeight,
                layout: 'fit',
                resizable: true,
                modal: true,
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: winCreateA31,
                defaultFocus: 'firstName'
            });

            winA31.show();
            //监听子窗口关闭事件
            winA31.on('close', function () {
            });
            
        }
    },

    MenuCreateWinA32: function (mainPanelId) {
        var me = this;
        me.viewTree = Ext.getCmp(mainPanelId).down('treepanel');//获取目录树控件ID
        var nodes = me.viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {

            var winCreateA32 = Ext.create('Ext.plug_ins.ZHEPC_Plugins.Report.winCreateA32', { title: "", mainPanelId: mainPanelId, projectKeyword: nodes[0].data.Keyword });
            winA32 = Ext.widget('window', {
                title: '生成A.32强条实施计划/细则报审表',
                closeAction: 'hide',
                width: 780,
                height: 580,
                minWidth: 500,
                minHeight: 580,
                layout: 'fit',
                resizable: true,
                modal: true,
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: winCreateA32,
                defaultFocus: 'firstName'
            });

            winA32.show();
            //监听子窗口关闭事件
            winA32.on('close', function () {
            });
            
        }
    },

    MenuCreateWinA33: function (mainPanelId) {
        var me = this;
        me.viewTree = Ext.getCmp(mainPanelId).down('treepanel');//获取目录树控件ID
        var nodes = me.viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {

            var winCreateA33 = Ext.create('Ext.plug_ins.ZHEPC_Plugins.Report.winCreateA33', { title: "", mainPanelId: mainPanelId, projectKeyword: nodes[0].data.Keyword });
            winA33 = Ext.widget('window', {
                title: '生成A.33工程变更费用报审表',
                closeAction: 'hide',
                width: winCreateA33.winWidth,
                height: winCreateA33.winHeight,
                minWidth: winCreateA33.winWidth,
                minHeight: winCreateA33.winHeight,
                layout: 'fit',
                resizable: true,
                modal: true,
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: winCreateA33,
                defaultFocus: 'firstName'
            });

            winA33.show();
            //监听子窗口关闭事件
            winA33.on('close', function () {
            });
            
        }
    },

    MenuCreateWinA34: function (mainPanelId) {
        var me = this;
        me.viewTree = Ext.getCmp(mainPanelId).down('treepanel');//获取目录树控件ID
        var nodes = me.viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {

            var winCreateA34 = Ext.create('Ext.plug_ins.ZHEPC_Plugins.Report.winCreateA34', { title: "", mainPanelId: mainPanelId, projectKeyword: nodes[0].data.Keyword });
            winA34 = Ext.widget('window', {
                title: '生成A.34工程变更签证单',
                closeAction: 'hide',
                width: winCreateA34.winWidth,
                height: winCreateA34.winHeight,
                minWidth: winCreateA34.winWidth,
                minHeight: winCreateA34.winHeight,
                layout: 'fit',
                resizable: true,
                modal: true,
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: winCreateA34,
                defaultFocus: 'firstName'
            });

            winA34.show();
            //监听子窗口关闭事件
            winA34.on('close', function () {
            });
            
        }
    },

    MenuCreateWinA35: function (mainPanelId) {
        var me = this;
        me.viewTree = Ext.getCmp(mainPanelId).down('treepanel');//获取目录树控件ID
        var nodes = me.viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {

            var winCreateA35 = Ext.create('Ext.plug_ins.ZHEPC_Plugins.Report.winCreateA35', { title: "", mainPanelId: mainPanelId, projectKeyword: nodes[0].data.Keyword });
            winA35 = Ext.widget('window', {
                title: '生成A.35承包单位资质报审表',
                closeAction: 'hide',
                width: 780,
                height: 580,
                minWidth: 500,
                minHeight: 580,
                layout: 'fit',
                resizable: true,
                modal: true,
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: winCreateA35,
                defaultFocus: 'firstName'
            });

            winA35.show();
            //监听子窗口关闭事件
            winA35.on('close', function () {
            });
            
        }
    },

    MenuCreateWinA37: function (mainPanelId) {
        var me = this;
        me.viewTree = Ext.getCmp(mainPanelId).down('treepanel');//获取目录树控件ID
        var nodes = me.viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {

            var winCreateA37 = Ext.create('Ext.plug_ins.ZHEPC_Plugins.Report.winCreateA37', { title: "", mainPanelId: mainPanelId, projectKeyword: nodes[0].data.Keyword });
            winA37 = Ext.widget('window', {
                title: '生成A.37工程竣工验收签证书',
                closeAction: 'hide',
                width: winCreateA37.winWidth,
                height: winCreateA37.winHeight,
                minWidth: winCreateA37.winWidth,
                minHeight: winCreateA37.winHeight,
                layout: 'fit',
                resizable: true,
                modal: true,
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: winCreateA37,
                defaultFocus: 'firstName'
            });

            winA37.show();
            //监听子窗口关闭事件
            winA37.on('close', function () {
            });
            
        }
    },

    MenuCreateWinA38: function (mainPanelId) {
        var me = this;
        me.viewTree = Ext.getCmp(mainPanelId).down('treepanel');//获取目录树控件ID
        var nodes = me.viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {

            var winCreateA38 = Ext.create('Ext.plug_ins.ZHEPC_Plugins.Report.winCreateA38', { title: "", mainPanelId: mainPanelId, projectKeyword: nodes[0].data.Keyword });
            winA38 = Ext.widget('window', {
                title: '生成A.38危险源辨识与风险评价报审表',
                closeAction: 'hide',
                width: winCreateA38.winWidth,
                height: winCreateA38.winHeight,
                minWidth: winCreateA38.winWidth,
                minHeight: winCreateA38.winHeight,
                layout: 'fit',
                resizable: true,
                modal: true,
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: winCreateA38,
                defaultFocus: 'firstName'
            });

            winA38.show();
            //监听子窗口关闭事件
            winA38.on('close', function () {
            });
            
        }
    },

    MenuCreateWinA41: function (mainPanelId) {
        var me = this;
        me.viewTree = Ext.getCmp(mainPanelId).down('treepanel');//获取目录树控件ID
        var nodes = me.viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {

            var winCreateA41 = Ext.create('Ext.plug_ins.ZHEPC_Plugins.Report.winCreateA41', { title: "", mainPanelId: mainPanelId, projectKeyword: nodes[0].data.Keyword });
            winA41 = Ext.widget('window', {
                title: '生成A.41 年（月）度资金需求计划报审表',
                closeAction: 'hide',
                width: winCreateA41.winWidth,
                height: winCreateA41.winHeight,
                minWidth: winCreateA41.winWidth,
                minHeight: winCreateA41.winHeight,
                layout: 'fit',
                resizable: true,
                modal: true,
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: winCreateA41,
                defaultFocus: 'firstName'
            });

            winA41.show();
            //监听子窗口关闭事件
            winA41.on('close', function () {
            });
            
        }
    },

    MenuCreateWinA53: function (mainPanelId) {
        var me = this;
        me.viewTree = Ext.getCmp(mainPanelId).down('treepanel');//获取目录树控件ID
        var nodes = me.viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {

            var winCreateA53 = Ext.create('Ext.plug_ins.ZHEPC_Plugins.Report.winCreateA53', { title: "", mainPanelId: mainPanelId, projectKeyword: nodes[0].data.Keyword });
            winA53 = Ext.widget('window', {
                title: '生成A.53 安全（质量)管理体系报审表',
                closeAction: 'hide',
                width: winCreateA53.winWidth,
                height: winCreateA53.winHeight,
                minWidth: winCreateA53.winWidth,
                minHeight: winCreateA53.winHeight,
                layout: 'fit',
                resizable: true,
                modal: true,
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: winCreateA53,
                defaultFocus: 'firstName'
            });

            winA53.show();
            //监听子窗口关闭事件
            winA53.on('close', function () {
            });
            
        }
    },

    MenuCreateWinA54: function (mainPanelId) {
        var me = this;
        me.viewTree = Ext.getCmp(mainPanelId).down('treepanel');//获取目录树控件ID
        var nodes = me.viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {

            var winCreateA54 = Ext.create('Ext.plug_ins.ZHEPC_Plugins.Report.winCreateA54', { title: "", mainPanelId: mainPanelId, projectKeyword: nodes[0].data.Keyword });
            winA54 = Ext.widget('window', {
                title: '生成A.54 安全文明施工二次策划报审表',
                closeAction: 'hide',
                width: winCreateA54.winWidth,
                height: winCreateA54.winHeight,
                minWidth: winCreateA54.winWidth,
                minHeight: winCreateA54.winHeight,
                layout: 'fit',
                resizable: true,
                modal: true,
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: winCreateA54,
                defaultFocus: 'firstName'
            });

            winA54.show();
            //监听子窗口关闭事件
            winA54.on('close', function () {
            });
            
        }
    },

    MenuCreateWinA57: function (mainPanelId) {
        var me = this;
        me.viewTree = Ext.getCmp(mainPanelId).down('treepanel');//获取目录树控件ID
        var nodes = me.viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {

            var winCreateA57 = Ext.create('Ext.plug_ins.ZHEPC_Plugins.Report.winCreateA57', { title: "", mainPanelId: mainPanelId, projectKeyword: nodes[0].data.Keyword });
            winA57 = Ext.widget('window', {
                title: '生成A.57安全问题整改回复单',
                closeAction: 'hide',
                width: winCreateA57.winWidth,
                height: winCreateA57.winHeight,
                minWidth: winCreateA57.winWidth,
                minHeight: winCreateA57.winHeight,
                layout: 'fit',
                resizable: true,
                modal: true,
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: winCreateA57,
                defaultFocus: 'firstName'
            });

            winA57.show();
            //监听子窗口关闭事件
            winA57.on('close', function () {
            });
            
        }
    },

    MenuCreateWinB1: function (mainPanelId) {
        var me = this;
        me.viewTree = Ext.getCmp(mainPanelId).down('treepanel');//获取目录树控件ID
        var nodes = me.viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {

            var winCreateB1 = Ext.create('Ext.plug_ins.ZHEPC_Plugins.Report.winCreateB1', { title: "", mainPanelId: mainPanelId, projectKeyword: nodes[0].data.Keyword });
            winB1 = Ext.widget('window', {
                title: '生成B.1监理工作联系单',
                closeAction: 'hide',
                width: 780,
                height: 580,
                minWidth: 500,
                minHeight: 580,
                layout: 'fit',
                resizable: true,
                modal: true,
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: winCreateB1,
                defaultFocus: 'firstName'
            });

            winB1.show();
            //监听子窗口关闭事件
            winB1.on('close', function () {
            });
            
        }
    },

    MenuCreateWinB2: function (mainPanelId) {
        var me = this;
        me.viewTree = Ext.getCmp(mainPanelId).down('treepanel');//获取目录树控件ID
        var nodes = me.viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {

            var winCreateB2 = Ext.create('Ext.plug_ins.ZHEPC_Plugins.Report.winCreateB2', { title: "", mainPanelId: mainPanelId, projectKeyword: nodes[0].data.Keyword });
            winB2 = Ext.widget('window', {
                title: '生成B.2监理工程师通知单',
                closeAction: 'hide',
                width: 780,
                height: 580,
                minWidth: 500,
                minHeight: 580,
                layout: 'fit',
                resizable: true,
                modal: true,
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: winCreateB2,
                defaultFocus: 'firstName'
            });

            winB2.show();
            //监听子窗口关闭事件
            winB2.on('close', function () {
            });
            
        }
    },

    MenuCreateWinB3: function (mainPanelId) {
        var me = this;
        me.viewTree = Ext.getCmp(mainPanelId).down('treepanel');//获取目录树控件ID
        var nodes = me.viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {

            var winCreateB3 = Ext.create('Ext.plug_ins.ZHEPC_Plugins.Report.winCreateB3', { title: "", mainPanelId: mainPanelId, projectKeyword: nodes[0].data.Keyword });
            winB3 = Ext.widget('window', {
                title: '生成生成B.3工程暂停令',
                closeAction: 'hide',
                width: 780,
                height: 580,
                minWidth: 500,
                minHeight: 580,
                layout: 'fit',
                resizable: true,
                modal: true,
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: winCreateB3,
                defaultFocus: 'firstName'
            });

            winB3.show();
            //监听子窗口关闭事件
            winB3.on('close', function () {
            });
            
        }
    },



    MenuCreateWinC1: function (mainPanelId) {
        var me = this;
        me.viewTree = Ext.getCmp(mainPanelId).down('treepanel');//获取目录树控件ID
        var nodes = me.viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {

            var winCreateC1 = Ext.create('Ext.plug_ins.ZHEPC_Plugins.Report.winCreateC1', { title: "", mainPanelId: mainPanelId, projectKeyword: nodes[0].data.Keyword });
            winC1 = Ext.widget('window', {
                title: '生成C.1图纸交付计划报审表',
                closeAction: 'hide',
                width: winCreateC1.winWidth,
                height: winCreateC1.winHeight,
                minWidth: winCreateC1.winWidth,
                minHeight: winCreateC1.winHeight,
                layout: 'fit',
                resizable: true,
                modal: true,
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: winCreateC1,
                defaultFocus: 'firstName'
            });

            winC1.show();
            //监听子窗口关闭事件
            winC1.on('close', function () {
            });
            
        }
    },

    MenuCreateWinC2: function (mainPanelId) {
        var me = this;
        me.viewTree = Ext.getCmp(mainPanelId).down('treepanel');//获取目录树控件ID
        var nodes = me.viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {

            var winCreateC2 = Ext.create('Ext.plug_ins.ZHEPC_Plugins.Report.winCreateC2', { title: "", mainPanelId: mainPanelId, projectKeyword: nodes[0].data.Keyword });
            winC2 = Ext.widget('window', {
                title: '生成C.2设计文件报检表',
                closeAction: 'hide',
                width: winCreateC2.winWidth,
                height: winCreateC2.winHeight,
                minWidth: winCreateC2.winWidth,
                minHeight: winCreateC2.winHeight,
                layout: 'fit',
                resizable: true,
                modal: true,
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: winCreateC2,
                defaultFocus: 'firstName'
            });

            winC2.show();
            //监听子窗口关闭事件
            winC2.on('close', function () {
            });
            
        }
    },

    MenuCreateWinC3: function (mainPanelId) {
        var me = this;
        me.viewTree = Ext.getCmp(mainPanelId).down('treepanel');//获取目录树控件ID
        var nodes = me.viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {

            var winCreateC3 = Ext.create('Ext.plug_ins.ZHEPC_Plugins.Report.winCreateC3', { title: "", mainPanelId: mainPanelId, projectKeyword: nodes[0].data.Keyword });
            winC3 = Ext.widget('window', {
                title: '生成C.3设计变更通知单',
                closeAction: 'hide',
                width: winCreateC3.winWidth,
                height: winCreateC3.winHeight,
                minWidth: winCreateC3.winWidth,
                minHeight: winCreateC3.winHeight,
                layout: 'fit',
                resizable: true,
                modal: true,
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: winCreateC3,
                defaultFocus: 'firstName'
            });

            winC3.show();
            //监听子窗口关闭事件
            winC3.on('close', function () {
            });
            
        }
    },

    MenuCreateWinC4: function (mainPanelId) {
        var me = this;
        me.viewTree = Ext.getCmp(mainPanelId).down('treepanel');//获取目录树控件ID
        var nodes = me.viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {

            var winCreateC4 = Ext.create('Ext.plug_ins.ZHEPC_Plugins.Report.winCreateC4', { title: "", mainPanelId: mainPanelId, projectKeyword: nodes[0].data.Keyword });
            winC4 = Ext.widget('window', {
                title: '生成C.4设计交底纪要',
                closeAction: 'hide',
                width: winCreateC4.winWidth,
                height: winCreateC4.winHeight,
                minWidth: winCreateC4.winWidth,
                minHeight: winCreateC4.winHeight,
                layout: 'fit',
                resizable: true,
                modal: true,
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: winCreateC4,
                defaultFocus: 'firstName'
            });

            winC4.show();
            //监听子窗口关闭事件
            winC4.on('close', function () {
            });
            
        }
    },

    MenuCreateWinD1: function (mainPanelId) {
        var me = this;
        me.viewTree = Ext.getCmp(mainPanelId).down('treepanel');//获取目录树控件ID
        var nodes = me.viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {

            var winCreateD1 = Ext.create('Ext.plug_ins.ZHEPC_Plugins.Report.winCreateD1', { title: "", mainPanelId: mainPanelId, projectKeyword: nodes[0].data.Keyword });
            winD1 = Ext.widget('window', {
                title: '生成D.1工程联系单',
                closeAction: 'hide',
                width: 780,
                height: 580,
                minWidth: 500,
                minHeight: 580,
                layout: 'fit',
                resizable: true,
                modal: true,
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: winCreateD1,
                defaultFocus: 'firstName'
            });

            winD1.show();
            //监听子窗口关闭事件
            winD1.on('close', function () {

            });

            

        }
    },

    MenuCreateWinD2: function (mainPanelId) {
        var me = this;
        me.viewTree = Ext.getCmp(mainPanelId).down('treepanel');//获取目录树控件ID
        var nodes = me.viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {

            var winCreateD2 = Ext.create('Ext.plug_ins.ZHEPC_Plugins.Report.winCreateD2', { title: "", mainPanelId: mainPanelId, projectKeyword: nodes[0].data.Keyword });
            winD2 = Ext.widget('window', {
                title: '生成D.2工程变更申请单',
                closeAction: 'hide',
                width: 780,
                height: 580,
                minWidth: 500,
                minHeight: 580,
                layout: 'fit',
                resizable: true,
                modal: true,
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: winCreateD2,
                defaultFocus: 'firstName'
            });

            winD2.show();
            //监听子窗口关闭事件
            winD2.on('close', function () {

            });

            

        }
    },

    MenuCreateWinD3: function (mainPanelId) {
        var me = this;
        me.viewTree = Ext.getCmp(mainPanelId).down('treepanel');//获取目录树控件ID
        var nodes = me.viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {

            var winCreateD3 = Ext.create('Ext.plug_ins.ZHEPC_Plugins.Report.winCreateD3', { title: "", mainPanelId: mainPanelId, projectKeyword: nodes[0].data.Keyword });
            winD3 = Ext.widget('window', {
                title: '生成D.3工程联系单',
                closeAction: 'hide',
                width: winCreateD3.winWidth,
                height: winCreateD3.winHeight,
                minWidth: winCreateD3.winWidth,
                minHeight: winCreateD3.winHeight,
                layout: 'fit',
                resizable: true,
                modal: true,
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: winCreateD3,
                defaultFocus: 'firstName'
            });

            winD3.show();
            //监听子窗口关闭事件
            winD3.on('close', function () {

            });

            

        }
    },

    MenuCreateWinD4: function (mainPanelId) {
        var me = this;
        me.viewTree = Ext.getCmp(mainPanelId).down('treepanel');//获取目录树控件ID
        var nodes = me.viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {

            var winCreateD4 = Ext.create('Ext.plug_ins.ZHEPC_Plugins.Report.winCreateD4', { title: "", mainPanelId: mainPanelId, projectKeyword: nodes[0].data.Keyword });
            winD4 = Ext.widget('window', {
                title: '生成D.4标准、规程、规范清单报审表',
                closeAction: 'hide',
                width: winCreateD4.winWidth,
                height: winCreateD4.winHeight,
                minWidth: winCreateD4.winWidth,
                minHeight: winCreateD4.winHeight,
                layout: 'fit',
                resizable: true,
                modal: true,
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: winCreateD4,
                defaultFocus: 'firstName'
            });

            winD4.show();
            //监听子窗口关闭事件
            winD4.on('close', function () {

            });

            

        }
    },

    MenuCreateWinSQU: function (mainPanelId) {
        var me = this;
        me.viewTree = Ext.getCmp(mainPanelId).down('treepanel');//获取目录树控件ID
        var nodes = me.viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {

            var winCreateSQU = Ext.create('Ext.plug_ins.ZHEPC_Plugins.Report.winCreateSQU', { title: "", mainPanelId: mainPanelId, projectKeyword: nodes[0].data.Keyword });
            winSQU = Ext.widget('window', {
                title: '生成SQU工程联系单',
                closeAction: 'hide',
                width: 780,
                height: 580,
                minWidth: 500,
                minHeight: 580,
                layout: 'fit',
                resizable: true,
                modal: true,
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: winCreateSQU,
                defaultFocus: 'firstName'
            });

            winSQU.show();
            //监听子窗口关闭事件
            winSQU.on('close', function () {

            });

            

        }
    },

    MenuMountClosedFile: function (mainPanelId) {
        var me = this;

        me.viewGrid = Ext.getCmp(mainPanelId).down('gridpanel');//获取目录树控件ID
        var nodes = me.viewGrid.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {

            var winMountFile = Ext.create('Ext.plug_ins.ZHEPC_Plugins.MountFile.MountCloseFile', {
                title: "", mainPanelId: mainPanelId, docKeyword: nodes[0].data.Keyword
            });
            winMount = Ext.widget('window', {
                title: '挂接已关闭文件',
                closeAction: 'hide',
                width: winMountFile.winWidth,
                height: winMountFile.winHeight,
                minWidth: winMountFile.winWidth,
                minHeight: winMountFile.winHeight,
                layout: 'fit',
                resizable: true,
                modal: true,
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: winMountFile,
                defaultFocus: 'firstName'
            });

            winMount.show();
            //监听子窗口关闭事件
            winMount.on('close', function () {

            });

            

        }
    },

    MenuUnCloseFile: function (mainPanelId) {
        var me = this;

        me.viewGrid = Ext.getCmp(mainPanelId).down('gridpanel');//获取目录树控件ID
        var nodes = me.viewGrid.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {
            var docKeyword = nodes[0].data.Keyword;

            //添加附件到流程
            Ext.Ajax.request({

                url: 'WebApi/Post',
                method: "POST",
                params: {
                    C: "AVEVA.CDMS.ZHEPC_Plugins.Document", A: "UnCloseFile",
                    sid: localStorage.getItem("sid"), DocKeyword: docKeyword
                },
                success: function (response, options) {

                    //获取数据后，更新窗口
                    var res = Ext.JSON.decode(response.responseText, true);
                    var state = res.success;
                    if (state === true) {

                        Ext.MessageBox.close();//关闭等待对话框

                        var recod = eval(res.data[0]);

                        Ext.Msg.alert("信息", "成功取消文件关闭状态");

                        //me.refreshWin("", true);
                        //me.docKeyword = recod.DocKeyword;//获取联系单文档id

                    } else {
                        var errmsg = res.msg;
                        Ext.Msg.alert("错误信息", errmsg);
                    }
                },
                failure: function (response, options) {
                    //////Ext.Msg.alert("错误", "连接服务器失败！<br>" + response.responseText);
                }
            });
        }
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
            var docDesc = nodes[0].data.Title;

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


    //文控编辑本单位用户
    MenuEditUnitUser: function (mainPanelId) {
        var me = this;

        me.viewTree = Ext.getCmp(mainPanelId).down('treepanel');//获取目录树控件ID
        var nodes = me.viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {
            var projKeyword = nodes[0].data.Keyword;

                 //添加附件到流程
            Ext.Ajax.request({

                url: 'WebApi/Post',
                method: "POST",
                params: {
                    C: "AVEVA.CDMS.ZHEPC_Plugins.Company", A: "GetGroupByUserSid",
                    sid: localStorage.getItem("sid"), ProjectKeyword: projKeyword
                },
                success: function (response, options) {

                    //获取数据后，更新窗口
                    var res = Ext.JSON.decode(response.responseText, true);
                    var state = res.success;
                    if (state === true) {

                        //Ext.MessageBox.close();//关闭等待对话框

                        var recod = eval(res.data[0]);

                        var groupName = recod.GroupName;
                        //弹出操作窗口
                        var _fmUserAdmin = Ext.create('Ext.ux.YDForm.User._UserManagement', { title: "", mainPanelId: Ext.getCmp(mainPanelId).mainprojecttree.id });

                        winUserAdmin = Ext.widget('window', {
                            title: '编辑本单位用户',
                            //closeAction: 'hide',
                            width: 800,
                            height: 596,
                            minWidth: 300,
                            minHeight: 300,
                            layout: 'fit',
                            resizable: true,
                            modal: true,
                            closeAction: 'close', //close 关闭  hide  隐藏  
                            items: _fmUserAdmin,
                            defaultFocus: 'firstName'
                        });


                        _fmUserAdmin._groupselstore.proxy.extraParams.Group = groupName;//"ZCNF00263_GEDI_ALLGroup";//重置机构组
                        _fmUserAdmin._groupselstore.currentPage = 1;
                        _fmUserAdmin._groupselstore.autoLoad = false;
                        _fmUserAdmin._groupselstore.load({
                            callback: function (records, options, success) {//添加回调，获取子目录的文件数量
                                _fmUserAdmin.delUserButton.hide();
                                _fmUserAdmin.usertypeCombo.setReadOnly(true);

                                _fmUserAdmin.send_save_user_info = function (isCreate) {
                                    var me = _fmUserAdmin;

                                    me.IsCreate = isCreate;
                                    var userKeyword = "";

                                    if (!isCreate) {
                                        var grid = me.groupgrid;
                                        var rs = grid.getSelectionModel().getSelection();//获取选择的文档

                                        if (!(rs !== null && rs.length > 0)) {
                                            return;
                                        }

                                        userKeyword = rs[0].data.id;//获取文档关键字
                                    }

                                    var userCode = me.userCodeText.value;
                                    var userDesc = me.userDescText.value;
                                    var email = me.emailText.value;
                                    var tel = me.telText.value;
                                    var userPwd = me.pwdText.value;
                                    var userRePwd = me.rePwdText.value;

                                    var uType = me.usertypeCombo.value;
                                    var uState = me.userstateCombo.value;

                                    var userType = "";
                                    var userState = "";

                                    for (var i = 0; i < me.usertypedata.length; i++) {
                                        if (me.usertypedata[i].value === uType) {
                                            userType = i.toString();
                                        }
                                    }

                                    for (var i = 0; i < me.userstatedata.length; i++) {
                                        if (me.userstatedata[i].value === uState) {
                                            userState = i.toString();
                                        }
                                    }

                                    var A = isCreate ? "CreateUser" : "SaveUserInfo";


                                    ////获取用户信息
                                    Ext.Ajax.request({
                                        url: 'WebApi/Post',
                                        method: "POST",
                                        params: {
                                            C: "AVEVA.CDMS.ZHEPC_Plugins.Company", A: A,
                                            sid: localStorage.getItem("sid"), ProjectKeyword: projKeyword,
                                            GroupName: groupName, UserKeyword: userKeyword,
                                            UserCode: userCode, UserDesc: userDesc,
                                            UserEmail: email, Phone: tel,
                                            UserPwd: userPwd, UserConfirmPwd: userRePwd,
                                            UserType: userType, UserStatus: userState
                                        },
                                        success: function (response, options) {


                                            //当没有附件时，处理返回事件
                                            me.send_save_user_info_callback(response, options);



                                        }
                                    });


                                }

                                winUserAdmin.show();
                                //监听子窗口关闭事件
                                winUserAdmin.on('close', function () {

                                });
                            }
                        });
                        

                    } else {
                        var errmsg = res.msg;
                        Ext.Msg.alert("错误信息", errmsg);
                    }
                },
                failure: function (response, options) {
                    //////Ext.Msg.alert("错误", "连接服务器失败！<br>" + response.responseText);
                }
            });
        }

    }


});
Ext.onReady;




