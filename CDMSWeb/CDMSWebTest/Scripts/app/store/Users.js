Ext.define("CDMSWeb.store.Users", {
    //扩展自'Ext.data.Store'类
    extend: 'Ext.data.Store',
    //引用数据模型
    model: 'CDMSWeb.model.User',
    //不实现批量操作，而是每当执行一个操作就提交数据
    batchActions: false,
    //自动去加载数据，不需要手动去加载
    autoLoad: true,
    //proxy:代理
    proxy: {
        type: "ajax",
        //api配置项，为4个操作固定好提交地址，提交的都是Users控制器的方法
        //Users控制器的路径：\CDMSWeb\Controllers\UsersController.cs
        api: {
            read: 'Users/List',
            destroy: 'Users/Delete',
            update: "Users/Edit",
            create: "Users/Add"
        },
        //render配置项中，固定了返回数据的格式
        reader: {
            type: 'json',
            //root配置项,固定数据都必须在data关键字内
            root: "data",
            //messageProperty:错误信息则固定在Msg关键字
            messageProperty: "Msg"
        },
        writer: {
            type: "json",
            //encode设置为true的作用就是使用习惯的提交方式提交数据，而不是以JSON流的方式提交
            encode: true,
            //root: 数据可通过读取data关键字获取
            root: "data",
            //allowSingle的作用是提交的数据是否一个个提交,这里设置为所有修改过的数据都会一次提交
            allowSingle: false
        },
        //监听\CDMSWeb\Controllers\UsersController.cs里面的错误信息
        //监听exception事件，把错误信息返回给Index.cshtml
        listeners:{
            exception : CDMSWeb.ProxyException
        }
    }
})
