var sessionId;
var userId;

appHub.on("SessionCreated", (session, user) => {
    sessionId = session;
    userId = user;
    console.log("Session: " + sessionId + " User: " + userId);
    $("#newSessionId").val(session);
    $(".createSessionInitial").hide();
    $(".createSessionSuccessful").show();
})

appHub.on("SessionJoined", (success) => {

    if (!success) {
        $("#enteredSessionId").addClass("is-invalid");
    }
    else {
        $("#enteredSessionId").addClass("is-valid");
        setTimeout(function () { $("#joinSessionModal").modal("hide"); }, 500);
    }
})

function createSession() {

    appHub.invoke("CreateSession").catch(function (err) {
        return console.error(err.toString());
    });
}

function joinSession() {

    var sessionId = $("#enteredSessionId").val();
    appHub.invoke("JoinSession", sessionId).catch(function (err) {
        return console.error(err.toString());
    });
}