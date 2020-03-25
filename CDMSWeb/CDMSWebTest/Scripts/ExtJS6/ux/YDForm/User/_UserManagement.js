Ext.define('Ext.ux.YDForm.User._UserManagement', {
    extend: 'Ext.container.Container',
    alias: 'widget._UserManagement',
    layout: 'fit',
    resultvalue: '',

    initComponent: function () {
        var me = this;

        me.xmlHttp = null;
        me.imgUrl = '';

        me.IsCreate =false;

        //类型combo初始数据
        me.usertypedata = [{ text: "Default", value: "Default" }, { text: "Windows", value: "Windows" },
            { text: "DefaultAdmin", value: "DefaultAdmin" }, { text: "WindowsAdmin", value: "WindowsAdmin" }];

        //状态combo初始数据
        me.userstatedata = [{ text: "OnLine", value: "OnLine" }, { text: "OffLine", value: "OffLine" },
            { text: "Disabled", value: "Disabled" }, { text: "Away", value: "Away" }];

        me.userCodeText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "代码",//"用户代码",
            anchor: "80%", labelWidth: 60, labelAlign: "right", margin: '8 8 8 0', width: 160, fieldStyle: 'border-color: red; background-image: none;'//红色边框
            
        });

        me.userDescText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "描述", anchor: "80%", labelWidth: 60, labelAlign: "right", margin: '8 8 8 0', width: 160, fieldStyle: 'border-color: red; background-image: none;'//红色边框
        });

        me.emailText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "电子邮箱", anchor: "80%", labelWidth: 60, labelAlign: "right", margin: '8 8 8 0', width: 160
        });

        me.telText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "联系电话", anchor: "80%", labelWidth: 60, labelAlign: "right", margin: '8 8 8 0', width: 160
        });

        me.pwdText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "密码", inputType: "password", anchor: "80%", labelWidth: 60, labelAlign: "right", margin: '8 8 8 0', width: 160,//flex: 1
            value: ""
        });

        me.rePwdText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "确认密码", inputType: "password", anchor: "80%", labelWidth: 60, labelAlign: "right", margin: '8 8 8 0', width: 160,//flex: 1
            value: ""
        });

        //添加类型combo
        Ext.define("usertypeModel", {
            extend: 'Ext.data.Model',
            fields: ["text", "value"]
        });
        me.usertypeProxy = Ext.create("Ext.data.proxy.Memory", {
            data: me.usertypedata,
            model: "usertypeModel"
        });

        me.usertypeStore = Ext.create("Ext.data.Store", {
            model: usertypeModel,
            proxy: me.usertypeProxy
            
        });


        me.usertypeCombo = Ext.create("Ext.form.field.ComboBox",
        {
            //xtype: "combo",
            fieldLabel: '类型', labelWidth: 60,
            triggerAction: "all", store: me.usertypeStore,
            valueField: 'value', editable: false,//不可输入
            displayField: 'text', margin: '8 8 8 0',
            anchor: "80%", labelAlign: "right", labelPad: 8, width: 160,// width: '50%',//
            emptyText: "--请选择--", autoLoadOnValue: true,
            //fieldStyle: 'border-color: red; background-image: none;',//红色边框
            listeners:
            {
                select: function (combo, records, eOpts) {
                    //var usertype = me.usertypeCombo.value;

                }
            }
        });

        //添加状态combo
        Ext.define("userstateModel", {
            extend: 'Ext.data.Model',
            fields: ["text", "value"]
        });
        me.userstateProxy = Ext.create("Ext.data.proxy.Memory", {
            data: me.userstatedata,
            model: "userstateModel"
        });

        me.userstateStore = Ext.create("Ext.data.Store", {
            model: userstateModel,
            proxy: me.userstateProxy
        });


        me.userstateCombo = Ext.create("Ext.form.field.ComboBox",
        {
            //xtype: "combo",
            fieldLabel: '状态', labelWidth: 60,
            triggerAction: "all", store: me.userstateStore,
            valueField: 'value', editable: false,//不可输入
            displayField: 'text', margin: '8 8 8 0',
            anchor: "80%", labelAlign: "right", labelPad: 8, width: 160,//width: '50%',//
            emptyText: "--请选择--", autoLoadOnValue: true,
            //fieldStyle: 'border-color: red; background-image: none;',//红色边框
            listeners:
            {
                select: function (combo, records, eOpts) {
                    //var userstate = me.userstateCombo.value;

                }
            }
        });

        //定义未选择用户的model
        Ext.define("_GroupAllUserSelection", {
            extend: "Ext.data.Model",
            fields: ["text", "id"],
            proxy: {
                type: "ajax",
                url: 'WebApi/Get',
                extraParams: {
                    C: "AVEVA.CDMS.WebApi.UserController", A: "GetAllUserList",
                    KeyWord: 1, sid: localStorage.getItem("sid"), Group:"Root"
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
            }

        });

        //定义未选择用户的store
        me._groupselstore = Ext.create("Ext.data.Store", {
            batchActions: false,
            //文章的Store需要支持远程排序和搜索
            remoteFilter: true,
            remoteSort: true,
            //每50条记录为一页
            pageSize: 50,//视图路径：\simplecdms\scripts\app\view\content\view.js
            model: "_GroupAllUserSelection"

        });

        //定义未选择用户tab的view
        me.groupgrid = Ext.widget("grid", {
            region: "center",
            cellTip: true,
            store: me._groupselstore,
            //Grid使用了复选框作为选择行的方式
            selType: "checkboxmodel",
            selModel: { checkOnly: false, mode: "MULTI" },
            bbar: new Ext.PagingToolbar({
                store: me._groupselstore,
                displayInfo: true,
                displayMsg: '当前显示{0} - {1}条，共{2}条数据',
                emptyMsg: "没有记录"
            }),

            columns: [
                { text: '用户名', dataIndex: 'text', width: '100%' }
            ],
            listeners: {
                'itemdblclick': function (view, record, item, index, e) {

                },
                'select': function (view, record, index, eOpts) {

                    //设置编辑控件可编辑
                    me.set_edit_panel_disabled(false);

                    me.delUserButton.setDisabled(false);
                    me.saveUserButton.setDisabled(false);

                    if (me.createUserButton.text==="确定")
                    me.createUserButton.setText("新建");

                    if (me.cancelButton.disabled === false)
                        me.cancelButton.setDisabled(true);

                    //Ext.Msg.alert("错误信息", record.data.id);
                    var userKeyword = record.data.id;
                    //获取用户信息
                    Ext.Ajax.request({
                        url: 'WebApi/Post',
                        method: "POST",
                        params: {
                            C: "AVEVA.CDMS.WebApi.UserController", A: "GetUserInfo",
                            sid: localStorage.getItem("sid"), UserKeyword: userKeyword
                        },
                        success: function (response, options) {
                            //me.sendGetDraftLetterCNDefault_callback(response, options);//, funCallback);
                            me.sendGetUserInfo_callback(response, options);
                            
                        }
                    });
                }
            }, flex: 1
        });

        //定义上传附件按钮
        me.FileUploadButton = Ext.create('Ext.ux.upload.Button', {
            renderTo: Ext.getBody(),
            text: '选择数字签名文件(后缀为JPG)',
            //plugins: [{
            //    ptype: 'ux.upload.window',
            //    //ptype: Ext.ux.upload.plugin.Window,
            //    title: '上传文件',
            //    width: 520,
            //    height: 350
            //}
            margins: "10 5 10 5",
            uploader:
            {
                url: 'WebApi/Post',
                uploadpath: '/Root/files',
                autoStart: true,//false,//选择文件后是否自动上传
                max_file_size: '20020mb',
                multipart_params: { 'guid': '', 'ProjectKeyword': '', 'sid': '', 'C': 'AVEVA.CDMS.WebApi.FileController', 'A': 'UploadFile', 'SureReplace': 'false', 'AppendFilePath': '', 'user': '' }, //设置你的参数//{},
                drop_element: me.filegrid,//拖拽控件
                statusQueuedText: '准备上传',
                statusUploadingText: '上传中 ({0}%)',
                statusFailedText: '<span style="color: red">错误</span>',
                statusDoneText: '<span style="color: green">已完成</span>',
                statusInvalidSizeText: '文件过大',
                statusInvalidExtensionText: '错误的文件类型'
            },
            flash_swf_url: 'js/Moxie.swf',
            filters: {
                mime_types : [ //只允许上传图片文件
                  { title : "图片文件", extensions : "jpg" }
                ]
            },
            listeners:
            {
                uploadstarted: function (uploader, files) {

                },

                filesadded: function (uploader, files) {

                    if (me.userCodeText.value === undefined || me.userCodeText.value==='') { // 最多上传1个文件
                        Ext.Msg.alert("提示", "请填写用户代码！");
                        return false;
                    }

                    if (files.length > 1) { // 最多上传1个文件
                        Ext.Msg.alert("提示", "只能上传一个文件，请删除多余文件！");
                        return false;
                    }

                    uploader.multipart_params.ProjectKeyword = "temp";// me.grid.store.proxy.extraParams.ProjectKeyWord;
                    uploader.multipart_params.sid = localStorage.getItem("sid");
                    
                    //for (var i = 0, len = files.length; i < len; i++) {
                    //    var file_name = files[i].name; //文件名
                    //    //构造html来更新UI
                    //    //var html = '<li id="file-' + files[i].id + '"><p class="file-name">' + file_name + '</p><p class="progress"></p></li>';
                    //    //$(html).appendTo('#file-list');
                    //    !function (i) {
                    //        me.previewImage(files[i], function (imgsrc) {
                    //            //$('#file-' + files[i].id).append('<img src="' + imgsrc + '" />');
                    //            var imageShow_box = me.signPicPanel.down('box');    //预览的图片框对象
                    //            imageShow_box.getEl().dom.src = imgsrc;
                    //        })
                    //    }(i);
                    //}

                    //////当有附件时，创建DOC文档成功后，上传附件
                    //me.FileUploadButton.uploader.start();

                    //var filePath = "file://" + files[0].path;
                    //getSignPic(filePath);

                    ////页面存在,显示图片
                    //var imageShow_box = me.signPicPanel.down('box');    //预览的图片框对象

                    //var imgsrc = preloader.type == 'image/jpeg' ? preloader.getAsDataURL('image/jpeg', 80) : preloader.getAsDataURL(); //得到图片src,实质为一个base64编码的数据
                    //file.imgsrc = imgsrc;

                    //if (Ext.isIE) {
                    //    var image = Ext.get('logoPic').dom;
                    //    imageShow_box.src = Ext.BLANK_IMAGE_URL;
                    //    imageShow_box.filters.item("DXImageTransform.Microsoft.AlphaImageLoader").src = filePath;
                    //} else {
                    //    //支持FF  
                    //    imageShow_box.getEl().dom.src = filePath;//window.URL.createObjectURL(filePath);
                    //}

                    //var preloader = new mOxie.Image();
                    //preloader.onload = function () {
                    //    preloader.downsize(180, 120);//先压缩一下要预览的图片
                    //    var imgsrc = preloader.type == 'image/jpeg' ? preloader.getAsDataURL('image/jpeg', 80) : preloader.getAsDataURL(); //得到图片src,实质为一个base64编码的数据
                    //    file.imgsrc = imgsrc;
                    //    preloader.destroy();
                    //    preloader = null;
                    //};
                    //preloader.load(file.getSource());

                    return true;
                },

                beforeupload: function (uploadbasic, uploader, file) {
                    //console.log('beforeupload');			
                    //上传文件而不是替换文件时，这里要把Doc关键字重置
                    uploadbasic.multipart_params.DocKeyword = "";

                    var extIndex = file.name.lastIndexOf(".");
                    var fileCode = file.name;
                    if (extIndex >= 0) {
                        fileCode = fileCode.substring(0, extIndex);
                    }

                    ////规范附件名称
                    //me.docUploadIndex = me.docUploadIndex + 1;
                    //fileCode = me.docCode + "附件" + me.docUploadIndex.toString();//+ " " + fileCode;

                    Ext.require('Ext.ux.Common.comm', function () {
                        ////先创建文档
                        //createDoc(uploadbasic, file, me.newProjectKeyword, fileCode, "", "", function (uploadbasic, res, options, DocKeyword) {
                        //    //var state = res.success;
                        //    if (res.success === true) {
                        //        //处理创建文档后的返回事件
                        //        me.docList = me.docList + "," + DocKeyword;

                        //        //触发服务端BeforeUploadFile事件，如果是断点续传，获取已上传的文件大小
                        //        sendBeforeUploadFile(uploadbasic, options.file, DocKeyword, me);
                        //    }
                        //});

                        sendBeforeUploadFile(uploadbasic, file, "", me, "BMP/" + me.userCodeText.value + ".jpg");
                    });
                },

                fileuploaded: function (uploader, file) {
                    //console.log('fileuploaded');
                    //文件上传后的事件
                    Ext.require('Ext.ux.Common.comm', function () {
                        afterUploadFile(uploader, file, me.ServerFullFileName);
                    });
                },

                uploadcomplete: function (uploader, success, failed) {

                    
                    //Ext.Msg.alert("提示", "成功上传数字签名文件！");

                    //设置上传附件完毕标记
                    me.uploadCompleteState = true;
                    me.getSignPic();
                },
                scope: this
            }, flex:1//width: 205


        });

        //定义显示模型的panel
        me.signPicPanel = Ext.create('Ext.panel.Panel', {
            //width: "50%",
            width: 50,
            height: 25,
            //baseCls: 'my-panel-no-border',//隐藏边框
            margins: "10 5 10 5",
            items: [
                {
                    xtype: 'box',
                    //id: 'logoPic',
                    autoEl: {
                        width: 50,
                        height: 25,
                        tag: 'img',
                        type: 'image',
                        src: '/BMP/blank.jpg',
                        style: 'filter:progid:DXImageTransform.Microsoft.AlphaImageLoader(sizingMethod=scale);width:50px;height:25px;text-align:center;',
                        complete: 'off'
                    }
                }
            ]
        });


        me.createUserButton = Ext.create("Ext.button.Button", {
            text: "新建", width: 60, margins: "10 5 10 5",
            listeners: {
                "click": function (btn, e, eOpts) {//添加点击按钮事件
                    me.new_user();
                }
            }
        });

        me.delUserButton = Ext.create("Ext.button.Button", {
            text: "删除", width: 60, margins: "10 5 10 5", disabled: true,
            listeners: {
                "click": function (btn, e, eOpts) {//添加点击按钮事件
                    me.send_drop_user();
                }
            }
        });

        me.saveUserButton = Ext.create("Ext.button.Button", {
            text: "保存", width: 60, margins: "10 5 10 5", disabled: true,
            listeners: {
                "click": function (btn, e, eOpts) {//添加点击按钮事件
                    //me.send_draft_document();
                    me.send_save_user_info(false);
                }
            }
        });

        me.cancelButton = Ext.create("Ext.button.Button", {
            text: "取消", width: 60, margins: "10 5 10 5", disabled:true,
            listeners: {
                "click": function (btn, e, eOpts) {//添加点击按钮事件

                    me.cancel_select();
                }
            }
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
                me.createUserButton,
                me.delUserButton,
                me.saveUserButton,
                me.cancelButton
            ]
        });

        //添加列表
        me.items = [
            			//{
            			//    xtype: "button",
            			//    text: "我的按钮"
            			//}
                        //me.maintab
                        //me._UserGroupPanel

          Ext.widget('form', {
              baseCls: 'my-panel-no-border',//隐藏边框
              layout: {
                  type: 'vbox',
                  align: 'stretch',
                  pack: 'start'
              },
              items:[ 
                  {//上部容器
                      baseCls: 'my-panel-no-border',//隐藏边框
                      layout: {
                          type: 'hbox',
                          align: 'stretch',
                          pack: 'start'
                      },
                      items: [
                         me.groupgrid,
                        {
                            xtype: "fieldset",
                            title: "一般属性",
                            width: 380, margin: '5 5 0 5',
                            //baseCls: 'my-panel-no-border',//隐藏边框
                            layout: {
                                type: 'vbox',
                                align: 'stretch',
                                pack: 'start'
                            },
                            items: [
                                me.userCodeText,
                                me.userDescText,
                                me.emailText,
                                me.telText,
                                me.usertypeCombo,
                                me.userstateCombo,
                                me.pwdText,
                                me.rePwdText,
                                {
                                    baseCls: 'my-panel-no-border',//隐藏边框
                                    layout: {
                                        type: 'hbox',
                                        align: 'stretch',
                                        pack: 'start'
                                    },
                                    items: [
                                        me.signPicPanel,
                                        me.FileUploadButton
                                                            
                                                            //me.signPic
                                    ]
                                }
                            ]
            			}
                      ],flex:1
                  }, me.bottomButtonPanel
              ]
          })
        ];

        me._groupselstore.proxy.extraParams.Filter = "";//把参数传给C#MVC,路径：\simplecdms\controllers\Doccontroller.cs 下的 GetWorkFlow()
        me._groupselstore.proxy.extraParams.Group = "Root";
        me._groupselstore.proxy.extraParams.sid = localStorage.getItem("sid");
        me._groupselstore.currentPage = 1;
        me._groupselstore.load();

        me.set_edit_panel_disabled(true);

        me.callParent(arguments);
    },

    //处理获取发文处理表单默认参数的返回
    sendGetUserInfo_callback: function (response, options) {
        var me = this;

        //获取数据后，更新窗口
        var res = Ext.JSON.decode(response.responseText, true);
        var state = res.success;
        if (state === true) {
            var recod = eval(res.data[0]);
            //Ext.Msg.alert("错误信息", recod);
            var userCode=recod.O_username;
            var userDesc=recod.O_userdesc;
            var email=recod.O_email;
            var tel=recod.Phone;
            var userTypeIndex = recod.UserType;
            var userStateIndex = recod.UserFlags;
            

            me.userCodeText.setValue(userCode);
            me.userDescText.setValue(userDesc);
            me.emailText.setValue(email);
            me.telText.setValue(tel);

            me.usertypeCombo.setRawValue( me.usertypedata[userTypeIndex].text);//设置显示值
            me.usertypeCombo.setValue( me.usertypedata[userTypeIndex].value); //设置ID值
            //me.usertypeCombo.setValue( userTypeIndex ); //设置ID值

            me.userstateCombo.setRawValue( me.userstatedata[userStateIndex].text);//设置显示值
            me.userstateCombo.setValue( me.userstatedata[userStateIndex].value); //设置ID值
            //me.userstateCombo.setValue( userStateIndex); //设置ID值
            //获取图片
            me.getSignPic();
        } else {
            var errmsg = res.msg;
            Ext.Msg.alert("错误信息", errmsg);
        }
    },

    send_save_user_info: function (isCreate) {
        var me = this;
        
        me.IsCreate = isCreate;
        var userKeyword = "";

        if (!isCreate) {
            var grid = me.groupgrid;
            var rs = grid.getSelectionModel().getSelection();//获取选择的文档

            if (!(rs !== null && rs.length > 0)) {
                return;
            }

            userKeyword = rs[0].data.id;//获取文档关键字
        }

            var userCode=me.userCodeText.value;
            var userDesc=me.userDescText.value;
            var email=me.emailText.value;
            var tel=me.telText.value;
            var userPwd = me.pwdText.value;
            var userRePwd=me.rePwdText.value;

            var uType = me.usertypeCombo.value;
            var uState = me.userstateCombo.value;
            
            var userType = "";
            var userState = "";

            for (var i = 0; i < me.usertypedata.length; i++) {
                if (me.usertypedata[i].value === uType) {
                    userType = i.toString();
                }
            }

            for (var i = 0; i < me.userstatedata.length; i++) {
                if (me.userstatedata[i].value === uState) {
                    userState = i.toString();
                }
            }

            var A = isCreate ? "CreateUser" : "SaveUserInfo";


            ////获取用户信息
            Ext.Ajax.request({
                url: 'WebApi/Post',
                method: "POST",
                params: {
                    C: "AVEVA.CDMS.WebApi.UserController", A: A,
                    sid: localStorage.getItem("sid"), UserKeyword: userKeyword,
                    UserCode: userCode, UserDesc: userDesc,
                    UserEmail: email, Phone: tel,
                    UserPwd: userPwd, UserConfirmPwd:userRePwd,
                    UserType: userType, UserStatus: userState
                },
                success: function (response, options) {


                        //当没有附件时，处理返回事件
                        me.send_save_user_info_callback(response, options);
        
                    

                }
            });

        
    },

    send_save_user_info_callback: function (response, options) {
        var me = this;

        //获取数据后，更新窗口
        var res = Ext.JSON.decode(response.responseText, true);
        var state = res.success;
        if (state === true) {

                var recod = eval(res.data[0]);


                Ext.Msg.alert("提示", "保存成功!");

                me.refreshPanel();

        } else {
            var errmsg = res.msg;
            Ext.Msg.alert("错误信息", errmsg);
        }
    },

    send_drop_user: function () {
        var me = this;

        var userKeyword = "";


        var grid = me.groupgrid;
        var rs = grid.getSelectionModel().getSelection();//获取选择的文档

        if (!(rs !== null && rs.length > 0)) {
            return;
        }

        userKeyword = rs[0].data.id;//获取文档关键字

        ////获取用户信息
        Ext.Ajax.request({
            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.WebApi.UserController", A: "DropUser",
                sid: localStorage.getItem("sid"), UserKeyword: userKeyword
            },
            success: function (response, options) {
                //处理返回事件
                me.send_drop_user_callback(response, options);

            }
        });

    },

    send_drop_user_callback: function (response, options) {
        var me = this;

        //获取数据后，更新窗口
        var res = Ext.JSON.decode(response.responseText, true);
        var state = res.success;
        if (state === true) {

            var recod = eval(res.data[0]);


            Ext.Msg.alert("提示", "成功删除用户!");

            me.groupgrid.store.loadPage(1);
            //me.refreshPanel();

        } else {
            var errmsg = res.msg;
            Ext.Msg.alert("错误信息", errmsg);
        }

    },

    refreshPanel: function () {
        var me = this;

        if (me.IsCreate) {

            me.cancel_select();
            me.groupgrid.store.loadPage(1);
        } else {
            var grid = me.groupgrid;
            var rs = grid.getSelectionModel().getSelection();//获取选择的文档

            if (!(rs !== null && rs.length > 0)) {
                return;
            }

            var strUser = me.userCodeText.value.trim() + "__" + me.userDescText.value.trim();
            if (rs[0].data.text != strUser) {
                rs[0].data.text = strUser;//获取文档关键字
                me.groupgrid.getView().refresh();
            }
        }
    },

    new_user: function () {
        var me = this;

        ///响应新建用户按钮
        if (me.createUserButton.getText() === "新建") {

            var userTypeIndex = 0;
            var userStateIndex = 0;

            me.userCodeText.setValue("");
            me.userDescText.setValue("");
            me.emailText.setValue("");
            me.telText.setValue("");
            me.pwdText.setValue("");
            me.rePwdText.setValue("");

            me.usertypeCombo.setRawValue(me.usertypedata[userTypeIndex].text);//设置显示值
            me.usertypeCombo.setValue(me.usertypedata[userTypeIndex].value); //设置ID值

            me.userstateCombo.setRawValue(me.userstatedata[userStateIndex].text);//设置显示值
            me.userstateCombo.setValue(me.userstatedata[userStateIndex].value); //设置ID值

            //设置编辑控件可以编辑
            me.set_edit_panel_disabled(false);

            me.createUserButton.setText("确定");
            me.delUserButton.setDisabled(true);
            me.saveUserButton.setDisabled(true);
            me.cancelButton.setDisabled(false);

            me.getSignPic();
        } else {
            //向服务器发送新建用户请求
            me.send_save_user_info(true);
        }
    },

    cancel_select: function () {
        var me = this;

        //设置编辑控件不可编辑
        me.set_edit_panel_disabled(true);

        me.createUserButton.setText("新建");
        me.delUserButton.setDisabled(true);
        me.saveUserButton.setDisabled(true);
        me.cancelButton.setDisabled(true);

        me.groupgrid.getSelectionModel().clearSelections();
        me.groupgrid.getView().refresh();

        me.getSignPic();
    },

    set_edit_panel_disabled: function (flag) {
        var me = this;

        if (me.userCodeText.disabled!=flag)
            me.userCodeText.setDisabled(flag);

        if (me.userDescText.disabled != flag)
            me.userDescText.setDisabled(flag);

        if (me.emailText.disabled != flag)
            me.emailText.setDisabled(flag);

        if (me.telText.disabled != flag)
            me.telText.setDisabled(flag);

        if (me.pwdText.disabled != flag)
            me.pwdText.setDisabled(flag);

        if (me.rePwdText.disabled != flag)
            me.rePwdText.setDisabled(flag);

        if (me.usertypeCombo.disabled != flag)
            me.usertypeCombo.setDisabled(flag);

        if (me.userstateCombo.disabled != flag)
            me.userstateCombo.setDisabled(flag);

        if (me.FileUploadButton.disabled != flag)
            me.FileUploadButton.setDisabled(flag);
    },

    //判断浏览器  
    createXMLHttpRequest: function () {
        var me = this;
        if (window.XMLHttpRequest) {  
            //Firefox,Netscape,Chrome,Safari等浏览器  
            me.xmlHttp = new XMLHttpRequest();  
        } else if (window.ActiveXObject) { //IE浏览器  
            try {  
                me.xmlHttp = new ActiveXObject("Msxml2.XMLHTTP"); //创建xmlHttp对象  
            } catch (e) {  
                try {  
                    me.xmlHttp = new ActiveXObject("Microsoft.XMLHTTP"); //创建xmlHttp对象  
                } catch (e) { }  
            }  
        }  
        },

    //判断页面是否存在
    //GetURL: function (url) {
    getSignPic: function (callBack_fun) {
        var me = this;
        me.imgUrl = "/BMP/" + me.userCodeText.value + ".jpg";

        me.createXMLHttpRequest();    
        me.xmlHttp.open("GET", me.imgUrl, true);
        me.xmlHttp.send(null);
        me.xmlHttp.onreadystatechange = function () {
            if (me.xmlHttp.readyState == 4) {
                if (me.xmlHttp.status == 200) {
                    //alert("页面存在");    

                } else {
                    //alert("页面不存在");    
                    me.imgUrl = "/BMP/" + "BLANK" + ".jpg";
                }

                ////定义显示模型的panel
                //me.signPicPanel = Ext.create('Ext.panel.Panel', {
                //    //width: "50%",
                //    width: 50,
                //    height: 25,
                //    //baseCls: 'my-panel-no-border',//隐藏边框
                //    margins: "10 5 10 5",
                //    items: [
                //        {
                //            xtype: 'box',
                //            //id: 'logoPic',
                //            autoEl: {
                //                width: 50,
                //                height: 25,
                //                tag: 'img',
                //                type: 'image',
                //                src: me.imgUrl,
                //                style: 'filter:progid:DXImageTransform.Microsoft.AlphaImageLoader(sizingMethod=scale);width:50px;height:25px;text-align:center;',
                //                complete: 'off'
                //            }
                //        }
                //    ]
                //});
                
                me.imgUrl = me.imgUrl + "?temp=" + Math.random();//userIconUrl 为图片URL

                //页面存在,显示图片
                var imageShow_box = me.signPicPanel.down('box');    //预览的图片框对象

                if (Ext.isIE) {
                    //var image = Ext.get('logoPic').dom;
                    imageShow_box.src = Ext.BLANK_IMAGE_URL;
                    imageShow_box.filters.item("DXImageTransform.Microsoft.AlphaImageLoader").src = me.imgUrl;
                } else {
                    //支持FF  
                    imageShow_box.getEl().dom.src = me.imgUrl;
                }
            }    
        }     
    },

    //plupload中为我们提供了mOxie对象
    //有关mOxie的介绍和说明请看：https://github.com/moxiecode/moxie/wiki/API
    //如果你不想了解那么多的话，那就照抄本示例的代码来得到预览的图片吧
    previewImage: function (file, callback) {//file为plupload事件监听函数参数中的file对象,callback为预览图片准备完成的回调函数
        if (!file || !/image\//.test(file.type)) return; //确保文件是图片
        if (file.type == 'image/gif') {//gif使用FileReader进行预览,因为mOxie.Image只支持jpg和png
            var fr = new mOxie.FileReader();
            fr.onload = function () {
                callback(fr.result);
                fr.destroy();
                fr = null;
            }
            fr.readAsDataURL(file.getSource());
        } else {
            var preloader = new mOxie.Image();
            preloader.onload = function () {
                preloader.downsize(300, 300);//先压缩一下要预览的图片,宽300，高300
                var imgsrc = preloader.type == 'image/jpeg' ? preloader.getAsDataURL('image/jpeg', 80) : preloader.getAsDataURL(); //得到图片src,实质为一个base64编码的数据
                callback && callback(imgsrc); //callback传入的参数为预览图片的url
                preloader.destroy();
                preloader = null;
            };
            preloader.load(file.getSource());
        }
    }
});