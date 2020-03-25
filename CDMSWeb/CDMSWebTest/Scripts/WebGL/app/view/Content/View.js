//定义文章内容页主视图
Ext.define('WebGL.view.Content.View', {
    extend: 'Ext.container.Container',
    alias: 'widget.contentview',
    //layout: "border",
    layout: 'fit',
    initComponent: function () {
        var me = this;


        //定义目录类型
        //window.SourceViewType = "1";

        //将树和Grid放到容器里

        //me._mainSourceView = Ext.create('Ext.ux.YDForm._MainSourceView');
        var mainPanelId = "";
        var docKeyword =  "." + CDMSWeb.RequestInfo.BIMDocKeyword;//"SHEPCP119792D336683";
        me.fmBIMView = Ext.create('Ext.plug_ins.ZHEPC_Plugins.winBIMView', { title: "", mainPanelId: mainPanelId, docKeyword: docKeyword });

        me.items = [
            me.fmBIMView
        //{
            //xtype: "button",
            //text: "我的按钮"
            
        //}
        ];

        me.callParent(arguments);
    }

});