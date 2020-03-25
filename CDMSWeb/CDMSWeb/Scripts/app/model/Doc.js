//定义文章内容模型
Ext.define('CDMSWeb.model.Doc', {
    extend: 'Ext.data.Model',
    fields: [
        "AttrCode",
        "TempAttrType",//附加属性的类别，        Common = 3, User = 4,
        "DataType",//附加属性的数据类型类别，user...
        "DefaultCode",//编辑属性的默认值
        "ShowData",//编辑属性的代码 
        "AttrName",
        "AttrValue",
        "AttrType",
        "Visible",
        "expiryDate",
        "CanEdit",//是否有权限编辑该属性
        "Tags"//代码中添加了一个Tags字段，用来显示标签，该字段将以数组形式返回数据，这样，在后续的处理中会很方便。
    ],
    proxy: {
        type: "ajax",
        //url: "Content/Details",
        reader: {
            type: 'json',
            root: "data",
            messageProperty: "Msg",
            resdc:"resdc"
        }
    },
    idProperty: "AttributeId"

});
