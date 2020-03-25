Ext.require(['Ext.ux.upload.plugin.Window']);
//定义个人工作台主视图
Ext.define('CDMSWeb.view._WorkflowPlace.View', {
    extend: 'Ext.container.Container',
    alias: 'widget._workflowplaceview',
    //layout: "border",
    layout: 'fit',
    initComponent: function () {
        var me = this;

        //定义目录类型
        //me._mainSourceView.setSourceViewType("4");
        window.SourceViewType = "7";

        localStorage.setItem("SourceViewTabType", EnSourceViewTabType.WorkPlace);    //2,第2个Tab

        //将树和Grid放到容器里

        me._mainWorkflowPlaceView = Ext.create('Ext.ux.YDForm.WorkflowPlace._MainWorkflowPlaceView');


        me.items = [
            me._mainWorkflowPlaceView
        ];

        me.callParent(arguments);
    }

});