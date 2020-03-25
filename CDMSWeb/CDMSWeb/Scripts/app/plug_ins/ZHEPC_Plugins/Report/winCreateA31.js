//新建目录
Ext.define('Ext.plug_ins.ZHEPC_Plugins.Report.winCreateA31', {
    extend: 'Ext.container.Container',
    alias: 'widget.winCreateA31',
    //layout: "border",
    layout: 'fit',
    resultvalue: '', mainPanelId: '',
    winWidth: 780,
    winHeight: 420,
    initComponent: function () {
        var me = this;

        me.rpTypedata = [{ text: "计划", value: "计划" }, { text: "细则", value: "细则" }];

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
            xtype: "textfield", fieldStyle: 'border-color: red; background-image: none;', labelSeparator: '', // 去掉laebl中的冒号
            fieldLabel: "根据《工程建设标准强制性条文》要求，我单位已编制完成", anchor: "80%", labelWidth: 320, labelAlign: "left", margin: '10 5 0 30', width: 460//flex: 1
        });

        me.titleText2 = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldStyle: 'border-color: red; background-image: none;', labelSeparator: '', // 去掉laebl中的冒号
            fieldLabel: "强制性条文实施", anchor: "80%", labelWidth: 10, labelAlign: "left", margin: '10 5 0 0', width: 90
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
            displayField: 'text', value: "计划", labelSeparator: '', // 去掉laebl中的冒号
            fieldLabel: "强制性条文实施", labelWidth: 90,
            emptyText: "请选择", autoLoadOnValue: true,
            anchor: "80%", labelAlign: "left", width: 170,
            margins: "10 5 0 5"
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
                          text: 'A.31强制性条文实施计划／细则报审表', margins: '0 0 0 10'
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
                                       me.rpTypeCombo,
                                       //me.titleText2, 
                                   {
                                       xtype: "label",
                                       text: "，", margins: "10 5 0 0", labelWidth: 70
                                   }]
                               },{
                                   xtype: "label",
                                   text: "特此上报，请予审批。", margins: "10 5 0 0", labelWidth: 70
                               }, me.fileUploadPanel
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
                                  me.send_create_A31();
                              }
                          }
                      },
                      {
                          xtype: "button",
                          text: "取消", width: 60, margins: "10 15 10 5",
                          listeners: {
                              "click": function (btn, e, eOpts) {//添加点击按钮事件
                                  winA31.close();
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
            me.fileCodePanel.documentTypeText.setValue("A.31");
            me.fileCodePanel.getFileCodeDefaultInfo();

        }
    },

    //创建A31联系单
    send_create_A31: function () {
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

        var title2 = me.titleText2.value === undefined ? "" : me.titleText2.value;

        var projKeyword = me.projectKeyword;

        var TEXT1 = me.titleText.value === undefined ? "" : me.titleText.value;
        var rpType = me.rpTypeCombo.value;

        var docAttr =
            [
                { name: 'TEXT1', value: TEXT1 },
                { name: 'rpType', value: rpType }
            ];
        var docAttrJson = Ext.JSON.encode(docAttr);

        Ext.require('Ext.plug_ins.ZHEPC_Plugins.Dist.ReportComm', function () {
            var mw = Ext.create('Ext.plug_ins.ZHEPC_Plugins.Dist.ReportComm');
            mw.winByName = winA31;
            mw.createDocument(me, 'A.31', projKeyword, fileCode, title, docAttrJson);
        });
    }


});