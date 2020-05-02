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

    document.querySelector('.search-body').classList.toggle('expand');
    $("#roller").fadeIn(50);
    document.getElementById("searchBtn").disabled = true;
    $.ajax({
        url: "/Search/?Handler=Search",
        type: "GET",
        data: { searchTerm: term },
        success: function (result) {
            if (result == 1) {
                $("#details").fadeIn(100);
                updateResult(true);
            }
            else if(result == 0)
                $("#noResults").fadeIn(100);
            else
                $("#error").fadeIn(100);

            $("#roller").fadeOut(50);
        }
    });
}

function cancelSearch() {

    $.ajax({
        url: "/Search/?Handler=Cancel",
        type: "GET",
        success: function () {
            $("#details").fadeOut(100);
            $("#noResults").fadeOut(100);
            $("#error").fadeOut(100);
            $("#roller").fadeOut(100);
            document.querySelector('.search-body').classList.toggle('expand');
            document.getElementById("searchBtn").disabled = false;
            document.getElementById("cancel").disabled = true;
            document.getElementById("searchText").value = "";
        }
    });
}