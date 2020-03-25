/*批量创建文档*/
Ext.define('Ext.plug_ins.HXEPC_Plugins.Project.BatchCreateSystem', {
    extend: 'Ext.container.Container',
    alias: 'widget.BatchCreateSystem',
    //layout: "border",
    layout: 'fit',
    resultvalue: '', mainPanelId: '',
    initComponent: function () {
        var me = this;
        me.renderTo = me.el;

        me.projectKeyword = "";//记录目录Keyword
        me.createDocIndex = 0;

        me.cellEditing = new Ext.grid.plugin.CellEditing({
            clicksToEdit: 1
        });


        me.CodeEditing = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            //id:"inputtext",
            fieldLabel: "", anchor: "80%", labelWidth: 0, labelAlign: "left", width: 160,//flex: 1
            value: "",
            listeners: {
                "afterrender": function () {
                    //添加渲染完成后触发的事件，把读取剪切板功能添加到控件
                    var inputTextId = me.CodeEditing.id;
                    var oInp = document.getElementById(inputTextId);//inputtext");
                    function getClipboardText(event) {
                        var clipboardData = event.clipboardData || window.clipboardData;
                        return clipboardData.getData("text");
                    };
                    //function setClipboardText(event, value) {
                    //    var clipboardData = event.clipboardData || window.clipboardData;

                    //    if (!event.clipboardData.setData("text/plain", value)) // 赋予 text 格式的数据 
                    //    {
                    //        alert("复制失败!");
                    //    }
                    //    //return clipboardData.setData("text",text);
                    //};
                    //监听文本框粘贴事件
                    oInp.addEventListener('paste', function (event) {
                        var event = event || window.event;
                        var text = getClipboardText(event);
                        var reEnter = "\r\n";//回车符
                        var reTab = "	";//tab符
                        var arr = text.split(reEnter);

                        if (arr.length > 0) {
                            if (!/^\d+$/.test(text)) {
                                event.preventDefault();
                            }

                            //设置发文单位
                            var nodes = me.contentGrid.getSelectionModel().getSelection();//获取已选取的节点

                            var records = me.contentStore.getRange();



                            if (nodes !== null && nodes.length > 0) {
                                //器具表格添加行
                                for (var i = 0; i < arr.length ; i++) {
                                    var recIns = new contentModel({
                                        name: ""
                                    });
                                    //  me.contentStore.insert(0, recIns);
                                    me.contentStore.insert(0, recIns);
                                }

                                var reStr = "";
                                for (var i = 0; i < arr.length ; i++) {
      
                                    var arrItem = arr[i].split(reTab);
                                    //if (i === 0)
                                    //{
                                    //    setClipboardText(event, arrItem[0]);
                                    //}
                                    if (arrItem[0] != "") {
                                        var rec = me.contentStore.getAt(i);
                                        rec.set('systemEngDesc', arrItem[0]);
                                        rec.set('systemCode', arrItem[1]);
                                        rec.set('systemDesc', arrItem[2]);
                   
                                        rec.commit();
                                    }
                                }
                                me.contentGrid.getView().refresh();
                            }
                        }
                    }, false);
                }
            }
        });

        me.EnDescEditing = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            //id:"inputtext",
            fieldLabel: "", anchor: "80%", labelWidth: 0, labelAlign: "left", width: 160,//flex: 1
            value: ""
        });

        me.DescEditing = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            //id:"inputtext",
            fieldLabel: "", anchor: "80%", labelWidth: 0, labelAlign: "left", width: 160,//flex: 1
            value: ""
        });

        //定义器具的model
        Ext.define("contentModel", {
            extend: "Ext.data.Model",
            fields: ["id", "systemCode", "systemDesc", "systemEngDesc", "tempDef"],
            url: "_blank",
        });

        me.contentStore = Ext.create('Ext.data.Store', {
            autoDestroy: true,
            model: contentModel,
            sorters: [{
                property: 'id',
                direction: 'ASC'
            }]
        });

        me.contentGrid = Ext.create('Ext.grid.Panel', {
            title: '输入文档的代码和描述',
            store: me.contentStore,
            columns: [
                {
                    header: '英文名', dataIndex: 'systemEngDesc',  width: 321,
                    editor: me.CodeEditing
                },
                {
                    header: '代 码', dataIndex: 'systemCode', width: 120,
                    editor: me.EnDescEditing
                },
                {
                    header: '描 述', dataIndex: 'systemDesc',  width: 321,
                    editor: me.DescEditing
                }
            ],
            stripeRows: true, //斑马线效果  
            plugins: [
                me.cellEditing
        //         Ext.create('Ext.grid.plugin.CellEditing', {
        //             clicksToEdit: 1//设置单击单元格编辑  
        //})
            ],
            height: 510,
            width: 400,
            renderTo: Ext.getBody(),
            selModel: {
                selType: 'cellmodel'
            }
        });

        //器具表格添加行
        for (var i = 0; i < 20; i++) {
            var rec = new contentModel({
                name: ""
            });
            me.contentStore.insert(0, rec);
        }

        //添加列表
        me.items = [
          Ext.widget('form', {
              baseCls: 'my-panel-no-border',//隐藏边框
              layout: {
                  type: 'vbox',
                  align: 'stretch',
                  pack: 'start'
              },
              items: [me.contentGrid,
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
                          text: "确定", width: 60, margins: "18 5 18 5",
                          listeners: {
                              "click": function (btn, e, eOpts) {//添加点击按钮事件
                                  me.send_create_Doc();
                              }
                          }
                      },
                      {
                          xtype: "button",
                          text: "取消", width: 60, margins: "18 5 18 5",
                          listeners: {
                              "click": function (btn, e, eOpts) {//添加点击按钮事件
                                  //Ext.Msg.alert("您展开了目录树节点！！！", "您展开了目录树节点！节点ID:" + me.tempDefnId);
                                  winBatchCreateSystem.close();
                              }
                          }
                      }]
              }
              ]
          })];



        me.callParent(arguments);

    },

    send_create_Doc: function () {
        var me = this;
        var batSend = false;//是否批量把文档名和属性上传服务端
        if (batSend === true) {
            me.batch_send_create_Doc2();
        } else {
            //每次上传一个文档名和属性
            //获取表格数据，转换成JSON字符串
            var datar = new Array();
            var contentJson = "";
            var docAttr = new Array();
            me.createDocIndex = 0;
            me.loop_create_system(0);//循环创建文档


            //Ext.Msg.alert("您粘贴了文本！！！", "渲染完成！" + contentJson);
        }
    },

    //循环创建文档
    loop_create_system: function (docIndex) {
        var me = this;
        var records = me.contentStore.getRange();
        if (docIndex >= records.length) {
            winBatchCreateSystem.close();
        } else {
            if (records[docIndex].data.systemCode === "") {
                docIndex = docIndex + 1;
                //循环创建文档
                me.loop_create_system(docIndex);
            } else {
                systemAttr =
                [{ name: 'systemCode', value: records[docIndex].data.systemCode },
                    { name: 'systemDesc', value: records[docIndex].data.systemDesc },
                    { name: 'systemEngDesc', value: records[docIndex].data.systemEngDesc },
                    { name: 'tempDefnId', value: "" }
                ];
                var systemAttrJson = Ext.JSON.encode(systemAttr);

                Ext.Ajax.request({

                    url: 'WebApi/Post',
                    method: "POST",
                    params: {
                        C: "AVEVA.CDMS.HXEPC_Plugins.HXProject", A: "CreateSystem",
                        sid: localStorage.getItem("sid"), projectKeyword: me.projectKeyword, systemAttrJson: systemAttrJson,
                    },
                    success: function (response, options) {
                        var res = Ext.JSON.decode(response.responseText, true);
                        var state = res.success;
                        if (state === false) {
                            var errmsg = res.msg;
                            //Ext.Msg.alert("错误信息", errmsg);
                        }
                        else {
                            ////更新DOC到表格
                            //var grid = Ext.getCmp(me.mainPanelId).up('_mainSourceView').down('_mainDocGrid').down("gridpanel");
                            //grid.store.add({
                            //    Keyword: res.data[0].Keyword,
                            //    O_itemno: res.data[0].O_itemno,
                            //    Title: res.data[0].Title,
                            //    O_size: res.data[0].O_size,
                            //    O_filename: res.data[0].O_filename,
                            //    O_dmsstatus_DESC: res.data[0].O_dmsstatus_DESC,
                            //    O_version: res.data[0].O_version,
                            //    Creater: res.data[0].Creater,
                            //    O_credatetime: res.data[0].O_credatetime,
                            //    Updater: res.data[0].Updater,
                            //    O_updatetime: res.data[0].O_updatetime,
                            //    O_outpath: res.data[0].O_outpath,
                            //    O_flocktime: res.data[0].O_flocktime,
                            //    O_conode: res.data[0].O_conode,
                            //    IsShort: res.data[0].IsShort,
                            //    WriteRight: res.data[0].WriteRight

                            //});
                        }
                        docIndex = docIndex + 1;
                        //循环创建文档
                        me.loop_create_system(docIndex);
                    }
                })
            }
        }
    },

    //批量创建文档
    batch_send_create_Doc: function () {
        var me = this;
        //获取表格数据，转换成JSON字符串
        var datar = new Array();
        var contentJson = "";
        var records = me.contentStore.getRange();
        for (var i = 0; i < records.length; i++) {
            if (records[i].data.systemCode != "") {
                datar.push(records[i].data);
            }
        }
        contentJson = Ext.JSON.encode(datar);

        Ext.Ajax.request({

            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.HXEPC_Plugins.HXProject", A: "BatchCreateSystem",
                sid: localStorage.getItem("sid"), projectKeyword: me.projectKeyword, contentJson: contentJson,
            },
            success: function (response, options) {
                me.create_Doc_callback(response, me.projectKeyword, true);
            }
        })
        //Ext.Msg.alert("您粘贴了文本！！！", "渲染完成！" + contentJson);
    },

    create_Doc_callback: function (response, parentKeyword, closeWin) {
        var me = this;
        var res = Ext.JSON.decode(response.responseText, true);
        var state = res.success;
        if (state === false) {
            var errmsg = res.msg;
            Ext.Msg.alert("错误信息", errmsg);
        }
        else {
            //var grid = Ext.getCmp('_DocGrid');
            var grid = Ext.getCmp(me.mainPanelId).up('_mainSourceView').down('_mainDocGrid').down("gridpanel");
            grid.store.load();
            if (closeWin)
                winBatchCreateSystem.close();
        }
    }
});

Ext.onReady(function () {
    //Ext.Msg.alert("您粘贴了文本！！！", "渲染完成！");

});