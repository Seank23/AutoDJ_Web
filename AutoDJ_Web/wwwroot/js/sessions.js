﻿var appHub = new signalR.HubConnectionBuilder().withUrl("/appHub").build();
var isHost = false;
var permissions = {"CanStop": true, "CanRemove": true, "CanAddPlaylist": true, "HidePlayer": false, "CanClearQueue": true};

appHub.start().then(function () {

    appHub.invoke("PingServer", Cookies.get('userId')).catch(function (err) {
        return console.error(err.toString());
    });
    appHub.invoke("JoinSession", Cookies.get('sessionId'), Cookies.get('userId'), true).catch(function (err) {
        return console.error(err.toString());
    });
    document.getElementById("searchBtn").disabled = false;
    document.getElementById("createSessionBtn").disabled = false;
    document.getElementById("joinSessionBtn").disabled = false;
    $(".overlay").fadeOut(200);
}).catch(function (err) {
    return console.error(err.toString());
});

setInterval(function () {

    appHub.invoke("PingServer", Cookies.get('userId')).catch(function (err) {
        return console.error(err.toString());
    });
}, 30000);

$(".sessionConnected").hide();

appHub.on("SessionCreated", (sessionId, userId) => {

    setSessionCookie(sessionId, userId);
    isHost = true;
    console.log("Session: " + Cookies.get('sessionId') + " User: " + userId);
    $("#newSessionId").val(sessionId);
    $(".createSessionInitial").hide();
    $(".permissionsSetup").hide();
    $(".createSessionSuccessful").show();

    if (player != null) {
        var details = [player.getVideoUrl().substring(32, player.getVideoUrl().length), document.getElementById("videoTitle").textContent, Math.round(player.getCurrentTime()).toString()];
        appHub.invoke("MigrateClientPlayer", sessionId, details).catch(function (err) {
            return console.error(err.toString());
        });
    }
    if (clientQueue.length > 0) {
        appHub.invoke("MigrateClientQueue", sessionId, JSON.stringify(clientQueue)).catch(function (err) {
            return console.error(err.toString());
        }); 
    }
    else
        sessionConnected();
});

appHub.on("SessionJoined", (success, sessionId, userId, clientPermissions, host) => {

    if (success) {
        permissions = clientPermissions;
        isHost = host;
        if (permissions['CanAddPlaylist'] == false && !isHost)
            $("#playlistToggle").prop('disabled', true);

        clearQueue();
        $("#enteredSessionId").removeClass("is-invalid");
        $("#enteredSessionId").addClass("is-valid");
        setSessionCookie(sessionId, userId);
        console.log("Session: " + Cookies.get('sessionId') + " User: " + userId);
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

    stopClient();
    clearQueue();
    sessionDisconnected();
});

appHub.on("SessionSynced", () => {
    $(".overlay").fadeOut(200);
});

appHub.on("PingReturned", (sessionId, userId) => {
    setSessionCookie(sessionId, userId);
});

function createSession() {

    var permissions = [!$("#stopToggle").parent().hasClass("off"), !$("#removeToggle").parent().hasClass("off"), !$("#noPlaylistToggle").parent().hasClass("off"), !$("#hidePlayerToggle").parent().hasClass("off")];
    appHub.invoke("CreateSession", Cookies.get('userId'), permissions).catch(function (err) {
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

    $("#loadingText").html("Syncing Session...")
    $(".overlay").fadeIn(200);
    if (isHost)
        $("#navSessionId").html(Cookies.get('sessionId') + " (Host)");
    else
        $("#navSessionId").html(Cookies.get('sessionId') + " (User)");
    $("#displaySessionId").val(Cookies.get('sessionId'));
    $(".sessionDisconnected").hide();
    $(".sessionConnected").show();
    checkQueueEmpty();

    appHub.invoke("SyncSession", Cookies.get('sessionId')).catch(function (err) {
        return console.error(err.toString());
    });
}

function sessionDisconnected() {

    permissions = { "CanStop": true, "CanRemove": true, "CanAddPlaylist": true, "HidePlayer": false, "CanClearQueue": true };
    setSessionCookie('', '');
    isHost = false;
    $(".sessionConnected").hide();
    $(".sessionDisconnected").show();
    $(".createSessionSuccessful").hide();
    $(".createSessionInitial").show();
    $(".permissionsSetup").show();
    checkQueueEmpty();
}

function setSessionCookie(sessionId, userId) {

    Cookies.remove('sessionId', { expires: '' });
    Cookies.set('sessionId', sessionId, { expires: 1 / 48 });
    Cookies.remove('userId', { expires: '' });
    Cookies.set('userId', userId, { expires: 1 / 48 });
}