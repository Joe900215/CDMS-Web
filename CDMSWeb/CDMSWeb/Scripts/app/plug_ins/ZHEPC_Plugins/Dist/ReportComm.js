Ext.define('Ext.plug_ins.ZHEPC_Plugins.Dist.ReportComm', {

    winByName : null,

    //实例方法
    Say: function () {
        alert("你好");
    },

    createDocument: function (me, documentType, projKeyword, fileCode, title, docAttrJson) {

        var self = this;

        Ext.MessageBox.wait("正在生成Word表单，请稍候...", "等待");
        Ext.Ajax.request({
            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.ZHEPC_Plugins.Document", A: "CreateDocument",
                documentType:documentType, ProjectKeyword: projKeyword,
                fileCode: fileCode,
                title: title,docAttrJson:docAttrJson,
                sid: localStorage.getItem("sid")
            },
            success: function (response, options) {
                //Ext.MessageBox.close();//关闭等待对话框
                //获取数据后，更新窗口
                var res = Ext.JSON.decode(response.responseText, true);
                var state = res.success;
                if (state === false) {
                    var errmsg = res.msg;
                    Ext.Msg.alert("错误信息", errmsg);
                }
                else {
                    Ext.MessageBox.close();//关闭等待对话框

                    var recod = eval(res.data[0]);

                    me.docKeyword = recod.DocKeyword;//获取联系单文档id
                    me.fileUploadPanel.docList = recod.DocList;//获取流程文档列表
                    me.newProjectKeyword = recod.ProjectKeyword;//获取新建的目录id
                    //获取附件文件名的前缀
                    me.fileUploadPanel.docCode = recod.DocCode;

                    me.fileUploadPanel.docKeyword = me.docKeyword;

                    if (me.fileUploadPanel.FileUploadButton.uploader.uploader.files.length > 0) {
                        //上传完所有文件后，刷新表单
                        me.fileUploadPanel.afterUploadAllFile = function () {

                            self.draft_document_callback(me, response, options, "");
                        };

                        me.fileUploadPanel.send_upload_file();
                    } else {
                        //当没有附件时，处理返回事件
                        self.draft_document_callback(me, response, options, "");
                    }
                } 
            },
            failure: function (response, options) {
                //Ext.MessageBox.close();//关闭等待对话框
                Ext.Msg.alert("系统提示", "连接服务器失败，请尝试重新提交！");
            }
        });
    },

    //处理发送起草函件后的返回事件
    draft_document_callback: function (me, response, options) {
        var self = this;

        var docList = me.fileUploadPanel.docList;
        var projKeyword = me.newProjectKeyword;
        var documentType = me.documentType;

        Ext.Ajax.request({
            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.ZHEPC_Plugins.Document", A: "CreateDocShortcut",
                documentType:documentType, ProjectKeyword: projKeyword,
                DocList: docList,
                sid: localStorage.getItem("sid")
            },
            success: function (response, options) {
                //Ext.MessageBox.close();//关闭等待对话框
                //获取数据后，更新窗口
                var res = Ext.JSON.decode(response.responseText, true);
                var state = res.success;
                if (state === false) {
                    var errmsg = res.msg;
                    Ext.Msg.alert("错误信息", errmsg);
                }
                else {

                } 

                self.refreshWin(me, me.docKeyword, true);
            },
            failure: function (response, options) {
                //Ext.MessageBox.close();//关闭等待对话框
                Ext.Msg.alert("系统提示", "连接服务器失败，请尝试重新提交！");
            }
        })

    },

    //刷新表单，参数:parentKeyword:新建的联系单目录
    refreshWin: function (me, parentKeyword, closeWin) {
        var self = this;

        var tree = Ext.getCmp(me.mainPanelId).down('treepanel');
        var viewTreeStore = tree.store;

        viewTreeStore.load({
            callback: function (records, options, success) {//添加回调，获取子目录的文件数量
                if (closeWin)
                    //winA3.close();
                    self.winByName.close();

                //展开目录
                Ext.require('Ext.ux.Common.comm', function () {
                    Ext.getCmp('contentPanel').down('_mainProjectTree').ExpendProject(parentKeyword);
                });
            }
        });
    }

})