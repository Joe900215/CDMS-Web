//新建目录
Ext.define('Ext.plug_ins.SysPlugins._ModiDocAttr', {
    extend: 'Ext.container.Container',
    alias: 'widget._ModiDocAttr',
    //layout: "border",
    layout: 'fit',
    resultvalue: '',mainPanelId:'',
    initComponent: function () {
        var me = this;
        me.winAction = "";//记录是新建文档还是修改文档属性
        me.tempDefnId = "";//记录模板ID
        me.projectKeyword = "";//记录目录Keyword
        me.docKeyword = "";//记录文档Keyword
        me.isUploadedFile = false;//记录是否上传了文件
        me.lastUploadedFile = null;//记录成功上传的最后一个文件
        //me.mainmainPanelId = "";

        //定义文档名Text
        me.docCodeText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "名字", anchor: "80%", labelWidth: 60, labelAlign: "left", width: 360, fieldStyle: 'border-color: red; background-image: none;'//红色边框//flex: 1
        });

        //定义文档描述Text
        me.docDescText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "描述", anchor: "80%", labelWidth: 60, labelAlign: "left", width: 360
        });

        //定义文档模板代码Text
        me.tempDefnCodeText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "模板", readOnly: true, anchor: "80%", labelWidth: 60, labelAlign: "left", margins: "15 8 15 8", width: 360
        });

        //定义文档模板描述Text
        me.tempDefnDescText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "描述", readOnly: true, anchor: "80%", labelWidth: 60, labelAlign: "left", width: 360
        });

        //定义文档名Text
        me.fileNameText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "文件名", readOnly: true, anchor: "80%", labelWidth: 60, labelAlign: "left", margins: "15 8 15 8", width: 360// flex: 1//width: 360
        });

        //定义文档描述Text
        me.fileSizeText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "文件大小", readOnly: true, anchor: "80%", labelWidth: 60, labelAlign: "left", margins: "15 8 15 8", width: 360
        });

        //添加上传文件控件
        me.FileUploadButton = Ext.create('Ext.ux.upload.Button', {
            renderTo: Ext.getBody(),
            text: '上传文件',
            plugins: [{
                ptype: 'ux.upload.window',
                //ptype: Ext.ux.upload.plugin.Window,
                title: '上传文件',
                width: 520,
                height: 350
            }
            ],  margins: "15 0 15 0", //width: 60,
            uploader:
            {
                url: 'WebApi/Post',
                uploadpath: '/Root/files',
                autoStart: true,///选择文件后是否自动上传
                max_file_size: '20020mb',
                multi_selection: false,//每次只能上传一个文档
                multipart_params: { 'guid': '', 'ProjectKeyword': '', 'sid': '', 'DocKeyword': '', 'C': 'AVEVA.CDMS.WebApi.FileController', 'A': 'UploadFile', 'SureReplace': 'false', 'AppendFilePath': '', 'user': '' }, //设置你的参数//{},
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
                beforestart: function (uploader, files) {

                },
                uploadstarted: function (uploader, files) {

                    //var tfiles = files;

                    //Ext.MessageBox.show({
                    //    title: '确认替换文件',
                    //    msg: ' 原来文件名称是:xxxx  使用新文件: xxxx 替换？',
                    //    buttons: Ext.MessageBox.YESNO,
                    //    buttonText: {
                    //        yes: "是",
                    //        no: "否"
                    //    },
                    //    fn: function (btn, parentFuctionName) {
                    //        if (btn === "yes") {
                    //            uploader.uploader.addFile(tfiles[0]);
                    //            uploader.uploader.start();
                    //        } else {
                    //            //uploader.uploader.stop();
                    //        }
                    //    }
                    //});
                    //uploader.uploader.stop();
                },

                filesadded: function (uploader, files) {

                    uploader.multipart_params.ProjectKeyword = me.projectKeyword;//me.maindocgrid.store.proxy.extraParams.ProjectKeyWord;
                    uploader.multipart_params.DocKeyword = me.docKeyword;

                    uploader.multipart_params.sid = localStorage.getItem("sid");

                    //            uploader.multipart_params.DocKeyword = me.docKeyword;
                      //          uploader.multipart_params.sid = localStorage.getItem("sid");
                                return true;

                },

                beforeupload: function (uploadbasic, uploader, file) {
                    //console.log('beforeupload');			

                    Ext.require('Ext.ux.Common.comm', function () {
                        var ObjectKeyword = "";
                        if (me.winAction === "CreateDoc") {
                            ObjectKeyword = me.projectKeyword;
                        } else {
                            ObjectKeyword = me.docKeyword;
                        }
                        //触发服务端BeforeUploadFile事件，如果是断点续传，获取已上传的文件大小
                        sendBeforeUploadFile(uploadbasic, file, ObjectKeyword, me);
                    });



                },


                fileuploaded: function (uploader, file) {
                    ////这里用common里面的事件会导致第一次上传时属性不正常显示
                    me.afterUploadFile(uploader, file, me.ServerFullFileName);

                },

                uploaderror: function (uploader, result) {
                    //文件上传失败事件
                    //var reObj = Ext.JSON.decode(result.response);
                    //Ext.Msg.alert("错误消息", reObj.msg);
                },

                uploadcomplete: function (uploader, success, failed) {
                    var gridPanel = Ext.getCmp(me.mainPanelId).up('_mainSourceView').down('_mainDocGrid');
                    //var store = me.maindocgrid.store;//刷新DOC列表
                    //store.load();
                    //刷新属性页
                    gridPanel.loadAttrPage("selectdoc");

                    me.fileNameText.setValue(success[0].name);
                    me.fileSizeText.setValue(success[0].size);

                    if (me.winAction === "CreateDoc") {
                        me.isUploadedFile = true;
                        me.lastUploadedFile = success[0];//记录成功上传的最后一个文件
                    }
                },
                scope: this
            }, width: 80


        });

        //添加列表
        me.items = [
          Ext.widget('form', {
              layout: "form",
              items: [{
                  xtype: "tabpanel",
                  layout: {
                      type: 'hbox',
                      pack: 'start',
                      align: 'stretch'
                  },
                  items: [
                        {
                            xtype: "panel",
                            title: "常规",
                            layout: "vbox",
                            width: '100%',
                            align: 'stretch',
                            pack: 'start',
                            margins: "5",
                            items: [
                                {
                                    xtype: "fieldset",
                                    title: "文档",
                                    layout: "vbox",
                                    width: '100%',
                                    align: 'stretch', //margin: '8 0 8 0', padding: '8 8 8 8',
                                    pack: 'start',
                                    items: [
                                           {
                                               layout: "hbox",
                                               width: '100%', margins: "15 8 15 8", baseCls: 'my-panel-no-border',//隐藏边框
                                               items: [
                                                   me.docCodeText//文档名Text
                                                   ]
                                           },
                                        {
                                            layout: "hbox",
                                            width: '100%', margins: "15 8 15 8", baseCls: 'my-panel-no-border',//隐藏边框
                                            items: [
                                                 me.docDescText]//文档描述Text
                                        }
                                    ], flex: 1
                                },
                                {
                                    xtype: "fieldset",
                                    title: "",
                                    layout: "vbox",
                                    width: '100%',
                                    align: 'stretch', //padding: '8 8 8 8', margin: '8 0 8 0',
                                    pack: 'start',
                                    items: [

                                        {
                                            xtype: "panel",
                                            layout: "hbox",
                                            width: '100%', baseCls: 'my-panel-no-border',//隐藏边框
                                            items: [
                                                me.tempDefnCodeText,//文件模板Text
                                                {
                                                    xtype: "button",
                                                    text: "改变", margins: "15 8 15 8", width: 60,
                                                    listeners: {
                                                        "click": function (btn, e, eOpts) {//添加点击按钮事件
                                                            me.get_doc_def();//获取模板
                                                        }
                                                    }
                                                }
                                            ]
                                        },
                                    {
                                        layout: "hbox",
                                        width: '100%', margins: "15 8 15 8", baseCls: 'my-panel-no-border',//隐藏边框
                                        items: [
                                             me.tempDefnDescText]//文件模板描述Text
                                    }
                                    ], flex: 1
                                },
                                {
                                    xtype: "fieldset",
                                    title: "文件",
                                    layout: "vbox",
                                    width: '100%', //margin: '0', padding:'8 8 8 8',
                                    align: 'stretch',
                                    pack: 'start',
                                    items: [
                                        {
                                            xtype: "panel",layout: "hbox",width: '100%', baseCls: 'my-panel-no-border',//隐藏边框
                                            items: [
                                                me.fileNameText
                                                ,
                                                me.FileUploadButton
                                                //{
                                                //    xtype: "button",
                                                //    text: "上传文件", margins: "15 8 15 8", width: 60,
                                                //    listeners: {
                                                //        "click": function (btn, e, eOpts) {//添加点击按钮事件
                                                //            me.replace_doc();//替换文档
                                                //        }
                                                //    }
                                                //}
                                            ]
                                        },
                                        {
                                            layout: "hbox",
                                            width: '100%', margins: "8 1 1 1", baseCls: 'my-panel-no-border',//隐藏边框
                                            items: [me.fileSizeText
                                                 ]
                                        }
                                    ], flex: 1
                                }
                            ]
                        }

                  ]
              },
              {
                  xtype: "panel",
                  layout: "hbox",
                  baseCls: 'my-panel-no-border',//隐藏边框
                  //align: 'right',
                  //pack: 'end',//组件在容器右边
                  items: [{
                      flex: 1, baseCls: 'my-panel-no-border'//隐藏边框
                  },
                      {
                          xtype: "button",
                          text: "确定", width: 60, margins: "18 5 18 5",
                          listeners: {
                              "click": function (btn, e, eOpts) {//添加点击按钮事件
                                  me.update_Doc_attr(true);
                              }
                          }
                      },
                      {
                          xtype: "button",
                          text: "取消", width: 60, margins: "18 5 18 5",
                          listeners: {
                              "click": function (btn, e, eOpts) {//添加点击按钮事件
                                  //Ext.Msg.alert("您展开了目录树节点！！！", "您展开了目录树节点！节点ID:" + me.tempDefnId);
                                  win.close();
                              }
                          }
                      },
                  {
                      xtype: "button",
                      text: "应用", width: 60, margins: "18 15 18 5",
                      listeners: {
                          "click": function (btn, e, eOpts) {//添加点击按钮事件
                              me.update_Doc_attr(false);
                          }
                      }
                  }
                  ]
              }]
          })];
        me.callParent(arguments);
    },

    //获取文档属性
    get_Doc_attr: function (ProjectKeyword, docKeyword) {
        var me = this;
        me.projectKeyword = ProjectKeyword;
        me.docKeyword = docKeyword;
        Ext.Ajax.request({
            
            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.WebApi.DocController", A: "GetDocAttr",
                action: me.winAction, ProjectKeyword: ProjectKeyword,DocKeyword:docKeyword, sid: localStorage.getItem("sid")
            },
            success: function (response, options) {
                try {
                    //获取数据后，更新窗口
                    var res = Ext.JSON.decode(response.responseText, true);
                    var state = res.success;
                    if (state === false) {
                        var errmsg = res.msg;
                        Ext.Msg.alert("错误信息", errmsg);
                    }
                    else {
                        var recod = eval(res.data[0]);

                        if (me.winAction === "ModiDoc") {
                            me.docCodeText.setValue(recod.doccode);
                            me.docDescText.setValue(recod.docdesc);

                            me.tempDefnCodeText.setValue(recod.defname);
                            me.tempDefnDescText.setValue(recod.defdesc);
                            me.tempDefnId = recod.defkeyid;

                            me.fileNameText.setValue(recod.filename);
                            me.fileSizeText.setValue(recod.filesize);
                        }
                    }
                } catch (e) { }
            }
        });
    },

    //获取文档模板
    get_doc_def: function () {
        var me = this;
        //me.viewTree = Ext.getCmp('_projectsTree');//获取目录树控件ID
        //var nodes = me.viewTree.getSelectionModel().getSelection();//获取已选取的节点
        me.viewGrid = Ext.getCmp(me.mainPanelId).up('_mainSourceView').down('_mainDocGrid').down('gridpanel');//Ext.getCmp('_DocGrid');
        var nodes = me.viewGrid.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {

            var _fmGetProjDef = Ext.create('Ext.plug_ins.SysPlugins._GetProjDef', { title: "" });
            //var _fmCreateNewProject = Ext.create('Ext.plug_ins.SysPlugins._CreateProjByDef', { title: "" });
            window.parent.resultvalue = "";
            window.parent.resultdesc = "";
            winGetDef = Ext.widget('window', {
                title: '模板选择',
                //closeAction: 'hide',
                width: 670,
                height: 560,
                minWidth: 670,
                minHeight: 560,
                maxWidth: 670,
                maxHeight: 560,
                layout: 'fit',
                resizable: true,//允许用户调整每个边缘和窗口的角落
                modal: true,//制作窗口模板并且掩饰他背后的一切
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: _fmGetProjDef,
                defaultFocus: 'firstName'
            });

            //监听子窗口关闭事件
            winGetDef.on('close', function () {
                if (window.parent.resultvalue != null && window.parent.resultvalue !== "") {
                    me.tempDefnId = window.parent.resultvalue;
                    var strArray = window.parent.resultdesc.split("__");
                    if (strArray.length > 1) {
                        me.tempDefnCodeText.setValue(strArray[0]);
                        me.tempDefnDescText.setValue(strArray[1]);
                    }
                }
            });
            winGetDef.show();

            _fmGetProjDef.winAction = me.winAction;
        }
    },

    //上传文档
    replace_doc: function () {
        var me = this;

    },
    //更新文档属性,参数closeWin:完成更新后是否关闭窗口
    update_Doc_attr: function (closeWin) {
        var me = this;
        var docCode = me.docCodeText.value;
        var docDesc = me.docDescText.value;
        var tempDefnId = me.tempDefnId;

        var docAttr = 
            [{ name: 'docCode', value: docCode },
                { name: 'docDesc', value: docDesc },
                { name: 'tempDefnId', value: tempDefnId }
        ];
        var docAttrJson = Ext.JSON.encode(docAttr);
        var action="UpdateDocAttr";
        if (me.winAction === "CreateDoc") {
            action = "CreateDoc";
        }

            Ext.Ajax.request({
                
                url: 'WebApi/Post',
                method: "POST",
                params: {
                    C: "AVEVA.CDMS.WebApi.DocController", A: action,
                    sid: localStorage.getItem("sid"), action: me.winAction, projectKeyword: me.projectKeyword, docKeyword: me.docKeyword,
                    docAttrJson: docAttrJson
                },
                success: function (response, options) {
                    me.update_Doc_attr_callback(action,response,me.projectKeyword, closeWin);
                }
            })
        //}
    },

    update_Doc_attr_callback: function (action,response,parentKeyword, closeWin) {
        var me = this;
        var res = Ext.JSON.decode(response.responseText, true);
        var state = res.success;
        if (state === false) {
            var errmsg = res.msg;
            Ext.Msg.alert("错误信息", errmsg);
        }
        else {
            var grid = Ext.getCmp(me.mainPanelId).up('_mainSourceView').down('_mainDocGrid').down("gridpanel");
            if (action === "CreateDoc")
            {
                var addGridStore = function (res) {
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
                };
                //如果是新建文件并且上传了文件，就更新Doc的文件属性
                if (me.isUploadedFile === true) {
                    var uploader = me.FileUploadButton.uploader;
                    //获取刚刚创建的docKeyword
                    uploader.multipart_params.DocKeyword = res.data[0].Keyword;
                    me.afterUploadFile(uploader, me.lastUploadedFile, me.ServerFullFileName);
                    grid.store.load();
                } else {
                    addGridStore(res);
                }


            } else {
                grid.store.load();
            }
            if (closeWin)
                win.close();
        }
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
                    //me.OnAfterCreateNewDocEvent(DocKeyword, file);
                }
            }
        });
    }
})