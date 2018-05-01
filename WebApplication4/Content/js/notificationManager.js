var watcher = $.connection.LikeWatcher; // the generated client-side hub proxy


$(document).ready(function () {
    $.connection.hub.logging = true;
    watcher.client.pushNotification = function (notification) {
        console.log(notification);
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
    };
 
    var $notificationList = $('.notifications-list');
    var notifTemplate = '<a href=""><div class="notification-item"></div></a>';

  

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
});
