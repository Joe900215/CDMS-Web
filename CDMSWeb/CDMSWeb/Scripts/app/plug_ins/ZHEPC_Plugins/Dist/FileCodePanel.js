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

        me.documentType = "";

        me.crewList = [];
        me.sendUnitList = [];
        me.professionList = [];
        me.kindList = [];

        me.sendUnitdata = [];   //发文单位combo

        me.crewdata = [];   //机组combo

        me.professiondata = []; //专业

        me.kinddata = [];   //种类

        //添加发文单位代码text
        me.sendUnitText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "发文编号", labelWidth: 60, readOnly: true, emptyText: "发文单位",
            margin: '10 0 0 0', anchor: "80%", labelAlign: "left", width: 160//flex: 1
        });

        //添加表式text
        me.documentTypeText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: " - ", labelWidth: 10, value: "", labelSeparator: '', readOnly: true,
            margin: '10 0 0 10', anchor: "80%", labelAlign: "left", width: 100//flex: 1
        });

        //添加流水单号text
        //me.fNumberText = Ext.create("Ext.form.field.Text", {
        me.fNumberText = Ext.widget('textfield', {
            fieldLabel: " - ", labelWidth: 10, value: "", labelSeparator: '', emptyText: "流水号",
            margin: '10 0 0 10', anchor: "80%", labelAlign: "left", width: 100
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

        me.sendUnitCloumns = [
            { text: '单位代码', dataIndex: 'text' },
            { text: '单位名称', dataIndex: 'value' }
        ];
        me.sendUnitPicker = Ext.create('Ext.plug_ins.ZHEPC_Plugins.Dist.GridPickBox', {
            store: me.sendUnitStore, columns: me.sendUnitCloumns,
            displayField: 'text', valueField: 'text',
            emptyText: "发文单位", fieldLabel: "发文编号",
            labelWidth: 60, readOnly: true,
            margin: '10 0 0 0', width: 160,
            pickerWidth: 200, showPagingbar: false,
            editable: true, enableKeyEvents: true
        });

        me.sendUnitKeyupExpandCount = 0;
        me.sendUnitPicker.on('keyup', function (view, e, eOpts) {
            return;

            if (me.sendUnitPicker.picker == undefined) {
                me.sendUnitPicker.picker = me.sendUnitPicker.createPicker();
                me.sendUnitPicker.expand();
            }

            if (me.sendUnitPicker.picker != undefined) {
                me.sendUnitPicker.picker.show();
                me.sendUnitPicker.focus(false, 100);

                var filter = me.sendUnitPicker.rawValue;

                me.getsendUnitList(filter);
            }
        });

        me.sendUnitPicker.onExpand = function () {
            //展开两次
            if (me.sendUnitKeyupExpandCount <= 1) {
                me.sendUnitKeyupExpandCount = me.sendUnitKeyupExpandCount + 1;

                var filter = me.sendUnitPicker.rawValue;
                me.getsendUnitList(filter);
            } else {
                //me.sendUnitPicker.rawValue = "";
                //me.sendUnitPicker.lastValue = "";
                me.getsendUnitList("");
            }
        };


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


        me.crewCloumns = [
            { text: '机组代码', dataIndex: 'text' },
            { text: '机组名称', dataIndex: 'value',flex:1 }
        ];
        me.crewPicker = Ext.create('Ext.plug_ins.ZHEPC_Plugins.Dist.GridPickBox', {
            store: me.crewStore, columns: me.crewCloumns,
            displayField: 'text', valueField: 'text',
            emptyText: "机组", fieldLabel: " - ",
            labelWidth: 10, labelSeparator: '',
            margin: '10 0 0 10', width: 96,
            pickerWidth: 220, showPagingbar: false,
            editable: true, enableKeyEvents: true
        });

        me.crewKeyupExpandCount = 0;
        me.crewPicker.on('keyup', function (view, e, eOpts) {
            if (me.crewPicker.picker == undefined) {
                me.crewPicker.picker = me.crewPicker.createPicker();
                me.crewPicker.expand();
            }

            if (me.crewPicker.picker != undefined) {
                me.crewPicker.picker.show();
                me.crewPicker.focus(false, 100);

                var filter = me.crewPicker.rawValue;

                me.getCrewList(filter);
            }
        });

        me.crewPicker.onExpand = function () {
            //展开两次
            if (me.crewKeyupExpandCount <= 1) {
                me.crewKeyupExpandCount = me.crewKeyupExpandCount + 1;

                var filter = me.crewPicker.rawValue;
                me.getCrewList(filter);
            } else {
                //me.crewPicker.rawValue = "";
                //me.crewPicker.lastValue = "";
                me.getCrewList("");
            }
        };

        //机组表格行选中事件
        me.crewPicker.onPickerItemClick = function () {
            me.getFileCodeNum();
        };
 
        //添加专业combo
        Ext.define("professionModel", {
            extend: 'Ext.data.Model',
            idProperty: 'professionId',
            fields: [//"type", "text", "value"
                { name: 'type', type: 'string' },
                { name: 'id', type: 'string' },
                { name: 'text', type: 'string' },
                { name: 'value', type: 'string' }
            ]
        });
        me.professionProxy = Ext.create("Ext.data.proxy.Memory", {
            data: me.professiondata,
            model: "professionModel"
        });

        me.professionStore = Ext.create("Ext.data.Store", {
            model: professionModel,
            proxy: me.professionProxy,
            sorters: { property: 'due', direction: 'ASC' },
            groupField: 'type'
        });


        me.professionCloumns = [
            
            {
                text: '代码', dataIndex: 'text',
                sortable: true
            },
            {
                text: '名称', dataIndex: 'value',
                tdCls: 'task', flex: 1,
                hideable: false,
                sortable: true
            },
            {
                text: '类别', dataIndex: 'type',
                sortable: true
            }
        ];
        me.professionPicker = Ext.create('Ext.plug_ins.ZHEPC_Plugins.Dist.GridPickBox', {
            store: me.professionStore,
            columns: me.professionCloumns,
            displayField: 'text', valueField: 'text',
            emptyText: "专业", fieldLabel: " - ",
            labelWidth: 10, labelSeparator: '',
            margin: '10 0 0 10', width: 96,
            pickerWidth: 220, showPagingbar: false,
            editable: true, enableKeyEvents: true
        });

        me.professionKeyupExpandCount = 0;
        me.professionPicker.on('keyup', function (view, e, eOpts) {
            if (me.professionPicker.picker == undefined) {
                me.professionPicker.picker = me.professionPicker.createPicker();
                me.professionPicker.expand();
            }

            if (me.professionPicker.picker != undefined) {
                me.professionPicker.picker.show();
                me.professionPicker.focus(false, 100);

                var filter = me.professionPicker.rawValue;

                me.getProfessionList(filter);
            }
        });

        me.professionPicker.onExpand = function () {
            //展开两次
            if (me.professionKeyupExpandCount <= 1) {
                me.professionKeyupExpandCount = me.professionKeyupExpandCount + 1;

                var filter = me.professionPicker.rawValue;
                me.getProfessionList(filter);
            } else {
                //me.professionPicker.rawValue = "";
                //me.professionPicker.lastValue = "";
                me.getProfessionList("");
            }
        };

        //专业表格行选中事件
        me.professionPicker.onPickerItemClick = function () {
            me.getFileCodeNum();
        };


        //添加种类combo
        Ext.define("kindModel", {
            extend: 'Ext.data.Model',
            fields: ["text", "value"]
        });
        me.kindProxy = Ext.create("Ext.data.proxy.Memory", {
            data: me.kinddata,
            model: "kindModel"
        });

        me.kindStore = Ext.create("Ext.data.Store", {
            model: kindModel,
            proxy: me.kindProxy
        });


        me.kindCloumns = [
            { text: '种类代码', dataIndex: 'text' },
            { text: '种类名称', dataIndex: 'value', flex:1 }
        ];
        me.kindPicker = Ext.create('Ext.plug_ins.ZHEPC_Plugins.Dist.GridPickBox', {
            store: me.kindStore, columns: me.kindCloumns,
            displayField: 'text', valueField: 'text',
            emptyText: "种类", fieldLabel: " - ",
            labelWidth: 10, labelSeparator: '',
            margin: '10 0 0 10', width: 96, hidden: true,
            pickerWidth: 200, showPagingbar: false,
            editable: true, enableKeyEvents: true
        });

        me.kindKeyupExpandCount = 0;
        me.kindPicker.on('keyup', function (view, e, eOpts) {
            if (me.kindPicker.picker == undefined) {
                me.kindPicker.picker = me.kindPicker.createPicker();
                me.kindPicker.expand();
            }

            if (me.kindPicker.picker != undefined) {
                me.kindPicker.picker.show();
                me.kindPicker.focus(false, 100);

                var filter = me.kindPicker.rawValue;

                me.getKindList(filter);
            }
        });

        me.kindPicker.onExpand = function () {
            //展开两次
            if (me.kindKeyupExpandCount <= 1) {
                me.kindKeyupExpandCount = me.kindKeyupExpandCount + 1;

                var filter = me.kindPicker.rawValue;
                me.getKindList(filter);
            } else {
                //me.kindPicker.rawValue = "";
                //me.kindPicker.lastValue = "";
                me.getKindList("");
            }
        };

        //专业表格行选中事件
        me.kindPicker.onPickerItemClick = function () {
            me.getFileCodeNum();
        };

        //广东院的编码
        me.DrawingWordNumText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "发文编号", labelWidth: 60,  emptyText: "", hidden: true, 
            margin: '10 0 0 0', anchor: "80%", labelAlign: "left", width: 160//flex: 1
        });

        me.DrawingNumberText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "字第", labelWidth: 30,  emptyText: "", labelSeparator: '',
            margin: '10 5 0 5', anchor: "80%", labelAlign: "left", width: 140, hidden: true
            
        });
        me.DrawingNumberLabel = Ext.create("Ext.form.Label", {
            text: '号', margin: '13 0 0 0', anchor: "80%", labelAlign: "left", hidden: true
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
             
                    me.sendUnitPicker,//发文单位
                    me.crewPicker,//机组
                    me.professionPicker,//专业
                    me.kindPicker,//专业
                       
                    me.documentTypeText,//表式
                    me.fNumberText,//流水号

                    me.groupinputtext,

                    me.DrawingWordNumText,  //XX字
                    me.DrawingNumberText,   //第XX
                    me.DrawingNumberLabel   //号
                ],
                flex: 1
            },

        ];



        me.callParent(arguments);
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
                    me.sendUnitList = eval(recod.UnitList);
                    me.professionList = eval(recod.ProfessionList);
                    me.kindList = eval(recod.KindList);

                    var curUnitCode = recod.CurUnitCode;

                    me.sendUnitPicker.store.removeAll(false);

                    //遍历发文单位数组，添加到发文单位combo
                    for (var itemKey in me.sendUnitList) {
                        var sendUnitItem = me.sendUnitList[itemKey];
                        var sendUnitItemCode = sendUnitItem.unitCode;
                        var sendUnitItemDesc = sendUnitItem.unitDesc;

                        //插入行到返回grid
                        var r = Ext.create('sendUnitModel', {
                            text: sendUnitItemCode, value: sendUnitItemDesc
                        });

                        var rowlength = me.sendUnitPicker.store.data.length;
                        me.sendUnitPicker.store.insert(rowlength, r);
                    }

                    me.sendUnitPicker.setValue(curUnitCode);

                    me.crewPicker.store.removeAll(false);

                    //遍历机组数组，添加到机组combo
                    for (var itemKey in me.crewList) {
                        var crewItem = me.crewList[itemKey];
                        var crewItemCode = crewItem.crewCode;
                        var crewItemDesc = crewItem.crewDesc;
             
                        //插入行到返回grid
                        var r = Ext.create('crewModel', {
                            text: crewItemCode, value: crewItemDesc
                        });

                        var rowlength = me.crewPicker.store.data.length;
                        me.crewPicker.store.insert(rowlength, r);
                    }

            

                    me.professionPicker.store.removeAll(false);

                    if (me.professionList.length > 0 && me.professionList[0].professionCode != undefined && me.professionList[0].professionCode != "") {
                        //当只有一个专业的时候，固定这个专业，并且不能手动选择
                        var itemKey = 0;
                        var professionItem = me.professionList[itemKey];
                        var professionItemCode = professionItem.professionCode;
                        var professionItemDesc = professionItem.professionDesc;

                        //插入行到返回grid
                        var r = Ext.create('professionModel', {
                            text: professionItemCode, value: professionItemDesc
                        });

                        var rowlength = me.professionPicker.store.data.length;
                        me.professionPicker.store.insert(rowlength, r);
                        me.professionPicker.setValue( professionItemCode);
                        me.professionPicker.setReadOnly(true);
                    } else {
                        //遍历专业数组，添加到专业combo
                        for (var itemKey in me.professionList) {
                            var professionItem = me.professionList[itemKey];
                            var professionItemCode = professionItem.professionCode;
                            var professionItemDesc = professionItem.professionDesc;

                            //插入行到返回grid
                            var r = Ext.create('professionModel', {
                                text: professionItemCode, value: professionItemDesc
                            });

                            var rowlength = me.professionPicker.store.data.length;
                            me.professionPicker.store.insert(rowlength, r);
                        }
                    }

                    me.kindPicker.store.removeAll(false);

                    //遍历种类数组，添加到种类combo
                    for (var itemKey in me.kindList) {
                        var kindItem = me.kindList[itemKey];
                        var kindItemCode = kindItem.kindCode;
                        var kindItemDesc = kindItem.kindDesc;

                        //插入行到返回grid
                        var r = Ext.create('kindModel', {
                            text: kindItemCode, value: kindItemDesc
                        });

                        var rowlength = me.kindPicker.store.data.length;
                        me.kindPicker.store.insert(rowlength, r);
                    }

                    //A.6表式，显示种类选择框
                    if (me.documentTypeText.value === "A.6") {
                        me.kindPicker.show();
                    }

                    //C.3表式，显示广东院文件编码
                    if (me.documentTypeText.value === "C.3") {
                        me.sendUnitText.hide();
                        me.documentTypeText.hide();
                        me.fNumberText.hide();
                        me.sendUnitPicker.hide();
                        me.crewPicker.hide();
                        me.professionPicker.hide();

                        me.DrawingWordNumText.show();
                        me.DrawingNumberText.show();
                        me.DrawingNumberLabel.show();
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

    //获取单位列表
    getsendUnitList: function (filter) {
        var me = this;

        Ext.Ajax.request({
            url: 'WebApi/Get',
            method: "Get",
            params: {
                C: "AVEVA.CDMS.ZHEPC_Plugins.Document", A: "GetUnitList",
                sid: localStorage.getItem("sid"), ProjectKeyword: me.projectKeyword,
                Filter: filter
            },
            success: function (response, options) {
                //获取数据后，更新窗口
                var res = Ext.JSON.decode(response.responseText, true);
                var state = res.success;
                if (state === true) {
                    var recod = eval(res.data[0]);

                    me.sendUnitList = eval(recod.UnitList);

                    me.sendUnitPicker.store.removeAll(false);

                    //遍历机组数组，添加到机组combo
                    for (var itemKey in me.sendUnitList) {
                        var sendUnitItem = me.sendUnitList[itemKey];
                        var sendUnitItemCode = sendUnitItem.unitCode;
                        var sendUnitItemDesc = sendUnitItem.unitDesc;
                        //me.sendUnitdata.push({ text: sendUnitItemCode, value: sendUnitItemDesc });//在数组里添加新的元素

                        //插入行到返回grid
                        var r = Ext.create('sendUnitModel', {
                            text: sendUnitItemCode, value: sendUnitItemDesc
                        });

                        var rowlength = me.sendUnitPicker.store.data.length;
                        me.sendUnitPicker.store.insert(rowlength, r);
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

    //获取机组列表
    getCrewList: function (filter) {
        var me = this;

        Ext.Ajax.request({
            url: 'WebApi/Get',
            method: "Get",
            params: {
                C: "AVEVA.CDMS.ZHEPC_Plugins.Document", A: "GetCrewList",
                sid: localStorage.getItem("sid"), ProjectKeyword: me.projectKeyword,
                Filter: filter
            },
            success: function (response, options) {
                //获取数据后，更新窗口
                var res = Ext.JSON.decode(response.responseText, true);
                var state = res.success;
                if (state === true) {
                    var recod = eval(res.data[0]);

                    me.crewList = eval(recod.CrewList);

                    me.crewPicker.store.removeAll(false);

                    //遍历机组数组，添加到机组combo
                    for (var itemKey in me.crewList) {
                        var crewItem = me.crewList[itemKey];
                        var crewItemCode = crewItem.crewCode;
                        var crewItemDesc = crewItem.crewDesc;
                        //me.crewdata.push({ text: crewItemCode, value: crewItemDesc });//在数组里添加新的元素

                        //插入行到返回grid
                        var r = Ext.create('crewModel', {
                            text: crewItemCode, value: crewItemDesc
                        });

                        var rowlength = me.crewPicker.store.data.length;
                        me.crewPicker.store.insert(rowlength, r);
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

    //获取专业列表
    getProfessionList: function (filter) {
        var me = this;

        Ext.Ajax.request({
            url: 'WebApi/Get',
            method: "Get",
            params: {
                C: "AVEVA.CDMS.ZHEPC_Plugins.Document", A: "GetProfessionList",
                sid: localStorage.getItem("sid"), ProjectKeyword: me.projectKeyword,
                Filter: filter
            },
            success: function (response, options) {
                //获取数据后，更新窗口
                var res = Ext.JSON.decode(response.responseText, true);
                var state = res.success;
                if (state === true) {
                    var recod = eval(res.data[0]);

                    me.professionList = eval(recod.ProfessionList);

                    me.professionPicker.store.removeAll(false);

                    //遍历机组数组，添加到机组combo
                    for (var itemKey in me.professionList) {
                        var professionItem = me.professionList[itemKey];
                        var professionItemCode = professionItem.professionCode;
                        var professionItemDesc = professionItem.professionDesc;
                        var professionType = professionItem.professionType;
                        //me.professiondata.push({ text: professionItemCode, value: professionItemDesc });//在数组里添加新的元素

                        //插入行到返回grid
                        var r = Ext.create('professionModel', {
                            type:professionType,text: professionItemCode, value: professionItemDesc
                        });

                        var rowlength = me.professionPicker.store.data.length;
                        me.professionPicker.store.insert(rowlength, r);
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

    //获取种类列表
    getKindList: function (filter) {
        var me = this;

        Ext.Ajax.request({
            url: 'WebApi/Get',
            method: "Get",
            params: {
                C: "AVEVA.CDMS.ZHEPC_Plugins.Document", A: "GetKindList",
                sid: localStorage.getItem("sid"), ProjectKeyword: me.projectKeyword,
                Filter: filter
            },
            success: function (response, options) {
                //获取数据后，更新窗口
                var res = Ext.JSON.decode(response.responseText, true);
                var state = res.success;
                if (state === true) {
                    var recod = eval(res.data[0]);

                    me.kindList = eval(recod.KindList);

                    me.kindPicker.store.removeAll(false);

                    //遍历机组数组，添加到机组combo
                    for (var itemKey in me.kindList) {
                        var kindItem = me.kindList[itemKey];
                        var kindItemCode = kindItem.kindCode;
                        var kindItemDesc = kindItem.kindDesc;
                        //me.kinddata.push({ text: kindItemCode, value: kindItemDesc });//在数组里添加新的元素

                        //插入行到返回grid
                        var r = Ext.create('kindModel', {
                            text: kindItemCode, value: kindItemDesc
                        });

                        var rowlength = me.kindPicker.store.data.length;
                        me.kindPicker.store.insert(rowlength, r);
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

    //获取文件编码
    getFileCode: function () {
        var me = this;
        var fileCode = "";
 
        if (me.documentTypeText.value === "C.3") {
            fileCode = me.DrawingWordNumText.value + "字第" +
                me.DrawingNumberText.value + "号";
        }
        else if (me.documentTypeText.value === "A.6") {
            fileCode = me.sendUnitPicker.value + "-" + me.crewPicker.value + "-" +
                me.professionPicker.value + "-" + me.kindPicker.value + "-" +
                me.documentTypeText.value + "-" + me.fNumberText.value;
        } else {
            fileCode = me.sendUnitPicker.value + "-" + me.crewPicker.value + "-" +
                me.professionPicker.value + "-" + me.documentTypeText.value + "-" + me.fNumberText.value;
        }

        return fileCode;
    },


    //获取发文单位代码
    getSendUnitCode: function () {
        var me = this;
        var result = me.sendUnitText.value;
        return result;
    },

    //检查文件代码是否填写完整
    checkFileCodeFill: function () {
        var me = this;
        var isFill = true;

        if (me.documentTypeText.value === "C.3") {
            if (me.DrawingWordNumText.value === undefined || me.DrawingWordNumText.value === "" ||
                me.DrawingNumberText.value === undefined || me.DrawingNumberText.value === "") {
                return "请输入图纸字号！";
            }
            return "true";
        }

        if (me.crewPicker.length <= 0 || me.crewPicker.value[0] === undefined || me.crewPicker.value[0] === "") {
            return "请选择机组！";
        }
        if (me.professionPicker.length <= 0 || me.professionPicker.value[0] === undefined || me.professionPicker.value[0] === "") {
            return "请选择专业！";
        }

        if (me.documentTypeText.value === undefined || me.documentTypeText.value === "") {
            return "请选择文件类型！";
        }

        if (me.documentTypeText.value === "A.6") {
            if (me.kindPicker.length <= 0 || me.kindPicker.value[0] === undefined || me.kindPicker.value[0] === "") {
                return "请选择种类！";
            }

        }

        if (me.fNumberText.value === undefined || me.fNumberText.value === "") {
            return "请输入流水号！";
        }

        return "true";
    },
 
 

    getFileId: function () {
        var me = this;
        return me.numberText.value;
    },

    //获取文件编码流水号
    getFileCodeNum: function () {
        var me = this;


        var strPerfix = "";
 
            //项目管理类
        var sendUnitCode = me.sendUnitPicker.value === undefined ? "" : me.sendUnitPicker.value;
        var crew = me.crewPicker.value === undefined ? "" : me.crewPicker.value;
        var profession = me.professionPicker.value === undefined ? "" : me.professionPicker.value;

        //文件类型
        var receiveType = me.documentTypeText.value === undefined ? "" : me.documentTypeText.value;
        if (receiveType === "") return;

        if (sendUnitCode === "") return;
            if (crew === "") return;
            if (profession === "") return;

            

            var kind = "";
            if (receiveType === "A.6") {
                kind = me.kindPicker.value === undefined ? "" : me.kindPicker.value;
                if (kind === "") return;
                strPerfix = strPerfix = sendUnitCode + "-" + crew + "-" + profession + "-" + kind + "-" + receiveType + "-";
            } else {
                strPerfix = sendUnitCode + "-" + crew + "-" + profession + "-" + receiveType + "-";
            }

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
