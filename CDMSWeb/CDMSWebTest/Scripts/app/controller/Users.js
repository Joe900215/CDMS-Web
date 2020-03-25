//定义用户管理控制器
Ext.define('CDMSWeb.controller.Users', {
    extend: 'Ext.app.Controller',
    //视图需要用到用户模型
    models: [
        'User'
    ],

    //视图需要用到用户及角色的Store(存储器)
    stores: [
        'Users',
        'Roles'
    ],

    //引用用户视图视图
    views: [
        //Users.View表示创建视图时，要在\Scripts\app\view目录下创建Users目录，然后再创建View.js文件
        'Users.View'
    ],

    //添加一些引用来获取视图和按钮,就可以通过get方法获得各控件
    //因为要在主面板的标签页面板内添加视图，因而需要引用标签页面板，因而要加入refs配置项
    refs: [
        //ref配置项生成引用的方法，可通过getUserPanel获取面板
        //而selector配置项就是面板的选择器，在这里使用它的id选择
        //这里的userPanel为主面板里用户管理panel的id
        //视图路径：\scripts\app\view\mainpanel.js
        { ref: "UserPanel", selector: "#userPanel" },
        //引用的视图路径：\Scripts\app\view\Users\View.js
        { ref: "UserView", selector: "#usersView" },
        //引用的视图路径：\Scripts\app\view\Users\View.js下面的buttonUserAdd
        { ref: "ButtonUserAdd", selector: "#buttonUserAdd" },
        ////引用删除用户按钮,路径：\Scripts\app\view\Users\View.js下面的buttonUserDelete
        { ref: "ButtonUserDelete", selector: "#buttonUserDelete" },
        
        { ref: "ButtonUserResetPassword", selector: "#buttonUserResetPassword" },
        //引用用户名的编辑组件，路径：\Scripts\app\view\Users\View.js下面的editorUsername
        { ref: "EditorUsername", selector: "#editorUsername" }
    ],

    //定义控制器时，都有1个init方法，在这里可以执行一些初始化操作
    init: function () {
        var me = this,
            //getUserPanel方法，就是refs配置项定义中自动生成的方法
            panel = me.getUserPanel(),
            //使用widget方法创建的视图
            view = Ext.widget("usersview");
        //将创建的用户视图通过add方法添加到面板
        panel.add(view);
        //使用getUserView返回用户视图后，调用on方法绑定selectionchange事件
        me.getUserView().on("selectionchange", me.onUserSelect, me);
        //完成添加操作，绑定click事件
        me.getButtonUserAdd().on("click", me.onAddUser, me);
        //在RowEditing有一个Edit事件，它会在编辑完成后触发，非常适合用来进行数据保存操作。
        //而且该事件会把事件冒泡到Grid，因而在Grid绑定该事件就行了
        me.getUserView().on("edit", me.onEditcomplete, me);
        //用户取消编辑时，要调用rejectChanges方法取消更改，这个通过监听canceledit事件就可以实现
        me.getUserView().on("canceledit", me.onCancelEdit, me);
        //定义RowEditing控件进入编辑之前的事件
        me.getUserView().on("beforeedit", me.onBeforeEdit, me);
        //为删除用户按钮绑定单击事件
        me.getButtonUserDelete().on("click", me.onDeleteUser, me);
        //为重置密码按钮绑定单击事件
        me.getButtonUserResetPassword().on("click", me.onResetPassword, me);
        me.control({
        });
    },

    //选择行后，设置两个按钮为开启状态
    onUserSelect: function (model, rs) {
        var me = this,
            length = rs.length;
        //利用get方法返回两个按钮后，调用对象的setDisabled方法设置其开启状态
        me.getButtonUserDelete().setDisabled(length == 0);
        me.getButtonUserResetPassword().setDisabled(length == 0);
    },

    //添加用户操作事件
    onAddUser: function () {
        var me = this
        //实例化编辑用户插件RowEditing
        //要注意plugins中的索引，因为当前示例只有一个插件，因而使用0就可以返回RowEditing实例了，如果有多个插件，要注意索引值
        edit = me.getUserView().plugins[0],
        //getUserModel是定义models配置项时自动生成的方法，可返回模型
            User = me.getUserModel();
        //先调用cancelEdit取消当前编辑操作，以避免在编辑过程中单击了添加按钮出现问题
        edit.cancelEdit();
        //在Store中添加一条记录
        //getUsersStore方法也是自动生成的，用于返回Store
        //Store路径：\Scripts\app\store\Users.js
        me.getUsersStore().insert(0, new User);
        //调用startEdit方法进入编辑状态
        edit.startEdit(0, 0);
    },

    //用户完成编辑事件
    onEditcomplete: function (editor, e) {
        var me = this;
        //调用Store的sync方法将数据同步到服务器
        me.getUsersStore().sync({
            //如果同步操作，服务器返回success为true，则调用commitChanges方法确认修改
            success: function (e, opt) {
                var me = this;
                me.getUsersStore().commitChanges();
            },
            //如果失败，则调用rejectChanges方法取消修改，并显示错误信息
            failure: function (e, opt) {
                var me = this, msg = "";
                me.getUsersStore().rejectChanges();
                Ext.Msg.alert("错误", e.exceptions[0].error);
            },
            scope: me
        });
    },

    //用户取消编辑事件
    onCancelEdit: function () {
        var me = this;
        me.getUsersStore().rejectChanges();
    },

    //进入编辑之前的事件
    onBeforeEdit: function (editor, e) {
        var me = this;
        //设置在编辑的时候，不允许用户修改用户名（Membership不允许修改用户名（id））
        if (e.record.data.id) {
            me.getEditorUsername().setDisabled(true);
        } else {
            me.getEditorUsername().setDisabled(false);
        }
    },

    //删除用户按钮单击事件
    onDeleteUser: function () {
        var me = this,
            //从Store的选择模型获取选择的记录
    		sm = me.getUserView().getSelectionModel();
        if (sm.getCount() > 0) {
            var rs = sm.getSelection();
            //使用了数组content来组合确认信息
            content = ["确定删除以下用户？"];
            for (var i = 0; ln = rs.length, i < ln; i++) {
                content.push(rs[i].data.Username);
            }
            //提示用户是否真的删除用户
            Ext.Msg.confirm("删除用户", content.join("<br/>"), function (btn) {
                if (btn == "yes") {
                    var me = this,
                        rs = me.getUserView().getSelectionModel().getSelection()
                    //getUsersStore方法是自动生成的，用于返回Store
                    //Store路径：\Scripts\app\store\Users.js
                    store = me.getUsersStore();
                    //remove方法删除记录
                    store.remove(rs);
                    //调用sync方法与服务端同步数据
                    store.sync({
                        success: function (e, opt) {
                            var me = this;
                            //如果成功，调用commitChanges方法确认修改
                            me.getUsersStore().commitChanges();
                        },
                        failure: function (e, opt) {
                            var me = this;
                            //如果失败，调用rejectChanges方法取消修改
                            me.getUsersStore().rejectChanges()
                            Ext.Msg.alert("发生错误", e.exceptions[0].error);
                        },
                        scope: me
                    });
                }
            }, me)
        } else {
            Ext.Msg.alert('删除用户', '请选择要删除的用户。');
        }
    },

    //重置密码按钮单击事件
    onResetPassword: function () {
        var me = this,
            rs = me.getUserView().getSelectionModel().getSelection();
        if (rs.length > 0) {
            //提取id
            var idList = [];
            for (var i = rs.length - 1; i >= 0; i--) {
                idList.push(rs[i].data.id);
            }
            //重置密码与删除用户差不多，也是从选择模型获取选择记录。
            //不过，这次，不能用sync同步，只能通过提取id，然后使用Ajax方法提交数据
            Ext.Ajax.request({
                params: { id: idList, sid: localStorage.getItem("sid") },
                url: '/Users/ResetPassword',
                scripts: true,
                scope: me,
                success: function (response, opt) {
                    //调用decode方法将数据转换为对象
                    var obj = Ext.JSON.decode(response.responseText);
                    if (obj) {
                        //根据success的值来做处理
                        if (obj.success) {
                            Ext.Msg.alert("提示信息", "重置密码成功");
                            return;
                        } else {
                            Ext.Msg.alert("错误",obj.Msg);
                        }
                    }
                },
                failure: function (response, options) {
                    Ext.Msg.alert("错误", "重置密码失败！<br>错误信息：" + response.responseText);
                }
            });
        }
    }


});
