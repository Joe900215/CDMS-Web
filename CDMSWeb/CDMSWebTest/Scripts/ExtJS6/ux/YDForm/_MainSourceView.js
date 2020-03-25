/*定义数据源视图面板*/
Ext.define('Ext.ux.YDForm._MainSourceView', {
    //extend: 'Ext.container.Container',
    extend: 'Ext.panel.Panel',
    alias: 'widget._mainSourceView', // 此类的xtype类型为buttontransparent  
    //title: "项目", region: "west", collapsible: true, rootVisible: false,
    //width: 300, minWidth: 100, split: true,
    layout: "border",
    SourceViewType:"1",
    initComponent: function () {
        var me = this;
        me.renderTo = me.el;

        //定义流程TAB页
        me.mainAttrTab = Ext.create('Ext.ux.YDForm._MainAttrTab');

        //定义文档列表
        me._maindocgrid = Ext.create('Ext.ux.YDForm._MainDocGrid');

        //定义目录树
        me._mainprojecttree = Ext.create('Ext.ux.YDForm._MainProjectTree');

        //添加属性TAB页面到容器
        me.items = [

                                 me._mainprojecttree,
                                 me._maindocgrid,
                                 me.mainAttrTab
        ];

        me.callParent(arguments);
    },
    setSourceViewType: function (projectType) {
        var me = this;
        me._mainprojecttree.setSourceViewType(projectType);
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
            var mpanel = Ext.getCmp('mainPanel');
            //检查是否是在项目源页
            if (mpanel.activeTab.title==="文档管理") {

                Ext.Ajax.request({
                    url: 'WebApi/Post',
                    params: {
                        C: 'AVEVA.CDMS.WebApi.MessageController', A: 'GetUserNoReadMessageList',
                        sid: localStorage.getItem("sid")
                    },
                    success: function (response) {
                        //showMsgTipsWin(response);
                    },
                    failure: function (e, opt) {
                        var me = this, msg = "";
                        // Ext.Msg.alert("错误", "1");
                    }

                });
            }
            //alert('run()　方法被执行.　传入参数个数：' + arguments.length + ",　分别是："
            //　　　　　　　　　　　　　　　　+ arguments[0] + "," + arguments[1] + "," + arguments[2]);
            //return false;　　//不返回　false，run()　方法会被永无止境的调用
        },
        scope : this,
        args: [100, 200, 300],  //添加args传入参数后，必须在run里面添加 return false;否则run()　方法会被永无止境的调用
        interval: 60000,//480000 ,　//每隔8分钟执行一次，本例中　run()　只在　1　秒后调用一次
        repeat: 2　　　　　　　//重复执行　2　次,　这个参数已不再启作用了
    });

    function showMsgTipsWin(response) {
        var res = Ext.decode(response.responseText);
        if (res.success == true) {
            var recods = eval(res.data);
            var title="",content;
            var node;
            var msgTotal;
            me.msg = "";

                for (var recId in recods) {
                    title = recods[recId].Title;//获取数量统计
                    content = recods[recId].Content;//获取数量统计
                    me.msg = me.msg + title + "</br>";
                    me.firstMsgTitle = title;
                    break;
                }
            //当消息数量有变化时，才弹出消息框
                if (res.total != me.msgStoreCount || me.firstMsgTitle != title) {
                    me.msgStoreCount = res.total;

                if (me.msg != "") {
                    var _MsgTips = Ext.create('Ext.ux.YDForm._MsgTipsForm', { title: "" });

                    msgTipsWin = new Ext.Window({
                        width: 271,
                        height: 192,
                        layout: 'fit',
                        modal: false,
                        plain: true,
                        shadow: false, //去除阴影  
                        draggable: false, //默认不可拖拽  
                        resizable: false,
                        closable: true,
                        closeAction: 'hide', //默认关闭为隐藏  
                        autoHide: 30, //30秒后自动隐藏，false则不自动隐藏 
                        title: '消息提醒',
                        //html: '' + me.msg + '',//'',
                        items: _MsgTips,
                        constructor: function (conf) {
                            Ext.Window.superclass.constructor.call(this, conf);
                            this.initPosition(true);
                        },
                        initEvents: function () {
                            Ext.Window.superclass.initEvents.call(this);

                            //自动隐藏  
                            if (false !== this.autoHide) {
                                var task = new Ext.util.DelayedTask(this.hide, this), second = (parseInt(this.autoHide) || 3) * 1000;;
                                task.delay(second);
                            }

                            this.on('beforeshow', this.showTips);
                            this.on('beforehide', this.hideTips);
                            this.initPosition(true);
                            //window大小改变时，重新设置坐标  
                            Ext.EventManager.onWindowResize(this.initPosition, this);
                            //window移动滚动条时，重新设置坐标  
                            Ext.EventManager.on(window, 'scroll', this.initPosition, this);

                        },
                        //参数flag为true时强制更新位置  
                        initPosition: function (flag) {
                            //alert('1');
                            //不可见时，不调整坐标
                            if (true !== flag && this.hidden) {
                                return false;
                            }
                            var doc = document, bd = (doc.body || doc.documentElement);
                            //Ext取可视范围宽高(与上面方法取的值相同), 加上滚动坐标  
                            var left = bd.scrollLeft + document.body.clientWidth - 4 - this.width;
                            var top = bd.scrollTop + document.body.clientHeight - 4 - this.height;

                            this.setPosition(left, top);
                        },
                        showTips: function () {
                            var self = this;
                            if (!self.hidden) { return false; }
                            //初始化坐标
                            self.initPosition(true);
                            self.el.slideIn('b', {
                                callback: function () {
                                    //显示完成后,手动触发show事件,并将hidden属性设置false,否则将不能触发hide事件   
                                    self.fireEvent('show', self);
                                    self.hidden = false;
                                }
                            });
                            //不执行默认的show
                            return false;
                        },
                        hideTips: function () {
                            var self = this;
                            if (self.hidden) { return false; }
                            self.el.slideOut('b', {
                                callback: function () {
                                    //渐隐动作执行完成时,手动触发hide事件,并将hidden属性设置true  
                                    self.fireEvent('hide', self);
                                    self.hidden = true;
                                }
                            });
                            //不执行默认的hide
                            return false;
                        }
                    });//.show();


                    msgTipsWin.show();

                    _MsgTips.countLabel.setText("您有 " + res.total + " 条新消息");
                    title = "标题： " + title;
                    content = "内容： " + content;
                    title = title.substring(0, 99);//48);
                    content = content.substring(0, 132);//86);
                    _MsgTips.titleLabel.setText(title);
                    _MsgTips.contentLabel.setText(content);
                }
            }
        }
    };

}

);
