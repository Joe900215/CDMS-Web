//选择专业
Ext.define('Ext.plug_ins.HXPC_Plugins.receiveDocSelectProfession', {
    extend: 'Ext.container.Container',
    alias: 'widget.receiveDocSelectProfession',
    //layout: "border",
    layout: 'fit',
    resultvalue: '', mainPanelId: '',//流程按钮所在的流程页的控件ID
    projectDesc: '',
    projectKeyword: '',wfKeyword:'',
    reSelect: 'false',//是否重新选择专业
    initComponent: function () {
        var me = this;

        //添加处理意见Text
        me.contextText = Ext.create("Ext.form.field.TextArea", {
            xtype: "textarea", anchor: "80%", labelWidth: 0, labelAlign: "right", margin: '0 0 5 0', width: 360,
            value: "请各专业主设处理", flex: 1 //height: 60
        });

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
            title: '请为该文件选择分发的专业',
            height: 210,
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
                                 me.checkboxfieldset,
                          {
                              xtype: "fieldset",
                              margin: '8 8 0 8',
                              title: '处理意见',
                              height: 120,
                              //baseCls: 'my-panel-no-border',//隐藏边框
                              layout: {
                                  type: 'vbox',
                                  pack: 'start',
                                  align: 'stretch'
                              },
                              items: [
                                   // me.checkboxgroup
                                   me.contextText
                              ]
                          }
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
                              text: "分发专业处理", width: 100, margins: "10 5 10 5",
                              listeners: {
                                  "click": function (btn, e, eOpts) {//添加点击按钮事件
                                      me.send_set_profession();

                                  }
                              }
                          },
                          {
                              xtype: "button",
                              text: "取消", width: 80, margins: "10 15 10 5",
                              listeners: {
                                  "click": function (btn, e, eOpts) {//添加点击按钮事件
                                      //Ext.Msg.alert("您展开了目录树节点！！！", "您展开了目录树节点！节点ID:" + me.tempDefnId);
                                      winReceiveDocSelectProfession.close();
                                  }
                              }
                          }
                      ]
                  }
                      ]
                  }
              ]
          })];

        //设置项目阶段
        //me.designPhaseCombo.setValue(me.projectDesc);

        //获取专业列表
        //me.send_get_profession_list();
        me.callParent(arguments);
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

    //获取专业列表
    send_get_profession_list: function (funCallback) {
        var me = this;
        //通过extjs的ajax获取操作全部名称
        Ext.Ajax.request({
            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.HXPC_Plugins.Document", A: "GetProfessionList",
                sid: localStorage.getItem("sid"), WorkFlowKeyword:me.wfKeyword
            },
            success: function (response, options) {
                me.send_get_profession_list_callback(response, options, funCallback);

            }
        });
    },

    //获取专业列表的返回
    send_get_profession_list_callback: function (response, options, funCallback) {
        var me = this;
        //获取数据后，更新窗口
        var res = Ext.JSON.decode(response.responseText, true);
        var state = res.success;
        if (state === true) {
            var recod = eval(res.data[0]);

            //遍历设计阶段数组，添加到设计阶段combo
            for (var itemKey in recod) {
                var checkbox = new Ext.form.Checkbox(
                  {
                      boxLabel: recod[itemKey],//obj.Table[i].Title; "Title"指的是返回的名字
                      name: recod[itemKey],
                      inputValue: itemKey,//projectKeyword
                      checked: false
                  });
                me.checkboxgroup.items.add(checkbox);

            }


            //me.checkboxgroup.remove(me.checkboxgroup.items[0]);
            //me.checkboxfieldset.doLayout(); //重新调整版面布局 
        }
        funCallback();
    },

    //设置所选专业
    send_set_profession: function () {
        var me = this;

        Ext.MessageBox.show({
            title: '',
            msg: '是否确定选择这些专业？',
            buttons: Ext.MessageBox.YESNO,
            buttonText: {
                yes: "是",
                no: "否"
            },
            fn: function (btn, parentFuctionName) {
                if (btn === "yes") {

                    ////判断是在消息页还是在项目数据页
                    //var isInMsgPage = false;
                    //var nodes;

                    //var wfPage = Ext.getCmp(me.mainPanelId);

                    //var msView = wfPage.up('_mainSourceView');
                    //if (msView === undefined) {
                    //    isInMsgPage = true;
                    //}
                    //else {
                    //    nodes = msView.down('_mainProjectTree').down('treepanel').getSelectionModel().getSelection();//获取已选取的节点 

                    //    if (nodes === null || nodes === undefined || nodes.length <= 0) {
                    //        Ext.Msg.alert("错误消息", "获取文件夹节点错误！");
                    //        return;
                    //    } 

                    //    me.projectKeyword = nodes[0].data.Keyword;
                    //}

                    //if (isInMsgPage === true || (nodes !== null && nodes.length > 0)) {

                        var professionList = "";
                        //var designPhase = me.designPhaseCombo.value;
                        //获取通过checkboxgroup定义的checkbox值，转换成JSON字符串
                        var checkgroupValue = me.checkboxgroup.getChecked();
                        var professionList;
                        var aryIndex = 0;
                        Ext.Array.each(checkgroupValue, function (item) {
                            if (item.boxLabel != '所有专业') {//把第一个'所有专业'去掉
                                if (aryIndex != 0)
                                    professionList += ',';
                                //professionList += item.inputValue + "__" + item.boxLabel;
                                //这里传递的是project的keyword
                                professionList += item.inputValue;//item.boxLabel;
                            }
                            aryIndex = aryIndex + 1;
                        });

                        Ext.MessageBox.wait("正在分发专业处理，请稍候...", "等待");

                        Ext.Ajax.request({

                            url: 'WebApi/Post',
                            method: "POST",
                            params: {
                                C: "AVEVA.CDMS.HXPC_Plugins.Document", A: "SetReceiveDocProfession",
                                sid: localStorage.getItem("sid"), WorkFlowKeyword: me.wfKeyword,
                                 professionList: professionList
                            },
                            success: function (response, options) {

                                //获取数据后，更新窗口
                                var res = Ext.JSON.decode(response.responseText, true);
                                var state = res.success;
                                if (state === true) {

                                    Ext.MessageBox.close();//关闭等待对话框

                                    var recod = eval(res.data[0]);

                                    me.projectKeyword = recod.ProjectKeyword;//获取新建的目录id

                                    me.send_set_receiveDocProfession_callback(response, options, "");

                                } else {
                                    var errmsg = res.msg;
                                    Ext.Msg.alert("错误信息", errmsg);
                                }
                            }
                        })
                    //}
                }
            }
        });
    },

    //处理选择专业的返回事件
    send_set_receiveDocProfession_callback(response, options) {
        var me = this;
        me.refreshWin(me.projectKeyword, true);
    },

    //刷新表单，参数:parentKeyword:要转到的的目录
    refreshWin: function (projKeyword, closeWin) {
        var me = this;

        //调用流程页事件，刷新父控件内容
        Ext.getCmp(me.mainPanelId).refreshMainPanle(projKeyword,function () {
            if (closeWin)
                winReceiveDocSelectProfession.close();
        });
    }

});