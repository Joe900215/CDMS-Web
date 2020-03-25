//定义文章内容模型
Ext.define('CDMSWap.model.WorkFlowPage', {
    extend: 'Ext.data.Model',
    fields: [
        //添加第二、第三页的字段()
        "UserName",//用户
        "WorkState",//工作状态
        "FinishDate",//流程页的流程提交时间
        "SendDate",//日期
        "PageId",//页码

        //添加第一页的字段
        "code",
        "desc",
        "o_filename",
        "o_size",
        "o_credatetime",
        "o_updatetime",
        "o_status",
        "attrKeyword",
        "attrType",
        "attrKeyword",
        "attrType",

        "Tags"//代码中添加了一个Tags字段，用来显示标签，该字段将以数组形式返回数据，这样，在后续的处理中会很方便。
    ],
    //proxy: {
    //    type: "ajax",
    //    //url: "Content/Details",
    //    reader: {
    //        type: 'json',
    //        root: "data",
    //        messageProperty: "Msg"

    //    }
    //}
    batchActions: false,
    //文章的Store需要支持远程排序和搜索
    remoteFilter: true,
    remoteSort: true,
    //data: [["普通用户"], ["系统管理员"]],
    //每50条记录为一页
    pageSize: 50,//视图路径：\simplecdms\scripts\app\view\content\view.js
    proxy: {
        type: "ajax",
        //url: "WorkFlow/GetWorkFlowPagesData",//调用路径：\simplecdms\controllersWorkFlowcontroller.cs
        url: 'WebApi/Get',
        extraParams: {
            C: "AVEVA.CDMS.WebApi.WorkFlowController", A: "GetWorkFlowPagesData"
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
        }
    }

});
