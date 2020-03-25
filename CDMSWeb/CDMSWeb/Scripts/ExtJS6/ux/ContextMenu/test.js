/** 
 * 定义一个子菜单类 
 */
Ext.define('Ext.ux.ContextMenu.test', {//arguments
    extend: 'Ext.menu.Item',
    text:"新建",
menu:[{
    text: "新建目录",
    menu: [
    { text: "新建目录" ,handler:function(){alert(11111);}},//action:  '_StartProcess',},
    { text: "模板创建目录" },
    { text: "复制创建目录" }]
},
{
    text: "新建文档",
    menu: [
    { text: "新建文档" },
    { text: "模板创建文档" }]
}]
});

//var menuItem1 = new Ext.menu.Item({ text: 'I like Ext' });
//Ext.define('Ext.ux.ContextMenu.test', {//arguments
//    extend: 'Ext.menu.Menu',
//    text: "新建",
//    items: [{
//        text: "新建", menu: [{
//            text: "新建目录",
//            menu: [
//            { text: "新建目录" },
//            { text: "模板创建目录" },
//            { text: "复制创建目录" }]
//        },
//        {
//            text: "新建文档",
//            menu: [
//            { text: "新建文档" },
//            { text: "模板创建文档" }]
//        }]
//    }]
//});