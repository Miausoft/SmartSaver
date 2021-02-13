$(document).ready(function () {
    $("#spendingButton").on('click', function () {
        if (!checkInputs($("#spending"), $("#spending").val().trim(), $("#spendingStatus")))
            return false;
    });

    $("#incomeButton").on('click', function () {
        if (!checkInputs($("#income"), $("#income").val().trim(), $("#incomeStatus")))
            return false;
    });

    $("#pdfButton").on('click', function () {
        $("#fromStatus").html("");
        $("#toStatus").html("");

        if (!checkDate($("#fromDate"), $("#toDate"), $("#fromStatus"), $("#toStatus")))
            return false;
    });
});

function checkInputs(input, value, status) {
    if (value === '') {
        setError(input, status, "Amount cannot be blank");
    } else if (!isNumber(value)) {
        setError(input, status, "Please provide numbers only without any sign");
    } else {
        setSuccess(input, status, "");
        return true;
    }
}

function checkDate(from, to, fromStatus, toStatus) {
    if (from.val() === '') {
        setError(from, fromStatus, "Please pick a date");
    } if (to.val() === '') {
        setError(to, toStatus, "Please pick a date");
    } else if (from.val() > to.val()) {
        setError(to, toStatus, "Invalid date selection");
    } else {
        setSuccess(to, toStatus, "");
        return true;
    }
}

function isNumber(amount) {
    return /^(0*[1-9][0-9]*(\,[0-9]+)?|0+\,[0-9]*[1-9][0-9]*)$/.test(amount);
}

