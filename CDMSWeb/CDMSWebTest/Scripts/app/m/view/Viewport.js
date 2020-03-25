//类名中最后一个小数点之前的内容为目录，之后的是文件名,
//因为CDMSWap指向的目录是scripts / app，因而文件所在目录是scripts / app / view,
//CDMSWap的路径在主页的Loader中定义
Ext.define('CDMSWap.view.Viewport', {
    //定义一个从Ext.container.Viewport派生的类，用来搭建应用程序的整体界面
    extend: 'Ext.container.Viewport',
    //加入布局,因为是垂直划分的三部分，因而不需要使用到Border布局了，使用VBox就可以了
    //这里一定要加align，以便布局可以填满宽度
    layout: { type: 'vbox', align: 'stretch' },
    //自动加载MainPanel
    requires: ['CDMSWap.view.MainPanel'],
    _Storage_DocName:"123456aaaaaaaa",
    initComponent: function () {
        var me = this;


        me.items = [
            //顶部因为还要添加按钮，因而使用一个工具栏比较方便
            { xtype: "component", height: 3, id: "North" },//顶部只是占位，用Component就行了
            { xtype: "mainpanel", id: "mainPanel" },//中部是标签页,加了一个id，是为了方便以后使用选择器查找组件
            {
                xtype: "panel",
                height: 2,//height: 50,
                id: "South", collapsible: false, bodyStyle: 'background:#6C86AE;',
                items: [
                  {
                      xtype: "toolbar",
                      //layout: "hbox",
                      width: '100%', baseCls: 'my-panel-no-border',//隐藏边框
                      align: 'stretch', margin: '3 0 3 0', padding: '0 0 0 0',
                      pack: 'start',
                      items: [
                      //加一个Component来显示项目名称,用cls定义一个样式表logo来改变显示文字的大小
                          //{
                          //    //iconCls: "logoimg", tooltip: "退出",// scale: "large",
                          //    //text: '艾三维协同管理信息系统',
                          //    tooltip: "点击隐藏标题栏",
                          //    cls: 'btn-m-logo', margin: '0 0 0 15',
                          //    //style: 'font-size:18px',
                          //    listeners: {
                          //        "click": function (btn, e, eOpts) {//添加点击按钮事件
                          //            var north = Ext.getCmp("North");
                          //            north.setHeight(0);

                          //        }

                          //    }

                          //},
                          ////“->”符号会让工具栏的图标显示在右边
                          //"->",
                          //{

                          //    iconCls: "account_manage",
                          //    tooltip: "账号管理", scale: "large",
                          //    text: '<font color="white"; style="font-size:15px;">账号管理</font>', margin: '0 15 0 5',
                          //    //style: 'font-size:18px','<span style="font-size:15px;">
                          //    handler: function () {
                          //        //设置退出操作
                          //        //window.location = "DBSource/Logout?UserName=" + localStorage.getItem("username");
                          //    },
                          //    listeners: {
                          //        "mouseover": function () {

                          //        }
                          //    },
                          //    menu: [
                          //      {
                          //          text: "修改资料", iconCls: "qicon-edit"
                          //      },
                          //      {
                          //          text: "密码修改", iconCls: "qicon-edit", handler: function () {
                          //              //window.location = "DBSource/Logout?UserName=" + localStorage.getItem("username");
                          //              me.SetPassword();
                          //          }
                          //      },
                          //      "-",
                          //      {
                          //          text: "退出", iconCls: "logout_16", handler: function () {
                          //              window.location = "DBSource/Logout?UserName=" + localStorage.getItem("username") + "&WebViewType=mobile";
                          //          }
                          //      }
                          //    ]
                          //}
                      ]
                  }]
            },
            //用隐藏控件保存全局值，方便WebBorwser交互
            {
                xtype: 'textfield',
                fieldLabel: 'Sid',
                hidden: true,
                id: "_Storage_Sid",
                name: 'sid_name',
                alias: 'widget._Storage_Sid', // 此类的xtype类型为buttontransparent  
                anchor: '95%',
                value: ""

            },
            {
                xtype: 'textfield',
                fieldLabel: 'DocKeyword',
                hidden: true,
                id: "_Storage_DocKeyword",
                name: 'name',
                alias: 'widget._Storage_DocKeyword', // 此类的xtype类型为buttontransparent  
                anchor: '95%',
                value: ""

            }
            //顶部的高度是53，底部是13。主体部分设置flex为1，表示它会占据剩余的空间

        ];

        me.callParent(arguments);
    },

    //SetPassword: function () {
    //    var me = this;
    //    //Ext.Msg.alert("错误信息", "流程提交预处理错误！错误消息：");

    //    var fmSetPassword = Ext.create('Ext.ux.YDForm.User._SetPassword', { title: "" });
    //    winSetPassword = Ext.widget('window', {
    //        title: '修改用户密码',
    //        closeAction: 'hide',
    //        width: 380,
    //        height: 280,
    //        minWidth: 380,
    //        minHeight: 280,
    //        layout: 'fit',
    //        resizable: true,
    //        modal: true,
    //        closeAction: 'close', //close 关闭  hide  隐藏  
    //        items: fmSetPassword,
    //        defaultFocus: 'firstName'
    //    });

    //    winSetPassword.show();
    //    //监听子窗口关闭事件
    //    winSetPassword.on('close', function () {
    //    });
    //},


});