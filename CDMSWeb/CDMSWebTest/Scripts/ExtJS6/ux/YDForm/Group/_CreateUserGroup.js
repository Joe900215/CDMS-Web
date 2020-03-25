//新建目录
Ext.define('Ext.ux.YDForm.Group._CreateUserGroup', {
    extend: 'Ext.container.Container',
    alias: 'widget._CreateUserGroup',
    //layout: "border",
    layout: 'fit',
    resultvalue: '', mainPanelId: '',
    parentPanelId: '',
    initComponent: function () {
        var me = this;

        //定义用户组代码Text
        me.groupCodeText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "代码", anchor: "80%", labelWidth: 50, labelAlign: "left", width: 130, margins: "15 8 8 8", fieldStyle: 'border-color: red; background-image: none;',//红色边框//flex: 1
            data: []
        });

        me.groupDescText = Ext.create("Ext.form.field.Text",
        {
            xtype: "textfield",
            fieldLabel: "描述", anchor: "80%", labelWidth: 50, labelAlign: "left", width: 130, margins: "8 8 8 8"
        });

        //添加列表
        me.items = [
          Ext.widget('form', {
              layout: "form",
              items: [
                  {
                      xtype: "panel",
                      layout: "hbox",
                      baseCls: 'my-panel-no-border',//隐藏边框
                      //align: 'right',
                      //pack: 'end',//组件在容器右边
                      items: [
                       {
                           xtype: "fieldset", margin: '8', //title: "项目类型",
                           width: "100%",
                           layout: {
                               type: 'vbox',
                               pack: 'start',
                               align: 'stretch'
                           },
                           items: [
                                me.groupCodeText,
                                me.groupDescText
                           ], flex: 1
                       }]
                  }
                      , {
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
                                  text: "确定", width: 60, margins: "5 5 5 5",
                                  listeners: {
                                      "click": function (btn, e, eOpts) {//添加点击按钮事件
                                          //me.send_create_A10();
                                          me.CreateUserGroup();
                                      }
                                  }
                              },
                              {
                                  xtype: "button",
                                  text: "取消", width: 60, margins: "5 15 5 5",
                                  listeners: {
                                      "click": function (btn, e, eOpts) {//添加点击按钮事件
                                          //Ext.Msg.alert("您展开了目录树节点！！！", "您展开了目录树节点！节点ID:" + me.tempDefnId);
                                          winCreateUserGroup.close();
                                      }
                                  }
                              }
                          ]
                      }
              ]

          })];

        me.callParent(arguments);
    },

    //创建用户组
    CreateUserGroup: function () {
        var me = this;
        var viewTree = Ext.getCmp(me.parentPanelId).down('treepanel');
        var nodes = viewTree.getSelectionModel().getSelection();//获取已选取的节点
        var groupCode=me.groupCodeText.value;
        var groupDesc = me.groupDescText.value;
        var autoCode=0;

        Ext.require('Ext.ux.Common.comm', function () {
            
            sendCreateUserGroup(nodes, groupCode, groupDesc,autoCode, function () {
                winCreateUserGroup.close();
            });
        });

        //if (nodes !== null && nodes.length > 0) {
        //    Ext.Ajax.request({
        //        url: 'WebApi/Post',
        //        method: "POST",
        //        params: {
        //            C: "AVEVA.CDMS.WebApi.GroupController", A: "NewGroup",
        //            sid: localStorage.getItem("sid"),groupCode:me.groupCodeText.value, 
        //            groupDesc: me.groupDescText.value, parentGroup: nodes[0].data.id
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
        //                //添加节点
        //                var newnode = [{ id: recod.groupKeyword, text: recod.groupName, leaf: true }];  //新节点信息  
        //                var pnode = nodes[0]; //查找父节点
        //                if (Ext.isEmpty(pnode)) //如果没有父节点，则pnode为根节点  
        //                {
        //                    pnode = viewTree.store.getRootNode();
        //                }
        //                pnode.appendChild(newnode); //添加子节点  
        //                pnode.set('leaf', false);
        //                pnode.expand();

        //                winCreateUserGroup.close();

        //            }
        //        }
        //    });
        //}
    }
});