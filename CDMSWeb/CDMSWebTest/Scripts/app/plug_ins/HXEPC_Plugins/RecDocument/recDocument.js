//收文处理
Ext.define('Ext.plug_ins.HXEPC_Plugins.RecDocument.recDocument', {
    extend: 'Ext.container.Container',
    alias: 'widget.recDocument',
    layout: 'fit',
    resultvalue: '', mainPanelId: '',
    projectKeyword: '',docKeyword: '',
    initComponent: function () {
        var me = this;

        me.draftmanCode = "";
        me.docType = "";

        //收发文单位列表初始数据
        me.recCompanyList = [];
        me.sendCompanyList = [];

        //定义发文单位combo初始数据
        me.sendCompanydata = [];

        //定义收文单位combo初始数据
        me.recCompanydata = [];

        //是否需要回复combo初始数据
        me.needreplydata = [{ text: "是", value: "是" }, { text: "否", value: "否" }];

        //紧急程度combo初始数据
        me.urgencydata = [{ text: "一般", value: "一般" }, { text: "紧急", value: "紧急" }];

        //添加项目号text
        me.projectCodeText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "收文编码", labelWidth: 80, readOnly: true, fieldStyle: ' background-color: #DFE9F6;border-color: #DFE9F6; background-image: none;',//fieldStyle:'color:red',
            margin: '10 10 0 10', anchor: "80%", labelAlign: "right", width: 170//flex: 1
        });

        //添加项目号text
        me.ProjCodeText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "项目号", labelWidth: 80, readOnly: true, fieldStyle: 'background-color: #DFE9F6;border-color: #DFE9F6; background-image: none;',
            margin: '10 0 0 10', anchor: "80%", labelAlign: "right", width: "50%"//flex: 1
        });

        //添加项目号text
        me.ProjectDescText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "项目名称", labelWidth: 80, readOnly: true, fieldStyle: 'background-color: #DFE9F6;border-color: #DFE9F6; background-image: none;',
            margin: '10 10 0 10', anchor: "80%", labelAlign: "right", width: "50%"//flex: 1
        });

        //添加来文单位text
        me.CommUnitText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "来文单位", anchor: "80%", labelWidth: 80, labelAlign: "right", labelPad: 8, width: "50%",//width: 230, 
            margin: '10 10 0 10'
        });

        //添加收文编码text
        me.ReceiptCodeText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "收文编码", anchor: "80%", labelWidth: 80, labelAlign: "right", labelPad: 8, width: "50%",//width: 230, 
            margin: '10 10 0 10'
        });

        //添加收文编号text
        me.ReceiptNumberText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "收文ID", anchor: "80%", labelWidth: 80, labelAlign: "right", labelPad: 8, width: "50%",//width: 230, 
            margin: '10 10 0 10'
        });


        //定义文件编码Text
        me.FileCodeText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "原文件编码", anchor: "80%", labelWidth: 80, labelAlign: "right", labelPad: 8, width: "50%",//width: 230, 
            margin: '10 10 0 10'
        });

        //定义文件题名Text
        me.TitleText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "文件题名", anchor: "80%", labelWidth: 80, labelAlign: "right", labelPad: 8, width: "50%",//width: 230, 
            margin: '10 10 0 10'
        });

        //定义页数Text
        me.totalPagesText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "页数", anchor: "80%", labelWidth: 80, labelAlign: "right", labelPad: 8, width: "50%",//width: 230, 
            margin: '10 10 0 10'
        });

        //添加我方发文编码text
        me.SendCodeText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "我方发文编码", anchor: "80%", labelWidth: 80, labelAlign: "right", labelPad: 8, width: "50%",//width: 230, 
            margin: '10 10 0 10'
        });

        //添加函件流水号text
        me.numberText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: " - ", labelWidth: 35, labelSeparator: '', labelPad: 8, // 去掉laebl中的冒号
            anchor: "80%", labelAlign: "right", width: 110, margin: '10 0 0 10',
        });

        //添加函件编号text
        me.DocCodeText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "函件编号", labelWidth: 80, labelSeparator: '', // 去掉laebl中的冒号
            anchor: "80%", labelAlign: "right", width: "100%"//flex: 1
        });

        //添加备注Text
        me.remarkText = Ext.create("Ext.form.field.TextArea", {
            xtype: "textarea", anchor: "80%", labelAlign: "right", labelPad: 8, margin: '10 10 0 10', //margin: '0 5 5 0',
            width: "100%",//flex:1, //width: 460, //
            height: 85, fieldLabel: "备注", labelWidth: 80
        });

        //添加著录人（起草人）text
        me.draftmanText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "著录人", labelWidth: 80, labelSeparator: '', // 去掉laebl中的冒号
            margin: '10 10 0 10', anchor: "80%", labelAlign: "right", width: "50%"//flex: 1
        });

        //定义收文日期Text
        me.recDateField = Ext.create("Ext.form.field.Date", {

            name: "date",
            fieldLabel: ' 收文日期', fieldStyle: ' background-image: none;',
            editable: true, labelWidth: 80, margin: '10 10 0 10',
            labelAlign: "right", labelPad: 8,
            emptyText: "--请选择--", autoLoadOnValue: true,
            format: 'Y年m月d日',
            value: new Date(),
            width: '50%'//width: 230
        });

        //定义回文期限Text
        me.replyDateField = Ext.create("Ext.form.field.Date", {

            name: "date",
            fieldLabel: ' 回文期限', fieldStyle: ' background-image: none;',
            editable: true, labelWidth: 80, margin: '10 10 0 10',
            labelAlign: "right", labelPad: 8,
            emptyText: "--请选择--", autoLoadOnValue: true,
            format: 'Y年m月d日',
            value: new Date(),
            width: '50%' //width: 230
        });

        //添加是否需要回复combo
        Ext.define("needreplyModel", {
            extend: 'Ext.data.Model',
            fields: ["text", "value"]
        });
        me.needreplyProxy = Ext.create("Ext.data.proxy.Memory", {
            data: me.needreplydata,
            model: "needreplyModel"
        });

        me.needreplyStore = Ext.create("Ext.data.Store", {
            model: needreplyModel,
            proxy: me.needreplyProxy
        });


        me.needreplyCombo = Ext.create("Ext.form.field.ComboBox",
        {
            //xtype: "combo",
            fieldLabel: '是否要求回文', labelWidth: 80,
            triggerAction: "all", store: me.needreplyStore,
            valueField: 'value', editable: false,//不可输入
            displayField: 'text', margin: '10 5 0 10',// 
            anchor: "80%", labelAlign: "right", labelPad: 8, width: '50%',// width: 120,//
            emptyText: "--请选择--", autoLoadOnValue: true, value: "是",
            //fieldStyle: 'border-color: red; background-image: none;',//红色边框
            listeners:
            {
                select: function (combo, records, eOpts) {
                    var seculevel = me.needreplyCombo.value;
                    if (seculevel === "否") {
                        me.replyDateField.setDisabled(true);
                    } else {
                        me.replyDateField.setDisabled(false);
                    }
                }
            }
        });

        //添加紧急程度combo
        Ext.define("urgencyModel", {
            extend: 'Ext.data.Model',
            fields: ["text", "value"]
        });
        me.urgencyProxy = Ext.create("Ext.data.proxy.Memory", {
            data: me.urgencydata,
            model: "urgencyModel"
        });

        me.urgencyStore = Ext.create("Ext.data.Store", {
            model: urgencyModel,
            proxy: me.urgencyProxy
        });


        me.urgencyCombo = Ext.create("Ext.form.field.ComboBox",
        {
            //xtype: "combo",
            fieldLabel: '紧急程度', labelWidth: 80,
            triggerAction: "all", store: me.urgencyStore,
            valueField: 'value', editable: false,//不可输入
            displayField: 'text', margin: '10 10 0 10',// 
            anchor: "80%", labelAlign: "right", labelPad: 8, width: '50%',//width: 120,//
            emptyText: "--请选择--", autoLoadOnValue: true, value: "一般",
            //fieldStyle: 'border-color: red; background-image: none;',//红色边框
            listeners:
            {
                select: function (combo, records, eOpts) {

                }
            }
        });

        //添加发文单位combo
        Ext.define("sendCompanyModel", {
            extend: 'Ext.data.Model',
            fields: ["text", "value"]
        });
        me.sendCompanyProxy = Ext.create("Ext.data.proxy.Memory", {
            data: me.sendCompanydata,
            model: "sendCompanyModel"
        });

        me.sendCompanyStore = Ext.create("Ext.data.Store", {
            model: sendCompanyModel,
            proxy: me.sendCompanyProxy
        });


        me.sendCompanyCombo = Ext.create("Ext.form.field.ComboBox",
        {
            //xtype: "combo",
            fieldLabel: ' - ', labelWidth: 10, labelSeparator: '', // 去掉laebl中的冒号
            triggerAction: "all", store: me.sendCompanyStore,
            valueField: 'value', editable: false,//不可输入
            displayField: 'text', margin: '10 0 0 0',// 
            anchor: "80%", labelAlign: "left", width: 110,//
            emptyText: "--请选择--", autoLoadOnValue: true,
            listeners:
            {
                select: function (combo, records, eOpts) {
                    var companyCode = combo.value;
                    if (companyCode != undefined) {
                        ////遍历来往单位数组，查找接收单位描述
                        for (var itemKey in me.sendCompanyList) {
                            if (itemKey === companyCode) {
                                var companyDesc = me.sendCompanyList[itemKey];
                              //  me.senderText.setValue(companyDesc);
                            }
                        }
                    }

                    //获取流水号
                    me.getRunNum();
                }
            }
        });


        //添加收文单位combo
        Ext.define("recCompanyModel", {
            extend: 'Ext.data.Model',
            fields: ["text", "value"]
        });
        me.recCompanyProxy = Ext.create("Ext.data.proxy.Memory", {
            data: me.recCompanydata,
            model: "recCompanyModel"
        });

        me.recCompanyStore = Ext.create("Ext.data.Store", {
            model: recCompanyModel,
            proxy: me.recCompanyProxy
        });


        me.recCompanyCombo = Ext.create("Ext.form.field.ComboBox",
        {
            //xtype: "combo",
            fieldLabel: ' - ', labelWidth: 10, labelSeparator: '', // 去掉laebl中的冒号
            triggerAction: "all", store: me.recCompanyStore,
            valueField: 'value', editable: false,//不可输入
            displayField: 'text', margin: '10 0 0 10',// 
            anchor: "80%", labelAlign: "left", width: 110,//
            emptyText: "--请选择--", autoLoadOnValue: true,
            listeners:
            {
                select: function (combo, records, eOpts) {

                    var companyCode = combo.value;
                    if (companyCode != undefined) {
                        ////遍历来往单位数组，查找接收单位描述
                        for (var itemKey in me.recCompanyList) {
                            if (itemKey === companyCode) {
                                var companyDesc = me.recCompanyList[itemKey];
                                //me.mainFeederText.setValue(companyDesc);
                            }
                        }
                    }

                    //获取流水号
                    me.getRunNum();
                }
            }
        });

        //选择发文单位按钮
        me.sendCompanyButton = Ext.create("Ext.button.Button", {
            text: "..", margins: "10 0 0 0",
            listeners: {
                "click": function (btn, e, eOpts) {//添加点击按钮事件
                    //if (me.docClass === "operation") {
                    //    //运营管理类，选择项目部门
                    //    me.selectSendDepartment();
                    //} else {
                        me.selectSendUnit();
                    //}

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
                                  text: '收文信息著录表', margins: '8 0 0 10'
                              }, { baseCls: 'my-panel-no-border', flex: 1 }]
                          },
                          {
                              xtype: "fieldset", margin: '0 5 16 5',
                              baseCls: 'my-panel-no-border',//隐藏边框
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
                            me.projectCodeText,
                            me.sendCompanyCombo, me.sendCompanyButton,
                            me.recCompanyCombo, me.recCompanyButton,
                            me.numberText
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
                                      items: [
                                          //me.ProjCodeText,
                                          me.ReceiptNumberText,
                                          me.ProjectDescText
                                         ]
                                  }
                                  ,
                                  {
                                      layout: "hbox",
                                      width: '100%',
                                      align: 'stretch',
                                      pack: 'start',
                                      baseCls: 'my-panel-no-border',//隐藏边框
                                      items: [
                                           me.CommUnitText,
                                           me.recDateField
                                      ]
                                  }
                                  ,
                                  //{
                                  //    layout: "hbox",
                                  //    width: '100%',
                                  //    align: 'stretch',
                                  //    pack: 'start',
                                  //    baseCls: 'my-panel-no-border',//隐藏边框
                                  //    items: [
                                  //        // me.ReceiptCodeText,
                                  //         me.ReceiptNumberText
                                  //    ]
                                  //}
                                  //,
                                  {
                                      layout: "hbox",
                                      width: '100%',
                                      align: 'stretch',
                                      pack: 'start',
                                      baseCls: 'my-panel-no-border',//隐藏边框
                                      items: [
                                            me.FileCodeText,
                                            me.TitleText
                                      ]
                                  }
                                  ,
                                  {
                                      layout: "hbox",
                                      width: '100%',
                                      align: 'stretch',
                                      pack: 'start',
                                      baseCls: 'my-panel-no-border',//隐藏边框
                                      items: [
                                            me.totalPagesText,
                                            me.SendCodeText 
                                      ]
                                  }
                                  ,
                                  {
                                      layout: "hbox",
                                      width: '100%',
                                      align: 'stretch',
                                      pack: 'start',
                                      baseCls: 'my-panel-no-border',//隐藏边框
                                      items: [
                                            me.needreplyCombo,
                                            me.replyDateField
                                      ]
                                  }
                                   ,
                                  {
                                      layout: "hbox",
                                      width: '100%',
                                      align: 'stretch',
                                      pack: 'start',
                                      baseCls: 'my-panel-no-border',//隐藏边框
                                      items: [
                                            me.urgencyCombo,
                                            me.draftmanText
                                      ]
                                  }
                                   ,
                                 {
                                     layout: "hbox",
                                     width: '100%',
                                     align: 'stretch',
                                     pack: 'start',
                                     height: 95, margin: '0 0 0 0',
                                     baseCls: 'my-panel-no-border',//隐藏边框
                                     items: [
                                          me.remarkText  // 备注
                                     ]//, flex: 1
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
                              text: "确定", width: 60, margins: "0 5 10 5",
                              listeners: {
                                  "click": function (btn, e, eOpts) {//添加点击按钮事件
                                      me.send_ReceiveDocument();

                                  }
                              }
                          },
                          {
                              xtype: "button",
                              text: "取消", width: 60, margins: "0 15 10 5",
                              listeners: {
                                  "click": function (btn, e, eOpts) {//添加点击按钮事件
                                      //Ext.Msg.alert("您展开了目录树节点！！！", "您展开了目录树节点！节点ID:" + me.tempDefnId);
                                      winRecDocument.close();
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
        me.sendGetReceiveDocumentDefault();


me.callParent(arguments);
    },

    //获取打开表单时的默认参数
    sendGetReceiveDocumentDefault: function () {
        var me = this;

        //通过extjs的ajax获取操作全部名称
        Ext.Ajax.request({
            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.HXEPC_Plugins.Document", A: "GetReceiveDocumentDefault",
                sid: localStorage.getItem("sid"), DocKeyword: me.docKeyword
            },
            success: function (response, options) {
                me.sendGetReceiveDocumentDefault_callback(response, options);//, funCallback);

            }
        });

    },

    //处理获取收文处理表单默认参数的返回
    sendGetReceiveDocumentDefault_callback: function (response, options) {
        var me = this;

        //获取数据后，更新窗口
        var res = Ext.JSON.decode(response.responseText, true);
        var state = res.success;
        if (state === true) {
            var recod = eval(res.data[0]);

            var strRootProjectCode = recod.RootProjectCode;
            var strRootProjectDesc = recod.RootProjectDesc;
            var strDocNumber = recod.DocNumber;
            var strCommUnit = recod.CommUnit;
            var strDraftmanCode = recod.DraftmanCode;
            var strDraftmanDesc = recod.DraftmanDesc;
            var strRecCode = recod.RecCode;
            var strRecNumber = recod.RecNumber;
            me.docType = recod.DocType;
            me.recCompanyList = eval(recod.RecCompanyList);
            me.sendCompanyList = eval(recod.SendCompanyList);

            me.ProjCodeText.setValue(strRootProjectCode);
            me.projectCodeText.setValue(strRootProjectCode);
            me.ProjectDescText.setValue(strRootProjectDesc);
            me.CommUnitText.setValue(strCommUnit);
            me.draftmanText.setValue(strDraftmanDesc);
            me.DraftmanCode = strDraftmanCode;
            me.FileCodeText.setValue(strRecCode)//原文件编码（对方发文编号）
            me.ReceiptNumberText.setValue(strRecNumber);//收文编号
            me.numberText.setFieldLabel(" -" + me.docType + "- ");

            var recobjLength = 0;
            //遍历来往单位数组，添加到来往单位combo
            for (var itemKey in me.recCompanyList) {
                var strCompany = me.recCompanyList[itemKey];
 
                me.recCompanydata.push({ text: itemKey, value: itemKey });//在数组里添加新的元素  

                recobjLength = recobjLength + 1;

            }

            if (recobjLength > 0) {
                me.recCompanyCombo.setRawValue(me.recCompanydata[0].text);//设置显示值
                me.recCompanyCombo.setValue(me.recCompanydata[0].value); //设置ID值

            }

            var sourceUnitIndex = -1;
            var sendobjLength = 0;
            var companyDesc = "";

            //遍历来往单位数组，添加到来往单位combo
            for (var itemKey in me.sendCompanyList) {

                me.sendCompanydata.push({ text: itemKey, value: itemKey });//在数组里添加新的元素  

                //if (sourceCompany != undefined && itemKey === sourceCompany) {
                //    sourceUnitIndex = sendobjLength;
                //    companyDesc = me.sendCompanyList[itemKey];
                //}
                if (sendobjLength === 0) {
                    companyDesc = me.sendCompanyList[itemKey];
                }
                sendobjLength = sendobjLength + 1;

            }


            if (sendobjLength > 0 ) {//&& sourceUnitIndex != -1) {
                me.sendCompanyCombo.setRawValue(me.sendCompanydata[0].text);//设置显示值
                me.sendCompanyCombo.setValue(me.sendCompanydata[0].value); //设置ID值

                me.CommUnitText.setValue(companyDesc);
            }

            me.getRunNum();
        }
    },

    //响应收文处理表单的确定按钮
    send_ReceiveDocument: function () {
        var me = this;

        //获取项目号Text
        var strProjectCode = me.ProjCodeText.value;

        //获取项目名称Text
        var strProjectDesc = me.ProjectDescText.value;

        //获取来文单位
        var strCommUnit = me.CommUnitText.value;

        //获取收文日期
        var strRecDate = me.recDateField.value;

        //获取收文编码
        var strRecCode = me.projectCodeText.value + "-" + me.sendCompanyCombo.value + "-" +
            me.recCompanyCombo.value + "-" + me.docType + "-" + me.numberText.value;
            //me.ReceiptCodeText.value;

        //获取收文编号
        var strRecNumber = me.ReceiptNumberText.value;

        //文件编码
        var strFileCode = me.FileCodeText.value;

        //文件题名
        var strTitle = me.TitleText.value;

        //获取页数
        var strPages = me.totalPagesText.value;

        ///获取我方发文编码
        var strSendCode = me.SendCodeText.value;

        ///获取是否要求回文
        var strNeedReply = me.needreplyCombo.value;

        //获取回文期限
        var strReplyDate = me.replyDateField.value;

        //获取紧急程度
        var strUrgency = me.urgencyCombo.value;

        //获取备注
        var strRemark = me.remarkText.value;

        //获取表单数据，转换成JSON字符串
        var docAttr =
        [
            { name: 'ProjectCode', value: strProjectCode },
            { name: 'ProjectDesc', value: strProjectDesc },
            { name: 'CommUnit', value: strCommUnit },
            { name: 'RecDate', value: strRecDate },
            { name: 'RecCode', value: strRecCode },
            { name: 'RecNumber', value: strRecNumber },
            { name: 'FileCode', value: strFileCode },
            { name: 'Title', value: strTitle },
            { name: 'Pages', value: strPages },
            { name: 'SendCode', value: strSendCode },
            { name: 'NeedReply', value: strNeedReply },
            { name: 'ReplyDate', value: strReplyDate },
            { name: 'Urgency', value: strUrgency },
            { name: 'Remark', value: strRemark },
        ]

        var docAttrJson = Ext.JSON.encode(docAttr);

        Ext.MessageBox.wait("正在创建收文处理单，请稍候...", "等待");

        Ext.Ajax.request({

            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.HXEPC_Plugins.Document", A: "ReceiveDocument",
                sid: localStorage.getItem("sid"), DocKeyword: me.docKeyword,
                docAttrJson: docAttrJson
            },
            success: function (response, options) {

                //获取数据后，更新窗口
                var res = Ext.JSON.decode(response.responseText, true);
                var state = res.success;
                if (state === true) {

                    Ext.MessageBox.close();//关闭等待对话框

                    var recod = eval(res.data[0]);

                    me.projectKeyword = recod.ProjectKeyword;//获取新建的目录id


                         //处理返回事件
                    me.send_receiveDocument_callback(response, options, "");//, me.projectKeyword, closeWin);

                } else {
                    var errmsg = res.msg;
                    Ext.Msg.alert("错误信息", errmsg);
                }
            }

        })
    },

    send_receiveDocument_callback: function (response, options) {
        var me = this;
        me.refreshWin(me.projectKeyword, true);
    },

    //获取流水号
    getRunNum: function () {
        var me = this;
        Ext.Ajax.request({
            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.HXEPC_Plugins.Document", A: "GetRecDocumentNumber",
                ProjectCode: me.projectCodeText.value,
                SendCompany: me.sendCompanyCombo.value, RecCompany: me.recCompanyCombo.value,
                DocType:'LET', sid: localStorage.getItem("sid")
            },
            success: function (response, options) {
                //获取数据后，更新窗口
                var res = Ext.JSON.decode(response.responseText, true);
                var state = res.success;
                if (state === true) {
                    var recod = eval(res.data[0]);
                    var runNum = recod.RunNum;//获取流水号
                    me.numberText.setValue(runNum);
                } else {
                    var errmsg = res.msg;
                    Ext.Msg.alert("错误信息", errmsg);
                }
            },
            failure: function (response, options) {
                //                Ext.Msg.alert("系统提示", "连接服务器失败，请尝试重新提交！");
            }
        });
    },

    selectSendUnit: function () {
        var me = this;

        var fmSelectUnit = Ext.create('Ext.plug_ins.HXEPC_Plugins.Document.SelectUnit', { title: "", mainPanelId: me.Id, projectKeyword: me.projectKeyword });

        winSelectUnit = Ext.widget('window', {
            title: '选择发文单位',
            width: 738,
            height: 558,
            minWidth: 738,
            minHeight: 558,
            layout: 'fit',
            resizable: true,
            modal: true,
            closeAction: 'close', //close 关闭  hide  隐藏  
            items: fmSelectUnit,
            defaultFocus: 'firstName'
        });

        fmSelectUnit.projectKeyword = me.projectKeyword;

        winSelectUnit.show();


        //监听子窗口关闭事件
        winSelectUnit.on('close', function () {
            if (window.parent.resultvalue != null && window.parent.resultvalue !== "") {

                var unitCode = "";
                var unitDesc = "";
                var unitValue = "";

                unitCode = window.parent.resultvalue;
                unitDesc = window.parent.unitdesclist;
                unitValue = window.parent.unitvaluelist;

                if (unitCode.indexOf(",") > 0) {
                    // var words = unitCode.split(',')
                    unitCode = unitCode.substring(0, unitCode.indexOf(","));
                    unitDesc = unitDesc.substring(0, unitDesc.indexOf(";"));
                }


                me.CommUnitText.setValue(unitDesc);

                //me.sendCompanyText.setValue(unitCode);
                me.sendCompanyCombo.setRawValue(unitCode);//设置显示值
                me.sendCompanyCombo.setValue(unitCode); //设置ID值

                me.getRunNum();
            }
        });
    },

    //刷新表单，参数:parentKeyword:新建的联系单目录
    refreshWin: function (projKeyword, closeWin) {
        var me = this;
        var tree = Ext.getCmp(me.mainPanelId).up('_mainSourceView').down('_mainProjectTree').down('treepanel');//;
        var viewTreeStore = tree.store;

        viewTreeStore.load({
            callback: function (records, options, success) {//添加回调，获取子目录的文件数量
                if (closeWin)
                    winRecDocument.close();

                //展开目录
                Ext.require('Ext.ux.Common.comm', function () {
                    Ext.getCmp('contentPanel').down('_mainProjectTree').ExpendProject(projKeyword);
                });
            }
        });
    }

});