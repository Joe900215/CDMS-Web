using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using AVEVA.CDMS.Server;

namespace AVEVA.CDMS.WebApi
{
        public class ExWebMenu
        {
            
            private string _MenuId;//菜单Id
            private string _MenuName;//菜单名称
            private enWebMenuType _MenuType;//菜单类型
            private enWebMenuPosition _MenuPosition;//菜单位置
            private AVEVA.CDMS.Server.Right _Right;//菜单权限
            private enWebTVMenuPosition _TvMenuPosition;//
            private enWebMenuState _MenuState;
            private List<Doc> selDocList = new List<Doc>();
            private List<object> selObjectList = new List<object>();
            private List<Project> selProjectList = new List<Project>();
            private User _LoginUser;
            private string _Sid;
            //private string _LoginUserCode;
            private JObject _Value;

            public virtual enWebMenuState MeasureMenuInitState()
            {
                return enWebMenuState.Hide;
            }

            public virtual enWebMenuState MeasureMenuState()
            {
                return enWebMenuState.Enabled;
            }

            public string MenuId
            {
                get
                {
                    return _MenuId;
                }
                set
                {
                    _MenuId = value;
                }
            }

            
            public string MenuName
            {
                get
                {
                    return _MenuName;
                }
                set
                {
                    _MenuName = value;
                }
            }

            
            public enWebMenuType MenuType
            {
                get
                {
                    return _MenuType;
                }
                set
                {
                    _MenuType = value;
                }
            }

            
            public enWebMenuPosition MenuPosition
            {
                get
                {
                    return _MenuPosition;
                }
                set
                {
                    _MenuPosition = value;
                }
            }


            public AVEVA.CDMS.Server.Right Right
            {
                get
                {
                    if (this._Right == null)
                    {
                        return null;
                    }
                    return this._Right;
                }
                set
                {
                    this._Right = value;
                }
            }

            public enWebTVMenuPosition TVMenuPositon
            {
                get
                {
                    return this._TvMenuPosition;
                }
                set
                {
                    this._TvMenuPosition = value;
                }
            }

            public enWebMenuState MenuState
            {
                get
                {
                    return this._MenuState;
                }
                set
                {
                    this._MenuState = value;
                }
            }

            public List<Doc> SelDocList
            {
                get
                {
                    if (this.selDocList == null)
                    {
                        return null;
                    }
                    return this.selDocList;
                }
                set
                {
                    this.selDocList = value;
                }
            }

            public List<object> SelObjectList
            {
                get
                {
                    return this.selObjectList;
                }
                set
                {
                    this.selObjectList = value;
                }
            }

            public List<Project> SelProjectList
            {
                get
                {
                    if (this.selProjectList == null)
                    {
                        return null;
                    }
                    return this.selProjectList;
                }
                set
                {
                    this.selProjectList = value;
                }
            }

        //public string LoginUserCode
        //{
        //    get
        //    {
        //        if (this.LoginUserCode == null)
        //        {
        //            return null;
        //        }
        //        return this._LoginUserCode;
        //    }
        //    set
        //    {
        //        this._LoginUserCode = value;
        //    }
        //}

        public User LoginUser
        {
            get
            {
                if (this._LoginUser == null)
                {
                    return null;
                }
                return this._LoginUser;
            }
            //set
            //{
            //    this._LoginUser = value;
            //}
        }

        public string Sid
        {
            get
            {
                return this._Sid;
            }
            set
            {
                User user = DBSourceController.GetUserBySid(value);
                this._LoginUser = user;
                this._Sid = value;
            }
        }

            public JObject Value
            {
                get {
                    string menuState = "Hide";
                    if (this.MenuState == enWebMenuState.Disabled) menuState = "Disabled";
                    else if (this.MenuState == enWebMenuState.Enabled) menuState = "Enabled";

                    if (this._MenuId == null) this._MenuId = "";
                    if (this._MenuName == null) this._MenuName = "";

                    return new JObject(new JProperty("Id", this._MenuId),
                    new JProperty("Name", this._MenuName),
                    new JProperty("State", menuState)
                );
                }
                set
                {
                    this._Value = value;
                }

            }
        }
}
