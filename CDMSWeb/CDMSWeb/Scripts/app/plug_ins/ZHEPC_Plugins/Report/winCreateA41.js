//新建目录
Ext.define('Ext.plug_ins.ZHEPC_Plugins.Report.winCreateA41', {
    extend: 'Ext.container.Container',
    alias: 'widget.winCreateA41',
    //layout: "border",
    layout: 'fit',
    resultvalue: '', mainPanelId: '',
    projectKeyword: '',
    winWidth: 780,
    winHeight: 430,
    initComponent: function () {
        var me = this;

        me.rpTypedata = [{ text: "施工组织设计", value: "施工组织设计" }, { text: "项目管理实施规划", value: "项目管理实施规划" }];

        //定义文件编码Panel
        me.fileCodePanel = Ext.create('Ext.plug_ins.ZHEPC_Plugins.Dist.FileCodePanel');

        //定义文件上传Panel
        me.fileUploadPanel = Ext.create('Ext.plug_ins.ZHEPC_Plugins.Dist.FileUploadPanel', {
            projectKeyword: me.projectKeyword
        });

        //设置上传控件为附件模式
        me.fileUploadPanel.setAttaMode();

        me.fileUploadPanel.setGridMinHeight(150);

        //添加事由text
        me.titleText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldStyle: 'border-color: red; background-image: none;', labelSeparator: '', // 去掉laebl中的冒号
            fieldLabel: "根据", anchor: "80%", labelWidth: 30, labelAlign: "left", margin: '10 5 0 30', width: 220,//flex: 1
            listeners: {
               "blur": function (view, e, eOpts) {//添加点击按钮事件
                   me.titleText2.setValue(me.titleText.value === undefined ? "" : me.titleText.value);
                }
            }
        });

        //添加事由text
        me.titleText2 = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldStyle: 'border-color: red; background-image: none;', labelSeparator: '', // 去掉laebl中的冒号
            fieldLabel: "工程施工进度计划要求，我单位已编制完成", anchor: "80%", labelWidth: 230, labelAlign: "left", margin: '10 5 0 0', width: 420,//flex: 1
            listeners: {
                "blur": function (view, e, eOpts) {//添加点击按钮事件
                    me.titleText.setValue(me.titleText2.value === undefined ? "" : me.titleText2.value);
                }
            }
        });

        me.fromDateField = Ext.create("Ext.form.field.Date", {
            xtype: 'datefield', fieldStyle: 'border-color: red; background-image: none;',
            anchor: '100%', width: 140,
            fieldLabel: ' ', labelSeparator: '',
            labelWidth: 5, labelAlign: "left", margin: '0 0 0 10',
            format: 'Y年m月d日',
            name: 'from_date',
            value: new Date()  // defaults to today
        });

        me.toDateField = Ext.create("Ext.form.field.Date", {
            xtype: 'datefield', fieldStyle: 'border-color: red; background-image: none;',
            anchor: '100%', width: 150,
            fieldLabel: '---',
            labelWidth: 15, labelAlign: "left", margin: '0 5 0 5', labelSeparator: '',
            format: 'Y年m月d日',
            name: 'to_date',
            value: new Date()  // defaults to today
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
                          text: 'A.41年（月）度资金需求计划报审表', margins: '0 0 0 10'
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
                              items: [me.titleText,
                                  me.titleText2,
                                  {
                                      xtype: "label",
                                      text: "工程", margins: "13 0 0 0"
                                  },

                              ]
                          },
                          {
                              layout: "hbox",
                              width: '100%', baseCls: 'my-panel-no-border',//隐藏边框
                              align: 'stretch', margin: '8 0 0 0', padding: '0 0 0 0',
                              pack: 'start',
                              items: [
                                  me.fromDateField,
                                  me.toDateField,
                                  {
                                      xtype: "label",
                                      text: "施工资金需求计划，特此上报，请予审批。", margins: "0 0 10 0"
                                  },

                              ]
                          },


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
                                  me.send_create_a41();
                              }
                          }
                      },
                      {
                          xtype: "button",
                          text: "取消", width: 60, margins: "10 15 10 5",
                          listeners: {
                              "click": function (btn, e, eOpts) {//添加点击按钮事件
                                  winA41.close();
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
            me.fileCodePanel.documentTypeText.setValue("A.41");
            me.fileCodePanel.getFileCodeDefaultInfo();

        }
    },

    //创建A3联系单
    send_create_a41: function () {
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

        var TEXT1 = me.titleText.value === undefined ? "" : me.titleText.value;
        var TEXT1A = me.titleText2.value === undefined ? "" : me.titleText2.value;
        var TEXT2 = me.fromDateField.value === undefined ? "" : me.fromDateField.value;
        var TEXT3 = me.toDateField.value === undefined ? "" : me.toDateField.value;
        
        var docAttr =
            [
                { name: 'TEXT1', value: TEXT1 },
                { name: 'TEXT1A', value: TEXT1A },
                { name: 'TEXT2', value: TEXT2 },
                { name: 'TEXT3', value: TEXT3 }
            ];
        var docAttrJson = Ext.JSON.encode(docAttr);

        Ext.require('Ext.plug_ins.ZHEPC_Plugins.Dist.ReportComm', function () {
            var mw = Ext.create('Ext.plug_ins.ZHEPC_Plugins.Dist.ReportComm');
            mw.winByName = winA41;
            mw.createDocument(me, 'A.41', projKeyword, fileCode, title, docAttrJson);
        });
    }

 
});