namespace PDSVWeb.Models
{
    using System;
    using System.ComponentModel;
    using System.Data.Linq;
    using System.Data.Linq.Mapping;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;

    [Table(Name="dbo.ProjectInfo")]
    public class ProjectInfo : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private string _BuildType;
        private string _BuildUnit;
        private string _BuildUnitRepre;
        private string _ConstructUnit;
        private string _ConstructUnitRepre;
        private string _DesignUnit;
        private string _DesignUnitRepre;
        private int _Id;
        private string _OperateUnit;
        private string _OperateUnitRepre;
        private EntityRef<PDSVWeb.Models.Project> _Project = new EntityRef<PDSVWeb.Models.Project>();
        private string _ProjectAddress;
        private string _ProjectDate;
        private string _ProjectName;
        private string _SupervisorUnit;
        private string _SupervisorUnitRepre;
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

        [Column(Storage="_BuildType", DbType="Text", UpdateCheck=UpdateCheck.Never)]
        public string BuildType
        {
            get
            {
                return this._BuildType;
            }
            set
            {
                if (this._BuildType != value)
                {
                    this.SendPropertyChanging();
                    this._BuildType = value;
                    this.SendPropertyChanged("BuildType");
                }
            }
        }

        [Column(Storage="_BuildUnit", DbType="VarChar(100)")]
        public string BuildUnit
        {
            get
            {
                return this._BuildUnit;
            }
            set
            {
                if (this._BuildUnit != value)
                {
                    this.SendPropertyChanging();
                    this._BuildUnit = value;
                    this.SendPropertyChanged("BuildUnit");
                }
            }
        }

        [Column(Storage="_BuildUnitRepre", DbType="VarChar(50)")]
        public string BuildUnitRepre
        {
            get
            {
                return this._BuildUnitRepre;
            }
            set
            {
                if (this._BuildUnitRepre != value)
                {
                    this.SendPropertyChanging();
                    this._BuildUnitRepre = value;
                    this.SendPropertyChanged("BuildUnitRepre");
                }
            }
        }

        [Column(Storage="_ConstructUnit", DbType="VarChar(100)")]
        public string ConstructUnit
        {
            get
            {
                return this._ConstructUnit;
            }
            set
            {
                if (this._ConstructUnit != value)
                {
                    this.SendPropertyChanging();
                    this._ConstructUnit = value;
                    this.SendPropertyChanged("ConstructUnit");
                }
            }
        }

        [Column(Storage="_ConstructUnitRepre", DbType="VarChar(50)")]
        public string ConstructUnitRepre
        {
            get
            {
                return this._ConstructUnitRepre;
            }
            set
            {
                if (this._ConstructUnitRepre != value)
                {
                    this.SendPropertyChanging();
                    this._ConstructUnitRepre = value;
                    this.SendPropertyChanged("ConstructUnitRepre");
                }
            }
        }

        [Column(Storage="_DesignUnit", DbType="VarChar(100)")]
        public string DesignUnit
        {
            get
            {
                return this._DesignUnit;
            }
            set
            {
                if (this._DesignUnit != value)
                {
                    this.SendPropertyChanging();
                    this._DesignUnit = value;
                    this.SendPropertyChanged("DesignUnit");
                }
            }
        }

        [Column(Storage="_DesignUnitRepre", DbType="VarChar(50)")]
        public string DesignUnitRepre
        {
            get
            {
                return this._DesignUnitRepre;
            }
            set
            {
                if (this._DesignUnitRepre != value)
                {
                    this.SendPropertyChanging();
                    this._DesignUnitRepre = value;
                    this.SendPropertyChanged("DesignUnitRepre");
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
                    if (this._Project.HasLoadedOrAssignedValue)
                    {
                        throw new ForeignKeyReferenceAlreadyHasValueException();
                    }
                    this.SendPropertyChanging();
                    this._Id = value;
                    this.SendPropertyChanged("Id");
                }
            }
        }

        [Column(Storage="_OperateUnit", DbType="VarChar(100)")]
        public string OperateUnit
        {
            get
            {
                return this._OperateUnit;
            }
            set
            {
                if (this._OperateUnit != value)
                {
                    this.SendPropertyChanging();
                    this._OperateUnit = value;
                    this.SendPropertyChanged("OperateUnit");
                }
            }
        }

        [Column(Storage="_OperateUnitRepre", DbType="VarChar(50)")]
        public string OperateUnitRepre
        {
            get
            {
                return this._OperateUnitRepre;
            }
            set
            {
                if (this._OperateUnitRepre != value)
                {
                    this.SendPropertyChanging();
                    this._OperateUnitRepre = value;
                    this.SendPropertyChanged("OperateUnitRepre");
                }
            }
        }

        [Association(Name="Project_ProjectInfo", Storage="_Project", ThisKey="Id", OtherKey="Id", IsForeignKey=true)]
        public PDSVWeb.Models.Project Project
        {
            get
            {
                return this._Project.Entity;
            }
            set
            {
                PDSVWeb.Models.Project entity = this._Project.Entity;
                if ((entity != value) || !this._Project.HasLoadedOrAssignedValue)
                {
                    this.SendPropertyChanging();
                    if (entity != null)
                    {
                        this._Project.Entity = null;
                        entity.ProjectInfo = null;
                    }
                    this._Project.Entity = value;
                    if (value != null)
                    {
                        value.ProjectInfo = this;
                        this._Id = value.Id;
                    }
                    else
                    {
                        this._Id = 0;
                    }
                    this.SendPropertyChanged("Project");
                }
            }
        }

        [Column(Storage="_ProjectAddress", DbType="Text", UpdateCheck=UpdateCheck.Never)]
        public string ProjectAddress
        {
            get
            {
                return this._ProjectAddress;
            }
            set
            {
                if (this._ProjectAddress != value)
                {
                    this.SendPropertyChanging();
                    this._ProjectAddress = value;
                    this.SendPropertyChanged("ProjectAddress");
                }
            }
        }

        [Column(Storage="_ProjectDate", DbType="VarChar(100)")]
        public string ProjectDate
        {
            get
            {
                return this._ProjectDate;
            }
            set
            {
                if (this._ProjectDate != value)
                {
                    this.SendPropertyChanging();
                    this._ProjectDate = value;
                    this.SendPropertyChanged("ProjectDate");
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

        [Column(Storage="_SupervisorUnit", DbType="VarChar(100)")]
        public string SupervisorUnit
        {
            get
            {
                return this._SupervisorUnit;
            }
            set
            {
                if (this._SupervisorUnit != value)
                {
                    this.SendPropertyChanging();
                    this._SupervisorUnit = value;
                    this.SendPropertyChanged("SupervisorUnit");
                }
            }
        }

        [Column(Storage="_SupervisorUnitRepre", DbType="VarChar(50)")]
        public string SupervisorUnitRepre
        {
            get
            {
                return this._SupervisorUnitRepre;
            }
            set
            {
                if (this._SupervisorUnitRepre != value)
                {
                    this.SendPropertyChanging();
                    this._SupervisorUnitRepre = value;
                    this.SendPropertyChanged("SupervisorUnitRepre");
                }
            }
        }
    }
}

