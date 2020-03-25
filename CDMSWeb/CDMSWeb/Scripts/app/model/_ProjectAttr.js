//定义文章内容模型
Ext.define('CDMSWeb.model._ProjectAttr', {
    extend: 'Ext.data.Model',
    fields: [
        //"Keyword",
    	//"Projectname",//目录名
        //"Version",//版本号
    	//"Version_seq",//版本顺序
        //"Manager",//管理者
        //"Creator",//创建者
        //"Updater",//更新者
        //"Storage",//存储空间
        //"CreateTime",//创建时间
        //"UpdateTime",//更新时间
        //"WorkFlow",//流程
        //"Status",//目录状态
        //"Type",//目录类型
        //"TempDefn",//模板
        "AttrCode",
        "AttrName",
        "AttrValue",
        "AttrType",
        "Visible",
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
    idProperty: "AttributeId"

});
