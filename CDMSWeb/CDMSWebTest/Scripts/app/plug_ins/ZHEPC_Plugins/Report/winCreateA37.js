//新建目录
Ext.define('Ext.plug_ins.ZHEPC_Plugins.Report.winCreateA37', {
    extend: 'Ext.container.Container',
    alias: 'widget.winCreateA37',
    //layout: "border",
    layout: 'fit',
    resultvalue: '', mainPanelId: '',
    winWidth: 820,
    winHeight: 680,
    initComponent: function () {
        var me = this;

        me.rpTypedata = [{ text: "验收", value: "验收" }, { text: "移交", value: "移交" }];

        //定义文件编码Panel
        me.fileCodePanel = Ext.create('Ext.plug_ins.ZHEPC_Plugins.Dist.FileCodePanel');

        //定义文件上传Panel
        me.fileUploadPanel = Ext.create('Ext.plug_ins.ZHEPC_Plugins.Dist.FileUploadPanel', {
            projectKeyword: me.projectKeyword
        });

        //设置上传控件为附件模式
        me.fileUploadPanel.setAttaMode();

        me.fileUploadPanel.setGridMinHeight(150);

        //新创建后的目录关键字
        me.newProjectKeyword = "";

        //发文单位combo
        me.dispatch = [];

        //添加事由text
        me.titleText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldStyle: 'border-color: red; background-image: none;',
            fieldLabel: "工程量", anchor: "80%", labelWidth: 60, labelAlign: "left", margin: '10 30 0 0', width: '50%'// flex: 1 //width: 200//flex: 1 //
        });

        me.titleText2 = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldStyle: 'border-color: red; background-image: none;',
            fieldLabel: "工程地址", anchor: "80%", labelWidth: 60, labelAlign: "left", margin: '10 25 0 0', flex: 1 // width: 380
        });

        me.titleText3 = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldStyle: 'border-color: red; background-image: none;',
            fieldLabel: "工程造价", anchor: "80%", labelWidth: 58, labelAlign: "left", margin: '10 5 0 0', flex: 1 //width: 380
        });

        me.titleText4 = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldStyle: 'border-color: red; background-image: none;',
            fieldLabel: "工程性质", anchor: "80%", labelWidth: 60, labelAlign: "left", margin: '10 5 0 0', flex: 1 //width: 380
        });

        me.titleText5 = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldStyle: 'border-color: red; background-image: none;',
            fieldLabel: "工程结构", anchor: "80%", labelWidth: 60, labelAlign: "left", margin: '10 5 0 0', flex: 1 // width: 380
        });

        me.titleText6 = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldStyle: 'border-color: red; background-image: none;',
            fieldLabel: "质量总评", anchor: "80%", labelWidth: 60, labelAlign: "left", margin: '10 5 0 20', flex: 1 // width: 380
        });

        me.fromDateField = Ext.create("Ext.form.field.Date", {
            xtype: 'datefield', fieldStyle: 'border-color: red; background-image: none;',
            anchor: '100%', 
            fieldLabel: '竣工日期', 
            labelWidth: 60, labelAlign: "left", margin: '10 5 0 20',
            format: 'Y年m月d日',
            name: 'from_date', flex: 1,
            value: new Date()  // defaults to today
        });

        me.toDateField = Ext.create("Ext.form.field.Date", {
            xtype: 'datefield', fieldStyle: 'border-color: red; background-image: none;',
            anchor: '100%', width: 230,
            fieldLabel: '上述工程验收手续于',
            labelWidth: 110, labelAlign: "left", margin: '0 5 0 10', labelSeparator: '',
            format: 'Y年m月d日',
            name: 'to_date',
            value: new Date()  // defaults to today
        });

        //添加内容Text
        me.contentText = Ext.create("Ext.form.field.TextArea", {
            xtype: "textarea", anchor: "80%", labelWidth: 60, labelAlign: "left", margin: '8 0 0 0', width: 200,
            fieldLabel: "工程范围及内容", height: 50
        });

        me.contentText2 = Ext.create("Ext.form.field.TextArea", {
            xtype: "textarea", anchor: "80%", labelWidth: 60, labelAlign: "left", margin: '8 0 0 0', width: 200,
            fieldLabel: "验收意见", height: 50
        });

        me.contentText3 = Ext.create("Ext.form.field.TextArea", {
            xtype: "textarea", anchor: "80%", labelWidth: 60, labelAlign: "left", margin: '8 0 0 0', width: 200,
            fieldLabel: "各方参加验收人员", height: 50
        });

        //添加机组combo
        Ext.define("rpTypeModel", {
            extend: 'Ext.data.Model',
            fields: ["text", "value"]
        });
        me.rpTypeProxy = Ext.create("Ext.data.proxy.Memory", {
            data: me.rpTypedata,
            model: "rpTypeModel"
        });

        me.rpTypeStore = Ext.create("Ext.data.Store", {
            model: rpTypeModel,
            proxy: me.rpTypeProxy
        });

        me.rpTypeCombo = Ext.create("Ext.form.field.ComboBox",
        {
            xtype: "combo",
            triggerAction: "all", store: me.rpTypeStore,
            valueField: 'value', editable: false,//不可输入 
            fieldLabel: "特填具", labelWidth: 40,
            emptyText: "请选择", autoLoadOnValue: true,
            displayField: 'text', value: "验收", labelSeparator: '', // 去掉laebl中的冒号
            anchor: "80%", labelAlign: "left", width: 100,
            margins: "0 5 0 5"
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
                          text: 'A.37工程竣工验收签证书', margins: '0 0 0 10'
                      }, { baseCls: 'my-panel-no-border', flex: 1 }]
                      }
                  ,
                  {//发文编号一栏
                      xtype: "fieldset",
                      //xtype: "panel",
                      margin: '8',
                      layout: {
                          type: 'vbox',
                          pack: 'start',
                          align: 'stretch'
                      },
                      bodyStyle: 'overflow-x:hidden; overflow-y:scroll',
                      items: [
                          me.fileCodePanel,


                                              {
                                                  layout: "hbox",
                                                  width: '100%', baseCls: 'my-panel-no-border',//隐藏边框
                                                  align: 'stretch', margin: '0 0 0 0', padding: '0 0 0 0',
                                                  pack: 'start',
                                                  items: [me.titleText]
                                              },
                                          {
                                              layout: "hbox",
                                              width: '100%', baseCls: 'my-panel-no-border',//隐藏边框
                                              align: 'stretch', margin: '0 0 0 0', padding: '0 0 0 0',
                                              pack: 'start',
                                              items: [me.titleText2,
                                                  {
                                                        layout: "hbox",
                                                        width: '100%', baseCls: 'my-panel-no-border',//隐藏边框
                                                        align: 'stretch', margin: '0 0 0 0', padding: '0 0 0 0',
                                                        pack: 'start',flex:1,
                                                        items: [me.titleText3,{
                                                            xtype: "label", margin: '12 0 0 0',
                                                            text: "万元"
                                                        }]
                                                    }
                                                  ]
                                          },
                                          {
                                              layout: "hbox",
                                              width: '100%', baseCls: 'my-panel-no-border',//隐藏边框
                                              align: 'stretch', margin: '0 0 0 0', padding: '0 0 0 0',
                                              pack: 'start',
                                              items: [me.titleText4, me.fromDateField]
                                          },
                                          {
                                              layout: "hbox",
                                              width: '100%', baseCls: 'my-panel-no-border',//隐藏边框
                                              align: 'stretch', margin: '0 0 0 0', padding: '0 0 0 0',
                                              pack: 'start',
                                              items: [me.titleText5, me.titleText6]
                                          },
                                          me.contentText,
                                          me.contentText2,
                                          {
                                              layout: "hbox",
                                              width: '100%', baseCls: 'my-panel-no-border',//隐藏边框
                                              align: 'stretch', margin: '8 0 8 0', padding: '0 0 0 0',
                                              pack: 'start',
                                              items: [me.toDateField, {
                                                  xtype: "label", margin: '3 0 0 0',
                                                  text: "已由甲、乙双方办理完成，认为全部工程质量标准合乎设计要求，"
                                              },
                                               me.rpTypeCombo, {
                                                   xtype: "label", margin: '3 0 0 0',
                                                   text: "证明书。"
                                               }
                                              ]
                                          },
                                          //{
                                          //    layout: "hbox",
                                          //    width: '100%', baseCls: 'my-panel-no-border',//隐藏边框
                                          //    align: 'stretch', margin: '0 0 5 0', padding: '0 0 0 0',
                                          //    pack: 'start',
                                          //    items: [
                                          //        me.rpTypeCombo, {
                                          //            xtype: "label", margin: '3 0 0 0',
                                          //            text: "证明书。"
                                          //        }
                                          //    ]
                                          //},

                                          me.contentText3

                                      ,
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
                          text: "确定", width: 60, margins: "10 5 10 5",
                          listeners: {
                              "click": function (btn, e, eOpts) {//添加点击按钮事件
                                  me.send_create_A37();
                              }
                          }
                      },
                      {
                          xtype: "button",
                          text: "取消", width: 60, margins: "10 15 10 5",
                          listeners: {
                              "click": function (btn, e, eOpts) {//添加点击按钮事件
                                  winA37.close();
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
            me.fileCodePanel.documentTypeText.setValue("A.37");
            me.fileCodePanel.getFileCodeDefaultInfo();

        }
    },

    //创建A37联系单
    send_create_A37: function () {
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
            Ext.Msg.alert("错误信息", "请填写工程量！");
            return;
        }

        if (title.indexOf("&") > 0) {
            Ext.Msg.alert("系统提示", "文件名称中包含非法字符‘&’！");
            return;
        }
        //文件标题不修改
        title = "";

        var TEXT1 = me.titleText.value === undefined ? "" : me.titleText.value;
        var TEXT2 = me.titleText2.value === undefined ? "" : me.titleText2.value;
        var TEXT3 = me.titleText3.value === undefined ? "" : me.titleText3.value;
        var TEXT4 = me.titleText4.value === undefined ? "" : me.titleText4.value;
        var TEXT5 = me.fromDateField.value === undefined ? "" : me.fromDateField.value;
        var TEXT6 = me.titleText5.value === undefined ? "" : me.titleText5.value;
        var TEXT7 = me.titleText6.value === undefined ? "" : me.titleText6.value;
        var TEXT8 = me.toDateField.value === undefined ? "" : me.toDateField.value;

        var TEXT9 = me.rpTypeCombo.value === undefined ? "" : me.rpTypeCombo.value;

        var CONTENT = me.contentText.value === undefined ? "" : me.contentText.value;
        var CONTENT2 = me.contentText2.value === undefined ? "" : me.contentText2.value;
        var CONTENT3 = me.contentText2.value === undefined ? "" : me.contentText3.value;

        var projKeyword = me.projectKeyword;

        var docAttr =
            [
                { name: 'TEXT1', value: TEXT1 },
                { name: 'TEXT2', value: TEXT2 },
                { name: 'TEXT3', value: TEXT3 },
                { name: 'TEXT4', value: TEXT4 },
                { name: 'TEXT5', value: TEXT5 },
                { name: 'TEXT6', value: TEXT6 },
                { name: 'TEXT7', value: TEXT7 },
                { name: 'TEXT8', value: TEXT8 },
                { name: 'TEXT9', value: TEXT9 },

                { name: 'CONTENT', value: CONTENT },
                { name: 'CONTENT2', value: CONTENT2 },
                { name: 'CONTENT3', value: CONTENT3 },

            ];
        var docAttrJson = Ext.JSON.encode(docAttr);

        Ext.require('Ext.plug_ins.ZHEPC_Plugins.Dist.ReportComm', function () {
            var mw = Ext.create('Ext.plug_ins.ZHEPC_Plugins.Dist.ReportComm');
            mw.winByName = winA37;
            mw.createDocument(me, 'A.37', projKeyword, fileCode, title, docAttrJson);
        });
    }


});