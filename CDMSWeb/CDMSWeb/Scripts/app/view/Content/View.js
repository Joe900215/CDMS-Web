Ext.require(['Ext.ux.upload.plugin.Window']);
//定义文章内容页主视图
Ext.define('CDMSWeb.view.Content.View', {
    extend: 'Ext.container.Container',
    alias: 'widget.contentview',
    //layout: "border",
    layout: 'fit',
    initComponent: function () {
        var me = this;
        //定义目录类型
        window.SourceViewType = "1";

        localStorage.setItem("SourceViewTabType", EnSourceViewTabType.Local);    //0,第0个Tab

        //将树和Grid放到容器里

        me._mainSourceView = Ext.create('Ext.ux.YDForm._MainSourceView');

        me.items = [
        me._mainSourceView];

        me.callParent(arguments);
    }

});