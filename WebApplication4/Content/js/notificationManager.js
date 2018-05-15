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
            $($row).find('a').attr('href', notification.Link);
            $($row).find('a').attr('id', notification.Id);
            $($row).find('a > div').text(notification.NotificationTitle);

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
    var requestTemplate = '<div class="notification-item">\n' +
        '                                <a id="" href="">\n' +
        '                                    <div></div>\n' +
        '                                 </a><div class="request-answers"><div class="accept-request">Yes</div>|<div class="deny-request">No</div></div>\n' +
        '                             </div>';


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
        var $notifContainer = $(".notifications-container");
        if (!$notifContainer.is(e.target)) {
            if ($notifContainer.css('display') !== 'none') {
                $notifContainer.css('display', 'none');
                var userProfileId = $('#hdnCurrentUserProfileID').val();
                checkForRemainingNotifications(userProfileId);
            }
            
        }
        var $reqContainer = $(".requests-container");
        if (!$reqContainer.is(e.target)) {
            if ($reqContainer.css('display') !== 'none') {
                $reqContainer.css('display', 'none');
            }
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

    $(document).on('click', '.requests-list .deny-request', function () {
        var answer = 2;
        var notificationId = $(this).parent().parent().find('a').attr('id');
        respondToFriendRequest(null, answer, notificationId);

    });

    $(document).on('click', '.requests-list .accept-request', function () {
        var answer = 1;
        var notificationId = $(this).parent().parent().find('a').attr('id');
        respondToFriendRequest(null, answer, notificationId);
    });

    $('.requests-nav').on('click', function (e) {
        if (e.target.className !== 'accept-request' && e.target.className !== 'deny-request') {
            e.stopPropagation();


            if ($('.requests-nav .requests-container').css('display') === 'none') {
                $('.requests-nav .requests-container').css('display', 'block');


            }
            else {
                $('.requests-nav .requests-container').css('display', 'none');

            }
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
            console.log('marked all notifications as seen', data);
        });
    }

    function respondToFriendRequest(initiatorProfileId, answer, notificationId) {

        var token = $('input[name="__RequestVerificationToken"]').val();
        $.ajax({
            url: '/friend-request',
            method: 'POST',
            data: {
                __RequestVerificationToken: token,
                initiatorProfileId: initiatorProfileId,
                notificationId: notificationId,
                response: answer
            },
            dataType: 'json'
        }).done(function (data, textStatus, jqXHR) {
            // because dataType is json 'data' is guaranteed to be an object
            console.log('done', data);
            if (notificationId !== undefined || notificationId !== null) {
                $('.requests-list a[id=' + notificationId + ']').parent().remove();
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
