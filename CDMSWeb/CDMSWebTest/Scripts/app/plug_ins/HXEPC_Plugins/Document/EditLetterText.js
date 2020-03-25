Ext.define('Ext.plug_ins.HXEPC_Plugins.Document.EditLetterText', {
    extend: 'Ext.container.Container',//编辑函件正文
    alias: 'widget.editLetterText',
    //layout: "border",
    layout: 'fit',
    resultvalue: '', mainPanelId: '', projectKeyword: '',
    initComponent: function () {
        var me = this;

        //添加函件正文Text
        me.contentText = Ext.create("Ext.form.field.TextArea", {
            xtype: "textarea", anchor: "80%", labelAlign: "right", labelPad: 8, margin: '10 10 0 10', //margin: '0 5 5 0',
            width: "100%",//flex:1, //width: 460, //
            height: 260, fieldLabel: "函件正文", labelWidth: 60
        });

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
                            me.send_edit_document();
                        }
                    }
                },
                {
                    xtype: "button",
                    text: "取消", width: 60, margins: "10 15 10 5",
                    listeners: {
                        "click": function (btn, e, eOpts) {//添加点击按钮事件

                            winEditLetterText.close();
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
                      me.contentText
                  ]
              }, me.bottomButtonPanel]
          })];


        me.callParent(arguments);
    },

    
    //获取起草信函表单默认参数
    sendGetEditLetterTextDefault: function (funCallback) {
        var me = this;

        //通过extjs的ajax获取操作全部名称
        Ext.Ajax.request({
            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.HXEPC_Plugins.Document", A: "GetEditLetterTextDefault",
                sid: localStorage.getItem("sid"), DocKeyword: me.docKeyword
            },
            success: function (response, options) {
                me.sendGetEditLetterTextDefault_callback(response, options, funCallback);

            }
        });
    },


    //处理获取发文处理表单默认参数的返回
    sendGetEditLetterTextDefault_callback: function (response, options, funCallback) {
        var me = this;

        //获取数据后，更新窗口
        var res = Ext.JSON.decode(response.responseText, true);
        var state = res.success;
        if (state === true) {
            var recod = eval(res.data[0]);
            
            var content = recod.Content;

            me.contentText.setValue(content);

            funCallback();
        }
    },

    //向服务器发送起草红头公文请求
    send_edit_document: function () {
        var me = this;


        //检查内容
        var content = me.contentText.value;
        if (content == undefined || content == "") {
            Ext.Msg.alert("错误信息", "请输入内容");
            return;
        }

        //获取表单数据，转换成JSON字符串
        var projectAttr =
        [
            { name: 'content', value: content }
     
        ];

        var projectAttrJson = Ext.JSON.encode(projectAttr);
    

        Ext.MessageBox.wait("正在生成信函，请稍候...", "等待");

        Ext.Ajax.request({

            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.HXEPC_Plugins.Document", A: "ModiLetterText",
                sid: localStorage.getItem("sid"), DocKeyword: me.docKeyword,
                DocAttrJson: projectAttrJson
            },
            success: function (response, options) {
                //me.draft_document_callback(response, options, "");//, me.projectKeyword, closeWin);

                //获取数据后，更新窗口
                var res = Ext.JSON.decode(response.responseText, true);
                var state = res.success;
                if (state === true) {

                    Ext.MessageBox.close();//关闭等待对话框

                    var recod = eval(res.data[0]);

                            me.edit_document_callback(response, options, "");
                
                } else {
                    var errmsg = res.msg;
                    Ext.Msg.alert("错误信息", errmsg);
                }
            },
            failure: function (response, options) {
                //////Ext.Msg.alert("错误", "连接服务器失败！<br>" + response.responseText);
            }
        })
    },

    edit_document_callback: function (response, options) {
        //Ext.Msg.alert("错误信息", errmsg);
        winEditLetterText.close();
    }
});