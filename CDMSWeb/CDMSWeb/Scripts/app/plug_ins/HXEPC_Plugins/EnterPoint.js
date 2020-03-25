Ext.define('Ext.plug_ins.HXEPC_Plugins.EnterPoint', {
    extend: 'Ext.menu.Item',
    addMenus_HXEPC: function (menus, mainPanelId, showRecords) {//参数：menus:菜单，mainPanelId:父控件ID,showRec:需要显示的菜单项
        var me = this;
        var hxepcMenus = menus;
			
        //定义菜单项
        // 这里的”HelloWorld”是菜单名，MenuNameA1是对应的函数名, mainPanelId用于传递发起控件的ID
        var menuCreatePrjDocument = menus.getMenuItem(showRecords, "生成立项单...", function () { me.MenuCreatePrjDocument(mainPanelId); });
        if (menuCreatePrjDocument !== null) {
            hxepcMenus.add('-');
            hxepcMenus.add(menuCreatePrjDocument);
        }

        //var menuDraftDocument = menus.getMenuItem(showRecords, "起草红头文...", function () { me.MenuDraftDocument(mainPanelId); });
        //if (menuDraftDocument !== null) {
        //    hxepcMenus.add('-');
        //    hxepcMenus.add(menuDraftDocument);
        //}

        var menuDraftMeetMinutes = menus.getMenuItem(showRecords, "起草会议纪要...", function () { me.MenuDraftMeetMinutes(mainPanelId); });
        if (menuDraftMeetMinutes !== null) {
            hxepcMenus.add('-');
            hxepcMenus.add(menuDraftMeetMinutes);
        }

        
        var menuDraftTransmittalCN = menus.getMenuItem(showRecords, "起草文件传递单(中文)...", function () { me.MenuDraftTransmittalCN(mainPanelId); });
        if (menuDraftTransmittalCN !== null) {
            hxepcMenus.add('-');
            hxepcMenus.add(menuDraftTransmittalCN);
        }

        var menuDraftLetterCN = menus.getMenuItem(showRecords, "起草信函(中文)...", function () { me.MenuDraftLetterCN(mainPanelId); });
        if (menuDraftLetterCN !== null) {
            hxepcMenus.add('-');
            hxepcMenus.add(menuDraftLetterCN);
        }

        var menuDraftRecognition = menus.getMenuItem(showRecords, "起草认质认价单...", function () { me.MenuDraftRecognition(mainPanelId); });
        if (menuDraftRecognition !== null) {
            hxepcMenus.add('-');
            hxepcMenus.add(menuDraftRecognition);
        }

        var menuDraftVisa = menus.getMenuItem(showRecords, "起草签证...", function () { me.MenuDraftVisa(mainPanelId); });
        if (menuDraftVisa !== null) {
            hxepcMenus.add('-');
            hxepcMenus.add(menuDraftVisa);
        }

        var menuDraftReport = menus.getMenuItem(showRecords, "起草报告...", function () { me.MenuDraftReport(mainPanelId); });
        if (menuDraftReport !== null) {
            hxepcMenus.add('-');
            hxepcMenus.add(menuDraftReport);
        }

        var menuDraftAsk = menus.getMenuItem(showRecords, "起草请示...", function () { me.MenuDraftReport(mainPanelId); });
        if (menuDraftAsk !== null) {
            hxepcMenus.add('-');
            hxepcMenus.add(menuDraftAsk);
        }

        var menuDraftNotify = menus.getMenuItem(showRecords, "起草通知...", function () { me.MenuDraftReport(mainPanelId); });
        if (menuDraftNotify !== null) {
            hxepcMenus.add('-');
            hxepcMenus.add(menuDraftNotify);
        }

        var menuEditLetterText = menus.getMenuItem(showRecords, "编辑正文...", function () { me.MenuEditLetterText(mainPanelId); });
        if (menuEditLetterText !== null) {
            hxepcMenus.add('-');
            hxepcMenus.add(menuEditLetterText);
        }

        var menuCompany = menus.getMenuItem(showRecords, "添加参建单位...", function () { me.MenuCompany(mainPanelId); });
        if (menuCompany !== null) {
            hxepcMenus.add('-');
            hxepcMenus.add(menuCompany);
        }

        var menuEditCompany = menus.getMenuItem(showRecords, "编辑参建单位...", function () { me.MenuEditCompany(mainPanelId); });
        if (menuEditCompany !== null) {
            hxepcMenus.add('-');
            hxepcMenus.add(menuEditCompany);
        }

        var menuEditDepartment = menus.getMenuItem(showRecords, "编辑项目部门...", function () { me.MenuEditDepartment(mainPanelId); });
        if (menuEditDepartment !== null) {
            hxepcMenus.add('-');
            hxepcMenus.add(menuEditDepartment);
        }

        var menuEditProjectGroup = menus.getMenuItem(showRecords, "编辑项目组...", function () { me.MenuEditProjectGroup(mainPanelId); });
        if (menuEditProjectGroup !== null) {
            hxepcMenus.add('-');
            hxepcMenus.add(menuEditProjectGroup);
        }

        var menuEditProjectInfo = menus.getMenuItem(showRecords, "编辑项目资料...", function () { me.MenuEditProjectInfo(mainPanelId); });
        if (menuEditProjectInfo !== null) {
            hxepcMenus.add('-');
            hxepcMenus.add(menuEditProjectInfo);
        }
       
        var menuImportFile = menus.getMenuItem(showRecords, "导入文件...", function () { me.MenuImportFile(mainPanelId); });
        if (menuImportFile !== null) {
            hxepcMenus.add('-');
            hxepcMenus.add(menuImportFile);
        }

        var menuExportFile = menus.getMenuItem(showRecords, "导出文件...", function () { me.MenuExportFile(mainPanelId); });
        if (menuExportFile !== null) {
            hxepcMenus.add('-');
            hxepcMenus.add(menuExportFile);
        }

        var menuEditFileAttr = menus.getMenuItem(showRecords, "修改文档...", function () { me.MenuEditFileAttr(mainPanelId); });
        if (menuEditFileAttr !== null) {
            hxepcMenus.add('-');
            hxepcMenus.add(menuEditFileAttr);
        }

        var menuUpgradeFile = menus.getMenuItem(showRecords, "文档升版...", function () { me.MenuUpgradeFile(mainPanelId); });
        if (menuUpgradeFile !== null) {
            hxepcMenus.add('-');
            hxepcMenus.add(menuUpgradeFile);
        }

        var menuReplyLetter = menus.getMenuItem(showRecords, "回复信函...", function () { me.MenuReplyLetter(mainPanelId); });
        if (menuReplyLetter !== null) {
            hxepcMenus.add('-');
            hxepcMenus.add(menuReplyLetter);
        }

        var menuGetSendCode = menus.getMenuItem(showRecords, "获取发文编号...", function () { me.MenuGetSendCode(mainPanelId); });
        if (menuGetSendCode !== null) {
            hxepcMenus.add('-');
            hxepcMenus.add(menuGetSendCode);
        }

        var menuSelfCheckDoc = menus.getMenuItem(showRecords, "自检", function () { me.MenuSelfCheckDoc(mainPanelId); });
        if (menuSelfCheckDoc !== null) {
            hxepcMenus.add('-');
            hxepcMenus.add(menuSelfCheckDoc);
        }

        var menuExportStatisProper = menus.getMenuItem(showRecords, "统计...", function () { me.ExportStatisProperMenu(mainPanelId); });
        if (menuExportStatisProper !== null) {
            hxepcMenus.add('-');
            hxepcMenus.add(menuExportStatisProper);
        }

        return hxepcMenus;
    },



    //定义菜单项对应的函数   
    //起草红头文
    MenuDraftDocument: function (mainPanelId) {
        var me = this;
        //Ext.Msg.alert("", "Hello World !");

        var viewTree = Ext.getCmp(mainPanelId).down('treepanel');//获取目录树控件ID

        var nodes =viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {
            var projectKeyword = nodes[0].data.Keyword;

            //var plugins = "HXEPC_Plugins";
            //var state = "exchangeDoc";

            var fmDraftDocument = Ext.create('Ext.plug_ins.HXEPC_Plugins.Document.DraftDocument',// + plugins + '.' + state,
                { title: "", mainPanelId: mainPanelId, projectKeyword: projectKeyword, projectDirKeyword: projectKeyword });

            winDraftDocument = Ext.widget('window', {
                title: '起草红头文',
                closeAction: 'hide',
                width: 618,
                height: 463,
                minWidth: 618,
                minHeight: 463,
                layout: 'fit',
                resizable: false,
                modal: true,
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: fmDraftDocument,
                defaultFocus: 'firstName'
            });

            //fmSelectProfession.send_get_profession_list(function () {
            //    //获取已创建的专业
            //    fmSelectProfession.send_get_created_profession(function () {
            //        winExchangeDoc.show();
            //    });
            //});

            winDraftDocument.show();

            //监听子窗口关闭事件
            winDraftDocument.on('close', function () {

            });

            winDraftDocument.on('beforeclose', function (panel, eOpts) {
                if (panel.title != '起草红头文') {
                    Ext.Msg.alert("错误", "请保存附件编辑！");
                    return false;
                }
            });
        }
    },

    //起草会议纪要
    MenuDraftMeetMinutes: function (mainPanelId) {
        var me = this;
        //Ext.Msg.alert("", "Hello World !");

        var viewTree = Ext.getCmp(mainPanelId).down('treepanel');//获取目录树控件ID

        var nodes = viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {
            var projectKeyword = nodes[0].data.Keyword;

            //var plugins = "HXEPC_Plugins";
            //var state = "exchangeDoc";

            var fmDraftMeetMinutes = Ext.create('Ext.plug_ins.HXEPC_Plugins.Document.DraftMeetMinutes',// + plugins + '.' + state,
                { title: "", mainPanelId: mainPanelId, projectKeyword: projectKeyword, projectDirKeyword: projectKeyword });

            winDraftMeetMinutesCN = Ext.widget('window', {
                title: '起草会议纪要',
                closeAction: 'hide',
                width: 788,
                height: 663,
                minWidth: 788,
                minHeight: 663,
                layout: 'fit',
                resizable: false,
                modal: true,
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: fmDraftMeetMinutes,
                defaultFocus: 'firstName'
            });

            fmDraftMeetMinutes.projectDirKeyword = projectKeyword;

            //获取打开表单时的默认参数
            fmDraftMeetMinutes.sendGetDraftMeetMinutesDefault(function () {
                winDraftMeetMinutesCN.show();
            });
            

            //监听子窗口关闭事件
            winDraftMeetMinutesCN.on('close', function () {

            });

            winDraftMeetMinutesCN.on('beforeclose', function (panel, eOpts) {
                if (panel.title != '起草会议纪要') {
                    Ext.Msg.alert("错误", "请保存附件编辑！");
                    return false;
                }
            });
        }
    },

    //起草文件传递单(中文)
    MenuDraftTransmittalCN: function (mainPanelId) {
        var me = this;
        //Ext.Msg.alert("", "Hello World !");

        var viewTree = Ext.getCmp(mainPanelId).down('treepanel');//获取目录树控件ID

        var nodes = viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {
            var projectKeyword = nodes[0].data.Keyword;

            //var plugins = "HXEPC_Plugins";
            //var state = "exchangeDoc";

            var fmDraftTransmittalCN = Ext.create('Ext.plug_ins.HXEPC_Plugins.Document.DraftTransmittalCN',// + plugins + '.' + state,
                { title: "", mainPanelId: mainPanelId, projectKeyword: projectKeyword, projectDirKeyword: projectKeyword });

            winDraftTransmittalCN = Ext.widget('window', {
                title: '起草文件传递单',
                closeAction: 'hide',
                width: 788,
                height: 588,
                minWidth: 788,
                minHeight: 588,
                layout: 'fit',
                resizable: false,
                modal: true,
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: fmDraftTransmittalCN,
                defaultFocus: 'firstName'
            });

            fmDraftTransmittalCN.projectDirKeyword = projectKeyword;

            //获取打开表单时的默认参数
            fmDraftTransmittalCN.sendGetDraftTransmittalCNDefault(function () {

                winDraftTransmittalCN.show();
            });

           // winDraftTransmittalCN.show();

            //监听子窗口关闭事件
            winDraftTransmittalCN.on('close', function () {

            });

            winDraftTransmittalCN.on('beforeclose', function (panel, eOpts) {
                if (panel.title != '起草文件传递单') {
                    Ext.Msg.alert("错误", "请保存附件编辑！");
                    return false;
                }
            });
        }
    },

    //起草信函（中文）
    MenuDraftLetterCN: function (mainPanelId) {
        var me = this;
        //Ext.Msg.alert("", "Hello World !");

        var viewTree = Ext.getCmp(mainPanelId).down('treepanel');//获取目录树控件ID

        var nodes = viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {
            var projectKeyword = nodes[0].data.Keyword;
            Ext.require('Ext.plug_ins.HXEPC_Plugins.common', function () {
                ShowLetterCNWin(mainPanelId, projectKeyword, projectKeyword);
            });
        }
    },

    

    //起草认质认价单
    MenuDraftRecognition: function (mainPanelId) {
        var me = this;

        var viewTree = Ext.getCmp(mainPanelId).down('treepanel');//获取目录树控件ID

        var nodes = viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {
            var projectKeyword = nodes[0].data.Keyword;

            var fmDraftRecognition = Ext.create('Ext.plug_ins.HXEPC_Plugins.Document.DraftRecognition',
                { title: "", mainPanelId: mainPanelId, projectKeyword: projectKeyword, projectDirKeyword: projectKeyword });

            winDraftRecognition = Ext.widget('window', {
                title: '起草认质认价单',
                closeAction: 'hide',
                width: 788,
                height: 488,
                minWidth: 788,
                minHeight: 488,
                layout: 'fit',
                resizable: false,
                modal: true,
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: fmDraftRecognition,
                defaultFocus: 'firstName'
            });


            fmDraftRecognition.projectDirKeyword = projectKeyword;

            //获取打开表单时的默认参数
            fmDraftRecognition.sendGetDraftRecognitionDefault(function () {

                winDraftRecognition.show();
            });

            

            //监听子窗口关闭事件
            winDraftRecognition.on('close', function () {

            });

            winDraftRecognition.on('beforeclose', function (panel, eOpts) {

            });
        }
    },


    //起草认质认价单
    MenuDraftVisa: function (mainPanelId) {
        var me = this;

        var viewTree = Ext.getCmp(mainPanelId).down('treepanel');//获取目录树控件ID

        var nodes = viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {
            var projectKeyword = nodes[0].data.Keyword;

            var fmDraftVisa = Ext.create('Ext.plug_ins.HXEPC_Plugins.Document.DraftVisa',
                { title: "", mainPanelId: mainPanelId, projectKeyword: projectKeyword, projectDirKeyword: projectKeyword });

            winDraftVisa = Ext.widget('window', {
                title: '起草签证',
                closeAction: 'hide',
                width: 788,
                height: 488,
                minWidth: 788,
                minHeight: 488,
                layout: 'fit',
                resizable: false,
                modal: true,
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: fmDraftVisa,
                defaultFocus: 'firstName'
            });

            fmDraftVisa.projectDirKeyword = projectKeyword;

            //获取打开表单时的默认参数
            fmDraftVisa.sendGetDraftVisaDefault(function () {

                winDraftVisa.show();
            });
            

            //监听子窗口关闭事件
            winDraftVisa.on('close', function () {

            });

            winDraftVisa.on('beforeclose', function (panel, eOpts) {

            });
        }
    },

    //起草报告、请示、通知
    MenuDraftReport: function (mainPanelId) {
        var me = this;

        var viewTree = Ext.getCmp(mainPanelId).down('treepanel');//获取目录树控件ID

        var nodes = viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {
            var projectKeyword = nodes[0].data.Keyword;
            var projectDesc = nodes[0].data.text;

            var fmDraftReport = Ext.create('Ext.plug_ins.HXEPC_Plugins.Document.DraftReport',
                { title: "", mainPanelId: mainPanelId, projectKeyword: projectKeyword, projectDirKeyword: projectKeyword });

            winDraftReport = Ext.widget('window', {
                title: '起草报告、请示、通知',
                closeAction: 'hide',
                width: 788,
                height: 538,
                minWidth: 788,
                minHeight: 538,
                layout: 'fit',
                resizable: false,
                modal: true,
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: fmDraftReport,
                defaultFocus: 'firstName'
            });

            fmDraftReport.fileCodePanel.projectDirKeyword = projectKeyword;
            fmDraftReport.fileCodePanel.projectKeyword = projectKeyword;
            fmDraftReport.fileCodePanel.projectDesc = projectDesc;

            //获取打开表单时的默认参数
            fmDraftReport.sendGetDraftReportDefault(function () {

                winDraftReport.show();
            });


            //监听子窗口关闭事件
            winDraftReport.on('close', function () {

            });

            winDraftReport.on('beforeclose', function (panel, eOpts) {

            });
        }
    },

    //添加参建单位
    MenuCompany: function (mainPanelId) {
        var me = this;
        //Ext.Msg.alert("", "Hello World !");

        var viewTree = Ext.getCmp(mainPanelId).down('treepanel');//获取目录树控件ID

        var nodes = viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {
            var projectKeyword = nodes[0].data.Keyword;

            var fmSelectUnit = Ext.create('Ext.plug_ins.HXEPC_Plugins.Document.SelectUnit', { title: "", mainPanelId: mainPanelId, projectKeyword: projectKeyword });

            winSelectUnit = Ext.widget('window', {
                title: '添加参建单位或项目部门',
                width: 738,
                height: 558,
                minWidth: 738,
                minHeight: 558,
                layout: 'fit',
                resizable: true,
                modal: true,
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: fmSelectUnit,
                defaultFocus: 'firstName'
            });

            fmSelectUnit.projectKeyword = projectKeyword;

            winSelectUnit.show();


            //监听子窗口关闭事件
            winSelectUnit.on('close', function () {
                if (window.parent.resultvalue != null && window.parent.resultvalue !== "") {

                    var unitCode = "";
                    var unitDesc = "";
                    var unitValue = "";

                    unitCode = window.parent.resultvalue;
                    unitDesc = window.parent.unitdesclist;
                    unitValue = window.parent.unitvaluelist;
                    unitType = window.parent.unitType;

                    if (unitCode.indexOf(",") > 0) {
                        unitCode = unitCode.substring(0, unitCode.indexOf(","));
                        unitDesc = unitDesc.substring(0, unitDesc.indexOf(";"));
                    }

                    me.send_createCompanyProject(projectKeyword, unitCode, unitDesc,unitType, mainPanelId);
                }
            });

            //var fmEditCompany = Ext.create('Ext.plug_ins.HXEPC_Plugins.Company.EditCompany',// + plugins + '.' + state,
            //    { title: "", mainPanelId: mainPanelId, projectKeyword: projectKeyword });

            //winEditCompany = Ext.widget('window', {
            //    title: '添加参建单位',
            //    closeAction: 'hide',
            //    width: 788,
            //    height: 518,
            //    minWidth: 788,
            //    minHeight: 518,
            //    layout: 'fit',
            //    resizable: false,
            //    modal: true,
            //    closeAction: 'close', //close 关闭  hide  隐藏  
            //    items: fmEditCompany,
            //    defaultFocus: 'firstName'
            //});

            //winEditCompany.show();

            ////监听子窗口关闭事件
            //winEditCompany.on('close', function () {

            //});

        }
    },

    
    //编辑函件正文
    MenuEditLetterText: function (mainPanelId) {
        var me = this;
        //Ext.Msg.alert("", "Hello World !");

        var viewGrid = Ext.getCmp(mainPanelId).down('gridpanel');//获取目录树控件ID

        var nodes = viewGrid.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {
            var docKeyword = nodes[0].data.Keyword;


            var fmEditLetterText = Ext.create('Ext.plug_ins.HXEPC_Plugins.Document.EditLetterText',// + plugins + '.' + state,
                { title: "", mainPanelId: mainPanelId, docKeyword: docKeyword });

            winEditLetterText = Ext.widget('window', {
                title: '编辑正文',
                closeAction: 'hide',
                width: 588,
                height: 358,
                minWidth: 588,
                minHeight: 358,
                layout: 'fit',
                resizable: false,
                modal: true,
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: fmEditLetterText,
                defaultFocus: 'firstName'
            });

            //fmEditLetterText.createProjectButton.setVisible(false);

            fmEditLetterText.sendGetEditLetterTextDefault(function () {
                winEditLetterText.show();
            }
                );

            //监听子窗口关闭事件
            winEditLetterText.on('close', function () {
              
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

            var fmEditCompany = Ext.create('Ext.plug_ins.HXEPC_Plugins.Company.EditCompany',// + plugins + '.' + state,
                { title: "", mainPanelId: mainPanelId, projectKeyword: projectKeyword });

            winEditCompany = Ext.widget('window', {
                title: '编辑参建单位',
                closeAction: 'hide',
                width: 788,
                height: 518,
                minWidth: 788,
                minHeight: 518,
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

    
  //编辑项目部门
    MenuEditDepartment: function (mainPanelId) {
            var me = this;
            //Ext.Msg.alert("", "Hello World !");

            var viewTree = Ext.getCmp(mainPanelId).down('treepanel');//获取目录树控件ID

            var nodes = viewTree.getSelectionModel().getSelection();//获取已选取的节点
            if (nodes !== null && nodes.length > 0) {
                var projectKeyword = nodes[0].data.Keyword;

                var fmEditDepartment = Ext.create('Ext.plug_ins.HXEPC_Plugins.Company.EditDepartment',// + plugins + '.' + state,
                    { title: "", mainPanelId: mainPanelId, projectKeyword: projectKeyword });

                winEditDepartment = Ext.widget('window', {
                    title: '编辑项目部门',
                    closeAction: 'hide',
                    width: 788,
                    height: 518,
                    minWidth: 788,
                    minHeight: 518,
                    layout: 'fit',
                    resizable: false,
                    modal: true,
                    closeAction: 'close', //close 关闭  hide  隐藏  
                    items: fmEditDepartment,
                    defaultFocus: 'firstName'
                });

                //fmSelectProfession.send_get_profession_list(function () {
                //    //获取已创建的专业
                //    fmSelectProfession.send_get_created_profession(function () {
                //        winExchangeDoc.show();
                //    });
                //});
                fmEditDepartment.createProjectButton.setVisible(false);

                winEditDepartment.show();

                //监听子窗口关闭事件
                winEditDepartment.on('close', function () {

                });

            }
        },

    MenuEditProjectGroup: function (mainPanelId) {
        var me = this;

        var viewTree = Ext.getCmp(mainPanelId).down('treepanel');//获取目录树控件ID

        var nodes = viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {
            var projectKeyword = nodes[0].data.Keyword;

            var fmEditProjectGroup = Ext.create('Ext.plug_ins.HXEPC_Plugins.Project.EditProjectGroup',// + plugins + '.' + state,
                { title: "", mainPanelId: mainPanelId, projectKeyword: projectKeyword });

            winEditProjectGroup = Ext.widget('window', {
                title: '编辑用户组',
                closeAction: 'hide',
                width: 408,
                height: 518,
                minWidth: 408,
                minHeight: 518,
                layout: 'fit',
                resizable: false,
                modal: true,
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: fmEditProjectGroup,
                defaultFocus: 'firstName'
            });

            //fmSelectProfession.send_get_profession_list(function () {
            //    //获取已创建的专业
            //    fmSelectProfession.send_get_created_profession(function () {
            //        winExchangeDoc.show();
            //    });
            //});
            //fmEditProjectGroup.createProjectButton.setVisible(false);

            winEditProjectGroup.show();

            //监听子窗口关闭事件
            winEditProjectGroup.on('close', function () {

            });

        }
    },

    
    MenuEditProjectInfo: function (mainPanelId) {
            var me = this;

            var viewTree = Ext.getCmp(mainPanelId).down('treepanel');//获取目录树控件ID

            var nodes = viewTree.getSelectionModel().getSelection();//获取已选取的节点
            if (nodes !== null && nodes.length > 0) {
                var projectKeyword = nodes[0].data.Keyword;

                var fmEditProjectInfo = Ext.create('Ext.plug_ins.HXEPC_Plugins.Project.EditProjectInfo',// + plugins + '.' + state,
                    { title: "", mainPanelId: mainPanelId, projectKeyword: projectKeyword });

                winEditProjectInfo = Ext.widget('window', {
                    title: '编辑项目资料',
                    closeAction: 'hide',
                    width: 788,
                    height: 538,
                    minWidth: 788,
                    minHeight: 538,
                    layout: 'fit',
                    resizable: false,
                    modal: true,
                    closeAction: 'close', //close 关闭  hide  隐藏  
                    items: fmEditProjectInfo,
                    defaultFocus: 'firstName'
                });

                //fmSelectProfession.send_get_profession_list(function () {
                //    //获取已创建的专业
                //    fmSelectProfession.send_get_created_profession(function () {
                //        winExchangeDoc.show();
                //    });
                //});
                //fmEditProjectInfo.createProjectButton.setVisible(false);

                winEditProjectInfo.show();

                //监听子窗口关闭事件
                winEditProjectInfo.on('close', function () {

                });

            }
        },

    //导入文件
    MenuImportFile:function (mainPanelId) {
        var me = this;

        var viewTree = Ext.getCmp(mainPanelId).down('treepanel');//获取目录树控件ID

        var nodes = viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {
            var projectKeyword = nodes[0].data.Keyword;

            var fmImportFile = Ext.create('Ext.plug_ins.HXEPC_Plugins.Document.ImportFile',// + plugins + '.' + state,
                { title: "", mainPanelId: mainPanelId, projectKeyword: projectKeyword, projectDirKeyword: projectKeyword });

            winImportFile = Ext.widget('window', {
                title: '导入文件',
                closeAction: 'hide',
                width: 788,
                height: 538,
                minWidth: 788,
                minHeight: 538,
                layout: 'fit',
                resizable: false,
                modal: true,
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: fmImportFile,
                defaultFocus: 'firstName'
            });

            fmImportFile.sendGetImportFileDefault(function () {
                winImportFile.show();
            });
            

            //监听子窗口关闭事件
            winImportFile.on('close', function () {

            });

        }
    },

    ////文档升级
    //MenuUpgradeFile: function (mainPanelId) {
    //    var me = this;

    //    var viewTree = Ext.getCmp(mainPanelId).up('_mainSourceView').down('_mainProjectTree').down('treepanel');//获取目录树控件ID

    //    var nodes = viewTree.getSelectionModel().getSelection();//获取已选取的节点
    //    if (nodes !== null && nodes.length > 0) {
    //        var projectKeyword = nodes[0].data.Keyword;

    //        var fmImportFile = Ext.create('Ext.plug_ins.HXEPC_Plugins.Document.ImportFile',// + plugins + '.' + state,
    //            { title: "", mainPanelId: mainPanelId, projectKeyword: projectKeyword, projectDirKeyword: projectKeyword });

    //        winImportFile = Ext.widget('window', {
    //            title: '文档升版',
    //            closeAction: 'hide',
    //            width: 788,
    //            height: 538,
    //            minWidth: 788,
    //            minHeight: 538,
    //            layout: 'fit',
    //            resizable: false,
    //            modal: true,
    //            closeAction: 'close', //close 关闭  hide  隐藏  
    //            items: fmImportFile,
    //            defaultFocus: 'firstName'
    //        });

    //        fmImportFile.winAction = "文档升版";
    //        var viewGrid = Ext.getCmp(mainPanelId).up('_mainSourceView').down('_mainDocGrid').down('gridpanel');//Ext.getCmp('_DocGrid');
    //        var gnodes = viewGrid.getSelectionModel().getSelection();//获取已选取的节点
    //        if (gnodes !== null && gnodes.length > 0) {
    //            fmImportFile.docKeyword = gnodes[0].data.Keyword;
    //            fmImportFile.setImportFileDefault();
    //        }

    //        fmImportFile.sendGetImportFileDefault(function () {
    //            winImportFile.show();
    //        });

    //        //winImportFile.show();

    //        //监听子窗口关闭事件
    //        winImportFile.on('close', function () {

    //        });


    //    }
    //},

    MenuReplyLetter: function (mainPanelId) {
        var me = this;

        var viewGrid = Ext.getCmp(mainPanelId).down('gridpanel');//获取目录树控件ID

        var nodes = viewGrid.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {
            var docKeyword = nodes[0].data.Keyword;
            
            Ext.Ajax.request({

                url: 'WebApi/Post',
                method: "POST",
                params: {
                    C: "AVEVA.CDMS.HXEPC_Plugins.Document", A: "GetPreReplyLetterInfo",
                    sid: localStorage.getItem("sid"), DocKeyword: docKeyword
                },
                success: function (response, options) {

                    //获取数据后，更新窗口
                    var res = Ext.JSON.decode(response.responseText, true);
                    var state = res.success;
                    if (state === true) {

                        var recod = eval(res.data[0]);

                        var projectKeyword = recod.ProjectKeyword;//获取新建的目录id
                        
                        var mDocKeyword = recod.DocKeyword;

                        var recUnitCode = recod.MainfeederCode;

                        var recUnitDesc = recod.MainfeederDesc;
                        //收文编码
                        var recCode = recod.RecCode;
                        var sendCode = recod.SendCode;

                        Ext.require('Ext.plug_ins.HXEPC_Plugins.common', function () {
                            ShowLetterCNWin(mainPanelId, projectKeyword, projectKeyword, mDocKeyword, recUnitCode, recUnitDesc, sendCode, recCode);
                        });
                    } else {
                        var errmsg = res.msg;
                        Ext.Msg.alert("错误信息", errmsg);
                    }
                }
            });
        }
    },

    //获取发文编号
    MenuGetSendCode: function (mainPanelId) {
        var me = this;

        var FileCode = "";
        var viewGrid = Ext.getCmp(mainPanelId).down('gridpanel');//获取目录树控件ID

        var nodes = viewGrid.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {
            var docKeyword = nodes[0].data.Keyword;

            Ext.Ajax.request({

                url: 'WebApi/Post',
                method: "POST",
                params: {
                    C: "AVEVA.CDMS.HXEPC_Plugins.Document", A: "GetSendCode",
                    sid: localStorage.getItem("sid"), DocKeyword: docKeyword
                },
                success: function (response, options) {

                    //获取数据后，更新窗口
                    var res = Ext.JSON.decode(response.responseText, true);
                    var state = res.success;
                    if (state === true) {

                        Ext.MessageBox.close();//关闭等待对话框

                        var recod = eval(res.data[0]);

                        FileCode = recod.FileCode;

                        var fmResetFileCode = Ext.create('Ext.plug_ins.' + 'HXEPC_Plugins' + '.Document.' + 'resetFileCode', { title: "", mainPanelId: mainPanelId, docKeyword: docKeyword });

                        winResetFileCode = Ext.widget('window', {
                            title: '填写发文编号',
                            closeAction: 'hide',
                            width: 550,
                            height: 250,
                            minWidth: 550,
                            minHeight: 250,
                            layout: 'fit',
                            resizable: true,
                            modal: true,
                            closeAction: 'close', //close 关闭  hide  隐藏  
                            items: fmResetFileCode,
                            defaultFocus: 'firstName'
                        });

                        fmResetFileCode.sendCodeText.setValue(FileCode);

                        fmResetFileCode.ifGotoNextState = false;

                        winResetFileCode.show();

                        //监听子窗口关闭事件
                        winResetFileCode.on('close', function () {

                        });
                    }
                }
            });
        }
    },

    //文档升级
    MenuUpgradeFile: function (mainPanelId) {
        var me = this;
        var projectKeyword;

        var viewTree = Ext.getCmp(mainPanelId).up('_mainSourceView').down('_mainProjectTree').down('treepanel');//获取目录树控件ID

        var nodes = viewTree.getSelectionModel().getSelection();//获取已选取的目录节点
        if (nodes !== null && nodes.length > 0) {
            projectKeyword = nodes[0].data.Keyword;

            //文件升版窗口
            var fmUpImportFile = Ext.create('Ext.plug_ins.HXEPC_Plugins.Document.UpImportFile',// + plugins + '.' + state,
                { title: "", mainPanelId: mainPanelId, projectKeyword: projectKeyword, projectDirKeyword: projectKeyword });

            winUpImportFile = Ext.widget('window', {
                title: '文件升版',
                closeAction: 'hide',
                width: 788,
                height: 538,
                minWidth: 788,
                minHeight: 538,
                layout: 'fit',
                resizable: false,
                modal: true,
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: fmUpImportFile,
                defaultFocus: 'firstName'
            });
            // winUpImportFile.winAction = "文档升版";

            //显示文件导入窗口
            fmUpImportFile.sendGetImportFileDefault(function () {

                var viewGrid = Ext.getCmp(mainPanelId).up('_mainSourceView').down('_mainDocGrid').down('gridpanel');

                var rs = viewGrid.getSelectionModel().getSelection();//获取选择的文档
                if (rs !== null && rs.length > 0) {
                    var rec = rs[0];//第一个文档
                    var docKeyword = rec.data.Keyword;

                    fmUpImportFile.fileUploadPanel.docKeyword = docKeyword;
                    fmUpImportFile.fileUploadPanel.setImportFileDefault();
                    winUpImportFile.show();
                }
            });

            //监听子窗口关闭事件
            winUpImportFile.on('close', function () {
                //me.loadDocListStore(mainPanelId, function () { });//重新加载文档


            });

        }

    },


    //导出文件
    MenuExportFile: function (mainPanelId) {
        var me = this;

        var viewTree = Ext.getCmp(mainPanelId).down('treepanel');//获取目录树控件ID
        var nodes = viewTree.getSelectionModel().getSelection();//获取已选取的节点

        if (nodes !== null && nodes.length > 0) {
            var projectKeyword = nodes[0].data.Keyword;
            var fmConditionalFilterEx = Ext.create('Ext.plug_ins.HXEPC_Plugins.Document.ConditionalFilter',// + plugins + '.' + state,
            { title: "", mainPanelId: mainPanelId, projectKeyword: projectKeyword, projectDirKeyword: projectKeyword });

            winexportStatisProperFormEx = Ext.widget('window', {
                title: '导出',
                closeAction: 'hide',
                height: 300,//275,
                width: 480,
                //minWidth: 300,
                //minHeight: 300,
                layout: 'fit',
                resizable: false,
                modal: true,
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: fmConditionalFilterEx,
                defaultFocus: 'firstName',
                buttons: [{
                    text: "提交", handler: function () {
                        // Ext.MessageBox.wait("导出中，请稍后...", "等待");

                        var itemAttrjson = fmConditionalFilterEx.getParam();
                        fmConditionalFilterEx.exportStatisProperForm.getForm().submit({//客户端的数据提交给服务器  
                        //Ext.Ajax.request({
                            url: 'WebApi/Post',
                            method: "POST",
                            standardSubmit: true,//正常表单提交。（默认是ajax提交，所以设置此值）
                            params: {
                                C: "AVEVA.CDMS.HXEPC_Plugins.Document", A: "ExportFolder",
                                sid: localStorage.getItem("sid"), projectKeyword: projectKeyword, ConditionalAttrJson: itemAttrjson
                            },

                            failure: function (form, action) {
                            //    Ext.MessageBox.alert('失败');
                            //   // Ext.MessageBox.alert('失败', action.result.errors);
                            //   // Ext.MessageBox.close();//关闭等待对话框
                            },
                            success: function (form, action) {

                            //    Ext.MessageBox.alert('信息', "导出成功");
                            //    //Ext.MessageBox.close();//关闭等待对话框
                            }
                        });
                        winexportStatisProperFormEx.close();

                        //Ext.MessageBox.wait("请稍候...", "等待");

                        //var itemAttrjson = fmConditionalFilterEx.getParam();

                        //Ext.Ajax.request({

                        //    url: 'WebApi/Post',
                        //         method: "POST",
                        //         //standardSubmit: true,//正常表单提交。（默认是ajax提交，所以设置此值）
                        //         params: {
                        //             C: "AVEVA.CDMS.HXEPC_Plugins.Document", A: "ExportFolder",
                        //             sid: localStorage.getItem("sid"), projectKeyword: projectKeyword,
                        //             ConditionalAttrJson: itemAttrjson
                        //         },
                        //    success: function (res, options) {

                        //        Ext.MessageBox.close();//关闭等待对话框

                        //        var obj = Ext.decode(res.responseText);
                        //        //console.log(obj);//可以到火狐的firebug下面看看obj里面的结构   
                        //        if (obj.success == true) {
                        //            var prePath = obj.data[0].prePath;
                        //            var fileName = obj.data[0].filename;
                        //            //var para = obj.data[0].para;
                        //            alert(prePath);
                        //            //组装文件下载路径
                        //            var url = prePath;//+ encodeURIComponent(fileName);//+ "?p=" + para;

                        //            //var url = encodeURIComponent(obj.data[0].path);

                        //            //window.open(url, '_blank');//新窗口打开链接
                        //            var popUp = window.open(url, '_blank');//新窗口打开链接
                        //            if (popUp == null || typeof (popUp) == 'undefined') {
                        //                Ext.Msg.alert("下载失败", '请解除窗口阻拦，重新点击下载。');
                        //            }
                        //            else {
                        //                popUp.focus();
                        //            }

                        //        } else { Ext.Msg.alert("下载失败", obj.msg); }


                        //    },
                        //    failure: function (response, options) {
                        //        Ext.MessageBox.close();//关闭等待对话框
                        //    }
                        //});
                    }
                },
                {
                    text: "取消", handler: function () {
                        winexportStatisProperFormEx.close();
                    }
                }]
            });

            winexportStatisProperFormEx.show();


        }



    },

    //生成立项单
    MenuCreatePrjDocument: function (mainPanelId) {
        var me = this;
        //Ext.Msg.alert("", "Hello World !");

        var viewTree = Ext.getCmp(mainPanelId).down('treepanel');//获取目录树控件ID
        //var viewTree = me.maindocgrid.up('_mainSourceView').down('_mainProjectTree').down('treepanel');//获取目录树控件ID

        var nodes = viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {

            var projectKeyword = nodes[0].data.Keyword;

            //弹出操作窗口
            var _fmCreateProjectListing = Ext.create('Ext.plug_ins.HXEPC_Plugins.CreateProjectListing', { title: "", mainPanelId: viewTree.id, projectKeyword: projectKeyword });

            winCreateProjectListing = Ext.widget('window', {
                title: '生成项目立项单',
                closeAction: 'hide',
                width: 765,
                height: 571,
                minWidth: 765,
                minHeight: 571,
                layout: 'fit',
                resizable: true,
                modal: true,
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: _fmCreateProjectListing,
                defaultFocus: 'firstName'
            });

            winCreateProjectListing.show();
            //监听子窗口关闭事件
            winCreateProjectListing.on('close', function () {

            });
        }
    },


    //统计属性
    ExportStatisProperMenu: function (mainPanelId) {
        var me = this;

        var viewTree = Ext.getCmp(mainPanelId).down('treepanel');//获取目录树控件ID
        var nodes = viewTree.getSelectionModel().getSelection();//获取已选取的节点

        if (nodes !== null && nodes.length > 0) {
            var projectKeyword = nodes[0].data.Keyword;

            var fmConditionalFilter = Ext.create('Ext.plug_ins.HXEPC_Plugins.Document.ConditionalFilter',// + plugins + '.' + state,
                { title: "", mainPanelId: mainPanelId, projectKeyword: projectKeyword, projectDirKeyword: projectKeyword });

            winexportStatisProperForm = Ext.widget('window', {
                title: '统计',
                closeAction: 'hide',
                height: 300,//275,
                width: 480,
                //minWidth: 300,
                //minHeight: 300,
                layout: 'fit',
                resizable: false,
                modal: true,
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: fmConditionalFilter,
                defaultFocus: 'firstName',
                buttons: [{
                    text: "提交", handler: function () {
                        // Ext.MessageBox.wait("导出中，请稍后...", "等待");

                        var itemAttrjson = fmConditionalFilter.getParam();
                       // fmConditionalFilter.exportStatisProperForm.getForm().submit({//客户端的数据提交给服务器  
                        Ext.Ajax.request({
                            url: 'WebApi/Post',
                            method: "POST",
                           // standardSubmit: true,//正常表单提交。（默认是ajax提交，所以设置此值）
                            params: {
                                C: "AVEVA.CDMS.HXEPC_Plugins.Document", A: "ExportProper",
                                sid: localStorage.getItem("sid"), projectKeyword: projectKeyword, ConditionalAttrJson: itemAttrjson
                            },

                            failure: function (form, action) {
                            //    Ext.MessageBox.alert('失败');
                            //   // Ext.MessageBox.alert('失败', action.result.errors);
                            //   // Ext.MessageBox.close();//关闭等待对话框
                            },
                            success: function (form, action) {

                            //    Ext.MessageBox.alert('信息', "导出成功");
                            //    //Ext.MessageBox.close();//关闭等待对话框
                            }
                        });
                        winexportStatisProperForm.close();
                    }
                },
                {
                    text: "取消", handler: function () {
                        winexportStatisProperForm.close();
                    }
                }]
            });

            winexportStatisProperForm.show();
        }



    },

    //自检菜单
    MenuSelfCheckDoc: function (mainPanelId) {
        var me = this;
        //Ext.getCmp(mainPanelId).up('_mainSourceView').down('_mainDocGrid').SelfCheckDoc();

        var viewGrid = Ext.getCmp(mainPanelId).up('_mainSourceView').down('_mainDocGrid').down('gridpanel');

        var rs = viewGrid.getSelectionModel().getSelection();//获取选择的文档
        if (rs !== null && rs.length > 0) {
            var rec = rs[0];//第一个文档
            var docKeyword = rec.data.Keyword;

            //著录表窗口
            var projectKeyword;

            var viewTree = Ext.getCmp(mainPanelId).up('_mainSourceView').down('_mainProjectTree').down('treepanel');
            var nodes = viewTree.getSelectionModel().getSelection();//获取已选取的节点
            if (nodes !== null && nodes.length > 0) {
                projectKeyword = nodes[0].data.Keyword;

            }
            var _fmEditFileProperties = Ext.create('Ext.plug_ins.HXEPC_Plugins.Document.EditFileProperties', {
                title: "", projectKeyword: projectKeyword, projectDirKeyword: projectKeyword,
                docClass: me.docClass
            });
            winEditFileProperties = Ext.widget('window', {
                title: '修改文件著录属性',
                closeAction: 'hide',
                width: 780,
                height: 466,
                minWidth: 300,
                minHeight: 300,
                layout: 'fit',
                resizable: true,
                modal: true,
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: _fmEditFileProperties,
                defaultFocus: 'firstName'
            });

            //获取文档的著录表属性
            Ext.Ajax.request({

                url: 'WebApi/Post',
                method: "POST",
                params: {
                    C: "AVEVA.CDMS.HXEPC_Plugins.Document", A: "GetCataloguAttr",
                    sid: localStorage.getItem("sid"), docKeyword: docKeyword
                },
                success: function (response, options) {
                    var res = Ext.JSON.decode(response.responseText, true);
                    var state = res.success;
                    if (state === false) {
                        var errmsg = res.msg;
                        Ext.Msg.alert("错误信息", errmsg);
                    }
                    else {
                        var recod = res.data[0];

                        _fmEditFileProperties.strRootProjectCode = recod.CA_PROCODE;//项目码
                        _fmEditFileProperties.strRootProjectDesc = recod.CA_PRONAME;//项目名称
                        if (_fmEditFileProperties.strRootProjectCode === undefined || _fmEditFileProperties.strRootProjectCode === "") {
                            // _fmEditFileProperties.getprocode();
                        }

                        var fileCodeType = "";
                        if (recod.CA_PROCODE === undefined || recod.CA_PROCODE === "") {
                            fileCodeType = "运营管理类";
                        } else {
                            fileCodeType = "项目管理类";
                        }

                        var orgiAttrArray =
                        [
                         {
                             needNewFileCode: false, fileCodeType: fileCodeType,
                             code: recod.CA_FILECODE, origcode: recod.CA_ORIFILECODE,
                             desc: recod.CA_FILETITLE,
                             reference: recod.CA_REFERENCE, volumenumber: recod.CA_VOLUMENUMBER,
                             responsibility: recod.CA_RESPONSIBILITY, page: recod.CA_PAGE,
                             share: recod.CA_NUMBER, medium: recod.CA_MEDIUM, languages: recod.CA_LANGUAGES,
                             proname: _fmEditFileProperties.strRootProjectDesc,//recod.CA_PRONAME,
                             procode: _fmEditFileProperties.strRootProjectCode, //recod.CA_PROCODE,

                             major: recod.CA_MAJOR, crew: recod.CA_CREW,
                             factorycode: recod.CA_FACTORY, factoryname: recod.CA_FACTORYNAME,
                             systemcode: recod.CA_SYSTEM, systemname: recod.CA_SYSTEMNAME,

                             workClass: recod.CA_WORKTYPE, workSub: recod.CA_WORKSUBTIEM,
                             department: recod.CA_UNIT,

                             receiveType: recod.CA_FILETYPE, fNumber: recod.CA_FLOWNUMBER,
                             edition: recod.CA_EDITION,

                             relationfilecode: recod.CA_RELATIONFILECODE, relationfilename: recod.CA_RELATIONFILENAME,
                             filespec: recod.CA_FILESPEC, fileunit: recod.CA_FILEUNIT,
                             secretgrade: recod.CA_SECRETGRADE, keepingtime: recod.CA_KEEPINGTIME,
                             filelistcode: recod.CA_FILELISTCODE, filelisttime: recod.CA_FILELISTTIME,
                             racknumber: recod.CA_RACKNUMBER, note: recod.CA_NOTE
                         }
                        ];
                        rec = me.getAttrFromOrgArray(rec, orgiAttrArray);
                        //从拿到的数据填充界面
                        _fmEditFileProperties.setFilePropertiesDefault(rec.data);
                    }
                }
            });

            window.parent.resultarray = undefined;

            //winImportFile.hide();
            winEditFileProperties.show();
            //监听子窗口关闭事件
            winEditFileProperties.on('close', function () {
                //winImportFile.show();
                if (window.parent.resultarray === undefined) { return; }

                var res = window.parent.resultarray;
                rec = me.getAttrFromOrgArray(rec, res);
                var docAttr = [
                    { name: "CA_REFERENCE", value: rec.data.reference, attrtype: "attrData" },
                    { name: "CA_VOLUMENUMBER", value: rec.data.volumenumber, attrtype: "attrData" },
                    { name: "CA_REFERENCE", value: rec.data.reference, attrtype: "attrData" },
                    { name: "CA_FILECODE", value: rec.data.code, attrtype: "attrData" },
                    //责任人
                    { name: "CA_RESPONSIBILITY", value: rec.data.responsibility, attrtype: "attrData" },
                    { name: "CA_FILETITLE", value: rec.data.desc, attrtype: "attrData" },
                    { name: "CA_PAGE", value: rec.data.page, attrtype: "attrData" },
                    { name: "CA_NUMBER", value: rec.data.share, attrtype: "attrData" },
                    { name: "CA_MEDIUM", value: rec.data.medium, attrtype: "attrData" },
                    { name: "CA_LANGUAGES", value: rec.data.languages, attrtype: "attrData" },
                    { name: "CA_PRONAME", value: rec.data.proname, attrtype: "attrData" },
                    { name: "CA_PROCODE", value: rec.data.procode, attrtype: "attrData" },
                    { name: "CA_MAJOR", value: rec.data.major, attrtype: "attrData" },
                    { name: "CA_CREW", value: rec.data.crew, attrtype: "attrData" },
                    { name: "CA_FACTORY", value: rec.data.factorycode, attrtype: "attrData" },
                    { name: "CA_FACTORYNAME", value: rec.data.factoryname, attrtype: "attrData" },
                    { name: "CA_SYSTEM", value: rec.data.systemcode, attrtype: "attrData" },
                    { name: "CA_SYSTEMNAME", value: rec.data.systemname, attrtype: "attrData" },
                    { name: "CA_RELATIONFILECODE", value: rec.data.relationfilecode, attrtype: "attrData" },
                    { name: "CA_RELATIONFILENAME", value: rec.data.relationfilename, attrtype: "attrData" },
                    { name: "CA_FILESPEC", value: rec.data.filespec, attrtype: "attrData" },
                    { name: "CA_FILEUNIT", value: rec.data.fileunit, attrtype: "attrData" },
                    { name: "CA_SECRETGRADE", value: rec.data.secretgrade, attrtype: "attrData" },
                    { name: "CA_KEEPINGTIME", value: rec.data.keepingtime, attrtype: "attrData" },
                    { name: "CA_FILELISTCODE", value: rec.data.filelistcode, attrtype: "attrData" },
                    { name: "CA_FILELISTTIME", value: rec.data.filelisttime, attrtype: "attrData" },
                    { name: "CA_RACKNUMBER", value: rec.data.racknumber, attrtype: "attrData" },
                    { name: "CA_NOTE", value: rec.data.note, attrtype: "attrData" },
                    { name: "CA_WORKTYPE", value: rec.data.workClass, attrtype: "attrData" },
                    { name: "CA_WORKSUBTIEM", value: rec.data.workSub, attrtype: "attrData" },
                    { name: "CA_UNIT", value: rec.data.department, attrtype: "attrData" },
                    { name: "CA_FILETYPE", value: rec.data.receiveType, attrtype: "attrData" },
                    { name: "CA_FLOWNUMBER", value: rec.data.fNumber, attrtype: "attrData" },
                    { name: "CA_EDITION", value: rec.data.edition, attrtype: "attrData" },
                    { name: "CA_ATTRTEMP", value: "NONCOMM", attrtype: "attrData" }
                ];
                me.updateDocAttr(rec.data.Keyword, docAttr);
            });

        } else {
            Ext.Msg.alert("错误信息", "请选择文档！");
        }


    },
    //自检属性用的方法，修改rec的data(临时)
    getAttrFromOrgArray: function (rec, orgiAttrArray) {
        //var me = this;
        if (orgiAttrArray === undefined || orgiAttrArray.length <= 0) {
            return;
        }
        var res = orgiAttrArray[0];
        //var Keyword = rec.data.name;
        //文件编码
        rec.set('code', res.code);
        //原文件编码
        rec.set('origcode', res.origcode);
        //文件题名
        rec.set('desc', res.desc);
        //档号
        rec.set('reference', res.reference);
        //卷内序号
        rec.set('volumenumber', res.volumenumber);
        //责任人
        rec.set('responsibility', res.responsibility);
        //页数
        rec.set('page', res.page);
        //份数
        rec.set('share', res.share);
        //介质
        rec.set('medium', res.medium);
        //语种
        rec.set('languages', res.languages);
        //项目名称
        rec.set('proname', res.proname);
        //项目代码
        rec.set('procode', res.procode);
        //专业
        rec.set('major', res.major);
        //机组
        rec.set('crew', res.crew);
        //厂房代码
        rec.set('factorycode', res.factorycode);
        //厂房名称
        rec.set('factoryname', res.factoryname);
        //系统代码
        rec.set('systemcode', res.systemcode);
        //系统名称
        rec.set('systemname', res.systemname);
        //关联文件编码
        rec.set('relationfilecode', res.relationfilecode);
        //关联文件题名
        rec.set('relationfilename', res.relationfilename);
        //案卷规格
        rec.set('filespec', res.filespec);
        //归档单位
        rec.set('fileunit', res.fileunit);
        //密级
        rec.set('secretgrade', res.secretgrade);
        //保管时间
        rec.set('keepingtime', res.keepingtime);
        //归档文件清单编码
        rec.set('filelistcode', res.filelistcode);
        //归档日期
        rec.set('filelisttime', res.filelisttime);
        //排架号
        rec.set('racknumber', res.racknumber);
        //备注
        rec.set('note', res.note);

        //是否新建文件编码
        rec.set('needNewFileCode', res.needNewFileCode);
        //文件编码类型
        rec.set('fileCodeType', res.fileCodeType);

        //文件类型
        rec.set('receiveType', res.receiveType);
        //流水号
        rec.set('fNumber', res.fNumber);
        //版本
        rec.set('edition', res.edition);

        //工作分类代码
        rec.set('workClass', res.workClass);
        //工作分项代码
        rec.set('workSub', res.workSub);
        //部门代码
        rec.set('department', res.department);

        //收文编码
        rec.set('receiptcode', res.receiptcode);

        //原件份数
        rec.set('originalshare', res.originalshare);
        //复印件份数
        rec.set('copyshare', res.copyshare);
        //扫描件份数
        rec.set('scanshare', res.scanshare);
        //电子文件份数
        rec.set('elecshare', res.elecshare);

        rec.commit();

        return rec;
    },

    //自检属性用的方法，修改著录表后提交数据给后台，后台修改数据库
    updateDocAttr: function (Keyword, AttrObject) {
        var me = this;
        AttrJson = Ext.JSON.encode(AttrObject);

        Ext.Ajax.request({

            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.WebApi.DocController", A: "UpdateDocAttr",
                sid: localStorage.getItem("sid"), docKeyword: Keyword,
                docAttrJson: AttrJson
            },
            success: function (response, options) {
                var obj = Ext.decode(response.responseText);
                if (obj.success == true) {
                    Ext.Msg.alert("提示", "修改成功");
                }
            }
        });
    },

    //新建公司资料目录
    send_createCompanyProject: function (projectKeyword, companyCode, companyDesc, companyType, mainPanelId) {
        var me = this;



        //获取公司编号Text
        //var companyCode = me.CompanyCodeText.value;

        //获取公司名称Text
        //var companyDesc = me.CompanyDescText.value;



        ////获取表单数据，转换成JSON字符串
        var projectAttr =
        [
           // { name: 'companyId', value: me.curCompanyId },
            { name: 'companyCode', value: companyCode },
            { name: 'companyDesc', value: companyDesc },
            { name: 'companyType', value: companyType }
        ];

        var projectAttrJson = Ext.JSON.encode(projectAttr);

        Ext.MessageBox.wait("正在创建参建单位目录，请稍候...", "等待");


        Ext.Ajax.request({

            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.HXEPC_Plugins.Company", A: "CreateCompanyProject",
                sid: localStorage.getItem("sid"), ProjectKeyword: projectKeyword,
                projectAttrJson: projectAttrJson
            },
            success: function (response, options) {

                //获取数据后，更新窗口
                var res = Ext.JSON.decode(response.responseText, true);
                var state = res.success;
                if (state === true) {

                    Ext.MessageBox.close();//关闭等待对话框

                    var recod = eval(res.data[0]);

                    //me.newProjectKeyword = recod.ProjectKeyword;//获取新建的目录id


                    //Ext.Msg.alert("提示", "保存成功!");

                    //me.refreshPanel();
                    //处理返回事件
                    //me.send_createCompanyProject_callback(recod.ProjectKeyword, options, "");//, me.projectKeyword, closeWin);

                    var tree = Ext.getCmp(mainPanelId).up('_mainSourceView').down('_mainProjectTree').down('treepanel');//;
                    var viewTreeStore = tree.store;

                    viewTreeStore.load({
                        callback: function (records, options, success) {//添加回调，获取子目录的文件数量

                            //展开目录
                            Ext.require('Ext.ux.Common.comm', function () {
                                Ext.getCmp('contentPanel').down('_mainProjectTree').ExpendProject(projectKeyword);
                            });
                        }
                    });
                } else {
                    var errmsg = res.msg;
                    Ext.Msg.alert("错误信息", errmsg);
                }
            },
            failure: function (response, options) {
                ////Ext.Msg.alert("错误", "连接服务器失败！<br>" + response.responseText);
            }

        })

    }

});
Ext.onReady;

