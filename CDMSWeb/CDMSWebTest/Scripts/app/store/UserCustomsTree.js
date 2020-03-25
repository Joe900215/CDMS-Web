//定义 分類树Store
Ext.define("CDMSWeb.store.UserCustomsTree", {
    extend: 'Ext.data.TreeStore',
    batchActions: false,
    remoteFilter: false,
    remoteSort: false,
    model: "CDMSWeb.model.UserCustomTree",
    //  root: { text: "文章分类", id: -1, expanded: true },
    //root: { id: "/", text: "根目录", expanded: true },
    root: { id: "Root", text: "根目录", expanded: true },

    //代理定义
    proxy: {
        type: 'ajax',
        ////代理的API定义了操作的提交路径
        ////路径：\CDMSWeb\Controllers\FileController.cs
        //api: {
        //    read: 'Project/GetProjectListJson?ProjectType=5' + '&sid=' + localStorage.getItem("sid"),//调用路径：\simplecdms\controllers\projectcontroller.cs
        //},
        url: "WebApi/Get",//调用路径：\simplecdms\controllers\projectcontroller.cs
        extraParams: {
            C: "AVEVA.CDMS.WebApi.ProjectController", A: "GetProjectListJson",
            ProjectType: 5, sid: localStorage.getItem("sid")
        },
        //在代理定义中，reader和writer的定义可标准化数据的输入输出，
        //这个与用户中的定义是一样的
        reader: {
            messageProperty: "Msg",
            type: 'json',
            root: "data"
        },
        writer: {
            type: "json",
            encode: true,
            root: "data",
            allowSingle: false
        },
        listeners: {
            exception: CDMSWeb.ProxyException
        }
    }
    //proxy: {
    //    //因为分类的新增和编辑都是通过表单形式完成的，因而在配置项api中只定义了删除地址。
    //    api: {
    //        destroy: 'Category/Delete'
    //    },
    //    type: "ajax",
    //    url: "/Category/List",
    //    reader: {
    //        type: 'json',
    //        root: "data",
    //        messageProperty: "Msg"
    //    },
    //    writer: {
    //        type: "json",
    //        encode: true,
    //        root: "data",
    //        allowSingle: true
    //    }
    //}
});

//Ext.override(Ext.grid.GridPanel, {
//    afterRender: Ext.Function.createSequence(Ext.grid.GridPanel.prototype.afterRender,
//        function () {
//            Ext.Msg.alert("您展开了目录树节点！！！", "您展开了目录树节点！节点ID:");
//        })
//});
