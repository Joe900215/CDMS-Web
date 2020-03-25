/*定义目录树*/
Ext.define('Ext.ux.YDForm._MainProjectTree', {
    //extend: 'Ext.container.Container',
    extend: 'Ext.panel.Panel',
    alias: 'widget._mainProjectTree', // 此类的xtype类型为buttontransparent  
    title: "项目", region: "west", collapsible: true, rootVisible: false,
    width: 300, minWidth: 100, split: true,
    layout: 'fit',
    initComponent: function () {
        var me = this;
        me.renderTo = me.el;

        //展开目录树时，如果ExpendDocKeyword不为""，就转到ExpendDocKeyword所在的文档，否则就跳转到目录的第一个文档记录
        me.ExpendDocKeyword = "";

        me.projTreeSeleteTime = 0;

        me.bFirstExpandPath = true;

        me.title = localStorage.getItem("userdesc") + "   " + " (" + localStorage.getItem("username") + ")";
        me.sourceViewType = window.SourceViewType;

        //定义project属性store
        me._ProjectsStore = Ext.create("Ext.data.TreeStore", {
            //batchActions: false,
            //remoteFilter: false,
            //remoteSort: false,
            useArrows: true,
            rootVisible: false,
            multiSelect: false,
            singleExpand: false,
            model: "CDMSWeb.model.ProjectTree",
            root: { id: "Root", text: "根目录" , expanded: true },

            //代理定义
            proxy: {
                type: 'ajax',
                //代理的API定义了操作的提交路径
                //路径：\CDMSWeb\Controllers\FileController.cs
                api: {
                    read: 'WebApi/Get?C=AVEVA.CDMS.WebApi.ProjectController&A=GetProjectListJson&ProjectType=' + me.sourceViewType + '&sid=' + localStorage.getItem("sid"),//调用路径：\simplecdms\controllers\projectcontroller.cs
                    update: 'WebApi/Post?C=AVEVA.CDMS.WebApi.ProjectController&A=SaveProject',
                },
                //url: 'WebApi/Get?C=AVEVA.CDMS.WebApi.ProjectController&A=GetProjectListJson&ProjectType=' + me.sourceViewType + '&sid=' + localStorage.getItem("sid"),//调用路径：\simplecdms\controllers\projectcontroller.cs

                //在代理定义中，reader和writer的定义可标准化数据的输入输出，
                //这个与用户中的定义是一样的
                reader: {
                    messageProperty: "Msg",
                    type: 'json',
                    //root: "data"
                    rootProperty: "data"
                },
                writer: {
                    type: "json",
                    //encode: true,
                    //root: "data",
                    //allowSingle: false
                },
                listeners: {
                    exception: CDMSWeb.ProxyException
                }
            }
        });

        //定义设置用户按钮
        me._editUserButton = Ext.create('Ext.button.Button', {
            iconCls: "user-blue-16", scope: me, tooltip: '设置用户',// hidden: true,
            text: "用户",
            listeners: {
                "click": function (btn, e, eOpts) {
                    me.UserSetting();
                }
            }
        });

        //定义设置用户组按钮
        me._editGroupButton = Ext.create('Ext.button.Button', {
            iconCls: "group-edit", scope: me, tooltip: '设置用户组', //hidden: true,
            text:"用户组",
            listeners: {
                "click": function (btn, e, eOpts) {
                    me.GroupSetting();
                }
            }
        });

        //定义目录树按钮
        me._projectTreeTbar = Ext.create('Ext.toolbar.Toolbar', {
            xtype: 'toolbar',
            dock: 'top',
            items: [

                {
                    iconCls: "folder-add", scope: me, tooltip: '创建目录', hidden: true, listeners: {
                        "click": function (btn, e, eOpts) {
                            me.CreateNewProject();
                        }
                    }
                },
                {
                    iconCls: "file-batchcreate", scope: me, tooltip: '复制创建目录', hidden: true, listeners: {
                        "click": function (btn, e, eOpts) {
                            me.CreateProjByDef();
                        }
                    }
                },
                {
                    iconCls: "folder-edit", scope: me, tooltip: '编辑目录属性', hidden: true, listeners: {
                        "click": function (btn, e, eOpts) {
                            me.ModiProjAttr();
                        }
                    }
                },//, disabled: true }
                {
                    iconCls: "folder-delete", scope: me, tooltip: '删除目录', hidden: true, listeners: {
                        "click": function (btn, e, eOpts) {
                            me.DelProject("false");
                        }
                    }
                },
                //{
                //    iconCls: "user-blue-16", scope: me, tooltip: '设置用户', hidden: true, listeners: {
                //        "click": function (btn, e, eOpts) {
                //            me.UserSetting();
                //        }
                //    }
                //},
                me._editUserButton,
                //{
                //    iconCls: "group-edit", scope: me, tooltip: '设置用户组', hidden: true, listeners: {
                //        "click": function (btn, e, eOpts) {
                //            me.GroupSetting();
                //        }
                //    }
                //},
                me._editGroupButton,
                {
                    iconCls: "refresh", scope: me, tooltip: '刷新',
                    text: "刷新",
                    listeners: {
                        "click": function (btn, e, eOpts) {
                            me.RefreshProjTree("LastProject");
                        }
                    }
                }
            ]
        });

        //定义目录树
        me.mainprojecttree = Ext.widget("treepanel", {

            store: me._ProjectsStore,
            rootVisible: false,
            width: 300, minWidth: 100, split: true,
            tbar: me._projectTreeTbar,
            //root: { id: "Root", text: "根目录"}, //, expanded: true },
            saveDelay: 0,//如果许多state事件在短时间内被触发，这个缓冲区将被应用。
            viewConfig: {
                stripeRows: true
            },
            listeners: {
                "beforeitemmousedown": function (view, record, item, index, e, eOpts) {
                    //var i = 1;
                },
                "select": function (model, record, index, eOpts) {
                    //菜单测试，暂时屏蔽
                    //这里延时，会导致右键菜单偶尔弹出失败
                    //var myDate = new Date();
                    //var tTime = myDate.getTime();      //获取当前时间(从1970.1.1开始的毫秒数)
                    //if (tTime-me.projTreeSeleteTime   < 100)
                    //{
                    //    /*js实现sleep功能 单位：毫秒*/
                    //    function sleep(numberMillis) {
                    //        var now = new Date();
                    //        var exitTime = now.getTime() + numberMillis;
                    //        while (true) {
                    //            now = new Date();
                    //            if (now.getTime() > exitTime)
                    //                return;
                    //        }
                    //    }

                    //    sleep(100);
                    //}
                    //me.projTreeSeleteTime = tTime;

                    //添加延时，防止多次点击目录树
                    setTimeout(function () {
                        me.onTreeItemSelect(model, record);
                    }, 5 * 100);

                },
                "afteritemexpand": function (node, index, item, eOpts) {//处理目录树展开后的事件(添加各个目录下文档数量)
                    me.afterTreeExpand(node, index, item, eOpts);
                },

                "itemcontextmenu": function (view, rec, node, index, e) {//添加鼠标右键菜单事件
                    me._showContextMenu(view, rec, node, index, e);
                },
                "containercontextmenu": function (view, e, eOpts) {
                    me._showContainerContextMenu(view, e, eOpts);
                },
                "afterrender": function () {//完成渲染后的事件

                    me.ExpendProject("LastProject");
                },
                "itemmouseenter": function (dataview, record, item, index, e, eOpts) {
                    //菜单测试，暂时屏蔽
                    me.onMainProjectTreeItemMouseEnter(dataview, record, item, index, e, eOpts);
                }
            }

        });
        //添加属性TAB页面到容器
        me.items = [
              me.mainprojecttree
        ];

        window.activeTreePanelId = me.mainprojecttree.id;

        me.callParent(arguments);
    },

    setSourceViewType: function (projectType) {
        var me = this;
        me.sourceViewType = projectType;
    },

    onTreeItemSelect: function (model, record) {
        var me = this;
        var Keyword = record.data.Keyword;
        var text = record.data.Desc;

        //if (sels.length > 0) {

        Keyword = record.data.Keyword;

        ////获取文件夹地址栏的路径
        //var getProjectShowPath2 = function (keyword, async) {
        //    var response = Ext.Ajax.request({
        //        url: 'WebApi/Post',
        //        method: "POST",
        //        async: async,
        //        params: {
        //            C: "AVEVA.CDMS.WebApi.ProjectController", A: "GetProjectShowPath",
        //            sid: localStorage.getItem("sid"), ProjectKeyword: keyword
        //        }
        //    });
        //    return Ext.decode(response, true);
        //};

        //getProjectShowPath2(Keyword, true).then(function (response, opts) { //用异步方法调用，并增加遮罩
        //    var res = Ext.JSON.decode(response.responseText);
        //    var state = res.success;
        //    if (state === true) {
        //        var recod = eval(res.data[0]);
        //        var docgrid = me.up('_mainSourceView').down('_mainDocGrid');
        //        docgrid.addressField.setValue(recod.path);
        //    }
        //}).done();



        //获取文件夹地址栏的路径
        Ext.Ajax.request({
            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.WebApi.ProjectController", A: "GetProjectShowPath",
                sid: localStorage.getItem("sid"), ProjectKeyword: Keyword
            },
            success: function (response, options) {

                var res = Ext.JSON.decode(response.responseText);
                var state = res.success;
                if (state === true) {
                    var recod = eval(res.data[0]);
                    var docgrid = me.up('_mainSourceView').down('_mainDocGrid');
                    docgrid.addressField.setValue(recod.path);
                }
            },
            failure: function (response, options) {

                ////Ext.Msg.alert("错误", "连接服务器失败！<br>" + response.responseText);
            }
        });

        var loadDocGridFun = function () {
            //刷新文档列表
            var docgrid = me.up('_mainSourceView').down('_mainDocGrid');
            docgrid._DocListStore.proxy.extraParams.page = 1;
            docgrid.loadDocListStore(function () { me.afterLoadDocListStore(); });

        }
        //展开目录树时，如果ExpendDocKeyword为""，就显示目录属性页
        if (me.ExpendDocKeyword === "") {
            //显示目录属性页
            me.setAttrPage(Keyword, loadDocGridFun);
        } else {
            loadDocGridFun();
        }



        //判断浏览器的类型，如果使用cefSharp浏览，通知cefSharp选中了文件夹
        if (_browserName == "cefSharp") {
            jsObj.onProjectTreeItemSelect(localStorage.getItem("sid"), Keyword, me.id);
        }

    },

    //加载文档列表完成后的事件
    afterLoadDocListStore: function (callBackFun) {
        var me = this;

        //展开目录树时，如果ExpendDocKeyword不为""，就转到ExpendDocKeyword所在的文档
        if (me.ExpendDocKeyword != "") {
            Keyword = me.ExpendDocKeyword;
            //复原ExpendDocKeyword
            me.ExpendDocKeyword = "";

            var gridItem;

            var docgrid = me.up('_mainSourceView').down('_mainDocGrid');

            for (var i = 0; i < docgrid.maindocgrid.store.getCount() ; i++) {
                var gridItemData = docgrid.maindocgrid.store.getAt(i); //遍历每一行
                if (gridItemData.data.Keyword === Keyword) {
                    gridItem = gridItemData;
                    break;
                }
            }

            if (gridItem != undefined) {
                docgrid.maindocgrid.getSelectionModel().select(gridItem, true);
            }

            if (callBackFun != undefined)
                callBackFun();

        }

    },

    //加载某个目录节点
    setAttrPage: function (Keyword, callBackFun) {
        var me = this;

        //获取类别为_mainAttrTab的控件
        var tabPlan = me.up('_mainSourceView').down('_mainAttrTab');
        var tab = tabPlan.mainattrtab.activeTab;
        tabPlan.seleObjType = "Project";

        var tabType = "";
        if (tab.title === "目录属性") {

            tabType = "projectAttr";
            tabPlan.loadDocAttrPage(tabType, Keyword);

        } else if (tab.title === "目录_附加属性") {

            tabType = "projectAttrData";
            tabPlan.loadDocAttrPage(tabType, Keyword);

        } else {//如果活动属性页是是“目录属性”，就转到“文档属性”页

            tabType = "projectAttr";

            //tabPlan.setAttrTabDisplay("Project", Keyword);

            //Tab页Change事件锁，防止目录和文档之间互相切换时，重复执行Tab的Change事件
            tabPlan.TabChangeLock = true;

            tabPlan.loadDocAttrPage(tabType, Keyword);
        }
        callBackFun();
    },

    //处理目录树展开后的事件(添加各个目录下文档数量)
    afterTreeExpand: function (node, index, item, eOpts) {
        var me = this;
        var runner = new Ext.util.TaskRunner();//定义多线程
        runner.start({　　　　　　//任务被调用的方法
            run: function () {　//run　方法原型不变，实际可以去遍历这个　arguments　参数数组
                var pList = "";
                var childnodes = arguments[0].childNodes;
                for (var i = 0; i < childnodes.length; i++) {  //从节点中取出子节点依次遍历
                    var rootnode = childnodes[i];
                    if (i === 0) {
                        pList = pList + rootnode.data.Keyword;//.internalId;
                    } else {
                        pList = pList + "," + rootnode.data.Keyword;//.internalId;
                    }
                }
                var parentNode = arguments[0];
                Ext.Ajax.request({
                    url: 'WebApi/Post',
                    method: "POST",
                    params: {
                        C: "AVEVA.CDMS.WebApi.ProjectController", A: "GetChildsDocsCount",
                        ProjectList: pList, sid: localStorage.getItem("sid")
                    },
                    //parentNode: arguments[0],
                    success: function (response, options) {
                        try {
                            //获取到文档数量后，更新到tree
                            var res = Ext.JSON.decode(response.responseText);
                            var state = res.success;
                            if (state === true) {
                                var recod = eval(res.data[0]);
                                var viewTree = me.mainprojecttree;
                                var viewTreeStore = viewTree.store; //me.getProjectsTreeStore();
                                var docCount;
                                var node;
                                for (var nodeid in recod) {

                                    var childnodes = parentNode.childNodes;
                                    for (var i = 0; i < childnodes.length; i++) {  //从节点中取出子节点依次遍历

                                        if (parentNode.childNodes[i].data.Keyword === nodeid) {
                                            docCount = recod[nodeid];//获取数量统计


                                            if (docCount != 0) {
                                                node = parentNode.childNodes[i];
                                                var nodeText = node.data.text;
                                                var zkhStart = nodeText.indexOf(" 【");

                                                if (zkhStart >= 0) {
                                                    nodeText = nodeText.substring(0, zkhStart)
                                                }

                                                node.set('text', nodeText + " 【" + docCount + "】");
                                            }
                                        }
                                    }
                                }
                                viewTreeStore.save();//保存store,去掉前面红色小箭头
                            } else {
                                var errmsg = res.msg;
                                //Ext.Msg.alert("错误信息", errmsg);
                            }
                        } catch (e) { }
                    },
                    failure: function (response, options) {
                        ////Ext.Msg.alert("错误", "连接服务器失败！<br>" + response.responseText);
                    }
                });

                return false;　　//不返回　false，run()　方法会被永无止境的调用
            },
            args: [node, item, eOpts],  //添加args传入参数后，必须在run里面添加 return false;否则run()　方法会被永无止境的调用
            interval: 1000,　//一秒执行一次，本例中　run()　只在　1　秒后调用一次
            repeat: 1　　　　　　　//重复执行　2　次,　这个参数已不再启作用了
        });
    },

    //展开指定Project,  IgnoreShortCut:是否能跳转到快捷方式，而不是快捷方式的实体文档或目录
    ExpendProject: function (ProjectId, IgnoreShortcut, callBackFun) {

        var me = this;
        if (me.bFirstExpandPath === true) {
            setTimeout(function () {
                me.bFirstExpandPath = false;
                me.ExpendProject(ProjectId, IgnoreShortcut, callBackFun);
            }, 2000)
        }
        else {
            var viewTree = me.mainprojecttree;
            var viewTreeStore = viewTree.store;
            var IgnoreShortCut = "false";
            if (IgnoreShortcut === true) {
                IgnoreShortCut = "true";
            }

            Ext.Ajax.request({
                //url: 'Project/GetProjectPath',
                url: 'WebApi/Post',
                method: "POST",
                params: {
                    C: "AVEVA.CDMS.WebApi.ProjectController", A: "GetProjectPath",
                    Keyword: ProjectId, sid: localStorage.getItem("sid"),
                    IgnoreShortCut: IgnoreShortCut
                },
                success: function (response, options) {
                    var res = Ext.JSON.decode(response.responseText);
                    var state = res.success;
                    if (state === true) {
                        var record = res.data[0];


                        //viewTree.collapseAll();//收缩所有子节点
                        viewTree.expandPath(record.ProjectPath);

                        //判断是否当前已经选中所要跳转的节点
                        var nodes = viewTree.getSelectionModel().getSelection();//获取已选取的节点
                        if (nodes !== null && nodes.length > 0 && nodes[0].data.id === record.NodeId && record.DocKeyword != "") {

                            //设置跳转到目录的某个文档
                            me.ExpendDocKeyword = record.DocKeyword;

                            //这里需要刷新文档列表，否则直接 me.afterLoadDocListStore();之前选中的文档不会取消。
                            var docgrid = me.up('_mainSourceView').down('_mainDocGrid');
                            docgrid.loadDocListStore(function () { me.afterLoadDocListStore(callBackFun); });

                            if (callBackFun != undefined) {
                                callBackFun();
                            } else {
                                return;
                            }
                        }


                        //等待上一个函数的执行结果，查找点击树节点
                        var count = 0, is_true = false;
                        var node = viewTree.store.getNodeById(record.NodeId);
                        //var docKeyword = viewTree.store.getNodeById(record.DocKeyword);

                        setInterval(function () {
                            if (!count) {
                                node = viewTree.store.getNodeById(record.NodeId);
                                if (Ext.isObject(node)) {
                                    is_true = true;
                                }
                                count++;
                            }

                            if (is_true) {
                                //设置跳转到目录的某个文档
                                me.ExpendDocKeyword = record.DocKeyword;

                                viewTree.getSelectionModel().select(node, true);
                                //viewTree.fireEvent('click', node);
                                is_true = false;


                            }
                        }, 1000);
                    } else {
                        var errmsg = res.msg;
                        //Ext.Msg.alert("错误信息", errmsg);
                    }
                }
            });
        }
    },

    //设置用户
    UserSetting: function () {
        var me = this;

        //弹出操作窗口
        var _fmUserAdmin = Ext.create('Ext.ux.YDForm.User._UserManagement', { title: "", mainPanelId: me.mainprojecttree.id });

        winUserAdmin = Ext.widget('window', {
            title: '用户设置',
            //closeAction: 'hide',
            width: 800,
            height: 596,
            minWidth: 300,
            minHeight: 300,
            layout: 'fit',
            resizable: true,
            modal: true,
            closeAction: 'close', //close 关闭  hide  隐藏  
            items: _fmUserAdmin,
            defaultFocus: 'firstName'
        });

        winUserAdmin.show();
        //监听子窗口关闭事件
        winUserAdmin.on('close', function () {

        });
    },

    //设置用户组
    GroupSetting: function () {
        var me = this;

        //弹出操作窗口
        var _fmAdminSetting = Ext.create('Ext.ux.YDForm.Group._GroupManagement', { title: "", mainPanelId: me.mainprojecttree.id });

        winAdminSetting = Ext.widget('window', {
            title: '用户组设置',
            closeAction: 'hide',
            width: 800,
            height: 596,
            minWidth: 300,
            minHeight: 300,
            layout: 'fit',
            resizable: true,
            modal: true,
            closeAction: 'close', //close 关闭  hide  隐藏  
            items: _fmAdminSetting,
            defaultFocus: 'firstName'
        });

        winAdminSetting.show();
        //监听子窗口关闭事件
        winAdminSetting.on('close', function () {

        });
    },

    //新建目录
    CreateNewProject: function (bCreateRoot) {
        var me = this;
        //me.viewTree = Ext.getCmp(mainPanelId).down('treepanel');//获取目录树控件ID
        var nodes = me.mainprojecttree.getSelectionModel().getSelection();//获取已选取的节点
        if ((nodes !== null && nodes.length > 0) || bCreateRoot === "true") {
            var parentKeyword = "";

            if (nodes !== null && nodes.length > 0) parentKeyword = nodes[0].data.Keyword;

            var menuType = "CreateNewProject";
            if (bCreateRoot === "true" ) menuType = "CreateNewRootProject";

            //请求判断是否有创建目录权限
            Ext.Ajax.request({
                url: 'WebApi/Post',
                method: "POST",
                params: {
                    C: "AVEVA.CDMS.WebApi.ProjectController", A: "GetMenuRight",
                    Menu: menuType, ProjectKeyword: parentKeyword,
                    sid: localStorage.getItem("sid")
                },
                success: function (response, options) {
                    var res = Ext.JSON.decode(response.responseText);
                    var state = res.success;
                    if (state === false) {
                        var errmsg = res.msg;
                        Ext.Msg.alert("错误信息", errmsg);
                    }
                    else {
                        //弹出操作窗口
                        var _fmCreateNewProject = Ext.create('Ext.ux.YDForm.Project._ModiProjAttr', { title: "" });

                        var title = '新建文件夹';
                        if (bCreateRoot === "true") { title = '新建根文件夹'; }

                        win = Ext.widget('window', {
                            title: title,
                            closeAction: 'hide',
                            width: 500,
                            height: 580,
                            minWidth: 500,
                            minHeight: 580,
                            layout: 'fit',
                            resizable: true,
                            modal: true,
                            closeAction: 'close', //close 关闭  hide  隐藏  
                            items: _fmCreateNewProject,
                            defaultFocus: 'firstName'
                        });

                        win.show();
                        //监听子窗口关闭事件
                        win.on('close', function () {

                        });

                        _fmCreateNewProject.mainPanelId = me.id;
                        _fmCreateNewProject.winAction = "CreateProject";
                        _fmCreateNewProject.projectNameText.setValue("新建文件夹");

                        if (bCreateRoot === "true") { _fmCreateNewProject.isRootCheckBox.setValue(true); }

                        _fmCreateNewProject.get_Project_attr(parentKeyword);
                        //_fmCreateNewProject.get_Project_attr("");

                    }
                },
                failure: function (response, options) {
                    ////Ext.Msg.alert("错误", "连接服务器失败！<br>" + response.responseText);
                }
            })
        } else {
            Ext.Msg.alert("错误信息", "请选择文件夹！");
        };
    },

    //新建个人工作台
    CreateUserCustom: function (bCreateRoot) {
        var me = this;
        
        var nodes = me.mainprojecttree.getSelectionModel().getSelection();//获取已选取的节点
        if ((nodes !== null && nodes.length > 0) || bCreateRoot === "true") {
            var parentKeyword = "";

            if (nodes !== null && nodes.length > 0) parentKeyword = nodes[0].data.Keyword;

            var menuType = "NewUserCustom";
            if (bCreateRoot === "true") menuType = "NewUserCustom";


            //弹出操作窗口
            var _fmNewglobaloruserProject = Ext.create('Ext.ux.YDForm.Project._NewglobaloruserProject', { title: "" });

            var title = '新建个人工作台';
            if (bCreateRoot === "true") { title = '新建个人工作台'; }

            win = Ext.widget('window', {
                title: title,
                closeAction: 'hide',
                width: 438,
                height: 230,
                minWidth: 438,
                minHeight: 230,
                layout: 'fit',
                resizable: true,
                modal: true,
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: _fmNewglobaloruserProject,
                defaultFocus: 'firstName'
            });

            win.show();
            //监听子窗口关闭事件
            win.on('close', function () {

            });

            _fmNewglobaloruserProject.mainPanelId = me.id;
            _fmNewglobaloruserProject.winAction = "CreateProject";
            _fmNewglobaloruserProject.isCreatRoot = true;
            _fmNewglobaloruserProject.projectType = EnProjectType.UserCustom;


        } else {
            Ext.Msg.alert("错误信息", "请选择文件夹！");
        };

    },

    //新建逻辑目录
    CreateGlobCustom: function (bCreateRoot) {
        var me = this;

        var nodes = me.mainprojecttree.getSelectionModel().getSelection();//获取已选取的节点
        if ((nodes !== null && nodes.length > 0) || bCreateRoot === "true") {
            var parentKeyword = "";

            if (nodes !== null && nodes.length > 0) parentKeyword = nodes[0].data.Keyword;

            var menuType = "NewGlobCustom";
            if (bCreateRoot === "true") menuType = "NewGlobCustom";


            //弹出操作窗口
            var _fmNewglobaloruserProject = Ext.create('Ext.ux.YDForm.Project._NewglobaloruserProject', { title: "" });

            var title = '新建逻辑目录';

            win = Ext.widget('window', {
                title: title,
                closeAction: 'hide',
                width: 438,
                height: 230,
                minWidth: 438,
                minHeight: 230,
                layout: 'fit',
                resizable: true,
                modal: true,
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: _fmNewglobaloruserProject,
                defaultFocus: 'firstName'
            });

            win.show();
            //监听子窗口关闭事件
            win.on('close', function () {

            });

            _fmNewglobaloruserProject.mainPanelId = me.id;
            _fmNewglobaloruserProject.winAction = "CreateProject";
            
            _fmNewglobaloruserProject.projectType = EnProjectType.GlobCustom;
            if (bCreateRoot === "true") {
                _fmNewglobaloruserProject.isCreatRoot = true;
            } else {
                _fmNewglobaloruserProject.parentKeyword = parentKeyword;
            }


        } else {
            Ext.Msg.alert("错误信息", "请选择文件夹！");
        };

    },

    //新建查询
    CreateGlobSearch: function (bCreateRoot) {
        var me = this;

        var nodes = me.mainprojecttree.getSelectionModel().getSelection();//获取已选取的节点
        if ((nodes !== null && nodes.length > 0) || bCreateRoot === "true") {
            var parentKeyword = "";

            if (nodes !== null && nodes.length > 0) parentKeyword = nodes[0].data.Keyword;

            var menuType = "NewGlobCustom";
            if (bCreateRoot === "true") menuType = "NewGlobCustom";


            //弹出操作窗口
            var _fmNewglobaloruserProject = Ext.create('Ext.ux.YDForm.Project._NewglobaloruserProject', { title: "" });

            var title = '新建查询';

            win = Ext.widget('window', {
                title: title,
                closeAction: 'hide',
                width: 438,
                height: 480,
                minWidth: 438,
                minHeight: 480,
                layout: 'fit',
                resizable: true,
                modal: true,
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: _fmNewglobaloruserProject,
                defaultFocus: 'firstName'
            });

            win.show();
            //监听子窗口关闭事件
            win.on('close', function () {

            });

            _fmNewglobaloruserProject.mainPanelId = me.id;
            _fmNewglobaloruserProject.winAction = "CreateProject";

            _fmNewglobaloruserProject.projectType = EnProjectType.GlobSearch;
            if (bCreateRoot === "true") {
                _fmNewglobaloruserProject.isCreatRoot = true;
            } else {
                _fmNewglobaloruserProject.parentKeyword = parentKeyword;
            }
            _fmNewglobaloruserProject.SQLText.show();

        } else {
            Ext.Msg.alert("错误信息", "请选择文件夹！");
        };

    },

    //编辑查询条件
    MdsqlGlobaloruserProject: function (bCreateRoot) {
  var me = this;

        var nodes = me.mainprojecttree.getSelectionModel().getSelection();//获取已选取的节点
        if ((nodes !== null && nodes.length > 0) || bCreateRoot === "true") {
            var parentKeyword = "";

            if (nodes !== null && nodes.length > 0) parentKeyword = nodes[0].data.Keyword;

            //var menuType = "NewGlobCustom";
            //if (bCreateRoot === "true") menuType = "NewGlobCustom";


            //弹出操作窗口
            var _fmNewglobaloruserProject = Ext.create('Ext.ux.YDForm.Project._NewglobaloruserProject', { title: "" });

            var title = '编辑查询条件';

            win = Ext.widget('window', {
                title: title,
                closeAction: 'hide',
                width: 438,
                height: 480,
                minWidth: 438,
                minHeight: 480,
                layout: 'fit',
                resizable: true,
                modal: true,
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: _fmNewglobaloruserProject,
                defaultFocus: 'firstName'
            });

            win.show();
            //监听子窗口关闭事件
            win.on('close', function () {

            });

            _fmNewglobaloruserProject.mainPanelId = me.id;
            _fmNewglobaloruserProject.winAction = "ModiAttr";

            _fmNewglobaloruserProject.projectType = EnProjectType.GlobSearch;
            if (bCreateRoot === "true") {
                _fmNewglobaloruserProject.isCreatRoot = true;
            } else {
                _fmNewglobaloruserProject.parentKeyword = parentKeyword;
            }
            _fmNewglobaloruserProject.SQLText.show();

            _fmNewglobaloruserProject.projectKeyword = parentKeyword;

            _fmNewglobaloruserProject.GetGlobaloruserProjectDefault(function () {
            });
        } else {
            Ext.Msg.alert("错误信息", "请选择文件夹！");
        };
    },

    //复制创建目录
    CreateProjByDef: function () {
        var me = this;
        var nodes = me.mainprojecttree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {
            var parentKeyword = nodes[0].data.Keyword;
            //请求判断是否有创建目录权限
            Ext.Ajax.request({
                url: 'WebApi/Post',
                method: "POST",
                params: {
                    C: "AVEVA.CDMS.WebApi.ProjectController", A: "GetMenuRight",
                    Menu: "CreateNewProject", ProjectKeyword: parentKeyword,
                    sid: localStorage.getItem("sid")
                },
                success: function (response, options) {
                    var res = Ext.JSON.decode(response.responseText);
                    var state = res.success;
                    if (state === false) {
                        var errmsg = res.msg;
                        Ext.Msg.alert("错误信息", errmsg);
                    }
                    else {
                        //弹出操作窗口
                        var _fmCreateChildProjectByTempDefn = Ext.create('Ext.plug_ins.SysPlugins._CreateProjByDef', { title: "" });

                        winCreateProjByDef = Ext.widget('window', {
                            title: '复制创建目录',
                            closeAction: 'hide',
                            width: 800,
                            height: 596,
                            minWidth: 300,
                            minHeight: 300,
                            layout: 'fit',
                            resizable: true,
                            modal: true,
                            closeAction: 'close', //close 关闭  hide  隐藏  
                            items: _fmCreateChildProjectByTempDefn,
                            defaultFocus: 'firstName'
                        });


                        winCreateProjByDef.show();
                        //监听子窗口关闭事件
                        winCreateProjByDef.on('close', function () {

                        });

                        _fmCreateChildProjectByTempDefn.mainPanelId = me.id;
                        _fmCreateChildProjectByTempDefn.targetLabel.setText("当前选中：" + nodes[0].data.text);
                        _fmCreateChildProjectByTempDefn.insertTargetStore(nodes[0].data.Keyword, nodes[0].data.text);
                    }
                },
                failure: function (response, options) {
                    ////Ext.Msg.alert("错误", "连接服务器失败！<br>" + response.responseText);
                }
            })
        } else {
            Ext.Msg.alert("错误信息", "请选择文件夹！");
        }
    },

    //编辑目录属性
    ModiProjAttr: function () {
        var me = this;
        var nodes = me.mainprojecttree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {
            var parentKeyword = nodes[0].data.Keyword;
            ////请求判断是否有创建目录权限
            //Ext.Ajax.request({
            //    url: 'WebApi/Post',
            //    method: "POST",
            //    params: {
            //        C: "AVEVA.CDMS.WebApi.ProjectController", A: "GetMenuRight",
            //        Menu: "ModiProjAttr", ProjectKeyword: parentKeyword,
            //        sid: localStorage.getItem("sid")
            //    },
            //    success: function (response, options) {
            //        var res = Ext.JSON.decode(response.responseText);
            //        var state = res.success;
            //        if (state === false) {
            //            var errmsg = res.msg;
            //            Ext.Msg.alert("错误信息", errmsg);
            //        }
            //        else {
            //弹出操作窗口

            var _fmModiProjAttr = Ext.create('Ext.ux.YDForm.Project._ModiProjAttr', { title: "" });
            win = Ext.widget('window', {
                title: '目录属性',
                closeAction: 'hide',
                width: 500,
                height: 590,
                minWidth: 500,
                minHeight: 590,
                layout: 'fit',
                resizable: true,
                modal: true,
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: _fmModiProjAttr,
                defaultFocus: 'firstName'
            });


            win.show();
            //监听子窗口关闭事件
            win.on('close', function () {

            });

            _fmModiProjAttr.mainPanelId = me.id;
            _fmModiProjAttr.winAction = "ModiProject";
            var parentKeyword = nodes[0].data.Keyword;
            _fmModiProjAttr.get_Project_attr(parentKeyword);

            //        }
            //    }
            //})
        } else {
            Ext.Msg.alert("错误信息", "请选择文件夹！");
        }
    },

    //保存最后访问的目录
    saveLastProject: function (callbackFunc) {
        //tree = Ext.getCmp('contentPanel').down('_mainProjectTree').down('treepanel');
        var me = this;
        rs = me.mainprojecttree.getSelectionModel().getSelection();
        if (rs !== null && rs.length > 0) {
            var projectId = rs[0].data.Keyword;

            Ext.Ajax.request({
                url: 'WebApi/Post',
                method: "POST",
                params: {
                    C: "AVEVA.CDMS.WebApi.ProjectController", A: "SaveLastProject",
                    ProjectKeyword: projectId, sid: localStorage.getItem("sid")
                },
                success: function (response, options) {
                    callbackFunc();
                },
                failure: function (response, options) {
                    ////Ext.Msg.alert("错误", "连接服务器失败！<br>" + response.responseText);
                }
            });
        }

    },

    //搜索目录
    SearchProject: function () {
        var me = this;
        var nodes = me.mainprojecttree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {

            var _fmSearchProject = Ext.create('Ext.ux.YDForm._SearchForm', { title: "" });
            _winSearchProject = Ext.widget('window', {
                title: '查找',
                closeAction: 'hide',
                width: 390,
                height: 200,
                minWidth: 390,
                minHeight: 200,
                layout: 'fit',
                resizable: true,
                modal: true,
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: _fmSearchProject,
                defaultFocus: 'firstName'
            });


            _winSearchProject.show();
            //监听子窗口关闭事件
            _winSearchProject.on('close', function () {

            });

            var viewTree = me.down('treepanel');
            _fmSearchProject.mainPanelId = viewTree.id;

        }
    },

    //添加到收藏夹
    AddFavorites: function () {
        var me = this;

        var nodes = me.mainprojecttree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {
            var projkeyword = nodes[0].get("Keyword");

            //Ext.Msg.alert("错误信息", "添加到收藏夹" + projkeyword);

            Ext.Ajax.request({
                url: 'WebApi/Post',
                method: "POST",
                params: {
                    C: "AVEVA.CDMS.WebApi.UserController", A: "AddFavorites",
                    ProjectKeyword: projkeyword, sid: localStorage.getItem("sid")
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
                        var State = recod.state;
                        if (State === "addSuccess") {

                        }
                    }
                },
                failure: function (response, options) {
                    ////Ext.Msg.alert("错误", "连接服务器失败！<br>" + response.responseText);
                }
            });
        }
    },

    //复制目录树
    CopyProject: function (sureDel) {
        var me = this;

        window.parent.clipboardObjectList = "";

        var nodes = me.mainprojecttree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {
            var projlist = "";
            for (var i = 0; i < nodes.length ; i++) {
                var projkeyword = nodes[i].get("Keyword");
                if (i === 0) {
                    projlist = projkeyword;
                }
                else {
                    projlist += "," + projkeyword;
                }
            }
            window.parent.clipboardObjectList = projlist;
        }
        window.parent.iscut = false;
    },

    //粘贴到目录树
    ProjectPaste: function () {
        var me = this;
        //Ext.Msg.alert("消息", "粘贴到目录！" + window.parent.clipboardObjectList);

        if (window.parent.clipboardObjectList != undefined && window.parent.clipboardObjectList != "") {
            var nodes = me.mainprojecttree.getSelectionModel().getSelection();//获取已选取的节点
            if (nodes !== null && nodes.length > 0) {
                var node = nodes[0];
                var projectId = nodes[0].data.Keyword;
                var arrayobj = window.parent.clipboardObjectList.split(",");
                me.sendProjectPaste(projectId, arrayobj, 0);



            };
        }
    },

    //循环把目录或文档粘贴到目标目录
    sendProjectPaste: function (projectId, arrayobj, objIndex) {
        var me = this;

        var objectkeyword = arrayobj[objIndex];
        Ext.Ajax.request({
            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.WebApi.ProjectController", A: "ProjectPaste",
                ProjectKeyword: projectId,
                //ObjectList: window.parent.clipboardObjectList,
                ObjectKeyword: objectkeyword,
                isCut: window.parent.iscut,
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
                    if (arrayobj.length > objIndex + 1) {
                        me.sendProjectPaste(projectId, arrayobj, objIndex + 1);
                    } else {
                        me.RefreshProjTree(projectId);
                    }
                }
            }
        });
    },

    //粘贴到目录树
    ProjectPasteShortcut: function () {
        var me = this;
        //Ext.Msg.alert("消息", "粘贴快捷方式到目录！" + window.parent.clipboardObjectList);

        if (window.parent.clipboardObjectList != undefined && window.parent.clipboardObjectList != "") {
            var nodes = me.mainprojecttree.getSelectionModel().getSelection();//获取已选取的节点
            if (nodes !== null && nodes.length > 0) {
                var node = nodes[0];
                var projectId = nodes[0].data.Keyword;
                var arrayobj = window.parent.clipboardObjectList.split(",");
                me.sendProjectPasteShortcut(projectId, arrayobj, 0);



            };
        }
    },

    //循环把目录或文档粘贴到目标目录
    sendProjectPasteShortcut: function (projectId, arrayobj, objIndex) {
        var me = this;

        var objectkeyword = arrayobj[objIndex];
        Ext.Ajax.request({
            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.WebApi.ProjectController", A: "ProjectPasteShortcut",
                ProjectKeyword: projectId,
                //ObjectList: window.parent.clipboardObjectList,
                ObjectKeyword: objectkeyword,
                isCut: window.parent.iscut,
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
                    if (arrayobj.length > objIndex + 1) {
                        me.sendProjectPasteShortcut(projectId, arrayobj, objIndex + 1);
                    } else {
                        me.RefreshProjTree(projectId);
                    }
                }
            }
        });
    },

    //删除目录树
    DelProject: function (sureDel) {
        var me = this;
        var nodes = me.mainprojecttree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {
            var node = nodes[0];
            var projectId = nodes[0].data.Keyword;

            Ext.Ajax.request({
                url: 'WebApi/Post',
                method: "POST",
                params: {
                    C: "AVEVA.CDMS.WebApi.ProjectController", A: "DelProject",
                    ProjectKeyword: projectId, sureDel: sureDel,
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
                        if (recod === undefined)
                        {
                            var nonodeId = node.data.id;
                            me.RefreshProjTree(nonodeId);
                            return;
                        }

                        var State = recod.state;
                        if (State === "delSuccess") {//
                            var nonode = node.parentNode;
                            var nonodeId = nonode.data.id;
                            me.RefreshProjTree(nonodeId);
                            //Ext.Msg.alert("删除成功", "删除成功" + nonode.data.id + "," + nonode.data.text);
                        } else if (State === "seleSureDel") {
                            //选择是否确定删除
                            Ext.MessageBox.show({
                                title: '确认删除',
                                msg: res.msg,
                                buttons: Ext.MessageBox.YESNO,
                                buttonText: {
                                    yes: "是",
                                    no: "否"
                                },
                                fn: function (btn) {
                                    if (btn === "yes") {
                                        me.DelProject("true");
                                    }
                                    else {
                                    }
                                }
                            });
                        }
                    }
                },
                failure: function (response, options) {
                    ////Ext.Msg.alert("错误", "连接服务器失败！<br>" + response.responseText);
                }
            });
        }
    },

    //刷新目录树
    RefreshProjTree: function (setLastProject) {
        var me = this;
        me.saveLastProject(function () {

            Ext.Ajax.request({
                url: 'WebApi/Post',
                method: "POST",
                params: {
                    C: "AVEVA.CDMS.WebApi.DBSourceController", A: "refreshDBSource",
                    sid: localStorage.getItem("sid")
                },
                success: function (response, options) {

                    me._ProjectsStore.load({
                        callback: function (records, options, success) {
                            me.ExpendProject(setLastProject);
                        }
                    });
                },
                failure: function (response, options) {
                    ////Ext.Msg.alert("错误", "连接服务器失败！<br>" + response.responseText);
                }
            });
        });
    },

    //添加鼠标悬停提示
    //me.onMainProjectTreeItemMouseEnter(dataview, record, item, index, e, eOpts);
    onMainProjectTreeItemMouseEnter: function (dataview, record, item, index, e, eOpts) {
        var me = this;

            dataview.tip = Ext.create('Ext.tip.ToolTip', {
                target: item,
                delegate: dataview.itemSelector,
                trackMouse: true,
                renderTo: Ext.getBody(),
                autoDestroy: true,
                listeners: {
                    beforeshow: function (tip) {
                        tip.update(record.data.text);
                    },
                    beforehide:function ( tip, eOpts ) {
                        tip.destroy();
                    }
                }
            });
        return true;
    },

    //启动目录的流程菜单
    _StartProjFlow: function () {
        var me = this;

        var rs = me.mainprojecttree.getSelectionModel().getSelection();//获取选择的文档
        if (rs) {
            //获取选取目录关键字
            var projectlist = "";
            projectlist = rs[0].data.Keyword;

            window.activeTreePanelId = me.mainprojecttree.id;

            //启动流程
            Ext.require('Ext.ux.Common.comm', function () {
                //参数：doclist,wfKeyword,userlist,callback_fun
                StartNewWorkFlow(projectlist, "", "", function (res, WorkFlowKeyword, CuWorkStateCode) {
                    me.refreshWin(projectlist, false);
                });
            })
        }

    },

    //刷新表单，参数:parentKeyword:需要定位到的目录目录
    refreshWin: function (projKeyword, funCallback) {
        var me = this;
        var tree = Ext.getCmp(window.activeTreePanelId);//.up('_mainSourceView').down('_mainProjectTree').down('treepanel');//;
        var viewTreeStore = tree.store;

        viewTreeStore.load({
            callback: function (records, options, success) {//添加回调，获取子目录的文件数量

                //展开目录
                Ext.require('Ext.ux.Common.comm', function () {
                    Ext.getCmp('contentPanel').down('_mainProjectTree').ExpendProject(projKeyword);
                });
                funCallback();
            }
        });
    },

    //显示右键菜单方法
    _showContextMenu: function (view, record, item, index, e, eOpts) {


        //显示右键菜单
        var menus = Ext.widget('_contextmenu');

        menus.showMainPanelMenu(view, e, false, 0);

    },

    //容器空白处的右键菜单
    _showContainerContextMenu: function (view, e, eOpts) {
        var me = this;

        //显示右键菜单
        var menus = Ext.widget('_contextmenu');



        var tvPosition = 0;

        //普通文档
        if (view.up("contentview") != undefined)
        {
            tvPosition = 1;
        }
        //逻辑目录
        else if (view.up("_logicprojectview") != undefined) {
            tvPosition = 2;
        }
        //个人工作台
        else if (view.up("_workplaceview") != undefined) {
            tvPosition = 3;
        }
        //全局查询
        else if (view.up("_queryview") != undefined) {
            tvPosition = 4;
        }

        //alert("目录类型!" + tvPosition);

        menus.showMainPanelMenu(view, e, true, tvPosition);

    },
});

//外部JS调用Extjs函数
Ext.RefreshProjectTree = function (treePanelId,projectKeyword) {
    //alert("接收到刷新目录树消息！请手动刷新目录树!!");
    //var me = this;
    //me.RefreshProjTree("LastProject");
    var view = Ext.getCmp(treePanelId);
    view.RefreshProjTree(projectKeyword);
    //view.RefreshProjTree("LastProject");
}