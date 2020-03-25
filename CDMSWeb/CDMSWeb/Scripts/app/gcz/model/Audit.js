//定义文章内容模型
Ext.define('CDMSWap.model.Audit', {
    extend: 'Ext.data.Model',
    fields: [
        "UserName",//用户
        "WorkState",//工作状态
        "ProcAudit",//处理意见
        "DeProcAudit",//修改意见
        "ProcTime",//处理时间
        "UserKeyword",//用户关键字
        "AuditRight",//获取修改意见的权限,"1":有填写校审意见权限，"2"：有主设人填写修改意见权限
        //"WsKeyword",//工作状态ID

        "BtnType",//按钮类型
        "Desc",//按钮描述
        "Enabled",//是否禁用按钮
        "Visible",//是否显示

        "WfKeyword",//记录当前工作流的Keyword
        "isFinish",//记录当前工作流是否已经完成
        "KeyWord",
        "Tags"//代码中添加了一个Tags字段，用来显示标签，该字段将以数组形式返回数据，这样，在后续的处理中会很方便。
    ],
    proxy: {
        type: "ajax",
        //url: "Content/Details",
        reader: {
            type: 'json',
            root: "data",
            messageProperty: "Msg"

        }
    },
    idProperty: "PropertyId"

});
