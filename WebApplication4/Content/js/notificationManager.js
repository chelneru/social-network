var watcher = $.connection.NotificationWatcher; // the generated client-side hub proxy


$(document).ready(function () {
    $.connection.hub.logging = true;
    watcher.client.pushNotification = function (notification) {
        console.log(notification);

        if (notification.isRequest == false) {
            //we have a normal notification
            $row = $(notifTemplate);
            $($row).attr('href', notification.Link);
            $($row).attr('id', notification.Id);
            $($row).find('div').text(notification.NotificationTitle);

            $notificationList.append($row);
            var $notificationCount = $('#logoutForm > ul > li.nav-item.notifications-nav > span');

            $($notificationCount).text(parseInt($($notificationCount).text()) + 1);
            if ($($notificationCount).css('display') === 'none') {
                $($notificationCount).css('display', 'inline-block');
            }
        }
        else {
            //we have a request (friend request)
            $row = $(requestTemplate);
            $($row).attr('href', notification.Link);
            $($row).attr('id', notification.Id);
            $($row).find('div').text(notification.NotificationTitle);

            $requestList.append($row);
            var $requestCount = $('#logoutForm > ul > li.nav-item.requests-nav > span');

            $($requestCount).text(parseInt($($requestCount).text()) + 1);
            if ($($requestCount).css('display') === 'none') {
                $($requestCount).css('display', 'inline-block');
            }

        }
    };
 
    var $notificationList = $('.notifications-list');
    var $requestList = $('.requests-list');
    var notifTemplate = '<a href=""><div class="notification-item"></div></a>';
    var requestTemplate = '<a href=""><div class="request-item"></div></a><div class="request-answers"><span class="accept-request">Yes</span>|<span class="deny-request">No</span></div>';

  

    function init(userId) {
      
    }

    //Add client-side hub methods that the server will call
     $.extend(watcher.client, {
         addNotification: function (notification) {
             $row = $(notifTemplate.supplant(notification)),
                 $notificationList.append($row);
         }
    });
     $.connection.hub.start().then(init);


     $('html').click(function (e) {
         var $container = $(".notifications-container");
             if (!$container.is(e.target)) {
              
                     $container.css('display', 'none');
                     var userProfileId = $('#hdnCurrentUserProfileID').val();
                     checkForRemainingNotifications(userProfileId);
             }
         
     });

     $('.notifications-nav').on('click', function (e) {
         e.stopPropagation();
         if ($('.notifications-nav .notifications-container').css('display') === 'none') {
             $('.notifications-nav .notifications-container').css('display', 'block');

             var notifIds = [];
             $('.notifications-list a').each(function () {
                 notifIds.push($(this).attr('id'));
             });
             markNotificationsAsSeen(notifIds);
         }
         else {
             $('.notifications-nav .notifications-container').css('display', 'none');
             var userProfileId = $('#hdnCurrentUserProfileID').val();
             checkForRemainingNotifications(userProfileId);
         }
     });
     
     $('.requests-list .accept-request').on('click',function () {
        var answer = 1;
        var notificationId = $(this).parent().parent().find('a').attr('id');
     });
     
     $('.requests-nav').on('click', function (e) {
         e.stopPropagation();
         if ($('.requests-nav .notifications-container').css('display') === 'none') {
             $('.requests-nav .notifications-container').css('display', 'block');

           
         }
         else {
             $('.requests-nav .requests-container').css('display', 'none');
             var userProfileId = $('#hdnCurrentUserProfileID').val();
             checkForRemainingNotifications(userProfileId);
         }
     });

     function checkForRemainingNotifications(userProfileId) {
         watcher.server.getAllNotifications(userProfileId).done(function (data) {
             console.log('checking for remaining notifications');
             $('.notifications-list > a').remove();
             for (var notifIter = 0; notifIter < data.Notifications.lenght; notifIter++) {
                 var $row = $(notifTemplate);
                 $($row).attr('href', data.Notifications[notifIter].Link);
                 $($row).attr('id', data.Notifications[notifIter].Id);
                 $($row).find('div').text(data.Notifications[notifIter].NotificationTitle);

                 $notificationList.append($row);
             }
             if (data.NotificationCount > 0) {
                 $('.notifications-nav').find('span.badge').text(data.NotificationCount);
             }
             else {
                 $('.notifications-nav').find('span.badge').text(data.NotificationCount);

                 $('.notifications-nav').find('span.badge').css('display', 'none');
             }
         });
     }
     
     function markNotificationsAsSeen(notificationsIds) {
         watcher.server.markSeenNotifications(notificationsIds).done(function (data) {
             console.log('marked all notifications as seen',data);
         });
     }
         function respondToFriendRequest(initiatorProfileId,answer,notificationId) {

             var token = $('input[name="__RequestVerificationToken"]').val();
             $.ajax({
                 url: '/friend-request',
                 method: 'POST',
                 data: {
                     __RequestVerificationToken: token,
                     initiatorProfileId: initiatorProfileId,
                     notificationId: notificationId,
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
