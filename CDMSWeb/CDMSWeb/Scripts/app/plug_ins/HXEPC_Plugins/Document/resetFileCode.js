//选择专业
Ext.define('Ext.plug_ins.HXEPC_Plugins.Document.resetFileCode', {
    extend: 'Ext.container.Container',
    alias: 'widget.resetFileCode',
    //layout: "border",
    layout: 'fit',
    resultvalue: '', mainPanelId: '', projectDesc: '',
    projectKeyword: '', docKeyword: '',workFlowKeyword:'',
    //reSelect: 'false',//是否重新选择专业
    initComponent: function () {
        var me = this;

        me.ifGotoNextState = true;

        //定义主送Text
        me.mainFeederText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", 
            fieldLabel: "主送", anchor: "80%", labelWidth: 60, labelAlign: "right", labelPad: 8, width: "50%",//width: 230, 
            margin: '10 10 0 10', fieldStyle: 'border-color: red; background-image: none;'//红色边框//flex: 1
        });

        //定义抄送Text
        me.copyPartyText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "抄送", anchor: "80%", labelWidth: 60, labelAlign: "right", labelPad: 8, width: "50%",//width: 230, 
            margin: '10 10 0 10', fieldStyle: ' background-image: none;'//红色边框//flex: 1
        });

        //定义发送方Text
        me.senderText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "发自", anchor: "80%", labelWidth: 60, labelAlign: "right", labelPad: 8, width: "50%",//width: 230, 
            margin: '10 10 0 10', fieldStyle: 'border-color: red; background-image: none;'//红色边框//flex: 1
        });

        //定义发文编码Text
        me.sendCodeText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "发文编码", anchor: "80%", labelWidth: 60, readOnly: true,
            labelAlign: "right", labelPad: 8, width: "50%",//width: 230, 
            margin: '60 10 60 10', fieldStyle: 'border-color: red; background-image: none;'//红色边框//flex: 1
        });

        //定义发送日期Text
        me.sendDateField = Ext.create("Ext.form.field.Date", {

            name: "date",
            fieldLabel: ' 发送日期', fieldStyle: ' background-image: none;',
            editable: true, labelWidth: 60, margin: '10 10 0 10',
            labelAlign: "right", labelPad: 8,
            emptyText: "--请选择--", autoLoadOnValue: true,
            format: 'Y年m月d日',
            value: new Date(),
            width: '50%'//width: 230
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
                            me.send_resetFileCode();
                        }
                    }
                },
                {
                    xtype: "button",
                    text: "取消", width: 60, margins: "10 15 10 5",
                    listeners: {
                        "click": function (btn, e, eOpts) {//添加点击按钮事件

                            winResetFileCode.close();
                        }
                    }
                }
            ]
        });

        me.items = [
          Ext.widget('form', {
              layout: "form",
              items: [

                  {
                      xtype: "panel",
                      baseCls: 'my-panel-no-border',//隐藏边框
                      layout: {
                          type: 'vbox',
                          pack: 'start',
                          align: 'stretch'
                      },
                      items: [
                          me.sendCodeText
                          //,
                          //me.mainFeederText,
                          //me.copyPartyText,
                          //me.senderText,
                          //me.sendDateField
                      ]
                  },
                  me.bottomButtonPanel
              ]
          })]

        me.callParent(arguments);
    },

    //向服务器发送文控填写发文信息请求
    send_resetFileCode: function () {
        var me = this;

        //获取主送Text
        var mainFeeder = me.mainFeederText.value;

        //获取抄送Text
        var copyParty = me.copyPartyText.value;

        //获取发送方Text
        var sender = me.senderText.value;

        //获取发文编码Text
        var sendCode = me.sendCodeText.value;

        //获取发送日期
        var sendDate = me.sendDateField.value;

        //获取表单数据，转换成JSON字符串
        var docAttr =
        [
            { name: 'mainFeeder', value: mainFeeder },
            { name: 'copyParty', value: copyParty },
            { name: 'sender', value: sender },
            { name: 'sendCode', value: sendCode },
            { name: 'sendDate', value: sendDate }
        ];

        var docAttrJson = Ext.JSON.encode(docAttr);

        Ext.MessageBox.wait("正在填写发文信息，请稍候...", "等待");

        Ext.Ajax.request({

            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.HXEPC_Plugins.Document", A: "ResetFileCode",
                sid: localStorage.getItem("sid"), DocKeyword: me.docKeyword,
                DocAttrJson: docAttrJson
            },
            success: function (response, options) {
                me.send_resetFileCode_callback(response, options, "");//, me.projectKeyword, closeWin);

                ////获取数据后，更新窗口
                //var res = Ext.JSON.decode(response.responseText, true);
                //var state = res.success;
                //if (state === true) {

                //    Ext.MessageBox.close();//关闭等待对话框

                //}
            },
            failure: function (response, options) {
              
                Ext.MessageBox.close();//关闭等待对话框
            }
        });

    },

    send_resetFileCode_callback: function (response, options) {
        var me = this;

        //获取数据后，更新窗口
        var res = Ext.JSON.decode(response.responseText, true);
        var state = res.success;
        if (state === true) {
            var recod = eval(res.data[0]);
            me.workFlowKeyword = recod.WorkFlowKeyword;
            //Ext.MessageBox.close();//关闭等待对话框

            //Ext.MessageBox.show({
            //    title: '',
            //    msg: '成功填写信函文档信息，是否发出文档？',
            //    buttons: Ext.MessageBox.YESNO,
            //    buttonText: {
            //        yes: "是",
            //        no: "否"
            //    },
            //    fn: function (btn, parentFuctionName) {
            //        if (btn === "yes") {

                        winResetFileCode.close();

                        if (me.ifGotoNextState === true) {
                            //跳转到下一状态
                            Ext.require('Ext.ux.Common.comm', function () {
                                GotoNextWorkflowState("TORECUNIT", me.workFlowKeyword, "", function () {
                                    //回调函数，通过流程分支
                                    me.refreshWin(me.docKeyword, false);
                                });
                            })
                        } else {
                            me.refreshWin(me.docKeyword, false);
                        }
                    //} else {
                        
                    //    me.refreshWin(me.docKeyword, true);
                    //    //强制刷新整个网页
                    //    //document.location.reload();
                    //}
               // }
            //});

        } else {
            var errmsg = res.msg;
            Ext.Msg.alert("错误信息", errmsg);
            winResetFileCode.close();
        }
    },

    //刷新表单，参数:parentKeyword:新建的联系单目录
    refreshWin: function (docKeyword, closeWin) {
        var me = this;

        //调用流程页事件，刷新父控件内容
        Ext.getCmp(me.mainPanelId).refreshMainPanle(docKeyword, function () {
            if (closeWin)
                winResetFileCode.close();
        });

        ////var tree = Ext.getCmp(me.mainPanelId);//.down('treepanel');
        //var tree = Ext.getCmp(me.mainPanelId).up('_mainSourceView').down('_mainProjectTree').down('treepanel');//.mainprojecttree;//获取目录树控件
        //var viewTreeStore = tree.store;

        //viewTreeStore.load({
        //    callback: function (records, options, success) {//添加回调，获取子目录的文件数量
        //        if (closeWin)
        //            winResetFileCode.close();

        //        //展开目录
        //        Ext.require('Ext.ux.Common.comm', function () {
        //            Ext.getCmp('contentPanel').down('_mainProjectTree').ExpendProject(docKeyword);
        //        });
        //    }
        //});
    }
});