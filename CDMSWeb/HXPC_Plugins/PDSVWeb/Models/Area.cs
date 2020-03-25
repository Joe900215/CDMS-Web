namespace PDSVWeb.Models
{
    using System;
    using System.ComponentModel;
    using System.Data.Linq.Mapping;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;

    [Table(Name="dbo.Area")]
    public class Area : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private int _AreaLevel;
        private string _AreaName;
        private string _Id;
        private string _ParentId;
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

        [Column(Storage="_AreaLevel", DbType="Int NOT NULL")]
        public int AreaLevel
        {
            get
            {
                return this._AreaLevel;
            }
            set
            {
                if (this._AreaLevel != value)
                {
                    this.SendPropertyChanging();
                    this._AreaLevel = value;
                    this.SendPropertyChanged("AreaLevel");
                }
            }
        }

        [Column(Storage="_AreaName", DbType="VarChar(200) NOT NULL", CanBeNull=false)]
        public string AreaName
        {
            get
            {
                return this._AreaName;
            }
            set
            {
                if (this._AreaName != value)
                {
                    this.SendPropertyChanging();
                    this._AreaName = value;
                    this.SendPropertyChanged("AreaName");
                }
            }
        }

        [Column(Storage="_Id", DbType="VarChar(100) NOT NULL", CanBeNull=false, IsPrimaryKey=true)]
        public string Id
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

        [Column(Storage="_ParentId", DbType="VarChar(100) NOT NULL", CanBeNull=false)]
        public string ParentId
        {
            get
            {
                return this._ParentId;
            }
            set
            {
                if (this._ParentId != value)
                {
                    this.SendPropertyChanging();
                    this._ParentId = value;
                    this.SendPropertyChanged("ParentId");
                }
            }
        }
    }
}

