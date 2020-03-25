Ext.define('Ext.plug_ins.HXEPC_Plugins.Document.SelectCrew', {
    extend: 'Ext.container.Container',
    alias: 'widget.SelectCrew',
    //layout: "border",
    layout: 'fit',
    //resultvalue: '', mainPanelId: '', 
    projectKeyword: '',
    initComponent: function () {
        var me = this;


        Ext.define('CrewModel', {
            extend: 'Ext.data.Model',
            idProperty: 'crewId',
            fields: [

                { name: 'crewType', type: 'string' },
                { name: 'crewId', type: 'string' },
                { name: 'crewCode', type: 'string' },
                { name: 'crewDesc', type: 'string' }
            ],
            proxy: {
                type: "ajax",
                url: 'WebApi/Get',
                extraParams: {
                    C: "AVEVA.CDMS.HXEPC_Plugins.Company", A: "GetSelectCrewList",
                    ProjectKeyword: me.projectKeyword, sid: localStorage.getItem("sid")
                },
                reader: {
                    type: 'json',
                    totalProperty: 'total',
                    root: "data",//从C#MVC获取数据\simplecdms\controllers\ProjectController.cs .GetDocList.data  ，获取到的数据传送到model里面
                    messageProperty: "Msg"
                },
                writer: {
                    type: "json",
                    encode: true,
                    root: "data",
                    allowSingle: false
                },
                listeners: {
                    exception: CDMSWeb.ProxyException
                }
            }
        });

        me.crewdata = [];


        me.crewstore = Ext.create('Ext.data.Store', {
            batchActions: false,
            //文章的Store需要支持远程排序和搜索
            remoteFilter: true,
            remoteSort: true,
            //每50条记录为一页
            pageSize: 50,//视图路径：\simplecdms\scripts\app\view\content\view.js

            model: 'CrewModel',
            data: me.crewdata,
            sorters: { property: 'due', direction: 'ASC' },
            groupField: 'crewType'
        });


        //定义文本输入框
        me.groupinputtext = Ext.widget('textfield', {
            name: "Title", width: "100%", enableKeyEvents: true,
            fieldLabel: "搜索", anchor: "80%", labelWidth: 30, labelAlign: "left", margin: '0 0 5 5',
            listeners: {
                //这里不能用change函数，因为刷新GRID的时候有问题
                //change: function (field, newValue, oldValue) {
                keyup: function (src, evt) {
                    //var value = src.getValue();
                    //me.sendSelectCrewDefault(value);

                    me.crewstore.proxy.extraParams.Filter = src.getValue();//把参数传给C#MVC,路径：\simplecdms\controllers\Doccontroller.cs 下的 GetWorkFlow()
                    me.crewstore.proxy.extraParams.sid = localStorage.getItem("sid");
                    me.crewstore.proxy.extraParams.Group = "";//重置机构组
                    me.crewstore.currentPage = 1;
                    me.crewstore.load();

                }
            }
        })

        me.crewGrid = Ext.create('Ext.grid.Panel', {
            width: 800,
            height: 450,
            frame: true,
            bbar: new Ext.PagingToolbar({
                //pageSize: 5,//pageSize,  
                store: me.crewstore,
                displayInfo: true,
                displayMsg: '当前显示{0} - {1}条，共{2}条数据',
                emptyMsg: "没有记录"
            }),
            //renderTo: document.body,
            store: me.crewstore,
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
                dataIndex: 'crewCode'
            }, {
                text: '名称',
                flex: 1,
                tdCls: 'task',
                sortable: true,
                dataIndex: 'crewDesc',
                hideable: false
            }, {
                header: 'Project',
                width: 180,
                sortable: true,
                dataIndex: 'crewType'
            }
            ]
        });

        //定义已选择用户的model
        Ext.define("_CrewResult", {
            extend: "Ext.data.Model",
            fields: ["crewType", "crewId", "crewCode", "crewDesc"],
            url: "_blank",
        });

        //定义已选择用户的store
        me._crewresultstore = Ext.create("Ext.data.Store", {
            batchActions: false,
            //文章的Store需要支持远程排序和搜索
            remoteFilter: true,
            remoteSort: true,
            //每50条记录为一页
            pageSize: 50,//视图路径：\simplecdms\scripts\app\view\content\view.js
            model: "_CrewResult"

        });

        //定义已选择用户的view
        me.resultgrid = Ext.widget("grid", {
            //region: "center",
            //layout: "hbox",
            height: '100%',
            //width: "100%",
            cellTip: true,
            store: me._crewresultstore,
            columns: [
                { text: '代码', dataIndex: 'crewCode', width: 120 },
                { text: '名称', dataIndex: 'crewDesc', flex: 1 }//width: '100%' }
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

                              me.groupinputtext,
                             me.crewGrid
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
                                        strresult = strresult + "," + resultstore.getAt(i).data.crewCode;
                                        strname = strname + ";" + resultstore.getAt(i).data.crewDesc;
                                        strvalue = strvalue + "," + resultstore.getAt(i).data.crewType + resultstore.getAt(i).data.crewId;
                                    } else {
                                        strresult = resultstore.getAt(i).data.crewCode;
                                        strname = resultstore.getAt(i).data.crewDesc;
                                        strvalue = resultstore.getAt(i).data.crewType + resultstore.getAt(i).data.crewId;
                                    }

                                }

                                window.parent.resultvalue = strresult;//functionName(strresult);
                                window.parent.crewdesclist = strname;//functionName(strresult);
                                window.parent.crewvaluelist = strvalue;//functionName(strresult);
                                winSelectCrew.close();

                            }
                        }
                    }, {
                        xtype: "button",
                        text: "取消", width: 60, margins: "10 5 10 5",
                        listeners: {
                            "click": function (btn, e, eOpts) {//添加点击按钮事件

                                winSelectCrew.close();

                            }
                        }
                    }
                  ]
              }]
          })];

        function addItemToResultGrid(record) {
            //双击时，插入记录到已选择grid
            if (typeof (record.raw) != 'undefined' || typeof (record.data) != 'undefined') {

                var CrewType = record.data.crewType;
                var CrewId = record.data.crewId;
                var CrewCode = record.data.crewCode;
                var CrewDesc = record.data.crewDesc;


                var flag = true;
                var resultstore = me.resultgrid.getStore();
                for (var i = 0; i < resultstore.getCount() ; i++) {//遍历每一行
                    if (resultstore.getAt(i).data.crewType === CrewType && resultstore.getAt(i).data.crewId === CrewId) {
                        flag = false;
                        break;
                    }
                }

                if (flag === true) {
                    //插入行到返回grid
                    var r = Ext.create('_CrewResult', {
                        crewType: CrewType,
                        crewId: CrewId,
                        crewCode: CrewCode,
                        crewDesc: CrewDesc
                    });


                    var rowlength = me.resultgrid.getStore().data.length;
                    me.resultgrid.getStore().insert(rowlength, r);

                    me.resultgrid.getView().refresh();
                }

            }
        };

        //添加用户选择grid双击事件
        me.crewGrid.on('itemdblclick', function (view, record, item, index, e) {
            addItemToResultGrid(record);
        });

        me.sendSelectCrewDefault("");

        window.parent.resultvalue = "";
        window.parent.crewdesclist = "";
        window.parent.crewvaluelist = "";

        me.callParent(arguments);

    },

    //获取新建厂家资料目录表单默认参数
    sendSelectCrewDefault: function (filter) {
        var me = this;

        ////通过extjs的ajax获取操作全部名称
        //Ext.Ajax.request({
        //    url: 'WebApi/Post',
        //    method: "POST",
        //    params: {
        //        C: "AVEVA.CDMS.HXEPC_Plugins.Company", A: "GetSelectReceiveTypeList",
        //        sid: localStorage.getItem("sid"), ProjectKeyword: me.projectKeyword,
        //        Filter: filter
        //    },
        //    success: function (response, options) {
        //        me.sendGetSelectReceiveTypeList_callback(response, options);//, funCallback);
        //    },
        //    failure: function (response, options) {
        //        ////Ext.Msg.alert("错误", "连接服务器失败！<br>" + response.responseText);
        //    }
        //});
    },

});