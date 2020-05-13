var appHub = new signalR.HubConnectionBuilder().withUrl("/appHub").build();

appHub.start().then(function () {

    appHub.invoke("JoinSession", Cookies.get('sessionId'), Cookies.get('userId'), true).catch(function (err) {
        return console.error(err.toString());
    });
    document.getElementById("searchBtn").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

setInterval(function () {
    setSessionCookie(Cookies.get('sessionId'), Cookies.get('userId'));
    appHub.invoke("Ping", Cookies.get('userId'), true).catch(function (err) {
        return console.error(err.toString());
    });
}, 300000);

$(".sessionConnected").hide();

appHub.on("SessionCreated", (sessionId, userId) => {

    setSessionCookie(sessionId, userId);
    console.log("Session: " + Cookies.get('sessionId') + " User: " + userId);
    $("#newSessionId").val(sessionId);
    $(".createSessionInitial").hide();
    $(".createSessionSuccessful").show();
    sessionConnected();
});

appHub.on("SessionJoined", (success, sessionId, userId) => {

    if (success) {
        $("#enteredSessionId").removeClass("is-invalid");
        $("#enteredSessionId").addClass("is-valid");
        setSessionCookie(sessionId, userId);
        setTimeout(function () {
            $("#joinSessionModal").modal("hide");
            $("#enteredSessionId").removeClass("is-valid");
        }, 500);
        sessionConnected();
    }
    else {
        $("#enteredSessionId").addClass("is-invalid");
    }
});

appHub.on("SessionLeft", () => {

    setSessionCookie('', '');
    sessionDisconnected();
});

function createSession() {

    appHub.invoke("CreateSession").catch(function (err) {
        return console.error(err.toString());
    });
}

function joinSession() {

    var sessionId = $("#enteredSessionId").val();
    appHub.invoke("JoinSession", sessionId, Cookies.get('userId'), false).catch(function (err) {
        return console.error(err.toString());
    });
}

function leaveSession() {

    bootbox.confirm({
        message: "Are you sure you want to leave the session?",
        buttons: {
            confirm: {
                label: 'Yes',
                className: 'btn-success'
            },
            cancel: {
                label: 'No',
                className: 'btn-danger'
            }
        },
        callback: function (result) {
            if (result == true) {
                appHub.invoke("LeaveSession", Cookies.get('sessionId'), Cookies.get('userId')).catch(function (err) {
                    return console.error(err.toString());
                });
            }
        }
    });
}

function sessionConnected() {

    $("#navSessionId").html(Cookies.get('sessionId'));
    $("#displaySessionId").val(Cookies.get('sessionId'));
    $(".sessionDisconnected").hide();
    $(".sessionConnected").show();
}

function sessionDisconnected() {

    $(".sessionConnected").hide();
    $(".sessionDisconnected").show();
    $(".createSessionSuccessful").hide();
    $(".createSessionInitial").show();
}

function setSessionCookie(sessionId, userId) {

    Cookies.remove('sessionId', { expires: '' });
    Cookies.set('sessionId', sessionId, { expires: 1 / 48 });
    Cookies.remove('userId', { expires: '' });
    Cookies.set('userId', userId, { expires: 1 / 48 });
}