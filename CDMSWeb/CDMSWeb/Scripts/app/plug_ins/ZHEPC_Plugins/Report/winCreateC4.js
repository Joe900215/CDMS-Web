//新建目录
Ext.define('Ext.plug_ins.ZHEPC_Plugins.Report.winCreateC4', {
    extend: 'Ext.container.Container',
    alias: 'widget.winCreateC4',
    //layout: "border",
    layout: 'fit',
    winWidth: 780,
    winHeight: 630,
    resultvalue: '',mainPanelId:'',
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

        //添加事由text
        me.titleText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldStyle: 'border-color: red; background-image: none;',value:" 设计交底",
            fieldLabel: "说明", anchor: "80%", labelWidth: 60, labelAlign: "left", margin: '8 5 0 0', flex: 1 //width: 210//
        });

        //添加建设单位text
        me.titleText2 = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldStyle: 'border-color: red; background-image: none;', readOnly: true,
            fieldLabel: "建设单位", value: '珠海市钰海电力有限公司',
            anchor: "80%", labelWidth: 60, labelAlign: "left", margin: '5 20 0 0', flex: 1 //width: 210//
        });

        //添加监理单位text
        me.titleText3 = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldStyle: 'border-color: red; background-image: none;', readOnly:true,
            fieldLabel: "监理单位", value: '中南电力项目管理咨询（湖北）有限公司珠海市钰海天然气热电联产工程监理部',
            anchor: "80%", labelWidth: 60, labelAlign: "left", margin: '5 5 0 0', flex: 1 //width: 210//
        });

        //添加承包单位text
        me.titleText4 = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldStyle: 'border-color: red; background-image: none;',
            fieldLabel: "承包单位", anchor: "80%", labelWidth: 60, labelAlign: "left", margin: '0 5 0 0', flex: 1 //width: 210//
        });

        //添加内容Text
        me.contentText = Ext.create("Ext.form.field.TextArea", {
            xtype: "textarea", anchor: "80%", labelWidth: 80, labelAlign: "left", margin: '8 0 0 0', width: 360,
            fieldLabel: "设计交底内容与注意事项", height: 60
        });

        //添加内容Text
        me.contentText2 = Ext.create("Ext.form.field.TextArea", {
            xtype: "textarea", anchor: "80%", labelWidth: 80, labelAlign: "left", margin: '8 0 0 0', width: 360,
            fieldLabel: "安全和施工特殊工艺要求", height: 60
        });

        //添加内容Text
        me.contentText3 = Ext.create("Ext.form.field.TextArea", {
            xtype: "textarea", anchor: "80%", labelWidth: 80, labelAlign: "left", margin: '8 0 0 0', width: 360,
            fieldLabel: "设计遗留问题", height: 60
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
                          text: 'C.4设计交底纪要', margins: '0 0 0 10'
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
                          align: 'stretch', margin: '3 0 8 0', padding: '0 0 0 0',
                          pack: 'start',
                          items: [
                              me.titleText2,
                              me.titleText3
                          ]
                      },
                       me.titleText4,
                        me.titleText,
                      , me.contentText,
                      me.contentText2,
                      me.contentText3,
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
                                  me.send_create_C4();
                              }
                          }
                      },
                      {
                          xtype: "button",
                          text: "取消", width: 60, margins: "10 15 10 5",
                          listeners: {
                              "click": function (btn, e, eOpts) {//添加点击按钮事件
                                  //Ext.Msg.alert("您展开了目录树节点！！！", "您展开了目录树节点！节点ID:" + me.tempDefnId);
                                  winC4.close();
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
            me.fileCodePanel.documentTypeText.setValue("C.4");
            me.fileCodePanel.getFileCodeDefaultInfo();

        }
    },

    //创建C4联系单
    send_create_C4: function () {
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

        title = title.replace("设计交底","");

        var projKeyword = me.projectKeyword;

        var TEXT1 = me.titleText2.value === undefined ? "" : me.titleText2.value;   //建设单位
        var TEXT2 = me.titleText3.value === undefined ? "" : me.titleText3.value;   //监理单位
        var TEXT3 = me.titleText4.value === undefined ? "" : me.titleText4.value;   //承包单位
        var TEXT4 = me.titleText.value === undefined ? "" : me.titleText.value;   //说明

        var CONTENT = me.contentText.value === undefined ? "" : me.contentText.value;   //交底内容
        var CONTENT2 = me.contentText2.value === undefined ? "" : me.contentText2.value;   //交底内容
        var CONTENT3 = me.contentText3.value === undefined ? "" : me.contentText3.value;   //交底内容

        var docAttr =
            [
                { name: 'TEXT1', value: TEXT1 },
                { name: 'TEXT2', value: TEXT2 },
                { name: 'TEXT3', value: TEXT3 },
                { name: 'TEXT4', value: TEXT4 },

                { name: 'CONTENT', value: CONTENT },
                { name: 'CONTENT2', value: CONTENT2 },
                { name: 'CONTENT3', value: CONTENT3 },
            ];
        var docAttrJson = Ext.JSON.encode(docAttr);

        Ext.require('Ext.plug_ins.ZHEPC_Plugins.Dist.ReportComm', function () {
            var mw = Ext.create('Ext.plug_ins.ZHEPC_Plugins.Dist.ReportComm');
            mw.winByName = winC4;
            mw.createDocument(me, 'C.4', projKeyword, fileCode, title, docAttrJson);
        });
    },

});