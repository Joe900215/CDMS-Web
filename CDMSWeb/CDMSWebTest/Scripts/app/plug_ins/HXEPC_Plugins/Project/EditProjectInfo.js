Ext.define('Ext.plug_ins.HXEPC_Plugins.Project.EditProjectInfo', {
    extend: 'Ext.container.Container',
    alias: 'widget.editProjectInfo',
    layout: 'fit',
    resultvalue: '', mainPanelId: '',
    projectKeyword: '',
    initComponent: function () {
        var me = this;

        //定义用户选择Tab
        me.editProjectInfoTab = Ext.create('Ext.plug_ins.HXEPC_Plugins.Project.EditProjectInfoTab', { projectKeywor: me.projectKeyword });

        me.editProjectInfoTab.editCrewPanel.setProjectKeyword(me.projectKeyword);
        me.editProjectInfoTab.editCrewPanel.sendEditCrewDefault();

        me.editProjectInfoTab.editFactoryPanel.setProjectKeyword(me.projectKeyword);
        me.editProjectInfoTab.editSystemPanel.setProjectKeyword(me.projectKeyword);
        me.editProjectInfoTab.editContractPanel.setProjectKeyword(me.projectKeyword);

        //添加列表
        me.items = [
            me.editProjectInfoTab
            //{
            //    xtype: 'button',
            //    text: "创建目录"
            //}
        ];
        me.callParent(arguments);
    },
});