//选择专业
Ext.define('Ext.plug_ins.HXPC_Plugins.productSelectProfession', {
    extend: 'Ext.container.Container',
    alias: 'widget.productSelectProfession',
    //layout: "border",
    layout: 'fit',
    resultvalue: '', mainPanelId: '', projectDesc: '',
    projectKeyword: '', docKeyword: '',
    reSelect: 'false',//是否重新选择专业
    initComponent: function () {
        var me = this;

        //添加签收人text
        me.ReceiverText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "会签人", labelWidth: 45, readOnly: true, margin: '8 8 0 5',
            fieldStyle: 'background-color: #DFE9F6;border-color: #DFE9F6; background-image: none;',//fieldStyle:'color:red',
            anchor: "80%", labelAlign: "right", width: "100%"//flex: 1
        });

        ////添加处理意见Text
        //me.contextText = Ext.create("Ext.form.field.TextArea", {
        //    xtype: "textarea", anchor: "80%", labelWidth: 0, labelAlign: "right", margin: '0 0 5 0', width: 360,
        //    value: "请各专业主设处理", flex: 1 //height: 60
        //});

        me.selAllCheckBox = new Ext.form.Checkbox(
          {
              boxLabel: '所有专业',//obj.Table[i].Title; "Title"指的是返回的名字
              inputValue: 'A',
              checked: false,
              listeners: {
                  change:function (field, newValue, oldValue, eOpts) {
                      me.getReceiverTextValue(field, newValue, oldValue, eOpts);
                  }
              }
          });

        //----------------------复选组开始----------------------//
        me.checkboxgroup = new Ext.form.CheckboxGroup({
            columns: 3,
            width: 170,

            //动态加载专业checkbox
            listeners: {
                render: function (view, opt) {
                    LoadingOperationBehavior();
                }
            },

            items: [
                me.selAllCheckBox
            ]

        });

        me.checkboxfieldset = new Ext.form.FieldSet({
            //xtype: "fieldset",
            margin: '8',
            title: '会签专业',
            height: 250,
            //baseCls: 'my-panel-no-border',//隐藏边框
            layout: {
                type: 'vbox',
                pack: 'start',
                align: 'stretch'
            },
            items: [
                  me.checkboxgroup
            ]
        });

        //动态加载专业checkbox
        function LoadingOperationBehavior() {
        }

        ////获取复选组的值
        //checkboxgroup.on('change', function (cbgroup, checked) {
        //    for (var i = 0; i < checked.length; i++) {
        //        alert(checked[i].getRawValue());
        //    }
        //});
        //----------------------复选组结束----------------------//

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
                              xtype: "fieldset", margin: '8',
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
                                          text: '选择成品校审会签专业', margins: '0 0 0 10'
                                      }, { baseCls: 'my-panel-no-border', flex: 1 }]
                                  },
                                 me.checkboxfieldset,
                                 me.ReceiverText

                              ],
                              flex: 1
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
                              text: "会签", width: 80, margins: "10 5 10 5",
                              listeners: {
                                  "click": function (btn, e, eOpts) {//添加点击按钮事件
                                      me.sendSetProductSign();

                                  }
                              }
                          },
                          {
                              xtype: "button",
                              text: "不会签", width: 80, margins: "10 15 10 5",
                              listeners: {
                                  "click": function (btn, e, eOpts) {//添加点击按钮事件
                                      if (me.ReceiverText.value != "" && me.ReceiverText.value != undefined) {
                                          Ext.MessageBox.show({
                                              title: '',
                                              msg: '您已经选择了会签的专业，是否确定不需要会签？',
                                              buttons: Ext.MessageBox.YESNO,
                                              buttonText: {
                                                  yes: "是",
                                                  no: "否"
                                              },
                                              fn: function (btn, parentFuctionName) {
                                                  if (btn === "yes") {
                                                      me.ReceiverText.setValue("");
                                                      me.sendSetProductSign();
                                                  }
                                              }
                                              //winProductSelectProfession.close();
                                          })
                                      } else {
                                          me.sendSetProductSign();
                                      }
                                  }
                              }
                          }
                      ]
                  }
                      ]
                  }
              ]
          })];

        me.callParent(arguments);
    },

    //获取专业列表
    sendGetProductSignDefault: function (funCallback) {
        var me = this;
        //通过extjs的ajax获取操作全部名称
        Ext.Ajax.request({
            url: 'WebApi/Post',
            method: "POST",
            params: {
                //C: "AVEVA.CDMS.HXPC_Plugins.Product", A: "GetProductSignDefault",
                C: "AVEVA.CDMS.HXPC_Plugins.Prodect", A: "GetProductSignDefault",
                sid: localStorage.getItem("sid"), DocKeyword: me.docKeyword
            },
            success: function (response, options) {
                me.send_GetProductSignDefault_callback(response, options, funCallback);

            }
        });
    },

    send_GetProductSignDefault_callback: function (response, options, funCallback) {
        var me = this;
        //获取数据后，更新窗口
        var res = Ext.JSON.decode(response.responseText, true);
        var state = res.success;
        if (state === true) {
            var recod = eval(res.data[0]);

            //获取专业列表
            var PressionList = eval(recod.PressionList);

            //遍历设计阶段数组，添加到设计阶段combo
            for (var itemKey in PressionList) {
                if (PressionList[itemKey].Pression != undefined) {
                    var checkValue = false;

                    var checkbox = new Ext.form.Checkbox(
                      {
                          boxLabel: PressionList[itemKey].Pression,//obj.Table[i].Title; "Title"指的是返回的名字
                          name: PressionList[itemKey].Pression,
                          inputValue: PressionList[itemKey].ProjectKeyword,
                          OwnerUser: PressionList[itemKey].OwnerUser,
                          checked: checkValue,
                          listeners: {
                              change:function (field, newValue, oldValue, eOpts) {
                                  me.getReceiverTextValue(field, newValue, oldValue, eOpts);
                              }
                          }
                      });

                    me.checkboxgroup.items.add(checkbox);
                }
            }

            //me.checkboxfieldset.doLayout(); //重新调整版面布局 
        }
        funCallback();
    },

    //选取所有专业触发的事件
    getReceiverTextValue: function (field, newValue, oldValue, eOpts) {
        //getReceiverTextValue: function (field, isDirty, eOpts) {
        var me = this;
        var strReceiver = "";

        //var newValue = field.checked;

        if (field.boxLabel === "所有专业" && field.inputValue === "A") {
            //如果是通过鼠标点击选择的所有专业
            if (field.hasFocus === true) {
                //把所有专业都选上
                if (newValue === true) {
                    for (var i = 1; i < me.checkboxgroup.items.items.length; i++) {
                        me.checkboxgroup.items.items[i].setValue(true);
                    }
                } else {
                    //取消所有专业的选择
                    for (var i = 1; i < me.checkboxgroup.items.items.length; i++) {
                        me.checkboxgroup.items.items[i].setValue(false);
                    }
                }
            }
        } else {
            if (newValue === false && field.hasFocus === true) {
                me.checkboxgroup.items.items[0].setValue(false);
            }
        }

        //加上hasFocus，保证只运行一次
        if (field.hasFocus === true) {
            for (var i = 0; i < me.checkboxgroup.items.items.length; i++) {

                if (me.checkboxgroup.items.items[i].value === true && me.checkboxgroup.items.items[i].OwnerUser != undefined && me.checkboxgroup.items.items[i].OwnerUser != "") {
                    strReceiver = strReceiver + me.checkboxgroup.items.items[i].OwnerUser + ",";
                }

            }
            if (strReceiver.length > 0)
                strReceiver = strReceiver.substring(0, strReceiver.length - 1);

            me.ReceiverText.setValue(strReceiver);
        }

    },

    //提交成品选择会签专业
    sendSetProductSign:function(){
        var me = this;

        //获取签收人Text
        var Receiver = me.ReceiverText.value;

        Ext.MessageBox.wait("正在创建互提资料单，请稍候...", "等待");

        Ext.Ajax.request({

            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.HXPC_Plugins.Prodect", A: "SetProdectSignProfession",
                sid: localStorage.getItem("sid"), DocKeyword: me.docKeyword,
                Receiver:Receiver
            },
            success: function (response, options) {

                //获取数据后，更新窗口
                var res = Ext.JSON.decode(response.responseText, true);
                var state = res.success;
                if (state === true) {

                    Ext.MessageBox.close();//关闭等待对话框

                    var recod = eval(res.data[0]);

                    me.projectKeyword = recod.ProjectKeyword;//获取新建的目录id
                    me.refreshWin(me.projectKeyword, true);

                } else {
                    var errmsg = res.msg;
                    Ext.Msg.alert("错误信息", errmsg);
                }
            }

        })
    },

    //刷新表单，参数:parentKeyword:新建的联系单目录
    refreshWin: function (projKeyword, closeWin) {
        var me = this;
        var tree = Ext.getCmp(window.activeGridPanelId).up('_mainSourceView').down('_mainProjectTree').down('treepanel');//;
        var viewTreeStore = tree.store;

        viewTreeStore.load({
            callback: function (records, options, success) {//添加回调，获取子目录的文件数量
                if (closeWin)
                    winProductSelectProfession.close();

                //展开目录
                Ext.require('Ext.ux.Common.comm', function () {
                    Ext.getCmp('contentPanel').down('_mainProjectTree').ExpendProject(projKeyword);
                });
            }
        });
    }

});