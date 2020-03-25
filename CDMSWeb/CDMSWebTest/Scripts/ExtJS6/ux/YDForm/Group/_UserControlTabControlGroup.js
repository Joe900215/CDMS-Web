Ext.define('Ext.ux.YDForm.Group._UserControlTabControlGroup', {
    extend: 'Ext.container.Container',
    alias: 'widget._UserControlTabControlGroup',
    layout: 'fit',
    resultvalue: '',

    initComponent: function () {
        var me = this;


        //定义用户组选择Panel
        me._sourceGroupUserPanel = Ext.create('Ext.ux.YDForm.User._SelectUserGroupPanel');

        //这里有标题，下部会出现空白
        me._sourceGroupUserPanel.setTitle("");

        //不显示子用户组的用户
        me._sourceGroupUserPanel.setUserListRange("false");

        //定义用户组选择Panel
        me._targetGroupUserPanel = Ext.create('Ext.ux.YDForm.User._SelectUserGroupPanel');

        //这里有标题，下部会出现空白
        me._targetGroupUserPanel.setTitle("");

        //不显示子用户组的用户
        me._targetGroupUserPanel.setUserListRange("false");
  
        //中间容器
        me.topMiddlePanel = Ext.widget("container", {
            //  xtype: "container",//中间容器
            autoEl: "div",
            autoheight: true,
            width: 90,
            layout: {
                type: 'vbox',
                align: 'stretch',
                pack: 'start',
            }, items: [{ height: 80 }, {
                xtype: "button",
                height: 80,
                margins: '0,5,0,5',
                text: "----><br/>添加用户",
                listeners: {
                    "click": function (btn, e, eOpts) {//添加点击按钮事件
                        me.AddUserList();
                    }
                }
            }
            , {
                xtype: "button",
                height: 80,
                margins: '0,5,0,5',
                text: "----><br/>添加用户组",
                listeners: {
                    "click": function (btn, e, eOpts) {//添加点击按钮事件
                        me.AddGroup();
                    }
                }
            }

            ], flex: 1
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
              items: [
                  {//上部容器
                      baseCls: 'my-panel-no-border',//隐藏边框
                      layout: {
                          type: 'hbox',
                          align: 'stretch',
                          pack: 'start'
                      },
                      items: [
                          //me.selectTab,
                          //me._sourceSelectUserTab,
                         // me._sourceGroupUserPanel,
                           {
                               layout: 'fit',
                               layout: "hbox",
                               //width: 400,
                               height: '100%',
                               //baseCls: 'my-panel-no-border',//隐藏边框
                               items: me._sourceGroupUserPanel,
                               flex: 1
                           },
                          {
                              layout: 'fit',
                              items: me.topMiddlePanel, width: 90
                          },
                          {
                              layout: 'fit', height: '100%',
                              //baseCls: 'my-panel-no-border',//隐藏边框
                              items: me._targetGroupUserPanel,
                              flex: 1
                          }
                         // me._targetSelectUserTab
                          //
                          //{
                          //    layout: 'border',
                          //    baseCls: 'my-panel-no-border',//隐藏边框
                          //    items: me._targetSelectUserTab,//me.targetGroupPanel, 
                          //    flex: 1
                          //}
                      ], flex: 1
                  },{
                  xtype: "panel",
                  layout: "hbox",
                  baseCls: 'my-panel-no-border',//隐藏边框
                  //align: 'right',
                  //pack: 'end',//组件在容器右边
                  items: [{
                      flex: 1, baseCls: 'my-panel-no-border'//隐藏边框
                  },
                      {
                          xtype: "button",
                          text: "新建", width: 60, margins: "5 5 5 5",
                          listeners: {
                              "click": function (btn, e, eOpts) {//添加点击按钮事件
                                  //me.send_create_A10();
                                  me.createUserGroup();
                              }
                          }
                      },
                      {
                          xtype: "button",
                          text: "删除用户组", width: 90, margins: "5 5 5 5",
                          listeners: {
                              "click": function (btn, e, eOpts) {//添加点击按钮事件
                                  me.sendDropGroup();
                              }
                          }
                      },
                      {
                          xtype: "button",
                          text: "删除组的用户", width: 90, margins: "5 5 5 5",
                          listeners: {
                              "click": function (btn, e, eOpts) {//添加点击按钮事件
                                  me.DropUser();
                              }
                          }
                      },
                      {
                          xtype: "button",
                          text: "取消", width: 60, margins: "5 15 5 5",
                          listeners: {
                              "click": function (btn, e, eOpts) {//添加点击按钮事件
                                  //Ext.Msg.alert("您展开了目录树节点！！！", "您展开了目录树节点！节点ID:" + me.tempDefnId);
                                  winAdminSetting.close();
                              }
                          }
                      }
                  ]
              }
     ]
          })

        ];

        me.callParent(arguments);
    },

    //创建用户组
    createUserGroup: function () {
        var me = this;

        var me = this;
        var viewTree = me._targetGroupUserPanel.down('treepanel');
        var nodes = viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {
            //弹出操作窗口
            var _fmCreateUserGroup = Ext.create('Ext.ux.YDForm.Group._CreateUserGroup', { title: "", mainPanelId: me.mainPanelId, parentPanelId: me._targetGroupUserPanel.id });

            winCreateUserGroup = Ext.widget('window', {
                title: '新建用户组',
                closeAction: 'hide',
                width: 417,
                height: 213,
                minWidth: 300,
                minHeight: 200,
                layout: 'fit',
                resizable: true,
                modal: true,
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: _fmCreateUserGroup,
                defaultFocus: 'firstName'
            });

            winCreateUserGroup.show();
            //监听子窗口关闭事件
            winCreateUserGroup.on('close', function () {
            });
        } else {
            Ext.Msg.alert("", "请选择用户组！");
        }
    },

    //删除用户组
    sendDropGroup: function () {
        var me = this;

        var viewTree = me._targetGroupUserPanel.down('treepanel');
        var nodes = viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {
            Ext.MessageBox.show({
                title: '提示',
                msg: '是否删除选中项？',
                buttons: Ext.MessageBox.YESNO,
                buttonText: {
                    yes: "是",
                    no: "否"
                },
                fn: function (btn) {
                    if (btn === "yes") {

                        Ext.Ajax.request({
                            url: 'WebApi/Post',
                            method: "POST",
                            params: {
                                C: "AVEVA.CDMS.WebApi.GroupController", A: "DropGroup",
                                sid: localStorage.getItem("sid"), groupKeyword: nodes[0].data.id
                            },
                            success: function (response, options) {
                                var res = Ext.JSON.decode(response.responseText, true);
                                var state = res.success;
                                if (state === false) {
                                    var errmsg = res.msg;
                                    Ext.Msg.alert("错误信息", errmsg);
                                }
                                else {
                                    var recod = res.data[0];

                                    var node = nodes[0];
                                    //删除目录树里面的节点
                                    node.removeAll();
                                    node.remove();

                                }
                            },
                            failure: function (response, options) {
                                ////Ext.Msg.alert("错误", "连接服务器失败！<br>" + response.responseText);
                            }
                        });
                    }
                }

            });
        } else {
            Ext.Msg.alert("", "请选择用户组！");
        }

    },

    //删除用户
    DropUser: function () {
        var me = this;

        var viewGrid = me._targetGroupUserPanel.down('treepanel');
        var treeNodes = viewGrid.getSelectionModel().getSelection();//获取已选取的节点
        if (treeNodes === null || treeNodes.length <= 0) {
            Ext.Msg.alert("", "请选择用户组！");
            return;
        }

        var viewGrid = me._targetGroupUserPanel.down('gridpanel');
        var gridNodes = viewGrid.getSelectionModel().getSelection();//获取已选取的节点
        if (gridNodes === null || gridNodes.length <= 0) {
            Ext.Msg.alert("", "请选择用户！");
            return;
        }

        Ext.MessageBox.show({
            title: '提示',
            msg: '是否删除选中项？',
            buttons: Ext.MessageBox.YESNO,
            buttonText: {
                yes: "是",
                no: "否"
            },
            fn: function (btn) {
                if (btn === "yes") {
                    var groupKeyword = treeNodes[0].data.id;
                    me.sendDropUserList(groupKeyword, gridNodes, 0);
                }
            }
        });
    },

    //添加用户
    sendDropUserList: function (groupKeyword,userNodes,userIndex) {
        var me = this;
        var userKeyword = userNodes[userIndex].data.id;
        var userName = userNodes[userIndex].data.text;
        var gKeyword = groupKeyword;

        Ext.Ajax.request({
            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.WebApi.GroupController", A: "DropUser",
                sid: localStorage.getItem("sid"), groupKeyword: gKeyword,
                userKeyword: userKeyword
            },
            success: function (response, options) {
                var res = Ext.JSON.decode(response.responseText, true);
                var state = res.success;
                if (state === false) {
                    var errmsg = res.msg;
                    Ext.Msg.alert("错误信息", errmsg);
                }
                else {
                    var recod = res.data[0];

                }
                
                if (userIndex < userNodes.length - 1) {
                    //继续删除下一个用户
                    userIndex = userIndex + 1;
                    me.sendDropUserList(gKeyword, userNodes, userIndex);
                } else {
                    //删除完毕刷新表格
                    var viewGrid = me._targetGroupUserPanel.down('gridpanel');
                    viewGrid.store.load();
                }
            },
            failure: function (response, options) {
                ////Ext.Msg.alert("错误", "连接服务器失败！<br>" + response.responseText);
            }
        });
    },

    //添加用户列表
    AddUserList: function () {
        var me = this;

        var viewTree = me._targetGroupUserPanel.down('treepanel');
        var nodes = viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {
            var sourceGrid = me._sourceGroupUserPanel.down('gridpanel');
            var userNodes = sourceGrid.getSelectionModel().getSelection();//获取已选取的节点
            if (userNodes  !== null && userNodes .length > 0) {
                var groupKeyword=nodes[0].data.id;
                me.sendAddUserList(groupKeyword,userNodes ,0);
            }
        } else {
            Ext.Msg.alert("", "请选择目标用户组！");
        }

    },

    //添加用户
    sendAddUserList: function (groupKeyword,userNodes,userIndex) {
        var me=this;
        var userKeyword = userNodes[userIndex].data.id;
        var userName = userNodes[userIndex].data.text;
        var gKeyword = groupKeyword;

        Ext.Ajax.request({
            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.WebApi.GroupController", A: "AddUser",
                sid: localStorage.getItem("sid"), groupKeyword: groupKeyword,
                userKeyword: userKeyword
            },
            success: function (response, options) {
                var res = Ext.JSON.decode(response.responseText, true);
                var state = res.success;
                if (state === false) {
                    var errmsg = res.msg;
                    Ext.Msg.alert("错误信息", errmsg);
                }
                else {
                    var recod = res.data[0];

                    if (recod.userKeyword != "undefined" && recod.userKeyword === userKeyword)
                    {
                        var targetGrid = me._targetGroupUserPanel.down('gridpanel');

                        var data = [{
                            'id': userKeyword,
                            'text': userName
                        }];
                        targetGrid.store.insert(0, data);//可以自定义在stroe的某个位置插入一行数据。
                    }
                }

                //继续添加下一个用户
                if (userIndex < userNodes.length - 1) {
                    userIndex = userIndex + 1;
                    me.sendAddUserList(gKeyword, userNodes, userIndex);
                }
            },
            failure: function (response, options) {
                ////Ext.Msg.alert("错误", "连接服务器失败！<br>" + response.responseText);
            }
        });
    },

    //复制添加用户组
    AddGroup: function () {
        var me = this;

        var viewTree = me._targetGroupUserPanel.down('treepanel');
        var targetTreeNodes = viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (targetTreeNodes !== null && targetTreeNodes.length > 0) {
            var sourceTree = me._sourceGroupUserPanel.down('treepanel');
            var sourceTreeNodes = sourceTree.getSelectionModel().getSelection();//获取已选取的节点
            if (sourceTreeNodes !== null && sourceTreeNodes.length > 0) {
                me.sendAddGroup(targetTreeNodes, sourceTreeNodes, 0);
            }
        } else {
            Ext.Msg.alert("", "请选择目标用户组！");
        }
    },

    ////创建用户组
    sendAddGroup: function (targetTreeNodes, sourceTreeNodes, userIndex) {
        var me = this;
        var viewTree = me._targetGroupUserPanel.down('treepanel');
        var nodes = viewTree.getSelectionModel().getSelection();//获取已选取的节点

        //将要添加的用户组名称
        var strs = sourceTreeNodes[0].data.text.split('__')
        var groupCode = strs[0];//sourceTreeNodes[0].data.text;
        var groupDesc = strs[1];//"";
        var autoCode = 1;

        Ext.require('Ext.ux.Common.comm', function () {

            sendCreateUserGroup(targetTreeNodes, groupCode, groupDesc,autoCode, function (res) {
                //winCreateUserGroup.close();
                var recod = res.data[0];
                if (recod.groupKeyword != "undefined") {
                    var sourceGrid = me._sourceGroupUserPanel.down('gridpanel');
                    //var userNodes = sourceGrid.getSelectionModel().getSelection();//获取所有用户节点
                    var userNodes = sourceGrid.store.data.items;//,获取出model.selModel.getSelection();//获取所有用户节点
                    //添加用户
                    me.sendAddUserList(recod.groupKeyword, userNodes, 0);
                }
            });
        });


    }
    
});