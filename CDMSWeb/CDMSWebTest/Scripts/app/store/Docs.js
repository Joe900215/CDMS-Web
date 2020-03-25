//定义文档的Store,已作废，转移到me._DocsAttrStore
Ext.define("CDMSWeb.store.Docs", {
    extend: 'Ext.data.Store',
    model: 'CDMSWeb.model.Doc',//模型路径：\simplecdms\scripts\app\model\Property.js
    //model: 'CDMSWeb.model.Attribute',//模型路径：\simplecdms\scripts\app\model\Property.js
    
    batchActions: false,
    //文章的Store需要支持远程排序和搜索
    remoteFilter: true,
    remoteSort: true,
    //data: [["普通用户"], ["系统管理员"]],
    //每50条记录为一页
    pageSize: 50,//视图路径：\simplecdms\scripts\app\view\content\view.js
    proxy: {
        type: "ajax",
        //url: "Doc/GetDocByKeyword",//调用路径：\simplecdms\controllers\projectcontroller.cs
        // method: "POST",
        url: 'WebApi/Get',
        extraParams: {
            C: "AVEVA.CDMS.WebApi.DocController", A: "GetDocByKeyword",
            CategoryId: 1
        },
        //extraParams: { : 1 },
        reader: {
            type: 'json',
            totalProperty: 'total',
            root: "data",
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
