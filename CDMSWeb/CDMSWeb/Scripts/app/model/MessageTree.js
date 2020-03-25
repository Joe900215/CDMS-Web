//定义分类树模型
Ext.define('CDMSWeb.model.MessageTree', {
    extend: 'Ext.data.Model',
    //parentId用来记录父目录
    fields: ["text", "id", "parentId"]
    //添加一个验证项，目录名称不能为空
    //validations: [{
    //    type: 'presence',
    //    field: 'text'
    //}]
    //代码里没定义代理，这个将由Store进行定义，因为基本操作是在Store层面。
});
