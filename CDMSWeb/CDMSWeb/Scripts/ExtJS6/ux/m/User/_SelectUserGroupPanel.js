/*定义文档列表grid*/
Ext.define('Ext.ux.m.User._SelectUserGroupPanel', {
    //extend: 'Ext.container.Container',
    extend: 'Ext.panel.Panel',
    alias: 'widget._SelectUserGroupPanel', // 此类的xtype类型为buttontransparent  
    title: "组织机构",
    layout: {
        type: 'vbox',
        pack: 'start',
        align: 'stretch'
    },
    width: 400,
    //height: '100%',
    flex:2,
    baseCls: 'my-panel-no-border',//隐藏边框
    initComponent: function () {
        var me = this;
        me.renderTo = me.el;


        //定义已选择用户的model
        Ext.define("_UserResult", {
            extend: "Ext.data.Model",
            fields: ["text", "id"],
            url: "_blank",
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

        //定义未选择用户的model
        Ext.define("_GroupAllUserSelection", {
            extend: "Ext.data.Model",
            fields: ["text", "id"],
            proxy: {
                type: "ajax",
                url: 'WebApi/Get',
                extraParams: {
                    C: "AVEVA.CDMS.WebApi.UserController", A: "GetAllUserList",
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
                    exception: CDMSWap.ProxyException
                }
            }

        });

        //定义未选择用户的store
        me._groupselstore = Ext.create("Ext.data.Store", {
            batchActions: false,
            //文章的Store需要支持远程排序和搜索
            remoteFilter: true,
            remoteSort: true,
            //每50条记录为一页
            pageSize: 50,//视图路径：\simplecdms\scripts\app\view\content\view.js
            model: "_GroupAllUserSelection"

        });

        //定义未选择用户tab的view
        me.groupgrid = Ext.widget("grid", {
            //region: "center",
            //cellTip: true,
            width: '100%',
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
                { text: '用户名', dataIndex: 'text',flex:1}//, width: '100%' }
            ],
            listeners: {
                'itemdblclick': function (view, record, item, index, e) {

                }
            }, flex: 1
        });

        //定义文本输入框
        me.groupinputtext = Ext.widget('textfield', {
            name: "Title", width: "100%", enableKeyEvents: true,
            fieldLabel: "搜索", anchor: "80%", labelWidth: 30, labelAlign: "left", margin: '2 2 2 2',
            listeners: {
                //这里不能用change函数，因为刷新GRID的时候有问题
                //change: function (field, newValue, oldValue) {
                "keyup": function (src, evt) {
                    me._groupselstore.proxy.extraParams.Filter = src.getValue();//把参数传给C#MVC,路径：\simplecdms\controllers\Doccontroller.cs 下的 GetWorkFlow()
                    me._groupselstore.proxy.extraParams.sid = localStorage.getItem("sid");
                    me._groupselstore.currentPage = 1;
                    me._groupselstore.load();

                }
            }
        })

        //定义组织机构树的model
        me.grouppickermodel = Ext.define("_GroupPicker", {
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
                    exception: CDMSWap.ProxyException
                }
            },
        });

        //创建树控件  
        me.grouppicker = Ext.create('Ext.tree.Panel', {
            title: '选择组织机构',
            //collapsible: true,
            rootVisible: true,
            store: me.grouppickerstore, split: true,
            //height: 180,
            width: "100%", minWidth: 100,
            flex: 1,
            //root: {
            //    id: "Root", text: "所有组织机构"
            //    //, expanded: true
            //},
            viewConfig: {
                stripeRows: true,
            //    listeners: {
            //        //itemcontextmenu: function (view, rec, node, index, e) {
            //        //    e.stopEvent();
            //        //    contextMenu.showAt(e.getXY());
            //        //    return false;
            //        //}

            //    }
            },
            listeners: {
                "itemclick": function (picker, record) {

                    //var me = this,
                    var selection = picker.getSelectionModel().getSelection();
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
        });

        function addItemToResultGrid(record) {
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
        };

        //添加用户选择grid双击事件
        me.groupgrid.on('itemdblclick', function (view, record, item, index, e) {
            addItemToResultGrid(record);
        });

        //定义已选择用户的view
        me.resultgrid = Ext.widget("grid", {
            height: '100%',
            width: "100%",
            cellTip: true, margin: "2 0 0 0",
            store: me._userresultstore,

            columns: [
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
            }, flex: 1
        });

        //添加属性TAB页面到容器
        me.items = [
                   me.grouppicker,
                   me.groupinputtext,
                   me.groupgrid,
                   me.resultgrid
        ];

        me.callParent(arguments);
    },

    //设置用户列表的范围（是否包含子用户组的用户）
    setUserListRange: function (isContains) {
        var me = this;
        //父窗口调用函数
        if (isContains === "false")
        {
            me._groupselstore.model.proxy.extraParams.A = "GetUserList";

        }
    }
});