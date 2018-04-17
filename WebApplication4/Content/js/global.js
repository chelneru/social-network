$(document).ready(function () {
    $('.notifications-nav .navbar-symbol').on('click', function (e) {
        e.stopPropagation();
        if ($('.notifications-nav .notifications-list').css('display') == 'none') {
            $('.notifications-nav .notifications-list').css('display', 'block');
        }
        else {
            $('.notifications-nav .notifications-list').css('display', 'none');
        }
    });
    $('body,html').click(function (e) {

        var container = $(".notifications-list, .popup ");

        if (!container.is(e.target) && container.has(e.target).length === 0) {
            if (container.hasClass('popup')) {
                $('.popup-overlayer').css('display', 'none');
                clearLinkPopup();
            }
            container.css('display', 'none');

        }
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
                    $('.preview').css('display', 'block');
                    $('.link-upload-panel .preview').attr('lpid', data.Id);
                    $('.link-upload-panel .preview img').attr('src', data.Image);
                    $('.link-upload-panel .preview .title').text(data.Title);
                    var parser = document.createElement('a');
                    parser.href = data.Url;
                    $('.link-upload-panel .preview .source').text(parser.hostname.toUpperCase());
                 

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

    
    
    
});
function getLocation (href) {
    var l = document.createElement("a");
    l.href = href;
    return l;
};
function isUrlValid(userInput) {
    var res = userInput.match(/(http(s)?:\/\/.)?(www\.)?[-a-zA-Z0-9@:%._\+~#=]{2,256}\.[a-z]{2,6}\b([-a-zA-Z0-9@:%_\+.~#?&//=]*)/g);
    if(res == null)
        return false;
    else
        return true;
}