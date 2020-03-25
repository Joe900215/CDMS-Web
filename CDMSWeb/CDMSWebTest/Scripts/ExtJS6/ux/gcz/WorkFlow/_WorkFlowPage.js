/*定义流程页*/
Ext.define('Ext.ux.m.WorkFlow._WorkFlowPage', {
    //extend: 'Ext.container.Container',
    extend: 'Ext.panel.Panel',
    alias: 'widget._workFlowPage', // 此类的xtype类型为buttontransparent 
    //layout:'vbox',
    layout: {
        type: 'vbox',
        align: 'stretch',
        pack: 'start'
    },
    //layout: 'fit',
    //height: '100%',
    flex:1,
    initComponent: function () {
        var me = this;
        me.renderTo = me.el;
        //me.renderTo = Ext.getBody();

        //工作流的父对象类型，可以是Doc,Project或者Workflow
        me.objKeywordType = "";
        me.objKeyword = "";

        me._contentAuditsStore = Ext.create("Ext.data.Store", {
            model: 'CDMSWap.model.Audit',//模型路径：\simplecdms\scripts\app\model\Property.js
            batchActions: false,
            //文章的Store需要支持远程排序和搜索
            remoteFilter: true,
            remoteSort: true,
            wfKeyword: '',//定义流程Keyword
            btntext: '',//记录按下了哪个按钮
            doclist: '',//记录选择的文档列表
            //data: [["普通用户"], ["系统管理员"]],
            //每50条记录为一页
            pageSize: 50,//视图路径：\simplecdms\scripts\app\view\content\view.js
            proxy: {
                type: "ajax",
                //url: "WorkFlow/GetWorkFlow",//调用路径：\simplecdms\controllers\projectcontroller.cs
                url: 'WebApi/Get',
                extraParams: {
                    C: "AVEVA.CDMS.WebApi.WorkFlowController", A: "GetWorkFlow"
                },
                reader: {
                    type: 'json',
                    totalProperty: 'total',
                    root: "data",
                    messageProperty: "Msg"
                },
                writer: {
                    type: "json",
                    encode: true,
                    root: "data",
                    allowSingle: false
                }
            }
        });

        me.WorkFlowPagesStore = Ext.create("Ext.data.Store", {
            model: 'CDMSWap.model.WorkFlowPage',//模型路径：\simplecdms\scripts\app\model\Property.js
        });

        //定义校审意见按钮
        me._contentAuditsTbar = Ext.create('Ext.toolbar.Toolbar', {
            xtype: 'toolbar',
            dock: 'top',
            items: [

            ]
            //, id: '_contentAuditsTbar'
        });

        me.AddAuditMenu=Ext.create('Ext.menu.Menu', {
            items: []
        });

        //意见combo
        me.unitdata = [{ text: "全部意见", value: "全部意见" }, { text: "校审意见", value: "校审意见" }, { text: "批准意见", value: "批准意见" }];
        //添加机组combo
        Ext.define("unitModel", {
            extend: 'Ext.data.Model',
            fields: ["text", "value"]
        });
        me.unitProxy = Ext.create("Ext.data.proxy.Memory", {
            data: me.unitdata,
            model: "unitModel"
        });

        me.unitStore = Ext.create("Ext.data.Store", {
            model: unitModel,
            proxy: me.unitProxy
        });
        me.unitCombo = Ext.create("Ext.form.field.ComboBox",
      {
          xtype: "combo",
          triggerAction: "all", store: me.unitStore,
          valueField: 'value', editable: false,//不可输入
          displayField: 'text', hideLabel: true,
          emptyText: "请选择", autoLoadOnValue: true,
          anchor: "80%", labelAlign: "left", margin: '0 50 0 0', width: 100//, margins: "8"
      });

        //定义grid意见列表
        me._contentAuditsGrid = Ext.widget("grid", {


            //title: '校审意见',
            xtype: 'grid',
            layout: 'fit',
            width: '100%',
            height: '100%',
            store: me._contentAuditsStore,//"Audits",
            autoscroll:true,
            //Grid使用了复选框作为选择行的方式
            columns: [
                {
                    text: '处理人', dataIndex: 'UserName', width: 100, renderer: function (value, metaData, record) {
                        //添加提示
                        if (null != value) {
                            metaData.tdAttr = 'data-qtip="' + value + '"';
                            return value;
                        } else {
                            return null;
                        }
                    }
                },
                { text: '处理时间', dataIndex: 'ProcTime', width: 80 },
                {
                    text: '意见', dataIndex: 'ProcAudit', flex: 1, renderer: function (value, metaData, record) {
                        //添加提示
                        if (null != value) {
                            metaData.tdAttr = 'data-qtip="' + value + '"';
                            return value;
                        } else {
                            return null;
                        }
                    }
                },
                {
                    text: '修改意见', dataIndex: 'DeProcAudit', flex: 1,
                    renderer: function (value, metaData, record) {
                        if (null != value) {
                            metaData.tdAttr = 'data-qtip="' + value + '"';
                            return value;
                        } else {
                            return null;
                        }
                    }
                },
                { text: '工作状态', dataIndex: 'WorkState', width: 80 }
            ],
            stripeRows: true,// height: 160,
            viewConfig: {
                getRowClass: function (record, rowIndex, p, store) {//CSS class name to add to the row.获得一行的css样式  
                    if (store.getAt(rowIndex).get('Visible') !== "True")
                        return "hide-store-row";
                }
            },
            listeners: {
                "itemdblclick": function (view, record, item, index, e, eOpts) {
                    //  me.onAuditsGridItemDbClick(view, record, item, index, e, eOpts);
                },
                "celldblclick": function (table, td, cellIndex, record, tr, rowIndex, e, eOpts) {
                   // me.onAuditsGridCellDbClick(table, td, cellIndex, record, tr, rowIndex, e, eOpts);
                },
                "cellclick": function (table, td, cellIndex, record, tr, rowIndex, e, eOpts) {
                    me.onAuditsGridCellClick(table, td, cellIndex, record, tr, rowIndex, e, eOpts);
                }
            }
        });

        //定义校审意见子TAB页面
        me._contentAuditsPanel = Ext.create('Ext.panel.Panel', {
            title: '校审意见',
            //id: '_contentAuditsPanel',
            xtype: "panel",
            layout: {
                type: 'vbox',
                align: 'stretch',
                pack: 'start'
            },
            border: false, margin: '0 0 0 0',
            //height: '100%', //height: 150,
            flex:1,
            dockedItems: [me._contentAuditsTbar],
            items: [
            {
                layout: {
                    type: 'vbox',
                    align: 'stretch',
                    pack: 'start'
                },
                //layout:'fit',
                width: '100%', baseCls: 'my-panel-no-border',//隐藏边框
                align: 'stretch', margin: '2 2 2 2', padding: '0 0 0 0',
                pack: 'start', height: '100%',//height: 110,
                items: [
                                       me._contentAuditsGrid
                //    ,
                //{
                //    layout: "hbox",
                //    width: '100%', baseCls: 'my-panel-no-border',//隐藏边框
                //    align: 'stretch', margin: '8 0 8 0', padding: '0 0 0 0',
                //    pack: 'start',
                //    items: [{
                //        xtype: "label",
                //        text: "意见类型：", margins: "2 0 0 3"
                //    }, me.unitCombo,
                //    {
                //        xtype: "button",
                //        text: "打开Excel填写意见并导入", width: 160, margins: "0 0 0 5",
                //        listeners: {
                //            "click": function (btn, e, eOpts) {//添加点击按钮事件
                //                // winA1.close();
                //            }
                //        }
                //    },
                //    {
                //        xtype: "button",
                //        text: "导入校审意见", width: 90, margins: "0 0 0 5",
                //        listeners: {
                //            "click": function (btn, e, eOpts) {//添加点击按钮事件
                //                // winA1.close();
                //            }
                //        }
                //    }]
                //}
                ]
            },/* {
                            xtype: "label",
                            text: "请审核。", margins: "3 0 0 3"
                        }*/
            ]
        });

        //添加代码text
        me._ContentPropertyCodeTxt = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            //width: 225,
            margins: "8 0 8 0",
            fieldLabel: "代码",
            readOnly: true,
            flex: 1
        });

        //添加描述text
        me._ContentPropertyDescTxt = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            //width: 225,
            margins: "8 0 8 0",
            fieldLabel: "描述",
            readOnly: true,
            flex: 1
        });

        //属性子TAB页面
        me._contentAttribPanel = Ext.create('Ext.panel.Panel', {
            title: '属性',
            xtype: "panel",
            layout: {
                type: 'vbox',
                pack: 'start',
                align: 'stretch'
            }, //margins: "8 0 8 0",
            items: [
                {
                    //xtype: "fieldset",
                    xtype: "panel",
                    //title: "流程基本信息",
                    layout: {
                        type: 'hbox',
                        pack: 'start',
                        align: 'stretch'
                    },
                    defaults: {
                        labelWidth: 40,
                        labelAlign: "right"
                    },
                    margins: "8 0 8 0",
                    items: [
                        me._ContentPropertyCodeTxt,
                        me._ContentPropertyDescTxt
                    ]
                }, {
                    xtype: "fieldset",
                    title: "处于流程下的文档列表",
                    layout: 'fit',
                    items: [{
                        title: '',
                        xtype: 'grid',
                        layout: 'fit',
                        store: me.WorkFlowPagesStore,//"WorkFlowPages",
                        //Grid使用了复选框作为选择行的方式
                        columns: [
                            { text: '代码', dataIndex: 'code', width: 110 },
                            { text: '描述', dataIndex: 'desc', width: 250 },
                            { text: '文件名', dataIndex: 'o_filename', width: 100 },
                            { text: '大小', dataIndex: 'o_size', width: 100 },
                            { text: '创建时间', dataIndex: 'o_credatetime', width: 100 },
                            { text: '更新时间', dataIndex: 'o_updatetime', width: 100 },
                            { text: '状态', dataIndex: 'o_status', width: 100 }
                        ],
                        stripeRows: true, height: 120,
                        viewConfig: {
                            getRowClass: function (record, rowIndex, p, store) {//CSS class name to add to the row.获得一行的css样式  
                                if (store.getAt(rowIndex).get('PageId') !== "1")
                                    return "hide-store-row";
                            }
                        }
                    }]
                }]
        });

        //流程子TAB页面
        me._contentWorkflowGrid = Ext.create('Ext.grid.Panel', {
            title: '流程',
            xtype: 'grid',
            layout: 'fit',
            store: me.WorkFlowPagesStore,//"WorkFlowPages",
            //Grid使用了复选框作为选择行的方式
            columns: [
                { text: '工作状态', dataIndex: 'WorkState', flex: 1 },
                { text: '处理人', dataIndex: 'UserName', flex: 1 },
                { text: '提交时间', dataIndex: 'SendDate', width: 100 }
            ],
            stripeRows: true, height: 160,
            viewConfig: {
                getRowClass: function (record, rowIndex, p, store) {//CSS class name to add to the row.获得一行的css样式  
                    if (store.getAt(rowIndex).get('PageId') !== "2")
                        return "hide-store-row";
                }
            }
        });

        //历史子TAB页面
        me._contentHistoryPanel = Ext.create('Ext.grid.Panel', {
            title: '历史',
            xtype: 'grid',
            layout: 'fit',
            store: me.WorkFlowPagesStore,//"WorkFlowPages",
            //Grid使用了复选框作为选择行的方式
            columns: [
                { text: '用户', dataIndex: 'UserName', width: 150 },
                { text: '操作', dataIndex: 'WorkState', flex:1 },
                { text: '时间', dataIndex: 'SendDate', width: 100 }
            ],
            stripeRows: true, height: 160,
            viewConfig: {
                getRowClass: function (record, rowIndex, p, store) {//CSS class name to add to the row.获得一行的css样式  
                    if (store.getAt(rowIndex).get('PageId') !== "3")
                        return "hide-store-row";
                }
            }
        });

        //添加流程TAB页面
        me.workflowtab = Ext.create('Ext.tab.Panel', {
            xtype: 'tabpanel',
            //id: 'workflowtab',
            activeTab: 0,

            layout: 'fit', split: true,// collapsible: true,
            //height:170,
            height: '100%',
            defaults: {
                //xtype: "panel",
                border: false,
                bodyPadding: 0, bodyStyle: "background:#DFE9F6"
            },
            items: [
                me._contentAuditsPanel,
                me._contentAttribPanel,
                me._contentWorkflowGrid,
                me._contentHistoryPanel
            ],
            listeners: {
                "tabchange": function (tabBar, newTab, oldTab) {//添加点击按钮事件
                    me.onWorkFlowTabChange(tabBar, newTab, oldTab);
                }
                
            }
        });


        //添加流程TAB页面到容器
        me.items = [
            			//{
            			//    xtype:"button",
            			//    text:"我的按钮"
            			//}
                               me.workflowtab
        ];

        me.callParent(arguments);
    },

    ////修改意见按钮点击事件
    //OnAuditGridItemBtnClick: function (grid, rowIndex, colIndex) {
    //    //me.OnAuditGridItemBtnClick(grid, rowIndex, colIndex);
    //    var me = this;
    //    var node = grid.store.getAt(rowIndex);
    //    //Ext.Msg.alert("错误信息", "按下了修改模板！");

    //    //if (node.data.AttrName === "文件名") {
    //    //}


    //    var ModiAuditDetail = Ext.create('Ext.ux.YDForm._ModiAuditDetail', { title: "" });

    //    _winModiAuditDetail = Ext.widget('window', {
    //        title: '填写意见',
    //        closeAction: 'hide',
    //        width: 690,
    //        height: 440,
    //        minWidth: 690,
    //        minHeight: 440,
    //        layout: 'fit',
    //        resizable: true,
    //        modal: true,
    //        closeAction: 'close', //close 关闭  hide  隐藏  
    //        items: ModiAuditDetail,
    //        defaultFocus: 'firstName'
    //    });
    //    window.parent.resultvalue = "";
    //    _winModiAuditDetail.show();
    //    //监听子窗口关闭事件
    //    _winModiAuditDetail.on('close', function () {
    //    });
    //    ModiAuditDetail.mainAuditGridId = grid.id;
    //    ModiAuditDetail.loadWin(me.objKeywordType, me.objKeyword, node.data.ProcAudit, node.data.DeProcAudit, node.data.UserName, node.data.WorkState, node.data.ProcTime);

    //},

    //加载流程意见页,参数：keywordType：keyword的类型，可以选择"Doc","WorkFlow"或者"Project",Keyword:关键字
    loadWorkflowAuditPage: function (keywordType, Keyword) {
        var me = this;
        me.objKeywordType = keywordType;
        me.objKeyword = Keyword;
        var storeAudit = me._contentAuditsStore;//me.getAuditsStore();//路径：\simplecdms\scripts\app\store\Audits.js
        storeAudit.proxy.extraParams.KeywordType = keywordType;
        storeAudit.proxy.extraParams.Keyword = Keyword;//把参数传给C#MVC,路径：\simplecdms\controllers\Doccontroller.cs 下的 GetWorkFlow()
        storeAudit.proxy.extraParams.sid = localStorage.getItem("sid");
        storeAudit.load({
            callback: function (records, options, success) {
                //向上级控件获取类别为_mainAttrTab的控件
                var tab = me.up('_mainAttrTab');

                //当文档没有流程时，隐藏TAB页
                if (success === true) {
                    if (records[0] !== null && records[0] !== undefined) {
                        if (tab != undefined)
                            tab.showWorkflowTab();
                        //tab.setAttrTabDisplay('showWorkflow');

                        //遍历store对象
                        //添加动态按钮
                        var btnCount = 0;
                        var btnIndex = 0, rebackIndex = 0;
                        var rebacktext = "";
                        var rebackenable = "F";
                        var addAduittext = "";
                        //var btnText = new Array()
                        //添加校审意见按钮
                        var tbar = me._contentAuditsTbar;  //Ext.getCmp('_contentAuditsTbar');
                        tbar.removeAll();
                        //获取流程关键字
                        var wfKeyword = "";
                        for (var i = 0; i < storeAudit.getCount() ; i++) {
                            if (storeAudit.getAt(i).get('WfKeyword') !== null && storeAudit.getAt(i).get('WfKeyword')) {
                                wfKeyword = storeAudit.getAt(i).get('WfKeyword');
                                storeAudit.wfKeyword = wfKeyword;
                                var isFinish = storeAudit.getAt(i).get('isFinish');
                                storeAudit.isWfFinish = isFinish;
                                if (tab != undefined)
                                    tab.setWorkFlowTabTitle(isFinish);
                                //break;
                            }
                        }
                        for (var i = 0; i < storeAudit.getCount() ; i++) {
                            var BtnType = storeAudit.getAt(i).get('BtnType');
                            if (BtnType !== null && BtnType !== undefined && BtnType !== "") //遍历每一行
                            {

                                if (BtnType === "btn") {

                                    var text = storeAudit.getAt(i).get('Desc');
                                    var enabled = storeAudit.getAt(i).get('Enabled');
                                    var disabled = false;
                                    if (enabled === "F")
                                        disabled = true;
                                    tbar.add([{
                                        text: text,
                                        disabled: disabled,
                                        listeners: {
                                            "click": function (btn, e, eOpts) {
                                                //跳转到下一状态
                                                Ext.require('Ext.ux.Common.m.comm', function () {
                                                    GotoNextWorkflowState(btn.text, storeAudit.wfKeyword, "", function (res) {
                                                        //回调函数

                                                        if (res === undefined)
                                                        {
                                                            return;
                                                        }
                                                        //if (res.data[0].success === true) {
                                                        if (res.data[0].state==="Pass"){
                                                            //通过流程分支
                                                            //var wfKeyword = storeAudit.wfKeyword;
                                                            me.loadWorkflowAuditPage("WorkFlow", wfKeyword);
                                                        } else if (res.data[0].state === "RunFunc") {



                                                            //没有通过流程分支，返回让用户插件处理
                                                            var plugins = res.data[0].plugins;
                                                            var DefWorkFlow = res.data[0].DefWorkFlow;
                                                            var CuWorkState = res.data[0].CuWorkState;
                                                            var FuncName = res.data[0].FuncName;

                                                            

                                                            if (plugins != "" && DefWorkFlow != "" && CuWorkState != "" && FuncName != "")
                                                            {
                                                                Ext.require('Ext.plug_ins.m.' + plugins + '.common', function () {
                                                                    //通过函数名调用函数
                                                                    function runCommFun(mainPanelId, res) {


                                                                        //state是需要调用的函数名
                                                                        var CommFun = eval(FuncName);
                                                                        new CommFun(mainPanelId, res);

                                                                    }
                                                                    runCommFun(me.id, res);
                                                                });

                                                            }
                                                             else {
                                                                //var winCreateA9 = Ext.create('Ext.plug_ins.'+plugins+'.winCreateA9', { title: "", mainPanelId: mainPanelId });
                                                                Ext.Msg.alert("错误信息", "流程提交预处理错误！错误消息："+plugins + "," + DefWorkFlow + "," + CuWorkState + "," + state);
                                                            }
                                                        }
                                                    });
                                                })

                                            }
                                        }
                                    }, '-']);
                                    //btn.setVisible(true);

                                    btnCount = btnCount + 1;
                                    btnIndex = btnIndex + 1;

                                }
                                else if (BtnType === "reback") {
                                    rebacktext = storeAudit.getAt(i).get('Desc');
                                    rebackenable = storeAudit.getAt(i).get('Enabled');
                                    rebackIndex = i;
                                }
                                else if (BtnType === "addAduit") {
                                    addAduittext = storeAudit.getAt(i).get('Desc');
                                    //rebackIndex = i;
                                }
                            }

                        }

                        if (addAduittext !== "") {
                            //清空下拉意见选项
                            me.AddAuditMenu = Ext.create('Ext.menu.Menu', {
                                items: [{
                                    text: "输入意见",
                                    listeners: {
                                        "click": function (btn, e, eOpts) {//添加点击按钮事件
                                            //me.showModiAuditDetail();
                                            me.onAddAuditMenuItemClick();
                                        }
                                    }
                                }, "-"]
                            });
                            //再添加默认意见列表选项
                            var strs= new Array(); //定义一数组
                            strs = addAduittext.split(";"); //字符分割
                            for (var j = 0; j < strs.length ; j++) {
                                me.AddAuditMenu.add([{
                                    text: strs[j],
                                    listeners: {
                                        "click": function (btn, e, eOpts) {//添加点击按钮事件
                                            me.onAuditMenuItemClick(btn.text);
                                        }
                                    }
                                }]);
                            }
                            tbar.add(['->',
                                {
                                    text: '添加意见',
                                    //arrowAlign: 'bottom',
                                    arrowAlign: 'right',
                                    menu: me.AddAuditMenu
                                }//, '-'
                            ]);
                        }
                        if (rebacktext !== "") {
                            var disabled = true;
                            if (rebackenable === "T")
                                disabled = false;
                            tbar.add(['->',
                                { //添加撤回修改或删除流程按钮
                                    text: storeAudit.getAt(rebackIndex).get('Desc'),
                                    disabled: disabled,
                                    listeners: {
                                        "click": function (btn, e, eOpts) {
                                            if (btn.text === "撤销提交") {
                                                Ext.MessageBox.show({
                                                    title: '确认',
                                                    msg: '是否撤销提交？',
                                                    buttons: Ext.MessageBox.YESNO,
                                                    //parentFuctionName: parentFuctionName,
                                                    //docKeyword: docKeyword,
                                                    buttonText: {
                                                        yes: "是",
                                                        no: "否"
                                                    },
                                                    fn: function (btn, parentFuctionName) {
                                                        if (btn === "yes") {
                                                            var ObjKeyword = me.objKeyword;
                                                            Ext.Ajax.request({
                                                                url: 'WebApi/Post',
                                                                method: "POST",
                                                                params: {
                                                                    C: "AVEVA.CDMS.WebApi.WorkFlowController", A: "RebackWorkFlow",
                                                                    Keyword: ObjKeyword, 
                                                                    sid: localStorage.getItem("sid")
                                                                },
                                                                success: function (response, options) {
                                                                    var res = Ext.JSON.decode(response.responseText, true);
                                                                    var state = res.success;
                                                                    if (state === false) {
                                                                        var errmsg = res.msg;
                                                                        Ext.Msg.alert("错误信息", errmsg);
                                                                    }
                                                                    else {
                                                                        me.loadWorkflowAuditPage(me.objKeywordType, me.objKeyword);
                                                                    }
                                                                }
                                                            });
                                                        }
                                                    }
                                                })
                                            } else if (btn.text === "删除流程") {
                                                Ext.MessageBox.show({
                                                    title: '确认',
                                                    msg: '是否删除流程？',
                                                    buttons: Ext.MessageBox.YESNO,
                                                    //parentFuctionName: parentFuctionName,
                                                    //docKeyword: docKeyword,
                                                    buttonText: {
                                                        yes: "是",
                                                        no: "否"
                                                    },
                                                    fn: function (btn, parentFuctionName) {
                                                        if (btn === "yes") {
                                                            var ObjKeyword = me.objKeyword;
                                                            Ext.Ajax.request({
                                                                url: 'WebApi/Post',
                                                                method: "POST",
                                                                params: {
                                                                    C: "AVEVA.CDMS.WebApi.WorkFlowController", A: "DeleteWorkFlow",
                                                                    Keyword: ObjKeyword,
                                                                    sid: localStorage.getItem("sid")
                                                                },
                                                                success: function (response, options) {
                                                                    var res = Ext.JSON.decode(response.responseText, true);
                                                                    var state = res.success;
                                                                    if (state === false) {
                                                                        var errmsg = res.msg;
                                                                        Ext.Msg.alert("错误信息", errmsg);
                                                                    }
                                                                    else {
                                                                        me.loadWorkflowAuditPage(me.objKeywordType, me.objKeyword);
                                                                    }
                                                                }
                                                            });
                                                        }
                                                    }
                                                })
                                            }
                                            ////跳转到下一状态
                                            //Ext.require('Ext.ux.Common.m.comm', function () {
                                            //    GotoNextWorkflowState(btn.text, storeAudit.wfKeyword, "", function () {
                                            //        //回调函数，通过流程分支
                                            //        //var wfKeyword = storeAudit.wfKeyword;
                                            //        me.loadWorkflowAuditPage("WorkFlow", wfKeyword);
                                            //    });
                                            //})

                                        }
                                    }
                                }]);
                        }
                    }
                    else {
                        if (tab != undefined)
                            tab.hideWorkflowTab();
                            //tab.setAttrTabDisplay('hideWorkflow');
                        
                    }

                    //var wftab = Ext.getCmp('workflowtab');
                    //wftab.setActiveTab(0);
                    me.workflowtab.setActiveTab(0);

                }
            }
        });
    },

    //流程TAB切换事件
    onWorkFlowTabChange: function (tabBar, newTab, oldTab) {
        var me = this;

        var TabTitle = newTab.title;
        if (TabTitle === "属性" || TabTitle === "流程" || TabTitle === "流程(完成)" || TabTitle === "历史") {

            if (me.objKeywordType!="" && me.objKeyword!=""){
                var storeWorkFlow = me.WorkFlowPagesStore;

                storeWorkFlow.proxy.extraParams.KeywordType = me.objKeywordType;
                storeWorkFlow.proxy.extraParams.Keyword = me.objKeyword;//把参数传给C#MVC,路径：\simplecdms\controllers\Doccontroller.cs 下的 GetWorkFlow()
                storeWorkFlow.proxy.extraParams.sid = localStorage.getItem("sid");
                storeWorkFlow.load({
                    callback: function (records, options, success) {
                        //显示流程模板代码和描述
                        if (records[0] !== null && records[0] !== undefined) {
                            for (var i = 0; i < storeWorkFlow.getCount() ; i++) {
                                var desc = storeWorkFlow.getAt(i).get('desc');
                                if (desc !== null && desc !== undefined) //遍历每一行
                                {
                                    if (desc === "DefWorkFlowCode") {
                                        me._ContentPropertyCodeTxt.setValue(storeWorkFlow.getAt(i).get('WorkState'));
                                    }
                                    else if (desc === "DefWorkFlowDesc") {
                                        me._ContentPropertyDescTxt.setValue(storeWorkFlow.getAt(i).get('WorkState'));
                                    }
                                }
                            }
                        }

                    }
                });

            }
        }
    },

    //添加意见菜单点击事件，添加流程意见
    onAuditMenuItemClick: function (auditeDesc) {
        var me = this;
        var ObjKeyword = me.objKeyword;
        Ext.Ajax.request({
            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.WebApi.WorkFlowController", A: "AddWorkflowAudit",
                Keyword: ObjKeyword, ProcAudit: auditeDesc,//me.objKeyword, ProcAudit: auditeDesc,
                sid: localStorage.getItem("sid")
            },
            success: function (response, options) {
                var res = Ext.JSON.decode(response.responseText, true);
                var state = res.success;
                if (state === false) {
                    var errmsg = res.msg;
                    Ext.Msg.alert("错误信息", errmsg);
                }
                else {
                    me.loadWorkflowAuditPage(me.objKeywordType, me.objKeyword);
                }
            }
        });
    },

    //双击意见表格单元格事件
    onAuditsGridCellClick: function (table, td, cellIndex, record, tr, rowIndex, e, eOpts) {
        var me = this;
        var AuditType = "";

        //填写意见
        if (cellIndex === 2) {
            AuditType = "ProcAudit";
        }
            //修改意见
        else if (cellIndex === 3) {
            AuditType = "DeProcAudit";
        } else {
            return;
        }

        var node = me._contentAuditsGrid.store.getAt(rowIndex);

        var ModiAuditDetail = Ext.create('Ext.ux.m.WorkFlow._ModiAuditDetail', { title: "", AuditType: AuditType, AuditRight: node.data.AuditRight });

        me.showModiAuditDetail(ModiAuditDetail);

        var AuditContent = "";

        //如果是设计人填写修改意见，就把所有意见列出到意见栏
        if (node.data.AuditRight === "2") {
            var store = table.getStore();

            for (var i = 0; i < store.getCount() ; i++) {
                var itemData = store.getAt(i).data;

                if (itemData.ProcAudit != "" && itemData.ProcAudit != undefined) {
                    AuditContent = AuditContent + itemData.WorkState + " " + itemData.UserName + " 意见：" + "\r\n";
                    AuditContent = AuditContent + itemData.ProcAudit + "\r\n";

                    if (itemData.DeProcAudit != "" && itemData.DeProcAudit != undefined) {
                        AuditContent = AuditContent + "修改意见：" + "\r\n";
                        AuditContent = AuditContent + itemData.DeProcAudit + "\r\n";
                    }

                    AuditContent = AuditContent + "\r\n";
                }
            }
        } else {
            AuditContent = node.data.ProcAudit;
        }

        ModiAuditDetail.mainAuditGridId = me._contentAuditsGrid.id;
        //loWin(流程Keyword, 流程状态Keyword, 审核人Keyword, 审核意见签署时间, 意见，修改意见，意见类型)
        ModiAuditDetail.loadWin(me.objKeyword, node.data.KeyWord, node.data.UserKeyword, node.data.ProcTime, AuditContent, node.data.DeProcAudit, node.data.AuditRight, AuditType);

    },

    //响应流程按钮菜单添加意见按钮，输入意见选项单击事件
    onAddAuditMenuItemClick: function () {
        var me = this;

        var ModiAuditDetail = Ext.create('Ext.ux.m.WorkFlow._ModiAuditDetail', { title: "", AuditType: "NewProcAudit" });

        me.showModiAuditDetail(ModiAuditDetail);

        ModiAuditDetail.mainAuditGridId = me._contentAuditsGrid.id;
        //loWin(流程Keyword, 流程状态Keyword, 审核人Keyword, 审核意见签署时间, 意见，修改意见，意见类型)
        ModiAuditDetail.loadWin(me.objKeyword, "", "", "", "", "", "", "NewProcAudit");
    },

    //显示填写意见窗口
    showModiAuditDetail: function (ModiAuditDetail) {
        var me = this;

        _winModiAuditDetail = Ext.widget('window', {
            title: '填写意见',
            closeAction: 'hide',
            width: '90%',
            height: '90%',
            minWidth: '90%',
            minHeight: '90%',
            layout: 'fit',
            resizable: true,
            modal: true,
            closeAction: 'close', //close 关闭  hide  隐藏  
            items: ModiAuditDetail,
            defaultFocus: 'firstName'
        });
        window.parent.resultvalue = "";
        _winModiAuditDetail.show();

        //监听子窗口关闭事件
        _winModiAuditDetail.on('close', function () {
        });
    },

    //刷新父控件内容，父控件的类型有项目数据源和消息两种分类
    refreshMainPanle: function (projKeyword, callbackFun) {
        var me = this;

        //如果是项目数据类型
        if (me.mainPanelType === "Project") {
            var tree = me.up('_mainProjectView').down('_mainProjectTree').down('treepanel');//;
            var viewTreeStore = tree.store;

            viewTreeStore.load({
                callback: function (records, options, success) {//添加回调，获取子目录的文件数量

                    callbackFun();

                    //展开目录
                    Ext.require('Ext.ux.Common.m.comm', function () {
                        Ext.getCmp('contentPanel').down('_mainProjectTree').ExpendProject(projKeyword);
                    });
                }
            });

        } else {
            var gridView = me.up("_mainMessageView").down("_mainMessageGrid");

            gridView.refreshMessagePage();


            callbackFun();
        }
    }


});