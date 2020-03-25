Ext.define("CDMSWeb.store.CategoriesCombo", {
    extend: 'Ext.data.Store',
    batchActions: false,
    remoteFilter: false,
    remoteSort: false,
    //配置项autoLoad为true，说明在Store实例化后会自动加载数据
    autoLoad: true,
    singleton: true,
    model: "CDMSWeb.model.CategoryCombo",
    proxy: {
        type: "ajax",
        url: "/Category/All",
        reader: {
            type: 'json',
            root: "data",
            messageProperty: "Msg"
        }
    },
    storeId: "categoriesCombo"
});