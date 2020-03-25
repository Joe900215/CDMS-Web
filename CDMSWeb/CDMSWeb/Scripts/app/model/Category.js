//定义完整的分类模型
Ext.define('CDMSWeb.model.Category', {
    extend: 'Ext.data.Model',
    //字段名与表格定义的字段名是一样的，这样的好处是方便
    //有些字段没有定义是因为，在编辑过程中不需要显示这些字段，就省略了
    fields: [
    	{ name: "ParentId", type: "int", defaultValue: "-1" },
        { name: "CategoryId", type: "int" },
    	"Title",
        "Image",
        "Content",
    	{ name: "Created", type: "date", dateFormat: "Y-m-d H:i:s", defaultValue: new Date() },
        { name: "SortOrder", type: "int", defaultValue: 0 }
    ],
    //代理的定义与之前的定义基本雷同，目的是保证使用统一的数据返回格式
    proxy: {
        type: "ajax",
        url: "Category/Details",
        reader: {
            type: 'json',
            root: "data",
            messageProperty: "Msg"

        }
    },
    idProperty: "CategoryId"
});
