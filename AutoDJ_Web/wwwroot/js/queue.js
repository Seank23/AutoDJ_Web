const topPos = 110
var curHeight = 0;
var curTop = topPos;

function addToQueue() {

    $.ajax({
        url: "/Search/?Handler=ResultId",
        type: "GET",
        success: function (id) {

            $.ajax({
                url: "/Queue/?Handler=Add",
                type: "GET",
                data: { id: id[0] },
                success: function (queueItem) {

                    console.log(queueItem);
                    $("#queueEmpty").hide();
                    document.getElementById("playButton").disabled = false;
                    if(player != null)
                        document.getElementById("skipButton").disabled = false;

                    var queueContainer = document.getElementById("queueContainer");
                    var itemContainer = document.createElement("div");
                    var itemId = "item" + queueItem['id'];
                    itemContainer.id = itemId
                    itemContainer.classList.add("queueItem");
                    itemContainer.style.top = curTop + "px";
                    queueContainer.appendChild(itemContainer);
                    $("#" + itemId).load("/QueueItemTemplate");
                    $(queueContainer).show();
                    cancelSearch();
                    setQueueDuration();

                    $(itemContainer).fadeIn(500);
                    $(queueContainer).animate({ "height": curHeight + 70 }, 200);
                    curHeight += 70; 
                    curTop += 70;
                }
            });
        }
    });
}

function addVote(id) {

    $.ajax({
        url: "/QueueItemTemplate/?Handler=AddVote",
        type: "GET",
        data: { id: id },
        success: function (rating) {

            console.log(rating);
            document.getElementById("addVote_" + id).textContent = "Vote (" + rating + ")";
            updateOrder();
        }
    });
}

function remove(id) {

    $.ajax({
        url: "/QueueItemTemplate/?Handler=Remove",
        type: "GET",
        data: { id: id },
        success: function () {

            $("#item" + id).fadeOut(500, function () {
                animateQueueMove();
                $(this).remove();
                setQueueDuration();
                if (!checkQueueEmpty(false))
                    updateOrder();
            }); 
            
        }
    });
}

function updateOrder() {

    $.ajax({
        url: "/Queue/?Handler=Order",
        type: "GET",
        success: function (orderList) {

            var items = $("#queueContainer").children();
            var orderDict = {};

            for (i = 0; i < orderList.length; i++)
                orderDict[orderList[i]] = i;

            for (i = 0; i < items.length; i++) {
                var myItem = items[i];
                var id = myItem.id.substring(4, items[i].id.length);

                $(myItem).animate({ "top": topPos + orderDict[id] * 70 }, 200);
            }
        }
    });
}

function popFromQueue() {

    var queue = $("#queueContainer").children();
    if (queue.length > 0) {
        for (i = 0; i < queue.length; i++) {
            if (queue[i].style.top == topPos + "px") {
                $(queue[i]).fadeOut(500, function () {
                    animateQueueMove();
                    $(this).remove();
                    updateOrder();
                    checkQueueEmpty(false);
                    setQueueDuration();
                });
                break;
            }
        }
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

function setQueueDuration() {

    if ($("#queueContainer").children().length != 0) {
        $.ajax({
            url: "/Queue/?Handler=Duration",
            type: "GET",
            success: function (duration) {
                document.getElementById("queueTime").textContent = duration;
            }
        });
    }
    else
        document.getElementById("queueTime").textContent = "";
}

function animateQueueMove() {

    $(queueContainer).animate({ "height": curHeight - 70 }, 200);
    curHeight -= 70;
}

