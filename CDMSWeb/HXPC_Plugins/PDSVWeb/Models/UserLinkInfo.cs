namespace PDSVWeb.Models
{
    using System;
    using System.ComponentModel;
    using System.Data.Linq;
    using System.Data.Linq.Mapping;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;

    [Table(Name="dbo.UserLinkInfo")]
    public class UserLinkInfo : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private int _LinkId;
        private EntityRef<PDSVWeb.Models.PRole> _PRole = new EntityRef<PDSVWeb.Models.PRole>();
        private EntityRef<PDSVWeb.Models.PUser> _PUser = new EntityRef<PDSVWeb.Models.PUser>();
        private int _UserId;
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(string.Empty);

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event PropertyChangedEventHandler PropertyChanged;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event PropertyChangingEventHandler PropertyChanging;

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

        [Column(Storage="_LinkId", DbType="Int NOT NULL", IsPrimaryKey=true)]
        public int LinkId
        {
            get
            {
                return this._LinkId;
            }
            set
            {
                if (this._LinkId != value)
                {
                    if (this._PRole.HasLoadedOrAssignedValue)
                    {
                        throw new ForeignKeyReferenceAlreadyHasValueException();
                    }
                    this.SendPropertyChanging();
                    this._LinkId = value;
                    this.SendPropertyChanged("LinkId");
                }
            }
        }

        [Association(Name="PRole_UserLinkInfo", Storage="_PRole", ThisKey="LinkId", OtherKey="Id", IsForeignKey=true)]
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
                        entity.UserLinkInfo.Remove(this);
                    }
                    this._PRole.Entity = value;
                    if (value != null)
                    {
                        value.UserLinkInfo.Add(this);
                        this._LinkId = value.Id;
                    }
                    else
                    {
                        this._LinkId = 0;
                    }
                    this.SendPropertyChanged("PRole");
                }
            }
        }

        [Association(Name="PUser_UserLinkInfo", Storage="_PUser", ThisKey="UserId", OtherKey="Id", IsForeignKey=true)]
        public PDSVWeb.Models.PUser PUser
        {
            get
            {
                return this._PUser.Entity;
            }
            set
            {
                PDSVWeb.Models.PUser entity = this._PUser.Entity;
                if ((entity != value) || !this._PUser.HasLoadedOrAssignedValue)
                {
                    this.SendPropertyChanging();
                    if (entity != null)
                    {
                        this._PUser.Entity = null;
                        entity.UserLinkInfo.Remove(this);
                    }
                    this._PUser.Entity = value;
                    if (value != null)
                    {
                        value.UserLinkInfo.Add(this);
                        this._UserId = value.Id;
                    }
                    else
                    {
                        this._UserId = 0;
                    }
                    this.SendPropertyChanged("PUser");
                }
            }
        }

        [Column(Storage="_UserId", DbType="Int NOT NULL", IsPrimaryKey=true)]
        public int UserId
        {
            get
            {
                return this._UserId;
            }
            set
            {
                if (this._UserId != value)
                {
                    if (this._PUser.HasLoadedOrAssignedValue)
                    {
                        throw new ForeignKeyReferenceAlreadyHasValueException();
                    }
                    this.SendPropertyChanging();
                    this._UserId = value;
                    this.SendPropertyChanged("UserId");
                }
            }
        }
    }
}

