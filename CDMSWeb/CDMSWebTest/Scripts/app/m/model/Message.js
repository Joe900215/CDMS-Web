//定义文章内容模型
Ext.define('CDMSWap.model.Message', {
    extend: 'Ext.data.Model',
    fields: [
        "Keyword",
        "Sender",
    	"Title",
        "SendDatetime",
    	"Workflow_DESC",
        "SignificantType",//重要程度
        "DelayDay",//延迟天数
        "WorkflowId",
        "Tags",//代码中添加了一个Tags字段，用来显示标签，该字段将以数组形式返回数据，这样，在后续的处理中会很方便。
        { name: 'lastpost', mapping: 'post_time', type: 'date', dateFormat: "timestamp" }//无限滚动需要
    ],
    idProperty: "Keyword"

});

