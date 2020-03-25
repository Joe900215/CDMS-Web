Ext.define("My.test.Animal", {
    height: 0,
    weight: 0
});
Ext.define('Ext.ux.Common.common', {
    //  extend: 'Ext.container.Container',
        //普通子段
        name: "",

        //属性
        config: {
            age: 0,
            father: {
                name: "",
                age: 0
            }
        },

        //构造方法
        constructor: function (name, height) {
            this.self.count++;
            if (name) this.name = name;
            if (height) this.height = height;

        },

        //继承
        extend: "My.test.Animal",

        //height: 0,
        //weight: 0,

        //实例方法
        Say: function () {
            alert("你好，我是：" + this.name + ",我今年" + this.age + "岁,我的身高是：" + this.height
    　　　　　　+ "。我的爸爸是：" + this.father.name + "，他" + this.father.age + "岁。");
        },
        

        //静态子段，方法
        statics: {
            type: "高等动物",
            count: 0,
            getCount: function () {
                return "当前共有" + this.count + "人";
            }
        },

        ////展开指定Project,
        //ExpendProject: function (ProjectId) {
        //    var viewTree = Ext.getCmp('_projectsTree');
        //    var viewTreeStore = viewTree.store;
        //    Ext.Ajax.request({
        //        //url: 'Project/GetProjectPath',
        //        url: 'WebApi/Post',
        //        method: "POST",
        //        params: {
        //            C: "AVEVA.CDMS.WebApi.ProjectController", A: "GetProjectPath",
        //            Keyword: ProjectId, sid: localStorage.getItem("sid")
        //        },
        //        success: function (response, options) {
        //            var res = Ext.JSON.decode(response.responseText);

        //            var record = res.data[0];
        //            //viewTree.collapseAll();//收缩所有子节点
        //            viewTree.expandPath(record.ProjectPath);
        //            //等待上一个函数的执行结果，查找点击树节点
        //            var count = 0, is_true = false;
        //            var node = viewTree.store.getNodeById(record.NodeId);

        //            setInterval(function () {
        //                if (!count) {
        //                    node = viewTree.store.getNodeById(record.NodeId);
        //                    if (Ext.isObject(node)) {
        //                        is_true = true;
        //                    }
        //                    count++;
        //                }

        //                if (is_true) {
        //                    viewTree.getSelectionModel().select(node);
        //                    viewTree.fireEvent('click', node);
        //                    is_true = false;
        //                }
        //            }, 1000);
        //        }
        //    });

        //},

 
})


