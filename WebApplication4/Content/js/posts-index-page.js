
$(document).ready(function () {
    console.log('loaded');
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
                $($self).find('img').css('opacity', '1');
                $($self).parent().find('.downvote img').css('opacity', '0.5');
                var votes = parseInt($($self).parent().find('.votes:first').text());
                $($self).parent().find('.votes:first').text(votes + 1);
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
                $($self).find('img').css('opacity', '1');
                $($self).parent().find('.upvote img').css('opacity', '0.5');

                var votes = parseInt($($self).parent().find('.votes:first').text());
                $($self).parent().find('.votes:first').text(votes - 1);
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
    // $('input.submit-post').on('click',function () {
    //     var token = $('input[name="__RequestVerificationToken"]').val();
    //     var content = $('textarea.post-content').text();
    //     var $self = $(this);
    //     $.ajax({
    //         url: '/vote-post',
    //         method: 'POST',
    //         data: {
    //             __RequestVerificationToken: token,
    //             content: content
    //         },
    //         dataType: 'json'
    //     }).done(function (data, textStatus, jqXHR) {
    //         console.log('done', data);
    //         if (data.Message.toString().toLowerCase().trim() === "vote_registered") {
    //             $($self).find('img').css('opacity', '1');
    //             $($self).parent().find('.upvote img').css('opacity', '0.5');
    //
    //             var votes = parseInt($($self).parent().find('.votes:first').text());
    //             $($self).parent().find('.votes:first').text(votes - 1);
    //         }
    //
    //     }).fail(function (data,jqXHR, textStatus, errorThrown) {
    //         // the response is not guaranteed to be json
    //         if (jqXHR.responseJSON) {
    //             // jqXHR.reseponseJSON is an object
    //             console.log('failed with json data', data);
    //         }
    //         else {
    //             // jqXHR.responseText is not JSON data
    //             console.log('failed with unknown data', data);
    //         }
    //     });
    // });
});