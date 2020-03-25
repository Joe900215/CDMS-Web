/*定义属性TAB里面的权限列表表格*/
Ext.define('Ext.ux.YDForm.Right._RightGrid', {
    extend: 'Ext.panel.Panel',
    alias: 'widget._RightGrid', // 此类的xtype类型为buttontransparent  
    //activeTab: 0, region: "east", width: "25%", minWidth: 100, split: true, collapsible: true,
    layout: 'fit',
    initComponent: function () {
        var me = this;
        me.renderTo = me.el;
        //是否有当前目录的控制权限
        me.hasPCntrlRight = false;
        me.hasDCntrlRight = false;

        //文档或文件夹
        me.objectType = "";
        //文档或文件夹的Keyword
        me.objectKeyword = "";

        //前面的UserName到DNone字段的顺序必须与Grid显示的列的顺序一致
        me.RightFieldsData = [ "UserName", "AcceObj",
                "PFull", "PCreate", "PRead", "PWrite", "PDelete", "PCntrl", "PNone",
                "DFull", "DCreate", "DRead", "DWrite", "DDelete", "DFRead", "DFWrite", "DCntrl", "DNone",
                 "Enable", "Visible","ObjectKeyWord","IsUser"];

        //添加专业combo
        Ext.define("RightListModel", {
            extend: 'Ext.data.Model',
            fields: me.RightFieldsData
            //["UserName", "PFull", "PCreate", "PRead", "PWrite", "PDelete", "PCntrl", "PNone",
            //"DFull", "DCreate", "DRead", "DWrite", "DDelete", "DFRead", "DFWrite", "DCntrl", "DNone",
            //"AcceObj","Enable", "Visible"]
        });

        me._addRightBtn = Ext.create('Ext.button.Button', {
            xtype: "button", disabled: true,
            iconCls: "address-book-add", text: '增加', scope: me, tooltip: '增加', listeners: {
                "click": function (btn, e, eOpts) {
                    me.addUserRight();

                }
            }
        });

        me._delRightBtn = Ext.create('Ext.button.Button', {
            xtype: "button", disabled: true,
            iconCls: "address-book-close", text: '删除', scope: me, tooltip: '删除', listeners: {
                "click": function (btn, e, eOpts) {
                    me.delUserRight();

                }
            }
        });

        //me._editGroupBtn = Ext.create('Ext.button.Button', {
        //    xtype: "button", disabled: true,
        //    iconCls: "group-edit", text: '编辑用户组', scope: me, tooltip: '编辑用户组', listeners: {
        //        "click": function (btn, e, eOpts) {
        //            me.editGroup();

        //        }
        //    }
        //});

        me._saveRightBtn = Ext.create('Ext.button.Button', {
            xtype: "button",disabled :true,
            iconCls: "save", text: '保存', scope: me, tooltip: '保存', listeners: {
                "click": function (btn, e, eOpts) {
                    me.setRight();

                }
            }
        });
        
        //定义权限表格按钮
        me._rightListTbar = Ext.create('Ext.toolbar.Toolbar', {
            xtype: 'toolbar',
            dock: 'top',
            items: [
                me._addRightBtn,
                me._delRightBtn,
               // me._editGroupBtn,
                 me._saveRightBtn
            ]
        });

      //  me.toolBar.items.add('bb_id', bb);

        //定义DOC属性store
        me._MainAttrStore = Ext.create("Ext.data.Store", {
            //model: 'CDMSWeb.model.Doc',//模型路径：\simplecdms\scripts\app\model\Property.js
            model:RightListModel,
            batchActions: false,
            //文章的Store需要支持远程排序和搜索
            remoteFilter: true,
            remoteSort: true,
            //每50条记录为一页
            pageSize: 50,//视图路径：\simplecdms\scripts\app\view\content\view.js
            proxy: {
                type: "ajax",
                url: 'WebApi/Get',
                extraParams: {
                    C: "AVEVA.CDMS.WebApi.DocController", A: "GetDocRightList"
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
                },
                listeners: {
                    exception: CDMSWeb.ProxyException
                }
            }
        });

        //定义目录及文档属性共用的表格
        me.MainAttrGrid = Ext.widget("grid", {
            layout: 'fit',
            store: me._MainAttrStore,
            tbar: me._rightListTbar,
            viewConfig: {
                getRowClass: function (record, rowIndex, p, store) {
                    if (record.data.UserName === "LoginUser")
                        return "hide-store-row";
                }
            },
            //Grid使用了复选框作为选择行的方式
            columns: [
                { text: '用户/用户组', dataIndex: 'UserName', flex: 1, listeners: {
                    "dblclick": function (btn, e, reccount, index, eOpts, recoder) {
                        // me.addUserRight();
                        me.onEditProjectUser(recoder);
                    }
                } },
                { text: '权限所在目录', dataIndex: 'AcceObj', flex: 1},//width:200 },
                {
                    text: '目录权限', sortable: false,
                    columns: [
                       {//添加图标
                           text: '所有',
                           sortable: false,
                           xtype: 'actioncolumn',
                           //enableColumnResize: false,
                           width: 50,
                           items: [{
                               getClass: function (v, metaData, record) {
                                   return me.getRightClass(v, metaData, record, 'PFull')
                               },
                               tooltip: '目录所有权限',
                               handler: function (grid, rowIndex, colIndex) {
                                   me.onCheckerClick(grid, rowIndex, colIndex,'PFull')
                               }
                           }]
                       },
                       {//添加图标
                           text: '创建',
                           sortable: false,
                           xtype: 'actioncolumn',
                           //enableColumnResize: false,
                           width: 50,
                           items: [{
                               getClass: function (v, metaData, record) {
                                   return me.getRightClass(v, metaData, record, 'PCreate')
                               },
                               tooltip: '目录创建权限',
                               handler: function (grid, rowIndex, colIndex) {
                                   me.onCheckerClick(grid, rowIndex, colIndex, 'PCreate')
                               }
                           }]
                       },
                       {//添加图标
                           text: '读',
                           sortable: false,
                           xtype: 'actioncolumn',
                           //enableColumnResize: false,
                           width: 50,
                           items: [{
                               getClass: function (v, metaData, record) {
                                   return me.getRightClass(v, metaData, record, 'PRead')
                               },
                               tooltip: '目录读权限',
                               handler: function (grid, rowIndex, colIndex) {
                                   me.onCheckerClick(grid, rowIndex, colIndex, 'PRead')
                               }
                           }]
                       },
                       {//添加图标
                           text: '写',
                           sortable: false,
                           xtype: 'actioncolumn',
                           //enableColumnResize: false,
                           width: 50,
                           items: [{
                               getClass: function (v, metaData, record) {
                                   return me.getRightClass(v, metaData, record, 'PWrite')
                               },
                               tooltip: '目录写权限',
                               handler: function (grid, rowIndex, colIndex) {
                                   me.onCheckerClick(grid, rowIndex, colIndex, 'PWrite')
                               }
                           }]
                       },
                       {//添加图标
                           text: '删除',
                           sortable: false,
                           xtype: 'actioncolumn',
                           //enableColumnResize: false,
                           width: 50,
                           items: [{
                               getClass: function (v, metaData, record) {
                                   return me.getRightClass(v, metaData, record, 'PDelete')
                               },
                               tooltip: '目录删除权限',
                               handler: function (grid, rowIndex, colIndex) {
                                   me.onCheckerClick(grid, rowIndex, colIndex, 'PDelete')
                               }
                           }]
                       },
                       {//添加图标
                           text: '控制',
                           sortable: false,
                           xtype: 'actioncolumn',
                           //enableColumnResize: false,
                           width: 50,
                           items: [{
                               getClass: function (v, metaData, record) {
                                   return me.getRightClass(v, metaData, record, 'PCntrl')
                               },
                               tooltip: '目录控制权限',
                               handler: function (grid, rowIndex, colIndex) {
                                   me.onCheckerClick(grid, rowIndex, colIndex, 'PCntrl')
                               }
                           }]
                       },
                       {//添加图标
                           text: '无',
                           sortable: false,
                           xtype: 'actioncolumn',
                           //enableColumnResize: false,
                           width: 50,
                           items: [{
                               getClass: function (v, metaData, record) {
                                   return me.getRightClass(v, metaData, record, 'PNone')
                               },
                               tooltip: '目录无权限',
                               handler: function (grid, rowIndex, colIndex) {
                                   me.onCheckerClick(grid, rowIndex, colIndex, 'PNone')
                               }
                           }]
                       },
                    ]
                },
                {
                    text: '文档权限', sortable: false,
                    columns: [
                       {//添加图标
                           text: '所有',
                           sortable: false,
                           xtype: 'actioncolumn',
                           //enableColumnResize: false,
                           width: 50,
                           items: [{
                               getClass: function (v, metaData, record) {
                                   return me.getRightClass(v, metaData, record, 'DFull')
                               },
                               tooltip: '文档所有权限',
                               handler: function (grid, rowIndex, colIndex) {
                                   me.onCheckerClick(grid, rowIndex, colIndex, 'DFull')
                               }
                           }]
                       },
                       {//添加图标
                           text: '创建',
                           sortable: false,
                           xtype: 'actioncolumn',
                           //enableColumnResize: false,
                           width: 50,
                           items: [{
                               getClass: function (v, metaData, record) {
                                   return me.getRightClass(v, metaData, record, 'DCreate')
                               },
                               tooltip: '文档创建权限',
                               handler: function (grid, rowIndex, colIndex) {
                                   me.onCheckerClick(grid, rowIndex, colIndex, 'DCreate')
                               }
                           }]
                       },
                       {//添加图标
                           text: '读',
                           sortable: false,
                           xtype: 'actioncolumn',
                           //enableColumnResize: false,
                           width: 50,
                           items: [{
                               getClass: function (v, metaData, record) {
                                   return me.getRightClass(v, metaData, record, 'DRead')
                               },
                               tooltip: '文档读权限',
                               handler: function (grid, rowIndex, colIndex) {
                                   me.onCheckerClick(grid, rowIndex, colIndex, 'DRead')
                               }
                           }]
                       },
                       {//添加图标
                           text: '写',
                           sortable: false,
                           xtype: 'actioncolumn',
                           //enableColumnResize: false,
                           width: 50,
                           items: [{
                               getClass: function (v, metaData, record) {
                                   return me.getRightClass(v, metaData, record, 'DWrite')
                               },
                               tooltip: '文档写权限',
                               handler: function (grid, rowIndex, colIndex) {
                                   me.onCheckerClick(grid, rowIndex, colIndex, 'DWrite')
                               }
                           }]
                       },
                       {//添加图标
                           text: '删除',
                           sortable: false,
                           xtype: 'actioncolumn',
                           //enableColumnResize: false,
                           width: 50,
                           items: [{
                               getClass: function (v, metaData, record) {
                                   return me.getRightClass(v, metaData, record, 'DDelete')
                               },
                               tooltip: '文档删除权限',
                               handler: function (grid, rowIndex, colIndex) {
                                   me.onCheckerClick(grid, rowIndex, colIndex, 'DDelete')
                               }
                           }]
                       },
                       {//添加图标
                           text: '读文件',
                           sortable: false,
                           xtype: 'actioncolumn',
                           //enableColumnResize: false,
                           width: 60,
                           items: [{
                               getClass: function (v, metaData, record) {
                                   return me.getRightClass(v, metaData, record, 'DFRead')
                               },
                               tooltip: '文档读文件权限',
                               handler: function (grid, rowIndex, colIndex) {
                                   me.onCheckerClick(grid, rowIndex, colIndex, 'DFRead')
                               }
                           }]
                       },
                       {//添加图标
                           text: '写文件',
                           sortable: false,
                           xtype: 'actioncolumn',
                           //enableColumnResize: false,
                           width: 60,
                           items: [{
                               getClass: function (v, metaData, record) {
                                   return me.getRightClass(v, metaData, record, 'DFWrite')
                               },
                               tooltip: '文档写文件权限',
                               handler: function (grid, rowIndex, colIndex) {
                                   me.onCheckerClick(grid, rowIndex, colIndex, 'DFWrite')
                               }
                           }]
                       },
                       {//添加图标
                           text: '控制',
                           sortable: false,
                           xtype: 'actioncolumn',
                           //enableColumnResize: false,
                           width: 50,
                           items: [{
                               getClass: function (v, metaData, record) {
                                   return me.getRightClass(v, metaData, record, 'DCntrl')
                               },
                               tooltip: '文档控制权限',
                               handler: function (grid, rowIndex, colIndex) {
                                   me.onCheckerClick(grid, rowIndex, colIndex, 'DCntrl')
                               }
                           }]
                       },
                       {//添加图标
                           text: '无',
                           sortable: false,
                           xtype: 'actioncolumn',
                           //enableColumnResize: false,
                           width: 50,
                           items: [{
                               getClass: function (v, metaData, record) {
                                   return me.getRightClass(v, metaData, record, 'DNone')
                               },
                               tooltip: '文档无权限',
                               handler: function (grid, rowIndex, colIndex) {
                                   me.onCheckerClick(grid, rowIndex, colIndex, 'DNone')
                               }
                           }]
                       }
                    ]
                }
            ]
        });

        //添加属性TAB页面到容器
        me.items = [
            			//{
            			//    xtype: "button",
            			//    text:"我的按钮"
            			//}
                               me.MainAttrGrid
        ];

        //me.contextmenu = Ext.create('Ext.menu.Menu', {
        //    float: true,
        //    items: [{
        //        text: '编辑用户组',
        //        action: 'submenu1',
        //        iconCls: 'leaf', handler: function () {
        //            me.submenu1OnClick();
        //        }
        //    }]
        //});

        me.callParent(arguments);
    },

    //获取Checker图标
    getRightClass: function (v, metaData, record, rightType) {
        var me = this;
        var wRight = record.get(rightType);
        if (me.hasPCntrlRight === false) {
            if (wRight === true) {
                return 'x-grid-checkheader-checked-disable';
            } else {
                return 'x-grid-checkheader-disable';
            }
        }
        //权限是否在当前目录
        var isObjno = record.get("Enable");

        if (isObjno === true) {
            if (wRight === true) {
                return 'x-grid-checkheader-checked';
            } else {
                return 'x-grid-checkheader';
            }
        } else {
            if (wRight === true) {
                return 'x-grid-checkheader-checked-disable';
            } else {
                return 'x-grid-checkheader-disable';
            }
        }
    },

    onCheckerClick: function (grid, rowIndex, colIndex, rightType) {
        var me = this;
        //是否有修改权限的权限
        if (me.hasPCntrlRight === false) {
            return;
        }

        var record = grid.store.getAt(rowIndex);

        //权限是否在当前目录
        var isObjno = record.get("Enable");

        if (isObjno === true) {

            var fieldKey = me.RightFieldsData[colIndex];
            var wRight = record.get(rightType);
            if (wRight === true) {
                //如果是进行取消目录无权限的操作，就忽略操作，否则就修改权限
                if (fieldKey != "PNone" && fieldKey != "DNone") {
                    record.set(rightType, false);
                    //取消勾选checker时，目录权限修改
                    //取消了目录创建，写，删除，控制权限
                    if (fieldKey == "PCreate" || fieldKey == "PWrite" || fieldKey == "PDelete" || fieldKey == "PCntrl") {
                        record.set("PFull", false);
                    }
                        //取消了目录读权限
                    else if (fieldKey == "PRead") {
                        record.set("PFull", false);
                        record.set("PCreate", false);
                        record.set("PWrite", false);
                        record.set("PDelete", false);
                        record.set("PCntrl", false);
                        record.set("PNone", true);
                    }

                        //取消勾选checker时，文档权限修改
                        //取消了文档创建，写，删除，控制权限
                    else if (fieldKey == "DCreate" || fieldKey == "DWrite" || fieldKey == "DDelete" || fieldKey == "DCntrl" || fieldKey == "DFRead" || fieldKey == "DFWrite") {
                        record.set("DFull", false);
                    }
                        //取消了文档读权限
                    else if (fieldKey == "DRead") {
                        record.set("DFull", false);
                        record.set("DCreate", false);
                        record.set("DWrite", false);
                        record.set("DDelete", false);
                        record.set("DFRead", false);
                        record.set("DFWrite", false);
                        record.set("DCntrl", false);
                        record.set("DNone", true);
                    }
                }
            }
            else
            {
                record.set(rightType, true);
                //勾选checker时，目录权限修改
                //选择了目录所有权限
                if (fieldKey == "PFull") { 
                    record.set("PCreate", true);
                    record.set("PRead", true);
                    record.set("PWrite", true);
                    record.set("PDelete", true);
                    record.set("PCntrl", true);
                    record.set("PNone", false);
                }
                    //选择了目录创建权限
                else if (fieldKey == "PCreate") {
                    record.set("PRead", true);
                    record.set("PWrite", true);
                    record.set("PNone", false);
                }
                    //选择了目录读权限
                else if (fieldKey == "PRead") {
                    record.set("PNone", false);
                }
                    //选择了目录写权限
                else if (fieldKey == "PWrite") {
                    record.set("PRead", true);
                    record.set("PNone", false);
                }
                    //选择了目录删除权限
                else if (fieldKey == "PDelete") {
                    record.set("PRead", true);
                    record.set("PWrite", true);
                    record.set("PNone", false);
                }
                    //选择了目录控制权限
                else if (fieldKey == "PCntrl") {
                    record.set("PRead", true);
                    record.set("PNone", false);
                }
                    //选择了目录无权限
                else if (fieldKey == "PNone") {
                    record.set("PFull", false);
                    record.set("PCreate", false);
                    record.set("PRead", false);
                    record.set("PWrite", false);
                    record.set("PDelete", false);
                    record.set("PCntrl", false);
                }

                    //勾选checker时，文档权限修改
                    //选择了文档所有权限
                else if (fieldKey == "DFull") {
                    record.set("DCreate", true);
                    record.set("DRead", true);
                    record.set("DWrite", true);
                    record.set("DDelete", true);
                    record.set("DFRead", true);
                    record.set("DFWrite", true);
                    record.set("DCntrl", true);
                    record.set("DNone", false);
                }
                    //选择了文档创建权限
                else if (fieldKey == "DCreate") {
                    record.set("DRead", true);
                    record.set("DWrite", true);
                    record.set("DNone", false);
                }
                    //选择了文档读权限
                else if (fieldKey == "DRead") {
                    record.set("DNone", false);
                }
                    //选择了文档写权限
                else if (fieldKey == "DWrite") {
                    record.set("DRead", true);
                    record.set("DNone", false);
                }
                    //选择了文档删除权限
                else if (fieldKey == "DDelete") {
                    record.set("DRead", true);
                    record.set("DWrite", true);
                    record.set("DFRead", true);
                    record.set("DFWrite", true);
                    record.set("DNone", false);
                }
                    //选择了文档读文件权限
                else if (fieldKey == "DFRead") {
                    record.set("DRead", true);
                    record.set("DNone", false);
                }
                    //选择了文档写文件权限
                else if (fieldKey == "DFWrite") {
                    record.set("DRead", true);
                    record.set("DWrite", true);
                    record.set("DFRead", true);
                    record.set("DNone", false);
                }
                    //选择了文档控制权限
                else if (fieldKey == "DCntrl") {
                    record.set("DRead", true);
                    record.set("DNone", false);
                }
                    //选择了文档无权限
                else if (fieldKey == "DNone") {
                    record.set("DFull", false);
                    record.set("DCreate", false);
                    record.set("DRead", false);
                    record.set("DWrite", false);
                    record.set("DDelete", false);
                    record.set("DFRead", false);
                    record.set("DFWrite", false);
                    record.set("DCntrl", false);
                }
            }
        }
    },

    //刷新文档权限TAB页
    loadRightPage: function (rightType, Keyword) {
        var me = this;
        me.objectType = rightType;
        me.objectKeyword = Keyword;

        if (rightType === "Doc") {
            //if (me.seleObjType === "Doc") {
            //me.DocKeyword = Keyword;
            var gridStore = me._MainAttrStore;
            gridStore.proxy.extraParams.C = "AVEVA.CDMS.WebApi.DocController";
            gridStore.proxy.extraParams.A = "GetDocRightList";
            gridStore.proxy.extraParams.DocKeyword = Keyword;//把参数传给C#MVC,路径：\simplecdms\controllers\projectcontroller.cs 下的 GetChildProject()
            gridStore.proxy.extraParams.sid = localStorage.getItem("sid");
            gridStore.load({
                callback: function (records, options, success) {
                    me.hasDCntrlRight = false;
                    me.setTBarDisabled(true);
                    for (var i = 0; i < records.length ; i++) {
                        if (records[i].data.UserName === "LoginUser" && (records[i].data.DCntrl === true || records[i].data.PCntrl === true)) {
                            me.hasDCntrlRight = true;
                            me.setTBarDisabled(false);
                        }
                    };
                }
            })
        } else if (rightType === "Project") {
            //if (me.seleObjType === "Doc") {
            //me.DocKeyword = Keyword;
            var gridStore = me._MainAttrStore;
            gridStore.proxy.extraParams.C = "AVEVA.CDMS.WebApi.ProjectController";
            gridStore.proxy.extraParams.A = "GetProjectRightList";
            gridStore.proxy.extraParams.ProjectKeyword = Keyword;//把参数传给C#MVC,路径：\simplecdms\controllers\projectcontroller.cs 下的 GetChildProject()
            gridStore.proxy.extraParams.sid = localStorage.getItem("sid");
            gridStore.load({
                callback: function (records, options, success) {
                    //var girdcount = 0;
                    ////records.each(function (r) {
                    me.hasPCntrlRight = false;
                    me.setTBarDisabled(true);
                    for (var i = 0; i < records.length ; i++) {
                        if (records[i].data.UserName === "LoginUser" && records[i].data.PCntrl === true) {
                            me.hasPCntrlRight = true;
                            me.setTBarDisabled(false);
                        }
                    };
                }
            })
        }
    },

    //设置是否禁用按钮
    setTBarDisabled: function (disabled) {
        var me = this;
        me._addRightBtn.setDisabled(disabled);
        me._delRightBtn.setDisabled(disabled);
      //  me._editGroupBtn.setDisabled(disabled);
        me._saveRightBtn.setDisabled(disabled);
    },

    //编辑项目用户组的用户
    onEditProjectUser: function (recoder) {
        var me = this;
        if (recoder.data.IsUser === false) {

            var objectKeyWord = recoder.data.ObjectKeyWord;
            var groupDesc = recoder.data.UserName;

            var fmEditProjectGroup = Ext.create('Ext.ux.YDForm.Group._EditProjectGroup',// + plugins + '.' + state,
                    { title: "", mainPanelId: me.Id, groupKeyword: objectKeyWord, groupDesc: groupDesc });

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

            winEditProjectGroup.show();

            //监听子窗口关闭事件
            winEditProjectGroup.on('close', function () {

            });

        }
    },

    addUserRight: function () {
        var me = this;

        if ((me.objectType === "Project" && me.hasPCntrlRight === false)
            || (me.objectType === "Doc" && me.hasDCntrlRight === false)) {
            Ext.Msg.alert("错误信息", "你没有修改目录的权限！");
            return;
        }

        Ext.require('Ext.ux.Common.comm', function () {
            showSelectUserWin("getUser", "", "", function () {

                var strUserNameList = window.parent.usernamelist;
                var strUserList = window.parent.resultvalue;

                if (strUserList === undefined || strUserList === "") return;

                var userList = strUserList.split(",");
                var userNameList = strUserNameList.split(",");

                //声明对应grid的Record对象 
                //var ItemRecord = Ext.data.Record.create([
                Ext.define('Record', {
                    extend: 'Ext.data.Model',
                    fields: [

                { name: 'AcceObj' },
                { name: 'DCntrl' },
                { name: 'DCreate' },
                { name: 'DDelete' },
                { name: 'DFRead' },
                { name: 'DFWrite' },
                { name: 'DFull' },
                { name: 'DNone' },
                { name: 'DRead' },
                { name: 'DWrite' },
                { name: 'Enable' },
                { name: 'ObjectKeyWord' },
                { name: 'PCntrl' },
                { name: 'PCreate' },
                { name: 'PDelete' },
                 { name: 'PFull' },
                { name: 'PNone' },
                { name: 'PRead' },
                { name: 'PWrite' },
                { name: 'UserName' }

                    ]
                });

                for (var uli = 0; uli < userList.length ; uli++) {
                    var isInStore = false;
                    for (var i = 0; i < me.MainAttrGrid.store.getCount() ; i++) {
                        var dataItem = me.MainAttrGrid.store.getAt(i).data; //遍历每一行
                        if (dataItem.ObjectKeyWord != undefined && dataItem.ObjectKeyWord != "") {
                            if (dataItem.ObjectKeyWord === userList[uli]) {
                                isInStore = true;
                            }
                        }
                    }
                    if (isInStore === false) {
                        //用户没有包含在表格store内,就向store里面插入一行
                        // Ext.Msg.alert("信息", userList[uli] + "," + userNameList[uli] + "没有包含在store内");
                        var rec = new Record({
                            AcceObj: "",
                            DCntrl: false,
                            DCreate: false,
                            DDelete: false,
                            DFRead: true,
                            DFWrite: false,
                            DFull: false,
                            DNone: false,
                            DRead: true,
                            DWrite: true,
                            Enable: true,
                            ObjectKeyWord: userList[uli],
                            PCntrl: false,
                            PCreate: false,
                            PDelete: false,
                            PFull: false,
                            PNone: false,
                            PRead: true,
                            PWrite: true,
                            UserName: userNameList[uli]
                        });
                        me.MainAttrGrid.store.insert(me.MainAttrGrid.store.getCount(), rec);//插入新行作为grid最后一行
                        me.MainAttrGrid.getView().refresh(); //刷新
                    } else {
                        //用户已经包含在表格store内
                    }
                }
                //Ext.Msg.alert("信息", userNameList);
            });
        })
    },

    delUserRight: function () {
        var me = this;

        if ((me.objectType === "Project" && me.hasPCntrlRight === false)
            || (me.objectType === "Doc" && me.hasDCntrlRight === false)) {
            Ext.Msg.alert("权限错误信息", "你没有修改目录的权限！");
            return;
        }

        var selerec = me.MainAttrGrid.getSelectionModel().getSelection()[0];

        //删除选中的第一条记录
        if (selerec != undefined) {
            if (selerec.data.Enable === false) {
                Ext.Msg.alert("权限错误信息", "你没有修改目录的权限！");
                return;
            }
            me.MainAttrGrid.store.remove(selerec);
        } else {
            Ext.Msg.alert("错误信息", "请选择要删除的用户或用户组！");
        }
    },

    //editGroup: function () {
    //    var me = this;

    //    if ((me.objectType === "Project" && me.hasPCntrlRight === false)
    //        || (me.objectType === "Doc" && me.hasDCntrlRight === false)) {
    //        Ext.Msg.alert("权限错误信息", "你没有编辑用户组的权限！");
    //        return;
    //    }

    //    var selerec = me.MainAttrGrid.getSelectionModel().getSelection()[0];

    //    //删除选中的第一条记录
    //    if (selerec != undefined) {
    //        if (selerec.data.Enable === false) {
    //            Ext.Msg.alert("权限错误信息", "你没有编辑用户组的权限！");
    //            return;
    //        }
    //        //me.MainAttrGrid.store.remove(selerec);
    //    } else {
    //        Ext.Msg.alert("错误信息", "请选择要删除的用户或用户组！");
    //    }
    //},

    //保存权限
    setRight: function () {
        var me = this;

        if ((me.objectType === "Project" && me.hasPCntrlRight === false)
            || (me.objectType === "Doc" && me.hasDCntrlRight === false)) {
            Ext.Msg.alert("权限错误信息", "你没有修改目录的权限！");
            return;
        }

        //获取表单数据，转换成JSON字符串
        var rightAttr = [];

        for (var i = 0; i < me.MainAttrGrid.store.getCount() ; i++) {
            var dataItem = me.MainAttrGrid.store.getAt(i).data; //遍历每一行
            if (dataItem.ObjectKeyWord != undefined && dataItem.ObjectKeyWord != "" && dataItem.Enable===true) {
                rightAttr.push({
                    ObjectKeyword: dataItem.ObjectKeyWord,

                    PFull: dataItem.PFull,
                    PCreate: dataItem.PCreate,
                    PRead: dataItem.PRead,
                    PWrite: dataItem.PWrite,
                    PDelete: dataItem.PDelete,
                    PCntrl: dataItem.PCntrl,
                    PNone: dataItem.PNone,

                    DFull: dataItem.DFull,
                    DCreate: dataItem.DCreate,
                    DRead: dataItem.DRead,
                    DWrite: dataItem.DWrite,
                    DDelete: dataItem.DDelete,
                    DFRead: dataItem.DFRead,
                    DFWrite: dataItem.DFWrite,
                    DCntrl: dataItem.DCntrl,
                    DNone: dataItem.DNone
                });//在数组里添加新的元素  
            }
        }
        var C, A, docKeyword, projectKeyword;
        var params;
        var rightAttrJson = Ext.JSON.encode(rightAttr);
        //Ext.Msg.alert("信息", rightAttrJson);

        if (me.objectType === "Doc") {
            //C = "AVEVA.CDMS.WebApi.DocController";
            //A = "SetDocRightList";
            //projectKeyword = "";
            //docKeyword = me.objectKeyword;
            params = {
                C: "AVEVA.CDMS.WebApi.DocController", A: "SetDocRightList",
                sid: localStorage.getItem("sid"),
                DocKeyword: me.objectKeyword, rightAttrJson: rightAttrJson
            }
        } else if (me.objectType === "Project") {
            //C = "AVEVA.CDMS.WebApi.ProjectController";
            //A = "SetProjectRightList";
            //projectKeyword = me.objectKeyword;
            //docKeyword = "";
            params = {
                C: "AVEVA.CDMS.WebApi.ProjectController", A: "SetProjectRightList",
                sid: localStorage.getItem("sid"),
                ProjectKeyword: me.objectKeyword, rightAttrJson: rightAttrJson
            }
        }

        Ext.Ajax.request({

            url: 'WebApi/Post',
            method: "POST",
            params: params,
            //    {
            //    C: C, A: A,
            //    sid: localStorage.getItem("sid"),
            //    ProjectKeyword:projectKeyword,DocKeyword:docKeyword,
            //    rightAttrJson: rightAttrJson
            //},
            success: function (response, options) {

                //获取数据后，更新窗口
                var res = Ext.JSON.decode(response.responseText, true);
                var state = res.success;
                if (state === true) {

                    Ext.Msg.alert("信息", "成功设置权限！");
                } else {
                    var errmsg = res.msg;
                    Ext.Msg.alert("错误信息", errmsg);
                  //  winExchangeDoc.close();
                }
            },
            failure: function (response, options) {
                ////Ext.Msg.alert("错误", "连接服务器失败！<br>" + response.responseText);
            }
        })
    },

    ////显示右键菜单方法
    //_showContextMenu: function (view, record, item, index, e, eOpts) {

    //    var me = this;
    //    //阻止浏览器默认右键事件
    //    e.preventDefault();
    //    e.stopEvent();

    //    //显示右键菜单
    //    me.contextmenu.showAt(e.getXY());


    //}

});