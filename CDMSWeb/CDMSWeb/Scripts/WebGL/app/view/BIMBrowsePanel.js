Ext.define('WebGL.view.BIMBrowsePanel', {
    extend: 'Ext.tab.Panel',
    //id:"mMainPanel",
    alias: 'widget.bimbrowsepanel',//必须用alias来为组件定义别名，才可以在Viewport里使用xtype来创建组件
    flex: 1,//主体部分设置flex为1，表示它会占据剩余的空间。
    activeTab: 0,//指定初始激活显示那个标签页

    initComponent: function () {
        var me = this;
        //创建主面板
        me.items = [

            { title: "BIM模型管理", id: "contentPanel", layout: "fit" },
            me.ToastWindow
            //为图片管理加回布局，布局类型为Fit

        ];



        me.callParent(arguments);
    }
});

