//namespace PDSVWeb.DataModels
//{
//    using LinqToDB;
//    using LinqToDB.Data;
//    using System;

//    public class PDSVModelDB : DataConnection
//    {
//        public PDSVModelDB()
//        {
//        }

//        public PDSVModelDB(string providerName, string configuration) : base(providerName, configuration)
//        {
//        }

//        public ITable<Node> Nodes
//        {
//            get
//            {
//                return base.GetTable<Node>();
//            }
//        }

//        public ITable<NodeTree> NodeTrees
//        {
//            get
//            {
//                return base.GetTable<NodeTree>();
//            }
//        }

//        public ITable<Paramete> Parametes
//        {
//            get
//            {
//                return base.GetTable<Paramete>();
//            }
//        }
//    }
//}

