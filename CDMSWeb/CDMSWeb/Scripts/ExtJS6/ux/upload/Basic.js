/**
 * @class Ext.ux.upload.Basic
 * @extends Ext.util.Observable
 * 
 * @author Harald Hanek (c) 2011-2012
 * @license http://harrydeluxe.mit-license.org
 */
Ext.define('Ext.ux.upload.Basic', {
    extend: 'Ext.util.Observable',
    //extend: 'Ext.mixin.Observable',
    autoStart: true,
    autoRemoveUploaded: true,
    alias: 'widget._plupload',

    statusQueuedText: 'Ready to upload',
    statusUploadingText: 'Uploading ({0}%)',
    statusFailedText: 'Error',
    statusDoneText: 'Complete',
    statusInvalidSizeText: 'File too large',
    statusInvalidExtensionText: 'Invalid file type',
    

    configs: {
        uploader: {
            runtimes: 'html5,flash,silverlight,html4',
                        //用来指定上传方式，指定多个上传方式请使用逗号隔开。一般情况下，你不需要配置该参数，因为Plupload默认会根据你的其他的参数配置来选择最合适的上传方式。
                        //如果没有特殊要求的话，Plupload会首先选择html5上传方式，如果浏览器不支持html5，则会使用flash或silverlight，如果前面两者也都不支持，则会使用最传统的html4上传方式。
                        //如果你想指定使用某个上传方式，或改变上传方式的优先顺序，则你可以配置该参数。
            url: '',//服务器端的上传页面地址
            browse_button: null,//触发文件选择对话框的DOM元素，当点击该元素后便后弹出文件选择对话框。该值可以是DOM元素对象本身，也可以是该DOM元素的id
            container: null,//用来指定Plupload所创建的html结构的父容器，默认为前面指定的browse_button的父元素。该参数的值可以是一个元素的id,也可以是DOM元素本身。
            max_file_size: '4096mb',//用来限定上传文件的大小，如果文件体积超过了该值，则不能被选取。值可以为一个数字，单位为b,也可以是一个字符串，由数字和单位组成，如'200kb'
            resize: '',//可以使用该参数对将要上传的图片进行压缩，该参数是一个对象，里面包括5个属性
            flash_swf_url: '/Scripts/ExtJS6/ux/upload/plupload/js/plupload.flash.swf',//flash上传组件的url地址，如果是相对路径，则相对的是调用Plupload的html文档。当使用flash上传方式会用到该参数。
            silverlight_xap_url: '/Scripts/ExtJS6/ux/upload/plupload/js/plupload.silverlight.xap',//silverlight文件，当需要使用silverlight方式进行上传时需要配置该参数
            filters: [],//可以使用该参数来限制上传文件的类型，大小等，该参数以对象的形式传入
            chunk_size: '512kb', // 分片上传文件时，每片文件被切割成的大小，为数字时单位为字节。也可以使用一个带单位的字符串，如"200kb"。当该值为0时表示不使用分片上传功能
            //chunk_size: null,
            unique_names: false,//当值为true时会为每个上传的文件生成一个唯一的文件名，并作为额外的参数post到服务器端，参数明为name,值为生成的文件名。
            multipart: true,//为true时将以multipart/form-data的形式来上传文件，为false时则以二进制的格式来上传文件。html4上传方式不支持以二进制格式来上传文件，在flash上传方式中，
                            //二进制上传也有点问题。并且二进制格式上传还需要在服务器端做特殊的处理。一般我们用multipart/form-data的形式来上传文件就足够了。
            multipart_params: {},//上传时的附加参数，以键/值对的形式传入，服务器端可是使用$_POST来获取这些参数
            multi_selection: true,//是否可以在文件浏览对话框中选择多个文件，true为可以，false为不可以。默认true，即可以选择多个文件。
            drop_element: null,//指定了使用拖拽方式来选择上传文件时的拖拽区域，即可以把文件拖拽到这个区域的方式来选择文件。该参数的值可以为一个DOM元素的id,也可是DOM元素本身，
                                //还可以是一个包括多个DOM元素的数组。如果不设置该参数则拖拽上传功能不可用。目前只有html5上传方式才支持拖拽上传。
            required_features: null,//可以使用该参数来设置你必须需要的一些功能特征，Plupload会根据你的设置来选择合适的上传方式。因为，不同的上传方式，支持的功能是不同的，
                                    //比如拖拽上传只有html5上传方式支持，图片压缩则只有html5,flash,silverlight上传方式支持。该参数的值是一个混合类型，可以是一个以逗号分隔的字符串， 
            cancelButtonCls: 'dele_upload',
            cancelButtonText:''
			//runtimes: '',
            //url: '',
            //browse_button: null,
            //container: null,
            //max_file_size: '128mb',
            //resize: '',
            //flash_swf_url: '',
            //silverlight_xap_url: '',
            //filters: [],
            ////chunk_size: '1mb', // @see http://www.plupload.com/punbb/viewtopic.php?id=1259
            //chunk_size: null,
            //unique_names: true,
            //multipart: true,
            //multipart_params: {},
            //multi_selection: true,
            //drop_element: null,
            //required_features: null
        }
    },
    

    //onbeforestart:{
    //    fn: function(uploader, file, name, size, percent)
    //        {
    //            // me.statusbar.setText(name + ' ' + percent + '%');
    //        },
    //    scope: this},
    //onuploadready:function(){},
    //onuploadstarted:function(){},
    //onuploadcomplete:function(){},
    //onuploaderror:function(){},
    //onfilesadded:function(){},
    //beforeupload:function(){},
    //onfileuploaded:function(){},
    //onupdateprogress:function(){},
    //onuploadprogress:function(){},
    //onstoreempty:function(){},
    //oncreatedoc:function(){},

    constructor: function(owner, config)
    {
        var me = this;
        me.owner = owner;
        me.success = [];
        me.failed = [];
        Ext.apply(me, config.listeners);
        me.uploaderConfig = Ext.apply(me, config.uploader, me.configs.uploader);

        me.windowId = "";

        ////定义类
        //Ext.define('Ext.tpetro.eval.model.DayReportSon', {
        //    extend : 'Ext.util.Observable', // 添加事件需要继承自Ext.util.Observable类
        //    //mixins: ['Ext.util.Observable'], // mixins必须和extend配合使用
	
        //    config : {
        //        dayreporteventid : '',
        //        departmentid : ''
        //    },
	
        //    // 构造函数
        //    constructor: function(cfg) {
		
        //        this.callParent(cfg); //替代Ext.util.Observable.constructor.call(this, config);
        //        this.initConfig(cfg);
		
        //        //dayReportSonModel.fireEvent('beforestart'); // 触发dayreportsonupdate.jsp生成的dayReportSonModel添加的afterchange事件;
        //    }
        //});

        var dayreporteventid = 'aabb';
        var departmentid = 'ccdd';
        dayReportSonModel = Ext.create('Ext.ux.eval.model.DayReportSon', {
            dayreporteventid: dayreporteventid,
            departmentid: departmentid
        });

        // 添加事件监听
        dayReportSonModel.addListener('beforestart', function () { });
        dayReportSonModel.addListener('uploadready', function () { });
        dayReportSonModel.addListener('uploadstarted', function () { });
        dayReportSonModel.addListener('uploadcomplete', function () { });
        dayReportSonModel.addListener('uploaderror', function () { });
        dayReportSonModel.addListener('filesadded', function () { });
        dayReportSonModel.addListener('beforeupload', function () { });
        dayReportSonModel.addListener('fileuploaded', function () { });
        dayReportSonModel.addListener('updateprogress', function () { });
        dayReportSonModel.addListener('uploadprogress', function () { });
        dayReportSonModel.addListener('storeempty', function () { });
        dayReportSonModel.addListener('createdoc', function () { });

        //me.addEvents('beforestart',
        //        'uploadready',
        //        'uploadstarted',
        //        'uploadcomplete',
        //        'uploaderror',
        //        'filesadded',
        //        'beforeupload',
        //        'fileuploaded',
        //        'updateprogress',
        //        'uploadprogress',
        //        'storeempty',
        //        'createdoc');


        Ext.define('Ext.ux.upload.Model', {
            extend: 'Ext.data.Model',
            fields: ['id',
                    'loaded',
                    'name',
                    'size',
                    'percent',
                    'status',
                    'msg']
        });
        
        me.store = Ext.create('Ext.data.JsonStore', {
            model: 'Ext.ux.upload.Model',
            listeners: {
                load: me.onStoreLoad,
                remove: me.onStoreRemove,
                update: me.onStoreUpdate,
                scope: me
            }
        });
        
        me.actions = {
            
            textStatus: Ext.create('Ext.Action', {
                text: '<i>uploader not initialized</i>'
            }),
            add: Ext.create('Ext.Action', {
                text: config.addButtonText || '添加文件',
                iconCls: config.addButtonCls,
                disabled: false
            }),
            start: Ext.create('Ext.Action', {
                text: config.uploadButtonText || '开始上传',
                disabled: true,
                iconCls: config.uploadButtonCls,
                handler: me.start,
                scope: me
            }),
            cancel: Ext.create('Ext.Action', {
                //text: config.cancelButtonText || '取消',
                disabled: true,
                iconCls: 'stop-upload',//config.cancelButtonCls,
                handler: me.cancel,
                scope: me
            }),
            removeUploaded: Ext.create('Ext.Action', {
                text: config.deleteUploadedText || '清除上传',
                disabled: true,
                handler: me.removeUploaded,
                scope: me
            }),
            removeAll: Ext.create('Ext.Action', {
                text: config.deleteAllText || '清除上传项',
                disabled: true,
                handler: me.removeAll,
                scope: me
            })
        };

        me.callParent();
    },
    
    /**
     * @private
     */
    initialize: function()
    {
        var me = this;
        if(!me.initialized)
        {
            me.initialized = true;
            me.initializeUploader();
        }
    },
    
    /**
     * Destroys this object.
     */
    destroy: function()
    {
        this.clearListeners();
    },
    
    setUploadPath: function(path)
    {
        this.uploadpath = path;
    },
    
    removeAll: function()
    {
        this.store.data.each(function(record)
        {
            this.removeFile(record.get('id'));
        }, this);
    },
    
    removeUploaded: function()
    {
        //console.log(this.store);
        this.store.each(function(record)
        {
            //console.log(record);
            if(record && record.get('status') == 5)
            {
                this.removeFile(record.get('id'));
            }
        }, this);
    },
    
    removeFile: function(id)
    {
        var me = this,
            file = me.uploader.getFile(id);
        
        if(file)
            me.uploader.removeFile(file);
        else
            me.store.remove(me.store.getById(id));
    },
    
    cancel: function()
    {
        var me = this;
        me.uploader.stop();
        me.actions.start.setDisabled(me.store.data.length == 0);
    },
    
    start: function()
    {

        var me = this;
        me.fireEvent('beforestart', me);
        if(me.multipart_params)
        {
            me.uploader.settings.multipart_params = me.multipart_params;
        }
        me.uploader.start();
        //console.log(me.uploader);
    },
    
    //uploader初始化事件
    initializeUploader: function()
    {
        var me = this;

        if (!me.uploaderConfig.runtimes) {
            var runtimes = ['html5'];
            
            me.uploaderConfig.flash_swf_url && runtimes.push('flash');
            me.uploaderConfig.silverlight_xap_url && runtimes.push('silverlight');

            runtimes.push('html4');

            me.uploaderConfig.runtimes = runtimes.join(',');
        }

        me.uploader = Ext.create('plupload.Uploader', me.uploaderConfig);
        //me.uploader = Ext.create('plupload.Uploader', {});
        
        Ext.each(['Init',
                'ChunkUploaded',
                'FilesAdded',
                'FilesRemoved',
                'FileUploaded',
                'PostInit',
                'QueueChanged',
                'Refresh',
                'StateChanged',
                'BeforeUpload',
                'UploadFile',
                'UploadProgress',
                'Error'], function (v) {
                    //绑定uploader事件
                    me.uploader.bind(v, eval("me._" + v), me);
                }, me);
        
        me.uploader.init();
    },
    
    updateProgress: function()
    {
        var me = this,
            t = me.uploader.total,
            speed = Ext.util.Format.fileSize(t.bytesPerSec),
            total = me.store.data.length,
            failed = me.failed.length,
            success = me.success.length,
            sent = failed + success + 1,
            queued = total - success - failed,
            percent = t.percent;
        
        me.fireEvent('updateprogress', me, total, percent, sent, success, failed, queued, speed);
    },
    
    //更新数据，刷新表单
    updateStore: function(v)
    {
        var me = this,
            data = me.store.getById(v.id);
        
        if(!v.msg)
        {
            v.msg = '';
        }
        if(data)
        {
            //data.data = v;
            //data.commit();

            data.set("percent", v.percent);
            if (v.percent <= 1 || v.percent >= 99) {
                data.set("status", v.status);
                data.set("msg", v.msg);
            }

            //data.set(data, v);
            data.commit();
        }
        else
        {
            me.store.loadData([v], true);
        }
    },
    
    onStoreLoad: function(store, record, operation)
    {
        this.updateProgress();
    },
    
    onStoreRemove: function(store, record, operation)
    {
        var me = this;
        if(!store.data.length)
        {
            me.actions.start.setDisabled(true);
            me.actions.removeUploaded.setDisabled(true);
            me.actions.removeAll.setDisabled(true);
            me.uploader.total.reset();
            me.fireEvent('storeempty', me);
        }
        
        //var id = record.get('id');
        var id = record[0].id;
        Ext.each(me.success, function(v)
        {
            if(v && v.id == id)
                Ext.Array.remove(me.success, v);
        }, me);
        
        Ext.each(me.failed, function(v)
        {
            if(v && v.id == id)
                Ext.Array.remove(me.failed, v);
        }, me);
        
        me.updateProgress();
    },
    
    onStoreUpdate: function(store, record, operation)
    {
        record.data = this.fileMsg(record.data);
        this.updateProgress();
    },
    
    fileMsg: function(file)
    {
        var me = this;
        if(file.status && file.server_error != 1)
        {
            switch(file.status)
            {
                case 1:
                    file.msg = me.statusQueuedText;
                    break;
                case 2:
                    file.msg = Ext.String.format(me.statusUploadingText, file.percent);
                    break;
                case 4:
                    file.msg = file.msg || me.statusFailedText;
                    break;
                case 5:
                    file.msg = me.statusDoneText;
                    break;
            }
        }
        return file;
    },
    
    /**
     * Plupload EVENTS
     */
    _Init: function(uploader, data)
    {
        this.runtime = data.runtime;
        this.owner.enable(true); // button aktiv schalten
        this.fireEvent('uploadready', this);
    },
    
    // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    //plupload.js文件 this.trigger("BeforeUpload", w) 后发生
    _BeforeUpload: function(uploader, file)
    {

        if (this.fireEvent('beforeupload', this, uploader, file) === false) {
            //返回false,就不会继续上传文件
            return false;
        } else {

            
        }
    },
    
    // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    //plupload.js文件 this.trigger("BeforeUpload", w) 后发生
    _CreateDoc: function (uploader, file) {
        //this.multipart_params.A = 'BeforeUploadFile';
        //this.multipart_params.AppendFilePath = '';
        //this.fireEvent('beforeupload', this, uploader, file);
    },

    _ChunkUploaded: function (uploader, file, responseObject)
    {
        //分块上传并且第一块文件块上传完成时，添加文件路径
        var filePath = "";
        var res = Ext.JSON.decode(responseObject.response, true);
        var state = res.success;

        if (state === false)
        {
            if (res.msg === "REUPLOAD") {

                //如果服务端返回重新上传文件消息，就重新上传文件
                file.loaded = 0;
                return false;

            } else if (res.msg === "RESUMEFILE") {

                //如果上一个文件块传送出现错误，终止继续上传文件块
                return false;

            }
        }

        this.fireEvent('chunkuploaded', this, uploader, file, filePath, state);
    },
    
    _FilesAdded: function(uploader, files)
    {
        var me = this;

        /* 
        *  方法:Array.baoremove(dx) 
        *  功能:删除数组元素. 
        *  参数:dx删除元素的下标. 
        *  返回:在原数组上修改数组. 
        */
        Array.prototype.baoremove = function (dx) {
            if (isNaN(dx) || dx > this.length) { return false; }
            this.splice(dx, 1);
        }

        //判断上传列表里面是否已经存在即将上传的文件
        for (var j = 0, jlen = files.length; j < jlen; j++) {
            for (var i = 0, len = uploader.files.length; i < len; i++) {
                if (uploader.files[i].status === 1 || uploader.files[i].status === 2) {
                    if (files[j].name === uploader.files[i].name && files[j].size === uploader.files[i].size) {
                        //files.remove(j);
                        files.baoremove(j);
                    } 
                }
            }
        }

        if (files.length > 0) {
            //如果配置里面不允许多选上传文档
            if (me.uploaderConfig.multi_selection != true) {
                if (me.store.data.length == 1) {
                    if (me.store.data.items[0].data.status === 4)
                    { me.store.removeAll(); } else {
                        //if(uploader.files.length == 1)
                        return false;
                    }
                }

                files = [files[0]];
                uploader.files = [files[0]];
            }

            //把文件添加到上传列表
            me.actions.removeUploaded.setDisabled(false);
            me.actions.removeAll.setDisabled(false);
            me.actions.start.setDisabled(uploader.state == 2);
            Ext.each(files, function (v) {
                me.updateStore(v);

            }, me);
        }

            //执行客户自定义事件
            if (me.fireEvent('filesadded', me, files) !== false) {
                if (me.autoStart && uploader.state != 2)
                    Ext.defer(function () {
                        me.start();
                    }, 300);
            }
    },
    
    _FilesRemoved: function(uploader, files)
    {
        Ext.each(files, function(file)
        {
            this.store.remove(this.store.getById(file.id));
        }, this);
    },
    //文件上传后触发的事件
    _FileUploaded: function(uploader, file, status)
    {
        var me = this,
            response = Ext.JSON.decode(status.response);
        
        if(response.success == true)
        {
            file.server_error = 0;
            me.success.push(file);

            me.multipart_params.SureReplace = "false";//把确认替换参数重置为零

            me.fireEvent('fileuploaded', me, file, response);//调用Button.js里面传过来的fileuploaded事件，加载用户自定义事件
        }
        else
        {
            if(response.message)
            {
                file.msg = '<span style="color: red">' + response.message + '</span>';
            }
            file.server_error = 1;
            me.failed.push(file);

            //文件上传失败事件

            result=Ext.apply(status, {
                file: file
            });

            var reObj = Ext.JSON.decode(result.response);
            if (reObj.success == false) {
                if (reObj.data.length > 0 && reObj.data[0].state === "seleSureReplace") {
                me.uploader.stop();
                var fileName = reObj.data[0].fileName;
                var fileSize=Ext.util.Format.fileSize(reObj.data[0].fileSize);
                var updateTime = reObj.data[0].updateTime;
                var sentFileSize = Ext.util.Format.fileSize(file.size);
                Ext.MessageBox.show({
                    title: '文件已存在',
                    msg: '已有原来文件 “' + fileName + '” (文件大小：' + fileSize + ' , 修改时间：' + updateTime + '),<br>  是否使用新文件 “' + file.name + '” (文件大小：' + sentFileSize + ') 替换？',
                    buttons: Ext.MessageBox.YESNO,
                    buttonText: {
                        yes: "是",
                        no: "否"
                    },
                    fn: function (btn) {
                        if (btn === "yes") {
                            var files = me.uploader.files;
                            for (var j = 0, len = files.length; j < len; j++) {
                                if (files[j].id === result.file.id) {
                                    me.uploader.files[j].loaded = 0;
                                    me.uploader.files[j].status = 1;
                                    me.uploader.files[j].percent = 0;
                                    me.uploader.files[j].server_error = 0;
                                    me.fireEvent('beforestart', me);
                                    if (me.multipart_params) {
                                        me.multipart_params.SureReplace = "true";
                                        me.uploader.settings.multipart_params = me.multipart_params;
                                    }
                                    me.uploader.start();

                                    break;
                                }
                            }
                        }
                        else { me.uploader.start(); }
                    }
                });

            } else {
                Ext.Msg.alert("错误信息", reObj.msg);
            }
            } 

            me.fireEvent('uploaderror', me, Ext.apply(status, {
                file: file
            }));
        }
        this.updateStore(file);
    },
    
    _PostInit: function(uploader)
    {
    },
    
    _QueueChanged: function(uploader)
    {
    },
    
    _Refresh: function(uploader)
    {
        Ext.each(uploader.files, function(v)
        {
            this.updateStore(v);
        }, this);
    },
    //状态改变时触发的事件，uploadstarted，uploadcomplete响应事件
    _StateChanged: function(uploader)
    {
        if(uploader.state == 2)
        {
            this.fireEvent('uploadstarted', this);
            this.actions.cancel.setDisabled(false);
            this.actions.start.setDisabled(true);
        }
        else
        {
            this.fireEvent('uploadcomplete', this, this.success, this.failed);
            if(this.autoRemoveUploaded)
                this.removeUploaded();
            this.actions.cancel.setDisabled(true);
            this.actions.start.setDisabled(this.store.data.length == 0);
        }
    },
    
    _UploadFile: function(uploader, file)
    {
        var me = this;
    },
    
    _UploadProgress: function(uploader, file)
    {
        var me = this,
            name = file.name,
            size = file.size,
            percent = file.percent; 
    
        me.fireEvent('uploadprogress', me, file, name, size, percent);

        if(file.server_error)
            file.status = 4;
        
        me.updateStore(file);
    },
    
    _Error: function(uploader, data)
    {
        if(data.file)
        {
            data.file.status = 4;
            if(data.code == -600)
            {
                data.file.msg = Ext.String.format('<span style="color: red">{0}</span>', this.statusInvalidSizeText);
            }
            else if(data.code == -700)
            {
                data.file.msg = Ext.String.format('<span style="color: red">{0}</span>', this.statusInvalidExtensionText);
            }
            else
            {
                data.file.msg = Ext.String.format('<span style="color: red">{2} ({0}: {1})</span>', data.code, data.details,
                        data.message);
            }
            this.failed.push(data.file);
            this.updateStore(data.file);
        }
        this.fireEvent('uploaderror', this, data);
    }
});