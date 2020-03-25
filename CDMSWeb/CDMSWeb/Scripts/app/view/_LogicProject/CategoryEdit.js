//Ext.define("CDMSWeb.view.Content.CategoryEdit", {
//    extend: "Ext.window.Window",
//    requires: ["Ext.ux.Dialog.PicSelected", "CDMSWeb.store.CategoriesCombo"],
//    hideMode: 'offsets',
//    closeAction: 'hide',
//    closable: true,
//    resizable: true,
//    layout: "fit",
//    title: '文章分类',
//    width: 800,
//    height: 600,
//    modal: true,
//    singleton: true,
//    bodyPadding: "0 0 10 0",


//    initComponent: function () {
//        var me = this;
//        me.form = Ext.create(Ext.form.Panel, {
//            border: false, bodyPadding: 5,
//            bodyStyle: "background:#DFE9F6",
//            trackResetOnLoad: true,
//            fieldDefaults: {
//                labelWidth: 80, labelSeparator: "：", anchor: "0"
//            },
//            items: [
//                { xtype: "hidden", name: "CategoryId" },
//				{ xtype: "textfield", fieldLabel: "标题", name: "Title", allowBlank: false },
//				{
//				    xtype: "fieldcontainer", layout: "hbox", fieldLabel: "题图", defaults: { hideLabel: true },
//				    items: [
//						{ xtype: 'textfield', flex: 1, name: "Image",
//						    listeners: {
//						        scope: me,
//						        change: function (filed, newValue, oldValue) {
//						            Ext.getCmp("CategoryPreviewImage").setSrc(newValue);
//						        }
//						    }
//						},
//						{ xtype: "button", width: 80, text: "选择",
//						    handler: function () {
//						        var img = this.up("form").getForm().findField("Image");
//						        Ext.ux.Dialog.PicSelected.imagePath = "upload";
//						        Ext.ux.Dialog.PicSelected.ed = img;
//						        Ext.ux.Dialog.PicSelected.show();
//						    }
//						}
//					]
//				},
//				{
//				    xtype: 'fieldset', title: "题图预览", height: 150, items: [
//						{ xtype: "image", id: "CategoryPreviewImage" }
//					]
//				},
//				{
//				    xtype: "combobox", fieldLabel: "父类", name: "ParentId", allowBlank: false,
//				    editable: true, shadow: false, mode: 'local', triggerAction: 'all', store: "categoriesCombo",
//				    displayField: "text", valueField: "id", flex: 1, queryMode: "local",
//				    listConfig: {
//				        displayField: "listText"
//				    }
//				},
//				{
//				    xtype: 'numberfield', fieldLabel: "排序序数", name: "SortOrder", allowBlank: false
//				},
//                {
//                    xtype: 'textareafield', fieldLabel: "说明", name: "Content", labelAlign: "top", height: 250
//                }
//            ],
//            dockedItems: [{
//                xtype: 'toolbar', dock: 'bottom', ui: 'footer', layout: { pack: "center" },
//                items: [
//		    	    { text: "保存", width: 80, disabled: true, formBind: true, handler: me.onSave, scope: me },
//		    	    { text: "重置", width: 80, handler: me.onReset, scope: me }
//			    ]
//            }]
//        });

//        me.items = [me.form];
//        me.callParent(arguments);
//    },

//    onReset: function () {
//        var me = this;
//        me.form.getForm().reset();
//    },

//    onSave: function () {
//        var me = this,
//			f = me.form.getForm();
//        if (f.isValid()) {
//            f.submit({
//                //waitMsg: "正在保存，请等待……",
//                //waitTitle: "正在保存",
//                success: function (form, action) {
//                    var me = this,
//                        values = form.getValues(),
//                        data = action.result.data[0];
//                    if (values.CategoryId > 10000) {
//                        me.store.load();
//                    } else {
//                        if (data.parentId) {
//                            var parentNode = me.store.getNodeById(data.parentId);
//                            if (parentNode) {
//                                if (parentNode.isExpanded()) {
//                                    parentNode.appendChild(data);
//                                } else {
//                                    parentNode.expand();
//                                }
//                            } else {
//                                parentNode = me.store.getNodeById(data.fullpath.substr(1, 5));
//                                me.view.expand(parentNode, true);
//                            }
//                        } else {
//                             me.store.getRootNode().appendChild(data);
//                        }
//                    }
//                    me.form.down("combobox").store.load();
//                    me.close();
//                },
//                failure: CDMSWeb.FormSubmitFailure,
//                scope: me
//            });
//        }
//    }


//})