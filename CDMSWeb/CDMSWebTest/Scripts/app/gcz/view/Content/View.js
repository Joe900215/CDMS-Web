Ext.require(['Ext.ux.upload.plugin.Window']);
//定义文章内容页主视图
Ext.define('CDMSWap.view.Content.View', {
    extend: 'Ext.container.Container',
    alias: 'widget.contentview',
    //layout: "border",
    layout: 'fit',
    initComponent: function () {
        var me = this;
        me.renderTo = me.el;
        //定义目录类型
        window.SourceViewType = "1";

        //将树和Grid放到容器里

        me._mainProjectView = Ext.create('Ext.ux.gcz.Project._MainProjectView');

        me.items = [
            me._mainProjectView
        ];

        me.callParent(arguments);
    }

});