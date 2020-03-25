Ext.define('Ext.ux.m.WorkFlow._ModiAuditDetail', {
    //编辑流程意见
    extend: 'Ext.container.Container',
    alias: 'widget._modiAuditDetail',
    layout: 'fit',
    resultvalue: '',
    mainAuditGridId : '',
    AuditType: '',
    AuditRight: '',
    margin: '5 5 5 5',

    initComponent: function () {
        var me = this;
        me.renderTo = me.el;
        var resultvalue = '';

        me.WorkFlowKeyword = "";
        me.WorkStateKeyword = "";
        me.CheckerKeyword = "";
        //me.WorkStateDesc = "";
        me.CheckDate = "";
        //me.AuditType = "";
        //me.AuditRight = "";

        //添加意见Text
        me.AuditText = Ext.create("Ext.form.field.TextArea", {
            xtype: "textarea", anchor: "80%", labelWidth: 60, labelAlign: "left", margin: '0 0 0 0', width: 270,
            height: "100%",value: "a"//400
        });


        //添加修改意见Text
        me.AmendmentText = Ext.create("Ext.form.field.TextArea", {
            xtype: "textarea", anchor: "80%", labelWidth: 60, labelAlign: "left", margin: '0 0 0 0', width: 270,
            height: "100%"//400 
        });

        me.AuditFieldset = new Ext.form.FieldSet({
            title: "意见", width: '100%', layout: 'fit', margin: '0 0 5 0',
            items: [
                me.AuditText
            ], flex: 1
        });

        var AmendmentFieldsetHide = false;

        //当新建意见或审核人编辑意见时，隐藏设计人修改意见栏
        if (me.AuditType === "NewProcAudit" || (me.AuditType === "ProcAudit" && me.AuditRight === "1")) {
            AmendmentFieldsetHide = true;
        }

        me.AmendmentFieldset = new Ext.form.FieldSet({
            title: "修改意见", width: '100%', layout: 'fit', margin: '0 0 5 0',
            hidden: AmendmentFieldsetHide,
            items: [
                me.AmendmentText
            ], flex: 1
        });

        me.AuditPlan = {
            xtype: "panel",
            layout: {
                type: 'vbox',
                pack: 'start',
                align: 'stretch'
            },
            baseCls: 'my-panel-no-border',//隐藏边框
            items: [

                me.AuditFieldset,

                me.AmendmentFieldset,
                {
                    xtype: "panel",
                    layout: "hbox",
                    baseCls: 'my-panel-no-border',//隐藏边框
                    height: 28,
                    //margins: "0 15 0 0",
                    items: [

                              {
                                  flex: 1, baseCls: 'my-panel-no-border'//隐藏边框
                              },
                              {
                                  xtype: "button",
                                  text: "保存", width: 80, margins: "0 5 10 5",
                                  listeners: {
                                      "click": function (btn, e, eOpts) {//添加点击按钮事件
                                          me.saveAuditBtnClick();
                                      }
                                  }
                              }, {
                                  xtype: "button",
                                  text: "语音输入", width: 80, margins: "0 5 10 5",
                                  listeners: {
                                      "click": function (btn, e, eOpts) {//添加点击按钮事件
                                          me.voiceBtnClick();
                                      }
                                  }
                              }, {
                                  xtype: "button",
                                  text: "取消", width: 80, margins: "0 15 10 5",
                                  listeners: {
                                      "click": function (btn, e, eOpts) {//添加点击按钮事件
                                          _winModiAuditDetail.close();
                                      }
                                  }
                              }
                    ]
                }
            ]
        };

        //添加属性TAB页面到容器
        me.items = [
                        me.AuditPlan
        ];

        me.callParent(arguments);
    },

    //第一次打开时初始化窗口
    //WorkStateKeyword:对象的关键字，ProcAudit：原有的意见，DeProcAudit：原有的修改意见
    //AuditType：意见类型，是属于新建审核人意见、修改审核人意见还是 设计人添加修改意见
    loadWin: function (WorkFlowKeyword, WorkStateKeyword, CheckerKeyword, CheckDate, ProcAudit, DeProcAudit, AuditRight, AuditType) {
        var me = this;

        me.WorkFlowKeyword = WorkFlowKeyword;
        me.WorkStateKeyword = WorkStateKeyword;

        me.CheckerKeyword = CheckerKeyword;
        //me.WorkStateDesc = WorkStateDesc;
        me.CheckDate = CheckDate;
        me.AuditType = AuditType;
        me.AuditRight = AuditRight;

        me.AuditText.setValue(ProcAudit);
        me.AmendmentText.setValue(DeProcAudit);
        //Ext.Msg.alert("错误信息", ProcAudit+","+DeProcAudit);

        //如果是新建审核人意见，就不需要访问服务器获取权限
        if (AuditType === "NewProcAudit") {
            me.AuditText.setReadOnly(false);
            me.AmendmentText.setReadOnly(true);
            me.AuditText.focus(false, 200);
        }
        //如果是修改审核人意见
        //else if (AuditType === "ProcAudit" && AuditRight==="1")
        else if (AuditRight === "1")
        {
            me.AuditText.setReadOnly(false);
            me.AmendmentText.setReadOnly(true);
            me.AuditText.focus(false, 200);
        }
        //如果是填写修改意见
        //else if (AuditType === "DeProcAudit" && AuditRight === "2") {
        else if (AuditRight === "2") {
            me.AuditText.setReadOnly(true);
            me.AmendmentText.setReadOnly(false);

            me.AmendmentText.focus(false, 200);
        } else {//if (AuditType === "ReadOnly") {
            me.AuditText.setReadOnly(true);
            me.AmendmentText.setReadOnly(true);

        }
    },

    //添加意见菜单点击事件，添加流程意见
    saveAuditBtnClick: function () {
        var me = this;

        if (me.AuditText.value === "") {
            Ext.Msg.alert("错误信息", "请输入意见！");
            return;
        }

        if (me.WorkFlowKeyword != "" && me.WorkStateKeyword === "") {
            //如果是新建意见
            me.addNewAudit();
            return;
        }

        var WorkStateKeyword = me.WorkStateKeyword;
        var auditeDesc = me.AuditText.value;

        Ext.Ajax.request({
            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.WebApi.WorkFlowController", A: "ModiWorkflowAudit",
                WorkStateKeyword: WorkStateKeyword, CheckerKeyword: me.CheckerKeyword,
                CheckDate: me.CheckDate, ProcAudit: me.AuditText.value,
                DeProcAudit: me.AmendmentText.value, sid: localStorage.getItem("sid")
            },
            success: function (response, options) {
                var res = Ext.JSON.decode(response.responseText, true);
                var state = res.success;
                if (state === false) {
                    var errmsg = res.msg;
                    Ext.Msg.alert("错误信息", errmsg);
                }
                else {
                    var grid = Ext.getCmp(me.mainAuditGridId);//Ext.getCmp('_DocGrid');
                    grid.store.load();

                    _winModiAuditDetail.close();
                }
            }
        });
    },

    voiceBtnClick: function () {
		//Ext.Msg.alert("浏览器类型", localStorage.getItem("_browserName"));
		
		var me=this;
        //如果是安卓的浏览器，就弹出语音识别窗口
        if (localStorage.getItem("_browserName") == "AndroidWebView") {
            //Ext.Msg.alert("浏览器类型", localStorage.getItem("_browserName"));
            window.android.showVoiceFm(me.id);
			//window.android.showVoiceFm();
        }
    },


    //添加新的意见
    addNewAudit: function () {
        var me = this;
        //var WorkStateKeyword = me.WorkStateKeyword;
        var auditeDesc = me.AuditText.value;

        Ext.Ajax.request({
            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.WebApi.WorkFlowController", A: "AddWorkflowAudit",
                Keyword: me.WorkFlowKeyword, ProcAudit: auditeDesc,
                sid: localStorage.getItem("sid")
            },
            success: function (response, options) {
                var res = Ext.JSON.decode(response.responseText, true);
                var state = res.success;
                if (state === false) {
                    var errmsg = res.msg;
                    Ext.Msg.alert("错误信息", errmsg);
                }
                else {
                    var grid = Ext.getCmp(me.mainAuditGridId);//Ext.getCmp('_DocGrid');
                    grid.store.load();

                    _winModiAuditDetail.close();
                }
            }
        });
    },
	
	//添加语音识别功能，转换成文字填写到意见栏
	setVoiceToAudit: function (msg) {
		var me = this;
	
		//me.AuditText.setValue(msg);
		me.AuditText.setValue(me.AuditText.getValue()+msg);
	}
	
});

//外部JS调用Extjs函数
Ext.setVoiceToAudit = function (auditPanelId, msg) {
    //alert("接收到刷新目录树消息！请手动刷新目录树!!");
	//alert(auditPanelId+","+msg);

    var view = Ext.getCmp(auditPanelId);
    view.setVoiceToAudit(msg);
}