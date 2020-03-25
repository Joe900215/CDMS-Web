/*定义文档列表grid*/
Ext.define('Ext.ux.YDForm._MainDocGrid', {
    //extend: 'Ext.container.Container',
    extend: 'Ext.panel.Panel',
    alias: 'widget._mainDocGrid', // 此类的xtype类型为buttontransparent  
    //requires: ['Ext.ux.Common.comm'],
    region: "center",
    layout: 'fit',
    initComponent: function () {
        var me = this;
        me.renderTo = me.el;

        me.isMouseDown = false;

        //记录文件路径
        me.ServerFullFileName = "";

        //定义文档列表store
        me._DocListStore = Ext.create("Ext.data.Store", {
            model: 'CDMSWeb.model._DocList',//模型路径：\simplecdms\scripts\app\model\content.js
            batchActions: false,

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
                },
                listeners: {
                    exception: CDMSWeb.ProxyException
                }
            },
            //排序
            remoteSort: true,
            sortInfo: {
                field: 'name',
                direction: 'ASC'
            }
            //文章的Store需要支持远程排序和搜索
            //remoteFilter: true,
            //remoteSort: true
        });

        //定义下载文件按钮
        me.FileDownLoadButton = Ext.create('Ext.button.Button', {
            //id: 'DownLoadFileButton',
            text: '下载',
            iconCls: "file-download", scope: me, tooltip: '下载文件',// hidden: true,
            listeners: {
                "click": function (btn, e, eOpts) {
                    me.onDownLoadFile();
                }
            }
        });

        //定义上传文件按钮
        me.fpFileUploadButtonXhr = Ext.create('Ext.ux.upload.Button', {
            //renderTo: Ext.getBody(),
            //id: 'fpFileUploadButtonXhr',
            //text: '上传文件',
            iconCls: "file-upload", scope: me, tooltip: '上传文件', //hidden: true,
            text: '上传',
            plugins: [{
                ptype: 'ux.upload.window',
                title: '上传文件',
                width: 520,
                height: 350
            }
            ],
            uploader:
            {
                url: 'WebApi/Post',
                uploadpath: '/Root/files',
                autoStart: true,///选择文件后是否自动上传
                max_file_size: '20020mb',
                multipart_params: { 'guid': '', 'ProjectKeyword': '', 'sid': '', 'C': 'AVEVA.CDMS.WebApi.FileController', 'A': 'UploadFile', 'SureReplace': 'false', 'AppendFilePath': '', 'user': '' }, //设置你的参数//{},
                drop_element: null,//定义好docgrid后再设置拖放控件
                statusQueuedText: '准备上传',
                //statusUploadingText: '上传中 ({0}%)',
                statusUploadingText: '上传中',
                statusFailedText: '<span style="color: red">错误</span>',
                statusDoneText: '<span style="color: green">已完成</span>',

                statusInvalidSizeText: '文件过大',
                statusInvalidExtensionText: '错误的文件类型'
            },
            listeners:
            {
                uploadstarted: function (uploader, files) {

                },

                filesadded: function (uploader, files) {
                    uploader.multipart_params.ProjectKeyword = me.maindocgrid.store.proxy.extraParams.ProjectKeyWord;

                    uploader.multipart_params.sid = localStorage.getItem("sid");

                    //只要是拖拽，无论是文件和目录都传给C#端处理
                    for (var i = 0; i < files.length ; i++) {
                        //判断浏览器的类型，是否使用cefSharp浏览
                        if (_browserName == "cefSharp") {

                            //只要是拖拽，无论是文件和目录都传给C#端处理
                            jsObj.onDocGridDropFolder(localStorage.getItem("sid"), me.maindocgrid.store.proxy.extraParams.ProjectKeyWord);

                            //从文件列表里删除文件夹
                            files = files.splice(i, 1)[0];
                            return false;
                        }
                    }
                },

                beforeupload: function (uploadbasic, uploader, file) {
                    //console.log('beforeupload');			
                    //上传文件而不是替换文件时，这里要把Doc关键字重置
                    uploadbasic.multipart_params.DocKeyword = "";

                    var extIndex = file.name.lastIndexOf(".");
                    var fileCode = file.name;
                    //if (extIndex >= 0 && fileCode.toLowerCase().endWith != '.sqlite')
                    //if (extIndex >= 0 && fileCode.endsWith(".sqlite") != true) {
                    if (extIndex >= 0 && fileCode.indexOf(".sqlite") >= 0) {
                        fileCode = fileCode.substring(0, extIndex);
                    }

                    me.beforeCreateDoc(uploadbasic, file, me.maindocgrid.store.proxy.extraParams.ProjectKeyWord, fileCode, function () {
                        //先创建文档
                        // me.createDoc(uploadbasic,file,me.maindocgrid.store.proxy.extraParams.ProjectKeyWord, fileCode, "", "");

                        Ext.require('Ext.ux.Common.comm', function () {
                            //先创建文档

                            createDoc(uploadbasic, file, me.maindocgrid.store.proxy.extraParams.ProjectKeyWord, fileCode, "", "", function (uploadbasic, res, options, DocKeyword) {
                                //处理创建文档后的返回事件
                                if (res.success === true) {

                                    //更新DOC到表格
                                    var grid = me.up('_mainSourceView').down('_mainDocGrid').down("gridpanel");
                                    grid.store.add({
                                        Keyword: res.data[0].Keyword,
                                        O_itemno: res.data[0].O_itemno,
                                        Title: res.data[0].Title,
                                        O_size: res.data[0].O_size,
                                        O_filename: res.data[0].O_filename,
                                        O_dmsstatus_DESC: res.data[0].O_dmsstatus_DESC,
                                        O_version: res.data[0].O_version,
                                        Creater: res.data[0].Creater,
                                        O_credatetime: res.data[0].O_credatetime,
                                        Updater: res.data[0].Updater,
                                        O_updatetime: res.data[0].O_updatetime,
                                        O_outpath: res.data[0].O_outpath,
                                        O_flocktime: res.data[0].O_flocktime,
                                        O_conode: res.data[0].O_conode,
                                        IsShort: res.data[0].IsShort,
                                        WriteRight: res.data[0].WriteRight

                                    });

                                    //触发服务端BeforeUploadFile事件，如果是断点续传，获取已上传的文件大小
                                    var boolSend = sendBeforeUploadFile(uploadbasic, options.file, DocKeyword, me);
                                    return boolSend;
                                } else {

                                    var errmsg = res.msg;
                                    Ext.Msg.alert("错误信息", errmsg);

                                    //setFileUploadError(uploadbasic, options.file);
                                    return false;
                                }
                            });
                        });
                    });
                },

                chunkuploaded: function (basic, uploader, file, result, state) {

                },

                //单个文件上传完毕事件
                fileuploaded: function (uploader, file) {
                    ////这里用common里面的事件会导致第一次上传时属性不正常显示
                    me.afterUploadFile(uploader, file, me.ServerFullFileName);
                    //文件上传后的事件
                    //Ext.require('Ext.ux.Common.comm', function () {
                    //    var result = afterUploadFile(uploader, file, me.ServerFullFileName);
                    //    return result;
                    //});
                },

                uploaderror: function (uploader, result) {
                    //文件上传失败事件

                },

                uploadcomplete: function (uploader, success, failed) {
                    var store = me.maindocgrid.store;//刷新DOC列表
                    //store.loadPage(1);
                    store.load();
                },
                scope: this
            }


        });

        //定义上传替换文件按钮
        me.ReplaceDocButton = Ext.create('Ext.ux.upload.Button', {
            //text: '上传文件',
            hidden: true,
            iconCls: "file-upload", scope: me, tooltip: '上传替换文件',
            plugins: [{
                ptype: 'ux.upload.window',
                title: '上传文件',
                width: 520,
                height: 350
            }
            ],
            uploader:
            {
                url: 'WebApi/Post',
                uploadpath: '/Root/files',
                autoStart: true,///选择文件后是否自动上传
                max_file_size: '20020mb',
                multi_selection: false,//每次只能上传一个文档
                multipart_params: { 'guid': '', 'ProjectKeyword': '', 'sid': '', 'DocKeyword': '', 'C': 'AVEVA.CDMS.WebApi.FileController', 'A': 'UploadFile', 'SureReplace': 'false', 'AppendFilePath': '', 'user': '' }, //设置你的参数//{},

                //drop_element: 'fpFileUploadButtonXhr',
                statusQueuedText: '准备上传',
                statusUploadingText: '上传中 ({0}%)',
                statusFailedText: '<span style="color: red">错误</span>',
                statusDoneText: '<span style="color: green">已完成</span>',

                statusInvalidSizeText: '文件过大',
                statusInvalidExtensionText: '错误的文件类型'
            },
            listeners:
            {
                uploadstarted: function (uploader, files) {

                },

                filesadded: function (uploader, files) {
                    var nodes = me.maindocgrid.getSelectionModel().getSelection();//获取已选取的节点
                    if (nodes !== null && nodes.length > 0) {
                        //uploader.multipart_params.ProjectKeyword = me.maindocgrid.store.proxy.extraParams.ProjectKeyWord;

                        uploader.multipart_params.DocKeyword = nodes[0].data.Keyword;
                        uploader.multipart_params.sid = localStorage.getItem("sid");
                    }
                },

                beforeupload: function (uploadbasic, uploader, file) {

                    //触发服务端BeforeUploadFile事件，如果是断点续传，获取已上传的文件大小
                    // me.sendBeforeUploadFile(uploadbasic, file, uploadbasic.multipart_params.DocKeyword);

                    Ext.require('Ext.ux.Common.comm', function () {
                        //触发服务端BeforeUploadFile事件，如果是断点续传，获取已上传的文件大小
                        //sendBeforeUploadFile(uploadbasic, options.file, DocKeyword, me);
                        sendBeforeUploadFile(uploadbasic, file, uploadbasic.multipart_params.DocKeyword, me);
                    });
                },

                //单个文件上传完毕事件
                fileuploaded: function (uploader, file) {
                    //这里用common里面的事件会导致第一次上传时属性不正常显示
                    me.afterUploadFile(uploader, file, me.ServerFullFileName);
                    //文件上传后的事件
                    //Ext.require('Ext.ux.Common.comm', function () {
                    //    afterUploadFile(uploader, file, me.ServerFullFileName);
                    //});
                },

                //文件上传失败事件
                uploaderror: function (uploader, result) {

                },

                uploadcomplete: function (uploader, success, failed) {
                    //刷新属性页
                    me.loadAttrPage("selectdoc");
                },
                scope: this
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
                        { name: 'o_itemname', value: strSQL },
                        { name: 'searchAttr', value: "true" },
                        { name: 'filterstr', value: value }
                        
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

        //定义目录树按钮
        me._docListTbar = Ext.create('Ext.toolbar.Toolbar', {
            xtype: 'toolbar',
            dock: 'top',
            items: [
                //{
                //    iconCls: "file-create", scope: me, tooltip: '创建文档', listeners: {
                //        "click": function (btn, e, eOpts) {
                //            me.CreateNewDoc();
                //        }
                //    }
                //},
                //{
                //    iconCls: "file-batchcreate", scope: me, tooltip: '批量创建文档', listeners: {
                //        "click": function (btn, e, eOpts) {
                //            me.BatchCreateDoc();
                //        }
                //    }
                //},
                {
                    iconCls: "file-preview", scope: me, tooltip: '预览文件',
                    text:'预览',
                    listeners: {
                        "click": function (btn, e, eOpts) {
                            //me.BatchCreateDoc();
                            me.PreviewDoc("false", "false", "", "");
                        }
                    }
                },
                {
                    iconCls: "workflow-start", scope: me, tooltip: '启动流程', hidden: true, listeners: {
                        "click": function (btn, e, eOpts) {
                            me._StartProcess();
                        }
                    }
                },

                {
                    iconCls: "file-modiattr", scope: me, tooltip: '编辑文档属性', hidden:true, listeners: {
                        "click": function (btn, e, eOpts) {
                            me.ModiDocAttr();
                        }
                    }
                },//, disabled: true }
                {
                    iconCls: "file-delete", scope: me, tooltip: '删除文档', hidden: true, listeners: {
                        "click": function (btn, e, eOpts) {
                            me.DeleteDoc("false");
                        }
                    }
                },
                me.fpFileUploadButtonXhr,
                me.FileDownLoadButton,
                //替换文件按钮，在文档的属性栏触发
                me.ReplaceDocButton,
               // ,
                //{ 1   
                //    iconCls: "file-upload", scope: me, tooltip: '上传文件', listeners: {
                //        "click": function (btn, e, eOpts) {
                //            //me.onDownLoadFile();
                //            me.fpFileUploadButtonXhr.fireEvent('click');
                //        }
                //    }
                //},
               //{
               //    iconCls: "file-download", scope: me, tooltip: '下载文件', listeners: {
               //        "click": function (btn, e, eOpts) {
               //            me.onDownLoadFile();
               //        }
               //    }
               //},
                //{
                //    iconCls: "refresh", scope: me, tooltip: '刷新', name2: 'DocGridRefreshBtn', listeners: {
                //        "click": function (btn, e, eOpts) {
                //            Ext.Ajax.request({
                //                url: 'WebApi/Post',
                //                method: "POST",
                //                params: {
                //                    C: "AVEVA.CDMS.WebApi.DBSourceController", A: "refreshDBSource",
                //                    sid: localStorage.getItem("sid")
                //                },
                //                success: function (response, options) {
                //                    me.loadDocListStore(function () { });
                //                },
                //                failure: function (response, options) {
                //                    ////Ext.Msg.alert("错误", "连接服务器失败！<br>" + response.responseText);
                //                }
                //            });
                //            //me.setDocGridBBarMsg(me.id);
                //        }
                //    }
                //}
                ,
                '-',
                {
                    iconCls: "project-create", scope: me, text: '项目立项', tooltip: '项目立项', listeners: {
                        "click": function (btn, e, eOpts) {
                            me.CreateRootProject();
                        }
                    }
                },
                //{
                //    iconCls: "project-create", scope: me, tooltip: '项目立项', listeners: {
                //        "click": function (btn, e, eOpts) {
                //            me.CreateHXEPCRootProject();
                //        }
                //    }
                //},
                {
                    iconCls: "task-report", scope: me, tooltip: '工日填报', name2: 'TaskReportBtn', listeners: {
                        "click": function (btn, e, eOpts) {
                            me.TaskReport();

                            //me.setDocGridBBarMsg(me.id);
                        }
                    }
                },
                {
                    iconCls: "file-modiattr", scope: me, tooltip: '未完成流程', hidden: true, listeners: {
                        "click": function (btn, e, eOpts) {
                            me.PreviewDoc("false", "false", "", "");
                            Ext.require('Ext.plug_ins.' + 'HXEPC_Plugins' + '.common', function () {
                                searchUnfinishWFDocList(me.id);
                            });
                        }
                    }
                }

            ]
        });


        //定义grid文章列表
        me.maindocgrid = Ext.widget("grid", {
            //title: "文档列表", 
            //id: "_DocGrid",
            store: me._DocListStore,//"Contents",//
            //Grid使用了复选框作为选择行的方式
            selType: "checkboxmodel",
            selModel: { checkOnly: false, mode: "MULTI" }, //"SIMPLE"},//"MULTI" },
            tbar: me._docListTbar,
            //remoteSort: true, //设置属性进行请求后台排序  
            //draggable: "true",
            //autoHeight: true,
            //enableDragDrop:false,
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
                                    menuDisabled: true,
                                    sortable: false,
                                    xtype: 'actioncolumn',
                                    enableColumnResize: false,
                                    width: 42,
                                    overCls: 'cursor-default',//CSS类名

                                    items: [{
                                        overCls: 'cursor-default',//CSS类名
                                        getClass: function (v, metaData, record) {
                                            var wRight = record.get('WriteRight');
                                            if (wRight === 'true') {//判断是否有写文件权限
                                                if (record.get('O_dmsstatus_DESC') === "检入") {
                                                    return 'doc-check-in';
                                                }
                                                else if (record.get('O_dmsstatus_DESC') === "检出") { return 'doc-check-out'; }
                                                else if (record.get('O_dmsstatus_DESC') === "最终状态") { return 'doc-final'; }
                                            } else {
                                                return 'doc-read-only'
                                            }
                                        },
                                        tooltip: '',
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
                                    }
                                    , {
                                        overCls: 'cursor-default',//CSS类名
                                        getClass: function (v, metaData, record) {
                                            var isShort = record.data.IsShort;//get('IsShort');
                                            if (isShort === "true") {
                                                return 'docatta'
                                            }

                                            var filename = record.get('O_filename');
                                            var extindex = filename.lastIndexOf('.');
                                            if (extindex >= 0) {
                                                var extname = (filename.substring(extindex + 1)).toLowerCase();

                                                //检查此样式是否已存在
                                                var allExtName = ",pdf,doc,docx,xls,xlsx,dwg,rar,zip,tar,cab,gz,7z,iso,thm,";
                                                if (allExtName.indexOf("," + extname+",") < 0)
                                                { return 'docunknown';}

                                                return 'doc-' + extname + '-ico';
                                            }
                                            else { return 'docunknown'}//alert-col' }

                                           

                                        },
                                        tooltip: '',
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
                                    }
                                    ]
                                },
                {
                    text: '名称', dataIndex: 'Title', width: 300, draggable: true, renderer: function (value, metaData, record) {
                        //添加提示
                        if (null != value) {
                            metaData.tdAttr = 'data-qtip="' + value + '"';
                            return value;
                        } else {
                            return null;
                        }
                    }
                },
                { text: '文件状态', dataIndex: 'O_dmsstatus_DESC', width: 90 },
                { text: '创建者', dataIndex: 'Creater', width: 120 },
                {
                    text: '创建时间', dataIndex: 'O_credatetime', width: 90//, sortable: true //设置这个属性进行排序   
                },
                { text: '更新者', dataIndex: 'Updater', width: 120 },
                { text: '更新时间', dataIndex: 'O_updatetime', width: 90 },
                { text: '文件大小', dataIndex: 'O_size', width: 90 },
                { text: '文件名', dataIndex: 'O_filename', width: 90 },
                { text: '版本', dataIndex: 'O_version', width: 90 },
                { text: '输出路径', dataIndex: 'O_outpath', width: 90 },
                { text: '锁定时间', dataIndex: 'O_flocktime', width: 90 },
                { text: '计算机', dataIndex: 'O_conode', width: 90 }

            ],

            listeners: {
                //添加点击文档事件
                "itemclick": function (view, record, item, index, e) {
                    //var me = this;



                },
                "itemdblclick": function (view, record, item, index, e) {//添加双击文档事件

                    //判断浏览器的类型，是否使用cefSharp浏览
                    if (_browserName == "cefSharp") {
                        //找到文件状态所在图标的DOM对象
                        var viewImg = view.el.dom.childNodes["0"].children["0"].childNodes[index + 1].children[1].children["0"].childNodes["0"];

                        //获取原来的Class状态
                        var oldClass = viewImg.getAttribute("class");


                        //调用C#定义事件
                        jsObj.OnDocGridDbClick(localStorage.getItem("sid"), record.data.Keyword, view.id, index, oldClass);


                    }
                    else if (localStorage.getItem("_browserName") == "WebBrowser") {
                        //调用C#定义事件
                        window.external.OnDocGridDbClick(localStorage.getItem("sid"), record.data.Keyword);
                    }
                    else {
                        me.FileDownLoadButton.fireEvent('click');
                    }
                    //Ext.Msg.alert("错误信息", "3");
                },
                "itemcontextmenu": function (view, record, item, index, e, eOpts) {//添加右键菜单事件
                    me._showContextMenu(view, record, item, index, e);
                },
                "itemmousedown": function (view, record, item, index, e, eOpts) {
                    //Ext.Msg.alert("错误信息", "2");
                    me.isMouseDown = true;
                },
                "itemmouseup": function (view, record, item, index, e, eOpts) {
                    //Ext.Msg.alert("错误信息", "2");
                    me.isMouseDown = false;
                },
                "itemmouseleave": function (view, record, item, index, e, eOpts) {

                },
                //取消选择一条记录后触发。
                "deselect": function (view, record, index, eOpts) {
                    me.onDocGridItemSelect(view, record, index, eOpts);
                },
                //选择完一条记录后触发。
                "select": function (view, record, index, eOpts) {

                    me.onDocGridItemSelect(view, record, index, eOpts);
                },

                "afterrender": function () {//完成渲染后的事件
                    
                    //var mainTreeView = me.up('_mainSourceView').down('_mainProjectTree');
                    ////var docgrid = me.up('_mainSourceView').down('_mainDocGrid');
                    //var h=mainTreeView.getHeight();
                    //me.maindocgrid.setHeight(h - 20);
                }
            },
            flex: 1
        }
        );

        var browserType = me.getBrowserType();

        //if (browserType != "IE") {
            //设置上传按钮的拖拽控件
            me.fpFileUploadButtonXhr.uploader.drop_element = me.maindocgrid.id;
        //}

        //var dgrid=document.getElementById(me.maindocgrid.id);
        //dgrid.draggable = "true";
        //document.getElementById(me.maindocgrid.id).addEventListener("dragstart", function (event) {
        //    // 存储拖拽数据和拖拽效果...
        //    //event.dataTransfer.setData("Text", ev.target.id);
        //}, false);

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

        //添加属性TAB页面到容器
        me.items = [
            			//{
            			//    xtype:"button",
            			//    text:"我的按钮"
            			//}
                        {
                            layout: "fit",
                            items: [
                                {
                                    xtype: "panel", margin: '0 0 0 0',
                                    //title: "任务内容",
                                    baseCls: 'my-panel-no-border',//隐藏边框
                                    layout: {
                                        type: 'vbox',
                                        pack: 'start',
                                        align: 'stretch'
                                    },
                                    items: [
                                        {
                                            xtype: "panel", margin: '0 0 0 0',
                                            //title: "任务内容",
                                            baseCls: 'my-panel-no-border',//隐藏边框
                                            layout: {
                                                type: 'hbox',
                                                pack: 'start',
                                                align: 'stretch'
                                            },
                                            items: [
                                                me.addressField,
                                                    me.searchTrigger
                                            ]},                                        
                                            {
                                                xtype: "panel", margin: '0 0 0 0',
                                                //title: "任务内容",
                                                baseCls: 'my-panel-no-border',//隐藏边框
                                                layout: {
                                                    type: 'hbox',
                                                    pack: 'start',
                                                    align: 'stretch'
                                                },
                                                //layout: 'fit',
                                                items: [
                                                    me.maindocgrid
                                                ]}
                                        
                                    ]
                                }

                            ]
                        }

        ];

        //设置 grid 自适应高度

        me.maindocgrid.setHeight(document.body.clientHeight - 155);

        window.onresize = function () {

            //var mainTreeView = me.up('_mainSourceView').down('_mainProjectTree');
            //var attrTabView = me.up('_mainSourceView').down('_mainAttrTab');
            
            //var treeWidth = mainTreeView.width;
            //var tabWidth = attrTabView.el.dom.offsetWidth;

            //var gridWidth = document.body.clientWidth - treeWidth - tabWidth;
            //me.maindocgrid.setWidth(gridWidth); 
            me.maindocgrid.setHeight(document.body.clientHeight - 155);
        };

        me.callParent(arguments);
    },

    //创建文档docCode：文件代码, docDesc：文件描述, docTemp：文件模板
    createDoc: function (uploadbasic,file, ProjectKeyword, docCode, docDesc, docTemp) {
        var me = this;
        //var myFile = file;
        var docAttr =
        [{ name: 'docCode', value: docCode },
            { name: 'docDesc', value: docDesc },
            { name: 'tempDefnId', value: "" }
        ];
        var docAttrJson = Ext.JSON.encode(docAttr);

        Ext.Ajax.request({

            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.WebApi.DocController", A: "CreateDoc",
                sid: localStorage.getItem("sid"), projectKeyword: ProjectKeyword, docAttrJson: docAttrJson,
            },
            file:file,
            success: function (response, options) {
                var res = Ext.JSON.decode(response.responseText, true);
                var state = res.success;
                if (state === false) {
                    var errmsg = res.msg;
                    Ext.Msg.alert("错误信息", errmsg);
                    return false
                }
                else {
                    //me.createDocCallBack(uploadbasic, res);

                    uploadbasic.multipart_params.DocKeyword = res.data[0].Keyword;

                    //更新DOC到表格
                    var grid = me.up('_mainSourceView').down('_mainDocGrid').down("gridpanel");
                    //var grid = Ext.getCmp(me.mainPanelId).up('_mainSourceView').down('_mainDocGrid').down("gridpanel");
                    grid.store.add({
                        Keyword: res.data[0].Keyword,
                        O_itemno: res.data[0].O_itemno,
                        Title: res.data[0].Title,
                        O_size: res.data[0].O_size,
                        O_filename: res.data[0].O_filename,
                        O_dmsstatus_DESC: res.data[0].O_dmsstatus_DESC,
                        O_version: res.data[0].O_version,
                        Creater: res.data[0].Creater,
                        O_credatetime: res.data[0].O_credatetime,
                        Updater: res.data[0].Updater,
                        O_updatetime: res.data[0].O_updatetime,
                        O_outpath: res.data[0].O_outpath,
                        O_flocktime: res.data[0].O_flocktime,
                        O_conode: res.data[0].O_conode,
                        IsShort: res.data[0].IsShort,
                        WriteRight: res.data[0].WriteRight

                    });

                    var DocKeyword = res.data[0].Keyword;
                    //触发服务端BeforeUploadFile事件，如果是断点续传，获取已上传的文件大小
                    // me.sendBeforeUploadFile(uploadbasic, options.file, DocKeyword);


                    Ext.require('Ext.ux.Common.comm', function () {
                        //触发服务端BeforeUploadFile事件，如果是断点续传，获取已上传的文件大小
                        sendBeforeUploadFile(uploadbasic, options.file, DocKeyword, me);
                    });

                    return true;
                }
            },
            failure: function (response, options) {
                ////Ext.Msg.alert("错误", "连接服务器失败！<br>" + response.responseText);
            }
        });
    },

    ////触发服务端BeforeUploadFile事件，如果是断点续传，获取已上传的文件大小
    //sendBeforeUploadFile: function (uploadbasic, file, DocKeyword) {
    //    var me = this;
    //    var ServerFileName = file.name;
    //    var CreateDate = "";
    //    var ModifyDate = file.updatetime;
    //    var fileSize = file.size;
    //    var MD5 = "";
    //    Ext.Ajax.request({

    //        url: 'WebApi/Post',
    //        method: "POST",
    //        params: {
    //            C: "AVEVA.CDMS.WebApi.FileController", A: "BeforeUploadFile",
    //            sid: localStorage.getItem("sid"), DocKeyword: DocKeyword,
    //            ServerFileName: ServerFileName, CreateDate: CreateDate,
    //            ModifyDate: ModifyDate, fileSize: fileSize, MD5: MD5
    //        },
    //        file: file,
    //        success: function (response, options) {
    //            var res = Ext.JSON.decode(response.responseText, true);
    //            var state = res.success;
    //            if (state === false) {
    //                var errmsg = res.msg;
    //                //Ext.Msg.alert("错误信息", errmsg);
    //                return "false";
    //            }
    //            else {
    //                //获取服务端返回的状态值
    //                var reValue = parseInt(res.data[0].resultValue);
    //                if (reValue > 0) {
    //                    options.file.lastloaded = reValue;
    //                    options.file.loaded = reValue;
    //                }

    //                uploadbasic.multipart_params.ServerFullFileName = res.data[0].ServerFullFileName;
    //                //me.ServerFullFileName = res.data[0].ServerFullFileName;

    //                //设置文件状态，开始上传文件
    //                options.file.status = 2;

    //                return res.data[0].ServerFullFileName;
    //            }
    //        }
    //    });
    //},

    //处理创建doc前的事件
    beforeCreateDoc:function(uploadbasic, file, ProjectKeyWord, fileCode, callBackFun){
        var me = this;
        Ext.Ajax.request({

            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.WebApi.DocController", A: "BeforeCreateDoc",
                sid: localStorage.getItem("sid"), ProjectKeyword: ProjectKeyWord, FileName: file.name
            },
            success: function (response, options) {
                var res = Ext.JSON.decode(response.responseText, true);
                var state = res.success;
                if (state === false) {
                    var errmsg = res.msg;


                    //把文件设置为上传错误(上传完成)
                    file.status = 4;

                    uploadbasic.uploader.stop();
                    uploadbasic.uploader.start();

                    Ext.Msg.alert("错误信息", errmsg);
                    //return false
                }
                else {
                    //uploadbasic.multipart_params.ServerFullFileName = res.data[0].ServerFullFileName;
                    //me.OnAfterCreateNewDocEvent(uploader, file);
                    callBackFun();
                }
            },
            failure: function (response, options) {
                ////Ext.Msg.alert("错误", "连接服务器失败！<br>" + response.responseText);
            }
        });
    },

    createDocCallBack: function (uploadbasic, res) {
        var me = this;
    },

    afterUploadFile: function (uploader, file, ServerFullFileName) {
        var me = this;
        var DocKeyword = uploader.multipart_params.DocKeyword;

        Ext.Ajax.request({

            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.WebApi.FileController", A: "AfterUploadFile",
                sid: localStorage.getItem("sid"), DocKeyword: DocKeyword, ServerFullFileName: ServerFullFileName
            },
            success: function (response, options) {
                var res = Ext.JSON.decode(response.responseText, true);
                var state = res.success;
                if (state === false) {
                    var errmsg = res.msg;
                    Ext.Msg.alert("错误信息", errmsg);
                    return false
                }
                else {
                    //uploadbasic.multipart_params.ServerFullFileName = res.data[0].ServerFullFileName;
                    me.OnAfterCreateNewDocEvent(DocKeyword, file);
                }
            },
            failure: function (response, options) {
                ////Ext.Msg.alert("错误", "连接服务器失败！<br>" + response.responseText);
            }
        });
    },

    //处理拖拽文档到收文目录的事件
    OnAfterCreateNewDocEvent: function (DocKeyword, file) {
        var me = this;
        //var DocKeyword = uploader.multipart_params.DocKeyword;
        Ext.MessageBox.wait("正在处理拖拽文件后的系统事件，请稍候...", "等待");
        Ext.Ajax.request({

            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.WebApi.DocController", A: "AfterCreateNewDocEvent",
                sid: localStorage.getItem("sid"), DocKeyword: DocKeyword
            },
            success: function (response, options) {
                var res = Ext.JSON.decode(response.responseText, true);
                var state = res.success;
                if (state === false) {
                    var errmsg = res.msg;
                    Ext.Msg.alert("错误信息", errmsg);
                    return false;
                }
                else {
                    //uploadbasic.multipart_params.ServerFullFileName = res.data[0].ServerFullFileName;
                    Ext.MessageBox.close();//关闭等待对话框

                    if (res.msg != "" && res.msg != undefined) {
                        //处理拖拽文档到收文目录的事件
                        var plugins = res.data[0].plugins;//插件名称
                        var FuncName = res.data[0].FuncName;//函数名称

                        Ext.require('Ext.plug_ins.' + plugins + '.common', function () {
                            //通过函数名调用函数
                            //这里如果直接传递uploader,因为afterUpload事件延迟的关系，uploader里面的docKeyword会因为被新的文档覆盖而出现错误，所以不直接传送uploader了
                            function runCommFun(mainPanelId, res, file) {
                                //state是需要调用的函数名
                                var CommFun = eval(FuncName);
                                new CommFun(mainPanelId, res, file);

                            }
                            runCommFun(me.id, res, file);
                        });
                    }
                }
            },
            failure: function (response, options) {
                ////Ext.Msg.alert("错误", "连接服务器失败！<br>" + response.responseText);
            }
        });
    },

    onDownLoadFile: function () {
        var me = this;
        //当一个文件双击后，在10S内不在响应双击事件
        var lastTime = localStorage.getItem("lastDownTime");
        var flag=false;
        if (lastTime == null || lastTime === "undefined")
            flag = true;
        else {
            var nowD = new Date();
            var d3 = nowD.getTime() - lastTime;
            if (d3>5000)
                flag=true;
        }


        if (flag === true) {
            var lastD = new Date();
            localStorage.setItem("lastDownTime", lastD.getTime());//记录下载的时间

            var grid = me.maindocgrid;
            var rs = grid.getSelectionModel().getSelection();//获取选择的文档

            if (rs !== null && rs.length > 0) {

                var Keyword = "";
                for (var i = 0; i < rs.length ; i++) {

                    Keyword = rs[i].data.Keyword;//获取文档关键字

                    //判断浏览器的类型，是否使用cefSharp浏览
                    if (_browserName == "cefSharp") {
                        //调用C#定义事件
                        jsObj.OnDocGridFileDownLoad(localStorage.getItem("sid"), Keyword, i);
                        //return;
                        continue;
                    }
                    //else if (_browserName == "WebBrowser") {
                    else if (localStorage.getItem("_browserName") == "WebBrowser") {
                    
                        window.external.OnDocGridFileDownLoad(localStorage.getItem("sid"), Keyword, i);
                        continue;
                    }

                    //window.open()方法只能在用户click事件的这个线程中使用。否则就会被拦截。ajax请求完成时已经是另外一个线程了。
                    //解决方法是
                    //先在用户click事件线程中用window.open()打开一个空的窗口，ajax请求完成时再改变窗口的内容。
                    //var tempwindow = window.open('_blank'); // 先打开页面

           


                    //window.downloadFile.isChrome = navigator.userAgent.toLowerCase().indexOf('chrome') > -1;
                    //window.downloadFile.isSafari = navigator.userAgent.toLowerCase().indexOf('safari') > -1;

                    //普通浏览器下载文件
                    Ext.Ajax.request({
                        url: 'WebApi/Post',
                        params: {
                            C: 'AVEVA.CDMS.WebApi.DocController', A: 'FileDownload',
                            DocKeyword: Keyword, sid: localStorage.getItem("sid")
                        },
                        success: function (res) {
                            var obj = Ext.decode(res.responseText);
                            //console.log(obj);//可以到火狐的firebug下面看看obj里面的结构   
                            if (obj.success == true) {
                                var prePath = obj.data[0].prePath;
                                var fileName = obj.data[0].filename;
                                var para = obj.data[0].para;
                                
                                //组装文件下载路径
                                var url = prePath + encodeURIComponent(fileName) + "?p=" + para;

                                //var url = encodeURIComponent(obj.data[0].path);
                                //原窗口下载，使用window.open新窗口下载，ie8会下载不了
                                //window.location.href = url;

                                //tempwindow.location = url; // 后更改页面地址

                                //window.downloadFile(url);
                                me.windowDownloadFile(url);

                                //window.open(url, '_blank');//新窗口打开链接

                                ////原窗口打开文件，会导致打开图片的时候把CDMS页面关闭掉了
                                //var popUp = window.open(url, '_blank');//新窗口打开链接
                                //if (popUp == null || typeof (popUp) == 'undefined') {
                                //    Ext.Msg.alert("下载失败", '请解除窗口阻拦，重新点击下载。');
                                //}
                                //else {
                                //    popUp.focus();
                                //}

                            } else { Ext.Msg.alert("下载失败", obj.msg); }
                            //window.location.href = obj.path;//这样就可以弹出下载对话框了  
                        },
                        failure: function (e, opt) {
                            var me = this, msg = "";
                            // Ext.Msg.alert("错误", "1");
                        }

                    });
                }
            }

        } else {
            Ext.Msg.alert("请稍候！", "下载文档后，请间隔10秒再下载其他文档！");
        }
    },



    windowDownloadFile : function (sUrl) {

        var isChrome = navigator.userAgent.toLowerCase().indexOf('chrome') > -1;
        var isSafari = navigator.userAgent.toLowerCase().indexOf('safari') > -1;

        //iOS devices do not support downloading. We have to inform user about this.
        if (/(iP)/g.test(navigator.userAgent)) {
            alert('Your device does not support files downloading. Please try again in desktop browser.');
            return false;
        }


        //If in Chrome or Safari - download via virtual link click
        //if (window.downloadFile.isChrome || window.downloadFile.isSafari) {
        if (isChrome || isSafari) {
            //Creating new link node.
            var link = document.createElement('a');
            link.href = sUrl;


            if (link.download !== undefined) {
                //Set HTML5 download attribute. This will prevent file from opening if supported.
                //var fileName = sUrl.substring(sUrl.lastIndexOf('/') + 1, sUrl.length);
                var filePath = sUrl.substring(0, sUrl.indexOf('?'));
                var fileName = filePath.substring(filePath.lastIndexOf('/') + 1, filePath.length);
                //fileName = encodeURI(encodeURI(fileName));
                link.download = fileName;
            }


            //Dispatching click event.
            if (document.createEvent) {
                var e = document.createEvent('MouseEvents');
                e.initEvent('click', true, true);
                link.dispatchEvent(e);
                return true;
            }
        }


        // Force file download (whether supported by server).
        if (sUrl.indexOf('?') === -1) {
            sUrl += '?download';
        }


        window.open(sUrl, '_self');
        return true;
    },

    //刷新属性页
    loadAttrPage: function () {
        var me = this;
        //var grid = Ext.getCmp('_DocGrid');
        var grid = me.maindocgrid;
        var rs = grid.getSelectionModel().getSelection();//获取选择的文档
        if (rs !== null && rs.length > 0) {
            var rs = rs[0];//第一个文档
            var Keyword = rs.data.Keyword;

            me.loadAttrPageByKeyword(Keyword);

        }
    },

    //刷新属性页
    loadAttrPageByKeyword: function (Keyword) {

        var me = this;

        //同步WebBrowser
        var storageSid = document.getElementById("_Storage_Sid");
        //storageSid.children[0].children[0].children[1].children[0].value = localStorage.getItem("sid");
        storageSid.children[0].children[0].children[0].value = localStorage.getItem("sid");

        var storageDocKeyword = document.getElementById("_Storage_DocKeyword");
        //storageDocKeyword.children[0].children[0].children[1].children[0].value = Keyword;
        storageDocKeyword.children[0].children[0].children[0].value = Keyword;

        var tabPlan = me.up('_mainSourceView').down('_mainAttrTab');
        var tab = tabPlan.mainattrtab.activeTab;
        var tabType = "";
        if (tab.title === "文档属性") {

            tabType = "docAttr";
            tabPlan.loadDocAttrPage(tabType, Keyword);

        } else if (tab.title === "文档_附加属性") {

            tabType = "docAttrData";
            tabPlan.loadDocAttrPage(tabType, Keyword);

        } else {//如果活动属性页是是“目录属性”，就转到“文档属性”页
            tabType = "docAttr";

            //tabPlan.setAttrTabDisplay("Doc", Keyword);

            //Tab页Change事件锁，防止目录和文档之间互相切换时，重复执行Tab的Change事件
            tabPlan.TabChangeLock = true;
            tabPlan.loadDocAttrPage(tabType, Keyword);
        }

    },

    //新建文档
    CreateNewDoc: function () {
        var me = this;
        //me.viewTree = Ext.getCmp(mainPanelId).down('treepanel');//获取目录树控件ID
        var viewTree = me.maindocgrid.up('_mainSourceView').down('_mainProjectTree').down('treepanel');
        var nodes = viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {
            var parentKeyword = nodes[0].data.Keyword;
            //请求判断是否有创建目录权限
            Ext.Ajax.request({
                url: 'WebApi/Post',
                method: "POST",
                params: {
                    C: "AVEVA.CDMS.WebApi.ProjectController", A: "GetMenuRight",
                    Menu: "CreateNewDoc", ProjectKeyword: parentKeyword,
                    sid: localStorage.getItem("sid")
                },
                success: function (response, options) {
                    var res = Ext.JSON.decode(response.responseText);
                    var state = res.success;
                    if (state === false) {
                        var errmsg = res.msg;
                        Ext.Msg.alert("错误信息", errmsg);
                    }
                    else {
                        //弹出操作窗口
                        var _fmCreateNewDoc = Ext.create('Ext.plug_ins.SysPlugins._ModiDocAttr', { title: "" });
                        win = Ext.widget('window', {
                            title: '新建文档',
                            closeAction: 'hide',
                            width: 500,
                            height: 580,
                            minWidth: 500,
                            minHeight: 580,
                            layout: 'fit',
                            resizable: true,
                            modal: true,
                            closeAction: 'close', //close 关闭  hide  隐藏  
                            items: _fmCreateNewDoc,
                            defaultFocus: 'firstName'
                        });


                        win.show();
                        //监听子窗口关闭事件
                        win.on('close', function () {

                        });

                        _fmCreateNewDoc.mainPanelId = viewTree.id;
                        _fmCreateNewDoc.winAction = "CreateDoc";
                        _fmCreateNewDoc.docCodeText.setValue("新建文档");
                        var projectKeyword = nodes[0].data.Keyword;
                        _fmCreateNewDoc.get_Doc_attr(projectKeyword, "");
                    }
                },
                failure: function (response, options) {
                    ////Ext.Msg.alert("错误", "连接服务器失败！<br>" + response.responseText);
                }
            })
        } else {
            Ext.Msg.alert("错误信息", "请选择文件夹！");
        }
    },

    //复制创建文档
    BatchCreateDoc: function () {
        var me = this;
        //获取目录树控件
        var viewTree = me.maindocgrid.up('_mainSourceView').down('_mainProjectTree').down('treepanel');

        var nodes = viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {
            var parentKeyword = nodes[0].data.Keyword;
            //请求判断是否有创建目录权限
            Ext.Ajax.request({
                url: 'WebApi/Post',
                method: "POST",
                params: {
                    C: "AVEVA.CDMS.WebApi.ProjectController", A: "GetMenuRight",
                    Menu: "CreateNewDoc", ProjectKeyword: parentKeyword,
                    sid: localStorage.getItem("sid")
                },
                success: function (response, options) {
                    var res = Ext.JSON.decode(response.responseText);
                    var state = res.success;
                    if (state === false) {
                        var errmsg = res.msg;
                        Ext.Msg.alert("错误信息", errmsg);
                    }
                    else {
                        //弹出操作窗口
                        var _fmBatchCreateDoc = Ext.create('Ext.plug_ins.SysPlugins._BatchCreateDoc', { title: "" });

                        winBatchCreateDoc = Ext.widget('window', {
                            title: '批量创建文档',
                            closeAction: 'hide',
                            width: 800,
                            height: 596,
                            minWidth: 300,
                            minHeight: 300,
                            layout: 'fit',
                            resizable: true,
                            modal: true,
                            closeAction: 'close', //close 关闭  hide  隐藏  
                            items: _fmBatchCreateDoc,
                            defaultFocus: 'firstName'
                        });


                        winBatchCreateDoc.show();
                        //监听子窗口关闭事件
                        winBatchCreateDoc.on('close', function () {

                        });

                        _fmBatchCreateDoc.mainPanelId = viewTree.id;
                        var projectKeyword = nodes[0].data.Keyword;
                        _fmBatchCreateDoc.projectKeyword = projectKeyword;
                    }
                },
                failure: function (response, options) {
                    ////Ext.Msg.alert("错误", "连接服务器失败！<br>" + response.responseText);
                }
            })
        } else {
            Ext.Msg.alert("错误信息", "请选择文件夹！");
        }
    },
    
    //编辑文档属性
    ModiDocAttr: function () {
        var me = this;
        var viewTree = me.maindocgrid.up('_mainSourceView').down('_mainProjectTree').down('treepanel');//获取目录树控件ID
        //me.viewGrid = Ext.getCmp(mainPanelId).down('gridpanel'); //Ext.getCmp('_DocGrid');//获取文档表格控件ID

        var nodes = viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {
            var parentKeyword = nodes[0].data.Keyword;
            var rs = me.maindocgrid.getSelectionModel().getSelection();//获取选择的文档
            if (rs !== null && rs.length > 0) {
                var rs = rs[0];//第一个文档
                var docKeyword = rs.data.Keyword;

                ////请求判断是否有创建目录权限
                //Ext.Ajax.request({
                //    url: 'WebApi/Post',
                //    method: "POST",
                //    params: {
                //        C: "AVEVA.CDMS.WebApi.DocController", A: "GetMenuRight",
                //        Menu: "ModiDocAttr", DocKeyword: docKeyword,
                //        sid: localStorage.getItem("sid")
                //    },
                //    success: function (response, options) {
                        //var res = Ext.JSON.decode(response.responseText);
                        //var state = res.success;
                        //if (state === false) {
                        //    var errmsg = res.msg;
                        //    Ext.Msg.alert("错误信息", errmsg);
                        //}
                        //else {

                            var _fmModiDocAttr = Ext.create('Ext.plug_ins.SysPlugins._ModiDocAttr', { title: "" });
                            //var _fmCreateNewProject = Ext.create('Ext.plug_ins.SysPlugins._CreateProjByDef', { title: "" });
                            win = Ext.widget('window', {
                                title: '文档属性',
                                closeAction: 'hide',
                                width: 500,
                                height: 580,
                                minWidth: 500,
                                minHeight: 580,
                                layout: 'fit',
                                resizable: true,
                                modal: true,
                                closeAction: 'close', //close 关闭  hide  隐藏  
                                items: _fmModiDocAttr,
                                defaultFocus: 'firstName'
                            });


                            win.show();
                            //监听子窗口关闭事件
                            win.on('close', function () {

                            });

                            _fmModiDocAttr.mainPanelId = me.maindocgrid.id;
                            _fmModiDocAttr.winAction = "ModiDoc";
                            //_fmModiDocAttr.docCodeText.setValue("新建文档");
                            var projectKeyword = nodes[0].data.Keyword;
                            _fmModiDocAttr.get_Doc_attr(projectKeyword, docKeyword);

                 //       }
                //    }
                //})
            } else {
                Ext.Msg.alert("错误信息", "请选择文档！");
            }
        }

    },

    //响应地址栏回车事件
    onAddressFieldPressEnter: function () {
        var me = this;

        var path = me.addressField.value;

        //根据文件夹地址栏的路径获取对象
        Ext.Ajax.request({
            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.WebApi.ProjectController", A: "GetShowPathObject",
                sid: localStorage.getItem("sid"), path: path
            },
            success: function (response, options) {

                var res = Ext.JSON.decode(response.responseText);
                var state = res.success;
                if (state === true) {
                    var recod = eval(res.data[0]);

                    if (recod.objectType != undefined && recod.objectKeyword != undefined) {
                        if (recod.objectType === "Project") {
                            //展开目录
                            Ext.require('Ext.ux.Common.comm', function () {
                                Ext.getCmp('contentPanel').down('_mainProjectTree').ExpendProject(recod.objectKeyword);
                            });
                        }
                    }
                    //var docgrid = me.up('_mainSourceView').down('_mainDocGrid');
                    //docgrid.addressField.setValue(recod.path);
                }
            },
            failure: function (response, options) {
                ////Ext.Msg.alert("错误", "连接服务器失败！<br>" + response.responseText);
            }
        });
    },

    //编辑文档属性,参数viewInPanel：true:在panle中打开，false:在新窗口打开
    //unRarPath:解压后文件的路径，unRarItemName：解压后文件名
    PreviewDoc: function (viewInPanel,isUnRar,unRarPath,unRarItemName) {
        var me = this;
        //先打开一个空白窗口
        if (viewInPanel === 'false')
            var _newWindow = window.open('', '_blank');

        var viewTree = me.maindocgrid.up('_mainSourceView').down('_mainProjectTree').down('treepanel');//获取目录树控件ID
        //me.viewGrid = Ext.getCmp(mainPanelId).down('gridpanel'); //Ext.getCmp('_DocGrid');//获取文档表格控件ID

        var nodes = viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {

            var rs = me.maindocgrid.getSelectionModel().getSelection();//获取选择的文档
            if (rs !== null && rs.length > 0) {
                var rs = rs[0];//第一个文档
                var docKeyword = rs.data.Keyword;
                var view = me.up('_mainSourceView').down('_mainAttrTab');
                var A = "";
                if (isUnRar === "false") {
                    A = "PreviewDoc";
                }
                else {
                    A = "PreviewZipDoc";
                }
                //先把预览视图清空
                Ext.require('Ext.ux.Common.comm', function () {
                    updateDocPreview(me, view._DocPreviewPanel, "", "","");
                });

                if (viewInPanel === "true")
                    Ext.MessageBox.wait("正在生成预览，请稍候...", "等待");

                Ext.Ajax.request({
                    url: 'WebApi/Post',
                    method: "POST",
                    timeout: 20000,
                    params: {
                        C: "AVEVA.CDMS.WebApi.DocController", A: A,
                        DocKeyword: docKeyword, sid: localStorage.getItem("sid"),
                        path: unRarPath, filename: unRarItemName
                    },
                    failure: function (response, options) {
                        if (response.timedout === true) {
                            setTimeout(me.PreviewDoc(viewInPanel, isUnRar, unRarPath, unRarItemName), 5000);
                        }
                            // 其他错误，如网络错误等
                        else {

                            //longPolling();
                            Ext.MessageBox.close();//关闭等待对话框
                            Ext.Msg.alert("错误", "预览文件失败，网络错误！" + response.responseText);
                        }
                    },
                    success: function (response, options) {
                        var res = Ext.JSON.decode(response.responseText, true);
                        var state = res.success;
                        if (state === false) {
                            var errmsg = res.msg;
                            if (errmsg === "isExtracting") {
                                setTimeout(me.PreviewDoc(viewInPanel, isUnRar, unRarPath, unRarItemName), 5000);
                            } else {
                                Ext.Msg.alert("错误信息", errmsg);
                            }
                        }
                        else {
                            var recod = res.data[0];
                            if (recod.filetype === "common") {
                                if (viewInPanel === 'false') {
                                    //当ajax返回后，再改变新窗口的地址
                                    if (_newWindow)
                                        _newWindow.location = recod.path;
                                    //window.open(recod.path, '_blank');//新窗口打开链接
                                } else {
                                    var view, viewPanel;
                                    if (viewInPanel === 'true') {
                                        view = me.up('_mainSourceView').down('_mainAttrTab');
                                        if (recod.isUnrar === 'true')
                                        { view.showRarGrid(); }
                                        else {
                                            view.hideRarGrid();
                                        }
                                        viewPanel = view._DocPreviewPanel;
                                    } else {
                                        viewPanel = Ext.getCmp(viewInPanel);

                                    }


                                    //刷新视图
                                    Ext.require('Ext.ux.Common.comm', function () {
                                        updateDocPreview(me, viewPanel, recod.path, options.params.DocKeyword, recod.filename);
                                    });

                                    Ext.MessageBox.close();//关闭等待对话框
                                }
                            } else {
                                //响应rar或zip文件，显示解压文件列表
                                var view = me.up('_mainSourceView').down('_mainAttrTab');
                                view._RarListStore.removeAll();
                                view.showRarGrid();
                                view.objfilelist = recod.filelist;
                                view.subFolder = recod.subFolder;
                                //Ext.each(recod.filelist, function (v) {
                                //    view.updateRarListStore(v);
                                //}, me);

                                view.updateRarListStore(recod.filelist);

                                Ext.require('Ext.ux.Common.comm', function () {
                                    updateDocPreview(me, view._DocPreviewPanel, "", options.params.DocKeyword, recod.filename);
                                });
                                view.tmpParentPath = "";
                                Ext.MessageBox.close();//关闭等待对话框
                            }

                        }
                    },
                    failure: function (response, options) {
                        ////Ext.Msg.alert("错误", "连接服务器失败！<br>" + response.responseText);
                    }
                });
            }
        }
    },

    //复制文档
    CopyDoc: function () {
        var me = this;

        window.parent.clipboardObjectList = "";

        var nodesDoc = me.maindocgrid.getSelectionModel().getSelection();//获取已选取文件列表的节点
        if (nodesDoc !== null && nodesDoc.length > 0) {
            var doclist = "";
            for (var i = 0; i < nodesDoc.length ; i++) {
                var dockeyword=nodesDoc[i].get("Keyword");
                if (i === 0)
                {
                    doclist =dockeyword;
                }
                else {
                    doclist += "," + dockeyword;
                }
            }
            //Ext.Msg.alert("消息", "复制文档！"+doclist);
            window.parent.clipboardObjectList = doclist;
        }
        window.parent.iscut = false;
        
    },

    //剪切文档
    MoveDoc: function () {
        var me = this;

        window.parent.clipboardObjectList = "";

        var nodesDoc = me.maindocgrid.getSelectionModel().getSelection();//获取已选取文件列表的节点
        if (nodesDoc !== null && nodesDoc.length > 0) {
            var doclist = "";
            for (var i = 0; i < nodesDoc.length ; i++) {
                var dockeyword = nodesDoc[i].get("Keyword");
                if (i === 0) {
                    doclist = dockeyword;
                }
                else {
                    doclist += "," + dockeyword;
                }
            }
            //Ext.Msg.alert("消息", "剪切文档！"+doclist);
            window.parent.clipboardObjectList = doclist;
        }
        window.parent.iscut = true;
    },

    //删除文档
    DeleteDoc: function (sureDel) {
        var me = this;

        //选择是否确定删除
        Ext.MessageBox.show({
            title: '确认删除',
            msg: "是否要删除选中的文档？",
            buttons: Ext.MessageBox.YESNO,
            buttonText: {
                yes: "是",
                no: "否"
            },
            fn: function (btn) {
                if (btn === "yes") {

                    var viewTree = me.maindocgrid.up('_mainSourceView').down('_mainProjectTree').down('treepanel');//获取目录树控件ID
                    var nodesProject = viewTree.getSelectionModel().getSelection();//获取已选取目录树的节点
                    if (nodesProject !== null && nodesProject.length > 0) {
                        var projectNode = nodesProject[0];
                        var projectId = nodesProject[0].data.Keyword;
                        var nodesDoc = me.maindocgrid.getSelectionModel().getSelection();//获取已选取文件列表的节点
                        if (nodesDoc !== null && nodesDoc.length > 0) {
                            me.sendDeleteDoc(projectId,nodesDoc, 0);


                        }
                    }
                }
                else {
                }
            }
        });
    },

    sendDeleteDoc: function (projectId,docNodes, nodesIndex) {
        var me = this;
        var docNode = docNodes[nodesIndex];
        var docId = docNodes[nodesIndex].data.Keyword;
        sureDel = "true";
        Ext.Ajax.request({
            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.WebApi.DocController", A: "DeleteDoc",
                ProjectKeyword: projectId, DocKeyword: docId,
                sureDel: sureDel, sid: localStorage.getItem("sid")
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
                    var State = recod.state;
                    if (State === "delSuccess") {
                        if (docNodes.length > nodesIndex + 1) {
                            me.sendDeleteDoc(projectId,docNodes, nodesIndex + 1);
                        } else {
                            var treeView = Ext.getCmp('contentPanel').down('_mainProjectTree');
                            treeView.setAttrPage(projectId, function () {
                                me.loadDocListStore(function () {
                                });
                            });
            
                        }
                        
                    }
                }
            },
            failure: function (response, options) {
                ////Ext.Msg.alert("错误", "连接服务器失败！<br>" + response.responseText);
            }
        });
    },

    //放弃文档
    FreeDoc: function () {
        var me = this;

        //选择是否确定放弃
        Ext.MessageBox.show({
            title: '确认放弃',
            msg: "是否确认要放弃文档的所有修改？（该文档可能正被别的用户编辑）",
            buttons: Ext.MessageBox.YESNO,
            buttonText: {
                yes: "是",
                no: "否"
            },
            fn: function (btn) {
                if (btn === "yes") {
                    var viewTree = me.maindocgrid.up('_mainSourceView').down('_mainProjectTree').down('treepanel');//获取目录树控件ID
                    var nodesProject = viewTree.getSelectionModel().getSelection();//获取已选取目录树的节点
                    if (nodesProject !== null && nodesProject.length > 0) {
                        var projectNode = nodesProject[0];
                        var projectId = nodesProject[0].data.Keyword;
                        var nodesDoc = me.maindocgrid.getSelectionModel().getSelection();//获取已选取文件列表的节点
                        if (nodesDoc !== null && nodesDoc.length > 0) {
                            me.sendFreeDoc(projectId, nodesDoc, 0);
                        }
                    }
                }
            }
        });
    },

    sendFreeDoc: function (projectId, docNodes, nodesIndex) {
        var me = this;
        var docNode = docNodes[nodesIndex];
        var docId = docNodes[nodesIndex].data.Keyword;
        var sureFree = "true";
        Ext.Ajax.request({
            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.WebApi.DocController", A: "FreeDoc",
                ProjectKeyword: projectId, DocKeyword: docId,
                sureFree: sureFree, sid: localStorage.getItem("sid")
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
                    var State = recod.state;
                    if (State === "freeSuccess") {
                        if (docNodes.length > nodesIndex + 1) {
                            me.sendFreeDoc(projectId, docNodes, nodesIndex + 1);
                        } else {
                            me.loadDocListStore(function () { });
                        }

                    }
                }
            },
            failure: function (response, options) {
                ////Ext.Msg.alert("错误", "连接服务器失败！<br>" + response.responseText);
            }
        });
    },

    //上传替换文档
    ReplaceDoc: function () {
        var me = this;
        //触发上传文件按钮事件(html5方法实现)
        var w = me.ReplaceDocButton.uploader.uploader;
        var y = document.getElementById(w.id + "_html5");
        if (y && !y.disabled) {
            y.click()
        } else {
            var z = document.getElementById(w.id + "_flash");
            if (z) {
                document.getElementById(w.settings.browse_button).click();
                //w.settings.browse_button_active = true;
                w.trigger("flash_click", w)

            }
        }
    },

    //刷新文档列表,docKeyword:加载完成后需要跳转的的文档Keyword
    loadDocListStore: function (callBackFun) {
        var me = this;

        var viewTreePanel = me.maindocgrid.up('_mainSourceView').down('_mainProjectTree');
        var viewTree = me.maindocgrid.up('_mainSourceView').down('_mainProjectTree').down('treepanel');//获取目录树控件ID

        var nodes = viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {
            var projectKeyword = nodes[0].data.Keyword;
            //防止多次点击目录树
            //Ext.MessageBox.wait("正在获取文档列表，请稍候...", "等待");
            //var myMask = new Ext.LoadMask(
            //    //Ext.getBody(),
            //    viewTreePanel.el,
            //    {
            //        //shadow: false,
            //        msg: '正在获取文档列表，请稍候...',
            //        //indicator: true,
            //        disabled: true,
            //        //removeMask: true

            //});
            //myMask.show();
            //获取文档列表
            var store = me._DocListStore;//路径：\simplecdms\scripts\app\store\contents.js
            store.proxy.extraParams.ProjectKeyWord = projectKeyword;//把参数传给C#MVC,路径：\simplecdms\controllers\projectcontroller.cs 下的 GetChildProject()
            store.proxy.extraParams.filterJson = "";
            store.proxy.extraParams.sid = localStorage.getItem("sid");
            //store.loadPage(1);
            store.load({
                callback: function (records, options, success) {
                    //Ext.MessageBox.close();//关闭等待对话框
                    //myMask.hide();
                    callBackFun();
                }
            });
        }
    },

    //响应选中或取消选中文档事件
    onDocGridItemSelect: function (view, record, index, eOpts) {
        var me = this;

        var view = me.up('_mainSourceView').down('_mainAttrTab');

        var actTitle = view.mainattrtab.getActiveTab().title;

        view.seleObjType = "Doc";

        if (actTitle === "文件预览") {
            me.PreviewDoc("true", "false", "", "");
        }
        else {
            //刷新文档属性页
            if (typeof (record.data) != 'undefined') {
                me.loadAttrPage("selectdoc");
            }
        }

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
        var viewTree = me.maindocgrid.up('_mainSourceView').down('_mainProjectTree').down('treepanel');//获取目录树控件ID

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


    //启动文档的流程菜单
    _StartProcess: function () {
        var me = this;

        window.activeGridPanelId = me.maindocgrid.id;

        var rs = me.maindocgrid.getSelectionModel().getSelection();//获取选择的文档
        if (rs) {
            //获取选取文档关键字
            var doclist = "";
            for (var i = 0; i < rs.length ; i++) {
                //遍历每一行
                if (i === 0)
                    doclist = rs[i].data.Keyword;
                else doclist = doclist + "," + rs[i].data.Keyword;
            }

            //启动流程
            Ext.require('Ext.ux.Common.comm', function () {
                //参数：doclist,wfKeyword,userlist,callback_fun
                StartNewWorkFlow(doclist, "", "", function (res,WorkFlowKeyword, CuWorkStateCode) {
                    me.loadAttrPage("selectdoc");
                });
            })
        }

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

    //转到源目录
    OpenShortcutProject: function () {
        var me = this;

        var nodes = me.maindocgrid.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {

            var mpanel = Ext.getCmp('mainPanel');
            if (mpanel.activeTab != 0)
                mpanel.setActiveTab(0);

            var DocKeyword = nodes[0].data.Keyword;

            Ext.require('Ext.ux.Common.comm', function () {
                Ext.getCmp('contentPanel').down('_mainProjectTree').ExpendProject(DocKeyword);
            });

        }

        

    },

    //项目立项
    CreateRootProject: function () {
        var me = this;

        var viewTree = me.maindocgrid.up('_mainSourceView').down('_mainProjectTree').down('treepanel');//获取目录树控件ID

        //弹出操作窗口
        var _fmCreateRootProject = Ext.create('Ext.plug_ins.HXPC_Plugins.CreateRootProject', { title: "", mainPanelId: viewTree.id });


        winCreateRootProject = Ext.widget('window', {
            title: '项目立项',
            closeAction: 'hide',
            width: 765,
            height: 571,
            minWidth: 765,
            minHeight: 571,
            layout: 'fit',
            resizable: true,
            modal: true,
            closeAction: 'close', //close 关闭  hide  隐藏  
            items: _fmCreateRootProject,
            defaultFocus: 'firstName'
        });

        //_fmCreateRootProject.get_combobox_default();//设置combobox默认值

        winCreateRootProject.show();
        //监听子窗口关闭事件
        winCreateRootProject.on('close', function () {

        });


    },

    //华西能源项目工日填报功能
    TaskReport:function(){
        var me = this;
        var viewTree = me.maindocgrid.up('_mainSourceView').down('_mainProjectTree').down('treepanel');//获取目录树控件ID
        //me.viewGrid = Ext.getCmp(mainPanelId).down('gridpanel'); //Ext.getCmp('_DocGrid');//获取文档表格控件ID


        var _fmTaskReport = Ext.create('Ext.plug_ins.HXPC_Plugins.taskReport', { title: "", mainPanelId: me.maindocgrid.id });

                winTaskReport = Ext.widget('window', {
                    title: '任务报告',
                    closeAction: 'hide',
                    width: 1020,
                    height: 641,
                    minWidth: 1020,
                    minHeight: 641,
                    layout: 'fit',
                    resizable: true,
                    modal: true,
                    closeAction: 'close', //close 关闭  hide  隐藏  
                    items: _fmTaskReport,
                    defaultFocus: 'firstName'
                });


                winTaskReport.show();
                //监听子窗口关闭事件
                winTaskReport.on('close', function () {

                });

    },

    //华西EPC项目立项
    CreateHXEPCRootProject: function () {
        var me = this;

        var viewTree = me.maindocgrid.up('_mainSourceView').down('_mainProjectTree').down('treepanel');//获取目录树控件ID

        //弹出操作窗口
        var _fmCreateRootProject = Ext.create('Ext.plug_ins.HXEPC_Plugins.CreateRootProject', { title: "", mainPanelId: viewTree.id });

        winCreateRootProject = Ext.widget('window', {
            title: '项目立项',
            closeAction: 'hide',
            width: 465,
            height: 351,
            minWidth: 465,
            minHeight: 351,
            layout: 'fit',
            resizable: true,
            modal: true,
            closeAction: 'close', //close 关闭  hide  隐藏  
            items: _fmCreateRootProject,
            defaultFocus: 'firstName'
        });

        winCreateRootProject.show();
        //监听子窗口关闭事件
        winCreateRootProject.on('close', function () {

        });


    },

    //显示右键菜单方法
    _showContextMenu: function (view, record, item, index, e, eOpts) {

        //显示右键菜单
        var menus = Ext.widget('_contextmenu');

        menus.showMainPanelMenu(view, e, false);

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

//Ext.EventManager.onWindowResize(function () {
//    var view = Ext.getCmp(gridPanelId);
//    view.refresh();
//    //grid1.getView().refresh() 
//})


