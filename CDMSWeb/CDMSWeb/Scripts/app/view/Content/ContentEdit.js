Ext.define("CDMSWeb.view.Content.ContentEdit", {
    extend: "Ext.window.Window",
    requires: ["Ext.ux.Dialog.PicSelected", "CDMSWeb.store.CategoriesCombo", "Ext.ux.form.TinyMCETextArea"],
    hideMode: 'offsets',
    closeAction: 'hide',
    closable: true,
    resizable: true,
    layout: "fit",
    title: '文章',
    width: 800,
    height: 600,
    modal: true,
    singleton: true,
    bodyPadding: "0 0 10 0",


    initComponent: function () {
        var me = this;
        me.form = Ext.create(Ext.form.Panel, {
            border: false, bodyPadding: 5,layout:"fit",
            bodyStyle: "background:#DFE9F6",
            trackResetOnLoad: true,
            fieldDefaults: {
                labelWidth: 80, labelSeparator: "：", anchor: "0"
            },
            items: [
                { xtype: "tabpanel", activeTab: 0,
                    defaults: { xtype: "panel", border: false, 
                        bodyPadding: 5, bodyStyle: "background:#DFE9F6"
                    },
                    items: [
            			{ title: "基本信息",layout: "anchor",
            			    defaultType: "textfield", 
            			    items: [
                                { xtype: "hidden", name: "ContentId" },
				                { xtype: "textfield", fieldLabel: "标题", name: "Title", allowBlank: false },
				                {
				                    xtype: "fieldcontainer", layout: "hbox", fieldLabel: "题图", defaults: { hideLabel: true },
				                    items: [
						                { xtype: 'textfield', flex: 1, name: "Image",
						                    listeners: {
						                        scope: me,
						                        change: function (filed, newValue, oldValue) {
						                            Ext.getCmp("ContentPreviewImage").setSrc(newValue);
						                        }
						                    }
						                },
						                { xtype: "button", width: 80, text: "选择",
						                    handler: function () {
						                        var img = this.up("form").getForm().findField("Image");
						                        Ext.ux.Dialog.PicSelected.imagePath = "upload";
						                        Ext.ux.Dialog.PicSelected.ed = img;
						                        Ext.ux.Dialog.PicSelected.show();
						                    }
						                }
					                ]
				                },
				                {
				                    xtype: 'fieldset', title: "题图预览", height: 280, items: [
						                { xtype: "image", id: "ContentPreviewImage" }
					                ]
				                },
				                {
				                    xtype: "combobox", fieldLabel: "文章类别", name: "CategroyId", allowBlank: false,
				                    editable: true, shadow: false, mode: 'local', triggerAction: 'all', store: "categoriesCombo",
				                    displayField: "text", valueField: "id", flex: 1, queryMode: "local",
				                    listConfig: {
				                        displayField: "listText"
				                    }
				                },
				                {
				                    xtype: 'numberfield', fieldLabel: "排序序数", name: "SortOrder", allowBlank: false
				                },
				                {
				                    fieldLabel: "标签", name: "Tags", allowBlank: false
				                },
				                {
				                    xtype: 'container', html: "**请使用英文逗号分隔标签", anchor: "0", padding: "0 0 0 80px"
				                }
                            ]
            			}
                    ]
                }
            ],
            dockedItems: [{
                xtype: 'toolbar', dock: 'bottom', ui: 'footer', layout: { pack: "center" },
                items: [
		    	    { text: "保存", width: 80, disabled: true, formBind: true, handler: me.onSave, scope: me },
		    	    { text: "重置", width: 80, handler: me.onReset, scope: me }
			    ]
            }]
        });

        me.items = [me.form];
        me.callParent(arguments);
    },

    onReset: function () {
        var me = this;
        me.form.getForm().reset();
    },

    onSave: function () {
        var me = this,
			f = me.form.getForm();
        if (f.isValid()) {
            f.submit({
                //waitMsg: "正在保存，请等待……",
                //waitTitle: "正在保存",
                success: function (form, action) {
                    var me = this;
                },
                failure: CDMSWeb.FormSubmitFailure,
                scope: me
            });
        }
    }


})