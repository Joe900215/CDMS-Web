//收文处理
Ext.define('Ext.plug_ins.HXPC_Plugins.getDocument', {
    extend: 'Ext.container.Container',
    alias: 'widget.getDocument',
    layout: 'fit',
    resultvalue: '', mainPanelId: '',
    projectKeyword: '',docKeyword: '',
    initComponent: function () {
        var me = this;

        //定义发文单位combo初始数据
        me.companydata = [];

        //添加项目号text
        me.ProjectText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "", labelWidth: 0, readOnly: true, fieldStyle: 'background-color: #DFE9F6;border-color: #DFE9F6; background-image: none;',//fieldStyle:'color:red',
            anchor: "80%", labelAlign: "right", width: 150//flex: 1
        });

        //添加函件流水号text
        me.NumberText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: " - G ", labelWidth: 20, labelSeparator: '', // 去掉laebl中的冒号
            anchor: "80%", labelAlign: "right", width: 150//flex: 1
        });

        //添加函件描述text
        me.DescText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "", labelWidth: 0, labelSeparator: '', // 去掉laebl中的冒号
            anchor: "80%", labelAlign: "right", flex: 1//width: 150//
        });

        //添加收文单位combo
        Ext.define("companyModel", {
            extend: 'Ext.data.Model',
            fields: ["text", "value"]
        });
        me.companyProxy = Ext.create("Ext.data.proxy.Memory", {
            data: me.companydata,
            model: "companyModel"
        });

        me.companyStore = Ext.create("Ext.data.Store", {
            model: companyModel,
            proxy: me.companyProxy
        });


        me.companyCombo = Ext.create("Ext.form.field.ComboBox",
        {
            //xtype: "combo",
            fieldLabel: ' - ', labelWidth: 10, labelSeparator: '', // 去掉laebl中的冒号
            triggerAction: "all", store: me.companyStore,
            valueField: 'value', editable: false,//不可输入
            displayField: 'text', margin: '0 0 0 10',// 
            anchor: "80%", labelAlign: "left", width: 150,//
            emptyText: "--请选择--", autoLoadOnValue: true,
            //fieldStyle: 'border-color: red; background-image: none;',//红色边框
            listeners:
            {
                select: function (combo, records, eOpts) {
                }
            }
        });

        //添加列表
        me.items = [
          Ext.widget('form', {
              layout: "form",
              items: [

                  {
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
                                  text: '收 文 处 理 表 单', margins: '8 0 0 10'
                              }, { baseCls: 'my-panel-no-border', flex: 1 }]
                          },
                          {
                              xtype: "fieldset", margin: '0 16 16 16',
                              baseCls: 'my-panel-no-border',//隐藏边框
                              layout: {
                                  type: 'vbox',
                                  pack: 'start',
                                  align: 'stretch'
                              },
                              items: [
                                  {
                                      xtype: "fieldset", margin: '8 0 0 0',
                                      title: "函件编码",
                                      layout: {
                                          type: 'vbox',
                                          pack: 'start',
                                          align: 'stretch'
                                      },
                                      items: [
                                          {
                                              baseCls: 'my-panel-no-border',//隐藏边框
                                              layout: {
                                                  type: 'hbox',
                                                  pack: 'start',
                                                  align: 'stretch'
                                              },
                                              items: [
                                                  me.ProjectText,
                                                  me.companyCombo,
                                                  me.NumberText
                                              ],
                                              flex: 1
                                          },                                         {
                                              baseCls: 'my-panel-no-border',//隐藏边框
                                              layout: {
                                                  type: 'hbox',
                                                  pack: 'start',
                                                  align: 'stretch'
                                              },
                                              margin: '0 0 0 0',
                                              items: [
                                                   {
                                                       xtype: "label",
                                                       text: "项目号",
                                                       margin: '3 55 0 55'
                                                   },
                                                   {
                                                       xtype: "label",
                                                       text: "发文单位",
                                                       margin: '3 45 0 75'
                                                   },
                                                   {
                                                       xtype: "label",
                                                       text: "函件流水号",
                                                       margin: '3 55 0 55'
                                                   },
                                              ],
                                              flex: 1
                                          }
                                      ],
                                      flex: 1
                                  },
                                  {
                                      xtype: "fieldset", margin: '8 0 0 0',
                                      title: "函件描述",
                                      layout: {
                                          type: 'hbox',
                                          pack: 'start',
                                          align: 'stretch'
                                      },
                                      items: [
                                            me.DescText
                                      ],
                                      flex: 1
                                  }]
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
                              text: "确定", width: 60, margins: "0 5 10 5",
                              listeners: {
                                  "click": function (btn, e, eOpts) {//添加点击按钮事件
                                      me.send_ReceiveDocument();

                                  }
                              }
                          },
                          {
                              xtype: "button",
                              text: "取消", width: 60, margins: "0 15 10 5",
                              listeners: {
                                  "click": function (btn, e, eOpts) {//添加点击按钮事件
                                      //Ext.Msg.alert("您展开了目录树节点！！！", "您展开了目录树节点！节点ID:" + me.tempDefnId);
                                      winGetDocument.close();
                                  }
                              }
                          }
                      ]
                  }
                      ]
                  }
              ]
          })];

        //获取打开表单时的默认参数
        me.sendGetReceiveDocumentDefault();


me.callParent(arguments);
    },

    //获取打开表单时的默认参数
    sendGetReceiveDocumentDefault: function () {
        var me = this;

        //通过extjs的ajax获取操作全部名称
        Ext.Ajax.request({
            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.HXPC_Plugins.Document", A: "GetReceiveDocumentDefault",
                sid: localStorage.getItem("sid"), DocKeyword: me.docKeyword
            },
            success: function (response, options) {
                me.sendGetReceiveDocumentDefault_callback(response, options);//, funCallback);

            }
        });

    },

    //处理获取收文处理表单默认参数的返回
    sendGetReceiveDocumentDefault_callback: function (response, options) {
        var me = this;

        //获取数据后，更新窗口
        var res = Ext.JSON.decode(response.responseText, true);
        var state = res.success;
        if (state === true) {
            var recod = eval(res.data[0]);

            var strRootProjectCode = recod.RootProjectCode;
            var strDocNumber = recod.DocNumber;
            var strCompany = recod.Company;

            me.ProjectText.setValue(strRootProjectCode);
            me.NumberText.setValue(strDocNumber);


            me.companydata.push({ text: strCompany, value: strCompany });//在数组里添加新的元素  


            me.companyCombo.setRawValue(me.companydata[0].text);//设置显示值
            me.companyCombo.setValue(me.companydata[0].value); //设置ID值

        }
    },

    //响应收文处理表单的确定按钮
    send_ReceiveDocument: function () {
        var me = this;

        //获取项目号Text
        var strRootProject = me.ProjectText.value;

        //获取收文单位
        var strCompany = me.companyCombo.value;

        //获取函件流水号Text
        var strNumber = me.NumberText.value;

        //获取函件主题Text
        var strDesc = me.DescText.value;

        //获取表单数据，转换成JSON字符串
        var docAttr =
        [
            { name: 'RootProject', value: strRootProject },
            { name: 'Company', value: strCompany },
            { name: 'Number', value: strNumber },
            { name: 'Title', value: strDesc }
        ]

        var docAttrJson = Ext.JSON.encode(docAttr);

        Ext.MessageBox.wait("正在创建收文处理单，请稍候...", "等待");

        Ext.Ajax.request({

            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.HXPC_Plugins.Document", A: "ReceiveDocument",
                sid: localStorage.getItem("sid"), DocKeyword: me.docKeyword,
                docAttrJson: docAttrJson
            },
            success: function (response, options) {

                //获取数据后，更新窗口
                var res = Ext.JSON.decode(response.responseText, true);
                var state = res.success;
                if (state === true) {

                    Ext.MessageBox.close();//关闭等待对话框

                    var recod = eval(res.data[0]);

                    me.projectKeyword = recod.ProjectKeyword;//获取新建的目录id


                         //处理返回事件
                    me.send_receiveDocument_callback(response, options, "");//, me.projectKeyword, closeWin);

                } else {
                    var errmsg = res.msg;
                    Ext.Msg.alert("错误信息", errmsg);
                }
            }

        })
    },

    send_receiveDocument_callback: function (response, options) {
        var me = this;
        me.refreshWin(me.projectKeyword, true);
    },

    //刷新表单，参数:parentKeyword:新建的联系单目录
    refreshWin: function (projKeyword, closeWin) {
        var me = this;
        var tree = Ext.getCmp(me.mainPanelId).up('_mainSourceView').down('_mainProjectTree').down('treepanel');//;
        var viewTreeStore = tree.store;

        viewTreeStore.load({
            callback: function (records, options, success) {//添加回调，获取子目录的文件数量
                if (closeWin)
                    winGetDocument.close();

                //展开目录
                Ext.require('Ext.ux.Common.comm', function () {
                    Ext.getCmp('contentPanel').down('_mainProjectTree').ExpendProject(projKeyword);
                });
            }
        });
    }

});