//新建目录
Ext.define('Ext.plug_ins.ZHEPC_Plugins.Report.winCreateA29', {
    extend: 'Ext.container.Container',
    alias: 'widget.winCreateA29',
    //layout: "border",
    layout: 'fit',
    resultvalue: '', mainPanelId: '',
    winWidth: 700,
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

        //新创建后的目录关键字
        me.newProjectKeyword = "";

        //发文单位combo
        me.dispatch = [];

        //添加事由text
        me.titleText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldStyle: 'border-color: red; background-image: none;',
            fieldLabel: "提出单位", anchor: "80%", labelWidth: 60, labelAlign: "left", margin: '10 5 0 0', width: 300//flex: 1
        });

        me.titleText2 = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldStyle: 'border-color: red; background-image: none;',
            fieldLabel: "编号", anchor: "80%", labelWidth: 60, labelAlign: "left", margin: '10 5 0 30', width: 300
        });

        me.titleText3 = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldStyle: 'border-color: red; background-image: none;',
            fieldLabel: "项目名称", anchor: "80%", labelWidth: 60, labelAlign: "left", margin: '10 5 0 0', width: 300
        });

        //添加内容Text
        me.contentText = Ext.create("Ext.form.field.TextArea", {
            xtype: "textarea", anchor: "80%", labelWidth: 60, labelAlign: "left", margin: '8 0 0 0', width: 200,
            fieldLabel: "工程量签证依据及工程量", height: 50, value: "（可附图及工程量计算书）"
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

        me.rpTypeStore2 = Ext.create("Ext.data.Store", {
            model: rpTypeModel,
            proxy: me.rpTypeProxy
        });

        me.rpTypeDate = Ext.create("Ext.form.field.Date",
        {
            xtype: "datefield",
            triggerAction: "all", store: me.rpTypeStore,
            anchor: "80%", labelAlign: "left", width: 200,
            margins: "10 5 0 30",
            fieldLabel: "时间", labelWidth: 60,
            format: 'Y年m月d日',
            name: 'to_date',
            value: new Date()
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
                          text: 'A.29工程量签证单', margins: '0 0 0 10'
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
                                   align: 'stretch', margin: '8 0 3 0', padding: '0 0 0 0',
                                   pack: 'start',
                                   items: [me.titleText, me.titleText2]
                               },
                            {
                                layout: "hbox",
                                width: '100%', baseCls: 'my-panel-no-border',//隐藏边框
                                align: 'stretch', margin: '0 0 0 0', padding: '0 0 0 0',
                                pack: 'start',
                                items: [me.titleText3,
                                    me.rpTypeDate]
                            },me.contentText,
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
                                  me.send_create_A29();
                              }
                          }
                      },
                      {
                          xtype: "button",
                          text: "取消", width: 60, margins: "10 15 10 5",
                          listeners: {
                              "click": function (btn, e, eOpts) {//添加点击按钮事件
                                  winA29.close();
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
            me.fileCodePanel.documentTypeText.setValue("A.29");
            me.fileCodePanel.getFileCodeDefaultInfo();

        }
    },

    //创建A29联系单
    send_create_A29: function () {
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
            Ext.Msg.alert("错误信息", "请填写项目名称！");
            return;
        }

        if (title.indexOf("&") > 0) {
            Ext.Msg.alert("系统提示", "文件名称中包含非法字符‘&’！");
            return;
        }

        var title3 = me.titleText2.value === undefined ? "" : me.titleText2.value;
        var title2 = me.titleText.value === undefined ? "" : me.titleText.value;
        var content = me.contentText.value === undefined ? "" : me.contentText.value;


        var projKeyword = me.projectKeyword;

        var Date = me.rpTypeDate.value;

        var docAttr =
            [
                { name: 'Date', value: Date },
                { name: 'title2', value: title2 },
                { name: 'title3', value: title3 },
                { name: 'content', value: content }
            ];
        var docAttrJson = Ext.JSON.encode(docAttr);

        Ext.require('Ext.plug_ins.ZHEPC_Plugins.Dist.ReportComm', function () {
            var mw = Ext.create('Ext.plug_ins.ZHEPC_Plugins.Dist.ReportComm');
            mw.winByName = winA29;
            mw.createDocument(me, 'A.29', projKeyword, fileCode, title, docAttrJson);
        });
    }


});