namespace PDSVWeb.Models
{
    using System;
    using System.ComponentModel;
    using System.Data.Linq;
    using System.Data.Linq.Mapping;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;

    [Table(Name="dbo.ProjectTechInfo")]
    public class ProjectTechInfo : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private double? _BuildHigh;
        private double? _CountArea;
        private int? _CountFloors;
        private double? _GroundArea;
        private int? _GroundFloors;
        private int _Id;
        private EntityRef<PDSVWeb.Models.Project> _Project = new EntityRef<PDSVWeb.Models.Project>();
        private double? _UnderGroundArea;
        private int? _UnderGroundFloors;
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

        [Column(Storage="_BuildHigh", DbType="Float")]
        public double? BuildHigh
        {
            get
            {
                return this._BuildHigh;
            }
            set
            {
                double? nullable = this._BuildHigh;
                double? nullable2 = value;
                if ((nullable.GetValueOrDefault() == nullable2.GetValueOrDefault()) ? (nullable.HasValue != nullable2.HasValue) : true)
                {
                    this.SendPropertyChanging();
                    this._BuildHigh = value;
                    this.SendPropertyChanged("BuildHigh");
                }
            }
        }

        [Column(Storage="_CountArea", DbType="Float")]
        public double? CountArea
        {
            get
            {
                return this._CountArea;
            }
            set
            {
                double? nullable = this._CountArea;
                double? nullable2 = value;
                if ((nullable.GetValueOrDefault() == nullable2.GetValueOrDefault()) ? (nullable.HasValue != nullable2.HasValue) : true)
                {
                    this.SendPropertyChanging();
                    this._CountArea = value;
                    this.SendPropertyChanged("CountArea");
                }
            }
        }

        [Column(Storage="_CountFloors", DbType="Int")]
        public int? CountFloors
        {
            get
            {
                return this._CountFloors;
            }
            set
            {
                int? nullable = this._CountFloors;
                int? nullable2 = value;
                if ((nullable.GetValueOrDefault() == nullable2.GetValueOrDefault()) ? (nullable.HasValue != nullable2.HasValue) : true)
                {
                    this.SendPropertyChanging();
                    this._CountFloors = value;
                    this.SendPropertyChanged("CountFloors");
                }
            }
        }

        [Column(Storage="_GroundArea", DbType="Float")]
        public double? GroundArea
        {
            get
            {
                return this._GroundArea;
            }
            set
            {
                double? nullable = this._GroundArea;
                double? nullable2 = value;
                if ((nullable.GetValueOrDefault() == nullable2.GetValueOrDefault()) ? (nullable.HasValue != nullable2.HasValue) : true)
                {
                    this.SendPropertyChanging();
                    this._GroundArea = value;
                    this.SendPropertyChanged("GroundArea");
                }
            }
        }

        [Column(Storage="_GroundFloors", DbType="Int")]
        public int? GroundFloors
        {
            get
            {
                return this._GroundFloors;
            }
            set
            {
                int? nullable = this._GroundFloors;
                int? nullable2 = value;
                if ((nullable.GetValueOrDefault() == nullable2.GetValueOrDefault()) ? (nullable.HasValue != nullable2.HasValue) : true)
                {
                    this.SendPropertyChanging();
                    this._GroundFloors = value;
                    this.SendPropertyChanged("GroundFloors");
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

        [Association(Name="Project_ProjectTechInfo", Storage="_Project", ThisKey="Id", OtherKey="Id", IsForeignKey=true)]
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
                        entity.ProjectTechInfo = null;
                    }
                    this._Project.Entity = value;
                    if (value != null)
                    {
                        value.ProjectTechInfo = this;
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

        [Column(Storage="_UnderGroundArea", DbType="Float")]
        public double? UnderGroundArea
        {
            get
            {
                return this._UnderGroundArea;
            }
            set
            {
                double? nullable = this._UnderGroundArea;
                double? nullable2 = value;
                if ((nullable.GetValueOrDefault() == nullable2.GetValueOrDefault()) ? (nullable.HasValue != nullable2.HasValue) : true)
                {
                    this.SendPropertyChanging();
                    this._UnderGroundArea = value;
                    this.SendPropertyChanged("UnderGroundArea");
                }
            }
        }

        [Column(Storage="_UnderGroundFloors", DbType="Int")]
        public int? UnderGroundFloors
        {
            get
            {
                return this._UnderGroundFloors;
            }
            set
            {
                int? nullable = this._UnderGroundFloors;
                int? nullable2 = value;
                if ((nullable.GetValueOrDefault() == nullable2.GetValueOrDefault()) ? (nullable.HasValue != nullable2.HasValue) : true)
                {
                    this.SendPropertyChanging();
                    this._UnderGroundFloors = value;
                    this.SendPropertyChanged("UnderGroundFloors");
                }
            }
        }
    }
}

