//选择流程模板
Ext.define('Ext.ux.m.WorkFlow._winSelDefWorkFlow', {
    extend: 'Ext.container.Container',
    alias: 'widget._winSelDefWorkFlow',
    //layout: "border",
    layout: 'fit',
    resultvalue:'',
    initComponent: function () {
        var me = this;
        me.renderTo = me.el;
        var resultvalue = '';

        //定义未选择用户的model
        Ext.define("_SelDefWorkFlow", {
            extend: 'Ext.data.Model',
            //parentId用来记录父目录
            fields: [{ name: 'id', type: 'string' },
                { name: 'text', type: 'string' },
                { name: 'o_code', type: 'string' }
            ],
            proxy: {
                type: "ajax",
                //type:'scripttag',
                //url: "Doc/GetDefWorkFlowList",//调用路径：\simplecdms\controllers\projectcontroller.cs
                url: 'WebApi/Get',
                extraParams: {
                    C: "AVEVA.CDMS.WebApi.WorkFlowController", A: "GetDefWorkFlowList",
                    KeyWord: 1
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
                    exception: CDMSWap.ProxyException
                }
            }
        });

        //定义未选择用户的store
        me._seldefworkflowstore = Ext.create("Ext.data.Store", {
            batchActions: false,
            //文章的Store需要支持远程排序和搜索
            remoteFilter: true,
            remoteSort: true,
            //每50条记录为一页
            pageSize: 50,//视图路径：\simplecdms\scripts\app\view\content\view.js
            model: "_SelDefWorkFlow"

        });


        //定义未选择用户tab的view
        me.resultgrid = Ext.widget("grid", {
            region: "center",
            //height: 390,
            layout: 'fit',
            store: me._seldefworkflowstore,
            //Grid使用了复选框作为选择行的方式
            //selType: "checkboxmodel",
            selModel: { checkOnly: true, mode: "MULTI" },
            columns: [
                { text: '流程', dataIndex: 'text', width: '100%' }
            ],
            listeners: {
            }
        });

        //添加列表
        me.items = [
          Ext.widget('form', {
              layout: {
                  type: 'border', 
                  padding: 5
              },
              baseCls: 'my-panel-no-border',//隐藏边框
              //bodyPadding: 10,
              items: [me.resultgrid
			//}
              ],
                  buttons: [{
                      text: '确定',
                      handler: function () {
                          //确定按钮事件
                          var strresult;
                          var rs = me.resultgrid.getSelectionModel().getSelection();//获取选择的文档
                          if (rs) {
                              //strresult = rs[0].data.id;
                              strresult = rs[0].data.o_code;

                              window.parent.resultvalue = strresult;//functionName(strresult);
                              winSelDef.close();
                          } else { Ext.Msg.alert("错误消息", "请选择流程！"); }
                      }
                  }, {
                      text: '取消',
                      handler: function () {
                          winSelDef.close();
                      }
                  }
                  ]
          })

        ];


        me.callParent(arguments);
    }
});