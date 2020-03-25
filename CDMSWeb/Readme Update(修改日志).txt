2017.4.2 

1. 登录函数需要传输登录机器名称
    /// <summary>
        /// 登录数据源
        /// </summary>
        /// <param name="UserName">用户名</param>
        /// <param name="Password">用户密码</param>
        /// <param name="hostname">登录主机名</param>
        /// <returns>返回一个JSON对象</returns>
        [HttpPost]
        public static JObject Login(string UserName,string Password, string hostname)
        {






2017.3.5 

1. 所有DBSOURCE 都获取当前 DBSOURCE， 包括创建， 放弃了创建对象采用 newDBSource 的方案。
2. 登录用户的User, 超过24小时自动删除。
3. 登录用户10分钟后，自动更新DBsource对象。
4. 修改了流程中的错误。
5. 所有内部使用的函数全部改为 inernet（内部函数）。