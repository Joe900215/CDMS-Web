//定义文档列表的Store,已作废，转移到me._DocListStore
Ext.define("CDMSWeb.store.Contents", {
    extend: 'Ext.data.Store',
    model: 'CDMSWeb.model._DocList',//模型路径：\simplecdms\scripts\app\model\content.js
    batchActions: false,
    //文章的Store需要支持远程排序和搜索
    remoteFilter: true,
    remoteSort: true,
    //sorters: {
    //    property: 'lastpost',
    //direction:'DESC'},
    //data: [["普通用户"], ["系统管理员"]],
    //每50条记录为一页
    pageSize: 50,//视图路径：\simplecdms\scripts\app\view\content\view.js
    proxy: {
        type: "ajax",
        //type:'scripttag',
        //url: "Doc/GetDocList",//调用路径：\simplecdms\controllers\projectcontroller.cs
        url: 'WebApi/Get',
        extraParams: {
            C: "AVEVA.CDMS.WebApi.DocController", A: "GetDocList",
            ProjectKeyWord: 1
        },
        reader: {
            type: 'json',
            totalProperty: 'total',
            root: "data",//从C#MVC获取数据\simplecdms\controllers\ProjectController.cs .GetDocList.data  ，获取到的数据传送到model里面
            messageProperty: "Msg"
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
})
