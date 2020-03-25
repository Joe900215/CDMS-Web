//定义文档列表的Store，已作废，转移到me._contentAuditsStore
Ext.define("CDMSWeb.store.Audits", {
    extend: 'Ext.data.Store',
    model: 'CDMSWeb.model.Audit',//模型路径：\simplecdms\scripts\app\model\Property.js
    batchActions: false,
    //文章的Store需要支持远程排序和搜索
    remoteFilter: true,
    remoteSort: true,
    wfKeyword: '',//定义流程Keyword
    btntext: '',//记录按下了哪个按钮
    doclist:'',//记录选择的文档列表
    //data: [["普通用户"], ["系统管理员"]],
    //每50条记录为一页
    pageSize: 50,//视图路径：\simplecdms\scripts\app\view\content\view.js
    proxy: {
        type: "ajax",
        //url: "WorkFlow/GetWorkFlow",//调用路径：\simplecdms\controllers\projectcontroller.cs
        url: 'WebApi/Get',
        extraParams: {
            C: "AVEVA.CDMS.WebApi.WorkFlowController", A: "GetWorkFlow"
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
