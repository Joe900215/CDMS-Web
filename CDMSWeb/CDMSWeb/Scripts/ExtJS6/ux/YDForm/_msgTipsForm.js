Ext.define('Ext.ux.YDForm._MsgTipsForm', {
    extend: 'Ext.container.Container',
    alias: 'widget._MsgTipsForm',
    layout: 'fit',
    resultvalue: '',

    initComponent: function () {
        var me = this;
        me.renderTo = me.el;
        me.msgCount = 0;


        me.countLabel = Ext.widget('label', {
                            //height: 10,
                            width: 100,
                            flex: 1,
                            margins: "15 15 5 15"
                        });

        me.titleLabel = Ext.widget('label', {
                    //height: 25,
                    width: 100,
                    flex: 3,
                    labelAlign: "right",      //靠右
                    labelWidth: 70 ,         //  label宽度
                    margins: "15 15 5 15"
        });
        me.contentLabel = Ext.widget('label', {
            //height: 30,
            width: 100,
            margins: "15 15 10 15",
            flex: 8
            ,//给整个消息框添加点击事件
            listeners : {
            render : function() {//渲染后添加click事件
                Ext.fly(this.el).on('click',
                  function (e, t) {

                      var me = this;
                      msgTipsWin.hideTips();
                      var mpanel = Ext.getCmp('mainPanel');
                      //跳转到消息页
                      mpanel.setActiveTab(1);
                  });
            },
            scope : this
            }

        });

        me.btnPlan = Ext.widget('panel', {
            layout: "hbox",
            height: 35,
            baseCls: 'my-panel-no-border',//隐藏边框
            items: [
                {
                    baseCls: 'my-panel-no-border',//隐藏边框
                    flex: 1
                },
                {
                    xtype: "button",
                    text: "查看", width: 60, margins: "5 15 5 5",
                    listeners: {
                        "click": function (btn, e, eOpts) {//添加点击按钮事件
                           // Ext.Msg.alert("您展开了目录树节点！！！", "您展开了目录树节点！节点ID:");
                        }
                    }
                }
            ]
        });

        me.msgContentPlan = {
            layout: {
                type: 'vbox',
                pack: 'start',
                align: 'stretch'
            },
            baseCls: 'my-panel-no-border',//隐藏边框
            items: [
               me.countLabel,
               me.titleLabel,
               me.contentLabel,
            ],flex:1
        };

        me.msgTipsPlan = {
            layout: {
                type: 'vbox',
                pack: 'start',
                align: 'stretch'
            },
            baseCls: 'my-panel-no-border',//隐藏边框
            items: [
               me.msgContentPlan
             //me.btnPlan
            ]
        };



        //添加列表
        me.items = [
                        //{
            			//    xtype:"button",
            			//    text:"我的按钮"
                        //}
                        me.msgTipsPlan
        ];

        me.callParent(arguments);
    }
});