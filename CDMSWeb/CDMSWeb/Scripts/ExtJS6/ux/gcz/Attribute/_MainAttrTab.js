/*定义属性TAB*/
Ext.define('Ext.ux.gcz.Attribute._MainAttrTab', {
    //extend: 'Ext.container.Container',
    extend: 'Ext.panel.Panel',
    alias: 'widget._mainAttrTab', // 此类的xtype类型为buttontransparent  
    activeTab: 0, region: "east", width: "30%", minWidth: 100, split: true, collapsible: true,
    layout: 'fit',
    initComponent: function () {
        var me = this;
        me.renderTo = me.el;
        
        //传递给后台的参数
        me.seleObjType = "";//是选择了DOC还是Project
        me.DocKeyword = "";
        me.ProjectKeyword = "";
        me.tmpParentPath = "";
        //文件列表
        me.objfilelist = "";
        //解压缩文件后，所在的子目录
        me.subFolder="";

        me.DocAttrDataCount = "0";//是否显示文档附加属性
        me.ProjectAttrDataCount = "0";//是否显示目录附加属性
        me.Doc_o_filename = "";//根据文件名，判断是否显示预览文件TAB页

        //Tab页显示类型开关,决定了点击目录树节点或文档列表节点时，是否切换Tab页的显示
        me.TabDispType = "Project";
        //Tab页Change事件锁，防止目录和文档之间互相切换时，重复执行Tab的Change事件
        me.TabChangeLock = false;

        me.oMainDocViewWidth = 0;
        me.oMainTreeViewWidth = 0;
        me.oViewWidth = 0;
        me.DocPreviewState = false;
        //记录上一次选择的Tab页
        me.lastTabTitle = "";

        me._ProjectAttrGrid = Ext.create('Ext.ux.YDForm._MainAttrGrid');

        me._ProjectAttrDataGrid = Ext.create('Ext.ux.YDForm._MainAttrGrid');

        me._DocAttrGrid = Ext.create('Ext.ux.YDForm._MainAttrGrid');

        me._DocAttrDataGrid = Ext.create('Ext.ux.YDForm._MainAttrGrid');

        //me._RightGrid = Ext.create('Ext.ux.YDForm._RightGrid');

        me._DocPreviewPanel=Ext.create('Ext.panel.Panel', {

            //width: '100%',
            flex:1,
            html: '<div id="mapPic">' + '<iframe src="Scripts/PDFJSInNet/web/viewer.html?file=/download/N4LRTNRJ2XV0TFB4/%E8%BD%AF%E4%BB%B6%E4%BD%BF%E7%94%A8%20%E8%AF%B4%E6%98%8E%E4%B9%A6.pdf"  scrolling="no" style="width:100%;height:600px" frameborder="0"></iframe> </div>'
            
        });

        Ext.define('Ext.ux.YDForm._MainAttrGrid._RarListModel', {
            extend: 'Ext.data.Model',
            fields: ['id',
                    'name',
                    'size',
                    'type',
                    'path',
                    'parentpath',
                    'subFolder'
            ]
        });



        me._RarListStore = Ext.create('Ext.data.JsonStore', {
            model: 'Ext.ux.YDForm._MainAttrGrid._RarListModel',
            listeners: {
                //load: me.Load_RarListStoreView(""),
                //remove: me.onStoreRemove,
                //update: me.onStoreUpdate,
                scope: me
            }
        });

        me._RarListView = new Ext.grid.Panel({
            store: me._RarListStore,
            stateful: true,
            multiSelect: true,
            height: '100%',
            width: 280,
            hidden : true,//默认是隐藏状态
            //stateId: 'stateGrid',
            columns: [
                  {//添加图标
                      menuDisabled: true,
                      sortable: false,
                      xtype: 'actioncolumn',
                      enableColumnResize: false,
                      width: 20,

                      items: [{

                          getClass: function (v, metaData, record) {
                              var filetype = record.get('type');
                              if (filetype === 'file') {
                                  var filename = record.get('name');
                                  var extindex = filename.lastIndexOf('.');
                                  if (extindex >= 0) {
                                      var extname = (filename.substring(extindex + 1)).toLowerCase();
                                      return 'doc-' + extname + '-ico';
                                  }
                                  else { return 'alert-col' }
                              } else {//设置目录图标
                                  return 'folder-rar'
                              }
                          },
                          //tooltip: 'Sell stock',
                          handler: function (grid, rowIndex, colIndex) {
                          }
                      }]
                  }, {
                      text: '文件名',
                      flex: 1,
                      sortable: false,
                      dataIndex: 'name'
                  }
            ,
                    {
                        text: '大小',
                        width: 60,
                        sortable: true,
                        align: 'right',
                        // renderer: Ext.util.Format.fileSize,
                        dataIndex: 'size'
                    }

            ],
            viewConfig: {
                getRowClass: function (record, rowIndex, p, store) {//CSS class name to add to the row.获得一行的css样式  
                    if (record.data.parentpath != me.tmpParentPath)
                        return "hide-store-row";
                }
            },
            listeners: {
                beforeselect: function (grid, record, index, eOpts) {
                    if (record.data.type === "file") {
                        var mainDocView = me.up('_mainSourceView').down('_mainDocGrid');
                        mainDocView.PreviewDoc("true", "true", me.subFolder + "/" + record.data.parentpath, record.data.name);
                    }
                },
                //itemclick: function( grid, record, item, index, e, eOpts ){
                //    if (record.data.type === "file") {
                //        var mainDocView = me.up('_mainSourceView').down('_mainDocGrid');
                //        mainDocView.PreviewDoc("true", record.data.path, record.data.name);
                //    }
                //},
                beforeitemdblclick: function (grid, record, item, index, e, eOpts)
                {
                    // var me = this;
                    if (record.data.type === "file") {
                        window.open(record.data.path + record.data.name, '_blank');//新窗口打开链接
                    } else if (record.data.type === "dir") {
                        if (record.data.name === "..")
                        {
                            me.tmpParentPath =  record.data.path;//parentpath; //me.tmpParentPath;//record.data.name;
                        }
                        else if (me.tmpParentPath === "")
                            me.tmpParentPath =  record.data.name;
                        else {
                            me.tmpParentPath =  me.tmpParentPath + "/" + record.data.name;
                        }
                        
                        //重新加载表格，动态隐藏表格行
                        me._RarListStore.removeAll();

                        //Ext.each(me.objfilelist, function (v) {
                        //    me.updateRarListStore(v);
                        //}, me);

                        me.updateRarListStore(me.objfilelist);

                    }
                }
            }
        });


        me._DocPreviewMainPanel = {
            xtype: 'panel',
            layout: "hbox",
            width: '100%',
            baseCls: 'my-panel-no-border',//隐藏边框
            margins: "5 5 5 5",
            items: [
                //me._RarListViewPanel,
                me._RarListView,
                
                me._DocPreviewPanel
            ],flex:1
        }
        
        //定义流程TAB页
        me.workFlowTabPage = Ext.create('Ext.ux.YDForm.WorkFlow._WorkFlowPage');

        me.workFlowTabPage.mainPanelType="Project";

        Ext.QuickTips.init();

        //添加属性TAB页面
        me.mainattrtab = Ext.create('Ext.tab.Panel', {
            xtype: 'tabpanel',
            activeTab: 0,
            defaults: {
                border: false,
                bodyPadding: 5, bodyStyle: "background:#DFE9F6"
            },
            items: [
                {
                    title: '目录属性',
                    //xtype: 'grid',
                    layout: 'fit',
                    xtype: 'panel',
                    items: me._ProjectAttrGrid
                }
            , {
                title: '目录_附加属性',
                //xtype: 'grid',
                layout: 'fit',
                xtype: 'panel',
                items: me._ProjectAttrDataGrid
            },
            {
                title: '文档属性',
                //xtype: 'grid',
                layout: 'fit',
                xtype: 'panel',
                hidden: true,
                items: me._DocAttrGrid
            },
            {
                title: '文档_附加属性',
                //xtype: 'grid',
                layout: 'fit',
                hidden: true,
                items:me._DocAttrDataGrid
            },
            {
                title: "文件预览", layout: "vbox", hidden: true,
                items: me._DocPreviewMainPanel
            },
            {
                title: "关联", layout: "anchor",
                //id: 'fpFileUpload',
                hidden: true,
                xtype: 'panel',
                width: 500,
                bodyPadding: '10 10 0',
                fileUpload: true,
                items: [
                   // fpFileUpload,
                   // fpFileUpload2
                ]

            },
            {
                title: "流程", layout: "anchor", hidden: true,
                items: [me.workFlowTabPage
                ]
            }
            //,
            //{
            //    title: "权限列表", layout: "anchor", hidden: false,
            //    items: [me._RightGrid
            //    ]
            //}
            ], listeners: {
                tabchange: function (tab, eOpt) {
                    //Ext.Msg.alert("错误信息", tab.text);

                    if (eOpt.title === "文件预览") {

                        var mainDocView = me.up('_mainSourceView').down('_mainDocGrid');
                        mainDocView.PreviewDoc("true", "false", "", "");

                        me.setDocPreviewWidth();
                    } else if (eOpt.title === "权限列表") {

                        var rightGridView = me.up('_mainSourceView').down('_RightGrid');
                        if (me.seleObjType === "Doc")
                            rightGridView.loadRightPage(me.seleObjType, me.DocKeyword);
                        else if (me.seleObjType === "Project")
                            rightGridView.loadRightPage(me.seleObjType, me.ProjectKeyword);

                        me.setDocPreviewWidth();
                    } else {
                        //这里无法一次正确完成查询，只有切换的时候查询两次才能正确显示
                        if (me.TabChangeLock === true)// && me.TabChangeLock === false)
                        {
                            //Tab页Change事件锁，防止目录和文档之间互相切换时，重复执行Tab的Change事件
                            me.TabChangeLock = false;
                        } else {
                            if (eOpt.title === "文档_附加属性") {
                                me.loadDocAttrPage("docAttrData", me.DocKeyword);
                                //  me.setAttrTabDisplay("Doc", me.DocKeyword);
                            }

                            else if (eOpt.title === "文档属性") {
                                me.loadDocAttrPage("docAttr", me.DocKeyword);
                                // me.setAttrTabDisplay("Doc", me.DocKeyword);
                            }

                            else if (eOpt.title === "目录属性") {
                                me.loadDocAttrPage("projectAttr", me.ProjectKeyword);
                                // me.setAttrTabDisplay("Project", me.ProjectKeyword);
                            }

                            else if (eOpt.title === "目录_附加属性") {
                                me.loadDocAttrPage("projectAttrData", me.ProjectKeyword);
                                // me.setAttrTabDisplay("Project", me.ProjectKeyword);
                            }
                        }
                            
                        

                        //如果是文件预览状态转换到非预览状态，就重新设置宽度
                        if (me.DocPreviewState === true)
                        {
                            var mainTreeView = me.up('_mainSourceView').down('_mainProjectTree');
                            var mainDocView = me.up('_mainSourceView').down('_mainDocGrid');

                            me.setWidth(me.oViewWidth);
                            mainDocView.setWidth(me.oMainDocViewWidth);
                            mainTreeView.setWidth(me.oMainTreeViewWidth);

                            me.DocPreviewState = false;
                        }
                    }
                    me.lastTabTitle = eOpt.title;
                }
            }
        });

        //添加属性TAB页面到容器
        me.items = [
            			//{
            			//    xtype:"button",
            			//    text:"我的按钮"
            			//}
                               me.mainattrtab
        ];

        me.callParent(arguments);

    },

    //刷新文档属性和附加属性TAB页
    loadDocAttrPage: function (tabType, Keyword) {

        var me = this;
        if (tabType === "docAttr") {

            me.DocKeyword = Keyword;
            me._DocAttrGrid.loadAttrPage(tabType, Keyword);
            me.setAttrTabDisplay("Doc", Keyword);

        } else if (tabType === "docAttrData") {

            me.DocKeyword = Keyword;
            me._DocAttrDataGrid.loadAttrPage(tabType, Keyword);
            me.setAttrTabDisplay("Doc", Keyword);

        } else if (tabType === "projectAttr") {

            me.ProjectKeyword = Keyword;
            me._ProjectAttrGrid.loadAttrPage(tabType, Keyword,"2");
            me.setAttrTabDisplay("Project", Keyword);

        } else if (tabType === "projectAttrData") {

            me.ProjectKeyword = Keyword;
            me._ProjectAttrDataGrid.loadAttrPage(tabType, Keyword);
            me.setAttrTabDisplay("Project", Keyword);

        }
       
    },

    //设置屏幕到预览的宽度
    setDocPreviewWidth: function () {
        var me = this;
        //如果上一次选择的也是宽屏的Tab，就不修改宽度
        if (me.lastTabTitle != "文件预览" && me.lastTabTitle != "权限列表") {
            var mainTreeView = me.up('_mainSourceView').down('_mainProjectTree');

            var mainDocView = me.up('_mainSourceView').down('_mainDocGrid');

            //保存原来的宽度
            me.oMainDocViewWidth = mainDocView.width ? mainDocView.width : me.getDomWidth(mainDocView); //mainDocView.width;
            me.oMainTreeViewWidth = mainTreeView.width ? mainTreeView.width : me.getDomWidth(mainTreeView);  //mainTreeView.width;
            me.oViewWidth = (me.width && me.width.toString().indexOf("%") < 0) ? me.width : me.getDomWidth(me); //me.width;

            //设置宽度
            me.setWidth(document.body.clientWidth / 3 * 2);
            mainDocView.setWidth(document.body.clientWidth / 6);
            mainTreeView.setWidth(document.body.clientWidth / 6);

            me.DocPreviewState = true;
        }
    },

    getDomWidth:function(view){
        var str = view.el.dom.style.width;
        var str2 = str.replace(/px/, "");
        var Width = parseInt(str2) - 70;
        return Width;
    },

    ////更新压缩文件列表数据，刷新表单
    updateRarListStore: function (data) {
        var me = this;

        me._RarListStore.loadData(data, true);
        //me._RarListStore.loadData([v], true);
    },

    ////刷新文档预览
    //updateDocPreview: function (strHtml,DocKeyword) {//Scripts/PDFJSInNet/web/viewer.html
    //    var me = this;
    //    if (DocKeyword!="")
    //        me.DocKeyword = DocKeyword;

    //    var str = me.el.dom.style.height;
    //    var str2=str.replace(/px/, "");
    //    var h = (parseInt(str2) - 70).toString() + "px";

    //    if (strHtml != "") {
            
    //        me._DocPreviewPanel.update('<div id="mapPic">' + '<iframe src="' + strHtml + '"  scrolling="no" style="width:100%;height:' + h + '" frameborder="0"></iframe> </div>', false, function () {
    //        });
    //    } else {
    //        me._DocPreviewPanel.update('<div id="mapPic">' + '<iframe src=""  scrolling="no" style="width:100%;height:' + h + '" frameborder="0"></iframe> </div>');
    //    }
    //},

    //设置是否显示文档附加属性
    setDocAttrDataCount: function (attrDataCount) {
        var me = this;
        me.DocAttrDataCount = attrDataCount;
        if (attrDataCount != "0") {
            me.mainattrtab.tabBar.items.items[3].show();   //显示文档附加属性
        } else {
            me.mainattrtab.tabBar.items.items[3].hide();   //隐藏文档附加属性
        }
    },

    //设置是否显示目录附加属性
    setProjectAttrDataCount: function (attrDataCount,e) {
        var me = this;
        me.ProjectAttrDataCount = attrDataCount;
        if (attrDataCount != "0" && attrDataCount != "1") {
            //if (me.mainattrtab.tabBar.items.items[1].hidden=true)
                me.mainattrtab.tabBar.items.items[1].show();   //显示文档附加属性
        } else {
            //if (me.mainattrtab.tabBar.items.items[1].hidden = false)
            me.mainattrtab.tabBar.items.items[1].hide();   //隐藏文档附加属性
        }
    },

    
    //设置是否显示文档预览文件
    setDocO_filename: function (O_filename) {
        var me = this;
        me.DocO_filename = O_filename;
        if (O_filename != "") {
            me.mainattrtab.tabBar.items.items[4].show();   //显示文档预览
        } else {
            me.mainattrtab.tabBar.items.items[4].hide();   //隐藏文档预览
        }
    },

    //
    showRarGrid: function () {
        var me = this;
        //me._RarListViewPanel.show();
        me._RarListView.show();
    },

    hideRarGrid: function () {
        var me = this;
        //me._RarListViewPanel.hide();
        me._RarListView.hide();
    },


    //流程TAB切换事件,参数：DispType：设置要显示的类别,Keyword:选取的doc的关键字
    setAttrTabDisplay: function (DispType, Keyword) {
        //Ext.Msg.alert("您切换了TAB页！！！", "您切换了TAB页！！！");
        var me = this;

        //获取类别为_mainAttrTab的控件
        var tab = me.mainattrtab;

        var activeTab = tab.getActiveTab().title;

        if (DispType === 'Doc') {//显示文档属性
            me.DocKeyword = Keyword;
            //if (activeTab.indexOf("目录") >= 0) {
            //如果目录Tab是显示状态，就隐藏目录TAB
            //if (tab.items.items["2"].hidden === true && tab.items.items["3"].hidden === true) {
            //if ((tab.items.items["2"].hidden === true || tab.items.items["2"].hidden==="undefined") && (tab.items.items["3"].hidden === true || tab.items.items["3"].hidden==="undefined")) {
            //if (tab.items.items["2"].hidden === true && tab.items.items["3"].hidden === true ){//|| tab.items.items["2"].hidden==="undefined"){
            if ( me.TabDispType === "Project"){
                tab.tabBar.items.items[0].hide();   //隐藏目录属性tab页
                tab.tabBar.items.items[1].hide();   //隐藏目录附加属性tab页
                tab.tabBar.items.items[2].show();   //文档属性

                if (me.DocAttrDataCount != "0") {//当文档附加属性数量为零，不显示附加属性Tab页
                    tab.tabBar.items.items[3].show();   //文档附加属性
                } else {
                    tab.tabBar.items.items[3].hide();   //文档附加属性
                }

                tab.tabBar.items.items[4].show();   //文件预览
                me.TabDispType = "Doc";
                tab.setActiveTab(2);
            }
            else if (me.DocAttrDataCount != "0") {//当文档附加属性数量为零，不显示附加属性Tab页
                tab.tabBar.items.items[3].show();   //文档附加属性
            } else {
                tab.tabBar.items.items[3].hide();   //文档附加属性
                if (activeTab.indexOf("文档_附加属性") >= 0) {
                    tab.setActiveTab(2);
                }
            }

            //判断是否有流程，有流程就显示流程意见页
            ////获取流程TAB
            var tabWf = me.workFlowTabPage;
            tabWf.loadWorkflowAuditPage("Doc", Keyword);
        }
        else if (DispType === 'Project')//显示目录属性
        {
            me.ProjectKeyword = Keyword;
            //if (activeTab.indexOf("目录") < 0) {
            //如果目录Tab是隐藏状态，就显示目录TAB
            //if ((tab.items.items["0"].hidden === true || tab.items.items["0"].hidden==="undefined") && (tab.items.items["1"].hidden === true || tab.items.items["1"].hidden==="undefined")) {
            var hh = tab.items.items["1"].hidden;

            //if (tab.items.items["0"].hidden === true && (tab.items.items["1"].hidden === true || tab.items.items["1"].hidden === "undefined")) {// || tab.items.items["0"].hidden==="undefined"){
            if ( me.TabDispType === "Doc"){
                tab.tabBar.items.items[0].show();   //显示tab页

                if (me.ProjectAttrDataCount != "0" && me.ProjectAttrDataCount != "1") {//当文档附加属性数量为零，不显示附加属性Tab页
                    //if (tab.tabBar.items.items[1].hidden = true)
                    tab.tabBar.items.items[1].show();   //显示tab页
                } else {
                   // if (tab.tabBar.items.items[1].hidden = false)
                    tab.tabBar.items.items[1].hide();   //显示tab页
                }

                tab.tabBar.items.items[2].hide();   //隐藏tab页
                tab.tabBar.items.items[3].hide();
                tab.tabBar.items.items[4].hide();   //文件预览
                tab.tabBar.items.items[5].hide();
                tab.tabBar.items.items[6].hide();

                //me.Item0Hidden = false;
                //me.Item2Hidden = false;
                me.TabDispType = "Project";

                tab.setActiveTab(0);
            }
            else if (me.ProjectAttrDataCount != "0" && me.ProjectAttrDataCount != "1") {//当文档附加属性数量为零，不显示附加属性Tab页
                tab.tabBar.items.items[1].show();   //显示tab页
            } else {
                if (activeTab.indexOf("目录_附加属性") >= 0) {
                    tab.setActiveTab(2);
                }
                tab.tabBar.items.items[1].hide();   //显示tab页

            }

            //判断是否有流程，有流程就显示流程意见页
            ////获取流程TAB
            var tabWf = me.workFlowTabPage;
            tabWf.loadWorkflowAuditPage("Project", Keyword);
        }
        //else if (DispType === 'showWorkflow')//显示流程TAB页
        //{
        //    //tab.tabBar.items.items[5].show();
        //    tab.tabBar.items.items[6].show();
        //}
        //else if (DispType === 'hideWorkflow') {//隐藏流程TAB页
            
        //    tab.tabBar.items.items[5].hide();
        //    tab.tabBar.items.items[6].hide();

        //    var activeTab = tab.getActiveTab().title;

        //    if (activeTab === "流程" || activeTab === "关联") {
        //        tab.setActiveTab(2);
        //    }
        //}
    },

    //显示流程TAB页
    showWorkflowTab: function () {
        var me = this;
        var tab = me.mainattrtab;
        tab.tabBar.items.items[6].show();
    },

    //隐藏流程TAB页
    hideWorkflowTab: function () {
        var me = this;
        var tab = me.mainattrtab;

        tab.tabBar.items.items[5].hide();
        tab.tabBar.items.items[6].hide();

        var activeTab = tab.getActiveTab().title;

        if (activeTab === "流程" || activeTab === "流程(完成)" || activeTab === "关联") {
            tab.setActiveTab(2);
        }

    },

    //设置流程标题，显示是否已完成
    setWorkFlowTabTitle: function (isFinish) {
        var me = this;
        var tab = me.mainattrtab;

        var page = me.workFlowTabPage;

        var tabItem=tab.tabBar.items.items[6];
        if (isFinish === "True") {
            tab.items.items[6].setTitle("流程(完成)");

        } else {
            tab.items.items[6].setTitle("流程");

        }
    }
});