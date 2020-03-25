Ext.define('Ext.plug_ins.HXPC_Plugins.taskReport', {
    extend: 'Ext.container.Container',

    //requires: ["Ext.ux.datetime.UX_TimePickerField"],
    //requires: ["Ext.ux.datetime.UX_DateTimePicker"],
    //requires: ["Ext.ux.datetime.UX_DateTimeMenu"],
    //requires: ["Ext.ux.datetime.UX_DateTimeField"],

    //requires: ["Ext.ux.datetime.datetime"],

    alias: 'widget.taskReport',
    layout: 'fit',
    resultvalue: '', mainPanelId: '',
    projectKeyword: '', WfKeyword: '',
    initComponent: function () {
        var me = this;

        //选中的任务Keyword
        me.selectTaskKeyword = "";

        //选中的任务报告Keyword
        me.selectReportKeyword = "";

        me.isCreateReport = false;

        //定义实耗工时combo初始数据
        me.labortimedata = [
            { text: "7.5", value: "7.5" }, { text: "7", value: "7" },
            { text: "6.5", value: "6.5" }, { text: "6", value: "6" },
            { text: "5.5", value: "5.5" }, { text: "5", value: "5" },
            { text: "4.5", value: "4.5" }, { text: "4", value: "4" },
            { text: "3.5", value: "3.5" }, { text: "3", value: "3" },
            { text: "2.5", value: "2.5" }, { text: "2", value: "2" },
            { text: "1.5", value: "1.5" }, { text: "1", value: "1" },
            { text: "0.5", value: "0.5" }
        ];

        //定义任务进度Text
        me.scheduleText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "任务进度", anchor: "80%", labelWidth: 55, labelAlign: "right", width: 145, margin: '0 5 0 0', fieldStyle: ' background-image: none;'//红色边框//flex: 1
        });


        //添加报告内容Text
        me.contentText = Ext.create("Ext.form.field.TextArea", {
            xtype: "textarea", anchor: "80%", labelWidth: 55, labelAlign: "right", margin: '8 0 0 0', width: 360,
            fieldLabel: "报告内容", height: 160
        });

        //添加存在问题Text
        me.problomText = Ext.create("Ext.form.field.TextArea", {
            xtype: "textarea", anchor: "80%", labelWidth: 55, labelAlign: "right", margin: '18 0 0 0', width: 360,
            fieldLabel: "存在问题", height: 160
        });

        //定义报告时间Text
        me.reportDateTimeField = Ext.create("Ext.form.field.DateTime", {
            //xtype: 'datetimefield',
            fieldLabel: '报告时间',
            anchor: "70%",//这个是设置控件的宽度，不与width同时使用
            enableKeyEvents: true,
            editable: true,
            labelWidth: 55, margin: '0 13 0 0', labelAlign: "right",
            format: 'Y年m月d日',
            value: new Date(),
            width:230
        });

        //定义任务下达时间Text
        me.releaseDateField = Ext.create("Ext.form.field.Date", {

            name: "date",
            fieldLabel: ' 任务下达时间', fieldStyle: ' background-image: none;',
            editable: true, labelWidth: 85, margin: '0 10 0 10',
            emptyText: "--请选择--", autoLoadOnValue: true,
            format: 'Y年m月d日',
            value: new Date(),
            width: 230

        });

        ////定义报告开始时间Text
        //me.startDateTimeField = Ext.create("Ext.form.field.DateTime", {
        //    //xtype: 'datetimefield',
        //    fieldLabel: '开始时间',
        //    anchor: "70%",//这个是设置控件的宽度，不与width同时使用
        //    enableKeyEvents: true,
        //    editable: true,
        //    labelWidth: 55, margin: '0 13 0 0', labelAlign: "right",
        //    format: 'Y年m月d日',
        //    value: new Date(),
        //    width: 230
        //});

        ////定义报告截止时间Text
        //me.endDateTimeField = Ext.create("Ext.form.field.DateTime", {
        //    //xtype: 'datetimefield',
        //    fieldLabel: '截止时间',
        //    anchor: "70%",//这个是设置控件的宽度，不与width同时使用
        //    enableKeyEvents: true,
        //    editable: true,
        //    labelWidth: 55, margin: '0 5 0 0', labelAlign: "right",
        //    format: 'Y年m月d日',
        //    value: new Date(),
        //    width: 230
        //});

        ////定义任务列表表格
        ////定义任务列表的model
        //Ext.define("tasklistmodel", {
        //    extend: "Ext.data.Model",
        //    fields: [
        //        "taskKeyword",
        //        "taskType",
        //        "taskName"
        //    ],
        //    url: "_blank",
        //});

        ////定义任务列表的store
        //me.taskliststore = Ext.create("Ext.data.Store", {
        //    model: "tasklistmodel"
        //});
        ////定义任务列表的view
        //me.tasklistgrid = Ext.widget("grid", {
        //    region: "center",
        //    height: 68,
        //    stateful: true,
        //    multiSelect: true,
        //    //hideHeaders: true,//隐藏表头
        //    flex: 1,
        //    store: me.taskliststore,
        //    viewConfig: {
        //        stripeRows: true,
        //        enableTextSelection: false
        //        //getRowClass: function () {
        //        //    // 在这里添加自定样式 改变这个表格的行高
        //        //    return 'x-grid-row upload-file-grid-row';
        //        //}
        //    },
        //    columns: [
        //        { text: '任务类型', dataIndex: 'taskType', width: 60 },
        //        { text: '任务名称', dataIndex: 'taskName', width: 180 }
        //    ],
        //    listeners: {
        //        itemmousedown: function (view, record, item, index, e, eOpts) {
        //        },
        //        select: function (view, record, index, eOpts) {
        //            me.onTaskListGridSelect(view, record, index, eOpts);
        //        }
        //    }
        //});

        ////定义任务报告列表表格
        ////定义任务报告列表的model
        //Ext.define("reportlistmodel", {
        //    extend: "Ext.data.Model",
        //    fields: [
        //        "reportKeyword",
        //        "reportContent",
        //        "reportDate",
        //        "schedule"
        //    ],
        //    url: "_blank",
        //});

        ////定义任务列表的store
        //me.reportliststore = Ext.create("Ext.data.Store", {
        //    model: "reportlistmodel"
        //});
        ////定义任务列表的view
        //me.reportlistgrid = Ext.widget("grid", {
        //    region: "center",
        //    height: 68,
        //    width: 260,
        //    stateful: true,
        //    multiSelect: true,
        //    //hideHeaders: true,//隐藏表头
        //    //flex: 1,
        //    store: me.reportliststore,
        //    viewConfig: {
        //        stripeRows: true,
        //        enableTextSelection: false
        //        //getRowClass: function () {
        //        //    // 在这里添加自定样式 改变这个表格的行高
        //        //    return 'x-grid-row upload-file-grid-row';
        //        //}
        //    },
        //    columns: [
        //        { text: '报告名称', dataIndex: 'reportContent', width: 105 },
        //        { text: '报告时间', dataIndex: 'reportDate', width: 105 },
        //        { text: '进度', dataIndex: 'schedule', width: 35 }
        //    ],
        //    listeners: {
        //        select: function (view, record, index, eOpts) {
        //            me.onReportListGridSelect(view, record, index, eOpts);
        //        }
        //    }
        //});

        ////添加实耗工时combo
        //Ext.define("laborTimeModel", {
        //    extend: 'Ext.data.Model',
        //    fields: ["text", "value"]
        //});
        //me.laborTimeProxy = Ext.create("Ext.data.proxy.Memory", {
        //    data: me.labortimedata,
        //    model: "laborTimeModel"
        //});

        //me.laborTimeStore = Ext.create("Ext.data.Store", {
        //    model: laborTimeModel,
        //    proxy: me.laborTimeProxy
        //});


        //me.laborTimeCombo = Ext.create("Ext.form.field.ComboBox",
        //{
        //    //xtype: "combo",
        //    fieldLabel: '实耗工时', labelWidth: 55,
        //    triggerAction: "all", store: me.laborTimeStore,
        //    valueField: 'value', editable: false,//不可输入
        //    displayField: 'text', margin: '0 0 0 0',// 
        //    emptyText: "--请选择--", autoLoadOnValue: true,
        //    anchor: "80%", labelAlign: "right", width: 145

        //});

        me.items = [
                    {
                        xtype:"button",
                        text: "我的按钮"
                    }
        ]
        ////添加列表
        //me.items = [
        //  Ext.widget('form', {
        //      layout: "form",
        //      items: [

        //          {
        //              xtype: "panel",
        //              baseCls: 'my-panel-no-border',//隐藏边框
        //              layout: {
        //                  type: 'vbox',
        //                  pack: 'start',
        //                  align: 'stretch'
        //              },
        //              items: [
        //                  {
        //                      xtype: "panel",
        //                      baseCls: 'my-panel-no-border',//隐藏边框
        //                      layout: {
        //                          type: 'hbox',
        //                          pack: 'start',
        //                          align: 'stretch'
        //                      },
        //                      height: 570,
        //                      items: [
        //                          {
        //                              xtype: "fieldset",
        //                              margin: '8 0 12 8',
        //                              title: '任务列表',
        //                              //height: 300,
        //                              width:280,
        //                              //baseCls: 'my-panel-no-border',//隐藏边框
        //                              layout: {
        //                                  type: 'vbox',
        //                                  pack: 'start',
        //                                  align: 'stretch'
        //                              },
        //                              items: [
        //                                    me.tasklistgrid
        //                              ]
        //                          }, {
        //                              xtype: "fieldset",
        //                              margin: '8 8 12 8',
        //                              title: '任务报告',
        //                              //height: 300,
        //                              flex: 1,
        //                              //baseCls: 'my-panel-no-border',//隐藏边框
        //                              layout: {
        //                                  type: 'hbox',
        //                                  pack: 'start',
        //                                  align: 'stretch'
        //                              },
        //                              items: [
        //                                    me.reportlistgrid,
        //                                  {
        //                                      xtype: "fieldset",
        //                                      margin: '0 0 0 8',
        //                                      title: '报告',
        //                                      //height: 300,
        //                                      flex: 1,
        //                                      //baseCls: 'my-panel-no-border',//隐藏边框
        //                                      layout: {
        //                                          type: 'vbox',
        //                                          pack: 'start',
        //                                          align: 'stretch'
        //                                      },
        //                                      items: [
        //                                        me.contentText,
        //                                        me.problomText,
        //                                        {
        //                                            xtype: "fieldset",
        //                                            layout: "hbox",
        //                                            width: '100%',
        //                                            align: 'stretch',
        //                                            pack: 'start', margin: '18 0 0 0',
        //                                            baseCls: 'my-panel-no-border',//隐藏边框
        //                                            items: [
        //                                                me.reportDateTimeField,
        //                                                me.scheduleText
        //                                            ]
        //                                        }, {
        //                                            xtype: "fieldset",
        //                                            layout: "hbox",
        //                                            width: '100%',
        //                                            align: 'stretch',
        //                                            pack: 'start', margin: '18 0 0 0',
        //                                            baseCls: 'my-panel-no-border',//隐藏边框
        //                                            items: [
        //                                                me.startDateTimeField,
        //                                                me.laborTimeCombo
                                                        
        //                                            ]
        //                                        },{
        //                                            xtype: "fieldset",
        //                                            layout: "hbox",
        //                                            width: '100%',
        //                                            align: 'stretch',
        //                                            pack: 'start', margin: '18 0 0 0',
        //                                            baseCls: 'my-panel-no-border',//隐藏边框
        //                                            items: [
        //                                                 me.endDateTimeField
        //                                            ]
        //                                        }
        //                                      ]
        //                                  }
        //                              ]
        //                          }
        //                      ],
        //                      flex: 1
        //                  },
        //                {
        //                    xtype: "panel",
        //                    layout: "hbox",
        //                    baseCls: 'my-panel-no-border',//隐藏边框
        //                    //align: 'right',
        //                    //pack: 'end',//组件在容器右边
        //                    items: [{
        //                        flex: 1, margins: "0 5 10 15",
        //                        baseCls: 'my-panel-no-border',//隐藏边框
        //                        items: [
        //                            {
        //                                xtype: "button",
        //                                text: "新建任务", width: 80, 
        //                                listeners: {
        //                                    "click": function (btn, e, eOpts) {//添加点击按钮事件
        //                                        me.onCreateTaskBtnClick();

        //                                    }
        //                                }
        //                            }
        //                        ]
        //                    },
        //                    {
        //                        xtype: "button",
        //                        text: "新建报告", width: 80, margins: "0 5 10 5",
        //                        listeners: {
        //                            "click": function (btn, e, eOpts) {//添加点击按钮事件
        //                                me.onCreateReportBtnClick();

        //                            }
        //                        }
        //                    },
        //                        {
        //                            xtype: "button",
        //                            text: "删除报告", width: 80, margins: "0 5 10 5",
        //                            listeners: {
        //                                "click": function (btn, e, eOpts) {//添加点击按钮事件
        //                                    me.onDeleteReportBtnClick();

        //                                }
        //                            }
        //                        },
        //                        {
        //                            xtype: "button",
        //                            text: "保存修改", width: 80, margins: "0 15 10 5",
        //                            listeners: {
        //                                "click": function (btn, e, eOpts) {//添加点击按钮事件
        //                                    me.onSaveReportClick();
        //                                }
        //                            }
        //                        }
        //                    ]
        //                }
        //              ]
        //          }
        //      ]
        //  })];

        //获取打开表单时的默认参数
        me.sendGetTaskReportDefault();

        me.callParent(arguments);
    },

    //获取打开表单时的默认参数
    sendGetTaskReportDefault: function () {
        var me = this;
        //通过extjs的ajax获取操作全部名称
        Ext.Ajax.request({
            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.HXPC_Plugins.WorkTask", A: "GetTaskReportDefault",
                sid: localStorage.getItem("sid")
            },
            success: function (response, options) {
                me.sendTaskReportDefaul_callback(response, options);//, funCallback);

            }
        });
    },

    //处理获取打开表单时的默认参数的返回事件
    sendTaskReportDefaul_callback: function (response, options) {
        var me = this;
        //获取数据后，更新窗口
        var res = Ext.JSON.decode(response.responseText, true);
        var state = res.success;
        if (state === true) {
            var recod = eval(res.data[0]);

            var taskList = eval(recod.taskList);

            //先清空列表
            me.tasklistgrid.getStore().removeAll();

            for (var itemKey in taskList) {

                if (taskList[itemKey].taskName != undefined) {

                    //插入行到文件taskListGrid
                    var r = Ext.create('tasklistmodel', {
                        //name: files[i].name
                        taskKeyword: taskList[itemKey].taskKeyword,
                        taskType: taskList[itemKey].taskType,
                        taskName: taskList[itemKey].taskName
                    });

                    var rowlength = me.tasklistgrid.getStore().data.length;
                    me.tasklistgrid.getStore().insert(rowlength, r);
                }
            }

            if (me.tasklistgrid.getStore().data.length > 0)
            {
                var gridSM = me.tasklistgrid.getSelectionModel();
                gridSM.select(0);
            }
        }
    },

    //处理任务列表选择一条记录后的事件
    onTaskListGridSelect: function (view, record, index, eOpts) {
        var me = this;

        me.selectTaskKeyword = record.data.taskKeyword;

        me.sendGetTaskReportList(me.selectTaskKeyword);
    },

    //处理任务列表选择一条记录后的事件
    sendGetTaskReportList: function (taskKeyword) {
        var me = this;

        //通过extjs的ajax获取操作全部名称,获取任务的报告列表
        Ext.Ajax.request({
            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.HXPC_Plugins.WorkTask", A: "GetTaskReportList",
                sid: localStorage.getItem("sid"), TaskKeyword: me.selectTaskKeyword
            },
            success: function (response, options) {
                me.getTaskReportList_callback(response, options);//, funCallback);

            }
        });

        me.isCreateReport = false;
    },

    getTaskReportList_callback: function (response, options) {
        var me = this;
        //获取数据后，更新窗口
        var res = Ext.JSON.decode(response.responseText, true);
        var state = res.success;
        if (state === true) {
            var recod = eval(res.data[0]);

            var localIndex = 0;

            var reportList = eval(recod.reportList);
            
            me.reportlistgrid.getStore().removeAll();

            for (var itemKey in reportList) {

                if (reportList[itemKey].reportKeyword != undefined) {

                    //插入行到文件taskListGrid
                    var r = Ext.create('reportlistmodel', {
                        //name: files[i].name
                        reportKeyword: reportList[itemKey].reportKeyword,
                        reportContent: reportList[itemKey].reportContent,
                        reportDate: reportList[itemKey].reportDate,
                        schedule: reportList[itemKey].schedule
                    });

                    var rowlength = me.reportlistgrid.getStore().data.length;
                    me.reportlistgrid.getStore().insert(rowlength, r);

                    if (me.selectReportKeyword === reportList[itemKey].reportKeyword)
                    {
                        localIndex = parseInt(itemKey);
                    }
                }
            }

            if (me.reportlistgrid.getStore().data.length > 0) {
                var gridSM = me.reportlistgrid.getSelectionModel();
                //gridSM.select(0);
                gridSM.select(localIndex);
            } else {
                me.setNullReportValue();
            }
        }
    },

    //处理报告列表选择一条记录后的事件
    onReportListGridSelect: function (view, record, index, eOpts) {
        var me = this;

        me.selectReportKeyword = record.data.reportKeyword;

        me.sendGetTaskReport(me.selectReportKeyword);
    },

    //处理报告列表选择一条记录后的事件
    sendGetTaskReport: function (reportKeyword)
    {
        var me = this;

        //通过extjs的ajax获取操作全部名称,获取任务的报告列表
        Ext.Ajax.request({
            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.HXPC_Plugins.WorkTask", A: "GetTaskReport",
                sid: localStorage.getItem("sid"), ReportKeyword: reportKeyword
            },
            success: function (response, options) {
                me.getTaskReport_callback(response, options);

            }
        });

        me.isCreateReport = false;
    },
    //处理报告列表选择一条记录后的事件的返回
    getTaskReport_callback: function (response, options) {
        var me = this;
        //获取数据后，更新窗口
        var res = Ext.JSON.decode(response.responseText, true);
        var state = res.success;
        if (state === true) {
            var recod = eval(res.data[0]);

            //获取报告内容
            me.contentText.setValue(recod.reportContent);

            //获取报告存在问题
            me.problomText.setValue(recod.reportProblom);
            

            //获取报告完成进度
            me.scheduleText.setValue(recod.schedule);

            //获取报告实耗工时
            me.laborTimeCombo.setValue(recod.laborTime);
            
            //获取报告时间
            var reportDateTime = new Date(Date.parse(recod.reportDate));
            me.reportDateTimeField.setValue(reportDateTime);

            //获取报告开始时间
            var startDateTime = new Date(Date.parse(recod.startDate));
            me.startDateTimeField.setValue(startDateTime);

            //获取报告截止时间
            if (recod.endDate != undefined) {
                var endDateTime = new Date(Date.parse(recod.endDate));
                me.endDateTimeField.setValue(endDateTime);
            } else {
                me.endDateTimeField.setValue(new Date());
            }
        }
    },

    //响应按下了保存修改按钮
    onSaveReportClick: function () {
        var me = this;
        //me.isCreateReport = false;
        if (me.isCreateReport === true) {
            me.sendCreateReport();
        } else {
            me.sendModiReport();
        }
    },

    //创建任务报告
    sendCreateReport: function () {
        var me = this;

        //获取报告内容Text
        var strContent = me.contentText.value;

        //获取报告存在问题Text
        var strProblem = me.problomText.value;

        //获取报告进度Text
        var strSchedule = me.scheduleText.value;

        //获取实耗工时Text
        var strLaborTime = me.laborTimeCombo.value;

        //获取报告时间
        var strReportDate = me.formatDateTimeFieldValue(me.reportDateTimeField.value);

        //获取报告开始时间
        var strStartDate = me.formatDateTimeFieldValue(me.startDateTimeField.value);

        //获取报告截止时间
        var strEndDate = me.formatDateTimeFieldValue(me.endDateTimeField.value);

        //获取表单数据，转换成JSON字符串
        var reportAttr =
        [
            { name: 'content', value: strContent },
            { name: 'problom', value: strProblem },
            { name: 'schedule', value: strSchedule },
            { name: 'laborTime', value: strLaborTime },
            { name: 'reportDate', value: strReportDate },
            { name: 'startDate', value: strStartDate },
            { name: 'endDate', value: strEndDate }
        ]

        var reportAttrJson = Ext.JSON.encode(reportAttr);

        Ext.MessageBox.wait("正在创建任务报告，请稍候...", "等待");

        Ext.Ajax.request({

            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.HXPC_Plugins.WorkTask", A: "CreateTaskReport",
                sid: localStorage.getItem("sid"), taskKeyword: me.selectTaskKeyword,
                reportAttrJson: reportAttrJson
            },
            success: function (response, options) {

                //获取数据后，更新窗口
                var res = Ext.JSON.decode(response.responseText, true);
                var state = res.success;
                if (state === false) {
                    var errmsg = res.msg;
                    Ext.Msg.alert("错误信息", errmsg);
                }
                else {
                    //Ext.MessageBox.close();//关闭等待对话框
                    Ext.Msg.alert("信息", "创建任务报告成功！");

                    me.isCreateReport = false;

                    var recod = eval(res.data[0]);

                    if (recod.reportKeyword != undefined)
                    {
                        me.selectReportKeyword = recod.reportKeyword;
                    }

                    me.sendGetTaskReportList(me.selectTaskKeyword);

                }
            }
        });
    },

    //修改任务报告
    sendModiReport: function () {
        var me = this;

        //获取报告内容Text
        var strContent = me.contentText.value;

        //获取报告存在问题Text
        var strProblem = me.problomText.value;

        //获取报告进度Text
        var strSchedule = me.scheduleText.value;

        //获取实耗工时Text
        var strLaborTime = me.laborTimeCombo.value;

        //获取报告时间
        var strReportDate = me.formatDateTimeFieldValue(me.reportDateTimeField.value);

        //获取报告开始时间
        var strStartDate = me.formatDateTimeFieldValue(me.startDateTimeField.value);

        //获取报告截止时间
        var strEndDate = me.formatDateTimeFieldValue(me.endDateTimeField.value);

        //获取表单数据，转换成JSON字符串
        var reportAttr =
        [
            { name: 'content', value: strContent },
            { name: 'problom', value: strProblem },
            { name: 'schedule', value: strSchedule },
            { name: 'laborTime', value: strLaborTime },
            { name: 'reportDate', value: strReportDate },
            { name: 'startDate', value: strStartDate },
            { name: 'endDate', value: strEndDate }
        ]

        var reportAttrJson = Ext.JSON.encode(reportAttr);

        Ext.MessageBox.wait("正在修改任务报告，请稍候...", "等待");

        Ext.Ajax.request({

            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.HXPC_Plugins.WorkTask", A: "ModiTaskReport",
                sid: localStorage.getItem("sid"), ReportKeyword: me.selectReportKeyword,
                reportAttrJson: reportAttrJson
            },
            success: function (response, options) {

                //获取数据后，更新窗口
                var res = Ext.JSON.decode(response.responseText, true);
                var state = res.success;
                if (state === false) {
                    var errmsg = res.msg;
                    Ext.Msg.alert("错误信息", errmsg);
                }
                else {
                    //Ext.MessageBox.close();//关闭等待对话框
                    Ext.Msg.alert("信息", "保存任务报告成功！");

                    me.isCreateReport = false;

                    me.sendGetTaskReportList(me.selectTaskKeyword);

                }
            }
        });
    },

    //响应新建报告按钮单击事件
    onCreateReportBtnClick: function () {
        var me = this;
        me.isCreateReport = true;

        me.setNullReportValue();
    },

    //当Report为空时，设置界面控件
    setNullReportValue: function () {
        var me = this;

        //初始化报告内容Text
        me.contentText.setValue("");

        //初始化报告存在问题Text
        me.problomText.setValue("");

        //初始化报告进度Text
        me.scheduleText.setValue("0");

        //初始化实耗工时Text
        me.laborTimeCombo.setValue("");

        ////初始化报告时间Field
        me.reportDateTimeField.setValue(new Date());

        ////初始化报告开始时间Field
        me.startDateTimeField.setValue(new Date());

        ////初始化报告截止时间Field
        me.endDateTimeField.setValue(new Date());

        me.isCreateReport = true;
    },

    //响应删除报告按钮单击事件
    onDeleteReportBtnClick: function () {
        var me = this;

        Ext.MessageBox.wait("正在删除任务报告，请稍候...", "等待");

        Ext.Ajax.request({

            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.HXPC_Plugins.WorkTask", A: "DeleteTaskReport",
                sid: localStorage.getItem("sid"), ReportKeyword: me.selectReportKeyword
            },
            success: function (response, options) {

                //获取数据后，更新窗口
                var res = Ext.JSON.decode(response.responseText, true);
                var state = res.success;
                if (state === false) {
                    var errmsg = res.msg;
                    Ext.Msg.alert("错误信息", errmsg);
                }
                else {
                    //Ext.MessageBox.close();//关闭等待对话框
                    Ext.Msg.alert("信息", "删除任务报告成功！");

                    me.isCreateReport = false;

                    me.selectReportKeyword = "";

                    me.sendGetTaskReportList(me.selectTaskKeyword);

                }
            }
        });
    },

    //响应新建任务按钮单击事件
    onCreateTaskBtnClick: function () {
        var me = this;
        var plugins = "HXPC_Plugins";
        var state = "createUserTask";

        var fmCreateUserTask = Ext.create('Ext.plug_ins.' + plugins + '.' + state,
            { title: "", mainPanelId: me.mainPanelId });

        winCreateUserTask = Ext.widget('window', {
            title: '创建工作任务',
            closeAction: 'hide',
            width: 541,
            height: 373,
            minWidth: 541,
            minHeight: 373,
            layout: 'fit',
            resizable: false,
            modal: true,
            closeAction: 'close', //close 关闭  hide  隐藏  
            items: fmCreateUserTask,
            defaultFocus: 'firstName'
        });


        winCreateUserTask.show();

        //监听子窗口关闭事件
        winCreateUserTask.on('close', function () {
            me.selectReportKeyword = "";

            me.sendGetTaskReportDefault();
            //me.sendGetTaskReportList(me.selectTaskKeyword);
        });
    },

    //格式化日期时间控件的值
    formatDateTimeFieldValue: function (fieldValue) {
        var strEndDate = "";
        if (typeof fieldValue === "object") {
            //当控件的值没被修改，获取到的是unix时间对象
            strEndDate = fieldValue;
        } else if (typeof fieldValue === "string") {
            //将20160928171823转换为可以被Date识别的格式 2016/09/28 17:18:23
            var pattern = /(\d{4})(\d{2})(\d{2})(\d{2})(\d{2})(\d{2})/;
            strEndDate = fieldValue.replace(pattern, '$1/$2/$3 $4:$5:$6');
        }
        return strEndDate;
    }

});