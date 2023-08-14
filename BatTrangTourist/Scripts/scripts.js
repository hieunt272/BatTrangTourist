AOS.init({
    once: true,
});

function homeJs() {
    $(".banner").slick({
        dots: false,
        infinite: true,
        slidesToShow: 1,
        slidesToScroll: 1,
        speed: 1500,
        autoplay: true,
        autoplaySpeed: 3000,
        arrows: true,
        prevArrow: '<button class="chevron-prev"><i class="far fa-chevron-left"></i></button>',
        nextArrow: '<button class="chevron-next"><i class="far fa-chevron-right"></i></button>',
    });

    $(".tour-list").slick({
        dots: false,
        infinite: true,
        slidesToShow: 4,
        slidesToScroll: 1,
        speed: 1000,
        autoplay: false,
        autoplaySpeed: 3000,
        prevArrow: '<button class="chevron-prev"><i class="far fa-chevron-left"></i></button>',
        nextArrow: '<button class="chevron-next"><i class="far fa-chevron-right"></i></button>',
        responsive: [
            {
                breakpoint: 992,
                settings: {
                    slidesToShow: 2,
                }
            },
            {
                breakpoint: 600,
                settings: {
                    slidesToShow: 1,
                }
            }
        ]
    });

    $(".feedback-list").slick({
        dots: true,
        infinite: true,
        slidesToShow: 3,
        slidesToScroll: 1,
        speed: 1000,
        autoplay: false,
        autoplaySpeed: 3000,
        arrows: false,
        responsive: [
            {
                breakpoint: 992,
                settings: {
                    slidesToShow: 2,
                }
            },
            {
                breakpoint: 600,
                settings: {
                    slidesToShow: 1,
                }
            }
        ]
    });

    $(".offer-list").slick({
        dots: false,
        infinite: true,
        speed: 1000,
        slidesToShow: 2,
        slidesToScroll: 1,
        arrows: false,
        centerMode: false,
        variableWidth: true,
    });
}

function tourDetail() {
    var highlight = $(".highlights").height();
    if (highlight === 150) {
        $(".toggle-view").addClass('active');
    }

    $(".nice-number").niceNumber();

    $(".view-text").click(function () {
        $(".highlights").toggleClass('show');
        $(".toggle-view").toggleClass('active');
    });

    $("#vote").rating({
        "value": 5,
        "click": function (e) {
            $("#starsInput").val(e.stars);
        },
        "color": "#ffa628",
    });

    $(function () {
        $("#ListImages").sortable();
        $("#ListImages").disableSelection();
    });

    var i = 1;
    $("#fileupload").fileupload({
        add: function (e, data) {
            var uploadErrors = [];
            var acceptFileTypes = /^image\/(gif|jpe?g|png)$/i;
            if (data.originalFiles[0]["type"].length && !acceptFileTypes.test(data.originalFiles[0]["type"])) {
                uploadErrors.push("Chỉ chấp nhận định dạng jpeg, jpg, png, gif");
            }
            if (data.originalFiles[0]["size"] > 4000000) {
                uploadErrors.push("Dung lượng ảnh lớn hơn 4MB");
            }
            if (data.originalFiles.length > 6) {
                uploadErrors.push("Chỉ đăng tối đa 6 ảnh");
            }
            if (uploadErrors.length > 0) {
                alert(uploadErrors.join("\n"));
                return false;
            } else {
                data.submit();
            }
            return true;
        },
        url: "/Uploader/Upload?folder=reviews&r=" + Math.random(),
        dataType: "json",
        done: function (e, data) {
            $.each(data.result.files, function (index, file) {
                $('#ListImages').append('<li class="thumb-img-box"><input type="hidden" name="Pictures" value ="' + file.name + '" /><img src="/images/reviews/' + file.name + '?w=80&h=80"/><a href="javascript:;" id="' + i + '" onclick="delfile(' + i + ')"><img src="/content/admin/icon-delete.png" alt="" style="vertical-align:middle" /></a></li>');
            });
            i = i + 1;
            $("#progress").fadeOut(2000);
        },
        start: function () {
            $("#progress .progress-bar").css("width", "0");
            $("#progress").show();
        },
        progressall: function (e, data) {
            var progress = parseInt(data.loaded / data.total * 100, 10);
            $("#progress .progress-bar").css("width", progress + "%");
        }
    }).prop('disabled', !$.support.fileInput).parent().addClass($.support.fileInput ? undefined : 'disabled');

    $(document).on("click", "#contentReview a[href]", function () {
        $.ajax({
            url: $(this).attr("href"),
            type: "GET",
            cache: false,
            success: function (result) {
                $("#list-review").html(result);
            }
        });
        return false;
    });

    $(".review-form").on("submit", function (e) {
        e.preventDefault();
        if ($(this).valid()) {
            $.post("/Home/ReviewForm", $(this).serialize(), function (data) {
                if (data.status) {
                    $.toast({
                        heading: "Gửi đánh giá thành công",
                        text: data.msg,
                        icon: "success",
                        position: "bottom-right"
                    });
                    $(".review-form").trigger("reset");
                } else {
                    $.toast({
                        heading: "Gửi đánh giá không thành công",
                        text: data.msg,
                        icon: "error",
                        position: "bottom-right"
                    });
                }
            });
        }
    });

    var startElm = $('#package_option').position().top;
    var endElm = startElm + $('#package_option').height();
    $(window).scroll(function () {
        var endPriceBox = $(".right-price").position().top + $(".right-price").height();

        if (startElm < endPriceBox && endPriceBox < endElm) {
            $(".right-price").addClass('hide');
        }
        if (startElm > endPriceBox || endPriceBox > endElm) {
            $(".right-price").removeClass('hide');
        }
    });

    $(".entry-date").html(function (index, value) {
        return moment(value, "DDMMYYYYhhmm").lang("vi").fromNow();
    });

    $(".ticket-type").click(function () {
        $(this).addClass('active');
        $(this).siblings().removeClass('active');
        var id = $(this).attr("data-id");
        $("#serviceId").val(id);

        $("body").append("<div class='loading'><i class='fa-regular fa-spinner fa-spin'></i></div>");
        $.get("/Home/GetService", { serviceId: id }, function (data) {
            $("#js-service").empty();
            $("#js-service").html(data);
        }).then(function () {
            $(".loading").remove();
        });

        $.getJSON("/Home/GetServicePrice", { serviceId: id }, function (data) {
            if (data.adultPrice != null) {
                $("#service-price").text(data.adultPrice.toLocaleString('it-IT') + " ₫");
            }
            else if (data.childPrice != null) {
                $("#service-price").text(data.childPrice.toLocaleString('it-IT') + " ₫");
            }

        });
    });

    $(".package-date").click(function () {
        $(".m-start-date").addClass("show");
        $(".action-overlay").addClass("show");
        $(".js-startdate").datepicker({
            dateFormat: "dd/mm/yy",
            minDate: 0
        });
    });

    $(".js-startdate").datepicker({ dateFormat: "dd/mm/yy", minDate: 0 }).on("change", function () {
        var date = $(this).val();
        $(".package-date span").text(date);
        $("#startDate").val(date);

        $(".action-overlay").removeClass("show");
        $(".m-start-date").removeClass("show");
    });

    $(".action-overlay").click(function () {
        $(this).removeClass("show");
        $(this).siblings(".m-start-date").removeClass("show");
    });

    $(".quantity .btn-light").click(function () {
        var quantity = $(this).siblings(".input-number").val();
        var price = $(this).parents(".package-detail").find(".price-hidden").val();
        var itemPrice = quantity * price;
        $(this).parents(".package-detail").find(".total-item").val(itemPrice);

        var items = $(".total-item");
        var totalCart = 0;
        items.each(function () {
            totalCart += parseInt(this.value);
        });
        $(".price-detail .price").text(totalCart.toLocaleString('it-IT') + " ₫");
    });

    $(".clear-all").click(function () {
        $(".price-detail .price").text("- ₫");
    });
}

$(document).ready(function () {
    $(window).scroll(function () {
        if ($(this).scrollTop() > 200) {
            $('.btn-scroll').fadeIn(200);
        } else {
            $('.btn-scroll').fadeOut(200);
        }
    });

    $('.btn-scroll').click(function (event) {
        event.preventDefault();
        $('html, body').animate({ scrollTop: 0 }, 300);
    });

    $('.user').click(function () {
        $('.header-login').slideToggle(200);
        $('.site-overlay').toggleClass('show');
    });

    $('.header-login').click(function (e) {
        e.stopImmediatePropagation();
    });

    $('.site-overlay').click(function () {
        $(this).removeClass('show');
        $('.header-login').slideUp(200);
    });

    $(".btn-search").click(function () {
        $(".body-overlay").addClass('active');
        $(".site-search").addClass('active');
        function delay() {
            $(".site-search .form-control").focus();
        }

        setTimeout(delay, 300);
    });

    $(".body-overlay, .site-search-close").click(function () {
        $(".body-overlay").removeClass('active');
        $(".site-search").removeClass('active');
    });

    $(".hamburger").click(function () {
        $(this).toggleClass("active");
        $(".menu-mb").toggleClass("active");
        $(".overlay").toggleClass("active");
    });

    $(".overlay").click(function () {
        $(this).removeClass("active");
        $(".menu-mb").removeClass("active");
        $(".hamburger").removeClass("active");
    });

    $(".expand-bar").click(function () {
        $(this).toggleClass('open');
        $(this).siblings('.sub-nav-mb').slideToggle();
    });

    $(".related-list").slick({
        dots: false,
        infinite: true,
        slidesToShow: 4,
        slidesToScroll: 1,
        autoplay: false,
        autoplaySpeed: 3000,
        prevArrow: '<button class="chevron-prev"><i class="far fa-chevron-left"></i></button>',
        nextArrow: '<button class="chevron-next"><i class="far fa-chevron-right"></i></button>',
        responsive: [
            {
                breakpoint: 992,
                settings: {
                    slidesToShow: 3,
                    slidesToScroll: 1
                }
            },
            {
                breakpoint: 600,
                settings: {
                    slidesToShow: 1,
                    slidesToScroll: 1
                }
            }
        ]
    });

    setTimeout(alertBox, 1500);
});

function alertBox() {
    $("#AlertBox").fadeOut(1000);
}

$(function () {
    $("#subcribeForm").on("submit", function (e) {
        e.preventDefault();
        if ($("#subcribeForm").valid()) {
            $.post("/Home/SubcribeForm", $(this).serialize(), function (data) {
                if (data.status) {
                    $.toast({
                        heading: "Gửi đăng ký tư vấn thành công",
                        text: data.msg,
                        icon: "success",
                        position: "bottom-right"
                    });
                    $("#subcribeForm").trigger("reset");
                } else {
                    $.toast({
                        heading: "Đăng ký tư vấn không thành công",
                        text: data.msg,
                        icon: "error",
                        position: "bottom-right"
                    });
                }
            });
        }
    });

    $("#subcribeForm2").on("submit", function (e) {
        e.preventDefault();
        if ($("#subcribeForm2").valid()) {
            $.post("/Home/SubcribeForm", $(this).serialize(), function (data) {
                if (data.status) {
                    $.toast({
                        heading: "Gửi đăng ký tư vấn thành công",
                        text: data.msg,
                        icon: "success",
                        position: "bottom-right"
                    });
                    $("#subcribeForm2").trigger("reset");
                } else {
                    $.toast({
                        heading: "Đăng ký tư vấn không thành công",
                        text: data.msg,
                        icon: "error",
                        position: "bottom-right"
                    });
                }
            });
        }
    });

    $(".contact-form").on("submit", function (e) {
        e.preventDefault();
        if ($(".contact-form").valid()) {
            $.post("/Home/ContactForm", $(this).serialize(), function (data) {
                if (data.status) {
                    $.toast({
                        heading: "Gửi liên hệ thành công",
                        text: data.msg,
                        icon: "success",
                        position: "bottom-right"
                    });
                    $(".contact-form").trigger("reset");
                } else {
                    $.toast({
                        heading: "Liên hệ không thành công",
                        text: data.msg,
                        icon: "error",
                        position: "bottom-right"
                    });
                }
            });
        }
    });
});

function Sort(action) {
    $(document).on("click", "[data-filter]", function () {
        const catId = $(this).val();
        const price = $(".filter-price").val();
        let url = $("input[name=currentUrl]").val();
        const keywords = $("input[name=keywords][type=hidden]").val();
        const title = $(".breadcrumb-item.active").text();

        url = url.split("/").at(-1);

        window.history.pushState(title, "", url);

        $("body").append("<div class='loading'><i class='fa-regular fa-spinner fa-spin'></i></div>");
        $.get(action, { keywords: keywords, url, catId: catId, price: price }, function (data) {
            $("#list-item-sort").empty();
            $("#list-item-sort").html(data);
        }).then(function () {
            $(".loading").remove();
        });
    });

    $(".filter-price").change(function () {
        const catId = $("input[name=brandId]").val();
        const price = $(this).val();
        let url = $("input[name=currentUrl]").val();
        const keywords = $("input[name=keywords][type=hidden]").val();
        const title = $(".breadcrumb-item.active").text();

        url = url.split("/").at(-1);

        window.history.pushState(title, "", url);

        $("body").append("<div class='loading'><i class='fa-regular fa-spinner fa-spin'></i></div>");
        $.get(action, { keywords: keywords, url, catId: catId, price: price }, function (data) {
            $("#list-item-sort").empty();
            $("#list-item-sort").html(data);
        }).then(function () {
            $(".loading").remove();
        });
    });

    $("#sort").change(function () {
        const sort = $(this).val();
        let url = $("input[name=currentUrl]").val();
        const keywords = $("input[name=keywords][type=hidden]").val();
        const title = $(".breadcrumb-item.active").text();

        url = url.split("/").at(-1);

        window.history.pushState(title, "", url);

        $("body").append("<div class='loading'><i class='fa-regular fa-spinner fa-spin'></i></div>");
        $.get(action, { keywords: keywords, url, sort: sort }, function (data) {
            $("#list-item-sort").empty();
            $("#list-item-sort").html(data);
        }).then(function () {
            $(".loading").remove();
        });
    });
};

function userOrder(action) {
    $(document).on('click', '.nav-link', function () {
        let status = parseInt($('.nav-link.active').val());
        $("body").append("<div class='loading'><i class='fa-regular fa-spinner fa-spin'></i></div>");
        $.get(action, { status: status }, function (data) {
            $('#list-item-sort').empty();
            $('#list-item-sort').html(data);
        }).then(function () {
            $(".loading").remove();
        });
    });
}