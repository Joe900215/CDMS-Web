/*定义数据源视图面板*/
Ext.define('Ext.ux.gcz.Project._MainProjectView', {
    extend: 'Ext.panel.Panel',
    alias: 'widget._mainProjectView',
    layout: "border",
    //layout:"fit",
    //SourceViewType: "1",
    initComponent: function () {
        var me = this;
        me.renderTo = me.el;
        //me.renderTo = Ext.getBody();

        ////定义目录树
        me._mainprojecttree = Ext.create('Ext.ux.gcz.Project._MainProjectTree');

        ////定义文档列表
        me._maindocgrid = Ext.create('Ext.ux.gcz.Doc._MainDocGrid');

        ////定义消息内容控件
        //me.mainmessagecontent = Ext.create('Ext.ux.m.Message._MainMessageContent');

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
                                    me._mainprojecttree,
                                    me._maindocgrid
                                    //me._mainmessagegrid,
                                    //me.mainmessagecontent
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