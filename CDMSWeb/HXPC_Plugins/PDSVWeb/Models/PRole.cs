namespace PDSVWeb.Models
{
    using System;
    using System.ComponentModel;
    using System.Data.Linq;
    using System.Data.Linq.Mapping;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;

    [Table(Name="dbo.PRole")]
    public class PRole : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private bool _CheckProject;
        private bool _DownLoadPerm;
        private int _Id;
        private bool _PermissionManager;
        private EntitySet<PDSVWeb.Models.PUser> _PUser;
        private string _RoleName;
        private bool _UploadPerm;
        private EntitySet<PDSVWeb.Models.UserLinkInfo> _UserLinkInfo;
        private bool _UserManager;
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(string.Empty);

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event PropertyChangedEventHandler PropertyChanged;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event PropertyChangingEventHandler PropertyChanging;

        public PRole()
        {
            this._UserLinkInfo = new EntitySet<PDSVWeb.Models.UserLinkInfo>(new Action<PDSVWeb.Models.UserLinkInfo>(this.attach_UserLinkInfo), new Action<PDSVWeb.Models.UserLinkInfo>(this.detach_UserLinkInfo));
            this._PUser = new EntitySet<PDSVWeb.Models.PUser>(new Action<PDSVWeb.Models.PUser>(this.attach_PUser), new Action<PDSVWeb.Models.PUser>(this.detach_PUser));
        }

        private void attach_PUser(PDSVWeb.Models.PUser entity)
        {
            this.SendPropertyChanging();
            entity.PRole = this;
        }

        private void attach_UserLinkInfo(PDSVWeb.Models.UserLinkInfo entity)
        {
            this.SendPropertyChanging();
            entity.PRole = this;
        }

        private void detach_PUser(PDSVWeb.Models.PUser entity)
        {
            this.SendPropertyChanging();
            entity.PRole = null;
        }

        private void detach_UserLinkInfo(PDSVWeb.Models.UserLinkInfo entity)
        {
            this.SendPropertyChanging();
            entity.PRole = null;
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

        [Column(Storage="_CheckProject", DbType="Bit NOT NULL")]
        public bool CheckProject
        {
            get
            {
                return this._CheckProject;
            }
            set
            {
                if (this._CheckProject != value)
                {
                    this.SendPropertyChanging();
                    this._CheckProject = value;
                    this.SendPropertyChanged("CheckProject");
                }
            }
        }

        [Column(Storage="_DownLoadPerm", DbType="Bit NOT NULL")]
        public bool DownLoadPerm
        {
            get
            {
                return this._DownLoadPerm;
            }
            set
            {
                if (this._DownLoadPerm != value)
                {
                    this.SendPropertyChanging();
                    this._DownLoadPerm = value;
                    this.SendPropertyChanged("DownLoadPerm");
                }
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

        [Column(Storage="_PermissionManager", DbType="Bit NOT NULL")]
        public bool PermissionManager
        {
            get
            {
                return this._PermissionManager;
            }
            set
            {
                if (this._PermissionManager != value)
                {
                    this.SendPropertyChanging();
                    this._PermissionManager = value;
                    this.SendPropertyChanged("PermissionManager");
                }
            }
        }

        [Association(Name="PRole_PUser", Storage="_PUser", ThisKey="Id", OtherKey="RoleId")]
        public EntitySet<PDSVWeb.Models.PUser> PUser
        {
            get
            {
                return this._PUser;
            }
            set
            {
                this._PUser.Assign(value);
            }
        }

        [Column(Storage="_RoleName", DbType="VarChar(20) NOT NULL", CanBeNull=false)]
        public string RoleName
        {
            get
            {
                return this._RoleName;
            }
            set
            {
                if (this._RoleName != value)
                {
                    this.SendPropertyChanging();
                    this._RoleName = value;
                    this.SendPropertyChanged("RoleName");
                }
            }
        }

        [Column(Storage="_UploadPerm", DbType="Bit NOT NULL")]
        public bool UploadPerm
        {
            get
            {
                return this._UploadPerm;
            }
            set
            {
                if (this._UploadPerm != value)
                {
                    this.SendPropertyChanging();
                    this._UploadPerm = value;
                    this.SendPropertyChanged("UploadPerm");
                }
            }
        }

        [Association(Name="PRole_UserLinkInfo", Storage="_UserLinkInfo", ThisKey="Id", OtherKey="LinkId")]
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

        [Column(Storage="_UserManager", DbType="Bit NOT NULL")]
        public bool UserManager
        {
            get
            {
                return this._UserManager;
            }
            set
            {
                if (this._UserManager != value)
                {
                    this.SendPropertyChanging();
                    this._UserManager = value;
                    this.SendPropertyChanged("UserManager");
                }
            }
        }
    }
}

