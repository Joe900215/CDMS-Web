//新建目录
Ext.define('Ext.ux.YDForm.Project._ModiProjAttr', {
    extend: 'Ext.container.Container',
    alias: 'widget._ModiProjAttr',
    //layout: "border",
    layout: 'fit',
    resultvalue: '', mainPanelId: '',
    initComponent: function () {
        var me = this;
        //定义目录名Text
        me.projectNameText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "名字", anchor: "80%", labelWidth: 60, labelAlign: "left", width: 360, margins: "17 8 17 8",fieldStyle: 'border-color: red; background-image: none;',//红色边框//flex: 1
            data: []
        });

        me.projectDescText = Ext.create("Ext.form.field.Text",
        {
            xtype: "textfield",
            fieldLabel: "描述", anchor: "80%", labelWidth: 60, labelAlign: "left", width: 360, margins: "17 8 17 8"
        });

        me.tempDefnId = "";//记录模板ID
        me.storagedata = [];
        me.winAction = "";//记录是新建目录还是修改目录属性
        me.projectKeyword = "";//记录目录Keyword

        //定义已选择目录的model
        Ext.define("storageModel", {
            extend: 'Ext.data.Model',
            fields: ["storagename", "storagekeyword"]
        });
        me.storageProxy = Ext.create("Ext.data.proxy.Memory", {
            data: me.storagedata,
            model: "storageModel"
        });

        me.storageStore = Ext.create("Ext.data.Store", {
            model: storageModel,
            proxy: me.storageProxy,
            listeners: {
                load: function() {
                    //me.storageCombo.setValue(me.storageCombo.getValue());
                   
                }
            }
        });

        me.storageCombo = Ext.create("Ext.form.field.ComboBox",
        {
            xtype: "combo",
            triggerAction: "all", store: me.storageStore, editable: false,//不可输入
            displayField: 'storagename',
            valueField: 'storagekeyword',
            emptyText: "请选择", autoLoadOnValue: true,
            fieldLabel: "存储", anchor: "80%", labelWidth: 60, labelAlign: "left", margins: "17 8 17 8", width: 360

        });

        //定义模板代码文本框
        me.tempDefnCodeText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "模板", readOnly: true, anchor: "80%", labelWidth: 60, labelAlign: "left", width: 360, margins: "17 8 17 8"
        });

        //定义模板描述文本框
        me.tempDefnDescText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "描述", readOnly: true, anchor: "80%", labelWidth: 60, labelAlign: "left", width: 360, margins: "17 8 17 8"

        });

        me.isHideCheckBox = Ext.create("Ext.form.field.Checkbox", {
            xtype: "checkbox",
            fieldLabel: "",
            boxLabel: "隐藏",
            anchor: "",
            hideLabel: true, margins: "5"
        });

        //添加列表
        me.items = [
          Ext.widget('form', {
              layout: "form",
              items: [{
                  xtype: "tabpanel",
                  layout: {
                      type: 'hbox',
                      pack: 'start',
                      align: 'stretch'
                  },
                  items: [
                        {
                            xtype: "panel",
                            title: "常规",
                            layout: "vbox",
                            width: '100%',
                            align: 'stretch',
                            pack: 'start',
                            margins: "5",
                            items: [
                                {
                                    xtype: "fieldset",
                                    title: "目录属性",
                                    layout: "vbox",
                                    width: '100%',
                                    align: 'stretch',
                                    pack: 'start',
                                    items: [
                                        me.projectNameText,
                                        me.projectDescText

                                    ], flex: 1
                                },
                                {
                                    xtype: "fieldset",
                                    title: "",
                                    layout: "vbox",
                                    width: '100%',
                                    align: 'stretch',
                                    pack: 'start',
                                    items: [
                                        {
                                            xtype: "panel",
                                            layout: "hbox",
                                            //width: '100%',
                                            align: 'stretch', baseCls: 'my-panel-no-border',//隐藏边框
                                            pack: 'start',
                                            items: [
                                                me.tempDefnCodeText,//.modelCodeText,
                                                {
                                                    xtype: "button",
                                                    text: "选择", margins: "17 8 17 8", width: 60,
                                                    listeners: {
                                                        "click": function (btn, e, eOpts) {//添加点击按钮事件
                                                            me.get_project_def();//获取模板
                                                        }
                                                    }
                                                }
                                            ]
                                        },
                                        me.tempDefnDescText
                                    ], flex: 1
                                },
                                {
                                    xtype: "fieldset",
                                    layout: "vbox",
                                    width: '100%',
                                    align: 'stretch',
                                    pack: 'start',
                                    items: [
                                        me.storageCombo

                                    ], flex: 1
                                },
                                me.isHideCheckBox

                            ]
                        },
                   ]
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
                          text: "确定", width: 60, margins: "18 5 18 5",
                          listeners: {
                              "click": function (btn, e, eOpts) {//添加点击按钮事件
                                  me.update_Project_attr(true);
                              }
                          }
                      },
                      {
                          xtype: "button",
                          text: "取消", width: 60, margins: "18 5 18 5",
                          listeners: {
                              "click": function (btn, e, eOpts) {//添加点击按钮事件
                                  win.close();
                              }
                          }
                      },
                  {
                      xtype: "button",
                      text: "应用", width: 60, margins: "18 15 18 5",
                      listeners: {
                          "click": function (btn, e, eOpts) {//添加点击按钮事件
                              me.update_Project_attr(false);
                              //win.close();
                          }
                      }
                  }
                  ]
              }]
          })];

        me.callParent(arguments);

        //Ext.onReady(
        //    //监听load事件
        //me.storageStore.on('load', function () {
        //    me.storageCombo.select(me.storageStore.getAt(0));
        //})
        //);
    },

    //获取目录属性
    get_Project_attr: function (ProjectKeyword) {
        var me = this;
        me.projectKeyword = ProjectKeyword;
        Ext.Ajax.request({
           
            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.WebApi.ProjectController", A: "GetProjectAttr",
                action: me.winAction, ProjectKeyword: ProjectKeyword, sid: localStorage.getItem("sid")
            },
            success: function (response, options) {
                try {
                    //获取数据后，更新窗口
                    var res = Ext.JSON.decode(response.responseText, true);
                    var state = res.success;
                    if (state === false) {
                        var errmsg = res.msg;
                        Ext.Msg.alert("错误信息", errmsg);
                    }
                    else {
                        var recod = eval(res.data[0]);
                        var storagelist = recod.storagelist;
                        for (var storageid in storagelist) {
                            storage = storagelist[storageid];
                            //添加数据  
                            me.storagedata.push({ storagename: storage.storagename, storagekeyword: storage.storagekeyword });
                        }
                        
                        me.storageStore.load();//必须load后再设置combobox的值
                        me.storageCombo.setRawValue(storagelist[0].storagename);//设置显示值
                        me.storageCombo.setValue(storagelist[0].storagekeyword); //设置ID值

                        if (me.winAction === "ModiProject")
                        {
                            me.projectNameText.setValue(recod.projname);
                            me.projectDescText.setValue(recod.projdesc);

                            me.tempDefnCodeText.setValue(recod.defname);
                            me.tempDefnDescText.setValue(recod.defdesc);
                            me.tempDefnId = recod.defkeyid;
                        }
                    }
                } catch (e) { }
            }
        });


    },

    //更新目录属性,参数closeWin:完成更新后是否关闭窗口
    update_Project_attr: function (closeWin) {
        var me = this;
        var projectName = me.projectNameText.value;
        var projectDesc = me.projectDescText.value;
        var tempDefnId = me.tempDefnId;
        var storageName = me.storageCombo.getRawValue();
        var isHide = me.isHideCheckBox.checked;

        var projectAttr =
        [{ name: 'projectName', value: projectName },
            { name: 'projectDesc', value: projectDesc },
            { name: 'tempDefnId', value: tempDefnId },
            { name: 'storageName', value: storageName },
            { name: 'isHide', value: isHide }
        ];
        var projectAttrJson = Ext.JSON.encode(projectAttr);

        //请求判断是否有创建目录权限
        Ext.Ajax.request({
            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.WebApi.ProjectController", A: "GetMenuRight",
                Menu: "ModiProjAttr", ProjectKeyword: me.projectKeyword,
                sid: localStorage.getItem("sid")
            },
            success: function (response, options) {
                var res = Ext.JSON.decode(response.responseText);
                var state = res.success;
                if (state === false) {
                    var errmsg = res.msg;
                    Ext.Msg.alert("错误信息", errmsg);
                }
                else {

                    var nodes = Ext.getCmp(me.mainPanelId).down('treepanel').getSelectionModel().getSelection();//获取已选取的节点
                    var action = "UpdateProjectAttr";
                    if (me.winAction === "CreateProject") {//创建目录
                        action = "CreateProject";
                    }

                    Ext.Ajax.request({
                        url: 'WebApi/Post',
                        method: "POST",
                        params: {
                            C: "AVEVA.CDMS.WebApi.ProjectController", A: action,//"UpdateProjectAttr",action: me.winAction,
                            sid: localStorage.getItem("sid"), projectKeyword: me.projectKeyword, projectAttrJson: projectAttrJson
                        },
                        success: function (response, options) {
                            me.update_Project_attr_callback(action,response, me.projectKeyword, closeWin);
                        }
                    })
                }
            }
        })

    },

    update_Project_attr_callback: function (action,response, parentKeyword, closeWin) {
        var me = this;
        var res = Ext.JSON.decode(response.responseText, true);
        var state = res.success;
        if (state === false) {
            var errmsg = res.msg;
            Ext.Msg.alert("错误信息", errmsg);
        }
        else {
            var tree = Ext.getCmp(me.mainPanelId).down('treepanel');
            var viewTreeStore = tree.store;
            if (action === "CreateProject") {
                var leaf = false;//为true添加子节点，为false添加同级节点
                var cellEditingPlugin = tree.cellEditingPlugin;
                var selectionModel = tree.getSelectionModel();
                var selectedList = selectionModel.getSelection()[0];
                var parentList = leaf ? selectedList.parentNode : selectedList;

                var newList = Ext.create("CDMSWeb.model.ProjectTree", {
                    id: res.data[0].id,
                    text: res.data[0].text,
                    Keyword: res.data[0].Keyword,
                    leaf: res.data[0].leaf,
                    iconCls: res.data[0].iconCls,
                    loaded: true
                });
                var expandAndEdit = function () {
                    if (parentList.isExpanded()) {
                        selectionModel.select(newList);
                    } else {
                        parentList.expand();
                    }
                };

                //判断是否是子节点
                if (selectedList.isLeaf()) {
                    if (!leaf) //判断是添加子节点还是兄弟节点
                    {
                        tree.store.getNodeById(selectedList.data.id).set('leaf', false);
                    }
                }

                parentList.appendChild(newList);

                if (tree.getView().isVisible(true)) {
                    expandAndEdit();
                } else {
                    tree.on('expand', function onExpand() {
                        expandAndEdit();
                        tree.un('expand', onExpand);
                    });
                    tree.expand();
                }

                if (closeWin)
                    win.close();

            } else {
                viewTreeStore.load({
                    callback: function (records, options, success) {//添加回调，获取子目录的文件数量
                        if (closeWin)
                            win.close();
                        //展开目录
                        Ext.require('Ext.ux.Common.comm', function () {
                            Ext.getCmp('contentPanel').down('_mainProjectTree').ExpendProject(parentKeyword);
                        })
                    }
                });
            }
        }
    },

    //修改目录模板
    get_project_def: function () {
        var me = this;

        me.viewTree = Ext.getCmp(me.mainPanelId).down('treepanel');//获取目录树控件ID
        var nodes = me.viewTree.getSelectionModel().getSelection();//获取已选取的节点
        if (nodes !== null && nodes.length > 0) {
            var parentKeyword = nodes[0].data.id
            //请求判断是否有创建目录权限
            Ext.Ajax.request({
                url: 'WebApi/Post',
                method: "POST",
                params: {
                    C: "AVEVA.CDMS.WebApi.ProjectController", A: "GetMenuRight",
                    Menu: "ModiProjAttr", ProjectKeyword: parentKeyword,
                    sid: localStorage.getItem("sid")
                },
                success: function (response, options) {
                    var res = Ext.JSON.decode(response.responseText);
                    var state = res.success;
                    if (state === false) {
                        var errmsg = res.msg;
                        Ext.Msg.alert("错误信息", errmsg);
                    }
                    else {
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
                                me.tempDefnId = window.parent.resultvalue;
                                var strArray = window.parent.resultdesc.split("__");
                                if (strArray.length > 1) {
                                    me.tempDefnCodeText.setValue(strArray[0]);
                                    me.tempDefnDescText.setValue(strArray[1]);
                                }

                            }

                        });

                        winGetDef.show();

                        _fmGetProjDef.winAction = me.winAction;
                    }
                }
            })
        } else {
            Ext.Msg.alert("错误信息", "请选择文件夹！");
        }
    }
});
