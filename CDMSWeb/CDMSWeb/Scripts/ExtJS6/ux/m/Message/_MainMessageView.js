/*定义数据源视图面板*/
Ext.define('Ext.ux.m.Message._MainMessageView', {
    extend: 'Ext.panel.Panel',
    alias: 'widget._mainMessageView', 
    layout: "border",
    //layout:"fit",
    //SourceViewType: "1",
    initComponent: function () {
        var me = this;
        me.renderTo = me.el;
        //me.renderTo = Ext.getBody();

        ////定义目录树
        //me._mainmessagetree = Ext.create('Ext.ux.YDForm.Message._MainMessageTree');

        ////定义消息列表
        me._mainmessagegrid = Ext.create('Ext.ux.m.Message._MainMessageGrid');

        ////定义消息内容控件
        me.mainmessagecontent = Ext.create('Ext.ux.m.Message._MainMessageContent');

        //添加属性TAB页面到容器
        me.items = [
            			//{
            			//    xtype:"button",
            			//    text:"我的按钮"
            			//},
                {
                    region: "center", layout: 'fit',
                    items: [

                            {
                               // xtype: "panel",
                                layout: {
                                    type: 'vbox',
                                    align: 'stretch',
                                    pack: 'start'
                                },
                                items: [
                                    me._mainmessagegrid,
                                    me.mainmessagecontent
                                ]
                            }

                    ]
                }
        ];

      //  me._mainmessagegrid.mainmessagegrid.store.load();

        me.callParent(arguments);
    }

}
);