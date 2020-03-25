//定义文章内容模型,已作废
Ext.define('CDMSWeb.model._DocList', {
    extend: 'Ext.data.Model',
    fields: [
        "Keyword",
        //"O_itemno",//docid
    	"Title",
        "O_size",
        "O_filename",//文件名
        "O_dmsstatus_DESC",
        "O_version",
    	"Creater",
        "O_credatetime",
        "Updater",
        "O_updatetime",
        "O_outpath",
        "O_flocktime",
        "O_conode",
        "Content",
        "WriteRight",//写文件权限

    	//{ name: "Created", type: "date", dateFormat: "Y-m-d H:i:s", Ext:Date.parse('10/15/2006', 'm/d/Y')}, // defaultValue: new Date() },
        { name: "ProjectId", type: "int" },
        { name: "O_itemno", type: "int" },
        "Tags"//代码中添加了一个Tags字段，用来显示标签，该字段将以数组形式返回数据，这样，在后续的处理中会很方便。
        //{name:'lastpost',type:'date',dateFormat:'timestamp'}//无限滚动需要
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
    idProperty: "ContentId"

});
