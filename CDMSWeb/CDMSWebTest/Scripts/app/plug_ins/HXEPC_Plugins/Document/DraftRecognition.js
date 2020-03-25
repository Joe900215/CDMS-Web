//认质认价

Ext.define('Ext.plug_ins.HXEPC_Plugins.Document.DraftRecognition', {
    extend: 'Ext.container.Container',
    alias: 'widget.DraftRecognition',
    //layout: "border",
    layout: 'fit',
    resultvalue: '', mainPanelId: '', projectKeyword: '',
    initComponent: function () {
        var me = this;

   
        //附件文件名的前缀
        me.docCode = "";
        me.docFileName = "";

        //附件序号
        me.docUploadIndex = 0;

        //下一流程状态用户
        me.nextStateUserList = "";

        //收发文单位列表初始数据
        me.recCompanyList = [];
        me.sendCompanyList = [];

        //定义文件编码Panel
        me.fileCodePanel = Ext.create('Ext.plug_ins.HXEPC_Plugins.Document.Panel.FileCodePanel');

        //定义文件上传Panel
        me.fileUploadPanel = Ext.create('Ext.plug_ins.HXEPC_Plugins.Document.Panel.FileUploadPanel', {
            projectKeyword: me.projectKeyword, projectDirKeyword: me.projectDirKeyword
        });

        //设置上传控件为附件模式
        me.fileUploadPanel.setAttaMode();

        //me.fileUploadPanel.gridMaxHeight = me.container.lastBox.height - 40;
        me.fileUploadPanel.setGridMinHeight(198);

        me.fileUploadPanel.onFileEditButtonClick = function () {
            me.editTopPanel.hide();
            me.editBottomPanel.hide();
            me.bottomButtonPanel.hide();
            me.fileUploadPanel.setHeight(me.container.lastBox.height - 40);
            me.fileUploadPanel.filegrid.setHeight(me.container.lastBox.height - 40);
            winDraftRecognition.setTitle("起草认质认价单 - 编辑附件");
            winDraftRecognition.closable = false;
        };

        //保存附件按钮事件
        me.fileUploadPanel.onFileSaveButtonClick = function () {
            //me.totalPagesText.setValue(me.fileUploadPanel.totalPages);
            me.editTopPanel.show();
            me.editBottomPanel.show();
            me.bottomButtonPanel.show();
            //me.filegrid.setHeight(me.gridMinHeight);
            winDraftRecognition.setTitle("起草认质认价单");
            winDraftRecognition.closable = true;
        };

        //定义区域combo初始数据
        me.areadata = [];

        //定义区域combo初始数据
        //me.professiondata = [];

        //定义发文单位combo初始数据
        me.sendCompanydata = [];

        //定义收文单位combo初始数据
        me.recCompanydata = [];

        //来文单类型combo初始数据
        me.rectypedata = [{ text: "信函", value: "信函" }, { text: "设计变更单", value: "设计变更单" }
            , { text: "委托单", value: "委托单" }, { text: "", value: "" }];

        //物资类型combo初始数据
        me.materialtypedata = [{ text: "材料", value: "材料" }, { text: "设备", value: "设备" }];

        //添加项目号text
        me.fProjectCodeText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "文件属性", labelWidth: 60, readOnly: true, fieldStyle: ' background-color: #DFE9F6;border-color: #DFE9F6; background-image: none;',//fieldStyle:'color:red',
            margin: '10 10 0 10', anchor: "80%", labelAlign: "right", width: 150//flex: 1
        });

        //添加区域text
        me.areaText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: " - ", labelWidth: 10, labelSeparator: '', emptyText: "机组+(厂房/系统)", // 去掉laebl中的冒号
            margin: '10 0 0 10', anchor: "80%", labelAlign: "left", width: 120//flex: 1
        });

        //添加专业text
        me.professionText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: " - ", labelWidth: 10, readOnly: true, labelSeparator: '', emptyText: "专业", // 去掉laebl中的冒号
            margin: '10 0 0 10', anchor: "80%", labelAlign: "left", width: 80//flex: 1
        });

        //添加收文类型text
        me.receiveTypeText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: " - ", labelWidth: 10, readOnly: true, labelSeparator: '', emptyText: "文件类型", // 去掉laebl中的冒号
            margin: '10 0 0 10', anchor: "80%", labelAlign: "left", width: 80//flex: 1
        });

        //添加合同名称text
        me.contractNameText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "合同名称", labelWidth: 60, readOnly: true,
            margin: '10 0 0 10', anchor: "80%", labelAlign: "right", width: '46%'//flex: 1
        });

        //添加合同号text
        me.contractCodeText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "合同号", labelWidth: 60, readOnly: true,
            margin: '10 0 0 10', anchor: "80%", labelAlign: "right", width: '46%'//flex: 1
        });



        //添加来源文件编码text
        me.recCodeText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "来源文件", labelWidth: 60, emptyText: "来源文件编码", labelSeparator: '', // 去掉laebl中的冒号
            margin: '10 10 0 10', anchor: "80%", labelAlign: "right", width: 250
        });

        //添加函件流水号text
        me.fNumberText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: " - ", labelWidth: 10, labelSeparator: '', // 去掉laebl中的冒号
            margin: '10 0 0 10', anchor: "80%", labelAlign: "left", width: 90//flex: 1
        });

        //版本号
        me.editionText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: " - ", labelWidth: 10, labelSeparator: '', emptyText: "版本", // 去掉laebl中的冒号
            margin: '10 0 0 10', anchor: "80%", labelAlign: "left", width: 50//flex: 1
        });

        //定义下一流程状态用户Text
        me.nextStateUserText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "项目专工", anchor: "80%", labelWidth: 60, labelAlign: "right", labelPad: 8, width: "40%",//width: 230, 
            margin: '10 5 0 10', fieldStyle: ' background-image: none;'//红色边框//flex: 1
        });

        //定义发送日期Text
        me.sendDateField = Ext.create("Ext.form.field.Date", {
            name: "date", 
            fieldLabel: ' 发送日期', fieldStyle: ' background-image: none;',
            editable: true, labelWidth: 60, margin: '10 10 0 10',
            labelAlign: "right", labelPad: 8,
            emptyText: "--请选择--", autoLoadOnValue: true,
            format: 'Y年m月d日',
            value: new Date(),
            width:'50%'//width: 230
        });
        
        //添加来文单类型combo
        Ext.define("rectypeModel", {
            extend: 'Ext.data.Model',
            fields: ["text", "value"]
        });
        me.rectypeProxy = Ext.create("Ext.data.proxy.Memory", {
            data: me.rectypedata,
            model: "rectypeModel"
        });

        me.rectypeStore = Ext.create("Ext.data.Store", {
            model: rectypeModel,
            proxy: me.rectypeProxy
        });


        me.rectypeCombo = Ext.create("Ext.form.field.ComboBox",
        {
            //xtype: "combo",
            fieldLabel: '', labelWidth: 50,
            triggerAction: "all", store: me.rectypeStore,
            valueField: 'value', editable: false,//不可输入
            displayField: 'text', margin: '10 10 0 10',// 
            anchor: "80%", labelAlign: "right", labelPad: 8, width: 100,//
            emptyText: "--请选择--", autoLoadOnValue: true, value: "信函",
            //fieldStyle: 'border-color: red; background-image: none;',//红色边框
            listeners:
            {
                select: function (combo, records, eOpts) {

                }
            }
        });

        //添加物资类型combo
        Ext.define("materialtypeModel", {
            extend: 'Ext.data.Model',
            fields: ["text", "value"]
        });
        me.materialtypeProxy = Ext.create("Ext.data.proxy.Memory", {
            data: me.materialtypedata,
            model: "materialtypeModel"
        });

        me.materialtypeStore = Ext.create("Ext.data.Store", {
            model: materialtypeModel,
            proxy: me.materialtypeProxy
        });


        me.materialtypeCombo = Ext.create("Ext.form.field.ComboBox",
        {
            //xtype: "combo",
            fieldLabel: '要求，现申请总包单位对以下拟采购', labelWidth: 200,
            triggerAction: "all", store: me.materialtypeStore,
            valueField: 'value', editable: false,//不可输入
            displayField: 'text', margin: '10 10 0 0', labelSeparator: '',
            anchor: "80%", labelAlign: "right", labelPad: 8, width: 300,//
            emptyText: "--请选择--", autoLoadOnValue: true, value: "材料",
            //fieldStyle: 'border-color: red; background-image: none;',//红色边框
            listeners:
            {
                select: function (combo, records, eOpts) {

                }
            }
        });

        me.cellEditing = new Ext.grid.plugin.CellEditing({
            clicksToEdit: 1
        });


   
        me.DescEditing = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            //id:"inputtext",
            fieldLabel: "", anchor: "80%", labelWidth: 0, labelAlign: "left", width: 160,//flex: 1
            value: ""
        });

   
        //选择合同按钮
        me.contractNameButton = Ext.create("Ext.button.Button", {
            text: "..", margins: "10 0 0 0",
            listeners: {
                "click": function (btn, e, eOpts) {//添加点击按钮事件
                    var fmSelectContract = Ext.create('Ext.plug_ins.HXEPC_Plugins.Document.SelectContract', { title: "", mainPanelId: me.Id, projectKeyword: me.projectKeyword });

                    winSelectContract = Ext.widget('window', {
                        title: '选择合同',
                        width: 738,
                        height: 558,
                        minWidth: 738,
                        minHeight: 558,
                        layout: 'fit',
                        resizable: true,
                        modal: true,
                        closeAction: 'close', //close 关闭  hide  隐藏  
                        items: fmSelectContract,
                        defaultFocus: 'firstName'
                    });

                    fmSelectContract.projectKeyword = me.projectKeyword;

                    winSelectContract.show();


                    //监听子窗口关闭事件
                    winSelectContract.on('close', function () {
                        if (window.parent.resultvalue != null && window.parent.resultvalue !== "") {

                            var contractCode = "";
                            var contractDesc = "";
                            var contractValue = "";

                            contractCode = window.parent.resultvalue;
                            contractDesc = window.parent.contractdesclist;

                            if (contractCode.indexOf(",") > 0) {
                               
                                contractCode = contractCode.substring(0, contractCode.indexOf(","));
                                contractDesc = contractDesc.substring(0, contractDesc.indexOf(";"));
                            }

                            me.contractCodeText.setValue(contractCode);

                            me.contractNameText.setValue(contractDesc);


                        }
                    });
                }
            }
        });

        //选择合同按钮
        me.contractCodeButton = Ext.create("Ext.button.Button", {
            text: "..", margins: "10 0 0 0",
            listeners: {
                "click": function (btn, e, eOpts) {//添加点击按钮事件
                    me.contractNameButton.fireEvent('click');
                }
            }
        });

        //编辑区域头部
        me.editTopPanel = Ext.create("Ext.panel.Panel", {
            baseCls: 'my-panel-no-border',//隐藏边框
            layout: {
                type: 'vbox',
                pack: 'start',
                align: 'stretch'
            },
            margin: '0 0 0 0',// 
            items: [
      

                     me.fileCodePanel,
                    {

                        baseCls: 'my-panel-no-border',//隐藏边框
                        layout: {
                            type: 'hbox',
                            pack: 'start',
                            align: 'stretch'
                        },
                        items: [
                            me.contractNameText, me.contractNameButton,
                            me.contractCodeText, me.contractCodeButton
                        ],
                        flex: 1
                    },
                   
                     {
                         layout: "hbox",
                         width: '100%',
                         align: 'stretch',
                         pack: 'start',
                         baseCls: 'my-panel-no-border',//隐藏边框
                         items: [
                            me.recCodeText,
                             me.rectypeCombo,
                             me.materialtypeCombo
                         ], flex: 1
                     },
			        {
			            xtype: "label",
			            margin: '10 0 10 20',
			            text:"进行认质认价工作，以便采购并施工："
			        },
                     
            ]
        });

   
        //编辑区域尾部
        me.editBottomPanel = Ext.create("Ext.panel.Panel", {
            layout: "hbox",
            width: '100%',
            align: 'stretch',
            pack: 'start', margins: "0 0 0 0",
            baseCls: 'my-panel-no-border',//隐藏边框
            items: [
             // me.approvpathCombo,//定义审批路径
                me.nextStateUserText,
                      {
                          xtype: "button",
                          text: "选择...", margins: "10 0 0 5",
                          listeners: {
                              "click": function (btn, e, eOpts) {//添加点击按钮事件
                                  Ext.require('Ext.ux.Common.comm', function () {
                                      showSelectUserWin("getUser", "", "", function () {
                                          me.nextStateUserText.setValue(window.parent.usernamelist);
                                          me.nextStateUserList = window.parent.resultvalue;
                                      });
                                  })
                              }
                          }
                      }
            ]//, flex: 1
        });

        //底部按钮区域
        me.bottomButtonPanel = Ext.create("Ext.panel.Panel", {
            //xtype: "panel",
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
                            me.send_draft_document();
                        }
                    }
                },
                {
                    xtype: "button",
                    text: "取消", width: 60, margins: "10 15 10 5",
                    listeners: {
                        "click": function (btn, e, eOpts) {//添加点击按钮事件

                            winDraftRecognition.close();
                        }
                    }
                }
            ]
        });

        //添加列表
        me.items = [
          Ext.widget('form', {
              baseCls: 'my-panel-no-border',//隐藏边框
              layout: {
                  type: 'vbox',
                  align: 'stretch',
                  pack: 'start'
              },
              items: [{//上部容器
                  baseCls: 'my-panel-no-border',//隐藏边框
                  layout: {
                      type: 'vbox',
                      pack: 'start',
                      align: 'stretch'
                  },
                  margin: '10 0 0 0',// 
                  items: [
                     me.editTopPanel,
                     me.fileUploadPanel,
                     me.editBottomPanel
                   

                  ], flex: 1
              },
              me.bottomButtonPanel

              ]
          })

        ];

        //获取打开表单时的默认参数
        //me.sendGetDraftRecognitionDefault();

        me.callParent(arguments);

    },


    //获取起草信函表单默认参数
    sendGetDraftRecognitionDefault: function (funCallback) {
        var me = this;

        //通过extjs的ajax获取操作全部名称
        Ext.Ajax.request({
            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.HXEPC_Plugins.Document", A: "GetDraftRecognitionDefault",
                sid: localStorage.getItem("sid"), ProjectKeyword: me.projectKeyword
            },
            success: function (response, options) {
                me.sendGetDraftRecognitionDefault_callback(response, options, funCallback);

            }
        });
    },

    //处理获取发文处理表单默认参数的返回
    sendGetDraftRecognitionDefault_callback: function (response, options,funCallback) {
        var me = this;

        //获取数据后，更新窗口
        var res = Ext.JSON.decode(response.responseText, true);
        var state = res.success;
        if (state === true) {
            var recod = eval(res.data[0]);

            var strRootProjectCode = recod.RootProjectCode;
            var strRootProjectDesc = recod.RootProjectDesc;

            var strDocNumber = recod.DocNumber;
            me.recCompanyList = eval(recod.RecCompanyList);
            me.sendCompanyList = eval(recod.SendCompanyList);
            var sourceCompany = recod.SourceCompany;//项目所属公司
            var sourceCompanyDesc = recod.SourceCompanyDesc;

            var strFormClassCode = "AAA"
            var strProjectDesc = "认质认价";

            //默认设置为不新建文件编码
            me.fileCodePanel.setNeedNewFileCode(true);

            //设置发起目录和项目所在目录
            me.fileCodePanel.projectKeyword = me.projectKeyword;//项目所在目录
            me.fileCodePanel.projectDirKeyword = me.projectKeyword;//当前目录

            //设置收发文单位的单位类型
            me.fileCodePanel.setFormClass(strFormClassCode, strProjectDesc);
            //设置项目管理类文件里项目的代码和描述
            me.fileCodePanel.setRootProject(strRootProjectCode, strRootProjectDesc);

            //认质认价默认都是项目发到部门
            //me.fileCodePanel.setDocUnitClass("项目", "部门");
            if (strRootProjectCode === undefined || strRootProjectCode === "") {
                //运营信函

                me.fileCodePanel.setDocUnitClass("部门", "部门");
                //隐藏发给项目单选框
                //me.fileCodePanel.toProjectCheckBox.setVisible(false);


            } else {
                //项目（非运营）信函
                me.fileCodePanel.setDocUnitClass("项目", "项目");

            }

            //设置发文单位代码
            me.fileCodePanel.setSendCompany(sourceCompany, sourceCompanyDesc);

            //设置文件编码Panel的各个按钮的用户事件
            me.fileCodePanel.AfterSelectRecCompany = function (code, desc) {
                //me.deliveryUnitText.setValue(desc);
            }

            me.checkUploadFilesDefault = function () {
                // 1.判断是否有选择了文件，并勾选了唯一一个文件
                if (me.fileUploadPanel.FileUploadButton.uploader.uploader.files.length <= 0) {
                    //当没有附件时，处理返回事件
                    Ext.Msg.alert("错误信息", "请选择需要上传的文件！");
                    return false;
                }
                return true;
            }

            //机组按钮按下前的事件
            me.fileCodePanel.beforeSelectCrew = me.checkUploadFilesDefault;

            //厂房按钮按下前的事件
            me.fileCodePanel.beforeSelectFactory = me.checkUploadFilesDefault;

            //系统按钮按下前的事件
            me.fileCodePanel.beforeSelectSystem = me.checkUploadFilesDefault;

            //专业按钮按下前的事件
            me.fileCodePanel.beforeSelectProfession = me.checkUploadFilesDefault;

            //录入属性按钮按下前的事件
            me.fileCodePanel.beforeEditAttr = me.checkUploadFilesDefault;

            //录入属性后的事件
            me.fileCodePanel.afterEditAttr = function (res) {
                //Ext.Msg.alert("错误信息", "请选择需要上传的文件！");
                me.fileUploadPanel.filegrid.getView().refresh();
            }

            //隐藏发文编码文本框
            me.fileCodePanel.setFileCodeTextVisible(false);

            //设置第一个文件是正件
            //me.fileCodePanel.firstFileIsPositive = true;

            //me.fileCodePanel.setFirstFileIsPositive(true);

            me.fileCodePanel.fNumberText.setValue(strDocNumber);


            me.fileUploadPanel.setSendCompany(sourceCompany, sourceCompanyDesc);

            //设置项目管理类文件里项目的代码和描述
            me.fileUploadPanel.setRootProject(strRootProjectCode, strRootProjectDesc);

            //设置文件上传表格的模式
            me.fileUploadPanel.setFileGridMode("REC");

            //录入属性按钮按下前的事件
            me.fileUploadPanel.beforeEditAttr = function () {

                //如果是第一个文件，就默认勾选新建文件编码，并设置文件类型
                var grid = me.fileUploadPanel.filegrid;
                var rs = grid.getSelectionModel().getSelection();//获取选择的文档
                if (rs !== null && rs.length > 0) {
                    var rec = rs[0];//第一个文档

                    var store = grid.store;
                    var recored = store.getAt(0);
                    if (rec.data.id === recored.data.id) {

                        //是否新建文件编码
                        rec.set('needNewFileCode', true);
                        //文件编码类型
                        rec.set('receiveType', "AAA");

                        rec.commit();

                        //是否新建文件编码
                        recored.set('needNewFileCode', true);
                        //文件编码类型
                        recored.set('receiveType', "AAA");

                        recored.commit();
                    }
                }
                return true;
            }

            //录入属性后的事件
            me.fileUploadPanel.afterEditAttr = function (res) {
                var store = me.fileUploadPanel.filegrid.store;

                //如果是第一个文件，就更新表单文件编码栏的著录属性
                var recored = store.getAt(0);
                if (res.id === recored.data.id) {
                    me.fileCodePanel.updateCataAttr(res);
                }

            }

            var recobjLength = 0;
            //遍历来往单位数组，添加到来往单位combo
            for (var itemKey in me.recCompanyList) {
                //var strCompany = me.recCompanyList[itemKey];
                 //me.recCompanydata.push({ text: itemKey, value: itemKey });//在数组里添加新的元素  

                recobjLength = recobjLength + 1;

            }


            var sourceUnitIndex = -1;
            var sendobjLength = 0;
            var companyDesc = "";

            //遍历来往单位数组，添加到来往单位combo
            for (var itemKey in me.sendCompanyList) {
                
            //    me.sendCompanydata.push({ text: itemKey, value: itemKey });//在数组里添加新的元素  

            //    if (sourceCompany != undefined && itemKey === sourceCompany) {
                    sourceUnitIndex = sendobjLength;
                   // companyDesc = me.sendCompanyList[itemKey];
            //    }

            //    sendobjLength = sendobjLength + 1;

            }


            if (sendobjLength > 0 && sourceUnitIndex!=-1) {
               // me.sendCompanyCombo.setRawValue(me.sendCompanydata[sourceUnitIndex].text);//设置显示值
                //me.sendCompanyCombo.setValue(me.sendCompanydata[sourceUnitIndex].value); //设置ID值
                
                //me.senderText.setValue(companyDesc);
            }

            funCallback();
        }
    },

    //向服务器发送起草认质认价单请求
    send_draft_document: function () {
        var me = this;

        //0.用户选择所有需要上传的文件，并勾选正件

        //1.判断是否有选择了文件，并勾选了唯一一个文件

        //2.上传所有文件

        //3.获取勾选的文件

        //4.把勾选的文件的属性，修改为正件的属性

        //检查文件编码
        var checkResult = me.fileCodePanel.checkFileCodeFill();
        if (checkResult != "true") {
            Ext.Msg.alert("错误信息", checkResult);
            return;
        }

        if (me.nextStateUserList === undefined || me.nextStateUserList === "") {
            Ext.Msg.alert("错误信息", "请选择项目专工！");
            return;
        }

        if (me.contractNameText.value === undefined || me.contractNameText.value === "") {
            Ext.Msg.alert("错误信息", "请选择合同名称！");
            return;
        }

        if (me.contractCodeText.value === undefined || me.contractCodeText.value === "") {
            Ext.Msg.alert("错误信息", "请选择合同号！");
            return;
        }

        if (me.recCodeText.value === undefined || me.recCodeText.value === "") {
            Ext.Msg.alert("错误信息", "请选择来源文件编码！");
            return;
        }

       // 1.判断是否有选择了文件，并勾选了唯一一个文件
        if (me.fileUploadPanel.FileUploadButton.uploader.uploader.files.length > 0) {

            //var grid = me.fileUploadPanel.filegrid;

            //var rs = grid.getSelectionModel().getSelection();//获取选择的文档

            //if (rs === undefined || rs === null || rs.length <= 0) {

            //    Ext.Msg.alert("错误信息", "请勾选认质认价单文件！");
            //    return;
 
            //}
            //if (rs.length > 1) {

            //    Ext.Msg.alert("错误信息", "请勾选唯一的认质认价单文件！");
            //    return;

            //}
            ////2.上传所有文件
            me.fileUploadPanel.afterUploadAllFile = function () {
                me.set_positive_file();
            };

            me.fileUploadPanel.send_upload_file();
        } else {
            //当没有附件时，处理返回事件
            //me.set_positive_file();
            Ext.Msg.alert("错误信息", "请选择需要上传的文件！");
        }

    },

    set_positive_file:function(){
    
        var me = this;

        //3.获取勾选的文件
        var grid = me.fileUploadPanel.filegrid;

        var store = grid.store;//获取选择的文档

        if (store.getCount() > 0) {
            var rec = store.getAt(0);//第一个文档
            me.docKeyword = rec.data.docKeyword;
            me.docFileName = rec.data.name;
        }
        else {
            Ext.Msg.alert("错误信息", "设置认质认价单出错！");
            return;
        }
        //4.把勾选的文件的属性，修改为正件的属性

        //获取文件编码
        var fileCode = me.fileCodePanel.getFileCode();

        //获取文件ID
        var fileId = me.fileCodePanel.getFileId();

        //获取文件类型代码
        var docIdentifier = me.fileCodePanel.getDocIdentifier();

        //项目代码
        var fProjectCode = me.fProjectCodeText.value;
        //范围
        var area = me.areaText.value;
        //专业
        var profession = me.professionText.value;
        //来文类型 
        var receiveType = me.receiveTypeText.value;
        //文件流水号
        var fNumber = me.fNumberText.value;
        //版本
        var edition = me.editionText.value;

        //获取文件编码
        //var fileCode = fProjectCode + "-" + area + "-" + profession + "-" + receiveType + "-" + fNumber + "-" + edition;

        //获取工程名称
        var projectName = me.contractNameText.value;
        //合同号
        var contractCode = me.contractCodeText.value;

        //日期
        var sendDate = me.sendDateField.value;
        //编号
        var sendCode = "";// me.sendCodeText.value;

        //来文编号
        var recCode = me.recCodeText.value;
        //来文类型
        var recType = me.rectypeCombo.value;
        //采购类型 （材料或者设备）e:\qgy2017\cdmsweb\cdmsweb\hxepc_plugins\document\draftdocumentmenu.cs
        var materialType = me.materialtypeCombo.value;

      

        //获取表单数据，转换成JSON字符串
        var docAttr =
        [
            { name: 'fileCode', value: fileCode },
            { name: 'projectName', value: projectName },
            { name: 'contractCode', value: contractCode },
            { name: 'sendDate', value: sendDate },
            { name: 'sendCode', value: sendCode },
            { name: 'recCode', value: recCode },
            { name: 'recType', value: recType },
            { name: 'materialType', value: materialType },
            //{ name: 'fileId', value: fileId },
        ];

        var docAttrJson = Ext.JSON.encode(docAttr);
        var cataAttrJson = Ext.JSON.encode(me.fileCodePanel.cataAttrArray);

        Ext.MessageBox.wait("正在生成认质认价单，请稍候...", "等待");

        Ext.Ajax.request({

            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.HXEPC_Plugins.Document", A: "DraftRecognition",
                sid: localStorage.getItem("sid"), ProjectKeyword: me.projectKeyword,
                DocAttrJson: docAttrJson, CataAttrJson: cataAttrJson, DocKeyword: me.docKeyword,
                FileName: me.docFileName
            },
            success: function (response, options) {
                //me.draft_document_callback(response, options, "");//, me.projectKeyword, closeWin);

                //获取数据后，更新窗口
                var res = Ext.JSON.decode(response.responseText, true);
                var state = res.success;
                if (state === true) {

                    Ext.MessageBox.close();//关闭等待对话框

                    var recod = eval(res.data[0]);

                    //处理返回事件
                    //me.draft_document_callback(response, options, "");//, me.projectKeyword, closeWin);


                    me.docKeyword = recod.DocKeyword;//获取联系单文档id
                    //me.fileUploadPanel.docList = recod.DocList;//获取流程文档列表
                    me.newProjectKeyword = recod.ProjectKeyword;//获取新建的目录id

                    me.draft_document_callback(response, options, "");

                    //me.fileUploadPanel.docKeyword = me.docKeyword;

                    //if (me.fileUploadPanel.FileUploadButton.uploader.uploader.files.length > 0) {
                    //    //上传完所有文件后，刷新表单
                    //    me.fileUploadPanel.afterUploadAllFile = function () {
                    //        //me.refreshWin(me.projectKeyword, true);
                    //        me.draft_document_callback(response, options, "");
                    //    };

                    //    me.fileUploadPanel.send_upload_file();
                    //} else {
                    //    //当没有附件时，处理返回事件
                    //    me.draft_document_callback(response, options, "");
                    //}
                } else {
                    var errmsg = res.msg;
                    Ext.Msg.alert("错误信息", errmsg);
                }
            },
            failure: function (response, options) {
                //////Ext.Msg.alert("错误", "连接服务器失败！<br>" + response.responseText);
            }
        })
    },

    //处理发送起草函件后的返回事件
    draft_document_callback: function (response, options) {
        var me = this;

        var sendUnitCode = me.fileCodePanel.getSendCompanyCode();

        //获取审批路径Combo
        //var approvpath = me.approvpathCombo.value;

        Ext.MessageBox.wait("正在启动流程，请稍候...", "等待");

        Ext.Ajax.request({

            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.HXEPC_Plugins.Document", A: "RecognitionStartWorkFlow",
                sid: localStorage.getItem("sid"), docKeyword: me.docKeyword,
                docList: me.fileUploadPanel.docList, //ApprovPath: approvpath,
                UserList: me.nextStateUserList
            },
            success: function (response, options) {

                //获取数据后，更新窗口
                var res = Ext.JSON.decode(response.responseText, true);
                var state = res.success;
                if (state === true) {

                    Ext.MessageBox.close();//关闭等待对话框

                    var recod = eval(res.data[0]);
                    //me.refreshWin(recod.ProjectKeyword, true);
                    me.refreshWin(me.docKeyword, true);
                    
                } else {
                    var errmsg = res.msg;
                    Ext.Msg.alert("错误信息", errmsg);
                    winDraftRecognition.close();
                }
            },
            failure: function (response, options) {
                Ext.MessageBox.close();//关闭等待对话框
            }

        })
    },


    //获取流水号
    getRunNum: function () {
        var me = this;
        Ext.Ajax.request({
            //url: 'SHEPCPlugins/GetRunNum',
            url: 'WebApi/Post', 
            method: "POST",
            params: {
                C: "AVEVA.CDMS.HXEPC_Plugins.Document", A: "GetRecognitionNumber",
                //action: me.winAction,
                ProjectCode: me.projectCodeText.value,
                SendCompany: me.sendCompanyCombo.value, RecCompany: me.recCompanyCombo.value,
                //Profession:me.professionCombo.value,DocType:me.postCategoryText.value,
                sid: localStorage.getItem("sid")
            },
            success: function (response, options) {
                //获取数据后，更新窗口
                var res = Ext.JSON.decode(response.responseText, true);
                var state = res.success;
                if (state === true) {
                    var recod = eval(res.data[0]);
                    var runNum = recod.RunNum;//获取流水号
                    me.fNumberText.setValue(runNum);
                } else {
                    var errmsg = res.msg;
                    Ext.Msg.alert("错误信息", errmsg);
                }
            },
            failure: function (response, options) {
                //                Ext.Msg.alert("系统提示", "连接服务器失败，请尝试重新提交！");
                //Ext.MessageBox.close();//关闭等待对话框
            }
        });
    },

    //刷新表单，参数:parentKeyword:新建的联系单目录
    refreshWin: function (parentKeyword, closeWin) {
        var me = this;
        var tree = Ext.getCmp(me.mainPanelId).down('treepanel');
        var viewTreeStore = tree.store;

        viewTreeStore.load({
            callback: function (records, options, success) {//添加回调，获取子目录的文件数量
                if (closeWin)
                    winDraftRecognition.close();

                //展开目录
                Ext.require('Ext.ux.Common.comm', function () {
                    Ext.getCmp('contentPanel').down('_mainProjectTree').ExpendProject(parentKeyword);
                });
            }
        });
    }
});