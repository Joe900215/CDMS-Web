Ext.require(['Ext.ux.upload.plugin.Window']);
//定义文章内容页主视图
Ext.define('CDMSWeb.view._LogicProject.View', {
    extend: 'Ext.container.Container',
    alias: 'widget._logicprojectview',
    //layout: "border",
    layout: 'fit',
    initComponent: function () {
        var me = this;

        //定义目录类型
        //me._mainSourceView.setSourceViewType("4");
        window.SourceViewType = "4";
        
        localStorage.setItem("SourceViewTabType", EnSourceViewTabType.LogicProject);    //3,第3个Tab

        //将树和Grid放到容器里

        me._mainSourceView = Ext.create('Ext.ux.YDForm._MainSourceView');

        me.items = [me._mainSourceView];

        me.callParent(arguments);
    }

});