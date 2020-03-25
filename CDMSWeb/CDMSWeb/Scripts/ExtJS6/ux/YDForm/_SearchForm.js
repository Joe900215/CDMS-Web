Ext.define('Ext.ux.YDForm._SearchForm', {
    extend: 'Ext.container.Container',
    alias: 'widget._SearchForm',
    layout: 'fit',
    resultvalue: '', mainPanelId: '',

    initComponent: function () {
        var me = this;
        me.renderTo = me.el;

        //定义文本输入框
        me.docInputText = Ext.widget('textfield', {
            width: "100%",
            fieldLabel: "搜索", anchor: "80%", labelWidth: 30, labelAlign: "left", margin: '5 5 5 5',
            enableKeyEvents: true,
            listeners: {
                ////这里不能用change函数，因为刷新GRID的时候有问题
                ////change: function (field, newValue, oldValue) {
                //keyup: function (src, evt) {
                //    me._seluserstore.proxy.extraParams.Filter = src.getValue();//把参数传给C#MVC,路径：\simplecdms\controllers\Doccontroller.cs 下的 GetWorkFlow()
                //    me._seluserstore.proxy.extraParams.sid = localStorage.getItem("sid");
                //    me._seluserstore.proxy.extraParams.Group = "";//重置机构组
                //    me._seluserstore.currentPage = 1;
                //    me._seluserstore.load();

                //}
            }
        })

        ////定义文本输入框
        //me.docInputText = Ext.widget('textfield', {
        //    width: "100%",
        //    fieldLabel: "搜索", anchor: "80%", labelWidth: 30, labelAlign: "left", margin: '5 5 5 5',
        //    enableKeyEvents: true,
        //    listeners: {
        //    }
        //})

        //定义文档搜索TAB页
        me.DocTabPage={
            xtype: "panel",
            title: "文档搜索",
            layout: {
                type: 'vbox',
                align: 'stretch',
                pack: 'start',
            },
            baseCls: 'my-panel-no-border',//隐藏边框
            items: [me.docInputText]
        };

        //定义文件夹搜索TAB页
        me.ProjectTabPage = {
            xtype: "panel",
            title: "文件夹搜索",
            layout: {
                type: 'vbox',
                align: 'stretch',
                pack: 'start',
            },
            baseCls: 'my-panel-no-border',//隐藏边框
            items: []
        };

        //定义SQL查询TAB页
        me.SQLTabPage = {
            xtype: "panel",
            title: "SQL查询",
            layout: {
                type: 'vbox',
                align: 'stretch',
                pack: 'start',
            },
            baseCls: 'my-panel-no-border',//隐藏边框
            items: []
        };

        me.SearchTab = {
            xtype: 'tabpanel',
            layout: "hbox",
            width:'100%', 
            //height:'100%',
            flex:1,
            items: [
                me.DocTabPage
                //me.ProjectTabPage,
                //me.SQLTabPage
                ]
        };

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
                        ////确定按钮事件
                        var value = me.docInputText.value;

                        var view = Ext.getCmp(me.mainPanelId).up('_mainSourceView').down('_mainDocGrid')

                        if (value.length > 0) {
                            var strSQL = " (o_itemname " + "LIKE" + " '%" + value + "%' or o_itemdesc " + "LIKE" + " '%" + value + "%' ) ";

                            var filterObj = [
                                { name: 'o_itemname', value: strSQL }
                            ];
                            view.selectDocList(filterObj);
                        }
                        _winSearchProject.close();
                    }
                }
            }, {
                xtype: "button",
                text: "取消", width: 80, margins: "5 5 5 5",
                listeners: {
                    "click": function (btn, e, eOpts) {//添加点击按钮事件
                        _winSearchProject.close();
                    }
                }
            }
            ]
        };

        me.SearchPanel = {
                xtype: "panel",
                layout: {
                    type: 'vbox',
                    align: 'stretch',
                    pack: 'start',
                },

                baseCls: 'my-panel-no-border',//隐藏边框

                items: [me.SearchTab,me.btnPlan],
                flex: 1
        };

        //添加属性TAB页面到容器
        me.items = [
            			//{
            			//    xtype:"button",
            			//    text:"我的按钮"
            			//}
                       me.SearchPanel
        ];
        me.callParent(arguments);

        //文本框设置焦点
        me.docInputText.focus(false,200);
    },

    functionName: function (strresult) {
        //父窗口调用函数
    }
});