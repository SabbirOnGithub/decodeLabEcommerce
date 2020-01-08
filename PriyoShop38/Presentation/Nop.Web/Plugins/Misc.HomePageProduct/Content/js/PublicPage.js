/*function setPictureSize() {
    $(".productContainer .product-item > .picture").css('height', $(".spc-body .product-item > .picture").innerWidth());
}

$(document).ready(function () {
    setPictureSize();
    if ($(window).width() < 768) {
        $(".divTitle").height("auto");
    }
    else {
        var productContainerHeight = ($(".spc-body").height() - 1);
        $(".divTitle").height(productContainerHeight);
    }

    $(".imgheightofcategory").height(productContainerHeight);

    $(".productContainer").load(function () {
        var productContainerHeight = ($(".spc-body").height() - 1);
        $(".divTitle").height(productContainerHeight);
        $(".imgheightofcategory").height(productContainerHeight);
    });

    $(window).resize(function () {
        if ($(window).width() < 768) {
            $(".divTitle").height("auto");
        }
        else {
            var productContainerHeight = ($(".spc-body").height() - 1);
            $(".divTitle").height(productContainerHeight);
        }
        var productContainerHeight = ($(".spc-body").height() - 1);
        $(".imgheightofcategory").height(productContainerHeight);
    });

});


$(window).resize(function () {
    if ($(window).width() < 768) {
        $(".divTitle").height("auto");
    }
    else {
        var productContainerHeight = ($(".productContainer").height() - 1);
        $(".divTitle").height(productContainerHeight);
    }
    setPictureSize();
});*/