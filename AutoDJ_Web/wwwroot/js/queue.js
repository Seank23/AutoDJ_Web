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

                    var queueContainer = document.getElementById("queueContainer");
                    var itemContainer = document.createElement("div");
                    var itemId = "item" + queueItem['id'];
                    itemContainer.id = itemId
                    itemContainer.style = "max-height: 70px; order: " + queueContainer.children.length + ";";
                    queueContainer.appendChild(itemContainer);
                    $("#" + itemId).load("/QueueItemTemplate");
                    $("#queueContainer").show();

                    cancelSearch();
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

            if ($("#queueContainer").children().length == 0)
                $("#queueEmpty").show();
            else
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

