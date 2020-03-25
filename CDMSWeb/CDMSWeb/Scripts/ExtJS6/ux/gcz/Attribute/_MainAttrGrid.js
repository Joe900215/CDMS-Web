/*定义属性TAB里面的表格，此表格目录属性，目录附加属性，文档属性，文档附加属性共同使用*/
Ext.define('Ext.ux.gcz.Attribute._MainAttrGrid', {
    //extend: 'Ext.container.Container',
    extend: 'Ext.panel.Panel',
    alias: 'widget._mainAttrGrid', // 此类的xtype类型为buttontransparent  
    //activeTab: 0, region: "east", width: "25%", minWidth: 100, split: true, collapsible: true,
    layout: 'fit',
    initComponent: function () {
        var me = this;
        me.renderTo = me.el;
        //me.renderTo = Ext.getBody();//me.el;

        me.hasDocEditRight = "";//编辑文档属性权限
        me.hasDFWriteRight = "";
        me.hasProjectEditRight = "";//编辑目录属性权限

        //ajax请求索引
        me._dcGetProjectBaseAttr = "";

        //定义DOC属性store
        me._MainAttrStore = Ext.create("Ext.data.Store", {
            model: 'CDMSWeb.model.Doc',//模型路径：\simplecdms\scripts\app\model\Property.js
            batchActions: false,
            //文章的Store需要支持远程排序和搜索
            remoteFilter: true,
            remoteSort: true,
            //每50条记录为一页
            pageSize: 50,//视图路径：\simplecdms\scripts\app\view\content\view.js
            proxy: {
                type: "ajax",
                // method: "POST",
                url: 'WebApi/Get',
                extraParams: {
                    C: "AVEVA.CDMS.WebApi.DocController", A: "GetDocAttrDataByKeyword",
                    AttrType: 2, ObjType: "Doc"
                },
                reader: {
                    type: 'json',
                    totalProperty: 'total',
                    root: "data",
                    messageProperty: "Msg",
                    resdc:"resdc"

                },
                writer: {
                    type: "json",
                    encode: true,
                    root: "data",
                    allowSingle: false
                }
            }
        });

        me.DocAttrComboData = [];//附加属性combo

        //添加DOC附加属性编辑combo控件

        //Ext.regModel("DocAttrComboModel", {
            Ext.define('DocAttrComboModel', {
                extend : 'Ext.data.Model',
            fields: [
            { name: 'text', type: 'string' },
            { name: 'value', type: 'string' }
            ]
        })

        me.DocAttrComboProxy = Ext.create("Ext.data.proxy.Memory", {
            data: me.DocAttrComboData,
            model: "DocAttrComboModel"
        });

        me.DocAttrComboStore = Ext.create("Ext.data.Store", {
            model: DocAttrComboModel,
            proxy: me.DocAttrComboProxy,
            autoLoad: true,
            triggerAction: "all",
            valueField: 'value',
            displayField: 'text',
            mode: 'local'
        });

        me.DocAttrDataColumn = Ext.create("Ext.grid.column.Column", {
            text: '属性值', dataIndex: 'AttrValue', flex: 1, editor: new Ext.form.field.Text(),
            renderer: function (value, metaData, record, rowIdx, colIdx, store) {
                if (record.get('DataType') === 5) {//如果是日期类型
                    //return Ext.util.Format.date(value, "Y.m.d");
                    //return Ext.util.Format.dateRenderer(value , "Y.m.d");
                    return value;
                } else { return value; }

            }
        });



        //定义目录及文档属性共用的表格
        me.MainAttrGrid = Ext.widget("grid", {
            //title: '文档_附加属性',
            //xtype: 'grid',
            layout: 'fit',
            // hidden: true,
            store: me._MainAttrStore,
            saveDelay: 0,

            //Grid使用了复选框作为选择行的方式
            columns: [
                { text: '属性名', dataIndex: 'AttrName', width: 100 },
                me.DocAttrDataColumn,//属性值
                       {//添加图标
                           menuDisabled: true,
                           sortable: false,
                           xtype: 'actioncolumn',
                           enableColumnResize: false,
                           width: 38,

                           items: [{
                               getClass: function (v, metaData, record) {
                                   var hasDFWriteRight = "False";
                                   var hasDocEditRight = "False";
                                   var hasProjectEditRight = "False";

                                   //遍历获取是否有上传文档权限和修改文档属性权限
                                   var records = record.store;
                                   for (var i = 0; i < records.totalCount ; i++) {
                                       rec = records.data.items[i];
                                       attrCode = rec.data.AttrCode;

                                       //上传文档权限
                                       if (attrCode === "hasDFWriteRight") {
                                           hasDFWriteRight = rec.data.AttrValue;
                                       }
                                           //修改文档属性权限
                                       else if (attrCode === "hasEditRight") {
                                           hasDocEditRight = rec.data.AttrValue;
                                       }
                                           //修改文档属性权限
                                       else if (attrCode === "hasProjectEditRight") {
                                           hasProjectEditRight = rec.data.AttrValue;
                                       }
                                   }

                                   //基本属性里用到这两个权限
                                   if (record.get('AttrName') === "文件名" && hasDFWriteRight === "True") {
                                       return 'file-upload';
                                   } else if (record.get('AttrName') === "模板" && (hasDocEditRight === "True" || hasProjectEditRight === "True")) {
                                       return 'file-modiattr';
                                   }
                                   //附加属性里用到这个权限
                                   if (record.get('TempAttrType') === 4 && record.get('CanEdit') === "True") {//用户类型
                                       return 'user-blue-16';
                                   }
                               },
                               handler: function (grid, rowIndex, colIndex) {

                                   var node = grid.store.getAt(rowIndex);
                                   //基本属性里用到这两个权限
                                   if (node.data.AttrName === "文件名" && me.hasDFWriteRight === "True") {
                                       //触发上传文件按钮事件(html5方法实现)
                                       var w = me.up('_mainSourceView').down('_mainDocGrid').ReplaceDocButton.uploader.uploader;
                                       var y = document.getElementById(w.id + "_html5");
                                       if (y && !y.disabled) {
                                           y.click()
                                       } else {
                                           var z = document.getElementById(w.id + "_flash");
                                           if (z) {
                                               document.getElementById(w.settings.browse_button).click();
                                               //w.settings.browse_button_active = true;
                                               w.trigger("flash_click", w)

                                           }
                                       }
                                   } else if (node.data.AttrName === "模板" && (me.hasDocEditRight === "True" || me.hasProjectEditRight === "True")) {
                                       //Ext.Msg.alert("错误信息", "按下了修改模板！");
                                       me.get_doc_def();
                                   }
                                   //附加属性里用到这个权限
                                   if (node.data.TempAttrType === 4 && node.data.CanEdit === "True") {
                                      
                                       Ext.require('Ext.ux.Common.comm', function () {
                                           showSelectUserWin("getUser", "", "", function () {
                                               //返回参数说明：window.parent.resultvalue:用户Keyword列表，window.parent.usernamelist:用户代码列表
                                               me.update_Doc_atta_attr(window.parent.usernamelist, rowIndex, "user");//修改文档附加属性
                                               
                                           });
                                       })
                                   }
                               }
                           }
                           ], renderer: function (value, metaData, record, rowIdx, colIdx, store) {

                               //调用QuickTips,显示提示信息
                               var value2 = "";
                               if (record.get('AttrName') === "文件名") {
                                   value2 = "上传替换文件";
                               }
                               metaData.tdAttr = 'data-qtip="' + Ext.String.htmlEncode(value2) + '"';
                           }
                       }
            ],
            stripeRows: true,
            viewConfig: {
                getRowClass: function (record, rowIndex, p, store) {//CSS class name to add to the row.获得一行的css样式  
                    //    if (store.getAt(rowIndex).get('AttrType') !== "AddiAttr" || store.getAt(rowIndex).get('Visible') !== "True")
                    if (record.data.Visible === "False")
                        return "hide-store-row";
                    if ((record.data.AttrType === "AddiAttr" && record.data.TempAttrType != 3 && record.data.TempAttrType != 4))
                        return "hide-store-row";
                }
            },
            plugins: [
             Ext.create('Ext.grid.plugin.CellEditing', {
                 clicksToEdit: 1, //设置点击几次触发编辑插件，默认次数为2  //设置单击单元格编辑  
                 listeners: {
                     'beforeedit': function (editor, e, eOpts) {
                         var tempAttrType = me._MainAttrStore.getAt(e.rowIdx).get('TempAttrType');//获取附加属性类别
                         if (tempAttrType === "" || tempAttrType === 4) {
                             return false;//设置不可以编辑
                         }else{
                             var editRowIdx = e.rowIdx;
                             if (me._MainAttrStore.getAt(e.rowIdx).get('CanEdit') == "True") {
                                 if (me._MainAttrStore.getAt(e.rowIdx).get('DataType') == 5) {
                                     me.DocAttrDataColumn.setEditor(new Ext.form.field.Date({
                                         format: 'Y.m.d', value: new Date(),
                                         listeners: {
                                             blur: function (editor, e, eOpts) {
                                                 me.update_Doc_atta_attr(editor.value, editRowIdx, "common");//修改文档附加属性
                                             },
                                             specialkey: function (editor, e, eOpts) {
                                                 if (e.getKey() == Ext.EventObject.ENTER) {
                                                     me.update_Doc_atta_attr(editor.value, editRowIdx, "common");//修改文档附加属性
                                                 }
                                             }
                                         }
                                     }));
                                 }
                                 else {
                                     var ShowData = me._MainAttrStore.getAt(e.rowIdx).get('ShowData');
                                     if (ShowData != "undefined") {
                                         if (ShowData.length === 0)//判断字符串是否为空
                                         {
                                             me.DocAttrDataColumn.setEditor(new Ext.form.field.Text(
                                                 {
                                                     listeners: {
                                                         blur: function (editor, e, eOpts) {
                                                             me.update_Doc_atta_attr(editor.value, editRowIdx, "common");//修改文档附加属性
                                                         },
                                                         specialkey: function (editor, e, eOpts) {
                                                             if (e.getKey() == Ext.EventObject.ENTER) {
                                                                 me.update_Doc_atta_attr(editor.value, editRowIdx, "common");//修改文档附加属性
                                                             }
                                                         }
                                                     }
                                                 }
                                                 ));
                                         } else {
                                             me.DocAttrComboData = ShowData;

                                             //添加当前文本的值到combo列表
                                             var hasCurValue = false;
                                             for (var i = 0; i < me.DocAttrComboData.length; i++) {
                                                 if (me.DocAttrComboData[i].text === e.value) {
                                                     hasCurValue = true;
                                                     break;
                                                 }
                                             }
                                             if (hasCurValue === false) {
                                                 me.DocAttrComboData.push({ text: e.value, value: e.value });
                                             }

                                             //刷新附加属性combo
                                             //方法一：新建一个Proxy
                                             var mDocAttrComboProxy = Ext.create("Ext.data.proxy.Memory", {
                                                 data: me.DocAttrComboData,
                                                 model: "DocAttrComboModel"
                                             });

                                             me.DocAttrComboStore.proxy = mDocAttrComboProxy;

                                             //方法二：更新me.DocAttrComboProxy,但是因为Extjs的bug不能实现，
                                             //bug的位置：updateOperation: function(operation, callback, scope) ， records的长度为空
                                             //me.DocAttrComboProxy.update(new Ext.data.Operation({
                                             //    action: 'update',   //指明操作的类型为更新  
                                             //    data: me.DocAttrComboData
                                             //}), function (result) { }, this);

                                             me.DocAttrDataColumn.setEditor(new Ext.form.field.ComboBox(
                                                 {
                                                     store: me.DocAttrComboStore,
                                                     triggerAction: 'all',
                                                     valueField: 'value', editable: false,//不可输入
                                                     displayField: 'text', hideLabel: true,
                                                     mode: 'local',
                                                     listeners: {
                                                         blur: function (editor, e, eOpts) {
                                                             me.update_Doc_atta_attr(editor.value, editRowIdx, "common");//修改文档附加属性
                                                         },
                                                         specialkey: function (editor, e, eOpts) {
                                                             if (e.getKey() == Ext.EventObject.ENTER) {
                                                                 me.update_Doc_atta_attr(editor.value, editRowIdx, "common");//修改文档附加属性
                                                             }
                                                         }
                                                     }
                                                 }
                                                 ));
                                         }
                                     }
                                 }
                                 return true;//设置可以编辑
                             } else {
                                 return false;//设置可以编辑
                             }
                             
                         } 
                     }
                 }
             })],
            //renderTo: Ext.getBody(),
            selModel: {
                selType: 'cellmodel'
            }

        });


        //添加属性TAB页面到容器
        me.items = [
            			//{
            			//    xtype:"button",
            			//    text:"我的按钮"
            			//}
                               me.MainAttrGrid
        ];

        me.callParent(arguments);
    },

    //更新文档属性,参数closeWin:完成更新后是否关闭窗口
    //update_Doc_attr: function (docKeyword,tempDefnId) {
    update_Doc_attr: function (objType,Keyword, AttrObject) {
        var me = this;

        AttrJson = Ext.JSON.encode(AttrObject);
        
        if (objType === "doc") {
            Ext.Ajax.request({

                url: 'WebApi/Post',
                method: "POST",
                params: {
                    C: "AVEVA.CDMS.WebApi.DocController", A: "UpdateDocAttr",
                    sid: localStorage.getItem("sid"), docKeyword: Keyword,//projectKeyword: me.projectKeyword,
                    docAttrJson: AttrJson
                },
                success: function (response, options) {
                    var obj = Ext.decode(response.responseText);
                    if (obj.success == true) {
                        var actTabTitle = me.up('_mainAttrTab').mainattrtab.activeTab.title;
                        if (actTabTitle === "文档属性") {//修改文档附加属性
                            me.loadAttrPage("docAttr", Keyword);
                        } else if (actTabTitle === "文档_附加属性") {//修改文档属性
                            me.loadAttrPage("docAttrData", Keyword);
                        }
                    } else { Ext.Msg.alert("错误信息", obj.msg); }

                }
            })
        } else if (objType === "project") {
            Ext.Ajax.request({

                url: 'WebApi/Post',
                method: "POST",
                params: {
                    C: "AVEVA.CDMS.WebApi.ProjectController", A: "UpdateProjectAttr",
                    sid: localStorage.getItem("sid"), projectKeyword: Keyword,//action: "ModiProject",
                    projectAttrJson: AttrJson
                },
                success: function (response, options) {
                    var obj = Ext.decode(response.responseText);
                    if (obj.success == true) {
                        var actTabTitle = me.up('_mainAttrTab').mainattrtab.activeTab.title;
                        if (actTabTitle === "目录属性") {//修改文档附加属性
                            me.loadAttrPage("projectAttr", Keyword);
                        } else if (actTabTitle === "目录_附加属性") {//修改文档属性
                            me.loadAttrPage("projectAttrData", Keyword);
                        }
                    } else { Ext.Msg.alert("错误信息", obj.msg); }

                }
            })
        }
    },

    //更新文档附加属性
    update_Doc_atta_attr: function (attrValueNew, editRowIdx, tempAttrType) {
        var me = this;//, attrCode = "";//, attrValueNew = value;
        var flagEqual = false;
        var attrCode = attrCode = me._MainAttrStore.getAt(editRowIdx).get('AttrCode');
        if (tempAttrType === "common") {//当是通用属性时，判断是否与原来的值相同
            //attrCode = me._MainAttrStore.getAt(editRowIdx).get('AttrCode');
            var attrValueOld = me._MainAttrStore.getAt(editRowIdx).get('AttrValue');
            if (attrValueOld == attrValueNew) { flagEqual=true }
        } else if (tempAttrType === "user") {//当是用户属性时
            //attrCode = me._MainAttrStore.getAt(editRowIdx).get('AttrCode');
        }
        if (!flagEqual) {//当是通用属性时，判断是否与原来的值相同
            var docAttr = [
               { name: attrCode, value: attrValueNew, attrtype: "attrData" }
            ];
            //获取活动tab页标题
            var actTabTitle = me.up('_mainAttrTab').mainattrtab.activeTab.title;
            if (actTabTitle === "文档_附加属性") {//修改文档附加属性
                me.viewGrid = me.up('_mainSourceView').down('_mainDocGrid').down('gridpanel');//Ext.getCmp('_DocGrid');
                var nodes = me.viewGrid.getSelectionModel().getSelection();//获取已选取的节点
                if (nodes !== null && nodes.length > 0) {

                    me.update_Doc_attr("doc",nodes[0].data.Keyword, docAttr);
                }
            } else if (actTabTitle === "目录_附加属性") {
                me.viewTree = me.up('_mainSourceView').down('_mainProjectTree').mainprojecttree;//获取目录树控件
                var nodes = me.viewTree.getSelectionModel().getSelection();//获取已选取的节点
                if (nodes !== null && nodes.length > 0) {

                    me.update_Doc_attr("project",nodes[0].data.Keyword, docAttr);
                }
            }
        }
        //me.viewGrid = me.up('_mainSourceView').down('_mainDocGrid').down('gridpanel');//Ext.getCmp('_DocGrid');
        //var nodes = me.viewGrid.getSelectionModel().getSelection();//获取已选取的节点
        //if (nodes !== null && nodes.length > 0) {

        //    me.update_Doc_attr(nodes[0].data.Keyword, docAttr);
        //}
    },

    //刷新文档属性和附加属性TAB页
    loadAttrPage: function (tabType, Keyword,e) {
        var me = this;
        if (tabType === "docAttr") {
            //me.DocKeyword = Keyword;
            var gridStore = me._MainAttrStore;
            gridStore.proxy.extraParams.C = "AVEVA.CDMS.WebApi.DocController";
            gridStore.proxy.extraParams.A = "GetDocBaseAttrByKeyword";
            gridStore.proxy.extraParams.DocKeyword = Keyword;//把参数传给C#MVC,路径：\simplecdms\controllers\projectcontroller.cs 下的 GetChildProject()
            gridStore.proxy.extraParams.AttrType = 1,
            gridStore.proxy.extraParams.ObjType = "Doc",
            gridStore.proxy.extraParams.sid = localStorage.getItem("sid");
            gridStore.load({
                callback: function (records, options, success) {
                    //查询是否需要显示附加属性
                    var record, attrCode, attrValue;
                        //显示流程模板代码和描述
                    if (records != undefined && records.length > 0 && records[0] !== null && records[0] !== undefined) {
                        me.hasEditRight = "False";
                        me.hasProjectEditRight = "False";
                            for (var i = 0; i < records.length ; i++) {
                                record = records[i];
                                attrCode = record.data.AttrCode;
                                
                                if (attrCode === "AttrDataCount") {
                                    attrValue = record.data.AttrValue;
                                    var view = me.up('_mainAttrTab');
                                    view.setDocAttrDataCount(attrValue);
                                } else if (attrCode === "O_filename") {
                                    attrValue = record.data.AttrValue;
                                    var view = me.up('_mainAttrTab');
                                    view.setDocO_filename(attrValue);
                                } else if (attrCode === "hasDFWriteRight") {
                                    me.hasDFWriteRight = record.data.AttrValue;
                                } else if (attrCode === "hasEditRight") {
                                    me.hasDocEditRight = record.data.AttrValue;
                                } 
                            }
                        }
                }
            });
        } else if (tabType === "docAttrData") {

            //me.DocKeyword = Keyword;
            var gridStore = me._MainAttrStore;
            gridStore.proxy.extraParams.C = "AVEVA.CDMS.WebApi.DocController";
            gridStore.proxy.extraParams.A = "GetDocAttrDataByKeyword";
            gridStore.proxy.extraParams.DocKeyword = Keyword;//把参数传给C#MVC,路径：\simplecdms\controllers\projectcontroller.cs 下的 GetChildProject()
            gridStore.proxy.extraParams.AttrType = 2,
            gridStore.proxy.extraParams.ObjType = "Doc",
            gridStore.proxy.extraParams.sid = localStorage.getItem("sid");
            gridStore.load({
                callback: function (records, options, success) {
                    //如果附加属性数量少于等于一，就不附加属性，转为显示文档属性
                    var record, attrCode, attrValue;
                    //显示流程模板代码和描述
                    //if (records[0] !== null && records[0] !== undefined) {
                        if (records.length <= 0) {
                            var view = me.up('_mainAttrTab');
                            //view.mainattrtab.setActiveTab(2);
                            //me.loadAttrPage("docAttr", Keyword);
                        }
                    //}
                }
            });
        } else if (tabType === "projectAttr") {
            //me._dcGetProjectBaseAttr = me._dcGetProjectBaseAttr + 1;
            //var _ajaxDc = me._dcGetProjectBaseAttr;

            me._dcGetProjectBaseAttr = Keyword;
            //me.DocKeyword = Keyword;
            var gridStore = me._MainAttrStore;
            gridStore.proxy.extraParams.C = "AVEVA.CDMS.WebApi.ProjectController";
            gridStore.proxy.extraParams.A = "GetProjectBaseAttrByKeyword";
            gridStore.proxy.extraParams.ProjectKeyword = Keyword;//把参数传给C#MVC,路径：\simplecdms\controllers\projectcontroller.cs 下的 GetChildProject()
            gridStore.proxy.extraParams.AttrType = 1,
            gridStore.proxy.extraParams.ObjType = "Project",
            gridStore.proxy.extraParams.sid = localStorage.getItem("sid");


            me.viewTree = me.up('_mainSourceView').down('_mainProjectTree').mainprojecttree;//获取目录树控件
            var nodes = me.viewTree.getSelectionModel().getSelection();//获取已选取的节点
            if (nodes !== null && nodes.length > 0) {
                //var returnBool = setTimeout(function () { if (nodes[0].data.id != Keyword) return true; }, 500);
                //if (returnBool === true ) return;
                //if (nodes[0].data.id != Keyword) return;
                //gridStore.proxy.extraParams.ProjectKeyword = nodes[0].data.id;
            }

            gridStore.load({
                callback: function (records, options, success) {
                    ////获取"_ajaxDc"参数,判断当前返回，与发出去的请求是否一致，防止多次提交出现差错
                    //if (options.request.params._ajaxDc < me._dcGetProjectBaseAttr) return;

                    //me.viewTree = me.up('_mainSourceView').down('_mainProjectTree').mainprojecttree;//获取目录树控件
                    //var nodes = me.viewTree.getSelectionModel().getSelection();//获取已选取的节点
                    //if (nodes !== null && nodes.length > 0) {
                    //    if (nodes[0].data.id != options.request.params.ProjectKeyword) return;
                    //}



                    //查询是否需要显示附加属性
                    var record, attrCode, attrValue;
                    if (success === true) {
                        //显示流程模板代码和描述
                        if (records !== null && records[0] !== null && records[0] !== undefined) {
                            for (var i = 0; i < records.length ; i++) {
                                record = records[i];
                                attrCode = record.data.AttrCode;

                                if (attrCode === "AttrDataCount") {
                                    attrValue = record.data.AttrValue;
                                    var view = me.up('_mainAttrTab');
                                    view.setProjectAttrDataCount(attrValue,e);
                                } else if (attrCode === "hasProjectEditRight") {
                                    me.hasProjectEditRight = record.data.AttrValue;
                                }
                            }
                           // setTimeout(function () { }, 1000);
                            //1000 毫秒  该方法执行一次
                        }
                    }
                }
            });
        } else if (tabType === "projectAttrData") {
            //me.DocKeyword = Keyword;
            var gridStore = me._MainAttrStore;
            gridStore.proxy.extraParams.C = "AVEVA.CDMS.WebApi.ProjectController";
            gridStore.proxy.extraParams.A = "GetProjectAttrDataByKeyword";
            gridStore.proxy.extraParams.ProjectKeyword = Keyword;//把参数传给C#MVC,路径：\simplecdms\controllers\projectcontroller.cs 下的 GetChildProject()
            gridStore.proxy.extraParams.AttrType = 2,
            gridStore.proxy.extraParams.ObjType = "Project",
            gridStore.proxy.extraParams.sid = localStorage.getItem("sid");
            gridStore.load({
                callback: function (records, options, success) {
                    //如果附加属性数量少于等于一，就不附加属性，转为显示目录属性
                    var record, attrCode, attrValue;
                    //显示流程模板代码和描述
                    //if (records[0] !== null && records[0] !== undefined) {
                        if (records.length <= 1)
                        {
                            var view = me.up('_mainAttrTab');
                            view.mainattrtab.setActiveTab(0);
                            me.loadAttrPage("projectAttr", Keyword,"1");
                        }
                    //}

                   // setTimeout(function () { }, 1000);
                    //1000 毫秒  该方法执行一次
                }
            });
        }

    },

    //获取文档模板
    get_doc_def: function () {
        var me = this;
        var actTabTitle = me.up('_mainAttrTab').mainattrtab.activeTab.title;
        if (actTabTitle === "文档属性") {//修改文档附加属性
            me.viewGrid = me.up('_mainSourceView').down('_mainDocGrid').down('gridpanel');//Ext.getCmp('_DocGrid');
        }
        else if (actTabTitle === "目录属性") {//修改文档附加属性
            me.viewGrid = me.up('_mainSourceView').down('_mainProjectTree').down('treepanel');//Ext.getCmp('_DocGrid');
        }
        var nodes = me.viewGrid.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {

            var _fmGetProjDef = Ext.create('Ext.plug_ins.SysPlugins._GetProjDef', { title: "" });
            window.parent.resultvalue = "";
            window.parent.resultdesc = "";
            winGetDef = Ext.widget('window', {
                title: '模板选择',
                //closeAction: 'hide',
                width: 670,
                height: 560,
                minWidth: 670,
                minHeight: 560,
                maxWidth: 670,
                maxHeight: 560,
                layout: 'fit',
                resizable: true,//允许用户调整每个边缘和窗口的角落
                modal: true,//制作窗口模板并且掩饰他背后的一切
                closeAction: 'close', //close 关闭  hide  隐藏  
                items: _fmGetProjDef,
                defaultFocus: 'firstName'
            });

            //监听子窗口关闭事件
            winGetDef.on('close', function () {
                if (window.parent.resultvalue != null && window.parent.resultvalue !== "") {
                    var tempDefnId = window.parent.resultvalue;
                    var strArray = window.parent.resultdesc.split("__");
                    if (strArray.length > 1) {
                        var docAttr = [
                                { name: 'tempDefnId', value: tempDefnId, attrtype: "" }
                        ];
                        //me.update_Doc_attr(nodes[0].data.Keyword, tempDefnId);
                        if (actTabTitle === "文档属性") {//修改文档附加属性
                            me.update_Doc_attr("doc", nodes[0].data.Keyword, docAttr);
                        }
                        else if (actTabTitle === "目录属性") {//修改文档附加属性
                            me.update_Doc_attr("project", nodes[0].data.Keyword, docAttr);
                        }
                    }
                }
            });
            winGetDef.show();

            if (actTabTitle === "文档属性") {//修改文档附加属性
                _fmGetProjDef.winAction = "ModiDoc";
            }
            else if (actTabTitle === "目录属性") {//修改文档附加属性
                _fmGetProjDef.winAction = "ModiProject";
            }
        }
    }
});