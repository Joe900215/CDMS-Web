Ext.define('Ext.plug_ins.WTMS_Plugins.Dist.SelectUnit', {
    extend: 'Ext.container.Container',
    alias: 'widget.SelectUnit',
    //layout: "border",
    layout: 'fit',
    //resultvalue: '', mainPanelId: '', 
    projectKeyword: '',
    initComponent: function () {
        var me = this;


        Ext.define('UnitModel', {
            extend: 'Ext.data.Model',
            idProperty: 'unitId',
            fields: [

                { name: 'unitType', type: 'string' },
                { name: 'unitId', type: 'string' },
                { name: 'unitCode', type: 'string' },
                { name: 'unitDesc', type: 'string' }
            ]
        });

        me.unitdata = [];

        me.unitstore = Ext.create('Ext.data.Store', {
            model: 'UnitModel',
            data: me.unitdata,
            sorters: { property: 'due', direction: 'ASC' },
            groupField: 'unitType'
        });

        ////定义搜索框
        //me.searchTrigger = {
        //    xtype: 'triggerfield',
        //    onClearValue: function () {
        //        //this.setValue('');
        //    },
        //    onTrigger1Click: function () {
        //        //if (this.hasSearch) {
        //        this.setValue('');

        //        me.sendSelectUnitDefault("");
        //        //}
        //    },
        //    onTrigger2Click: function () {
        //        var value = this.getValue();

        //        if (value.length > 0) {

        //            //var strSQL = " (o_itemname " + "LIKE" + " '%" + value + "%' or o_itemdesc " + "LIKE" + " '%" + value + "%' ) ";

        //            //var filterObj = [
        //            //    { name: 'o_itemname', value: strSQL }
        //            //];
        //            //me.selectDocList(filterObj);

        //            me.sendSelectUnitDefault(value);
        //            this.hasSearch = false;
        //        }
        //    },
        //    trigger1Cls: 'x-form-clear-trigger',
        //    trigger2Cls: 'x-form-search-trigger',
        //    store: 'Person4Select',
        //    paramName: 'query',
        //    hasSearch: 'false',
        //    width: '100%',//220,
        //    margins: '1 1 1 0',
        //    emptyText: '查找参建单位或项目部门',
        //    //fieldLabel: '指标名称',
        //    labelAlign: 'right',
        //    labelWidth: 55,
        //    selectOnFocus: true,
        //    listeners: {
        //        //afterrender: {
        //        //    fn: me.onTriggerfieldAfterRender1111,
        //        //    scope: me
        //        //},
        //        //specialkey: {
        //        //    fn: me.onTriggerfieldSpecialkey1111,
        //        //    scope: me
        //        //}
        //    }
        //};

        //定义文本输入框
        me.groupinputtext = Ext.widget('textfield', {
            name: "Title", width: "100%", enableKeyEvents: true,
            fieldLabel: "搜索", anchor: "80%", labelWidth: 30, labelAlign: "left", margin: '0 0 5 5',
            listeners: {
                //这里不能用change函数，因为刷新GRID的时候有问题
                //change: function (field, newValue, oldValue) {
                keyup: function (src, evt) {
                    var value = src.getValue();
                    me.sendSelectUnitDefault(value);
                    //me._groupselstore.proxy.extraParams.Filter = src.getValue();//把参数传给C#MVC,路径：\simplecdms\controllers\Doccontroller.cs 下的 GetWorkFlow()
                    //me._groupselstore.proxy.extraParams.sid = localStorage.getItem("sid");
                    //me._groupselstore.currentPage = 1;
                    //me._groupselstore.load();

                }
            }
        })

        me.unitGrid = Ext.create('Ext.grid.Panel', {
            width: 800,
            height: 450,
            //frame: true,
           
            //title: '',
            //iconCls: 'icon-grid',
            renderTo: document.body,
            store: me.unitstore,
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
                dataIndex: 'unitCode'
            },{
                text: '名称',
                flex: 1,
                tdCls: 'task',
                sortable: true,
                dataIndex: 'unitDesc',
                hideable: false
            }, {
                header: 'Project',
                width: 180,
                sortable: true,
                dataIndex: 'unitType'
            }
            ],
            //增加提示
            afterRender: Ext.Function.createSequence(Ext.grid.GridPanel.prototype.afterRender,
                function () {
                    /* 默认显示提示
                    if (!this.cellTip) {
                        return;
                    }*/

                    var view = this.getView();

                    this.tip = new Ext.ToolTip({
                        target: view.el,
                        delegate: '.x-grid-cell-inner',
                        trackMouse: true,
                        renderTo: Ext.getBody(),
                        listeners: {
                            beforeshow: function updateTipBody(tip) {
                                //取cell的值
                                //fireFox  tip.triggerElement.textContent
                                //IE  tip.triggerElement.innerText 
                                var tipText = (tip.triggerElement.innerText || tip.triggerElement.textContent)+"  【双击选中】" ;
                                if (Ext.isEmpty(tipText) || Ext.isEmpty(tipText.trim())) {
                                    return false;
                                }

                                tip.update(tipText);
                            }
                        }
                    });
                })
        });


        //定义已选择用户的model
        Ext.define("_UnitResult", {
            extend: "Ext.data.Model",
            fields: ["unitType","unitId", "unitCode","unitDesc"],
            url: "_blank",
        });

        //定义已选择用户的store
        me._unitresultstore = Ext.create("Ext.data.Store", {
            batchActions: false,
            //文章的Store需要支持远程排序和搜索
            remoteFilter: true,
            remoteSort: true,
            //每50条记录为一页
            pageSize: 50,//视图路径：\simplecdms\scripts\app\view\content\view.js
            model: "_UnitResult"

        });

        //定义已选择用户的view
        me.resultgrid = Ext.widget("grid", {
            //region: "center",
            //layout: "hbox",
            height: '100%',
            //width: "100%",
            cellTip: true,
            store: me._unitresultstore,
            columns: [
                { text: '代码', dataIndex: 'unitCode', width: 120 },
                { text: '名称', dataIndex: 'unitDesc', flex: 1 }//width: '100%' }
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
            },
            //增加提示
            afterRender: Ext.Function.createSequence(Ext.grid.GridPanel.prototype.afterRender,
                function () {
                    /* 默认显示提示
                    if (!this.cellTip) {
                        return;
                    }*/

                    var view = this.getView();

                    this.tip = new Ext.ToolTip({
                        target: view.el,
                        delegate: '.x-grid-cell-inner',
                        trackMouse: true,
                        renderTo: Ext.getBody(),
                        listeners: {
                            beforeshow: function updateTipBody(tip) {
                                //取cell的值
                                //fireFox  tip.triggerElement.textContent
                                //IE  tip.triggerElement.innerText 
                                var tipText = (tip.triggerElement.innerText || tip.triggerElement.textContent)+"  【双击取消选中】" ;
                                if (Ext.isEmpty(tipText) || Ext.isEmpty(tipText.trim())) {
                                    return false;
                                }

                                tip.update(tipText);
                            }
                        }
                    });
                }),flex: 1
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
                             me.unitGrid
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
                      var strresult, strname;
                      var resultstore = me.resultgrid.getStore();
                      var userArray = new Array();
                      for (var i = 0; i < resultstore.getCount() ; i++) {
                          //strresult = strresult + ";" + resultstore.getAt(i).data.id + "," + resultstore.getAt(i).data.text; //遍历每一行
                          if (i > 0) {
                              strresult = strresult + "," + resultstore.getAt(i).data.unitCode;
                              strname = strname + ";" + resultstore.getAt(i).data.unitDesc;
                              strvalue = strvalue + "," + resultstore.getAt(i).data.unitType + resultstore.getAt(i).data.unitId;
                          } else {
                              strresult = resultstore.getAt(i).data.unitCode;
                              strname = resultstore.getAt(i).data.unitDesc;
                              strvalue = resultstore.getAt(i).data.unitType + resultstore.getAt(i).data.unitId;
                              strtype = resultstore.getAt(i).data.unitType;
                          }

                      }

                      window.parent.resultvalue = strresult;//functionName(strresult);
                      window.parent.unitdesclist = strname;//functionName(strresult);
                      window.parent.unitvaluelist = strvalue;//functionName(strresult);
                      window.parent.unitType = strtype;
                      winSelectUnit.close();

                  }
              }
          }, {
              xtype: "button",
              text: "取消", width: 60, margins: "10 5 10 5",
              listeners: {
                  "click": function (btn, e, eOpts) {//添加点击按钮事件

                      winSelectUnit.close();

                  }
              }
          }
        ]
    }]
          })];

        function addItemToResultGrid(record) {
            //双击时，插入记录到已选择grid
            if (typeof (record.raw) != 'undefined' || typeof (record.data) != 'undefined') {

                var UnitType=record.data.unitType;
                var UnitId=record.data.unitId;
                var UnitCode=record.data.unitCode;
                var UnitDesc=record.data.unitDesc;


                var flag = true;
                var resultstore = me.resultgrid.getStore();
                for (var i = 0; i < resultstore.getCount() ; i++) {//遍历每一行
                    if (resultstore.getAt(i).data.unitType === UnitType && resultstore.getAt(i).data.unitId === UnitId) {
                        flag = false;
                        break;
                    }
                }

                if (flag === true) {
                    //插入行到返回grid
                    var r = Ext.create('_UnitResult', {
                        unitType: UnitType,
                        unitId: UnitId,
                        unitCode: UnitCode,
                        unitDesc: UnitDesc
                    });


                    var rowlength = me.resultgrid.getStore().data.length;
                    me.resultgrid.getStore().insert(rowlength, r);

                    me.resultgrid.getView().refresh();
                }

            }
        };

        //添加用户选择grid双击事件
        me.unitGrid.on('itemdblclick', function (view, record, item, index, e) {
            addItemToResultGrid(record);
        });

        me.sendSelectUnitDefault("");

        window.parent.resultvalue = "";
        window.parent.unitdesclist = "";
        window.parent.unitvaluelist = "";

        me.callParent(arguments);

    },

    //获取新建厂家资料目录表单默认参数
    sendSelectUnitDefault: function (filter) {
        var me = this;

        //通过extjs的ajax获取操作全部名称
        Ext.Ajax.request({
            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.WTMS_Plugins.Company", A: "GetSelectUnitList",
                sid: localStorage.getItem("sid"), ProjectKeyword: me.projectKeyword,
                Filter: filter
            },
            success: function (response, options) {
                me.sendGetSelectUnitList_callback(response, options);//, funCallback);
            },
            failure: function (response, options) {
                ////Ext.Msg.alert("错误", "连接服务器失败！<br>" + response.responseText);
            }
        });
    },

    //处置获取新建厂家资料目录表单默认参数的返回
    sendGetSelectUnitList_callback: function (response, options) {
        var me = this;



        //获取数据后，更新窗口
        var res = Ext.JSON.decode(response.responseText, true);
        var state = res.success;
        if (state === true) {
            //var recod = eval(res.data[0]);
            
            //var companyList = eval(recod.CompanyList);
            me.unitGrid.store.removeAll();

            var companyList = eval(res.data);

            for (var itemKey in companyList) {
                //me.projectKeyword = companyList[itemKey].companyCode;
                if (companyList[itemKey].unitCode != undefined) {

                    //插入行到文件selectUserGrid
                    var r = Ext.create('UnitModel', {

                        unitType: companyList[itemKey].unitType,
                        unitId: companyList[itemKey].unitId,
                            unitCode: companyList[itemKey].unitCode,
                        unitDesc: companyList[itemKey].unitDesc
                    });

                    var rowlength = me.unitGrid.getStore().data.length;
                    me.unitGrid.getStore().insert(rowlength, r);
                }
            }
            me.unitGrid.getView().refresh();
        }
    }

});