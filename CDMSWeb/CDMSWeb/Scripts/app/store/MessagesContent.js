//定义文档列表的Store
Ext.define("CDMSWeb.store.MessagesContent", {
    extend: 'Ext.data.Store',
    model: 'CDMSWeb.model.MessageContent',//模型路径：\simplecdms\scripts\app\model\content.js
    //batchActions: false,
    ////文章的Store需要支持远程排序和搜索
    //remoteFilter: true,
    //remoteSort: true,
    ////无限滚动需要//
    //sorters: {
    //    property: 'lastpost',
    //    direction: 'DESC'
    //},
    //autoLoad: true,
    ////////////////////////
    ////data: [["普通用户"], ["系统管理员"]],
    ////每50条记录为一页
    //pageSize: 50,//视图路径：\simplecdms\scripts\app\view\content\view.js
    
    //proxy: {
    //    type: "ajax",
    //    url: 'WebApi/Get',
    //    extraParams: {
    //        C: "AVEVA.CDMS.WebApi.MessageController", A: "GetMessage",
    //        MessageKeyword: 1, total: 50000, sid: localStorage.getItem("sid")
    //    },
    //    reader: {
    //        type: 'json',
    //        totalProperty: 'total',
    //        root: "data",//从C#MVC获取数据\simplecdms\controllers\ProjectController.cs .GetDocList.data  ，获取到的数据传送到model里面
    //        messageProperty: "Msg"
    //    },
    //    writer: {
    //        type: "json",
    //        encode: true,
    //        root: "data",
    //        allowSingle: false
    //    },
    //},
    //simpleSortMode: true
})
