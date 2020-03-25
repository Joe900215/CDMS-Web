//项目立项
Ext.define('Ext.plug_ins.HXEPC_Plugins.CreateRootProject', {
    extend: 'Ext.container.Container',
    alias: 'widget.CreateRootProject',
    //layout: "border",
    layout: 'fit',
    resultvalue: '',mainPanelId:'',
    initComponent: function () {
        var me = this;

        me.sourceUnitdata = [{ text: "CWEC", value: "CWEC" }, { text: "CWPC", value: "CWPC" },
    { text: "CWIC", value: "CWIC" }, { text: "其他", value: "其他" }];

        me.sourceTypedata = [{ text: "国内项目", value: "国内项目" }, { text: "国际项目", value: "国际项目" }];

        //定义项目代码Text
        me.projectCodeText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "项目代码", anchor: "80%", labelWidth: 60, labelAlign: "left", width: "100%", //width: 230, 
            margin: '0 10 0 10', fieldStyle: 'border-color: red; background-image: none;'//红色边框//flex: 1
        });


        //定义项目名称Text
        me.projectDescCNText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "项目名称", anchor: "80%", labelWidth: 60, labelAlign: "left", width: "100%",//width: 230, 
            margin: '0 10 0 10', fieldStyle: 'border-color: red; background-image: none;'//红色边框//flex: 1
        });
        
        //定义所属公司Text
        me.sourceSuppText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "请补充", anchor: "80%", labelWidth: 50, labelAlign: "right", width: "50%",//width: 230, 
            margin: '0 10 0 10', disabled: true
        });

        //定义所属公司Text
        me.sourceUnitText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "来源名称", anchor: "80%", labelWidth: 60, labelAlign: "left", width: "100%",//width: 230, 
            margin: '0 10 0 10', readOnly: true//, fieldStyle: 'background-color: #DFE9F6;border-color: #DFE9F6; background-image: none;',//fieldStyle:'color:red',
        });

        //定义项目地址Text
        me.projectAddrText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "项目地址", anchor: "80%", labelWidth: 60, labelAlign: "left", width: "100%",//width: 230, 
            margin: '0 10 0 10'
        });

        //定义电话Text
        me.projectTelText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "项目电话", anchor: "80%", labelWidth: 60, labelAlign: "left", width: "100%",//width: 230, 
            margin: '0 10 0 10'
        });

        //定义项目文控Text
        me.secretarilManText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "项目文控", anchor: "80%", labelWidth: 60, labelAlign: "left", labelPad: 8,flex: 1, //width: "40%",//width: 230, 
            margin: '0 10 0 10', fieldStyle: ' background-image: none;'//红色边框//flex: 1
        });

        //添加审批路径combo
        Ext.define("sourceUnitModel", {
            extend: 'Ext.data.Model',
            fields: ["text", "value"]
        });
        me.sourceUnitProxy = Ext.create("Ext.data.proxy.Memory", {
            data: me.sourceUnitdata,
            model: "sourceUnitModel"
        });

        me.sourceUnitStore = Ext.create("Ext.data.Store", {
            model: sourceUnitModel,
            proxy: me.sourceUnitProxy
        });


        me.sourceUnitCombo = Ext.create("Ext.form.field.ComboBox",
        {
            //xtype: "combo",
            fieldLabel: '项目来源', labelWidth: 60,
            triggerAction: "all", store: me.sourceUnitStore,
            valueField: 'value', editable: false,//不可输入
            displayField: 'text', margin: '0 5 0 10',// 
            anchor: "80%", labelAlign: "left", labelPad: 8, width: '50%',//width: 120,//
            emptyText: "--请选择--", autoLoadOnValue: true,
            //fieldStyle: 'border-color: red; background-image: none;',//红色边框
            listeners:
            {
                select: function (combo, records, eOpts) {
                    var sourceUnit = me.sourceUnitCombo.value;
                    //if (sourceUnit === "CWEC") {
                    //    me.sourceUnitText.setValue("华西能源工程有限公司");
                    //} else if (sourceUnit === "CWPC") {
                    //    me.sourceUnitText.setValue("华西能源工业股份有限公司");
                    //} else if (sourceUnit === "CWIC") {
                    //    me.sourceUnitText.setValue("华西能源工业有限公司");
                    //} else if (sourceUnit === "其他") {
                    //    me.sourceUnitText.setValue("");
                    //}

                     if (sourceUnit === "其他") {
                        me.sourceUnitText.setValue("");
                    }

                    if (sourceUnit === "其他") {
                        me.sourceSuppText.setDisabled(false);
                        me.sourceUnitText.setReadOnly(false);
                    } else {
                        me.sourceSuppText.setDisabled(true);
                        me.sourceUnitText.setReadOnly(true);
                    }
                }
            }
        });

        //添加审批路径combo
        Ext.define("sourceTypeModel", {
            extend: 'Ext.data.Model',
            fields: ["text", "value"]
        });
        me.sourceTypeProxy = Ext.create("Ext.data.proxy.Memory", {
            data: me.sourceTypedata,
            model: "sourceTypeModel"
        });

        me.sourceTypeStore = Ext.create("Ext.data.Store", {
            model: sourceTypeModel,
            proxy: me.sourceTypeProxy
        });


        me.sourceTypeCombo = Ext.create("Ext.form.field.ComboBox",
        {
            //xtype: "combo",
            fieldLabel: '项目类型', labelWidth: 60,
            triggerAction: "all", store: me.sourceTypeStore,
            valueField: 'value', editable: false,//不可输入
            displayField: 'text', margin: '0 5 0 10',// 
            anchor: "80%", labelAlign: "left", labelPad: 8, width: '50%',//width: 120,//
            emptyText: "--请选择--", autoLoadOnValue: true, value: "国内项目",
            //fieldStyle: 'border-color: red; background-image: none;',//红色边框
            listeners:
            {
                select: function (combo, records, eOpts) {
                }
            }
        });

        //编辑区域尾部
        me.editBottomPanel = Ext.create("Ext.panel.Panel", {
            layout: "hbox",
            width: '100%',
            align: 'stretch',
            pack: 'start', margins: "0 0 0 0",
            baseCls: 'my-panel-no-border',//隐藏边框
            items: [
                me.secretarilManText,
                      {
                          xtype: "button",
                          text: "选择...", margins: "0 10 0 0",
                          listeners: {
                              "click": function (btn, e, eOpts) {//添加点击按钮事件
                                  Ext.require('Ext.ux.Common.comm', function () {
                                      showSelectUserWin("getUser", "", "", function () {
                                          me.secretarilManText.setValue(window.parent.usernamelist);
                                          me.secretarilManList = window.parent.resultvalue;
                                      });
                                  })
                              }
                          }
                      }
            ]//, flex: 1
        });

        //添加列表
        me.items = [
          Ext.widget('form', {
              baseCls: 'my-panel-no-border',//隐藏边框
              layout: {
                  type: 'vbox',
                  align: 'stretch',
                  pack  : 'start'
              },
              items: [{//上部容器
                  baseCls: 'my-panel-no-border',//隐藏边框
                  layout: {
                      type: 'vbox',
                      pack: 'start',
                      align: 'stretch'
                  },
                  margin: '10 0 0 0',// 
                  items: [
                     {
                         layout: "hbox",
                         width: '100%',
                         align: 'stretch',
                         pack: 'start',
                         baseCls: 'my-panel-no-border',//隐藏边框
                         items: [
                            me.projectCodeText,//项目代码

                         ], flex: 1
                     },
                     {
                         layout: "hbox",
                         width: '100%',
                         align: 'stretch',
                         pack: 'start',
                         baseCls: 'my-panel-no-border',//隐藏边框
                         items: [
                            me.projectDescCNText//项目名称

                         ], flex: 1
                     },
                     {
                         layout: "hbox",
                         width: '100%',
                         align: 'stretch',
                         pack: 'start',
                         baseCls: 'my-panel-no-border',//隐藏边框
                         items: [
                             me.sourceUnitCombo,//所属公司
                             me.sourceSuppText
                         ], flex: 1
                     },
                     {
                         layout: "hbox",
                         width: '100%',
                         align: 'stretch',
                         pack: 'start',
                         baseCls: 'my-panel-no-border',//隐藏边框
                         items: [
                            
                             me.sourceUnitText
                         ], flex: 1
                     },
                     {
                         layout: "hbox",
                         width: '100%',
                         align: 'stretch',
                         pack: 'start',
                         baseCls: 'my-panel-no-border',//隐藏边框
                         items: [
                            
                             me.sourceTypeCombo
                         ], flex: 1
                     },
                     {
                         layout: "hbox",
                         width: '100%',
                         align: 'stretch',
                         pack: 'start',
                         baseCls: 'my-panel-no-border',//隐藏边框
                         items: [

                             me.projectAddrText
                         ], flex: 1
                     },
                      {
                          layout: "hbox",
                          width: '100%',
                          align: 'stretch',
                          pack: 'start',
                          baseCls: 'my-panel-no-border',//隐藏边框
                          items: [

                             me.projectTelText
                          ], flex: 1
                      },
                    
                     me.editBottomPanel
                   
                  ], flex: 1
              },
              {//下部按钮容器
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
                                  me.send_create_root_project();
                              }
                          }
                      },
                      {
                          xtype: "button",
                          text: "取消", width: 60, margins: "10 15 10 5",
                          listeners: {
                              "click": function (btn, e, eOpts) {//添加点击按钮事件
                                  //Ext.Msg.alert("您展开了目录树节点！！！", "您展开了目录树节点！节点ID:" + me.tempDefnId);
                                  winCreateRootProject.close();
                              }
                          }
                      }
                  ]
              }

              ]
          })

        ];


        me.callParent(arguments);
    },


    //创建项目
    send_create_root_project: function () {
        var me = this;

        //获取项目代码Text
        var projectCode = me.projectCodeText.value;

        //获取项目名称Text
        var projectDescCN = me.projectDescCNText.value;

        //获取所属公司
        var sourceUnit = me.sourceUnitCombo.value;

        //项目地址
        var projectAddr=me.projectAddrText.value;

        //项目电话
        var projectTel = me.projectTelText.value;

        var secretarilMan = me.secretarilManText.value;//me.secretarilManList;

        //获取所属公司简称
        if (sourceUnit === "其他") {
            if (me.sourceSuppText.value === undefined || me.sourceSuppText.value.trim() === "")
            {
                Ext.Msg.alert("错误信息", "请补充项目来源！");
                return;
            } else if (me.sourceUnitText.value === undefined || me.sourceUnitText.value.trim() === "") {
                Ext.Msg.alert("错误信息", "请补充项目来源名称！");
                return;
            }
            sourceUnit = me.sourceSuppText.value;
        }
        //获取所属公司代码
        var sourceDesc = me.sourceUnitText.value;

        //获取所属公司类型
        var sourceType = me.sourceTypeCombo.value;

        //获取表单数据，转换成JSON字符串
        var projectAttr =
        [
            { name: 'projectCode', value: projectCode },
            { name: 'projectDescCN', value: projectDescCN },
            { name: 'sourceUnit', value: sourceUnit },
            { name: 'sourceDesc', value: sourceDesc },
            { name: 'sourceType', value: sourceType },
            { name: 'projectAddr', value: projectAddr },
            { name: 'projectTel', value: projectTel },
            { name: 'secretarilMan', value: secretarilMan }
          
        ];

        var projectAttrJson = Ext.JSON.encode(projectAttr);

        Ext.MessageBox.wait("正在创建项目，请稍候...", "等待");

        Ext.Ajax.request({

            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.HXEPC_Plugins.HXProject", A: "CreateRootProject",
                sid: localStorage.getItem("sid"), 
                projectAttrJson: projectAttrJson
            },
            success: function (response, options) {
                me.create_root_project_callback(response, options,"");//, me.projectKeyword, closeWin);
            },
            failure: function (response, options) {
                ////Ext.Msg.alert("错误", "连接服务器失败！<br>" + response.responseText);
            }
        })

    },

    //创建项目后的返回事件
    create_root_project_callback: function (response, options,parentKeyword) {
        var me = this;
        var res = Ext.JSON.decode(response.responseText, true);
        var state = res.success;
        if (state === false) {
            var errmsg = res.msg;
            Ext.Msg.alert("错误信息", errmsg);
        }
        else {

            Ext.MessageBox.close();//关闭等待对话框

            Ext.require('Ext.ux.Common.comm', function () {

                winCreateRootProject.close();

                var projectKeyword = res.data[0].projectKeyword;

                me.refreshWin(projectKeyword, false);

            });
        }
    },

    //刷新表单，参数:parentKeyword:新建的联系单目录
    refreshWin: function (parentKeyword, closeWin) {
        var me = this;
        var tree = Ext.getCmp(me.mainPanelId);//.down('treepanel');
        var viewTreeStore = tree.store;

        viewTreeStore.load({
            callback: function (records, options, success) {//添加回调，获取子目录的文件数量
                if (closeWin)
                    winCreateRootProject.close();

                //展开目录
                Ext.require('Ext.ux.Common.comm', function () {
                    Ext.getCmp('contentPanel').down('_mainProjectTree').ExpendProject(parentKeyword);
                });
            }
        });
    }
});

