//新建目录
Ext.define('Ext.plug_ins.ZHEPC_Plugins.Report.winCreateC3', {
    extend: 'Ext.container.Container',
    alias: 'widget.winCreateC3',
    //layout: "border",
    layout: 'fit',
    resultvalue: '', mainPanelId: '',
    winWidth: 780,
    winHeight: 580,
    initComponent: function () {
        var me = this;
       
        //定义文件编码Panel
        me.fileCodePanel = Ext.create('Ext.plug_ins.ZHEPC_Plugins.Dist.FileCodePanel');

        //定义文件上传Panel
        me.fileUploadPanel = Ext.create('Ext.plug_ins.ZHEPC_Plugins.Dist.FileUploadPanel', {
            projectKeyword: me.projectKeyword
        });

        //设置上传控件为附件模式
        me.fileUploadPanel.setAttaMode();

        me.fileUploadPanel.setGridMinHeight(150);

        //添加卷册名称text
        me.titleText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldStyle: 'border-color: red; background-image: none;',
            fieldLabel: "卷册名称", anchor: "80%", labelWidth: 60, labelAlign: "left", margin: '0 25 0 0', flex: 1//width: 210//
        });

        //添加图号text
        me.titleText2 = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldStyle: 'border-color: red; background-image: none;',
            fieldLabel: "图号", anchor: "80%", labelWidth: 60, labelAlign: "left", margin: '0 5 0 0', flex: 1// width: 210//
        });

        //添加提出专业text
        me.titleText3 = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldStyle: 'border-color: red; background-image: none;',
            fieldLabel: "提出专业", anchor: "80%", labelWidth: 60, labelAlign: "left", margin: '0 25 0 0', flex: 1// width: 210//
        });

        //添加提出时间日期控件
        me.toDateField = Ext.create("Ext.form.field.Date", {
            xtype: 'datefield',
            anchor: '100%',
            fieldLabel: '提出时间',// fieldStyle: 'border-color: red; background-image: none;',
            labelWidth: 60, labelAlign: "left", margin: '0 5 0 0',
            format: 'Y年m月d日',
            name: 'to_date',flex: 1,//
            value: new Date()  // defaults to today
        });

        //添加修改原因text
        me.titleText4 = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldStyle: 'border-color: red; background-image: none;',
            fieldLabel: "修改原因", anchor: "80%", labelWidth: 60, labelAlign: "left", margin: '0 5 0 0', flex: 1// width: 210//
        });

        //添加主要工程量增减与费用估算text
        me.titleText5 = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", //fieldStyle: 'border-color: red; background-image: none;',
            fieldLabel: "主要工程量增减与费用估算", anchor: "80%", labelWidth: 160, labelAlign: "left", margin: '0 0 0 0', flex: 1// width: 210//
        });

        me.select1CheckBox = Ext.create("Ext.form.field.Checkbox", {
            fieldLabel: "", margin: '2 5 0 10', labelWidth: 0, 
            boxLabel: "设计改进"
        });

        me.select2CheckBox = Ext.create("Ext.form.field.Checkbox", {
            fieldLabel: "", margin: '2 5 0 10', labelWidth: 0, 
            boxLabel: "设计错误"
        });

        me.select3CheckBox = Ext.create("Ext.form.field.Checkbox", {
            fieldLabel: "", margin: '2 5 0 10', labelWidth: 0, 
            boxLabel: "设计漏项"
        });

        me.select4CheckBox = Ext.create("Ext.form.field.Checkbox", {
            fieldLabel: "", margin: '2 5 0 10', labelWidth: 0, 
            boxLabel: "设备材料选择不当"
        });

        me.select5CheckBox = Ext.create("Ext.form.field.Checkbox", {
            fieldLabel: "", margin: '2 5 0 10', labelWidth: 0, 
            boxLabel: "专业配合不佳"
        });

        me.select6CheckBox = Ext.create("Ext.form.field.Checkbox", {
            fieldLabel: "", margin: '5 5 0 10', labelWidth: 0, 
            boxLabel: "施工运行不便"
        });

        me.select7CheckBox = Ext.create("Ext.form.field.Checkbox", {
            fieldLabel: "", margin: '5 5 0 10', labelWidth: 0, 
            boxLabel: "工艺资料修改"
        });

        me.select8CheckBox = Ext.create("Ext.form.field.Checkbox", {
            fieldLabel: "", margin: '5 5 0 10', labelWidth: 0, 
            boxLabel: "厂家资料变动"
        });

        me.select9CheckBox = Ext.create("Ext.form.field.Checkbox", {
            fieldLabel: "", margin: '5 5 0 10', labelWidth: 0, 
            boxLabel: "其它"
        });

        //添加内容Text
        me.contentText = Ext.create("Ext.form.field.TextArea", {
            xtype: "textarea", anchor: "80%", labelWidth: 60, labelAlign: "left", margin: '8 0 8 0', width: 360,
            fieldLabel: "修改内容", height: 75
        });
       
        me.naturePanel = Ext.create("Ext.panel.Panel", {
            //xtype: "panel",
            layout: "hbox",
            baseCls: 'my-panel-no-border',//隐藏边框
            items: [
                {
                    xtype: 'label',
                    text: '修改性质:', margin: '7 0 0 0',
                    width:60
                },
                {
                    layout: "vbox",
                    width: '100%', baseCls: 'my-panel-no-border',//隐藏边框
                    align: 'stretch', margin: '0 0 0 0', padding: '0 0 0 0',
                    pack: 'start', flex: 1,
                    items: [
                        {
                            layout: "hbox",
                            width: '100%', baseCls: 'my-panel-no-border',//隐藏边框
                            align: 'stretch', margin: '0 0 0 0', padding: '0 0 0 0',
                            pack: 'start', flex: 1,
                            items: [
                                me.select1CheckBox,
                                me.select2CheckBox,
                                me.select3CheckBox,
                                me.select4CheckBox,
                                me.select5CheckBox
                            ]
                        },
                        {
                            layout: "hbox",
                            width: '100%', baseCls: 'my-panel-no-border',//隐藏边框
                            align: 'stretch', margin: '0 0 0 0', padding: '0 0 0 0',
                            pack: 'start', flex: 1,
                            items: [
                                me.select6CheckBox,
                                me.select7CheckBox,
                                me.select8CheckBox,
                                me.select9CheckBox
                            ]
                        }
                    ]
                }
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
                          text: 'C.3设计变更通知单报检表', margins: '0 0 0 10'
                      }, { baseCls: 'my-panel-no-border', flex: 1 }]
                      }
                  ,
                  {//发文编号一栏
                      xtype: "fieldset", margin: '5 8 8 8',
                      layout: {
                          type: 'vbox',
                          pack: 'start',
                          align: 'stretch'
                      },
                      items: [
                    me.fileCodePanel,
                      {
                          layout: "hbox",
                          width: '100%', baseCls: 'my-panel-no-border',//隐藏边框
                          align: 'stretch', margin: '8 0 5 0', padding: '0 0 0 0',
                          pack: 'start',
                          items: [
                              me.titleText,
                              me.titleText2
                          ]
                      },
                      me.naturePanel,   //修改性质
                      {
                          layout: "hbox",
                          width: '100%', baseCls: 'my-panel-no-border',//隐藏边框
                          align: 'stretch', margin: '8 0 8 0', padding: '0 0 0 0',
                          pack: 'start',
                          items: [
                              me.titleText3,
                              me.toDateField
                          ]
                      },
                      me.titleText4,
                      me.contentText,
                      me.titleText5,
                        me.fileUploadPanel

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
                          text: "确定", width: 60, margins: "2 5 10 5",
                          listeners: {
                              "click": function (btn, e, eOpts) {//添加点击按钮事件
                                  me.send_create_C3();
                              }
                          }
                      },
                      {
                          xtype: "button",
                          text: "取消", width: 60, margins: "2 15 10 5",
                          listeners: {
                              "click": function (btn, e, eOpts) {//添加点击按钮事件
                                  //Ext.Msg.alert("您展开了目录树节点！！！", "您展开了目录树节点！节点ID:" + me.tempDefnId);
                                  winC3.close();
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

        //设置发文单位
        var nodes = Ext.getCmp(me.mainPanelId).down('treepanel').getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {

            me.projectKeyword = nodes[0].data.Keyword;//定义本页面全局变量
            var projKeyword = nodes[0].data.Keyword;

            me.fileCodePanel.projectKeyword = me.projectKeyword;
            me.fileCodePanel.documentTypeText.setValue("C.3");
            me.fileCodePanel.getFileCodeDefaultInfo();

        }
    },

    //创建C3联系单
    send_create_C3: function () {
        var me = this;

        //检查文件编码
        var checkResult = me.fileCodePanel.checkFileCodeFill();
        if (checkResult != "true") {
            Ext.Msg.alert("错误信息", checkResult);
            return;
        }

        var fileCode = me.fileCodePanel.getFileCode();

        var title = me.titleText.value === undefined ? "" : me.titleText.value;
        if (title === "") {
            Ext.Msg.alert("错误信息", "请填写工程名称！");
            return;
        }

        if (title.indexOf("&") > 0) {
            Ext.Msg.alert("系统提示", "文件名称中包含非法字符‘&’！");
            return;
        }

        var projKeyword = me.projectKeyword;

        var TEXT2 = me.titleText2.value === undefined ? "" : me.titleText2.value;   //图号
        var TEXT3 = me.titleText3.value === undefined ? "" : me.titleText3.value;   //提出专业
        var TEXT4 = me.toDateField.value === undefined ? "" : me.toDateField.value; //提出时间
        var TEXT5 = me.titleText4.value === undefined ? "" : me.titleText4.value;   //修改原因
        var TEXT6 = me.titleText5.value === undefined ? "" : me.titleText5.value;   //估算

        var CONTENT = me.contentText.value === undefined ? "" : me.contentText.value;   //修改内容

        var select1 = me.select1CheckBox.checked;
        var select2 = me.select2CheckBox.checked;
        var select3 = me.select3CheckBox.checked;
        var select4 = me.select4CheckBox.checked;
        var select5 = me.select5CheckBox.checked;
        var select6 = me.select6CheckBox.checked;
        var select7 = me.select7CheckBox.checked;
        var select8 = me.select8CheckBox.checked;
        var select9 = me.select9CheckBox.checked;

        var docAttr =
            [
                { name: 'TEXT1', value: title },
                { name: 'TEXT2', value: TEXT2 },
                { name: 'TEXT3', value: TEXT3 },
                { name: 'TEXT4', value: TEXT4 },
                { name: 'TEXT5', value: TEXT5 },
                { name: 'TEXT6', value: TEXT6 },

                { name: 'CONTENT', value: CONTENT },

                { name: 'select1', value: select1 },
                { name: 'select2', value: select2 },
                { name: 'select3', value: select3 },
                { name: 'select4', value: select4 },
                { name: 'select5', value: select5 },
                { name: 'select6', value: select6 },
                { name: 'select7', value: select7 },
                { name: 'select8', value: select8 },
                { name: 'select9', value: select9 }

            ];
        var docAttrJson = Ext.JSON.encode(docAttr);

        Ext.require('Ext.plug_ins.ZHEPC_Plugins.Dist.ReportComm', function () {
            var mw = Ext.create('Ext.plug_ins.ZHEPC_Plugins.Dist.ReportComm');
            mw.winByName = winC3;
            mw.createDocument(me, 'C.3', projKeyword, fileCode, title, docAttrJson);
        });
    },

});