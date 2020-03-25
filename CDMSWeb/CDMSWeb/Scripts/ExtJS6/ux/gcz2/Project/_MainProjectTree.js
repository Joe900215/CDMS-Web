/*定义目录树*/
Ext.define('Ext.ux.gcz.Project._MainProjectTree', {
    //extend: 'Ext.container.Container',
    extend: 'Ext.panel.Panel',
    alias: 'widget._mainProjectTree', // 此类的xtype类型为buttontransparent  
    title: "项目", //region: "west",
    collapsible: true, rootVisible: false,
     split: true,
     layout: 'fit',
     width: '100%',
    flex:1,
    initComponent: function () {
        var me = this;
        me.renderTo = me.el;
        
        //展开目录树时，如果ExpendDocKeyword不为""，就转到ExpendDocKeyword所在的文档，否则就跳转到目录的第一个文档记录
        me.ExpendDocKeyword = "";

        me.title = "项目   " + " ("+localStorage.getItem("username")+")";
        me.sourceViewType = window.SourceViewType; 

        //定义project属性store
        me._ProjectsStore = Ext.create("Ext.data.TreeStore", {
            batchActions: false,
            remoteFilter: false,
            remoteSort: false,
            model: "CDMSWap.model.ProjectTree",
            //root: { id: "Root", text: "根目录"}, //, expanded: true },

            //代理定义
            proxy: {
                type: 'ajax',
                //代理的API定义了操作的提交路径
                //路径：\CDMSWap\Controllers\FileController.cs
                api: {
                    read: 'WebApi/Get?C=AVEVA.CDMS.WebApi.ProjectController&A=GetProjectListJson&ProjectType='+me.sourceViewType + '&sid=' + localStorage.getItem("sid"),//调用路径：\simplecdms\controllers\projectcontroller.cs
                    update: 'WebApi/Post?C=AVEVA.CDMS.WebApi.ProjectController&A=SaveProject',
                },

                //在代理定义中，reader和writer的定义可标准化数据的输入输出，
                //这个与用户中的定义是一样的
                reader: {
                    messageProperty: "Msg",
                    type: 'json',
                    root: "data"
                },
                writer: {
                    type: "json",
                    encode: true,
                    root: "data",
                    allowSingle: false
                },
                listeners: {
                    exception: CDMSWap.ProxyException
                }
            }
        });

        //定义目录树右侧菜单按钮
        me._projectMenu = Ext.create('Ext.menu.Menu', {
            items: [{
                text: "输入意见",
                listeners: {
                    "click": function (btn, e, eOpts) {//添加点击按钮事件
                        //me.showModiAuditDetail();
                       // me.onAddAuditMenuItemClick();
                    }
                }
            }, "-"]
        });

        //定义目录树按钮
        me._projectTreeTbar = Ext.create('Ext.toolbar.Toolbar', {
            xtype: 'toolbar',
            dock: 'top',
            items: [
                //{
                //    iconCls: "project-create", scope: me, tooltip: '项目立项', listeners: {
                //        "click": function (btn, e, eOpts) {
                //            me.CreateRootProject();
                //        }
                //    }
                //},
                //{ iconCls: "folder-add", scope: me, tooltip: '创建目录' , listeners: {
                //    "click": function (btn, e, eOpts) {
                //        me.CreateNewProject();
                //    }
                //}},
                //{
                //    iconCls: "file-batchcreate", scope: me, tooltip: '复制创建目录', listeners: {
                //        "click": function (btn, e, eOpts) {
                //            me.CreateProjByDef();
                //        }
                //    }
                //},
                //{
                //    iconCls: "folder-edit", scope: me, tooltip: '编辑目录属性', listeners: {
                //        "click": function (btn, e, eOpts) {
                //            me.ModiProjAttr();
                //        }
                //    }
                //},//, disabled: true }
                //{
                //    iconCls: "folder-delete", scope: me, tooltip: '删除目录', listeners: {
                //        "click": function (btn, e, eOpts) {
                //            me.DelProject("false");
                //        }
                //    }
                //},
                //{
                //    iconCls: "group-edit", scope: me, tooltip: '设置用户组', listeners: {
                //        "click": function (btn, e, eOpts) {
                //            me.AdminSetting();
                //        }
                //    }
                //},
                //{
                //    iconCls: "refresh", scope: me, tooltip: '刷新', listeners: {
                //        "click": function (btn, e, eOpts) {
                //            me.RefreshProjTree("LastProject");
                //        }
                //    }
                //},
                "->",
                {
                    iconCls: "ellipsis", scope: me, tooltip: '显示菜单',
                    //menu: me._projectMenu,
                    //enableToggle: false,
                    listeners: {
                        "click": function (view, e, eOpts) {
                            //me.RefreshProjTree("LastProject");
                            me._showContextMenu(view, e, eOpts);
                        }
                        //,
                        //"touchend": function (e) {
                        //    //me.RefreshProjTree("LastProject");
                        //    me._showContextMenu(e);
                        //}
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
            root: { id: "Root", text: "根目录"}, //, expanded: true },
            viewConfig: {
                stripeRows: true
            },
            listeners: {
                //"selectionchange": function (model, sels) {//处理点击选择目录节点后事件

                //},
                "select":function( model, record, index, eOpts ){
                    me.onTreeItemSelect(model, record);
                },
                "afteritemexpand": function (node, index, item, eOpts) {//处理目录树展开后的事件(添加各个目录下文档数量)
                    me.afterTreeExpand(node, index, item, eOpts);
                }
                //,

                //"itemcontextmenu": function (view, rec, node, index, e) {//添加鼠标右键菜单事件
                //   // me._showContextMenu(view, rec, node, index, e);
                //},
                //"afterrender": function () {//完成渲染后的事件

                //  //  me.ExpendProject("LastProject");
                //},
                //"itemmouseenter": function (dataview, record, item, index, e, eOpts) {
                //   // me.onMainProjectTreeItemMouseEnter(dataview, record, item, index, e, eOpts);
                //}
            }

        });
        //添加属性TAB页面到容器
        me.items = [
            			//{
            			//    xtype:"button",
            			//    text:"我的按钮"
            			//}
                               me.mainprojecttree
        ];

       // window.activeTreePanelId = me.mainprojecttree.id;

        me.callParent(arguments);
    },

    setSourceViewType: function (projectType) {
        var me = this;
        me.sourceViewType=projectType;
    },

    onTreeItemSelect: function (model, record) {
        var me = this;
        var Keyword = record.data.Keyword;
        var text = record.data.Desc;

        //if (sels.length > 0) {

        Keyword = record.data.Keyword;

        //刷新文档列表
        var docgrid = me.up('_mainProjectView').down('_mainDocGrid');
        docgrid.loadDocListStore(function () { me.afterLoadDocListStore(); });

        //展开目录树时，如果ExpendDocKeyword为""，就显示目录属性页
        if (me.ExpendDocKeyword === "") {
            //显示目录属性页
            me.setAttrPage(Keyword);
        }

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
                    var docgrid = me.up('_mainProjectView').down('_mainDocGrid');
                    docgrid.addressField.setValue(recod.path);
                }
            }
        });


        //判断浏览器的类型，如果使用cefSharp浏览，通知cefSharp选中了文件夹
        if (_browserName == "cefSharp") {
            jsObj.onProjectTreeItemSelect(localStorage.getItem("sid"), Keyword, me.id);
        }

    },

    //加载文档列表完成后的事件
    afterLoadDocListStore: function () {
        var me = this;

        //展开目录树时，如果ExpendDocKeyword不为""，就转到ExpendDocKeyword所在的文档
        if (me.ExpendDocKeyword != "") {
            Keyword = me.ExpendDocKeyword;
            //复原ExpendDocKeyword
            me.ExpendDocKeyword = "";

            var gridItem;

            var docgrid = me.up('_mainProjectView').down('_mainDocGrid');

            for (var i = 0; i < docgrid.maindocgrid.store.getCount() ; i++) {
                var gridItemData = docgrid.maindocgrid.store.getAt(i); //遍历每一行
                if (gridItemData.data.Keyword === Keyword)
                {
                    gridItem = gridItemData;
                    break;
                }
            }

            if (gridItem != undefined) {
                docgrid.maindocgrid.getSelectionModel().select(gridItem, true);
            }

        }
       
    },

    //加载某个目录节点
    setAttrPage: function (Keyword) {
        var me = this;

        ////获取类别为_mainAttrTab的控件
        //var tabPlan = me.up('_mainProjectView').down('_mainAttrTab');
        //var tab = tabPlan.mainattrtab.activeTab;
        //tabPlan.seleObjType = "Project";

        //var tabType = "";
        //if (tab.title === "目录属性") {

        //    tabType = "projectAttr";
        //    tabPlan.loadDocAttrPage(tabType, Keyword);

        //} else if (tab.title === "目录_附加属性") {

        //    tabType = "projectAttrData";
        //    tabPlan.loadDocAttrPage(tabType, Keyword);

        //} else {//如果活动属性页是是“目录属性”，就转到“文档属性”页

        //    tabType = "projectAttr";

        //    //tabPlan.setAttrTabDisplay("Project", Keyword);

        //    //Tab页Change事件锁，防止目录和文档之间互相切换时，重复执行Tab的Change事件
        //    tabPlan.TabChangeLock = true;

        //    tabPlan.loadDocAttrPage(tabType, Keyword);
        //}

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
                    }
                });

                return false;　　//不返回　false，run()　方法会被永无止境的调用
            },
            args: [node, item, eOpts],  //添加args传入参数后，必须在run里面添加 return false;否则run()　方法会被永无止境的调用
            interval: 1000,　//一秒执行一次，本例中　run()　只在　1　秒后调用一次
            repeat: 1　　　　　　　//重复执行　2　次,　这个参数已不再启作用了
        });
    },

    //展开指定Project,
    ExpendProject: function (ProjectId) {
        //var viewTree = Ext.getCmp('_projectsTree');
        var me = this;
        //var viewTree = me.view.up('_mainSourceView').down('_mainProjectTree');
        var viewTree = me.mainprojecttree;
        var viewTreeStore = viewTree.store;
        Ext.Ajax.request({
            //url: 'Project/GetProjectPath',
            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.WebApi.ProjectController", A: "GetProjectPath",
                Keyword: ProjectId, sid: localStorage.getItem("sid")
            },
            success: function (response, options) {
                var res = Ext.JSON.decode(response.responseText);
                var state = res.success;
                if (state === true) {
                    var record = res.data[0];

                    //判断是否当前已经选中所要跳转的节点
                    var nodes = viewTree.getSelectionModel().getSelection();//获取已选取的节点
                    if (nodes !== null && nodes.length > 0 && nodes[0].data.Keyword === record.NodeId && record.DocKeyword!="") {

                        //设置跳转到目录的某个文档
                        me.ExpendDocKeyword = record.DocKeyword;

                        //这里需要刷新文档列表，否则直接 me.afterLoadDocListStore();之前选中的文档不会取消。
                        var docgrid = me.up('_mainProjectView').down('_mainDocGrid');
                        docgrid.loadDocListStore(function () { me.afterLoadDocListStore(); });

                        return;
                    }


                    //viewTree.collapseAll();//收缩所有子节点
                    viewTree.expandPath(record.ProjectPath);
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

                            viewTree.getSelectionModel().select(node,true);
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
    },

    //项目立项
    CreateRootProject: function () {
        var me = this;

        //弹出操作窗口
        var _fmCreateRootProject = Ext.create('Ext.plug_ins.HXPC_Plugins.CreateRootProject', { title: "", mainPanelId: me.mainprojecttree.id });

        winCreateRootProject = Ext.widget('window', {
            title: '项目立项',
            closeAction: 'hide',
            width: 765,
            height: 571,
            minWidth: 765,
            minHeight: 571,
            layout: 'fit',
            resizable: true,
            modal: true,
            closeAction: 'close', //close 关闭  hide  隐藏  
            items: _fmCreateRootProject,
            defaultFocus: 'firstName'
        });

        winCreateRootProject.show();
        //监听子窗口关闭事件
        winCreateRootProject.on('close', function () {

        });


    },

    ////项目立项
    //AdminSetting: function () {
    //    var me = this;

    //    //弹出操作窗口
    //    var _fmAdminSetting = Ext.create('Ext.ux.YDForm.Admin._GroupManagement', { title: "", mainPanelId: me.mainprojecttree.id });

    //    winAdminSetting = Ext.widget('window', {
    //        title: '系统设置',
    //        closeAction: 'hide',
    //        width: 800,
    //        height: 596,
    //        minWidth: 300,
    //        minHeight: 300,
    //        layout: 'fit',
    //        resizable: true,
    //        modal: true,
    //        closeAction: 'close', //close 关闭  hide  隐藏  
    //        items: _fmAdminSetting,
    //        defaultFocus: 'firstName'
    //    });

    //    winAdminSetting.show();
    //    //监听子窗口关闭事件
    //    winAdminSetting.on('close', function () {

    //    });
    //},

    //新建目录
    CreateNewProject: function () {
        var me = this;
        //me.viewTree = Ext.getCmp(mainPanelId).down('treepanel');//获取目录树控件ID
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
                        var _fmCreateNewProject = Ext.create('Ext.ux.YDForm.Project._ModiProjAttr', { title: "" });

                        win = Ext.widget('window', {
                            title: '新建文件夹',
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

                        _fmCreateNewProject.get_Project_attr(parentKeyword);
                        //_fmCreateNewProject.get_Project_attr("");

                    }
                }
            })
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
    saveLastProject:function(callbackFunc){
        //tree = Ext.getCmp('contentPanel').down('_mainProjectTree').down('treepanel');
        var me = this;
        rs = me.mainprojecttree.getSelectionModel().getSelection();
        if (rs !== null && rs.length > 0) {
            var projectId = rs[0].data.id;

            Ext.Ajax.request({
                url: 'WebApi/Post',
                method: "POST",
                params: {
                    C: "AVEVA.CDMS.WebApi.ProjectController", A: "SaveLastProject",
                    ProjectKeyword: projectId, sid: localStorage.getItem("sid")
                },
                success: function (response, options) {
                    callbackFunc();
                }
            });
        }

    },

    //SearchProject: function () {
    //    var me = this;
    //    var nodes = me.mainprojecttree.getSelectionModel().getSelection();//获取已选取的节点
    //    if (nodes !== null && nodes.length > 0) {

    //        var _fmSearchProject = Ext.create('Ext.ux.YDForm._SearchForm', { title: "" });
    //        _winSearchProject = Ext.widget('window', {
    //            title: '查找',
    //            closeAction: 'hide',
    //            width: 390,
    //            height: 200,
    //            minWidth: 390,
    //            minHeight: 200,
    //            layout: 'fit',
    //            resizable: true,
    //            modal: true,
    //            closeAction: 'close', //close 关闭  hide  隐藏  
    //            items: _fmSearchProject,
    //            defaultFocus: 'firstName'
    //        });


    //        _winSearchProject.show();
    //        //监听子窗口关闭事件
    //        _winSearchProject.on('close', function () {

    //        });

    //        var viewTree = me.down('treepanel');
    //        _fmSearchProject.mainPanelId = viewTree.id;

    //    }
    //},

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
                    ProjectKeyword: projectId, sureDel:sureDel,
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
                }
            });
        }
    },

    //刷新目录树
    RefreshProjTree: function (setLastProject) {
        var me = this;
        me.saveLastProject(function () {
            me._ProjectsStore.load({
                callback: function (records, options, success) {
                    //if (setLastProject != null) {
                    //        me.ExpendProject("LastProject");
                    //}else
                    me.ExpendProject(setLastProject);
                    //}
                }
            });
        });

    },

    //添加鼠标悬停提示
    //me.onMainProjectTreeItemMouseEnter(dataview, record, item, index, e, eOpts);
    onMainProjectTreeItemMouseEnter: function (dataview, record, item, index, e, eOpts) {
        dataview.tip = Ext.create('Ext.tip.ToolTip', {
            target: item,
            delegate: dataview.itemSelector,
            trackMouse: true,
            renderTo: Ext.getBody(),
            listeners: {
                beforeshow: function updateTipBody(tip) {
                    //tip.update(record.get('name'));
                    tip.update(record.data.text);
                }
            }
        });
        return false;
    },
    ////启动目录的流程菜单
    //_StartProjFlow: function () {
    //    var me = this;

    //    var rs = me.mainprojecttree.getSelectionModel().getSelection();//获取选择的文档
    //    if (rs) {
    //        //获取选取目录关键字
    //        var projectlist = "";
    //        projectlist=rs[0].data.Keyword;

    //        window.activeTreePanelId = me.mainprojecttree.id;

    //        //启动流程
    //        Ext.require('Ext.ux.Common.m.comm', function () {
    //            //参数：doclist,wfKeyword,userlist,callback_fun
    //            StartNewWorkFlow(projectlist, "", "", function (res, WorkFlowKeyword, CuWorkStateCode) {
    //                me.refreshWin(projectlist,false);
    //            });
    //        })
    //    }

    //},

    //刷新表单，参数:parentKeyword:需要定位到的目录目录
    refreshWin: function (projKeyword, funCallback) {
        var me = this;
        //var tree = Ext.getCmp(window.activeTreePanelId);//.up('_mainSourceView').down('_mainProjectTree').down('treepanel');//;
        var viewTreeStore = me.mainprojecttree.store;

        viewTreeStore.load({
            callback: function (records, options, success) {//添加回调，获取子目录的文件数量

                //展开目录
                Ext.require('Ext.ux.Common.m.comm', function () {
                    Ext.getCmp('contentPanel').down('_mainProjectTree').ExpendProject(projKeyword);
                });
                funCallback();
            }
        });
    },

    //显示右键菜单方法
    _showContextMenu: function (view, e, eOpts) {
    //_showContextMenu: function (e) {
        var me = this;

        //显示右键菜单
        var menus = Ext.widget('_contextmenu');

        menus.showMainPanelMenu(me.mainprojecttree, e);

    }


});

//外部JS调用Extjs函数
Ext.RefreshProjectTree = function (treePanelId, projectKeyword) {
    //alert("接收到刷新目录树消息！请手动刷新目录树!!");
    //var me = this;
    //me.RefreshProjTree("LastProject");
    var view = Ext.getCmp(treePanelId);
    view.RefreshProjTree(projectKeyword);
    //view.RefreshProjTree("LastProject");
}