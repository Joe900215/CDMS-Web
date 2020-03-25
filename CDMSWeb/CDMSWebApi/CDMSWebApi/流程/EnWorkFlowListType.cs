using System;

namespace AVEVA.CDMS.WebApi
{
    [Serializable]
    public enum enWorkFlowPlaceType
    {
        ProcessWorkFlow = 5,    //正在处理流程
        PlanWorkFlow = 6,       //参与工作流
        FinishWorkFlow = 7,     //完成工作流
        ErrorWorkFlow = 8,        //异常工作流
    }
}
