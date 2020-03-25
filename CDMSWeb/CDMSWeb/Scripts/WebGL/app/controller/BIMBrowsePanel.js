//定义主面板的控制
Ext.define('WebGL.controller.BIMBrowsePanel', {
    extend: 'Ext.app.Controller',
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
            }

        });
    }
});
