//定义分类树模型
Ext.define('CDMSWeb.model.ProjectTree', {
    extend: 'Ext.data.Model',
    //alias: 'widget.ProjectTree',
    //parentId用来记录父目录
    fields: [{ name: 'id', type: 'string' },
        {
            name: 'text', type: 'string'
        },
        { name: 'Keyword', type: 'string' }
        //,
        //{ name: 'Code', type: 'string' },
        //{ name: 'Desc', type: 'string' }
    ],

    idProperty: "id"
});
