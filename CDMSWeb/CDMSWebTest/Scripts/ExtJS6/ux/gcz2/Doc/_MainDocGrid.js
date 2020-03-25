/*定义文档列表grid*/
Ext.define('Ext.ux.gcz.Doc._MainDocGrid', {
    extend: 'Ext.panel.Panel',
    alias: 'widget._mainDocGrid', // 此类的xtype类型为buttontransparent  
    layout: {
        type: 'vbox',
        align: 'stretch',
        pack: 'start'
    },
    flex: 1,
    initComponent: function () {
        var me = this;
        me.renderTo = me.el;

        me.isMouseDown = false;

        //记录文件路径
        me.ServerFullFileName = "";

        //定义文档列表store
        me._DocListStore = Ext.create("Ext.data.Store", {
            model: 'CDMSWap.model._DocList',//模型路径：\simplecdms\scripts\app\model\content.js
            batchActions: false,
            //文章的Store需要支持远程排序和搜索
            remoteFilter: true,
            remoteSort: true,
            pageSize: 50,//视图路径：\simplecdms\scripts\app\view\content\view.js
            proxy: {
                type: "ajax",
                url: 'WebApi/Get',
                extraParams: {
                    C: "AVEVA.CDMS.WebApi.DocController", A: "GetDocList",
                    ProjectKeyWord: 1
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
                }
            }
        });



        //定义搜索框
        me.searchTrigger = {
            xtype: 'triggerfield',
            onClearValue: function () {
                //this.setValue('');
            },
            onTrigger1Click: function () {
                //if (this.hasSearch) {
                this.setValue('');

                me.loadDocListStore(function () { });
                //}
            },
            onTrigger2Click: function () {
                var value = this.getValue();

                if (value.length > 0) {
                    var strSQL = " (o_itemname " + "LIKE" + " '%" + value + "%' or o_itemdesc " + "LIKE" + " '%" + value + "%' ) ";

                    var filterObj = [
                        { name: 'o_itemname', value: strSQL }
                    ];
                    me.selectDocList(filterObj);
                    this.hasSearch = false;
                }
            },
            trigger1Cls: 'x-form-clear-trigger',
            trigger2Cls: 'x-form-search-trigger',
            store: 'Person4Select',
            paramName: 'query',
            hasSearch: 'false',
            width: 220,
            margins: '1 1 1 0',
            emptyText:'搜索文件',
            //fieldLabel: '指标名称',
            labelAlign: 'right',
            labelWidth: 55,
            selectOnFocus: true,
            listeners: {
                //afterrender: {
                //    fn: me.onTriggerfieldAfterRender1111,
                //    scope: me
                //},
                //specialkey: {
                //    fn: me.onTriggerfieldSpecialkey1111,
                //    scope: me
                //}
            }
        };

        //定义文档列表按钮
        me._docListTbar = Ext.create('Ext.toolbar.Toolbar', {
            xtype: 'toolbar',
            dock: 'top',
            items: [
                {
                    iconCls: "refresh", scope: me, tooltip: '刷新', name2: 'DocGridRefreshBtn', listeners: {
                        "click": function (btn, e, eOpts) {
                            me.loadDocListStore(function () { });
                        }
                    }
                }

            ]
        });

        //定义grid文章列表
        me.maindocgrid = Ext.widget("grid", {
            //title: "文档列表", 
            store: me._DocListStore,//"Contents",//
            //Grid使用了复选框作为选择行的方式
            selType: "checkboxmodel",
            selModel: { checkOnly: false, mode: "MULTI" }, //"SIMPLE"},//"MULTI" },
            tbar: me._docListTbar,
            layout: 'fit',
            width: '100%',
            height: '100%',
            //隐藏部分记录
            viewConfig: {
                getRowClass: function (record, rowIndex, p, store) {//CSS class name to add to the row.获得一行的css样式  
                    String.prototype.endWith = function (endStr) {
                        var d = this.length - endStr.length;
                        return (d >= 0 && this.lastIndexOf(endStr) == d)
                    }
                    if (record.data.Title.endWith(".sqlite"))
                        return "hide-store-row";
                }
            },
            bbar: new Ext.PagingToolbar({
                store: me._DocListStore,//"Contents",//
                displayInfo: true,
                displayMsg: '当前显示{0} - {1}条，共{2}条数据',
                emptyMsg: "没有记录"
            }),

            columns: [
                               {//添加图标

                                   //iconCls: 'doc-read-only',
                                   tooltip: 'Edit',
                                   header: '',
                                   dataIndex: 'Creater',
                                   width: 42,
                                   renderer: function (value, obj, b, c, d) {
                                       var resultImg = "";
                                       var imgRight = "";
                                       var record = obj.record;

                                       //获取权限图标
                                       var wRight = record.get('WriteRight');
                                       if (wRight === 'true') {//判断是否有写文件权限
                                           if (record.get('O_dmsstatus_DESC') === "检入") {
                                               imgRight = 'Scripts/app/resources/css/icons/AppIco1/edit.png';
                                           }
                                           else if (record.get('O_dmsstatus_DESC') === "检出") { return 'doc-check-out'; }
                                           else if (record.get('O_dmsstatus_DESC') === "最终状态") {
                                               imgRight = 'Scripts/app/resources/css/icons/AppIco1/final.png';
                                           }
                                       } else {
                                           imgRight = 'Scripts/app/resources/css/icons/AppIco1/readonly.png'
                                       }
                                       imgRight = "<img src='" + imgRight + "' />";

                                       var imgDocType = "";
                                       //获取文档类型图标
                                       var filename = record.get('O_filename');
                                       var extindex = filename.lastIndexOf('.');
                                       if (extindex >= 0) {
                                           var extname = (filename.substring(extindex + 1)).toLowerCase();

                                           //检查此样式是否已存在
                                           var allExtName = ",pdf,doc,docx,xls,xlsx,dwg,rar,zip,tar,cab,gz,7z,iso,thm,";
                                           if (allExtName.indexOf("," + extname + ",") < 0) {
                                               imgDocType = 'Scripts/app/resources/css/icons/AppIco3/unknown.png';
                                           }
                                           else {
                                               imgDocType = 'Scripts/app/resources/css/icons/AppIco3/' + extname + '.png';
                                           }
                                       }
                                       else { imgDocType = 'Scripts/app/resources/css/icons/AppIco3/unknown.png'; }//alert-col' }

                                       imgDocType = "<img src='" + imgDocType + "' />";

                                       return imgRight + imgDocType;
                                       //return "<span style='color:green;font-weight:bold;'>绿男</span><img src='folder_add.png' />";
                                   },
                                   sortable: true,
                                   handler: function (grid, rowIndex, colIndex) {
                                       //点击图标的时候选中行
                                       grid.getSelectionModel().select(rowIndex, true);
                                   },
                                   listeners: {
                                       "dragstart": function (event) {
                                           // 存储拖拽数据和拖拽效果...
                                           event.dataTransfer.setData("Text", "22");
                                           //点击图标的时候选中行
                                           grid.getSelectionModel().select(rowIndex, true);
                                       }

                                   }


                               },
                                //{//添加图标
                                //    menuDisabled: true,
                                //    sortable: false,
                                //    xtype: 'actioncolumn',
                                //    enableColumnResize: false,
                                //    width: 38,

                                //    items: [{

                                //        getClass: function (v, metaData, record) {
                                //            var wRight = record.get('WriteRight');
                                //            if (wRight === 'true') {//判断是否有写文件权限
                                //                if (record.get('O_dmsstatus_DESC') === "检入") {
                                //                    return 'doc-check-in';
                                //                }
                                //                else if (record.get('O_dmsstatus_DESC') === "检出") { return 'doc-check-out'; }
                                //                else if (record.get('O_dmsstatus_DESC') === "最终状态") {
                                //                    return 'doc-final';
                                //                }
                                //            } else {
                                //                return 'doc-read-only'
                                //            }
                                //        },
                                //        tooltip: '',
                                //        handler: function (grid, rowIndex, colIndex) {
                                //            //点击图标的时候选中行
                                //            grid.getSelectionModel().select(rowIndex, true);
                                //        },
                                //        listeners: {
                                //            "dragstart": function (event) {
                                //                // 存储拖拽数据和拖拽效果...
                                //                event.dataTransfer.setData("Text", "22");
                                //                //点击图标的时候选中行
                                //                grid.getSelectionModel().select(rowIndex, true);
                                //            }

                                //        }
                                //    }, {

                                //        getClass: function (v, metaData, record) {

                                //            var filename = record.get('O_filename');
                                //            var extindex = filename.lastIndexOf('.');
                                //            if (extindex >= 0) {
                                //                var extname = (filename.substring(extindex + 1)).toLowerCase();

                                //                //检查此样式是否已存在
                                //                var allExtName = ",pdf,doc,docx,xls,xlsx,dwg,rar,zip,tar,cab,gz,7z,iso,thm,";
                                //                if (allExtName.indexOf("," + extname+",") < 0)
                                //                {
                                //                    return 'docunknown'
                                //                }

                                //                return 'doc-' + extname + '-ico';
                                //            }
                                //            else { return 'docunknown'}//alert-col' }



                                //        },
                                //        tooltip: '',
                                //        handler: function (grid, rowIndex, colIndex) {
                                //            //点击图标的时候选中行
                                //            grid.getSelectionModel().select(rowIndex, true);
                                //        },
                                //        listeners: {
                                //            "dragstart": function (event) {
                                //                // 存储拖拽数据和拖拽效果...
                                //                event.dataTransfer.setData("Text", "22");
                                //                //点击图标的时候选中行
                                //                grid.getSelectionModel().select(rowIndex, true);
                                //            }

                                //        }
                                //    }]
                                //},
                {
                    text: '名称', dataIndex: 'Title', flex:1,//width: 300,
                    draggable: true, renderer: function (value, metaData, record) {
                        //添加提示
                        if (null != value) {
                            metaData.tdAttr = 'data-qtip="' + value + '"';
                            return value;
                        } else {
                            return null;
                        }
                    }
                }
                //,
                //{ text: '文件状态', dataIndex: 'O_dmsstatus_DESC', width: 90 },
                //{ text: '创建者', dataIndex: 'Creater', width: 120 },
                //{ text: '创建时间', dataIndex: 'O_credatetime', width: 90 },
                //{ text: '更新者', dataIndex: 'Updater', width: 120 },
                //{ text: '更新时间', dataIndex: 'O_updatetime', width: 90 },
                //{ text: '文件大小', dataIndex: 'O_size', width: 90 },
                //{ text: '文件名', dataIndex: 'O_filename', width: 90 },
                //{ text: '版本', dataIndex: 'O_version', width: 90 },
                //{ text: '输出路径', dataIndex: 'O_outpath', width: 90 },
                //{ text: '锁定时间', dataIndex: 'O_flocktime', width: 90 },
                //{ text: '计算机', dataIndex: 'O_conode', width: 90 }

            ],
            listeners: {
                "itemcontextmenu": function (view, record, item, index, e, eOpts) {//添加右键菜单事件

                   // me._showContextMenu(view, e);
                },

                "itemmouseleave": function (view, record, item, index, e, eOpts) {

                },
                //取消选择一条记录后触发。
                "deselect": function (view, record, index, eOpts) {
                    me.onDocGridItemSelect(view, record, index, eOpts);
                },
                //选择完一条记录后触发。
                "select" : function (view, record, index, eOpts) {
                    me.onDocGridItemSelect(view, record, index, eOpts);
                },

                "afterrender": function () {//完成渲染后的事件
                    
                }
            }
            //,
            //flex: 1
        }
        );

        var browserType = me.getBrowserType();

        //添加地址栏text
        me.addressField = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", labelWidth: 0, margins: '1 5 1 1',
            anchor: "80%", labelAlign: "right", flex: 1,
            listeners: {
                'focus': function () {
                    //点击时选中全部字符
                    this.selectText();  
                },
                'specialkey': function (field, e) {
                    //侦听回车事件，转到地址
                    if (e.getKey() == Ext.EventObject.ENTER) {
                        //Ext.Msg.alert("错误信息", "aa");
                        me.onAddressFieldPressEnter();
                    }
                }
            }  
        });

        me.maindocpanel = Ext.create('Ext.Panel', {
            xtype: 'panel',
            layout: {
                type: 'vbox',
                align: 'stretch',
                pack: 'start'
            },
            border: false, margin: '0 0 0 0',
            //height: '100%', //height: 150,
            flex: 1,
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
                        me.maindocgrid
                    ]
                }
            ]

        });

        //添加属性TAB页面到容器
        me.items = [
            me.maindocpanel

        ];


        me.callParent(arguments);
    },
    

    //刷新文档列表,docKeyword:加载完成后需要跳转的的文档Keyword
    loadDocListStore: function (callBackFun) {
        var me = this;

        var viewTree = me.maindocgrid.up('_mainProjectView').down('_mainProjectTree').down('treepanel');//获取目录树控件ID

        var nodes = viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {
            var projectKeyword = nodes[0].data.Keyword;
            //获取文档列表
            var store = me._DocListStore;//路径：\simplecdms\scripts\app\store\contents.js
            store.proxy.extraParams.ProjectKeyWord = projectKeyword;//把参数传给C#MVC,路径：\simplecdms\controllers\projectcontroller.cs 下的 GetChildProject()
            store.proxy.extraParams.filterJson = "";
            store.proxy.extraParams.sid = localStorage.getItem("sid");
            //store.loadPage(1);
            store.load({
                callback: function (records, options, success) {
                    callBackFun();
                }
            });
        }
    },

    //响应选中或取消选中文档事件
    onDocGridItemSelect: function (view, record, index, eOpts) {
        var me = this;

        //判断浏览器的类型，如果使用cefSharp浏览，通知cefSharp选中了文件夹
        if (_browserName == "cefSharp") {
            var records = me.maindocgrid.getSelectionModel().getSelection();
            var recordList = "";
            for (var i = 0; i < records.length ; i++) {
                recordList = (i === 0 ? recordList : recordList + ",");
                recordList = recordList + records[i].data.Keyword;
            }
            jsObj.onDocGridItemSelect(localStorage.getItem("sid"), recordList, me.id);
        }
    },

    //响应查询文档列表
    selectDocList: function (filterObj) {
        var me = this;
        var viewTree = me.maindocgrid.up('_mainProjectView').down('_mainProjectTree').down('treepanel');//获取目录树控件ID

        var nodes = viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {
            var projectKeyword = nodes[0].data.Keyword;

            var filterJson = Ext.JSON.encode(filterObj);

            //获取文档列表
            var store = me._DocListStore;//路径：\simplecdms\scripts\app\store\contents.js
            store.proxy.extraParams.ProjectKeyWord = projectKeyword;//把参数传给C#MVC,路径：\simplecdms\controllers\projectcontroller.cs 下的 GetChildProject()
            store.proxy.extraParams.filterJson = filterJson;
            store.proxy.extraParams.sid = localStorage.getItem("sid");
            store.loadPage(1);
        }
    },

    //设置文档列表底部消息
    setDocGridBBarMsg: function (msg) {
        var me = this;
        var txtEL = me.maindocgrid.el.dom.children[4].children["0"].children["0"].children[12];

        //固定左边距
        txtEL.style.left = "300px";
        txtEL.innerHTML = msg;
        
    },



    /**
* 向tab中添加选项卡
* @params myId 被添加的组件id  myTitle 创建tabpanel时需要 myurl 将要加载数据的地址
*/
    addTab: function (docKeyword, myTitle) {//, myUrl) {
        var me = this;

        var mainTabPanel = me.up('mainpanel');

        var myId = "previewtab" + docKeyword;

        var gid = Ext.getCmp(myId);
        if (gid != null) {
            mainTabPanel.remove(gid.id); // 如果该选项卡面板里已有选项卡，先将其移除
        }

        var fmPreviewFile = Ext.create('Ext.ux.m.File._PreviewFile', {
            ///title: "123",
            mainPanelId: me.id, DocKeyword: docKeyword
        });

        var tp = new Ext.panel.Panel({
            //iconCls: 'tab',
            id: myId,
            //enableTabScroll: true,
            closable: true,
            title: myTitle,
            layout: 'fit',
            items: fmPreviewFile
        });
        // 向选项卡面板里添加选项卡
        mainTabPanel.add(tp).show();
    },

    //获取浏览器类型
    getBrowserType: function () {
        var userAgent = navigator.userAgent; //取得浏览器的userAgent字符串
        var isOpera = userAgent.indexOf("Opera") > -1;
        if (isOpera) {
            return "Opera"
        }; //判断是否Opera浏览器
        if (userAgent.indexOf("Firefox") > -1) {
            return "FF";
        } //判断是否Firefox浏览器
        if (userAgent.indexOf("Chrome") > -1) {
            return "Chrome";
        }
        if (userAgent.indexOf("Safari") > -1) {
            return "Safari";
        } //判断是否Safari浏览器
        if (userAgent.indexOf("compatible") > -1 && userAgent.indexOf("MSIE") > -1 && !isOpera) {
            return "IE";
        }; //判断是否IE浏览器
    },


    //显示右键菜单方法
    _showContextMenu: function (view, e, eOpts) {
        var me = this;
        //显示右键菜单
        var menus = Ext.widget('_contextmenu');

        menus.showMainPanelMenu(me.maindocgrid, e);

    }


});

//外部JS调用Extjs函数
Ext.setDocGridBarMsg = function (gridPanelId, msg) {
    //alert("接收到刷新目录树消息！请手动刷新目录树!!");
    //var me = this;
    //me.RefreshProjTree("LastProject");
    var view = Ext.getCmp(gridPanelId);
    view.setDocGridBBarMsg(msg);
}




