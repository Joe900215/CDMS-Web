Ext.define('Ext.ux.YDForm._StringSelector', {
    extend: 'Ext.container.Container',
    alias: 'widget._StringSelector',
    layout: 'fit',
    resultvalue: '',
    selectAllUser: "",

    initComponent: function () {
        var me = this;
        me.renderTo = me.el;
        var resultvalue = '';

        //是否显示子用户组的用户
        var A = "GetAllUserList";
        if (me.selectAllUser === "false")
            A = "GetUserList";

        //定义未选择用户的model
        Ext.define("_UserSelection", {
            extend: "Ext.data.Model",
            fields: ["text", "id"],
            proxy: {
                type: "ajax",
                url: 'WebApi/Get',
                extraParams: {
                    C: "AVEVA.CDMS.WebApi.UserController", A: A,
                    KeyWord: 1, sid: localStorage.getItem("sid")
                },
                reader: {
                    type: 'json',
                    totalProperty: 'total',
                    root: "data",//从C#MVC获取数据\simplecdms\controllers\ProjectController.cs .GetDocList.data  ，获取到的数据传送到model里面
                    messageProperty: "Msg"
                },
                writer: {
                    type: "json",
                    encode: true,
                    root: "data",
                    allowSingle: false
                },
                listeners: {
                    exception: CDMSWeb.ProxyException
                }
            }

        });

        //每一个列都会出现鼠标悬浮上去显示内容
        /** 
         * //适用于Extjs4.x
        * @class Ext.grid.GridView 
        * @override Ext.grid.GridView 
        * GridPanel单元格不能选中复制问题 
        * 单元格数据显示不完整 ,增加title 浮动提示信息 
        */
        Ext.override(Ext.grid.GridPanel, {
            afterRender: Ext.Function.createSequence(Ext.grid.GridPanel.prototype.afterRender,
                function () {
                    // 默认显示提示
                    if (!this.cellTip) {
                        return;
                    }

                    var view = this.getView();
                    this.tip = new Ext.ToolTip({
                        target: view.el,
                        delegate: '.x-grid-cell-inner',
                        trackMouse: true,
                        renderTo: document.body,
                        listeners: {
                            beforeshow: function updateTipBody(tip) {
                                //取cell的值
                                //fireFox  tip.triggerElement.textContent
                                //IE  tip.triggerElement.innerText 
                                var tipText = ("双击选择");//tip.triggerElement.innerText || tip.triggerElement.textContent);
                                if (Ext.isEmpty(tipText) || Ext.isEmpty(tipText.trim())) {
                                    return false;
                                }
                                tip.update(tipText);
                            }
                        }
                    });
                })
        });


        //定义未选择用户的store
        me._seluserstore = Ext.create("Ext.data.Store", {
            batchActions: false,
            //文章的Store需要支持远程排序和搜索
            remoteFilter: true,
            remoteSort: true,
            //每50条记录为一页
            pageSize: 50,//视图路径：\simplecdms\scripts\app\view\content\view.js
            model: "_UserSelection"

        });


        //定义未选择用户tab的view
        me.usergrid = Ext.widget("grid", {
            region: "center",
            //height: '100%',//415,
            cellTip: true,
             store: me._seluserstore,
            //无限滚动需要//
            verticalScroller: {
                xtype: 'paginggridscroller'
            },
            //    //Grid使用了复选框作为选择行的方式
            selType: "checkboxmodel",
            selModel: { checkOnly: false, mode: "MULTI" },
            bbar: new Ext.PagingToolbar({
                //pageSize: 5,//pageSize,  
                store: me._seluserstore,
                displayInfo: true,
                displayMsg: '当前显示{0} - {1}条，共{2}条数据',
                emptyMsg: "没有记录"
            }),

            columns: [
                //{ text: '用户代码', dataIndex: 'Sender', width: 120 },
                { text: '用户名', dataIndex: 'text', width: '100%' }
            ],
            listeners: {
                'itemdblclick': function (view, record, item, index, e) {
                    //双击时，插入记录到已选择grid
                    if (typeof (record.data) != 'undefined') {
                        var Keyword = record.data.id;
                        var Text = record.data.text;

                        var flag = true;
                        var resultstore = me.resultgrid.getStore();
                        for (var i = 0; i < resultstore.getCount() ; i++) {//遍历每一行
                            if (resultstore.getAt(i).data.id === Keyword) {
                                flag = false;
                                break;
                            }
                        }

                        if (flag === true) {
                            //插入行到返回grid
                            var r = Ext.create('_UserResult', {
                                id: Keyword,
                                text: Text
                            });


                            var rowlength = me.resultgrid.getStore().data.length;
                            me.resultgrid.getStore().insert(rowlength, r);
                        }

                    }
                }
            },flex:1
        });

        //定义未选择用户的store
        me._groupselstore = Ext.create("Ext.data.Store", {
            batchActions: false,
            //文章的Store需要支持远程排序和搜索
            remoteFilter: true,
            remoteSort: true,
            //每50条记录为一页
            pageSize: 50,//视图路径：\simplecdms\scripts\app\view\content\view.js
            model: "_UserSelection"

        });

        //定义未选择用户tab的view
        me.groupgrid = Ext.widget("grid", {
            region: "center",
            cellTip: true,
            store: me._groupselstore,
            //Grid使用了复选框作为选择行的方式
            selType: "checkboxmodel",
            selModel: { checkOnly: false, mode: "MULTI" },
            bbar: new Ext.PagingToolbar({
                store: me._groupselstore,
                displayInfo: true,
                displayMsg: '当前显示{0} - {1}条，共{2}条数据',
                emptyMsg: "没有记录"
            }),

            columns: [
                { text: '用户名', dataIndex: 'text', width: '100%' }
            ],
            listeners: {
                'itemdblclick': function (view, record, item, index, e) {
                    //双击时，插入记录到已选择grid
                    if (typeof (record.data) != 'undefined') {
                        var Keyword = record.data.id;
                        var Text = record.data.text;

                        var flag = true;
                        var resultstore = me.resultgrid.getStore();
                        for (var i = 0; i < resultstore.getCount() ; i++) {
                            if (resultstore.getAt(i).data.id === Keyword) {
                                flag = false;
                                break;
                            }
                        }

                        if (flag === true) {
                            //插入行到返回grid
                            var r = Ext.create('_UserResult', {
                                id: Keyword,
                                text: Text
                            });


                            var rowlength = me.resultgrid.getStore().data.length;
                            me.resultgrid.getStore().insert(rowlength, r);
                        }

                    }
                }
            }, flex: 1
        });

        //定义已选择用户的model
        Ext.define("_UserResult", {
            extend: "Ext.data.Model",
            fields: ["text", "id"],
            url:"_blank",
        });

        //定义已选择用户的store
        me._userresultstore = Ext.create("Ext.data.Store", {
            batchActions: false,
            //文章的Store需要支持远程排序和搜索
            remoteFilter: true,
            remoteSort: true,
            //每50条记录为一页
            pageSize: 50,//视图路径：\simplecdms\scripts\app\view\content\view.js
            model: "_UserResult"

        });


        //定义已选择用户的view
        me.resultgrid = Ext.widget("grid", {
            //region: "center",
            //layout: "hbox",
            height: '100%',
            width: "100%",
            cellTip: true,
            store: me._userresultstore,

            columns: [
                //{ text: '用户代码', dataIndex: 'Sender', width: 120 },
                { text: '用户名', dataIndex: 'text', width: '100%' }
            ],
            listeners: {
                //'rowdblclick': function (grid, rowIndex, e) {
                'itemdblclick': function (view, record, item, index, e) {
                    //双击时，从已选择grid删除双击的节点
                    if (typeof (record.data) != 'undefined') {
                        var Keyword = record.data.id;
                        var Text = record.data.text;

                        //删除行
                        var sm = me.resultgrid.getSelectionModel();
                        var store = me.resultgrid.getStore();
                        store.remove(sm.getSelection());
                        if (store.getCount() > 0) {
                            sm.select(0);
                        }
                    }
                }
            },flex:1
        });

        //定义文本输入框
        me.userinputtext = Ext.widget('textfield', {
            name: "Title", width: "100%",
            fieldLabel: "搜索",  anchor: "80%", labelWidth: 30, labelAlign: "left", margin: '5 5 5 5',
            enableKeyEvents: true,
            listeners: {
                //这里不能用change函数，因为刷新GRID的时候有问题
                //change: function (field, newValue, oldValue) {
                keyup: function (src, evt) {
                    me._seluserstore.proxy.extraParams.Filter = src.getValue();//把参数传给C#MVC,路径：\simplecdms\controllers\Doccontroller.cs 下的 GetWorkFlow()
                    me._seluserstore.proxy.extraParams.sid = localStorage.getItem("sid");
                    me._seluserstore.proxy.extraParams.Group = "";//重置机构组
                    me._seluserstore.currentPage = 1;
                    me._seluserstore.load();
                    
                }
            }
            //, allowBlank: false
        })
        //me.on("itemcollapse", me.onTreeExpand, me);

        //定义文本输入框
        me.groupinputtext = Ext.widget('textfield', {
            name: "Title", width: "100%", enableKeyEvents: true,
            fieldLabel: "搜索", anchor: "80%", labelWidth: 30, labelAlign: "left", margin: '5 5 5 5',
            listeners: {
                //这里不能用change函数，因为刷新GRID的时候有问题
                //change: function (field, newValue, oldValue) {
                keyup: function (src, evt) {
                    me._groupselstore.proxy.extraParams.Filter = src.getValue();//把参数传给C#MVC,路径：\simplecdms\controllers\Doccontroller.cs 下的 GetWorkFlow()
                    me._groupselstore.proxy.extraParams.sid = localStorage.getItem("sid");
                    me._groupselstore.currentPage = 1;
                    me._groupselstore.load();

                    //var tbar = grid.getBottomToolbar();
                    //tbar.doLoad(tbar.cursor);
                }
            }
            //, allowBlank: false
        })

        //定义组织机构树的model
        me.grouppickermodel = Ext.define("_GroupPicker",{
            extend: 'Ext.data.Model',
            //parentId用来记录父目录
            fields: [{ name: 'id', type: 'string' },
                { name: 'text', type: 'string' }
            ],

            idProperty: "id"
        });


        me.grouppickerstore = Ext.create("Ext.data.TreeStore", {

            batchActions: false,
            remoteFilter: false,
            remoteSort: false,
            model: me.grouppickermodel,
            root: { id: "Root", text: "所有组织机构", expanded: true },
            ////代理定义
            proxy: {
            type: 'ajax',
                //代理的API定义了操作的提交路径
            //api: {
            //    read: 'User/GetUserGroupList?sid=' + localStorage.getItem("sid"),//调用路径：\simplecdms\controllers\projectcontroller.cs
            //},
            url: "WebApi/Get",//调用路径：\simplecdms\controllers\projectcontroller.cs
            extraParams: {
                C: "AVEVA.CDMS.WebApi.UserController", A: "GetUserGroupList",
                sid: localStorage.getItem("sid")
            },
                //在代理定义中，reader和writer的定义可标准化数据的输入输出，
                //这个与用户中的定义是一样的
            reader: {
                messageProperty: "Msg",
                type: 'json',
                root: "data"
            },
            writer: {
                type: "json",
                encode: true,
                root: "data",
                allowSingle: false
            },
            listeners: {
                exception: CDMSWeb.ProxyException
            }
        },
        });

        //创建树控件  
        me.grouppicker = Ext.create('Ext.tree.Panel', {
            title:'选择组织机构',
            collapsible: true, rootVisible: true,
            store: me.grouppickerstore, split: true,
            height: 150,
            width: "100%", minWidth: 100,
            //root: { id: "Root", text: "所有组织机构"}, //, expanded: true },
            viewConfig: {
                stripeRows: true,
                listeners: {
                    itemcontextmenu: function (view, rec, node, index, e) {
                        e.stopEvent();
                        contextMenu.showAt(e.getXY());
                        return false;
                    }

                }
            },
            listeners: {
                itemclick: function (picker, record) {

                    //var me = this,
                    var   selection = picker.getSelectionModel().getSelection();
                    //var valueField = me.valueField;

                    var strmsg;
                    //Ext.Msg.alert("您展开了目录树节点！！！", "您展开了目录树节点！节点ID:" + selection[0].data.id + "," + selection[0].data.text);
                    me._groupselstore.proxy.extraParams.Filter = me.groupinputtext.getValue();//把参数传给C#MVC,路径：\simplecdms\controllers\Doccontroller.cs 下的 GetWorkFlow()
                    me._groupselstore.proxy.extraParams.Group = selection[0].data.id;
                    me._groupselstore.proxy.extraParams.sid = localStorage.getItem("sid");
                    me._groupselstore.currentPage = 1;
                    me._groupselstore.load();
                }
            }


            //selModel: {
            //    mode: me.multiSelect ? 'SIMPLE' : 'SINGLE'
            //},
         //   floating: true,
            //hidden: true,
         //   focusOnToFront: false
        });

        ////注册事件用于选择用户选择的值  
        //me.mon(me.grouppicker, {
        //    itemclick: me.onItemClick,
        //    refresh: me.onListRefresh,
        //    scope: me
        //});



        //me.UserGroupCombo = Ext.create('Ext.ux.UserSelector.ComboBox.ComboTreeBox', { title: "" });

        //me.UserGroupCombo.getStore().load();
        me.btnPlan = {
            xtype: "panel",
            layout: "hbox",
            height: 40,
            baseCls: 'my-panel-no-border',//隐藏边框
            items: [{
                baseCls: 'my-panel-no-border',//隐藏边框
                flex: 1
            },
            {
                xtype: "button",
                text: "确定", width: 80, margins: "5 5 5 5",
                listeners: {
                    "click": function (btn, e, eOpts) {//添加点击按钮事件
                        //确定按钮事件
                        var strresult, strname;
                        var resultstore = me.resultgrid.getStore();
                        var userArray = new Array();
                        for (var i = 0; i < resultstore.getCount() ; i++) {
                            //strresult = strresult + ";" + resultstore.getAt(i).data.id + "," + resultstore.getAt(i).data.text; //遍历每一行
                            if (i > 0) {
                                strresult = strresult + "," + resultstore.getAt(i).data.id;
                                strname = strname + "," + resultstore.getAt(i).data.text;
                            } else {
                                strresult = resultstore.getAt(i).data.id;
                                strname = resultstore.getAt(i).data.text;
                            }

                        }

                        window.parent.resultvalue = strresult;//functionName(strresult);
                        window.parent.usernamelist = strname;//functionName(strresult);
                        _winUserSelector.close();

                    }
                }
            }, {
                xtype: "button",
                text: "取消", width: 80, margins: "5 5 5 5",
                listeners: {
                    "click": function (btn, e, eOpts) {//添加点击按钮事件
                        _winUserSelector.close();
                    }
                }
            }
            ]
        };

        me.selectTab = {
            xtype: 'tabpanel',
            //layout: "fit",
            layout: "hbox",
            width: 450,
            height:'100%',
            //baseCls: 'my-panel-no-border',//隐藏边框
            items: [
                {
                    xtype: "panel",
                    title: "用户",
                    layout: {
                        type: 'vbox',
                        align: 'stretch',
                        pack: 'start',
                    },
                    baseCls: 'my-panel-no-border',//隐藏边框
                    items: [
                        me.userinputtext,
                me.usergrid
                    //{
                    //    xtype: "panel",
                    //    width: "100%",
                    //    baseCls: 'my-panel-no-border',//隐藏边框
                    //    items: [me.usergrid], flex: 1
                    //}
                    ]
                }
            , {
                xtype: "panel",
                title: "组织机构",
                 layout: {
                    type: 'vbox',
                    align: 'stretch',
                    pack: 'start',
                 },
                 baseCls: 'my-panel-no-border',//隐藏边框
                items: [
                   me.grouppicker,
                   me.groupinputtext,
                   me.groupgrid//Plan
                   // me.usergrid
            ]
            }

            ]
        };
        me.resultgridPlan = {
            xtype: "panel",
            //width: "100%",
            layout: "hbox",
            height: "100%",
            margins: "0 0 0 5",
            items: [me.resultgrid],flex:1
        };

        me.selectPlan = {
            xtype: "panel",
            layout: "hbox",
            height: "100%", margins: "5 5 5 5",
            baseCls: 'my-panel-no-border',//隐藏边框
            items: [me.selectTab, me.resultgridPlan],
            flex: 1
        };

        me.mainSelectPlan = {
            xtype: "panel",
            layout: {
                type: 'vbox',
                align : 'stretch',
                pack  : 'start',
                },

            baseCls: 'my-panel-no-border',//隐藏边框
            items: [me.selectPlan, me.btnPlan]
        };

        //添加列表
        me.items = [me.mainSelectPlan

        ];

        //me.usergrid.setSize(panel.getWidth(), panel.ownerCt.getHeight() - panel.getHeight() - me.userinputtext.getHeight() - 300 - 28);

        me.callParent(arguments);
    },




    functionName :function (strresult){
    //父窗口调用函数
    }
});