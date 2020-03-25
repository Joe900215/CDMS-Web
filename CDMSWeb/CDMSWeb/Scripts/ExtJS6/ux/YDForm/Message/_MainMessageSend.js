Ext.define('Ext.ux.YDForm.Message._MainMessageSend', {
    extend: 'Ext.panel.Panel',
    alias: 'widget._mainMessageSend',
    //layout: "border",
    layout: 'fit',

    initComponent: function () {
        var me = this;
        me.renderTo = me.el;

        me.cuFormType = "";

        //定义enMessageFormType枚举
        me.enMessageFormType = {
            NewMessage: 1,
            ViewMessage: 2,
            TransmitMessage: 3,
            DraftMessage: 4,
            ReplyMessage: 5
        }

        me.IsReplyMsg = false;

        me.recUserList = "";
        me.CCUserList = "";

        me.curMessageKeyword = "";

        //附件Keyword列表
        //me.MessageAttaList = "";

        me.curRecord = null;

        me.sendMsgBtn = Ext.create("Ext.button.Button", {

            xtype: "button",
            iconCls: "msg_read",
            text: "发送",
            listeners: {
            "click": function (btn, e, eOpts) {//添加点击按钮事件
                me.sendMessage();

            }
        }
        });

        me.replyBtn = Ext.create("Ext.button.Button", {
            
            xtype: "button",
            iconCls: "msg-reply",
            text: "回复",
            listeners: {
                "click": function (btn, e, eOpts) {//添加点击按钮事件
                    me.cuFormType = me.enMessageFormType.ReplyMessage;
                   me.displayMessAttr(me.curRecord);
                }
            }
            
        });

        me.newMsgBtn = Ext.create("Ext.button.Button", {

            xtype: "button",
            iconCls: "msg-new",
            text: "创建新消息",
            listeners: {
                "click": function (btn, e, eOpts) {//添加点击按钮事件
                    me.cuFormType = me.enMessageFormType.NewMessage;
                    me.displayMessAttr();
                }
            }

        });

        me.transmitBtn = Ext.create("Ext.button.Button", {

            xtype: "button",
            iconCls: "msg_transmit",
            text: "转发",
            listeners: {
                "click": function (btn, e, eOpts) {//添加点击按钮事件
                    //me.sendMessage();
                    //me.cuFormType = "TransmitMessage";
                    me.cuFormType = me.enMessageFormType.TransmitMessage;
                    me.displayMessAttr(me.curRecord);
                }
            }

        });

        me.draftMessageBtn = Ext.create("Ext.button.Button", {

            xtype: "button",
            iconCls: "msg-new",
            text: "保存草稿",
            listeners: {
                "click": function (btn, e, eOpts) {//添加点击按钮事件
                    me.cuFormType = me.enMessageFormType.DraftMessage;
                    me.draftMessage();
                }
            }

        });

        me.recUserBtn= Ext.create("Ext.button.Button", {
            xtype: "button",
            text: "接收方...", margin: '5 5 0 0',
            width: 65,
            listeners: {
                "click": function (btn, e, eOpts) {//添加点击按钮事件
                    Ext.require('Ext.ux.Common.comm', function () {
                        showSelectUserWin("getUser", "", "", function () {
                            if (window.parent.resultvalue != undefined && window.parent.resultvalue != "") {
                                if (me.recUserList != "") {
                                    me._RecUsersText.setValue(me._RecUsersText.value + "," + window.parent.usernamelist);
                                    me.recUserList = me.recUserList + "," + window.parent.resultvalue;
                                } else {
                                    me._RecUsersText.setValue(window.parent.usernamelist);
                                    me.recUserList = window.parent.resultvalue;
                                }
                            }
                        });
                    })
                }
            }
        });
        
        me.ccUserBtn = Ext.create("Ext.button.Button", {
            xtype: "button",
            text: "抄送...", margin: '5 5 0 0',
            width:65,
            listeners: {
                "click": function (btn, e, eOpts) {//添加点击按钮事件
                    Ext.require('Ext.ux.Common.comm', function () {
                        showSelectUserWin("getUser", "", "", function () {
                            if (window.parent.resultvalue != undefined && window.parent.resultvalue != "") {
                                if (me.CCUserList != "") {
                                    me._CCUsersText.setValue(me._CCUsersText.value + "," + window.parent.usernamelist);
                                    me.CCUserList = me.CCUserList + "," + window.parent.resultvalue;
                                } else {
                                    me._CCUsersText.setValue( window.parent.usernamelist);
                                    me.CCUserList =  window.parent.resultvalue;
                                }
                            }
                        });
                    })
                }
            }
        });

        me.attaBtn= Ext.create("Ext.button.Button", {
            xtype: "button",
            text: "附件...", margin: '0 5 0 0',
            width: 65,
            listeners: {
                "click": function (btn, e, eOpts) {//添加点击按钮事件
                    Ext.require('Ext.ux.Common.comm', function () {
                        showSelectDocAndProjectWin("getDoc", function () {
                            var docArray = window.parent.resultvalue;
                            var recordList = "";
                            //me.filestore.removeAll();

                            for (var i = 0; i < docArray.length ; i++) {
                                recordList = (i === 0 ? recordList : recordList + ",");

                                recordList = recordList + docArray[i].Keyword;

                                var rowlength = me.filegrid.getStore().data.length;

                                var r = Ext.create('filemodel', {
                                    id: docArray[i].Keyword,
                                    desc: docArray[i].Title
                                });

                                me.filegrid.getStore().insert(rowlength, r);

                            }
                            //me.MessageAttaList = recordList;

                        });
                    })
                }
            }
        });


        me.contextmenu = Ext.create('Ext.menu.Menu', {
            float: true,
            items: [{
                text: '删除', //删除附件
                action: 'submenu1',
                iconCls: 'leaf', handler: function () {
                    me.removeFileMenuOnClick();
                }
            }]
        });

        me.tbar = Ext.create("Ext.Toolbar", {
            items: [

             me.sendMsgBtn,
             "-",
             me.replyBtn,
             "-",
             me.newMsgBtn,
             me.transmitBtn,
             me.draftMessageBtn
            ]
        });

        //添加发送方text
        me._SenderText = Ext.create("Ext.form.field.Text", {
            fieldLabel: "发送方", frame: true, labelWidth: 70,
            labelAlign: "right", margin: '5 5 0 0', readOnly: true
        });

        //添加接收方text
        me._RecUsersText = Ext.create("Ext.form.field.Text", {
            //fieldLabel: "接收方",
            frame: true,flex:1, //labelWidth: 50,
            labelAlign: "right", margin: '5 0 0 0', readOnly: true
        });

        //添加抄送方text
        me._CCUsersText = Ext.create("Ext.form.field.Text", {
            //fieldLabel: "抄送...",
            flex: 1, frame: true, //labelWidth: 50,
            labelAlign: "right", margin: '5 0 0 0', readOnly: true
        });

        //添加标题text
        me._TitleText = Ext.create("Ext.form.field.Text", {
            fieldLabel: "标题", frame: true, labelWidth: 70,
            labelAlign: "right", margin: '5 5 0 0'//, readOnly: true
        });

        //添加消息内容Text
        me._MessageContentText = Ext.create("Ext.form.field.TextArea", {
            name: 'msg',
            margin: '5 5 5 5',
            labelAlign: "right",flex:1
        });

        me._MessagesContentStore = Ext.create("Ext.data.Store", {

            model: 'CDMSWeb.model.MessageContent',//模型路径：\simplecdms\scripts\app\model\MessageContent.js
            batchActions: false,
            //文章的Store需要支持远程排序和搜索
            remoteFilter: true,
            remoteSort: true,
            //无限滚动需要//
            sorters: {
                property: 'lastpost',
                direction: 'DESC'
            },

            //每50条记录为一页
            pageSize: 50,

            proxy: {
                type: "ajax",
                url: 'WebApi/Get',
                extraParams: {
                    C: "AVEVA.CDMS.WebApi.MessageController", A: "GetMessage",
                    MessageKeyword: 1, total: 50000, sid: localStorage.getItem("sid")
                },
                reader: {
                    type: 'json',
                    totalProperty: 'total',
                    root: "data",//从C#MVC获取数据\simplecdms\controllers\ProjectController.cs .GetDocList.data  ，获取到的数据传送到model里面
                    messageProperty: "Msg"
                },
                writer: {
                    type: "json",
                    encode: true,
                    root: "data",
                    allowSingle: false
                },
                listeners: {
                    exception: CDMSWeb.ProxyException
                }
            },
            simpleSortMode: true
        });


        //定义流程TAB页
        me.msgWorkFlowTabPage = Ext.create('Ext.ux.YDForm.WorkFlow._WorkFlowPage');

        me.msgWorkFlowTabPage.mainPanelType = "Message";

        me.descColumn = Ext.create("Ext.grid.column.Column", {
            text: '名称', dataIndex: 'desc', flex:1
        });

        //定义已上传附件的model
        Ext.define("filemodel", {
            extend: "Ext.data.Model",
            fields: ["id", "no", "name", "code",  "desc",
              "Keyword"
            ],
            url: "_blank",
        });

        //定义附件的store
        me.filestore = Ext.create("Ext.data.Store", {
            model: "filemodel"
            //model: "CDMSWeb.plug_ins.HXEPC_Plugins.model.filemodel"
        });

        //定义附件表格的view
        me.filegrid = Ext.widget("grid", {
            region: "center",
            height: 438,
            hideHeaders: true,//隐藏表头
            selType: "checkboxmodel",
            selModel: { checkOnly: false, mode: "MULTI" },
            //tbar: me.fileTbar,
            flex:1,
            height: 80,
            store: me.filestore,
            listeners: {
                "edit": function (editor, e) {//去除红色箭头
                    e.record.commit();
                },
                "itemcontextmenu": function (view, record, item, index, e, eOpts) {//添加右键菜单事件
                    me._showContextMenu(view, record, item, index, e);
                },
            },
            columns: [
                //me.codeColumn,
                me.descColumn
            ]

        });

        
        me.recUserFieldSet = Ext.create('Ext.form.FieldSet', {
            layout: "hbox",
            // width: '100%',
            baseCls: 'my-panel-no-border',//隐藏边框
            align: 'stretch',
            margin: '5 5 0 5', padding: '0 0 0 0',
            pack: 'start',
            items: [

                //{
                //    xtype: "button",
                //    text: "接收方...", margin: '5 5 0 0',
                //    width: 65,
                //    listeners: {
                //        "click": function (btn, e, eOpts) {//添加点击按钮事件
                //            Ext.require('Ext.ux.Common.comm', function () {
                //                showSelectUserWin("getUser", "", "", function () {
                //                    if (window.parent.resultvalue != undefined && window.parent.resultvalue != "") {
                //                        if (me.recUserList != "") {
                //                            me._RecUsersText.setValue(me._RecUsersText.value + "," + window.parent.usernamelist);
                //                            me.recUserList = me.recUserList + "," + window.parent.resultvalue;
                //                        } else {
                //                            me._RecUsersText.setValue(window.parent.usernamelist);
                //                            me.recUserList = window.parent.resultvalue;
                //                        }
                //                    }
                //                });
                //            })
                //        }
                //    }
                //},
                me.recUserBtn,
                me._RecUsersText
            ]
        });


        me.ccUserFieldSet = Ext.create('Ext.form.FieldSet', {
            layout: "hbox",
            // width: '100%',
            baseCls: 'my-panel-no-border',//隐藏边框
            align: 'stretch',
            margin: '5 5 0 5', padding: '0 0 0 0',
            pack: 'start',
            items: [

                //{
                //    xtype: "button",
                //    text: "抄送...", margin: '5 5 0 0',
                //    width:65,
                //    listeners: {
                //        "click": function (btn, e, eOpts) {//添加点击按钮事件
                //            Ext.require('Ext.ux.Common.comm', function () {
                //                showSelectUserWin("getUser", "", "", function () {
                //                    if (window.parent.resultvalue != undefined && window.parent.resultvalue != "") {
                //                        if (me.CCUserList != "") {
                //                            me._CCUsersText.setValue(me._CCUsersText.value + "," + window.parent.usernamelist);
                //                            me.CCUserList = me.CCUserList + "," + window.parent.resultvalue;
                //                        } else {
                //                            me._CCUsersText.setValue( window.parent.usernamelist);
                //                            me.CCUserList =  window.parent.resultvalue;
                //                        }
                //                    }
                //                });
                //            })
                //        }
                //    }
                //},
                me.ccUserBtn,
               me._CCUsersText
            ]
        });
        

        me.attaFieldSet = Ext.create('Ext.form.FieldSet', {
            layout: "hbox",
            // width: '100%',
            baseCls: 'my-panel-no-border',//隐藏边框
            align: 'stretch',
            margin: '5 5 0 5', padding: '0 0 0 0',
            pack: 'start',
            items: [


                      //{
                      //    xtype: "button",
                      //    text: "附件...", margin: '0 5 0 0',
                      //    width: 65,
                      //    listeners: {
                      //        "click": function (btn, e, eOpts) {//添加点击按钮事件
                      //            //Ext.require('Ext.ux.Common.comm', function () {
                      //            //    showSelectUserWin("getUser", "", "", function () {
                      //            //        //me.checkerText.setValue(window.parent.usernamelist);
                      //            //        //me.checkerList = window.parent.resultvalue;
                      //            //    });
                      //            //})
                      //        }
                      //    }
                      //},
                    me.attaBtn,
                    , me.filegrid
             
            ]

        });


        me.mainmessagepanel = Ext.create('Ext.Panel', {
            xtype: 'panel',
            //layout: 'fit',
            layout: {
                type: 'vbox',
                align: 'stretch',
                pack: 'start'
            },
            height:'100%',
            activeTab: 0,
            baseCls: 'my-panel-no-border',//隐藏边框
            //defaults: {
            //    border: false,
            //    bodyPadding: 5, bodyStyle: "background:#DFE9F6"
            //},
            items: [
                {
                    layout: "hbox",
                    // width: '100%',
                    height: '100%',
                    align: 'stretch',
                    pack: 'start',
                    baseCls: 'my-panel-no-border',//隐藏边框
                    items: [
                        {
                            xtype: "panel",
                            layout: {
                                type: 'vbox',
                                align: 'stretch'
                            },
                            baseCls: 'my-panel-no-border',//隐藏边框
                            height: '100%',
                            defaults: {
                                frame: true,
                                //labelWidth: 50,
                                labelAlign: "right",
                                margin: '5 5 0 0',
                                readOnly: true,

                            },
                            items: [
                            me._SenderText,

                            //{
                            //    layout: "hbox",
                            //    baseCls: 'my-panel-no-border',//隐藏边框
                            //    items: [

                            //        {
                            //            xtype: "button",
                            //            text: "接收方...", margin: '5 5 0 5',
                            //            width: 65,
                            //            listeners: {
                            //                "click": function (btn, e, eOpts) {//添加点击按钮事件
                            //                    Ext.require('Ext.ux.Common.comm', function () {
                            //                        showSelectUserWin("getUser", "", "", function () {
                            //                            //me.checkerText.setValue(window.parent.usernamelist);
                            //                            //me.checkerList = window.parent.resultvalue;
                            //                        });
                            //                    })
                            //                }
                            //            }
                            //        },
                            //        me._RecUsersText
                            //    ]
                            //},
                            me.recUserFieldSet,

                            //{
                            //    layout: "hbox",
                            //    baseCls: 'my-panel-no-border',//隐藏边框
                            //    items: [

                            //        {
                            //            xtype: "button",
                            //            text: "抄送...", margin: '5 5 0 5',
                            //            width:65,
                            //            listeners: {
                            //                "click": function (btn, e, eOpts) {//添加点击按钮事件
                            //                    Ext.require('Ext.ux.Common.comm', function () {
                            //                        showSelectUserWin("getUser", "", "", function () {
                            //                            //me.checkerText.setValue(window.parent.usernamelist);
                            //                            //me.checkerList = window.parent.resultvalue;
                            //                        });
                            //                    })
                            //                }
                            //            }
                            //        },
                            //       me._CCUsersText
                            //    ]
                            //},
                            me.ccUserFieldSet,

                            me._TitleText,

                            me.attaFieldSet,//附件

                            me._MessageContentText
 

                            ], flex: 1
                        }
                    ], flex: 1
                }
           //, me.msgWorkFlowTabPage
            ], flex: 1
        });

        //添加属性TAB页面到容器
        me.items = [
            Ext.widget('form', {
                baseCls: 'my-panel-no-border',//隐藏边框
                layout: {
                    type: 'vbox',
                    align: 'stretch',
                    pack  : 'start'
                },
                items: [
             me.mainmessagepanel
                ]})
        ];

        me.callParent(arguments);
    },

    //显示选中的消息
    displayMessAttr: function (record) {
        var me = this;

        if (me.cuFormType === me.enMessageFormType.NewMessage) {
            //新消息
            //禁用按钮
            me.replyBtn.setDisabled(true);
            me.transmitBtn.setDisabled(true);

            //启用按钮
            me.sendMsgBtn.setDisabled(false);
            me.recUserBtn.setDisabled(false);
            me.ccUserBtn.setDisabled(false);
            me.attaBtn.setDisabled(false);

            //设置发送方
            me._SenderText.setValue(localStorage.getItem("username") + "__" + localStorage.getItem("userdesc"));

            //设置接收方,原发送人为接收人
            me._RecUsersText.setValue("");

            me.recUserList = ("");

            me._CCUsersText.setValue("");

            me.CCUserList = "";

            //  清空附件
            me._MessagesContentStore.removeAll();
            me.filestore.removeAll();

            //设置标题
            me._TitleText.setValue("");

            //设置内容
            me._MessageContentText.setValue("");

            me.curMessageKeyword = "";

            return;
        }

        me.curRecord = record;

        var Keyword = record.data.Keyword;
        var sender = record.data.Sender;
        var title = record.data.Title;

        var MessKeyword = record.data.Keyword;

        me.curMessageKeyword = MessKeyword;

        me._MessagesContentStore.proxy.extraParams.MessageKeyword = MessKeyword;//把参数传给C#MVC,路径：\simplecdms\controllers\projectcontroller.cs 下的 GetChildProject()
        me._MessagesContentStore.proxy.extraParams.sid = localStorage.getItem("sid");

        me._MessagesContentStore.load({
            callback: function (records, options, success) {

                //if (me.cuFormType === "ReplyMessage") {
                if (me.cuFormType === me.enMessageFormType.ReplyMessage) {
                    //回复消息
                    //禁用按钮
                    me.replyBtn.setDisabled(true);

                    //启用按钮
                    me.sendMsgBtn.setDisabled(false);
                    me.recUserBtn.setDisabled(false);
                    me.ccUserBtn.setDisabled(false);
                    me.attaBtn.setDisabled(false);

                    var strChaoSongs = ""

                    //设置发送方
                    me._SenderText.setValue(localStorage.getItem("username") + "__" + localStorage.getItem("userdesc"));

                    //设置接收方,原发送人为接收人
                        me._RecUsersText.setValue(records[0].data.Sender);

                        me.recUserList = (records[0].data.SenderKeyword);

                        me._CCUsersText.setValue(records[0].data.CCUsers);

                        me.CCUserList = records[0].data.CCUserList;
                        //设置标题
                        me._TitleText.setValue("回复:" + records[0].data.Title);

                        var strContent = "\r\n\r\n\r\n\r\n-----------原始消息------------\r\n"
                            + "发送者:" + records[0].data.Sender + "\r\n"
                            + "发送时间:" + records[0].data.SendDate + "\r\n"
                            + "接收者:" + records[0].data.RecUsers + "\r\n"
                            + "抄送:" + records[0].data.CCUsers + "\r\n"
                            + "主题:" + records[0].data.Title + "\r\n\r\n"
                            + records[0].data.Content;

                        //设置内容
                        me._MessageContentText.setValue(strContent);

                       //设置附件
                        me.setFileStore(records);
                //} else if (me.cuFormType === "ViewMessage") {
                } else if (me.cuFormType ===  me.enMessageFormType.ViewMessage) {
                    //打开消息

                    //禁用按钮
                    me.sendMsgBtn.setDisabled(true);
                    me.recUserBtn.setDisabled(true);
                    me.ccUserBtn.setDisabled(true);
                    me.attaBtn.setDisabled(true);


                    //设置发送方
                    me._SenderText.setValue(records[0].data.Sender);

                    //设置接收方
                    me._RecUsersText.setValue(records[0].data.RecUsers);

                    //设置标题
                    me._TitleText.setValue(records[0].data.Title);

                    //设置内容
                    me._MessageContentText.setValue(records[0].data.Content);

                    //设置附件
                    me.setFileStore(records);
                } else if (me.cuFormType === me.enMessageFormType.TransmitMessage) {
                    //相当于新建
                    //this.SetTransmitMsgData();

                    //禁用按钮
                    me.replyBtn.setDisabled(true);
                    me.transmitBtn.setDisabled(true);

                    //启用按钮
                    me.sendMsgBtn.setDisabled(false);
                    me.recUserBtn.setDisabled(false);
                    me.ccUserBtn.setDisabled(false);
                    me.attaBtn.setDisabled(false);

                    //设置发送方
                    me._SenderText.setValue(localStorage.getItem("username") + "__" + localStorage.getItem("userdesc"));

                    //设置接收方,原发送人为接收人
                    me._RecUsersText.setValue("");

                    me.recUserList = ("");

                    me._CCUsersText.setValue("");

                    me.CCUserList = "";

                    //设置标题
                    me._TitleText.setValue("转发:" + records[0].data.Title);

                    var strContent = "\r\n\r\n\r\n\r\n-----------原始消息------------\r\n"
                        + "发送者:" + records[0].data.Sender + "\r\n"
                        + "发送时间:" + records[0].data.SendDate + "\r\n"
                        + "接收者:" + records[0].data.RecUsers + "\r\n"
                        + "抄送:" + records[0].data.CCUsers + "\r\n"
                        + "主题:" + records[0].data.Title + "\r\n\r\n"
                        + records[0].data.Content;

                    //设置内容
                    me._MessageContentText.setValue(strContent);

                    //设置附件
                    me.setFileStore(records);
                }
                else if (me.cuFormType === me.enMessageFormType.DraftMessage) {
                    //草稿消息
                    //禁用按钮
                    me.replyBtn.setDisabled(true);
                    me.transmitBtn.setDisabled(true);

                    //启用按钮
                    me.sendMsgBtn.setDisabled(false);
                    me.recUserBtn.setDisabled(false);
                    me.ccUserBtn.setDisabled(false);
                    me.attaBtn.setDisabled(false);

                    //设置发送方
                    me._SenderText.setValue(records[0].data.Sender);

                    //设置接收方
                    me._RecUsersText.setValue(records[0].data.RecUsers);

                    me.recUserList = records[0].data.RecUserList;
                    //me.CCUserList = records[0].CCUserList;

                    
                    //设置标题
                    me._TitleText.setValue(records[0].data.Title);

                    //设置内容
                    me._MessageContentText.setValue(records[0].data.Content);

                    //设置附件
                    me.setFileStore(records);
                }



            }
        });



    },

    setFileStore: function (records) {
        var me = this;

        me.filestore.removeAll();

        var rowlength = me.filegrid.getStore().data.length;

        for (var i = 0; i < records.length ; i++) {
            var AttaType = records[i].data.AttaType;
            if (AttaType === "Doc" || AttaType == "Project") {

                var r = Ext.create('filemodel', {
                    id: records[i].data.AttaKeyword,
                    desc: records[i].data.Attachment
                });

                me.filegrid.getStore().insert(rowlength, r);
            }
        }
    },

    sendMessage: function () {
        var me = this;

        //获取标题Text
        var title = me._TitleText.value;

        //获取内容
        var content = me._MessageContentText.value;

        //接收方列表
        var recUserlist = me.recUserList;

        //抄送方列表
        var CCUserlist = me.CCUserList;

        //获取附件列表
        //var jsonArray = [];
        var attaList = "";
        for (var i = 0; i < me.filestore.getCount() ; i++) {
            var record = me.filestore.getAt(i);
            var attaKeyword = record.get('id');
            if (attaKeyword != undefined && attaKeyword != "") {
                //jsonArray.push(attaKeyword);
                if (attaList === "") {
                    attaList = attaKeyword;
                } else {
                    attaList += "," + attaKeyword;
                }
            }
        }

        Ext.MessageBox.wait("正在发送消息，请稍候...", "等待");

        Ext.Ajax.request({

            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.WebApi.MessageController", A: "SendMessage",
                sid: localStorage.getItem("sid"),
                RecUserlist:recUserlist,CCUserlist:CCUserlist,
                Title:title ,Content:content,
                Attalist: attaList,
            },
            success: function (response, options) {

                //获取数据后，更新窗口
                var res = Ext.JSON.decode(response.responseText, true);
                var state = res.success;
                if (state === true) {

                    winSendMessage.close();
                    Ext.Msg.alert("信息", "消息发送成功！");


                } else {
                    var errmsg = res.msg;
                    Ext.Msg.alert("错误信息", errmsg);
                }
            },
            failure: function (response, options) {
                ////Ext.Msg.alert("错误", "连接服务器失败！<br>" + response.responseText);
            }
        })
    },

    draftMessage: function () {
        var me = this;

        //获取标题Text
        var title = me._TitleText.value;

        //获取内容
        var content = me._MessageContentText.value;

        //接收方列表
        var recUserlist = me.recUserList;

        //抄送方列表
        var CCUserlist = me.CCUserList;

        //获取附件列表
        //var jsonArray = [];
        var attaList = "";
        for (var i = 0; i < me.filestore.getCount() ; i++) {
            var record = me.filestore.getAt(i);
            var attaKeyword = record.get('id');
            if (attaKeyword != undefined && attaKeyword != "") {
                //jsonArray.push(attaKeyword);
                if (attaList === "") {
                    attaList = attaKeyword;
                } else {
                    attaList += "," + attaKeyword;
                }
            }
        }

        Ext.MessageBox.wait("正在发送消息，请稍候...", "等待");

        Ext.Ajax.request({

            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.WebApi.MessageController", A: "SaveDraftMessage",
                sid: localStorage.getItem("sid"), MessageKeyword: me.curMessageKeyword,
                RecUserlist: recUserlist, CCUserlist: CCUserlist,
                Title: title, Content: content,
                Attalist: attaList,
            },
            success: function (response, options) {

                //获取数据后，更新窗口
                var res = Ext.JSON.decode(response.responseText, true);
                var state = res.success;
                if (state === true) {

                    winSendMessage.close();
                    Ext.Msg.alert("信息", "成功保存草稿！");


                } else {
                    var errmsg = res.msg;
                    Ext.Msg.alert("错误信息", errmsg);
                }
            },
            failure: function (response, options) {
                ////Ext.Msg.alert("错误", "连接服务器失败！<br>" + response.responseText);
            }
        })
    },


    //右键菜单转到源目录
    removeFileMenuOnClick: function () {//widget, event) {

        var me = this;

        var nodes = me.filegrid.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {
            var grid = me.filegrid;
            var rs = grid.getSelectionModel().getSelection();//获取选择的文档
            if (rs !== null && rs.length > 0) {

                var rec = rs[0];//第一个文档

                me.filegrid.store.remove(rec);

            }
        }
    },

    //显示右键菜单方法
        _showContextMenu: function (view, record, item, index, e, eOpts) {

            var me = this;
            //阻止浏览器默认右键事件
            e.preventDefault();
            e.stopEvent();

            //显示右键菜单
            me.contextmenu.showAt(e.getXY());


        }
});