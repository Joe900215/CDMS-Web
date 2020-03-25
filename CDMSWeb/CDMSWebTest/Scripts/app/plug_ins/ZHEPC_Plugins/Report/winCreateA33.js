//新建目录
Ext.define('Ext.plug_ins.ZHEPC_Plugins.Report.winCreateA33', {
    extend: 'Ext.container.Container',
    alias: 'widget.winCreateA33',
    //layout: "border",
    layout: 'fit',
    resultvalue: '', mainPanelId: '',
    winWidth: 700,
    winHeight: 460,
    initComponent: function () {
        var me = this;

        //定义文件编码Panel
        me.fileCodePanel = Ext.create('Ext.plug_ins.ZHEPC_Plugins.Dist.FileCodePanel');

        //定义文件上传Panel
        me.fileUploadPanel = Ext.create('Ext.plug_ins.ZHEPC_Plugins.Dist.FileUploadPanel', {
            projectKeyword: me.projectKeyword
        });

        me.fileUploadPanel.titleLabel.setText("");

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
            fieldLabel: "变更内容", anchor: "80%", labelWidth: 70, labelAlign: "left", margin: '10 5 0 0', width: 270//flex: 1
        });

        me.titleText2 = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldStyle: 'border-color: red; background-image: none;', 
            fieldLabel: "变更单编号", anchor: "80%", labelWidth: 70, labelAlign: "left", margin: '10 5 0 20', width: 270
        });

        me.titleText3 = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldStyle: 'border-color: red; background-image: none;', labelSeparator: '', // 去掉laebl中的冒号
            fieldLabel: "现报上", anchor: "80%", labelWidth: 50, labelAlign: "left", margin: '5 5 0 30', width: 270
        });

        me.titleText4 = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", //fieldStyle: 'border-color: red; background-image: none;',
            fieldLabel: "附件", value: '劳动力、材料、工程量及费用增减情况说明。',
            anchor: "80%", labelWidth: 60, labelAlign: "left", margin: '5 5 0 0', width: 270
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
                          text: 'A.33工程变更费用报审表', margins: '0 0 0 10'
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
                                   items: [me.titleText, me.titleText2]
                               }, {
                                   layout: "hbox",
                                   width: '100%', baseCls: 'my-panel-no-border',//隐藏边框
                                   align: 'stretch', margin: '4 0 14 0', padding: '0 0 0 0',
                                   pack: 'start',
                                   items: [me.titleText3,{
                                       xtype: "label",
                                       text: "项目变更费用申请，请予审批。", margins: "10 5 0 0", labelWidth: 70
                                   }]
                               },
                               me.titleText4,
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
                                  me.send_create_A33();
                              }
                          }
                      },
                      {
                          xtype: "button",
                          text: "取消", width: 60, margins: "10 15 10 5",
                          listeners: {
                              "click": function (btn, e, eOpts) {//添加点击按钮事件
                                  winA33.close();
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
            me.fileCodePanel.documentTypeText.setValue("A.33");
            me.fileCodePanel.getFileCodeDefaultInfo();

        }
    },

    //创建A33联系单
    send_create_A33: function () {
        var me = this;

        //检查文件编码
        var checkResult = me.fileCodePanel.checkFileCodeFill();
        if (checkResult != "true") {
            Ext.Msg.alert("错误信息", checkResult);
            return;
        }

        var fileCode = me.fileCodePanel.getFileCode();

        var title = me.titleText3.value === undefined ? "" : me.titleText3.value;
        if (title === "") {
            Ext.Msg.alert("错误信息", "请填写工程名称！");
            return;
        }

        if (title.indexOf("&") > 0) {
            Ext.Msg.alert("系统提示", "文件名称中包含非法字符‘&’！");
            return;
        }

        var title2 = me.titleText.value === undefined ? "" : me.titleText.value;
        var title3 = me.titleText2.value === undefined ? "" : me.titleText2.value;
        var title4 = me.titleText4.value === undefined ? "" : me.titleText4.value;

        var projKeyword = me.projectKeyword;

        var docAttr =
            [
                { name: 'title2', value: title2 },
                { name: 'title3', value: title3 },
                { name: 'title4', value: title4 }
            ];
        var docAttrJson = Ext.JSON.encode(docAttr);

        Ext.require('Ext.plug_ins.ZHEPC_Plugins.Dist.ReportComm', function () {
            var mw = Ext.create('Ext.plug_ins.ZHEPC_Plugins.Dist.ReportComm');
            mw.winByName = winA33;
            mw.createDocument(me, 'A.33', projKeyword, fileCode, title, docAttrJson);
        });
    }


});