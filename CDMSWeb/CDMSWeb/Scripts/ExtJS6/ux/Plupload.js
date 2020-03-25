//图片管理要在两个地方使用：一是标签页内的图片管理，一是文章内容编辑时嵌套到插入图片的窗口内。
//因而，将图片管理做成一个扩展比较方便。
Ext.define('Ext.ux.UploadManager', {
    extend: 'Ext.container.Container',
    alias: 'widget.ploadManager',
    layout: "border"
} );