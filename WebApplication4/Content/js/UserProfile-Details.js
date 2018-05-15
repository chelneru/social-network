$(document).ready(function () {
    console.log("file loaded");

    var friend_request_button_text = $('.friends-button').text();
    console.log("file loaded");
    //File Upload response from the server
    if ($('.dropzone').length > 0) {
        Dropzone.forElement(".dropzone").options.autoProcessQueue = false;
        Dropzone.forElement(".dropzone").options.acceptedFiles = "image/*,.mp4";
        Dropzone.forElement(".dropzone").options.paramName = "file";
        Dropzone.forElement(".dropzone").options.resizeWidth = "500";
        Dropzone.forElement(".dropzone").options.resizeHeight = "500";
        Dropzone.forElement(".dropzone").options.resizeMimeType = "image/jpeg";
        //Dropzone.forElement(".dropzone").options.addRemoveLinks = true;
        Dropzone.forElement(".dropzone").options.maxFilesize = 10;
    }

    $('.friends-button').on("click", function () {
        $(".friends-button").css("pointer-events", "none");

        var $friendButton = $(this);
        var user_id = document.querySelector('meta[name="id"]').content;
        var token = $('input[name="__RequestVerificationToken"]').val();

        $.ajax({
            url: '/add-remove-friend',
            method: 'POST',
            data: {
                __RequestVerificationToken: token,
                user_id: user_id
            },
            dataType: 'json'
        }).done(function (data, textStatus, jqXHR) {
            // because dataType is json 'data' is guaranteed to be an object
            console.log('done', data);
            if (data.Message.toString().toLowerCase().trim() === "friend request sent") {
                $($friendButton).text("Friend request sent");
            }
            else if (data.Message.toString().toLowerCase().trim() === "friend removed") {
                $($friendButton).text("Add friend");
            }
            else if (data.Message.toString().toLowerCase().trim() === "friend request canceled") {
                $($friendButton).text("Add friend");
            }
            
            $(".friends-button").css("pointer-events", "auto");
        }).fail(function (jqXHR, textStatus, errorThrown) {
            // the response is not guaranteed to be json
            if (jqXHR.responseJSON) {
                // jqXHR.reseponseJSON is an object
                console.log('failed with json data', data);
            }
            else {
                // jqXHR.responseText is not JSON data
                console.log('failed with unknown data', data);
            }
        }).always(function (dataOrjqXHR, textStatus, jqXHRorErrorThrown) {
            console.log('always');
        });

    });
    $('.accept-friend-request').on('click',function () {
        var initiatorProfileId = $('.friend-request-dialog').attr('init-profile-id');
        var answer = 1;
        respondToFriendRequest(initiatorProfileId,answer);
    });

    
    $('.deny-friend-request').on('click',function () {
        var initiatorProfileId = $('.friend-request-dialog').attr('init-profile-id');
        var answer = 2;
        respondToFriendRequest(initiatorProfileId,answer);
    });
    
    $('.friends-button').mouseenter(function () {
        if ($(this).text().toString().toLowerCase().trim() === "friend request sent") {
            $(this).text('Cancel friend request');
        } 
        else if ($(this).text().toString().toLowerCase().trim() === "friend") {
            $(this).text('Unfriend');
        }
    });
    
    $('.friends-button').mouseleave(function () {
        if ($(this).text().toString().toLowerCase().trim() === "cancel friend request") { 
            $(this).text(friend_request_button_text);
        }
        else if ($(this).text().toString().toLowerCase().trim() === "unfriend") {
            $(this).text(friend_request_button_text);
        }
        
    });
    $("#submit-all").on("click", function () {
        Dropzone.forElement(".dropzone").processQueue();
    });
    console.log("file loaded");
    if ($('.dropzone').length > 0) {

        Dropzone.forElement(".dropzone").on("addedfile", function (file) {
            var removeDiv = Dropzone.createElement("<div class='remove-div'><span>Remove image</span><img class='icon' src='../../Content/removebutton.svg' alt='Click me to remove the file.' data-dz-remove/></div>");
            var descInputDiv = Dropzone.createElement("<div class='description-div'><textarea type='text' name='image-description' rows='3' placeholder='Put a description'></textarea></textarea></div>");
            
            var _this = this;

            // Listen to the click event
            removeDiv.addEventListener("click", function (e) {
                // Make sure the button click doesn't submit the form:
                e.preventDefault();
                e.stopPropagation();

                // Remove the file preview.
                _this.removeFile(file);
                // If you want to the delete the file on the server as well,
                // you can do the AJAX request here.
            });

            // Add the button to the file preview element.
            file.previewElement.appendChild(descInputDiv);
            file.previewElement.appendChild(removeDiv);

        });
    }
    
    function respondToFriendRequest(initiatorProfileId,answer) {
        
        var token = $('input[name="__RequestVerificationToken"]').val();
        $.ajax({
            url: '/friend-request',
            method: 'POST',
            data: {
                __RequestVerificationToken: token,
                initiatorProfileId: initiatorProfileId,
                response :answer
            },
            dataType: 'json'
        }).done(function (data, textStatus, jqXHR) {
            // because dataType is json 'data' is guaranteed to be an object
            console.log('done', data);
            if (answer === 1) { 
            if (data.Message.toString().toLowerCase().trim() === "friend added") {
                $('.friend-request-dialog').css("display",'none');
            }

            $(".friends-button").text("Friend");
            $(".friends-button").removeClass("avoid-clicks");
            }
            else if (asnwer === 2) {
                if (data.Message.toString().toLowerCase().trim() === "friend request denied") {
                    $('.friend-request-dialog').css("display",'none');
                }

                $(".friends-button").text("Add friend");
                $(".friends-button").removeClass("avoid-clicks");
            } 

        }).fail(function (jqXHR, textStatus, errorThrown) {
            // the response is not guaranteed to be json
            if (jqXHR.responseJSON) {
                // jqXHR.reseponseJSON is an object
                console.log('failed with json data', data);
            }
            else {
                // jqXHR.responseText is not JSON data
                console.log('failed with unknown data', data);
            }
        }).always(function (dataOrjqXHR, textStatus, jqXHRorErrorThrown) {
            console.log('always');
        });
    }
    
    });

$("#upload-avatar-form .file-input").on("change", function () {
    var mimeType = $(this)[0].files[0]["type"];
    if (mimeType.split("/")[0] === "image") {

        $("#upload-avatar-form").submit();
    }
});
$("#upload-avatar-form").submit(function () {
});
