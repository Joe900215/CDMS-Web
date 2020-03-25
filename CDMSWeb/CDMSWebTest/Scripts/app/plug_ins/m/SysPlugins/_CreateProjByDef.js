//复制创建目录
Ext.define('Ext.plug_ins.SysPlugins._CreateProjByDef', {
    extend: 'Ext.container.Container',
    alias: 'widget._CreateProjByDef',
    //layout: "border",
    layout: 'fit',
    resultvalue: '',mainPanelId:'',
    initComponent: function () {
        var me = this;
        me.renderTo = me.el;
        me.CreatProjLeavel = 0;
        me.ProjLeavel = 0;
        
        var resultvalue = '';
        //me.targetProjId = 'r';
        //me.targetProjText = 'q';

        //定义已选择目录的model
        Ext.define("_SelProject", {
            extend: 'Ext.data.Model',
            fields: ["text", "id"],
            proxy: {
                type: 'ajax',
                api: {
                    read: "_blank"
                }
            }
            //url: "_blank",
        });


        //定义选择目录的sotre
        me.targetStore = Ext.create("Ext.data.TreeStore", {
            batchActions: false,
            //文章的Store需要支持远程排序和搜索
            remoteFilter: true,
            remoteSort: true,
            //每50条记录为一页
            pageSize: 50,//视图路径：\simplecdms\scripts\app\view\content\view.js
            model: "_SelProject"

        });

        //定义源目录树
        me.sourceTree = Ext.widget("treepanel", {
            title: "项目目录结构",  rootVisible: false, store: "ProjectsTree",
            //layout: 'fit',
            width:"100%",
            //anchor: '100% 100%',
			root: { id: "/", text: "根目录"},//, expanded: true },
            autoScroll: true,
            containerScroll: true, // 随自身或父容器的改变而显示或隐藏scroll
            split: true// expanded: true,

            //height: "100%",
            //checked:true,
            //checked:false,	//添加复选框
            
        });


        //定义目标目录树
        me.targetTree = Ext.widget("treepanel", {
            title: "新建目录",  rootVisible: false,
            store: me.targetStore,
            layout: 'fit',
            split: true,
            height:'100%',
            listeners: {
                'itemdblclick': function (view, record, item, index, e) {
                    if (typeof (record.raw) != 'undefined' || typeof (record.data) != 'undefined') {
                        //Ext.Msg.alert("错误信息", "错误信息");
                        //双击时，从已选择grid删除双击的节点
                        if ((typeof (record.raw) != 'undefined'  || typeof (record.data) != 'undefined') && record.data.parentId!= "/") {
                            ////删除行
                            record.remove();
                        }

                    }
                }
            }
        });


        me.targetLabel = Ext.create("Ext.form.Label", {
            xtype: "label",
            height: 100,
            margins: '12,15,12,15',
            pack: 'center',//垂直居中 
            //text: "当前选中：" + me.targetProjText,
            flex: 3
        });

        //添加列表
        me.items = [
          Ext.widget('form', {
              baseCls: 'my-panel-no-border',//隐藏边框
              layout: {
                  type: 'vbox',
                  align: 'stretch',
                  pack  : 'start'
              },
              items: [{//上部容器
                  baseCls: 'my-panel-no-border',//隐藏边框
                  layout: {
                      type: 'hbox',
                      pack: 'start', 
                      align: 'stretch'
                  },
                  items: [
                      {
                          layout: 'border',
                          height: "100%",
                          weight: "100%",
                          baseCls: 'my-panel-no-border',//隐藏边框
                          items: {//左边容器
                              xtype: "panel",
                              layout: "fit",
                              region: 'center',
                              layout: 'border', 
                              items: [me.sourceTree],
                              listeners: {//自适应treepanel高度和宽度
                                  resize: function (athis, adjWidth, adjHeight, eOpts) {
                                      // adjHeight为Panel的高度
                                      if (adjHeight > 0) {
                                          // 获取Panel下所有的treepanel
                                          var components = athis.query('treepanel');
                                          for (var i in components) {
                                              components[i].setHeight(adjHeight);
                                          }
                                      }
                                      if (adjWidth > 0) {
                                          // 获取Panel下所有的treepanel
                                          var components = athis.query('treepanel');
                                          for (var i in components) {
                                              components[i].setWidth(adjWidth);
                                          }
                                      }
                                  }
                              }
                          }, flex: 1 },
                      {
                          items: {
                              xtype: "container",//中间容器
                              autoEl: "div",
                              autoheight:true,
                              width: 110,
                              //layout: 'fit',
                              //items: [{
                              layout: {
                                  type: 'vbox',
                                  align: 'stretch',
                                  pack: 'start',
                              }, items: [{ height: 100 }, {
                                  xtype: "button",
                                  height: 100,
                                  margins: '0,5,0,5',
                                  text: "----><br/>选择目录【一个】",
                                  listeners: {
                                      "click": function (btn, e, eOpts) {//添加点击按钮事件
                                          me.insert_Select_Project("false");
                                      }
                                  }
                              }, {
                                  xtype: "button",
                                  height: 100,
                                  margins: '0,5,0,5',
                                  text: "----><br/>选择目录及<br/>同级目录",
                                  listeners: {
                                      "click": function (btn, e, eOpts) {//添加点击按钮事件
                                          //me.insert_First_Level_Project();
                                          //me.insert_Project_By_Level('1', 'true');
                                          me.insert_Select_Project("true");
                                      }
                                  }
                              }, {
                                  xtype: "button",
                                  height: 100,
                                  margins: '0,5,0,5',
                                  text: "----><br/>选择目录及子目录",
                                  //listeners: {
                                  //    "click": function (btn, e, eOpts) {//添加点击按钮事件
                                  //        me.insert_Project_By_Level('2');
                                  //    }
                                  //},
                                  arrowAlign: 'bottom',
                                  menu: [{
                                      text: '选择目录及一级子目录', handler: function () { me.insert_Project_By_Level('1', 'false'); }
                                  }, {
                                      text: '选择目录及一二级子目录', handler: function () { me.insert_Project_By_Level('2', 'false'); }
                                  }, {
                                      text: '选择目录及所有子目录', handler: function () { me.insert_Project_By_Level('4', 'false'); }//最大四级子目录
                                  }]
                              },
                              //{

                              //    xtype: "button",
                              //    height: 100,
                              //    margins: '0,5,0,5',
                              //    text: "----><br/>选择目录及子目录<br/>【所有子目录】",
                              //    listeners: {
                              //        "click": function (btn, e, eOpts) {//添加点击按钮事件
                              //            me.insert_Project_By_Level('4');//最大四级子目录
                              //        }
                              //    }
                              //}
                              ], flex: 1
                              //}]
                          }, width: 110 },
                  {
                      layout: 'border',
                      baseCls: 'my-panel-no-border',//隐藏边框
                      items: {
                          xtype: "panel",
                          region: 'center',
                          baseCls: 'my-panel-no-border',//隐藏边框
                          items: [me.targetTree],
                          listeners: {//自适应treepanel高度和宽度
                              resize: function (athis, adjWidth, adjHeight, eOpts) {
                                  // adjHeight为Panel的高度
                                  if (adjHeight > 0) {
                                      // 获取Panel下所有的treepanel
                                      var components = athis.query('treepanel');
                                      for (var i in components) {
                                          components[i].setHeight(adjHeight);
                                      }
                                  }
                                  if (adjWidth > 0) {
                                      // 获取Panel下所有的treepanel
                                      var components = athis.query('treepanel');
                                      for (var i in components) {
                                          components[i].setWidth(adjWidth);
                                      }
                                  }
                              }
                          }
                      }, flex: 1 }
                  ], flex: 1
              }, {//下部容器
                  layout: {
                      type: 'hbox',
                      pack: 'start',
                      align: 'stretch'
                  },
                  items: [
                      //{
                      //    xtype: "button",
                      //    height: 100,
                      //    margins: '12,15,12,15',
                      //    text: "↑选中[同级所有]目录",flex:2
                      //}, {
                      //    xtype: "button",
                      //    height: 100,
                      //    margins: '12,15,12,15',
                      //    text: "↑取消所有选择", flex: 2
                      //}, 
                      {fles:4},
                          me.targetLabel
                          //xtype: "label",
                          //height: 100,
                          //margins: '12,15,12,15',
                          //pack: 'center',//垂直居中 
                          ////text: "当前选中：" + me.targetProjText,
                          //flex: 3
                      , {
                          xtype: "button",
                          height: 100,
                          margins: '12,15,12,15',
                          text: "创建",
                          listeners: {
                              "click": function (btn, e, eOpts) {//添加点击按钮事件
                                  me.create_Project();
                              }
                          }, flex: 1
                      }], height: 60
              }, ]
          })

        ];

        

        me.callParent(arguments);
    },

    //向服务器传送所选的目录树数据，创建目录
    create_Project: function () {
        var me = this;
        var root = me.targetTree.getRootNode();
        var childnodes = root.childNodes;

        var jsonstr = "[]";
        var jsonarray = eval('(' + jsonstr + ')');
        //获取所有的子节点  
        function findchildnode(node) {
            var me = this;
            var childnodes = node.childNodes;
            var nd;
            for (var i = 0; i < childnodes.length; i++) {  //从节点中取出子节点依次遍历
                nd = childnodes[i];
                var arr =
                 {
                     "id": nd.data.id,
                     "text": nd.data.text,
                     "parentId": nd.data.parentId
                 }
                jsonarray.push(arr);
                if (nd.hasChildNodes()) {  //判断子节点下是否存在子节点
                    findchildnode(nd);    //如果存在子节点  递归
                }
            }
        }
        findchildnode(root);
        var jsonstr = JSON.stringify(jsonarray); //可以将json对象转换成json对符串 
        Ext.MessageBox.wait("正在创建目录，请稍候...", "等待");
        Ext.Ajax.request({ 
            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.WebApi.ProjectController", A: "CreateProjectByDef",
                sid: localStorage.getItem("sid"),
                Projects: jsonstr
            },
            disableCaching: false,
            success: function (response, options) {

                me.create_Project_callback(response, childnodes[0].data.id);
            }
        });
    },
    
    create_Project_callback: function (response, ProjectId) {
        var me = this;
        var restext = response.responseText;
        var res = Ext.JSON.decode(restext);

        var state = res.success;
        if (state === false) {
            var errmsg = res.msg;
            Ext.Msg.alert("错误信息", errmsg);
        }
        else {

            Ext.MessageBox.close();//关闭等待对话框

            var tree = Ext.getCmp(me.mainPanelId).down('treepanel');
            var viewTreeStore = tree.store;
            viewTreeStore.load({
                callback: function (records, options, success) {//添加回调，获取子目录的文件数量
                    winCreateProjByDef.close();
                    //展开目录
                    Ext.require('Ext.ux.Common.comm', function () {
                        Ext.getCmp('contentPanel').down('_mainProjectTree').ExpendProject(ProjectId);
                    })
                }
            });
        }
    },



    //},
    //插入子节点到目标目录树
    insertTargetStore: function (Keyword,Text) {
        var me = this;
        var root = me.targetTree.getRootNode();
        var parent = root.appendChild({
            id: Keyword,
            text: Text,
            iconCls: "writefolder",
            leaf: true
        });
        //parent.set('leaf', false);
    },

    ////插入一级子目录到目标目录树
    //insert_First_Level_Project: function () {
    //    var me = this;
    //    var fistLevelNode = me.insert_Select_Project();//插入第一级节点

    //    var nodes = me.sourceTree.getSelectionModel().getSelection();//获取源节点
    //    var sourceNode = nodes[0];

    //    me.insert_Child_Project(sourceNode.data.id);//插入源节点树里面所选节点的所有子节点到目标节点树


    //},

    //插入源节点树里面所选节点的所有子节点到目标节点树，参数：源节点ID
    insert_Child_Project: function (sourceNodeId) {
        var me = this;

        Ext.Ajax.request({
            //url: 'Project/GetProjectListJson?ProjectType=1',
            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.WebApi.ProjectController", A: "GetProjectListJson",
                ProjectType:1,
                node: sourceNodeId,
                sid: localStorage.getItem("sid")
            },
            success: function (response, options) {
                me.getProject_callback(response, options);

            }
        });

    },

    //响应获取子project回调
    getProject_callback: function (response, options){
        var me = this;
        var restext = response.responseText;
        var res = Ext.JSON.decode(restext);

        var state = res.success;
        if (state === false) {
            var errmsg = res.msg;
            Ext.Msg.alert("错误信息", errmsg);
        }
        else {
            var recod = res.data;
            var node = me.targetTree.store.getNodeById(options.params.node);
            me.loop_insert_project(recod,node);//循环插入project
            //var node = me.targetTree.store.getNodeById(options.params.node);
            //for (var i = 0; i < recod.length ; i++) {
            //    rec = recod[i];
            //    var reNode=me.insert_One_Project_ById(node, rec.id, rec.text);
            //    if (rec.children != null && rec.children.length > 0) {

            //    }
            //}
            //node.expand();
        }
    },


    //循环插入project
    loop_insert_project: function (recod,node) {
        
        var me=this;

        for (var i = 0; i < recod.length ; i++) {
            var rec = recod[i];
            var reNode=me.insert_One_Project_ById(node, rec.id, rec.text);
            if (rec.children != null && rec.children.length > 0) {
                me.loop_insert_project(rec.children, reNode);//循环插入project
            }
        }
        node.expand();
    },
        
    //插入一级子目录到目标目录树
    insert_Project_By_Level: function (level,peer) {
        var me = this;
        var firstLevelNode = me.insert_Select_Project("false");//插入第一级节点
        if (firstLevelNode != null) {
            var nodes = me.sourceTree.getSelectionModel().getSelection();//获取源节点
            var sourceNode = nodes[0];
            me.CreatProjLeavel = 2, me.ProjLeavel = 1;//总共输入二级子目录，当前一级子目录

            Ext.Ajax.request({
                url: 'WebApi/Post',
                method: "POST",
                //method: "GET",
                params: {
                    C: "AVEVA.CDMS.WebApi.ProjectController", A: "getProjectListWithLevel",

                    sid: localStorage.getItem("sid"),
                    ProjectType: 1,
                    node: sourceNode.data.id,
                    peer:peer,//是否需要获取同级目录
                    Level: level //需要添加的目录层级，0为无限级

                },
                success: function (response, options) {
                    me.getProject_callback(response, options);

                }
            });
        }

    },

    ////插入同级所有目录到目标目录树
    //insert_Peer_Project: function (level) {
    //    var me = this;
    //    var firstLevelNode = me.insert_Select_Project();//插入第一级节点
    //    if (firstLevelNode != null) {
    //        var nodes = me.sourceTree.getSelectionModel().getSelection();//获取源节点
    //        var sourceNode = nodes[0];
    //        me.CreatProjLeavel = 2, me.ProjLeavel = 1;//总共输入二级子目录，当前一级子目录

    //        Ext.Ajax.request({
    //            url: 'WebApi/Post',
    //            method: "POST",
    //            //method: "GET",
    //            params: {
    //                C: "AVEVA.CDMS.WebApi.ProjectController", A: "getPeerProjectList",

    //                sid: localStorage.getItem("sid"),
    //                ProjectType: 1,
    //                node: sourceNode.data.id,
    //                Level: level //需要添加的目录层级，0为无限级

    //            },
    //            success: function (response, options) {
    //                me.getProject_callback(response, options);

    //            }
    //        });
    //    }

    //},

    //插入仅勾选节点 到目标目录树
    //selPeer:是否选择同级节点
    insert_Select_Project: function (selPeer) {
        var me = this;
        var reNode;
        var nodes = me.sourceTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {

            var root = me.targetTree.getRootNode();
            var parent = root.firstChild;
            if (parent.data.id != nodes[0].data.id) {
                if (selPeer === 'false') {
                    reNode = me.insert_One_Project(parent, nodes[0]);//插入一个节点
                } else {
                    var sourceParentNode = nodes[0].parentNode;
                    var peerNodes = sourceParentNode.childNodes;
                    for (var i = 0; i < peerNodes.length; i++) {  //从节点中取出子节点依次遍历
                        reNode = me.insert_One_Project(parent, peerNodes[i]);//插入一个节点
                    }
                }
            }
        }
        return reNode;
    },

    //插入一个目录，参数：parent：目标节点，sourceNode:源节点id 
    insert_One_Project: function (parent,sourceNode) {
        var reNode;
        var flag = false;
        var isExists = parent.findChild("id", sourceNode.data.id);//查找属性名为attribute值为指定的value的第一个子节点。
        if (isExists === null) {
            var reNode = parent.appendChild({
                id: sourceNode.data.id,
                text: sourceNode.data.text,
                iconCls: "writefolder",
                leaf: true
            });
            parent.set('leaf', false);
            parent.expand();
        } else {
            reNode = isExists;
        }
        return reNode;
    },

    //插入一个目录，参数：parent：目标节点，sourceNode:源节点id 
    insert_One_Project_ById: function (parent, NodeId,NodeText) {
        var reNode;
        var flag = false;
        var isExists = parent.findChild("id", NodeId);//查找属性名为attribute值为指定的value的第一个子节点。
        if (isExists === null) {
            var reNode = parent.appendChild({
                id: NodeId,
                text: NodeText,
                iconCls: "writefolder",
                leaf: true
            });
            parent.set('leaf', false);
            parent.expand();
        } else {
            reNode = isExists;
        }
        return reNode;
    },

    setTargetLabelText: function (txt) {
        var me = this;
        me.targetLabel.setText(txt);

    }
});