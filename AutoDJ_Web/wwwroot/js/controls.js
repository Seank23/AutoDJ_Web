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

function playClicked() {

    $.ajax({
        url: "/Player/?Handler=Play",
        type: "GET",
        success: function (result) {

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
        }
    });
}

function stopClicked() {

    $.ajax({
        url: "/Player/?Handler=Stop",
        type: "GET",
        success: function () {

            $(".button.play").removeClass('pause');
            document.getElementById("stopButton").disabled = true;
            document.getElementById("skipButton").disabled = true;
            checkQueueEmpty(true);
            disposePlayer();
        }
    });
}

function skipClicked() {

    playNextSong();
}
