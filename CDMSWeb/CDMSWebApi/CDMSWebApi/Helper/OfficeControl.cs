using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AVEVA.CDMS.WebApi
{
    public class OfficeControl
    {
        public Microsoft.Office.Interop.Excel.Application appExcel ;

    public OfficeControl() 
	{

	}


    ~OfficeControl()
	{
		try
		{

			if(appExcel != null )
			{

                //关闭Excel
                IntPtr ptr = new IntPtr(appExcel.Hwnd);
                uint pid = 0;
                WebCDMSOffice.GetWindowThreadProcessId(ptr, ref pid);

                try
                {
                    appExcel.Quit();
                    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(appExcel);

                }
                catch (Exception subEx)
                { }

                System.Diagnostics.Process appProcess = null;
                appProcess = System.Diagnostics.Process.GetProcessById((int)pid);

                if (appProcess != null)
                {
                    appProcess.Kill();
                }



                appExcel = null; 
			}

		}catch(Exception ex)
		{
            CDMS.Server.CDMS.Write(ex.ToString()); 
		}
	}

    }
}
