function addToQueue() {

    $.ajax({
        url: "/Search/?Handler=ResultId",
        type: "GET",
        success: function (id) {

            $.ajax({
                url: "/Queue/?Handler=Add",
                type: "GET",
                data: { id: id[0] },
                success: function (queue) {
                    console.log(queue);
                    cancelSearch();
                }
            });
        }
    });
}