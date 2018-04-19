
$(document).ready(function () {
    console.log('loaded');
    $("div#upload-video").dropzone({ url: "/file/post", clickable: ['.post-video'] });
    $('.upvote').on('click', function () {
        var token = $('input[name="__RequestVerificationToken"]').val();

        var $self = $(this);
        $.ajax({
            url: '/vote-post',
            method: 'POST', 
            data: {
                __RequestVerificationToken: token,
                post_id: $($self).parent().parent().attr('id'),

                value: 1
            },
            dataType: 'json'
        }).done(function (data, textStatus, jqXHR) {
            console.log('done', data.Message);
            if (data.Message.toString().toLowerCase().trim() === "vote_registered") {
               
                var votes = parseInt($($self).parent().find('.votes:first').text());
                if ($($self).parent().find('.downvote').hasClass('voted')) {
                    $($self).parent().find('.votes:first').text(votes + 2);
                }
                else {
                    $($self).parent().find('.votes:first').text(votes + 1);

                }

                $($self).parent().find('.upvote').addClass('voted');
                $($self).parent().find('.downvote').removeClass('voted');
            }

            }).fail(function (data,jqXHR, textStatus, errorThrown) {
            // the response is not guaranteed to be json
            if (jqXHR.responseJSON) {
                // jqXHR.reseponseJSON is an object
                console.log('failed with json data', data);
            }
            else {
                // jqXHR.responseText is not JSON data
                console.log('failed with unknown data', data);
            }
        });
    });
    $('.downvote').on('click', function () {
        var token = $('input[name="__RequestVerificationToken"]').val();

        var $self = $(this);
        $.ajax({
            url: '/vote-post',
            method: 'POST',
            data: {
                __RequestVerificationToken: token,
                post_id: $($self).parent().parent().attr('id'),
                value: -1
            },
            dataType: 'json'
        }).done(function (data, textStatus, jqXHR) {
            console.log('done', data);
            if (data.Message.toString().toLowerCase().trim() === "vote_registered") {
                var votes = parseInt($($self).parent().find('.votes:first').text());

                if ($($self).parent().find('.upvote').hasClass('voted')) {
                    $($self).parent().find('.votes:first').text(votes - 2);
                }
                else {
                    $($self).parent().find('.votes:first').text(votes - 1);
                }
                $($self).parent().find('.downvote').addClass('voted');
                $($self).parent().find('.upvote').removeClass('voted');

            }

            }).fail(function (data,jqXHR, textStatus, errorThrown) {
            // the response is not guaranteed to be json
            if (jqXHR.responseJSON) {
                // jqXHR.reseponseJSON is an object
                console.log('failed with json data', data);
            }
            else {
                // jqXHR.responseText is not JSON data
                console.log('failed with unknown data', data);
            }
        });
    });
    $('.link-upload-panel textarea').on('input', function () {

        var self = $(this);
        console.log('activated');
        setTimeout(function () {
            var text = $(self).val();

            if (isUrlValid(text)) {
                $('.popup .submit-post').css('display', 'inline-block');

                $('.placeholder-text').text('Fetching preview...');
                var token = $('input[name="__RequestVerificationToken"]').val();

                var $self = $(this);
                $.ajax({
                    url: '/get-preview',
                    method: 'POST',
                    data: {
                        __RequestVerificationToken: token,
                        url: text
                    },
                    dataType: 'json'
                }).done(function (data, textStatus, jqXHR) {
                    $('.placeholder-text').text('');
                    $('.preview-placeholder').css('display', 'none');
                    var $preview =  $('.preview');
                    $($preview).css('display', 'block');
                    $($preview).attr('lpid', data.Id);
                    $($preview.find('img').attr('src', data.Image);
                    $($preview.find('.title').text(data.Title);
                    var parser = document.createElement('a');
                    parser.href = data.Url;
                    $($preview.find('.source').text(parser.hostname.toUpperCase());


                    console.log('done', data);


                }).fail(function (data,jqXHR, textStatus, errorThrown) {
                    // the response is not guaranteed to be json
                    if (jqXHR.responseJSON) {
                        // jqXHR.reseponseJSON is an object
                        console.log('failed with json data', data);
                    }
                    else {
                        // jqXHR.responseText is not JSON data
                        console.log('failed with unknown data', data);
                    }
                });
            }
        }, 100);
    });

    $(".post-content").focus(function () {
        $(".feed-container form .submit-post").css('display', 'block');
        $(".post-additional-options").css("display", "inline-block");
        $(this).animate({
            height: 123
        }, "normal");
    }).blur(function () {
        //$(".submit-post").css('display', 'none');
        //$(".post-additional-options").css("display", "none");
        //$(this).animate({
        //    height: 60
        //}, "normal");
    });

    $('.popup .submit-post').on('click', function () {
        if ($('textarea.link-input').val() != '') {

            var link_preview_id = $('.link-upload-panel .preview').attr('lpid');
            if (link_preview_id != '' && link_preview_id != undefined) {
                var token = $('input[name="__RequestVerificationToken"]').val();

                $.ajax({
                    url: '/post/create',
                    method: 'POST',
                    data: {
                        __RequestVerificationToken: token,
                        lpid: link_preview_id
                    },
                    dataType: 'json'
                }).done(function (data, textStatus, jqXHR) {
                    window.location.reload();

                }).fail(function (data, jqXHR, textStatus, errorThrown) {
                    // the response is not guaranteed to be json
                    if (jqXHR.responseJSON) {
                        // jqXHR.reseponseJSON is an object
                        console.log('failed with json data', data);
                    }
                    else {
                        // jqXHR.responseText is not JSON data
                        window.location.reload();

                    }
                });
            }
        }
    });
    $('.post-video, .post-image, .post-link').on('click', function (e) {
        e.stopPropagation();
        $('.popup-overlayer').css('display', 'block');
        $('.popup').css('display', 'block');

        if ($(this).hasClass('post-link')) {
        $('.link-upload-panel').css('display', 'block');
        } if ($(this).hasClass('post-video')) {
            $('.video-upload-panel').css('display', 'block');
            
        }

    });

});
function clearLinkPopup() {
    $('.link-upload-panel .preview').attr('lpid','');
    $('.link-upload-panel .preview img').attr('src', '');
    $('.link-upload-panel .preview .title').text('');
    $('.link-upload-panel .preview .source').text('');
    $('textarea.link-input').val('');
}