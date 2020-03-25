//互提资料
Ext.define('Ext.plug_ins.HXPC_Plugins.exchangeDoc', {
    extend: 'Ext.container.Container',
    alias: 'widget.exchangeDoc',
    layout: 'fit',
    resultvalue: '', mainPanelId: '', 
    projectKeyword: '', exchangeType: '',//互提资料的类型，Create：生成提资单，Continue：继续提资，UpEdition：提资升版
    initComponent: function () {
        var me = this;

        //图纸数量combo初始数据
        me.bookdata = [{ text: "0", value: "0" }, { text: "1", value: "1" }, { text: "2", value: "2" },
        { text: "3", value: "3" }, { text: "4", value: "4" }, { text: "5", value: "5" },
        { text: "6", value: "6" }, { text: "7", value: "7" }, { text: "8", value: "8" },
        { text: "9", value: "9" }];

        //表单数量combo初始数据
        me.formdata = [{ text: "0", value: "0" }, { text: "1", value: "1" }, { text: "2", value: "2" },
        { text: "3", value: "3" }, { text: "4", value: "4" }, { text: "5", value: "5" },
        { text: "6", value: "6" }, { text: "7", value: "7" }, { text: "8", value: "8" },
        { text: "9", value: "9" }];

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

        //添加工程名称text
        me.prjNameText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "工程名称", labelWidth: 75, readOnly: true, fieldStyle:'background-color: #DFE9F6;border-color: #DFE9F6; background-image: none;' ,//fieldStyle:'color:red',
            anchor: "80%", labelAlign: "right", width: 100//flex: 1
        });

        //添加工程编号text
        me.PrjCodeText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "工程编号", labelWidth: 75, readOnly: true, fieldStyle: 'background-color: #DFE9F6;border-color: #DFE9F6; background-image: none;',//fieldStyle:'color:red',
            anchor: "80%", labelAlign: "right", width: 100//flex: 1
        });

        //添加设计阶段text
        me.PhaseText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "设计阶段", labelWidth: 75, fieldStyle: 'background-color: #DFE9F6;border-color: #DFE9F6; background-image: none;',//fieldStyle:'color:red',
            anchor: "80%", labelAlign: "right", width: 100//flex: 1
        });

        //添加资料单编号text
        me.DocNum1Text = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "资料单编号", labelWidth: 75, 
            anchor: "80%", labelAlign: "right", width: 200//flex: 1
        });

        //添加资料单编号text
        me.DocNum2Text = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "字 第", labelWidth: 30, margin: '0 5 0 5', labelSeparator: '', // 去掉laebl中的冒号
            anchor: "80%", labelAlign: "right", width: 120//flex: 1
        });

        //添加资料单编号text
        me.DocNum3Label = Ext.create("Ext.form.Label", {
            xtype: 'label',
            //forId: 'myFieldId',
            text: '',
            margin: '3 0 0 10'
            //xtype: "textfield", fieldLabel: "", labelWidth: 30, margin: '0 5 0 5', labelSeparator: '', // 去掉laebl中的冒号
            //anchor: "80%", labelAlign: "right", width: 120//flex: 1
        });

        //添加提出专业text
        me.OutProfessionText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "提出专业", labelWidth: 75, readOnly: true, fieldStyle: 'background-color: #DFE9F6;border-color: #DFE9F6; background-image: none;',//fieldStyle:'color:red',
            anchor: "80%", labelAlign: "right", width: 100//flex: 1
        });

        //添加签收人text
        me.ReceiverText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "签收人", labelWidth: 75, readOnly: true, fieldStyle: 'background-color: #DFE9F6;border-color: #DFE9F6; background-image: none;',//fieldStyle:'color:red',
            anchor: "80%", labelAlign: "right", width: 100//flex: 1
        });

        //添加资料标题text
        me.TitleText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "资料标题", labelWidth: 75,
            anchor: "80%", labelAlign: "right", width: 100//flex: 1
        });

        //添加内容Text
        me.contentText = Ext.create("Ext.form.field.TextArea", {
            xtype: "textarea", anchor: "80%", labelWidth: 75, labelAlign: "right", margin: '8 0 5 0', width: 360,
            fieldLabel: "内容", height: 60
        });

        //定义资料重要性RadioGroup
        me.priImportRadioGroup = Ext.create("Ext.form.RadioGroup", {
            //xtype:'radiogroup',
            //fieldLabel:'文件类别',
            name: 'projectType',  //后台返回的JSON格式，直接赋值；
            width: 210,
            items: [
            { boxLabel: '一般资料', name: 'priImport', width:80,inputValue: '一般资料', checked: true },
            { boxLabel: '综合性重要资料', name: 'priImport', inputValue: '综合性重要资料' }
            ]
        });

        me.selAllCheckBox = new Ext.form.Checkbox(
          {
              boxLabel: '所有专业',//obj.Table[i].Title; "Title"指的是返回的名字
              inputValue: 'A',
              checked: false,
              listeners: {
                  change:function (field, newValue, oldValue, eOpts) {
                      me.getReceiverTextValue(field, newValue, oldValue, eOpts);
                  },

                  dirtychange:function (field, isDirty, eOpts) {
                      //Ext.Msg.alert("您展开了目录树节点！！！", "您展开了目录树节点！节点ID:");
                      //me.getReceiverTextValue(field, newValue, oldValue, eOpts);
                      //me.getReceiverTextValue(field, isDirty, eOpts);
                  }
              }
          });

        //----------------------复选组开始----------------------//
        me.checkboxgroup = new Ext.form.CheckboxGroup({
            columns: 3,
            width: 170,
            //vertical: true,//填满一列再填第二列
            //动态加载专业checkbox
            listeners: {
                render: function (view, opt) {
                    LoadingOperationBehavior();
                }
            },

            items: [
                me.selAllCheckBox
                //{
                //    boxLabel: '所有专业',
                //    inputValue: 'A'
                //}
            ]

        });

        me.checkboxfieldset = new Ext.form.FieldSet({
            //xtype: "fieldset",
            margin: '0 0 8 0',
            //title: '接受专业',
            height: 100,
            autoScroll:true,//显示滚动条
            //baseCls: 'my-panel-no-border',//隐藏边框
            layout: {
                type: 'vbox',
                pack: 'start',
                align: 'stretch'
            },
            items: [
                  me.checkboxgroup
            ],flex:1
        });

        //动态加载专业checkbox
        function LoadingOperationBehavior() {
        }

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
                    fileCode = me.docName + " 互提资料单_附件" + me.docUploadIndex.toString() + " " + fileCode;

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

        //添加图纸数量combo
        Ext.define("bookModel", {
            extend: 'Ext.data.Model',
            fields: ["text", "value"]
        });
        me.bookProxy = Ext.create("Ext.data.proxy.Memory", {
            data: me.bookdata,
            model: "bookModel"
        });

        me.bookStore = Ext.create("Ext.data.Store", {
            model: bookModel,
            proxy: me.bookProxy
        });


        me.bookCombo = Ext.create("Ext.form.field.ComboBox",
        {
            //xtype: "combo",
            fieldLabel: '图纸数量', labelWidth: 60,
            triggerAction: "all", store: me.bookStore,
            valueField: 'value', //editable: false,//不可输入
            displayField: 'text', margin: '0 10 0 10',// 
            anchor: "80%", labelAlign: "left", width: 120,//
            emptyText: "--请选择--", autoLoadOnValue: true,value:"0",
            //fieldStyle: 'border-color: red; background-image: none;',//红色边框
            listeners:
            {
                select: function (combo, records, eOpts) {
                }
            }
        });

        //添加表单数量combo
        Ext.define("formModel", {
            extend: 'Ext.data.Model',
            fields: ["text", "value"]
        });
        me.formProxy = Ext.create("Ext.data.proxy.Memory", {
            data: me.formdata,
            model: "formModel"
        });

        me.formStore = Ext.create("Ext.data.Store", {
            model: formModel,
            proxy: me.formProxy
        });


        me.formCombo = Ext.create("Ext.form.field.ComboBox",
        {
            //xtype: "combo",
            fieldLabel: '表单数量', labelWidth: 60,
            triggerAction: "all", store: me.formStore,
            valueField: 'value', //editable: false,//不可输入
            displayField: 'text', margin: '0 10 0 10',// 
            anchor: "80%", labelAlign: "left", width: 120,//
            emptyText: "--请选择--", autoLoadOnValue: true, value: "0",
            //fieldStyle: 'border-color: red; background-image: none;',//红色边框
            listeners:
            {
                select: function (combo, records, eOpts) {
                }
            }
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
                                  text: '专 业 间 互 提 资 料 单', margins: '0 0 0 10'
                              }, { baseCls: 'my-panel-no-border', flex: 1 }]
                          },
                          {
                              xtype: "fieldset", margin: '8 16 0 16',
                              baseCls: 'my-panel-no-border',//隐藏边框
                              layout: {
                                  type: 'vbox',
                                  pack: 'start',
                                  align: 'stretch'
                              },
                              items: [

                                  me.prjNameText,
                                  me.PrjCodeText,
                                  me.PhaseText,
                                  {
                                      xtype: "fieldset", margin: '3 0 5 0',
                                      baseCls: 'my-panel-no-border',//隐藏边框
                                      layout: {
                                          type: 'hbox',
                                          pack: 'start',
                                          align: 'stretch'
                                      },
                                      items: [
                                           me.DocNum1Text,
                                           me.DocNum2Text,
                                           {
                                               xtype: "label",
                                               text: "号",
                                               margin: '3 0 8 0'
                                           },
                                           me.DocNum3Label
                                      ]
                                  },
                                  me.OutProfessionText,
                                  {
                                      layout: "hbox",
                                      //width: '100%',
                                      //width:80,
                                      baseCls: 'my-panel-no-border',//隐藏边框
                                      align: 'stretch', margin: '0 0 0 0', padding: '0 0 0 0',
                                      pack: 'start', items: [//{ width: 60, baseCls: 'my-panel-no-border' },
                                       {
                                           xtype: "label",
                                           text: "接受专业:",
                                           margin: '8 -15 8 19',
                                           width: 75
                                       },
                                       me.checkboxfieldset]
                                  },
                                  
                                  me.ReceiverText,
                                  me.TitleText,
                                  me.contentText,
                                  {
                                      layout: "vbox",
                                      width: '100%', baseCls: 'my-panel-no-border',//隐藏边框
                                      align: 'stretch', margin: '0 0 8 0', padding: '0 0 0 0',
                                      pack: 'start',
                                      items: [
                                          //{
                                          //    layout: "hbox",
                                          //    width: '100%', baseCls: 'my-panel-no-border',//隐藏边框
                                          //    align: 'stretch', margin: '0 0 0 0', padding: '0 0 0 0',
                                          //    pack: 'start', items: [{ width: 60, baseCls: 'my-panel-no-border' },
                                          //    me.FileUploadButton]
                                          //},
                                          {
                                              layout: "hbox",
                                              width: '100%', baseCls: 'my-panel-no-border',//隐藏边框
                                              align: 'stretch', margin: '8 0 0 0', padding: '0 0 0 0',
                                              pack: 'start',
                                              items: [                                        
                                                  {
                                                  layout: "vbox",
                                                      //width: '100%',
                                                  width:80,
                                                  baseCls: 'my-panel-no-border',//隐藏边框
                                                  align: 'stretch', margin: '0 0 0 0', padding: '0 0 0 0',
                                                  pack: 'start', items: [//{ width: 60, baseCls: 'my-panel-no-border' },
                                                   {
                                                      xtype: "label",
                                                      text: "接口附件:",
                                                      margin: '8 0 8 19',
                                                      width: 75
                                                  }, me.FileUploadButton]
                                              }, me.filegrid]
                                          }
                                      ], flex: 1
                                  },
                                  {
                                      //xtype: "fieldset",
                                      //title: "资料重要性",
                                      //layout: "form",
                                      //width: 200,
                                      xtype: "fieldset", margin: '8 0 8 0',
                                      baseCls: 'my-panel-no-border',//隐藏边框
                                      layout: {
                                          type: 'hbox',
                                          pack: 'start',
                                          align: 'stretch'
                                      },
                                      items: [
                                          {
                                              xtype: "label",
                                              text: "资料重要性:",
                                              margin: '3 0 0 11',
                                              width:80
                                          },
                                               me.priImportRadioGroup,
                                               me.bookCombo,
                                               me.formCombo
                                      ]
                                  }


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
                                      me.send_exchangeDoc();

                                  }
                              }
                          },
                          {
                              xtype: "button",
                              text: "取消", width: 60, margins: "0 15 10 5",
                              listeners: {
                                  "click": function (btn, e, eOpts) {//添加点击按钮事件
                                      //Ext.Msg.alert("您展开了目录树节点！！！", "您展开了目录树节点！节点ID:" + me.tempDefnId);
                                      winExchangeDoc.close();
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
        me.sendGetExchangeDocDefault();


        me.callParent(arguments);
    },
    
    //获取打开表单时的默认参数
    sendGetExchangeDocDefault: function () {
        var me = this;
        //通过extjs的ajax获取操作全部名称
        Ext.Ajax.request({
            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.HXPC_Plugins.ExchangeDoc", A: "GetExchangeDocDefault",
                sid: localStorage.getItem("sid"), ProjectKeyword: me.projectKeyword,
                ExchangeType:me.exchangeType
            },
            success: function (response, options) {
                me.sendGetExchangeDocDefaul_callback(response, options);//, funCallback);

            },
            failure: function (response, options) {
                ////Ext.Msg.alert("错误", "连接服务器失败！<br>" + response.responseText);
            }
        });
    },

    sendGetExchangeDocDefaul_callback:function (response, options){
        var me = this;
        //获取数据后，更新窗口
        var res = Ext.JSON.decode(response.responseText, true);
        var state = res.success;
        if (state === true) {
            var recod = eval(res.data[0]);

            var strPrjName = recod.PrjName;
            var strPrjCode = recod.PrjCode;
            var strPhase = recod.Phase;
            var strDocNum1 = recod.DocNum1;
            var strDocNum2 = recod.DocNum2;
            var strOutProfession = recod.OutProfession;
            var PressionList = eval(recod.PressionList);
            var strEDProfession = recod.EDProfession;//已选择专业列表

            me.prjNameText.setValue(strPrjName);
            me.PrjCodeText.setValue(strPrjCode);
            me.PhaseText.setValue(strPhase);
            me.DocNum1Text.setValue(strDocNum1);
            me.DocNum2Text.setValue(strDocNum2);

            //提出专业
            me.OutProfessionText.setValue(strOutProfession);

            //如果是提资升版
            if (me.exchangeType === 'UpEdition')
            {
                var strDocNum3 = recod.DocNum3;
                var strContent = recod.Content;
                var strReceiver = recod.Receiver;
                var strPriNormal = recod.PriNormal;
                
                me.DocNum3Label.setText(strDocNum3);
                me.contentText.setValue(strContent);
                me.ReceiverText.setValue(strReceiver);
                if (strPriNormal === "综合性重要资料")
                {
                    me.priImportRadioGroup.setValue({ priImport: strPriNormal });//综合性重要资料被选中
                }
            }

            //遍历设计阶段数组，添加到设计阶段combo
            for (var itemKey in PressionList) {
                if (PressionList[itemKey].Pression != undefined) {
                    var checkValue = false;

                    //判断是否已选择
                    if (strEDProfession.indexOf(PressionList[itemKey].Pression)>=0)
                    {
                        checkValue = true;
                    }

                    var checkbox = new Ext.form.Checkbox(
                      {
                          boxLabel: PressionList[itemKey].Pression,//obj.Table[i].Title; "Title"指的是返回的名字
                          name: PressionList[itemKey].Pression,
                          inputValue: PressionList[itemKey].Pression,
                          OwnerUser: PressionList[itemKey].OwnerUser,
                          checked: checkValue,
                          listeners: {
                              change:function (field, newValue, oldValue, eOpts) {
                                  me.getReceiverTextValue(field, newValue, oldValue, eOpts);
                              }
                          }
                      });

                    me.checkboxgroup.items.add(checkbox);
                }
            }

            //me.checkboxfieldset.doLayout(); //重新调整版面布局 
        }
    },

    //选取所有专业触发的事件
    getReceiverTextValue: function (field, newValue, oldValue, eOpts) {
    //getReceiverTextValue: function (field, isDirty, eOpts) {
        var me = this;
        var strReceiver = "";

        //var newValue = field.checked;

        if (field.boxLabel === "所有专业" && field.inputValue === "A") {
            //如果是通过鼠标点击选择的所有专业
            if (field.hasFocus === true) {
                //把所有专业都选上
                if (newValue === true) {
                    for (var i = 1; i < me.checkboxgroup.items.items.length; i++) {
                        me.checkboxgroup.items.items[i].setValue(true);
                    }
                } else {
                    //取消所有专业的选择
                    for (var i = 1; i < me.checkboxgroup.items.items.length; i++) {
                        me.checkboxgroup.items.items[i].setValue(false);
                    }
                }
            }
        } else {
            if (newValue === false && field.hasFocus === true) {
                me.checkboxgroup.items.items[0].setValue(false);
            }
        }

        //加上hasFocus，保证只运行一次
        if (field.hasFocus === true) {
            for (var i = 0; i < me.checkboxgroup.items.items.length; i++) {

                if (me.checkboxgroup.items.items[i].value === true && me.checkboxgroup.items.items[i].OwnerUser != undefined && me.checkboxgroup.items.items[i].OwnerUser != "") {
                    strReceiver = strReceiver + me.checkboxgroup.items.items[i].OwnerUser + ",";
                }

            }
            if (strReceiver.length > 0)
                strReceiver = strReceiver.substring(0, strReceiver.length - 1);

            me.ReceiverText.setValue(strReceiver);
        }

    },

    //提交创建互提资料但
    send_exchangeDoc: function () {
        var me = this;


        //获取工程名称Text
        var prjName = me.prjNameText.value;

        //获取工程编号Text
        var PrjCode = me.PrjCodeText.value;

        //获取设计阶段Text
        var Phase = me.PhaseText.value;

        //获取资料单编号Text
        var DocNum1 = me.DocNum1Text.value;

        //获取资料单编号Text
        var DocNum2 = me.DocNum2Text.value;

        //获取资料单编号Text
        var DocNum3 = me.DocNum3Label.text;

        me.docName = DocNum1 + "字第" + DocNum2 + "号" + DocNum3;

        //获取提出专业Text
        var OutProfession = me.OutProfessionText.value;

        //获取签收人Text
        var Receiver = me.ReceiverText.value;

        //获取资料标题Text
        var Title = me.TitleText.value;

        //获取内容Text
        var content = me.contentText.value;

        //获取资料重要性RadioGroup
        var priImport = me.priImportRadioGroup.getValue().priImport;

        //获取图纸数量combo
        var bookQuantity = me.bookCombo.value;

        //获取表单数量combo
        var formQuantity = me.formCombo.value;

        //获取接收专业
        var checkgroupValue = me.checkboxgroup.getChecked();
        var professionList = '';
        var aryIndex = 0;
        Ext.Array.each(checkgroupValue, function (item) {
            if (item.boxLabel != '所有专业') {//把第一个'所有专业'去掉
                if (aryIndex != 0)
                    professionList += ',';
                professionList += item.boxLabel;
            }
            aryIndex = aryIndex + 1;
        });

        //获取表单数据，转换成JSON字符串
        var docAttr =
        [
            { name: 'PrjName', value: prjName },
            { name: 'PrjCode', value: PrjCode },
            { name: 'Phase', value: Phase },
            { name: 'DocNum1', value: DocNum1 },
            { name: 'DocNum2', value: DocNum2 },
            { name: 'DocNum3', value: DocNum3 },
            { name: 'OutProfession', value: OutProfession },
            { name: 'ProfessionList', value: professionList },
            { name: 'Receiver', value: Receiver },
            { name: 'Title', value: Title },
            { name: 'Content', value: content },
            { name: 'PriImport', value: priImport },
            { name: 'BookQuantity', value: bookQuantity },
            { name: 'FormQuantity', value: formQuantity }
            
        ]


        var docAttrJson = Ext.JSON.encode(docAttr);

        Ext.MessageBox.wait("正在创建互提资料单，请稍候...", "等待");

        Ext.Ajax.request({

            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.HXPC_Plugins.ExchangeDoc", A: "CreateExchangeDoc",
                sid: localStorage.getItem("sid"), ProjectKeyword: me.projectKeyword,
                ExchangeType: me.exchangeType, docAttrJson: docAttrJson
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

                    if (me.FileUploadButton.uploader.uploader.files.length > 0) {
                        ////当有附件时，创建DOC文档成功后，上传附件
                        me.FileUploadButton.uploader.start();

                        Ext.MessageBox.wait("正在上传附件，请稍候...", "等待");

                        var int = window.setInterval(function () {
                            //上传附件完毕
                            if (me.uploadCompleteState === true) {
                                Ext.MessageBox.close();//关闭等待对话框
                                //处理返回事件
                                me.send_exchangeDoc_callback(response, options, "");//, me.projectKeyword, closeWin);
                                //me.send_create_doc_callback(projectKeyword, docKeyword, me.docList, true);
                                //停止线程
                                window.clearInterval(int);
                            }
                        }, 500);
                    } else {
                        //当没有附件时，处理返回事件
                        me.send_exchangeDoc_callback(response, options, "");//, me.projectKeyword, closeWin);
                        //me.send_create_doc_callback(projectKeyword, docKeyword, me.docList, true);
                    }
                } else {
                    var errmsg = res.msg;
                    Ext.Msg.alert("错误信息", errmsg);
                }
            }

        })
    },

    //处理创建互提资料单的返回
    send_exchangeDoc_callback:function (response, options){
        var me = this;

        Ext.MessageBox.wait("正在启动流程，请稍候...", "等待");

        Ext.Ajax.request({

            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.HXPC_Plugins.ExchangeDoc", A: "ExchangeDocStartWorkFlow",
                sid: localStorage.getItem("sid"), docKeyword: me.docKeyword,
                docList:me.docList
                //ExchangeType: me.exchangeType
            },
            success: function (response, options) {

                //获取数据后，更新窗口
                var res = Ext.JSON.decode(response.responseText, true);
                var state = res.success;
                if (state === true) {

                    Ext.MessageBox.close();//关闭等待对话框

                    var recod = eval(res.data[0]);
                    //winExchangeDoc.close();
                    me.refreshWin(me.newProjectKeyword, true);
                } else {
                    var errmsg = res.msg;
                    Ext.Msg.alert("错误信息", errmsg);
                    winExchangeDoc.close();
                }
            }

        })

        //启动流程
        //Ext.require('Ext.ux.Common.comm', function () {
        //    //参数：doclist,wfKeyword,userlist,callback_fun
        //    StartNewWorkFlow(docList, "", me.checkList, function (res, WorkFlowKeyword, CuWorkStateCode) {
        //        var strs = me.checkText.value.split("__"); //字符分割 
        //        if (CuWorkStateCode === "CHECK" && strs.length > 0 && localStorage.getItem("username") === strs[0]) {
        //            //跳转到下一状态
        //            Ext.require('Ext.ux.Common.comm', function () {
        //                GotoNextWorkflowState("提交项目经理", WorkFlowKeyword, "", function () {
        //                    //回调函数，通过流程分支
        //                    me.refreshWin(parentKeyword, false);
        //                });
        //            })
        //        } else {
        //            //刷新窗口,前面已经关闭了窗口，所以这里不需要再关闭窗口
        //            me.refreshWin(parentKeyword, false);
        //        }

        //    });
        //})

        //var res = Ext.JSON.decode(response.responseText, true);
        //var state = res.success;

        //if (state === false) {
        //    var errmsg = res.msg;
        //    Ext.Msg.alert("错误信息", errmsg);
        //}
        //else {
        //    me.refreshWin(me.projectKeyword, true);
        //}
    },

    //刷新表单，参数:parentKeyword:新建的联系单目录
    refreshWin: function (projKeyword, closeWin) {
        var me = this;
        var tree = Ext.getCmp(me.mainPanelId).up('_mainSourceView').down('_mainProjectTree').down('treepanel');//;
        var viewTreeStore = tree.store;

        viewTreeStore.load({
            callback: function (records, options, success) {//添加回调，获取子目录的文件数量
                if (closeWin)
                    winExchangeDoc.close();

                //展开目录
                Ext.require('Ext.ux.Common.comm', function () {
                    Ext.getCmp('contentPanel').down('_mainProjectTree').ExpendProject(projKeyword);
                });
            }
        });
    }
    
});