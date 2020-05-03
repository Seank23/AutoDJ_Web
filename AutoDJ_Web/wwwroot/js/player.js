var tag = document.createElement('script');
tag.src = "https://www.youtube.com/iframe_api";
var firstScriptTag = document.getElementsByTagName('script')[0];
firstScriptTag.parentNode.insertBefore(tag, firstScriptTag);
var player = null
var initVolume = 10;
var timebarTimer = null

function onPlayerReady(event) {

    player.setVolume(initVolume);
    event.target.playVideo();
    timebarTimer = setInterval(checkPlayerTime, 1000);
}

function onPlayerStateChange(event) {

    if (event.data == 0) {
        playNextSong();
    }
}

function setPlayerVolume(volume) {

    if (player != null)
        player.setVolume(volume);
    else
        initVolume = volume;
}

function playNextSong() {

    $.ajax({
        url: "/Player/?Handler=NextSong",
        type: "GET",
        success: function (result) {
            if (result != "empty") {
                popFromQueue();
                document.getElementById("videoTitle").textContent = result[0];
                player.loadVideoById(result[1])
                checkQueueEmpty(false);
                setQueueDuration();
                $("#timelineBar").css("width", 0);
                $("#timelineTime").html("0:00");
            }
            else {
                checkQueueEmpty(true);
                disposePlayer();
            }
        }
    });
}

function playPlayer() {

    document.getElementById("playState").textContent = "Now Playing...";
    player.playVideo();
}

function pausePlayer() {

    document.getElementById("playState").textContent = "Paused";
    player.pauseVideo();
}

function startPlayer(video) {

    popFromQueue();
    document.getElementById("videoTitle").textContent = video[0];
    initPlayer(video[1])
}

function initPlayer(video) {

    player = new YT.Player('player', {
        videoId: video,
        playerVars: { "disablekb": 1, "fs": 0, "rel": 0, "modestbranding": 1, "enablejsapi": 1 },
        events: { "onReady": onPlayerReady, "onStateChange": onPlayerStateChange }
    });
    $('#player').attr("style", "width: 100%; height: 350px;");
    $("#playerCard").fadeIn(500);
}

function disposePlayer() {

    player.pauseVideo();
    clearInterval(timebarTimer);
    $("#playerCard").fadeOut(500, function () {
        $("#player").remove();
        player = null;
        var playerDiv = document.createElement("DIV");
        playerDiv.id = "player";
        $("#playerContainer").append(playerDiv);
        document.getElementById("videoTitle").textContent = "";
        $("#timelineBar").css("width", 0);
        $("#timelineTime").html("0:00");
        $("#videoDuration").html("0:00");
    });
    
}

function checkPlayerTime() {

    if (player.getPlayerState() == 1) {

        var time = player.getCurrentTime();
        var duration = player.getDuration();
        var percentComplete = (time / duration) * 100;
        $("#timelineBar").css("width", percentComplete + "%");
        $("#timelineTime").html(intToTime(Math.round(time)));
        $("#videoDuration").html(intToTime(Math.round(player.getDuration())));
    }
}

function intToTime(seconds) {
    if (seconds % 60 < 10)
        return Math.floor(seconds / 60) + ":0" + (seconds % 60);
    else
        return Math.floor(seconds / 60) + ":" + (seconds % 60);
}