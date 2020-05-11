$(document).ready(function () {
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

playerHub.on("Play", (result) => {

    if (result == "paused") {
        $(".button.play").removeClass("pause");
        pausePlayer();
    }
    else if (result == "playing") {
        $(".button.play").addClass("pause");
        playPlayer();
    }
    else {
        $(".button.play").addClass("pause");
        document.getElementById("stopButton").disabled = false;
        document.getElementById("skipButton").disabled = false;
        startPlayer(result);
    }
});

playerHub.on("Stop", () => {

    $(".button.play").removeClass('pause');
    document.getElementById("stopButton").disabled = true;
    document.getElementById("skipButton").disabled = true;
    checkQueueEmpty(true);
    disposePlayer();
});

function playClicked() {

    playerHub.invoke("Play").catch(function (err) {
        return console.error(err.toString());
    });
}

function stopClicked() {

    playerHub.invoke("Stop").catch(function (err) {
        return console.error(err.toString());
    });
}

function skipClicked() {

    playNextSong();
}
