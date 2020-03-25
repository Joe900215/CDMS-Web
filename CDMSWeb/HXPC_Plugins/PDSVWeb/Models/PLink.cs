namespace PDSVWeb.Models
{
    using System;
    using System.ComponentModel;
    using System.Data.Linq.Mapping;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;

    [Table(Name="dbo.PLink")]
    public class PLink : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private int _Id;
        private string _LinkUrl;
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

        [Column(Storage="_LinkUrl", DbType="Text NOT NULL", CanBeNull=false, UpdateCheck=UpdateCheck.Never)]
        public string LinkUrl
        {
            get
            {
                return this._LinkUrl;
            }
            set
            {
                if (this._LinkUrl != value)
                {
                    this.SendPropertyChanging();
                    this._LinkUrl = value;
                    this.SendPropertyChanged("LinkUrl");
                }
            }
        }
    }
}

