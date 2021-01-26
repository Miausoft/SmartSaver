$(document).ready(function () {
    $("#spendingButton").on('click', function () {
        if (!checkInputs($("#spending").val().trim(), $("#spendingStatus")))
            return false;
    });

    $("#incomeButton").on('click', function () {
        if (!checkInputs($("#income").val().trim(), $("#incomeStatus")))
            return false;
    });

    $("#pdfButton").on('click', function () {
        $("#fromStatus").html("");
        $("#toStatus").html("");

        if (!checkDate($("#fromDate").val(), $("#toDate").val(), $("#fromStatus"), $("#toStatus")))
            return false;
    });
});

function checkInputs(value, status) {
    if (value === '') {
        setError(status, "Amount cannot be blank");
    } else if (!isNumber(value)) {
        setError(status, "Please provide numbers only without any signs");
    } else {
        return true;
    }
}

function checkDate(from, to, fromStatus, toStatus) {
    if (from === '') {
        setError(fromStatus, "Please pick a date");
    } if (to === '') {
        setError(toStatus, "Please pick a date");
    } else if (from > to) {
        setError(toStatus, "Invalid date selection");
    } else {
        setError(toStatus, "");
        return true;
    }
}

function setError(status, message) {
    $(status).html('<font color="Red">' + message + '</font>');
}

function isNumber(amount) {
    return /^(0*[1-9][0-9]*(\,[0-9]+)?|0+\,[0-9]*[1-9][0-9]*)$/.test(amount);
}

