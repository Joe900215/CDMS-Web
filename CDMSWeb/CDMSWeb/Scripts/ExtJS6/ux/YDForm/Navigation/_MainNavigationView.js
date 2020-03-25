/*定义数据源视图面板*/
Ext.define('Ext.ux.YDForm.Navigation._MainNavigationView', {
    //extend: 'Ext.container.Container',
    extend: 'Ext.panel.Panel',
    alias: 'widget._mainNavigationView', // 此类的xtype类型为buttontransparent  
    //layout: "border",
    layout: "fit",
    SourceViewType: "1",
    initComponent: function () {
        var me = this;
        me.renderTo = me.el;
        //me.renderTo = Ext.getBody();

        //收藏夹表格
        me._navfavoritesgrid = Ext.create('Ext.ux.YDForm.Navigation._NavFavoritesGrid');

        //定义消息列表
        me._navmessagegrid = Ext.create('Ext.ux.YDForm.Navigation._NavMessageGrid');

        // -- 这里可以插入项目用户自定义控件
        //me.navnewdocgrid = Ext.create('Ext.plug_ins.ZHEPC_Plugins.Dist.NavNewDocGrid');
        me.navnewdocgrid = Ext.create('Ext.plug_ins.WTMS_Plugins.Dist.NavWorkTaskGrid');

        me._navCheckBox = Ext.create("Ext.form.field.Checkbox", {
            xtype: "checkbox",
            boxLabel: "启动时显示此页",
            listeners: {
                change: function (view, newValue, oldValue, eOpts) {
                    me.onNavCheckBoxChange(view, newValue, oldValue, eOpts);
                }
            }
        });

        //添加属性TAB页面到容器
        me.items = [
            {
                layout: {
                    type: 'vbox',
                    pack: 'start',
                    align: 'stretch'
                },items:[
                {
                    layout: {
                        type: 'hbox',
                        pack: 'start',
                        align: 'stretch'
                    },
                    flex: 1,
                    items: [
                   //{
                   //    xtype: "grid",
                   //    title: "待办事项",
                   //    columns: [
                   //        {
                   //            //header: "Column 1",
                   //            sortable: true,
                   //            resizable: true,
                   //            dataIndex: "data1",
                   //            width: 100
                   //        }
                   //    ]
                   //},
                   me._navfavoritesgrid,
                   me.navnewdocgrid,
                   me._navmessagegrid 

                    ]
                },
                		me._navCheckBox
                ]
            }
        ];

        me.callParent(arguments);
    },
    setSourceViewType: function (projectType) {
        var me = this;
        me._mainprojecttree.setSourceViewType(projectType);
    },

    onNavCheckBoxChange: function (view, newValue, oldValue, eOpts) {
        var me = this;

        //Ext.Msg.alert("错误", "按下了checkbox:" + newValue);

        //if (newValue === true) {
        var bDisplay = newValue;

            Ext.Ajax.request({
                url: 'WebApi/Post',
                method: "POST",
                params: {
                    C: "AVEVA.CDMS.WebApi.UserController", A: "SetNavigationDisplay",
                    bDisplay: bDisplay, sid: localStorage.getItem("sid")
                },
                success: function (response, options) {
                    var res = Ext.JSON.decode(response.responseText, true);
                    var state = res.success;
                    if (state === false) {
                        var errmsg = res.msg;
                        Ext.Msg.alert("错误信息", errmsg);
                    }
                    else {
                        //var recod = res.data[0];
                        //var State = recod.state;
                        //if (State === "delSuccess") {
                        //    me.refreshGrid();
                        //}
                    }
                },
                failure: function (response, options) {
                    ////Ext.Msg.alert("错误", "连接服务器失败！<br>" + response.responseText);
                }
            });
        //}
    },

    setNavCheckBox: function () {
        var me = this;
        Ext.Ajax.request({
            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.WebApi.UserController", A: "GetNavigationDisplay",
                sid: localStorage.getItem("sid")
            },
            success: function (response, options) {
                var res = Ext.JSON.decode(response.responseText, true);
                var state = res.success;
                if (state === false) {
                    var errmsg = res.msg;
                    Ext.Msg.alert("错误信息", errmsg);
                }
                else {
                    var recod = res.data[0];
                    var State = recod.IFShowNAV;
                    if (State === "true") {
                        me._navCheckBox.setValue(true);
                    } else {
                        me._navCheckBox.setValue(false);
                    }
                }
            },
            failure: function (response, options) {
                ////Ext.Msg.alert("错误", "连接服务器失败！<br>" + response.responseText);
            }
        });
        
    },
    refreshView: function () {

        var me = this;
        //刷新收藏夹
        me._navfavoritesgrid.refreshGrid();

        //刷新消息列表
        me._navmessagegrid.refreshGrid();

        // 刷新项目用户自定义控件
        me.navnewdocgrid.refreshGrid();

        me.setNavCheckBox();

    }
    

}
);

//设置自动刷新轮询数据库，弹出消息框
Ext.onReady(function () {
    var me = this;
    me.msg = "";
    me.firstMsgTitle = "";
    me.msgStoreCount = 0;
    var runner = new Ext.util.TaskRunner();//定义多线程
    runner.start({　　　　　　//任务被调用的方法
        run: function () {　//run　方法原型不变，实际可以去遍历这个　arguments　参数数组
            //var mpanel = Ext.getCmp('mainPanel');
            ////检查是否是在项目源页
            //if (mpanel.activeTab.title === "文档管理") {

            //    Ext.Ajax.request({
            //        url: 'WebApi/Post',
            //        params: {
            //            C: 'AVEVA.CDMS.WebApi.MessageController', A: 'GetUserNoReadMessageList',
            //            sid: localStorage.getItem("sid")
            //        },
            //        success: function (response) {
            //            showMsgTipsWin(response);
            //        },
            //        failure: function (e, opt) {
            //            var me = this, msg = "";
            //            // Ext.Msg.alert("错误", "1");
            //        }

            //    });
            //}
            ////alert('run()　方法被执行.　传入参数个数：' + arguments.length + ",　分别是："
            ////　　　　　　　　　　　　　　　　+ arguments[0] + "," + arguments[1] + "," + arguments[2]);
            ////return false;　　//不返回　false，run()　方法会被永无止境的调用
        },
        scope: this,
        args: [100, 200, 300],  //添加args传入参数后，必须在run里面添加 return false;否则run()　方法会被永无止境的调用
        interval: 60000,//480000 ,　//每隔8分钟执行一次，本例中　run()　只在　1　秒后调用一次
        repeat: 2　　　　　　　//重复执行　2　次,　这个参数已不再启作用了
    });


}

);
