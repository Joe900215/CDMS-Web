//新建厂家资料目录
Ext.define('Ext.plug_ins.HXPC_Plugins.editCompany', {
    extend: 'Ext.container.Container',
    alias: 'widget.editCompany',
    layout: 'fit',
    resultvalue: '', mainPanelId: '',
    projectKeyword: '',
    initComponent: function () {
        var me = this;

        me.newProjectKeyword = '';

        //添加厂家编码text
        me.CompanyCodeText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "厂家编码", labelWidth: 70, margins: '15 0 0 0',
            anchor: "80%", labelAlign: "left", width: 150//flex: 1
        });

        //添加厂家名称text
        me.CompanyChineseText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "厂家名称", labelWidth: 70, margins: '15 0 0 0',
            anchor: "80%", labelAlign: "left", width: 150//flex: 1
        });
        
        //添加厂家地址text
        me.AddressText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "厂家地址", labelWidth: 70, margins: '15 0 0 0',
            anchor: "80%", labelAlign: "left", width: 150//flex: 1
        });

        //添加厂家省份text
        me.ProvinceText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "厂家省份", labelWidth: 70, margins: '15 0 0 0',
            anchor: "80%", labelAlign: "left", width: 150//flex: 1
        });

        //添加厂家邮政text
        me.PostCodeText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "厂家邮政", labelWidth: 70, margins: '15 0 0 0',
            anchor: "80%", labelAlign: "left", width: 150//flex: 1
        });

        //添加厂家邮箱text
        me.EMailText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "厂家邮箱", labelWidth: 70, margins: '15 0 0 0',
            anchor: "80%", labelAlign: "left", width: 150//flex: 1
        });

        //添加厂家收件人text
        me.ReceiverText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "厂家收件人", labelWidth: 70, margins: '15 0 0 0',
            anchor: "80%", labelAlign: "left", width: 150//flex: 1
        });

        //厂家传真号
        me.FaxNoText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "厂家传真号", labelWidth: 70, margins: '15 0 0 0',
            anchor: "80%", labelAlign: "left", width: 150//flex: 1
        });

        //收件人电话
        me.PhoneText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield", fieldLabel: "收件人电话", labelWidth: 70, margins: '15 0 0 0',
            anchor: "80%", labelAlign: "left", width: 150//flex: 1
        });

        //定义选择公司表格
        //定义选择公司表格的model
        Ext.define("companylistmodel", {
            extend: "Ext.data.Model",
            fields: [
                "companyCode",
                "companyChinese",
                "address",
                "province",
                "postCode",
                "eMail",
                "receiver",
                "faxNo",
                "phone"
            ],
            url: "_blank",
        });

        //定义选择公司表格的store
        me.companyliststore = Ext.create("Ext.data.Store", {
            model: "companylistmodel"
        });
        //定义选择公司表格的view
        me.companylistgrid = Ext.widget("grid", {
            region: "center",
            height: 68,
            stateful: true,
            multiSelect: true,
            //hideHeaders: true,//隐藏表头
            flex: 1,
            store: me.companyliststore,
            viewConfig: {
                stripeRows: true,
                enableTextSelection: false
                //getRowClass: function () {
                //    // 在这里添加自定样式 改变这个表格的行高
                //    return 'x-grid-row upload-file-grid-row';
                //}
            },
            columns: [
                { text: '编码', dataIndex: 'companyCode', width: 60 },
                { text: '名称', dataIndex: 'companyChinese', width: 150 },
                { text: '地址', dataIndex: 'address', width: 150 },
                { text: '省份', dataIndex: 'province', width: 60 },
                { text: '邮编', dataIndex: 'postCode', width: 60 },
                { text: '邮箱', dataIndex: 'eMail', width: 60 },
                { text: '传真', dataIndex: 'faxNo', width: 100 },
                { text: '联系人', dataIndex: 'receiver', width: 100 },
                { text: '电话', dataIndex: 'phone', width: 100 }
            ],
            listeners: {
                itemmousedown: function (view, record, item, index, e, eOpts) {
                    //Ext.Msg.alert("错误信息", record.data.userName);
                    //me.mouseDownGrid = "selectUserGrid";
                    //me.dragUser = record.data.userName;
                },
                select: function (view, record, index, eOpts) {
                    //Ext.Msg.alert("错误信息", record.data.companyChinese);
                    me.onCompanyGridSelect(view, record, index, eOpts);
                }
            }
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
                      },
                      items: [
                          //{
                          //    layout: "hbox",
                          //    width: '100%', baseCls: 'my-panel-no-border',//隐藏边框
                          //    items: [{ baseCls: 'my-panel-no-border', flex: 1 },
                          //    {
                          //        xtype: 'label',
                          //        cls: 'classDiv2',
                          //        itemId: 'label1',
                          //        text: '发 文 处 理 表 单', margins: '0 0 0 10'
                          //    }, { baseCls: 'my-panel-no-border', flex: 1 }]
                          //},
                          {
                              xtype: "fieldset", margin: '8 16 8 16',
                              baseCls: 'my-panel-no-border',//隐藏边框
                              layout: {
                                  type: 'hbox',
                                  pack: 'start',
                                  align: 'stretch'
                              },
                              items: [
                                  {
                                      xtype: "fieldset", margin: '0 8 0 0',
                                      //baseCls: 'my-panel-no-border',//隐藏边框
                                      title: '厂家列表',
                                      layout: {
                                          type: 'hbox',
                                          pack: 'start',
                                          align: 'stretch'
                                      },
                                      width:420,height:420,
                                      items: [
                                            me.companylistgrid
                                      ]
                                  }, {
                                      xtype: "fieldset", margin: '0 0 0 0',
                                      //baseCls: 'my-panel-no-border',//隐藏边框
                                      title: '厂家资料',
                                      layout: {
                                          type: 'vbox',
                                          pack: 'start',
                                          align: 'stretch'
                                      },
                                      flex:1,
                                      items: [
                                          me.CompanyCodeText,
                                          me.CompanyChineseText,
                                          me.AddressText,
                                          me.ProvinceText,
                                          me.PostCodeText,
                                          me.EMailText,
                                          me.ReceiverText,
                                          me.FaxNoText,
                                          me.PhoneText
                                      ]
                                  }
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
                              text: "确定", width: 60, margins: "5 5 10 5",
                              listeners: {
                                  "click": function (btn, e, eOpts) {//添加点击按钮事件
                                      me.send_editCompany();

                                  }
                              }
                          },
                          {
                              xtype: "button",
                              text: "取消", width: 60, margins: "5 15 10 5",
                              listeners: {
                                  "click": function (btn, e, eOpts) {//添加点击按钮事件
                                      //Ext.Msg.alert("您展开了目录树节点！！！", "您展开了目录树节点！节点ID:" + me.tempDefnId);
                                      winEditCompany.close();
                                  }
                              }
                          }
                      ]
                  }
                      ]
                  }
              ]
          })];

        //获取打开表单时的默认参数
        me.sendEditCompanyDefault();


        me.callParent(arguments);
    },

    //获取新建厂家资料目录表单默认参数
    sendEditCompanyDefault: function () {
        var me = this;

        //通过extjs的ajax获取操作全部名称
        Ext.Ajax.request({
            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.HXPC_Plugins.Company", A: "GetEditCompanyDefault",
                sid: localStorage.getItem("sid"), ProjectKeyword: me.projectKeyword
            },
            success: function (response, options) {
                me.sendGetEditCompanyDefault_callback(response, options);//, funCallback);
            },
            failure: function (response, options) {
                ////Ext.Msg.alert("错误", "连接服务器失败！<br>" + response.responseText);
            }
        });
    },

    //处置获取新建厂家资料目录表单默认参数的返回
    sendGetEditCompanyDefault_callback: function (response, options) {
        var me = this;



            //获取数据后，更新窗口
            var res = Ext.JSON.decode(response.responseText, true);
            var state = res.success;
            if (state === true) {
                var recod = eval(res.data[0]);

                var companyList = eval(recod.CompanyList);

                for (var itemKey in companyList) {
                    //me.projectKeyword = companyList[itemKey].companyCode;
                    if (companyList[itemKey].companyCode != undefined) {
                        //str = str + companyList[itemKey].projectName;
                        //插入行到文件selectUserGrid
                        var r = Ext.create('companylistmodel', {
                            //name: files[i].name
                            companyCode: companyList[itemKey].companyCode,
                            companyChinese: companyList[itemKey].companyChinese,
                            address: companyList[itemKey].address,
                            province: companyList[itemKey].province,
                            postCode: companyList[itemKey].postCode,
                            eMail: companyList[itemKey].eMail,
                            receiver: companyList[itemKey].receiver,
                            faxNo: companyList[itemKey].faxNo,
                            phone: companyList[itemKey].phone
                        });

                        var rowlength = me.companylistgrid.getStore().data.length;
                        me.companylistgrid.getStore().insert(rowlength, r);
                    }
                }
            }
    },

    //响应选择公司列表表格的事件
    onCompanyGridSelect: function (view, record, index, eOpts) {
        var me = this;

        me.CompanyCodeText.setValue(record.data.companyCode);
        me.CompanyChineseText.setValue(record.data.companyChinese);
        me.AddressText.setValue(record.data.address);
        me.ProvinceText.setValue(record.data.province);
        me.PostCodeText.setValue(record.data.postCode);
        me.EMailText.setValue(record.data.eMail);
        me.ReceiverText.setValue(record.data.receiver);
        me.FaxNoText.setValue(record.data.faxNo);
        me.PhoneText.setValue(record.data.phone);
    },

    //新建公司资料目录
    send_editCompany: function () {
        var me = this;

        //获取公司编号Text
        var companyCode = me.CompanyCodeText.value;

        //获取公司名称Text
        var companyChinese = me.CompanyChineseText.value;

        //获取地址Text
        var address = me.AddressText.value;

        //获取省份Text
        var province = me.ProvinceText.value;

        //获取邮编Text
        var postCode = me.PostCodeText.value;

        //获取邮件Text
        var eMail = me.EMailText.value;

        //获取接收人Text
        var receiver = me.ReceiverText.value;

        //获取传真号Text
        var faxNo = me.FaxNoText.value;

        //获取电话Text
        var phone = me.PhoneText.value;

        ////获取表单数据，转换成JSON字符串
        var projectAttr =
        [
            { name: 'companyCode', value: companyCode },
            { name: 'companyChinese', value: companyChinese },
            { name: 'address', value: address },
            { name: 'province', value: province },
            { name: 'postCode', value: postCode },
            { name: 'eMail', value: eMail },
            { name: 'receiver', value: receiver },
            { name: 'faxNo', value: faxNo },
            { name: 'phone', value: phone },
        ];

        var projectAttrJson = Ext.JSON.encode(projectAttr);

        Ext.MessageBox.wait("正在创建互提资料单，请稍候...", "等待");

        Ext.Ajax.request({

            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.HXPC_Plugins.Company", A: "EditCompany",
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
                    me.send_editCompany_callback(me.newProjectKeyword, options, "");//, me.projectKeyword, closeWin);

                } else {
                    var errmsg = res.msg;
                    Ext.Msg.alert("错误信息", errmsg);
                }
            },
            failure: function (response, options) {
                ////Ext.Msg.alert("错误", "连接服务器失败！<br>" + response.responseText);
            }

        })

    },

    //处理新建公司资料目录的返回事件
    send_editCompany_callback: function (projectKeyword, options) {
        var me = this;

        me.refreshWin(me.newProjectKeyword, true);
    },

    //刷新表单，参数:parentKeyword:新建的联系单目录
    refreshWin: function (projKeyword, closeWin) {
    var me = this;
    var tree = Ext.getCmp(me.mainPanelId).up('_mainSourceView').down('_mainProjectTree').down('treepanel');//;
    var viewTreeStore = tree.store;

    viewTreeStore.load({
        callback: function (records, options, success) {//添加回调，获取子目录的文件数量
            if (closeWin)
                winEditCompany.close();

            //展开目录
            Ext.require('Ext.ux.Common.comm', function () {
                Ext.getCmp('contentPanel').down('_mainProjectTree').ExpendProject(projKeyword);
            });
        }
    });
}

});