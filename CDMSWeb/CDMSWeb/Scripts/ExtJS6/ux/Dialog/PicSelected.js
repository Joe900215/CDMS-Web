Ext.define("Ext.ux.Dialog.PicSelected", {
    extend: "Ext.window.Window",
    requires: ["Ext.ux.PicManager"],
    singleton: true,
    hideMode: 'offsets',
    closeAction: 'hide',
    closable: true,
    layout: "fit",
    imagePath: "",
    resizable: true,
    title: '插入图片',
    width: 1000,
    height: 600,
    modal: true,
    initComponent: function () {
        var me = this;

        me.picmanager = Ext.create("Ext.ux.PicManager", { title: "" });
        me.items = [me.picmanager];
        me.dockedItems = [{
            xtype: 'toolbar', dock: 'bottom', ui: 'footer', layout: { pack: "center" },
            items: [
	    		{ text: "插入", width: 80, handler: me.onInsert, scope: me }
		    ]
        }];

        me.callParent(arguments);
    },

    onInsert: function () {
        var me = this, rs = me.picmanager.dataview.getSelectionModel().getSelection();
        if (rs.length > 0) {
            if (me.ed.isFormField) {
                me.ed.setValue(Ext.String.format("{0}{1}{2}", me.imagePath, rs[0].data.path, rs[0].data.filename));
            } else {
                var html = "";
                me.imagePath = "upload";
                for (var i = 0; ln = rs.length, i < ln; i++) {
                    html += Ext.String.format("<img src='{0}{1}{2}' >", me.imagePath, rs[i].data.path, rs[i].data.filename);
                }
                me.ed.execCommand('mceInsertContent', false, html);
            }
        }
        me.close();
    }


})