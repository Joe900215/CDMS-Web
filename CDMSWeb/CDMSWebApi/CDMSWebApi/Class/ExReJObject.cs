using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AVEVA.CDMS.WebApi
{
    /// <summary>
    /// 
    /// </summary>
    public class ExReJObject
    {
        private bool _success;//返回是否操作成功
        private int _total;//返回记录总数
        private string _msg;//返回错误消息
        private JArray _data;//返回的数据
        private JObject _Value;//返回的值

        public bool success
        {
            get
            {
                //if (this._success == null)
                //{
                //    return false;
                //}
                return this._success;
            }
            set
            {
                this._success = value;
            }
        }

        public int total
        {
            get
            {
                //if (this._total == null)
                //{
                //    return 0;
                //}
                return this._total;
            }
            set
            {
                this._total = value;
            }
        }

        public string msg
        {
            get
            {
                if (this._msg == null)
                {
                    return "";
                }
                return this._msg;
            }
            set
            {
                this._msg = value;
            }
        }

        public JArray data
        {
            get
            {
                if (this._data == null)
                {
                    return new JArray();
                }
                return this._data;
            }
            set
            {
                this._data = value;
            }
        }

        public JObject Value
        {
            get
            {
                if (this._msg == null)
                {
                    this._msg = "";
                }
                if (this._data == null)
                {
                    this._data = new JArray();
                }
                return new JObject { 
                new JProperty("success",this._success),
                new JProperty("total",this._total),
                new JProperty("msg",this._msg),
                new JProperty("data",this._data)
            };
            }
            set
            {
                this._success=(bool)value["success"];
                this._total = (int)value["total"];
                this._msg = (string)value["msg"];
                this._data = (JArray)value["data"];
                this._Value = value;
            }
        }

        public static ExReJObject Decrypt(JObject value)
        {
            ExReJObject reJo = new ExReJObject();
            reJo.Value = value;
            return reJo;
        }
    }
}
