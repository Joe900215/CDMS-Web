//选择项目设总
Ext.define('Ext.plug_ins.m.HXPC_Plugins.selectProfession', {
    extend: 'Ext.container.Container',
    alias: 'widget.selectProfession',
    //layout: "border",
    layout: 'fit',
    resultvalue: '', mainPanelId: '', projectDesc: '',
    projectKeyword: '',
    reSelect:'false',//是否重新选择专业
    initComponent: function () {
        var me = this;

        //设计阶段combo
        me.designPhasedata = [];//{ text: "C__初步设计", value: "C__初步设计" }, { text: "G__初步可行性研究", value: "G__初步可行性研究" },
            //{ text: "H__运行回访", value: "H__运行回访" }, { text: "K__可行性研究", value: "K__可行性研究" },
            //{ text: "S__施工图设计", value: "S__施工图设计" }, { text: "Z__竣工图设计", value: "Z__竣工图设计" }];

        //添加设计阶段combo
        Ext.define("designPhaseModel", {
            extend: 'Ext.data.Model',
            fields: ["text", "value"]
        });
        me.designPhaseProxy = Ext.create("Ext.data.proxy.Memory", {
            data: me.designPhasedata,
            model: "designPhaseModel"
        });

        me.designPhaseStore = Ext.create("Ext.data.Store", {
            model: designPhaseModel,
            proxy: me.designPhaseProxy
        });

        me.designPhaseCombo = Ext.create("Ext.form.field.ComboBox",
        {
            xtype: "combo",
            fieldLabel: "设计阶段",
            triggerAction: "all", store: me.designPhaseStore,
            valueField: 'value', editable: false,//不可输入
            displayField: 'text', //hideLabel: true,
            anchor: "80%", labelAlign: "left", labelWidth: 66,//, margins: "8"
            width: 170, emptyText: "--请选择--", autoLoadOnValue: true,
            readOnly: true,//false,//
            listeners:
            {
                "select": function (combo, record, eOpts) {
                    //获取已创建专业
                    
                    me.send_get_designphase_profession(record.data.value, function () {
                    });
                }
            }
        });

        me.selAllCheckBox = new Ext.form.Checkbox(
          {
              boxLabel: '所有专业',//obj.Table[i].Title; "Title"指的是返回的名字
              inputValue: 'A',
              checked: false,
              listeners: {
                  "change":function (field, newValue, oldValue, eOpts) {
                      me.getReceiverTextValue(field, newValue, oldValue, eOpts);
                  }
              }
          });

        //----------------------复选组开始----------------------//
        me.checkboxgroup = new Ext.form.CheckboxGroup({
            columns: 2,
            width: 170,

            //动态加载专业checkbox
            listeners: {
                "render": function (view, opt) {
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
            title: '专业 - 主设人列表',
            height:280,
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
                              xtype: "panel", margin: '8',
                              baseCls: 'my-panel-no-border',//隐藏边框
                              layout: {
                                  type: 'vbox',
                                  pack: 'start',
                                  align: 'stretch'
                              },
                              items: [
                                  me.designPhaseCombo,
                                 me.checkboxfieldset
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
                              text: "确定", width: 60, margins: "10 5 10 5",
                              listeners: {
                                  "click": function (btn, e, eOpts) {//添加点击按钮事件
                                      me.send_set_profession();

                                  }
                              }
                          },
                          {
                              xtype: "button",
                              text: "取消", width: 60, margins: "10 15 10 5",
                              listeners: {
                                  "click": function (btn, e, eOpts) {//添加点击按钮事件
                                      //Ext.Msg.alert("您展开了目录树节点！！！", "您展开了目录树节点！节点ID:" + me.tempDefnId);
                                      winSelectProfession.close();
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
                C: "AVEVA.CDMS.HXPC_Plugins.Profession", A: "GetProfessionList",
                sid: localStorage.getItem("sid")
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
                      inputValue: recod[itemKey],
                      checked: false
                  });
                me.checkboxgroup.items.add(checkbox);
 
            }

            //var recodDesignPhase = eval(res.data[1]);
            //遍历设计阶段数组，添加到设计阶段combo
            var recDesignPhase = eval(res.data[1]);
            for (var itemKey in recDesignPhase) {

                var itemText = recDesignPhase[itemKey];

                me.designPhasedata.push({ text: itemText, value: itemText });//在数组里添加新的元素  
            };

            me.designPhaseStore.proxy.setData(me.designPhasedata);

            me.designPhaseStore.load();//必须loadStore后再设置combobox的值

        }
        funCallback();
    },

    //获取已创建专业（当所选目录时项目阶段目录）
    send_get_created_profession: function (funCallback) {
        var me = this;



        //通过extjs的ajax获取操作全部名称
        Ext.Ajax.request({
            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.HXPC_Plugins.Profession", A: "GetCreatedProfession",
                sid: localStorage.getItem("sid"), ProjectKeyword: me.projectKeyword
            },
            success: function (response, options) {
                me.send_get_created_profession_callback(response, options, funCallback);

            }
        });
    },

    //获取专业列表的返回
    send_get_created_profession_callback: function (response, options, funCallback) {
        var me = this;

        //先取消所有选中的专业
        for (var i = 0; i < me.checkboxgroup.items.items.length; i++) {
            me.checkboxgroup.items.items[i].setValue(false);
        }

        //获取数据后，更新窗口
        var res = Ext.JSON.decode(response.responseText, true);
        var state = res.success;
        if (state === true) {
            var recod = eval(res.data[0]);

            //me.checkboxfieldset.doLayout(); //重新调整版面布局 
            me.projectDesc = recod.DesignPhase;

            //设置项目阶段
            me.designPhaseCombo.setRawValue(me.projectDesc);//设置显示值
            me.designPhaseCombo.setValue(me.projectDesc); //设置ID值

            var strCreatedProfession=recod.CreatedProfession;

            if (strCreatedProfession != undefined) {
                for (var i = 0; i < me.checkboxgroup.items.items.length; i++) {
                    if (strCreatedProfession.indexOf(me.checkboxgroup.items.items[i].boxLabel) >= 0) {
                        me.checkboxgroup.items.items[i].setValue(true);
                    }
                }
            } else if (me.projectDesc === undefined) {
                //新建设计阶段
                me.designPhaseCombo.setReadOnly(false);

            }
        }
        //me.checkboxfieldset.doLayout(); //重新调整版面布局 
        funCallback();
    },

    //获取已创建专业（当所选目录是项目根目录）
    send_get_designphase_profession: function (designPhaseCode, funCallback) {
        var me = this;
        //通过extjs的ajax获取操作全部名称
        Ext.Ajax.request({
            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.HXPC_Plugins.Profession", A: "GetDesignPhaseProfession",
                sid: localStorage.getItem("sid"), ProjectKeyword: me.projectKeyword,
                DesignPhaseCode: designPhaseCode
            },
            success: function (response, options) {
                me.send_get_created_profession_callback(response, options, funCallback);

            }
        });
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

                    //var nodes = Ext.getCmp(me.mainPanelId).up('_mainSourceView').down('_mainProjectTree').down('treepanel').getSelectionModel().getSelection();//获取已选取的节点 
                    //if (nodes !== null && nodes.length > 0) {

                    //    me.projectKeyword = nodes[0].data.Keyword;

                        var professionList = "";
                        var designPhase=me.designPhaseCombo.value;
                        //获取通过checkboxgroup定义的checkbox值，转换成JSON字符串
                        var checkgroupValue = me.checkboxgroup.getChecked();
                        var professionList;
                        var aryIndex = 0;
                        Ext.Array.each(checkgroupValue, function (item) {
                            if (item.boxLabel != '所有专业') {//把第一个'所有专业'去掉
                                if (aryIndex != 0)
                                    professionList += ',';
                                //professionList += item.inputValue + "__" + item.boxLabel;
                                professionList += item.boxLabel;
                            }
                            aryIndex = aryIndex + 1;
                        });

                        Ext.MessageBox.wait("正在创建专业，请稍候...", "等待");

                        Ext.Ajax.request({

                            url: 'WebApi/Post',
                            method: "POST",
                            params: {
                                C: "AVEVA.CDMS.HXPC_Plugins.Profession", A: "SetProfession",
                                sid: localStorage.getItem("sid"), prjoectKeyword: me.projectKeyword,
                                designPhase: designPhase , professionList: professionList
                            },
                            success: function (response, options) {

                                me.send_set_profession_callback(response, options, "");
                            }
                        })
                    }
                //}
            }
        });
    },

    //处理任命项目设总服务端返回事件
    send_set_profession_callback: function (response, options) {

        var me = this;

        var res = Ext.JSON.decode(response.responseText, true);
        var state = res.success;

        if (state === false) {
            var errmsg = res.msg;
            Ext.Msg.alert("错误信息", errmsg);
        }
        else {

            Ext.MessageBox.close();//关闭等待对话框

            me.refreshWin(me.projectKeyword, true);
        }

    },

    //根据project列表，启动流程
    StartWorkFlowByProjectList: function (projectList, projectIndex) {
        var me = this;
        if (projectList.length-1 >= projectIndex) {
            var projKeyword = projectList[projectIndex].projKeyword;
            var userlist = projectList[projectIndex].userlist;
            Ext.require('Ext.ux.Common.m.comm', function () {
                //参数：doclist,wfKeyword,userlist,callback_fun
                StartNewWorkFlow(projKeyword, "SELECTPROFESSIONFLOW", userlist, function (res, WorkFlowKeyword, CuWorkStateCode) {
                    if (projectList.length-1 >= projectIndex + 1) {
                        me.StartWorkFlowByProjectList(projectList, projectIndex + 1);
                    } else {
                        //Ext.Msg.alert("信息", "完成了所有专业的流程启动！");
                        me.refreshWin(projKeyword, true);
                    }
                }
                )
            });
        }
    },

    //刷新表单，参数:parentKeyword:要转到的的目录
    refreshWin: function (projKeyword, closeWin) {
        var me = this;

        //调用流程页事件，刷新父控件内容
        var mainPanel = Ext.getCmp(me.mainPanelId);
        mainPanel.refreshWin(projKeyword, function () {
            if (closeWin)
                winSelectProfession.close();
        });
    }

    
});