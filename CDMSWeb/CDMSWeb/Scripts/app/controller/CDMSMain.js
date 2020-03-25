//定义用户管理控制器
Ext.define('CDMSWeb.controller.CDMSMain', {
    extend: 'Ext.app.Controller',
    //视图需要用到用户模型
    models: [
        'CDMSMain'
    ],

    //视图需要用到用户及角色的Store(存储器)
    stores: [
        'CDMSMain',
        'Roles'
    ],

    //引用用户视图视图
    views: [
        //Users.View表示创建视图时，要在\Scripts\app\view目录下创建Users目录，然后再创建View.js文件
        'CDMSMain.View'
    ],

    //添加一些引用来获取视图和按钮,就可以通过get方法获得各控件
    //因为要在主面板的标签页面板内添加视图，因而需要引用标签页面板，因而要加入refs配置项
    refs: [
        //ref配置项生成引用的方法，可通过getUserPanel获取面板
        //而selector配置项就是面板的选择器，在这里使用它的id选择
        //这里的userPanel为主面板里用户管理panel的id
        //视图路径：\scripts\app\view\mainpanel.js
        { ref: "CDMSMainPanel", selector: "#CDMSMainPanel" }
        //引用的视图路径：\Scripts\app\view\Users\View.js
        //{ ref: "CDMSMainView", selector: "#CDMSMainView" },
   ],

    //定义控制器时，都有1个init方法，在这里可以执行一些初始化操作
    init: function () {
        var me = this;
            //getUserPanel方法，就是refs配置项定义中自动生成的方法
            //panel = me.getCDMSMainPanel(),
            //使用widget方法创建的视图
            //view = Ext.widget("cdmsmainview");
        //将创建的用户视图通过add方法添加到面板
        //panel.add(view);
        //使用getUserView返回用户视图后，调用on方法绑定selectionchange事件
       // me.getCDMSMainView().on("selectionchange", me.onUserSelect, me);
        me.control({
        });
    },




});
