var pageUrl, domain;

$(document).ready(function () {
    pageUrl = window.location.href.split('#')[0].split('?')[0];
    domain = "http://localhost:2000/";
    FilterEmployeeType();
});

function startChange() {
    var endPicker = $("#EndDate").data("kendoDatePicker"),
        startDate = this.value();

    if (startDate) {
        startDate = new Date(startDate);
        startDate.setDate(startDate.getDate() + 1);
        endPicker.min(startDate);
    }

    CheckSubmission();
}

function endChange() {
    var startPicker = $("#StartDate").data("kendoDatePicker"),
        endDate = this.value();

    if (endDate) {
        endDate = new Date(endDate);
        endDate.setDate(endDate.getDate() - 1);
        startPicker.max(endDate);
    }

    CheckSubmission();
}

function GetEmployeeImages(object) {
    var html = "";
    if (object !== null && object.length > 0) {
        for (var x = 0; x < object.length; x++) {
            html += "<img src=\"" + object[x] + "\" />";
        }
    }
    return html;
}

function GetEmployeeDepartments(object) {
    var html = "";
    if (object !== null && object.length > 0) {
        html = "<p><b>Department/s:</b>";
        for (var x = 0; x < object.length; x++) {
            html += "<div class=\"department\">";
            html += "<p><b>Department Name:</b>&nbsp;" + object[x].DepartmentName + "</p>";
            html += "<p><b>Department Head:</b>&nbsp;" + object[x].DepartmentHead + "</p>";
            html += "</div>";
        }
    }
    else {
        html = "<p><b>Department/s:</b> N/A</p>";
    }
    return html;
}

function CheckSubmission() {
    var startDate = $("#StartDate").data("kendoDatePicker").value(),
        endDate = $("#EndDate").data("kendoDatePicker").value();

    if (startDate != null && endDate != null) {
        if (startDate > endDate) {
            alert("Start date higher than end date.");
        }
        else {
            FilterEmployeeType();
        }
    }
    else {
        FilterEmployeeType();
    }
}

function FilterEmployeeType() {
    var _empTypes = [], _empStartDate = "1/1/1900", _empEndDate = new Date();
    if ($('#EmployeeTypes').data("kendoMultiSelect") != null)
        _empTypes = $('#EmployeeTypes').data("kendoMultiSelect").value();

    if ($('#StartDate').data("kendoDatePicker").value() != null && $('#StartDate').data("kendoDatePicker").value() != "")
        _empStartDate = $('#StartDate').data("kendoDatePicker").value();

    if ($('#EndDate').data("kendoDatePicker").value() != null && $('#EndDate').data("kendoDatePicker").value() != "")
        _empEndDate = $('#EndDate').data("kendoDatePicker").value();

    $.ajax({
        type: "POST",
        url: domain + "Custom/Services/EmployeesList.svc/GetEmployees",
        data: JSON.stringify({
            selectedEmployeeTypeIds: _empTypes,
            selectedStartDate: _empStartDate,
            selectedEndDate: _empEndDate
        }),
        contentType: "application/json;charset=utf-8",
        async: true,
        success: function (result) {
            $("#EmployeeList").data("kendoListView").dataSource.data(jQuery.parseJSON(result.d));
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert("Something went wrong. Please contact your system administrator.");
        }
    });
}