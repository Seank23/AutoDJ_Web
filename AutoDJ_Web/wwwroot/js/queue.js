﻿const topPos = 90
var curHeight = 0;
var curTop = topPos;

appHub.on("AddToQueue", (queueItem, sessionId) => { addToQueue(queueItem, sessionId); });

appHub.on("UpdateOrder", (orderList) => {

    var items = $("#queueContainer").children();
    var orderDict = {};

    for (i = 0; i < orderList.length; i++)
        orderDict[orderList[i]] = i;

    for (i = 0; i < items.length; i++) {
        var myItem = items[i];
        var id = myItem.id.substring(4, items[i].id.length);

        $(myItem).animate({ "top": topPos + orderDict[id] * 70 }, 200);
    }
});

appHub.on("SetQueueDuration", (duration) => {

    document.getElementById("queueTime").textContent = duration;

    if ($("#queueContainer").children().length == 1)
        document.getElementById("songCount").textContent = $("#queueContainer").children().length + " song - ";
    else
        document.getElementById("songCount").textContent = $("#queueContainer").children().length + " songs - ";
});

appHub.on("SetRating", (rating, id) => {

    console.log(rating);
    document.getElementById("addVote_" + id).textContent = "Vote (" + rating + ")";
    updateOrder();
});

appHub.on("RemoveItem", (id) => {

    $("#item" + id).fadeOut(500, function () {
        animateQueueMove();
        $(this).remove();
        setQueueDuration();
        if (!checkQueueEmpty(false))
            updateOrder();
    }); 
});

appHub.on("SyncQueue", (sessionId, queue) => {

    for (i = 0; i < queue.length; i++) {
        addToQueue(queue[i], sessionId);
    }
});

function addToQueue(queueItem, sessionId) {

    console.log(sessionId);
    console.log(queueItem);
    $("#queueEmpty").hide();
    document.getElementById("playButton").disabled = false;
    if (player != null)
        document.getElementById("skipButton").disabled = false;

    var queueContainer = document.getElementById("queueContainer");
    var itemContainer = document.createElement("div");
    var itemId = "item" + queueItem['id'];
    itemContainer.id = itemId
    itemContainer.classList.add("queueItem");
    itemContainer.style.top = curTop + "px";
    queueContainer.appendChild(itemContainer);
    var url = "/QueueItemTemplate?sessionId=" + sessionId + "&itemId=" + queueItem['id'];
    console.log(url);
    $("#" + itemId).load("/QueueItemTemplate?sessionId=" + sessionId + "&itemId=" + queueItem['id']);
    $(queueContainer).show();
    setQueueDuration();

    $(itemContainer).fadeIn(500);
    $(queueContainer).animate({ "height": curHeight + 70 }, 200);
    curHeight += 70;
    curTop += 70;
    return 1;
}

function updateOrder() {

    appHub.invoke("Order", Cookies.get('sessionId')).catch(function (err) {
        return console.error(err.toString());
    });
}

function setQueueDuration() {

    if ($("#queueContainer").children().length != 0) {

        appHub.invoke("Duration", Cookies.get('sessionId')).catch(function (err) {
            return console.error(err.toString());
        });
    }
    else {
        document.getElementById("queueTime").textContent = "";
        document.getElementById("songCount").textContent = "";
    }
}

function addVote(id) {

    appHub.invoke("AddVote", Cookies.get('sessionId'), id).catch(function (err) {
        return console.error(err.toString());
    });
}

function remove(id) {

    appHub.invoke("Remove", Cookies.get('sessionId'), id).catch(function (err) {
        return console.error(err.toString());
    });
}

function popFromQueue() {

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
            animateQueueMove();
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
        document.getElementById("skipButton").disabled = true;

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

function animateQueueMove() {

    $(queueContainer).animate({ "height": curHeight - 70 }, 200);
    curHeight -= 70;
    curTop -= 70;
}

