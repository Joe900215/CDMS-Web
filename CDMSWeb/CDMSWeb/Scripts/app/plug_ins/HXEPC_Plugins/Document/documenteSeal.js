//选择专业
Ext.define('Ext.plug_ins.HXEPC_Plugins.Document.documenteSeal', {
    extend: 'Ext.container.Container',
    alias: 'widget.documenteSeal',
    //layout: "border",
    layout: 'fit',
    resultvalue: '', mainPanelId: '', projectDesc: '',
    projectKeyword: '', docKeyword: '', workFlowKeyword: '',
    //reSelect: 'false',//是否重新选择专业
    initComponent: function () {
        var me = this;

        //底部按钮区域
        me.bottomButtonPanel = Ext.create("Ext.panel.Panel", {
            //xtype: "panel",
            layout: "hbox",
            baseCls: 'my-panel-no-border',//隐藏边框
            //align: 'right',
            //pack: 'end',//组件在容器右边
            items: [{
                flex: 1, baseCls: 'my-panel-no-border'//隐藏边框
            }, {
                xtype: "button",
                text: "盖章", width: 60, margins: "10 5 20 5",
                listeners: {
                    "click": function (btn, e, eOpts) {//添加点击按钮事件
                        me.send_documenteSeal("true");
                    }
                }
            }, {
                xtype: "button",
                text: "不盖章", width: 60, margins: "10 5 20 5",
                listeners: {
                    "click": function (btn, e, eOpts) {//添加点击按钮事件
                        me.send_documenteSeal("false");
                    }
                }
            }
                ,
                {
                    xtype: "button",
                    text: "取消", width: 60, margins: "10 15 20 5",
                    listeners: {
                        "click": function (btn, e, eOpts) {//添加点击按钮事件

                            winDocumenteSeal.close();
                        }
                    }
                }
                            		, {
                            		    flex: 1, baseCls: 'my-panel-no-border'//隐藏边框
                            		}

            ]

        });
        //底部按钮区域
        me.topEditPanel = Ext.create("Ext.panel.Panel", {
            //xtype: "panel",
            layout: "hbox",
            baseCls: 'my-panel-no-border',//隐藏边框
            //align: 'right',
            //pack: 'end',//组件在容器右边
            items: [{
                flex: 1, baseCls: 'my-panel-no-border'//隐藏边框
            }, {
                xtype: "label", margins: "50 30 20 30",
                text: "请选择是否盖章。"
            }
                , {
                    flex: 1, baseCls: 'my-panel-no-border'//隐藏边框
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
                    me.topEditPanel,

                      ]
                  }, me.bottomButtonPanel
                  ]
              })
        ];

        me.callParent(arguments);
    },

    //向服务器发送文控填写发文信息请求
    send_documenteSeal: function (isSeal) {
        var me = this;

        Ext.MessageBox.wait("正在提交，请稍候...", "等待");

        Ext.Ajax.request({

            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.HXEPC_Plugins.Document", A: "DocumenteSeal",
                sid: localStorage.getItem("sid"), DocKeyword: me.docKeyword,
                isSeal: isSeal
            },
            success: function (response, options) {
                me.send_documenteSeal_callback(response, options, "");//, me.projectKeyword, closeWin);

            },
            failure: function (response, options) {

                Ext.MessageBox.close();//关闭等待对话框
            }
        });

    },
    send_documenteSeal_callback: function (response, options) {
        var me = this;

        //获取数据后，更新窗口
        var res = Ext.JSON.decode(response.responseText, true);
        var state = res.success;
        if (state === true) {
            var recod = eval(res.data[0]);
            me.workFlowKeyword = recod.WorkFlowKeyword;

            winDocumenteSeal.close();

            //跳转到下一状态
            Ext.require('Ext.ux.Common.comm', function () {
                GotoNextWorkflowState("TOSECRE", me.workFlowKeyword, "", function () {
                    //回调函数，通过流程分支
                    //me.refreshWin(me.docKeyword, false);
                    me.refreshWin(me.projectKeyword, false);
                });
            })


        } else {
            var errmsg = res.msg;
            Ext.Msg.alert("错误信息", errmsg);
            winDocumenteSeal.close();
        }
    },

    //刷新表单，参数:parentKeyword:新建的联系单目录
    refreshWin: function (parentKeyword, closeWin) {
        var me = this;

        var mpanel = Ext.getCmp(me.mainPanelId);
        mpanel.refreshMainPanle(parentKeyword, function () { });

        //var tree = Ext.getCmp(me.mainPanelId).down('treepanel');
        //var tree = Ext.getCmp(me.mainPanelId).up('_mainSourceView').down('_mainProjectTree').down('treepanel');

        //var mpanel = Ext.getCmp(me.mainPanelId);
        //var msPanle = mpanel.up('_mainSourceView');
        //var tree;
        //if (msPanle != undefined) {
        //    tree = mpanel.up('_mainSourceView').down('_mainProjectTree').down('treepanel');
        //    var viewTreeStore = tree.store;

        //    viewTreeStore.load({
        //        callback: function (records, options, success) {//添加回调，获取子目录的文件数量
        //            //if (closeWin)
        //            //    winDraftLetterCN.close();

        //            //展开目录
        //            Ext.require('Ext.ux.Common.comm', function () {
        //                Ext.getCmp('contentPanel').down('_mainProjectTree').ExpendProject(parentKeyword);
        //            });

        //        }
        //    });

        //}
        //else {
        //    tree = mpanel.up("_mainMessageView").down("_mainMessageGrid");

        //    gridView.refreshMessagePage();
        //}



    }
});