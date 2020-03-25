
Ext.define("Ext.ux.Login", {
    extend: "Ext.window.Window",
    singleton: true,
    //title: '艾三维协同管理信息系统登录窗口',
    plain: true,
    header: false,
    border: false,
    draggable: false,
    frame: false,
    width: 420,
    height: 500,
    //modal: true,
    modal: false,
    closable: false,
    resizable: false,
    closeAction: 'hide',
    hideMode: 'offsets',
    //style: 'opacity: 0.5;filter: alpha(opacity=50);',
    style: 'opacity: 0.8;filter: alpha(opacity=50);',

    initComponent: function () {
        var me = this;

        //me.image = Ext.create(Ext.Img, {
        //    style: "cursor:pointer ",
        //    src: "/VerifyCode",
        //    listeners: {
        //        click: me.onRefrehImage,
        //        element: "el",
        //        scope: me
        //    }
        //});
        //var localUserName = localStorage.getItem("username");
        //if (localStorage.getItem("username")===null || localStorage.getItem("username")===undefined)
        //{}

        me.img_sys_logo = Ext.create("Ext.Component", {
            iconCls: "login_sys_logo"
        });

        me.userLabel = Ext.create("Ext.form.Label", {
            text: '用户名',
            margin: '18 0 5 32',
            style: 'font-size:18px; color:#0F7FFF'
        });

        me.pswLabel = Ext.create("Ext.form.Label", {
            text: '密码',
            margin: '15 0 5 32',
            style: 'font-size:18px; color:#0F7FFF'
        });

        me.userText = Ext.create("Ext.form.field.Text", {
            //fieldLabel: "用户名",
            name: "UserName", anchor: "80%", labelWidth: 0, labelAlign: "left", height: 45, width: 295, margin: '0 0 0 30',
            emptyText: '请输入用户名', style: 'font-size:18px',
            value: localStorage.getItem("username"), //fieldStyle: 'border-color: red; background-image: none;',//红色边框     
            listeners: {
                specialkey: function (field, e) {
                    //侦听回车事件，密码输入框设置焦点
                    if (e.getKey() == Ext.EventObject.ENTER) {
                        me.pswText.focus(false, 200);
                    }
                }
            }
        });

        me.pswText = Ext.create("Ext.form.field.Text", {
            name: "Password", inputType: "password", anchor: "80%", labelWidth: 0, labelAlign: "top", height: 45, width: 295, margin: '0 0 0 30',
            emptyText: '请输入密码', style: 'font-size:18px',
            labelAlign: "left", 
            listeners: {
                specialkey: function (field, e) {
                    //侦听回车事件，登录用户
                    if (e.getKey() == Ext.EventObject.ENTER) {
                        me.onLogin();
                    }
                }
            }
        });
    
        me.form = Ext.create(Ext.form.Panel, {
            border: false,
            bodyPadding: 0,
            //bodyStyle: "background:#e2eff3",
            url: "DBSource/Login",

            items: [
                {
                    xtype: "fieldset",
                    height: 400,
                    //flex:1,
                    //layout: "form",
                    layout: "vbox",
                    defaultType: "textfield",
                    margin: '0 20 0 20',
                    baseCls: 'my-panel-no-border',//隐藏边框
                    //fieldDefaults: {
                    //    //labelWidth: 60,
                    //    labelSeparator: "：",
                    //    //anchor: "0",
                    //    //allowBlank: false
                    //    //frame: true,

                    //    margin: '20 25 20 25'
                    //},
                    items: [
                        {


                            xtype: 'box', // 或者xtype: 'component',
                            width: 290, // 图片宽度
                            height: 54, // 图片高度
                            id: 'imagebox', margin: '30 0 0 30',
                            autoEl: {
                                tag: 'img', // 指定为img标签
                                src: '/Images/loginsyslogo.png' // 指定url路径
                            }
                        },
                        {
                            xtype: 'box', // 或者xtype: 'component',
                            margin: '25 0 0 0',
                            style: {
                                height: '2px',
                                width: '350px',
                                background: '-webkit-linear-gradient(left,rgba(255, 255, 255, 0),#e5e8f0,rgba(255,255,255,0))',
                                background: 'linear-gradient(to right, rgba(255, 255, 255, 0),#e5e8f0,rgba(255,255,255,0))'
                            }
                        },
                        me.userLabel,
                            me.userText,
                            me.pswLabel,
                            me.pswText,
                    {
                        xtype: 'box', // 或者xtype: 'component', 0f7fff 0F7FFF
                        margin: '55 0 0 0',
                        style: {
                            height: '2px',
                            width: '350px',
                            background: '-webkit-linear-gradient(left,rgba(255, 255, 255, 0),#e5e8f0,rgba(255,255,255,0))',  //0F7FFF
                            background: 'linear-gradient(to right, rgba(255, 255, 255, 0),#e5e8f0,rgba(255,255,255,0))'
                        }
                    }
                            //{
                            //    fieldLabel: "密码", name: "Password", inputType: "password", anchor: "80%", labelWidth: 60, labelAlign: "left",
                            //},
                            //{
                            //    fieldLabel: "验证码", name: "Vcode", minLength: 6, maxLength: 6
                            //},
                            //{
                            //    xtype: "container", height: 80, anchor: "-5", layout: "fit",
                            //    items: [me.image]
                            //},
                            //{
                            //    xtype: "container", anchor: "-5", html: "**验证码不区分大小写，如果看不清楚验证码，可单击图片刷新验证码。"
                            //}
                    ]
                }
            ],
            dockedItems: [{
                xtype: 'toolbar', dock: 'bottom', ui: 'footer', layout: { pack: "center" },style:{background:'#ffffff'},
                items: [
		    	    
                    new Ext.button.Button({
                        text: '<span style="font-size:20px; font-weight:lighter;">登录</span>',style: 'background-color:#0F7FFF', width: 132, height: 45, margin: '0 20 0 0', handler: me.onLogin, scope: me
                    }),
		    	    { text: '<span style="font-size:20px; font-weight:lighter; ">重置</span>', width: 132, height: 45, margin: '0 0 0 20', handler: me.onReset, scope: me }
			    ]
            }]
        });

        me.items = [me.form]

        me.callParent(arguments);

        me.pswText.focus(false, 200);

    },

    //onRefrehImage: function () {
    //    this.image.setSrc("/VerifyCode?_dc=" + (new Date()).getTime());
    //},

    onReset: function () {
        var me = this;
        me.form.getForm().reset();
        if (me.form.items.items[0]) {
            me.form.items.items[0].focus(true, 10);
        }
        //me.onRefrehImage();
    },

    onLogin: function () {
        //提交到路径：\simplecdms\controllers\accountcontroller.cs
        var me = this,
			f = me.form.getForm();
        var encrypt = Ext.create('JSEncrypt');

        var pubkey = "-----BEGIN PUBLIC KEY-----MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQC0w036ClSD0LvxPROMun0u022ROJlZE6P3m+gjq3gpi4n7lo8jhTqMqgccDbVJqnIfMzWS9O3lnlQXWTxJ3B4XJ52FAcriY5brOXUVgBLx5QMHLLd1gtJnmG4i7r4ytgX7XVKRnojR6zca1YnS0lbGGDF1CGllB1riNrdksSQP+wIDAQAB-----END PUBLIC KEY-----";

        encrypt.setPublicKey(pubkey);
        var data = encrypt.encrypt("123456789").replace(/\+/g, '%2B');  //+号的处理：因为数据在网络上传输时，非字母数字字符都将被替换成百分号（%）后跟两位十六进制数，而base64编码在传输到后端的时候，+会变成空格，因此先替换掉。后端再替换回来;

        var encrypted = encrypt.encrypt(me.pswText.value);
        //var encrypted = me.pswText.value;

        //获取机器名
        //var WshShell = new ActiveXObject("Wscript.Shell");
        //var hostname = WshShell.ExpandEnvironmentStrings("%COMPUTERNAME%");

        Ext.Ajax.request({
            url: "DBSource/Login",
            method: "POST",

            //url: "Handler2.ashx?ParamUserName=" + userName + "&ParamUserPass=" + userPass, //将用户名和密码传送到url  
         //   method: "get",
            params: { UserName: me.userText.value, Password: encrypted},//, hostname: hostname },
            success: function (response) {
                 //获取服务器数据  
                var res = Ext.JSON.decode(response.responseText);
                var state = res.success;
                if (state === true) {
                    localStorage.setItem("username", res.msg.username);
                    localStorage.setItem("userdesc", res.msg.userdesc);
                    localStorage.setItem("sid", res.msg.guid);


                    window.location.reload();
                } else {
                    if (res.errors === undefined)
                    {
                        Ext.Msg.alert("登录失败", "登录失败,服务器连接错误！");
                    } else {
                        var errmsg = res.errors.errorMsg;
                        Ext.Msg.alert("登录失败", errmsg.substr(errmsg.lastIndexOf(",") + 1));
                    }
                }
            },
            failure: function (response, options) { alert("服务器连接错误，登录失败！"); }
        });

        //if (f.isValid()) {
        //    f.submit({
        //        //waitMsg: "正在登录，请等待……",
        //        //waitTitle: "正在登录",
        //        success: function (form, action) {
        //            //localStorage.setItem("username", Ext.getCmp('UserName').getValue());
        //            //var uname=form._fields.items[0].value;
        //            //localStorage.setItem("username", uname);
        //            localStorage.setItem("username", action.result.msg.username);
        //            localStorage.setItem("sid", action.result.msg.guid);
        //            window.location.reload();
        //            //localStorage.getItem("sid") = action.result.msg.guid;
        //        },
        //        //failure: CDMSWeb.FormSubmitFailure,
        //        //failure:function (e, opt){Ext.Msg.alert("错误", e.exceptions[0].error);},

        //        failure:function (form, action) {
        //            //Ext.Msg.alert("登录失败", "用户名密码不正确！" + JSON.stringify(action.result.errors));//获取返回信息的jsonObject对象，转换成字符串
        //            var errmsg=action.result.errors.errorMsg;
        //            Ext.Msg.alert("登录失败", errmsg.substr(errmsg.lastIndexOf(",")+1));
        //        },
        //        scope: me
        //    });
        //}
    }


});