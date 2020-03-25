//签证

Ext.define('Ext.plug_ins.HXEPC_Plugins.Document.DraftVisa', {
    extend: 'Ext.container.Container',
    alias: 'widget.DraftVisa',
    //layout: "border",
    layout: 'fit',
    resultvalue: '', mainPanelId: '', projectKeyword: '',
    initComponent: function () {
        var me = this;

        //上传文档的doc关键字列表
        me.docList = "";

        //附件文件名的前缀
        me.docCode = "";
        me.docFileName = "";

        //附件序号
        me.docUploadIndex = 0;

        //下一流程状态用户
        me.nextStateUserList = "";

        //收发文单位列表初始数据
        me.recCompanyList = [];
        me.sendCompanyList = [];

        //定义文件编码Panel
        me.fileCodePanel = Ext.create('Ext.plug_ins.HXEPC_Plugins.Document.Panel.FileCodePanel');

        //定义文件上传Panel
        me.fileUploadPanel = Ext.create('Ext.plug_ins.HXEPC_Plugins.Document.Panel.FileUploadPanel', {
            projectKeyword: me.projectKeyword, projectDirKeyword: me.projectDirKeyword
        });

        //设置上传控件为附件模式
        me.fileUploadPanel.setAttaMode();

        //me.fileUploadPanel.gridMaxHeight = me.container.lastBox.height - 40;
        me.fileUploadPanel.setGridMinHeight(198);

        me.fileUploadPanel.onFileEditButtonClick = function () {
            me.editTopPanel.hide();
            me.editBottomPanel.hide();
            me.bottomButtonPanel.hide();
            me.fileUploadPanel.setHeight(me.container.lastBox.height - 40);
            me.fileUploadPanel.filegrid.setHeight(me.container.lastBox.height - 40);
            winDraftVisa.setTitle("起草签证单 - 编辑附件");
            winDraftVisa.closable = false;
        };

        //保存附件按钮事件
        me.fileUploadPanel.onFileSaveButtonClick = function () {

            //me.totalPagesText.setValue(me.fileUploadPanel.totalPages);

            me.editTopPanel.show();
            me.editBottomPanel.show();
            me.bottomButtonPanel.show();
            //me.filegrid.setHeight(me.gridMinHeight);
            winDraftVisa.setTitle("起草签证单");
            winDraftVisa.closable = true;
        };

        //定义区域combo初始数据
        me.areadata = [];

        //定义区域combo初始数据
        //me.professiondata = [];

        //定义发文单位combo初始数据
        me.sendCompanydata = [];

        //定义收文单位combo初始数据
        me.recCompanydata = [];


        //工程名称text
        me.projectNameText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "工程名称", labelWidth: 70,
            margin: '10 10 0 10', anchor: "80%", labelAlign: "right", width: '50%'//flex: 1
        });
        
        //施工部位text
        me.construcSiteText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "施工部位", labelWidth: 60,
            margin: '10 10 0 10', anchor: "80%", labelAlign: "right", width: '50%'//flex: 1
        });

        //根据指令人姓名text
        me.instrucManNameText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "根据", labelWidth: 70, emptyText: "指令人姓名", labelSeparator: '', // 去掉laebl中的冒号
            margin: '10 10 0 10', anchor: "80%", labelAlign: "right", width: 200
        });


        //定义下一流程状态用户Text
        me.nextStateUserText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "项目专工", anchor: "80%", labelWidth: 60, labelAlign: "right", labelPad: 8, width: "40%",//width: 230, 
            margin: '10 5 0 10', fieldStyle: ' background-image: none;'//红色边框//flex: 1
        });

        //定义通知日期Text
        me.noticeDateField = Ext.create("Ext.form.field.Date", {
            name: "date",
            //fieldLabel: ' 通知日期',
            fieldStyle: ' background-image: none;',
            editable: true, labelWidth: 50, margin: '10 10 0 10',
            anchor: "80%", labelAlign: "right", labelPad: 8, width: 150,
            emptyText: "--请选择日期--",
            format: 'Y年m月d日'
            //,value: new Date()
        });



        //Text
        me.CenterText = Ext.create("Ext.form.Label", {
            xtype: "label",
            margin: '12 0 0 0',
            text: "的书面通知，我方要求完成此项工作应支付价款金额为"
        }),


        //大写金额Text
        me.MaxMoneyText = Ext.create("Ext.form.field.Text",
        {
            //xtype: "combo",
            fieldLabel: '(大写)', labelWidth: 70,
            displayField: 'text',
            // readonly:true,
            margin: '10 0 0 0', labelSeparator: '',
            anchor: "80%", labelAlign: "right", //labelPad: 8, 
            width: 375,//
            emptyText: "大写金额,不可输入,直接在小写框输入",
            margin: '10 0 0 10',
            //fieldStyle: 'border-color: red; background-image: none;',//红色边框

        });
        me.MaxMoneyText.readOnly = true;
        //小写金额Text
        me.SmallMoneyText = Ext.create("Ext.form.field.Number", {
            // xtype: "textfield",
            anchor: "100%", labelAlign: "right",
            fieldLabel: "(小写)",
            labelWidth: 40,
            width: 220,//width: , "40%"
            hideTrigger: true,//隐藏微调按钮
            emptyText: "小写金额",
            margin: '10 0 0 10',
            labelSeparator: '', // 去掉laebl中的冒号
            fieldStyle: ' background-image: none;',//红色边框//flex: 1
            listeners:
            {
                change: function (textfield, newValue, oldValue) {

                    // alert(field);
                    var value = textfield.getValue();
                    var chang = me.lowMoneyToUp(value);
                    me.MaxMoneyText.setValue(chang);

                }
                //select: function (combo, records, eOpts) {
                //}
            }
            //margin: '10 0 0 10', fieldStyle: ' background-image: none;'//红色边框//flex: 1
        }),

        //Text
        me.TailText = Ext.create("Ext.form.Label", {
            xtype: "label",
            margin: '12 0 10 10',
            text: "，请予核准。"
        }),
        ////小写金额Text
        //me.SmallMoneyText = Ext.create("Ext.form.field.Text", {
        //    xtype: "textfield",
        //    anchor: "80%", labelAlign: "right", labelPad: 8, width: 100,//width: , "40%"
        //    emptyText: "小写金额",
        //    margin: '10 0 0 10', fieldStyle: ' background-image: none;'//红色边框//flex: 1
        //}),

        me.cellEditing = new Ext.grid.plugin.CellEditing({
            clicksToEdit: 1
        });





        //编辑区域头部
        me.editTopPanel = Ext.create("Ext.panel.Panel", {
            baseCls: 'my-panel-no-border',//隐藏边框
            layout: {
                type: 'vbox',
                pack: 'start',
                align: 'stretch'
            },
            margin: '0 0 0 0',// 
            items: [
                     me.fileCodePanel,
                    {

                        baseCls: 'my-panel-no-border',//隐藏边框
                        layout: {
                            type: 'hbox',
                            pack: 'start',
                            align: 'stretch'
                        },
                        items: [
                            me.projectNameText,
                            me.construcSiteText
                        ],
                        flex: 1
                    },

                     {
                         layout: "hbox",
                         width: '100%',
                         align: 'stretch',
                         pack: 'start',
                         baseCls: 'my-panel-no-border',//隐藏边框
                         items: [
                            me.instrucManNameText,
                             me.noticeDateField,
                           me.CenterText,
 
                         ], flex: 2
                     },
                    // me.SmallMoneyText,
			        {
			            layout: "hbox",
			            width: '100%',
			            align: 'left',
			            pack: 'start',
			            baseCls: 'my-panel-no-border',//隐藏边框
			            items: [

                              me.MaxMoneyText,
			                  me.SmallMoneyText,
                              me.TailText,

			            ], flex: 1
			        },

            ]
        });


        //编辑区域尾部
        me.editBottomPanel = Ext.create("Ext.panel.Panel", {
            layout: "hbox",
            width: '100%',
            align: 'stretch',
            pack: 'start', margins: "0 0 0 0",
            baseCls: 'my-panel-no-border',//隐藏边框
            items: [
             // me.approvpathCombo,//定义审批路径
                me.nextStateUserText,
                      {
                          xtype: "button",
                          text: "选择...", margins: "10 0 0 5",
                          listeners: {
                              "click": function (btn, e, eOpts) {//添加点击按钮事件
                                  Ext.require('Ext.ux.Common.comm', function () {
                                      showSelectUserWin("getUser", "", "", function () {
                                          me.nextStateUserText.setValue(window.parent.usernamelist);
                                          me.nextStateUserList = window.parent.resultvalue;
                                      });
                                  })
                              }
                          }
                      }
            ]//, flex: 1
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
                            me.send_draft_document();
                        }
                    }
                },
                {
                    xtype: "button",
                    text: "取消", width: 60, margins: "10 15 10 5",
                    listeners: {
                        "click": function (btn, e, eOpts) {//添加点击按钮事件
                            winDraftVisa.close();
                        }
                    }
                }
            ]
        });


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
                     me.fileUploadPanel,
                     me.editBottomPanel


                  ], flex: 1
              },
              me.bottomButtonPanel

              ]
          })

        ];



        //
        //me.sendGetDraftVisaDefault();


        //以下为隐藏
        me.fileCodePanel.projectCodeText.setVisible(false);
        me.fileCodePanel.projectCodeButton.setVisible(false);
        me.fileCodePanel.sendCompanyText.setVisible(false);
        me.fileCodePanel.sendCompanyButton.setVisible(false);
        me.fileCodePanel.recCompanyText.setVisible(false);
        me.fileCodePanel.recCompanyButton.setVisible(false);
        me.fileCodePanel.numberText.setVisible(false);

        me.callParent(arguments);
    },

    //获取起草信函表单默认参数
    sendGetDraftVisaDefault: function (funCallback) {
        var me = this;

        //通过extjs的ajax获取操作全部名称
        Ext.Ajax.request({
            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.HXEPC_Plugins.Document", A: "GetDraftVisaDefault",
                sid: localStorage.getItem("sid"), ProjectKeyword: me.projectKeyword
            },
            success: function (response, options) {
                me.sendGetDraftVisaDefault_callback(response, options, funCallback);

            }
        });
    },


    //处理获取发文处理表单默认参数的返回
    sendGetDraftVisaDefault_callback: function (response, options, funCallback) {
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
            var sourceCompany = recod.SourceCompany;//项目所属公司
            var sourceCompanyDesc = recod.SourceCompanyDesc;

            var strFormClassCode = "AQV"
            var strProjectDesc = "起草签证单";

            //默认设置为不新建文件编码
            me.fileCodePanel.setNeedNewFileCode(true);

            //设置发起目录和项目所在目录
            me.fileCodePanel.projectKeyword = me.projectKeyword;//项目所在目录
            me.fileCodePanel.projectDirKeyword = me.projectKeyword;//当前目录

            //设置收发文单位的单位类型
            me.fileCodePanel.setFormClass(strFormClassCode, strProjectDesc);
            //设置项目管理类文件里项目的代码和描述
            me.fileCodePanel.setRootProject(strRootProjectCode, strRootProjectDesc);

            //默认都是项目发到部门
            //me.fileCodePanel.setDocUnitClass("项目", "部门");
            if (strRootProjectCode === undefined || strRootProjectCode === "") {
                //运营信函

                me.fileCodePanel.setDocUnitClass("部门", "部门");
                //隐藏发给项目单选框
                //me.fileCodePanel.toProjectCheckBox.setVisible(false);


            } else {
                //项目（非运营）信函
                me.fileCodePanel.setDocUnitClass("项目", "项目");

            }

            //设置发文单位代码
            me.fileCodePanel.setSendCompany(sourceCompany, sourceCompanyDesc);

            //设置文件编码Panel的各个按钮的用户事件
            me.fileCodePanel.AfterSelectRecCompany = function (code, desc) {
                //me.deliveryUnitText.setValue(desc);
            }

            me.checkUploadFilesDefault = function () {
                // 1.判断是否有选择了文件，并勾选了唯一一个文件
                if (me.fileUploadPanel.FileUploadButton.uploader.uploader.files.length <= 0) {
                    //当没有附件时，处理返回事件
                    Ext.Msg.alert("错误信息", "请选择需要上传的文件！");
                    return false;
                }
                return true;
            }

            //机组按钮按下前的事件
            me.fileCodePanel.beforeSelectCrew = me.checkUploadFilesDefault;

            //厂房按钮按下前的事件
            me.fileCodePanel.beforeSelectFactory = me.checkUploadFilesDefault;

            //系统按钮按下前的事件
            me.fileCodePanel.beforeSelectSystem = me.checkUploadFilesDefault;

            //专业按钮按下前的事件
            me.fileCodePanel.beforeSelectProfession = me.checkUploadFilesDefault;

            //录入属性按钮按下前的事件
            me.fileCodePanel.beforeEditAttr = me.checkUploadFilesDefault;

            //录入属性后的事件
            me.fileCodePanel.afterEditAttr = function (res) {
                //Ext.Msg.alert("错误信息", "请选择需要上传的文件！");
                me.fileUploadPanel.filegrid.getView().refresh();
            }

            me.fileUploadPanel.setSendCompany(sourceCompany, sourceCompanyDesc);

            //设置项目管理类文件里项目的代码和描述
            me.fileUploadPanel.setRootProject(strRootProjectCode, strRootProjectDesc);

            //设置文件上传表格的模式
            me.fileUploadPanel.setFileGridMode("REC");

            //录入属性按钮按下前的事件
            me.fileUploadPanel.beforeEditAttr = function () {

                //如果是第一个文件，就默认勾选新建文件编码，并设置文件类型
                var grid = me.fileUploadPanel.filegrid;
                var rs = grid.getSelectionModel().getSelection();//获取选择的文档
                if (rs !== null && rs.length > 0) {
                    var rec = rs[0];//第一个文档

                    var store = grid.store;
                    var recored = store.getAt(0);
                    if (rec.data.id === recored.data.id) {

                        //是否新建文件编码
                        rec.set('needNewFileCode', true);
                        //文件编码类型
                        rec.set('receiveType', "AQV");

                        rec.commit();

                        //是否新建文件编码
                        recored.set('needNewFileCode', true);
                        //文件编码类型
                        recored.set('receiveType', "AQV");

                        recored.commit();
                    }
                }
                return true;
            }

            //录入属性后的事件
            me.fileUploadPanel.afterEditAttr = function (res) {
                var store = me.fileUploadPanel.filegrid.store;

                //如果是第一个文件，就更新表单文件编码栏的著录属性
                var recored = store.getAt(0);
                if (res.id === recored.data.id) {
                    me.fileCodePanel.updateCataAttr(res);
                }

            }

            //设置第一个文件是正件
            //me.fileCodePanel.firstFileIsPositive = true;

            //me.fileCodePanel.setFirstFileIsPositive(true);


            me.fileCodePanel.fNumberText.setValue(strDocNumber);

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

    //功能：小写金额转大写
    lowMoneyToUp: function (money) {//小写数字金额
        // var u = App.base.Utils,
        var cnNums = new Array("零", "壹", "贰", "叁", "肆", "伍", "陆", "柒", "捌", "玖");
        var cnIntRadice = new Array("", "拾", "佰", "仟"); //基本单位
        var cnIntUnits = new Array("", "万", "亿", "兆"); //对应整数部分扩展单位
        var cnDecUnits = new Array("角", "分", "毫", "厘"); //对应小数部分单位
        var cnInteger = "整"; //整数金额时后面跟的字符
        var cnIntLast = "圆"; //整型完以后的单位
        var maxNum = 999999999999999.9999; //最大处理的数字
        var IntegerNum; //金额整数部分
        var DecimalNum;//金额小数部分
        var ChineseStr = ""; //输出的中文金额字符串
        var parts; //分离金额后用的数组，预定义
        if (!!money)
            money = parseFloat(money);
        else
            return "";
        if (money >= maxNum) {
            u.toast('超出最大处理数字');
            return "";
        }
        if (money == 0) {
            ChineseStr = cnNums[0] + cnIntLast + cnInteger;
            return ChineseStr;
        }
        money = money.toString(); //转换为字符串
        if (money.indexOf(".") == -1) {
            IntegerNum = money;
            DecimalNum = '';
        } else {
            parts = money.split(".");
            IntegerNum = parts[0];
            DecimalNum = parts[1].substr(0, 4);
        }
        if (parseInt(IntegerNum, 10) > 0) { //获取整型部分转换
            var zeroCount = 0,
                IntLen = IntegerNum.length;
            for (var i = 0; i < IntLen; i++) {
                var n = IntegerNum.substr(i, 1);
                var p = IntLen - i - 1;
                var q = p / 4;
                var m = p % 4;
                if (n == "0") {
                    zeroCount++;
                } else {
                    if (zeroCount > 0) {
                        ChineseStr += cnNums[0];
                    }
                    zeroCount = 0; //归零
                    ChineseStr += cnNums[parseInt(n)] + cnIntRadice[m];
                }
                if (m == 0 && zeroCount < 4) {
                    ChineseStr += cnIntUnits[q];
                }
            }
            ChineseStr += cnIntLast;
            //整型部分处理完毕
        }
        if (DecimalNum != '') { //小数部分
            var decLen = DecimalNum.length;
            for (var i = 0; i < decLen; i++) {
                var n = DecimalNum.substr(i, 1);
                if (n != '0') {
                    ChineseStr += cnNums[Number(n)] + cnDecUnits[i];
                }
            }
        }
        if (ChineseStr == '') {
            ChineseStr += cnNums[0] + cnIntLast + cnInteger;
        } else if (DecimalNum == '') {
            ChineseStr += cnInteger;
        }
        return ChineseStr;//返回大写人民币金额

    },

    //向服务器发送起草认质认价单请求
    send_draft_document: function () {
        var me = this;

        //0.用户选择所有需要上传的文件，并勾选正件

        //1.判断是否有选择了文件，并勾选了唯一一个文件

        //2.上传所有文件

        //3.获取勾选的文件

        //4.把勾选的文件的属性，修改为正件的属性

        //检查文件编码
        var checkResult = me.fileCodePanel.checkFileCodeFill();
        if (checkResult != "true") {
            Ext.Msg.alert("错误信息", checkResult);
            return;
        }

        if (me.nextStateUserList === undefined || me.nextStateUserList === "") {
            Ext.Msg.alert("错误信息", "请选择项目专工！");
            return;
        }

        if (me.projectNameText.value === undefined || me.projectNameText.value === "") {
            Ext.Msg.alert("错误信息", "请填写工程名称！");
            return;
        }

        if (me.construcSiteText.value === undefined || me.construcSiteText.value === "") {
            Ext.Msg.alert("错误信息", "请填写施工部位！");
            return;
        }

        if (me.instrucManNameText.value === undefined || me.instrucManNameText.value === "") {
            Ext.Msg.alert("错误信息", "请填写指令人姓名！");
            return;
        }

        if (me.noticeDateField.value === undefined || me.noticeDateField.value === "") {
            Ext.Msg.alert("错误信息", "请选择通知日期！");
            return;
        }
        if (me.SmallMoneyText.value === undefined || me.SmallMoneyText.value === "") {
            Ext.Msg.alert("错误信息", "请填写金额！");
            return;
        }

        // 1.判断是否有选择了文件，并勾选了唯一一个文件
        if (me.fileUploadPanel.FileUploadButton.uploader.uploader.files.length > 0) {

            //var grid = me.fileUploadPanel.filegrid;

            //var rs = grid.getSelectionModel().getSelection();//获取选择的文档

            //if (rs === undefined || rs === null || rs.length <= 0) {

            //    Ext.Msg.alert("错误信息", "请勾签证单文件！");
            //    return;

            //}
            //if (rs.length > 1) {

            //    Ext.Msg.alert("错误信息", "请勾选唯一的签证单文件！");
            //    return;

            //}
            ////2.上传所有文件
            me.fileUploadPanel.afterUploadAllFile = function () {
                me.set_positive_file();
            };

            me.fileUploadPanel.send_upload_file();
        } else {
            //当没有附件时，处理返回事件
            //me.set_positive_file();
            Ext.Msg.alert("错误信息", "请选择需要上传的文件！");
        }

    },

    set_positive_file: function () {

        var me = this;

        //3.获取勾选的文件
        var grid = me.fileUploadPanel.filegrid;

        var store = grid.store;//获取选择的文档

        if (store.getCount() > 0) {
            var rec = store.getAt(0);//第一个文档
            me.docKeyword = rec.data.docKeyword;
            me.docFileName = rec.data.name;
        }
        else {
            Ext.Msg.alert("错误信息", "设置签证单出错！");
            return;
        }
        //4.把勾选的文件的属性，修改为正件的属性

        //获取文件编码
        var fileCode = me.fileCodePanel.getFileCode();

        //获取文件ID
        var fileId = me.fileCodePanel.getFileId();

        //获取文件类型代码
        var docIdentifier = me.fileCodePanel.getDocIdentifier();

        //获取工程名称
        var projectName = me.projectNameText.value;

        //施工部位
        var construcSite = me.construcSiteText.value;

        //指令人姓名
        var instrucManName = me.instrucManNameText.value;

        //通知日期
        var noticeDate = me.noticeDateField.value;

        //大小写金额
        var maxMoney=me.MaxMoneyText.value;
        var smallMoney=me.SmallMoneyText.value;

        //编号
        var sendCode = "";


        //来文类型
        //var recType = me.rectypeCombo.value;


        //获取表单数据，转换成JSON字符串
        var docAttr =
        [
            { name: 'fileCode', value: fileCode },
            { name: 'projectName', value: projectName },
            { name: 'construcSite', value: construcSite },
            { name: 'instrucManName', value: instrucManName },
            { name: 'noticeDate', value: noticeDate },
            { name: 'maxMoney', value: maxMoney },
            { name: 'smallMoney', value: smallMoney },
            //{ name: 'sendCode', value: sendCode },
            //{ name: 'recType', value: recType },
            //{ name: 'fileId', value: fileId },
        ];

        var docAttrJson = Ext.JSON.encode(docAttr);
        var cataAttrJson = Ext.JSON.encode(me.fileCodePanel.cataAttrArray);

        Ext.MessageBox.wait("正在生成签证单，请稍候...", "等待");

        Ext.Ajax.request({

            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.HXEPC_Plugins.Document", A: "DraftVisa",
                sid: localStorage.getItem("sid"), ProjectKeyword: me.projectKeyword,
                DocAttrJson: docAttrJson, CataAttrJson: cataAttrJson, DocKeyword: me.docKeyword,
                FileName: me.docFileName
            },
            success: function (response, options) {
                //me.draft_document_callback(response, options, "");//, me.projectKeyword, closeWin);

                //获取数据后，更新窗口
                var res = Ext.JSON.decode(response.responseText, true);
                var state = res.success;
                if (state === true) {

                    Ext.MessageBox.close();//关闭等待对话框

                    var recod = eval(res.data[0]);

                    //处理返回事件
                    //me.draft_document_callback(response, options, "");//, me.projectKeyword, closeWin);


                    me.docKeyword = recod.DocKeyword;//获取联系单文档id
                    //me.fileUploadPanel.docList = recod.DocList;//获取流程文档列表
                    me.newProjectKeyword = recod.ProjectKeyword;//获取新建的目录id

                    me.draft_document_callback(response, options, "");

                    //me.fileUploadPanel.docKeyword = me.docKeyword;

                    //if (me.fileUploadPanel.FileUploadButton.uploader.uploader.files.length > 0) {
                    //    //上传完所有文件后，刷新表单
                    //    me.fileUploadPanel.afterUploadAllFile = function () {
                    //        //me.refreshWin(me.projectKeyword, true);
                    //        me.draft_document_callback(response, options, "");
                    //    };

                    //    me.fileUploadPanel.send_upload_file();
                    //} else {
                    //    //当没有附件时，处理返回事件
                    //    me.draft_document_callback(response, options, "");
                    //}
                } else {
                    var errmsg = res.msg;
                    Ext.Msg.alert("错误信息", errmsg);
                }
            },
            failure: function (response, options) {
                //////Ext.Msg.alert("错误", "连接服务器失败！<br>" + response.responseText);
            }
        })
    },

    //处理发送起草函件后的返回事件
    draft_document_callback: function (response, options) {
        var me = this;

        var sendUnitCode = me.fileCodePanel.getSendCompanyCode();

        //获取审批路径Combo
        //var approvpath = me.approvpathCombo.value;

        Ext.MessageBox.wait("正在启动流程，请稍候...", "等待");

        Ext.Ajax.request({

            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.HXEPC_Plugins.Document", A: "RecognitionStartWorkFlow",
                sid: localStorage.getItem("sid"), docKeyword: me.docKeyword,
                docList: me.fileUploadPanel.docList, //ApprovPath: approvpath,
                UserList: me.nextStateUserList
            },
            success: function (response, options) {

                //获取数据后，更新窗口
                var res = Ext.JSON.decode(response.responseText, true);
                var state = res.success;
                if (state === true) {

                    Ext.MessageBox.close();//关闭等待对话框

                    var recod = eval(res.data[0]);
                    //me.refreshWin(recod.ProjectKeyword, true);
                    me.refreshWin(me.docKeyword, true);

                } else {
                    var errmsg = res.msg;
                    Ext.Msg.alert("错误信息", errmsg);
                    winDraftVisa.close();
                }
            },
            failure: function (response, options) {
                Ext.MessageBox.close();//关闭等待对话框
            }

        })
    },

    //刷新表单，参数:parentKeyword:新建的联系单目录
    refreshWin: function (parentKeyword, closeWin) {
        var me = this;
        var tree = Ext.getCmp(me.mainPanelId).down('treepanel');
        var viewTreeStore = tree.store;

        viewTreeStore.load({
            callback: function (records, options, success) {//添加回调，获取子目录的文件数量
                if (closeWin)
                    winDraftVisa.close();

                //展开目录
                Ext.require('Ext.ux.Common.comm', function () {
                    Ext.getCmp('contentPanel').down('_mainProjectTree').ExpendProject(parentKeyword);
                });
            }
        });
    }
});
