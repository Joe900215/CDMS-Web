//Ext.onReady(function () {
    Ext.define('Ext.plug_ins.WTMS_Plugins.Api.workTaskApi', {
        canSay: function () {
            alert('cansay...');
        },

        //创建工作任务
        createWorkTask: function (projKeyword, title, docAttrJson, async) {
            //var response = Ext.Ajax.request({
            //    url: 'WebApi/Post',
            //    method: "POST",
            //    async: async, //是否异步调用
            //    params: {
            //        C: "AVEVA.CDMS.WTMS_Plugins.WorkTask", A: "CreateWorkTask",
            //        ProjectKeyword: projKeyword,
            //        title: title, docAttrJson: docAttrJson,
            //        sid: localStorage.getItem("sid")
            //    }
            //});
            ////return Ext.decode(response, true);
            //return response;

            return new Ext.Promise(function (resolve, reject) {
                Ext.Ajax({
                    url: 'WebApi/Post',
                    method: "POST",
                    params: {
                        C: "AVEVA.CDMS.WTMS_Plugins.WorkTask", A: "CreateWorkTask",
                        ProjectKeyword: projKeyword,
                        title: title, docAttrJson: docAttrJson,
                        sid: localStorage.getItem("sid")
                    },
                    success: function (response) {
                        resolve(response.responseText);
                    },
                    failure: function (response) {
                        reject(response.status);
                    }
                });
            });
        }
    });
//});
