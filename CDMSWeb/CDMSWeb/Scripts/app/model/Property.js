//定义文章内容模型
Ext.define('CDMSWeb.model.Property', {
    extend: 'Ext.data.Model',
    fields: [
    	"PropertyName",
        "PropertyValue",
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
