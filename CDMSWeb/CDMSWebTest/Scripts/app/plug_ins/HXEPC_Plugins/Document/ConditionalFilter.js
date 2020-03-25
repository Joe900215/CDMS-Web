//统计/查询 条件选择框

Ext.define('Ext.plug_ins.HXEPC_Plugins.Document.ConditionalFilter', {
    extend: 'Ext.container.Container',
    alias: 'widget.ConditionalFilter',
    //layout: "border",
    layout: 'fit',
    resultvalue: '', mainPanelId: '', projectKeyword: '',
    initComponent: function () {
        var me = this;

        //设置文档是项目文档类型，还是运营类文件类型
        me.docClass = "";

        //发送者分类，发送者是部门还是项目
        me.senderClass = "";
        //接收者分类，接收者是部门还是项目
        me.recverClass = "";

        //记录著录表属性
        me.cataAttrArray = [{ receiveType: "LET", needNewFileCode: false }];

        me.sendCompanyDesc = "";
        me.recCompanyDesc = "";


        //收发文单位列表初始数据
        me.recCompanyList = [];
        me.sendCompanyList = [];

        //定义发送日期Text
        me.startDateField = Ext.create("Ext.form.field.Date", {
            name: "date",
            fieldLabel: '起始', fieldStyle: ' background-image: none;',
            editable: true, labelWidth: 60, margin: '5 0 0 0',//margin,顺时针，上右下左
            labelAlign: "right", labelPad: 8,
            emptyText: "--请选择--", autoLoadOnValue: true,
            format: 'Y年m月d日',
            value: new Date(),
            //width: '50%'
            width: 200
        });
        //定义发送日期Text
        me.endDateField = Ext.create("Ext.form.field.Date", {
            name: "date",
            fieldLabel: '结束', fieldStyle: ' background-image: none;',
            editable: true, labelWidth: 40, margin: '5 0 0 0',//margin,顺时针，上右下左
            labelAlign: "right", labelPad: 8,
            emptyText: "--请选择--", autoLoadOnValue: true,
            format: 'Y年m月d日',
            value: new Date(),
            //width: '50%'
            width: 200
        });

        //添加发文单位代码text
        me.sendCompanyText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "", labelWidth: 0, readOnly: true, emptyText: "发文单位",
            margin: '10 0 0 8', anchor: "80%", labelAlign: "left", width: 96//flex: 1
        });


        //添加收 文单位代码text
        me.recCompanyText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "", labelWidth: 0, readOnly: true, emptyText: "收文单位",
            margin: '10 0 0 8', anchor: "80%", labelAlign: "left", width: 96//flex: 1
        });


        //添加发文单位代码label
        me.sendCompanyNameText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "", labelWidth: 0, readOnly: true, emptyText: "",
            margin: '10 0 0 8', anchor: "80%", width: 220//flex: 1
        });
        me.sendCompanyNameText.readOnly = true;

        //添加收 文单位代码label
        me.recCompanyNameText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "", labelWidth: 0, readOnly: true, emptyText: "",
            margin: '10 0 0 8', anchor: "80%", width: 220//flex: 1
        });
        me.recCompanyNameText.readOnly = true;



        //选择发文单位按钮
        me.sendCompanyButton = Ext.create("Ext.button.Button", {
            text: "..", margins: "10 0 0 0",
            listeners: {
                "click": function (btn, e, eOpts) {//添加点击按钮事件
                    if (me.docClass === "operation") {
                        //运营管理类，选择项目部门
                        me.selectSendDepartment();
                    } else {
                        me.selectSendUnit();
                    }
                }
            }
        });
        me.AfterSelectSendCompany = function () { }

        //选择收文单位按钮
        me.recCompanyButton = Ext.create("Ext.button.Button", {
            text: "..", margins: "10 0 0 0",
            listeners: {
                "click": function (btn, e, eOpts) {//添加点击按钮事件
                    if (me.docClass === "operation") {
                        //运营管理类，选择项目部门
                        me.selectRecDepartment();
                    } else {
                        me.selectRecUnit();
                    }
                }
            }
        });

        me.AfterSelectRecCompany = function () { }


        me.datacheckBox = Ext.create("Ext.form.field.Checkbox", {
            fieldLabel: "", margin: '5 0 0 10',labelWidth: 0,
            boxLabel: "选择生成日期",
            listeners: {
                change: function (view, newValue, oldValue, eOpts) {
                    if (newValue === true) {
                        // me.fNumberText.setVisible(true);
                    } else {
                        // me.fNumberText.setVisible(false);
                    }
                }
            }
        });

        me.sendcheckBox = Ext.create("Ext.form.field.Checkbox", {
            fieldLabel: "", margin: '10 0 0 10', labelWidth: 0,
            boxLabel: "发文单位",
            listeners: {
                change: function (view, newValue, oldValue, eOpts) {
                    if (newValue === true) {
                        // me.fNumberText.setVisible(true);
                    } else {
                        // me.fNumberText.setVisible(false);
                    }
                }
            }
        });

        me.reccheckBox = Ext.create("Ext.form.field.Checkbox", {
            fieldLabel: "", margin: '10 0 0 10', labelWidth: 0,
            boxLabel: "收文单位",
            listeners: {
                change: function (view, newValue, oldValue, eOpts) {
                    if (newValue === true) {
                        // me.fNumberText.setVisible(true);
                    } else {
                        // me.fNumberText.setVisible(false);
                    }
                }
            }
        });

        me.checkboxgroup = Ext.create("Ext.form.CheckboxGroup", {
            layout: {
                type: 'hbox',
                pack: 'start',
                align: 'stretch'
            },
            margin: '10 0 0 10',
            fieldLabel: '是否回复',
            labelWidth: 70,
            // Arrange checkboxes into two columns, distributed vertically
            columns: 2,
            vertical: true,
            items: [
                { boxLabel: '是', name: 'reply', inputValue: '1', margin: '0 20 0 0' },
                { boxLabel: '否', name: 'reply', inputValue: '0', margin: '0 20 0 0' },
            ]
        });


        ////编辑区域头部
        //me.editTopPanel = Ext.create("Ext.panel.Panel", {
        //    baseCls: 'my-panel-no-border',//隐藏边框
        //    layout: {
        //        type: 'vbox',
        //        pack: 'start',
        //        align: 'stretch'
        //    },
        //    margin: '0 0 0 0',// 
        //    items: [
        //             {
        //                 baseCls: 'my-panel-no-border',//隐藏边框
        //                 layout: {
        //                     type: 'hbox',
        //                     pack: 'start',
        //                     align: 'stretch'
        //                 },
        //                 items: [
        //                      me.datacheckBox,
        //                     {
        //                         xtype: "label",
        //                         margin: '6 0 0 8',
        //                         text: "选择生成日期"
        //                     }
        //                 ],
        //                 flex: 1
        //             },
        //            {
        //                baseCls: 'my-panel-no-border',//隐藏边框
        //                layout: {
        //                    type: 'hbox',
        //                    pack: 'start',
        //                    align: 'stretch'
        //                },
        //                items: [
        //                     //me.datacheckBox,
        //                     me.startDateField,
        //                     me.endDateField
        //                ],
        //                flex: 1
        //            },
        //            {
        //                baseCls: 'my-panel-no-border',//隐藏边框
        //                layout: {
        //                    type: 'hbox',
        //                    pack: 'start',
        //                    align: 'stretch'
        //                },
        //                items: [
        //                    me.sendcheckBox,
        //                    me.sendCompanyText, me.sendCompanyButton,
        //                ]
        //            },
        //            {
        //                baseCls: 'my-panel-no-border',//隐藏边框
        //                layout: {
        //                    type: 'hbox',
        //                    pack: 'start',
        //                    align: 'stretch'
        //                },
        //                items: [
        //                    me.reccheckBox,
        //                    me.recCompanyText, me.recCompanyButton, ]
        //            },
        //          me.radiogroup
        //    ]
        //});
        ////编辑区域头部
        me.editTopPanel = Ext.create("Ext.panel.Panel", {
            baseCls: 'my-panel-no-border',//隐藏边框
            layout: 'fit',
            margin: '0 0 0 0',
            items: [
                {
                    //第 1 列中的  Fieldset - 通过toggle 按钮来 收缩/展开
                    xtype: 'fieldset',
                   // columnWidth: 0.5,
                    title: '条件',
                    collapsible: false,
                    layout: {
                        type: 'vbox',
                        pack: 'start',
                        align: 'stretch'
                    },
                    margin: '0 15 0 15',
                    padding :'0 0 30 0',
                   // height:260,
                    items: [
                     {
                         baseCls: 'my-panel-no-border',//隐藏边框
                         layout: {
                             type: 'hbox',
                             pack: 'start',
                             align: 'stretch'
                         },
                         items: [
                              me.datacheckBox,
                             //{
                             //    xtype: "label",
                             //    margin: '6 0 0 8',
                             //    text: "选择生成日期"
                             //}
                         ],
                         flex: 1
                     },
                    {

                        baseCls: 'my-panel-no-border',//隐藏边框
                        layout: {
                            type: 'hbox',
                            pack: 'start',
                            align: 'stretch'
                        },
                        items: [
                             //me.datacheckBox,
                             me.startDateField,
                             me.endDateField
                        ],
                        flex: 1
                    },
                    {
                        baseCls: 'my-panel-no-border',//隐藏边框
                        layout: {
                            type: 'hbox',
                            pack: 'start',
                            align: 'stretch'
                        },
                        items: [
                            me.sendcheckBox,
                            me.sendCompanyText, me.sendCompanyButton, me.sendCompanyNameText
                        ]
                    },
                    {
                        baseCls: 'my-panel-no-border',//隐藏边框
                        layout: {
                            type: 'hbox',
                            pack: 'start',
                            align: 'stretch'
                        },
                        items: [
                            me.reccheckBox,
                            me.recCompanyText, me.recCompanyButton, me.recCompanyNameText]
                    },    
                    me.checkboxgroup

                    ]
                }]
        });

        me.exportStatisProperForm = new Ext.form.FormPanel({});

        //添加列表
        me.items = [
          Ext.widget('form', {
              baseCls: 'my-panel-no-border',//隐藏边框
              layout: {
                  type: 'vbox',
                  align: 'stretch',
                  pack: 'start'
              },
              items: [{//上部容器
                  baseCls: 'my-panel-no-border',//隐藏边框
                  layout: {
                      type: 'vbox',
                      pack: 'start',
                      align: 'stretch'
                  },
                  margin: '10 0 0 0',
                  items: [
                     me.editTopPanel,
                  ], flex: 1
              },
  
              ]
          })

        ];
        me.sendGetDraftRecognitionDefault();
        me.callParent(arguments);
    },


    //返回json字符串
    getParam:function(){
        var me = this;
        var itemsstr = "";

        var stardata = "";//起始日期
        var enddata = "";//终止日期
        var sendCompany = "";//发送单位
        var recCompany = "";//接收单位
        var reply = "";//是否需回复

        if(me.datacheckBox.getValue())
        {
            stardata = me.startDateField.getRawValue();
            enddata = me.endDateField.getRawValue();
        }
        if (me.sendcheckBox.getValue()) {
            sendCompany = me.sendCompanyText.value;
        }
       
        if (me.reccheckBox.getValue()) {
            recCompany = me.recCompanyText.value;
        }
        var replycheck = me.checkboxgroup.getChecked();
        var hobby;
        Ext.Array.each(replycheck, function (item) {
            if (hobby === "" || hobby === undefined) {
                hobby = item.inputValue ;
            }
            else {
                hobby =  hobby + ','+ item.inputValue;
            }
        });
        if (hobby === "" || hobby === undefined)
        {
            reply = "";
        }
        else
        {
            reply = hobby;
            //var strArray = hobby.split(',');
            //if (strArray.length > 1) reply = "";
            //else reply = strArray[0];
        }
        //数据，转换成JSON字符串
        var itemAttr =
        [
            { name: 'stardata', value: stardata },//生成的起始日期
            { name: 'enddata', value: enddata },//生成的终止日期
            { name: 'sendCompany', value: sendCompany },//发送单位
            { name: 'recCompany', value: recCompany },//接收单位
            { name: 'reply', value: reply }//是否需回复
        ];

        return Ext.JSON.encode(itemAttr);
    },


    //选择收文单位
    selectRecUnit: function () {
        var me = this;

        var fmSelectUnit = Ext.create('Ext.plug_ins.HXEPC_Plugins.Document.SelectUnit', { title: "", mainPanelId: me.Id, projectKeyword: me.projectDirKeyword });

        winSelectUnit = Ext.widget('window', {
            title: '选择主送',
            width: 738,
            height: 558,
            minWidth: 738,
            minHeight: 558,
            layout: 'fit',
            resizable: true,
            modal: true,
            closeAction: 'close', //close 关闭  hide  隐藏  
            items: fmSelectUnit,
            defaultFocus: 'firstName'
        });

        fmSelectUnit.projectKeyword = me.projectDirKeyword;

        winSelectUnit.show();


        //监听子窗口关闭事件
        winSelectUnit.on('close', function () {
            if (window.parent.resultvalue != null && window.parent.resultvalue !== "") {

                var unitCode = "";
                var unitDesc = "";
                var unitValue = "";

                unitCode = window.parent.resultvalue;
                unitDesc = window.parent.unitdesclist;
                unitValue = window.parent.unitvaluelist;

                if (unitCode.indexOf(",") > 0) {
                    // var words = unitCode.split(',')
                    unitCode = unitCode.substring(0, unitCode.indexOf(","));
                    unitDesc = unitDesc.substring(0, unitDesc.indexOf(";"));
                }


                //me.mainFeederText.setValue(unitDesc);

                me.recCompanyText.setValue(unitCode);
                if (unitCode === undefined || unitCode === "")
                { }
                else
                {
                    me.reccheckBox.setValue(true);
                }
                
                me.cataAttrArray[0].recUnitCode = unitCode;

                me.recCompanyDesc = unitDesc;
                me.recCompanyNameText.setValue(unitDesc);
                //me.sendGetFileId();

                me.AfterSelectRecCompany(unitCode, unitDesc);
            }
        });
    },


    selectRecDepartment: function () {
        var me = this;

        var fmSelectDepartment = Ext.create('Ext.plug_ins.HXEPC_Plugins.Document.SelectDepartment', { title: "", mainPanelId: me.Id, projectKeyword: me.projectKeyword });

        winSelectDepartment = Ext.widget('window', {
            title: '选择项目部门',
            width: 738,
            height: 558,
            minWidth: 738,
            minHeight: 558,
            layout: 'fit',
            resizable: true,
            modal: true,
            closeAction: 'close', //close 关闭  hide  隐藏  
            items: fmSelectDepartment,
            defaultFocus: 'firstName'
        });

        fmSelectDepartment.projectKeyword = me.projectKeyword;

        winSelectDepartment.show();


        //监听子窗口关闭事件
        winSelectDepartment.on('close', function () {
            if (window.parent.resultvalue != null && window.parent.resultvalue !== "") {

                var departmentCode = "";
                var departmentDesc = "";
                var departmentValue = "";

                departmentCode = window.parent.resultvalue;
                departmentDesc = window.parent.departmentdesclist;
                departmentType = window.parent.resulttype;

                if (departmentCode.indexOf(",") > 0) {
                    // var words = departmentCode.split(',')
                    departmentCode = departmentCode.substring(0, departmentCode.indexOf(","));
                    departmentDesc = departmentDesc.substring(0, departmentDesc.indexOf(";"));
                    departmentType = departmentType.substring(0, departmentType.indexOf(","));
                }

                //me.mainFeederText.setValue(departmentDesc);

                me.recCompanyText.setValue(departmentCode);

                if (departmentCode === undefined || departmentCode === "")
                { }
                else
                {
                    me.reccheckBox.setValue(true);
                }

                me.cataAttrArray[0].recUnitCode = departmentCode;

                me.recCompanyDesc = departmentDesc;
                me.recCompanyNameText.setValue(departmentDesc);
               // me.sendGetFileId();
                me.AfterSelectRecCompany(departmentCode, departmentDesc);
            }
        });
    },


    selectSendUnit: function () {
        var me = this;

        var fmSelectUnit = Ext.create('Ext.plug_ins.HXEPC_Plugins.Document.SelectUnit', { title: "", mainPanelId: me.Id, projectKeyword: me.projectDirKeyword });

        winSelectUnit = Ext.widget('window', {
            title: '选择发文单位',
            width: 738,
            height: 558,
            minWidth: 738,
            minHeight: 558,
            layout: 'fit',
            resizable: true,
            modal: true,
            closeAction: 'close', //close 关闭  hide  隐藏  
            items: fmSelectUnit,
            defaultFocus: 'firstName'
        });

        fmSelectUnit.projectKeyword = me.projectDirKeyword;

        winSelectUnit.show();


        //监听子窗口关闭事件
        winSelectUnit.on('close', function () {
            if (window.parent.resultvalue != null && window.parent.resultvalue !== "") {

                var unitCode = "";
                var unitDesc = "";
                var unitValue = "";

                unitCode = window.parent.resultvalue;
                unitDesc = window.parent.unitdesclist;
                unitValue = window.parent.unitvaluelist;

                if (unitCode.indexOf(",") > 0) {
                    // var words = unitCode.split(',')
                    unitCode = unitCode.substring(0, unitCode.indexOf(","));
                    unitDesc = unitDesc.substring(0, unitDesc.indexOf(";"));
                }


                //me.senderText.setValue(unitDesc);

                me.setSendCompany(unitCode, unitDesc);

                //me.sendCompanyText.setValue(unitCode);

                //me.departmentText.setValue(unitCode);

                //me.cataAttrArray[0].department = unitCode;

               // me.sendGetFileId();
               // me.getFileCodeNum();


            }
        });
    },

    selectSendDepartment: function () {
        var me = this;

        var fmSelectDepartment = Ext.create('Ext.plug_ins.HXEPC_Plugins.Document.SelectDepartment', { title: "", mainPanelId: me.Id, projectKeyword: me.projectKeyword });

        winSelectDepartment = Ext.widget('window', {
            title: '选择发文单位',
            width: 738,
            height: 558,
            minWidth: 738,
            minHeight: 558,
            layout: 'fit',
            resizable: true,
            modal: true,
            closeAction: 'close', //close 关闭  hide  隐藏  
            items: fmSelectDepartment,
            defaultFocus: 'firstName'
        });

        fmSelectDepartment.projectKeyword = me.projectKeyword;

        winSelectDepartment.show();


        //监听子窗口关闭事件
        winSelectDepartment.on('close', function () {
            if (window.parent.resultvalue != null && window.parent.resultvalue !== "") {

                var departmentCode = "";
                var departmentDesc = "";
                var departmentValue = "";

                departmentCode = window.parent.resultvalue;
                departmentDesc = window.parent.departmentdesclist;
                departmentType = window.parent.resulttype;

                if (departmentCode.indexOf(",") > 0) {
                    // var words = departmentCode.split(',')
                    departmentCode = departmentCode.substring(0, departmentCode.indexOf(","));
                    departmentDesc = departmentDesc.substring(0, departmentDesc.indexOf(";"));
                    departmentType = departmentType.substring(0, departmentType.indexOf(","));
                }

                //me.senderText.setValue(departmentDesc);

                //me.sendCompanyText.setValue(departmentCode);

                //me.departmentText.setValue(departmentCode);

                //me.cataAttrArray[0].department = departmentCode;

                me.setSendCompany(departmentCode, departmentDesc);

               // me.sendGetFileId();
              //  me.getFileCodeNum();


            }
        });
    },

    setSendCompany: function (code, desc) {
        var me = this;
        me.sendCompanyText.setValue(code);
        me.sendCompanyNameText.setValue(desc);

        if (code === undefined || code === "")
        { }
        else
        {
            me.sendcheckBox.setValue(true);
        }

      //  me.departmentText.setValue(code);
        me.cataAttrArray[0].department = code;
        me.cataAttrArray[0].sendUnitCode = code;
        me.AfterSelectSendCompany(code, desc);
    },
    //设置文档收发文单位的类型
    setDocUnitClass: function (sendUnitClass, recUnitClass) {
        var me = this;
        //发送者分类，发送者是部门还是项目
        me.senderClass = sendUnitClass;
        //接收者分类，接收者是部门还是项目
        me.recverClass = recUnitClass;

        //设置文档是项目文档类型，还是运营类文件类型
        me.docClass = "";
        if (sendUnitClass === "部门" && recUnitClass === "部门") {
            me.docClass = "operation";
        } else {
            me.docClass = "project";
        }
    },

    //获取起草信函表单默认参数
    sendGetDraftRecognitionDefault: function (funCallback) {
        var me = this;

        //通过extjs的ajax获取操作全部名称
        Ext.Ajax.request({
            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.HXEPC_Plugins.Document", A: "GetDraftRecognitionDefault",
                sid: localStorage.getItem("sid"), ProjectKeyword: me.projectKeyword
            },
            success: function (response, options) {
                me.sendGetDraftRecognitionDefault_callback(response, options, funCallback);

            }
        });
    },

    //处理获取发文处理表单默认参数的返回
    sendGetDraftRecognitionDefault_callback: function (response, options, funCallback) {
        var me = this;

        //获取数据后，更新窗口
        var res = Ext.JSON.decode(response.responseText, true);
        var state = res.success;
        if (state === true) {
            var recod = eval(res.data[0]);

            var strRootProjectCode = recod.RootProjectCode;
            var strRootProjectDesc = recod.RootProjectDesc;

            var strDocNumber = recod.DocNumber;
            me.recCompanyList = eval(recod.RecCompanyList);
            me.sendCompanyList = eval(recod.SendCompanyList);
            //var sourceCompany = recod.SourceCompany;//项目所属公司

            //var strFormClassCode = "AAA"
            //var strProjectDesc = "认质认价";

            ////默认设置为不新建文件编码
            //me.fileCodePanel.setNeedNewFileCode(true);

            ////设置发起目录和项目所在目录
            //me.fileCodePanel.projectKeyword = me.projectKeyword;//项目所在目录
            //me.fileCodePanel.projectDirKeyword = me.projectKeyword;//当前目录

            ////设置收发文单位的单位类型
            //me.fileCodePanel.setFormClass(strFormClassCode, strProjectDesc);
            ////设置项目管理类文件里项目的代码和描述
            //me.fileCodePanel.setRootProject(strRootProjectCode, strRootProjectDesc);

            //认质认价默认都是项目发到部门
            //me.fileCodePanel.setDocUnitClass("项目", "部门");
            if (strRootProjectCode === undefined || strRootProjectCode === "") {
                //运营信函

                me.setDocUnitClass("部门", "部门");
                //隐藏发给项目单选框
                //me.fileCodePanel.toProjectCheckBox.setVisible(false);


            } else {
                //项目（非运营）信函
                me.setDocUnitClass("项目", "项目");

            }

            //设置发文单位代码
           // me.setSendCompany(sourceCompany);

            //设置文件编码Panel的各个按钮的用户事件
            me.AfterSelectRecCompany = function (code, desc) {
                //me.deliveryUnitText.setValue(desc);
            }

            //隐藏发文编码文本框
           // me.fileCodePanel.setFileCodeTextVisible(false);

            //设置第一个文件是正件
            //me.fileCodePanel.firstFileIsPositive = true;

            //me.fileCodePanel.setFirstFileIsPositive(true);

            //设置文件上传表格的模式
           // me.fileUploadPanel.setFileGridMode("REC");

           // me.fNumberText.setValue(strDocNumber);

            var recobjLength = 0;
            //遍历来往单位数组，添加到来往单位combo
            for (var itemKey in me.recCompanyList) {
                //var strCompany = me.recCompanyList[itemKey];
                //me.recCompanydata.push({ text: itemKey, value: itemKey });//在数组里添加新的元素  

                recobjLength = recobjLength + 1;

            }


            var sourceUnitIndex = -1;
            var sendobjLength = 0;
            var companyDesc = "";

            //遍历来往单位数组，添加到来往单位combo
            for (var itemKey in me.sendCompanyList) {

                //    me.sendCompanydata.push({ text: itemKey, value: itemKey });//在数组里添加新的元素  

                //    if (sourceCompany != undefined && itemKey === sourceCompany) {
                sourceUnitIndex = sendobjLength;
                // companyDesc = me.sendCompanyList[itemKey];
                //    }

                //    sendobjLength = sendobjLength + 1;

            }


            if (sendobjLength > 0 && sourceUnitIndex != -1) {
                // me.sendCompanyCombo.setRawValue(me.sendCompanydata[sourceUnitIndex].text);//设置显示值
                //me.sendCompanyCombo.setValue(me.sendCompanydata[sourceUnitIndex].value); //设置ID值

                //me.senderText.setValue(companyDesc);
            }

            funCallback();
        }
    },


});
