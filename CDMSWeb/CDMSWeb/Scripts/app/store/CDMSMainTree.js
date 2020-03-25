//定义 分類树Store
Ext.define("CDMSWeb.store.CDMSMainTree", {
    extend: 'Ext.data.TreeStore',
    batchActions: false,
    remoteFilter: false,
    remoteSort: false,
    model: "CDMSWeb.model.CDMSMainTree",
    root: { text: "文章分类", id: -1,expanded:true },
    proxy: {
        //因为分类的新增和编辑都是通过表单形式完成的，因而在配置项api中只定义了删除地址。
        api: {
            //destroy: 'Category/Delete'
        },
        type: "ajax",
        //url: "/Category/List",
        reader: {
            type: 'json',
            root: "data",
            messageProperty: "Msg"
        },
        writer: {
            type: "json",
            encode: true,
            root: "data",
            allowSingle: true
        },
        listeners: {
            exception: CDMSWeb.ProxyException
        }
    }
})
