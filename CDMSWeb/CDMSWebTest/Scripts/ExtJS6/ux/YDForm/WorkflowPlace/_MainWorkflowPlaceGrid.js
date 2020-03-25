/*定义流程列表grid*/
Ext.define('Ext.ux.YDForm.WorkflowPlace._MainWorkflowPlaceGrid', {
    //extend: 'Ext.container.Container',
    extend: 'Ext.panel.Panel',
    alias: 'widget._mainWorkflowPlaceGrid', // 此类的xtype类型为buttontransparent  
    //requires: ['Ext.ux.Common.comm'],
    region: "center",
    layout: 'fit',
    initComponent: function () {
        var me = this;
        me.renderTo = me.el;

        //定义消息表格按钮
        me._msgListTbar = Ext.create('Ext.toolbar.Toolbar', {
            xtype: 'toolbar',
            dock: 'top',
            items: [
               // {
               //     iconCls: "msg-open", scope: me, text: '打开', tooltip: '打开消息', listeners: {
               //         "click": function (btn, e, eOpts) {
               //             me.openWorkflowPlace();

               //         }
               //     }
               // },
               // {
               //     iconCls: "msg-del", scope: me, text: '删除', tooltip: '删除消息', listeners: {
               //         "click": function (btn, e, eOpts) {
               //             me.deleteMessage("false");
 
               //         }
               //     }
               // },
               // {
               //     iconCls: "msg-reply", scope: me, text: '回复', tooltip: '回复消息', listeners: {
               //         "click": function (btn, e, eOpts) {
               //             me.replyMessage();

               //         }
               //     }
               // },
               // {
               //     iconCls: "msg-new", scope: me, text: '创建新消息', tooltip: '创建新消息', listeners: {
               //         "click": function (btn, e, eOpts) {
               //             me.newMessage();

               //         }
               //     }
               // },
               // {
               //     iconCls: "msg_transmit", scope: me, text: '转发', tooltip: '转发消息', listeners: {
               //         "click": function (btn, e, eOpts) {
               //             me.transmitMessage();

               //         }
               //     }
               // },
               //{
               //    iconCls: "msg_read", scope: me, text: '设为已读', tooltip: '设为已读', listeners: {
               //        "click": function (btn, e, eOpts) {
               //            me.setMessageRead();

               //        }
               //    }
               //}
            ]
        });

        me._WorkflowPlacesStore2 = Ext.create("Ext.data.Store", {
            model: 'CDMSWeb.model.WorkflowPlace',//模型路径：\simplecdms\scripts\app\model\content.js
            batchActions: false,
            //文章的Store需要支持远程排序和搜索
            remoteFilter: true,
            remoteSort: true,
            //data: [["普通用户"], ["系统管理员"]],
            //每50条记录为一页
            pageSize: 50,//视图路径：\simplecdms\scripts\app\view\content\view.js
            proxy: {
                type: "ajax",
                url: 'WebApi/Get',
                extraParams: {
                    C: "AVEVA.CDMS.WebApi.WorkFlowController", A: "GetWorkFlowPlaceList",
                    WorkflowType: "/_5", total: 50000, sid: localStorage.getItem("sid")
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

        me._WorkflowPlacesStore = Ext.create("Ext.data.Store", {
            model: 'CDMSWeb.model.WorkflowPlace',//模型路径：\simplecdms\scripts\app\model\content.js
            batchActions: false,
            //文章的Store需要支持远程排序和搜索
            //remoteFilter: true,
            remoteSort: true,
            //data: [["普通用户"], ["系统管理员"]],
            //每50条记录为一页
            pageSize: 50,//视图路径：\simplecdms\scripts\app\view\content\view.js
            proxy: {
                type: "ajax",
                url: 'WebApi/Get',
                extraParams: {
                    //C: "AVEVA.CDMS.WebApi.MessageController", A: "GetMessageList",
                    //MessageType: "/_1", total: 50000, sid: localStorage.getItem("sid")
                    C: "AVEVA.CDMS.WebApi.WorkFlowController", A: "GetWorkFlowPlaceList",
                     WorkflowType: "/_5", total: 50000, sid: localStorage.getItem("sid")
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
            }//,
            //simpleSortMode: true
        });

        ////定义grid消息列表
        me.mainworkflowplacegrid = Ext.widget("grid", {
            title: "流程列表", region: "center",
            _id: "WorkflowPlaceGrid",
            renderTo: Ext.getBody(),
            store: me._WorkflowPlacesStore,//"Messages",
            //无限滚动需要//
            verticalScroller: {
                xtype:'paginggridscroller'
            },
            ////////////////
            //Grid使用了复选框作为选择行的方式
            selType: "checkboxmodel",
            selModel: { checkOnly: false, mode: "MULTI" },
            tbar: me._msgListTbar,
            bbar: new Ext.PagingToolbar({  
                //store: "Messages",
                store: me._WorkflowPlacesStore,
                displayInfo: true,  
                displayMsg: '当前显示{0} - {1}条，共{2}条数据',  
                emptyMsg: "没有记录"  
            })  ,

            columns: [
                { text: '文件名称', dataIndex: 'File',  flex: 1  },
                { text: '流程名称', dataIndex: 'DefWorkFlow', width: 360 },
                { text: '发起人', dataIndex: 'Creater', width: 120 },
                { text: '发送时间', dataIndex: 'CreateDate', width: 120 }
            ],
            listeners: {
                //选中某条记录的事件
                "select": function (rowModel, record, index, eOpts) {
                    me.OnWorkFlowPlacesGridSelect(rowModel, record, index, eOpts);
                }
            }
        });

        me.items = [
            me.mainworkflowplacegrid
                        
        ];


        me.callParent(arguments);
    },

    //选中某条记录的事件
    OnWorkFlowPlacesGridSelect: function (rowModel, record, index, eOpts) {
        var me = this;

        if (typeof (record.data) != 'undefined') {
            //获取消息内容页面.
            var contentView = me.up("_mainWorkflowPlaceView").down("_mainWorkflowPlaceContent");

            contentView.displayMessAttr(record);
        }
    },

    ////打开消息
    //openMessage: function(){
    //    var me = this;

    //    var nodesMsg = me.mainworkflowplacegrid.getSelectionModel().getSelection();//获取已选取消息列表的节点

    //    if (nodesMsg === undefined || nodesMsg.length <= 0) {
    //        Ext.Msg.alert("错误信息", "请选择需要打开的消息！");
    //        return;
    //    }

    //    //UserMessageForm2 msgForm = new UserMessageForm2(m_dbs, mU.message, UserMessageForm2.enMessageFormType.ViewMessage, null, null);

    //    //弹出操作窗口
    //    var _fmMessageOpen = Ext.create('Ext.ux.YDForm.Message._MainMessageSend', { title: "", mainPanelId: me.id });

    //    winSendMessage = Ext.widget('window', {
    //        title: '消息内容',
    //        closeAction: 'hide',
    //        width: 560,
    //        height: 556,
    //        minWidth: 560,
    //        minHeight: 556,
    //        layout: 'fit',
    //        resizable: true,
    //        modal: true,
    //        closeAction: 'close', //close 关闭  hide  隐藏  
    //        items: _fmMessageOpen,
    //        defaultFocus: 'firstName'
    //    });

    //    winSendMessage.show();
    //    //监听子窗口关闭事件
    //    winSendMessage.on('close', function () {

    //    });

    //    var viewTree = me.up("_mainMessageView").down("_mainMessageTree").down('treepanel');

    //    var msgTypes = viewTree.getSelectionModel().getSelection();

    //    var msgType = msgTypes[0].get("text");

    //    if (msgType != undefined && msgType.indexOf("草稿") >= 0) {
    //        _fmMessageOpen.cuFormType = _fmMessageOpen.enMessageFormType.DraftMessage;
    //    } else {
    //        _fmMessageOpen.cuFormType = _fmMessageOpen.enMessageFormType.ViewMessage;
    //    }
    //    _fmMessageOpen.IsReplyMsg = false;

    //    // if (typeof (record.data) != 'undefined') {
    //    //获取消息内容页面
    //    var contentView = me.up("_mainMessageView").down("_mainMessageContent");

    //    _fmMessageOpen.displayMessAttr(nodesMsg[0]);
    //    //  }



    //    if (msgType != undefined && msgType.indexOf("新消息") >= 0) {
    //        //把消息设置为已读
    //        me.sendSetMsgRead(nodesMsg, 0, function () {
    //            me.loadMsgListStore(function () { });
    //        });
    //    }


    //},

    ////删除消息
    //deleteMessage: function (sureDel) {
    //    var me = this;

    //    var nodesMsg = me.mainworkflowplacegrid.getSelectionModel().getSelection();//获取已选取消息列表的节点

    //    if (nodesMsg === undefined || nodesMsg.length <= 0) {
    //        Ext.Msg.alert("错误信息", "请选择需要删除的消息！");
    //        return;
    //    }

    //    sureDel = "true";

    //    Ext.MessageBox.show({
    //        title: '确定删除',
    //        msg: '是否要删除选中的消息？',
    //        buttons: Ext.MessageBox.YESNO,
    //        buttonText: {
    //            yes: "是",
    //            no: "否"
    //        },
    //        fn: function (btn, parentFuctionName) {
    //            if (btn === "yes") {
    //                //Ext.Msg.alert("信息", "删除消息!");

    //                var nodesMsg = me.mainworkflowplacegrid.getSelectionModel().getSelection();//获取已选取消息列表的节点

    //                if (nodesMsg !== null && nodesMsg.length > 0) {
    //                    for (var i = 0; i < nodesMsg.length; i++) {
    //                        me.sendDeleteMsg(nodesMsg, i);
    //                    }

    //                }
    //            }
    //        }
    //    });
    //},

    ////回复消息
    //replyMessage: function () {
    //    var me = this;
    //    var nodesMsg = me.mainworkflowplacegrid.getSelectionModel().getSelection();//获取已选取消息列表的节点

    //    if (nodesMsg === undefined || nodesMsg.length <= 0) {
    //        Ext.Msg.alert("错误信息", "请选择需要回复的消息！");
    //        return;
    //    }

    //    //Ext.Msg.alert("信息", "回复消息!ID:" + nodesMsg[0].data.Keyword);

    //    //var viewTree = me.maindocgrid.up('_mainSourceView').down('_mainProjectTree').down('treepanel');//获取目录树控件ID

    //    //弹出操作窗口
    //    var _fmMessageReply = Ext.create('Ext.ux.YDForm.Message._MainMessageSend', { title: "", mainPanelId: me.id });

    //    winSendMessage = Ext.widget('window', {
    //        title: '消息内容',
    //        closeAction: 'hide',
    //        width: 560,
    //        height: 556,
    //        minWidth: 560,
    //        minHeight: 556,
    //        layout: 'fit',
    //        resizable: true,
    //        modal: true,
    //        closeAction: 'close', //close 关闭  hide  隐藏  
    //        items: _fmMessageReply,
    //        defaultFocus: 'firstName'
    //    });

    //    winSendMessage.show();
    //    //监听子窗口关闭事件
    //    winSendMessage.on('close', function () {

    //    });

    //    //_fmMessageReply.cuFormType = "ReplyMessage";
    //    _fmMessageReply.cuFormType = _fmMessageReply.enMessageFormType.ReplyMessage;
    //    _fmMessageReply.IsReplyMsg = true;

    //    // if (typeof (record.data) != 'undefined') {
    //    //获取消息内容页面
    //    var contentView = me.up("_mainMessageView").down("_mainMessageContent");

    //    _fmMessageReply.displayMessAttr(nodesMsg[0]);
    //    //  }


    //},

    ////转发消息
    //transmitMessage: function () {
    //    var me = this;
    //    var nodesMsg = me.mainworkflowplacegrid.getSelectionModel().getSelection();//获取已选取消息列表的节点

    //    if (nodesMsg === undefined || nodesMsg.length <= 0) {
    //        Ext.Msg.alert("错误信息", "请选择需要转发的消息！");
    //        return;
    //    }

    //    //弹出操作窗口
    //    var _fmMessageTransmit = Ext.create('Ext.ux.YDForm.Message._MainMessageSend', { title: "", mainPanelId: me.id });

    //    winSendMessage = Ext.widget('window', {
    //        title: '消息内容',
    //        closeAction: 'hide',
    //        width: 560,
    //        height: 556,
    //        minWidth: 560,
    //        minHeight: 556,
    //        layout: 'fit',
    //        resizable: true,
    //        modal: true,
    //        closeAction: 'close', //close 关闭  hide  隐藏  
    //        items: _fmMessageTransmit,
    //        defaultFocus: 'firstName'
    //    });

    //    winSendMessage.show();
    //    //监听子窗口关闭事件
    //    winSendMessage.on('close', function () {

    //    });

    //    _fmMessageTransmit.cuFormType = _fmMessageTransmit.enMessageFormType.TransmitMessage;
    //    _fmMessageTransmit.IsReplyMsg = true;

    //    // if (typeof (record.data) != 'undefined') {
    //    //获取消息内容页面
    //    var contentView = me.up("_mainMessageView").down("_mainMessageContent");

    //    _fmMessageTransmit.displayMessAttr(nodesMsg[0]);
    //    //  }


    //},

    ////创建新消息
    //newMessage:  function () {
    //    var me = this;


    //    //弹出操作窗口
    //    var _fmNewMessage = Ext.create('Ext.ux.YDForm.Message._MainMessageSend', { title: "", mainPanelId: me.id });

    //    winSendMessage = Ext.widget('window', {
    //        title: '消息内容',
    //        closeAction: 'hide',
    //        width: 560,
    //        height: 556,
    //        minWidth: 560,
    //        minHeight: 556,
    //        layout: 'fit',
    //        resizable: true,
    //        modal: true,
    //        closeAction: 'close', //close 关闭  hide  隐藏  
    //        items: _fmNewMessage,
    //        defaultFocus: 'firstName'
    //    });

    //    winSendMessage.show();
    //    //监听子窗口关闭事件
    //    winSendMessage.on('close', function () {

    //    });

    //    _fmNewMessage.cuFormType = _fmNewMessage.enMessageFormType.NewMessage;
    //    _fmNewMessage.IsReplyMsg = true;

    //    // if (typeof (record.data) != 'undefined') {
    //    //获取消息内容页面
    //    var contentView = me.up("_mainMessageView").down("_mainMessageContent");

    //    _fmNewMessage.displayMessAttr();
    //    //  }


    //},

    ////向服务器发送删除消息的请求
    //sendDeleteMsg: function ( msgNodes, nodesIndex) {
    //    var me = this;
    //    var msgNode = msgNodes[nodesIndex];
    //    var msgId = msgNodes[nodesIndex].data.Keyword;

    //    sureDel = "true";
    //    Ext.Ajax.request({
    //        url: 'WebApi/Post',
    //        method: "POST",
    //        params: {
    //            C: "AVEVA.CDMS.WebApi.MessageController", A: "DeleteMessage",
    //            MessageKeyword: msgId,
    //            sureDel: sureDel, sid: localStorage.getItem("sid")
    //        },
    //        success: function (response, options) {
    //            var res = Ext.JSON.decode(response.responseText, true);
    //            var state = res.success;
    //            if (state === false) {
    //                var errmsg = res.msg;
    //                Ext.Msg.alert("错误信息", errmsg);
    //            }
    //            else {
    //                var recod = res.data[0];
    //                var State = recod.state;
    //                if (State === "delSuccess") {
    //                    if (msgNodes.length > nodesIndex + 1) {
    //                        me.sendDeleteMsg(msgNodes, nodesIndex + 1);
    //                    } else {
    //                        me.loadMsgListStore(function () { });
    //                    }

    //                }
    //            }
    //        },
    //        failure: function (response, options) {
    //            ////Ext.Msg.alert("错误", "连接服务器失败！<br>" + response.responseText);
    //        }
    //    });
    //},

    ////把消息设为已读
    //setMessageRead: function () {
    //    var me = this;
    //    var msgNodes = me.mainworkflowplacegrid.getSelectionModel().getSelection();//获取已选取消息列表的节点

    //    if (msgNodes === undefined || msgNodes.length <= 0) {
    //        Ext.Msg.alert("错误信息", "请选择需要设为已读的消息！");
    //        return;
    //    }

    //    for (var i = 0; i < msgNodes.length; i++) {
    //        me.sendSetMsgRead(msgNodes, i, function () { });
 
    //    }
    //    me.loadMsgListStore(function () { });


    //},

    //////向服务器发送把消息设为已读的请求
    //sendSetMsgRead: function (msgNodes, nodesIndex,funcCallBack) {
    //    var me = this;

    //    var msgNode = msgNodes[nodesIndex];
    //    var msgId = msgNodes[nodesIndex].data.Keyword;

    //    Ext.Ajax.request({
    //        url: 'WebApi/Post',
    //        method: "POST",
    //        params: {
    //            C: "AVEVA.CDMS.WebApi.MessageController", A: "SetMessageRead",
    //            MessageKeyword: msgId,
    //            sid: localStorage.getItem("sid")
    //        },
    //        success: function (response, options) {
    //            var res = Ext.JSON.decode(response.responseText, true);
    //            var state = res.success;
    //            if (state === false) {
    //                var errmsg = res.msg;
    //                Ext.Msg.alert("错误信息", errmsg);
    //            }
    //            else {
    //                //var recod = res.data[0];
    //                //var State = recod.state;
    //                //if (State === "delSuccess") {
    //                //    if (msgNodes.length > nodesIndex + 1) {
    //                //        me.sendDeleteMsg(msgNodes, nodesIndex + 1);
    //                //    } else {
    //                //        me.loadMsgListStore(function () { });
    //                //    }

    //                //}
    //            }
    //            funcCallBack();
    //        },
    //        failure: function (response, options) {
    //            ////Ext.Msg.alert("错误", "连接服务器失败！<br>" + response.responseText);
    //        }
    //    });
    //},

    //刷新文档列表,docKeyword:加载完成后需要跳转的的文档Keyword
    loadMsgListStore: function (callBackFun) {
        var me = this;
        //refreshMessagePage
        //var viewTree = me.maindocgrid.up('_mainSourceView').down('_mainProjectTree').down('treepanel');//获取目录树控件ID
        var viewTree = me.up("_mainMessageView").down("_mainMessageTree");
        viewTree.refreshMessagePage();
        //var nodes = viewTree.getSelectionModel().getSelection();//获取已选取的节点
        //if (nodes !== null && nodes.length > 0) {
        //    var projectKeyword = nodes[0].data.Keyword;
          
            //获取文档列表
        //var store = me._MessagesStore;//路径：\simplecdms\scripts\app\store\contents.js
        //    store.proxy.extraParams.ProjectKeyWord = projectKeyword;//把参数传给C#MVC,路径：\simplecdms\controllers\projectcontroller.cs 下的 GetChildProject()
        //    store.proxy.extraParams.filterJson = "";
        //    store.proxy.extraParams.sid = localStorage.getItem("sid");
        //    //store.loadPage(1);
        //    store.load({
        //        callback: function (records, options, success) {
        //            //Ext.MessageBox.close();//关闭等待对话框
        //            callBackFun();
        //        }
        //    });
        //}
    },
});