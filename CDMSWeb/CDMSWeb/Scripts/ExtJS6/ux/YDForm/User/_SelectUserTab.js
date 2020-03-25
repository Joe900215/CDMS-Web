Ext.define('Ext.ux.YDForm.User._SelectUserTab', {
    //extend: 'Ext.container.Container',
    extend: 'Ext.tab.Panel',//'Ext.panel.Panel',
    alias: 'widget._SelectUserTab', // 此类的xtype类型为buttontransparent  
    selectAllUser: "",
    layout: 'fit',
    layout: "hbox",
    width: 400,
    height: '100%',
    initComponent: function () {
        var me = this;
        me.renderTo = me.el;

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
                { text: '用户名', dataIndex: 'text', width: 350} //'100%' }
            ],
            listeners: {
                'itemdblclick': function (view, record, item, index, e) {
                    ////双击时，插入记录到已选择grid
                    //if (typeof (record.data) != 'undefined') {
                    //    var Keyword = record.data.id;
                    //    var Text = record.data.text;

                    //    var flag = true;
                    //    var resultstore = me.resultgrid.getStore();
                    //    for (var i = 0; i < resultstore.getCount() ; i++) {//遍历每一行
                    //        if (resultstore.getAt(i).data.id === Keyword) {
                    //            flag = false;
                    //            break;
                    //        }
                    //    }

                    //    if (flag === true) {
                    //        //插入行到返回grid
                    //        var r = Ext.create('_UserResult', {
                    //            id: Keyword,
                    //            text: Text
                    //        });


                    //        var rowlength = me.resultgrid.getStore().data.length;
                    //        me.resultgrid.getStore().insert(rowlength, r);
                    //    }

                    //}
                }
            }, flex: 1
        });

         //定义文本输入框
        me.userinputtext = Ext.widget('textfield', {
            name: "Title", width: "100%",
            fieldLabel: "搜索", anchor: "80%", labelWidth: 30, labelAlign: "left", margin: '5 5 5 5',
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
        });

        //定义用户组选择Panel
        me._selectGroupUserPanel = Ext.create('Ext.ux.YDForm.User._SelectUserGroupPanel');
        
        //添加属性TAB页面到容器
        me.items = [
            // me.selectTab
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
            ]
        }, me._selectGroupUserPanel
            //, {
            //    xtype: "panel",
            //    title: "组织机构",
            //    layout: {
            //        type: 'vbox',
            //        align: 'stretch',
            //        pack: 'start',
            //    },
            //    baseCls: 'my-panel-no-border',//隐藏边框
            //    items: [
            //       me.grouppicker,
            //       me.groupinputtext,
            //       me.groupgrid
            //    ]
            //}
        ];

        me._seluserstore.load();

        me.callParent(arguments);
    },
    setActTab: function (tabType, tabPara) {
        var me = this;
        if (tabType === "org" && tabPara != undefined) {
            me.setActiveTab(1);
            me._selectGroupUserPanel.selectGroup(tabPara);
            //me._selectGroupUserPanel.setDefaultGroup(tabPara);
            
        }
    }
});