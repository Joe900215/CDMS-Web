//定义 分類树Store
Ext.define("CDMSWeb.store.ProjectsTree", {
    extend: 'Ext.data.TreeStore',
    batchActions: false,
    remoteFilter: false,
    remoteSort: false,
    model: "CDMSWeb.model.ProjectTree",
    root: { id: "Root", text: "根目录", expanded: true },

    //代理定义
    proxy: {
        type: 'ajax',
        //代理的API定义了操作的提交路径
        //路径：\CDMSWeb\Controllers\FileController.cs
        //api: {
        //    read: 'WebApi/Get?C=AVEVA.CDMS.WebApi.ProjectController&A=GetProjectListJson&ProjectType=1' + '&sid=' + localStorage.getItem("sid"),//调用路径：\simplecdms\controllers\projectcontroller.cs
        //    update: 'WebApi/Post?C=AVEVA.CDMS.WebApi.ProjectController&A=SaveProject',
            
        //},
        url: 'WebApi/Get?C=AVEVA.CDMS.WebApi.ProjectController&A=GetProjectListJson&ProjectType=1' + '&sid=' + localStorage.getItem("sid"),//调用路径：\simplecdms\controllers\projectcontroller.cs

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

});

