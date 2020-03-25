/**
 * @class Ext.ux.upload.Button
 * @extends Ext.button.Button
 * 
 * @author Harald Hanek (c) 2011-2012
 * @license http://harrydeluxe.mit-license.org
 */
Ext.define('Ext.ux.upload.Button', {
    extend: 'Ext.button.Button',
    alias: 'widget.uploadbutton',
    requires: ['Ext.ux.upload.Basic'],
    disabled: true,
    
    constructor: function(config)
    {
        var me = this;
        config = config || {};
        Ext.applyIf(config.uploader, {
            browse_button: config.id || Ext.id(me)
        });
        me.callParent([config]);
    },
    
    initComponent: function()
    {
        var me = this,
            e;
        me.callParent();
        me.uploader = me.createUploader();
        


        if(me.uploader.drop_element && (e = Ext.getCmp(me.uploader.drop_element)))
        {
            e.addListener('afterRender', function()
                {
                       me.uploader.initialize();
                },
                {
                    single: true,
                    scope: me
                });
        }
        else
        {
            //var dayreporteventid = 'aabbcc';
            //var departmentid = 'ccddee';
            //dayReportSonModel = Ext.create('Ext.ux.eval.model.DayReportSon', {
            //    dayreporteventid: dayreporteventid,
            //    departmentid: departmentid
            //});

            //// Ìí¼ÓÊÂ¼þ¼àÌý
            //dayReportSonModel.addListener('afterRender', function () {
            //    me.uploader.initialize();
            //});

            me.on({
                afterRender: function () {
                    //alert('cccfff');
                    me.uploader.initialize();
                },
                scope: this // Important. Ensure "this" is correct during handler execution
            });


            //me.listeners = {
            //    afterRender: {
            //        fn: function()
            //        {
            //            me.uploader.initialize();
            //        },
            //        single: true,
            //        scope: me
            //    }
            //};
        }
        
        me.relayEvents(me.uploader, ['beforestart',
                'uploadready',
                'uploadstarted',
                'uploadcomplete',
                'uploaderror',
                'filesadded',
                'beforeupload',
                'chunkuploaded',
                'fileuploaded',
                'updateprogress',
                'uploadprogress',
                'storeempty']);
    },
    
    //listeners : {
    //    afterRender: function () {
    //        this.uploader.initialize();
    //        //alert("ccccffff");
    //    }

    //},

    /**
     * @private
     */
    createUploader: function()
    {
        return Ext.create('Ext.ux.upload.Basic', this, Ext.applyIf({
            listeners: {}
        }, this.initialConfig));
    }
});