Ext.define('Ext.ux.gcz.File._PreviewFile', {
    //extend: 'Ext.container.Container',
    extend: 'Ext.panel.Panel',
    alias: 'widget._previewFile',
    layout: 'fit',
    //height:200,
    //DocKeyword: '', mainPanelId: '',
    //baseCls: 'my-panel-no-border',//隐藏边框
    //projectKeyword: '', WfKeyword: '',
    initComponent: function () {
        var me = this;

        me._DocPreviewPanel = Ext.create('Ext.panel.Panel', {

            //width: '100%',
            layout: 'fit',
            html: '<div id="mapPic">' + '<iframe src="Scripts/PDFJSInNet/web/viewer.html?file=/download/N4LRTNRJ2XV0TFB4/%E8%BD%AF%E4%BB%B6%E4%BD%BF%E7%94%A8%20%E8%AF%B4%E6%98%8E%E4%B9%A6.pdf"  scrolling="no" style="width:100%;height:600px" frameborder="0"></iframe> </div>',
            flex: 1
        });

        me.items = [
            me._DocPreviewPanel
              //{
              //    xtype: "button",
              //    text: "我的按钮"
              //}
        ];

        me.PreviewDoc(me._DocPreviewPanel.id, "false", "", "");

        me.callParent(arguments);
    },

    //编辑文档属性,参数viewInPanel：true:在panle中打开，false:在新窗口打开
    //unRarPath:解压后文件的路径，unRarItemName：解压后文件名
    PreviewDoc: function (viewInPanel, isUnRar, unRarPath, unRarItemName) {
        var me = this;

        var docKeyword = me.DocKeyword;//rs.data.Keyword;
                var view = me;//.up('_mainSourceView').down('_mainAttrTab');
                var A = "";
                if (isUnRar === "false") {
                    A = "PreviewDoc";
                }
                else {
                    A = "PreviewZipDoc";
                }
                //先把预览视图清空
                Ext.require('Ext.ux.Common.m.comm', function () {
                    updateDocPreview(me, view._DocPreviewPanel, "", "", "");
                });

                //if (viewInPanel === "true")
                //    Ext.MessageBox.wait("正在生成预览，请稍候...", "等待");

                Ext.Ajax.request({
                    url: 'WebApi/Post',
                    method: "POST",
                    timeout: 20000,
                    params: {
                        C: "AVEVA.CDMS.WebApi.DocController", A: A,
                        DocKeyword: docKeyword, sid: localStorage.getItem("sid"),
                        path: unRarPath, filename: unRarItemName
                    },
                    failure: function (response, options) {
                        if (response.timedout === true) {
                            setTimeout(me.PreviewDoc(viewInPanel, isUnRar, unRarPath, unRarItemName), 5000);
                        }
                            // 其他错误，如网络错误等
                        else {

                            //longPolling();
                            //Ext.MessageBox.close();//关闭等待对话框
                            Ext.Msg.alert("错误", "预览文件失败，网络错误！" + response.responseText);
                        }
                    },
                    success: function (response, options) {
                        var res = Ext.JSON.decode(response.responseText, true);
                        var state = res.success;
                        if (state === false) {
                            var errmsg = res.msg;
                            if (errmsg === "isExtracting") {
                                setTimeout(me.PreviewDoc(viewInPanel, isUnRar, unRarPath, unRarItemName), 5000);
                            } else {
                                Ext.Msg.alert("错误信息", errmsg);
                            }
                        }
                        else {
                            var recod = res.data[0];
                            if (recod.filetype === "common") {
                                if (viewInPanel === 'false') {
                                    window.open(recod.path, '_blank');//新窗口打开链接
                                } else {
                                    var view, viewPanel;
                                    if (viewInPanel === 'true') {
                                        view = me.up('_mainSourceView').down('_mainAttrTab');
                                        if (recod.isUnrar === 'true')
                                        { view.showRarGrid(); }
                                        else {
                                            view.hideRarGrid();
                                        }
                                        viewPanel = view._DocPreviewPanel;
                                    } else {
                                        viewPanel = Ext.getCmp(viewInPanel);

                                    }


                                    //刷新视图
                                    Ext.require('Ext.ux.Common.m.comm', function () {
                                        updateDocPreview(me, viewPanel, recod.path, options.params.DocKeyword, recod.filename);
                                    });

                                    //Ext.MessageBox.close();//关闭等待对话框
                                }
                            } else {
                                //响应rar或zip文件，显示解压文件列表
                                var view = me.up('_mainSourceView').down('_mainAttrTab');
                                view._RarListStore.removeAll();
                                view.showRarGrid();
                                view.objfilelist = recod.filelist;
                                view.subFolder = recod.subFolder;
                                //Ext.each(recod.filelist, function (v) {
                                //    view.updateRarListStore(v);
                                //}, me);

                                view.updateRarListStore(recod.filelist);

                                Ext.require('Ext.ux.Common.m.comm', function () {
                                    updateDocPreview(me, view._DocPreviewPanel, "", options.params.DocKeyword, recod.filename);
                                });
                                view.tmpParentPath = "";
                                //Ext.MessageBox.close();//关闭等待对话框
                            }

                        }
                    }
                });
        //    }
        //}
    },
});