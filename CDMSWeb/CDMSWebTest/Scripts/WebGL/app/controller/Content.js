Ext.define('WebGL.controller.Content', {
    extend: 'Ext.app.Controller',

    //引用视图：文章内容视图，分类修改视图，内容修改视图
    views: [
        'Content.View'
    ],

    //添加一些引用来获取视图和按钮,就可以通过get方法获得各控件
    refs: [
          //ref配置项生成引用的方法，可通过getUserPanel获取面板
          //而selector配置项就是面板的选择器，在这里使用它的id选择
          //视图路径：\scripts\app\view\mainpanel.js
         { ref: "ContentPanel", selector: "#contentPanel" },
    ],

    //加载init进入显示
    init: function () {
        var me = this,
            panel = me.getContentPanel();  //获取ContentPanel实例
        me.view = Ext.widget("contentview");//视图路径：\Scripts\app\view\Content\View.js
        panel.add(me.view);

    },


});