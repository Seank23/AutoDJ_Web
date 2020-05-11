var appHub = new signalR.HubConnectionBuilder().withUrl("/appHub").build();

document.getElementById("searchBtn").disabled = true;

appHub.start().then(function () {
    document.getElementById("searchBtn").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

appHub.on("Search", (status) => {

    if (status == 1) {
        $("#details").fadeIn(100);
        updateResult(true);
    }
    else if (status == 0)
        $("#noResults").fadeIn(100);
    else if (status == -1)
        $("#error").fadeIn(100);

    $("#roller").fadeOut(50);
});

appHub.on("UpdateResult", (result) => {

    if (result != null) {
        document.getElementById("resultIndex").textContent = result[0];
        document.getElementById("numResults").textContent = result[1];
        document.getElementById("resultName").textContent = result[3];
        document.getElementById("resultChannel").textContent = result[4];
        document.getElementById("resultLength").textContent = result[6];
        document.getElementById("resultDate").textContent = result[5];
        $("#resultThumbnail").attr("src", result[7]);
        if (result[0] > 1)
            document.getElementById("prevButton").disabled = false;
        else
            document.getElementById("prevButton").disabled = true;
        if (result[0] < result[1])
            document.getElementById("nextButton").disabled = false;
        else
            document.getElementById("nextButton").disabled = true;
    }
});

appHub.on("Cancel", () => {

    cancelSearch();
});

appHub.on("ResultToAdd", (result) => {

    addToQueue(result[0]);
});

function cancelSearch() {

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

    appHub.invoke("UpdateResult", next).catch(function (err) {
        return console.error(err.toString());
    });
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


function onCancelSearch() {

    appHub.invoke("Cancel").catch(function (err) {
        return console.error(err.toString());
    });
}

function onAddToQueue() {

    appHub.invoke("ResultId").catch(function (err) {
        return console.error(err.toString());
    });
}