//新建目录
Ext.define('Ext.plug_ins.ZHEPC_Plugins.Report.winCreateA7', {
    extend: 'Ext.container.Container',
    alias: 'widget.winCreateA7',
    //layout: "border",
    layout: 'fit',
    resultvalue: '', mainPanelId: '',
    winWidth: 780,
    winHeight: 550,
    initComponent: function () {
        var me = this;

        me.rpTypedata = [{ text: "主要管理人员", value: "主要管理人员" },
            { text: "特殊工种", value: "特殊工种" },
            { text: "特种作业人员", value: "特种作业人员" }];

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
        me.titleText = Ext.create("Ext.form.Label", {
            xtype: "label",width: 360,margin: '10 0 10 0',
            text: " 现报上本项目部主要管理人员∕特殊工种/特种作业人员名单及其资格证件，请查验。工程进行中如有调整，将重新统计并上报。"
        });

        //添加 主要管理人员∕特殊工种/特种作业人员 类型combo
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
            displayField: 'text', hideLabel: true, value: "主要管理人员",
            anchor: "80%", labelAlign: "left", width: 120,
            emptyText: "请选择", autoLoadOnValue: true,
            margins: "10 5 0 5"
        });

        me.titlePanel = Ext.create('Ext.panel.Panel', {
            xtype: "panel",
            layout: {
                type: 'hbox',
                align: 'stretch',
                pack: 'start'
            },
            width: '100%', baseCls: 'my-panel-no-border',//隐藏边框
            margin: '8 0 8 0', padding: '0 0 0 0',
            items: [
                {
                    xtype: "label",
                    text: "现报上本项目部", margins: "13 0 0 0"
                },
                me.rpTypeCombo,
                {
                    xtype: "label",
                    text: "名单及其资格证件，请查验。工程进行中如有调整，将重新统计并上报。", margins: "13 0 0 0"
                }
            ]
        });

        me.cellEditing = new Ext.grid.plugin.CellEditing({
            clicksToEdit: 1
        });

        //定义器具的model
        Ext.define("contentModel", {
            extend: "Ext.data.Model",
            //fields: ['姓 名', '岗位/工种', '证件名称', '证件编号', '发证单位', '有效期'],
            fields: ["id", "name", "jobs", "certiName", "certiSerial", "issueUnit",
            //"expiryDate"],
                { name: 'expiryDate', mapping: 'availability', type: 'date', dateFormat: 'Y年m月d日' }],
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
            title: '作业人员名单及其资格证件',
            store: me.contentStore,
            columns: [
                {
                    header: '姓 名', dataIndex: 'name', width: 118,
                    editor: {
                        allowBlank: false
                    }
                },
                {
                    header: '岗位/工种', dataIndex: 'jobs', width: 118,
                    editor: {
                        allowBlank: false
                    }
                },
                {
                    header: '证件名称', dataIndex: 'certiName', width: 118,
                    editor: {
                        allowBlank: false
                    }
                },
                {
                    header: '证件编号', dataIndex: 'certiSerial', width: 118,
                    editor: {
                        allowBlank: false
                    }
                },
                        {
                            header: '发证单位', dataIndex: 'issueUnit', width: 118,
                            editor: {
                                allowBlank: false
                            }
                        },
                {
                    header: '有效期', dataIndex: 'expiryDate', width: 118,
                    editor: {
                        xtype: 'datefield',
                        format: 'Y年m月d日',
                        value: new Date() 
                    }, renderer: Ext.util.Format.dateRenderer('Y年m月d日')
                },
            ],
            stripeRows: true, //斑马线效果  
            plugins: [
                 Ext.create('Ext.grid.plugin.CellEditing', {
                     clicksToEdit: 1 //设置单击单元格编辑  
                 })],
            height: 160,
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
                          text: 'A.7人员资质报审表', margins: '0 0 0 10'
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
                          me.titlePanel,//me.titleText,
                          me.contentGrid,
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
                                  me.send_create_A7();
                              }
                          }
                      },
                      {
                          xtype: "button",
                          text: "取消", width: 60, margins: "10 15 10 5",
                          listeners: {
                              "click": function (btn, e, eOpts) {//添加点击按钮事件
                                  winA7.close();
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
            me.fileCodePanel.documentTypeText.setValue("A.7");
            me.fileCodePanel.getFileCodeDefaultInfo();

        }
    },

    //创建A7联系单
    send_create_A7: function () {
        var me = this;

        //检查文件编码
        var checkResult = me.fileCodePanel.checkFileCodeFill();
        if (checkResult != "true") {
            Ext.Msg.alert("错误信息", checkResult);
            return;
        }

        var fileCode = me.fileCodePanel.getFileCode();

        var projKeyword = me.projectKeyword;

        var rpType = me.rpTypeCombo.value;

        var title = rpType;

        //获取表格数据，转换成JSON字符串
        var datar = new Array();
        var contentJson = "";
        var records = me.contentStore.getRange();
        for (var i = 0; i < records.length; i++) {
            if (records[i].data.name != "" || records[i].data.jobs != "" || records[i].data.certiName != "" || records[i].data.certiSerial != "" || records[i].data.issueUnit != "") {
                datar.push(records[i].data);
            }
        }
        contentJson = Ext.JSON.encode(datar);

        var docAttr =
        [
            { name: 'rpType', value: rpType },
            { name: 'auditAry', value: datar }
        ];
        var docAttrJson = Ext.JSON.encode(docAttr);

        Ext.require('Ext.plug_ins.ZHEPC_Plugins.Dist.ReportComm', function () {
            var mw = Ext.create('Ext.plug_ins.ZHEPC_Plugins.Dist.ReportComm');
            mw.winByName = winA7;
            mw.createDocument(me, 'A.7', projKeyword, fileCode, title, docAttrJson);
        });
    }

});