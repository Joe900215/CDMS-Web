//弹出消息框，选择设总
function selectProjectOwner(mainPanelId, res) {

    var plugins = res.data[0].plugins;
    var DefWorkFlow = res.data[0].DefWorkFlow;
    var CuWorkState = res.data[0].CuWorkState;
    var FuncName = res.data[0].FuncName;

    if (plugins === "HXPC_Plugins" && DefWorkFlow === "CREATEPROJECTFLOW" && CuWorkState === "DESIGNDEPARTMENT" && FuncName === "selectProjectOwner") {
        //Ext.Msg.alert("返回信息", plugins + "," + DefWorkFlow + "," + CuWorkState + "," + state);


        var fmSelectProjectOwner = Ext.create('Ext.plug_ins.' + plugins + '.' + FuncName, { title: "", mainPanelId: mainPanelId });

        winSelectProjectOwner = Ext.widget('window', {
            title: '角色任命',
            closeAction: 'hide',
            width: 417,
            height: 213,
            minWidth: 417,
            minHeight: 213,
            layout: 'fit',
            resizable: true,
            modal: true,
            closeAction: 'close', //close 关闭  hide  隐藏  
            items: fmSelectProjectOwner,
            defaultFocus: 'firstName'
        });

        winSelectProjectOwner.show();
        //监听子窗口关闭事件
        winSelectProjectOwner.on('close', function () {

        });
    }
}

//弹出消息框，选择设总
function selectDirector(mainPanelId, res) {

    var plugins = res.data[0].plugins;
    var DefWorkFlow = res.data[0].DefWorkFlow;
    var CuWorkState = res.data[0].CuWorkState;
    var FuncName = res.data[0].FuncName;

    if (plugins === "HXPC_Plugins" && DefWorkFlow === "CREATEPROJECTFLOW" && CuWorkState === "CONTROLDIRECTOR" && FuncName === "selectDirector") {

        var fmSelectDirector = Ext.create('Ext.plug_ins.' + plugins + '.' + FuncName, { title: "", mainPanelId: mainPanelId });

        winSelectDirector = Ext.widget('window', {
            title: '角色任命',
            closeAction: 'hide',
            width: 417,
            height: 213,
            minWidth: 417,
            minHeight: 213,
            layout: 'fit',
            resizable: true,
            modal: true,
            closeAction: 'close', //close 关闭  hide  隐藏  
            items: fmSelectDirector,
            defaultFocus: 'firstName'
        });

        winSelectDirector.show();
        //监听子窗口关闭事件
        winSelectDirector.on('close', function () {

        });
    }
}

//弹出消息框，选择参与专业
//调用本函数的位置在：Ext.ux.YDForm.WorkFlow._WorkFlowPage.loadWorkflowAuditPage函数里面的GotoNextWorkflowState的返回处理函数
function selectProfession(mainPanelId, res) {

    var plugins = res.data[0].plugins;
    var DefWorkFlow = res.data[0].DefWorkFlow;
    var CuWorkState = res.data[0].CuWorkState;
    var FuncName = res.data[0].FuncName;
    var projectDesc = res.data[0].projectDesc;
    var projectKeyword = res.data[0].projectKeyword;

    if (plugins === "HXPC_Plugins" && DefWorkFlow === "CREATEPROJECT" && CuWorkState === "DESIGNDIRECTOR" && FuncName === "selectProfession") {

        var fmSelectProfession = Ext.create('Ext.plug_ins.' + plugins + '.' + FuncName, {
            title: "", mainPanelId: mainPanelId, projectDesc: projectDesc, projectKeyword: projectKeyword, selectProfession,
                startAtWfBtn: "true"//记录是否在流程按钮里面调用的窗口
        });

        winSelectProfession = Ext.widget('window', {
            title: '选择参与专业',
            closeAction: 'hide',
            width: 550,
            height: 450,
            minWidth: 550,
            minHeight: 450,
            layout: 'fit',
            resizable: true,
            modal: true,
            closeAction: 'close', //close 关闭  hide  隐藏  
            items: fmSelectProfession,
            defaultFocus: 'firstName'
        });

        fmSelectProfession.send_get_profession_list(function () {
            winSelectProfession.show();
        });



        //监听子窗口关闭事件
        winSelectProfession.on('close', function () {

        });
    }


}

//弹出消息框，主任选择校核审核人员
//调用本函数的位置在：Ext.ux.YDForm.WorkFlow._WorkFlowPage.loadWorkflowAuditPage，里面的GotoNextWorkflowState的返回处理函数
function selectUserEx(mainPanelId, res) {
    var plugins = res.data[0].plugins;
    var DefWorkFlow = res.data[0].DefWorkFlow;
    var CuWorkState = res.data[0].CuWorkState;
    var FuncName = res.data[0].FuncName;
    var WfKeyword = res.data[0].WfKeyword;

    if (plugins === "HXPC_Plugins" && (DefWorkFlow === "EXCHANGEDOC" || DefWorkFlow == "SENDDOC" || DefWorkFlow == "WORKTASK") && CuWorkState === "DIRECTORSELECT" && FuncName === "selectUserEx") {
        //Ext.Msg.alert("返回信息", plugins + "," + DefWorkFlow + "," + CuWorkState + "," + state);
        var fmSelectUserEx = Ext.create('Ext.plug_ins.' + plugins + '.' + FuncName, { title: "", mainPanelId: mainPanelId, WfKeyword: WfKeyword });

        winSelectUserEx = Ext.widget('window', {
            title: '主任选择校审人员',
            closeAction: 'hide',
            width: 1068,
            height: 618,
            minWidth: 1068,
            minHeight: 618,
            layout: 'fit',
            resizable: true,
            modal: true,
            closeAction: 'close', //close 关闭  hide  隐藏  
            items: fmSelectUserEx,
            defaultFocus: 'firstName'
        });

        //fmSelectUserEx.send_get_profession_list(function () {
        //    winSelectUserEx.show();
        //});

        winSelectUserEx.show();


        //监听子窗口关闭事件
        winSelectUserEx.on('close', function () {

        });
    }
}

//弹出消息框，主任选择校核审核批准人员
//调用本函数的位置在：Ext.ux.YDForm.WorkFlow._WorkFlowPage.loadWorkflowAuditPage，里面的GotoNextWorkflowState的返回处理函数
function selectUserApprov(mainPanelId, res) {
    var plugins = res.data[0].plugins;
    var DefWorkFlow = res.data[0].DefWorkFlow;
    var CuWorkState = res.data[0].CuWorkState;
    var FuncName = res.data[0].FuncName;
    var WfKeyword = res.data[0].WfKeyword;

    if (plugins === "HXPC_Plugins" && (DefWorkFlow === "PRODUCT" || DefWorkFlow == "PUBLICPRODUCT" ) && CuWorkState === "DIRECTORSELECT" && FuncName === "selectUserApprov") {
        //Ext.Msg.alert("返回信息", plugins + "," + DefWorkFlow + "," + CuWorkState + "," + state);
        var fmSelectUserApprov = Ext.create('Ext.plug_ins.' + plugins + '.' + FuncName, { title: "", mainPanelId: mainPanelId, WfKeyword: WfKeyword });

        winSelectUserApprov = Ext.widget('window', {
            title: '主任选择校审人员',
            closeAction: 'hide',
            width: 1168,
            height: 618,
            minWidth: 1168,
            minHeight: 618,
            layout: 'fit',
            resizable: true,
            modal: true,
            closeAction: 'close', //close 关闭  hide  隐藏  
            items: fmSelectUserApprov,
            defaultFocus: 'firstName'
        });


        winSelectUserApprov.show();


        //监听子窗口关闭事件
        winSelectUserApprov.on('close', function () {

        });
    }
}

//弹出消息框，选择收文流程的参与专业
//调用本函数的位置在：Ext.ux.YDForm.WorkFlow._WorkFlowPage.loadWorkflowAuditPage，里面的GotoNextWorkflowState的返回处理函数
function receiveDocSelectProfession(mainPanelId, res) {
    var plugins = res.data[0].plugins;
    var DefWorkFlow = res.data[0].DefWorkFlow;
    var CuWorkState = res.data[0].CuWorkState;
    var FuncName = res.data[0].FuncName;
    var wfKeyword = res.data[0].WfKeyword;

    if (plugins === "HXPC_Plugins" && DefWorkFlow === "RECEIVEDOC" && CuWorkState === "PROJECTOWNER" && FuncName === "receiveDocSelectProfession") {
        //Ext.Msg.alert("返回信息", plugins + "," + DefWorkFlow + "," + CuWorkState + "," + state);
        var fmReceiveDocSelectProfession = Ext.create('Ext.plug_ins.' + plugins + '.' + FuncName, { title: "", mainPanelId: mainPanelId, wfKeyword: wfKeyword });

        winReceiveDocSelectProfession = Ext.widget('window', {
            title: '选择分发专业',
            closeAction: 'hide',
            width: 550,
            height: 450,
            minWidth: 550,
            minHeight: 450,
            layout: 'fit',
            resizable: true,
            modal: true,
            closeAction: 'close', //close 关闭  hide  隐藏  
            items: fmReceiveDocSelectProfession,
            defaultFocus: 'firstName'
        });

        fmReceiveDocSelectProfession.send_get_profession_list(function () {
            winReceiveDocSelectProfession.show();
        });



        //监听子窗口关闭事件
        winReceiveDocSelectProfession.on('close', function () {

        });
    }
}

//弹出消息框，选择成品校审流程的会签专业
//调用本函数的位置在：Ext.ux.Common.comm，里面的StartNewWorkFlow的返回处理函数
function productSelectProfession(mainPanelId, res) {
    var plugins = res.data[0].plugins;
    var DefWorkFlow = res.data[0].DefWorkFlow;
    var CuWorkState = res.data[0].CuWorkState;
    var FuncName = res.data[0].FuncName;
    var docKeyword = res.data[0].DocKeyword;

    if (plugins === "HXPC_Plugins" && DefWorkFlow === "PRODUCT" && CuWorkState === "START" && FuncName === "productSelectProfession") {
        //Ext.Msg.alert("返回信息", plugins + "," + DefWorkFlow + "," + CuWorkState + "," + state);
        var fmProductSelectProfession = Ext.create('Ext.plug_ins.' + plugins + '.' + FuncName, { title: "", mainPanelId: mainPanelId, docKeyword: docKeyword });

        winProductSelectProfession = Ext.widget('window', {
            title: '选择会签专业',
            closeAction: 'hide',
            width: 550,
            height: 430,
            minWidth: 550,
            minHeight: 430,
            layout: 'fit',
            resizable: true,
            modal: true,
            closeAction: 'close', //close 关闭  hide  隐藏  
            items: fmProductSelectProfession,
            defaultFocus: 'firstName'
        });

        fmProductSelectProfession.sendGetProductSignDefault(function () {
            winProductSelectProfession.show();
        });

        //winProductSelectProfession.show();

        //监听子窗口关闭事件
        winProductSelectProfession.on('close', function () {

        });
    }
}

////处理拖拽文件到DocGrid控件的事件__取消
//function onBeforeAddFile(mainPanelId, uploader, files) {
//    //获取父目录
//    var projectKeyword = uploader.multipart_params.ProjectKeyword;

//    //通过extjs的ajax获取操作全部名称
//    Ext.Ajax.request({
//        url: 'WebApi/Post',
//        method: "POST",
//        params: {
//            C: "AVEVA.CDMS.HXPC_Plugins.Document", A: "OnBeforeFileAddEvent",
//            sid: localStorage.getItem("sid"), ProjectKeyword: projectKeyword
//        },
//        success: function (response, options) {
//            //me.sendGetSelectUserExDefault_callback(response, options);//, funCallback);
//        }
//    });

//}

//处理拖拽文件到DocGrid控件的事件（收文）
//调用本函数的位置在：Ext.ux.YDForm._MainDocGrid 里面的 OnAfterCreateNewDocEvent 函数
//这里如果直接传递uploader,因为afterUpload事件延迟的关系，uploader里面的docKeyword会因为被新的文档覆盖而出现错误，所以不直接传送uploader了
function getDocument(mainPanelId, res,  files) {
    var plugins = res.data[0].plugins;
    var FuncName = res.data[0].FuncName;
    var DocKeyword = res.data[0].DocKeyword;

    if (plugins === "HXPC_Plugins" && FuncName === "getDocument") {
        //Ext.Msg.alert("返回信息", plugins + "," + DefWorkFlow + "," + CuWorkState + "," + state);
        var fmGetDocument = Ext.create('Ext.plug_ins.' + plugins + '.' + FuncName, { title: "", mainPanelId: mainPanelId, docKeyword: DocKeyword });

        winGetDocument = Ext.widget('window', {
            title: '收文处理',
            closeAction: 'hide',
            width: 558,
            height: 281,
            minWidth: 558,
            minHeight: 281,
            layout: 'fit',
            resizable: true,
            modal: true,
            closeAction: 'close', //close 关闭  hide  隐藏  
            items: fmGetDocument,
            defaultFocus: 'firstName'
        });

        //fmSelectUserEx.send_get_profession_list(function () {
        //    winSelectUserEx.show();
        //});

        winGetDocument.show();


        //监听子窗口关闭事件
        winGetDocument.on('close', function () {

        });
    }

}


//这一个定义必须保留在文件末尾
Ext.define('Ext.plug_ins.HXPC_Plugins.common', {
});
