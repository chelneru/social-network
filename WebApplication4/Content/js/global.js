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

        var container = $(".notifications-list");

        if (!container.is(e.target) && container.has(e.target).length === 0) {
            container.css('display', 'none');

        }
    });
    $('textarea').on('input',function () {
        var self = $(this);
        console.log('activated');
        setTimeout(function () {
            var text = $(self).val();

            if (isUrlValid(text)) {
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

   
    
    
    
});

function isUrlValid(userInput) {
    var res = userInput.match(/(http(s)?:\/\/.)?(www\.)?[-a-zA-Z0-9@:%._\+~#=]{2,256}\.[a-z]{2,6}\b([-a-zA-Z0-9@:%_\+.~#?&//=]*)/g);
    if(res == null)
        return false;
    else
        return true;
}