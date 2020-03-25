//该模型实际只需要2个字段，不过为了显示上的需要，特意添加了另外几个字段。
Ext.define('CDMSWeb.model.CategoryCombo', {
    extend: 'Ext.data.Model',
    fields: ["text",
		{ name: "id", type: "int" },
		{ name: "parentId", type: "int" },
        "listText",
        "fullpath",
        { name: "Hierarchylevel", type: "int" }
    ]
});
