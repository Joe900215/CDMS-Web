//定义分类树模型
Ext.define('CDMSWap.model.ProjectTree', {
    extend: 'Ext.data.Model',
    //parentId用来记录父目录
    fields: [{ name: 'id', type: 'string' },
        { name: 'text', type: 'string' },
        { name: 'Keyword', type: 'string' }
    ],

    idProperty: "id"
});
