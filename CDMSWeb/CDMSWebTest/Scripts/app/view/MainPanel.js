Ext.define('CDMSWeb.view.MainPanel', {
    extend: 'Ext.tab.Panel',
    //id:"mMainPanel",
    alias: 'widget.mainpanel',//必须用alias来为组件定义别名，才可以在Viewport里使用xtype来创建组件
    flex: 1,//主体部分设置flex为1，表示它会占据剩余的空间。
    activeTab: 0,//指定初始激活显示那个标签页

    initComponent: function () {
        var me = this;
        //创建主面板
        me.items = [

            { title: "文档管理", id: "contentPanel", layout: "fit" },
            { title: "个人消息", id: "messagePanel", layout: "fit" },
            { title: "流程工作台", id: "_workflowplacePanel", layout: "fit" },
            { title: "个人工作台", id: "_workplacePanel", layout: "fit" },
            { title: "逻辑目录", id: "_logicProjectPanel", layout: "fit" },
            { title: "查询", id: "_queryPanel", layout: "fit" },
            { title: "导航页", id: "_navigationPanel", layout: "fit" }
            //{ title: "查看BIM模型", id: "_queryPanel2", layout: "fit" },
            //me.ToastWindow
            //为图片管理加回布局，布局类型为Fit
            
        ];

        //通过Userinfo判断用户是否管理员
        var roles = "." + CDMSWeb.Userinfo.Roles.join('.') + ".";
        if (roles.indexOf(".系统管理员.") >= 0) {
            //如果是系统管理员，添加用户管理标签页
            //layout:使用Fit布局后，视图就可填满标签页面板主体了
            //me.items.push({ title: "用户管理", id: "userPanel", layout: "fit" });
        }


        me.callParent(arguments);
    }, //tab切换事件
    listeners: {
        'beforerender': function (view, eOpts) {
            var me = this;
            
            var preunloadtime = localStorage.getItem("preunloadtime");
            var sourceViewTabType = localStorage.getItem("SourceViewTabType");
            var curtime = new Date().getTime();
            var time3 = curtime - preunloadtime;

            //console.log("触发渲染前的事件！" + preunloadtime + "," + curtime + "," + time3 + "," + sourceViewTabType);
            ////如果是刷新，就转到刷新前的页面
            if (time3 <= 3000 && sourceViewTabType != undefined) {
              
                me.setActiveTab(parseInt(sourceViewTabType));

                return true;
            }

            Ext.Ajax.request({
                url: 'WebApi/Post',
                method: "POST",
                async: false,//发送异步请求
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
                            //var mpanel = Ext.getCmp('mainPanel');
                            
                            me.setActiveTab(6);
                        } else {
                            //var mpanel = Ext.getCmp('mainPanel');

                            me.setActiveTab(0);
                        }
                    }
                },
                failure: function (response, options) {
                    ////Ext.Msg.alert("错误", "连接服务器失败！<br>" + response.responseText);
                }
            });
            //Ext.Msg.alert("错误", "触发渲染前的事件！");
        },
        'tabchange': function (tabpanel, node) {
            if (node.title.indexOf("导航") >= 0) {
                
                Ext.getCmp('_navigationPanel').down('_mainNavigationView').refreshView();

                     
            }
        }
    }

});



