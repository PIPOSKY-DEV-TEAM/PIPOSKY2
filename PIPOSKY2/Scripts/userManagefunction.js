/**
 * 
 * @authors Wang Anqi (waqthss2012@163.com)
 * @date    2014-07-28 12:26:29
 * @version $1.0$
 */
$("td.deleteTd a").click(function deleteEnsure() {
    if (window.confirm('你确定要删除该用户吗？')) {
        //alert("确定");
        return true;
    } else {
        //alert("取消");
        return false;
    }
})
