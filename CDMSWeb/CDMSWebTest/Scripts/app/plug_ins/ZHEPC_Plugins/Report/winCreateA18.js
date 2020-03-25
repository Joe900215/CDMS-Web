//新建目录
Ext.define('Ext.plug_ins.ZHEPC_Plugins.Report.winCreateA18', {
    extend: 'Ext.container.Container',
    alias: 'widget.winCreateA18',
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

        me.fileUploadPanel.titleLabel.setText("");

        //添加事由text
        me.titleText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldStyle: 'border-color: red; background-image: none;', labelSeparator: '', // 去掉laebl中的冒号
            fieldLabel: "我方接到编号为", anchor: "80%", labelWidth: 100, labelAlign: "left",margin: '10 5 0 30', width: 265//flex: 1
        });
        //添加事由text
        me.titleText2 = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldStyle: 'border-color: red; background-image: none;', labelSeparator: '', // 去掉laebl中的冒号
            fieldLabel: "", labelWidth: 10, labelAlign: "left", margin: '10 5 0 0', width: 165//flex: 1
        });

        //添加内容Text
        me.contentText = Ext.create("Ext.form.field.TextArea", {
            xtype: "textarea", anchor: "80%", labelWidth: 55, labelAlign: "left", //labelSeparator: '', // 去掉laebl中的冒号
            margin: '8 0 8 0', width: 360,fieldLabel: "详细内容", height: 120
        });
       
        me.select1CheckBox = Ext.create("Ext.form.field.Checkbox", {
            fieldLabel: "", margin: '5 0 0 5', labelWidth: 0,checked:true,
            boxLabel: "回复材料"
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
                          text: 'A.18监理工程师通知回复单', margins: '0 0 0 10'
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
                          align: 'stretch', margin: '8 0 0 0', padding: '0 0 0 0',
                          pack: 'start',
                          items: [me.titleText,
                              {
                                  xtype: "label",
                                  text: "的监理工程师通知后，已按要求完成了", margins: "13 5 0 0"
                              },
                                me.titleText2,
                          ]
                      },
                        {
                            layout: "hbox",
                            width: '100%', baseCls: 'my-panel-no-border',//隐藏边框
                            align: 'stretch', margin: '0 0 8 0', padding: '0 0 0 0',
                            pack: 'start',
                            items: [
                                
                                {
                                    xtype: "label",
                                    text: "工作，现报上，请予以复查。", margins: "13 0 0 0"
                                }]
                        },
                       me.contentText,
                         {
                             layout: "hbox",
                             width: '100%', baseCls: 'my-panel-no-border',//隐藏边框
                             align: 'stretch', margin: '0 0 0 0', padding: '0 0 0 0',
                             pack: 'start',
                             items: [
                                 {
                                     xtype: "label",
                                     text: "附件:", margin: '5 0 0 0',
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
                                  me.send_create_A18();
                              }
                          }
                      },
                      {
                          xtype: "button",
                          text: "取消", width: 60, margins: "10 15 10 5",
                          listeners: {
                              "click": function (btn, e, eOpts) {//添加点击按钮事件
                                  //Ext.Msg.alert("您展开了目录树节点！！！", "您展开了目录树节点！节点ID:" + me.tempDefnId);
                                  winA18.close();
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
            me.fileCodePanel.documentTypeText.setValue("A.18");
            me.fileCodePanel.getFileCodeDefaultInfo();

        }
    },

    //创建A18联系单
    send_create_A18: function () {
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
        var TEXT2 = me.titleText2.value === undefined ? "" : me.titleText2.value;
        var CONTENT = me.contentText.value === undefined ? "" : me.contentText.value;

        var select1 = me.select1CheckBox.checked;

        var docAttr =
            [
                { name: 'TEXT1', value: TEXT1 },
                { name: 'TEXT2', value: TEXT2 },
                { name: 'CONTENT', value: CONTENT },
                { name: 'select1', value: select1 }
            ];
        var docAttrJson = Ext.JSON.encode(docAttr);

        Ext.require('Ext.plug_ins.ZHEPC_Plugins.Dist.ReportComm', function () {
            var mw = Ext.create('Ext.plug_ins.ZHEPC_Plugins.Dist.ReportComm');
            mw.winByName = winA18;
            mw.createDocument(me, 'A.18', projKeyword, fileCode, title, docAttrJson);
        });
    }
});