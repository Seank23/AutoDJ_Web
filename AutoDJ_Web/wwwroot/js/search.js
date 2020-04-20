function updateResult(next) {

    $.ajax({
        url: "/Search/?Handler=UpdateResult",
        type: "GET",
        data: { next: next },
        success: function (result) {
            console.log(result);
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
        },
        error(error) {
            console.log(error);
        }
    });
}

function search() {

    document.getElementById("cancel").disabled = false;
    var term = document.getElementById("searchText").value;

    $("#roller").show();
    document.getElementById("searchBtn").disabled = true;
    $.ajax({
        url: "/Search/?Handler=Search",
        type: "GET",
        data: { searchTerm: term },
        success: function (result) {
            if (result == 1) {
                $("#details").show();
                updateResult(true);
            }
            else if(result == 0)
                $("#noResults").show();
            else
                $("#error").show();

            $("#roller").hide();
        }
    });
}

function cancelSearch() {

    $.ajax({
        url: "/Search/?Handler=Cancel",
        type: "GET",
        success: function () {
            $("#details").hide();
            $("#noResults").hide();
            $("#error").hide();
            $("#roller").hide();
            document.getElementById("searchBtn").disabled = false;
            document.getElementById("cancel").disabled = true;
            document.getElementById("searchText").value = "";
        }
    });
}