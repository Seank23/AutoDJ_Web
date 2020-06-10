var tag = document.createElement('script');
tag.src = "https://www.youtube.com/iframe_api";
var firstScriptTag = document.getElementsByTagName('script')[0];
firstScriptTag.parentNode.insertBefore(tag, firstScriptTag);
var player = null
var initVolume = 10;
var timebarTimer = null

var playerState = "";

appHub.on("NextSong", (result) => { nextSong(result); });

function nextSong(result) {

    if (result != "empty") {
        popFromQueue();
        document.getElementById("videoTitle").textContent = result[0];
        player.loadVideoById(result[1])
        checkQueueEmpty(false);
        setQueueDuration();
        $("#timelineBar").css("width", 0);
        $("#timelineBar").attr("aria-valuenow", 0);
        $("#timelineTime").html("0:00");
    }
    else {
        checkQueueEmpty(true);
        disposePlayer();
    }
}

appHub.on("SyncPlayer", (video, time) => {

    setupControlsOnPlay();
    document.getElementById("videoTitle").textContent = video[0];
    initPlayer(video[1], time);
    checkQueueEmpty();
    document.getElementById('playButton').disabled = false;
});

function playNextSong() {

    if (Cookies.get('sessionId') != "") {
        appHub.invoke("NextSong", Cookies.get('sessionId'), false).catch(function (err) {
            return console.error(err.toString());
        });
    }
    else {
        if (clientQueue.length > 0)
            nextSong([clientQueue[0][1][1], clientQueue[0][1][0]]);
        else
            nextSong("empty");
    }
}

function onPlayerReady(event) {

    player.setVolume(initVolume);
    event.target.playVideo();
    timebarTimer = setInterval(checkPlayerTime, 500);
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

function playPlayer() {

    document.getElementById("playState").textContent = "Now Playing...";
    player.playVideo();
}

function pausePlayer() {

    document.getElementById("playState").textContent = "Paused";
    player.pauseVideo();
}

function startPlayer(video) {

    if (Cookies.get('sessionId') == "")
        video = [clientQueue[0][1][1], clientQueue[0][1][0]];

    popFromQueue();
    document.getElementById("videoTitle").textContent = video[0];
    initPlayer(video[1])

}

function initPlayer(video, time) {

    playerState = "paused";
    player = new YT.Player('player', {
        videoId: video,
        playerVars: { "start": time, "autoplay": 1, "disablekb": 1, "fs": 0, "rel": 0, "modestbranding": 1, "enablejsapi": 1 },
        events: { "onReady": onPlayerReady, "onStateChange": onPlayerStateChange }
    });
    $("#playerCard").fadeIn(500);
}

function disposePlayer() {

    player.pauseVideo();
    clearInterval(timebarTimer);
    playerState = "";
    $("#playerCard").fadeOut(500, function () {
        $("#player").remove();
        player = null;
        var playerDiv = document.createElement("DIV");
        playerDiv.id = "player";
        $("#playerContainer").append(playerDiv);
        document.getElementById("videoTitle").textContent = "";
        $("#timelineBar").css("width", 0);
        $("#timelineTime").html("0:00");
    });
    
}

function checkPlayerTime() {

    if (player.getPlayerState() == 1) {

        var time = player.getCurrentTime();
        var duration = player.getDuration();
        var percentComplete = (time / duration) * 100;
        $("#timelineBar").css("width", percentComplete + "%");
        $("#timelineBar").attr("aria-valuenow", percentComplete);
        $("#timelineTime").html(intToTime(Math.round(time)));
    }
}

function intToTime(seconds) {
    if (seconds % 60 < 10)
        return Math.floor(seconds / 60) + ":0" + (seconds % 60);
    else
        return Math.floor(seconds / 60) + ":" + (seconds % 60);
}

function isPlayerStateVaild() {

    if (player != null) {
        if (player.getCurrentTime() >= player.getDuration() - 1)
            return false;
    }
    return true;
}