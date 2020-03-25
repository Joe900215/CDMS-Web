Ext.require(['Ext.ux.upload.plugin.Window']);
//定义文章内容页主视图
Ext.define('CDMSWeb.view._Navigation.View', {
    extend: 'Ext.container.Container',
    alias: 'widget._navigationview',
    //layout: "border",
    layout: 'fit',
    initComponent: function () {
        var me = this;

        //定义目录类型
        //me._mainSourceView.setSourceViewType("4");
        //window.SourceViewType = "8";

        localStorage.setItem("SourceViewTabType", EnSourceViewTabType.Navigation);    //5,第5个Tab

        //将树和Grid放到容器里

        me._mainNavigationView = Ext.create('Ext.ux.YDForm.Navigation._MainNavigationView');

        me.items = [

               me._mainNavigationView
        ];

        me.callParent(arguments);
    }

});