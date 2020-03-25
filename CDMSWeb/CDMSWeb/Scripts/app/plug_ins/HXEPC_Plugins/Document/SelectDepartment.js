Ext.define('Ext.plug_ins.HXEPC_Plugins.Document.SelectDepartment', {
    extend: 'Ext.container.Container',
    alias: 'widget.SelectDepartment',
    //layout: "border",
    layout: 'fit',
    //resultvalue: '', mainPanelId: '', 
    projectKeyword: '',
    initComponent: function () {
        var me = this;


        Ext.define('DepartmentModel', {
            extend: 'Ext.data.Model',
            idProperty: 'departmentId',
            fields: [

                { name: 'departmentType', type: 'string' },
                { name: 'departmentId', type: 'string' },
                { name: 'departmentCode', type: 'string' },
                { name: 'departmentDesc', type: 'string' }
            ],
            proxy: {
                type: "ajax",
                url: 'WebApi/Get',
                extraParams: {
                    C: "AVEVA.CDMS.HXEPC_Plugins.Company", A: "GetSelectDepartmentList",
                    ProjectKeyword: me.projectKeyword, sid: localStorage.getItem("sid")
                },
                //    params: {
                //        C: "AVEVA.CDMS.HXEPC_Plugins.Company", A: "GetSelectDepartmentList",
                //        sid: localStorage.getItem("sid"), ProjectKeyword: me.projectKeyword,
                //        Filter: filter
                //    },
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

        me.departmentdata = [];


        me.departmentstore = Ext.create('Ext.data.Store', {
            batchActions: false,
            //文章的Store需要支持远程排序和搜索
            remoteFilter: true,
            remoteSort: true,
            //每50条记录为一页
            pageSize: 50,//视图路径：\simplecdms\scripts\app\view\content\view.js

            model: 'DepartmentModel',
            data: me.departmentdata,
            sorters: { property: 'due', direction: 'ASC' },
            groupField: 'departmentType'
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
                    //me.sendSelectDepartmentDefault(value);

                    me.departmentstore.proxy.extraParams.Filter = src.getValue();//把参数传给C#MVC,路径：\simplecdms\controllers\Doccontroller.cs 下的 GetWorkFlow()
                    me.departmentstore.proxy.extraParams.sid = localStorage.getItem("sid");
                    me.departmentstore.proxy.extraParams.Group = "";//重置机构组
                    me.departmentstore.currentPage = 1;
                    me.departmentstore.load();

                }
            }
        })

        me.departmentGrid = Ext.create('Ext.grid.Panel', {
            width: 800,
            height: 450,
            frame: true,
            bbar: new Ext.PagingToolbar({
                //pageSize: 5,//pageSize,  
                store: me.departmentstore,
                displayInfo: true,
                displayMsg: '当前显示{0} - {1}条，共{2}条数据',
                emptyMsg: "没有记录"
            }),
            //renderTo: document.body,
            store: me.departmentstore,
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
                dataIndex: 'departmentCode'
            },{
                text: '名称',
                flex: 1,
                tdCls: 'task',
                sortable: true,
                dataIndex: 'departmentDesc',
                hideable: false
            }, {
                header: 'Project',
                width: 180,
                sortable: true,
                dataIndex: 'departmentType'
            }
            ]
        });

        //定义已选择用户的model
        Ext.define("_DepartmentResult", {
            extend: "Ext.data.Model",
            fields: ["departmentType","departmentId", "departmentCode","departmentDesc"],
            url: "_blank",
        });

        //定义已选择用户的store
        me._departmentresultstore = Ext.create("Ext.data.Store", {
            batchActions: false,
            //文章的Store需要支持远程排序和搜索
            remoteFilter: true,
            remoteSort: true,
            //每50条记录为一页
            pageSize: 50,//视图路径：\simplecdms\scripts\app\view\content\view.js
            model: "_DepartmentResult"

        });

        //定义已选择用户的view
        me.resultgrid = Ext.widget("grid", {
            //region: "center",
            //layout: "hbox",
            height: '100%',
            //width: "100%",
            cellTip: true,
            store: me._departmentresultstore,
            columns: [
                { text: '代码', dataIndex: 'departmentCode', width: 120 },
                { text: '名称', dataIndex: 'departmentDesc', flex: 1 }//width: '100%' }
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
                          },width:'50%',
                          items: [

                              me.groupinputtext,
                             me.departmentGrid
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
              },{
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
                      var strresult, strname, strtype, strvalue;
                      var resultstore = me.resultgrid.getStore();
                      var userArray = new Array();
                      for (var i = 0; i < resultstore.getCount() ; i++) {
                          //strresult = strresult + ";" + resultstore.getAt(i).data.id + "," + resultstore.getAt(i).data.text; //遍历每一行
                          if (i > 0) {
                              strresult = strresult + "," + resultstore.getAt(i).data.departmentCode;
                              strname = strname + ";" + resultstore.getAt(i).data.departmentDesc;
                              strvalue = strvalue + "," + resultstore.getAt(i).data.departmentType + resultstore.getAt(i).data.departmentId;
                              strtype = strtype + "," + resultstore.getAt(i).data.departmentType;

                          } else {
                              strresult = resultstore.getAt(i).data.departmentCode;
                              strname = resultstore.getAt(i).data.departmentDesc;
                              strvalue = resultstore.getAt(i).data.departmentType + resultstore.getAt(i).data.departmentId;
                              strtype = resultstore.getAt(i).data.departmentType;
                          }

                      }

                      window.parent.resultvalue = strresult;//functionName(strresult);
                      window.parent.departmentdesclist = strname;//functionName(strresult);
                      window.parent.departmentvaluelist = strvalue;//functionName(strresult);
                      window.parent.resulttype = strtype;
                      winSelectDepartment.close();

                  }
              }
          }, {
              xtype: "button",
              text: "取消", width: 60, margins: "10 5 10 5",
              listeners: {
                  "click": function (btn, e, eOpts) {//添加点击按钮事件

                      winSelectDepartment.close();

                  }
              }
          }
        ]
    }]
          })];

        function addItemToResultGrid(record) {
            //双击时，插入记录到已选择grid
            if (typeof (record.raw) != 'undefined' || typeof (record.data) != 'undefined') {

                var DepartmentType=record.data.departmentType;
                var DepartmentId=record.data.departmentId;
                var DepartmentCode=record.data.departmentCode;
                var DepartmentDesc=record.data.departmentDesc;


                var flag = true;
                var resultstore = me.resultgrid.getStore();
                for (var i = 0; i < resultstore.getCount() ; i++) {//遍历每一行
                    if (resultstore.getAt(i).data.departmentType === DepartmentType && resultstore.getAt(i).data.departmentId === DepartmentId) {
                        flag = false;
                        break;
                    }
                }

                if (flag === true) {
                    //插入行到返回grid
                    var r = Ext.create('_DepartmentResult', {
                        departmentType: DepartmentType,
                        departmentId: DepartmentId,
                        departmentCode: DepartmentCode,
                        departmentDesc: DepartmentDesc
                    });


                    var rowlength = me.resultgrid.getStore().data.length;
                    me.resultgrid.getStore().insert(rowlength, r);

                    me.resultgrid.getView().refresh();
                }

            }
        };

        //添加用户选择grid双击事件
        me.departmentGrid.on('itemdblclick', function (view, record, item, index, e) {
            addItemToResultGrid(record);
        });

        me.sendSelectDepartmentDefault("");

        window.parent.resultvalue = "";
        window.parent.departmentdesclist = "";
        window.parent.departmentvaluelist = "";

        me.callParent(arguments);

    },

    //获取新建厂家资料目录表单默认参数
    sendSelectDepartmentDefault: function (filter) {
        var me = this;

        ////通过extjs的ajax获取操作全部名称
        //Ext.Ajax.request({
        //    url: 'WebApi/Post',
        //    method: "POST",
        //    params: {
        //        C: "AVEVA.CDMS.HXEPC_Plugins.Company", A: "GetSelectDepartmentList",
        //        sid: localStorage.getItem("sid"), ProjectKeyword: me.projectKeyword,
        //        Filter: filter
        //    },
        //    success: function (response, options) {
        //        me.sendGetSelectDepartmentList_callback(response, options);//, funCallback);
        //    },
        //    failure: function (response, options) {
        //        ////Ext.Msg.alert("错误", "连接服务器失败！<br>" + response.responseText);
        //    }
        //});
    },

    ////处置获取新建厂家资料目录表单默认参数的返回
    //sendGetSelectDepartmentList_callback: function (response, options) {
    //    var me = this;



    //    //获取数据后，更新窗口
    //    var res = Ext.JSON.decode(response.responseText, true);
    //    var state = res.success;
    //    if (state === true) {
    //        //var recod = eval(res.data[0]);
            
    //        //var companyList = eval(recod.CompanyList);
    //        me.departmentGrid.store.removeAll();

    //        var companyList = eval(res.data);

    //        for (var itemKey in companyList) {
    //            //me.projectKeyword = companyList[itemKey].companyCode;
    //            if (companyList[itemKey].departmentCode != undefined) {

    //                //插入行到文件selectUserGrid
    //                var r = Ext.create('DepartmentModel', {

    //                    departmentType: companyList[itemKey].departmentType,
    //                    departmentId: companyList[itemKey].departmentId,
    //                        departmentCode: companyList[itemKey].departmentCode,
    //                    departmentDesc: companyList[itemKey].departmentDesc
    //                });

    //                var rowlength = me.departmentGrid.getStore().data.length;
    //                me.departmentGrid.getStore().insert(rowlength, r);
    //            }
    //        }
    //        me.departmentGrid.getView().refresh();
    //    }
    //}

});