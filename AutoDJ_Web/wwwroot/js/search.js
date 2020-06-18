var searchResults;
var resultIndex = -1;

var searchClicked = false;
var isPlaylist = false;

document.getElementById("searchBtn").disabled = true;

appHub.on("Search", (results, playlist) => {

    if (results == -1)
        $("#error").fadeIn(100);
    else if (results == 0)
        $("#noResults").fadeIn(100);
    else {
        $("#details").fadeIn(100);
        searchResults = results;
        isPlaylist = playlist;
        updateResult(true);
    }
        
    $("#roller").fadeOut(50);
});

appHub.on("Cancel", () => { cancelSearch(); });

function cancelSearch() {

    searchClicked = false;
    $("#searchBtn").html("Search");
    $("#searchBtn").removeClass("btn-danger");
    $("#searchBtn").addClass("btn-primary");
    searchResults = null;
    resultIndex = -1;
    $("#details").fadeOut(100);
    $("#noResults").fadeOut(100);
    $("#error").fadeOut(100);
    $("#roller").fadeOut(100);
    $('.search-body').removeClass('expand');
    document.getElementById("searchBtn").disabled = false;
    document.getElementById("searchText").disabled = false;
    document.getElementById("searchText").value = "";
    $(".myToggle").addClass("off");
}

function updateResult(next) {

    if (next)
        resultIndex++;
    else
        resultIndex--;

    var result = searchResults[resultIndex];

    document.getElementById("resultIndex").textContent = resultIndex + 1;
    document.getElementById("numResults").textContent = searchResults.length;
    document.getElementById("resultName").textContent = result["name"];
    document.getElementById("resultChannel").textContent = result["channel"];
    document.getElementById("resultDate").textContent = result["publishedDate"];
    $("#resultThumbnail").attr("src", result["thumbnail"]);

    if (isPlaylist) {
        document.getElementById("resultType").textContent = "Playlist";
        document.getElementById("lengthOrDescription").textContent = "Description";
        document.getElementById("resultLengthOrDescription").textContent = result["description"];
    }
    else {
        document.getElementById("resultType").textContent = "Video";
        document.getElementById("lengthOrDescription").textContent = "Length";
        document.getElementById("resultLengthOrDescription").textContent = result["duration"];
    }

    if (resultIndex > 0)
        document.getElementById("prevButton").disabled = false;
    else
        document.getElementById("prevButton").disabled = true;
    if (resultIndex < searchResults.length - 1)
        document.getElementById("nextButton").disabled = false;
    else
        document.getElementById("nextButton").disabled = true;
}

function onSearch() {

    if (!searchClicked) {

        searchClicked = true;
        $("#searchBtn").html("Cancel");
        $("#searchBtn").removeClass("btn-primary");
        $("#searchBtn").addClass("btn-danger");

        var term = document.getElementById("searchText").value;
        document.getElementById("searchText").disabled = true;

        $('.search-body').addClass('expand');
        $("#roller").fadeIn(50);

        var type = false;
        if ($("#playlistToggle").is(":checked"))
            type = true;

        appHub.invoke("Search", term, type).catch(function (err) {
            return console.error(err.toString());
        });
    }
    else {
        cancelSearch();
    }
}

function onAddToQueue() {

    var result = searchResults[resultIndex];
    var resultData = [];
    if (isPlaylist) {
        resultData = [result.playlistId.toString(), result.name.toString(), result.channel.toString(), result.publishedDate.toString(), result.description.toString(), result.thumbnail.toString()];
        $("#loadingText").html("Loading Playlist...");
        $(".overlay").fadeIn(200);
    }
    else
        resultData = [result.videoId.toString(), result.name.toString(), result.channel.toString(), result.publishedDate.toString(), result.duration.toString(), result.thumbnail.toString()];

    if (Cookies.get('sessionId') != "") {
        appHub.invoke("AddToQueue", Cookies.get('sessionId'), Cookies.get('userId'), resultData, isPlaylist).catch(function (err) {
            return console.error(err.toString());
        });
    }
    else if (isPlaylist) {
        appHub.invoke("QueuePlaylist", null, clientQueue.length, null, resultData).catch(function (err) {
            return console.error(err.toString());
        });
    }
    else {
        var queueItem = [clientQueue.length, resultData, 1];
        clientQueue.push(queueItem);
        cancelSearch();
        growQueueContainer(1);
        addToQueue(queueItem);
        updateOrder();
    }
}