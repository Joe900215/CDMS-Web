Ext.define('Ext.plug_ins.HXEPC_Plugins.Document.EditFileProperties', {
    extend: 'Ext.container.Container',
    alias: 'widget.EditFileProperties',
    //layout: "border",
    layout: 'fit',
    resultvalue: '', mainPanelId: '', projectKeyword: '',projectDirKeyword:'',docClass:'',
    initComponent: function () {
        var me = this;
        me.renderTo = me.el;

        //保密等级combo初始数据
        me.seculeveldata = [{ text: "商业秘密", value: "商业秘密" }, { text: "受限", value: "受限" }, { text: "公开", value: "公开" }];

        //文件编码类型
        me.fileCodeTypedata = [{ text: "项目管理类", value: "项目管理类" }, { text: "运营管理类", value: "运营管理类" }];

        me.fa = {};
        ////附件的参数传递
        //me.receiptcode = "";

        ////原件份数
        //me.originalshare = "";
        ////复印件份数
        //me.copyshare = "";
        ////扫描件份数
        //me.scanshare = "";
        ////电子文件份数
        //me.elecshare = "";

        //是否需要输入页数
        me.needInputPage = true;

        /////////////////文件编码/////////////////
        //添加项目代码text(项目管理类)
        me.fProjectCodeText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "文件编码", labelWidth: 60, readOnly: true, fieldStyle: ' background-color: #DFE9F6;border-color: #DFE9F6; background-image: none;',//fieldStyle:'color:red',
            margin: '10 0 0 10', anchor: "80%", labelAlign: "right", width: 120//flex: 1
        });

        //添加机组text(项目管理类)
        me.crewText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "-", labelWidth: 10, readOnly: true, emptyText: "机组", labelSeparator: '', // 去掉laebl中的冒号
            margin: '10 0 0 0', anchor: "80%", labelAlign: "right", width: 50//flex: 1
        });

        //添加厂房text(项目管理类)
        me.factoryText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "", labelWidth: 0, readOnly: true, emptyText: "厂房", labelSeparator: '', // 去掉laebl中的冒号
            margin: '10 0 0 10', anchor: "80%", labelAlign: "left", width: 40//flex: 1
        });

        //添加系统text(项目管理类)
        me.systemText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "/", labelWidth: 10, readOnly: true, emptyText: "系统", labelSeparator: '',// 去掉laebl中的冒号
            margin: '10 0 0 10', anchor: "80%", labelAlign: "left", width: 60//flex: 1
        });

        //添加专业text(项目管理类)
        me.professionText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "-", labelWidth: 10, readOnly: true, emptyText: "专业", labelSeparator: '',// 去掉laebl中的冒号
            margin: '10 0 0 10', anchor: "80%", labelAlign: "left", width: 60//flex: 1
        });

        //添加机组text(项目管理类)
        me.crew2Text = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "机组号", labelWidth: 80, readOnly: true, emptyText: "", //
            margin: '10 0 0 10', anchor: "80%", labelAlign: "right", flex: 1
        });

        //添加厂房text(项目管理类)
        me.factory2Text = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "厂房代码", labelWidth: 80, readOnly: true, emptyText: "", //
            margin: '10 0 0 10', anchor: "80%", labelAlign: "right", flex: 1
        });

        me.factoryDescText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "厂房名称", labelWidth: 80,  emptyText: "", //
            margin: '10 0 0 10', anchor: "80%", labelAlign: "right", flex: 1
        });

        //添加系统text(项目管理类)
        me.system2Text = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "系统代码", labelWidth: 80, readOnly: true, emptyText: "", //
            margin: '10 0 0 10', anchor: "80%", labelAlign: "right", flex: 1
        });

        //添加系统text(项目管理类)
        me.systemDescText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "系统名称", labelWidth: 80, emptyText: "", //
            margin: '10 0 0 10', anchor: "80%", labelAlign: "right", flex: 1
        });

        //添加专业text(项目管理类)
        me.profession2Text = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "专业", labelWidth: 80, readOnly: true, emptyText: "", //
            margin: '10 0 0 10', anchor: "80%", labelAlign: "right", flex: 1
        });

        //添加工作分类代码text(运营管理类)
        me.workClassText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "文件编码", labelWidth: 60, readOnly: true, emptyText: "工作分类代码", fieldStyle: ' background-color: #DFE9F6;border-color: #DFE9F6; background-image: none;',
             margin: '10 10 0 10', anchor: "80%", labelAlign: "right", width: 150//flex: 1
        });

        //添加工作分项代码text(运营管理类)
        me.workSubText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: " - ", labelWidth: 10, readOnly: true, labelSeparator: '', emptyText: "工作分项代码", labelSeparator: '', // 去掉laebl中的冒号
            margin: '10 0 0 0', anchor: "80%", labelAlign: "left", width: 100//flex: 1
        });

        //添加工作部门代码text(运营管理类)
        me.departmentText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: " - ", labelWidth: 10, readOnly: true, labelSeparator: '', emptyText: "部门代码", labelSeparator: '', // 去掉laebl中的冒号
            margin: '10 0 0 10', anchor: "80%", labelAlign: "left", width: 80//flex: 1
        });

        //添加收文类型text
        me.receiveTypeText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "-", labelWidth: 10, readOnly: true, emptyText: "文件类型", labelSeparator: '',// 去掉laebl中的冒号
            margin: '10 0 0 10', anchor: "80%", labelAlign: "left", width: 80, value: ""//flex: 1
        });

        //添加函件流水号text
        me.fNumberText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: " - ", labelWidth: 10, labelSeparator: '', // 去掉laebl中的冒号
            margin: '10 0 0 10', anchor: "80%", labelAlign: "left", width: 90//flex: 1
        });

        //版本号
        me.editionText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: " - ", labelWidth: 10, labelSeparator: '', emptyText: "版本", // 去掉laebl中的冒号
            margin: '10 0 0 10', anchor: "80%", labelAlign: "left", width: 50//flex: 1
        });


        //添加档号text
        me.archivalCodeText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "档号", labelWidth: 80, emptyText: "", //
            margin: '10 0 0 10', anchor: "80%", labelAlign: "right", flex: 1//width: 130
        });
        
        //卷内序号
        me.coelNumberText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "卷内序号", labelWidth: 80, emptyText: "", //
            margin: '10 0 0 10', anchor: "80%", labelAlign: "right", flex: 1//width: 130
        });

        //原文件编码
        me.originalCodeText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "原文件编码", labelWidth: 80, emptyText: "", //
            margin: '10 0 0 10', anchor: "80%", labelAlign: "right", flex: 1//width: 130
        });

        //添加责任者text
        me.responPersonText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "责任者", labelWidth: 80, emptyText: "", //
            margin: '10 0 0 10', anchor: "80%", labelAlign: "right", flex: 1//width: 130
        });
        
        //文件题名
        me.fileDescText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "文件题名", labelWidth: 80, emptyText: "", fieldStyle: 'border-color: red; background-image: none;',
            margin: '10 0 0 10', anchor: "80%", labelAlign: "right", flex: 1//width: 130
        });

        //页数
        me.pageText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "页数", labelWidth: 80, emptyText: "", fieldStyle: 'border-color: red; background-image: none;',
            margin: '10 0 0 10', anchor: "80%", labelAlign: "right", flex: 1//width: 130
        });

        //份数
        me.copiesText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "份数", labelWidth: 80, emptyText: "", fieldStyle: 'border-color: red; background-image: none;',
            margin: '10 0 0 10', anchor: "80%", labelAlign: "right", flex: 1//width: 130
        });

        //添加介质text
        me.agentText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "介质", labelWidth: 80, emptyText: "", fieldStyle: 'border-color: red; background-image: none;',
            margin: '10 0 0 10', anchor: "80%", labelAlign: "right", flex: 1//width: 130
        });

        //语种
        me.languageText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "语种", labelWidth: 80, emptyText: "", //
            margin: '10 0 0 10', anchor: "80%", labelAlign: "right", flex: 1//width: 130
        });

        //项目代码
        me.projectCodeText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "项目代码", labelWidth: 80, emptyText: "", //
            margin: '10 0 0 10', anchor: "80%", labelAlign: "right", flex: 1//width: 130
        });

        //项目名称
        me.projectDescText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "项目名称", labelWidth: 80, emptyText: "", //
            margin: '10 0 0 10', anchor: "80%", labelAlign: "right", flex: 2//width: 130
        });

        //关联文件编码	关联文件题名	"案卷规格"
        //关联文件编码
        me.assocFileCodeText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "关联文件编码", labelWidth: 80, emptyText: "", //
            margin: '10 0 0 10', anchor: "80%", labelAlign: "right", flex: 1//width: 130
        });

        //关联文件题名
        me.assocFileDescText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "关联文件题名", labelWidth: 80, emptyText: "", //
            margin: '10 0 0 10', anchor: "80%", labelAlign: "right", flex: 1//width: 130
        });

        //案卷规格
        me.fileSpecText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "案卷规格", labelWidth: 80, emptyText: "", //
            margin: '10 0 0 10', anchor: "80%", labelAlign: "right", flex: 1//width: 130
        });

        //归档单位
        me.filingUnitText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "归档单位", labelWidth: 80, emptyText: "", //
            margin: '10 0 0 10', anchor: "80%", labelAlign: "right", flex: 1//width: 130
        });


        //保管期限
        me.secrTermText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "保管期限", labelWidth: 80, emptyText: "", //
            margin: '10 0 0 10', anchor: "80%", labelAlign: "right", flex: 1//width: 130
        });

        //归档文件清单编码
        me.arcFileListCodeText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "归档文件清单编码", labelWidth: 110, emptyText: "", //
            margin: '10 0 0 10', anchor: "80%", labelAlign: "right", flex: 1//width: 130
        });

        //归档日期Archive date
        me.archiveDateText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "归档日期", labelWidth: 80, emptyText: "", //
            margin: '10 0 0 10', anchor: "80%", labelAlign: "right", flex: 1//width: 130
        });


        //排架号
        me.shelfNumberText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "排架号", labelWidth: 80, emptyText: "", //
            margin: '10 0 0 10', anchor: "80%", labelAlign: "right", flex: 1//width: 130
        });

        //备注
        me.remarkText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "备注", labelWidth: 80, emptyText: "", //
            margin: '10 0 0 10', anchor: "80%", labelAlign: "right", flex: 1//width: 130
        });

        me.newFileCodeCheckBox = Ext.create("Ext.form.field.Checkbox", {
            fieldLabel: "", labelWidth: 0, margin: '10 10 0 50',
            boxLabel: "新建文件编码",
            listeners: {
                change: function (view, newValue, oldValue, eOpts) {
                    if (newValue === true) {
                        me.fNumberText.setVisible(true);
                    } else {
                        me.fNumberText.setVisible(false);
                    }
                }
            }
        });

        //添加文件编码分类combo
        Ext.define("fileCodeTypeModel", {
            extend: 'Ext.data.Model',
            fields: ["text", "value"]
        });
        me.fileCodeTypeProxy = Ext.create("Ext.data.proxy.Memory", {
            data: me.fileCodeTypedata,
            model: "fileCodeTypeModel"
        });

        me.fileCodeTypeStore = Ext.create("Ext.data.Store", {
            model: fileCodeTypeModel,
            proxy: me.fileCodeTypeProxy
        });


        me.fileCodeTypeCombo = Ext.create("Ext.form.field.ComboBox",
        {
            //xtype: "combo",
            fieldLabel: '文件编码类型', labelWidth: 100,
            triggerAction: "all", store: me.fileCodeTypeStore,
            valueField: 'value', editable: false,//不可输入
            displayField: 'text', margin: '10 5 0 10',// 
            anchor: "80%", labelAlign: "right", labelPad: 8, width: 220,// flex: 1,// width: '28%',//
            emptyText: "--请选择--", autoLoadOnValue: true, value: "运营管理类",
            //fieldStyle: 'border-color: red; background-image: none;',//红色边框
            listeners:
            {
                select: function (combo, records, eOpts) {
                    me.setIsProjectFile();
                //    var fileCodeType = me.fileCodeTypeCombo.value;
                //    if (fileCodeType === "运营管理类") {
                //        me.setIsProjectFile(false);
                //   } else {
                //        me.setIsProjectFile(true);
                //    }
                }
            }
        });

        //添加保密等级combo
        Ext.define("seculevelModel", {
            extend: 'Ext.data.Model',
            fields: ["text", "value"]
        });
        me.seculevelProxy = Ext.create("Ext.data.proxy.Memory", {
            data: me.seculeveldata,
            model: "seculevelModel"
        });

        me.seculevelStore = Ext.create("Ext.data.Store", {
            model: seculevelModel,
            proxy: me.seculevelProxy
        });


        me.seculevelCombo = Ext.create("Ext.form.field.ComboBox",
        {
            //xtype: "combo",
            fieldLabel: '密级', labelWidth: 80,
            triggerAction: "all", store: me.seculevelStore,
            valueField: 'value', editable: false,//不可输入
            displayField: 'text', margin: '10 5 0 10',// 
            anchor: "80%", labelAlign: "right", labelPad: 8, flex:1,// width: '28%',//width: 120,//
            emptyText: "--请选择--", autoLoadOnValue: true, value: "受限",
            //fieldStyle: 'border-color: red; background-image: none;',//红色边框
            listeners:
            {
                select: function (combo, records, eOpts) {
                    //var seculevel = me.seculevelCombo.value;
                    //if (seculevel === "公开") {
                    //    me.secrTermText.setDisabled(true);
                    //} else {
                    //    me.secrTermText.setDisabled(false);
                    //}
                }
            }
        });

        me.projectCodeButton = Ext.create("Ext.button.Button", {
            text: "..", margins: "10 0 0 0",
            listeners: {
                "click": function (btn, e, eOpts) {//添加点击按钮事件
                    me.selectProject();
                }
            }
        });

        me.fProjectCodeButton = Ext.create("Ext.button.Button", {
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
                    me.selectCrew();
                    
                }
            }
        });

        //选择机组按钮
        me.crew2Button = Ext.create("Ext.button.Button", {
            text: "..", margins: "10 0 0 0",
            listeners: {
                "click": function (btn, e, eOpts) {//添加点击按钮事件
                    me.selectCrew();

                }
            }
        });

        //选择厂房按钮
        me.factoryButton = Ext.create("Ext.button.Button", {
            text: "..", margins: "10 0 0 0",
            listeners: {
                "click": function (btn, e, eOpts) {//添加点击按钮事件
                    me.selectFactory();
                   
                }
            }
        });

        //选择厂房按钮
        me.factory2Button = Ext.create("Ext.button.Button", {
            text: "..", margins: "10 0 0 0",
            listeners: {
                "click": function (btn, e, eOpts) {//添加点击按钮事件
                    me.selectFactory();

                }
            }
        });

        //选择系统按钮
        me.systemButton = Ext.create("Ext.button.Button", {
            text: "..", margins: "10 0 0 0",
            listeners: {
                "click": function (btn, e, eOpts) {//添加点击按钮事件
                    me.selectSystem();
                  
                }
            }
        });

        //选择系统按钮
        me.system2Button = Ext.create("Ext.button.Button", {
            text: "..", margins: "10 0 0 0",
            listeners: {
                "click": function (btn, e, eOpts) {//添加点击按钮事件
                    me.selectSystem();

                }
            }
        });


        me.professionButton = Ext.create("Ext.button.Button", {
            text: "..", margins: "10 0 0 0",
            listeners: {
                "click": function (btn, e, eOpts) {//
                    me.selectProfession();
                   
                }
            }
        });

        me.profession2Button = Ext.create("Ext.button.Button", {
            text: "..", margins: "10 0 0 0",
            listeners: {
                "click": function (btn, e, eOpts) {//
                    me.selectProfession();

                }
            }
        });

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

                            me.getRunNum();
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
                    me.selectRecDepartment();

                }
            }
        });

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

                            me.getRunNum();
                        }
                    });
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

        //编辑区域头部
        me.editTopPanel = Ext.create("Ext.panel.Panel", {
            baseCls: 'my-panel-no-border',//隐藏边框
            layout: {
                type: 'vbox',
                pack: 'start',
                align: 'stretch'
            },
            margin: '0 10 0 0',// 
            items: [
                {

                    baseCls: 'my-panel-no-border',//隐藏边框
                    layout: {
                        type: 'hbox',
                        pack: 'start',
                        align: 'stretch'
                    },
                    items: [
                        me.newFileCodeCheckBox,
                        me.fileCodeTypeCombo
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
                         //项目管理类
                         me.fProjectCodeText, me.fProjectCodeButton,
                         me.crewText, me.crewButton,
                         me.factoryText, me.factoryButton,
                         me.systemText, me.systemButton,
                         me.professionText, me.professionButton,

                         //运营管理类
                         me.workClassText,
                         me.workSubText, me.workSubButton,
                         me.departmentText, me.departmentButton,

                         me.receiveTypeText, me.receiveTypeButton,
                         me.fNumberText, me.editionText
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
                    me.archivalCodeText,
                    me.coelNumberText,
                    me.originalCodeText
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
                    me.responPersonText,
                    me.fileDescText,
                    me.pageText
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
                    me.copiesText,
                    me.agentText,
                    me.languageText
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
                        {
                            baseCls: 'my-panel-no-border',//隐藏边框
                            layout: {
                                type: 'hbox',
                                pack: 'start',
                                align: 'stretch'
                            },
                            items: [
                              me.projectCodeText, me.projectCodeButton,
                            ], flex: 1
                        },
                    
                    me.projectDescText
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
                        {
                            baseCls: 'my-panel-no-border',//隐藏边框
                            layout: {
                                type: 'hbox',
                                pack: 'start',
                                align: 'stretch'
                            },
                            items: [
                               me.crew2Text, me.crew2Button,
                            ], flex: 1
                        },
                        {
                            baseCls: 'my-panel-no-border',//隐藏边框
                            layout: {
                                type: 'hbox',
                                pack: 'start',
                                align: 'stretch'
                            },
                            items: [
                                me.factory2Text, me.factory2Button,
                            ], flex: 1
                        },
                    me.factoryDescText
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
                        {
                            baseCls: 'my-panel-no-border',//隐藏边框
                            layout: {
                                type: 'hbox',
                                pack: 'start',
                                align: 'stretch'
                            },
                            items: [
                                me.profession2Text, me.profession2Button,
                            ], flex: 1
                        },
                        {
                            baseCls: 'my-panel-no-border',//隐藏边框
                            layout: {
                                type: 'hbox',
                                pack: 'start',
                                align: 'stretch'
                            },
                            items: [
                                me.system2Text, me.system2Button,
                            ], flex: 1
                        },

                    me.systemDescText
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
                me.assocFileCodeText,
                   me.assocFileDescText,
                   me.fileSpecText
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
               me.filingUnitText,
               me.seculevelCombo,
                   me.secrTermText
                   
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
               me.arcFileListCodeText,
               me.archiveDateText,
                   me.shelfNumberText

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
               me.remarkText

                ]
            }]
        });


        //底部按钮区域
        me.bottomButtonPanel = Ext.create("Ext.panel.Panel", {
            //xtype: "panel",
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
                            me.save_file_properties();
                        }
                    }
                },
                {
                    xtype: "button",
                    text: "取消", width: 60, margins: "10 15 10 5",
                    listeners: {
                        "click": function (btn, e, eOpts) {//添加点击按钮事件

                            winEditFileProperties.close();
                        }
                    }
                }
            ]
        });

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
                  margin: '10 0 0 0',// 
                  items: [
                     me.editTopPanel,
                     me.bottomButtonPanel
                  ]
              }]
          }
        )];

        me.setIsProjectFile(false);

        me.callParent(arguments);
    },

    setFilePropertiesDefault: function (data) {
        var me = this;

        me.fa = data;

        //是否需要新建文件编码
        if (data.needNewFileCode === true) {
            me.newFileCodeCheckBox.setValue(data.needNewFileCode);
            me.fNumberText.setVisible(true);
        } else {
            me.fNumberText.setVisible(false);
        }

        //文件编码
        me.originalCodeText.setValue(data.origcode);
        //文件题名
        me.fileDescText.setValue(data.desc);
        //档号
        me.archivalCodeText.setValue(data.reference);
        //卷内序号
        me.coelNumberText.setValue(data.volumenumber);
        //责任人
        me.responPersonText.setValue(data.responsibility);
        //页数
        me.pageText.setValue(data.page);
        //份数
        me.copiesText.setValue(data.share);
        //介质
        me.agentText.setValue(data.medium);
        //语种
        me.languageText.setValue(data.languages);
        //项目名称
        me.projectDescText.setValue(data.proname);
        //项目代码
        me.projectCodeText.setValue(data.procode);
        me.fProjectCodeText.setValue(data.procode);

        //专业
        me.professionText.setValue(data.major);
        me.profession2Text.setValue(data.major);
        //机组
        me.crewText.setValue(data.crew);
        me.crew2Text.setValue(data.crew);
        //厂房代码
        me.factoryText.setValue(data.factorycode);
        me.factory2Text.setValue(data.factorycode);
        //厂房名称
        me.factoryDescText.setValue(data.factoryname);
        //系统代码
        me.systemText.setValue(data.systemcode);
        me.system2Text.setValue(data.systemcode);
        //系统名称
        me.systemDescText.setValue(data.systemname);

        //关联文件编码
        me.assocFileCodeText.setValue(data.relationfilecode);
        //关联文件题名
        me.assocFileDescText.setValue(data.relationfilename);
        //案卷规格
        me.fileSpecText.setValue(data.filespec);
        //归档单位
        me.filingUnitText.setValue(data.fileunit);
        //密级
        var secretgrade = data.secretgrade;
        if (!(secretgrade === undefined || secretgrade === "")) {
            me.seculevelCombo.setRawValue(data.secretgrade);//设置显示值
            me.seculevelCombo.setValue(data.secretgrade); //设置ID值
        }
        //保管期限
        me.secrTermText.setValue(data.keepingtime);
        //归档文件清单编码
        me.arcFileListCodeText.setValue(data.filelistcode);
        //归档日期
        me.archiveDateText.setValue(data.filelisttime);
        //排架号
        me.shelfNumberText.setValue(data.racknumber);
        //备注
        me.remarkText.setValue(data.note);

        //文件类型
        me.receiveTypeText.setValue(data.receiveType);
        //流水号
        me.fNumberText.setValue(data.fNumber);
        //版本
        me.editionText.setValue(data.edition);

        //工作分类代码
        me.workClassText.setValue(data.workClass);
        //工作分项代码	
        me.workSubText.setValue(data.workSub);
        //部门代码
        me.departmentText.setValue(data.department);

        //文件编码类型(项目类或运营类)
        var fileCodeType = data.fileCodeType;
        if (!(fileCodeType === undefined || fileCodeType === "")) {
            me.fileCodeTypeCombo.setRawValue(data.fileCodeType);//设置显示值
            me.fileCodeTypeCombo.setValue(data.fileCodeType); //设置ID值

            me.setIsProjectFile();
        }

        ////是否新建文件编码
        //var isNewCode = data.isNewCode;
        //if (!(isNewCode === undefined || isNewCode === "")) {
        //    me.newFileCodeCheckBox.setValue(isNewCode);

        //}

    },

    selectCrew: function ()
    {
        var me = this;
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

                me.crew2Text.setValue(crewCode);

                me.getRunNum();
            }
        });
    },

    selectFactory: function () {
        var me = this;
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
                factoryValue = window.parent.factoryvaluelist;

                if (factoryCode.indexOf(",") > 0) {
                 var words = factoryCode.split(',')
                factoryCode = factoryCode.substring(0, factoryCode.indexOf(","));
                factoryDesc = factoryDesc.substring(0, factoryDesc.indexOf(";"));
                }

                me.factoryText.setValue(factoryCode);
                me.factory2Text.setValue(factoryCode);

                me.factoryDescText.setValue(factoryDesc);

                //重置系统文本框
                me.systemText.setValue("");
                me.system2Text.setValue("");

                me.systemDescText.setValue("");

                me.getRunNum();
            }
        });

    },

    selectSystem: function () {
        var me = this;

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
                SystemValue = window.parent.systemvaluelist;

                if (SystemCode.indexOf(",") > 0) {
                    var words = SystemCode.split(',')
                    SystemCode = SystemCode.substring(0, SystemCode.indexOf(","));
                    SystemDesc = SystemDesc.substring(0, SystemDesc.indexOf(";"));
                }

                me.systemText.setValue(SystemCode);
                me.system2Text.setValue(SystemCode);

                me.systemDescText.setValue(SystemDesc);

                //重置厂房文本框
                me.factoryText.setValue("");
                me.factory2Text.setValue("");

                me.factoryDescText.setValue("");

                me.getRunNum();
            }
        });
    },

    selectProfession: function () {
        var me = this;

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
                me.profession2Text.setValue(professionCode);

                me.getRunNum();
            }
        });
    },

    selectRecDepartment: function () {
        var me = this;

        if (me.docClass === "project" && me.projectKeyword === "") {
            Ext.Msg.alert("错误信息", "请选择项目！");
            return;
        }

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

               // me.mainFeederText.setValue(departmentDesc);

               // me.recCompanyText.setValue(departmentCode);

                me.departmentText.setValue(departmentCode);
                //me.workClassText.setValue(departmentType);
                me.getRunNum();
            }
        });
    },

    setIsProjectFile: function (){//isProjectFile) {
        var me = this;

        var isProjectFile = false;

        var fileCodeType = me.fileCodeTypeCombo.value;
        if (fileCodeType === "运营管理类") {
           // me.setIsProjectFile(false);
            isProjectFile = false;
        } else {
           // me.setIsProjectFile(true);
            isProjectFile = true;
        }

        if (!isProjectFile) {
            //运营信函
            me.docClass = "operation";

            me.fProjectCodeText.setVisible(false);
            me.fProjectCodeButton.setVisible(false);

            me.crewText.setVisible(false);
            me.crewButton.setVisible(false);
            me.systemText.setVisible(false);
            me.systemButton.setVisible(false);
            me.factoryText.setVisible(false);
            me.factoryButton.setVisible(false);

            me.professionText.setVisible(false);
            me.professionButton.setVisible(false);

     
            ///////////////////////////////////////////////

            me.workClassText.setVisible(true);
            me.workSubText.setVisible(true);
            me.departmentText.setVisible(true);
            me.workSubButton.setVisible(true);
            me.departmentButton.setVisible(true);

            ///////////////////////////////////////////////
            me.fProjectCodeText.setFieldStyle('');
            me.crewText.setFieldStyle('background-image: none;');
            me.factoryText.setFieldStyle('background-image: none;');
            me.systemText.setFieldStyle('background-image: none;');
            me.professionText.setFieldStyle('background-image: none;');

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
            me.fProjectCodeButton.setVisible(true);

            me.crewText.setVisible(true);
            me.crewButton.setVisible(true);
            me.systemText.setVisible(true);
            me.systemButton.setVisible(true);
            me.factoryText.setVisible(true);
            me.factoryButton.setVisible(true);

            me.professionText.setVisible(true);
            me.professionButton.setVisible(true);

            ///////////////////////////////////////////////
            me.fProjectCodeText.setFieldStyle('border-color: red; background-image: none;');
            me.crewText.setFieldStyle('border-color: red; background-image: none;');
            me.factoryText.setFieldStyle('border-color: red; background-image: none;');
            me.systemText.setFieldStyle('border-color: red; background-image: none;');
            me.professionText.setFieldStyle('border-color: red; background-image: none;');
        }
    },

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



            me.projectCodeText.setValue(projectCode);

            me.fProjectCodeText.setValue(projectCode);

            me.projectDescText.setValue(projectDesc);

            me.projectDirKeyword = projectValue;

        });
    },

    save_file_properties: function () {
        var me = this;

        if (me.needInputPage === true && (me.pageText.value === undefined || me.pageText.value === "")) {
            Ext.Msg.alert("错误信息", "请填写页数！");
            return;
        }
        if (me.copiesText.value === undefined || me.copiesText.value === "") {
            Ext.Msg.alert("错误信息", "请填写份数！");
            return;
        }
        if (me.fileDescText.value === undefined || me.fileDescText.value === "") {
            Ext.Msg.alert("错误信息", "请填写文件题名！");
            return;
        }
        if (me.agentText.value === undefined || me.agentText.value === "") {
            Ext.Msg.alert("错误信息", "请填写介质！");
            return;
        }

        var fileCodeType = me.fileCodeTypeCombo.value;
        if (fileCodeType === "项目管理类")
        {
            if (me.fProjectCodeText.value === undefined || me.fProjectCodeText.value === "") {
                Ext.Msg.alert("错误信息", "请填写项目代码！");
                return;
            }
            if (me.crewText.value === undefined || me.crewText.value === "") {
                Ext.Msg.alert("错误信息", "请填写机组！");
                return;
            }
            if ((me.factoryText.value === undefined || me.factoryText.value === "") &&
                (me.systemText.value === undefined || me.systemText.value === "")) {
                Ext.Msg.alert("错误信息", "请填写厂房或系统！");
                return;
            }
            if (me.professionText.value === undefined || me.professionText.value === "") {
                Ext.Msg.alert("错误信息", "请填写专业！");
                return;
            }
        }

        //是否新建文件编码
        var needNewFileCode = me.newFileCodeCheckBox.getValue();

        //文件编码类型
        var fileCodeType = me.fileCodeTypeCombo.value;

        //文件编码
        var fileCode = me.originalCodeText.value === undefined ? "" : me.originalCodeText.value;

      

        //文件题名
        var fileDesc = me.fileDescText.value === undefined ? "" : me.fileDescText.value;

        //档号
        var archivalCode = me.archivalCodeText.value === undefined ? "" : me.archivalCodeText.value;

        //卷内序号
        var   coelNumber=me.coelNumberText.value === undefined ? "" :me.coelNumberText.value ;

        //责任人
        var responPerson= me.responPersonText.value === undefined ? "" : me.responPersonText.value;

        //页数
        var page = me.pageText.value === undefined ? "" : me.pageText.value;

        //份数
        var copies = me.copiesText.value === undefined ? "" :me.copiesText.value;

        //介质
        var agent= me.agentText.value === undefined ? "" :me.agentText.value ;

        //语种
        var language=  me.languageText.value === undefined ? "" :me.languageText.value;

        //项目名称
        var projectDesc = me.projectDescText.value === undefined ? "" :me.projectDescText.value;

        //项目代码
        var projectCode=me.projectCodeText.value === undefined ? "" :me.projectCodeText.value;
        var fProjectCode = projectCode;

        //专业
        var profession = me.profession2Text.value === undefined ? "" : me.profession2Text.value ;

        //机组
        var crew= me.crew2Text.value === undefined ? "" :me.crew2Text.value;

        //厂房代码
        var factoryCode = me.factory2Text.value === undefined ? "" : me.factory2Text.value;
        var factory = factoryCode;
        //厂房名称
        var factoryDesc=  me.factoryDescText.value === undefined ? "" : me.factoryDescText.value ;
        //系统代码
        var system = me.system2Text.value === undefined ? "" :me.system2Text.value ;
        //系统名称
        var systemDesc = me.systemDescText.value === undefined ? "" :me.systemDescText.value ;

        //工作分类代码（运营管理类）
        var workClass = me.workClassText.value === undefined ? "" : me.workClassText.value;
        //工作分项代码（运营管理类）
        var workSub = me.workSubText.value === undefined ? "" : me.workSubText.value;
        //部门代码（运营管理类）
        var department = me.departmentText.value === undefined ? "" : me.departmentText.value;

        //文件类型
        var receiveType = me.receiveTypeText.value === undefined ? "" : me.receiveTypeText.value;
        //流水号
        var fNumber = me.fNumberText.value === undefined ? "" : me.fNumberText.value;
        //版本
        var edition = me.editionText.value === undefined ? "" : me.editionText.value;

        //关联文件编码
        var assocFileCode= me.assocFileCodeText.value === undefined ? "" :me.assocFileCodeText.value;

        //关联文件题名
        var assocFileDesc=me.assocFileDescText.value === undefined ? "" :me.assocFileDescText.value;

        //案卷规格
        var fileSpec = me.fileSpecText.value === undefined ? "" : me.fileSpecText.value ;

        //归档单位
        var filingUnit=me.filingUnitText.value === undefined ? "" :me.filingUnitText.value;

        //密级
        var seculevel = me.seculevelCombo.value === undefined ? "" :me.seculevelCombo.value;

        //保管期限
        var secrTerm = me.secrTermText.value === undefined ? "" :me.secrTermText.value ;

        //归档文件清单编码
        var arcFileListCode = me.arcFileListCodeText.value === undefined ? "" : me.arcFileListCodeText.value ;
        //归档日期
        var archiveDate=me.archiveDateText.value === undefined ? "" :me.archiveDateText.value;
        //排架号
        var shelfNumber= me.shelfNumberText.value === undefined ? "" :me.shelfNumberText.value;
        //备注
        var remark = me.remarkText.value === undefined ? "" :me.remarkText.value;

        //获取新建的文件编码
        if (needNewFileCode === true) {
            var fileCodeType = me.fileCodeTypeCombo.value;
            var checkCode = false;
            if (fileCodeType === "运营管理类") {
                if (workClass === "") checkCode=true;
                if (workSub === "") checkCode = true;
                if (department === "") checkCode = true;

                fileCode = workClass + "-" + workSub + "-" + department + "-" + receiveType + "-" + fNumber+ "-" +edition;
            } else {
                if (fProjectCode === "") checkCode = true;
                if (crew === "") checkCode = true;
                if (system === "" && factory === "") checkCode = true;
                if (profession === "") checkCode = true;

                fileCode = fProjectCode + "-" + crew + factory + system + "-" + profession + "-" + receiveType + "-" + fNumber + "-" + edition;
            }
            if (receiveType === "") checkCode = true;
            if (fNumber === "") checkCode = true;
            if (edition === "") checkCode = true;

            if (checkCode === true) {
                Ext.Msg.alert("错误信息", "请正确填写文件编码！");
                return;
            }
        }

        //var fileAttr =
        //[
        //    {
        //        //name:, 
        //        needNewFileCode: needNewFileCode, fileCodeType: fileCodeType,
        //        code: fileCode, origcode: fileCode, desc: fileDesc,
        //        reference: archivalCode, volumenumber: coelNumber,
        //        responsibility: responPerson, page: page,
        //        share: copies, medium: agent, languages: language,
        //        proname: projectDesc, procode: projectCode,

        //        major: profession, crew: crew,
        //        factorycode: factoryCode, factoryname: factoryDesc,
        //        systemcode: system, systemname: systemDesc,

        //        workClass: workClass, workSub: workSub,
        //        department: department,

        //        receiveType: receiveType, fNumber: fNumber,
        //        edition:edition,

        //        relationfilecode: assocFileCode, relationfilename: assocFileDesc,
        //        filespec: fileSpec, fileunit: filingUnit,
        //        secretgrade: seculevel, keepingtime: secrTerm,
        //        filelistcode: arcFileListCode, filelisttime: archiveDate ,
        //        racknumber: shelfNumber, note: remark,


        //    }
        //]

        //window.parent.resultarray = fileAttr;

        me.fa.needNewFileCode = needNewFileCode; me.fa.fileCodeType = fileCodeType;
        me.fa.code = fileCode; me.fa.origcode = fileCode; me.fa.desc = fileDesc;
        me.fa.reference = archivalCode; me.fa.volumenumber = coelNumber;
        me.fa.responsibility = responPerson; me.fa.page = page;
        me.fa.share = copies; me.fa.medium = agent; me.fa.languages = language;
        me.fa.proname = projectDesc; me.fa.procode = projectCode;

        me.fa.major = profession; me.fa.crew = crew;
        me.fa.factorycode = factoryCode; me.fa.factoryname = factoryDesc;
        me.fa.systemcode = system; me.fa.systemname = systemDesc;

        me.fa.workClass = workClass; me.fa.workSub = workSub;
        me.fa.department = department;

        me.fa.receiveType = receiveType; me.fa.fNumber = fNumber;
        me.fa.edition = edition;

        me.fa.relationfilecode = assocFileCode; me.fa.relationfilename = assocFileDesc;
        me.fa.filespec = fileSpec; me.fa.fileunit = filingUnit;
        me.fa.secretgrade = seculevel; me.fa.keepingtime = secrTerm;
        me.fa.filelistcode = arcFileListCode; me.fa.filelisttime = archiveDate;
        me.fa.racknumber = shelfNumber; me.fa.note = remark;

        var fileAttr = [me.fa];

        window.parent.resultarray = fileAttr;

        winEditFileProperties.close();
    },

    //获取流水号
    getRunNum: function () {
        var me = this;

        var isProjectFile = false;

        var fileCodeType = me.fileCodeTypeCombo.value;
        if (fileCodeType === "运营管理类") {
            isProjectFile = false;
        } else {
            isProjectFile = true;
        }

        //me.fProjectCodeText.setVisible(false);

        //me.crewText.setVisible(false);
        //me.crewButton.setVisible(false);
        //me.systemText.setVisible(false);
        //me.systemButton.setVisible(false);
        //me.factoryText.setVisible(false);
        //me.factoryButton.setVisible(false);

        //me.professionText.setVisible(false);
        //me.professionButton.setVisible(false);


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
            //url: 'SHEPCPlugins/GetRunNum',
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
                } else {
                    var errmsg = res.msg;
                    Ext.Msg.alert("错误信息", errmsg);
                }
            },
            failure: function (response, options) {
                //                Ext.Msg.alert("系统提示", "连接服务器失败，请尝试重新提交！");
            }
        });
    }
});