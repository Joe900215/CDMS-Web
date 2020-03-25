Ext.define('CDMSWap.view.MainPanel', {
    extend: 'Ext.tab.Panel',
    //id:"mMainPanel",
    alias: 'widget.mainpanel',//必须用alias来为组件定义别名，才可以在Viewport里使用xtype来创建组件
    flex: 1,//主体部分设置flex为1，表示它会占据剩余的空间。
    activeTab: 0,//指定初始激活显示那个标签页

    initComponent: function () {
        var me = this;
        //创建主面板
        me.items = [


           // { title: "个人消息", id: "messagePanel", layout: "fit" },
            { title: "文档管理", id: "contentPanel", layout: "fit" }
            //,
            //{ title: "个人工作台", id: "_workplacePanel", layout: "fit" },
            //{ title: "逻辑目录", id: "_logicProjectPanel", layout: "fit" },
            //{ title: "查询", id: "_queryPanel", layout: "fit" }
            
        ];

        //通过Userinfo判断用户是否管理员
        var roles = "." + CDMSWap.Userinfo.Roles.join('.') + ".";
        if (roles.indexOf(".系统管理员.") >= 0) {
            //如果是系统管理员，添加用户管理标签页
            //layout:使用Fit布局后，视图就可填满标签页面板主体了
            //me.items.push({ title: "用户管理", id: "userPanel", layout: "fit" });
        }


        me.callParent(arguments);
    }
});



