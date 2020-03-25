//定义主面板的控制
Ext.define('CDMSWeb.controller.MainPanel', {
    extend: 'Ext.app.Controller',
    requires: ["Ext.ux.Common.Enum"],
    init: function () {
        //使用控制器的control方法来获取主面板内的标签页，并为其添加activate事件
        this.control({
            //组件id前面要添加“#”符号，表示使用id查找组件
            '#contentPanel': {
                activate: {
                    single: true,  //single配置项说明该事件只执行一次
                    fn: function (panel) {  //配置项fn则是事件的回调函数
                        //调用getController方法加载文章管理的控制器到应用,并调用控制器的init方法
                        //引用路径：\CDMSWeb\Scripts\app\controller\Content.js
                        this.application.getController('Content').init();
                    }
                }
            },
            //定义逻辑目录
            '#_logicProjectPanel': {
                activate: {
                    single: true,  //single配置项说明该事件只执行一次
                    fn: function (panel) {  //配置项fn则是事件的回调函数
                        //调用getController方法加载文章管理的控制器到应用,并调用控制器的init方法
                        //引用路径：\CDMSWeb\Scripts\app\controller\Content.js
                        this.application.getController('_LogicProject').init();
                    }
                }
            },
            //定义个人工作台
            '#_workplacePanel': {
                activate: {
                    single: true,  //single配置项说明该事件只执行一次
                    fn: function (panel) {  //配置项fn则是事件的回调函数
                        //调用getController方法加载文章管理的控制器到应用,并调用控制器的init方法
                        //引用路径：\CDMSWeb\Scripts\app\controller\Content.js
                        this.application.getController('_Workplace').init();
                    }
                }
            },
            //定义查询
            '#_queryPanel': {
                activate: {
                    single: true,  //single配置项说明该事件只执行一次
                    fn: function (panel) {  //配置项fn则是事件的回调函数
                        //调用getController方法加载文章管理的控制器到应用,并调用控制器的init方法
                        //引用路径：\CDMSWeb\Scripts\app\controller\Content.js
                        this.application.getController('_Query').init();
                    }
                }
            },
            //定义导航页
            '#_navigationPanel': {
                activate: {
                    single: true,  //single配置项说明该事件只执行一次
                    fn: function (panel) {  //配置项fn则是事件的回调函数
                        //调用getController方法加载文章管理的控制器到应用,并调用控制器的init方法
                        //引用路径：\CDMSWeb\Scripts\app\controller\Content.js
                        this.application.getController('_Navigation').init();
                    }
                }
            },
            '#messagePanel': {
                //个人消息的activate方法
                activate: {
                    single: true,
                    fn: function (panel) {
                        //create方法的第一参数必须为字符串，这样，Ext才会去自动加载类文件
                        this.application.getController('Message').init();

                    }
                }
            },
            '#_workflowplacePanel': {
                //流程工作台的activate方法
                activate: {
                    single: true,
                    fn: function (panel) {
                        //create方法的第一参数必须为字符串，这样，Ext才会去自动加载类文件
                        this.application.getController('_WorkflowPlace').init();
                    }
                }
            },
            '#userPanel': {
                activate: {
                    single: true,
                    fn: function (panel) {
                        //调用getController方法加载控制器并调用控制器的init方法就行
                        this.application.getController('Users').init();
                    }
                }
            }
            
        });
        Ext.get(window).on('beforeunload', function (e) {

            var me = this;

            var date = new Date().getTime();
            window.localStorage.setItem("preunloadtime", date);

            Ext.getCmp('contentPanel').down('_mainProjectTree').saveLastProject(function () { });


            //return (window.event.returnValue = e.returnValue = '确认离开当前页面？！！' + window.SourceViewType)
        });
    }
});
