Ext.define('Ext.plug_ins.ZHEPC_Plugins.Dist.FileCodePanel', {
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

        //当前起草函件时选中的目录（菜单所在目录）
        me.projectKeyword = "";

        me.sendUnitdata = [];
        //me.sendUnitdata =[{ text: "YHDL", value: "YHDL" }, { text: "GEDI", value: "GEDI" },
            //{ text: "ZNJL", value: "ZNJL" }, { text: "ZJYJ", value: "ZJYJ" },
            //{ text: "GPEC", value: "GPEC" }]//发文单位combo
        
        me.crewdata = [];//机组combo
        //me.crewdata = [{ text: "00", value: "00" }, { text: "01", value: "01" }, { text: "02", value: "02" }];//机组combo

        me.professiondata = [];
        //me.professiondata = [{ text: "ZH", value: "ZH" }, { text: "TJ", value: "TJ" }, { text: "SG", value: "SG" },
        //    { text: "NT", value: "NT" }, { text: "AZ", value: "AZ" }, { text: "GL", value: "GL" },
        //    { text: "QJ", value: "QJ" }, { text: "GD", value: "GD" }, { text: "HS", value: "HS" },
        //    { text: "DQ", value: "DQ" }, { text: "RK", value: "RK" }, { text: "TS", value: "TS" },
        //    { text: "AQ", value: "AQ" }
        //];//专业combo

 
        //添加发文单位代码text
        me.sendUnitText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "发文编号", labelWidth: 60, readOnly: true, emptyText: "发文单位",
            margin: '10 0 0 0', anchor: "80%", labelAlign: "left", width: 160//flex: 1
        });

        //添加表式text
        me.postCategoryText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: " - ", labelWidth: 10, value: "A.3", labelSeparator: '', readOnly: true,
            margin: '10 0 0 10', anchor: "80%", labelAlign: "left", width: 100//flex: 1
        });

        //添加流水单号text
        //me.docNumText = Ext.create("Ext.form.field.Text", {
        me.docNumText = Ext.widget('textfield', {
             fieldLabel: " - ", labelWidth: 10, value: "", labelSeparator: '', emptyText: "流水号",
            margin: '10 0 0 10', anchor: "80%", labelAlign: "left", width: 100,//flex: 1
        //    listeners:
        //{
          
        //    keyup: function () {
        //        Ext.Msg.alert("您展开了目录树节点！！！", "ccc");

        //    }
        //}
        });
      

        //添加发文单位combo
        Ext.define("sendUnitModel", {
            extend: 'Ext.data.Model',
            fields: ["text", "value"]
        });
        me.sendUnitProxy = Ext.create("Ext.data.proxy.Memory", {
            data: me.sendUnitdata,
            model: "sendUnitModel"
        });

        me.sendUnitStore = Ext.create("Ext.data.Store", {
            model: sendUnitModel,
            proxy: me.sendUnitProxy
        });

        me.sendUnitCombo = Ext.create("Ext.form.field.ComboBox",
        //me.sendUnitCombo = Ext.widget("combo",
        {
            xtype: "combo", enableKeyEvents: true,
            triggerAction: "all", store: me.sendUnitStore,
            valueField: 'value', editable: false,//不可输入
            displayField: 'text', fieldLabel: "发文编号",
            labelWidth: 60, labelSeparator: '',
            anchor: "80%", labelAlign: "left",
            margin: '10 0 0 0', width: 160,editable:true,
            emptyText: "发文单位",//, margins: "8"
            listeners:
            {
                select: function (combo, records, eOpts) {
                    //获取流水号
                    me.getRunNum();
                },
                keyup: function () {
                    Ext.Msg.alert("您展开了目录树节点！！！", "ccc");

                }
            }
        });

   
        //添加机组combo
        Ext.define("crewModel", {
            extend: 'Ext.data.Model',
            fields: ["text", "value"]
        });
        me.crewProxy = Ext.create("Ext.data.proxy.Memory", {
            data: me.crewdata,
            model: "crewModel"
        });

        me.crewStore = Ext.create("Ext.data.Store", {
            model: crewModel,
            proxy: me.crewProxy
        });

      
        me.crewCloumns=[
        {text: '机组代码',  dataIndex:'text'},
        { text: '机组名称', dataIndex: 'value' }
        ];
        me.crewPicker = Ext.create('Ext.plug_ins.ZHEPC_Plugins.Dist.GridPickBox',{
        //me.crewPicker = Ext.create('Ext.plug_ins.ZHEPC_Plugins.Dist.GridPicker', { 
            store: me.crewStore, columns: me.crewCloumns,
            displayField: 'text', valueField: 'text',
            emptyText: "机组", fieldLabel: " - ",
            labelWidth: 10, labelSeparator: '',
            margin: '10 0 0 10', width: 96,
            pickerWidth: 200, showPagingbar: false,
            editable: true, enableKeyEvents: true
        });

        me.crewPicker.on('keyup', function (view, e, eOpts) {
            //Ext.Msg.alert("您修改了值!", view.rawValue);
            if (me.crewPicker.picker == undefined) {
                me.crewPicker.picker = me.crewPicker.createPicker();
                me.crewPicker.expand();
            }

            if (me.crewPicker.picker != undefined) {
                me.crewPicker.picker.show();
                me.crewPicker.focus(false, 100);

                me.seleCrewList();
             
            }
            

        });
        //me.crewPicker.createPicker();

        me.crewPicker.on('beforedestroy', function (view, eOpts) {
            var me = this;
            if (this.picker != undefined) {
                this.picker.hide();
                //this.picker.fireEvent("destroy",me);
            }
        });

        //添加专业combo
        Ext.define("professionModel", {
            extend: 'Ext.data.Model',
            fields: ["text", "value"]
        });
        me.professionProxy = Ext.create("Ext.data.proxy.Memory", {
            data: me.professiondata,
            model: "professionModel"
        });

        me.professionStore = Ext.create("Ext.data.Store", {
            model: professionModel,
            proxy: me.professionProxy
        });

        me.professionCombo = Ext.create("Ext.form.field.ComboBox",
        {
            xtype: "combo",
            triggerAction: "all", store: me.professionStore,
            valueField: 'value', editable: false,//不可输入
            displayField: 'text', fieldLabel: " - ",
            labelWidth: 10, labelSeparator: '',
            anchor: "80%", labelAlign: "left",
            margin: '10 0 0 10', width: 96,
            emptyText: "专业",//, margins: "8"
            listeners:
            {
                select: function (combo, records, eOpts) {
                    //获取流水号
                    me.getRunNum();
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
                    //me.sendUnitText,//发文单位
                    
                    me.sendUnitCombo,//发文单位
                     //me.crewCombo,//机组
                    me.crewPicker,
                             me.professionCombo,//专业
                             me.postCategoryText,//表式
                           me.docNumText,//流水号

                           me.groupinputtext
                ],
                flex: 1
            },
                    
        ];

        

        me.callParent(arguments);
    },

    listeners: {
        "beforedestroy": function (view, eOpts) {
        }
    },
    //获取文件编码控件默认信息（combo的默认值）
    getFileCodeDefaultInfo: function () {
        var me = this;

        Ext.Ajax.request({
            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.ZHEPC_Plugins.Document", A: "GetFileCodeDefaultInfo",
                sid: localStorage.getItem("sid"), ProjectKeyword: me.projectKeyword

            },
            success: function (response, options) {
                //获取数据后，更新窗口
                var res = Ext.JSON.decode(response.responseText, true);
                var state = res.success;
                if (state === true) {
                    var recod = eval(res.data[0]);

                    me.crewList = eval(recod.CrewList);
                    me.unitList = eval(recod.UnitList);
                    me.professionList = eval(recod.ProfessionList);

                    //遍历发文单位数组，添加到发文单位combo
                    for (var itemKey in me.unitList) {
                        var unitItem = me.unitList[itemKey];
                        var unitItemCode = unitItem.unitCode;
                        var unitItemDesc = unitItem.unitDesc;
                        me.sendUnitdata.push({ text: unitItemCode, value: unitItemCode });//在数组里添加新的元素  
                    }

                    //var crewgrid = me.crewPicker.down("gridpanel");

                    //遍历机组数组，添加到机组combo
                    for (var itemKey in me.crewList) {
                        var crewItem = me.crewList[itemKey];
                        var crewItemCode = crewItem.crewCode;
                        var crewItemDesc = crewItem.crewDesc;
                        me.crewdata.push({ text: crewItemCode, value: crewItemCode });//在数组里添加新的元素

                        //插入行到返回grid
                        var r = Ext.create('crewModel', {
                            text: crewItemCode, value: crewItemDesc
                        });

                        
                        
                        var rowlength = me.crewPicker.store.data.length;
                        me.crewPicker.store.insert(rowlength, r);

                        //var p = new Record({
                        //    text: crewItem.crewCode,
                        //    value: crewItem.crewDesc,
                        //    taskStageId: ''
                        //});
                        //me.crewStore.insert(me.crewStore.getCount(), p);
                    }
                    //crewgrid.getView().refresh(); //刷新
                    //me.crewStore.loadData(me.crewdata, true);
                    //me.crewProxy = Ext.create("Ext.data.proxy.Memory", {
                    //    data: me.crewdata,
                    //    model: "crewModel"
                    //});

                    //me.crewStore = Ext.create("Ext.data.Store", {
                    //    model: crewModel,
                    //    proxy: me.crewProxy
                    //});

                    //遍历专业数组，添加到专业combo
                    for (var itemKey in me.professionList) {
                        var professionItem = me.professionList[itemKey];
                        var professionItemCode = professionItem.professionCode;
                        var professionItemDesc = professionItem.professionDesc;
                        me.professiondata.push({ text: professionItemCode, value: professionItemCode });//在数组里添加新的元素  
                    }

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

    seleCrewList: function () {
        var me = this;

        Ext.Ajax.request({
            url: 'WebApi/Get',
            method: "Get",
            params: {
                C: "AVEVA.CDMS.ZHEPC_Plugins.Document", A: "GetSelectCrewList",
                ProjectKeyword: me.projectKeyword, sid: localStorage.getItem("sid")
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
            //me.sendUnitText.setValue(sourceUnit);
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

    setSendUnit: function (code, desc) {
        var me = this;
        me.sendUnitText.setValue(code);
        me.departmentText.setValue(code);
        me.cataAttrArray[0].department = code;
        me.cataAttrArray[0].sendUnitCode = code;
        me.AfterSelectSendUnit(code, desc);
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
            sendCode = me.projectCodeText.value + "-" + me.sendUnitText.value + "-" + me.recUnitText.value + "-" + me.cataAttrArray[0].receiveType + "-" + me.numberText.value;

        } else if (me.docClass === "operation") {
            //运营管理类
            sendCode = me.sendUnitText.value + "-" + me.recUnitText.value + "-" + me.cataAttrArray[0].receiveType + "-" + me.numberText.value;
        }
        return sendCode;
    },

    getDocIdentifier: function () {
        var me = this;
        return me.cataAttrArray[0].receiveType;
    },

    //获取发文单位代码
    getSendUnitCode: function () {
        var me = this;
        var result = me.sendUnitText.value;
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

        var fmSelectProject = Ext.create('Ext.plug_ins.ZHEPC_Plugins.Document.SelectProject', { title: "", mainPanelId: me.Id, projectKeyword: me.projectKeyword });

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

        var fmSelectUnit = Ext.create('Ext.plug_ins.ZHEPC_Plugins.Document.SelectUnit', { title: "", mainPanelId: me.Id, projectKeyword: me.projectDirKeyword });

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

                me.recUnitText.setValue(unitCode);
                me.cataAttrArray[0].recUnitCode = unitCode;

                me.recUnitDesc = unitDesc;

                me.sendGetFileId();

                me.AfterSelectRecUnit(unitCode, unitDesc);
            }
        });
    },


    selectRecDepartment: function () {
        var me = this;

        var fmSelectDepartment = Ext.create('Ext.plug_ins.ZHEPC_Plugins.Document.SelectDepartment', { title: "", mainPanelId: me.Id, projectKeyword: me.projectKeyword });

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

                me.recUnitText.setValue(departmentCode);
                me.cataAttrArray[0].recUnitCode = departmentCode;

                me.recUnitDesc = departmentDesc;

                me.sendGetFileId();
                me.AfterSelectRecUnit(departmentCode, departmentDesc);
            }
        });
    },


    selectSendUnit: function () {
        var me = this;

        var fmSelectUnit = Ext.create('Ext.plug_ins.ZHEPC_Plugins.Document.SelectUnit', { title: "", mainPanelId: me.Id, projectKeyword: me.projectDirKeyword });

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

                me.setSendUnit(unitCode, unitDesc);

                //me.sendUnitText.setValue(unitCode);

                //me.departmentText.setValue(unitCode);

                //me.cataAttrArray[0].department = unitCode;

                me.sendGetFileId();
                me.getFileCodeNum();

                
            }
        });
    },

    selectSendDepartment: function () {
        var me = this;

        var fmSelectDepartment = Ext.create('Ext.plug_ins.ZHEPC_Plugins.Document.SelectDepartment', { title: "", mainPanelId: me.Id, projectKeyword: me.projectKeyword });

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

                //me.sendUnitText.setValue(departmentCode);

                //me.departmentText.setValue(departmentCode);

                //me.cataAttrArray[0].department = departmentCode;

                me.setSendUnit(departmentCode, departmentDesc);

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

        var fmSelectUnit = Ext.create('Ext.plug_ins.ZHEPC_Plugins.Document.SelectUnit', { title: "", mainPanelId: me.Id, projectKeyword: prjKeyword });

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

        var fmSelectDepartment = Ext.create('Ext.plug_ins.ZHEPC_Plugins.Document.SelectDepartment', { title: "", mainPanelId: me.Id, projectKeyword: me.projectKeyword });

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
        var _fmEditFileProperties = Ext.create('Ext.plug_ins.ZHEPC_Plugins.Document.EditFileProperties', {
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
            me.sendUnitText.setValue(res.department);
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
        me.sendUnitText.setValue(res.department);
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
                C: "AVEVA.CDMS.ZHEPC_Plugins.Document", A: "GetFileCodeNumber",
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
                //C: "AVEVA.CDMS.ZHEPC_Plugins.Document", A: "GetMeetMinutesCNNumber",
                C: "AVEVA.CDMS.ZHEPC_Plugins.Document", A: "GetFileId",
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
            me.sendUnitText.hide();
            me.recUnitText.hide();
            me.numberText.hide();

            me.projectCodeButton.hide();
            me.sendUnitButton.hide();
            me.recUnitButton.hide();
        }
    },
});
