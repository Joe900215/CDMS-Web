/**
 * 系统通用下拉选择Gird
 */
//Ext.define("Common.picker.GridPicker", {
Ext.define("Ext.plug_ins.ZHEPC_Plugins.Dist.GridPicker", {
    extend: "Ext.form.field.Picker",
    //extend: "Ext.form.field.ComboBox",
    alias: 'widget.gridPicker',

    displayField: null,

    valueField: null,

    matchFieldWidth: false,

    store: null,

    columns: null,

    pickerWidth: 400,

    pickerHeight: 300,

    editable: false,

    showPagingbar:false,


    /**
     * 创建Picker
     * @return {Ext.grid.Panel} 
     */
    createPicker: function () {
        var me = this,
            picker = me.createComponent();

        picker.on("itemclick", me.onItemClick, me);

        me.on("focus", me.onFocusHandler, me);
        return picker;
    },

    /**
     * 创建gridPanel,子类可以扩展返回个性化grid(比如条件查询等)
     * @return {Ext.grid.Panel} 
     */
    createComponent: function () {
        var me = this;

        var pagingbar=null;
        if (me.showPagingbar == true) {
            pagingbar = Ext.create("Ext.toolbar.Paging", {
                store: me.store,
                dock: 'bottom',
                displayInfo: true
            });
        }

       
            picker = Ext.create("Ext.grid.Panel", {
                floating: true,
                store: me.store,
                columns: me.columns,
                width: me.pickerWidth,
                heigkt: me.pickerHeight,
                dockedItems: [ pagingbar
                    //{
                    //xtype: 'pagingtoolbar',
                    //store: me.store,
                    //dock: 'bottom',
                    //displayInfo: true
                    //}
                ]
            });
        return picker;
    },

    /**
     * 处理grid行单击事件
     */
    onItemClick: function (view, record, item, index, e, eOpts) {
        var me = this;
        me.setValue(record.get(me.valueField));
        me.getPicker().hide();
        me.inputEl.focus();
    },

    /**
     * 获得焦点弹出
     */
    onFocusHandler: function () {
        var me = this;
        if (!me.isExpanded) {
            this.expand();
            this.focus();
        }
    },

    /**
     * 设置值
     * @param {Mixed} value
     * @return {Common.picker.GridPicker} this
     */
    setValue: function (value) {
        var me = this,
             record;

        me.value = value;

        if (me.store.isLoading()) {
            //当store加载暂时不做处理
            return me;
        }

        if (value === undefined) {
            return me;
        } else {
            record = me.getPicker().getSelectionModel().getSelection()[0];
        }

        me.setRawValue(record ? record.get(me.displayField) : '');

        return me;
    },

    /**
     * 返回field的值
     * @return {String}
     */
    getValue: function () {
        return this.value;
    },

    /**
     * 返回提交到服务器端的值
     * @return {String}
     */
    getSubmitValue: function () {
        return this.value;
    }

});