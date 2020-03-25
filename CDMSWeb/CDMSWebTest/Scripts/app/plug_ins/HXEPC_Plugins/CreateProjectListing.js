//项目立项
Ext.define('Ext.plug_ins.HXEPC_Plugins.CreateProjectListing', {
    extend: 'Ext.container.Container',
    alias: 'widget.CreateProjectListing',
    //layout: "border",
    layout: 'fit',
    resultvalue: '', mainPanelId: '', projectKeyword: '',
    initComponent: function () {
        var me = this;

        //定义项目类型combo初始数据
        me.projectTypedata = [];//{ text: "Ⅰ类项目", value: "Ⅰ类项目" }, { text: "Ⅱ类项目", value: "Ⅱ类项目" }];

        //定义二级项目类型combo初始数据
        me.projectTypeIIdata = [];

        //定义项目类型combo初始数据
        me.qualitydata = [{ text: "省优秀I", value: "省优秀I" }];//, { text: "市优", value: "市优" },
            //{ text: "院优", value: "院优" }, { text: "合格", value: "合格" }];

        //定义项目实施性质combo初始数据
        me.projectNaturedata = [];

        //定义项目来源combo初始数据
        me.projectSourcedata = [];

        //定义合同类型combo初始数据
        me.contractTypedata = [];

        me.auditorList = "";

        //定义项目代码Text
        me.projectCodeText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "项目代码", anchor: "80%", labelWidth: 110, labelAlign: "left", width: '50%', margin: '0 10 0 10', fieldStyle: 'border-color: red; background-image: none;'//红色边框//flex: 1
        });

        //定义业主/甲方 (建设单位) Text
        me.unintNameText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "业主/甲方", anchor: "80%", labelWidth: 90, labelAlign: "left", width: "100%",//width: 230, 
            margin: '0 10 0 10', fieldStyle: ' background-image: none;'//红色边框//flex: 1
        });

        //定义项目实施地址Text
        me.projectAddrText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "项目实施地址", anchor: "80%", labelWidth: 90, labelAlign: "left", width: "100%",// width: 230,
            margin: '0 10 0 10', fieldStyle: ' background-image: none;'//红色边框//flex: 1
        });


        //定义项目名称Text
        me.projectDescENText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "项目名称(英文)", anchor: "80%", labelWidth: 90, labelAlign: "left", width: "100%",//width: 230,
            margin: '0 10 0 10', fieldStyle: ' background-image: none;'//红色边框//flex: 1
        });

        //定义项目名称Text
        me.projectDescCNText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "项目名称(中文)", anchor: "80%", labelWidth: 90, labelAlign: "left", width: "100%",//width: 230, 
            margin: '0 10 0 10', fieldStyle: 'border-color: red; background-image: none;'//红色边框//flex: 1
        });

        //定义项目简称Text
        me.projectShortDescENText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "项目简称(英文)", anchor: "80%", labelWidth: 90, labelAlign: "left", width: "100%",//width: 230,
            margin: '0 10 0 10', fieldStyle: ' background-image: none;'//红色边框//flex: 1
        });

        //定义项目简称Text
        me.projectShortDescCNText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "项目简称(中文)", anchor: "80%", labelWidth: 90, labelAlign: "left", width: "100%",//width: 230, 
            margin: '0 10 0 10', fieldStyle: ' background-image: none;'//flex: 1
        });

        //定义国内通信代码Text
        me.communicationCodeText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "通信代码(Onshore)", anchor: "80%", labelWidth: 110, labelAlign: "left", width: '50%', margin: '0 10 0 10', fieldStyle: ' background-image: none;'//红色边框//flex: 1
        });

        //定义国际通信代码Text
        me.intcommunicationCodeText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "通信代码(Offshore)", anchor: "80%", labelWidth: 110, labelAlign: "left", width: '50%', margin: '0 10 0 10', fieldStyle: ' background-image: none;'//红色边框//flex: 1
        });

        //定义通信邮箱Text
        me.emailText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "通信邮箱", anchor: "80%", labelWidth: 110, labelAlign: "left", width:'50%', margin: '0 10 0 10', fieldStyle: ' background-image: none;'//红色边框//flex: 1
        });


        //定义计划工期Text
        me.projectDurationText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "计划工期", anchor: "80%", labelWidth: 90, labelAlign: "left", width: 230, margin: '0 10 0 10', fieldStyle: ' background-image: none;'//红色边框//flex: 1
        });
        
        //定义预估金额Text
        me.projectAmountText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",// labelSeparator: '', // 去掉laebl中的冒号
            fieldLabel: "预估金额(RMB)", anchor: "80%", labelWidth: 90, labelAlign: "left", width: 230, margin: '0 10 0 10', fieldStyle: 'background-image: none;'//红色边框//flex: 1
        });


        ////定义建筑面积Text
        //me.buildAreaText = Ext.create("Ext.form.field.Text", {
        //    xtype: "textfield",
        //    fieldLabel: "建筑面积", anchor: "80%", labelWidth: 60, labelAlign: "left", width: 160, margin: '0 10 0 10', fieldStyle: ' background-image: none;' //flex: 1
        //});


        //定义立项人Text
        me.writeManText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "立项人", anchor: "80%", labelWidth: 90, labelAlign: "left", width: 230, margin: '0 10 0 10', fieldStyle: ' background-image: none;'//红色边框//flex: 1
        });

        //项目类型补充说明
        me.projectTypeSuppText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", disabled: true,
            fieldLabel: "请补充", anchor: "80%", labelWidth: 90, labelAlign: "left",
            width: 230, margin: '0 10 0 10', fieldStyle: ' background-image: none;'//红色边框//flex: 1
        });


        //项目实施性质补充说明
        me.projectNatureSuppText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", disabled: true,
            fieldLabel: "请补充", anchor: "80%", labelWidth: 90, labelAlign: "left", width: 230, margin: '0 10 0 10', fieldStyle: ' background-image: none;'//红色边框//flex: 1
        });

        //项目来源补充说明
        me.projectSourceSuppText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",disabled :true,
            fieldLabel: "请补充", anchor: "80%", labelWidth: 90, labelAlign: "left", width: 230, margin: '0 10 0 10', fieldStyle: ' background-image: none;'//红色边框//flex: 1
        });

        //合同类型补充说明
        me.contractTypeSuppText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", disabled: true,
            fieldLabel: "请补充", anchor: "80%", labelWidth: 90, labelAlign: "left", width: 230, margin: '0 10 0 10', fieldStyle: ' background-image: none;'//红色边框//flex: 1
        });

        //添加项目概况Text
        me.ProjectOverviewText = Ext.create("Ext.form.field.TextArea", {
            xtype: "textarea", anchor: "80%", labelAlign: "left", margin: '0 10 0 10', //margin: '0 5 5 0',
            width: "100%",//flex:1, //width: 460, //
            height: 50 ,fieldLabel: "项目概况",labelWidth: 90,
        });
        
        //定义负责人Text
        me.auditorText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "负责人", anchor: "80%", labelWidth: 90, labelAlign: "right", labelPad: 8, width: "40%",//width: 230, 
            margin: '10 5 0 10', fieldStyle: ' background-image: none;'//红色边框//flex: 1
        });

        //定义批准人Text
        me.approverText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "批准人", anchor: "80%", labelWidth: 60, labelAlign: "right", labelPad: 8, width: "40%",//width: 230, 
            margin: '10 5 0 10', fieldStyle: ' background-image: none;'//红色边框//flex: 1
        });

        //定义计划开始时间Text
        me.planStartDateField = Ext.create("Ext.form.field.Date", {

            name: "date",
            fieldLabel: ' 计划开始时间', fieldStyle: ' background-image: none;',
            editable: true, labelWidth: 90, margin: '0 10 0 10',
            emptyText: "--请选择--", autoLoadOnValue: true,
            format: 'Y年m月d日',
            value: new Date(),
            width: 230
        });


        //定义计划结束时间Text
        me.planEndDateField = Ext.create("Ext.form.field.Date", {

            name: "date",
            fieldLabel: ' 计划结束时间', fieldStyle: ' background-image: none;',
            editable: true, labelWidth: 90, margin: '0 10 0 10',
            emptyText: "--请选择--", autoLoadOnValue: true,
            format: 'Y年m月d日',
            value: new Date(),
            width: 230
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
            fieldLabel: '项目类型', labelWidth: 90,
            triggerAction: "all", store: me.projectTypeStore,
            valueField: 'value', editable: false,//不可输入
            displayField: 'text', margin: '0 10 0 10',// 
            emptyText: "--请选择--", autoLoadOnValue: true,
            anchor: "80%", labelAlign: "left", width: 230,
            listeners:
            {
                select: function (combo, records, eOpts) {


                    //获取项目实施性质combo
                    var projectType = me.projectTypeCombo.value;
                    if (projectType === "其他") {
                        me.projectTypeSuppText.setDisabled(false);
                        //获取流水号
                        me.getProjectTypeII();
                        me.projectTypeIICombo.setDisabled(true);
                    } else {
                        me.projectTypeSuppText.setValue("");
                        me.projectTypeSuppText.setDisabled(true);
                        //获取流水号
                        me.getProjectTypeII();
                        me.projectTypeIICombo.setDisabled(false);
                        //me.onProjectTypeIIComboSelect();
                    }
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
            anchor: "80%", labelAlign: "left", width: 230,
            listeners:
            {
                select: function (combo, records, eOpts) {
                    me.onProjectTypeIIComboSelect();
                    ////获取项目实施性质combo
                    //var projectTypeII = me.projectTypeIICombo.value;
                    //if (projectTypeII === "其他") {
                    //    me.projectTypeSuppText.setDisabled(false);
                    //} else {
                    //    me.projectTypeSuppText.setValue("");
                    //    me.projectTypeSuppText.setDisabled(true);
                    //}
                }
            }
        });

        //添加项目实施性质combo
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
            fieldLabel: '项目实施性质', labelWidth: 90,
            triggerAction: "all", store: me.projectNatureStore,
            valueField: 'value', editable: false,//不可输入
            displayField: 'text', margin: '0 10 0 10',// 
            emptyText: "--请选择--", autoLoadOnValue: true,
            anchor: "80%", labelAlign: "left", width: 230,
                listeners:
                {
                    select: function (combo, records, eOpts) {
                        //获取项目实施性质combo
                        var projectNature = me.projectNatureCombo.value;
                        if (projectNature === "其他") {
                            me.projectNatureSuppText.setDisabled(false);
                        } else {
                            //me.projectNatureSuppText.setValue("");
                            me.projectNatureSuppText.setDisabled(true);
                        }
                    }
                }
        });

        //添加合同类型combo
        Ext.define("contractTypeModel", {
            extend: 'Ext.data.Model',
            fields: ["text", "value"]
        });
        me.contractTypeProxy = Ext.create("Ext.data.proxy.Memory", {
            data: me.contractTypedata,
            model: "contractTypeModel"
        });

        me.contractTypeStore = Ext.create("Ext.data.Store", {
            model: contractTypeModel,
            proxy: me.contractTypeProxy
        });


        me.contractTypeCombo = Ext.create("Ext.form.field.ComboBox",
        {
            //xtype: "combo",
            fieldLabel: '合同类型', labelWidth: 90,
            triggerAction: "all", store: me.contractTypeStore,
            valueField: 'value', editable: false,//不可输入
            displayField: 'text', margin: '0 10 0 10',// 
            emptyText: "--请选择--", autoLoadOnValue: true,
            anchor: "80%", labelAlign: "left", width: 230,
            listeners:
            {
                select: function (combo, records, eOpts) {
                    //获取项目实施性质combo
                    var contractType = me.contractTypeCombo.value;
                    if (contractType === "其他") {
                        me.contractTypeSuppText.setDisabled(false);
                    } else {
                        //me.projectNatureSuppText.setValue("");
                        me.contractTypeSuppText.setDisabled(true);
                    }
                }
            }
        });

        //添加项目来源combo
        Ext.define("projectSourceModel", {
            extend: 'Ext.data.Model',
            fields: ["text", "value"]
        });
        me.projectSourceProxy = Ext.create("Ext.data.proxy.Memory", {
            data: me.projectSourcedata,
            model: "projectSourceModel"
        });

        me.projectSourceStore = Ext.create("Ext.data.Store", {
            model: projectSourceModel,
            proxy: me.projectSourceProxy
        });


        me.projectSourceCombo = Ext.create("Ext.form.field.ComboBox",
        {
            //xtype: "combo",
            fieldLabel: '项目来源', labelWidth: 90,
            triggerAction: "all", store: me.projectSourceStore,
            valueField: 'value', editable: false,//不可输入
            displayField: 'text', margin: '0 10 0 10',// 
            emptyText: "--请选择--", autoLoadOnValue: true,
            anchor: "80%", labelAlign: "left", width: 230,
            listeners:
            {
                select: function (combo, records, eOpts) {
                    //获取项目实施性质combo
                    var projectSource = me.projectSourceCombo.value;
                    if (projectSource === "其他") {
                        me.projectSourceSuppText.setDisabled(false);
                    } else {
                        //me.projectNatureSuppText.setValue("");
                        me.projectSourceSuppText.setDisabled(true);
                    }
                }
            }

        });

        ////添加质量目标combo
        //Ext.define("qualityModel", {
        //    extend: 'Ext.data.Model',
        //    fields: ["text", "value"]
        //});
        //me.qualityProxy = Ext.create("Ext.data.proxy.Memory", {
        //    data: me.qualitydata,
        //    model: "qualityModel"
        //});

        //me.qualityStore = Ext.create("Ext.data.Store", {
        //    model: qualityModel,
        //    proxy: me.qualityProxy
        //});


        //me.qualityCombo = Ext.create("Ext.form.field.ComboBox",
        //{
        //    //xtype: "combo",
        //    fieldLabel: '质量目标', labelWidth: 90,
        //    triggerAction: "all", store: me.qualityStore,
        //    valueField: 'value', editable: false,//不可输入
        //    displayField: 'text', margin: '0 10 0 10',// 
        //    anchor: "80%", labelAlign: "left", width: 230,//
        //    emptyText: "--请选择--", autoLoadOnValue: true,
        //    listeners:
        //    {
        //        select: function (combo, records, eOpts) {
        //        }
        //    }
        //});



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
                            me.projectDescCNText//项目名称
                            //

                         ], flex: 1
                     }, {
                         layout: "hbox",
                         width: '100%',
                         align: 'stretch',
                         pack: 'start',
                         baseCls: 'my-panel-no-border',//隐藏边框
                         items: [
                             
                              me.projectDescENText//项目名称英文
                             //me.unitManText,
                             
                         ], flex: 1
                     }, {
                         layout: "hbox",
                         width: '100%',
                         align: 'stretch',
                         pack: 'start',
                         baseCls: 'my-panel-no-border',//隐藏边框
                         items: [
                            me.projectShortDescCNText//项目简称
                            //

                         ], flex: 1
                     }, {
                         layout: "hbox",
                         width: '100%',
                         align: 'stretch',
                         pack: 'start',
                         baseCls: 'my-panel-no-border',//隐藏边框
                         items: [

                              me.projectShortDescENText//项目简称英文
                             //me.unitManText,

                         ], flex: 1
                     }, {
                         layout: "hbox",
                         width: '100%',
                         align: 'stretch',
                         pack: 'start',
                         baseCls: 'my-panel-no-border',//隐藏边框
                         items: [
                            me.unintNameText //建设单位
                           // me.projectAddrText

                          
                          //me.designPrincText
                         ], flex: 1
                     }, {
                         layout: "hbox",
                         width: '100%',
                         align: 'stretch',
                         pack: 'start',
                         baseCls: 'my-panel-no-border',//隐藏边框
                         items: [
                            me.projectAddrText //项目实施地址
                         ], flex: 1
                     }, {
                         //xtype: "fieldset", margin: '8',// title: "项目概况",
                         //layout: {
                         //    type: 'hbox',
                         //    pack: 'start',
                         //    align: 'stretch'
                         //},
                         layout: "hbox",
                         width: '100%',
                         align: 'stretch',
                         pack: 'start',
                         height: 55,
                         baseCls: 'my-panel-no-border',//隐藏边框
                         items: [
                              //me.RequirementText
                              me.ProjectOverviewText  // 项目概况
                         ]//, flex: 1
                     },

                     {
                         layout: "hbox",
                         width: '100%',
                         align: 'stretch',
                         pack: 'start',
                         baseCls: 'my-panel-no-border',//隐藏边框
                         items: [

                         me.projectTypeCombo,//项目类型
                         me.projectTypeIICombo,
                         me.projectTypeSuppText //项目类型补充

                         ], flex: 1
                     }, {
                         layout: "hbox",
                         width: '100%',
                         align: 'stretch',
                         pack: 'start',
                         baseCls: 'my-panel-no-border',//隐藏边框
                         items: [
                              me.projectNatureCombo,//项目实施性质 
                              me.projectNatureSuppText//项目实施性质 补充
                               
                         ], flex: 1
                     }, {
                         layout: "hbox",
                         width: '100%',
                         align: 'stretch',
                         pack: 'start',
                         baseCls: 'my-panel-no-border',//隐藏边框
                         items: [
                             me.projectSourceCombo, //项目来源
                             me.projectSourceSuppText //项目来源补充
                         ], flex: 1
                     }, {
                         layout: "hbox",
                         width: '100%',
                         align: 'stretch',
                         pack: 'start',
                         baseCls: 'my-panel-no-border',//隐藏边框
                         items: [
                             me.contractTypeCombo,//合同类型
                             me.contractTypeSuppText, //合同类型补充
                          me.projectAmountText//预估金额
                             

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
                             me.projectDurationText,
                             me.planStartDateField,
                             me.planEndDateField//,
                             //me.qualityCombo

                         ], flex: 1
                     }, {
                         layout: "hbox",
                         //width: '100%',
                         align: 'stretch',
                         pack: 'start',
                         baseCls: 'my-panel-no-border',//隐藏边框
                         items: [

                          me.projectCodeText,//项目代码
                          //me.communicationCodeText,//通信代码
                          me.emailText//通信邮箱


                         ], flex: 1
                     }, {
                         layout: "hbox",
                         //width: '100%',
                         align: 'stretch',
                         pack: 'start',
                         baseCls: 'my-panel-no-border',//隐藏边框
                         items: [

                          me.communicationCodeText,//国内通信代码
                          me.intcommunicationCodeText,//国际通信代码

                         ], flex: 1
                     }
                     , {
                         layout: "hbox",
                         //width: '100%',
                         align: 'stretch',
                         pack: 'start',
                         baseCls: 'my-panel-no-border',//隐藏边框
                         items: [

                            me.auditorText,//负责人
                             {
                                 xtype: "button",
                                 text: "选择...", margins: "10 0 0 5",
                                 listeners: {
                                     "click": function (btn, e, eOpts) {//添加点击按钮事件
                                         Ext.require('Ext.ux.Common.comm', function () {
                                             showSelectUserWin("getUser", "", "", function () {
                                                 me.auditorText.setValue(window.parent.usernamelist);
                                                 me.auditorList = window.parent.resultvalue;
                                             });
                                         })
                                     }
                                 }
                             }

                         ], flex: 1
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
                                  winCreateProjectListing.close();
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
        
        //me.qualityCombo.setRawValue(me.qualitydata[0].text);//设置显示值
        //me.qualityCombo.setValue(me.qualitydata[0].value); //设置ID值

        Ext.Ajax.request({
            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.HXEPC_Plugins.HXProject", A: "GetCreateProjectListingDefault",
                sid: localStorage.getItem("sid")
            },
            success: function (response, options) {
                try {
                    //获取数据后，更新窗口
                    var res = Ext.JSON.decode(response.responseText, true);
                    var state = res.success;
                    if (state === true) {
                        var recod = eval(res.data[0]);

                        ////遍历设计阶段数组，添加到设计阶段combo
                        //var recDesignPhase = eval(recod.DesignPhase);
                        //for (var itemKey in recDesignPhase) {

                        //    var itemText = recDesignPhase[itemKey];

                        //    me.designPhasedata.push({ text: itemText, value: itemText });//在数组里添加新的元素  
                        //};

                      

                        //遍历合同类型数组，添加到合同类型combo
                        var reccontractType = eval(recod.ContractType);
                        for (var itemKey in reccontractType) {

                            var itemText = reccontractType[itemKey];

                            me.contractTypedata.push({ text: itemText, value: itemText });//在数组里添加新的元素  
                        };


                        //遍历项目来源数组，添加到项目来源combo
                        var recprojectSource = eval(recod.ProjectSource);
                        for (var itemKey in recprojectSource) {

                            var itemText = recprojectSource[itemKey];

                            me.projectSourcedata.push({ text: itemText, value: itemText });//在数组里添加新的元素  
                        };

                        //遍历项目实施性质数组，添加到项目实施性质combo
                        var recEngineeringProperties = eval(recod.EngineeringProperties);
                        for (var itemKey in recEngineeringProperties) {

                            var itemText = recEngineeringProperties[itemKey];

                            me.projectNaturedata.push({ text: itemText, value: itemText });//在数组里添加新的元素  
                        };

                        //遍历项目实施性质数组，添加到项目实施性质combo
                        var recProjectType = eval(recod.ProjectType);
                        for (var itemKey in recProjectType) {

                            var itemText = recProjectType[itemKey];

                            me.projectTypedata.push({ text: itemText, value: itemText });//在数组里添加新的元素  
                        };

                    }
                } catch (e) { }
            },
            failure: function (response, options) {
                ////Ext.Msg.alert("错误", "连接服务器失败！<br>" + response.responseText);
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
                C: "AVEVA.CDMS.HXEPC_Plugins.HXProject", A: "GetProjectTypeII",
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

                        if (projectType!="其他")
                            me.onProjectTypeIIComboSelect();
                    }
                } catch (e) { }
            },
            failure: function (response, options) {
                ////Ext.Msg.alert("错误", "连接服务器失败！<br>" + response.responseText);
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

        //获取项目实施地址Text
        var projectAddr = me.projectAddrText.value;

        //获取项目名称Text
        var projectDescCN = me.projectDescCNText.value;
        var projectDescEN = me.projectDescENText.value;

        //获取项目简称Text
        var projectShortDescCN = me.projectShortDescCNText.value;
        var projectShortDescEN = me.projectShortDescENText.value;
        
        //获取通信代码Text
        var communicationCode = me.communicationCodeText.value;
        var intcommunicationCode = me.intcommunicationCodeText.value;

        //获取通信邮箱Text
        var email = me.emailText.value;

        //获取合同号Text
        //var PROJECTNOMBER = me.projectNoText.value;

        //获取计划工期Text
        var projectDuration = me.projectDurationText.value;

        //获取预估金额Text
        var projectAmount = me.projectAmountText.value;


        //获取立项人Text
        //var writeMan = me.writeManText.value;

        //获取项目概况Text
        var projectOverview = me.ProjectOverviewText.value;

        //获取项目类型combo
        var projectType = me.projectTypeCombo.value;

        //获取二级项目类型combo
        var projectTypeII = me.projectTypeIICombo.value;

        //获取项目类型补充说明Text
        var projectTypeSupp = me.projectTypeSuppText.value;

        //获取项目实施性质combo
        var projectNature = me.projectNatureCombo.value;

        //获取项目实施性质补充说明Text
        var projectNatureSupp = me.projectNatureSuppText.value;

        //获取项目来源Combo
        var projectSource = me.projectSourceCombo.value;

        //获取项目来源补充Text
        var projectSourceSupp = me.projectSourceSuppText.value;

        //获取合同类型combo
        var contractType = me.contractTypeCombo.value;

        //获取合同类型补充Text
        var contractTypeSupp = me.contractTypeSuppText.value;

        //获取计划开始时间Text
        var planStartDate= me.planStartDateField.value;

        //获取计划结束时间Text
        var planEndDate = me.planEndDateField.value;



        //获取表单数据，转换成JSON字符串
        var projectAttr =
        [
            { name: 'projectCode', value: projectCode },
            { name: 'projectDescEN', value: projectDescEN },
            { name: 'projectDescCN', value: projectDescCN },
            { name: 'projectShortDescEN', value: projectShortDescEN },
            { name: 'projectShortDescCN', value: projectShortDescCN },
            { name: 'unintName', value: unintName },  //业主
            { name: 'projectAddr', value: projectAddr },
            { name: 'projectOverview', value: projectOverview },//项目概况
            { name: 'projectType', value: projectType },//项目类型
            { name: 'projectTypeII', value: projectTypeII },
            { name: 'projectTypeSupp', value: projectTypeSupp },//项目类型补充
            { name: 'projectNature', value: projectNature },//项目实施性质
            { name: 'projectNatureSupp', value: projectNatureSupp },//项目实施性质补充说明
            { name: 'projectSource', value: projectSource },//项目来源
            { name: 'projectSourceSupp', value: projectSourceSupp },
            { name: 'contractType', value: contractType },//合同类型
            { name: 'contractTypeSupp', value: contractTypeSupp },//合同类型补充
            { name: 'projectAmount', value: projectAmount },//预估金额
            { name: 'projectDuration', value: projectDuration },//计划工期
            { name: 'planStartDate', value: planStartDate },
            { name: 'planEndDate', value: planEndDate },
            { name: 'communicationCode', value: communicationCode },//通信代码
            { name: 'intcommunicationCode', value: intcommunicationCode },//通信代码
            { name: 'email', value: email },//通信邮箱
            { name: 'auditor', value: me.auditorList }//负责人
            //{ name: 'buildArea', value: buildArea }
            //,{ name: 'writeMan', value: writeMan }
            
        ];

        var projectAttrJson = Ext.JSON.encode(projectAttr);

        Ext.MessageBox.wait("正在生成项目立项单，请稍候...", "等待");

        Ext.Ajax.request({

            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.HXEPC_Plugins.HXProject", A: "CreateProjectListing",
                sid: localStorage.getItem("sid"), ProjectKeyword:me.projectKeyword,
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

                winCreateProjectListing.close();

                var projectKeyword = res.data[0].ProjectKeyword;

                me.refreshWin(projectKeyword, false);

                //创建流程
                //参数：doclist,wfKeyword,userlist,callback_fun
                //StartNewWorkFlow(projectKeyword, "CREATEPROJECT", "", function (res, WorkFlowKeyword, CuWorkStateCode) {
                //    me.refreshWin(projectKeyword, false);
                //})
            });
        }
    },

    onProjectTypeIIComboSelect: function () {
        var me = this;
        //获取二级项目类型combo
        var projectTypeII = me.projectTypeIICombo.value;
        if (projectTypeII === "其他") {
            me.projectTypeSuppText.setDisabled(false);
        } else {
            me.projectTypeSuppText.setValue("");
            me.projectTypeSuppText.setDisabled(true);
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
                    winCreateProjectListing.close();

                //展开目录
                Ext.require('Ext.ux.Common.comm', function () {
                    Ext.getCmp('contentPanel').down('_mainProjectTree').ExpendProject(parentKeyword);
                });
            }
        });
    }
});

