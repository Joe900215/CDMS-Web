Ext.define('Ext.ux.YDForm.User._SetPassword', {
    //extend: 'Ext.container.Container',
    extend: 'Ext.panel.Panel',
    alias: 'widget._SetPassword', // 此类的xtype类型为buttontransparent  
    //xtype: "panel",
    title: "修改用户密码",
    //height: '100%',
    layout:'fit',
    baseCls: 'my-panel-no-border',//隐藏边框
    initComponent: function () {
        var me = this;
        me.renderTo = me.el;

        me.userCodeText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "用户名",//"用户代码",
            anchor: "80%", labelWidth: 70, labelAlign: "right", margin: '8 8 8 0', width: 160,//flex: 1
            fieldStyle: 'background-color: #DFE9F6;border-color: #DFE9F6; background-image: none;',//设置颜色
            value: localStorage.getItem("username"),//"",
            readOnly: true
        });

        me.userDescText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "用户描述", anchor: "80%", labelWidth: 70, labelAlign: "right", margin: '8 8 8 0', width: 160,//flex: 1
            fieldStyle: 'background-color: #DFE9F6;border-color: #DFE9F6; background-image: none;',//设置颜色
            value: "", readOnly: true
        });

        me.oldPassText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "旧密码", inputType: "password", anchor: "80%", labelWidth: 70, labelAlign: "right", margin: '8 8 8 0', width: 160,//flex: 1
            value: ""
        });

        me.newPassText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "新密码", inputType: "password", anchor: "80%", labelWidth: 70, labelAlign: "right", margin: '8 8 8 0', width: 160,//flex: 1
            value: ""
        });

        me.reNewPassText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "确认新密码", inputType: "password", anchor: "80%", labelWidth: 70, labelAlign: "right", margin: '8 8 8 0', width: 160,//flex: 1
            value: ""
        });

        //添加属性TAB页面到容器
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
                      },
                      items: [
                  {
                      xtype: "fieldset", margin: '12 8 16 8',
                      layout: {
                          type: 'vbox',
                          pack: 'start',
                          align: 'stretch'
                      },
                      items: [

                       me.userCodeText,
                       //me.userDescText,
                       me.oldPassText,
                       me.newPassText,
                       me.reNewPassText

                      ]
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
                                    text: "确定", width: 60, margins: "0 5 10 5",
                                    listeners: {
                                        "click": function (btn, e, eOpts) {//添加点击按钮事件
                                            me.sendSetPassword();

                                        }
                                    }
                                },
                                {
                                    xtype: "button",
                                    text: "取消", width: 60, margins: "0 15 10 5",
                                    listeners: {
                                        "click": function (btn, e, eOpts) {//添加点击按钮事件
                                            winSetPassword.close();
                                        }
                                    }
                                }
                            ]
                        }]
                  }]
          })

        ];

        me.callParent(arguments);

        //设置焦点
        //me.oldPassText.focus(false, 200);
    },

    //修改用户密码
    sendSetPassword: function () {
        var me = this;

        //获取旧密码Text
        var oldPassword = me.oldPassText.value;

        //获取新密码Text
        var newPassword = me.newPassText.value;

        //获取重新输入新密码Text
        var reNewPassword = me.reNewPassText.value;

        if (oldPassword==="") {
            Ext.Msg.alert("提示信息", "请输入旧密码！");
            return;
        }

        if (newPassword === "") {
            Ext.Msg.alert("提示信息", "请输入新密码！");
            return;
        }

        if (reNewPassword === "") {
            Ext.Msg.alert("提示信息", "请确认新密码！");
            return;
        }

        if (newPassword != reNewPassword)
        {
            Ext.Msg.alert("提示信息", "您输入的新密码和确认密码不一致，请重新输入！");
            return;
        }

        Ext.Ajax.request({

            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.WebApi.UserController", A: "SetUserPassword",
                sid: localStorage.getItem("sid"), OldPassword: oldPassword,
                NewPassword: newPassword
            },
            success: function (response, options) {
                me.setPassword_callback(response, options, "");
            },
            failure: function (response, options) {
                ////Ext.Msg.alert("错误", "连接服务器失败！<br>" + response.responseText);
            }
        })
    },

    //处理修改密码返回消息
    setPassword_callback: function (response, options) {
        var me = this;

        //获取数据后，更新窗口
        var res = Ext.JSON.decode(response.responseText, true);
        var state = res.success;
        if (state === true) {
            //Ext.MessageBox.close();//关闭等待对话框
            var recod = eval(res.data[0]);

            Ext.Msg.alert("提示信息", "修改用户密码成功！");
            winSetPassword.close();

        } else {
            var errmsg = res.msg;
            Ext.Msg.alert("错误信息", errmsg);
        }
    }
});