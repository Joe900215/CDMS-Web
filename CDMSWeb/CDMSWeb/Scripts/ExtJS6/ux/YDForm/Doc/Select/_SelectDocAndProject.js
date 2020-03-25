Ext.define('Ext.ux.YDForm.Doc.Select._SelectDocAndProject', {
    extend: 'Ext.container.Container',
    alias: 'widget._SelectDocAndProject',
    layout: 'fit',
    resultvalue: '',

    initComponent: function () {
        var me = this;
        me.renderTo = me.el;
        var resultvalue = '';

        window.parent.resultvalue = "";
        window.parent.doclist = "";

        //定义源目录树
        me.sourceTree = Ext.widget("treepanel", {
            title: "项目目录结构", rootVisible: false, store: "ProjectsTree",
            //layout: 'fit',
            width: "100%",
            //anchor: '100% 100%',
            autoScroll: true,
            containerScroll: true, // 随自身或父容器的改变而显示或隐藏scroll
            split: true,// expanded: true,

            root: { id: "/", text: "根目录"}, //, expanded: true },
            listeners: {
                "select": function (model, record, index, eOpts) {
       
                    //添加延时，防止多次点击目录树
                    setTimeout(function () {
                        me.onTreeItemSelect(model, record);
                    }, 5 * 100);

                }
            }
        });

        //定义grid文档列表
        me.docgrid = Ext.widget("grid", {
            //me.maindocgrid = Ext.create('Ext.grid.Panel',  {
            //title: "文档列表", 
            //id: "_DocGrid",
            store: "Contents",//me._DocListStore,//
            //Grid使用了复选框作为选择行的方式
            selType: "checkboxmodel",
            selModel: { checkOnly: false, mode: "MULTI" }, //"SIMPLE"},//"MULTI" },
            tbar: me._docListTbar,
            //draggable: "true",
            //autoHeight: true,
            //enableDragDrop:true,
            layout: 'fit',
            width: '100%',
            height: '100%',
            //隐藏部分记录
            viewConfig: {
                getRowClass: function (record, rowIndex, p, store) {//CSS class name to add to the row.获得一行的css样式  
                    String.prototype.endWith = function (endStr) {
                        var d = this.length - endStr.length;
                        return (d >= 0 && this.lastIndexOf(endStr) == d)
                    }
                    if (record.data.Title.endWith(".sqlite"))
                        return "hide-store-row";
                }
            },
            bbar: new Ext.PagingToolbar({
                store: me._DocListStore,//"Contents",//
                displayInfo: true,
                displayMsg: '当前显示{0} - {1}条，共{2}条数据',
                emptyMsg: "没有记录"
            }),

            columns: [

                                {//添加图标
                                    menuDisabled: true,
                                    sortable: false,
                                    xtype: 'actioncolumn',
                                    enableColumnResize: false,
                                    width: 38,

                                    items: [{

                                        getClass: function (v, metaData, record) {
                                            var wRight = record.get('WriteRight');
                                            if (wRight === 'true') {//判断是否有写文件权限
                                                if (record.get('O_dmsstatus_DESC') === "检入") {
                                                    return 'doc-check-in';
                                                }
                                                else if (record.get('O_dmsstatus_DESC') === "检出") { return 'doc-check-out'; }
                                                else if (record.get('O_dmsstatus_DESC') === "最终状态") {
                                                    return 'doc-final';
                                                }
                                            } else {
                                                return 'doc-read-only'
                                            }
                                        },
                                        tooltip: '',
                                        handler: function (grid, rowIndex, colIndex) {
                                            //点击图标的时候选中行
                                            grid.getSelectionModel().select(rowIndex, true);
                                        },
                                        listeners: {


                                        }
                                    }, {

                                        getClass: function (v, metaData, record) {
                                            var isShort = record.get('IsShort');
                                            if (isShort === "true") {
                                                return 'docatta'
                                            }

                                            var filename = record.get('O_filename');
                                            var extindex = filename.lastIndexOf('.');
                                            if (extindex >= 0) {
                                                var extname = (filename.substring(extindex + 1)).toLowerCase();

                                                //检查此样式是否已存在
                                                var allExtName = ",pdf,doc,docx,xls,xlsx,dwg,rar,zip,tar,cab,gz,7z,iso,thm,";
                                                if (allExtName.indexOf("," + extname + ",") < 0) {
                                                    return 'docunknown'
                                                }

                                                return 'doc-' + extname + '-ico';
                                            }
                                            else { return 'docunknown' }//alert-col' }



                                        },
                                        tooltip: '',
                                        handler: function (grid, rowIndex, colIndex) {
                                            //点击图标的时候选中行
                                            grid.getSelectionModel().select(rowIndex, true);
                                        },
                                        listeners: {


                                        }
                                    }]
                                },
                {
                    text: '名称', dataIndex: 'Title', flex: 1,//width: 300,
                    draggable: true, renderer: function (value, metaData, record) {
                        //添加提示
                        if (null != value) {
                            metaData.tdAttr = 'data-qtip="' + value + '"';
                            return value;
                        } else {
                            return null;
                        }
                    }
                }

            ],
            listeners: {
                //添加点击文档事件
                "itemclick": function (view, record, item, index, e) {
                    //var me = this;



                },
                "itemdblclick": function (view, record, item, index, e) {//添加双击文档事件

             
                },
                "itemcontextmenu": function (view, record, item, index, e, eOpts) {//添加右键菜单事件
                    
                },
                "itemmouseleave": function (view, record, item, index, e, eOpts) {

                },
                //取消选择一条记录后触发。
                "deselect": function (view, record, index, eOpts) {
                    
                },
                //选择完一条记录后触发。
                "select": function (view, record, index, eOpts) {
                   
                },

                "afterrender": function () {//完成渲染后的事件

                }
            }
            //,
            //flex: 1
        }
        );

        //添加列表
        me.items = [//me.mainSelectPlan
            //{
            //    xtype: "button",
            //    text: "选择用户"
            //}

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
                                              components = athis.query('gridpanel');
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
                                              components = athis.query('gridpanel');
                                              for (var i in components) {
                                                  components[i].setWidth(adjWidth);
                                              }
                                          }
                                      }
                                  }
                              }, flex: 1 },
                      {
                          layout: 'border',
                          baseCls: 'my-panel-no-border',//隐藏边框
                          items: {
                              xtype: "panel",
                              region: 'center',
                              baseCls: 'my-panel-no-border',//隐藏边框
                              items: [me.docgrid],
                              listeners: {//自适应treepanel高度和宽度
                                  resize: function (athis, adjWidth, adjHeight, eOpts) {
                                      // adjHeight为Panel的高度
                                      if (adjHeight > 0) {
                                          // 获取Panel下所有的treepanel
                                          var components = athis.query('treepanel');
                                          for (var i in components) {
                                              components[i].setHeight(adjHeight);
                                          }
                                          components = athis.query('gridpanel');
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
                                          // 获取Panel下所有的treepanel
                                          components = athis.query('gridpanel');
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
                          {
                              baseCls: 'my-panel-no-border',//隐藏边框
                              flex: 1
                          },
                          , {
                              xtype: "button",
                              height: 100,
                              margins: '12,15,12,15',
                              text: "选择",
                              listeners: {
                                  "click": function (btn, e, eOpts) {//添加点击按钮事件
                                      me.select_Doc();
                                  }
                              }, width:100 //flex: 1
                          }, {
                              xtype: "button",
                              height: 100,
                              margins: '12,15,12,15',
                              text: "取消",
                              listeners: {
                                  "click": function (btn, e, eOpts) {//添加点击按钮事件
                                      //me.create_Project();
                                      _winDocAndProjectSelector.close();
                                  }
                              }, width: 100 //flex: 1
                          }], height: 60
                  }, ]
              })

        ];

        //me.usergrid.setSize(panel.getWidth(), panel.ownerCt.getHeight() - panel.getHeight() - me.userinputtext.getHeight() - 300 - 28);

        me.callParent(arguments);
    },

    onTreeItemSelect: function (model, record) {
        var me = this;
        //var Keyword = record.data.Keyword;
        var text = record.data.Desc;

        //if (sels.length > 0) {

        var ProjectKeyword = record.data.Keyword;

        me.loadDocListStore(ProjectKeyword, function () { });

    },

    //刷新文档列表,docKeyword:加载完成后需要跳转的的文档Keyword
    loadDocListStore: function (projectKeyword,callBackFun) {
        var me = this;


            //获取文档列表
        var store = me.docgrid.store;//_DocListStore;//路径：\simplecdms\scripts\app\store\contents.js
            store.proxy.extraParams.ProjectKeyWord = projectKeyword;//把参数传给C#MVC,路径：\simplecdms\controllers\projectcontroller.cs 下的 GetChildProject()
            store.proxy.extraParams.filterJson = "";
            store.proxy.extraParams.sid = localStorage.getItem("sid");
            //store.loadPage(1);
            store.load({
                callback: function (records, options, success) {
                    callBackFun();
                }
            });
    },

    select_Doc: function (projectKeyword, callBackFun) {
        var me = this;

        var records = me.docgrid.getSelectionModel().getSelection();
        var recordList = "";
        var strname = "";

        var docArray = [];

        for (var i = 0; i < records.length ; i++) {
            //recordList = (i === 0 ? recordList : recordList + ",");
            //strname = (i === 0 ? strname : strname + ",");
            //recordList = recordList + records[i].data.Keyword;
            //strname = strname + records[i].data.Title;

            docArray.push(records[i].data);
        }

        window.parent.resultvalue = docArray;
        //window.parent.resultvalue = recordList;
        //window.parent.docnamelist = strname;

        _winDocAndProjectSelector.close();

    }
});