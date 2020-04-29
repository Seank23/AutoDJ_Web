var tag = document.createElement('script');
tag.src = "https://www.youtube.com/iframe_api";
var firstScriptTag = document.getElementsByTagName('script')[0];
firstScriptTag.parentNode.insertBefore(tag, firstScriptTag);
var player = false

function playClicked() {

    $.ajax({
        url: "/Player/?Handler=Play",
        type: "GET",
        success: function (result) {
            var btn = $(".button.play");
            btn.toggleClass("paused");

            if (result == "paused") {
                document.getElementById("playState").textContent = "Paused";
                player.pauseVideo();
            }
            else if (result == "playing") {
                document.getElementById("playState").textContent = "Now Playing...";
                player.playVideo();
            }
            else {
                popFromQueue();
                document.getElementById("videoTitle").textContent = result[0];
                initPlayer(result[1])
                checkQueueEmpty();
                setQueueDuration();
            }
        }
    });
}

function onPlayerReady(event) {

    player.setVolume(10);
    event.target.playVideo();
}

function onPlayerStateChange(event) {

    if (event.data == 0) {
        $.ajax({
            url: "/Player/?Handler=NextSong",
            type: "GET",
            success: function (result) {
                if (result != "empty") {
                    popFromQueue();
                    document.getElementById("videoTitle").textContent = result[0];
                    player.loadVideoById(result[1])
                    checkQueueEmpty();
                    setQueueDuration();
                }
                else {
                    $("#player").remove();
                    var playerDiv = document.createElement("DIV");
                    playerDiv.id = "player";
                    $("#playerContainer").append(playerDiv);
                    document.getElementById("videoTitle").textContent = "";
                    checkQueueEmpty();
                }
            }
        });
    }
}

function initPlayer(video) {

    player = new YT.Player('player', {
        videoId: video,
        playerVars: { "disablekb": 1, "fs": 0, "rel": 0, "modestbranding": 1 },
        events: { "onReady": onPlayerReady, "onStateChange": onPlayerStateChange }
    });
    $('#player').attr("style", "width: 100%; height: 350px;");
}