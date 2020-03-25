namespace PDSVWeb.Models
{
    using System;
    using System.ComponentModel;
    using System.Data.Linq;
    using System.Data.Linq.Mapping;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;

    [Table(Name="dbo.PUser")]
    public class PUser : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private int _Id;
        private EntityRef<PDSVWeb.Models.PRole> _PRole;
        private int _RoleId;
        private EntitySet<PDSVWeb.Models.UserLinkInfo> _UserLinkInfo;
        private string _UserName;
        private string _UserPwd;
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(string.Empty);

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event PropertyChangedEventHandler PropertyChanged;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event PropertyChangingEventHandler PropertyChanging;

        public PUser()
        {
            this._UserLinkInfo = new EntitySet<PDSVWeb.Models.UserLinkInfo>(new Action<PDSVWeb.Models.UserLinkInfo>(this.attach_UserLinkInfo), new Action<PDSVWeb.Models.UserLinkInfo>(this.detach_UserLinkInfo));
            this._PRole = new EntityRef<PDSVWeb.Models.PRole>();
        }

        private void attach_UserLinkInfo(PDSVWeb.Models.UserLinkInfo entity)
        {
            this.SendPropertyChanging();
            entity.PUser = this;
        }

        private void detach_UserLinkInfo(PDSVWeb.Models.UserLinkInfo entity)
        {
            this.SendPropertyChanging();
            entity.PUser = null;
        }

        protected virtual void SendPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected virtual void SendPropertyChanging()
        {
            if (this.PropertyChanging != null)
            {
                this.PropertyChanging(this, emptyChangingEventArgs);
            }
        }

        [Column(Storage="_Id", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
        public int Id
        {
            get
            {
                return this._Id;
            }
            set
            {
                if (this._Id != value)
                {
                    this.SendPropertyChanging();
                    this._Id = value;
                    this.SendPropertyChanged("Id");
                }
            }
        }

        [Association(Name="PRole_PUser", Storage="_PRole", ThisKey="RoleId", OtherKey="Id", IsForeignKey=true)]
        public PDSVWeb.Models.PRole PRole
        {
            get
            {
                return this._PRole.Entity;
            }
            set
            {
                PDSVWeb.Models.PRole entity = this._PRole.Entity;
                if ((entity != value) || !this._PRole.HasLoadedOrAssignedValue)
                {
                    this.SendPropertyChanging();
                    if (entity != null)
                    {
                        this._PRole.Entity = null;
                        entity.PUser.Remove(this);
                    }
                    this._PRole.Entity = value;
                    if (value != null)
                    {
                        value.PUser.Add(this);
                        this._RoleId = value.Id;
                    }
                    else
                    {
                        this._RoleId = 0;
                    }
                    this.SendPropertyChanged("PRole");
                }
            }
        }

        [Column(Storage="_RoleId", DbType="Int NOT NULL")]
        public int RoleId
        {
            get
            {
                return this._RoleId;
            }
            set
            {
                if (this._RoleId != value)
                {
                    if (this._PRole.HasLoadedOrAssignedValue)
                    {
                        throw new ForeignKeyReferenceAlreadyHasValueException();
                    }
                    this.SendPropertyChanging();
                    this._RoleId = value;
                    this.SendPropertyChanged("RoleId");
                }
            }
        }

        [Association(Name="PUser_UserLinkInfo", Storage="_UserLinkInfo", ThisKey="Id", OtherKey="UserId")]
        public EntitySet<PDSVWeb.Models.UserLinkInfo> UserLinkInfo
        {
            get
            {
                return this._UserLinkInfo;
            }
            set
            {
                this._UserLinkInfo.Assign(value);
            }
        }

        [Column(Storage="_UserName", DbType="VarChar(50) NOT NULL", CanBeNull=false)]
        public string UserName
        {
            get
            {
                return this._UserName;
            }
            set
            {
                if (this._UserName != value)
                {
                    this.SendPropertyChanging();
                    this._UserName = value;
                    this.SendPropertyChanged("UserName");
                }
            }
        }

        [Column(Storage="_UserPwd", DbType="VarChar(50) NOT NULL", CanBeNull=false)]
        public string UserPwd
        {
            get
            {
                return this._UserPwd;
            }
            set
            {
                if (this._UserPwd != value)
                {
                    this.SendPropertyChanging();
                    this._UserPwd = value;
                    this.SendPropertyChanged("UserPwd");
                }
            }
        }
    }
}

