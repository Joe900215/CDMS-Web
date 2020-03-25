namespace PDSVWeb.Models
{
    using System;
    using System.Data;
    using System.Data.Linq;
    using System.Data.Linq.Mapping;

    [Database(Name="PDSV")]
    public class PDSVDBDataContext : DataContext
    {
        private static MappingSource mappingSource = new AttributeMappingSource();


        public PDSVDBDataContext() : base(System.Configuration.ConfigurationManager.ConnectionStrings["PDSVConnectionString"].ConnectionString, mappingSource)
        {
        }

        public PDSVDBDataContext(IDbConnection connection) : base(connection, mappingSource)
        {
        }

        public PDSVDBDataContext(string connection) : base(connection, mappingSource)
        {
        }

        public PDSVDBDataContext(IDbConnection connection, MappingSource mappingSource) : base(connection, mappingSource)
        {
        }

        public PDSVDBDataContext(string connection, MappingSource mappingSource) : base(connection, mappingSource)
        {
        }

        public Table<PDSVWeb.Models.Area> Area
        {
            get
            {
                return base.GetTable<PDSVWeb.Models.Area>();
            }
        }

        public Table<PDSVWeb.Models.PLink> PLink
        {
            get
            {
                return base.GetTable<PDSVWeb.Models.PLink>();
            }
        }

        public Table<PDSVWeb.Models.Project> Project
        {
            get
            {
                return base.GetTable<PDSVWeb.Models.Project>();
            }
        }

        public Table<PDSVWeb.Models.ProjectInfo> ProjectInfo
        {
            get
            {
                return base.GetTable<PDSVWeb.Models.ProjectInfo>();
            }
        }

        public Table<PDSVWeb.Models.ProjectTechInfo> ProjectTechInfo
        {
            get
            {
                return base.GetTable<PDSVWeb.Models.ProjectTechInfo>();
            }
        }

        public Table<PDSVWeb.Models.PRole> PRole
        {
            get
            {
                return base.GetTable<PDSVWeb.Models.PRole>();
            }
        }

        public Table<PDSVWeb.Models.PUser> PUser
        {
            get
            {
                return base.GetTable<PDSVWeb.Models.PUser>();
            }
        }

        public Table<PDSVWeb.Models.UserLinkInfo> UserLinkInfo
        {
            get
            {
                return base.GetTable<PDSVWeb.Models.UserLinkInfo>();
            }
        }
    }
}

