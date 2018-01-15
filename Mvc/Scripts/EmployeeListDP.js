var pageUrl;

$(document).ready(function () {
    pageUrl = window.location.href.split('#')[0].split('?')[0];
    FilterEmployeeType();
});

function startChange() {
    var endPicker = $("#EndDate").data("kendoDateTimePicker"),
        startDate = this.value();

    if (startDate) {
        startDate = new Date(startDate);
        startDate.setDate(startDate.getDate() + 1);
        endPicker.min(startDate);
    }

    CheckSubmission();
}

function endChange() {
    var startPicker = $("#StartDate").data("kendoDateTimePicker"),
        endDate = this.value();

    if (endDate) {
        endDate = new Date(endDate);
        endDate.setDate(endDate.getDate() - 1);
        startPicker.max(endDate);
    }

    CheckSubmission();
}

function CheckSubmission() {
    var startDate = $("#StartDate").data("kendoDateTimePicker").value(),
        endDate = $("#EndDate").data("kendoDateTimePicker").value();

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
    var _empTypes = [], _deptTypes = [], _empStartDate = "1/1/1900", _empEndDate = new Date();
    if ($('#EmployeeTypes').data("kendoMultiSelect") != null)
        _empTypes = $('#EmployeeTypes').data("kendoMultiSelect").value();

    if ($('#EmployeeDepartments').data("kendoMultiSelect") != null)
        _deptTypes = $('#EmployeeDepartments').data("kendoMultiSelect").value();

    if ($('#StartDate').data("kendoDateTimePicker").value() != null)
        _empStartDate = $('#StartDate').data("kendoDateTimePicker").value();

    if ($('#EndDate').data("kendoDateTimePicker").value() != null)
        _empEndDate = $('#EndDate').data("kendoDateTimePicker").value();

    $.ajax({
        type: "POST",
        url: pageUrl + "/api/GetMultiList",
        data: JSON.stringify({
            selectedEmployeeTypeIds: _empTypes,
            selectedDepartmentIds: _deptTypes,
            selectedStartDate: _empStartDate,
            selectedEndDate: _empEndDate
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