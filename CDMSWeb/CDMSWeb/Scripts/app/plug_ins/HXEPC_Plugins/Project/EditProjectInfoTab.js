Ext.define('Ext.plug_ins.HXEPC_Plugins.Project.EditProjectInfoTab', {
    //extend: 'Ext.container.Container',
    extend: 'Ext.tab.Panel',//'Ext.panel.Panel',
    alias: 'widget.EditProjectInfoTab', // 此类的xtype类型为buttontransparent  
    selectAllUser: "",
    layout: 'fit',
    layout: "hbox",
    width: 400,
    height: '100%', projectKeyword: '',
    initComponent: function () {
        var me = this;
        me.renderTo = me.el;

        //定义用户选择Tab
        me.editCrewPanel = Ext.create('Ext.plug_ins.HXEPC_Plugins.Project.EditCrewPanel', {projectKeywor:me.projectKeyword});

        //定义用户选择Tab
        me.editFactoryPanel = Ext.create('Ext.plug_ins.HXEPC_Plugins.Project.EditFactoryPanel', { projectKeywor: me.projectKeyword });

        //定义用户选择Tab
        me.editSystemPanel = Ext.create('Ext.plug_ins.HXEPC_Plugins.Project.EditSystemPanel', { projectKeywor: me.projectKeyword });

        //定义用户选择Tab
        me.editContractPanel = Ext.create('Ext.plug_ins.HXEPC_Plugins.Project.EditContractPanel', { projectKeywor: me.projectKeyword });

        me.items = [
            me.editCrewPanel,
            me.editFactoryPanel,
            me.editSystemPanel,
            me.editContractPanel
        ];

        me.callParent(arguments);
    },
    listeners: {
        'tabchange': function (tabBar, newTab, oldTab) {
            this.onTabChange(tabBar, newTab, oldTab);
        }
    },

    onTabChange: function (tabBar, newTab, oldTab) {
        var me = this;

        var TabTitle = newTab.title;
        if (TabTitle === "机组") {
            me.editCrewPanel.sendEditCrewDefault();
        }
        else if (TabTitle === "厂房") {
            me.editFactoryPanel.sendEditFactoryDefault();
        }
        else if (TabTitle === "系统") {
            me.editSystemPanel.sendEditSystemDefault();
        }
        else if (TabTitle === "合同") {
            me.editContractPanel.sendEditContractDefault();//GetEditContractDefault
        }

    }
});