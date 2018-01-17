var pageUrl, domain;

$(document).ready(function () {
    pageUrl = window.location.href.split('#')[0].split('?')[0];
    domain = "http://localhost:2000/";
});

function CheckNewSubmission() {
    SubmitNewForm();
}

function SubmitNewForm() {
    var _empTypes = [], _empDepts = [], _empBirthDate = "1/1/1900";
    var _employeeID = $('#txt_EmployeeID').val();
    var _employeeFName = $('#txt_FirstName').val();
    var _employeeLName = $('#txt_LastName').val();
    var _employeeJobDesc = $('#txt_JobDescription').val();
    var _employeeSalary = $('#txt_Salary').val();

    if ($('#txt_BirthDate').data("kendoDatePicker").value() != null && $('#txt_BirthDate').data("kendoDatePicker").value() != "")
        _empBirthDate = $('#txt_BirthDate').data("kendoDatePicker").value();

    if ($('#mul_EmployeeTypes').data("kendoMultiSelect") != null)
        _empTypes = $('#mul_EmployeeTypes').data("kendoMultiSelect").value();

    if ($('#mul_EmployeeDepartment').data("kendoMultiSelect") != null)
        _empDepts = $('#mul_EmployeeDepartment').data("kendoMultiSelect").value();

    if (_employeeID == "") {
        alert("Requires Employee ID.");
    }
    else {
        $.ajax({
            type: "POST",
            url: domain + "Custom/Services/EmployeesList.svc/AddEmployee",
            data: JSON.stringify({
                EmployeeID: _employeeID,
                FirstName: _employeeFName,
                LastName: _employeeLName,
                JobDescription: _employeeJobDesc,
                Salary: _employeeSalary,
                BirthDate: _empBirthDate,
                EmployeeTypeIds: _empTypes,
                DepartmentIds: _empDepts
            }),
            contentType: "application/json;charset=utf-8",
            async: true,
            success: function (response) {
                if (response.d.IsSuccessful == true) {
                    window.location.href = domain + "employees-api";
                }
                else {
                    alert(response.d.ResponseMessage);
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert("Something went wrong. Please contact your system administrator.");
            }
        });
    }
}

function CheckEditSubmission() {
    SubmitEditForm();
}

function SubmitEditForm() {
    var _empTypes = [], _empBirthDate = "1/1/1900";
    var _employeeGUID = $('#hidGUID').val();
    var _employeeID = $('#txt_EmployeeID').val();
    var _employeeFName = $('#txt_FirstName').val();
    var _employeeLName = $('#txt_LastName').val();
    var _employeeJobDesc = $('#txt_JobDescription').val();
    var _employeeSalary = $('#txt_Salary').val();

    if ($('#txt_BirthDate').data("kendoDatePicker").value() != null && $('#txt_BirthDate').data("kendoDatePicker").value() != "")
        _empBirthDate = $('#txt_BirthDate').data("kendoDatePicker").value();

    if ($('#mul_EmployeeTypes').data("kendoMultiSelect") != null)
        _empTypes = $('#mul_EmployeeTypes').data("kendoMultiSelect").value();

    if (_employeeID == "") {
        alert("Requires Employee ID.");
    }
    else {
        $.ajax({
            type: "POST",
            url: domain + "Custom/Services/EmployeesList.svc/EditEmployee",
            data: JSON.stringify({
                EmployeeGUID: _employeeGUID,
                EmployeeID: _employeeID,
                FirstName: _employeeFName,
                LastName: _employeeLName,
                JobDescription: _employeeJobDesc,
                Salary: _employeeSalary,
                BirthDate: _empBirthDate,
                EmployeeTypeIds: _empTypes
            }),
            contentType: "application/json;charset=utf-8",
            async: true,
            success: function (response) {
                if (response.d.IsSuccessful == true) {
                    window.location.href = domain + "employees-api";
                }
                else {
                    alert(response.d.ResponseMessage);
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert("Something went wrong. Please contact your system administrator.");
            }
        });
    }
}