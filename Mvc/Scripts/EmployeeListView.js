var pageUrl;

$(document).ready(function () {
    pageUrl = window.location.href.split('#')[0].split('?')[0];
    FilterEmployeeType();
});

function FilterEmployeeType() {
    var _empTypes = [];
    if ($('#EmployeeTypes').data("kendoMultiSelect") != null)
        _empTypes = $('#EmployeeTypes').data("kendoMultiSelect").value();

    $.ajax({
        type: "POST",
        url: pageUrl + "/api/GetMultiList",
        data: JSON.stringify({
            selectedEmployeeTypeIds: _empTypes
        }),
        contentType: "application/json;charset=utf-8",
        async: true,
        beforeSend: function () {
            $("#EmployeeListDiv").hide();
        },
        success: function (result) {
            $("#EmployeeListDiv").html(result);
        },
        error: function (error) {
            alert("Something went wrong. Please contact your system administrator.");
        },
        complete: function () {
            $("#EmployeeListDiv").show();
        }
    });
}