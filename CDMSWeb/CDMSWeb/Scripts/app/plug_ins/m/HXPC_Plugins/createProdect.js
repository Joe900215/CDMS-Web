Ext.define('Ext.plug_ins.m.HXPC_Plugins.createProdect', {
    extend: 'Ext.container.Container',
    alias: 'widget.createProdect',
    layout: 'fit',
    resultvalue: '', mainPanelId: '',
    projectKeyword: '', WfKeyword: '',
    initComponent: function () {
        var me = this;

        me.newProjectKeyword = '';

        //添加成品编码text
        me.ProjectCodeText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "成品编码", labelWidth: 70, margin: '15 0 0 0',
            anchor: "80%", labelAlign: "left", width: 150//flex: 1
        });

        //添加成品描述text
        me.ProjectDescText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "成品描述", labelWidth: 70, margin: '25 0 0 0',
            anchor: "80%", labelAlign: "left", width: 150//flex: 1
        });

        //添加列表
        me.items = [
          Ext.widget('form', {
              layout: "form",
              items: [

                  {
                      xtype: "panel",
                      baseCls: 'my-panel-no-border',//隐藏边框
                      layout: {
                          type: 'vbox',
                          pack: 'start',
                          align: 'stretch'
                      }, height:'100%',
                      items: [
                          {
                              xtype: "panel",
                              baseCls: 'my-panel-no-border',//隐藏边框
                              layout: {
                                  type: 'vbox',
                                  pack: 'start',
                                  align: 'stretch'
                              },
                              margin: "15 10 35 10",
                              //height:550,
                              items: [
                                  me.ProjectCodeText,
                                   me.ProjectDescText
                                 ],flex:1
                          },
                        {
                            xtype: "panel",
                            layout: "hbox",
                            baseCls: 'my-panel-no-border',//隐藏边框
                            //align: 'right',
                            //pack: 'end',//组件在容器右边
                            height:30,
                            items: [{
                                flex: 1, baseCls: 'my-panel-no-border'//隐藏边框
                            },
                                {
                                    xtype: "button",
                                    text: "确定", width: 60, margin: "0 5 10 5",
                                    listeners: {
                                        "click": function (btn, e, eOpts) {//添加点击按钮事件
                                            me.sendCreateProdect();

                                        }
                                    }
                                },
                                {
                                    xtype: "button",
                                    text: "取消", width: 60, margin: "0 15 10 5",
                                    listeners: {
                                        "click": function (btn, e, eOpts) {//添加点击按钮事件
                                            //Ext.Msg.alert("您展开了目录树节点！！！", "您展开了目录树节点！节点ID:" + me.tempDefnId);
                                            winCreateProdect.close();
                                        }
                                    }
                                }
                            ]
                        }
                      ]
                  }
              ]
          })];

        me.callParent(arguments);
    },

    //新建成品目录
    sendCreateProdect: function () {

        var me = this;

        //获取成品编号Text
        var projectCode = me.ProjectCodeText.value;

        //获取成品描述Text
        var projectDesc = me.ProjectDescText.value;

        var projectAttrJson = Ext.JSON.encode(projectAttr);

        ////获取表单数据，转换成JSON字符串
        var projectAttr =
        [
            { name: 'projectCode', value: projectCode },
            { name: 'projectDesc', value: projectDesc }
        ];

        var projectAttrJson = Ext.JSON.encode(projectAttr);

        Ext.MessageBox.wait("正在创建互提资料单，请稍候...", "等待");

        Ext.Ajax.request({

            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.HXPC_Plugins.Prodect", A: "CreateProdect",
                sid: localStorage.getItem("sid"), ProjectKeyword: me.projectKeyword,
                projectAttrJson: projectAttrJson
            },
            success: function (response, options) {

                //获取数据后，更新窗口
                var res = Ext.JSON.decode(response.responseText, true);
                var state = res.success;
                if (state === true) {
                    Ext.MessageBox.close();//关闭等待对话框
                    var recod = eval(res.data[0]);

                    me.newProjectKeyword = recod.ProjectKeyword;//获取新建的目录id


                    //处理返回事件
                    me.send_createProdect_callback(me.newProjectKeyword, options, "");//, me.projectKeyword, closeWin);

                } else {
                    var errmsg = res.msg;
                    Ext.Msg.alert("错误信息", errmsg);
                }
            }

        })
    },

    //处理创建成品目录的返回
    send_createProdect_callback: function () {
        var me = this;

        me.refreshWin(me.newProjectKeyword, true);
    },

    //刷新表单，参数:parentKeyword:新建的联系单目录
    refreshWin: function (projKeyword, closeWin) {
        var me = this;
        var tree = Ext.getCmp(me.mainPanelId).up('_mainProjectView').down('_mainProjectTree').down('treepanel');//;
        var viewTreeStore = tree.store;

        viewTreeStore.load({
            callback: function (records, options, success) {//添加回调，获取子目录的文件数量
                if (closeWin)
                    winCreateProdect.close();

                //展开目录
                Ext.require('Ext.ux.Common.m.comm', function () {
                    Ext.getCmp('contentPanel').down('_mainProjectTree').ExpendProject(projKeyword);
                });
            }
        });
    }
});