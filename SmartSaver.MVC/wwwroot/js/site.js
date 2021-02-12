$(document).ready(function () {
    $("form").on('submit', function () {
        $(".close").click();
        $('.preloader').show();
    });
});

function getObjects(obj, key, val) {
    var objects = [];
    for (var i in obj) {
        if (!obj.hasOwnProperty(i)) continue;
        if (typeof obj[i] == 'object') {
            objects = objects.concat(getObjects(obj[i], key, val));
        } else if (i == key && obj[key] == val) {
            objects.push(obj);
        }
    }
    return objects;
}

function setError(input, status, message) {
    status.html('<font color="Red">' + message + '</font>');
    input.css("border-color", "Red");
    input.removeClass("is-valid").addClass("is-invalid");
}

function setSuccess(input, status, message) {
    status.html('<font color="Green">' + message + '</font>');
    input.css("border-color", "Green");
    input.removeClass("is-invalid").addClass("is-valid");
}