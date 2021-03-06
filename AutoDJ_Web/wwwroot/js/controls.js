﻿$(document).ready(function () {
    $("#volumeSlider").slider({
        min: 0,
        max: 100,
        value: 0,
        range: "min",
        slide: function (event, ui) {
            setVolume(ui.value);
        }
    });
});

function setVolume(myVolume) {
    setPlayerVolume(myVolume);
}

appHub.on("Play", (result) => { play(result); });

appHub.on("Stop", () => { stopClient(); });

function play(result) {

    if (result == "paused") {
        $(".button.play").removeClass("pause");
        playerState = "playing";
        pausePlayer();
    }
    else if (result == "playing") {
        $(".button.play").addClass("pause");
        playerState = "paused";
        playPlayer();
    }
    else {
        playerState = "playing";
        setupControlsOnPlay();
        startPlayer(result);
    }
}

function stopClient() {

    $(".button.play").removeClass('pause');
    document.getElementById("stopButton").disabled = true;
    document.getElementById("skipButton").disabled = true;
    checkQueueEmpty(true);
    disposePlayerCard();
}

function setupControlsOnPlay() {

    $(".button.play").addClass("pause");
    if (permissions['CanStop'] || isHost) {
        document.getElementById("stopButton").disabled = false;
        document.getElementById("skipButton").disabled = false;
    }
}

function playClicked() {

    if (!isPlayerStateVaild)
        return;

    if (Cookies.get('sessionId') != "") {
        appHub.invoke("Play", Cookies.get('sessionId')).catch(function (err) {
            return console.error(err.toString());
        });
    }
    else
        play(playerState);
}

function stopClicked() {

    if (!isPlayerStateVaild)
        return;

    if (Cookies.get('sessionId') != "") {
        appHub.invoke("Stop", Cookies.get('sessionId'), Cookies.get('userId')).catch(function (err) {
            return console.error(err.toString());
        });
    }
    else
        stopClient();
}

function skipClicked() {

    if (!isPlayerStateVaild)
        return;
    playNextSong();
}
