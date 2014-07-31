/**
 * 
 * @authors Wang Anqi (waqthss2012@163.com)
 * @date    2014-07-28 12:26:29
 * @version $1.0$
 */
$("a.deleteTd").click(function deleteEnsure() {
    if (window.confirm('你确定要删除该用户吗？')) {
        //alert("确定");
        return true;
    } else {
        //alert("取消");
        return false;
    }
})
$("#SelectAll").click(function selectall() {
    var numOfCheckBox = $(".selected").size();
    var IsSelected;
    IsSelected = document.getElementById("SelectAll").checked;
    for (var i = 0; i < numOfCheckBox; i++) {
        ($(".selected").get(i)).checked = IsSelected;
    }
})
$("#ensureBatchRemove").click(function deleteEnsure() {
    if (window.confirm('你确定要删除用户吗？')) {
        //alert("确定");
        return true;
    } else {
        //alert("取消");
        return false;
    }
})