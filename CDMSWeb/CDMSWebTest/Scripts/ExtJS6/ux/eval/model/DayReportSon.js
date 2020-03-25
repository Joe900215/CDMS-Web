//定义事件类
Ext.define('Ext.ux.eval.model.DayReportSon', {
    extend: 'Ext.util.Observable', // 添加事件需要继承自Ext.util.Observable类
    //mixins: ['Ext.util.Observable'], // mixins必须和extend配合使用

    config: {
        dayreporteventid: '',
        departmentid: ''
    },

    // 构造函数
    constructor: function (cfg) {

        this.callParent(cfg); //替代Ext.util.Observable.constructor.call(this, config);
        this.initConfig(cfg);

        //dayReportSonModel.fireEvent('beforestart'); // 触发dayreportsonupdate.jsp生成的dayReportSonModel添加的afterchange事件;
    }
});