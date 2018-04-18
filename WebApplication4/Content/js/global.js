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