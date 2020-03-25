//定义逻辑目录树模型
Ext.define('CDMSWeb.model.LogicProjectTree', {
    extend: 'Ext.data.Model',
    //parentId用来记录父目录
    fields: ["id", "text"],
    //添加一个验证项，目录名称不能为空
    //validations: [{
    //    type: 'presence',
    //    field: 'text'
    //}]
});
