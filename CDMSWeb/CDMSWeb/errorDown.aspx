<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <meta name="viewport" content="width=device-width" />
    <title>下载文件出错</title>
</head>
<body>
    <div>
        <% Response.Write("下载文件出错，你没有登录或没有文件访问权限！");%> 
    </div>
</body>
</html>
