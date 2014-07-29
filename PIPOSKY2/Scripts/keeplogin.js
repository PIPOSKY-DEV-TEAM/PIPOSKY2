/**
 * 
 * @authors Wang Anqi (waqthss2012@163.com)
 * @date    2014-07-28 21:49:29
 * @version $1.0$
 */
$(function () {
    //判断cookies是否存在
    if (getCookie("cookie_userid") != "" && getCookie("password") != "") {
        alert("login success:\nid=" + getCookie("cookie_userid") + "   password=" + getCookie("password"));
        //转移到登陆成功页面
        location.href = "test.html";
    }
 
    //login按钮的点击处理
    $("#login").click(function () {
        //获取文本框的userid和password
        var userName = $("#UserName").val();
        var password = $("#UserPwd").val();
        //设置cookies
        setCookie("cookie_userName", userName, 7);
        setCookie("cookie_password", password, 7);
    });
});
 
//设置cookies的方法(w3cschool上的简单设置cookie代码，自己可以去搜索下)
//c_name:cookie名称
//value:cookie的值
//expiredays:有效天数
function setCookie(c_name, value, expiredays) {
    var exdate = new Date()
    exdate.setDate(exdate.getDate() + expiredays)
    document.cookie = c_name + "=" + escape(value) +
    ((expiredays == null) ? "" : ";expires=" + exdate.toGMTString())
}
//获取cookies的方法
function getCookie(c_name) {
    if (document.cookie.length > 0) {
        c_start = document.cookie.indexOf(c_name + "=")
        if (c_start != -1) {
            c_start = c_start + c_name.length + 1
            c_end = document.cookie.indexOf(";", c_start)
            if (c_end == -1) c_end = document.cookie.length
            return unescape(document.cookie.substring(c_start, c_end))
        }
    }
    return ""
}
