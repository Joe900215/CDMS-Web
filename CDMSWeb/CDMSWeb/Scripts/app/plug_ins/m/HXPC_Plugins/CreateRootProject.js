//项目立项
Ext.define('Ext.plug_ins.m.HXPC_Plugins.CreateRootProject', {
    extend: 'Ext.container.Container',
    alias: 'widget.CreateRootProject',
    //layout: "border",
    layout: 'fit',
    resultvalue: '',mainPanelId:'',
    initComponent: function () {
        var me = this;

        //定义设计阶段combo初始数据
        me.designPhasedata = [];//{ text: "C__初步设计", value: "C__初步设计" }, { text: "G__初步可行性研究", value: "G__初步可行性研究" },
            //{ text: "H__运行回访", value: "H__运行回访" }, { text: "K__可行性研究", value: "K__可行性研究" },
        //{ text: "S__施工图设计", value: "S__施工图设计" }, { text: "Z__竣工图设计", value: "Z__竣工图设计" } ];//设计combo

        //定义项目类型combo初始数据
        me.projectTypedata = [];//{ text: "Ⅰ类项目", value: "Ⅰ类项目" }, { text: "Ⅱ类项目", value: "Ⅱ类项目" }];

        //定义二级项目类型combo初始数据
        me.projectTypeIIdata = [];

        //定义项目类型combo初始数据
        me.qualitydata = [{ text: "省优秀I", value: "省优秀I" }];//, { text: "市优", value: "市优" },
            //{ text: "院优", value: "院优" }, { text: "合格", value: "合格" }];

        //定义工程实施性质combo初始数据
        me.projectNaturedata = [];

        //定义项目开发阶段combo初始数据
        me.developmentPhasedata = [];

        //定义合同模式combo初始数据
        me.contractModeldata = [];

        //定义项目代码Text
        me.projectCodeText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "项目编号", anchor: "80%", labelWidth: 60, labelAlign: "left", width: 230, margin: '0 10 0 10', fieldStyle: 'border-color: red; background-image: none;'//红色边框//flex: 1
        });

        //定义建设单位Text
        me.unintNameText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "建设单位", anchor: "80%", labelWidth: 80, labelAlign: "left", width: 230, margin: '0 10 0 10', fieldStyle: ' background-image: none;'//红色边框//flex: 1
        });

        //定义设计依据Text
        me.designBasisText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "设计依据", anchor: "80%", labelWidth: 80, labelAlign: "left", width: 230, margin: '0 10 0 10', fieldStyle: ' background-image: none;'//红色边框//flex: 1
        });


        //定义项目名称Text
        me.projectDescText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "项目名称", anchor: "80%", labelWidth: 60, labelAlign: "left", width: 230, margin: '0 10 0 10', fieldStyle: 'border-color: red; background-image: none;'//红色边框//flex: 1
        });

        //定义甲方联系人Text
        me.unitManText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "甲方联系人", anchor: "80%", labelWidth: 80, labelAlign: "left", width: 230, margin: '0 10 0 10', fieldStyle: ' background-image: none;'//红色边框//flex: 1
        });

        //定义人力资源Text
        me.HRText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "人力资源", anchor: "80%", labelWidth: 80, labelAlign: "left", width: 230, margin: '0 10 0 10', fieldStyle: ' background-image: none;'//红色边框//flex: 1
        });

        //定义甲方联系人电话Text
        me.unitManPhoneText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "甲方联系人电话", anchor: "80%", labelWidth: 90, labelAlign: "left", width: 230, margin: '0 10 0 10', fieldStyle: ' background-image: none;'//红色边框//flex: 1
        });

        //定义主要设计原则Text
        me.designPrincText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "主要设计原则", anchor: "80%", labelWidth: 80, labelAlign: "left", width: 230, margin: '0 10 0 10', fieldStyle: ' background-image: none;'//红色边框//flex: 1
        });

        //定义合同号Text
        me.projectNoText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "合同号", anchor: "80%", labelWidth: 60, labelAlign: "left", width: 230, margin: '0 10 0 10', fieldStyle: ' background-image: none;'//红色边框//flex: 1
        });

        //定义文本编辑规定Text
        me.editRequireText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "文本编辑规定", anchor: "80%", labelWidth: 80, labelAlign: "left", width: 230, margin: '0 10 0 10', fieldStyle: ' background-image: none;'//红色边框//flex: 1
        });
        
        //定义合同额Text
        me.projectAmountText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "合同额", anchor: "80%", labelWidth: 60, labelAlign: "left", width: 230, margin: '0 10 0 10', fieldStyle: 'background-image: none;'//红色边框//flex: 1
        });

        //定义项目规模Text
        me.projectSizeText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "项目规模", anchor: "80%", labelWidth: 60, labelAlign: "left", width: 230, margin: '0 10 0 10', fieldStyle: 'background-image: none;'//红色边框//flex: 1
        });

        //定义建筑面积Text
        me.buildAreaText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "建筑面积", anchor: "80%", labelWidth: 60, labelAlign: "left", width: 160, margin: '0 10 0 10', fieldStyle: ' background-image: none;' //flex: 1
        });


        //定义立项人Text
        me.writeManText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "立项人", anchor: "80%", labelWidth: 80, labelAlign: "left", width: 230, margin: '0 10 0 10', fieldStyle: ' background-image: none;'//红色边框//flex: 1
        });

        
        //添加合同主要条款Text
        me.contractTermsText = Ext.create("Ext.form.field.TextArea", {
            xtype: "textarea", anchor: "80%", labelAlign: "left", margin: '0 0 0 0', width: 460, //width: "100%",
            height: 70 //fieldLabel: "内容",labelWidth: 60,
        });



        //添加工程总结Text
        me.ProjectSummaryText = Ext.create("Ext.form.field.TextArea", {
            xtype: "textarea", anchor: "80%", labelAlign: "left", margin: '0 0 0 0', width: 210, //width: "100%",
            height: 70 //fieldLabel: "内容",labelWidth: 60,
        });

        //添加项目特殊要求Text
        me.RequirementText = Ext.create("Ext.form.field.TextArea", {
            xtype: "textarea", anchor: "80%", labelAlign: "left", margin: '0 0 0 0', width: 460, //width: "100%",//
            height: 60 //fieldLabel: "内容",labelWidth: 60,
        });

        //添加备注Text
        me.RemarksText = Ext.create("Ext.form.field.TextArea", {
            xtype: "textarea", anchor: "80%", labelAlign: "left", margin: '0 0 0 0', width: 210, //width: "100%",//
            height: 60 //fieldLabel: "内容",labelWidth: 60,
        });

        //定义任务下达时间Text
        me.releaseDateField = Ext.create("Ext.form.field.Date", {

            name: "date",
            fieldLabel: ' 任务下达时间', fieldStyle: ' background-image: none;',
            editable: true, labelWidth: 80, margin: '0 10 0 10',
            emptyText: "--请选择--", autoLoadOnValue: true,
            format: 'Y年m月d日',
            value: new Date(),
            width: 230

            });

        //定义计划开始时间Text
        me.planStartDateField = Ext.create("Ext.form.field.Date", {

            name: "date",
            fieldLabel: ' 计划开始时间', fieldStyle: ' background-image: none;',
            editable: true, labelWidth: 80, margin: '0 10 0 10',
            emptyText: "--请选择--", autoLoadOnValue: true,
            format: 'Y年m月d日',
            value: new Date(),
            width: 230
        });


        //定义计划结束时间Text
        me.planEndDateField = Ext.create("Ext.form.field.Date", {

            name: "date",
            fieldLabel: ' 计划结束时间', fieldStyle: ' background-image: none;',
            editable: true, labelWidth: 80, margin: '0 10 0 10',
            emptyText: "--请选择--", autoLoadOnValue: true,
            format: 'Y年m月d日',
            value: new Date(),
            width: 230
        });

        //定义项目类型RadioGroup
        me.projectTypeRadioGroup = Ext.create("Ext.form.RadioGroup", {
            //xtype:'radiogroup',
            //fieldLabel:'文件类别',
            name: 'projectType',  //后台返回的JSON格式，直接赋值；
            width: 120,
            items:[
            { boxLabel: '燃煤', name: 'projectType', inputValue: 1, checked: true },
            { boxLabel: '燃气', name: 'projectType', inputValue: 2 }
            ]
        });
        //获取值：
        //var FileItype = EditfileForm.getForm().findField('projectType').getValue().inputValue;
        //var projectTypeR =me.projectTypeRadioGroup.getValue().inputValue;

        //定义项目规模RadioGroup
        me.projectSizeRadioGroup = Ext.create("Ext.form.RadioGroup", {
            //xtype:'radiogroup',
            //fieldLabel:'文件类别',
            name: 'projectSize',  //后台返回的JSON格式，直接赋值；
            width: 360,// value: 'projectSize1',
            items: [
            { boxLabel: '300MW', name: 'projectSize', inputValue: 1 , checked: true},
            { boxLabel: '600MW', name: 'projectSize', inputValue: 2 },
            { boxLabel: '1000MW', name: 'projectSize', inputValue: 3 },
            { boxLabel: '400MW', name: 'projectSize', inputValue: 4 }
            ]
        });


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
            //xtype: "combo",
            fieldLabel: '设计阶段', labelWidth: 60,
            triggerAction: "all", store: me.designPhaseStore,
            valueField: 'value', editable: false,//不可输入
            displayField: 'text', margin: '0 10 0 10',// 
            anchor: "80%", labelAlign: "left", width: 230,//
            emptyText: "--请选择--", autoLoadOnValue: true,
            fieldStyle: 'border-color: red; background-image: none;',//红色边框
            listeners:
            {
                select: function (combo, records, eOpts) {
                }
            }
        });

        //添加项目类型combo
        Ext.define("projectTypeModel", {
            extend: 'Ext.data.Model',
            fields: ["text", "value"]
        });
        me.projectTypeProxy = Ext.create("Ext.data.proxy.Memory", {
            data: me.projectTypedata,
            model: "projectTypeModel"
        });

        me.projectTypeStore = Ext.create("Ext.data.Store", {
            model: projectTypeModel,
            proxy: me.projectTypeProxy
        });


        me.projectTypeCombo = Ext.create("Ext.form.field.ComboBox",
        {
            //xtype: "combo",
            fieldLabel: '项目类型', labelWidth: 60,
            triggerAction: "all", store: me.projectTypeStore,
            valueField: 'value', editable: false,//不可输入
            displayField: 'text', margin: '0 10 0 10',// 
            emptyText: "--请选择--", autoLoadOnValue: true,
            anchor: "80%", labelAlign: "left", width: 230,
            listeners:
            {
                select: function (combo, records, eOpts) {
                    //获取流水号
                    me.getProjectTypeII();
                }
            }
        });

        //添加二级项目类型combo
        Ext.define("projectTypeIIModel", {
            extend: 'Ext.data.Model',
            fields: ["text", "value"]
        });
        me.projectTypeIIProxy = Ext.create("Ext.data.proxy.Memory", {
            data: me.projectTypeIIdata,
            model: "projectTypeIIModel"
        });

        me.projectTypeIIStore = Ext.create("Ext.data.Store", {
            model: projectTypeIIModel,
            proxy: me.projectTypeIIProxy
        });


        me.projectTypeIICombo = Ext.create("Ext.form.field.ComboBox",
        {
            triggerAction: "all", store: me.projectTypeIIStore,
            valueField: 'value', editable: false,//不可输入
            displayField: 'text', margin: '0 10 0 10',// 
            emptyText: "--请选择--", autoLoadOnValue: true,
            anchor: "80%", labelAlign: "left", width: 230

        });

        //添加工程实施性质combo
        Ext.define("projectNatureModel", {
            extend: 'Ext.data.Model',
            fields: ["text", "value"]
        });
        me.projectNatureProxy = Ext.create("Ext.data.proxy.Memory", {
            data: me.projectNaturedata,
            model: "projectNatureModel"
        });

        me.projectNatureStore = Ext.create("Ext.data.Store", {
            model: projectNatureModel,
            proxy: me.projectNatureProxy
        });


        me.projectNatureCombo = Ext.create("Ext.form.field.ComboBox",
        {
            //xtype: "combo",
            fieldLabel: '工程实施性质', labelWidth: 80,
            triggerAction: "all", store: me.projectNatureStore,
            valueField: 'value', editable: false,//不可输入
            displayField: 'text', margin: '0 10 0 10',// 
            emptyText: "--请选择--", autoLoadOnValue: true,
            anchor: "80%", labelAlign: "left", width: 230

        });

        //添加合同模式combo
        Ext.define("contractModelModel", {
            extend: 'Ext.data.Model',
            fields: ["text", "value"]
        });
        me.contractModelProxy = Ext.create("Ext.data.proxy.Memory", {
            data: me.contractModeldata,
            model: "contractModelModel"
        });

        me.contractModelStore = Ext.create("Ext.data.Store", {
            model: contractModelModel,
            proxy: me.contractModelProxy
        });


        me.contractModelCombo = Ext.create("Ext.form.field.ComboBox",
        {
            //xtype: "combo",
            fieldLabel: '合同模式', labelWidth: 60,
            triggerAction: "all", store: me.contractModelStore,
            valueField: 'value', editable: false,//不可输入
            displayField: 'text', margin: '0 10 0 10',// 
            emptyText: "--请选择--", autoLoadOnValue: true,
            anchor: "80%", labelAlign: "left", width: 230

        });

        //添加项目开发阶段combo
        Ext.define("developmentPhaseModel", {
            extend: 'Ext.data.Model',
            fields: ["text", "value"]
        });
        me.developmentPhaseProxy = Ext.create("Ext.data.proxy.Memory", {
            data: me.developmentPhasedata,
            model: "developmentPhaseModel"
        });

        me.developmentPhaseStore = Ext.create("Ext.data.Store", {
            model: developmentPhaseModel,
            proxy: me.developmentPhaseProxy
        });


        me.developmentPhaseCombo = Ext.create("Ext.form.field.ComboBox",
        {
            //xtype: "combo",
            fieldLabel: '项目开发阶段', labelWidth: 80,
            triggerAction: "all", store: me.developmentPhaseStore,
            valueField: 'value', editable: false,//不可输入
            displayField: 'text', margin: '0 10 0 10',// 
            emptyText: "--请选择--", autoLoadOnValue: true,
            anchor: "80%", labelAlign: "left", width: 230

        });

        //添加质量目标combo
        Ext.define("qualityModel", {
            extend: 'Ext.data.Model',
            fields: ["text", "value"]
        });
        me.qualityProxy = Ext.create("Ext.data.proxy.Memory", {
            data: me.qualitydata,
            model: "qualityModel"
        });

        me.qualityStore = Ext.create("Ext.data.Store", {
            model: qualityModel,
            proxy: me.qualityProxy
        });


        me.qualityCombo = Ext.create("Ext.form.field.ComboBox",
        {
            //xtype: "combo",
            fieldLabel: '质量目标', labelWidth: 80,
            triggerAction: "all", store: me.qualityStore,
            valueField: 'value', editable: false,//不可输入
            displayField: 'text', margin: '0 10 0 10',// 
            anchor: "80%", labelAlign: "left", width: 230,//
            emptyText: "--请选择--", autoLoadOnValue: true,
            listeners:
            {
                select: function (combo, records, eOpts) {
                }
            }
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
                            me.projectCodeText,
                            me.unintNameText, //建设单位
                            me.designBasisText
                            //

                         ], flex: 1
                     }, {
                         layout: "hbox",
                         width: '100%',
                         align: 'stretch',
                         pack: 'start',
                         baseCls: 'my-panel-no-border',//隐藏边框
                         items: [
                             me.projectDescText,//项目名称
                             me.unitManText,
                             me.HRText
                         ], flex: 1
                     }, {
                         layout: "hbox",
                         width: '100%',
                         align: 'stretch',
                         pack: 'start',
                         baseCls: 'my-panel-no-border',//隐藏边框
                         items: [
                          me.designPhaseCombo,
                          me.unitManPhoneText,
                          me.designPrincText
                         ], flex: 1
                     }, {
                         layout: "hbox",
                         //width: '100%',
                         align: 'stretch',
                         pack: 'start',
                         baseCls: 'my-panel-no-border',//隐藏边框
                         items: [

                          me.projectNoText,//合同编号
                          me.releaseDateField,
                          me.editRequireText

                         ], flex: 1
                     }, {
                         layout: "hbox",
                         width: '100%',
                         align: 'stretch',
                         pack: 'start',
                         baseCls: 'my-panel-no-border',//隐藏边框
                         items: [
                          me.projectAmountText,
                          me.planStartDateField,
                          me.projectNatureCombo
                         ], flex: 1
                     },

                     //{
                     //    layout: "hbox",
                     //    width: '100%',
                     //    align: 'stretch',
                     //    pack: 'start',
                     //    margin: '10 0 0 0',// 
                     //    baseCls: 'my-panel-no-border',//隐藏边框

                     //    items: [
                     //{
                     //    layout: "vbox",
                     //    width: '100%',
                     //    align: 'stretch',
                     //    pack: 'start',
                     //    margin: '10 0 0 0',// 
                     //    baseCls: 'my-panel-no-border',//隐藏边框

                     //    items: [
                     {
                         layout: "hbox",
                         width: '100%',
                         align: 'stretch',
                         pack: 'start',
                         baseCls: 'my-panel-no-border',//隐藏边框
                         items: [
                             me.contractModelCombo,//合同模式
                             me.planEndDateField,
                             me.qualityCombo

                         ], flex: 1
                     }, {
                         layout: "hbox",
                         width: '100%',
                         align: 'stretch',
                         pack: 'start',
                         baseCls: 'my-panel-no-border',//隐藏边框
                         items: [
                             me.projectSizeText,
                             me.developmentPhaseCombo,
                             me.writeManText
                         ], flex: 1
                     },
                     //{
                     //    layout: "hbox",
                     //    width: '100%',
                     //    align: 'stretch',
                     //    pack: 'start',
                     //    baseCls: 'my-panel-no-border',//隐藏边框
                     //    items: [
                     //        me.writeManText



                     //    ], flex: 1
                     //},
                     {
                         layout: "hbox",
                         width: '100%',
                         align: 'stretch',
                         pack: 'start',
                         baseCls: 'my-panel-no-border',//隐藏边框
                         items: [

                         me.projectTypeCombo,
                         me.projectTypeIICombo
                         //me.buildAreaText


                         ], flex: 1
                     //}]
                     //}]
                     }
                     , {
                         layout: "hbox",
                         width: '100%',
                         height: 120,
                         align: 'stretch',
                         pack: 'start',
                         baseCls: 'my-panel-no-border',//隐藏边框
                         items: [
                          //me.writeManText
                            {
                                xtype: "fieldset", margin: '8', title: "合同主要条款",
                                layout: {
                                    type: 'hbox',
                                    pack: 'start',
                                    align: 'stretch'
                                },
                                items: [
                                    me.contractTermsText
                                ]
                            }
                            , {
                                xtype: "fieldset", margin: '8', title: "工程总结",
                                layout: {
                                    type: 'hbox',
                                    pack: 'start',
                                    align: 'stretch'
                                },
                                items: [
                                     me.ProjectSummaryText
                                ]
                            }
                         ]//, flex: 2
                     }
                     , {
                         layout: "hbox",
                         width: '100%',
                         height: 120,
                         align: 'stretch',
                         pack: 'start',
                         baseCls: 'my-panel-no-border',//隐藏边框
                         items: [
                      {
                          xtype: "fieldset", margin: '8', title: "项目特殊要求",
                          layout: {
                              type: 'hbox',
                              pack: 'start',
                              align: 'stretch'
                          },
                          items: [
                               me.RequirementText
                          ]
                      }
                     ,
                     {
                         xtype: "fieldset", margin: '8', title: "备注",
                         layout: {
                             type: 'hbox',
                             pack: 'start',
                             align: 'stretch'
                         },
                         items: [
                             me.RemarksText
                         ]
                     }]
                     }


                  ], flex: 1
              },
              //{//下部容器
              //    layout: {
              //        type: 'hbox',
              //        pack: 'start',
              //        align: 'stretch'
              //    },
              //    items: [

              //        {fles:4},
              //            me.targetLabel
              //        , {
              //            xtype: "button",
              //            height: 100,
              //            margins: '12,15,12,15',
              //            text: "创建",
              //            listeners: {
              //                "click": function (btn, e, eOpts) {//添加点击按钮事件
              //                    //me.create_Project();
              //                }
              //            }, flex: 1
              //        }], height: 60
              //},
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

        me.get_combobox_default();//设置combobox默认值

        me.callParent(arguments);
    },

    //设置combobox默认值
    get_combobox_default: function () {
        var me = this;
        
        me.qualityCombo.setRawValue(me.qualitydata[0].text);//设置显示值
        me.qualityCombo.setValue(me.qualitydata[0].value); //设置ID值

        Ext.Ajax.request({
            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.HXPC_Plugins.CreateProject", A: "GetCreateRootProjectDefault",
                sid: localStorage.getItem("sid")
            },
            success: function (response, options) {
                try {
                    //获取数据后，更新窗口
                    var res = Ext.JSON.decode(response.responseText, true);
                    var state = res.success;
                    if (state === true) {
                        var recod = eval(res.data[0]);

                        //遍历设计阶段数组，添加到设计阶段combo
                        var recDesignPhase = eval(recod.DesignPhase);
                        for (var itemKey in recDesignPhase) {

                            var itemText = recDesignPhase[itemKey];

                            me.designPhasedata.push({ text: itemText, value: itemText });//在数组里添加新的元素  
                        };

                      

                        //遍历合同模式数组，添加到合同模式combo
                        var recContractModel = eval(recod.ContractModel);
                        for (var itemKey in recContractModel) {

                            var itemText = recContractModel[itemKey];

                            me.contractModeldata.push({ text: itemText, value: itemText });//在数组里添加新的元素  
                        };


                        //遍历项目开发阶段数组，添加到项目开发阶段combo
                        var recDevelopmentPhase = eval(recod.DevelopmentPhase);
                        for (var itemKey in recDevelopmentPhase) {

                            var itemText = recDevelopmentPhase[itemKey];

                            me.developmentPhasedata.push({ text: itemText, value: itemText });//在数组里添加新的元素  
                        };

                        //遍历工程实施性质数组，添加到工程实施性质combo
                        var recEngineeringProperties = eval(recod.EngineeringProperties);
                        for (var itemKey in recEngineeringProperties) {

                            var itemText = recEngineeringProperties[itemKey];

                            me.projectNaturedata.push({ text: itemText, value: itemText });//在数组里添加新的元素  
                        };

                        //遍历工程实施性质数组，添加到工程实施性质combo
                        var recProjectType = eval(recod.ProjectType);
                        for (var itemKey in recProjectType) {

                            var itemText = recProjectType[itemKey];

                            me.projectTypedata.push({ text: itemText, value: itemText });//在数组里添加新的元素  
                        };

                    }
                } catch (e) { }
            }
        });
    },

    //获取二级项目类型
    getProjectTypeII: function () {
        var me = this;

        //获取项目类型combo
        var projectType = me.projectTypeCombo.value;

        Ext.Ajax.request({
            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.HXPC_Plugins.CreateProject", A: "GetProjectTypeII",
                sid: localStorage.getItem("sid"), ProjectType: projectType
            },
            success: function (response, options) {
                try {
                    //获取数据后，更新窗口
                    var res = Ext.JSON.decode(response.responseText, true);
                    var state = res.success;
                    if (state === true) {

                        //清空数组
                        me.projectTypeIIdata.length = 0;

                        //遍历二级项目类型数组，添加到二级项目类型combo
                        var recProjectTypeII = eval(res.data[0]);

                        for (var itemKey in recProjectTypeII) {

                            var itemText = recProjectTypeII[itemKey];

                            me.projectTypeIIdata.push({ text: itemText, value: itemText });//在数组里添加新的元素  
                        };

                        //清空数组后，需要对数组进行更新  
                        me.projectTypeIIStore.load();
                        
                        if (me.projectTypeIIdata.length > 0) {
                            me.projectTypeIICombo.setRawValue(me.projectTypeIIdata[0].text);//设置显示值
                            me.projectTypeIICombo.setValue(me.projectTypeIIdata[0].value); //设置ID值
                        }
                    }
                } catch (e) { }
            }
        })
    },

    //创建项目
    send_create_root_project: function () {
        var me = this;

        //获取项目代码Text
        var projectCode = me.projectCodeText.value;

        //获取建设单位Text
        var unintName = me.unintNameText.value;

        //获取设计依据Text
        var designBasis = me.designBasisText.value;

        //获取项目名称Text
        var projectDesc = me.projectDescText.value;

        //获取甲方联系人Text
        var unitMan = me.unitManText.value;

        //获取人力资源Text
        var HUMAN = me.HRText.value;

        //获取甲方联系人电话Text
        var unitManPhone = me.unitManPhoneText.value;

        //获取主要设计原则Text
        var DESIGNPRINCIPLES = me.designPrincText.value;

        //获取合同号Text
        var PROJECTNOMBER = me.projectNoText.value;

        //获取文本编辑规定Text
        var TEXTEDITING = me.editRequireText.value;

        //获取合同额Text
        var projectAmount = me.projectAmountText.value;



        //获取建筑面积Text
        var buildArea = me.buildAreaText.value;

        //获取立项人Text
        var writeMan = me.writeManText.value;

        //获取项目特殊要求Text
        var requirement = me.RequirementText.value;

        //获取合同主要条款Text
        var contractTerms = me.contractTermsText.value;

        //获取工程总结Text
        var projectSummary = me.ProjectSummaryText.value;

        //获取备注Text
        var remarks = me.RemarksText.value;

        //获取设计阶段combo
        var designPhase = me.designPhaseCombo.value;

        //获取项目类型combo
        var projectType = me.projectTypeCombo.value;

        //获取二级项目类型combo
        var PROJECTTYPES = me.projectTypeIICombo.value;

        //获取工程实施性质combo
        var CONSTRUCTIONNATURE = me.projectNatureCombo.value;

        //获取合同模式combo
        var contractModel = me.contractModelCombo.value;

        //获取质量目标combo
        var QUALITYTARGET = me.qualityCombo.value;

        //获取任务下达时间Text
        var releaseDate=me.releaseDateField.value;

        //获取计划开始时间Text
        var planStartDate= me.planStartDateField.value;

        //获取计划结束时间Text
        var planEndDate = me.planEndDateField.value;

        //获取项目类型RadioGroup
        //var projectTypeR3 = me.projectTypeRadioGroup.getValue().projectType;
        var projectTypeR = me.projectTypeRadioGroup.getChecked()[0].boxLabel;


        //获取项目大小RadioGroup
        var projectSize = me.projectSizeText.value;
        //var projectSize = me.projectSizeRadioGroup.getChecked()[0].boxLabel;

        //获取项目开发阶段Combo
        var developmentPhase = me.developmentPhaseCombo.value;

        //获取表单数据，转换成JSON字符串
        var projectAttr =
        [
            { name: 'projectCode', value: projectCode },
            { name: 'projectDesc', value: projectDesc },
            { name: 'unintName', value: unintName },
            { name: 'designBasis', value: designBasis },            
            { name: 'PROJECTNOMBER', value: PROJECTNOMBER },
            { name: 'TEXTEDITING', value: TEXTEDITING },
            { name: 'unitMan', value: unitMan },
            { name: 'HUMAN', value: HUMAN },
            { name: 'projectAmount', value: projectAmount },
            { name: 'CONSTRUCTIONNATURE', value: CONSTRUCTIONNATURE },
            { name: 'contractModel', value: contractModel },
            { name: 'unitManPhone', value: unitManPhone },
            { name: 'DESIGNPRINCIPLES', value: DESIGNPRINCIPLES },
            { name: 'buildArea', value: buildArea },
            { name: 'writeMan', value: writeMan },
            { name: 'requirement', value: requirement },
            { name: 'contractTerms', value: contractTerms },
            { name: 'projectSummary', value: projectSummary },
            { name: 'remarks', value: remarks },
            { name: 'designPhase', value: designPhase },
            { name: 'projectType', value: projectType },
            { name: 'PROJECTTYPES', value: PROJECTTYPES },
            { name: 'QUALITYTARGET', value: QUALITYTARGET },
            { name: 'releaseDate', value: releaseDate },
            { name: 'planStartDate', value: planStartDate },
            { name: 'planEndDate', value: planEndDate },
            { name: 'projectTypeR ', value: projectTypeR },
            { name: 'projectSize', value: projectSize },
            { name: 'developmentPhase', value: developmentPhase }
            
        ];

        var projectAttrJson = Ext.JSON.encode(projectAttr);

        Ext.Ajax.request({

            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.HXPC_Plugins.CreateProject", A: "CreateRootProject",
                sid: localStorage.getItem("sid"), 
                projectAttrJson: projectAttrJson
            },
            success: function (response, options) {
                me.create_root_project_callback(response, options,"");//, me.projectKeyword, closeWin);
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
            Ext.require('Ext.ux.Common.m.comm', function () {

                winCreateRootProject.close();

                var projectKeyword = res.data[0].projectKeyword;
                //参数：doclist,wfKeyword,userlist,callback_fun
                StartNewWorkFlow(projectKeyword, "CREATEPROJECT", "", function (res, WorkFlowKeyword, CuWorkStateCode) {
                    me.refreshWin(projectKeyword, false);
                })
            });
        }
    },

    //刷新表单，参数:parentKeyword:新建的联系单目录
    refreshWin: function (parentKeyword, closeWin) {
        var me = this; var tree = Ext.getCmp(me.mainPanelId);//.down('treepanel');
        var viewTreeStore = tree.store;

        viewTreeStore.load({
            callback: function (records, options, success) {//添加回调，获取子目录的文件数量
                if (closeWin)
                    winCreateRootProject.close();

                //展开目录
                Ext.require('Ext.ux.Common.m.comm', function () {
                    Ext.getCmp('contentPanel').down('_mainProjectTree').ExpendProject(parentKeyword);
                });
            }
        });
    }
});

