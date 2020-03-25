Ext.define('Ext.ux.Control.ComboBox.ComboTreeBox', {
    //下拉树控件
    extend: 'Ext.form.field.ComboBox',
    alias: 'widget.combotreebox',

//Ext.define('ComboTreeBox', {
//    extend: 'Ext.form.field.ComboBox',

    multiSelect: true,

    createPicker: function () {
        var me = this;

        //定义未选择用户的model
        me.model= Ext.define("combotreebox",{
            extend: 'Ext.data.Model',
            //parentId用来记录父目录
            fields: [{ name: 'id', type: 'string' },
                { name: 'text', type: 'string' }
            ],
            //代理定义
            proxy: {
                type: 'ajax',
                //代理的API定义了操作的提交路径
                //路径：\CDMSWeb\Controllers\FileController.cs
                //api: {
                //    read: 'Project/GetProjectList?ProjectType=1'

                //},
                url: "WebApi/Get",//调用路径：\simplecdms\controllers\projectcontroller.cs
                extraParams: {
                    C: "AVEVA.CDMS.WebApi.ProjectController", A: "GetProjectListJson",
                    ProjectType: 1, sid: localStorage.getItem("sid")
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
                    exception: CDMSWeb.ProxyException
                }
            },


            idProperty: "id"
        });

        me.store = Ext.create("Ext.data.TreeStore", {
                
            batchActions: false,
            remoteFilter: false,
            remoteSort: false,
            model: me.model,
            root: { id: "Root", text: "根目录", expanded: true }


        });


        //创建树控件  
        var picker = Ext.create('Ext.tree.Panel', {
            store: me.store,
            rootVisible: false,
            selModel: {
                mode: me.multiSelect ? 'SIMPLE' : 'SINGLE'
            },
            floating: true,
            hidden: true,
            focusOnToFront: false
        });
        //注册事件用于选择用户选择的值  
        me.mon(picker, {
            itemclick: me.onItemClick,
            refresh: me.onListRefresh,
            scope: me
        });

        me.mon(picker.getSelectionModel(), {
            beforeselect: me.onBeforeSelect,
            beforedeselect: me.onBeforeDeselect,
            selectionchange: me.onListSelectionChange,
            scope: me
        });
        this.picker = picker;
        return picker;
    },

    onItemClick: function (picker, record) {
        /* 
         * If we're doing single selection, the selection change events won't fire when 
         * clicking on the selected element. Detect it here. 
         */
        var me = this,
            selection = me.picker.getSelectionModel().getSelection(),
            valueField = me.valueField;
        

        if (!me.multiSelect && selection.length) {
            if (record.get(valueField) === selection[0].get(valueField)) {
                // Make sure we also update the display value if it's only partial  
                me.displayTplData = [record.data];
                me.setRawValue(me.getDisplayValue());
                me.collapse();
            }
        }
    }
});