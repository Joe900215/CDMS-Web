Ext.define('Ext.plug_ins.WTMS_Plugins.Dist.FileUploadPanel', {
    //extend: 'Ext.container.Container',
    extend: 'Ext.panel.Panel',
    alias: 'widget.fileUploadPanel', // 此类的xtype类型为buttontransparent
    layout: {
        type: 'vbox',
        pack: 'start',
        align: 'stretch'
    },
    projectKeyword: '',
    baseCls: 'my-panel-no-border',//隐藏边框
    //height: 80,
    initComponent: function () {
        var me = this;
        me.renderTo = me.el;

        
        me.winAction = "";

        me.UploadMode = "atta";

        me.docKeyword = "";

        me.totalPages = 0;

        me.docUploadIndex = 0;

        //默认的项目代码
        me.defaultProCode = "";
        //默认的部门
        me.defaultDepartment = "";
        //默认的文件编码类型
        me.defaultFileCodeType = "";
        //默认的项目名称
        me.defaultProName = "";

        //上传文档的doc关键字列表
        me.docList = "";

        //第一个文件是否是正件
        me.firstFileIsPositive = true;

        //是否挂接文件
        me.bIsMountFile = false;

        //升版时，原版的属性信息
        me.orgiAttrArray = [];

        //定义上传附件按钮
        me.FileUploadButton = Ext.create('Ext.ux.upload.Button', {
            renderTo: Ext.getBody(),
            //iconCls: "file-create",
            text: '选择文件',
            uploader:
            {
                url: 'WebApi/Post',
                uploadpath: '/Root/files',
                autoStart: false,//选择文件后是否自动上传
                max_file_size: '20020mb',
                multipart_params: { 'guid': '', 'ProjectKeyword': '', 'sid': '', 'C': 'AVEVA.CDMS.WebApi.FileController', 'A': 'UploadFile', 'SureReplace': 'false', 'AppendFilePath': '', 'user': '' }, //设置你的参数//{},
                drop_element: me.filegrid,//拖拽控件
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
              
                    uploader.multipart_params.ProjectKeyword = me.projectKeyword;
                    uploader.multipart_params.sid = localStorage.getItem("sid");


                    for (var i = 0; i < files.length ; i++) {
                        var fname = files[i].name;
                        var pos = fname.lastIndexOf(".");
                        var sct = fname.substring(0, pos);
                        var fileDesc = "";

                        var splitIndex = sct.indexOf("_");
                        if (splitIndex > 0) {
                            var fullCode = sct;
                            sct = fullCode.substring(0, splitIndex);
                            fileDesc = fullCode.substring(splitIndex + 1, fullCode.length);
                        }

                        fileCode = sct;

               
                        //插入行到文件grid
                        var rowlength = me.filegrid.getStore().data.length;

                        var r = Ext.create('filemodel', {
                        //var r = Ext.create("CDMSWeb.plug_ins.ZHEPC_Plugins.model.filemodel", {
                            id: files[i].id,
                            name: files[i].name,
                            code: fileCode,  // === undefined ? "" : (sct.length > 0 ? sct + "附件" + (rowlength + 1) : "")
                            desc: fileDesc,
                            origcode: sct
                        });

                

                        me.filegrid.getStore().insert(rowlength, r);

                        
                   
                    }
                    return true;
                },

                beforeupload: function (uploadbasic, uploader, file) {
                    //console.log('beforeupload');			
                    if (me.bIsMountFile != true) {
                        //上传文件而不是替换文件时，这里要把Doc关键字重置
                        uploadbasic.multipart_params.DocKeyword = "";
                    } else {
                        uploadbasic.multipart_params.DocKeyword = me.docKeyword;
                        Ext.require('Ext.ux.Common.comm', function () {
                            //触发服务端BeforeUploadFile事件，如果是断点续传，获取已上传的文件大小
                            sendBeforeUploadFile(uploadbasic, file, me.docKeyword, me);
                        });
                        return;
                    }

                    var extIndex = file.name.lastIndexOf(".");
                    var fileCode = file.name;
                    var fileDesc = "";

                    if (extIndex >= 0) {
                        fileCode = fileCode.substring(0, extIndex);

                        var splitIndex = fileCode.indexOf("_");
                        if (splitIndex > 0) {
                            var fullCode = fileCode;
                            fileCode = fullCode.substring(0, splitIndex);
                            fileDesc = fullCode.substring(splitIndex + 1, fullCode.length);
                            //fileDesc = fullCode.substring(splitIndex + 1, fullCode.length - splitIndex - 1);
                        }
                    }

                    //修改附件名称
                    me.docUploadIndex = me.docUploadIndex + 1;
                    //fileCode = me.docCode + "附件" + me.docUploadIndex.toString();//+ " " + fileCode;

                  
                    //修改附件名称
                    //for (var i = 0; i < me.filestore.getCount() ; i++) {
                    //    var recored = me.filestore.getAt(i);
                    //    if (recored.data.id === file.id && recored.data.needNewFileCode === true) {
                    //        fileCode = recored.data.code+" "+recored.data.desc;
                    //        break;
                    //    }
                    //}
                   

                    Ext.require('Ext.ux.Common.comm', function () {
                        //先创建文档
    
                        //createDoc(uploadbasic, file, me.newProjectKeyword, fileCode, fileDesc, "CATALOGUING", function (uploadbasic, res, options, DocKeyword) {
                        createDoc(uploadbasic, file, me.newProjectKeyword, fileCode, fileDesc, "", function (uploadbasic, res, options, DocKeyword) {
                            //var state = res.success;
                            if (res.success === true) {
                                //处理创建文档后的返回事件
                                me.docList = me.docList + "," + DocKeyword;

                                for (var i = 0; i < me.filestore.getCount() ; i++) {
                                    var recored = me.filestore.getAt(i);
                                    if (recored.data.id === options.file.id) {
                                        recored.data.docKeyword = DocKeyword;
                                        break;
                                    }
                                }

                         
                                //触发服务端BeforeUploadFile事件，如果是断点续传，获取已上传的文件大小
                                sendBeforeUploadFile(uploadbasic, options.file, DocKeyword, me);
                            }
                        });
                    });
                },

                fileuploaded: function (uploader, file) {
                    for (var i = 0; i < me.filestore.getCount() ; i++) {
                        var recored = me.filestore.getAt(i);
                        if (recored.data.docKeyword != undefined && recored.data.docKeyword != "") {
                           // var docAttr = [
                           //    { name: "CA_REFERENCE", value: recored.data.reference, attrtype: "attrData" },
                           //    { name: "CA_VOLUMENUMBER", value: recored.data.volumenumber, attrtype: "attrData" },
                           // { name: "CA_REFERENCE", value: recored.data.reference, attrtype: "attrData" },
                           // { name: "CA_FILECODE", value: recored.data.code, attrtype: "attrData" },
                           // { name: "CA_ORIFILECODE", value: recored.data.origcode, attrtype: "attrData" },

                           //{ name: "CA_ATTRTEMP", value: "NONCOMM", attrtype: "attrData" }
                           // ];
                            var docAttr = [
                            ];


                            //me.updateDocAttr(recored.data.docKeyword, docAttr);
                        }
                    }
                    //console.log('fileuploaded');
                    //文件上传后的事件
                    Ext.require('Ext.ux.Common.comm', function () {
                        afterUploadFile(uploader, file, me.ServerFullFileName);
                    });
                },

                uploadcomplete: function (uploader, success, failed) {
                    //设置上传附件完毕标记
                    me.uploadCompleteState = true;
                },
                scope: this
            }, width: 80


        });

        

        //定义文档列表按钮
        me.fileTbar = Ext.create('Ext.toolbar.Toolbar', {
            xtype: 'toolbar',
            dock: 'top',
            items: [
                me.FileUploadButton,
                {
                    //iconCls: "file-create",
                    scope: me, text: '删除文件', tooltip: '删除文件', listeners: {
                        "click": function (btn, e, eOpts) {
                            me.removeFile();
                        }
                    }
                }
            ]
        });

        //定义已上传附件的model
        Ext.define("filemodel", {
            extend: "Ext.data.Model",
            fields: ["id", "no", "name", "code", "origcode", "desc"
            , "reference"
              , "volumenumber"
              , "responsibility"
              , "page"
              , "share"
              , "medium"
              , "languages"
              , "proname"
              , "procode"
              , "major"
              , "crew"
              , "factorycode"
              , "factoryname"
              , "systemcode"
              , "systemname"
              , "relationfilecode"
              , "relationfilename"
              , "filespec"
              , "fileunit"
              , "secretgrade"
              , "keepingtime"
              , "filelistcode"
              , "filelisttime"
              , "racknumber"
              , "note",
              "needNewFileCode", "fileCodeType",
              "receiveType", "fNumber", "edition",
              "workClass", "workSub", "department",
               "receiptcode",
              "originalshare", "copyshare",
              "scanshare", "elecshare",
              "docKeyword"
            ],
            url: "_blank",
        });

        //定义已上传附件的store
        me.filestore = Ext.create("Ext.data.Store", {
            model: "filemodel"
            //model: "CDMSWeb.plug_ins.ZHEPC_Plugins.model.filemodel"
        });

        me.rownumColumn = Ext.create("Ext.grid.column.Column", {
            header: '序号', xtype: 'rownumberer', dataIndex: 'no', width: 30, align: 'center', sortable: false, readOnly: true
        });

        me.nameColumn = Ext.create("Ext.grid.column.Column", {
            text: '文件名称', dataIndex: 'name', width: 500,  readOnly: true
        });

        me.codeColumn = Ext.create("Ext.grid.column.Column", {
            text: '文件编码', dataIndex: 'code', width: 190, 
        });

        me.origcodeColumn = Ext.create("Ext.grid.column.Column", {
            text: '原文件编码', dataIndex: 'origcode', width: 190, hidden:true
        });

        me.descColumn = Ext.create("Ext.grid.column.Column", {
            text: '文件题名', dataIndex: 'desc', width: 130,
            editor: {
                allowBlank: true
            }
        });


        //定义已上传附件的view
        me.filegrid = Ext.widget("grid", {
            region: "center",
            height: 438,
            //hideHeaders: true,//隐藏表头
            selType: "checkboxmodel",
            selModel: { checkOnly: false, mode: "MULTI" },
            tbar: me.fileTbar,
            flex: 1,
            store: me.filestore,
           
            columns: [
                //me.rownumColumn,
                me.nameColumn
         
            ]

        });


        me.gridMinHeight = me.filegrid.height;
        me.gridMaxHeight = me.filegrid.height;


        me.onFileEditButtonClick = function () { };

        me.titleLabel = Ext.create("Ext.form.Label", {
            text: "附件:"
        });
        //左边按钮区域
        me.leftButtonPanel = Ext.create("Ext.panel.Panel", {
            layout: "vbox",
            //width: '100%',
            width: 50,
            baseCls: 'my-panel-no-border',//隐藏边框
            align: 'stretch', margin: '5 5 0 5', padding: '0 0 0 0',
            pack: 'start', items: [
                me.titleLabel
            ]
        });

        me.items = [
    {
        layout: "vbox",
        width: '100%', baseCls: 'my-panel-no-border',//隐藏边框
        align: 'stretch', margin: '0 0 0 0', padding: '0 0 0 0',
        pack: 'start', height: 100,
        items: [
            {
                layout: "hbox",
                width: '100%', baseCls: 'my-panel-no-border',//隐藏边框
                align: 'stretch', margin: '10 0 0 0', padding: '0 0 0 0',
                pack: 'start',
                items: [
                    
                     me.leftButtonPanel
                    , me.filegrid]
            }
        ], flex: 1
    }
        ];

        me.callParent(arguments);
    },


    setGridMinHeight:function(minHeight) {
        var me = this;
        me.gridMinHeight = minHeight;
        me.setHeight(me.gridMinHeight + 10);
        me.filegrid.setHeight(me.gridMinHeight);
    },

    setGridMaxHeight: function (maxHeight) {
        var me = this;
        me.gridMaxHeight = maxHeight;
        me.setHeight(me.gridMaxHeight + 10);
        me.filegrid.setHeight(me.gridMaxHeight);
    },

    




    //删除选中的文件
    removeFile: function () {
        var me = this;

        var grid = me.filegrid;
        var rs = grid.getSelectionModel().getSelection();//获取选择的文档
        if (rs !== null && rs.length > 0) {
            for (var i = rs.length - 1; i >= 0; i--) {
                var rec = rs[i];//第一个文档
                for (var B = me.FileUploadButton.uploader.uploader.files.length - 1; B >= 0; B--) {
                    //var fileItem = me.FileUploadButton.uploader.uploader.files[B];
                    if (me.FileUploadButton.uploader.uploader.files[B].id === rec.data.id) {
                        me.FileUploadButton.uploader.uploader.files[B].status = 5;
                    }
                }
                me.filegrid.store.remove(rec);
            }
        }
    },

    updateDocAttr: function (Keyword, AttrObject) {
        var me = this;
        AttrJson = Ext.JSON.encode(AttrObject);

        Ext.Ajax.request({

            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.WebApi.DocController", A: "UpdateDocAttr",
                sid: localStorage.getItem("sid"), docKeyword: Keyword,
                docAttrJson: AttrJson
            },
            success: function (response, options) {
                var obj = Ext.decode(response.responseText);
                if (obj.success == true) {
                    me.afterUploadOneFile(Keyword);

                }
            }
        });
    },

    afterUploadOneFile: function (Keyword) { },

    setFirstFileIsPositive: function (value) {
        var me = this;
        me.firstFileIsPositive = value;
    },

    send_upload_file: function () {
        var me = this;

        me.newProjectKeyword = me.projectKeyword;

        if (me.FileUploadButton.uploader.uploader.files.length <= 0) {
            Ext.Msg.alert("错误", "请添加文件！");
            return;
        }

        if (me.FileUploadButton.uploader.uploader.files.length > 0) {
            ////当有附件时，创建DOC文档成功后，上传附件
            me.FileUploadButton.uploader.start();

            Ext.MessageBox.wait("正在上传文件，请稍候...", "等待");

            var int = window.setInterval(function () {
                //上传附件完毕
                if (me.uploadCompleteState === true) {
                    Ext.MessageBox.close();//关闭等待对话框

                    //停止线程
                    window.clearInterval(int);

                    //处理返回事件
                    me.send_upload_file_callback();


                }
            }, 500);
        } else {
            //当没有附件时，处理返回事件
            me.send_upload_file_callback(response, options, "");
        }
    },

    send_upload_file_callback: function (response, options) {
        var me = this;
        Ext.MessageBox.close();//关闭等待对话框

        me.afterUploadAllFile();
    },

    afterUploadAllFile: function () { },

    //设置为附件模式
    setAttaMode: function ()
    {
        var me = this;
        me.UploadMode = "atta";
        //me.fileTbar.hide();
    },

});
