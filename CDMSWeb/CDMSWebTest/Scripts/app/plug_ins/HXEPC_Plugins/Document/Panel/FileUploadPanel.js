Ext.define('Ext.plug_ins.HXEPC_Plugins.Document.Panel.FileUploadPanel', {
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

        me.fixedAloneFileName = "";
        me.fixedAloneDocKeyword = "";

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

        //升版时，原版的属性信息
        me.orgiAttrArray = [];

        //定义上传附件按钮
        me.FileUploadButton = Ext.create('Ext.ux.upload.Button', {
            renderTo: Ext.getBody(),
            iconCls: "file-create",
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
                    if (me.winAction === "文档升版" || me.UploadMode === "fixedAlone") {
                        for (var B = uploader.uploader.files.length - 1; B >= 0; B--) {
                            uploader.uploader.files[B].status = 5;
                        }

                        me.filestore.removeAll();
                    }

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

                        if (me.UploadMode === "fixedAlone")
                        {
                            fileCode = me.fixedAloneFileName;
                            fileDesc = "";
                        }

                        //插入行到文件grid
                        var rowlength = me.filegrid.getStore().data.length;

                        var r = Ext.create('filemodel', {
                        //var r = Ext.create("CDMSWeb.plug_ins.HXEPC_Plugins.model.filemodel", {
                            id: files[i].id,
                            name: files[i].name,
                            code: fileCode,  // === undefined ? "" : (sct.length > 0 ? sct + "附件" + (rowlength + 1) : "")
                            desc: fileDesc,
                            origcode: sct
                        });

                        if (me.winAction === "文档升版") {
                            r = me.getAttrFromOrgArray(r);
                        }

                        me.filegrid.getStore().insert(rowlength, r);

                        
                        if (me.UploadMode === "fixedAlone")
                        {
                            break;
                        }
                    }
                    return true;
                },

                beforeupload: function (uploadbasic, uploader, file) {
                    //console.log('beforeupload');			
                    //上传文件而不是替换文件时，这里要把Doc关键字重置
                    uploadbasic.multipart_params.DocKeyword = "";

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
                    //me.docUploadIndex = me.docUploadIndex + 1;
                    //fileCode = me.docCode + "附件" + me.docUploadIndex.toString();//+ " " + fileCode;

                    if (me.UploadMode === "fixedAlone") {
                        fileCode = me.fixedAloneFileName;
                        fileDesc = "";
                    }

                    //修改附件名称
                    for (var i = 0; i < me.filestore.getCount() ; i++) {
                        var recored = me.filestore.getAt(i);
                        if (recored.data.id === file.id && recored.data.needNewFileCode === true) {
                            fileCode = recored.data.code+" "+recored.data.desc;
                            break;
                        }
                    }
                   

                    Ext.require('Ext.ux.Common.comm', function () {
                        //先创建文档
                        
                        ////如果是上传附件，且第一个是正件，就无需创建文档，直接把第一个文件作为正件文档的文件
                        //if (me.UploadMode === "atta" && me.firstFileIsPositive && file.index===0) {
                        //    for (var i = 0; i < me.filestore.getCount() ; i++) {
                        //        var recored = me.filestore.getAt(i);
                        //        if (recored.data.id === file.id) {
                        //            recored.data.docKeyword = me.docKeyword;
                        //            break;
                        //        }
                        //    }

                        //    sendBeforeUploadFile(uploadbasic, file, me.docKeyword, me);
                        //    return;
                        //}

                        createDoc(uploadbasic, file, me.newProjectKeyword, fileCode, fileDesc, "CATALOGUING", function (uploadbasic, res, options, DocKeyword) {
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

                                if (me.UploadMode === "fixedAlone") {
                                    me.fixedAloneDocKeyword = DocKeyword;
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
                            var docAttr = [
                               { name: "CA_REFERENCE", value: recored.data.reference, attrtype: "attrData" },
                               { name: "CA_VOLUMENUMBER", value: recored.data.volumenumber, attrtype: "attrData" },
                            { name: "CA_REFERENCE", value: recored.data.reference, attrtype: "attrData" },
                            { name: "CA_FILECODE", value: recored.data.code, attrtype: "attrData" },
                            { name: "CA_ORIFILECODE", value: recored.data.origcode, attrtype: "attrData" },
                            //责任人
                           { name: "CA_RESPONSIBILITY", value: recored.data.responsibility, attrtype: "attrData" },
                           { name: "CA_FILETITLE", value: recored.data.desc, attrtype: "attrData" },
                           { name: "CA_PAGE", value: recored.data.page, attrtype: "attrData" },
                           { name: "CA_NUMBER", value: recored.data.share, attrtype: "attrData" },
                           { name: "CA_MEDIUM", value: recored.data.medium, attrtype: "attrData" },
                           { name: "CA_LANGUAGES", value: recored.data.languages, attrtype: "attrData" },
                           { name: "CA_PRONAME", value: recored.data.proname, attrtype: "attrData" },
                           { name: "CA_PROCODE", value: recored.data.procode, attrtype: "attrData" },
                           { name: "CA_MAJOR", value: recored.data.major, attrtype: "attrData" },
                           { name: "CA_CREW", value: recored.data.crew, attrtype: "attrData" },
                           { name: "CA_FACTORY", value: recored.data.factoryCode, attrtype: "attrData" },
                           { name: "CA_FACTORYNAME", value: recored.data.factoryname, attrtype: "attrData" },
                           { name: "CA_SYSTEM", value: recored.data.systemcode, attrtype: "attrData" },
                           { name: "CA_SYSTEMNAME", value: recored.data.systemname, attrtype: "attrData" },
                           { name: "CA_RELATIONFILECODE", value: recored.data.relationfilecode, attrtype: "attrData" },
                           { name: "CA_RELATIONFILENAME", value: recored.data.relationfilename, attrtype: "attrData" },
                           { name: "CA_FILESPEC", value: recored.data.filespec, attrtype: "attrData" },
                           { name: "CA_FILEUNIT", value: recored.data.fileunit, attrtype: "attrData" },
                           { name: "CA_SECRETGRADE", value: recored.data.secretgrade, attrtype: "attrData" },
                           { name: "CA_KEEPINGTIME", value: recored.data.keepingtime, attrtype: "attrData" },
                           { name: "CA_FILELISTCODE", value: recored.data.filelistcode, attrtype: "attrData" },
                           { name: "CA_FILELISTTIME", value: recored.data.filelisttime, attrtype: "attrData" },
                           { name: "CA_RACKNUMBER", value: recored.data.racknumber, attrtype: "attrData" },
                           { name: "CA_NOTE", value: recored.data.note, attrtype: "attrData" },
                           { name: "CA_WORKTYPE", value: recored.data.workClass, attrtype: "attrData" },
                           { name: "CA_WORKSUBTIEM", value: recored.data.workSub, attrtype: "attrData" },
                           { name: "CA_UNIT", value: recored.data.department, attrtype: "attrData" },
                           { name: "CA_FILETYPE", value: recored.data.receiveType, attrtype: "attrData" },
                           { name: "CA_FLOWNUMBER", value: recored.data.fNumber, attrtype: "attrData" },
                           { name: "CA_EDITION", value: recored.data.edition, attrtype: "attrData" },

                           //运营管理类批量上传用到的属性
                           { name: "CA_DESIGN", value: recored.data.design, attrtype: "attrData" },
                           { name: "CA_APPROVTIME", value: recored.data.approvtime, attrtype: "attrData" },
                           { name: "CA_MAINFEEDER", value: recored.data.mainfeeder, attrtype: "attrData" },
                           { name: "CA_COPY", value: recored.data.copy, attrtype: "attrData" },
                           { name: "CA_SENDDATE", value: recored.data.senddate, attrtype: "attrData" },
                           { name: "CA_IFREPLY", value: recored.data.ifreply, attrtype: "attrData" },
                           { name: "CA_REPLYDATE", value: recored.data.replydate, attrtype: "attrData" },
                           { name: "CA_REPLYCODE", value: recored.data.replycode, attrtype: "attrData" },
                           { name: "CA_REPLYTIME", value: recored.data.replytime, attrtype: "attrData" },

                           { name: "CA_ATTRTEMP", value: "NONCOMM", attrtype: "attrData" }
                            ];


                            me.updateDocAttr(recored.data.docKeyword, docAttr);
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

        
        //保存附件按钮
        me.fileSaveButton = Ext.widget('button', {
            text: "保存", width: 55, margins: "5 0 8 0", hidden: true,
            iconCls: "file-create",
            listeners: {
                "click": function (btn, e, eOpts) {//添加点击按钮事件

                    //获取总页数
                    var tPages = 0;
                    for (var i = 0; i < me.filestore.getCount() ; i++) {
                        var recored = me.filestore.getAt(i);
                        if (recored.data.page === undefined || recored.data.page === "") {
                            Ext.Msg.alert("错误", "请填写文件【" + recored.data.origcode + "】的页数！");
                            return;
                        } else if (recored.data.desc === undefined || recored.data.desc === "") {
                            Ext.Msg.alert("错误", "请填写文件【" + recored.data.origcode + "】的文件题名！");
                            return;
                        } else if (recored.data.code === undefined || recored.data.code === "") {
                            Ext.Msg.alert("错误", "请填写文件【" + recored.data.origcode + "】的文件编码！");
                            return;
                        } else if (recored.data.medium === undefined || recored.data.medium === "") {
                            Ext.Msg.alert("错误", "请填写文件【" + recored.data.origcode + "】的份数！");
                            return;
                        }

                        var pageItem = Number(recored.data.page);//parseInt(recored.data.page);
                        if (String(pageItem) === "NaN")// <= 0)//= "NaN")
                        {
                            Ext.Msg.alert("错误", "文件【" + recored.data.origcode + "】的页数填写错误，请重新填写！");
                            return;
                        }
                        tPages = tPages + pageItem;
                    }

                    me.totalPages = tPages;

                    me.fileTbar.hide();

                    me.setHeight(me.gridMinHeight + 10);
                    me.filegrid.setHeight(me.gridMinHeight);

                    me.FileUploadButton.hide();
                    //me.fileEditButton.show();
                    me.leftButtonPanel.show();

                    me.fileSaveButton.hide();
                    //me.filegrid.setHeight(98);


                    me.onFileSaveButtonClick();

                }
            }
        });

        me.onFileSaveButtonClick = function () { };

        me.editAttrButton = Ext.widget('button', {
            iconCls: "file-create", scope: me, text: '修改属性', tooltip: '修改属性', listeners: {
                "click": function (btn, e, eOpts) {
                    me.editFileAttr();
                }
            }

        });

        me.beforeEditAttr = function () {
            return true;
        }

        me.afterEditAttr = function () { }

        me.batImportButton = Ext.widget('button', {
            iconCls: "file-create", scope: me, text: '批量导入', tooltip: '批量导入', listeners: {
                "click": function (btn, e, eOpts) {
                    me.batEditAttr();
                }
            }
        });

        //定义文档列表按钮
        me.fileTbar = Ext.create('Ext.toolbar.Toolbar', {
            xtype: 'toolbar',
            dock: 'top',
            items: [
                me.FileUploadButton,
                {
                    iconCls: "file-create", scope: me, text: '删除文件', tooltip: '删除文件', listeners: {
                        "click": function (btn, e, eOpts) {
                            me.removeFile();
                        }
                    }
                },
                me.editAttrButton,
                //{
                //    iconCls: "file-create", scope: me, text: '修改属性', tooltip: '修改属性', listeners: {
                //        "click": function (btn, e, eOpts) {
                //            me.editFileAttr();
                //        }
                //    }
                //},
                me.batImportButton,
                 //{
                 //    iconCls: "file-create", scope: me, text: '批量导入', tooltip: '批量导入', listeners: {
                 //        "click": function (btn, e, eOpts) {
                 //            me.batEditAttr();
                 //        }
                 //    }
                 //},
                me.fileSaveButton
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
            //model: "CDMSWeb.plug_ins.HXEPC_Plugins.model.filemodel"
        });

        me.rownumColumn = Ext.create("Ext.grid.column.Column", {
            header: '序号', xtype: 'rownumberer', dataIndex: 'no', width: 30, align: 'center', sortable: false, readOnly: true
        });

        me.nameColumn = Ext.create("Ext.grid.column.Column", {
            text: '文件名称', dataIndex: 'name', width: 150, hidden: true, readOnly: true
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

        me.referenceColumn = Ext.create("Ext.grid.column.Column", {
            text: '档号', dataIndex: 'reference', width: 60, hidden: true
        });

        me.volumenumberColumn = Ext.create("Ext.grid.column.Column", {
            text: '卷内序号', dataIndex: 'volumenumber', width: 60, hidden: true
        });

        me.responsibilityColumn = Ext.create("Ext.grid.column.Column", {
            text: '责任人', dataIndex: 'responsibility', width: 60, hidden: true
        });

        me.pageColumn = Ext.create("Ext.grid.column.Column", {
            text: '页数', dataIndex: 'page', width: 60, hidden: true,
            editor: {
                allowBlank: true
            }

        });

        me.editionColumn = Ext.create("Ext.grid.column.Column", {
            text: '版次', dataIndex: 'edition', width: 60, hidden: true,
            editor: {
                allowBlank: true
            }

        });

        me.receiptcodeColumn = Ext.create("Ext.grid.column.Column", {
            text: '收文编码', dataIndex: 'receiptcode', width: 100, hidden: true,
            editor: {
                allowBlank: true
            }
        });

        me.originalshareColumn = Ext.create("Ext.grid.column.Column", {
            text: '原文份数', dataIndex: 'originalshare', width: 60, hidden: true,
            editor: {
                allowBlank: true
            }
        });

        me.copyshareColumn = Ext.create("Ext.grid.column.Column", {
            text: '复印件份数', dataIndex: 'copyshare', width: 80, hidden: true,
            editor: {
                allowBlank: true
            }
        });

        me.scanshareColumn = Ext.create("Ext.grid.column.Column", {
            text: '扫描件份数', dataIndex: 'scanshare', width: 80, hidden: true,
            editor: {
                allowBlank: true
            }
        });

        me.elecshareColumn = Ext.create("Ext.grid.column.Column", {
            text: '电子文件份数', dataIndex: 'elecshare', width: 80, hidden: true,
            editor: {
                allowBlank: true
            }
        });

        me.shareColumn = Ext.create("Ext.grid.column.Column", {
            text: '份数', dataIndex: 'share', width: 60, hidden: true

        });
        me.mediumColumn = Ext.create("Ext.grid.column.Column", {
            text: '介质', dataIndex: 'medium', width: 60, hidden: true

        });
        me.languagesColumn = Ext.create("Ext.grid.column.Column", {
            text: '语种', dataIndex: 'languages', width: 60, hidden: true

        });
        me.pronameColumn = Ext.create("Ext.grid.column.Column", {
            text: '项目名称', dataIndex: 'proname', width: 60, hidden: true

        });
        me.procodeColumn = Ext.create("Ext.grid.column.Column", {
            text: '项目代码', dataIndex: 'procode', width: 60, hidden: true

        });
        me.majorColumn = Ext.create("Ext.grid.column.Column", {
            text: '专业', dataIndex: 'major', width: 60, hidden: true

        });
        me.crewColumn = Ext.create("Ext.grid.column.Column", {
            text: '机组', dataIndex: 'crew', width: 60, hidden: true

        });
        me.factorycodeColumn = Ext.create("Ext.grid.column.Column", {
            text: '厂房代码', dataIndex: 'factorycode', width: 60, hidden: true

        });
        me.factorynameColumn = Ext.create("Ext.grid.column.Column", {
            text: '厂家名称', dataIndex: 'factoryname', width: 60, hidden: true

        });
        me.systemcodeColumn = Ext.create("Ext.grid.column.Column", {
            text: '系统代码', dataIndex: 'systemcode', width: 60, hidden: true

        });
        me.systemnameColumn = Ext.create("Ext.grid.column.Column", {
            text: '系统名称', dataIndex: 'systemname', width: 60, hidden: true

        });
        me.relationfilecodeColumn = Ext.create("Ext.grid.column.Column", {
            text: '关联文件编码', dataIndex: 'relationfilecode', width: 60, hidden: true

        });
        me.relationfilenameColumn = Ext.create("Ext.grid.column.Column", {
            text: '关联文件名称', dataIndex: 'relationfilename', width: 60, hidden: true

        });
        me.filespecColumn = Ext.create("Ext.grid.column.Column", {
            text: '案卷规格', dataIndex: 'filespec', width: 60, hidden: true

        });
        me.fileunitColumn = Ext.create("Ext.grid.column.Column", {
            text: '归档单位', dataIndex: 'fileunit', width: 60, hidden: true

        });
        me.secretgradeColumn = Ext.create("Ext.grid.column.Column", {
            text: '密级', dataIndex: 'secretgrade', width: 60, hidden: true

        });
        me.keepingtimeColumn = Ext.create("Ext.grid.column.Column", {
            text: '保管期限', dataIndex: 'keepingtime', width: 60, hidden: true

        });
        me.filelistcodeColumn = Ext.create("Ext.grid.column.Column", {
            text: '清单编码', dataIndex: 'filelistcode', width: 60, hidden: true

        });
        me.filelisttimeColumn = Ext.create("Ext.grid.column.Column", {
            text: '归档日期', dataIndex: 'filelisttime', width: 60, hidden: true

        });
        me.racknumberColumn = Ext.create("Ext.grid.column.Column", {
            text: '排架号', dataIndex: 'racknumber', width: 60, hidden: true

        });
        me.noteColumn = Ext.create("Ext.grid.column.Column", {
            text: '备注', dataIndex: 'note', width: 60, hidden: true,
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
            listeners: {
                "edit": function (editor, e) {//去除红色箭头
                    e.record.commit();
                }
            },
            plugins: [
                     Ext.create('Ext.grid.plugin.CellEditing', {
                         clicksToEdit: 1 //设置单击单元格编辑(设置为2是双击进行修改)
                     })
            ],
            viewConfig: {
                plugins: {
                    ptype: 'gridviewdragdrop',
                    ddGroup: 'DragDropGroup'//此处代表拖动的组 拖动组件与放置组件要同属一组才能实现相互拖放  
                }
            },
            columns: [
                me.rownumColumn,
                me.codeColumn,
                me.origcodeColumn,
                me.descColumn,
                me.referenceColumn,
                me.volumenumberColumn,
                me.responsibilityColumn,
                me.pageColumn,
                me.editionColumn,
                me.receiptcodeColumn,//收文编码
                me.originalshareColumn,//原文份数 
                me.copyshareColumn,//复印件份数
                me.scanshareColumn,//扫描件份数
                me.elecshareColumn,//电子文件份数
                me.shareColumn,
                me.mediumColumn,
                me.languagesColumn,
                me.pronameColumn,
                me.procodeColumn,
                me.majorColumn,
                me.crewColumn,
                me.factorycodeColumn,
                me.factorynameColumn,
                me.systemcodeColumn,
                me.systemnameColumn,
                me.relationfilecodeColumn,
                me.relationfilenameColumn,
                me.filespecColumn,
                me.fileunitColumn,
                me.secretgradeColumn,
                me.keepingtimeColumn,
                me.filelistcodeColumn,
                me.filelisttimeColumn,
                me.racknumberColumn,
                me.noteColumn
            ]

        });


        me.gridMinHeight = me.filegrid.height;
        me.gridMaxHeight = me.filegrid.height;

        //编辑附件按钮
        me.fileEditButton = Ext.create("Ext.button.Button", {
            //xtype: "button",
            text: "附件", width: 48, margins: "0 0 8 0",
            listeners: {
                "click": function (btn, e, eOpts) {//添加点击按钮事件

                    me.fileTbar.show();

                    me.FileUploadButton.show();
                    me.fileSaveButton.show();
                    //me.fileEditButton.hide();
                    me.leftButtonPanel.hide();

                    me.onFileEditButtonClick();
                }
            }
        });
        me.onFileEditButtonClick = function () { };

        //左边按钮区域
        me.leftButtonPanel = Ext.create("Ext.panel.Panel", {
            layout: "vbox",
            //width: '100%',
            width: 50,
            baseCls: 'my-panel-no-border',//隐藏边框
            align: 'stretch', margin: '5 5 0 12', padding: '0 0 0 0',
            pack: 'start', items: [
             me.fileEditButton
            ]
        });

        me.items = [
    {
        layout: "vbox",
        width: '100%', baseCls: 'my-panel-no-border',//隐藏边框
        align: 'stretch', margin: '0 10 0 0', padding: '0 0 0 0',
        pack: 'start', height: 100,
        items: [
            {
                layout: "hbox",
                width: '100%', baseCls: 'my-panel-no-border',//隐藏边框
                align: 'stretch', margin: '10 0 0 10', padding: '0 0 0 0',
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

    


    //修改文件属性
    editFileAttr: function () {
        var me = this;
        //Ext.Msg.alert("错误", "修改文件属性");
        if (me.beforeEditAttr() === false) return;

        var grid = me.filegrid;
        var rs = grid.getSelectionModel().getSelection();//获取选择的文档
        if (rs !== null && rs.length > 0) {
            var rec = rs[0];//第一个文档
            var Keyword = rec.data.Keyword;
            //me.curSelectGridRecod = rec;

            //弹出操作窗口
            var _fmEditFileProperties = Ext.create('Ext.plug_ins.HXEPC_Plugins.Document.EditFileProperties', {
                title: "", projectKeyword: me.projectKeyword, projectDirKeyword: me.projectDirKeyword,
                docClass: me.docClass
            });

            winEditFileProperties = Ext.widget('window', {
                title: '修改文件著录属性',
                closeAction: 'hide',
                width: 780,
                height: 466,
                minWidth: 300,
                minHeight: 300,
                layout: 'fit',
                resizable: true,
                modal: true,
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: _fmEditFileProperties,
                defaultFocus: 'firstName'
            });

            if (rec.data.procode === "" && rec.data.department === "") {
                //项目代码
                rec.set('procode', me.defaultProCode);
                //项目代码
                rec.set('proname', me.defaultProName);
                //部门代码
                rec.set('department', me.defaultDepartment);
                //文件编码类型,"项目管理类"或"运营管理类"
                rec.set('fileCodeType', me.defaultFileCodeType);
                rec.commit();
            }
            
            //文件题名
            _fmEditFileProperties.setFilePropertiesDefault(rec.data);

            window.parent.resultarray = undefined;

            //winImportFile.hide();
            winEditFileProperties.show();
            //监听子窗口关闭事件
            winEditFileProperties.on('close', function () {
                //winImportFile.show();

                if (window.parent.resultarray === undefined) { return; }
                var res = window.parent.resultarray[0];
                //var Keyword = rec.data.name;
                //文件编码
                rec.set('code', res.code);
                //文件题名
                rec.set('desc', res.desc);
                //档号
                rec.set('reference', res.reference);
                //卷内序号
                rec.set('volumenumber', res.volumenumber);
                //责任人
                rec.set('responsibility', res.responsibility);
                //页数
                rec.set('page', res.page);
                //份数
                rec.set('share', res.share);
                //介质
                rec.set('medium', res.medium);
                //语种
                rec.set('languages', res.languages);
                //项目名称
                rec.set('proname', res.proname);
                //项目代码
                rec.set('procode', res.procode);
                //专业
                rec.set('major', res.major);
                //机组
                rec.set('crew', res.crew);
                //厂房代码
                rec.set('factorycode', res.factorycode);
                //厂房名称
                rec.set('factoryname', res.factoryname);
                //系统代码
                rec.set('systemcode', res.systemcode);
                //系统名称
                rec.set('systemname', res.systemname);
                //关联文件编码
                rec.set('relationfilecode', res.relationfilecode);
                //关联文件题名
                rec.set('relationfilename', res.relationfilename);
                //案卷规格
                rec.set('filespec', res.filespec);
                //归档单位
                rec.set('fileunit', res.fileunit);
                //密级
                rec.set('secretgrade', res.secretgrade);
                //保管时间
                rec.set('keepingtime', res.keepingtime);
                //归档文件清单编码
                rec.set('filelistcode', res.filelistcode);
                //归档日期
                rec.set('filelisttime', res.filelisttime);
                //排架号
                rec.set('racknumber', res.racknumber);
                //备注
                rec.set('note', res.note);

                //是否新建文件编码
                rec.set('needNewFileCode', res.needNewFileCode);
                //文件编码类型
                rec.set('fileCodeType', res.fileCodeType);

                //文件类型
                rec.set('receiveType', res.receiveType);
                //流水号
                rec.set('fNumber', res.fNumber);
                //版本
                rec.set('edition', res.edition);

                //工作分类代码
                rec.set('workClass', res.workClass);
                //工作分项代码
                rec.set('workSub', res.workSub);
                //部门代码
                rec.set('department', res.department);

                //收文编码
                rec.set('receiptcode', res.receiptcode);

                //原件份数
                rec.set('originalshare', res.originalshare);
                //复印件份数
                rec.set('copyshare', res.copyshare);
                //扫描件份数
                rec.set('scanshare', res.scanshare);
                //电子文件份数
                rec.set('elecshare', res.elecshare);

                rec.commit();

                me.afterEditAttr(res);
            });


        } else {
            if (me.filegrid.store.getCount() === 1) {
                var model = me.filegrid.getSelectionModel();
                //model.selectAll();//选择所有行  
                model.select(0);

                me.editFileAttr();
            }
            else {
                Ext.Msg.alert("错误", "请选择要修改著录属性的文件！");
            }
        }
    },

    //批量修改文件属性
    batEditAttr: function () {
        var me = this;
        
        //弹出操作窗口
        var _fmBatchEditFileAttr = Ext.create('Ext.plug_ins.HXEPC_Plugins.Document.BatchEditFileAttr', { title: "", mainPanelId: me.id });

        winBatchEditFileAttr = Ext.widget('window', {
            title: '批量导入著录属性',
            closeAction: 'hide',
            width: 500,
            height: 436,
            minWidth: 500,
            minHeight: 430,
            layout: 'fit',
            resizable: true,
            modal: true,
            closeAction: 'close', //close 关闭  hide  隐藏  
            items: _fmBatchEditFileAttr,
            defaultFocus: 'firstName'
        });

        _fmBatchEditFileAttr.mainPanelId = me.id;

        _fmBatchEditFileAttr.projectKeyword = me.projectKeyword;

        winBatchEditFileAttr.show();
        //监听子窗口关闭事件
        //winBatchEditFileAttr.on('close', function () {
        //   // me.sendEditFileAttrDefault();
        //});



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


    //升版时获取原版本信息
    setImportFileDefault: function () {
        var me = this;
        Ext.Ajax.request({

            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.HXEPC_Plugins.Document", A: "GetCataloguAttr",
                sid: localStorage.getItem("sid"), docKeyword: me.docKeyword
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

                    me.orgiAttrArray =
                      [
                          {

                              needNewFileCode: true, fileCodeType: "",
                              code: recod.FileCode, origcode: "", desc: "",
                              reference: recod.CA_REFERENCE, volumenumber: recod.CA_VOLUMENUMBER,
                              responsibility: recod.CA_RESPONSIBILITY, page: recod.CA_PAGE,
                              share: recod.CA_NUMBER, medium: recod.CA_MEDIUM, languages: recod.CA_LANGUAGES,
                              proname: recod.CA_PRONAME, procode: recod.CA_PROCODE,

                              major: recod.CA_MAJOR, crew: recod.CA_CREW,
                              factorycode: recod.CA_FACTORY, factoryname: recod.CA_FACTORYNAME,
                              systemcode: recod.CA_SYSTEM, systemname: recod.CA_SYSTEMNAME,

                              workClass: recod.CA_WORKTYPE, workSub: recod.CA_WORKSUBTIEM,
                              department: recod.CA_UNIT,

                              receiveType: recod.CA_FILETYPE, fNumber: recod.CA_FLOWNUMBER,
                              edition: recod.CA_EDITION,

                              relationfilecode: recod.CA_RELATIONFILECODE, relationfilename: recod.CA_RELATIONFILENAME,
                              filespec: recod.CA_FILESPECc, fileunit: recod.CA_FILEUNIT,
                              secretgrade: recod.CA_SECRETGRADE, keepingtime: recod.CA_KEEPINGTIME,
                              filelistcode: recod.CA_FILELISTCODE, filelisttime: recod.CA_FILELISTTIME,
                              racknumber: recod.CA_RACKNUMBER, note: recod.CA_NOTE
                          }
                      ]
                }
            }
        });
    },

    getAttrFromOrgArray: function (rec) {
        var me = this;
        if (me.orgiAttrArray === undefined || me.orgiAttrArray.length <= 0) {
            return rec;
        }
        var res = me.orgiAttrArray[0];
        //var Keyword = rec.data.name;
        //文件编码
        //rec.set('code', res.code);
        //文件题名
        //rec.set('desc', res.desc);
        //档号
        rec.set('reference', res.reference);
        //卷内序号
        rec.set('volumenumber', res.volumenumber);
        //责任人
        rec.set('responsibility', res.responsibility);
        //页数
        rec.set('page', res.page);
        //份数
        rec.set('share', res.share);
        //介质
        rec.set('medium', res.medium);
        //语种
        rec.set('languages', res.languages);
        //项目名称
        rec.set('proname', res.proname);
        //项目代码
        rec.set('procode', res.procode);
        //专业
        rec.set('major', res.major);
        //机组
        rec.set('crew', res.crew);
        //厂房代码
        rec.set('factorycode', res.factorycode);
        //厂房名称
        rec.set('factoryname', res.factoryname);
        //系统代码
        rec.set('systemcode', res.systemcode);
        //系统名称
        rec.set('systemname', res.systemname);
        //关联文件编码
        rec.set('relationfilecode', res.relationfilecode);
        //关联文件题名
        rec.set('relationfilename', res.relationfilename);
        //案卷规格
        rec.set('filespec', res.filespec);
        //归档单位
        rec.set('fileunit', res.fileunit);
        //密级
        rec.set('secretgrade', res.secretgrade);
        //保管时间
        rec.set('keepingtime', res.keepingtime);
        //归档文件清单编码
        rec.set('filelistcode', res.filelistcode);
        //归档日期
        rec.set('filelisttime', res.filelisttime);
        //排架号
        rec.set('racknumber', res.racknumber);
        //备注
        rec.set('note', res.note);

        //是否新建文件编码
        rec.set('needNewFileCode', res.needNewFileCode);
        //文件编码类型
        rec.set('fileCodeType', res.fileCodeType);

        //文件类型
        rec.set('receiveType', res.receiveType);
        //流水号
        rec.set('fNumber', res.fNumber);
        //版本
        rec.set('edition', res.edition);

        //工作分类代码
        rec.set('workClass', res.workClass);
        //工作分项代码
        rec.set('workSub', res.workSub);
        //部门代码
        rec.set('department', res.department);

        //收文编码
        rec.set('receiptcode', res.receiptcode);

        //原件份数
        rec.set('originalshare', res.originalshare);
        //复印件份数
        rec.set('copyshare', res.copyshare);
        //扫描件份数
        rec.set('scanshare', res.scanshare);
        //电子文件份数
        rec.set('elecshare', res.elecshare);

        rec.commit();

        return rec;
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

        if (me.UploadMode === "import") {
            for (var i = 0; i < me.filestore.getCount() ; i++) {
                var recored = me.filestore.getAt(i);
                if (recored.data.page === undefined || recored.data.page === "") {
                    Ext.Msg.alert("错误", "请填写文件【" + recored.data.origcode + "】的页数！");
                    return;
                }
                if (recored.data.share === undefined || recored.data.share === "") {
                    Ext.Msg.alert("错误", "请填写文件【" + recored.data.origcode + "】的份数！");
                    return;
                }
            }
        }

        if (me.FileUploadButton.uploader.uploader.files.length > 0) {
            ////当有附件时，创建DOC文档成功后，上传附件
            me.FileUploadButton.uploader.start();

            Ext.MessageBox.wait("正在上传文件，请稍候...", "等待");

            var int = window.setInterval(function () {
                //上传附件完毕
                if (me.uploadCompleteState === true) {
                    Ext.MessageBox.close();//关闭等待对话框
                    //处理返回事件
                    me.send_upload_file_callback();//response, options, "");//, me.projectKeyword, closeWin);
                    //me.send_create_doc_callback(projectKeyword, docKeyword, me.docList, true);
                    //停止线程
                    window.clearInterval(int);
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
        me.fileTbar.hide();
    },

    //设置文件表格模式
    setFileGridMode: function (mode) {
        var me = this;
        //会议纪要模式
        if (mode === "MOM" || mode === "NOT" || mode === "LET" || mode === "REC" ) {
            me.rownumColumn.show();
            me.codeColumn.show();
            me.descColumn.show();
            me.pageColumn.show();
            me.editionColumn.show();
            me.noteColumn.show();
        }
        //文件传递单模式
        else if ( mode === "TRA" ) {
            me.rownumColumn.show();
            me.codeColumn.show();
            me.descColumn.show();
            me.pageColumn.show();
            me.editionColumn.show();
            me.receiptcodeColumn.show();
            me.originalshareColumn.show();
            me.copyshareColumn.show();
            me.scanshareColumn.show();
            me.elecshareColumn.show();
            me.noteColumn.show();
        }
        
        else if (mode === "import") {
            me.rownumColumn.show();
            me.codeColumn.show();
            me.origcodeColumn.show();
            me.descColumn.show();
            me.referenceColumn.show();
            me.volumenumberColumn.show();
            me.responsibilityColumn.show();
            me.pageColumn.show();
            me.shareColumn.show();
            me.mediumColumn.show();
            me.languagesColumn.show();
            me.pronameColumn.show();
            me.procodeColumn.show();
            me.majorColumn.show();
            me.crewColumn.show();
            me.factorycodeColumn.show();
            me.factorynameColumn.show();
            me.systemcodeColumn.show();
            me.systemnameColumn.show();
            me.relationfilecodeColumn.show();
            me.relationfilenameColumn.show();
            me.filespecColumn.show();
            me.fileunitColumn.show();
            me.secretgradeColumn.show();
            me.keepingtimeColumn.show();
            me.filelistcodeColumn.show();
            me.filelisttimeColumn.show();
            me.racknumberColumn.show();
            me.noteColumn.show();

        }

        else if (mode === "fixedAlone") {
            me.rownumColumn.show();
            me.codeColumn.show();
            me.origcodeColumn.show();
            me.descColumn.show();
        }
    },

    //设置默认发文部门
    setSendCompany: function (code, desc) {
        var me = this;
        me.defaultDepartment = code;
    },

    //设置项目管理类文件里项目的代码和描述
    setRootProject: function (code, desc)
    {
        var me = this;
        me.defaultProCode = code;
        me.defaultProName = desc;

        if (code != undefined && code != "") {
            me.defaultFileCodeType = "项目管理类";
        } else {
            me.defaultFileCodeType = "运营管理类";
        }
    },

    //设置为非通信文件导入模式
    setImportMode: function () {

        var me = this;
        me.UploadMode = "import";
        //me.fileTbar.hide();
        me.leftButtonPanel.hide();
        me.setGridMaxHeight(430);
    },

    //设置为认质认价流程手写校审意见单文件导入模式
    //固定单独一个文件
    setFixedAloneMode: function (fileName,docKeyword) {

        var me = this;
        me.UploadMode = "fixedAlone";
        me.leftButtonPanel.hide();
        me.setGridMaxHeight(430);

        me.fixedAloneFileName = fileName;
        me.fixedAloneDocKeyword = docKeyword;
    }
});
