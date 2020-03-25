Ext.define('CDMSWeb.view.Message.contextMenu', {
    extend: 'Ext.menu.Menu',
    //id: 'contextMenu',
    alias: 'widget.contextmenu',
    float: true,
    items: [{
        //xtype: 'button',
        //id: 'gotoSourceMenu',
        text: '转到源目录',
        //action: 'add',
        action: 'submenu1',
        iconCls: 'leaf'
    }]
});