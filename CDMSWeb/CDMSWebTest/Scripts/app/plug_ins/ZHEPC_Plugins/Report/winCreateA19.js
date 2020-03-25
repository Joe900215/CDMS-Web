//新建目录
Ext.define('Ext.plug_ins.ZHEPC_Plugins.Report.winCreateA19', {
    extend: 'Ext.container.Container',
    alias: 'widget.winCreateA19',
    //layout: "border",
    layout: 'fit',
    resultvalue: '', mainPanelId: '',
    winWidth: 780,
    winHeight: 480,
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

        me.fromDateField = Ext.create("Ext.form.field.Date", {
            xtype: 'datefield', fieldStyle: 'border-color: red; background-image: none;',
            anchor: '100%', width: 175,
            fieldLabel: ' 我方于', labelSeparator: '',
            labelWidth: 40, labelAlign: "left", margin: '10 0 0 30',
            format: 'Y年m月d日',
            name: 'to_date',
            value: new Date()  // defaults to today
        });

        me.toDateField = Ext.create("Ext.form.field.Date", {
            xtype: 'datefield', fieldStyle: 'border-color: red; background-image: none;',
            anchor: '100%', width: 150,
            fieldLabel: '--',
            labelWidth: 10, labelAlign: "left", margin: '10 5 0 5', labelSeparator: '',
            format: 'Y年m月d日',
            name: 'to_date',
            value: new Date()  // defaults to today
        });

        //添加事由text
        me.titleText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldStyle: 'border-color: red; background-image: none;', labelSeparator: '', // 去掉laebl中的冒号
            fieldLabel: "共完成合同价款", anchor: "80%", labelWidth: 90, labelAlign: "left", margin: '10 5 0 0', width: 220//flex: 1
        });
        //添加事由text
        me.titleText2 = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldStyle: 'border-color: red; background-image: none;', labelSeparator: '', // 去掉laebl中的冒号
            fieldLabel: "规定扣除", anchor: "80%", labelWidth: 50, labelAlign: "left", margin: '0 5 0 0', width: 120//flex: 1
        });
        //添加事由text
        me.titleText3 = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldStyle: 'border-color: red; background-image: none;',
            fieldLabel: "%预付款和", anchor: "80%", labelWidth: 60, labelAlign: "left", margin: '0 5 0 0', width: 120,labelSeparator:''//flex: 1
        });
        //添加事由text
        me.titleText4 = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldStyle: 'border-color: red; background-image: none;', labelSeparator: '', // 去掉laebl中的冒号
            fieldLabel: "%质量保证金，特申请支付进度款", anchor: "80%", labelWidth: 180, labelAlign: "left", margin: '0 5 0 0', width: 290//flex: 1
        });

        me.select1CheckBox = Ext.create("Ext.form.field.Checkbox", {
            fieldLabel: "", margin: '5 0 0 10', labelWidth: 0, checked: true,
            boxLabel: "产值报表、报表详表及汇总表"
        });

        //添加内容Text
        me.contentText = Ext.create("Ext.form.field.TextArea", {
            xtype: "textarea", anchor: "80%", labelWidth: 60, labelAlign: "left", margin: '8 0 8 0', width: 360,
            fieldLabel: "内容", height: 105
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
                          text: 'A.19工程款支付申请表', margins: '0 0 0 10'
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
                      me.fileCodePanel,
                      {
                          layout: "hbox",
                          width: '100%', baseCls: 'my-panel-no-border',//隐藏边框
                          align: 'stretch', margin: '8 0 8 0', padding: '0 0 0 0',
                          pack: 'start',
                          items: [
                              me.fromDateField,me.toDateField
                              , me.titleText,
                               {
                                   xtype: "label",
                                   text: "元，按合同", margins: "13 0 0 0"
                               }]
                      },
                              {
                                  layout: "hbox",
                                  width: '100%', baseCls: 'my-panel-no-border',//隐藏边框
                                  align: 'stretch', margin: '8 0 8 0', padding: '0 0 0 0',
                                  pack: 'start',
                                  items: [
                                      me.titleText2, me.titleText3, me.titleText4,
                                      {
                                          xtype: "label",
                                          text: "元，请审核。", margins: "3 0 0 0"
                                      }
                                  ]
                              }, {
                                  layout: "hbox",
                                  width: '100%', baseCls: 'my-panel-no-border',//隐藏边框
                                  align: 'stretch', margin: '8 0 8 0', padding: '0 0 0 0',
                                  pack: 'start',
                                  items: [
                                      {
                                          xtype: "label",
                                          text: "附件:", margin: '8 0 8 0',
                                          width: 60
                                      },
                                     {
                                         layout: "vbox",
                                         width: '100%', baseCls: 'my-panel-no-border',//隐藏边框
                                         align: 'stretch', margin: '0 0 0 0', padding: '0 0 0 0',
                                         pack: 'start', flex: 1,
                                         items: [
                                             me.select1CheckBox,
                               
                                         ]
                                     }
                                  ]
                              },
                  me.fileUploadPanel,
                   
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
                                  me.send_create_A19();
                              }
                          }
                      },
                      {
                          xtype: "button",
                          text: "取消", width: 60, margins: "10 15 10 5",
                          listeners: {
                              "click": function (btn, e, eOpts) {//添加点击按钮事件
                                  //Ext.Msg.alert("您展开了目录树节点！！！", "您展开了目录树节点！节点ID:" + me.tempDefnId);
                                  winA19.close();
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
            me.fileCodePanel.documentTypeText.setValue("A.19");
            me.fileCodePanel.getFileCodeDefaultInfo();

        }
    },

    //创建A19联系单
    send_create_A19: function () {
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

        var TEXT1 = me.fromDateField.value;
        var TEXT2 = me.toDateField.value;

        var TEXT3 = me.titleText.value === undefined ? "" : me.titleText.value;
        var TEXT4 = me.titleText2.value === undefined ? "" : me.titleText2.value;
        var TEXT5 = me.titleText3.value === undefined ? "" : me.titleText3.value;
        var TEXT6 = me.titleText4.value === undefined ? "" : me.titleText4.value;

        var select1 = me.select1CheckBox.checked;

        var docAttr =
            [
                { name: 'TEXT1', value: TEXT1 },
                { name: 'TEXT2', value: TEXT2 },
                { name: 'TEXT3', value: TEXT3 },
                { name: 'TEXT4', value: TEXT4 },
                { name: 'TEXT5', value: TEXT5 },
                { name: 'TEXT6', value: TEXT6 },
                { name: 'select1', value: select1 }
            ];
        var docAttrJson = Ext.JSON.encode(docAttr);

        Ext.require('Ext.plug_ins.ZHEPC_Plugins.Dist.ReportComm', function () {
            var mw = Ext.create('Ext.plug_ins.ZHEPC_Plugins.Dist.ReportComm');
            mw.winByName = winA19;
            mw.createDocument(me, 'A.19', projKeyword, fileCode, title, docAttrJson);
        });
    }
});