Ext.define('CDMSWeb.controller._Query', {
    extend: 'Ext.app.Controller',
    requires: ["Ext.ux.YDForm._UserSelector"],
    requires: ["Ext.ux.YDForm.WorkFlow._winSelDefWorkFlow"],
    //requires: ["Ext.ux.Common.Enum"],

    /*分类的操作和文章的操作,在这里不进行拆分，就单独一个控制器*/

    //视图需要用到操作模型，分别为目录模型，目录树模型，目录combo模型，文章内容模型
    models: [
        'Category', 'ProjectTree', 'LogicProjectTree', 'UserCustomTree', 'GlobSearchTree', 'CategoryCombo', '_DocList', 'WorkFlowPage'
    ],

    //视图需要用到目录树及文章内容的Store(存储器)
    stores: [
        'ProjectsTree', 'LogicProjectsTree', 'UserCustomsTree', 'GlobSearchsTree', 'Contents', 'Audits', 'Attributes', 'Docs', 'WorkFlowPages'
    ],

    //引用视图：文章内容视图，分类修改视图，内容修改视图
    views: [
        '_Query.View', 'Ext.ux.YDForm._contextMenu'
    ],

    //添加一些引用来获取视图和按钮,就可以通过get方法获得各控件
    refs: [
          //ref配置项生成引用的方法，可通过getUserPanel获取面板
          //而selector配置项就是面板的选择器，在这里使用它的id选择
          //视图路径：\scripts\app\view\mainpanel.js
         { ref: "_QueryPanel", selector: "#_queryPanel" }
         //{ ref: "CategoryAdd", selector: "#CategoryButtonAdd" },
    ],

    //加载init进入显示
    init: function () {
        var me = this,
            //win = CDMSWeb.view.Content.CategoryEdit,
            panel = me.get_QueryPanel();  //获取ContentPanel实例
        me.view = Ext.widget("_queryview");//视图路径：\Scripts\app\view\Content\View.js
        panel.add(me.view);
        

        //定时刷新，防止Session过期
        setInterval(function () {
            //var store = me.getContentsStore();
            //store.reload();  // dataStore 换成你的 store 的变量名
        }, 480000);  //每隔 8 分钟
    }



});

