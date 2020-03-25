//选择项目设总
Ext.define('Ext.plug_ins.HXPC_Plugins.selectProjectOwner', {
    extend: 'Ext.container.Container',
    alias: 'widget.selectProjectOwner',
    //layout: "border",
    layout: 'fit',
    resultvalue: '',mainPanelId:'',
    initComponent: function () {
        var me = this;

        me.projectOwnerList = "";

        //记录已选择的目录Keyword
        me.projKeyword = "";

        //设总
        me.projectOwnerText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", readOnly: true, fieldStyle: 'border-color: red; background-image: none;',
            fieldLabel: "设总", anchor: "80%", labelWidth: 55, labelAlign: "left", margin: '0 0 0 0', width: 260//flex: 1
        });

 
        //添加列表
        me.items = [
          Ext.widget('form', {
              layout: "form",
              items: [{
                  xtype: "panel",
                  baseCls: 'my-panel-no-border',//隐藏边框
                  layout: {
                      type: 'vbox',
                      pack: 'start',
                      align: 'stretch'
                  },
                  items: [
                      {
                          layout: "hbox",
                          width: '100%', baseCls: 'my-panel-no-border',//隐藏边框
                          items: [{ baseCls: 'my-panel-no-border', flex: 1 },
                      {
                          xtype: 'label',
                          cls: 'classDiv2',
                          itemId: 'label1',
                          text: '选择项目设总', margins: '0 0 0 10'
                      }, { baseCls: 'my-panel-no-border', flex: 1 }]
                      }
                  ,
                  {
                      xtype: "fieldset", margin: '8',
                      layout: {
                          type: 'vbox',
                          pack: 'start',
                          align: 'stretch'
                      },
                      items: [
                   {
                       layout: "hbox",
                       width: '100%', baseCls: 'my-panel-no-border',//隐藏边框
                       align: 'stretch', margin: '8 0 8 0', padding: '0 0 0 0',
                       pack: 'start',
                       items: [
                           me.projectOwnerText,
                           {
                               xtype: "button",
                               text: "选择...", margins: "0 0 0 10",
                               listeners: {
                                   "click": function (btn, e, eOpts) {//添加点击按钮事件
                                       Ext.require('Ext.ux.Common.comm', function () {
                                           showSelectUserWin("getUser", "", "", function () {
                                               me.projectOwnerText.setValue(window.parent.usernamelist);
                                               me.projectOwnerList = window.parent.resultvalue;
                                           });
                                       })
                                   }
                               }
                           }]
                   }]
                  },
              {
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
                          text: "确定", width: 60, margins: "10 5 10 5",
                          listeners: {
                              "click": function (btn, e, eOpts) {//添加点击按钮事件
                                    me.send_set_project_owner();

                              }
                          }
                      },
                      {
                          xtype: "button",
                          text: "取消", width: 60, margins: "10 15 10 5",
                          listeners: {
                              "click": function (btn, e, eOpts) {//添加点击按钮事件
                                  //Ext.Msg.alert("您展开了目录树节点！！！", "您展开了目录树节点！节点ID:" + me.tempDefnId);
                                  winSelectProjectOwner.close();
                              }
                          }
                      }
                  ]
              }
                  ]
              }]
          })];
        
        me.callParent(arguments);
    },

    //任命项目设总
    send_set_project_owner: function () {
        var me = this;

        var nodes = Ext.getCmp(me.mainPanelId).up('_mainSourceView').down('_mainProjectTree').down('treepanel').getSelectionModel().getSelection();//获取已选取的节点 
        if (nodes !== null && nodes.length > 0) {

            me.projectKeyword = nodes[0].data.Keyword;

            Ext.Ajax.request({

                url: 'WebApi/Post',
                method: "POST",
                params: {
                    C: "AVEVA.CDMS.HXPC_Plugins.EnterPoint", A: "SetProjectOwner",
                    sid: localStorage.getItem("sid"),
                    prjoectKeyword: me.projectKeyword, userlist: me.projectOwnerList
                },
                success: function (response, options) {
                    me.send_set_project_owner_callback(response, options, "");//, me.projectKeyword, closeWin);
                }
            })
        }
    },

    //处理任命项目设总服务端返回事件
    send_set_project_owner_callback : function (response, options){
        var me = this;
        var res = Ext.JSON.decode(response.responseText, true);
        var state = res.success;
        if (state === false) {
            var errmsg = res.msg;
            Ext.Msg.alert("错误信息", errmsg);
        }
        else {
            var recod = res.data[0];
            var passState = recod.state;
            if (passState === "selectUser") {

                var workflowKeyword = recod.wfKeyword;
                //跳转到下一状态
                Ext.require('Ext.ux.Common.comm', function () {
                    GotoNextWorkflowState("选择项目设总", workflowKeyword, "", function () {
                        //回调函数，通过流程分支
                        winSelectProjectOwner.close();

                        me.refreshWin(me.projectKeyword, false);
                    });
                })


            }

        }
    },

    //刷新表单，参数:parentKeyword:新建的联系单目录
    refreshWin: function (parentKeyword, closeWin) {
        var me = this;
        var tree = Ext.getCmp(me.mainPanelId).up('_mainSourceView').down('_mainProjectTree').down('treepanel');//;
    var viewTreeStore = tree.store;

    viewTreeStore.load({
        callback: function (records, options, success) {//添加回调，获取子目录的文件数量
            if (closeWin)
                winSelectProjectOwner.close();

            //展开目录
            Ext.require('Ext.ux.Common.comm', function () {
                Ext.getCmp('contentPanel').down('_mainProjectTree').ExpendProject(me.projectKeyword);
            });
        }
    });
}
    
});