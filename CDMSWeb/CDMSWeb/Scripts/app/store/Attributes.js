//定义文档列表的Store，目录的附加属性栏,已作废
Ext.define("CDMSWeb.store.Attributes", {
    extend: 'Ext.data.Store',
    model: 'CDMSWeb.model._ProjectAttr',//模型路径：\simplecdms\scripts\app\model\Property.js
    batchActions: false,
    //文章的Store需要支持远程排序和搜索
    remoteFilter: true,
    remoteSort: true,
    //data: [["普通用户"], ["系统管理员"]],
    //每50条记录为一页
    pageSize: 50,//视图路径：\simplecdms\scripts\app\view\content\view.js
    proxy: {
        type: "ajax",
        //url: "Project/GetProject",//调用路径：\simplecdms\controllers\projectcontroller.cs
        url: 'WebApi/Get',
        //extraParams: { CategoryId: 1 },
        extraParams: {
            C: "AVEVA.CDMS.WebApi.ProjectController", A: "GetProject",
            ProjectKeyWord: 1
        },
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
