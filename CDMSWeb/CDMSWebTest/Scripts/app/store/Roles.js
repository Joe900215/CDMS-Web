//定义角色Store（角色存储器）
Ext.define("CDMSWeb.store.Roles", {
    //扩展自'Ext.data.ArrayStore'类
    extend: 'Ext.data.ArrayStore',
    fields: ["text"],
    //为了简单起见，就不从服务器获取角色数据了，直接定义在Store里
    data: [["普通用户"], ["系统管理员"]]
})
