/*定义数据源视图面板*/
Ext.define('Ext.ux.YDForm.WorkflowPlace._MainWorkflowPlaceView', {
    extend: 'Ext.panel.Panel',
    alias: 'widget._mainWorkflowPlaceView',
    layout: "border",
    //layout:"fit",
    //SourceViewType: "1",
    initComponent: function () {
        var me = this;
        me.renderTo = me.el;

        //////定义消息内容控件
        me.mainworkflowplacecontent = Ext.create('Ext.ux.YDForm.WorkflowPlace._MainWorkflowPlaceContent');

        //////定义消息列表
        me._mainworkflowplacegrid = Ext.create('Ext.ux.YDForm.WorkflowPlace._MainWorkflowPlaceGrid');

        //////定义目录树
        me._mainworkflowplacetree = Ext.create('Ext.ux.YDForm.WorkflowPlace._MainWorkflowPlaceTree');

        //添加属性TAB页面到容器
        me.items = [
            			//{
            			//    xtype:"button",
            			//    text:"我的按钮"
            			//}
                                 me._mainworkflowplacetree,
                                 me._mainworkflowplacegrid,
                                 me.mainworkflowplacecontent
        ];

        me.callParent(arguments);
    },
    //setSourceViewType: function (projectType) {
    //    var me = this;
    //    me._mainprojecttree.setSourceViewType(projectType);
    //}
}
);