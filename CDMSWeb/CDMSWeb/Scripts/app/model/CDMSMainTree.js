//定义分类树模型
Ext.define('CDMSWeb.model.CDMSMainTree', {
    //扩展自'Ext.data.Model'类
    extend: 'Ext.data.Model',
    //定义字段
    fields: ["text",
		{ name: "id", type: "int" },
		{ name: "parentId", type: "int" }
    ]
    //代码里没定义代理，这个将由Store进行定义，因为基本操作是在Store层面。
});
