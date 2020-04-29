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

                    var queueContainer = document.getElementById("queueContainer");
                    var itemContainer = document.createElement("div");
                    var itemId = "item" + queueItem['id'];
                    itemContainer.id = itemId
                    itemContainer.style = "max-height: 70px; order: " + queueContainer.children.length + ";";
                    queueContainer.appendChild(itemContainer);
                    $("#" + itemId).load("/QueueItemTemplate");
                    $("#queueContainer").show();

                    cancelSearch();
                    setQueueDuration();
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

            $("#item" + id).remove();
            setQueueDuration();
            if (!checkQueueEmpty())
                updateOrder();
            
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
                var id = items[i].id.substring(4, items[i].id.length);
                items[i].style.order = orderDict[id];
            }
        }
    });
}

function popFromQueue() {

    var queue = $("#queueContainer").children();
    if (queue.length > 0) {
        for (i = 0; i < queue.length; i++) {
            if (queue[i].style.order == 0) {
                queue[i].remove();
                updateOrder();
                break;
            }
        }
    }
}

function checkQueueEmpty() {

    if ($("#queueContainer").children().length == 0) {
        $("#queueEmpty").show();
        if (document.getElementById('player').tagName == "DIV") {
            document.getElementById('playButton').disabled = true;
            $(".button.play").toggleClass('paused');
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

