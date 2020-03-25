﻿Ext.define('Ext.plug_ins.HXEPC_Plugins.Document.UpImportFile', {
    extend: 'Ext.container.Container',
    alias: 'widget.UpImportFile',
    //layout: "border",
    layout: 'fit',
    resultvalue: '', mainPanelId: '', projectKeyword: '',
    initComponent: function () {
        var me = this;
        var docKeyword1 ;
        me.viewGrid1 = Ext.getCmp(me.mainPanelId).up('_mainSourceView').down('_mainDocGrid').down('gridpanel');//Ext.getCmp('_DocGrid');
        var gnodes1 = me.viewGrid1.getSelectionModel().getSelection();//获取已选取的节点
        if (gnodes1 !== null && gnodes1.length > 0) {
            var rec = gnodes1[0];//第一个文档
            docKeyword1 = rec.data.Keyword;
        }

        //定义文件上传Panel
        me.fileUploadPanel = Ext.create('Ext.plug_ins.HXEPC_Plugins.Document.Panel.FileUploadPanel', { projectKeyword: me.projectKeyword ,mainPanelId:me.mainPanelId });
        //设置FileUploadPanel
        me.fileUploadPanel.setImportMode();
        me.fileUploadPanel.projectKeyword = me.projectKeyword;
        me.fileUploadPanel.projectDirKeyword = me.projectKeyword;
        me.fileUploadPanel.winAction = "文档升版";
        me.fileUploadPanel.docKeyword = docKeyword1;//旧版的 docKeyword
        me.fileUploadPanel.batImportButton.setVisible(false);//设置批量上传按钮不可见
      

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
                            me.send_import_file();
                            winUpImportFile.close();

                        }
                    }
                },
                {
                    xtype: "button",
                    text: "取消", width: 60, margins: "10 15 10 5",
                    listeners: {
                        "click": function (btn, e, eOpts) {//添加点击按钮事件

                            winUpImportFile.close();
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
                  margin: '10 0 0 0',// 
                  items: [
                    me.fileUploadPanel,
                    me.bottomButtonPanel
                  ]
              }]
          })]

        //设置文件上传表格的模式
        //me.fileUploadPanel.setFileGridMode("import");

        me.callParent(arguments);
    },


    //获取导入文件表单默认参数
    sendGetImportFileDefault: function (funCallback) {
        var me = this;

        var draftOnProject = "false";
        if (me.projectKeyword === me.projectDirKeyword) {
            draftOnProject = "true";
        }
        //通过extjs的ajax获取操作全部名称
        Ext.Ajax.request({
            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.HXEPC_Plugins.Document", A: "GetImportFileDefault",
                sid: localStorage.getItem("sid"), ProjectKeyword: me.projectKeyword,
                DraftOnProject: draftOnProject
            },
            success: function (response, options) {
                me.sendGetImportFileDefault_callback(response, options, funCallback);

            }
        });
    },


    //处理获取发文处理表单默认参数的返回
    sendGetImportFileDefault_callback: function (response, options, funCallback) {
        var me = this;

        //获取数据后，更新窗口
        var res = Ext.JSON.decode(response.responseText, true);
        var state = res.success;
        if (state === true) {
            var recod = eval(res.data[0]);

            var strRootProjectCode = recod.RootProjectCode;
            var strRootProjectDesc = recod.RootProjectDesc;
            var strProjectDesc = recod.ProjectDesc;

            var sourceCompany = recod.SourceCompany;//项目所属公司
            var sourceCompanyDesc = recod.SourceCompanyDesc;


            //设置发文单位代码
            me.fileUploadPanel.setSendCompany(sourceCompany, sourceCompanyDesc);

            //设置项目管理类文件里项目的代码和描述
            me.fileUploadPanel.setRootProject(strRootProjectCode, strRootProjectDesc);

            //设置文件上传表格的模式
            me.fileUploadPanel.setFileGridMode("import");

            funCallback();
        }
    },

    send_import_file: function () {
        var me = this;
          
        //每上传完一个文件后，启动一个检查流程
        me.fileUploadPanel.afterUploadOneFile = function (Keyword) {
            //me.docKeyword = Keyword;
            //每个文件都启动一个检查流程
            //me.editFileAttr(Keyword, me.projectKeyword);
            Ext.Ajax.request({
                url: 'WebApi/Post',
                method: "POST",
                params: {
                    C: "AVEVA.CDMS.HXEPC_Plugins.Document", A: "StartCheckFileWorkFlow",
                    sid: localStorage.getItem("sid"), docKeyword: Keyword
                },
                success: function (response, options) {
                    me.refreshWin(me.projectKeyword, true);
                    var obj = Ext.decode(response.responseText);
                    if (obj.success == true) {
                       
                    }
                }
            });
            Ext.MessageBox.close();//关闭等待对话框
           // me.loadDocListStore(me.mainPanelId, function () { });//重新加载文档
            
        }

        
        ////上传完所有文件后，刷新表单
        //me.fileUploadPanel.afterUploadAllFile = function () {
        //    me.refreshWin(me.projectKeyword, true);
        //};
         
        me.fileUploadPanel.send_upload_file();

    },


    //刷新表单，参数:parentKeyword:新建的联系单目录
    refreshWin: function (parentKeyword, closeWin) {
        var me = this;
        //var tree = Ext.getCmp(me.mainPanelId).down('treepanel');
        var tree = Ext.getCmp(me.mainPanelId).up('_mainSourceView').down('_mainProjectTree').down('treepanel');
        var viewTreeStore = tree.store;

        viewTreeStore.load({
            callback: function (records, options, success) {//添加回调，获取子目录的文件数量
                if (closeWin)
                    winUpImportFile.close();

                //展开目录
                Ext.require('Ext.ux.Common.comm', function () {
                    Ext.getCmp('contentPanel').down('_mainProjectTree').ExpendProject(parentKeyword);
                });

                //回复函件，如果是点下了回复按钮，就把回复函件的流程提交到下一状态
                //me.replyCallbackFun();
            }
        });
    },

    //刷新文档列表.
    loadDocListStore: function (mainPanelId,callBackFun) {
        var me = this;

        var viewTree = Ext.getCmp(mainPanelId).up('_mainSourceView').down('_mainProjectTree').down('treepanel');//获取目录树控件ID

        var nodes = viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {
            var projectKeyword = nodes[0].data.Keyword;
            Ext.MessageBox.wait("正在获取文档列表，请稍候...", "等待");
            var view = Ext.getCmp(mainPanelId).up('_mainSourceView').down('_mainDocGrid');//Ext.getCmp('_DocGrid');

            //获取文档列表
            var store = view._DocListStore;//路径：\simplecdms\scripts\app\store\contents.js
            store.proxy.extraParams.ProjectKeyWord = projectKeyword;//把参数传给C#MVC,路径：\simplecdms\controllers\projectcontroller.cs 下的 GetChildProject()
            store.proxy.extraParams.filterJson = "";
            store.proxy.extraParams.sid = localStorage.getItem("sid");
            //store.loadPage(1);
            store.load({
                callback: function (records, options, success) {
                    Ext.MessageBox.close();//关闭等待对话框
                    callBackFun();
                }
            });
        }
    },

});