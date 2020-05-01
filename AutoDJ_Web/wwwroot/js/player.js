var tag = document.createElement('script');
tag.src = "https://www.youtube.com/iframe_api";
var firstScriptTag = document.getElementsByTagName('script')[0];
firstScriptTag.parentNode.insertBefore(tag, firstScriptTag);
var player = null
var initVolume = 10;

function onPlayerReady(event) {

    player.setVolume(initVolume);
    event.target.playVideo();
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
                checkQueueEmpty();
                setQueueDuration();
            }
            else {
                disposePlayer();
                checkQueueEmpty();
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
    checkQueueEmpty();
    setQueueDuration();
}

function initPlayer(video) {

    player = new YT.Player('player', {
        videoId: video,
        playerVars: { "disablekb": 1, "fs": 0, "rel": 0, "modestbranding": 1 },
        events: { "onReady": onPlayerReady, "onStateChange": onPlayerStateChange }
    });
    $('#player').attr("style", "width: 100%; height: 350px;");
    $("#playerCard").show();
}

function disposePlayer() {

    player.pauseVideo();
    $("#playerCard").hide();
    $("#player").remove();
    player = null;
    var playerDiv = document.createElement("DIV");
    playerDiv.id = "player";
    $("#playerContainer").append(playerDiv);
    document.getElementById("videoTitle").textContent = "";
}