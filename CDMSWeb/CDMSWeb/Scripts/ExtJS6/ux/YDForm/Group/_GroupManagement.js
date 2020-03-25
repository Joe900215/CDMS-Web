Ext.define('Ext.ux.YDForm.Group._GroupManagement', {
    extend: 'Ext.container.Container',
    alias: 'widget._GroupManagement',
    layout: 'fit',
    resultvalue: '',

    initComponent: function () {
        var me = this;

        me._UserGroupPanel = Ext.create('Ext.ux.YDForm.Group._UserControlTabControlGroup');

        //添加属性TAB页面
        me.maintab = Ext.create('Ext.tab.Panel', {
            xtype: 'tabpanel',
            activeTab: 0,
            defaults: {
                border: false,
                bodyPadding: 5, bodyStyle: "background:#DFE9F6"
            },
            items: [
                {
                    title: '用户组',
                    //xtype: 'grid',
                    layout: 'fit',
                    xtype: 'panel',
                    items: me._UserGroupPanel
                }
            ]
        });

        //添加列表
        me.items = [
            			//{
            			//    xtype: "button",
            			//    text: "我的按钮"
            			//}
                        //me.maintab
                        me._UserGroupPanel
        ];

        me.callParent(arguments);
    }
});