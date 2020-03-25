//发文处理
Ext.define('Ext.plug_ins.HXPC_Plugins.sendDocument', {
    extend: 'Ext.container.Container',
    alias: 'widget.sendDocument',
    layout: 'fit',
    resultvalue: '', mainPanelId: '',
    projectKeyword: '',
    initComponent: function () {
        var me = this;

        //以下参数为上传附件相关参数
        //目录树选取的节点关键字
        //me.projectKeyword = "";
        //新创建后的目录关键字
        me.newProjectKeyword = "";
        //上传附件服务器地址
        me.ServerFullFileName = "";
        //上传文档的doc关键字列表
        me.docList = "";
        //单号
        me.docName = "";
        //附件序号
        me.docUploadIndex = 0;
        //附件上传状态
        me.uploadCompleteState = false;
        //附件文件名的前缀
        me.docCode = "";

        //定义收文单位combo初始数据
        me.companydata = [];

        //定义是否回文combo初始数据
        me.ifdata = [{ text: "是", value: "是" }, { text: "否", value: "否" }];

        //定义发送版本combo初始数据
        me.languagedata = [{ text: "中文", value: "中文" }, { text: "英文", value: "英文" }];

        //定义是否保密combo初始数据
        me.mifdata = [{ text: "是", value: "是" }, { text: "否", value: "否" } ];

        //定义发放路径combo初始数据
        me.routedata = [{ text: "邮寄", value: "邮寄" }, { text: "当面递交", value: "当面递交" },
            { text: "邮件", value: "邮件" }, { text: "内部OA", value: "内部OA" },
            { text: "共享盘", value: "共享盘" }, { text: "业主共享平台", value: "业主共享平台" }
        ];

        //定义文件形式combo初始数据
        me.filedata = [{ text: "EN_电子可修改版", value: "EN_电子可修改版" }, { text: "P_打印件", value: "P_打印件" },
            { text: "O_原件", value: "O_原件" }, { text: "C_拷贝盘", value: "C_拷贝盘" },
            { text: "E_电子版(.pdf)", value: "E_电子版(.pdf)" }
        ];

        //定义提交目的combo初始数据
        me.sendObjdata = [{ text: "1_按需求提交", value: "1_按需求提交" }, { text: "2_审查", value: "2_审查" },
            { text: "3_告知", value: "3_告知" }, { text: "4_采购", value: "4_采购" },
            { text: "5_供应商", value: "5_供应商" }, { text: "6_交工资料", value: "6_交工资料" },
            { text: "7_批准", value: "7_批准" }, { text: "8_预制", value: "8_预制" },
            { text: "9_施工", value: "9_施工" }, { text: "10_提交", value: "10_提交" },
            { text: "11_其他", value: "11_其他" }
        ];

        //添加项目号text
        me.ProjectText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "", labelWidth: 0, readOnly: true, fieldStyle: 'background-color: #DFE9F6;border-color: #DFE9F6; background-image: none;',//fieldStyle:'color:red',
            anchor: "80%", labelAlign: "right", width: 150//flex: 1
        });

        //添加函件流水号text
        me.NumberText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: " - S ", labelWidth: 20,  labelSeparator: '', // 去掉laebl中的冒号
            anchor: "80%", labelAlign: "right", width: 150//flex: 1
        });

        //添加函件主题text
        me.DescText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "", labelWidth: 0, labelSeparator: '', // 去掉laebl中的冒号
            anchor: "80%", labelAlign: "right", flex: 1//width: 150//
        });

        //添加增加的意见text
        me.NoteText = Ext.create("Ext.form.field.TextArea", {
            xtype: "textarea", anchor: "80%", labelAlign: "left", margin: '8 0 0 0', labelSeparator: '', // 去掉laebl中的冒号
            height: 100, fieldLabel: "增加的意见", labelAlign: "top", labelWidth: 80, flex: 1
        });

        //定义任务下达时间Text
        me.replyDateField = Ext.create("Ext.form.field.Date", {

            name: "date",
            fieldLabel: '回复时间', fieldStyle: ' background-image: none;',
            editable: true, labelWidth: 55, margin: '0 10 0 0',
            emptyText: "--请选择--", autoLoadOnValue: true,
            format: 'Y年m月d日',
            value: new Date(),
            width: 175

        });

        //添加收文单位combo
        Ext.define("companyModel", {
            extend: 'Ext.data.Model',
            fields: ["text", "value"]
        });
        me.companyProxy = Ext.create("Ext.data.proxy.Memory", {
            data: me.companydata,
            model: "companyModel"
        });

        me.companyStore = Ext.create("Ext.data.Store", {
            model: companyModel,
            proxy: me.companyProxy
        });


        me.companyCombo = Ext.create("Ext.form.field.ComboBox",
        {
            //xtype: "combo",
            fieldLabel: ' - ', labelWidth: 10, labelSeparator: '', // 去掉laebl中的冒号
            triggerAction: "all", store: me.companyStore,
            valueField: 'value', editable: false,//不可输入
            displayField: 'text', margin: '0 0 0 10',// 
            anchor: "80%", labelAlign: "left", width: 150,//
            emptyText: "--请选择--", autoLoadOnValue: true,
            //fieldStyle: 'border-color: red; background-image: none;',//红色边框
            listeners:
            {
                select: function (combo, records, eOpts) {
                }
            }
        });

        //添加是否回文combo
        Ext.define("ifModel", {
            extend: 'Ext.data.Model',
            fields: ["text", "value"]
        });
        me.ifProxy = Ext.create("Ext.data.proxy.Memory", {
            data: me.ifdata,
            model: "ifModel"
        });

        me.ifStore = Ext.create("Ext.data.Store", {
            model: ifModel,
            proxy: me.ifProxy
        });


        me.ifCombo = Ext.create("Ext.form.field.ComboBox",
        {
            //xtype: "combo",
            fieldLabel: '是否回文', labelWidth: 55, 
            triggerAction: "all", store: me.ifStore,
            valueField: 'value', editable: false,//不可输入
            displayField: 'text', margin: '0 10 0 0',// 
            anchor: "80%", labelAlign: "left", width: 150,//
            emptyText: "--请选择--", autoLoadOnValue: true,
            //fieldStyle: 'border-color: red; background-image: none;',//红色边框
            listeners:
            {
                select: function (combo, records, eOpts) {
                }
            }
        });

        //添加发送版本combo
        Ext.define("languageModel", {
            extend: 'Ext.data.Model',
            fields: ["text", "value"]
        });
        me.languageProxy = Ext.create("Ext.data.proxy.Memory", {
            data: me.languagedata,
            model: "languageModel"
        });

        me.languageStore = Ext.create("Ext.data.Store", {
            model: languageModel,
            proxy: me.languageProxy
        });


        me.languageCombo = Ext.create("Ext.form.field.ComboBox",
        {
            //xtype: "combo",
            fieldLabel: '发送版本', labelWidth: 55,
            triggerAction: "all", store: me.languageStore,
            valueField: 'value', editable: false,//不可输入
            displayField: 'text', margin: '0 10 0 0',// 
            anchor: "80%", labelAlign: "left", width: 140,//
            emptyText: "--请选择--", autoLoadOnValue: true,
            //fieldStyle: 'border-color: red; background-image: none;',//红色边框
            listeners:
            {
                select: function (combo, records, eOpts) {
                }
            }
        });

        //添加是否保密combo
        Ext.define("mifModel", {
            extend: 'Ext.data.Model',
            fields: ["text", "value"]
        });
        me.mifProxy = Ext.create("Ext.data.proxy.Memory", {
            data: me.mifdata,
            model: "mifModel"
        });

        me.mifStore = Ext.create("Ext.data.Store", {
            model: mifModel,
            proxy: me.mifProxy
        });


        me.mifCombo = Ext.create("Ext.form.field.ComboBox",
        {
            //xtype: "combo",
            fieldLabel: '是否保密', labelWidth: 55, 
            triggerAction: "all", store: me.mifStore,
            valueField: 'value', editable: false,//不可输入
            displayField: 'text', margin: '0 10 0 0',// 
            anchor: "80%", labelAlign: "left", width: 150,//
            emptyText: "--请选择--", autoLoadOnValue: true,
            //fieldStyle: 'border-color: red; background-image: none;',//红色边框
            listeners:
            {
                select: function (combo, records, eOpts) {
                }
            }
        });


        //添加发放路径combo
        Ext.define("routeModel", {
            extend: 'Ext.data.Model',
            fields: ["text", "value"]
        });
        me.routeProxy = Ext.create("Ext.data.proxy.Memory", {
            data: me.routedata,
            model: "routeModel"
        });

        me.routeStore = Ext.create("Ext.data.Store", {
            model: routeModel,
            proxy: me.routeProxy
        });


        me.routeCombo = Ext.create("Ext.form.field.ComboBox",
        {
            //xtype: "combo",
            fieldLabel: '发放路径', labelWidth: 55,
            triggerAction: "all", store: me.routeStore,
            valueField: 'value', editable: false,//不可输入
            displayField: 'text', margin: '0 10 0 0',// 
            anchor: "80%", labelAlign: "left", width: 175,//
            emptyText: "--请选择--", autoLoadOnValue: true,
            //fieldStyle: 'border-color: red; background-image: none;',//红色边框
            listeners:
            {
                select: function (combo, records, eOpts) {
                }
            }
        });

        //添加文件形式combo
        Ext.define("fileModel", {
            extend: 'Ext.data.Model',
            fields: ["text", "value"]
        });
        me.fileProxy = Ext.create("Ext.data.proxy.Memory", {
            data: me.filedata,
            model: "fileModel"
        });

        me.fileStore = Ext.create("Ext.data.Store", {
            model: fileModel,
            proxy: me.fileProxy
        });


        me.fileCombo = Ext.create("Ext.form.field.ComboBox",
        {
            //xtype: "combo",
            fieldLabel: '文件形式', labelWidth: 55, 
            triggerAction: "all", store: me.fileStore,
            valueField: 'value', editable: false,//不可输入
            displayField: 'text', margin: '0 10 0 0',// 
            anchor: "80%", labelAlign: "left", width: 150,//
            emptyText: "--请选择--", autoLoadOnValue: true,
            //fieldStyle: 'border-color: red; background-image: none;',//红色边框
            listeners:
            {
                select: function (combo, records, eOpts) {
                }
            }
        });


        //添加提交目的combo
        Ext.define("sendObjModel", {
            extend: 'Ext.data.Model',
            fields: ["text", "value"]
        });
        me.sendObjProxy = Ext.create("Ext.data.proxy.Memory", {
            data: me.sendObjdata,
            model: "sendObjModel"
        });

        me.sendObjStore = Ext.create("Ext.data.Store", {
            model: sendObjModel,
            proxy: me.sendObjProxy
        });


        me.sendObjCombo = Ext.create("Ext.form.field.ComboBox",
        {
            //xtype: "combo",
            fieldLabel: '提交目的', labelWidth: 55, 
            triggerAction: "all", store: me.sendObjStore,
            valueField: 'value', editable: false,//不可输入
            displayField: 'text', margin: '0 10 0 0',// 
            anchor: "80%", labelAlign: "left", width: 175,//
            emptyText: "--请选择--", autoLoadOnValue: true,
            //fieldStyle: 'border-color: red; background-image: none;',//红色边框
            listeners:
            {
                select: function (combo, records, eOpts) {
                }
            }
        });

        //定义选择附件按钮
        me.FileUploadButton = Ext.create('Ext.ux.upload.Button', {
            renderTo: Ext.getBody(),
            text: '添加附件',
            //plugins: [{
            //    ptype: 'ux.upload.window',
            //    //ptype: Ext.ux.upload.plugin.Window,
            //    title: '上传文件',
            //    width: 520,
            //    height: 350
            //}
            //], //margin: '8 0 8 0',
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
                    uploader.multipart_params.ProjectKeyword = "temp";// me.grid.store.proxy.extraParams.ProjectKeyWord;
                    uploader.multipart_params.sid = localStorage.getItem("sid");

                    for (var i = 0; i < files.length ; i++) {
                        //插入行到文件grid
                        var r = Ext.create('filemodel', {
                            name: files[i].name
                        });

                        var rowlength = me.filegrid.getStore().data.length;
                        me.filegrid.getStore().insert(rowlength, r);
                    }
                    return true;
                },

                beforeupload: function (uploadbasic, uploader, file) {
                    //console.log('beforeupload');			
                    //上传文件而不是替换文件时，这里要把Doc关键字重置
                    uploadbasic.multipart_params.DocKeyword = "";

                    var extIndex = file.name.lastIndexOf(".");
                    var fileCode = file.name;
                    if (extIndex >= 0) {
                        fileCode = fileCode.substring(0, extIndex);
                    }

                    //规范附件名称
                    me.docUploadIndex = me.docUploadIndex + 1;
                    fileCode = me.docCode + "_附件" + me.docUploadIndex.toString() + " " + fileCode;

                    Ext.require('Ext.ux.Common.comm', function () {
                        //先创建文档
                        createDoc(uploadbasic, file, me.newProjectKeyword, fileCode, "", "", function (uploadbasic, res, options, DocKeyword) {
                            //var state = res.success;
                            if (res.success === true) {
                                //处理创建文档后的返回事件
                                me.docList = me.docList + "," + DocKeyword;

                                //触发服务端BeforeUploadFile事件，如果是断点续传，获取已上传的文件大小
                                sendBeforeUploadFile(uploadbasic, options.file, DocKeyword, me);
                            }
                        });
                    });
                },

                fileuploaded: function (uploader, file) {
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
            }, width: 70


        });

        //定义已上传附件的model
        Ext.define("filemodel", {
            extend: "Ext.data.Model",
            fields: ["name"],
            url: "_blank",
        });

        //定义已上传附件的store
        me.filestore = Ext.create("Ext.data.Store", {
            model: "filemodel"
        });
        //定义已上传附件的view
        me.filegrid = Ext.widget("grid", {
            region: "center",
            height: 68,
            hideHeaders: true,//隐藏表头
            flex: 1,
            store: me.filestore,
            columns: [
                { text: '', dataIndex: 'name', width: '100%' }
            ]
        });

        //添加列表
        me.items = [
          Ext.widget('form', {
              layout: "form",
              items: [

                  {
                      xtype: "panel",
                      baseCls: 'my-panel-no-border',//隐藏边框
                      layout: {
                          type: 'vbox',
                          pack: 'start',
                          align: 'stretch'
                      },
                      items: [
                          {
                              layout: "hbox",
                              width: '100%', baseCls: 'my-panel-no-border',//隐藏边框
                              items: [{ baseCls: 'my-panel-no-border', flex: 1 },
                              {
                                  xtype: 'label',
                                  cls: 'classDiv2',
                                  itemId: 'label1',
                                  text: '发 文 处 理 表 单', margins: '0 0 0 10'
                              }, { baseCls: 'my-panel-no-border', flex: 1 }]
                          },
                          {
                              xtype: "fieldset", margin: '8 16 8 16',
                              baseCls: 'my-panel-no-border',//隐藏边框
                              layout: {
                                  type: 'vbox',
                                  pack: 'start',
                                  align: 'stretch'
                              },
                              items: [
                                  {
                                      xtype: "fieldset", margin: '8 0 0 0',
                                      title:"函件编码",
                                      layout: {
                                          type: 'vbox',
                                          pack: 'start',
                                          align: 'stretch'
                                      },
                                      items: [
                                          {
                                              baseCls: 'my-panel-no-border',//隐藏边框
                                              layout: {
                                                  type: 'hbox',
                                                  pack: 'start',
                                                  align: 'stretch'
                                              },
                                              items: [
                                                  me.ProjectText,
                                                  me.companyCombo,
                                                  me.NumberText
                                              ],
                                              flex: 1
                                          },
                                          {
                                              baseCls: 'my-panel-no-border',//隐藏边框
                                              layout: {
                                                  type: 'hbox',
                                                  pack: 'start',
                                                  align: 'stretch'
                                              },
                                              margin: '0 0 0 0',
                                              items: [
                                                   {
                                                       xtype: "label",
                                                       text: "项目号",
                                                       margin: '3 55 0 55'
                                                   },
                                                   {
                                                       xtype: "label",
                                                       text: "收文单位",
                                                       margin: '3 45 0 75'
                                                   },
                                                   {
                                                       xtype: "label",
                                                       text: "函件流水号",
                                                       margin: '3 55 0 55'
                                                   },
                                              ],
                                              flex: 1
                                          }
                                      ],
                                      flex: 1
                                  },
                                  {
                                      xtype: "fieldset", margin: '8 0 0 0',
                                      title: "函件主题",
                                      layout: {
                                          type: 'hbox',
                                          pack: 'start',
                                          align: 'stretch'
                                      },
                                      items: [
                                            me.DescText
                                      ],
                                      flex: 1
                                  },
                                  {
                                      xtype: "fieldset", margin: '8 0 0 0',
                                      title: "函件正文",
                                      layout: {
                                          type: 'vbox',
                                          pack: 'start',
                                          align: 'stretch'
                                      },
                                      items: [
                                          {
                                              layout: {
                                                  type: 'hbox',
                                                  pack: 'start',
                                                  align: 'stretch'
                                              },
                                              margin: '0 0 0 0', baseCls: 'my-panel-no-border',//隐藏边框
                                              items: [
                                                  me.ifCombo, me.replyDateField, me.languageCombo
                                              ]
                                          }, {
                                              layout: {
                                                  type: 'hbox',
                                                  pack: 'start',
                                                  align: 'stretch'
                                              },
                                              margin: '8 0 0 0', baseCls: 'my-panel-no-border',//隐藏边框
                                              items: [
                                                  me.mifCombo, me.routeCombo
                                              ]
                                          }, {
                                              layout: {
                                                  type: 'hbox',
                                                  pack: 'start',
                                                  align: 'stretch'
                                              },
                                              margin: '8 0 0 0', baseCls: 'my-panel-no-border',//隐藏边框
                                              items: [
                                                  me.fileCombo, me.sendObjCombo
                                              ]
                                          },
                                          me.NoteText
                                          //me.ifCombo, me.languageCombo
                                      ],
                                      flex: 1
                                  },
                                  {
                                      xtype: "fieldset", margin: '8 0 0 0',
                                      title: "函件附件",
                                      layout: {
                                          type: 'vbox',
                                          pack: 'start',
                                          align: 'stretch'
                                      },
                                      items: [
                                          me.filegrid,
                                          {
                                              layout: {
                                                  type: 'hbox',
                                                  pack: 'start',
                                                  align: 'stretch'
                                              },
                                              baseCls: 'my-panel-no-border',//隐藏边框
                                              xtype: "fieldset", margin: '3 0 0 0',
                                              items: [
                                          me.FileUploadButton
                                              ]
                                          }
                                      ],
                                      flex: 1
                                  },
                              ],
                              flex: 1
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
                              text: "确定", width: 60, margins: "0 5 10 5",
                              listeners: {
                                  "click": function (btn, e, eOpts) {//添加点击按钮事件
                                      me.send_sendDocument();

                                  }
                              }
                          },
                          {
                              xtype: "button",
                              text: "取消", width: 60, margins: "0 15 10 5",
                              listeners: {
                                  "click": function (btn, e, eOpts) {//添加点击按钮事件
                                      //Ext.Msg.alert("您展开了目录树节点！！！", "您展开了目录树节点！节点ID:" + me.tempDefnId);
                                      winSendDocument.close();
                                  }
                              }
                          }
                      ]
                  }
                      ]
                  }
              ]
          })];

        //获取打开表单时的默认参数
        me.sendGetSendDocumentDefault();


        me.callParent(arguments);
    },

    //获取发文处理表单默认参数
    sendGetSendDocumentDefault: function(){
        var me = this;

        me.ifCombo.setRawValue(me.ifdata[0].text);//设置显示值
        me.ifCombo.setValue(me.ifdata[0].value); //设置ID值

        me.languageCombo.setRawValue(me.languagedata[0].text);//设置显示值
        me.languageCombo.setValue(me.languagedata[0].value); //设置ID值

        me.mifCombo.setRawValue(me.mifdata[0].text);//设置显示值
        me.mifCombo.setValue(me.mifdata[0].value); //设置ID值

        me.routeCombo.setRawValue(me.routedata[0].text);//设置显示值
        me.routeCombo.setValue(me.routedata[0].value); //设置ID值

        me.fileCombo.setRawValue(me.filedata[0].text);//设置显示值
        me.fileCombo.setValue(me.filedata[0].value); //设置ID值

        me.sendObjCombo.setRawValue(me.sendObjdata[0].text);//设置显示值
        me.sendObjCombo.setValue(me.sendObjdata[0].value); //设置ID值

        //通过extjs的ajax获取操作全部名称
        Ext.Ajax.request({
            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.HXPC_Plugins.Document", A: "GetSendDocumentDefault",
                sid: localStorage.getItem("sid"), ProjectKeyword: me.projectKeyword
            },
            success: function (response, options) {
                me.sendGetSendDocumentDefault_callback(response, options);//, funCallback);

            }
        });
    },

    //处理获取发文处理表单默认参数的返回
    sendGetSendDocumentDefault_callback: function (response, options) {
        var me = this;

        //获取数据后，更新窗口
        var res = Ext.JSON.decode(response.responseText, true);
        var state = res.success;
        if (state === true) {
            var recod = eval(res.data[0]);

            var strRootProjectCode = recod.RootProjectCode;
            var strDocNumber = recod.DocNumber;
            var CompanyList = eval(recod.CompanyList);

            me.ProjectText.setValue(strRootProjectCode);
            me.NumberText.setValue(strDocNumber);

            var objLength = 0;
            //遍历来往单位数组，添加到来往单位combo
            for (var itemKey in CompanyList) {
                var strCompany = CompanyList[itemKey];
                me.companydata.push({ text: strCompany, value: strCompany });//在数组里添加新的元素  

                objLength = objLength + 1;
                //if (CompanyList[itemKey].Pression != undefined) {
                //}
            }

            if (objLength > 0) {
                me.companyCombo.setRawValue(me.companydata[0].text);//设置显示值
                me.companyCombo.setValue(me.companydata[0].value); //设置ID值
            }
        }
    },

    //响应发文处理表单的确定按钮
    send_sendDocument: function () {
        var me = this;

        //获取项目号Text
        var strRootProject = me.ProjectText.value;

        //获取收文单位
        var strCompany = me.companyCombo.value;

        //获取函件流水号Text
        var strNumber = me.NumberText.value;

        //获取函件主题Text
        var strDesc = me.DescText.value;

        //获取是否回文
        var strIf = me.ifCombo.value;

        //获取任务下达时间
        var replyDate = me.replyDateField.value;

        //获取发送版本
        var strLanguage = me.languageCombo.value;

        //获取是否保密
        var strMif = me.mifCombo.value;

        //获取发放路径
        var strRoute = me.routeCombo.value;

        //获取文件形式
        var strFile = me.fileCombo.value;

        //获取提交目的
        var strSendObj = me.sendObjCombo.value;

        //获取增加的意见Text
        var strNote = me.NoteText.value;

        //获取表单数据，转换成JSON字符串
        var docAttr =
        [
            { name: 'RootProject', value: strRootProject },
            { name: 'Company', value: strCompany },
            { name: 'Number', value: strNumber },
            { name: 'Title', value: strDesc },
            { name: 'If', value: strIf },
            { name: 'ReplyDate', value: replyDate },
            { name: 'Language', value: strLanguage },
            { name: 'Mif', value: strMif },
            { name: 'Route', value: strRoute },
            { name: 'File', value: strFile },
            { name: 'SendObj', value: strSendObj },
            { name: 'Note', value: strNote }
        ]

        var docAttrJson = Ext.JSON.encode(docAttr);

        Ext.MessageBox.wait("正在创建外部文件传输单，请稍候...", "等待");

        Ext.Ajax.request({

            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.HXPC_Plugins.Document", A: "SendDocument",
                sid: localStorage.getItem("sid"), ProjectKeyword: me.projectKeyword,
                docAttrJson: docAttrJson
            },
            success: function (response, options) {


                //获取数据后，更新窗口
                var res = Ext.JSON.decode(response.responseText, true);
                var state = res.success;
                if (state === true) {

                    Ext.MessageBox.close();//关闭等待对话框

                    var recod = eval(res.data[0]);

                    me.docKeyword = recod.DocKeyword;//获取联系单文档id
                    me.docList = recod.DocList;//获取流程文档列表
                    me.newProjectKeyword = recod.ProjectKeyword;//获取新建的目录id
                    //获取附件文件名的前缀
                    me.docCode = recod.DocCode;

                    if (me.FileUploadButton.uploader.uploader.files.length > 0) {
                        ////当有附件时，创建DOC文档成功后，上传附件
                        me.FileUploadButton.uploader.start();

                        Ext.MessageBox.wait("正在上传附件，请稍候...", "等待");

                        var int = window.setInterval(function () {
                            //上传附件完毕
                            if (me.uploadCompleteState === true) {
                                Ext.MessageBox.close();//关闭等待对话框
                                //处理返回事件
                                me.send_sendDocument_callback(response, options, "");//, me.projectKeyword, closeWin);
                                //me.send_create_doc_callback(projectKeyword, docKeyword, me.docList, true);
                                //停止线程
                                window.clearInterval(int);
                            }
                        }, 500);
                    } else {
                        //当没有附件时，处理返回事件
                        me.send_sendDocument_callback(response, options, "");//, me.projectKeyword, closeWin);
                        //me.send_create_doc_callback(projectKeyword, docKeyword, me.docList, true);
                    }
                } else {
                    var errmsg = res.msg;
                    Ext.Msg.alert("错误信息", errmsg);
                }
            }

        })
    },

    //处理创建发文单的返回
    send_sendDocument_callback :function (response, options){
        var me = this;

        Ext.MessageBox.wait("正在启动流程，请稍候...", "等待");

        Ext.Ajax.request({

            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.HXPC_Plugins.Document", A: "DocumentStartWorkFlow",
                sid: localStorage.getItem("sid"), docKeyword: me.docKeyword,
                docList: me.docList
            },
            success: function (response, options) {

                //获取数据后，更新窗口
                var res = Ext.JSON.decode(response.responseText, true);
                var state = res.success;
                if (state === true) {

                    Ext.MessageBox.close();//关闭等待对话框

                    var recod = eval(res.data[0]);
                    me.refreshWin(me.newProjectKeyword, true);
                } else {
                    var errmsg = res.msg;
                    Ext.Msg.alert("错误信息", errmsg);
                    winSendDocument.close();
                }
            }

        })
    },

    //刷新表单，参数:parentKeyword:新建的联系单目录
    refreshWin: function (projKeyword, closeWin) {
        var me = this;
        var tree = Ext.getCmp(me.mainPanelId).up('_mainSourceView').down('_mainProjectTree').down('treepanel');//;
        var viewTreeStore = tree.store;

        viewTreeStore.load({
            callback: function (records, options, success) {//添加回调，获取子目录的文件数量
                if (closeWin)
                    winSendDocument.close();

                //展开目录
                Ext.require('Ext.ux.Common.comm', function () {
                    Ext.getCmp('contentPanel').down('_mainProjectTree').ExpendProject(projKeyword);
                });
            }
        });
    }
});