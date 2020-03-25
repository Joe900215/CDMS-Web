
//定义文章内容页主视图
Ext.define('CDMSWap.view.Message.View', {
    extend: 'Ext.container.Container',
    alias: 'widget.messageview',
    //layout: "border",
    layout: 'fit',
    //views: ['contextMenu'],

    initComponent: function () {
        var me = this;
 
        me.renderTo = me.el;

        //将树和Grid放到容器里

        me._mainMessageView = Ext.create('Ext.ux.gcz.Message._MainMessageView');

        me.items = [
        me._mainMessageView
        			//{
        			//    xtype: "button",
        			//    text: "消息列表"
        			//}
        ];

        me.callParent(arguments);
    }

});