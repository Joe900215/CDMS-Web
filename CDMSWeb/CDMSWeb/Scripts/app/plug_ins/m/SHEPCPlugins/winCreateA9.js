//新建目录
Ext.define('Ext.plug_ins.SHEPCPlugins.winCreateA9', {
    extend: 'Ext.container.Container',
    alias: 'widget.winCreateA9',
    //layout: "border",
    layout: 'fit',
    resultvalue: '',mainPanelId:'',
    initComponent: function () {
        var me = this;
        //目录树选取的节点关键字
        me.projectKeyword = "";
        me.winAction = "CreateA9";

        //新创建后的目录关键字
        me.newProjectKeyword = "";
        //上传附件服务器地址
        me.ServerFullFileName = "";
        //上传文档的doc关键字列表
        me.docList = "";
        //单号
        me.docNum = "";
        //附件序号
        me.docUploadIndex = 0;
        //附件上传状态
        me.uploadCompleteState = false;

        me.unitdata = [{ text: "00", value: "00" }, { text: "01", value: "01" }, { text: "02", value: "02" }];//机组combo
        //发文单位combo
        me.dispatch = [];
        me.professiondata = [{ text: "DQ", value: "DQ" }, { text: "GD", value: "GD" }, { text: "GL", value: "GL" },
            { text: "HJ", value: "HJ" }, { text: "HS", value: "HS" }, { text: "PZ", value: "PZ" },
            { text: "QJ", value: "QJ" }, { text: "RK", value: "RK" }, { text: "TJ", value: "TJ" }, { text: "TS", value: "TS" },
            { text: "AQ", value: "AQ" }, { text: "ZL", value: "ZL" }, { text: "GC", value: "GC" }, { text: "YB", value: "YB" }
        ];//专业combo


        me.projectCodeText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "发文编号", anchor: "80%", labelWidth: 60, labelAlign: "left", width: 160,//flex: 1
            value: "FA06241", readOnly: true
        });

        //添加发文类别text
        me.postCategoryText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", hideLabel: true, value: "A.9", readOnly: true,
            anchor: "80%", labelAlign: "left", width: 100//flex: 1
        });

        //添加发文单位代码text
        me.dispatchText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", hideLabel: true, readOnly: true,
            anchor: "80%", labelAlign: "left", width: 100//flex: 1
        });

        //添加流水单号text
        me.docNumText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", hideLabel: true, value: "",
            anchor: "80%", labelAlign: "left", width: 100//flex: 1
        });

        //添加事由text
        me.titleText = Ext.create("Ext.form.Label", {
            xtype: "label", width: 360, margin: '10 0 10 0',
            text: " 现报上拟用于本工程的主要施工机械/工器具/安全用具清单及其检验资料，请查验。工程进行中如有调整，将重新统计并上报。"//, anchor: "80%", labelWidth: 220, labelAlign: "left", margin: '10 0 0 0'//, width: 360//flex: 1
        });

        me.cellEditing = new Ext.grid.plugin.CellEditing({
            clicksToEdit: 1
        });

        //定义器具的model
        Ext.define("contentModel", {
            extend: "Ext.data.Model",
            fields: ["id", "name", "number", "checkNum", "company",
                { name: 'checkDate', mapping: 'availability', type: 'date', dateFormat: 'Y年m月d日' }],
            url: "_blank",
        });

        me.contentStore = Ext.create('Ext.data.Store', {
            autoDestroy: true,
            model: contentModel,
            sorters: [{
                property: 'id',
                direction: 'ASC'
            }]
        });

        me.contentText = Ext.create('Ext.grid.Panel', {
            title: '工程的主要施工机械/工器具/安全用具清单及其检验资料',
            store: me.contentStore,
            columns: [
                {
                    header: '器具名称', dataIndex: 'name', width: 141,
                    editor: {
                        allowBlank: false
                    }
                },
                {
                    header: '编 号', dataIndex: 'number', width: 141,
                    editor: {
                        allowBlank: false
                    }
                },
                {
                    header: '检验证编号', dataIndex: 'checkNum', width: 141,
                    editor: {
                        allowBlank: false
                    }
                },
                {
                    header: '检验单位', dataIndex: 'company', width: 141,
                    editor: {
                        allowBlank: false
                    }
                },
                {
                    header: '检定日期/有效期', dataIndex: 'checkDate', width: 141,
                    editor: {
                        xtype: 'datefield',
                        format: 'Y年m月d日',
                        value: new Date()
                    }, renderer: Ext.util.Format.dateRenderer('Y年m月d日')
                },
            ],
            stripeRows: true, //斑马线效果  
            plugins: [
                 Ext.create('Ext.grid.plugin.CellEditing', {
                     clicksToEdit: 1 //设置单击单元格编辑  
                 })],
            height: 160,
            width: 400,
            renderTo: Ext.getBody(),
            selModel: {
                selType: 'cellmodel'
            }
        });

        //器具表格添加行
        for (var i = 0; i < 8; i++) {
            var rec = new contentModel({
                name: ""
            });
            me.contentStore.insert(0, rec);
        }

        //Ext.create('Ext.data.Store', {
        //    storeId: 'simpsonsStore',
        //    fields: ['器具名称', '编 号', '检验证编号', '检验单位', '检定日期/有效期'],
        //    data: {
        //        'items': [
        //            { '器具名称': '1', "编 号": " ", "检验证编号": " ", "检验单位": " ", "检定日期/有效期": " " },
        //            { '器具名称': '1', "编 号": " ", "检验证编号": " ", "检验单位": " ", "检定日期/有效期": " " },
        //            { '器具名称': '1', "编 号": " ", "检验证编号": " ", "检验单位": " ", "检定日期/有效期": " " },
        //            { '器具名称': '1', "编 号": " ", "检验证编号": " ", "检验单位": " ", "检定日期/有效期": " " },
        //            { '器具名称': '1', "编 号": " ", "检验证编号": " ", "检验单位": " ", "检定日期/有效期": " " },
        //            { '器具名称': '1', "编 号": " ", "检验证编号": " ", "检验单位": " ", "检定日期/有效期": " " },
        //            { '器具名称': '1', "编 号": " ", "检验证编号": " ", "检验单位": " ", "检定日期/有效期": " " },
        //            { '器具名称': '1', "编 号": " ", "检验证编号": " ", "检验单位": " ", "检定日期/有效期": " " },
        //            { '器具名称': '1', "编 号": " ", "检验证编号": " ", "检验单位": " ", "检定日期/有效期": " " }
        //        ]
        //    },
        //    proxy: {
        //        type: 'memory',
        //        reader: {
        //            type: 'json',
        //            root: 'items'
        //        }
        //    }
        //});
        //me.contentText = Ext.create('Ext.grid.Panel', {
        //    title: '工程的主要施工机械/工器具/安全用具清单及其检验资料',
        //    store: Ext.data.StoreManager.lookup('simpsonsStore'),
        //    columns: [
        //        { header: '器具名称', dataIndex: '器具名称', width: 141 },
        //        { header: '编 号', dataIndex: '编 号', width: 141 },
        //        { header: '检验证编号', dataIndex: '检验证编号', width: 141 },
        //        { header: '检验单位', dataIndex: '检验单位', width: 141 },
        //        { header: '检定日期/有效期', dataIndex: '检定日期/有效期', width: 141 },
        //    ],
        //    selType: 'cellmodel',
        //    plugins: [
        //         Ext.create('Ext.grid.plugin.CellEditing', {
        //             clicksToEdit: 1 //设置单击单元格编辑  
        //         })],
        //    height: 160,
        //    width: 400,
        //    renderTo: Ext.getBody()
        //});
        //添加机组combo
        Ext.define("unitModel", {
            extend: 'Ext.data.Model',
            fields: ["text", "value"]
        });
        me.unitProxy = Ext.create("Ext.data.proxy.Memory", {
            data: me.unitdata,
            model: "unitModel"
        });

        me.unitStore = Ext.create("Ext.data.Store", {
            model: unitModel,
            proxy: me.unitProxy
        });

        me.unitCombo = Ext.create("Ext.form.field.ComboBox",
        {
            xtype: "combo",
            triggerAction: "all", store: me.unitStore,
            valueField: 'value', editable: false,//不可输入
            displayField: 'text', hideLabel: true,
            emptyText: "请选择", autoLoadOnValue: true,anchor: "80%", labelAlign: "left", width: 100,//, margins: "8"
            listeners:
            {
                select: function (combo, records, eOpts) {
                    //获取流水号
                    me.getRunNum();
                }
            }
        });
        //专工 经手人
        me.checkText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", readOnly: true,
            fieldLabel: "专工", anchor: "80%", labelWidth: 55, labelAlign: "left", margin: '0 0 0 0', width: 260//flex: 1
        });
        me.approvText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", readOnly: true,
            fieldLabel: "经手人", anchor: "80%", labelWidth: 55, labelAlign: "left", margin: '0 0 0 86', width: 260//flex: 1
        });

        ////添加发文单位combo
        //Ext.define("dispatchModel", {
        //    extend: 'Ext.data.Model',
        //    fields: ["text", "value"]
        //});
        //me.dispatchProxy = Ext.create("Ext.data.proxy.Memory", {
        //    data: me.dispatch,
        //    model: "dispatchModel"
        //});

        //me.dispatchStore = Ext.create("Ext.data.Store", {
        //    model: dispatchModel,
        //    proxy: me.dispatchProxy
        //});

        //me.dispatchCombo = Ext.create("Ext.form.field.ComboBox",
        //{
        //    xtype: "combo",
        //    triggerAction: "all", store: me.dispatchStore,
        //    valueField: 'value', editable: false,//不可输入
        //    displayField: 'text', hideLabel: true,
        //    anchor: "80%", labelAlign: "left", width: 80//, margins: "8"
        //});

        //添加专业combo
        Ext.define("professionModel", {
            extend: 'Ext.data.Model',
            fields: ["text", "value"]
        });
        me.professionProxy = Ext.create("Ext.data.proxy.Memory", {
            data: me.professiondata,
            model: "professionModel"
        });

        me.professionStore = Ext.create("Ext.data.Store", {
            model: professionModel,
            proxy: me.professionProxy
        });

        me.professionCombo = Ext.create("Ext.form.field.ComboBox",
        {
            xtype: "combo",
            triggerAction: "all", store: me.professionStore,
            valueField: 'value', editable: false,//不可输入
            displayField: 'text', hideLabel: true,
            emptyText: "请选择", autoLoadOnValue: true,anchor: "80%", labelAlign: "left", width: 96,//, margins: "8"
            listeners:
{
    select: function (combo, records, eOpts) {
        //获取流水号
        me.getRunNum();
    }
}
        });

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
                    fileCode = me.docNum + " 附件" + me.docUploadIndex.toString() + " " + fileCode;

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
                    //me.afterUploadFile(uploader, file, me.ServerFullFileName);

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
            height: 115,
            hideHeaders: true,//隐藏表头
            flex: 1,
            store: me.filestore,
            columns: [
                { text: '', dataIndex: 'name', width: '100%' }
            ]
        });
        //============================================================
        //添加列表
        me.items = [
          Ext.widget('form', {
              layout: "form",
              items: [{
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
                          text: 'A.9主要施工机械/工器具/安全用具报审表', margins: '0 0 0 10'
                      }, { baseCls: 'my-panel-no-border', flex: 1 }]
                      }
                  ,
                  {//发文编号一栏
                      xtype: "fieldset", margin: '8',
                      layout: {
                          type: 'vbox',
                          pack: 'start',
                          align: 'stretch'
                      },
                      items: [
                      {
                          layout: "hbox",
                          width: '100%', baseCls: 'my-panel-no-border',//隐藏边框
                          align: 'stretch', margin: '8 0 8 0', padding: '0 0 0 0',
                          pack: 'start',
                          items: [me.projectCodeText,
                            {
                                xtype: "label",
                                text: "-", margins: "0 5 0 5"
                            }, me.unitCombo,
                            {
                                xtype: "label",
                                text: "-", margins: "0 5 0 5"
                            }, me.dispatchText,//me.dispatchCombo,
                            {
                                xtype: "label",
                                text: "-", margins: "0 5 0 5"
                            }, me.professionCombo,
                            {
                                xtype: "label",
                                text: "-", margins: "0 5 0 5"
                            }, me.postCategoryText,
                            {
                                xtype: "label",
                                text: "-", margins: "0 5 0 5"
                            }, me.docNumText
                          ]
                      }, me.titleText, me.contentText,
                  {
                      layout: "vbox",
                      width: '100%', baseCls: 'my-panel-no-border',//隐藏边框
                      align: 'stretch', margin: '8 0 8 0', padding: '0 0 0 0',
                      pack: 'start',
                      items: [
                          {
                              layout: "hbox",
                              width: '100%', baseCls: 'my-panel-no-border',//隐藏边框
                              align: 'stretch', margin: '0 0 0 0', padding: '0 0 0 0',
                              pack: 'start', items: [{ width: 60, baseCls: 'my-panel-no-border' },
                              me.FileUploadButton]
                          },
                          {
                              layout: "hbox",
                              width: '100%', baseCls: 'my-panel-no-border',//隐藏边框
                              align: 'stretch', margin: '8 0 0 0', padding: '0 0 0 0',
                              pack: 'start',
                              items: [{
                                  xtype: "label",
                                  text: "附件:",
                                  width: 60
                              }, me.filegrid]
                          }
                      ], flex: 1
                  },
                   {
                       layout: "hbox",
                       width: '100%', baseCls: 'my-panel-no-border',//隐藏边框
                       align: 'stretch', margin: '8 0 8 0', padding: '0 0 0 0',
                       pack: 'start',
                       items: [me.checkText, {
                           xtype: "button",
                           text: "选择...", margins: "0 0 0 10",
                           listeners: {
                               "click": function (btn, e, eOpts) {//添加点击按钮事件

                                   Ext.require('Ext.ux.Common.comm', function () {
                                       showSelectUserWin("getUser", "", "", function () {
                                           me.checkText.setValue(window.parent.usernamelist);
                                           me.checkList = window.parent.resultvalue;
                                       });
                                   })
                               }
                           }
                       }, me.approvText,
                           {
                               xtype: "button",
                               text: "选择...", margins: "0 0 0 10",
                               listeners: {
                                   "click": function (btn, e, eOpts) {//添加点击按钮事件
                                       Ext.require('Ext.ux.Common.comm', function () {
                                           showSelectUserWin("getUser", "", "", function () {
                                               me.approvText.setValue(window.parent.usernamelist);
                                               me.approvList = window.parent.resultvalue;
                                           });
                                       })
                                   }
                               }
                           }, ]
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
                          text: "确定", width: 60, margins: "10 5 10 5",
                          listeners: {
                              "click": function (btn, e, eOpts) {//添加点击按钮事件
                                  me.send_create_A9();
                              }
                          }
                      },
                      {
                          xtype: "button",
                          text: "取消", width: 60, margins: "10 15 10 5",
                          listeners: {
                              "click": function (btn, e, eOpts) {//添加点击按钮事件
                                  //Ext.Msg.alert("您展开了目录树节点！！！", "您展开了目录树节点！节点ID:" + me.tempDefnId);
                                  winA9.close();
                              }
                          }
                      }
                  ]
              }
                  ]
              }]
          })];

        me.get_combobox_default();//设置combobox默认值

        me.callParent(arguments);
    },

    //设置combobox默认值
    get_combobox_default: function () {
        var me = this;
        me.unitCombo.setRawValue(me.unitdata[0].text);//设置显示值
        me.unitCombo.setValue(me.unitdata[0].value); //设置ID值

        me.professionCombo.setRawValue(me.professiondata[0].text);//设置显示值
        me.professionCombo.setValue(me.professiondata[0].value); //设置ID值

        //设置发文单位
        var nodes = Ext.getCmp(me.mainPanelId).down('treepanel').getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {

            me.projectKeyword = nodes[0].data.Keyword;//定义本页面全局变量
            var projKeyword = nodes[0].data.Keyword;
            Ext.Ajax.request({
                url: 'WebApi/Post',
                method: "POST",
                params: {
                    C: "AVEVA.CDMS.SHEPC_Plugins.EnterPoint", A: "GetCreateWinDefault",
                    action: me.winAction, ProjectKeyword: projKeyword,
                    projectCode: me.projectCodeText.value, Unit: me.unitCombo.value,
                    Send: me.dispatchText.value, profession: me.professionCombo.value,
                    type: me.postCategoryText.value, sid: localStorage.getItem("sid")
                },
                success: function (response, options) {
                    try {
                        //获取数据后，更新窗口
                        var res = Ext.JSON.decode(response.responseText, true);
                        var state = res.success;
                        if (state === true) {
                            var recod = eval(res.data[0]);
                            me.dispatchText.setValue(recod.DispatchCode);
                            me.docNumText.setValue(recod.RunNum);
                            me.checkText.setValue(recod.checkText);
                            me.checkList = recod.checkValue;
                            me.approvText.setValue(recod.auditorText);
                            me.approvList = recod.auditorValue;
                        } else {
                            var errmsg = res.msg;
                            Ext.Msg.alert("错误信息", errmsg);
                        }
                    } catch (e) { }
                }
            });
        }
    },

    //创建A9联系单
    send_create_A9: function () {
        var me = this;
        //var docCode = me.docCodeText.value;
        //var docDesc = me.docDescText.value;
        me.docNum = me.projectCodeText.value + "-" + me.unitCombo.value + "-" + me.dispatchText.value + "-" + me.professionCombo.value + "-" + me.postCategoryText.value + "-" + me.docNumText.value;
        var title = me.titleText.value;
if (title.indexOf("#") > 0) {
            Ext.Msg.alert("系统提示", "文件名称中包含非法字符 # 请去掉！");
            return;
        }
        //获取表格数据，转换成JSON字符串
        var datar = new Array();
        var contentJson = "";
        var records = me.contentStore.getRange();
        for (var i = 0; i < records.length; i++) {
            if (records[i].data.name != "" || records[i].data.number != "" || records[i].data.checkNum != "" || records[i].data.company != "") {
                datar.push(records[i].data);
            }
        }
        contentJson = Ext.JSON.encode(datar);

        //var content = me.contentText.value;
        var projKeyword = me.projectKeyword;
        var filelist = "";
        var fstore = me.filestore;
        if (fstore.getCount() > 0) {
            for (var i = 0; i < fstore.getCount() ; i++) {
                if (i > 0) {
                    filelist = filelist + "|" + fstore.getAt(i).data.name;
                } else { filelist = fstore.getAt(i).data.name; }

            }
        }
        Ext.MessageBox.wait("正在生成Word表单，请稍候...", "等待");
        Ext.Ajax.request({
            //url: 'SHEPCPlugins/CreateD3',
            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.SHEPC_Plugins.EnterPoint", A: "CreateA9",
                //IsUpEdition:是否升版,filelist：文件列表，action：操作命令
                action: me.winAction, ProjectKeyword: projKeyword,
                docNum: me.docNum, title: title,
                content: contentJson,
                checkList: me.checkList, approvList: me.approvList,
                filelist: filelist, IsUpEdition: "flase",
                sid: localStorage.getItem("sid")
            },
            success: function (response, options) {
                //Ext.MessageBox.close();//关闭等待对话框
                //获取数据后，更新窗口
                var res = Ext.JSON.decode(response.responseText, true);
                var state = res.success;
                if (state === true) {
                    var recod = eval(res.data[0]);
                    var projectKeyword = recod.ProjectKeyword;//获取新建的目录id
                    var docKeyword = recod.DocKeyword;//获取联系单文档id

                    me.docList = recod.DocList;//获取流程文档列表
                    me.newProjectKeyword = recod.ProjectKeyword;//获取新建的目录id

                    Ext.MessageBox.close();//关闭等待对话框

                    if (me.FileUploadButton.uploader.uploader.files.length > 0) {
                        ////当有附件时，创建DOC文档成功后，上传附件
                        me.FileUploadButton.uploader.start();

                        Ext.MessageBox.wait("正在上传附件，请稍候...", "等待");

                        var int = window.setInterval(function () {
                            //上传附件完毕
                            if (me.uploadCompleteState === true) {
                                Ext.MessageBox.close();//关闭等待对话框
                                //处理返回事件
                                me.send_create_doc_callback(projectKeyword, docKeyword, me.docList, true);
                                //停止线程
                                window.clearInterval(int);
                            }
                        }, 500);
                    } else {
                        //当没有附件时，处理返回事件
                        me.send_create_doc_callback(projectKeyword, docKeyword, me.docList, true);
                    }
                } else {
                    var errmsg = res.msg;
                    Ext.Msg.alert("错误信息", errmsg);
                }
            },
            failure: function (response, options) {
                //Ext.MessageBox.close();//关闭等待对话框
                Ext.Msg.alert("系统提示", "连接服务器失败，请尝试重新提交！");
            }
        });
    },

    //处理服务端创建联系单后的返回事件
    send_create_doc_callback: function (parentKeyword, docKeyword, docList, closeWin) {
        var me = this;
        me.btnIsClick = false;
        var parentFuctionName = "";//上一级函数名
        ////Ext.MessageBox.close();
        //Ext.MessageBox.show({
        //    title: '工作流启动',
        //    msg: '是否启动校审流程？',
        //    buttons: Ext.MessageBox.YESNO,
        //    parentFuctionName: parentFuctionName,
        //    docKeyword: docKeyword,
        //    buttonText: {
        //        yes: "是",
        //        no: "否"
        //    },
        //    fn: function (btn, parentFuctionName) {
        //        if (me.btnIsClick === false) {//防止第二次按下
        //            me.btnIsClick = true;
        //            if (btn === "yes") {
                        winA9.close();
                        //启动流程
                        Ext.require('Ext.ux.Common.comm', function () {
                            //参数：doclist,wfKeyword,userlist,callback_fun
                            StartNewWorkFlow(docList, "", me.checkList, function (res,WorkFlowKeyword, CuWorkStateCode) {
                                var strs = me.checkText.value.split("__"); //字符分割 
                                if (CuWorkStateCode === "CHECK" && strs.length > 0 && localStorage.getItem("username") === strs[0]) {
                                    //跳转到下一状态
                                    Ext.require('Ext.ux.Common.comm', function () {
                                        GotoNextWorkflowState("提交项目经理", WorkFlowKeyword, "", function () {
                                            //回调函数，通过流程分支
                                            me.refreshWin(parentKeyword, false);
                                        });
                                    })
                                } else {
                                    //刷新窗口,前面已经关闭了窗口，所以这里不需要再关闭窗口
                                    me.refreshWin(parentKeyword, false);
                                }

                            });
                        })
        //            }
        //            else {

        //                Ext.Ajax.request({
        //                    //url: 'SHEPCPlugins/SendManualStartMsg',
        //                    url: 'WebApi/Post',
        //                    method: "POST",
        //                    params: {
        //                        C: "AVEVA.CDMS.SHEPC_Plugins.EnterPoint", A: "SendManualStartMsg",
        //                        action: me.winAction, DocKeyword: docKeyword, sid: localStorage.getItem("sid")
        //                    },
        //                    success: function (response, options) {
        //                        //关闭新建联系单窗口并刷新父窗口
        //                        me.refreshWin(parentKeyword, true);

        //                        Ext.MessageBox.alert("手动启动工作流提示", "提示：您在手动启动流程时，请将主文件与附件全部选中再启动流程，以免流程结束没有附件");
        //                    }
        //                });
        //            }
        //        }
        //    }
        //});
    },

    //刷新表单，参数:parentKeyword:新建的联系单目录
    refreshWin: function (parentKeyword, closeWin) {
        var me=this; var tree = Ext.getCmp(me.mainPanelId).down('treepanel');
        var viewTreeStore = tree.store;

        viewTreeStore.load({
            callback: function (records, options, success) {//添加回调，获取子目录的文件数量
                if (closeWin)
                    winA9.close();

                //展开目录
                Ext.require('Ext.ux.Common.comm', function () {
                    Ext.getCmp('contentPanel').down('_mainProjectTree').ExpendProject(parentKeyword);
                });
            }
        });
    },

    //获取流水号
    getRunNum: function () {
        var me = this;
        Ext.Ajax.request({
            //url: 'SHEPCPlugins/GetRunNum',
            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.SHEPC_Plugins.EnterPoint", A: "GetRunNum",
                action: me.winAction, ProjectCode: me.projectCodeText.value,
                Unit: me.unitCombo.value, Send: me.dispatchText.value,
                Profession: me.professionCombo.value, DocType: me.postCategoryText.value,
                sid: localStorage.getItem("sid")
            },
            success: function (response, options) {
                //获取数据后，更新窗口
                var res = Ext.JSON.decode(response.responseText, true);
                var state = res.success;
                if (state === true) {
                    var recod = eval(res.data[0]);
                    var runNum = recod.RunNum;//获取流水号
                    me.docNumText.setValue(runNum);
                } else {
                    var errmsg = res.msg;
                    Ext.Msg.alert("错误信息", errmsg);
                }
            },
            failure: function (response, options) {
                //                Ext.Msg.alert("系统提示", "连接服务器失败，请尝试重新提交！");
            }
        });
    }
});