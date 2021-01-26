async function UsernameCheck() {
    var length = $("#Username").val().length;
    if (length != 0) {
        $("#Status1").html('<font color="Green">Loading...</font>');
        $("#Username").css("border-color", "Green");
        if (length >= 5) {
            try {
                if (await getUser($("#Username").val()) != null) {
                    $("#Status1").html('<font color="Red">Username is already taken.</font>');
                    $("#Username").css("border-color", "Red");
                } else {
                    $("#Status1").html('<font color="Green">Username is available.</font>');
                    $("#Username").css("border-color", "Green");
                }
            } catch (error) {
                $("#Status1").html('<font color="Red"Something went wrong.</font>');
            }
        }
        else {
            $("#Status1").html('<font color="Red">Username must be at least 5 characters long.</font>');
            $("#Username").css("border-color", "Red");
        }
    }
}

async function EmailCheck() {
    if ($("#Email").val().length != 0) {
        try {
            if (await getUser($("#Email").val()) != null) {
                $("#Status2").html('<font color="Red">Email is already asociated with another account.</font>');
                $("#Email").css("border-color", "Red");
            } else {
                $("#Status2").html("");
            }
        } catch (error) {
            $("#Status1").html('<font color="Red"Something went wrong.</font>');
        }
    }
}

function getUser(data) { // TODO: need to call api 
    return Promise.resolve($.ajax({
        url: '/Authentication/GetUser',
        datatype: 'json',
        method: 'GET',
        data: { user: data }
    }));
}
