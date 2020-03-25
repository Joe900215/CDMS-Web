//获取目录模板//新建目录
Ext.define('Ext.plug_ins.SysPlugins._GetProjDef', {
    extend: 'Ext.container.Container',
    alias: 'widget._GetProjDef',
    //layout: "border",
    layout: 'fit',
    resultvalue: '',//mainPanelId:'',
    initComponent: function () {
        var me = this;
        me.winAction = "";
        //定义组织机构树的model
        me.grouppickermodel = Ext.define("_GroupPicker", {
            extend: 'Ext.data.Model',
            //parentId用来记录父目录
            fields: [{ name: 'id', type: 'string' },
                { name: 'text', type: 'string' },
                {name:'type',type:'string'}
            ],

            idProperty: "id"
        });


        me.grouppickerstore = Ext.create("Ext.data.TreeStore", {

            batchActions: false,
            remoteFilter: false,
            remoteSort: false,
            model: me.grouppickermodel,
            root: { id: "Root", text: "所有模板", expanded: true },
            ////代理定义
            proxy: {
                type: 'ajax',
                //代理的API定义了操作的提交路径

                url: 'WebApi/Get',
                extraParams: {
                    C: "AVEVA.CDMS.WebApi.ProjectController", A: "GetTempDefList",
                    attrType: "Project", sid: localStorage.getItem("sid")
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
                    //这里为了兼容IE修改成这样，原来是 exception: CDMSWeb.ProxyException 
                    //exception: function () { CDMSWeb.ProxyException }
                    exception: CDMSWeb.ProxyException
                }

            },
        });

        //创建树控件  
        me.grouppicker = Ext.create('Ext.tree.Panel', {
            //title: '选择组织机构',
            //collapsible: true,
            rootVisible: true,
            store: me.grouppickerstore, split: true,
            height: 430,
            width: "100%", minWidth: 100,
            //root: { id: "Root", text: "所有模板", expanded: true },
            viewConfig: {
                stripeRows: true,
                listeners: {
                    itemcontextmenu: function (view, rec, node, index, e) {
                        //e.stopEvent();
                        //contextMenu.showAt(e.getXY());
                        //return false;
                    }

                }
            },
            listeners: {
                itemclick: function (picker, record) {
                },
                itemdblclick: function (view, record, item, index, e, eOpts) {
                   //var me = this;
                   me.sele_tempdefn();
                    //if ((record.data.type === "Doc" && (me.winAction === "ModiDoc" || me.winAction === "CreateDoc")) || (record.data.type === "Project" && (me.winAction === "ModiProject" || me.winAction === "CreateProject"))) {
                    //    window.parent.resultvalue = record.data.id;
                    //    window.parent.resultdesc = record.data.text;
                    //    //Ext.Msg.alert("您展开了目录树节点！！！", "正确选择模板！");
                    //    winGetDef.close();
                    //}
                    //else {
                    //    if (record.data.type === "Doc")
                    //        Ext.Msg.alert("错误消息！", "请选择目录模板！");
                    //    else if (record.data.type === "Project")
                    //        Ext.Msg.alert("错误消息！", "请选择文档模板！");
                    //}
                    
                    
                }
            }


            //selModel: {
            //    mode: me.multiSelect ? 'SIMPLE' : 'SINGLE'
            //},
            //   floating: true,
            //hidden: true,
            //   focusOnToFront: false
        });



        //添加列表
        me.items = [
          Ext.widget('form', {
              layout: "form",
              items: [{
                  xtype: "fieldset",
                  title: "双击选择模板",
                  layout: {
                      type: 'vbox',
                      pack: 'start',
                      align: 'stretch'
                  },
                  items: [
                        me.grouppicker],flex:1
              },
              {
                  xtype: "panel",
                  layout: "hbox",
                  baseCls: 'my-panel-no-border',//隐藏边框
                  //align: 'right',
                  //pack: 'end',//组件在容器右边
                  items: [{
                      flex: 1, baseCls: 'my-panel-no-border'//隐藏边框
                  },
                      {
                          xtype: "button",
                          text: "确定", width: 60, margins: "5"
                          ,
                          listeners: {
                              "click": function (btn, e, eOpts) {//添加点击按钮事件
                                  me.sele_tempdefn();
                                  //winGetDef.close();
                              }
                          }
                      },
                      {
                          xtype: "button",
                          text: "取消", width: 60, margins: "5",
                          listeners: {
                              "click": function (btn, e, eOpts) {//添加点击按钮事件
                                  winGetDef.close();
                              }
                          }
                      }
                  ]
              }]
          })];


        me.callParent(arguments);
    },

    sele_tempdefn: function () {
        var me = this; 
        var nodes = me.grouppicker.getSelectionModel().getSelection();
        if (nodes !== null && nodes.length > 0) {
            var record = nodes[0];
            if ((record.data.type === "Doc" && (me.winAction === "ModiDoc" || me.winAction === "CreateDoc")) || (record.data.type === "Project" && (me.winAction === "ModiProject" || me.winAction === "CreateProject"))) {
                window.parent.resultvalue = record.data.id;
                window.parent.resultdesc = record.data.text;
                //Ext.Msg.alert("您展开了目录树节点！！！", "正确选择模板！");
                winGetDef.close();
            }
            else {
                if (record.data.type === "Doc")
                    Ext.Msg.alert("错误消息！", "请选择目录模板！");
                else if (record.data.type === "Project")
                    Ext.Msg.alert("错误消息！", "请选择文档模板！");
            }
        }
    }



});

