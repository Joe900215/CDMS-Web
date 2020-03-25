Ext.define('Ext.plug_ins.HXEPC_Plugins.Document.Recognition.addRecognitionAtta', {
    extend: 'Ext.container.Container',
    alias: 'widget.addRecognitionAtta',
    //layout: "border",
    layout: 'fit',
    resultvalue: '', mainPanelId: '', projectKeyword: '', fileCode:'',auditSheetDocKeyword:'',
    initComponent: function () {
        var me = this;
        me.renderTo = me.el;

        //定义文件上传Panel
        me.fileUploadPanel = Ext.create('Ext.plug_ins.HXEPC_Plugins.Document.Panel.FileUploadPanel', {
            projectKeyword: me.projectKeyword, projectDirKeyword: me.projectKeyword
        });

        //设置为固定一个文件模式
        me.fileUploadPanel.setFixedAloneMode(me.fileCode, me.auditSheetDocKeyword);//+" 手写校审意见单");

        //设置文件上传表格的模式
        me.fileUploadPanel.setFileGridMode("fixedAlone");

        //me.fileUploadPanel.gridMaxHeight = me.container.lastBox.height - 40;
        //me.fileUploadPanel.setGridMinHeight(100);

        me.fileUploadPanel.onFileEditButtonClick = function () {
            //me.editTopPanel.hide();
            //me.approvPathPanel.hide();
            me.bottomButtonPanel.hide();
            me.fileUploadPanel.setHeight(me.container.lastBox.height - 40);
            me.fileUploadPanel.filegrid.setHeight(me.container.lastBox.height - 40);
            //winDraftLetterCN.setTitle("起草信函 - 编辑附件");
            //winDraftLetterCN.closable = false;
        };

        //保存附件按钮事件
        me.fileUploadPanel.onFileSaveButtonClick = function () {

           // me.totalPagesText.setValue(me.fileUploadPanel.totalPages);

            //me.editTopPanel.show();
            //me.approvPathPanel.show();
            me.bottomButtonPanel.show();
            ////me.filegrid.setHeight(me.gridMinHeight);
            //winDraftLetterCN.setTitle("起草信函");
            //winDraftLetterCN.closable = true;
        };



        //底部按钮区域
        me.bottomButtonPanel = Ext.create("Ext.panel.Panel", {
            //xtype: "panel",
            layout: "hbox",
            baseCls: 'my-panel-no-border',//隐藏边框
            //align: 'right',
            //pack: 'end',//组件在容器右边
            items: [{
                flex: 1, baseCls: 'my-panel-no-border'//隐藏边框
            },
                {
                    xtype: "button",
                    text: "确定", width: 60, margins: "10 5 10 5",
                    listeners: {
                        "click": function (btn, e, eOpts) {//添加点击按钮事件
                            //me.send_draft_document();
                            me.send_upload_atta();
                        }
                    }
                },
                {
                    xtype: "button",
                    text: "取消", width: 60, margins: "10 15 10 5",
                    listeners: {
                        "click": function (btn, e, eOpts) {//添加点击按钮事件

                            winAddWorkFlowAttachment.close();
                        }
                    }
                }
            ]
        });


        //添加列表
        me.items = [
          Ext.widget('form', {
              baseCls: 'my-panel-no-border',//隐藏边框
              layout: {
                  type: 'vbox',
                  align: 'stretch',
                  pack: 'start'
              },
              items: [{//上部容器
                  baseCls: 'my-panel-no-border',//隐藏边框
                  layout: {
                      type: 'vbox',
                      pack: 'start',
                      align: 'stretch'
                  },
                  margin: '10 0 0 0',
                  items: [

                     //me.editTopPanel,
                     me.fileUploadPanel
                     //me.editBottomPanel


                  ], flex: 1
              },
              me.bottomButtonPanel

              ]
          })

        ];



        me.callParent(arguments);
    },

    send_upload_atta: function () {
        var me = this;

        me.fileUploadPanel.docList = "";//获取流程文档列表
        //me.newProjectKeyword = recod.ProjectKeyword;//获取新建的目录id
        //获取附件文件名的前缀
        //me.docCode = recod.DocCode;

        var response = "", options = "";

        me.fileUploadPanel.docKeyword = me.docKeyword;

        if (me.fileUploadPanel.FileUploadButton.uploader.uploader.files.length > 0) {
            //上传完所有文件后，刷新表单
            me.fileUploadPanel.afterUploadAllFile = function () {

                me.send_upload_atta_callback(response, options, "");
            };

            me.fileUploadPanel.send_upload_file();
        } else {
            //当没有附件时，处理返回事件
            me.send_upload_atta_callback(response, options, "");
        }

    },

    send_upload_atta_callback: function (response, options) {
        var me = this;

        //Ext.MessageBox.wait("正在生成信函，请稍候...", "等待");
        var attaDocKeyword = me.fileUploadPanel.fixedAloneDocKeyword;
        //var attaDocKeyword2 = me.fileUploadPanel.docList;

        //添加附件到流程
        Ext.Ajax.request({

            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.HXEPC_Plugins.Document", A: "AddRecognitionAtta",
                sid: localStorage.getItem("sid"), DocKeyword: me.docKeyword,
                AttaDocKeyword: attaDocKeyword
            },
            success: function (response, options) {
                //me.draft_document_callback(response, options, "");//, me.projectKeyword, closeWin);

                //获取数据后，更新窗口
                var res = Ext.JSON.decode(response.responseText, true);
                var state = res.success;
                if (state === true) {

                    Ext.MessageBox.close();//关闭等待对话框

                    var recod = eval(res.data[0]);

                    me.refreshWin("",true);
                    //me.docKeyword = recod.DocKeyword;//获取联系单文档id

                } else {
                    var errmsg = res.msg;
                    Ext.Msg.alert("错误信息", errmsg);
                }
            },
            failure: function (response, options) {
                //////Ext.Msg.alert("错误", "连接服务器失败！<br>" + response.responseText);
            }
        });
    },

    //刷新表单，参数:parentKeyword:新建的联系单目录
    refreshWin: function (parentKeyword, closeWin) {
        var me = this;

        //调用流程页事件，刷新父控件内容
        Ext.getCmp(me.mainPanelId).refreshMainPanle(me.projectKeyword, function () {
            if (closeWin)
                winAddWorkFlowAttachment.close();

                    //回复函件，如果是点下了回复按钮，就把回复函件的流程提交到下一状态
                    me.replyCallbackFun();
        });

        //var tree = Ext.getCmp(me.mainPanelId).up('_mainSourceView').down('_mainProjectTree').down('treepanel');
        //var viewTreeStore = tree.store;

        //viewTreeStore.load({
        //    callback: function (records, options, success) {//添加回调，获取子目录的文件数量
        //        if (closeWin)
        //            winAddWorkFlowAttachment.close();

        //        //展开目录
        //        Ext.require('Ext.ux.Common.comm', function () {
        //            Ext.getCmp('contentPanel').down('_mainProjectTree').ExpendProject(me.projectKeyword);
        //        });

        //        //回复函件，如果是点下了回复按钮，就把回复函件的流程提交到下一状态
        //        me.replyCallbackFun();
        //    }
        //});
    }
});