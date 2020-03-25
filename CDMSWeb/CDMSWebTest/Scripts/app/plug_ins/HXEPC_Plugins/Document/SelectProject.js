Ext.define('Ext.plug_ins.HXEPC_Plugins.Document.SelectProject', {
    extend: 'Ext.container.Container',
    alias: 'widget.SelectProject',
    //layout: "border",
    layout: 'fit',
    //resultvalue: '', mainPanelId: '', 
    projectKeyword: '',
    initComponent: function () {
        var me = this;


        Ext.define('ProjectModel', {
            extend: 'Ext.data.Model',
            idProperty: 'projectId',
            fields: [

                { name: 'projectType', type: 'string' },
                { name: 'projectId', type: 'string' },
                { name: 'projectCode', type: 'string' },
                { name: 'projectDesc', type: 'string' }
            ]
        });

        me.projectdata = [];

        me.projectstore = Ext.create('Ext.data.Store', {
            model: 'ProjectModel',
            data: me.projectdata,
            sorters: { property: 'due', direction: 'ASC' },
            groupField: 'projectType'
        });

  

        //定义文本输入框
        me.groupinputtext = Ext.widget('textfield', {
            name: "Title", width: "100%", enableKeyEvents: true,
            fieldLabel: "搜索", anchor: "80%", labelWidth: 30, labelAlign: "left", margin: '0 0 5 5',
            listeners: {
                //这里不能用change函数，因为刷新GRID的时候有问题
                //change: function (field, newValue, oldValue) {
                keyup: function (src, evt) {
                    var value = src.getValue();
                    me.sendSelectProjectDefault(value);
                    //me._groupselstore.proxy.extraParams.Filter = src.getValue();//把参数传给C#MVC,路径：\simplecdms\controllers\Doccontroller.cs 下的 GetWorkFlow()
                    //me._groupselstore.proxy.extraParams.sid = localStorage.getItem("sid");
                    //me._groupselstore.currentPage = 1;
                    //me._groupselstore.load();

                }
            }
        })

        me.projectGrid = Ext.create('Ext.grid.Panel', {
            width: 800,
            height: 450,
            frame: true,
            //title: '',
            //iconCls: 'icon-grid',
            renderTo: document.body,
            store: me.projectstore,
            features: [{
                id: 'group',
                ftype: 'grouping',
                groupHeaderTpl: '{name}',
                hideGroupedHeader: true,
                enableGroupingMenu: false
            }],
            columns: [{
                header: '编码',
                width: 90,
                sortable: true,
                dataIndex: 'projectCode'
            }, {
                text: '名称',
                flex: 1,
                tdCls: 'task',
                sortable: true,
                dataIndex: 'projectDesc',
                hideable: false
            }, {
                header: 'Project',
                width: 180,
                sortable: true,
                dataIndex: 'projectType'
            }
            ]
        });

        //定义已选择用户的model
        Ext.define("_ProjectResult", {
            extend: "Ext.data.Model",
            fields: ["projectType", "projectId", "projectCode", "projectDesc"],
            url: "_blank",
        });

        //定义已选择用户的store
        me._projectresultstore = Ext.create("Ext.data.Store", {
            batchActions: false,
            //文章的Store需要支持远程排序和搜索
            remoteFilter: true,
            remoteSort: true,
            //每50条记录为一页
            pageSize: 50,//视图路径：\simplecdms\scripts\app\view\content\view.js
            model: "_ProjectResult"

        });

        //定义已选择用户的view
        me.resultgrid = Ext.widget("grid", {
            //region: "center",
            //layout: "hbox",
            height: '100%',
            //width: "100%",
            cellTip: true,
            store: me._projectresultstore,
            columns: [
                { text: '代码', dataIndex: 'projectCode', width: 120 },
                { text: '名称', dataIndex: 'projectDesc', flex: 1 }//width: '100%' }
            ],
            listeners: {
                //'rowdblclick': function (grid, rowIndex, e) {
                'itemdblclick': function (view, record, item, index, e) {
                    //双击时，从已选择grid删除双击的节点
                    if (typeof (record.raw) != 'undefined' || typeof (record.data) != 'undefined') {
                        //var Keyword = record.data.id;
                        //var Text = record.data.text;

                        //删除行
                        var sm = me.resultgrid.getSelectionModel();
                        var store = me.resultgrid.getStore();
                        store.remove(sm.getSelection());
                        if (store.getCount() > 0) {
                            sm.select(0);
                        }
                    }
                }
            }, flex: 1
        });



        //添加列表
        me.items = [
          Ext.widget('form', {
              baseCls: 'my-panel-no-border',//隐藏边框
              layout: {
                  type: 'vbox',
                  align: 'stretch',
                  pack: 'start'
              },
              items: [{//上部容器
                  baseCls: 'my-panel-no-border',//隐藏边框
                  layout: {
                      type: 'hbox',
                      pack: 'start',
                      align: 'stretch'
                  },
                  margin: '10 5 0 5',// 
                  items: [
                      {
                          xtype: "panel", margin: '0 0 0 0',
                          baseCls: 'my-panel-no-border',//隐藏边框
                          layout: {
                              type: 'vbox',
                              pack: 'start',
                              align: 'stretch'
                          }, width: '50%',
                          items: [
                              //{
                              //    xtype: "panel", margin: '0 0 5 0',
                              //    baseCls: 'my-panel-no-border',//隐藏边框
                              //    layout: {
                              //        type: 'hbox',
                              //        pack: 'start',
                              //        align: 'stretch'
                              //    },
                              //    items: [
                              //      //  me.addressField,
                              //            me.searchTrigger
                              //    ]
                              //},
                              me.groupinputtext,
                             me.projectGrid
                          ]
                      }, {
                          xtype: "panel", margin: '0 0 0 5',
                          baseCls: 'my-panel-no-border',//隐藏边框
                          layout: {
                              type: 'vbox',
                              pack: 'start',
                              align: 'stretch'
                          }, width: '50%',
                          items: [
                               me.resultgrid
                          ]
                      }
                  ]
              }, {
                  xtype: "panel",
                  layout: "hbox",
                  baseCls: 'my-panel-no-border',//隐藏边框
                  //align: 'right',
                  //pack: 'end',//组件在容器右边
                  margins: "0 15 0 15",
                  items: [
                      {
                          flex: 1, baseCls: 'my-panel-no-border'//隐藏边框
                      },
                    {
                        xtype: "button",
                        text: "确定", width: 60, margins: "10 5 10 5",
                        listeners: {
                            "click": function (btn, e, eOpts) {//添加点击按钮事件
                                //确定按钮事件
                                var strresult, strname;
                                var resultstore = me.resultgrid.getStore();
                                var userArray = new Array();
                                for (var i = 0; i < resultstore.getCount() ; i++) {
                                    //strresult = strresult + ";" + resultstore.getAt(i).data.id + "," + resultstore.getAt(i).data.text; //遍历每一行
                                    if (i > 0) {
                                        strresult = strresult + "," + resultstore.getAt(i).data.projectCode;
                                        strname = strname + ";" + resultstore.getAt(i).data.projectDesc;
                                        strvalue = strvalue + "," + resultstore.getAt(i).data.projectId;
                                    } else {
                                        strresult = resultstore.getAt(i).data.projectCode;
                                        strname = resultstore.getAt(i).data.projectDesc;
                                        strvalue =  resultstore.getAt(i).data.projectId;
                                        strtype = resultstore.getAt(i).data.projectType;
                                    }

                                }

                                window.parent.resultvalue = strresult;//functionName(strresult);
                                window.parent.projectdesclist = strname;//functionName(strresult);
                                window.parent.projectvaluelist = strvalue;//functionName(strresult);
                                window.parent.projectType = strtype;
                                winSelectProject.close();

                            }
                        }
                    }, {
                        xtype: "button",
                        text: "取消", width: 60, margins: "10 5 10 5",
                        listeners: {
                            "click": function (btn, e, eOpts) {//添加点击按钮事件

                                winSelectProject.close();

                            }
                        }
                    }
                  ]
              }]
          })];

        function addItemToResultGrid(record) {
            //双击时，插入记录到已选择grid
            if (typeof (record.raw) != 'undefined' || typeof (record.data) != 'undefined') {

                var ProjectType = record.data.projectType;
                var ProjectId = record.data.projectId;
                var ProjectCode = record.data.projectCode;
                var ProjectDesc = record.data.projectDesc;


                var flag = true;
                var resultstore = me.resultgrid.getStore();
                for (var i = 0; i < resultstore.getCount() ; i++) {//遍历每一行
                    if (resultstore.getAt(i).data.projectType === ProjectType && resultstore.getAt(i).data.projectId === ProjectId) {
                        flag = false;
                        break;
                    }
                }

                if (flag === true) {
                    //插入行到返回grid
                    var r = Ext.create('_ProjectResult', {
                        projectType: ProjectType,
                        projectId: ProjectId,
                        projectCode: ProjectCode,
                        projectDesc: ProjectDesc
                    });


                    var rowlength = me.resultgrid.getStore().data.length;
                    me.resultgrid.getStore().insert(rowlength, r);

                    me.resultgrid.getView().refresh();
                }

            }
        };

        //添加用户选择grid双击事件
        me.projectGrid.on('itemdblclick', function (view, record, item, index, e) {
            addItemToResultGrid(record);
        });

        me.sendSelectProjectDefault("");

        window.parent.resultvalue = "";
        window.parent.projectdesclist = "";
        window.parent.projectvaluelist = "";

        me.callParent(arguments);

    },

    //获取新建厂家资料目录表单默认参数
    sendSelectProjectDefault: function (filter) {
        var me = this;

        //通过extjs的ajax获取操作全部名称
        Ext.Ajax.request({
            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.HXEPC_Plugins.Company", A: "GetSelectProjectList",
                sid: localStorage.getItem("sid"), ProjectKeyword: me.projectKeyword,
                Filter: filter
            },
            success: function (response, options) {
                me.sendGetSelectProjectList_callback(response, options);//, funCallback);
            },
            failure: function (response, options) {
                ////Ext.Msg.alert("错误", "连接服务器失败！<br>" + response.responseText);
            }
        });
    },

    //处置获取新建厂家资料目录表单默认参数的返回
    sendGetSelectProjectList_callback: function (response, options) {
        var me = this;



        //获取数据后，更新窗口
        var res = Ext.JSON.decode(response.responseText, true);
        var state = res.success;
        if (state === true) {
            //var recod = eval(res.data[0]);

            //var companyList = eval(recod.CompanyList);
            me.projectGrid.store.removeAll();

            var companyList = eval(res.data);

            for (var itemKey in companyList) {
                //me.projectKeyword = companyList[itemKey].companyCode;
                if (companyList[itemKey].projectCode != undefined) {

                    //插入行到文件selectUserGrid
                    var r = Ext.create('ProjectModel', {

                        projectType: companyList[itemKey].projectType,
                        projectId: companyList[itemKey].projectId,
                        projectCode: companyList[itemKey].projectCode,
                        projectDesc: companyList[itemKey].projectDesc
                    });

                    var rowlength = me.projectGrid.getStore().data.length;
                    me.projectGrid.getStore().insert(rowlength, r);
                }
            }
            me.projectGrid.getView().refresh();
        }
    }

});