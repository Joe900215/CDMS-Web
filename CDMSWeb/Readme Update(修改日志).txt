2017.4.2 

1. ��¼������Ҫ�����¼��������
    /// <summary>
        /// ��¼����Դ
        /// </summary>
        /// <param name="UserName">�û���</param>
        /// <param name="Password">�û�����</param>
        /// <param name="hostname">��¼������</param>
        /// <returns>����һ��JSON����</returns>
        [HttpPost]
        public static JObject Login(string UserName,string Password, string hostname)
        {






2017.3.5 

1. ����DBSOURCE ����ȡ��ǰ DBSOURCE�� ���������� �����˴���������� newDBSource �ķ�����
2. ��¼�û���User, ����24Сʱ�Զ�ɾ����
3. ��¼�û�10���Ӻ��Զ�����DBsource����
4. �޸��������еĴ���
5. �����ڲ�ʹ�õĺ���ȫ����Ϊ inernet���ڲ���������