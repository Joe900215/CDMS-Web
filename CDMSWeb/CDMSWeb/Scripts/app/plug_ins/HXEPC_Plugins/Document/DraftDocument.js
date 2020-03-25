//起草红头文 DraftDocument

Ext.define('Ext.plug_ins.HXEPC_Plugins.Document.DraftDocument', {
    extend: 'Ext.container.Container',
    alias: 'widget.DraftDocument',
    //layout: "border",
    layout: 'fit',
    resultvalue: '', mainPanelId: '', projectKeyword: '',
    initComponent: function () {
        var me = this;

        //定义函件编码Text
        me.documentCodeText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "函件编码", anchor: "80%", labelWidth: 70, labelAlign: "left", width: "100%", //width: 230, 
            margin: '10 10 0 10', fieldStyle: 'border-color: red; background-image: none;'//红色边框//flex: 1
        });


        //定义函件标题Text
        me.titleText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "函件标题", anchor: "80%", labelWidth: 70, labelAlign: "left", width: "100%",//width: 230, 
            margin: '10 10 0 10', fieldStyle: 'border-color: red; background-image: none;'//红色边框//flex: 1
        });

        //定义主送单位Text
        me.deliveryUnitText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "主送单位", anchor: "80%", labelWidth: 70, labelAlign: "left", width: "100%",//width: 230, 
            margin: '10 10 0 10', fieldStyle: 'border-color: red; background-image: none;'//红色边框//flex: 1
        });

        //添加函件正文Text
        me.contentText = Ext.create("Ext.form.field.TextArea", {
            xtype: "textarea", anchor: "80%", labelAlign: "left", margin: '10 10 0 10', //margin: '0 5 5 0',
            width: "100%",//flex:1, //width: 460, //
            height: 270, fieldLabel: "函件正文", labelWidth: 70,
        });

        //定义联系人Text
        me.contactText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "联系人", anchor: "80%", labelWidth: 70, labelAlign: "left", width: "50%",//width: 230, 
            margin: '10 10 0 10', fieldStyle: ' background-image: none;'//红色边框//flex: 1
        });

        //定义联系电话Text
        me.telText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "联系电话", anchor: "80%", labelWidth: 70, labelAlign: "left", width: "50%",//width: 230, 
            margin: '10 10 0 10', fieldStyle: ' background-image: none;'//红色边框//flex: 1
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
                     {
                         layout: "hbox",
                         width: '100%',
                         align: 'stretch',
                         pack: 'start',
                         baseCls: 'my-panel-no-border',//隐藏边框
                         items: [
                            me.documentCodeText,//函件标题

                         ], flex: 1
                     },
                     {
                         layout: "hbox",
                         width: '100%',
                         align: 'stretch',
                         pack: 'start',
                         baseCls: 'my-panel-no-border',//隐藏边框
                         items: [
                            me.titleText,//标题

                         ], flex: 1
                     },
                     {
                         layout: "hbox",
                         width: '100%',
                         align: 'stretch',
                         pack: 'start',
                         baseCls: 'my-panel-no-border',//隐藏边框
                         items: [
                            me.deliveryUnitText//项目名称

                         ], flex: 1
                     } ,{

                      layout: "hbox",
                        width: '100%',
                        align: 'stretch',
                        pack: 'start',
                        height: 240, margin: '0 0 0 0',
                        baseCls: 'my-panel-no-border',//隐藏边框
                        items: [
                             me.contentText  // 函件正文
                        ]//, flex: 1
                     },
                     {
                         layout: "hbox",
                         width: '100%',
                         align: 'stretch',
                         pack: 'start',
                         baseCls: 'my-panel-no-border',//隐藏边框
                         items: [
                            me.contactText,//联系人
                            me.telText  //联系电话

                         ], flex: 1
                     } 
                    

                  ], flex: 1
              },
              {//下部按钮容器
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
                                  me.send_draft_document();
                              }
                          }
                      },
                      {
                          xtype: "button",
                          text: "取消", width: 60, margins: "10 15 10 5",
                          listeners: {
                              "click": function (btn, e, eOpts) {//添加点击按钮事件
                  
                                  winDraftDocument.close();
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

    //向服务器发送起草红头公文请求
    send_draft_document: function () {
        var me = this;

        //获取函件编号Text
        var documentCode = me.documentCodeText.value;

        //获取标题Text
        var title = me.titleText.value;

        //获取主送单位Text
        var deliveryUnit = me.deliveryUnitText.value;

        //获取正文内容Text
        var content = me.contentText.value;

        //获取联系人Text
        var contact = me.contactText.value;

        //获取联系电话Text
        var tel = me.telText.value;

        //获取表单数据，转换成JSON字符串
        var projectAttr =
        [
            { name: 'documentCode', value: documentCode },
            { name: 'title', value: title },
            { name: 'deliveryUnit', value: deliveryUnit },
            { name: 'content', value: content },//正文内容
            { name: 'contact', value: contact },//联系人
            { name: 'tel', value: tel }
        ];

        var projectAttrJson = Ext.JSON.encode(projectAttr);

        Ext.MessageBox.wait("正在生成函件，请稍候...", "等待");

        Ext.Ajax.request({

            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.HXEPC_Plugins.Document", A: "DraftDocument",
                sid: localStorage.getItem("sid"), ProjectKeyword: me.projectKeyword,
                DocAttrJson: projectAttrJson
            },
            success: function (response, options) {
                me.draft_document_callback(response, options, "");//, me.projectKeyword, closeWin);
            },
            failure: function (response, options) {
                ////Ext.Msg.alert("错误", "连接服务器失败！<br>" + response.responseText);
            }
        })
    },

    //发送起草函件后的返回事件
    draft_document_callback: function (response, options, parentKeyword) {
        var me = this;
        var res = Ext.JSON.decode(response.responseText, true);
        var state = res.success;
        if (state === false) {
            var errmsg = res.msg;
            Ext.Msg.alert("错误信息", errmsg);
        }
        else {

            Ext.MessageBox.close();//关闭等待对话框

            Ext.require('Ext.ux.Common.comm', function () {

                winDraftDocument.close();

                var projectKeyword = res.data[0].ProjectKeyword;

                me.refreshWin(projectKeyword, false);

                //创建流程
                //参数：doclist,wfKeyword,userlist,callback_fun
                //StartNewWorkFlow(projectKeyword, "CREATEPROJECT", "", function (res, WorkFlowKeyword, CuWorkStateCode) {
                //    me.refreshWin(projectKeyword, false);
                //})
            });
        }
    },

    //刷新表单，参数:parentKeyword:新建的联系单目录
    refreshWin: function (parentKeyword, closeWin) {
        var me = this;
        var tree = Ext.getCmp(me.mainPanelId);//.down('treepanel');
        var viewTreeStore = tree.store;

        viewTreeStore.load({
            callback: function (records, options, success) {//添加回调，获取子目录的文件数量
                if (closeWin)
                    winDraftDocument.close();

                //展开目录
                Ext.require('Ext.ux.Common.comm', function () {
                    Ext.getCmp('contentPanel').down('_mainProjectTree').ExpendProject(parentKeyword);
                });
            }
        });
    }
});