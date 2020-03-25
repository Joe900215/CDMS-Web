//新建目录
Ext.define('Ext.plug_ins.ZHEPC_Plugins.Report.winCreateA3', {
    extend: 'Ext.container.Container',
    alias: 'widget.winCreateA3',
    //layout: "border",
    layout: 'fit',
    resultvalue: '', mainPanelId: '',
    projectKeyword: '',
    winWidth: 700,
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
            fieldLabel: "我方已根据承包合同的有关规定完成了", anchor: "80%", labelWidth: 210, labelAlign: "left", margin: '10 5 0 0', width: 350//flex: 1
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
            displayField: 'text', hideLabel: true,value:"施工组织设计",
            anchor: "80%", labelAlign: "left", width: 120,
            emptyText: "请选择", autoLoadOnValue: true,
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
                          text: 'A.3施工组织设计报审表', margins: '0 0 0 10'
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
                              {
                                  xtype: "label",
                                  text: "工程", margins: "13 0 0 0"
                              },
                          me.rpTypeCombo, {
                              xtype: "label",
                              text: "的编制，并经我单位技术", margins: "13 0 0 0"
                          }, ]
                      },
                      {
                          xtype: "label",
                          text: "负责人审查批准，请予以审查。", margins: "0 0 10 0"
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
                                  me.send_create_a3();
                              }
                          }
                      },
                      {
                          xtype: "button",
                          text: "取消", width: 60, margins: "10 15 10 5",
                          listeners: {
                              "click": function (btn, e, eOpts) {//添加点击按钮事件
                                  winA3.close();
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
            me.fileCodePanel.documentTypeText.setValue("A.3");
            me.fileCodePanel.getFileCodeDefaultInfo();

        }
    },

    //创建A3联系单
    send_create_a3: function () {
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

        var rpType = me.rpTypeCombo.value;

        var docAttr =
            [
                { name: 'rpType', value: rpType }
            ];
        var docAttrJson = Ext.JSON.encode(docAttr);

        Ext.require('Ext.plug_ins.ZHEPC_Plugins.Dist.ReportComm', function () {
            var mw = Ext.create('Ext.plug_ins.ZHEPC_Plugins.Dist.ReportComm');
            mw.winByName = winA3;
            mw.createDocument(me, 'A.3', projKeyword, fileCode, title, docAttrJson);
        });
    }

 
});