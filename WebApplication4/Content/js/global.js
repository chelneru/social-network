String.prototype.supplant = function (o) {
    return this.replace(
        /{([^{}]*)}/g,
        function (a, b) {
            var r = o[b];
            return typeof r === 'string' || typeof r === 'number' ? r : a;
        }
    );
};

$(document).ready(function () {
   
    $('body,html').click(function (e) {

        var $container = $(".popup");
        if ($($container).find('textarea').val() === '') {
            if (!$container.is(e.target) && $container.has(e.target).length === 0 && !$(e.target).hasClass('dz-hidden-input')) {
                if ($container.hasClass('popup')) {
                    var $displayed_panel = $($container).find(".panel").filter(function () {
                        return $(this).css('display') === 'block';
                    });
                    if ($displayed_panel !== undefined) {
                        if ($($displayed_panel).find('textarea').val() === '') {
                            $container.css('display', 'none');
                        }
                    }
                    else {
                        $('.popup-overlayer').css('display', 'none');
                        clearLinkPopup();
                    }
                }
                else {
                    $container.css('display', 'none');
                }

            } 
        }
    });

    function clearLinkPopup() {
        $('.link-upload-panel .preview').attr('lpid', '');
        $('.link-upload-panel .preview img').attr('src', '');
        $('.link-upload-panel .preview .title').text('');
        $('.link-upload-panel .preview .source').text('');
        $('textarea.link-input').val('');
    }


});

function getLocation(href) {
    var l = document.createElement("a");
    l.href = href;
    return l;
};

function isUrlValid(userInput) {
    var res = userInput.match(/(http(s)?:\/\/.)?(www\.)?[-a-zA-Z0-9@:%._\+~#=]{2,256}\.[a-z]{2,6}\b([-a-zA-Z0-9@:%_\+.~#?&//=]*)/g);
    return res === null;
}