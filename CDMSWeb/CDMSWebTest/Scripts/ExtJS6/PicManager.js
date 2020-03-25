//图片管理要在两个地方使用：一是标签页内的图片管理，一是文章内容编辑时嵌套到插入图片的窗口内。
//因而，将图片管理做成一个扩展比较方便。
Ext.define('Ext.ux.PicManager', {
    //图片管理的界面将从容器扩展
    extend: 'Ext.container.Container',
    alias: 'widget.picmanager',
    //使用边框布局
    layout: "border",
    requires: ["Ext.ux.DataView.DragSelector"],

    //定义模型:左边的目录树
    //模型的定义可以独立成一个文件，也可以在initComponet方法内定义。
    //如果在独立的文件定义，就要在扩展中添加requires配置项引用模型。
    initComponent: function () {
        var me = this;

        //定义目录树数据模型
        Ext.define("Folder", {
            //扩展自'Ext.data.Store'类
            extend: "Ext.data.Model",
            //parentId用来记录父目录
            fields: ["text", "id", "parentId"],
            //添加一个验证项，目录名称不能为空
            validations: [{
                type: 'presence',
                field: 'text'
            }],
            //代理定义
            proxy: {
                type: 'ajax',
                //代理的API定义了操作的提交路径
                //路径：\CDMSWeb\Controllers\FileController.cs
                api: {
                    read: 'Folder/List',
                    create: 'Folder/Add',
                    destroy: 'Folder/Delete',
                    update: 'Folder/Edit'
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
            }
        });

        //定义TreeStore
        me.treestore = Ext.create("Ext.data.TreeStore", {
            //定义根节点,这里根节点的id使用“/”，是为了方便后台将虚拟路径转换为实际路径。
            root: { id: "/", text: "根目录", expanded: true },
            //引用目录树数据模型
            model: "Folder"
        });

        //定义树面板
        me.tree = Ext.widget("treepanel", {
            // rootVisible:true, 设置显示根目录，这是因为系统将允许在根目录上传文件
            //collapsible,设置是否折叠
            //split:显示分割控件
            title: "文件目录", region: "west", collapsible: true, rootVisible: true,
            width: 250, minWidth: 100, maxWidth: 500, split: true, store: me.treestore,
            hideHeaders: true,
            plugins: [{ ptype: "cellediting",
                listeners: {
                    beforeedit: function (edit, e) {
                        if (e.record.isRoot()) return false;
                    },
                    edit: function (edit, e) {
                        e.record.save({
                            success: function (rec, opt) {
                                opt.records[0].setId(opt.records[0].data.parentId + opt.records[0].data.text + "/");
                                opt.records[0].commit();
                            },
                            failure: function (e, op) {
                                op.records[0].reject();
                                Ext.Msg.alert("发生错误", op.error);
                            }
                        });
                    }
                }
            }],
            columns: [
				{ xtype: "treecolumn", dataIndex: "text", flex: 1,
				    field: { allowBlank: false, selectOnFocus: true }
				}
			],
            viewConfig: {
                toggleOnDblClick: false
            },
            tbar: [
            	{ iconCls: "folder-add", handler: me.onAddFolder, scope: me, tooltip: "添加目录" },
            	{ iconCls: "folder-delete", handler: me.onDeleteFolder, scope: me, disabled: true, tooltip: "删除目录" },
            	{ iconCls: "refresh", handler: me.onRefreshFolder, scope: me, tooltip: "刷新目录树" }
            ],
            listeners: {
                scope: me,
                viewready: function (panel) {
                    var view = panel.getView();
                    view.getSelectionModel().select(0);
                },
                selectionchange: function (model, sels) {
                    var me = this;
                    if (sels.length > 0) {
                        var rs = sels[0],
							store = me.filestore;
                        store.proxy.extraParams.path = rs.data.id;
                        store.loadPage(1);
                    }
                    me.tree.down("button[tooltip=删除目录]").setDisabled(sels.length == 0);

                }
            }
        });

        //右边的文件预览
        Ext.define('File', {
            //定义文件预览数据模型
            extend: 'Ext.data.Model',
            //定义字段，除了文件名、路径、最后编辑时间、文件大小
            fields: [
		        'filename', 'path',
		        "modify",
		        { name: "size", type: "int" }

		     ]
        });

        //定义文件预览Store,代理proxy可以在Model里面定义，也可以在Store里面定义
        me.filestore = Ext.create("Ext.data.Store", {
            batchActions: false,
            remoteFilter: false,
            remoteSort: true,
            pageSize: 50,
            model: "File",
            sorters: [
                { property: "modify", direction: "DESC" }
            ],
            proxy: {
                type: "ajax",
                //代理的API定义了操作的提交路径
                //路径：\CDMSWeb\Controllers\FileController.cs
                //这里的API没有create和update配置项是因为，文件是上传的，不能通过该方式提交，
                //而文件一般也不进行更新，而是先删除后再上传。
                api: {
                    read: 'File/List',
                    destroy: 'File/Delete'
                },
                reader: {
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
            }
        });

        //用数据视图来显示文件
        //因为视图不是面板，没有工具栏组件，因而要在其外面套一个面板用来放置工具栏，所以在视图的定义中没有边框布局的配置项。
        me.dataview = Ext.widget("dataview", {
            store: me.filestore, autoScroll: true,
            multiSelect: true, selectedItemCls: "selected", itemSelector: 'div.imageList',
            overItemCls: "overitem", trackOver: true,
            plugins: [
                Ext.create('Ext.ux.DataView.DragSelector', {})
            ],
            //定义模板,定义数据视图的关键就是模板的定义
            tpl: [
			    '<tpl for=".">',
			        '<div class="imageList">',
			            '<img width="160" height="160" src="../Thumbnail{path}{filename}?width=160&height=160" data-qtip="文件名：{filename}<br/>修改日期：{modify}<br>大小：{size:this.filesize}" /><br/>',
			            '<p class="ellipsis">{filename}</p>',
			        '</div>',
			    '</tpl>',
			    '<div class="x-clear"></div>',
			    {
			        //在模板添加了一个filesize方法，用来转换文件大小的显示格式
			        filesize: function (size) {
			            if (size < 1024) {
			                return v + " 字节";
			            } else if (size < 1048576) {
			                return (Math.round(((size * 10) / 1024)) / 10) + " KB";
			            } else {
			                return (Math.round(((size * 10) / 1048576)) / 10) + " MB";
			            }
			        }
			    }
			],
            listeners: {
                scope: me,
                selectionchange: me.onPictureSelect
            }
        });

        me.progress = Ext.widget("progressbar", { text: "上传进度", flex: 1 });
        me.spanid = Ext.id();

        //完成整个界面框架
        me.items = [
            me.tree,
            { title: "图片文件", region: "center", layout: 'fit', items: [me.dataview],
                tbar: {
                    xtype: "pagingtoolbar",
                    pageSize: 20, displayInfo: true, store: me.filestore,
                    items: [
	                	'-',
		            	{ xtype: "splitbutton", iconCls: "sort", tooltip: "排序", text: "排序",
		            	    scope: me,
		            	    menu: {
		            	        items: [
                                    {
                                        text: '文件名顺序', group: 'sort', checked: false, checkHandler: me.onSort, scope: me,
                                        fieldname: "filename", sortdir: "ASC"
                                    },
                                    {
                                        text: '文件名降序', group: 'sort', checked: false, checkHandler: me.onSort, scope: me,
                                        fieldname: "filename", sortdir: "DESC"
                                    },
                                    {
                                        text: '修改日期顺序', group: 'sort', checked: false, checkHandler: me.onSort, scope: me,
                                        fieldname: "modify", sortdir: "ASC"
                                    },
                                    {
                                        text: '修改日期降序', checked: true, group: 'sort', checkHandler: me.onSort, scope: me,
                                        fieldname: "modify", sortdir: "DESC"
                                    },
                                    {
                                        text: '文件大小顺序', group: 'sort', checked: false, checkHandler: me.onSort, scope: me,
                                        fieldname: "size", sortdir: "ASC"
                                    },
                                    {
                                        text: '文件大小降序', group: 'sort', checked: false, checkHandler: me.onSort, scope: me,
                                        fieldname: "size", sortdir: "DESC"
                                    }
                                ]
		            	    }
		            	},
                        { iconCls: "picture-delete", handler: me.onDelete, scope: me, disabled: true, tooltip: "删除图片" }
	                ]
                },
                dockedItems: [
	        		{ xtype: "toolbar", dock: "bottom",
	        		    items: [
	        			    { xtype: "tbtext", text: "<span id='" + me.spanid + "'></span>", width: 70 },
	        			    me.progress
	        		    ]
	        		}
	        	]
            }
        ];

        me.on("afterrender", me.onAfterRender);
        me.callParent(arguments);
    },

    onAddFolder: function () {
        var tree = this.tree,
			parent = tree.getSelectionModel().getSelection()[0];
        if (!parent) {
            parent = tree.getRootNode();
        }
        var rec = new Folder({
            text: "新建文件夹",
            id: "",
            parentId: parent.data.id
        });
        rec.save({
            url: "Folder/Add",
            parentNode: parent,
            success: function (rec, opt) {
                if (opt.parentNode.isExpanded())
                    opt.parentNode.appendChild(rec);
                else
                    opt.parentNode.expand();
            },
            failure: CDMSWeb.ModelException,
            scope: tree
        });
    },

    onDeleteFolder: function () {
        var me = this,
            tree = me.tree,
            rs = tree.getSelectionModel().getSelection();
        if (rs.length > 0) {
            rs = rs[0];
            if (rs.data.root) {
                Ext.Msg.alert("删除文件夹", "根目录不允许删除！");
                return;
            }
            var content = "确定删除目录“" + rs.data.text + "”？<br/><p style='color:red'>注意：目录下的文件及子目录都会被删除。</p>";
            Ext.Msg.confirm("删除目录", content, function (btn) {
                if (btn == "yes") {
                    var rs = this.getSelectionModel().getSelection();
                    if (rs.length > 0) {
                        rs = rs[0];
                        rs.remove();
                        this.store.sync({
                            success: function (e, opt) {
                                var me = this;
                                me.store.commitChanges();
                                me.view.select(0);
                                me.view.focus(false);
                            },
                            failure: CDMSWeb.ModelException,
                            scope: tree
                        });
                    }
                }
            }, tree)
        }
    },

    onRefreshFolder: function () {
        this.tree.store.load();
    },

    onSort: function (item, checked) {
        var me = this;
        if (checked) {
            me.filestore.sort({ property: item.fieldname, direction: item.sortdir });
            me.filestore.load();
        }
    },

    onPictureSelect: function (model, sels) {
        this.down("button[tooltip=删除图片]").setDisabled(sels.length == 0);
    },

    onDelete: function () {
        var me = this,
    		rs = me.dataview.getSelectionModel().getSelection();
        if (rs.length > 0) {
            content = ["确定删除以下图片？"];
            for (var i = 0; ln = rs.length, i < ln; i++) {
                content.push(rs[i].data.filename);
            }
            Ext.Msg.confirm("删除图片", content.join("<br/>"), function (btn) {
                if (btn == "yes") {
                    var me = this,
                        store = me.dataview.store,
                        rs = me.dataview.getSelectionModel().getSelection();
                    store.remove(rs);
                    store.sync({
                        success: function (e, opt) {
                            this.store.commitChanges();
                        },
                        failure: function (e, opt) {
                            this.store.rejectChanges()
                            Ext.Msg.alert("发生错误", e.exceptions[0].error);
                        },
                        scope: me.dataview
                    });
                }
            }, me)
        } else {
            Ext.Msg.alert('删除记录', '请选择要删除的记录。');
        }
    },

    onAfterRender: function(){
        var me = this;

        me.swfu = new SWFUpload({
            upload_url: "/File/Upload",
            file_size_limit: "10 MB",
            file_types: "*.jpg;*.png;*.gif;*.bmp;",
            file_types_description: "图片文件",
            file_upload_limit: 100,
            file_queue_limit: 0,
            file_post_name: "Filedata",

            swfupload_preload_handler: me.UploadPreLoad,
            swfupload_load_failed_handler: me.UploadLoadFailed,
            swfupload_loaded_handler: me.UploadLoaded,
            file_queued_handler: me.fileQueued,
            file_queue_error_handler: me.fileQueueError,
            file_dialog_complete_handler: me.fileDialogComplete,
            upload_start_handler: me.uploadStart,
            upload_progress_handler: me.uploadProgress,
            upload_error_handler: me.uploadError,
            upload_success_handler: me.uploadSuccess,
            upload_complete_handler: me.uploadComplete,
            queue_complete_handler: me.queueComplete,

            // Button settings
            button_window_mode: SWFUpload.WINDOW_MODE.TRANSPARENT,
            button_image_url: '',
            button_placeholder_id: me.spanid,
            button_width: 60,
            button_height: 20,
            button_text: '上传图片',
            button_text_style: '',
            button_text_top_padding: 0,
            button_text_left_padding: 0,

            // Flash Settings
            flash_url: "Scripts/swfupload/swfupload.swf", // Relative to this file
            flash9_url: "Scripts/swfupload/swfupload_FP9.swf", // Relative to this file

            custom_settings: { scope: me },
            // Debug Settings
            debug: false
        });        
    },

    UploadPreLoad: function () {
    },

    UploadLoadFailed: function () {
    },

    UploadLoaded: function () {

    },

    fileQueued: function () {
    },

    fileQueueError: function (file, errorCode, message) {
        try {
            var dlg = Ext.Msg.alert;
            if (errorCode === SWFUpload.QUEUE_ERROR.QUEUE_LIMIT_EXCEEDED) {
                dlg("选择的文件太多。\n一次最多上传100个文件，而你选择了" + message + "个文件。");
                return;
            }

            switch (errorCode) {
                case SWFUpload.QUEUE_ERROR.FILE_EXCEEDS_SIZE_LIMIT:
                    dlg("文件超过了10M.");
                    this.debug("错误代码: File too big, File name: " + file.name + ", File size: " + file.size + ", Message: " + message);
                    break;
                case SWFUpload.QUEUE_ERROR.ZERO_BYTE_FILE:
                    dlg("不允许上传0字节文件。");
                    this.debug("Error Code: Zero byte file, File name: " + file.name + ", File size: " + file.size + ", Message: " + message);
                    break;
                case SWFUpload.QUEUE_ERROR.INVALID_FILETYPE:
                    dlg("非法的文件类型。");
                    this.debug("Error Code: Invalid File Type, File name: " + file.name + ", File size: " + file.size + ", Message: " + message);
                    break;
                default:
                    if (file !== null) {
                        dlg("未知错误。");
                    }
                    this.debug("Error Code: " + errorCode + ", File name: " + file.name + ", File size: " + file.size + ", Message: " + message);
                    break;
            }
        } catch (ex) {
            this.debug(ex);
        }
    },

    fileDialogComplete: function (numFilesSelected, numFilesQueued) {
        try {
            if (numFilesQueued > 0) {
                var me = this.customSettings.scope, sels = me.tree.getSelectionModel().getSelection(), path = "/";
                if (sels.length > 0) {
                    path = sels[0].data.id;
                    CDMSWeb.postParams.path = path;
                    this.setPostParams(CDMSWeb.postParams);
                    this.startUpload();
                }
                else {
                    Ext.Msg.alert("请先选择文件夹。");
                }
            }
        } catch (ex) {
            this.debug(ex);
        }
    },

    uploadStart: function (file) {
        try {
            var me = this.customSettings.scope;
            me.progress.updateProgress(0);
            me.progress.updateText("正在上传文件" + file.name + "...");
        }
        catch (ex) { }

        return true;
    },

    uploadProgress: function (file, bytesLoaded, bytesTotal) {
        try {
            var percent = bytesLoaded / bytesTotal;
            var me = this.customSettings.scope;
            me.progress.updateProgress(percent);
            me.progress.updateText("正在上传文件" + file.name + "...");
        } catch (ex) {
            this.debug(ex);
        }
    },

    uploadError: function (file, errorCode, message) {
        try {
            var me = this.customSettings.scope;
            me.progress.updateText("正在上传文件" + file.name + "...");
            switch (errorCode) {
                case SWFUpload.UPLOAD_ERROR.HTTP_ERROR:
                    me.progress.updateText("上传错误：" + message);
                    this.debug("Error Code: HTTP Error, File name: " + file.name + ", Message: " + message);
                    break;
                case SWFUpload.UPLOAD_ERROR.UPLOAD_FAILED:
                    me.progress.updateText("上传失败。");
                    this.debug("Error Code: Upload Failed, File name: " + file.name + ", File size: " + file.size + ", Message: " + message);
                    break;
                case SWFUpload.UPLOAD_ERROR.IO_ERROR:
                    me.progress.updateText("Server (IO) 错误");
                    this.debug("Error Code: IO Error, File name: " + file.name + ", Message: " + message);
                    break;
                case SWFUpload.UPLOAD_ERROR.SECURITY_ERROR:
                    me.progress.updateText("安全错误");
                    this.debug("Error Code: Security Error, File name: " + file.name + ", Message: " + message);
                    break;
                case SWFUpload.UPLOAD_ERROR.UPLOAD_LIMIT_EXCEEDED:
                    me.progress.updateText("文件大小超出限制。");
                    this.debug("Error Code: Upload Limit Exceeded, File name: " + file.name + ", File size: " + file.size + ", Message: " + message);
                    break;
                case SWFUpload.UPLOAD_ERROR.FILE_VALIDATION_FAILED:
                    me.progress.updateText("验证失败。");
                    this.debug("Error Code: File Validation Failed, File name: " + file.name + ", File size: " + file.size + ", Message: " + message);
                    break;
                case SWFUpload.UPLOAD_ERROR.FILE_CANCELLED:
                    break;
                case SWFUpload.UPLOAD_ERROR.UPLOAD_STOPPED:
                    me.progress.updateText("停止");
                    break;
                default:
                    me.progress.updateText("未知错误：" + errorCode);
                    this.debug("Error Code: " + errorCode + ", File name: " + file.name + ", File size: " + file.size + ", Message: " + message);
                    break;
            }
        } catch (ex) {
            this.debug(ex);
        }
    },

    uploadSuccess: function (file, serverData) {
        try {
            var me = this.customSettings.scope;
            me.progress.updateProgress(1);
            me.progress.updateText(serverData);

        } catch (ex) {
            this.debug(ex);
        }
    },

    uploadComplete: function (file) {
        try {
            if (this.getStats().files_queued > 0) {
                this.startUpload();
            } else {
                var me = this.customSettings.scope;
                me.progress.updateProgress(1);
                me.progress.updateText("所有文件已上传。");
                me.filestore.load();
            }
        } catch (ex) {
            this.debug(ex);
        }
    },

    queueComplete: function (numFilesUploaded) {
        var me = this.customSettings.scope;
        me.progress.updateProgress(1);
        me.progress.updateText("已上传" + numFilesUploaded + "个文件");
        if (numFilesUploaded > 0) {
            me.filestore.load();
        }
    }

});