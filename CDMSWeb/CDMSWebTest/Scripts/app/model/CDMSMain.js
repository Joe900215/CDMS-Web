Ext.define('CDMSWeb.model.CDMSMain', {
    //扩展自'Ext.data.Model'类
    extend: 'Ext.data.Model',
    fields: [
    	"id",
        //defaultValue:在新建用户的时候，会使用该配置项的值作为默认值
    	{ name: "Username", defaultValue: "newuser" },
		{ name: "Email", defaultValue: "newuser@email.com" },
        //角色字段
        { name: "Roles", defaultValue: "普通用户" },
        //创建日期字段
    	{ name: "Created", type: "date", dateFormat: "Y-m-d H:i:s", defaultValue: new Date() },
        //最后登录时间字段
        { name: "LastLoginDate", type: "date", dateFormat: "Y-m-d H:i:s" },
        //是否禁用字段
        { name: "IsApproved", type: "bool", defaultValue: true }
    ],
    //定义ID属性字段
    idProperty: "id"

});
