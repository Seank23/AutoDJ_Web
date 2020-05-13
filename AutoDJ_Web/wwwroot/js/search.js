var searchResults;
var resultIndex = -1;

document.getElementById("searchBtn").disabled = true;

appHub.on("Search", (results) => {

    if (results == -1)
        $("#error").fadeIn(100);
    else if (results == 0)
        $("#noResults").fadeIn(100);
    else {
        $("#details").fadeIn(100);
        searchResults = results;
        updateResult(true);
    }
        
    $("#roller").fadeOut(50);
});

appHub.on("Cancel", () => { cancelSearch(); });

function cancelSearch() {

    searchResults = null;
    resultIndex = -1;
    $("#details").fadeOut(100);
    $("#noResults").fadeOut(100);
    $("#error").fadeOut(100);
    $("#roller").fadeOut(100);
    $('.search-body').removeClass('expand');
    document.getElementById("searchBtn").disabled = false;
    document.getElementById("cancel").disabled = true;
    document.getElementById("searchText").value = "";
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
    document.getElementById("resultLength").textContent = result["duration"];
    document.getElementById("resultDate").textContent = result["publishedDate"];
    $("#resultThumbnail").attr("src", result["thumbnail"]);
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

    document.getElementById("cancel").disabled = false;
    var term = document.getElementById("searchText").value;

    $('.search-body').addClass('expand');
    $("#roller").fadeIn(50);
    document.getElementById("searchBtn").disabled = true;

    appHub.invoke("Search", term).catch(function (err) {
        return console.error(err.toString());
    });
}

function onAddToQueue() {

    var result = searchResults[resultIndex];
    var videoData = [result.videoId.toString(), result.name.toString(), result.channel.toString(), result.publishedDate.toString(), result.duration.toString(), result.thumbnail.toString()];
    appHub.invoke("AddToQueue", videoData).catch(function (err) {
        return console.error(err.toString());
    });
}