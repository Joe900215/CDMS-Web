namespace PDSVWeb.Models
{
    using System;
    using System.ComponentModel;
    using System.Data.Linq;
    using System.Data.Linq.Mapping;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;

    [Table(Name="dbo.Project")]
    public class Project : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private string _About;
        private string _AreaId;
        private string _BIMDataBaseDir;
        private string _BIMModelDir;
        private int _Id;
        private string _ProjectGuid;
        private EntityRef<PDSVWeb.Models.ProjectInfo> _ProjectInfo = new EntityRef<PDSVWeb.Models.ProjectInfo>();
        private string _ProjectName;
        private EntityRef<PDSVWeb.Models.ProjectTechInfo> _ProjectTechInfo = new EntityRef<PDSVWeb.Models.ProjectTechInfo>();
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

        [Column(Storage="_About", DbType="Text NOT NULL", CanBeNull=false, UpdateCheck=UpdateCheck.Never)]
        public string About
        {
            get
            {
                return this._About;
            }
            set
            {
                if (this._About != value)
                {
                    this.SendPropertyChanging();
                    this._About = value;
                    this.SendPropertyChanged("About");
                }
            }
        }

        [Column(Storage="_AreaId", DbType="VarChar(100) NOT NULL", CanBeNull=false)]
        public string AreaId
        {
            get
            {
                return this._AreaId;
            }
            set
            {
                if (this._AreaId != value)
                {
                    this.SendPropertyChanging();
                    this._AreaId = value;
                    this.SendPropertyChanged("AreaId");
                }
            }
        }

        [Column(Storage="_BIMDataBaseDir", DbType="Text", UpdateCheck=UpdateCheck.Never)]
        public string BIMDataBaseDir
        {
            get
            {
                return this._BIMDataBaseDir;
            }
            set
            {
                if (this._BIMDataBaseDir != value)
                {
                    this.SendPropertyChanging();
                    this._BIMDataBaseDir = value;
                    this.SendPropertyChanged("BIMDataBaseDir");
                }
            }
        }

        [Column(Storage="_BIMModelDir", DbType="Text NOT NULL", CanBeNull=false, UpdateCheck=UpdateCheck.Never)]
        public string BIMModelDir
        {
            get
            {
                return this._BIMModelDir;
            }
            set
            {
                if (this._BIMModelDir != value)
                {
                    this.SendPropertyChanging();
                    this._BIMModelDir = value;
                    this.SendPropertyChanged("BIMModelDir");
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

        [Column(Storage="_ProjectGuid", DbType="Text", UpdateCheck=UpdateCheck.Never)]
        public string ProjectGuid
        {
            get
            {
                return this._ProjectGuid;
            }
            set
            {
                if (this._ProjectGuid != value)
                {
                    this.SendPropertyChanging();
                    this._ProjectGuid = value;
                    this.SendPropertyChanged("ProjectGuid");
                }
            }
        }

        [Association(Name="Project_ProjectInfo", Storage="_ProjectInfo", ThisKey="Id", OtherKey="Id", IsUnique=true, IsForeignKey=false)]
        public PDSVWeb.Models.ProjectInfo ProjectInfo
        {
            get
            {
                return this._ProjectInfo.Entity;
            }
            set
            {
                PDSVWeb.Models.ProjectInfo entity = this._ProjectInfo.Entity;
                if ((entity != value) || !this._ProjectInfo.HasLoadedOrAssignedValue)
                {
                    this.SendPropertyChanging();
                    if (entity != null)
                    {
                        this._ProjectInfo.Entity = null;
                        entity.Project = null;
                    }
                    this._ProjectInfo.Entity = value;
                    if (value != null)
                    {
                        value.Project = this;
                    }
                    this.SendPropertyChanged("ProjectInfo");
                }
            }
        }

        [Column(Storage="_ProjectName", DbType="Text NOT NULL", CanBeNull=false, UpdateCheck=UpdateCheck.Never)]
        public string ProjectName
        {
            get
            {
                return this._ProjectName;
            }
            set
            {
                if (this._ProjectName != value)
                {
                    this.SendPropertyChanging();
                    this._ProjectName = value;
                    this.SendPropertyChanged("ProjectName");
                }
            }
        }

        [Association(Name="Project_ProjectTechInfo", Storage="_ProjectTechInfo", ThisKey="Id", OtherKey="Id", IsUnique=true, IsForeignKey=false)]
        public PDSVWeb.Models.ProjectTechInfo ProjectTechInfo
        {
            get
            {
                return this._ProjectTechInfo.Entity;
            }
            set
            {
                PDSVWeb.Models.ProjectTechInfo entity = this._ProjectTechInfo.Entity;
                if ((entity != value) || !this._ProjectTechInfo.HasLoadedOrAssignedValue)
                {
                    this.SendPropertyChanging();
                    if (entity != null)
                    {
                        this._ProjectTechInfo.Entity = null;
                        entity.Project = null;
                    }
                    this._ProjectTechInfo.Entity = value;
                    if (value != null)
                    {
                        value.Project = this;
                    }
                    this.SendPropertyChanged("ProjectTechInfo");
                }
            }
        }
    }
}

