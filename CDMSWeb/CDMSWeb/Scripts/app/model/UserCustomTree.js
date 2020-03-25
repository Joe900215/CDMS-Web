//定义个人工作台树模型
Ext.define('CDMSWeb.model.UserCustomTree', {
    extend: 'Ext.data.Model',
    //parentId用来记录父目录
    fields: ["id", "text"],
    //代码里没定义代理，这个将由Store进行定义，因为基本操作是在Store层面。
});
