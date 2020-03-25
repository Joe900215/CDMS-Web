Ext.define('Ext.plug_ins.HXEPC_Plugins.Document.Panel.FileCodePanel', {
    //extend: 'Ext.container.Container',
    extend: 'Ext.panel.Panel',
    alias: 'widget.fileCodePanel', // 此类的xtype类型为buttontransparent
    layout: {
        type: 'vbox',
        pack: 'start',
        align: 'stretch'
    },
    projectKeyword: '',
    baseCls: 'my-panel-no-border',//隐藏边框
    //height: 80,
    initComponent: function () {
        var me = this;
        me.renderTo = me.el;

        //设置文档是项目文档类型，还是运营类文件类型
        me.docClass = "";

        //发送者分类，发送者是部门还是项目
        me.senderClass = "";
        //接收者分类，接收者是部门还是项目
        me.recverClass = "";

        //记录著录表属性
        me.cataAttrArray = [{ receiveType: "LET",needNewFileCode:false }];

        //当前起草信函时选中的目录（菜单所在目录）
        me.projectKeyword = "";
        //项目目录所在目录
        me.projectDirKeyword = "";

        me.sendCompanyDesc = "";
        me.recCompanyDesc = "";

        //表单分类描述
        me.formClassDesc = "";

        //是否需要新建文件编码
        me.needNewFileCode = true;

        //是否需要输入页数
        me.needInputPage = true;
        //项目管理类文件的代码
        //me.rootProjectCode = "";
        //me.rootProjectDesc = "";

        ///////////////////文件编码/////////////////
        //添加项目号text(项目管理类)
        me.fProjectCodeText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "文件编码", labelWidth: 70, readOnly: true, emptyText: "项目简码",// fieldStyle: ' background-color: #DFE9F6;border-color: #DFE9F6; background-image: none;',
            margin: '10 0 0 10', anchor: "80%", labelAlign: "right", width: 130//flex: 1
        });

        //添加机组text(项目管理类)
        me.crewText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "-", labelWidth: 5, emptyText: "机组", readOnly: true, labelSeparator: '',// 去掉laebl中的冒号labelSeparator: '',
            margin: '10 0 0 10', anchor: "80%", labelAlign: "right", width: 50//flex: 1
        });

        //添加厂房text(项目管理类)
        me.factoryText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "", labelWidth: 1, emptyText: "厂房", readOnly: true, labelSeparator: '',// 去掉laebl中的冒号labelSeparator: '',
            margin: '10 0 0 10', anchor: "80%", labelAlign: "left", width: 50//flex: 1
        });

        //添加系统text(项目管理类)
        me.systemText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "/", labelWidth: 5, emptyText: "系统", readOnly: true, labelSeparator: '', // 去掉laebl中的冒号labelSeparator: '',
            margin: '10 0 0 10', anchor: "80%", labelAlign: "left", width: 50//flex: 1
        });

        //添加专业text(项目管理类)
        me.professionText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "-", labelWidth: 5, readOnly: true, labelSeparator: '', emptyText: "专业", // 去掉laebl中的冒号
            margin: '10 0 0 10', anchor: "80%", labelAlign: "left", width: 50//flex: 1
        });

        //添加工作分类代码text(运营管理类)
        me.workClassText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "文件编码", labelWidth: 60, readOnly: true, emptyText: "工作分类代码", fieldStyle: ' background-color: #DFE9F6;border-color: #DFE9F6; background-image: none;',
            margin: '10 10 0 10', anchor: "80%", labelAlign: "right", width: 120//flex: 1
        });

        //添加工作分项代码text(运营管理类)
        me.workSubText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "-", labelWidth: 5, readOnly: true, labelSeparator: '', emptyText: "工作分项", // 去掉laebl中的冒号
            margin: '10 0 0 0', anchor: "80%", labelAlign: "left", width: 80//flex: 1
        });

        //添加部门text(运营管理类)
        me.departmentText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "-", labelWidth: 5, readOnly: true, labelSeparator: '', emptyText: "部门代码", // 去掉laebl中的冒号
            margin: '10 0 0 10', anchor: "80%", labelAlign: "left", width: 80//flex: 1
        });

        //添加收文类型text
        me.receiveTypeText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "-", labelWidth: 5, readOnly: true, labelSeparator: '', emptyText: "文件类型", // 去掉laebl中的冒号
            margin: '10 0 0 10', anchor: "80%", labelAlign: "left", width: 50, value: "MOM"//flex: 1
        });

        //添加函件流水号text
        me.fNumberText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "-", labelWidth: 5, labelSeparator: '', // 去掉laebl中的冒号
            margin: '10 0 0 10', anchor: "80%", labelAlign: "left", width: 60, //flex: 1
            listeners: {
                change: function (view, newValue, oldValue, eOpts) {
                    me.cataAttrArray[0].fNumber = newValue;
                }
            }
        });

        //版本号
        me.editionText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "-", labelWidth: 5, labelSeparator: '', emptyText: "版本", // 去掉laebl中的冒号
            margin: '10 0 0 10', anchor: "80%", labelAlign: "left", width: 40, //flex: 1
            listeners: {
                change: function (view, newValue, oldValue, eOpts) {
                    me.cataAttrArray[0].edition = newValue;
                }
            }
        });

        /////////////////发文编码/////////////////
        //添加项目号text
        me.projectCodeText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "发文编码", labelWidth: 70, readOnly: true, emptyText: "项目简码",//fieldStyle: ' background-color: #DFE9F6;border-color: #DFE9F6; background-image: none;',//fieldStyle:'color:red',
            margin: '10 0 0 10', anchor: "80%", labelAlign: "right", width: 150//flex: 1
        });

        //添加发文单位代码text
        me.sendCompanyText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "发文单位", labelWidth: 70, readOnly: true, emptyText: "发文单位",
            margin: '10 0 0 10', anchor: "80%", labelAlign: "right", width: 150//flex: 1
        });

    
        //添加收 文单位代码text
        me.recCompanyText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "收文单位", labelWidth: 60, readOnly: true, emptyText: "收文单位",
            margin: '10 0 0 10', anchor: "80%", labelAlign: "left", width: 130//flex: 1
        });




        //添加函件流水号text
        me.numberText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "文件ID", labelWidth: 45, labelSeparator: '', // 去掉laebl中的冒号
            margin: '10 10 0 0', anchor: "80%", labelAlign: "right", width: 150//flex: 1
        });


        //录入属按钮
        me.editAttrButton = Ext.create("Ext.button.Button", {
            text: "录入属性", margins: "10 0 0 10",
            listeners: {
                "click": function (btn, e, eOpts) {//添加点击按钮事件
                    me.editFileAttr();
                }
            }
        });

        me.beforeEditAttr = function () {
            return true;
        }

        me.afterEditAttr = function () {}

        me.projectCodeButton = Ext.create("Ext.button.Button", {
            text: "..", margins: "10 0 0 0",
            listeners: {
                "click": function (btn, e, eOpts) {//添加点击按钮事件
                    me.selectProject();
                }
            }
        });

        //选择机组按钮
        me.crewButton = Ext.create("Ext.button.Button", {
            text: "..", margins: "10 0 0 0",
            listeners: {
                "click": function (btn, e, eOpts) {//添加点击按钮事件

                    if (me.beforeSelectCrew() === false) return;

                    if (me.docClass === "project" && me.projectDirKeyword === "") {
                        Ext.Msg.alert("错误信息", "请选择项目！");
                        return;
                    }

                    var fmSelectCrew = Ext.create('Ext.plug_ins.HXEPC_Plugins.Document.SelectCrew', { title: "", mainPanelId: me.Id, projectKeyword: me.projectDirKeyword });

                    winSelectCrew = Ext.widget('window', {
                        title: '选择机组',//（机组+厂房/系统）',
                        width: 738,
                        height: 558,
                        minWidth: 738,
                        minHeight: 558,
                        layout: 'fit',
                        resizable: true,
                        modal: true,
                        closeAction: 'close', //close 关闭  hide  隐藏  
                        items: fmSelectCrew,
                        defaultFocus: 'firstName'
                    });

                    fmSelectCrew.projectKeyword = me.projectDirKeyword;

                    winSelectCrew.show();


                    //监听子窗口关闭事件
                    winSelectCrew.on('close', function () {
                        if (window.parent.resultvalue != null && window.parent.resultvalue !== "") {

                            var crewCode = "";
                            var crewDesc = "";
                            var crewValue = "";

                            crewCode = window.parent.resultvalue;
                            //crewDesc = window.parent.crewdesclist;
                            //crewValue = window.parent.crewvaluelist;

                            //if (crewCode.indexOf(",") > 0) {
                            // var words = crewCode.split(',')
                            //crewCode = crewCode.substring(0, crewCode.indexOf(","));
                            //crewDesc = crewDesc.substring(0, crewDesc.indexOf(";"));
                            //}

                            me.crewText.setValue(crewCode);

                            me.cataAttrArray[0].crew = crewCode;

                            me.getFileCodeNum();
                        }
                    });
                }
            }
        });

        me.beforeSelectCrew = function () {
            return true;
        }

        //选择厂房按钮
        me.factoryButton = Ext.create("Ext.button.Button", {
            text: "..", margins: "10 0 0 0",
            listeners: {
                "click": function (btn, e, eOpts) {//添加点击按钮事件

                    if (me.beforeSelectFactory() === false) return;

                    if (me.docClass === "project" && me.projectDirKeyword === "") {
                        Ext.Msg.alert("错误信息", "请选择项目！");
                        return;
                    }

                    var fmSelectFactory = Ext.create('Ext.plug_ins.HXEPC_Plugins.Document.SelectFactory', { title: "", mainPanelId: me.Id, projectKeyword: me.projectDirKeyword });

                    winSelectFactory = Ext.widget('window', {
                        title: '选择厂房',
                        width: 738,
                        height: 558,
                        minWidth: 738,
                        minHeight: 558,
                        layout: 'fit',
                        resizable: true,
                        modal: true,
                        closeAction: 'close', //close 关闭  hide  隐藏  
                        items: fmSelectFactory,
                        defaultFocus: 'firstName'
                    });

                    fmSelectFactory.projectKeyword = me.projectDirKeyword;

                    winSelectFactory.show();


                    //监听子窗口关闭事件
                    winSelectFactory.on('close', function () {
                        if (window.parent.resultvalue != null && window.parent.resultvalue !== "") {

                            var factoryCode = "";
                            var factoryDesc = "";
                            var factoryValue = "";

                            factoryCode = window.parent.resultvalue;
                            factoryDesc = window.parent.factorydesclist;
                            //factoryValue = window.parent.factoryvaluelist;

                            if (factoryCode.indexOf(",") > 0) {
                                var words = factoryCode.split(',')
                                factoryCode = factoryCode.substring(0, factoryCode.indexOf(","));
                                factoryDesc = factoryDesc.substring(0, factoryDesc.indexOf(";"));
                            }

                            me.factoryText.setValue(factoryCode);

                            me.cataAttrArray[0].factorycode = factoryCode;
                            me.cataAttrArray[0].factoryname = factoryDesc;

                            me.systemText.setValue("");

                            me.cataAttrArray[0].systemcode = "";
                            me.cataAttrArray[0].systemname = "";

                            me.getFileCodeNum();
                        }
                    });
                }
            }
        });

        me.beforeSelectFactory = function () {
            return true;
        }

        //选择系统按钮
        me.systemButton = Ext.create("Ext.button.Button", {
            text: "..", margins: "10 0 0 0",
            listeners: {
                "click": function (btn, e, eOpts) {//添加点击按钮事件

                    if (me.beforeSelectSystem() === false) return;

                    if (me.docClass === "project" && me.projectDirKeyword === "") {
                        Ext.Msg.alert("错误信息", "请选择项目！");
                        return;
                    }

                    var fmSelectSystem = Ext.create('Ext.plug_ins.HXEPC_Plugins.Document.SelectSystem', { title: "", mainPanelId: me.Id, projectKeyword: me.projectDirKeyword });

                    winSelectSystem = Ext.widget('window', {
                        title: '选择系统',
                        width: 738,
                        height: 558,
                        minWidth: 738,
                        minHeight: 558,
                        layout: 'fit',
                        resizable: true,
                        modal: true,
                        closeAction: 'close', //close 关闭  hide  隐藏  
                        items: fmSelectSystem,
                        defaultFocus: 'firstName'
                    });

                    fmSelectSystem.projectKeyword = me.projectDirKeyword;

                    winSelectSystem.show();


                    //监听子窗口关闭事件
                    winSelectSystem.on('close', function () {
                        if (window.parent.resultvalue != null && window.parent.resultvalue !== "") {

                            var SystemCode = "";
                            var SystemDesc = "";
                            var SystemValue = "";

                            SystemCode = window.parent.resultvalue;
                            SystemDesc = window.parent.systemdesclist;
                            //SystemValue = window.parent.Systemvaluelist;

                            if (SystemCode.indexOf(",") > 0) {
                                var words = SystemCode.split(',')
                                SystemCode = SystemCode.substring(0, SystemCode.indexOf(","));
                                SystemDesc = SystemDesc.substring(0, SystemDesc.indexOf(";"));
                            }

                            me.systemText.setValue(SystemCode);

                            me.cataAttrArray[0].systemcode = SystemCode;
                            me.cataAttrArray[0].systemname = SystemDesc;

                            me.factoryText.setValue("");

                            me.cataAttrArray[0].factorycode = "";
                            me.cataAttrArray[0].factoryname = "";

                            me.getFileCodeNum();

                        }
                    });
                }
            }
        });

        me.beforeSelectSystem = function () {
            return true;
        }

        me.workSubButton = Ext.create("Ext.button.Button", {
            text: "..", margins: "10 0 0 0",
            listeners: {
                "click": function (btn, e, eOpts) {//添加点击按钮事件
                    var fmSelectWorkSub = Ext.create('Ext.plug_ins.HXEPC_Plugins.Document.SelectWorkSub', { title: "", mainPanelId: me.Id, projectKeyword: me.projectKeyword });

                    winSelectWorkSub = Ext.widget('window', {
                        title: '选择工作分项',
                        width: 738,
                        height: 558,
                        minWidth: 738,
                        minHeight: 558,
                        layout: 'fit',
                        resizable: true,
                        modal: true,
                        closeAction: 'close', //close 关闭  hide  隐藏  
                        items: fmSelectWorkSub,
                        defaultFocus: 'firstName'
                    });

                    fmSelectWorkSub.projectKeyword = me.projectKeyword;

                    winSelectWorkSub.show();


                    //监听子窗口关闭事件
                    winSelectWorkSub.on('close', function () {
                        if (window.parent.resultvalue != null && window.parent.resultvalue !== "") {

                            var workSubCode = "";
                            var workSubDesc = "";
                            var workSubValue = "";

                            workSubCode = window.parent.resultvalue;
                            workSubDesc = window.parent.workSubdesclist;
                            workSubType = window.parent.resulttype;

                            if (workSubCode.indexOf(",") > 0) {
                                // var words = workSubCode.split(',')
                                workSubCode = workSubCode.substring(0, workSubCode.indexOf(","));
                                workSubDesc = workSubDesc.substring(0, workSubDesc.indexOf(";"));
                                workSubType = workSubType.substring(0, workSubType.indexOf(","));
                            }

                            me.workSubText.setValue(workSubCode);
                            me.workClassText.setValue(workSubType);

                            me.cataAttrArray[0].workClass = workSubType;
                            me.cataAttrArray[0].workSub = workSubCode;

                            me.getFileCodeNum();
                        }
                    });
                }
            }
        });


        //文件编码里面的选择部门按钮
        me.departmentButton = Ext.create("Ext.button.Button", {
            text: "..", margins: "10 0 0 0",
            listeners: {
                "click": function (btn, e, eOpts) {//添加点击按钮事件
                    me.selectSendDepartment();

                }
            }
        });

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

        me.AfterSelectRecCompany = function () {}

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

        //选择专业按钮
        me.professionButton = Ext.create("Ext.button.Button", {
            text: "..", margins: "10 0 0 0",
            listeners: {
                "click": function (btn, e, eOpts) {//

                    if (me.beforeSelectProfession() === false) return;

                    if (me.docClass === "project" && me.projectDirKeyword === "") {
                        Ext.Msg.alert("错误信息", "请选择项目！");
                        return;

                    }
                    var fmSelectProfession = Ext.create('Ext.plug_ins.HXEPC_Plugins.Document.SelectProfession', { title: "", mainPanelId: me.Id, projectKeyword: me.projectDirKeyword });

                    winSelectProfession = Ext.widget('window', {
                        title: '选择专业',
                        width: 738,
                        height: 558,
                        minWidth: 738,
                        minHeight: 558,
                        layout: 'fit',
                        resizable: true,
                        modal: true,
                        closeAction: 'close', //close 关闭  hide  隐藏  
                        items: fmSelectProfession,
                        defaultFocus: 'firstName'
                    });

                    fmSelectProfession.projectKeyword = me.projectDirKeyword;

                    winSelectProfession.show();


                    //监听子窗口关闭事件
                    winSelectProfession.on('close', function () {
                        if (window.parent.resultvalue != null && window.parent.resultvalue !== "") {

                            var professionCode = "";
                            var professionDesc = "";
                            var professionValue = "";

                            professionCode = window.parent.resultvalue;
                            professionDesc = window.parent.professiondesclist;
                            //professionValue = window.parent.professionvaluelist;

                            if (professionCode.indexOf(",") > 0) {
                                // var words = professionCode.split(',')
                                professionCode = professionCode.substring(0, professionCode.indexOf(","));
                                professionDesc = professionDesc.substring(0, professionDesc.indexOf(";"));
                            }

                            me.professionText.setValue(professionCode);

                            me.cataAttrArray[0].major = professionCode;

                            me.getFileCodeNum();
                        }
                    });
                }
            }
        });

        me.beforeSelectProfession = function () {
            return true;
        }

        //选择来文的文件类型按钮
        me.receiveTypeButton = Ext.create("Ext.button.Button", {
            text: "..", margins: "10 0 0 0",
            listeners: {
                "click": function (btn, e, eOpts) {//添加点击按钮事件

                    if (me.docClass === "project" && me.projectDirKeyword === "") {
                        Ext.Msg.alert("错误信息", "请选择项目！");
                        return;
                    }

                    var fmSelectReceiveType = Ext.create('Ext.plug_ins.HXEPC_Plugins.Document.SelectReceiveType', { title: "", mainPanelId: me.Id, projectKeyword: me.projectDirKeyword });

                    winSelectReceiveType = Ext.widget('window', {
                        title: '选择文件类型',
                        width: 738,
                        height: 558,
                        minWidth: 738,
                        minHeight: 558,
                        layout: 'fit',
                        resizable: true,
                        modal: true,
                        closeAction: 'close', //close 关闭  hide  隐藏  
                        items: fmSelectReceiveType,
                        defaultFocus: 'firstName'
                    });

                    fmSelectReceiveType.projectKeyword = me.projectDirKeyword;

                    winSelectReceiveType.show();


                    //监听子窗口关闭事件
                    winSelectReceiveType.on('close', function () {
                        if (window.parent.resultvalue != null && window.parent.resultvalue !== "") {

                            var receiveTypeCode = "";
                            var receiveTypeDesc = "";
                            var receiveTypeValue = "";

                            receiveTypeCode = window.parent.resultvalue;
                            receiveTypeDesc = window.parent.receiveTypedesclist;
                            //receiveTypeValue = window.parent.receiveTypevaluelist;

                            if (receiveTypeCode.indexOf(",") > 0) {
                                // var words = receiveTypeCode.split(',')
                                receiveTypeCode = receiveTypeCode.substring(0, receiveTypeCode.indexOf(","));
                                receiveTypeDesc = receiveTypeDesc.substring(0, receiveTypeDesc.indexOf(";"));
                            }

                            me.receiveTypeText.setValue(receiveTypeCode);

                            me.cataAttrArray[0].receiveType = receiveTypeCode;

                            me.getFileCodeNum();
                        }
                    });
                }
            }
        });


        me.toProjectCheckBox = Ext.create("Ext.form.field.Checkbox", {
            // xtype: "checkbox",
            fieldLabel: "", labelWidth: 0, margin: '10 10 0 10',
            boxLabel: "发给项目",
            listeners: {
                change: function (view, newValue, oldValue, eOpts) {
                    me.projectDirKeyword = "";
                    //if (newValue === true) {
                    //
                    me.setIsProjectFile(newValue);
                    // }
                }
            }
        });

        me.items = [
            {

                baseCls: 'my-panel-no-border',//隐藏边框
                layout: {
                    type: 'hbox',
                    pack: 'start',
                    align: 'stretch'
                },
                items: [
                    //项目管理类
                    me.fProjectCodeText,
                    me.crewText, me.crewButton,
                    me.factoryText, me.factoryButton,
                    me.systemText, me.systemButton,
                    me.professionText, me.professionButton,

                    //运营管理类
                    me.workClassText,
                    me.workSubText, me.workSubButton,
                    me.departmentText, me.departmentButton,

                    me.receiveTypeText, me.receiveTypeButton,
                    me.fNumberText, me.editionText,
                    me.editAttrButton,
                         {
                             flex: 1, baseCls: 'my-panel-no-border'//隐藏边框
                         },
                         me.toProjectCheckBox
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
                            me.projectCodeText, me.projectCodeButton,
                            me.sendCompanyText, me.sendCompanyButton,
                            me.recCompanyText, me.recCompanyButton,
                            me.numberText
                        ],
                        flex: 1


                    }
        ];

        me.callParent(arguments);
    },

    //设置文档收发文单位的类型
    setDocUnitClass: function(sendUnitClass,recUnitClass) {
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

        me.setTextVisable();
    },


    //设置界面上的控件的显示状态
    setTextVisable: function () {
        var me = this;
        if (me.docClass === "operation")
        {
            me.fProjectCodeText.setVisible(false);

            me.crewText.setVisible(false);
            me.crewButton.setVisible(false);
            me.systemText.setVisible(false);
            me.systemButton.setVisible(false);
            me.factoryText.setVisible(false);
            me.factoryButton.setVisible(false);

            me.professionText.setVisible(false);
            me.professionButton.setVisible(false);

            me.projectCodeText.setVisible(false);
            me.projectCodeButton.setVisible(false);

        } else if (me.docClass === "project") {
            me.workClassText.setVisible(false);
            me.workSubText.setVisible(false);
            me.departmentText.setVisible(false);
            me.workSubButton.setVisible(false);
            me.departmentButton.setVisible(false);


            me.projectCodeText.setValue(me.cataAttrArray[0].procode);
            me.fProjectCodeText.setValue(me.cataAttrArray[0].procode);

            //me.projectCodeText.setValue(me.rootProjectCode);
            //me.fProjectCodeText.setValue(me.rootProjectCode);

            //me.cataAttrArray[0].procode = me.rootProjectCode;
            //me.cataAttrArray[0].proname = me.rootProjectDesc;

            //项目发起的时候，不需要显示发给项目选择框
            me.toProjectCheckBox.setVisible(false);
            //me.sendCompanyText.setValue(sourceCompany);
            //me.senderText.setValue(me.cataAttrArray[0].proname);
        }
    },


    setIsProjectFile: function (isProjectFile) {
        var me = this;
        if (!isProjectFile) {
            //运营信函
            me.docClass = "operation";

            me.fProjectCodeText.setVisible(false);

            me.crewText.setVisible(false);
            me.crewButton.setVisible(false);
            me.systemText.setVisible(false);
            me.systemButton.setVisible(false);
            me.factoryText.setVisible(false);
            me.factoryButton.setVisible(false);

            me.professionText.setVisible(false);
            me.professionButton.setVisible(false);

            me.projectCodeText.setVisible(false);
            me.projectCodeButton.setVisible(false);


            ///////////////////////////////////////////////

            me.workClassText.setVisible(true);
            me.workSubText.setVisible(true);
            me.departmentText.setVisible(true);
            me.workSubButton.setVisible(true);
            me.departmentButton.setVisible(true);
        } else {
            //项目（非运营）信函
            me.docClass = "project";

            me.workClassText.setVisible(false);
            me.workSubText.setVisible(false);
            me.departmentText.setVisible(false);
            me.workSubButton.setVisible(false);
            me.departmentButton.setVisible(false);

            //////////////////////////////////////////////

            me.fProjectCodeText.setVisible(true);

            me.crewText.setVisible(true);
            me.crewButton.setVisible(true);
            me.systemText.setVisible(true);
            me.systemButton.setVisible(true);
            me.factoryText.setVisible(true);
            me.factoryButton.setVisible(true);

            me.professionText.setVisible(true);
            me.professionButton.setVisible(true);

            me.projectCodeText.setVisible(true);
            me.projectCodeButton.setVisible(true);

        }
    },

    setFormClass: function (code, desc) {
        var me = this;

        me.cataAttrArray[0].receiveType = code;//NOT(通知)
        me.receiveTypeText.setValue(code);

        me.formClassDesc = desc;
        
    },

    setSendCompany: function (code, desc) {
        var me = this;
        me.sendCompanyText.setValue(code);
        me.departmentText.setValue(code);
        me.cataAttrArray[0].department = code;
        me.cataAttrArray[0].sendUnitCode = code;
        me.AfterSelectSendCompany(code, desc);
    },

    setRootProject: function (code, desc) {
        var me = this;
        //me.rootProjectCode = code;
        //me.rootProjectDesc = desc;
        me.cataAttrArray[0].procode = code;
        me.cataAttrArray[0].proname = desc;
    },

    //设置是否需要新建文件编码
    setNeedNewFileCode:function(value){
        var me = this;

        me.cataAttrArray[0].needNewFileCode = value;

        if (value === true) {
            me.fNumberText.setVisible(true);
            me.editionText.setVisible(true);
        } else {
            me.fNumberText.setVisible(false);
            me.editionText.setVisible(false);
        }
    },

    //获取文件编码
    getFileCode: function () {
        var me = this;
        var fileCode = "";

        //是否需要新建文件编码
        if (me.cataAttrArray[0].needNewFileCode != true) {
            return fileCode;
        }

        if (me.docClass === "project") {
            //项目管理类
            fileCode = me.projectCodeText.value + "-" + me.crewText.value + 
                me.systemText.value + me.factoryText.value + "-" +
                me.professionText.value + "-" + me.cataAttrArray[0].receiveType + "-" +
                me.fNumberText.value + "-" + me.editionText.value;

        } else if (me.docClass === "operation") {
            //运营管理类
            fileCode = me.workClassText.value + "-" + me.workSubText.value + "-" + me.departmentText.value + "-" +
                me.cataAttrArray[0].receiveType + "-" + me.fNumberText.value + "-" + me.editionText.value;
        }
        return fileCode;
    },

    //获取文件发文编码
    getFileSendCode: function () {
        var me = this;
        var sendCode = "";
        if (me.docClass === "project") {
            //项目管理类
            sendCode = me.projectCodeText.value + "-" + me.sendCompanyText.value + "-" + me.recCompanyText.value + "-" + me.cataAttrArray[0].receiveType + "-" + me.numberText.value;

        } else if (me.docClass === "operation") {
            //运营管理类
            sendCode = me.sendCompanyText.value + "-" + me.recCompanyText.value + "-" + me.cataAttrArray[0].receiveType + "-" + me.numberText.value;
        }
        return sendCode;
    },

    getDocIdentifier: function () {
        var me = this;
        return me.cataAttrArray[0].receiveType;
    },

    //获取发文单位代码
    getSendCompanyCode: function () {
        var me = this;
        var result = me.sendCompanyText.value;
        return result;
    },

    //检查文件代码是否填写完整
    checkFileCodeFill:function(){
        var me = this;
        var isFill = true;

        //判断是否是项目类
        if (me.docClass === "project") {
            if (me.crewText.value === undefined || me.crewText.value === "") {
                return "请选择机组！";
            }
            if ((me.factoryText.value === undefined || me.factoryText.value === "") && 
                (me.systemText.value === undefined || me.systemText.value === "")
                ){
                return "请选择厂房或系统！";
            }
            if (me.professionText.value === undefined || me.professionText.value === "") {
                return "请选择专业！";
            }
  
        } else if (me.docClass === "operation") {
            if (me.workClassText.value === undefined || me.workClassText.value === "") {
                return "请选择工作分类！";
            }
            if (me.workSubText.value === undefined || me.workSubText.value === "") {
                return "请选择工作分项！";
            }
            if (me.departmentText.value === undefined || me.departmentText.value === "") {
                return "请选择部门！";
            }
        }

        if (me.receiveTypeText.value === undefined || me.receiveTypeText.value === "") {
            return "请选择文件类型！";
        }

        if (me.fNumberText.value === undefined || me.fNumberText.value === "") {
            return "请输入流水号！";
        }

        if (me.editionText.value === undefined || me.editionText.value === "") {
            return "请输入版次！";
        }


        if (me.cataAttrArray[0].desc === undefined || me.cataAttrArray[0].desc === "") {
            return "请输入著录属性：文件题名！";
        }


        if (me.cataAttrArray[0].share === undefined || me.cataAttrArray[0].share === "") {
            return "请输入著录属性：份数！";
        }

        if (me.cataAttrArray[0].medium === undefined || me.cataAttrArray[0].medium === "") {
            return "请输入著录属性：介质！";
        }

        return "true";
    },

    //选择项目
    selectProject: function () {
        var me = this;

        var fmSelectProject = Ext.create('Ext.plug_ins.HXEPC_Plugins.Document.SelectProject', { title: "", mainPanelId: me.Id, projectKeyword: me.projectKeyword });

        winSelectProject = Ext.widget('window', {
            title: '选择项目',
            width: 738,
            height: 558,
            minWidth: 738,
            minHeight: 558,
            layout: 'fit',
            resizable: true,
            modal: true,
            closeAction: 'close', //close 关闭  hide  隐藏  
            items: fmSelectProject,
            defaultFocus: 'firstName'
        });

        fmSelectProject.projectKeyword = me.projectKeyword;

        winSelectProject.show();


        //监听子窗口关闭事件
        winSelectProject.on('close', function () {
            //if (window.parent.resultvalue != null && window.parent.resultvalue !== "") {

            var projectCode = "";
            var projectDesc = "";
            var projectValue = "";

            projectCode = window.parent.resultvalue;
            projectDesc = window.parent.projectdesclist;
            projectValue = window.parent.projectvaluelist;

            if (projectCode.indexOf(",") > 0) {
                // var words = projectCode.split(',')
                projectCode = projectCode.substring(0, projectCode.indexOf(","));
                projectDesc = projectDesc.substring(0, projectDesc.indexOf(";"));
                projectValue = projectValue.substring(0, projectValue.indexOf(","));
            }


            me.fProjectCodeText.setValue(projectCode);
            me.projectCodeText.setValue(projectCode);

            me.projectDirKeyword = projectValue;

            me.cataAttrArray[0].procode = projectCode;
            me.cataAttrArray[0].proname = projectDesc;

        });
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
                me.cataAttrArray[0].recUnitCode = unitCode;

                me.recCompanyDesc = unitDesc;

                me.sendGetFileId();

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
                me.cataAttrArray[0].recUnitCode = departmentCode;

                me.recCompanyDesc = departmentDesc;

                me.sendGetFileId();
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

                me.sendGetFileId();
                me.getFileCodeNum();

                
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

                me.sendGetFileId();
                me.getFileCodeNum();

               
            }
        });
    },

    selectCopyCallBackFun :function () {},

                         
    callSelectCopyParty: function () {
        var me = this;
        if (me.docClass === "operation") {
            //运营管理类 ，选择接收部门
            me.selectCopyDepartment();
            return;
        }
        else {
            //项目管理类，选择接收单位
            me.selectCopyUnit();
        }
    },

    selectCopyUnit: function () {
        var me = this;

        if (me.docClass === "project" && me.projectDirKeyword === "") {
            Ext.Msg.alert("错误信息", "请选择项目！");
            return;
        }

        var prjKeyword = "";
        if (me.docClass === "project") {
            prjKeyword = me.projectDirKeyword;
        } else {
            prjKeyword = me.projectKeyword;
        }

        var fmSelectUnit = Ext.create('Ext.plug_ins.HXEPC_Plugins.Document.SelectUnit', { title: "", mainPanelId: me.Id, projectKeyword: prjKeyword });

        winSelectUnit = Ext.widget('window', {
            title: '选择抄送',
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

        // fmSelectUnit.projectKeyword = me.projectKeyword;
        if (me.docClass === "project") {
            fmSelectUnit.projectKeyword = me.projectDirKeyword;
        } else {
            fmSelectUnit.projectKeyword = me.projectKeyword;
        }

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

                me.cataAttrArray[0].copyUnitCode = unitValue;

                me.selectCopyCallBackFun(unitCode, unitDesc);
               

            }
        });
    },

    selectCopyDepartment: function () {
        var me = this;

        if (me.docClass === "project" && me.projectKeyword === "") {
            Ext.Msg.alert("错误信息", "请选择项目！");
            return;
        }

        var fmSelectDepartment = Ext.create('Ext.plug_ins.HXEPC_Plugins.Document.SelectDepartment', { title: "", mainPanelId: me.Id, projectKeyword: me.projectKeyword });

        winSelectDepartment = Ext.widget('window', {
            title: '选择抄送',
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

                //if (departmentCode.indexOf(",") > 0) {
                //    // var words = departmentCode.split(',')
                //    departmentCode = departmentCode.substring(0, departmentCode.indexOf(","));
                //    //departmentDesc = departmentDesc.substring(0, departmentDesc.indexOf(";"));
                //    departmentType = departmentType.substring(0, departmentType.indexOf(","));
                //}

                me.cataAttrArray[0].copyUnitCode = departmentCode;

                me.selectCopyCallBackFun(window.parent.resultvalue, departmentDesc);

                //me.copyPartyList = window.parent.resultvalue;
                //me.copyPartyText.setValue(departmentDesc);
            }
        });
    },
    
    //修改文件属性
    editFileAttr: function () {
        var me = this;

        if (me.beforeEditAttr() === false) return;

        //弹出操作窗口
        var _fmEditFileProperties = Ext.create('Ext.plug_ins.HXEPC_Plugins.Document.EditFileProperties', {
            title: "", projectKeyword: me.projectKeyword, projectDirKeyword: me.projectDirKeyword,
            docClass: me.docClass
        });

        winEditFileProperties = Ext.widget('window', {
            title: '修改文件著录属性',
            closeAction: 'hide',
            width: 780,
            height: 466,
            minWidth: 300,
            minHeight: 300,
            layout: 'fit',
            resizable: true,
            modal: true,
            closeAction: 'close', //close 关闭  hide  隐藏  
            items: _fmEditFileProperties,
            defaultFocus: 'firstName'
        });

        if (me.docClass === "project") {
            _fmEditFileProperties.fileCodeTypeCombo.setValue("项目管理类");
            _fmEditFileProperties.setIsProjectFile();
        }

        _fmEditFileProperties.projectKeyword = me.projectKeyword;
        _fmEditFileProperties.projectDirKeyword = me.projectDirKeyword;//projectKeyword;
        _fmEditFileProperties.needInputPage = me.needInputPage;

        if (me.cataAttrArray[0] != undefined) {
            _fmEditFileProperties.setFilePropertiesDefault(me.cataAttrArray[0]);
        }

        window.parent.resultarray = undefined;

        // winImportFile.hide();
        winEditFileProperties.show();

        _fmEditFileProperties.projectCodeText.setValue(me.projectCodeText.value);

        _fmEditFileProperties.fProjectCodeText.setValue(me.projectCodeText.value);

        //_fmEditFileProperties.projectDescText.setValue(projectDesc);

        //监听子窗口关闭事件
        winEditFileProperties.on('close', function () {
            //winImportFile.show();

            if (window.parent.resultarray === undefined) { return; }
            var res = window.parent.resultarray[0];

            me.updateCataAttr(res);

            //保存著录属性记录
            me.cataAttrArray = window.parent.resultarray;

            me.setNeedNewFileCode(res.needNewFileCode);

            //机组
            me.crewText.setValue(res.crew);
            //厂房
            me.factoryText.setValue(res.factorycode);
            //系统
            me.systemText.setValue(res.systemcode);

            //工作分类
            me.workClassText.setValue(res.workClass);
            //工作分项
            me.workSubText.setValue(res.workSub);
            //部门
            me.departmentText.setValue(res.department);
            me.sendCompanyText.setValue(res.department);
            me.cataAttrArray[0].sendUnitCode = res.department;

            //专业
            me.professionText.setValue(res.major);
            //文件类型
            me.receiveTypeText.setValue(res.receiveType);
            //流水号
            me.fNumberText.setValue(res.fNumber);
            //版次
            me.editionText.setValue(res.edition);

            //页数
            //me.totalPagesText.setValue(res.page);

            //密级
            var secretgrade = res.secretgrade;
            if (!(secretgrade === undefined || secretgrade === "")) {
                // me.seculevelCombo.setRawValue(res.secretgrade);//设置显示值
                // me.seculevelCombo.setValue(res.secretgrade); //设置ID值
            }

            if (secretgrade === "公开") {
                //me.secrTermText.setDisabled(true);
            } else {
                //me.secrTermText.setDisabled(false);
            }

            me.projectCodeText.setValue(res.procode);
            me.fProjectCodeText.setValue(res.procode);

            me.afterEditAttr(res);
        });

    },

    updateCataAttr: function (res) {
        var me = this;

        //保存著录属性记录
        me.cataAttrArray = [res]; //window.parent.resultarray;

        me.setNeedNewFileCode(res.needNewFileCode);

        //机组
        me.crewText.setValue(res.crew);
        //厂房
        me.factoryText.setValue(res.factorycode);
        //系统
        me.systemText.setValue(res.systemcode);

        //工作分类
        me.workClassText.setValue(res.workClass);
        //工作分项
        me.workSubText.setValue(res.workSub);
        //部门
        me.departmentText.setValue(res.department);
        me.sendCompanyText.setValue(res.department);
        me.cataAttrArray[0].sendUnitCode = res.department;

        //专业
        me.professionText.setValue(res.major);
        //文件类型
        me.receiveTypeText.setValue(res.receiveType);
        //流水号
        me.fNumberText.setValue(res.fNumber);
        //版次
        me.editionText.setValue(res.edition);

        //页数
        //me.totalPagesText.setValue(res.page);

        //密级
        var secretgrade = res.secretgrade;
        if (!(secretgrade === undefined || secretgrade === "")) {
            // me.seculevelCombo.setRawValue(res.secretgrade);//设置显示值
            // me.seculevelCombo.setValue(res.secretgrade); //设置ID值
        }

        if (secretgrade === "公开") {
            //me.secrTermText.setDisabled(true);
        } else {
            //me.secrTermText.setDisabled(false);
        }

        me.projectCodeText.setValue(res.procode);
        me.fProjectCodeText.setValue(res.procode);
    },

    getFileId: function () {
        var me = this;
        return me.numberText.value;
    },

    //获取文件编码流水号
    getFileCodeNum: function () {
        var me = this;

        var isProjectFile = false;

        if (me.docClass === "project") {
            isProjectFile = true;
        }

        //var fileCodeType = me.fileCodeTypeCombo.value;
        //if (fileCodeType === "运营管理类") {
        //    isProjectFile = false;
        //} else {
        //    isProjectFile = true;
        //}

        var strPerfix = "";
        if (!isProjectFile) {
            //运营管理类
            var workClass = me.workClassText.value === undefined ? "" : me.workClassText.value;
            var workSub = me.workSubText.value === undefined ? "" : me.workSubText.value;
            var department = me.departmentText.value === undefined ? "" : me.departmentText.value;

            if (workClass === "") return;
            if (workSub === "") return;
            if (department === "") return;

            strPerfix = workClass + "-" + workSub + "-" + department + "-";
        } else {
            //项目管理类
            var fProjectCode = me.fProjectCodeText.value === undefined ? "" : me.fProjectCodeText.value;
            var crew = me.crewText.value === undefined ? "" : me.crewText.value;
            var factory = me.factoryText.value === undefined ? "" : me.factoryText.value;
            var system = me.systemText.value === undefined ? "" : me.systemText.value;
            var profession = me.professionText.value === undefined ? "" : me.professionText.value;

            if (fProjectCode === "") return;
            if (crew === "") return;
            if (system === "" && factory === "") return;
            if (profession === "") return;

            strPerfix = fProjectCode + "-" + crew + factory + system + "-" + profession + "-";
        }

        //文件类型
        var receiveType = me.receiveTypeText.value === undefined ? "" : me.receiveTypeText.value;
        if (receiveType === "") return;

        strPerfix = strPerfix + receiveType + "-";



        Ext.Ajax.request({
            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.HXEPC_Plugins.Document", A: "GetFileCodeNumber",
                sid: localStorage.getItem("sid"),
                FileCodePerfix: strPerfix

            },
            success: function (response, options) {
                //获取数据后，更新窗口
                var res = Ext.JSON.decode(response.responseText, true);
                var state = res.success;
                if (state === true) {
                    var recod = eval(res.data[0]);
                    var runNum = recod.RunNum;//获取流水号
                    me.fNumberText.setValue(runNum);
                    me.editionText.setValue("A");

                    me.cataAttrArray[0].fNumber = runNum;
                    me.cataAttrArray[0].edition = "A";

                } else {
                    var errmsg = res.msg;
                    Ext.Msg.alert("错误信息", errmsg);
                }
            },
            failure: function (response, options) {
                //                Ext.Msg.alert("系统提示", "连接服务器失败，请尝试重新提交！");
            }
        });
    },

    //获取文件ID流水号
    sendGetFileId: function () {
        var me = this;
        var projectCode = me.projectCodeText.value;
        if (projectCode === undefined) {
            projectCode = "";
        }

        Ext.Ajax.request({
            url: 'WebApi/Post',
            method: "POST",
            params: {
                //C: "AVEVA.CDMS.HXEPC_Plugins.Document", A: "GetMeetMinutesCNNumber",
                C: "AVEVA.CDMS.HXEPC_Plugins.Document", A: "GetFileId",
                ProjectCode: projectCode,
                CommType: "S", DocType: me.cataAttrArray[0].receiveType,
                sid: localStorage.getItem("sid")
            },
            success: function (response, options) {
                //获取数据后，更新窗口
                var res = Ext.JSON.decode(response.responseText, true);
                var state = res.success;
                if (state === true) {
                    var recod = eval(res.data[0]);
                    var runNum = recod.FileId;//获取流水号
                    me.numberText.setValue(runNum);
                } else {
                    var errmsg = res.msg;
                    Ext.Msg.alert("错误信息", errmsg);
                }
            },
            failure: function (response, options) {
                //                Ext.Msg.alert("系统提示", "连接服务器失败，请尝试重新提交！");
            }
        });
    },

    //设置是否隐藏发文编码框
    setFileCodeTextVisible: function (value) {
        var me = this;
        //
        if (value === false) {
            me.projectCodeText.hide();
            me.sendCompanyText.hide();
            me.recCompanyText.hide();
            me.numberText.hide();

            me.projectCodeButton.hide();
            me.sendCompanyButton.hide();
            me.recCompanyButton.hide();
        }
    },
});
