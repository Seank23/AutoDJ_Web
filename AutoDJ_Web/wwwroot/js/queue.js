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
                    
                    var itemContainer = document.createElement("div");
                    var itemId = "item" + queueItem['id'];
                    itemContainer.id = itemId
                    document.getElementById("queueContainer").appendChild(itemContainer);
                    $("#" + itemId).load("/QueueItemTemplate");
                    $("#queueContainer").show();

                    cancelSearch();
                }
            });
        }
    });
}