/*批量修改文件著录属性*/
Ext.define('Ext.plug_ins.HXEPC_Plugins.Document.BatchEditFileAttr', {
    extend: 'Ext.container.Container',
    alias: 'widget.BatchEditFileAttr',
    //layout: "border",
    layout: 'fit',
    resultvalue: '', mainPanelId: '',
    initComponent: function () {
        var me = this;
        me.renderTo = me.el;

        me.projectKeyword = "";//记录目录Keyword
        
        me.docKeyword = "";

        me.docList = "";

        //定义上传文件按钮
        me.fpFileUploadButtonXhr = Ext.create('Ext.ux.upload.Button', {
            //renderTo: Ext.getBody(),
            //id: 'fpFileUploadButtonXhr',
            text: '选择属性表格',
            iconCls: "file-upload", scope: me, tooltip: '上传文件',
            plugins: [{
                ptype: 'ux.upload.window',
                title: '上传文件',
                width: 520,
                height: 350
            }
            ],
            uploader:
            {
                url: 'WebApi/Post',
                uploadpath: '/Root/files',
                autoStart: true,///选择文件后是否自动上传
                max_file_size: '20020mb',
                multipart_params: { 'guid': '', 'ProjectKeyword': '', 'sid': '', 'C': 'AVEVA.CDMS.WebApi.FileController', 'A': 'UploadFile', 'SureReplace': 'false', 'AppendFilePath': '', 'user': '' }, //设置你的参数//{},
                drop_element: null,//定义好docgrid后再设置拖放控件
                statusQueuedText: '准备上传',
                //statusUploadingText: '上传中 ({0}%)',
                statusUploadingText: '上传中',
                statusFailedText: '<span style="color: red">错误</span>',
                statusDoneText: '<span style="color: green">已完成</span>',
                multi_selection:false,
                statusInvalidSizeText: '文件过大',
                statusInvalidExtensionText: '错误的文件类型'
            },
            listeners:
            {
                uploadstarted: function (uploader, files) {

                },

                filesadded: function (uploader, files) {
                   
                    uploader.multipart_params.ProjectKeyword = me.projectKeyword;//.maindocgrid.store.proxy.extraParams.ProjectKeyWord;

                    uploader.multipart_params.sid = localStorage.getItem("sid");

                    
                },

                beforeupload: function (uploadbasic, uploader, file) {
                    //console.log('beforeupload');			
                    //上传文件而不是替换文件时，这里要把Doc关键字重置
                    uploadbasic.multipart_params.DocKeyword = "";

                    var extIndex = file.name.lastIndexOf(".");
                    var fileCode = file.name;

                    var getTimestamp=new Date().getTime();
                    fileCode = fileCode + getTimestamp;

                    me.beforeCreateDoc(uploadbasic, file, me.projectKeyword, fileCode, function () {
                        //先创建文档
                      
                        Ext.require('Ext.ux.Common.comm', function () {
                            //先创建文档

                            createDoc(uploadbasic, file, me.projectKeyword, fileCode, "", "", function (uploadbasic, res, options, DocKeyword) {
                                //处理创建文档后的返回事件
                                if (res.success === true) {

                                    me.docKeyword = DocKeyword;
                                    me.docList = me.docList + DocKeyword + ",";

                                    //触发服务端BeforeUploadFile事件，如果是断点续传，获取已上传的文件大小
                                    var boolSend = sendBeforeUploadFile(uploadbasic, options.file, DocKeyword, me);
                                    return boolSend;
                                } else {

                                    var errmsg = res.msg;
                                    Ext.Msg.alert("错误信息", errmsg);

                                    return false;
                                }
                            });
                        });
                    });
                },

                chunkuploaded: function (basic, uploader, file, result, state) {

                },

                //单个文件上传完毕事件
                fileuploaded: function (uploader, file) {
                    ////这里用common里面的事件会导致第一次上传时属性不正常显示
                    me.afterUploadFile(uploader, file, me.ServerFullFileName);
               
                },

                uploaderror: function (uploader, result) {
                    //文件上传失败事件

                },

                uploadcomplete: function (uploader, success, failed) {
                   // var store = me.maindocgrid.store;//刷新DOC列表
                  
                  //  store.load();
                },
                scope: this
            }


        });


        //定义下载文件按钮
        me.FileDownLoadButton = Ext.create('Ext.button.Button', {
            //id: 'DownLoadFileButton',
            text: '下载未匹配表格', margin: '0 0 0 10',
            iconCls: "file-download", scope: me, tooltip: '下载文件',
            listeners: {
                "click": function (btn, e, eOpts) {
                    me.onDownLoadFailFile();
                },
                "afterrender": function () {
                    //设置窗口关闭事件
                    winBatchEditFileAttr.on('close', function () {
                        //删除临时文件
                        //Ext.Msg.alert("您展开了目录树节点！！！", "您展开了目录树");
                        if (me.docList != undefined && me.docList != "") {
                            me.docList = me.docList.substring(0, me.docList.length - 1);
                            var docs = me.docList.split(",");
                            me.sendDeleteDoc(me.projectKeyword, docs, 0);
                            //me.sendDeleteDoc(me.projectKeyword, me.docKeyword);
                        }
                    });
                }
            }
        });

        //添加会议内容Text
        me.contentText = Ext.create("Ext.form.field.TextArea", {
            xtype: "textarea", anchor: "80%", labelAlign: "right", labelPad: 8, margin: '10 10 0 10', //margin: '0 5 5 0',
            width: "100%",value:"",
            height: 305, fieldLabel: "", labelWidth: 70,
        });


        me.conentPanel = Ext.create("Ext.panel.Panel", {
            baseCls: 'my-panel-no-border',//隐藏边框
            layout: {
                type: 'vbox',
                align: 'stretch',
                pack: 'start'
            },
            items: [
                
                {
                    layout: "hbox",
                    baseCls: 'my-panel-no-border',//隐藏边框
                    //align: 'right',
                    //pack: 'end',//组件在容器右边
                    margin: '10 10 0 10',
                    items: [

                    me.fpFileUploadButtonXhr,
                    me.FileDownLoadButton
                    ]
                },
                me.contentText
            ]
        });
        //底部按钮区域
        me.bottomButtonPanel = Ext.create("Ext.panel.Panel", {
            //xtype: "panel",
            layout: "hbox",
            baseCls: 'my-panel-no-border',//隐藏边框
            //align: 'right',
            //pack: 'end',//组件在容器右边
            items: [{
                flex: 1, baseCls: 'my-panel-no-border'//隐藏边框
            },
            ////me.fpFileUploadButtonXhr,
            //    {
            //        xtype: "button",
            //        text: "确定", width: 60, margins: "18 5 18 5",
            //        listeners: {
            //            "click": function (btn, e, eOpts) {//添加点击按钮事件
            //                me.send_create_Doc();
            //            }
            //        }
            //    },
                {
                    xtype: "button",
                    text: "关闭", width: 60, margins: "18 5 18 5",
                    listeners: {
                        "click": function (btn, e, eOpts) {//添加点击按钮事件
                            //Ext.Msg.alert("您展开了目录树节点！！！", "您展开了目录树节点！节点ID:" + me.tempDefnId);
                            winBatchEditFileAttr.close();
                        }
                    }
                }]
        });

        //添加列表
        me.items = [
          Ext.widget('form', {
              baseCls: 'my-panel-no-border',//隐藏边框
              layout: {
                  type: 'vbox',
                  align: 'stretch',
                  pack: 'start'
              },
              items: [
                 me.conentPanel,
                 me.bottomButtonPanel
              ]
          })];


        me.callParent(arguments);

    },

    //处理创建doc前的事件
    beforeCreateDoc: function (uploadbasic, file, ProjectKeyWord, fileCode, callBackFun) {
        var me = this;
        Ext.Ajax.request({

            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.WebApi.DocController", A: "BeforeCreateDoc",
                sid: localStorage.getItem("sid"), ProjectKeyword: ProjectKeyWord, FileName: file.name
            },
            success: function (response, options) {
                var res = Ext.JSON.decode(response.responseText, true);
                var state = res.success;
                if (state === false) {
                    var errmsg = res.msg;


                    //把文件设置为上传错误(上传完成)
                    file.status = 4;

                    uploadbasic.uploader.stop();
                    uploadbasic.uploader.start();

                    Ext.Msg.alert("错误信息", errmsg);
                    //return false
                }
                else {

                    callBackFun();
                }
            },
            failure: function (response, options) {

            }
        });
    },

    afterUploadFile: function (uploader, file, ServerFullFileName) {
        var me = this;
        var DocKeyword = uploader.multipart_params.DocKeyword;

        Ext.Ajax.request({

            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.WebApi.FileController", A: "AfterUploadFile",
                sid: localStorage.getItem("sid"), DocKeyword: DocKeyword, ServerFullFileName: ServerFullFileName
            },
            success: function (response, options) {
                var res = Ext.JSON.decode(response.responseText, true);
                var state = res.success;
                if (state === false) {
                    var errmsg = res.msg;
                    Ext.Msg.alert("错误信息", errmsg);
                    return false
                }
                else {
                    //uploadbasic.multipart_params.ServerFullFileName = res.data[0].ServerFullFileName;
                    me.OnAfterCreateNewDocEvent(DocKeyword, file);
                }
            },
            failure: function (response, options) {
                ////Ext.Msg.alert("错误", "连接服务器失败！<br>" + response.responseText);
            }
        });
    },

    OnAfterCreateNewDocEvent: function (DocKeyword, file) {
        //Ext.Msg.alert("错误", "成功上传文件！");
        var me=this;
        me.send_read_file_attr(DocKeyword, file);
    },

    send_read_file_attr: function (DocKeyword, file) {
        var me = this;

        Ext.MessageBox.wait("正在导入文件属性，请稍候...", "等待");

        Ext.Ajax.request({
            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.HXEPC_Plugins.Document", A: "ReadAttrFromExcel",
                sid: localStorage.getItem("sid"), docKeyword: DocKeyword
            },
            success: function (response, options) {

                //获取数据后，更新窗口
                var resp = Ext.JSON.decode(response.responseText, true);
                var state = resp.success;
                if (state === true) {

                    Ext.MessageBox.close();//关闭等待对话框

                    var mpanel = Ext.getCmp(me.mainPanelId);
                    var mpFileStore = mpanel.filestore;

                    var attrList= eval(resp.data);

                    var firstLog = true;
                    
                    var failInfo = "";
                    var succCount = 0;
                    var failCount = 0;
                    me.contentText.setValue("");

                    for (var itemKey in attrList) {
                        var reference = me.getStrValue(attrList[itemKey].reference);    //档号
                        var desc = me.getStrValue(attrList[itemKey].desc);  //文件题名
                        var code = me.getStrValue(attrList[itemKey].code);  //文件编码
                        var res = attrList[itemKey];

                        var bSuccMatch = false;

                        //遍历已选择文件的表格
                        for (var i = 0; i < mpFileStore.getCount() ; i++) {
                            var rec = mpFileStore.getAt(i);
                            if (rec.data.code === code) {
                                succCount = succCount + 1;

                                // rec.set('reference', reference);
                                //档号
                                rec.set('reference', me.getStrValue(res.reference));
                                //卷内序号
                                rec.set('volumenumber', me.getStrValue(res.volumenumber));
                                //责任人
                                rec.set('responsibility', me.getStrValue(res.responsibility));
                                //页数
                                rec.set('page', me.getStrValue(res.page));
                                //份数
                                rec.set('share', me.getStrValue(res.share));
                                //介质
                                rec.set('medium', me.getStrValue(res.medium));
                                //语种
                                rec.set('languages', me.getStrValue(res.languages));
                                //项目名称
                                rec.set('proname', me.getStrValue(res.proname));
                                //项目代码
                                rec.set('procode', me.getStrValue(res.procode));
                                //专业
                                rec.set('major', me.getStrValue(res.major));
                                //机组
                                rec.set('crew', me.getStrValue(res.crew));
                                //厂房代码
                                rec.set('factorycode', me.getStrValue(res.factorycode));
                                //厂房名称
                                rec.set('factoryname', me.getStrValue(res.factoryname));
                                //系统代码
                                rec.set('systemcode', me.getStrValue(res.systemcode));
                                //系统名称
                                rec.set('systemname', me.getStrValue(res.systemname));
                                //关联文件编码
                                rec.set('relationfilecode', me.getStrValue(res.relationfilecode));
                                //关联文件题名
                                rec.set('relationfilename', me.getStrValue(res.relationfilename));
                                //案卷规格
                                rec.set('filespec', me.getStrValue(res.filespec));
                                //归档单位
                                rec.set('fileunit', me.getStrValue(res.fileunit));
                                //密级
                                rec.set('secretgrade', me.getStrValue(res.secretgrade));
                                //保管时间
                                rec.set('keepingtime', me.getStrValue(res.keepingtime));
                                //归档文件清单编码
                                rec.set('filelistcode', me.getStrValue(res.filelistcode));
                                //归档日期
                                rec.set('filelisttime', me.getStrValue(res.filelisttime));
                                //排架号
                                rec.set('racknumber', me.getStrValue(res.racknumber));
                                //备注
                                rec.set('note', me.getStrValue(res.note));

                                //是否新建文件编码
                                rec.set('needNewFileCode', false);//me.getStrValue(res.needNewFileCode));
                                //文件编码类型
                                rec.set('fileCodeType', me.getStrValue(res.fileCodeType));

                                //文件类型
                                rec.set('receiveType', me.getStrValue(res.receiveType));
                                //流水号
                                rec.set('fNumber', me.getStrValue(res.fNumber));
                                //版本
                                rec.set('edition', me.getStrValue(res.edition));

                                //工作分类代码
                                rec.set('workClass', me.getStrValue(res.workClass));
                                //工作分项代码
                                rec.set('workSub', me.getStrValue(res.workSub));
                                //部门代码
                                rec.set('department', me.getStrValue(res.department));

                                //运营类批量导入的属性
                                //编制人
                                rec.set('design', me.getStrValue(res.design));
                                //生效日期
                                rec.set('approvtime', me.getStrValue(res.approvtime));
                                //版本
                                rec.set('edition', me.getStrValue(res.edition));
                                //主送方
                                rec.set('mainfeeder', me.getStrValue(res.mainfeeder));
                                //抄送方
                                rec.set('copy', me.getStrValue(res.copy));
                                //发送日期
                                rec.set('senddate', me.getStrValue(res.senddate));
                                //是否要求回复
                                rec.set('ifreply', me.getStrValue(res.ifreply));
                                //回复时限
                                rec.set('replydate', me.getStrValue(res.replydate));
                                //回文编码
                                rec.set('replycode', me.getStrValue(res.replycode));
                                //回文日期
                                rec.set('replytime', me.getStrValue(res.replytime));

                                rec.commit();

                                bSuccMatch = true;
                                break;
                            }


                        }

                        if (bSuccMatch == false) {
                            if (code != undefined) {
                                failCount = failCount + 1;
                                if (firstLog === true) {
                                    firstLog = false;
                                    //me.contentText.setValue(me.contentText.value + "导入失败的文件：\n");
                                }
                                failInfo = failInfo + code + " " + desc + "\n";

                            }
                        }

                    }

                    me.contentText.setValue("成功导入" + succCount.toString() + "个文件属性\n" +
                        "导入失败" + failCount.toString() + "个文件属性：\n" + failInfo);

                } else {
                    var errmsg = resp.msg;
                    Ext.Msg.alert("错误信息", errmsg);
                }
            },
            failure: function (response, options) {
                Ext.MessageBox.close();//关闭等待对话框
            }
        })
    },

    onDownLoadFailFile: function () {
        var me = this;
        if (me.docList === "") return;

  
        //获取文件列表
        var fileArray = [];

        //遍历已选择文件的表格
        var mpanel = Ext.getCmp(me.mainPanelId);
        var mpFileStore = mpanel.filestore;
        for (var i = 0; i < mpFileStore.getCount() ; i++) {
            var record = mpFileStore.getAt(i);
            var fn = record.get('name');
            var fc = record.get('code');
            var fd = record.get('desc');
           
            var fa =
               { fn: fn, fc: fc, fd: fd };

            fileArray.push(fa);
        }
        var fileListJson = Ext.JSON.encode(fileArray);

        Ext.MessageBox.wait("正在获取表格，请稍候...", "等待");

        Ext.Ajax.request({

            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.HXEPC_Plugins.Document", A: "GetFailAttrFromExcel",
                sid: localStorage.getItem("sid"), docKeyword: me.docKeyword,
                FileListJson: fileListJson
            },
            success: function (res, options) {

                Ext.MessageBox.close();//关闭等待对话框

                var obj = Ext.decode(res.responseText);
                //console.log(obj);//可以到火狐的firebug下面看看obj里面的结构   
                if (obj.success == true) {
                    var prePath = obj.data[0].prePath;
                    var fileName = obj.data[0].filename;
                    var para = obj.data[0].para;

                    //组装文件下载路径
                    var url = prePath + encodeURIComponent(fileName) + "?p=" + para;

                    //var url = encodeURIComponent(obj.data[0].path);

                    //window.open(url, '_blank');//新窗口打开链接
                    var popUp = window.open(url, '_blank');//新窗口打开链接
                    if (popUp == null || typeof (popUp) == 'undefined') {
                        Ext.Msg.alert("下载失败", '请解除窗口阻拦，重新点击下载。');
                    }
                    else {
                        popUp.focus();
                    }

                } else { Ext.Msg.alert("下载失败", obj.msg); }

      
            },
            failure: function (response, options) {
                Ext.MessageBox.close();//关闭等待对话框
            }
        });

    },

    sendDeleteDoc: function (projectId, docNodes, nodesIndex) {
        var me = this;
        var docId = docNodes[nodesIndex];
       // var docId = docKeyword;
        sureDel = "true";
        Ext.Ajax.request({
            url: 'WebApi/Post',
            method: "POST",
            params: {
                C: "AVEVA.CDMS.WebApi.DocController", A: "DeleteDoc",
                ProjectKeyword: projectId, DocKeyword: docId,
                sureDel: sureDel, sid: localStorage.getItem("sid")
            },
            success: function (response, options) {
                var res = Ext.JSON.decode(response.responseText, true);
                var state = res.success;
                if (state === false) {
                    var errmsg = res.msg;
                   // Ext.Msg.alert("错误信息", errmsg);
                }
                else {
                    var recod = res.data[0];
                    var State = recod.state;
                    if (State === "delSuccess") {
                        if (docNodes.length > nodesIndex + 1) {
                            me.sendDeleteDoc(projectId, docNodes, nodesIndex + 1);
                        } else {
                           // me.loadDocListStore(function () { });
                        }

                    }
                }
            },
            failure: function (response, options) {
                ////Ext.Msg.alert("错误", "连接服务器失败！<br>" + response.responseText);
            }
        });
    },

    getStrValue: function (str) {
        return  str===undefined?"":str;
    }
});


