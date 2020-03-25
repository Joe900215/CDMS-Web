//新建目录
Ext.define('Ext.ux.YDForm.Project._NewglobaloruserProject', {
    extend: 'Ext.container.Container',
    alias: 'widget._NewglobaloruserProject',
    //layout: "border",
    layout: 'fit',
    resultvalue: '', mainPanelId: '',
    initComponent: function () {
        var me = this;

        me.projectType = "";

        me.winAction = "";//记录是新建目录还是修改目录属性
        me.isCreatRoot = false;

        me.parentKeyword = "";

        //定义目录名Text
        me.projectNameText = Ext.create("Ext.form.field.Text", {
            xtype: "textfield",
            fieldLabel: "名称", anchor: "80%", labelWidth: 60, labelAlign: "left", width: 360,// flex: 1,//
            margins: "8 8 8 8",// fieldStyle: 'border-color: red; background-image: none;',//红色边框//
            data: []
        });

        me.projectDescText = Ext.create("Ext.form.field.Text",
        {
            xtype: "textfield",
            fieldLabel: "描述", anchor: "80%", labelWidth: 60, labelAlign: "left", width: 360, //flex: 1,//
            margins: "8 8 8 8"
        });

        //添加查询字符串Text
        me.SQLText = Ext.create("Ext.form.field.TextArea", {
            xtype: "textarea", hidden: true,
            fieldLabel: "查询语句", anchor: "80%", labelWidth: 60, labelAlign: "left",  width: 360, //width: "100%",
            height: 220, margins: "8 8 8 8"
        });

        //添加列表
        me.items = [
          Ext.widget('form', {
              layout: "form",
              margins: "5",
              items: [{
                  xtype: "panel",
                  layout: {
                      type: 'vbox',
                      pack: 'start',
                      align: 'stretch'
                  },
                  baseCls: 'my-panel-no-border',//隐藏边框
                  items: [
                {
                    xtype: "fieldset",
                    title: "文件夹",
                    layout: "vbox",
                    width: '100%',
                    align: 'stretch', margins: "5",
                    pack: 'start',
                    items: [
                        me.projectNameText,
                        me.projectDescText,
                        me.SQLText
                    ], flex: 1
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
                          text: "确定", width: 60, margins: "5 5 10 5",
                          listeners: {
                              "click": function (btn, e, eOpts) {//添加点击按钮事件
                                  me.sendNewGlobalOrUserProject();
                              }
                          }
                      },
                      {
                          xtype: "button",
                          text: "取消", width: 60, margins: "5 15 10 5",
                          listeners: {
                              "click": function (btn, e, eOpts) {//添加点击按钮事件
                                  win.close();
                              }
                          }
                      }
                  ]
              }]
              }]
          })];

        me.callParent(arguments);

    },

    sendNewGlobalOrUserProject: function () {
        var me = this;

        //是否在根目录下创建
        if (me.winAction === "CreateProject" && me.isCreatRoot === true)
        { me.parentKeyword = "Root"; }

        var projectCode = me.projectNameText.value;
        var projectDesc = me.projectDescText.value;

        var strSql = "";
        if (me.projectType === EnProjectType.GlobSearch)
            strSql = me.SQLText.value;
        
        var A="NewGlobalOrUserProject";
        if (me.winAction != "CreateProject" && me.winAction==="ModiAttr") {
            A="UpdateGlobalOrUserProject";
        }

        Ext.Ajax.request({
            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.WebApi.ProjectController", A: A ,
                sid: localStorage.getItem("sid"), projectKeyword: me.parentKeyword,
                projectCode: projectCode, projectDesc: projectDesc,
                projectType: me.projectType, SQLString: strSql
            },
            success: function (response, options) {
                me.sendNewGlobalOrUserProject_callback(response, me.projectKeyword, true);
            }
        })
    },

    sendNewGlobalOrUserProject_callback: function (response, parentKeyword, closeWin) {
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
            if (me.winAction === "CreateProject") {
                var leaf = false;//为true添加子节点，为false添加同级节点
                var cellEditingPlugin = tree.cellEditingPlugin;
                var selectionModel = tree.getSelectionModel();
                var selectedList = selectionModel.getSelection()[0];
                var parentList = res.data[0].parentid === "Root" ? tree.getRootNode() : (leaf ? selectedList.parentNode : selectedList);

                var newList = Ext.create("CDMSWeb.model.ProjectTree", {
                    id: res.data[0].id,
                    text: res.data[0].text,
                    Keyword: res.data[0].Keyword,
                    leaf: res.data[0].leaf,
                    iconCls: res.data[0].iconCls,
                    loaded: true
                });
                var expandAndEdit = function () {
                    if (parentList) {
                        if (parentList.isExpanded()) {
                            selectionModel.select(newList);
                        } else {
                            parentList.expand();
                        }
                    }
                };

                //判断是否是子节点
                if (selectedList && selectedList.isLeaf()) {
                    if (!leaf) //判断是添加子节点还是兄弟节点
                    {
                        var seleNode = tree.store.getNodeById(selectedList.data.id);
                        if (seleNode)
                        { 
                            seleNode.set('leaf', false);
                        }
                    }
                }

                if (parentList)
                    parentList.appendChild(newList);

                if (tree.getView().isVisible(true)) {
                //if (true) {
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
                            var mp = Ext.getCmp(me.mainPanelId);
                                mp.ExpendProject(parentKeyword);
                        })
                    }
                });
            }
        }

    },

    //获取表单默认值
    GetGlobaloruserProjectDefault: function (funCallback) {
        var me = this;


        //通过extjs的ajax获取操作全部名称
        Ext.Ajax.request({
            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.WebApi.ProjectController", A: "GetGlobalOrUserProject",
                sid: localStorage.getItem("sid"), ProjectKeyword: me.projectKeyword
            },
            success: function (response, options) {
                me.GetGlobaloruserProjectDefault_callback(response, options, funCallback);

            }
        });
    },

    GetGlobaloruserProjectDefault_callback: function (response, options, funCallback) {
        var me = this;

        //获取数据后，更新窗口
        var res = Ext.JSON.decode(response.responseText, true);
        var state = res.success;
        if (state === true) {
            var recod = eval(res.data[0]);

            var ProjectCode = recod.Code;
            var ProjectDesc = recod.Desc;
            var strSql = recod.SQLString;

            me.projectNameText.setValue(ProjectCode);

            me.projectDescText.setValue(ProjectDesc);

            me.SQLText.setValue(strSql);

        }
        funCallback();
    }

});