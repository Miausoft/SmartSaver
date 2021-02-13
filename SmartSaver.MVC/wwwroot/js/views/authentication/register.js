$(document).ready(function () {
    $("#register").on('click', function (e) {
        UsernameCheck().then(function (result) {
            if (!result) {
                e.preventDefault();
            }
        });

        EmailCheck().then(function (result) {
            if (!result) {
                e.preventDefault();
            }
        });

        if (!Password1Check() || !Password2Check()) {
            e.preventDefault();
        }
    });
});

async function UsernameCheck() {
    const username = $("#username");
    const usernameStatus = $("#usernameStatus");

    if (username.val() !== '') {
        setSuccess(username, usernameStatus, "Loading...");
        if (validateUsername(username.val())) {
            if (username.val().length >= 5) {
                try {
                    if (getObjects(await getUsers(), "username", username.val()).length !== 0) {
                        setError(username, usernameStatus, "Username is already taken.");
                    } else {
                        setSuccess(username, usernameStatus, "");
                        return true;
                    }
                } catch (error) {
                    setError(username, usernameStatus, "Something went wrong.");
                }
            } else {
                setError(username, usernameStatus, "Username must be at least 5 characters long.");
            }
        } else {
            setError(username, usernameStatus, "Username may only contain alphanumeric characters.");
        }
    } else {
        setError(username, usernameStatus, "");
    }
}

async function EmailCheck() {
    const email = $("#email");
    const emailStatus = $("#emailStatus");

    if (email.val() !== '') {
        if (!validateEmail(email.val())) {
            setError(email, emailStatus, "Email is invalid.");
        } else {
            try {
                if (getObjects(await getUsers(), "email", email.val()).length !== 0) {
                    setError(email, emailStatus, "Email is already asociated with another account.");
                } else {
                    setSuccess(email, emailStatus, "")
                    return true;
                }
            } catch (error) {
                setError(email, emailStatus, "Something went wrong.");
            }
        }
    } else {
        setError(email, emailStatus, "");
    }
}

function Password1Check(input, status) {
    const password = $("#password1");
    const length = $("#length");
    const number = $("#number");
    const lower = $("#lower");
    const capital = $("#capital");

    if (password.val() !== '') {
        if (password.val().length < 5) {
            bad(length);
        } else {
            good(length);
        }

        if (!hasNumber(password)) {
            bad(number);
        } else {
            good(number);
        }

        if (!hasLower(password)) {
            bad(lower);
        } else {
            good(lower);
        }

        if (!hasCapital(password)) {
            bad(capital);
        } else {
            good(capital);
        }

        if (password.val().length >= 5 && hasNumber(password) && hasLower(password) && hasCapital(password)) {
            setSuccess(password, password, "");
            return true;
        } else {
            setError(password, password, "");
        }
    } else {
        setError(password, password, "");
        bad(length);
        bad(number);
        bad(lower);
        bad(capital);
    }
}

function Password2Check() {
    const password1 = $("#password1");
    const password2 = $("#password2");
    const password2Status = $("#password2Status");

    if (password2.val() === '') {
        setError(password2, password2Status, "The field is required")
    } else if (password1.val() !== password2.val()) {
        setError(password2, password2Status, 'Passwords does not match');
    } else {
        setSuccess(password2, password2Status, "");
        return true;
    }
}

function getUsers() {
    return Promise.resolve($.ajax({
        url: 'https://localhost:44320/users',
        datatype: 'json',
        method: 'GET'
    }));
}

function validateUsername(username) {
    return /^[a-zA-Z][a-zA-Z0-9]*$/.test(username);
}

function validateEmail(email) {
    const re = /^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    return re.test(String(email).toLowerCase());
}

function hasNumber(input) {
    return /\d/.test(input.val());
}

function hasLower(input) {
    return /[a-z]+/.test(input.val());
}

function hasCapital(input) {
    return /[A-Z]+/.test(input.val());
}

function good(input) {
    input.removeClass("font-weight-bold").addClass("font-wight-light");
    input.removeClass("text-danger").addClass("text-success");
}

function bad(input) {
    input.addClass("font-weight-bold");
    input.addClass("text-danger");
}
