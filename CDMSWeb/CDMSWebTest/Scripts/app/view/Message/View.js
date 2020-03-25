
//定义文章内容页主视图
Ext.define('CDMSWeb.view.Message.View', {
    extend: 'Ext.container.Container',
    alias: 'widget.messageview',
    //layout: "border",
    layout: 'fit',
    //views: ['contextMenu'],

    initComponent: function () {
        var me = this;
 

        localStorage.setItem("SourceViewTabType", EnSourceViewTabType.Message);    //1,第1个Tab

        //将树和Grid放到容器里

        me._mainMessageView = Ext.create('Ext.ux.YDForm.Message._MainMessageView');

        me.items = [
        me._mainMessageView];

        me.callParent(arguments);
    }

});