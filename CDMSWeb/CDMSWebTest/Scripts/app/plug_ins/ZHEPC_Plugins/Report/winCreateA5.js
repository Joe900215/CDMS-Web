//新建目录
Ext.define('Ext.plug_ins.ZHEPC_Plugins.Report.winCreateA5', {
    extend: 'Ext.container.Container',
    alias: 'widget.winCreateA5',
    //layout: "border",
    layout: 'fit',
    resultvalue: '', mainPanelId: '',
    projectKeyword: '',
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

        //新创建后的目录关键字
        me.newProjectKeyword = "";

        //发文单位combo
        me.dispatch = [];
  
        //添加事由text
        me.titleText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldStyle: 'border-color: red; background-image: none;',
            fieldLabel: "经考察，我方认为拟选择的", anchor: "80%", labelWidth: 150, labelAlign: "left", margin: '10 5 0 0', width: 300//flex: 1
        });
        //添加内容Text
        me.contentText = Ext.create("Ext.form.field.TextArea", {
            xtype: "textarea", anchor: "80%", labelWidth: 60, labelAlign: "left", margin: '8 0 8 0', width: 360,
            fieldLabel: "内容", height: 105
        });
    
        //添加金额合计text
        me.amountCountText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldStyle: 'border-color: red; background-image: none;',
            fieldLabel: "合计金额", anchor: "80%", labelWidth: 60, labelAlign: "right", margin: '10 5 0 0', width: 180//flex: 1
        });

        //添加比例合计text
        me.scaleCountText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldStyle: 'border-color: red; background-image: none;',
            fieldLabel: "合计比例", anchor: "80%", labelWidth: 60, labelAlign: "right", margin: '10 25 0 0', width: 180//flex: 1
        });

        me.select1CheckBox = Ext.create("Ext.form.field.Checkbox", {
            fieldLabel: "", margin: '5 0 0 10', labelWidth: 0, checked: true,
            boxLabel: "分包单位资质材料"
        });

        me.select2CheckBox = Ext.create("Ext.form.field.Checkbox", {
            fieldLabel: "", margin: '5 0 0 10', labelWidth: 0, checked: true,
            boxLabel: "分包单位业绩资料"
        });

        me.cellEditing = new Ext.grid.plugin.CellEditing({
            clicksToEdit: 1
        });

        //定义工程量的model
        Ext.define("contentModel", {
            extend: "Ext.data.Model",
            //fields: ['分包工程名称(部位)', '工程量', '拟分包工程合同额', '分包工程占全部工程比例'],
            fields: ["id", "name", "quantity", "amount", "scale", "issueUnit"],
            url: "_blank",
        });

        me.contentStore = Ext.create('Ext.data.Store', {
            autoDestroy: true,
            model: contentModel,
            sorters: [{
                property: 'id',
                direction: 'ASC'
            }]
        });

        me.contentGrid = Ext.create('Ext.grid.Panel', {
            //title: '作业人员名单及其资格证件',
            store: me.contentStore,
            columns: [
                {
                    header: '分包工程名称(部位)', dataIndex: 'name', width: 250,
                    editor: {
                        allowBlank: false
                    }
                },
                {
                    header: '工程量', dataIndex: 'quantity', width: 150,
                    editor: {
                        allowBlank: false
                    }
                },
                {
                    header: '拟分包工程合同额', dataIndex: 'amount', width: 150,
                    editor: {
                        allowBlank: false
                    }
                },
                {
                    header: '分包工程占全部工程比例', dataIndex: 'scale', width: 150,
                    editor: {
                        allowBlank: false
                    }
                }
            ],
            stripeRows: true, //斑马线效果  
            plugins: [
                 Ext.create('Ext.grid.plugin.CellEditing', {
                     clicksToEdit: 1 //设置单击单元格编辑  
                 })],
            height: 90,
            width: 400,
            renderTo: Ext.getBody(),
            selModel: {
                selType: 'cellmodel'
            }
        });

        //人员表格添加行
        for (var i = 0; i < 8; i++) {
            var rec = new contentModel({
                name: ""
            });
            me.contentStore.insert(0, rec);
        }


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
                          text: 'A.5分包单位资格报审表', margins: '0 0 0 10'
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
                                  text: "（分包单位）具有承担下列工程的施工资质和施工能力，可以保证本工程项目按", margins: "13 0 0 0"
                              }, ]
                      },
                      {
                          xtype: "label",
                          text: "承包合同的规定进行施工。分包后，我方仍承担总包单位的全部责任。请予以审批。", margins: "0 0 10 0"
                      },
                      me.contentGrid,
                      {
                          layout: "hbox",
                          width: '100%', baseCls: 'my-panel-no-border',//隐藏边框
                          align: 'stretch', margin: '0 0 0 0', padding: '0 0 0 0',
                          pack: 'start',
                          items: [
                              { flex: 1, baseCls: 'my-panel-no-border' },
                              me.amountCountText,
                          me.scaleCountText]
                      },
                      {
                          layout: "hbox",
                          width: '100%', baseCls: 'my-panel-no-border',//隐藏边框
                          align: 'stretch', margin: '0 0 0 0', padding: '0 0 0 0',
                          pack: 'start',
                          items: [
                              {
                                  xtype: "label",
                                  text: "附件:", margin: '8 0 8 0',
                                  width: 55
                              },
                             {
                                 layout: "vbox",
                                 width: '100%', baseCls: 'my-panel-no-border',//隐藏边框
                                 align: 'stretch', margin: '0 0 0 0', padding: '0 0 0 0',
                                 pack: 'start', flex: 1,
                                 items: [
                                     me.select1CheckBox,
                                     me.select2CheckBox
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
                                  me.send_create_A5();
                              }
                          }
                      },
                      {
                          xtype: "button",
                          text: "取消", width: 60, margins: "10 15 10 5",
                          listeners: {
                              "click": function (btn, e, eOpts) {//添加点击按钮事件
                                  winA5.close();
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
            me.fileCodePanel.documentTypeText.setValue("A.5");
            me.fileCodePanel.getFileCodeDefaultInfo();

        }
    },

    //创建A5联系单
    send_create_A5: function () {
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
        var content = me.contentText.value;
        var projKeyword = me.projectKeyword;

        
        var amountCount = me.amountCountText.value;
        var scaleCount = me.scaleCountText.value;

        var select1 = me.select1CheckBox.checked;
        var select2 = me.select2CheckBox.checked;

        //获取表格数据，转换成JSON字符串
        var datar = new Array();
        var contentJson = "";
        var records = me.contentStore.getRange();
        for (var i = 0; i < records.length; i++) {
            if (records[i].data.name != "") {
                datar.push(records[i].data);
            }
        }

        var docAttr =
            [
                { name: 'amountCount', value: amountCount },
                { name: 'scaleCount', value: scaleCount },
                { name: 'select1', value: select1 },
                { name: 'select2', value: select2 },
                { name: 'auditAry', value: datar }
            ];
        var docAttrJson = Ext.JSON.encode(docAttr);

        Ext.require('Ext.plug_ins.ZHEPC_Plugins.Dist.ReportComm', function () {
            var mw = Ext.create('Ext.plug_ins.ZHEPC_Plugins.Dist.ReportComm');
            mw.winByName = winA5;
            mw.createDocument(me, 'A.5', projKeyword, fileCode, title, docAttrJson);
        });
    }

});