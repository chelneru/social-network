Dropzone.autoDiscover = false;

$(document).ready(function () {
    var promise = new RSVP.Promise(function (resolve, reject) { });
    console.log('loaded');
    var dropzoneOptions = {
        dictDefaultMessage: 'Drop Here!',
        paramName: "file",
        maxFilesize: 20, // MB
        addRemoveLinks: true,
        autoProcessQueue: false,
        createImageThumbnails:true,
        init: function () {
            this.on("success", function (file) {
                console.log("success > " + file.name);
            });
            this.on("addedfile", function (file) {
                if (this.files[1]!==undefined) {
                    this.removeFile(this.files[0]);
                }
                // check file extension, see:
                // http://stackoverflow.com/questions/190852/how-can-i-get-file-extensions-with-javascript
                var comps = file.name.split(".");
                if (comps.length === 1 || comps[0] === "" && comps.length === 2) {
                    return;
                }
                var ext = comps.pop().toLowerCase();
                if (ext === 'mov' || ext === 'mpeg' || ext === 'mp4' || ext === 'wmv') {
                    // create a hidden <video> element with video file.
                    FrameGrab.blob_to_video(file).then(
                        function videoRendered(videoEl) {

                            // extract video frame at 1 sec into a 160px image and
                            // set to the <img> element.
                            var frameGrab = new FrameGrab({ video: videoEl });
                            var itemEntry = document.querySelector('div.dz-image > img');
                            frameGrab.grab(itemEntry, 1, 160).then(
                                function frameGrabbed(itemEntry) {
                                    self.emit('thumbnail', file, itemEntry.container.src);
                                },
                                function frameFailedToGrab(reason) {
                                    console.log("Can't grab the video frame from file: " +
                                        file.name + ". Reason: " + reason);
                                }
                            );
                        },
                        function videoFailedToRender(reason) {
                            console.log("Can't convert the file to a video element: " +
                                file.name + ". Reason: " + reason);
                        }
                    );
                }
            });
           
        }
    };
    var uploader = document.querySelector('#uploader');
    var newDropzone = null;

    if (uploader !== null) {
        newDropzone = new Dropzone(uploader, dropzoneOptions);
    } 

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
                    $($preview).find('img').attr('src', data.Image);
                    $($preview).find('.title').text(data.Title);
                    var parser = document.createElement('a');
                    parser.href = data.Url;
                    $($preview).find('.source').text(parser.hostname.toUpperCase());


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
        if ($('textarea.link-input').val() !== '') {

            var link_preview_id = $('.link-upload-panel .preview').attr('lpid');
            if (link_preview_id !== '' && link_preview_id !== undefined) {
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
        }
        if ($(this).hasClass('post-video')) {
            $('.video-upload-panel').css('display', 'block');
            $('#uploader').click();
            
        }

    });

});
