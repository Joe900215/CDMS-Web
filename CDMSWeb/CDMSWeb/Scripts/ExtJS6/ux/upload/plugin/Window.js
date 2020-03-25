/**
 * @class Ext.ux.upload.plugin.Window
 * @extends Ext.AbstractPlugin
 * 
 * @author Harald Hanek (c) 2011-2012
 * @license http://harrydeluxe.mit-license.org
 */
Ext.define('Ext.ux.upload.plugin.Window', {
    extend: 'Ext.AbstractPlugin',
    alias: 'plugin.ux.upload.window',
    //alias: 'plugin.ux.upload._uploadWindow', // 此类的xtype类型为buttontransparent  
    requires: [ 'Ext.ux.statusbar.StatusBar',
                'Ext.ux.statusbar.ValidationStatus' ],
            
    constructor: function(config)
    {
        var me = this;
        Ext.apply(me, config);
        me.callParent(arguments);
    },

    initComponent:function ()
    {
        var me = this;
        me.renderTo = me.el;

        
    },
        
    init: function (cmp)
    {
        var me = this,
            uploader = cmp.uploader;
        
        cmp.on({
            filesadded: {
                fn: function(uploader, files)
                {

                    //当uploader添加文档后，显示本窗口
                    me.window.show();
                    cmp.windowId = me.window.id;

                },
                scope: me
            },
            updateprogress: {
                fn: function(uploader, total, percent, sent, success, failed, queued, speed)
                {
                    var sl = " , 上传速度："+speed+"/s";
                    var t = Ext.String.format('已上传{0}% (第{1}个文件 共有 {2}个文件){3}', percent, sent, total,sl);
                    me.statusbar.showBusy({
                        text: t,
                        clear: false
                    });
                },
                scope: me
            },
            uploadprogress: {
                fn: function(uploader, file, name, size, percent)
                {
                    // me.statusbar.setText(name + ' ' + percent + '%');
                },
                scope: me
            },
            uploadcomplete: {
                fn: function(uploader, success, failed)
                {
                    if(failed.length == 0)
                        me.window.hide();
                },
                scope: me
            }
        });
        
        me.statusbar = new Ext.ux.StatusBar({
            dock: 'bottom',
            //id: 'form-statusbar',
            defaultText: 'Ready'
        });
        
        //添加图标
        me.actioncolumn = Ext.create('Ext.grid.column.Action', {
            menuDisabled: true,
            sortable: false,
            xtype: 'actioncolumn',
            enableColumnResize: false,
            width: 38,
            items: [{
                getClass: function (v, metaData, record) {
                    var recStatus = record.get('status');
                    //if (recStatus != 2) {
                        return 'dele-upload';
                    //}
                },
                handler: function (grid, rowIndex, colIndex) {
                    //Ext.Msg.alert("错误信息", "按下了修改模板2！");
                    var node = grid.store.getAt(rowIndex);
                    var id = node.data.id;
                    var Uploader = uploader;
                    var file = Uploader.uploader.getFile(id);

                    if (file) {
                        if (file.status == 2)
                        {
                            Uploader.uploader.stop();
                        }
                            Uploader.uploader.removeFile(file);
                    }
                    else
                        Uploader.store.remove(Uploader.store.getById(id));
                }
            }
            ]
        });
          
        //添加图标
        me.percentColumn = Ext.create('Ext.grid.column.Column', {
            //xtype: 'gridcolumn',
            width: 110,
            //dataIndex: 'progress',
            dataIndex: 'percent',
            text: '进度',
            region: "center",
            renderer: function (v, m, r) {
                var tmpValue = v / 100;
                //var tmpText = r.data.Data1 + ' / ' + r.data.Data2;
                var tmpText = v + '%';
                var progressRenderer = (function (pValue, pText) {
                    var b = new Ext.ProgressBar();
                    return function(pValue, pText) {
                        //b.updateProgress(pValue, pText, true);
                        b.updateProgress(pValue, pText, true);
                        //b.updateProgress(pValue, "", true);
                        return Ext.DomHelper.markup(b.getRenderTree());
                    };
                })(tmpValue, tmpText);
                return progressRenderer(tmpValue, tmpText);
            }

            //renderer: function (value, metaData, record) {
            //    var id = Ext.id();
            //    metaData.tdAttr = 'data-qtip="' + value + '%"';
            //    Ext.defer(function () {
            //        Ext.widget('progressbar', {
            //            renderTo: id,
            //            value: value / 100,
            //            height: 20,
            //            width: 100,
            //            text: value + '%'
            //        });
            //    }, 50);
            //    return Ext.String.format('<div id="{0}"></div>', id);
            //}
        });

        me.view = new Ext.grid.Panel({
            store: uploader.store,
            stateful: true,
            multiSelect: true,
            //hideHeaders: true,
            stateId: 'stateGrid',
            columns: [{
                text: '文件名',
                flex: 1,
                sortable: false,
                dataIndex: 'name'
            },
                    {
                        text: '大小',
                        width: 90,
                        sortable: true,
                        align: 'right',
                        renderer: Ext.util.Format.fileSize,
                        dataIndex: 'size'
                    },
                    {
                        text: '改变',
                        width: 75,
                        sortable: true,
                        hidden: true,
                        dataIndex: 'percent'
                    },
                    {
                        text: '状态',
                        width: 75,
                        hidden: true,
                        sortable: true,
                        dataIndex: 'status'
                    },
                    me.percentColumn,
                    //{
                    //    text: '信息',
                    //    width: 75,
                    //    sortable: true,
                    //    dataIndex: 'msg'
                    //},
                    me.actioncolumn
            ],
            viewConfig: {
                stripeRows: true,
                enableTextSelection: false,
                getRowClass: function () {
                    // 在这里添加自定样式 改变这个表格的行高
                    return 'x-grid-row upload-file-grid-row';
                }
            },
            dockedItems: [{
                dock: 'top',
                enableOverflow: true,
                xtype: 'toolbar',
                style: {
                    background: 'transparent',
                    border: 'none',
                    padding: '5px 0'
                },
                listeners: {
                    beforerender: function(toolbar)
                    {
                        if(uploader.autoStart == false)
                            toolbar.add(uploader.actions.start);
                        //定义清除上传项按钮
                        if (uploader.autoStart == false) {
                            toolbar.add(uploader.actions.cancel);
                            toolbar.add(uploader.actions.removeAll);
                        }
                        if(uploader.autoRemoveUploaded == false)
                            toolbar.add(uploader.actions.removeUploaded);
                        //toolbar.add('->');
                        //toolbar.add(uploader.actions.cancel);
                    },
                    itemclick: function (View , record, item, index,  e, eOpts ) { Ext.Msg.alert("错误信息", "按下了修改模板2！"); },
                    scope: me
                }
            },
                    me.statusbar]
        });
        
        me.window = new Ext.Window({
            title: me.title || 'Upload files',
            width: me.width || 640,
            height: me.height || 380,
            // modal : true, // harry
            plain: true,
            constrain: true,
            border: false,
            layout: 'fit',
            items: me.view,
            closeAction: 'hide',
            listeners: {
                hide: function(window)
                {
                    /*
                     * if(this.clearOnClose) { this.uploadpanel.onDeleteAll(); }
                     */
                },
                scope: this,

            },
            updatePercentColumn : function  (currentprogress) {
                me.percentColumn.updateProgress(currentprogress, "当前进度为" + currentprogress + "%");
    }
            });


       // me.callParent(arguments);
    },



});