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

appHub.on("Play", (result) => {

    if (result == "paused") {
        $(".button.play").removeClass("pause");
        pausePlayer();
    }
    else if (result == "playing") {
        $(".button.play").addClass("pause");
        playPlayer();
    }
    else {
        setupControlsOnPlay();
        startPlayer(result);
    }
});

appHub.on("Stop", () => {

    $(".button.play").removeClass('pause');
    document.getElementById("stopButton").disabled = true;
    document.getElementById("skipButton").disabled = true;
    checkQueueEmpty(true);
    disposePlayer();
});

function setupControlsOnPlay() {

    $(".button.play").addClass("pause");
    document.getElementById("stopButton").disabled = false;
    document.getElementById("skipButton").disabled = false;
}

function playClicked() {

    appHub.invoke("Play").catch(function (err) {
        return console.error(err.toString());
    });
}

function stopClicked() {

    appHub.invoke("Stop").catch(function (err) {
        return console.error(err.toString());
    });
}

function skipClicked() {

    playNextSong();
}
