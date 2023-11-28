var isDesignerSubmit = true;
var isViewerSubmit = true;
var draftAlertMessage = "This Report is not saved at least once. Please edit and save the report to access the schedule.";
var defaultCategoryName = "reports";
var resizeTimeOut = null;
var countsnap;
var isSaveAs = false;
var saveAsName = "";
var avoidSpecialCharactersAlertMessage = 'Please avoid below special characters in item name <br><br> [*\\\]|:<>%+\'\#?\"\;\,/ ';
var reportNameAlertMessage = 'Please provide the report name.'
//var allPermission = ej.ReportDesigner.Permission.All;

$("#main").find(".openbtn").bind("click", designerResize);

$("#save-report-item").on("click", function () {
    var designer = $('#' + controlId).data('boldReportDesigner');
    loadingIconShow("none", "block");
    saveReport();
});

function SaveNewReport() {
    var designer = $('#' + controlId).data('boldReportDesigner');
    loadingIconShow("none", "block");
    saveReport();
}

function SaveAsNewReport(isSaveAsData, saveAsReportName) {
    var designer = $('#' + controlId).data('boldReportDesigner');
    loadingIconShow("none", "block");
    isSaveAs = isSaveAsData;
    saveAsName = saveAsReportName;
    saveReport();
}

function SaveAsNewReportDialog() {
    var dlgContent = "<div><label id = 'name-label'>Name</label><input type='text' id='dialog-report-name' name='dname' placeholder='Enter the report name' style='margin-left: 20px;width: 200px;'><br><br></div>";
    document.body.style.pointerEvents = "none";
    document.getElementById("sub-dialog").style.pointerEvents = "auto";
    var dialogObj1 = new ejs.popups.Dialog({
        header: "Save As Report",
        content: dlgContent,
        width: '420px',
        showCloseIcon: true,
        closeOnEscape: true,

        buttons: [
            {
                'click': () => {
                    var saveAsreportName = $("#dialog-report-name").val();
                    document.body.style.pointerEvents = "auto";
                    dialogObj1.hide();
                    $("#loading_icon").removeClass("hide");
                    $("#loading_icon").addClass("show-flex");
                    var regex = new RegExp(/[*\[\\\]\|\/\:\<\>\%\+\#\?\'\"\;\,]/);
                    if (regex.test(saveAsreportName)) {
                        InvalidReportNameAlert("New Report");
                        $("#loading_icon").addClass("hide");
                        $("#loading_icon").removeClass("show-flex");
                    }
                    else {
                        IsReportExist(saveAsreportName, "New Report", function () {
                            SaveAsNewReport(true, saveAsreportName);
                        });   
                    }
                },
                buttonModel: { content: 'Create', isPrimary: true }
            },
            {
                'click': () => {
                    dialogObj1.hide();
                    document.body.style.pointerEvents = "auto";
                },
                buttonModel: { content: 'Cancel' }
            }
        ],
        close: function () {
            dialogObj1.destroy();
            document.body.style.pointerEvents = "auto";
        }
    });
    dialogObj1.appendTo('#sub-dialog');
}

String.prototype.replaceAll = function (searchStr, replaceStr) {
    var str = this;

    // no match exists in string?
    if (str.indexOf(searchStr) === -1) {
        // return string
        return str;
    }

    // replace and remove first match, and do another recursirve search/replace
    return (str.replace(searchStr, replaceStr)).replaceAll(searchStr, replaceStr);
}

function saveReport() {
    var designer = $('#' + controlId).data('boldReportDesigner');
    var userEmail = getParams(document.location.href, "email");
    var updatedReportName = $("#report-item").val();
    if (designer.isNewReport()) {
        if (reportName === updatedReportName) {
            designer.saveReport(updatedReportName);

        }
        else {
            saveReportName("NewReport");
        }
    }
    else {

        saveReportName("EditReport");
    }
}

function loadingIconShow(block, none) {
    $("#save-report-item").css("display", block);
    $("#close-report-item").css("display", block);
    $("#create-report").css("display", block);
    $("#close-report").css("display", block);
    $("#delete-item").css("display", block);
    $("#report-loading-icon").css("display", none);
    $("#item-loading-icon").css("display", none);
}

function saveReportName(type) {
    var designer = $('#' + controlId).data('boldReportDesigner');
    var updatedReportName = $("#report-item").val();
    var regex = new RegExp(/[*\[\\\]\|\/\:\<\>\%\+\#\?\'\"\;\,]/);
    if (regex.test(updatedReportName)) {
        InvalidNameAlert("Update Report");
    }
    else {
        if (type === "NewReport") {
            IsReportExist(reportName, "Update Report", function () {
                updateReportName();
                designer.saveReport(reportName);
            });
        }
        else {
            if (type === "EditReport") {
                designer.saveReport(reportName);
            }
            else {
                IsReportExist(reportName, "Update Report", function () {
                    if (isDraft) {
                        updateReportName();
                        designer.saveReport(reportName);
                    }
                    else {
                        $.ajax({
                            type: "POST",
                            url: renameReportUrl,
                            data: { itemName: reportName, categoryName: 'reports', updatedReportName: updatedReportName, userEmail: currentReportUserEmail },
                            success: function (response) {
                                if (response === 'true') {
                                    updateReportName();
                                    designer.saveReport(reportName);
                                }
                            }
                        });
                    }
                });
            }
        }
    }

}

function updateReportName() {
    var rprtName = (isSaveAs && saveAsName !== null && saveAsName !== "" && saveAsName.length > 0) ? saveAsName : $("#report-item").val();
    var designer = $('#' + controlId).data('boldReportDesigner');
    designer.reportFileName = (rprtName && rprtName.length > 0) ? rprtName : reportName;
    reportName = designer.reportFileName;
}

function InvalidReportNameAlert(dlgHeader, errorContent) {
    $("#loading-item").removeClass("show-flex");
    $("#loading-item").addClass("hide");
    //$("#save-report-item").css("display", "none");
    $("#close-report-item").css("display", "none");
    $("#create-report").css("display", "none");
    $("#create-new-report").css("display", "none");
    $("#close-report").css("display", "none");
    $("#delete-item").css("display", "none");
    $("#report-loading-icon").css("display", "block");
    $("#item-loading-icon").css("display", "block");
    $(".dropdown-menu #create-new-report").css("display", "block");
    var dlgContent = errorContent;
    document.body.style.pointerEvents = "none";
    document.getElementById("dialog").style.pointerEvents = "auto";
    var dialogObj = new ejs.popups.Dialog({
        header: dlgHeader,
        content: dlgContent,
        width: '420px',
        showCloseIcon: true,
        closeOnEscape: true,
        buttons: [
            {
                'click': () => {
                    dialogObj.hide();
                    document.body.style.pointerEvents = "auto";
                    //$("#save-report-item").css("display", "block");
                    $("#close-report-item").css("display", "block");
                    $("#create-report").css("display", "block");
                    $(".dropdown-menu #create-new-report").css("display", "block");
                    $("#close-report").css("display", "block");
                    $("#report-loading-icon").css("display", "none");
                    $("#item-loading-icon").css("display", "none");
                    $("#report-name").val("");
                },
                buttonModel: { content: 'OK', isPrimary: true }
            }
        ],
        close: function () {
            document.body.style.pointerEvents = "auto";
            //$("#save-report-item").css("display", "block");
            $("#close-report-item").css("display", "block");
            $("#create-report").css("display", "block");
            $("#close-report").css("display", "block");
            $("#report-loading-icon").css("display", "none");
            $("#item-loading-icon").css("display", "none");
            $("#report-name").val("");
            dialogObj.destroy();
        }
    });
    dialogObj.appendTo('#dialog');
}


function CreateNewReport() {
    $(".nav-item-heading #create-new-report").css("display", "none");
    $("#create-report-name").css("display", "flex");
}

function CloseItemNewReport() {
    $("#report-name").val("");
    $(".nav-item #create-new-report").css("display", "block");
    $("#create-report-name").css("display", "none");
}

function CreateReport() {
    var dlgContent = "<div><label id = 'name-label' style = 'font-size:medium'>Name</label><input type='text' id='dialog-report-name' name=reportname' placeholder='Enter the report name' style='margin-left: 50px;width: 225px; font-size:medium '><span id='errornull' style='display:none;color:red;margin-left:95px;/*margin-top:5px*/'>Please enter report name</span><br><br></div>";
    document.body.style.pointerEvents = "none";
    document.getElementById("dialog").style.pointerEvents = "auto";
    var dialogObj2 = new ejs.popups.Dialog({
        header: "Create Report",
        content: dlgContent,
        width: '470px',
        showCloseIcon: true,
        closeOnEscape: true,

        buttons: [
            {
                'click': () => {
                    var reportName = $("#dialog-report-name").val();
                    var regex = new RegExp(/[*\[\\\]\|\/\:\<\>\%\+\#\?\'\"\;\,]/);
                    document.body.style.pointerEvents = "auto";
                    if (reportName === " " || reportName ==="") {
                        dialogObj2.show();
                        $("#errornull").css("display", "block");
                        $("#loading_icon").addClass("hide");
                        $("#loading_icon").removeClass("show-flex");
                    }
                    else {
                        dialogObj2.destroy();
                        IsReportExist(reportName, "New Report", function () {
                            IsDraftReportExist(reportName, "New Report", function () {
                                openReportDesignerForCreate(reportName);
                            })
                            
                        });
                    }
                },
                buttonModel: { content: 'Create', isPrimary: true }
            },
            {
                'click': () => {
                    dialogObj2.hide();
                    document.body.style.pointerEvents = "auto";
                },
                buttonModel: { content: 'Cancel' }
            }
        ],
        close: function () {
            dialogObj2.destroy();
            document.body.style.pointerEvents = "auto";
        }
    });
    dialogObj2.appendTo('#dialog');
}

function CreateNewReportFromDatasource(args, datasourceName) {
    var dlgContent = "<div><label id = 'name-label'>Name</label><input type='text' id='dialog-report-name' name='dname' placeholder='Enter the report name' style='margin-left: 20px;width: 200px;'><br><br></div>";
    document.body.style.pointerEvents = "none";
    document.getElementById("sub-dialog").style.pointerEvents = "auto";
    var dialogObj1 = new ejs.popups.Dialog({
        header: "Create Report with " + datasourceName,
        content: dlgContent,
        width: '420px',
        showCloseIcon: true,
        closeOnEscape: true,

        buttons: [
            {
                'click': () => {
                    var reportName = $("#dialog-report-name").val();
                    document.body.style.pointerEvents = "auto";
                    dialogObj1.hide();
                    $("#loading_icon").removeClass("hide");
                    $("#loading_icon").addClass("show-flex");
                    var regex = new RegExp(/[*\[\\\]\|\/\:\<\>\%\+\#\?\'\"\;\,]/);
                    if (reportName === "") {
                        InvalidReportNameAlert("New Report", reportNameAlertMessage);
                        $("#loading_icon").addClass("hide");
                        $("#loading_icon").removeClass("show-flex");
                    } else if (regex.test(reportName)) {
                        InvalidReportNameAlert("New Report", avoidSpecialCharactersAlertMessage);
                        $("#loading_icon").addClass("hide");
                        $("#loading_icon").removeClass("show-flex");
                    }
                    else {
                        IsReportExist(reportName, "New Report", function () {
                            IsDraftReportExist(reportName, "New Report", function () {
                                openReportDesignerForCreate(reportName);
                            })

                        });
                    }
                },
                buttonModel: { content: 'Create', isPrimary: true }
            },
            {
                'click': () => {
                    dialogObj1.hide();
                    document.body.style.pointerEvents = "auto";
                },
                buttonModel: { content: 'Cancel' }
            }
        ],
        close: function () {
            dialogObj1.destroy();
            document.body.style.pointerEvents = "auto";
        }
    });
    dialogObj1.appendTo('#sub-dialog');
}

function CloseReport() {
    var userEmail = getParams(document.location.href, "email");
    document.location.href = baseUrl + (isEmptyOrWhiteSpace(userEmail) ? "" : "?email=" + userEmail);
}

function CloseEditReport(args, reportname, reportid) {
    var userEmail = getParams(document.location.href, "email");
    document.location.href = closeEditReportUrl + "?categoryName=" + defaultCategoryName + "&sampleName=" + reportname + (isEmptyOrWhiteSpace(userEmail) ? "" : "&email=" + userEmail + "&id=" + reportid);
}

$(document).on("click", "#create-report", function () {
    $(".nav-item-heading #create-new-report").css("display", "block");
    $(".dropdown-menu #create-new-report").css("display", "block");
    $("#create-report-name").css("display", "none");
    $("#loading-item").removeClass("hide");
    $("#loading-item").addClass("show-flex");
    var reportName = $("#report-name").val();
    var regex = new RegExp(/[*\[\\\]\|\/\:\<\>\%\+\#\?\'\"\;\,]/);
    if (reportName === "" || reportName === " ") {
        InvalidReportNameAlert("New Report", reportNameAlertMessage);
        $("#loading_icon").addClass("hide");
        $("#loading_icon").removeClass("show-flex");
    }
    else {
        IsReportExist(reportName, "New Report", function () {
            IsDraftReportExist(reportName, "New Report", function () {
                openReportDesignerForCreate(reportName);
            })

        });
    }
});

function openReportDesignerForCreate(reportName) {
    var userEmail = getParams(document.location.href, "email");
    $.ajax({
        type: "POST",
        url: draftUrl,
        data: { itemName: reportName, userEmail: userEmail },
        success: function (reportData) {
            document.location.href = reportDesignerUrl + "?name=" + reportName + "&id=" + reportData.Id + (isEmptyOrWhiteSpace(userEmail) ? "" : "&email=" + userEmail) + "&edit=True" /*+ "&dslist=" + items*/;
        }
    });
}

function categoryNameFromEmail() {
    var values = currentReportUserEmail.split('@');
    var userName = values[0];
    if (values[1] != "" && values[1] != null) {
        var lastname = values[1].split('.');
        for (var i = 0; i < lastname.length; i++) {
            userName = userName + "_" + lastname[i];
        }

        var catagoryName = userName;
        return catagoryName;
    }
}

function IsReportExist(reportName, dlgHeader, callback) {
    var userEmail = getParams(document.location.href, "email");
    var cname = categoryNameFromEmail();
    $.ajax({
        type: "POST",
        url: isReportExistUrl,
        data: { itemName: reportName, categoryName: cname, userEmail: currentReportUserEmail },
        success: function (response) {
            if (response === "true") {
                $("#loading-item").removeClass("show-flex");
                $("#loading-item").addClass("hide");
                $("#close-report-item").css("display", "none");
                $("#create-report").css("display", "none");
                $("#close-report").css("display", "none");
                $("#delete-item").css("display", "none");
                $("#report-loading-icon").css("display", "block");
                $("#item-loading-icon").css("display", "block");
                var dlg = "Entered report name -\"" + reportName + "\" has been already exist";
                document.body.style.pointerEvents = "none";
                document.getElementById("dialog").style.pointerEvents = "auto";
                var dlgObj = new ejs.popups.Dialog({
                    header: dlgHeader,
                    content: dlg,
                    width: '420px',
                    showCloseIcon: true,
                    closeOnEscape: true,
                    buttons: [
                        {
                            'click': () => {
                                dlgObj.hide();
                                document.body.style.pointerEvents = "auto";
                                $("#loading-item").removeClass("show-flex");
                                $("#loading-item").addClass("hide");
                                $("#loading_icon").addClass("hide");
                                $("#loading_icon").removeClass("show-flex");
                                $("#close-report-item").css("display", "block");
                                $("#create-report").css("display", "block");
                                $("#close-report").css("display", "block");
                                $("#report-loading-icon").css("display", "none");
                                $("#item-loading-icon").css("display", "none");
                                $("#report-name").val("");
                            },
                            buttonModel: { content: 'OK', isPrimary: true }
                        }
                    ],
                    close: function () {
                        document.body.style.pointerEvents = "auto";
                        $("#loading-item").removeClass("show-flex");
                        $("#loading-item").addClass("hide");
                        $("#loading_icon").addClass("hide");
                        $("#loading_icon").removeClass("show-flex");
                        $("#close-report-item").css("display", "block");
                        $("#create-report").css("display", "block");
                        $("#close-report").css("display", "block");
                        $("#report-loading-icon").css("display", "none");
                        $("#item-loading-icon").css("display", "none");
                        $("#report-name").val("");
                        dlgObj.destroy();
                    }
                });
                dlgObj.appendTo('#dialog');
            }
            else {
                callback();
            }
        }
    });
}

function IsDraftReportExist(reportName, dlgHeader, callback) {
    var userEmail = getParams(document.location.href, "email");
    $.ajax({
        type: "POST",
        url: isDraftReportExistsUrl,
        data: { itemName: reportName, userEmail: currentReportUserEmail },
        success: function (response) {
            if (response === "true") {
                $("#loading-item").removeClass("show-flex");
                $("#loading-item").addClass("hide");
                $("#close-report-item").css("display", "none");
                $("#create-report").css("display", "none");
                $("#close-report").css("display", "none");
                $("#delete-item").css("display", "none");
                $("#report-loading-icon").css("display", "block");
                $("#item-loading-icon").css("display", "block");
                var dlg = "Entered report name -\"" + reportName + "\" has been already exist in drafts";
                document.body.style.pointerEvents = "none";
                document.getElementById("dialog").style.pointerEvents = "auto";
                var dlgObj = new ejs.popups.Dialog({
                    header: dlgHeader,
                    content: dlg,
                    width: '420px',
                    showCloseIcon: true,
                    closeOnEscape: true,
                    buttons: [
                        {
                            'click': () => {
                                dlgObj.hide();
                                document.body.style.pointerEvents = "auto";
                                $("#loading-item").removeClass("show-flex");
                                $("#loading-item").addClass("hide");
                                $("#loading_icon").addClass("hide");
                                $("#loading_icon").removeClass("show-flex");
                                $("#close-report-item").css("display", "block");
                                $("#create-report").css("display", "block");
                                $("#close-report").css("display", "block");
                                $("#report-loading-icon").css("display", "none");
                                $("#item-loading-icon").css("display", "none");
                                $("#report-name").val("");
                            },
                            buttonModel: { content: 'OK', isPrimary: true }
                        }
                    ],
                    close: function () {
                        document.body.style.pointerEvents = "auto";
                        $("#loading-item").removeClass("show-flex");
                        $("#loading-item").addClass("hide");
                        $("#loading_icon").addClass("hide");
                        $("#loading_icon").removeClass("show-flex");
                        $("#close-report-item").css("display", "block");
                        $("#create-report").css("display", "block");
                        $("#close-report").css("display", "block");
                        $("#report-loading-icon").css("display", "none");
                        $("#item-loading-icon").css("display", "none");
                        $("#report-name").val("");
                        dlgObj.destroy();
                    }
                });
                dlgObj.appendTo('#dialog');
            }
            else {
                callback();
            }
        }
    });
}

function reOpenReportDesigner(reportName, itemId, isEdit, category) {
    var selectedItems = dslistboxobj.value;
    var userEmail = getParams(document.location.href, "email");
    var items = JSON.stringify(selectedItems);
    if (isEdit === "true") {
        document.location.href = reportDesignerUrl + "?categoryName=" + category + "&sampleName=" + reportName + "&id=" + itemId + (isEmptyOrWhiteSpace(userEmail) ? "" : "&email=" + userEmail) + "&edit=True" + "&dslist=" + items;
    } else {
        document.location.href = reportDesignerUrl + "?name=" + reportName + "&id=" + itemId + (isEmptyOrWhiteSpace(userEmail) ? "" : "&email=" + userEmail) + "&edit=True" + "&dslist=" + items;
    }
}

function openReportDesignerForCreateWithDatasource(reportName, datasourceName) {
    var userEmail = getParams(document.location.href, "email");
    $.ajax({
        type: "POST",
        url: draftUrl,
        data: { itemName: reportName, userEmail: userEmail },
        success: function (reportData) {
            document.location.href = reportDesignerUrl + "?name=" + reportName + "&id=" + reportData.Id + "&dsname=" + datasourceName + (isEmptyOrWhiteSpace(userEmail) ? "" : "&email=" + userEmail) + "&edit=True";
        }
    });
}

function updateReportDesigner(designerargs) {
    var selectedItems = JSON.stringify(dslistboxobj.value);
    $.ajax({
        type: "POST",
        url: sharedDataUrl,
        data: { dataSets: selectedItems, userEmail: currentReportUserEmail },
        success: function (response) {
            if (response) {
                addSharedDatasets(JSON.parse(response));
                if (designerargs && designerargs.dataInfo) {
                    populateWidgetData(designerargs);
                } else {
                    updateWidgetData(designerargs);
                }
            }
            $("#loading_icon").removeClass("show-flex");
            $("#loading_icon").addClass("hide");
        },
        error: (msg) => {
            $("#loading_icon").removeClass("show-flex");
            $("#loading_icon").addClass("hide");
        }
    });
}

function ViewReport(args) {
    document.location.href = args.dataset.src;
}

function EditReport(args, editReportUrl, reportname, itemId, isDraft) {
    $("#loading_icon").removeClass("hide");
    $("#loading_icon").addClass("show-flex");
    if (isDraft.toLowerCase() === 'false') {
        document.location.href = editReportUrl;
    }
    else {
        document.location.href = "/Report/Designer?edit=True&id=" + itemId + "&name=" + reportname;
    }
}

function DeleteReport(args, reportDeleteUrl, reportId, reportName) {;
    var dlgContent = 'Are you sure you want to delete the Report - ' + reportName + '?';
    document.body.style.pointerEvents = "none";
    document.getElementById("dialog").style.pointerEvents = "auto";
    var dialogObj = new ejs.popups.Dialog({
        header: "Delete Report",
        content: dlgContent,
        width: '420px',
        showCloseIcon: true,
        closeOnEscape: true,

        buttons: [
            {
                'click': () => {
                    dialogObj.hide();
                    document.body.style.pointerEvents = "auto";
                    DeleteServerReport(args, reportDeleteUrl, reportId, reportName);

                },
                buttonModel: { content: 'Yes', isPrimary: true }
            },
            {
                'click': () => {
                    dialogObj.hide();
                    document.body.style.pointerEvents = "auto";
                },
                buttonModel: { content: 'No' }
            }
        ],
        close: function () {
            dialogObj.destroy();
            document.body.style.pointerEvents = "auto";
        }
    });
    dialogObj.appendTo('#dialog');
}

function DeleteServerReport(args, reportDeleteUrl, itemId, reportName) {
    var userEmail = getParams(document.location.href, "email");
    $.ajax({
        type: "POST",
        url: reportDeleteUrl,
        data: { itemId: itemId, userEmail: userEmail },
        success: function (response) {

            if (response === "OK") {
                $("#header-text").html(reportName + " deleted successfully");
                $("#card_item").css("display", "block");
                $(".toaster-block").css("display", "block");
                if (args.parentElement.classList.contains('active')) {
                    document.location.href = baseUrl;
                }
                else {
                    var parent = args.parentElement.parentElement.parentElement;
                    parent.removeChild(args.parentElement.parentElement);
                }
            }
            else {
                $("#header-text").html("Error in report delete");
                $("#card_item").removeClass("card-success");
                $("#card_item").addClass("card-danger");
                $("#card_item").css("display", "block");
                $(".toaster-block").css("display", "block");
            }
            setTimeout(function () {
                $('.toaster-block').hide();
            }, 10000);
        }
    });
}

function viewerBeforeUnload(args) {
    var viewer = $('#reportingTool').data('boldReportViewer');

    if (viewer && isViewerSubmit) {
        viewer.clearReportCache();
    }
    isViewerSubmit = true;
}

function viewerFormSubmit(args) {
    isViewerSubmit = false;
}

function addReportViewer() {
    $("#reportingTool").boldReportViewer(
        {
            reportServiceUrl: reportViewerServiceURL,
            serviceAuthorizationToken: serviceAuthorizationToken,
            reportPath: reportPath,
            reportServerUrl: reportServerURL,
        });

}

function GetSnaps(reportID) {
    $('.dropdown-menu').empty();
    $.ajax({
        type: "POST",
        url: getSnapItemUrl,
        data: { reportID: reportID },
        success: function (response) {
            $.each(response, function (key, value) {
                var nameArray = value.split('\\');
                var name = nameArray[nameArray.length - 1];
                var snapPath = reportID + "\\" + name;
                var $lisavesnapBtn = $('<button class="dropdown-item" value = ' + snapPath + ' onclick="OpenPdf(this)">' + name + '</button>');
                $('.dropdown-menu').append($lisavesnapBtn);
            });

        }
    });
}

function OpenPdf(objButton) {
    var sampleName = reportName;
    var userEmail = getParams(document.location.href, "email");
    var fileName = objButton.value;
    document.location.href = pdfViewerUrl + "?filename=" + fileName + "&email=" + userEmail + "&categoryName=" + defaultCategoryName + "&sampleName=" + sampleName;
}

function CountItems(reportID) {
    $.ajax({
        type: "POST",
        url: countSnapItemUrl,
        data: { reportID: reportID },
        success: function (response) {
            countsnap = response;
        }
    });
}

function designerBeforeUnload(args) {
    var designer = $('#' + controlId).data('boldReportDesigner');
    var viewerVisible = $('#' + controlId).find('.e-reportviewer.e-js').is(':visible');

    if (designer) {
        if (isDesignerSubmit && !viewerVisible) {
            designer.disposeViewerCache();
        }
        if (designer.hasReportChanges() && isDesignerSubmit) {
            return 'Changes you made may not be saved';
        }
    }
    isDesignerSubmit = true;
}

function designerFormSubmit(args) {
    isDesignerSubmit = false;
}

function designerResize(args) {
    var reportdesigner = $("#reportingTool");
    if (reportdesigner) {
        var designer = reportdesigner ? reportdesigner.data('boldReportDesigner') : null;
        var designerArea = reportdesigner.find('#reportingTool_designerContainer');
        if (designer && designerArea.is(':visible')) {
            if (resizeTimeOut) {
                clearTimeout(resizeTimeOut);
            }
            resizeTimeOut = setTimeout(() => {
                designer.designerResize();
            }, 80);
        }
    }
}

function addReportDesigner() {
    $(document.body).bind('submit', designerFormSubmit);
    $(window).bind('beforeunload', designerBeforeUnload);
    $("#reportingTool").boldReportDesigner(
        {
            enableAutoDraft: true,
            serviceUrl: reportDesignerServiceURL,
            reportServerUrl: reportServerURL,
            serviceAuthorizationToken: serviceAuthorizationToken,
            reportItemExtensions: window.itemExtensions,
            reportEmbedName: 'Ninox',
            permissionSettings: {
                dataSet: ej.ReportDesigner.Permission.All,
                dataSource: ej.ReportDesigner.Permission.All
            },
            toolbarSettings: {
                items: ej.ReportDesigner.ToolbarItems.All & ~ej.ReportDesigner.ToolbarItems.Open
                    & ~ej.ReportDesigner.ToolbarItems.Save & ~ej.ReportDesigner.ToolbarItems.New
                    & ~ej.ReportDesigner.ToolbarItems.Preview & ~ej.ReportDesigner.ToolbarItems.EditDesign
            },
            configurePaneSettings: {
                items: ej.ReportDesigner.ConfigureItems.All
            },
            create: controlInitialized,
            reportModified: reportModified,
            reportOpened: reportOpened,
            ajaxBeforeLoad: designerAjaxBeforeSend,
            customizeWidgets: updateWidgetData,
            customizeWidgetsData: populateWidgetData,
            toolbarRendering: toolbarRendered,
            reportSaved: reportSaved,
            extensionLocaleChanged: getExtensionLocale
        });
}

$(document).on("click", "#reportitem-link-copy", function (e) {
    $("#report-item-link").select();
    document.execCommand('copy');
});

function ReportDraftAlert(content) {
    document.body.style.pointerEvents = "none";
    document.getElementById("dialog").style.pointerEvents = "auto";
    var dialogObj = new ejs.popups.Dialog({
        header: "Save Report Alert",
        content: content,
        width: '420px',
        showCloseIcon: true,
        closeOnEscape: true,
        buttons: [
            {
                'click': () => {
                    dialogObj.hide();
                    document.body.style.pointerEvents = "auto";
                },
                buttonModel: { content: 'OK', isPrimary: true }
            }
        ],
        close: function () {
            dialogObj.destroy();
            document.body.style.pointerEvents = "auto";
        }
    });
    dialogObj.appendTo('#dialog');
}

function getDataSet() {
    var datasetName = " ";
    var dataset = null;
    if (datasetName != '') {
        $.ajax({
            type: "POST",
            url: "DataSet",
            data: { "DataSetName": datasetName },
            async: false,
            success: function (data) {
                dataset = JSON.parse(data);
            }
        });
    } else {
        alert("Choose a valid dataset");
    }
    return dataset;
}

function controlInitialized(args) {
    var designer = $('#' + controlId).data('boldReportDesigner');
    if (args && isDraft == "false") {
        designer.openReport(categoryName + '/' + reportName);
    }
    else {
        designer.newServerReport(reportName);
        var dataset = getDataSet();
        designer.addDataSet(dataset);
    }
}

function openServerReport(name, category) {
    var designer = $('#' + controlId).data('boldReportDesigner');
    categoryName = category;
    reportName = name;
    //description = null;
    //var newTitle = reportName + '- Design Report -' + pageTitle;
    var trimmedCategoryName = categoryName.split('/');
    if (trimmedCategoryName.length > 1) {
        categoryName = trimmedCategoryName[1];
    }

    designer.openReport(categoryName + '/' + reportName);
}

function reportSaved(args) {
    if (args.reportAction !== 'AutoDraft') {
        $("#header-text").html(reportName + " saved successfully");
        $("#card_item").css("display", "block");
        $(".toaster-block").css("display", "block");

        if (args.reportAction === 'Saved') {
            reportId = args.publishId ? args.publishId : reportId;
            document.location.href = reportviewerUrl + "?categoryName=" + defaultCategoryName + "&sampleName=" + reportName + "&id=" + reportId;
        }
    }
}
function reportModified(args) {
    if (args.isModified) {
    }
}
function reportOpened(args) {
    isEditReport = true;
    //var designer = $('#' + controlId).data('boldReportDesigner');
    //var dataSets = JSON.parse(sharedDatasets);
    //addSharedDatasets(dataSets);
    if (!args.isServerReport) {
        //history.pushState('', newTitle, window.location.href.split("report-designer")[0] + "report-designer");
    }
}

function designerAjaxBeforeSend(args) {
    if (args.headers) {
        args.headers.push({ Key: 'ReportId', Value: reportId });
    }

    if (isSaveAs) {
        $("#loading_icon").addClass("hide");
        $("#loading_icon").removeClass("show-flex");
    }

    if (args.actionType === 'openServerReport'
        || args.actionType === 'saveServerReport'
        || args.actionType === 'createServerReport'
        || args.actionType === 'CheckDraftReport') {
        console.log('send');
        var reportServerData = {
            'category': categoryName,
            'categoryId': categoryId,
            'reportName': reportName,
            'version': isSaveAs ? null : version === '' ? null : version,
            'description': '',
            'isPublic': false,
            'isDraft': isDraft === 'true' ? true : false,
            'isEdit': isEditReport
        };

        args.data = reportServerData;
    }
}

function updateWidgetData(eventArgs) {
    var designer = $('#' + controlId).data('boldReportDesigner');
    if (!fileCabinetDialog && window.FileCabinet) {
        fileCabinetDialog = new FileCabinet(designer);
    }
    var boldItemData = {
        datasets: designer.getInstance('DataSet').datasets,
        datasetsInfo: JSON.parse(sharedDataInfo),
        itemType: eventArgs.itemType,
        dataFieldsInfo: eventArgs.dataFieldsInfo
    };
    fileCabinetDialog.openFileCabinetDialog(boldItemData, function (args) {
        eventArgs.dataInfo = args;
        designer.renderWidgetItem(eventArgs);
    }, function (args) {
        showDataLists(null, true, args, eventArgs);
    });
}

function populateWidgetData(eventArgs) {
    var designer = $('#' + controlId).data('boldReportDesigner');
    if (!fileCabinetDialog && window.FileCabinet) {
        fileCabinetDialog = new FileCabinet(designer);
    }
    var boldItemData = {
        datasets: designer.getInstance('DataSet').datasets,
        datasetsInfo: JSON.parse(sharedDataInfo),
        itemType: eventArgs.itemType,
        dataFieldsInfo: eventArgs.dataFieldsInfo,
        oldDataInfo: eventArgs.dataInfo
    };
    fileCabinetDialog.openFileCabinetDialog(boldItemData, function (args) {
        eventArgs.wizardInfo = args;
        designer.updateWidgetItem(eventArgs);
    }, function (args) {
        showDataLists(null, true, args, eventArgs);
    });
}

function getItemClass(itemType) {
    if (itemType && window.itemExtensions && window.itemExtensions.length > 0) {
        for (var index = 0; index < window.itemExtensions.length; index++) {
            var itemExtension = window.itemExtensions[index];
            if (itemExtension.displayName && itemExtension.displayName.replace(' ', '').toLowerCase() == itemType.replace(' ', '').toLowerCase()) {
                return itemExtension.className;
            }
        }
    }
    return null;
}

function getExtensionLocale(args) {
    if (args.text && args.locale) {
        var itemType = args.text.split('_')[0];
        var text = args.text.split('_')[1];
        var itemClass = getItemClass(itemType);
        if (itemClass && text) {
            var barcodeLocale;
            var customInstance = window[itemClass];
            var defaultLocale = customInstance.Locale['en-US'];
            if (customInstance.Locale[args.locale]) {
                barcodeLocale = customInstance.Locale[args.locale];
            }
            switch (text.toLowerCase()) {
                case 'title':
                    args.localeText = defaultLocale.toolTip.title;
                    if (barcodeLocale && barcodeLocale.toolTip && barcodeLocale.toolTip.title) {
                        args.localeText = barcodeLocale.toolTip.title;
                    }
                    break;
                case 'desc':
                    args.localeText = defaultLocale.toolTip.description;
                    if (barcodeLocale && barcodeLocale.toolTip && barcodeLocale.toolTip.description) {
                        args.localeText = barcodeLocale.toolTip.description;
                    }
                    break;
                case 'datareq':
                    args.localeText = defaultLocale.toolTip.requirements;
                    if (barcodeLocale && barcodeLocale.toolTip && barcodeLocale.toolTip.requirements) {
                        args.localeText = barcodeLocale.toolTip.requirements;
                    }
                    break;
            }
        }
    }
}

function getObjectListFields() {
    var designer = $('#' + controlId).data('boldReportDesigner');
    var objectListFields = {
        Cabinet: ['Cabinet'], Drawer: ['Drawer'], Folder: ['Folder'], SubFolders: [], File_Name: ['FileName']
    };
    var dataSet = null;
    var datasets = designer.getInstance('DataSet').datasets;
    if (datasets && datasets.length > 0) {
        dataSet = datasets[0];
    }
    if (dataSet && dataSet.Fields && dataSet.Fields.length > 0) {
        var startIndex = 0;
        var endIndex = 0;
        var regx = /([ #$%&()*+',./:;<=>?@\[\\\]^\{|}~_-])/g;
        for (var index = 0; index < dataSet.Fields.length; index++) {
            var fieldName = dataSet.Fields[index].Name.replace(regx, '').toLowerCase().trim();
            if (fieldName === 'folder') {
                startIndex = index + 1;
            }
            else if (fieldName === 'filename') {
                endIndex = index;
                break;
            }
        }
        for (var fieldIndex = startIndex; fieldIndex < endIndex; fieldIndex++) {
            objectListFields.SubFolders.push(dataSet.Fields[fieldIndex].Name);
        }
    }
    return objectListFields;
}

function toolbarRendered(args) {
    var target = args.target;
    var isViewer = target.hasClass('e-reportviewer-toolbarcontainer');
    var id = (isViewer ? '_viewer_' : '') + controlId;
    var ultoolBar = ej.buildTag(
        'ul.e-rptdesigner-toolbarul e-rptdesigner-toolbarul-designSwitcher e-show', '', {
            'float': 'right',
            'width': '180px',
            'height': '35px'
        });
    var designerViewDiv = ej.buildTag('div', '', {
        'height': '36px',
        'display': 'table',
        'position': 'absolute'
    }, {
            'id': id + '_design_header_view_div'
        });
    var designerInnerDiv = ej.buildTag('div', '', {
        'display': 'table-cell',
        'vertical-align': 'middle'
    });
    var designEle = ej.buildTag(
        'span.e-rptdesigner-header-design e-userselect e-designer-fontfamily',
        'Design',
        {},
        {
            'id': id + '_design_header_designview'
        });
    var previewEle = ej.buildTag(
        'span.e-rptdesigner-header-preview e-userselect e-designer-fontfamily',
        'View',
        {},
        {
            'id': id + '_design_header_preview'
        });
    designerInnerDiv.append(designEle, previewEle);
    designerViewDiv.append(designerInnerDiv);
    ultoolBar.append(designerViewDiv);
    if (isViewer) {
        target.append(ultoolBar);
        designEle.bind('click', designViewChange);
        previewEle.addClass('e-rptdesigner-header-selection');
    } else {
        var previewTag = target.find('.e-rptdesigner-toolbarul-preview');
        ultoolBar.insertBefore(previewTag);
        previewEle.bind('click', designViewChange);
        designEle.addClass('e-rptdesigner-header-selection');
    }
}

function designViewChange(args) {
    if (args && args.currentTarget) {
        var targetEle = $(args.currentTarget);
        if (!targetEle.hasClass('e-rptdesigner-header-selection')) {
            if (targetEle.hasClass('e-rptdesigner-header-design')) {
                var designer = $('#' + controlId).data('boldReportDesigner');
                designer.showDesign();
            } else if (targetEle.hasClass('e-rptdesigner-header-preview')) {
                var designer = $('#' + controlId).data('boldReportDesigner');
                designer.showPreview();
            }
        }
    }
}

$("#report-name").keyup(function (event) {
    if (event.keyCode === 13) {
        $("#create-report").click();
    }
});

$("#report-item").keyup(function (event) {
    if (event.keyCode === 13) {
        $("#save-report-item").click();
    }
});

function ReportDraftAlert(content) {
    document.body.style.pointerEvents = "none";
    document.getElementById("dialog").style.pointerEvents = "auto";
    var dialogObj = new ejs.popups.Dialog({
        header: "Save Report Alert",
        content: content,
        width: '420px',
        showCloseIcon: true,
        closeOnEscape: true,
        buttons: [
            {
                'click': () => {
                    dialogObj.hide();
                    document.body.style.pointerEvents = "auto";
                },
                buttonModel: { content: 'Ok', isPrimary: true }
            }
        ],
        close: function () {
            dialogObj.destroy();
            document.body.style.pointerEvents = "auto";
        }
    });
    dialogObj.appendTo('#dialog');
}

function ReportScheduleItem(args, itemName, itemId, parentName, isDraft) {
    if (isDraft.toLowerCase() == "true") {
        ReportDraftAlert(draftAlertMessage);
    }
    else {
        $("#loading_icon").removeClass("hide");
        $("#loading_icon").addClass("show-flex");
        document.location.href = schedulesUrl + "?itemId=" + itemId + "&name=" + itemName + "&parent=" + parentName;
    }
}

function saveAsServer(name, category, catId, reportDescription, isMarkPublic) {
    var designer = $('#' + controlId).data('boldReportDesigner');
    categoryName = category;
    categoryId = catId;
    reportName = name;
    description = reportDescription;
    isPublic = isMarkPublic;
    designer.reportFileName = reportName;
    designer.saveReport(categoryName + '/' + reportName);
    $('#' + controlId + '_designer_header_reportName').val(reportName);
}


$(document).ready(function () {
    $('#sidebarCollapse').on('click', function () {
        $('#mysidebar').toggleClass('collapse');
        $('#sidebarCollapse').toggleClass('buttonCollapse');
        $('#header').toggleClass('headerCollapse');
        $('#content').toggleClass('contentCollapse');
        $('#icon').toggleClass('fa-angle-right');
        var instance = $("#sample_dashboard_embeddedbi").data('BoldBIDashboardDesigner');
        if(instance != null){
            setTimeout(function () {
                instance.resizeDashboard(); 
            }, 100);
        }
        //$("#reportingTool").data('boldReportDesigner').designerResize();
        //setTimeout(function () {
        //    $("#reportingTool").data('boldReportDesigner').designerResize();
        //}, 90);
    });
});

//function controlInitialized(args) {
//    var designer = $('#reportingTool').data('boldReportDesigner');

//}