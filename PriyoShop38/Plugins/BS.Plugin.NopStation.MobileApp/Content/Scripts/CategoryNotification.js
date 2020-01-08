
function AddCategoryOrProductInScheduleNotification(scheduleId,catOrProdId) {
    var postData = {
        scheduleId:scheduleId,
        catOrProdId: catOrProdId
    };

    $.ajax({
        cache: false,
        type: "POST",
        url: "/BsNotificationAdmin/SetCatOrProdId",
        data: postData,
        success: function (data) {
            //var grid = $("#categorylist-grid");
            //grid.data('kendoGrid').dataSource.read();
            window.close();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert('Can not add category');
        }
    });

}

