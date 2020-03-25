Ext.define('Ext.plug_ins.HXEPC_Plugins.Project.EditProjectGroup', {
    extend: 'Ext.container.Container',
    alias: 'widget.editProjectGroup',
    layout: 'fit',
    resultvalue: '', mainPanelId: '',
    projectKeyword: '',
    initComponent: function () {
        var me = this;

        var viewTree = Ext.getCmp(me.mainPanelId).up('_mainSourceView').down('_mainProjectTree').down('treepanel');//获取目录树控件ID

        var nodes = viewTree.getSelectionModel().getSelection();//获取已选取的节点

        me.projectCode = '';
        if (nodes !== null && nodes.length > 0) {
            me.projectCode = nodes[0].data.text.split('__')[0];
        }

        ////定义用户组选择Panel
        me._selectGroupUserPanel = Ext.create('Ext.ux.YDForm.User._SelectUserGroupPanel', { GroupType: 'Project', Filter: me.projectCode });

        //这里有标题，下部会出现空白
        me._selectGroupUserPanel.setTitle("");

        //me._selectGroupUserPanel.setGroupType( 'Project');

        ////添加列表
        //me.items = [
        //  Ext.widget('form', {
        //      layout: "form",
        //      items: [

        //          {
        //              xtype: "panel",
        //              baseCls: 'my-panel-no-border',//隐藏边框
        //              layout: {
        //                  type: 'vbox',
        //                  pack: 'start',
        //                  align: 'stretch'
        //              },
        //              items: [
        //          {
        //              xtype: "panel",
        //              baseCls: 'my-panel-no-border',//隐藏边框
        //              layout: {
        //                  type: 'vbox',
        //                  pack: 'start',
        //                  align: 'stretch'
        //              },
        //              layout:"fit",
        //              //height: '100%',
        //              items: [
        //                  me._selectGroupUserPanel
        //              ], flex: 1
        //          },
        //                  {xtype:'button',
        //                  text: "创建目录",
        //                      height:30
        //                  }
        //              ]
        //          }]
        //  })];

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
                           {
                               layout: 'fit',
                               layout: "hbox",
                               height: '100%',
                               items: me._selectGroupUserPanel
                           }
                      ], flex: 1
                  }, {
                      xtype: "panel",
                      layout: "hbox",
                      baseCls: 'my-panel-no-border',//隐藏边框
                      items: [{
                          flex: 1, baseCls: 'my-panel-no-border'//隐藏边框
                      },
                          //{
                          //    xtype: "button",
                          //    text: "新建", width: 60, margins: "5 5 5 5",
                          //    listeners: {
                          //        "click": function (btn, e, eOpts) {//添加点击按钮事件
                          //            me.createUserGroup();
                          //        }
                          //    }
                          //},
                          {
                              xtype: "button",
                              text: "添加组的用户", width: 90, margins: "5 5 5 5",
                              listeners: {
                                  "click": function (btn, e, eOpts) {//添加点击按钮事件
                                      Ext.require('Ext.ux.Common.comm', function () {
                                          showSelectUserWin("getUser", "", "", function () {
                                                 me.addUser();
                                          });
                                      })

                                  }
                              }
                          },
                          {
                              xtype: "button",
                              text: "删除组的用户", width: 90, margins: "5 5 5 5",
                              listeners: {
                                  "click": function (btn, e, eOpts) {//添加点击按钮事件
                                      me.dropUser();
                                  }
                              }
                          },
                          {
                              xtype: "button",
                              text: "取消", width: 60, margins: "5 15 5 5",
                              listeners: {
                                  "click": function (btn, e, eOpts) {//添加点击按钮事件
                                      //Ext.Msg.alert("您展开了目录树节点！！！", "您展开了目录树节点！节点ID:" + me.tempDefnId);
                                      winEditProjectGroup.close();
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



    //删除用户
    dropUser: function () {
        var me = this;

        var viewGrid = me._selectGroupUserPanel.down('treepanel');
        var treeNodes = viewGrid.getSelectionModel().getSelection();//获取已选取的节点
        if (treeNodes === null || treeNodes.length <= 0) {
            Ext.Msg.alert("", "请选择用户组！");
            return;
        }

        var viewGrid = me._selectGroupUserPanel.down('gridpanel');
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
    sendDropUserList: function (groupKeyword, userNodes, userIndex) {
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
                    var viewGrid = me._selectGroupUserPanel.down('gridpanel');
                    viewGrid.store.load();
                }
            },
            failure: function (response, options) {
                ////Ext.Msg.alert("错误", "连接服务器失败！<br>" + response.responseText);
            }
        });
    },

    ////添加用户到用户组
    addUser:function(){
        var me = this;
        var viewTree = me._selectGroupUserPanel.down('treepanel');
        var nodes = viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {
                var groupKeyword = nodes[0].data.id;

                var userKeywords = window.parent.resultvalue.split(',');
                var userNames = window.parent.usernamelist.split(';');
                
                for (var i = 0, l = userKeywords.length; i < l; i++) {
                    var userKeyword = userKeywords[i];
                    var userName = userNames[i];

                    me.sendAddUserList(groupKeyword, userKeyword, userName, 0);
            }
        } else {
            Ext.Msg.alert("", "请选择目标用户组！");
        }
    },
    //添加用户
    sendAddUserList: function (groupKeyword, userKeyword, userName, userIndex) {
        var me = this;
        //var userKeyword = userNodes[userIndex].data.id;
        //var userName = userNodes[userIndex].data.text;
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

                    if (recod.userKeyword != "undefined" && recod.userKeyword === userKeyword) {
                        var targetGrid = me._selectGroupUserPanel.down('gridpanel');

                        var data = [{
                            'id': userKeyword,
                            'text': userName
                        }];
                        targetGrid.store.insert(0, data);//可以自定义在stroe的某个位置插入一行数据。
                    }
                }

                ////继续添加下一个用户
                //if (userIndex < userNodes.length - 1) {
                //    userIndex = userIndex + 1;
                //    me.sendAddUserList(gKeyword, userNodes, userIndex);
                //}
            },
            failure: function (response, options) {
                ////Ext.Msg.alert("错误", "连接服务器失败！<br>" + response.responseText);
            }
        });
    }
});