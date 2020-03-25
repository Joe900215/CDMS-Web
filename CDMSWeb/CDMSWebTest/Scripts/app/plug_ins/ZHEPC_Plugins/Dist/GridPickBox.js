Ext.define('Ext.plug_ins.ZHEPC_Plugins.Dist.GridPickBox', {
    extend: 'Ext.form.field.ComboBox',
    alias: ["widget.searchCombo", "widget.searchComboBox", ],

    multiSelect: true,

    displayField: null,

    valueField: null,

    matchFieldWidth: false,

    store: null,

    columns: null,

    pickerWidth: 400,

    pickerHeight: 300,

    showPagingbar: false,


    editable: true,
    enableKeyEvents: true,

    onExpand: function () {

    },
    //重写expand函数，设置焦点
    expand: function () {
        var me = this,
            bodyEl, picker, collapseIf;

        me.onExpand();

        if (me.rendered && !me.isExpanded && !me.isDestroyed) {
            bodyEl = me.bodyEl;
            picker = me.getPicker();
            collapseIf = me.collapseIf;

            // show the picker and set isExpanded flag
            picker.show();
            me.isExpanded = true;
            me.alignPicker();
            bodyEl.addCls(me.openCls);

            // monitor clicking and mousewheel
            me.mon(Ext.getDoc(), {
                mousewheel: collapseIf,
                mousedown: collapseIf,
                scope: me
            });
            Ext.EventManager.onWindowResize(me.alignPicker, me);
            me.fireEvent('expand', me);
            me.onExpand();
            me.focus(false, 100);
        }
    },

    collapse: function () {
        if (this.isExpanded && !this.isDestroyed) {
            var me = this,
                openCls = me.openCls,
                picker = me.picker,
                doc = Ext.getDoc(),
                collapseIf = me.collapseIf,
                aboveSfx = '-above';


            //picker.hide();
            me.isExpanded = false;


            me.bodyEl.removeCls([openCls, openCls + aboveSfx]);
            picker.el.removeCls(picker.baseCls + aboveSfx);

            picker.hide();

            doc.un('mousewheel', collapseIf, me);
            doc.un('mousedown', collapseIf, me);
            Ext.EventManager.removeResizeListener(me.alignPicker, me);
            me.fireEvent('collapse', me);
            me.onCollapse();
        }
    },
    //blur: function (view, The, eOpts) {
    //hide: function (view, eOpts) {
    //hide: function (view, eOpts) {
    //    var me = this;
    //},
    hidePicker: function (view, eOpts) {

        var me = this;
        if (this.picker != undefined) {
            this.picker.hide();
        }
    },


    /**
  * 创建Picker
  * @return {Ext.grid.Panel} 
  */
    createPicker: function () {
        var me = this,
            picker = me.createComponent();

        picker.on("itemclick", me.onItemClick, me);
        return picker;
    },

    /**
     * 创建gridPanel,子类可以扩展返回个性化grid(比如条件查询等)
     * @return {Ext.grid.Panel} 
     */
    createComponent: function () {
        var me = this;

        var pagingbar = null;
        if (me.showPagingbar == true) {
            pagingbar = Ext.create("Ext.toolbar.Paging", {
                store: me.store,
                dock: 'bottom',
                displayInfo: true
            });
        }

        var pgx = 387;// this.left; //387
        var pgy = 256;// this.top - thi.height; //256

        picker = Ext.create("Ext.grid.Panel", {
            frame: true,
            renderTo: document.body,
            store: me.professionStore,
            features: [{
                id: 'group',
                ftype: 'grouping',
                groupHeaderTpl: '{name}',
                hideGroupedHeader: true,
                enableGroupingMenu: false
            }],
            hideHeaders: true,
            floating: true,
            store: me.store,
            columns: me.columns,
            width: me.pickerWidth,
            heigkt: me.pickerHeight,
            //pageX : pgx,
            //pageY : pgy,
            dockedItems: [pagingbar
                //{
                //xtype: 'pagingtoolbar',
                //store: me.store,
                //dock: 'bottom',
                //displayInfo: true
                //}
            ]
        });

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

        //if (!me.multiSelect && selection.length) {
        //    if (record.get(valueField) === selection[0].get(valueField)) {
        //        // Make sure we also update the display value if it's only partial
        //        me.displayTplData = [record.data];
        //        me.setRawValue(me.getDisplayValue());
        //        me.collapse();
        //    }
        //}

        me.displayTplData = [record.data];
        me.setRawValue(me.getDisplayValue());
        me.collapse();
        me.onPickerItemClick();

        //隐藏后需要点两次才出现Picker
        me.getPicker().hide();

        this.isExpanded = false;
    },

    onPickerItemClick: function () {

    },

    matchFieldWidth: false,

    onListSelectionChange: function (list, selectedRecords) {
        var me = this,
            isMulti = me.multiSelect,
            hasRecords = selectedRecords.length > 0;
        // Only react to selection if it is not called from setValue, and if our list is
        // expanded (ignores changes to the selection model triggered elsewhere)
        if (!me.ignoreSelection && me.isExpanded) {
            if (!isMulti) {
                Ext.defer(me.collapse, 1, me);
            }
            /*
             * Only set the value here if we're in multi selection mode or we have
             * a selection. Otherwise setValue will be called with an empty value
             * which will cause the change event to fire twice.
             */
            if (isMulti || hasRecords) {
                me.setValue(selectedRecords, false);
            }
            if (hasRecords) {
                me.fireEvent('select', me, selectedRecords);
            }
            //me.inputEl.focus();
            me.focus(false, 100);
        }
        console.log(me.getValue());
    },

    doAutoSelect: function () {
        var me = this,
            picker = me.picker,
            lastSelected, itemNode;
        if (picker && me.autoSelect && me.store.getCount() > 0) {
            // Highlight the last selected item and scroll it into view
            lastSelected = picker.getSelectionModel().lastSelected;
            itemNode = picker.view.getNode(lastSelected || 0);
            if (itemNode) {
                picker.view.highlightItem(itemNode);
                picker.view.el.scrollChildIntoView(itemNode, false);
            }
        }
    }


});
