/*定义选择用户组控件*/
Ext.define('Ext.ux.YDForm.User._SelectUserGroupPanel', {
    //extend: 'Ext.container.Container',
    extend: 'Ext.panel.Panel',
    alias: 'widget._SelectUserGroupPanel', // 此类的xtype类型为buttontransparent  
    //xtype: "panel",
    title: "组织机构",
    layout: {
        type: 'vbox',
        pack: 'start',
        align: 'stretch'
    },
    width: 400,
    height: '100%',
    GroupType:'Org',Filter:'',
    baseCls: 'my-panel-no-border',//隐藏边框
    initComponent: function () {
        var me = this;
        me.renderTo = me.el;//Ext.getBody(); //

        me.defaultGroup = "";

        //me.GroupType = 'Org';
        me.GroupTypeDesc = '组织机构';
        if (me.GroupType === 'Project') {
            me.GroupTypeDesc = '项目组';
            me.title = me.GroupTypeDesc;
        }

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
                    exception: CDMSWeb.ProxyException
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
            region: "center",
            cellTip: true,
            store: me._groupselstore,
            //Grid使用了复选框作为选择行的方式
            //selType: "checkboxmodel",
            //selModel: { checkOnly: false, mode: "MULTI" },
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

                }
            }, flex: 1
        });

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
            autoLoad: false,
            idProperty: "id"
        });


        me.grouppickerstore = Ext.create("Ext.data.TreeStore", {

            batchActions: false,
            remoteFilter: false,
            remoteSort: false,
            model: me.grouppickermodel,
            root: { id: "Root", text: "所有" + me.GroupTypeDesc, expanded: true },
            autoLoad: false,
            ////代理定义
            proxy: {
                type: 'ajax',
                url: "WebApi/Get",//调用路径：\simplecdms\controllers\projectcontroller.cs
                autoLoad:false,
                extraParams: {
                    C: "AVEVA.CDMS.WebApi.UserController", A: "GetUserGroupList",
                    sid: localStorage.getItem("sid"), GroupType: me.GroupType,
                    Filter: me.Filter
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
            title: '选择' + me.GroupTypeDesc,
            collapsible: true, rootVisible: true,
            autoLoad: false,
            store: me.grouppickerstore, split: true,
            height: 180,
            width: "100%", minWidth: 100,
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
                //itemclick: function (picker, record) {

                //    //var me = this,
                //    var selection = picker.getSelectionModel().getSelection();
                //    //var valueField = me.valueField;

                //    var strmsg;
                //    //Ext.Msg.alert("您展开了目录树节点！！！", "您展开了目录树节点！节点ID:" + selection[0].data.id + "," + selection[0].data.text);
                //    me._groupselstore.proxy.extraParams.Filter = me.groupinputtext.getValue();//把参数传给C#MVC,路径：\simplecdms\controllers\Doccontroller.cs 下的 GetWorkFlow()
                //    me._groupselstore.proxy.extraParams.Group = selection[0].data.id;
                //    me._groupselstore.proxy.extraParams.sid = localStorage.getItem("sid");
                //    me._groupselstore.currentPage = 1;
                //    me._groupselstore.load();
                //},
                "select": function (picker, record, index, eOpts) {
                    //var me = this,
                    //var selection = picker.getSelectionModel().getSelection();
                    //var valueField = me.valueField;

                    var strmsg;
                    me._groupselstore.proxy.extraParams.Filter = me.groupinputtext.getValue();//把参数传给C#MVC,路径：\simplecdms\controllers\Doccontroller.cs 下的 GetWorkFlow()
                    me._groupselstore.proxy.extraParams.Group = record.data.id;
                    me._groupselstore.proxy.extraParams.sid = localStorage.getItem("sid");
                    me._groupselstore.currentPage = 1;
                    me._groupselstore.load();
                },
                "render": function () {
                    //setInterval(function () {
                    //        if (me.grouppicker.getRootNode().firstChild != undefined) {
                    //            var record = me.grouppicker.getRootNode().firstChild;

                    //            var fnode = me.grouppicker.store.getNodeById(record.data.id);
                    //            me.grouppicker.getSelectionModel().select(fnode, true);
                    //        }
                    //}, 1000);

                }
            }
        });

        //添加属性TAB页面到容器
        me.items = [
                   me.grouppicker,
                   me.groupinputtext,
                   me.groupgrid
        ];

        //if (me.defaultGroup != "")
        //{
        //    me.selectGroup(me.defaultGroup);
        //}

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
    },

    setGroupType: function (type) {
        var me = this;
        me.GroupType = type;

        //me.grouppickerstore.removeAll();

        me.grouppickerstore.proxy.extraParams.GroupType = type;
        me.grouppickerstore.load();
    },

    //setDefaultGroup: function (groupCode) {

    //    var me = this;
    //    me.defaultGroup = groupCode;

    //},
    selectGroup: function (groupKeyword) {

        var me = this;

        me._groupselstore.proxy.extraParams.Filter = "";//把参数传给C#MVC,路径：\simplecdms\controllers\Doccontroller.cs 下的 GetWorkFlow()
        me._groupselstore.proxy.extraParams.Group = groupKeyword;
        me._groupselstore.proxy.extraParams.sid = localStorage.getItem("sid");
        me._groupselstore.currentPage = 1;
        me._groupselstore.load();

    }
});