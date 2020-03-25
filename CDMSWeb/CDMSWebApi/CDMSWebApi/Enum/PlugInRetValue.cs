namespace AVEVA.CDMS.WebApi
{
    using System;

    public class PlugInRetValue
    {
        private bool bRetValue;

        public bool RetValue
        {
            get
            {
                return this.bRetValue;
            }
            set
            {
                this.bRetValue = value;
            }
        }
    }
}

