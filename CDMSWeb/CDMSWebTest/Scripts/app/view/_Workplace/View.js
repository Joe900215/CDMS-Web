Ext.require(['Ext.ux.upload.plugin.Window']);
//定义个人工作台主视图
Ext.define('CDMSWeb.view._Workplace.View', {
    extend: 'Ext.container.Container',
    alias: 'widget._workplaceview',
    //layout: "border",
    layout: 'fit',
    initComponent: function () {
        var me = this;

        //定义目录类型
        //me._mainSourceView.setSourceViewType("4");
        window.SourceViewType = "5";

        localStorage.setItem("SourceViewTabType", EnSourceViewTabType.WorkPlace);    //2,第2个Tab

        //将树和Grid放到容器里

        me._mainSourceView = Ext.create('Ext.ux.YDForm._MainSourceView');

        me.items = [me._mainSourceView];

        me.callParent(arguments);
    }

});