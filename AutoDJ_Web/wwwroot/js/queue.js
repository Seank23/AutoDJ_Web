const initialDisplayCount = 10;
const topPos = $("#queueCardBody").outerHeight() + 20;
var curHeight = -20;
var curTop = topPos;

var clientQueue = [];
var queueCountTotal = 0;

appHub.on("SetRating", (rating, id) => { setRating(rating, id) });

appHub.on("UpdateOrder", (orderList) => { updateOrderClient(orderList); });

appHub.on("RemoveItem", (id) => { removeItem(id); });

appHub.on("ClearQueue", () => { clearQueue(); });

appHub.on("AddToQueue", (queueItem) => {

    growQueueContainer(1);
    addToQueue(queueItem);
    updateOrder();
});

appHub.on("SetQueueDuration", (duration) => {

    document.getElementById("queueTime").textContent = duration;

    if ($("#queueContainer").children().length == 1)
        document.getElementById("songCount").textContent = $("#queueContainer").children().length + " song - ";
    else
        document.getElementById("songCount").textContent = $("#queueContainer").children().length + " songs - ";
});

appHub.on("SyncQueue", (queue) => {

    growQueueContainer(queue.length);
    for (var i = 0; i < queue.length; i++) {
        addToQueue(queue[i]);
    }
    checkItemsVisible();
});

appHub.on("QueuePlaylist", (playlist) => {

    if (Cookies.get('sessionId') == "") {
        for (i = 0; i < playlist.length; i++) {
            var vid = playlist[i]['video'];
            var video = [vid.videoId.toString(), vid.name.toString(), vid.channel.toString(), vid.publishedDate.toString(), vid.duration.toString(), vid.thumbnail.toString()];
            clientQueue.push([playlist[i]['id'], video, playlist[i]['rating']]);
        }
    }
    growQueueContainer(playlist.length);
    var i = 0;
    while (i < playlist.length) {
        addToQueue(playlist[i]);
        i++;
    }
    cancelSearch();
    checkItemsVisible();
    $(".overlay").fadeOut(200);
});

appHub.on("QueueMigrated", () => {

    clearQueue();
    sessionConnected();
});

function addToQueue(queueItem) {

    console.log(queueItem);
    $("#queueEmpty").hide();
    document.getElementById('clearQBtn').disabled = false;
    document.getElementById("playButton").disabled = false;
    if (player != null)
        document.getElementById("skipButton").disabled = false;

    queueCountTotal++;

    var url = "";
    var itemId = "";
    if (Cookies.get('sessionId') == "" && !isPlaylist) {
        for (i = 0; i < queueItem[1].length; i++) {
            queueItem[1][i] = queueItem[1][i].replace("&", "%26");
        }
        url = `/QueueItemTemplate?id=${queueItem[0]}&rating=${queueItem[2]}&videoId=${queueItem[1][0]}&videoName=${queueItem[1][1]}&videoChannel=${queueItem[1][2]}&videoDate=${queueItem[1][3]}&videoDuration=${queueItem[1][4]}&videoThumbnail=${queueItem[1][5]}`;
        itemId = "item" + queueItem[0];
    }
    else {
        var keys = Object.keys(queueItem['video']);
        for (i = 0; i < keys.length; i++) {
            queueItem['video'][keys[i]] = queueItem['video'][keys[i]].replace('&', "%26");
        }
        url = `/QueueItemTemplate?id=${queueItem['id']}&rating=${queueItem['rating']}&videoId=${queueItem['video']['videoId']}&videoName=${queueItem['video']['name']}&videoChannel=${queueItem['video']['channel']}&videoDate=${queueItem['video']['publishedDate']}&videoDuration=${queueItem['video']['duration']}&videoThumbnail=${queueItem['video']['thumbnail']}`;
        itemId = "item" + queueItem['id'];
    }
    url = url.split(' ').join('%20');

    var queueContainer = document.getElementById("queueContainer");
    var itemContainer = document.createElement("div");
    itemContainer.id = itemId
    itemContainer.classList.add("queueItem");
    itemContainer.style.top = curTop + "px";
    queueContainer.appendChild(itemContainer);
    $("#" + itemId).load(url);

    $(queueContainer).show();
    setQueueDuration();

    if (curTop - topPos > queueContainer.style.maxHeight.substring(0, queueContainer.style.maxHeight.length)) {
        $(itemContainer).addClass("hidden");
    }
    else
        $(itemContainer).fadeIn(500);

    curHeight += 70;
    curTop += 70;
}

function growQueueContainer(numItems) {

    $(queueContainer).animate({ "height": curHeight + (70 * numItems) }, 200);
}

function shrinkQueueContainer(numItems) {

    $(queueContainer).animate({ "height": curHeight - (70 * numItems) }, 200);
}

function setRating(rating, id) {

    document.getElementById("addVote_" + id).textContent = "Vote (" + rating + ")";
    updateOrder();
}

function updateOrderClient(orderList) {

    var items = $("#queueContainer").children();
    var orderDict = {};

    for (i = 0; i < orderList.length; i++)
        orderDict[orderList[i]] = i;

    for (i = 0; i < items.length; i++) {
        var myItem = items[i];
        var id = myItem.id.substring(4, items[i].id.length);

        $(myItem).animate({ "top": topPos + orderDict[id] * 70 }, 200, function () {
            checkItemsVisible();
        });
    }
    
}

function removeItem(id) {

    $("#item" + id).fadeOut(500, function () {

        shrinkQueueContainer(1);
        curHeight -= 70;
        curTop -= 70;

        $(this).remove();
        setQueueDuration();
        if (!checkQueueEmpty(false))
            updateOrder();

        checkItemsVisible();
    });
}

function updateOrder() {

    if (Cookies.get('sessionId')) {
        appHub.invoke("Order", Cookies.get('sessionId')).catch(function (err) {
            return console.error(err.toString());
        });
    }
    else {
        clientQueue.sort(function (a, b) {
            return parseInt(b[2]) - parseInt(a[2]);
        });
        var orderList = [];
        for (i = 0; i < clientQueue.length; i++)
            orderList.push(clientQueue[i][0]);
        updateOrderClient(orderList);
    }
}

function setQueueDuration() {

    if ($("#queueContainer").children().length != 0) {

        if (Cookies.get('sessionId') != "") {
            appHub.invoke("Duration", Cookies.get('sessionId'), null).catch(function (err) {
                return console.error(err.toString());
            });
        }
        else {
            var list = getDurationList();
            appHub.invoke("Duration", Cookies.get('sessionId'), list).catch(function (err) {
                return console.error(err.toString());
            });
        }
    }
    else {
        document.getElementById("queueTime").textContent = "";
        document.getElementById("songCount").textContent = "";
    }
}

function addVote(id) {

    if (Cookies.get('sessionId') != "") {
        appHub.invoke("AddVote", Cookies.get('sessionId'), id).catch(function (err) {
            return console.error(err.toString());
        });
    }
    else {
        var item = clientQueue.find(function (item) {
            return parseInt(item[0]) == id;
        });
        item[2]++;
        setRating(item[2], id);
    }
}

function remove(id) {

    if (Cookies.get('sessionId') != "") {
        appHub.invoke("Remove", Cookies.get('sessionId'), id).catch(function (err) {
            return console.error(err.toString());
        });
    }
    else {
        for (i = 0; i < clientQueue.length; i++) {
            if (clientQueue[i][0] == id)
                clientQueue.splice(i, 1);
        }
        removeItem(id);
    }
}

function popFromQueue() {

    if (Cookies.get('sessionId') == "")
        clientQueue.splice(0, 1);

    var queue = $("#queueContainer").children();
    if (queue.length > 0) {

        minVal = queue[0].style.top;
        minIndex = 0;
        for (i = 1; i < queue.length; i++) {
            if (parseInt(queue[i].style.top) < parseInt(minVal)) {
                minVal = queue[i].style.top;
                minIndex = i;
            }
        }
        $(queue[minIndex]).fadeOut(500, function () {

            shrinkQueueContainer(1);
            curHeight -= 70;
            curTop -= 70;

            $(this).remove();
            updateOrder();
            checkQueueEmpty(false);
            setQueueDuration();
        });
    }
}

function checkQueueEmpty(onStop) {

    if ($("#queueContainer").children().length == 0) {
        $("#queueEmpty").show();
        $("#queueShowHide").hide();
        document.getElementById("skipButton").disabled = true;
        document.getElementById('clearQBtn').disabled = true;

        if (document.getElementById('player').tagName == "DIV" || onStop == true) {
            document.getElementById('playButton').disabled = true;
            document.getElementById('stopButton').disabled = true;
            $(".button.play").removeClass('pause');
        }
        return true;
    }
    else
        return false;
}

function getDurationList() {

    var durations = []
    for (i = 0; i < clientQueue.length; i++)
        durations.push(clientQueue[i][1][4])
    return durations;
}

function clearQueue() {

    clientQueue = [];
    queueCountTotal = 0;
    var queue = $("#queueContainer").children();
    shrinkQueueContainer(queue.length);
    for (i = 0; i < queue.length; i++) {
        queue[i].remove();
        curHeight -= 70;
        curTop -= 70;
    }
    document.getElementById("queueTime").textContent = "";
    document.getElementById("songCount").textContent = "";
    checkQueueEmpty();
}

function clearQueueClicked() {

    if (Cookies.get('sessionId') != "") {
        appHub.invoke("ClearQueue", Cookies.get('sessionId'), Cookies.get("userId")).catch(function (err) {
            return console.error(err.toString());
        });
    }
    else {
        clearQueue();
    }
}

function queueShowHideClicked() {

    $("#queueContainer").toggleClass("expand");
    checkItemsVisible();
}

function checkItemsVisible() {

    var queue = $("#queueContainer").children();
    var queueHeight = $("#queueContainer").css("max-height");

    if (queueHeight != "none")
        queueHeight = queueHeight.substring(0, queueHeight.length - 2);
    else
        queueHeight = 1000000;

    for (i = 0; i < queue.length; i++) {
        var itemTop = queue[i].style.top.substring(0, queue[i].style.top.length - 2)
        if (parseInt(itemTop) - topPos > parseInt(queueHeight) - 48) {
            $(queue[i]).addClass("hidden");
        }
        else {
            if ($(queue[i]).hasClass("hidden"))
                $(queue[i]).removeClass("hidden");
            $(queue[i]).fadeIn(500);
        }
    }
    displayQueueShowHide();
}

function displayQueueShowHide() {

    if ($("#queueContainer").children().length <= initialDisplayCount) {
        $("#queueContainer").removeClass("expand");
        $("#queueShowHide").fadeOut(500);
    }
    else if ($(".hidden").length == 0 && $("#queueContainer").hasClass("expand")) {
        $("#queueShowHideButton").html("Show Less");
        $("#queueShowHide").fadeIn(500);
    }
    else {    
        $("#queueShowHideButton").html("Show All");
        $("#queueShowHide").fadeIn(500);
    }
}

