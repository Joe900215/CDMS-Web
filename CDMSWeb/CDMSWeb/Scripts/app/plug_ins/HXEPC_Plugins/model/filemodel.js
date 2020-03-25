//定义著录表文档模型
Ext.define('CDMSWeb.plug_ins.HXEPC_Plugins.model.filemodel', {
    extend: 'Ext.data.Model',
    fields: ["id", "no", "name", "code", "origcode", "desc"
    , "reference"
, "volumenumber"
, "responsibility"
, "page"
, "share"
, "medium"
, "languages"
, "proname"
, "procode"
, "major"
, "crew"
, "factorycode"
, "factoryname"
, "systemcode"
, "systemname"
, "relationfilecode"
, "relationfilename"
, "filespec"
, "fileunit"
, "secretgrade"
, "keepingtime"
, "filelistcode"
, "filelisttime"
, "racknumber"
, "note",
"needNewFileCode", "fileCodeType",
"receiveType", "fNumber", "edition",
"workClass", "workSub", "department",
"docKeyword"
    ],
    proxy: {
        type: "ajax",
        //url: "Content/Details",
        reader: {
            type: 'json',
            root: "data",
            messageProperty: "Msg",
            resdc: "resdc"
        }
    },
    idProperty: "AttributeId"
,
    url: "_blank",
});
