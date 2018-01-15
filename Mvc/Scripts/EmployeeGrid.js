var pageUrl;

$(document).ready(function () {
    pageUrl = window.location.href.split('#')[0].split('?')[0];
});

function FilterEmployeeType() {
    $.ajax({
        type: "POST",
        url: pageUrl + "/api/GetList",
        data: JSON.stringify({
            selectedEmployeeTypeId: $('#EmpClassification').data("kendoDropDownList").value()
        }),
        contentType: "application/json;charset=utf-8",
        async: true,
        beforeSend: function () {
            $("#EmpListContainer").hide();
        },
        success: function (result) {
            $("#EmpListContainer").html(result);
        },
        error: function (error) {
            alert("Something went wrong. Please contact your system administrator.");
        },
        complete: function () {
            $("#EmpListContainer").show();
        }
    });
}