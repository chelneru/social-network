$(document).ready(function() {
    $('.search-input').on('keyup', function() {
        if ($(this).val().length > 2 && $(this).val().length <17) {
            var token = $('input[name="__RequestVerificationToken"]').val();
            var search_query = $(this).val();

            $.ajax({
                url: '/search',
                method: 'POST',
                data: {
                    __RequestVerificationToken: token,
                    search_query: search_query
                },
                dataType: 'json'
            }).done(function(data, textStatus, jqXHR) {
                $('.results-container').css('display', 'block');
                $('.results-container table tr').remove();
                for (var upIter = 0; upIter < data.UsersProfiles.length; upIter++) {
                    $('.results-table tbody').append('<tr><td><a class="profile-result" href="' + data.UsersProfiles[upIter].Link + '">' + data.UsersProfiles[upIter].Content + '</a></td></tr>');
                }

                for (var postIter = 0; postIter < data.Posts.length; postIter++) {
                    var start_content_string = null,
                        end_content_string = null,
                        content = null;
                    start_content_string = data.Posts[postIter].Content.indexOf(search_query) - 10;
                    if (start_content_string < 0) {
                        start_content_string = 0;
                    }
                    
                    end_content_string = data.Posts[postIter].Content.indexOf(search_query) + search_query.length + 10;
                    if (end_content_string >= data.Posts[postIter].Content.length) {
                        end_content_string = data.Posts[postIter].Content.length - 1;
                    }
                    content = data.Posts[postIter].Content.substr(start_content_string, end_content_string - start_content_string);
                    if (start_content_string > 3) {
                        content = '...' + content;
                    }
                    if (end_content_string < data.Posts[postIter].Content.length - 4) {
                        content = content + '...';
                    }
                    $('.results-table tbody').append('<tr><td><a class="profile-result" href="' + data.Posts[postIter].Link + '">' +
                        content.substr(0, content.indexOf(search_query)) + '<span>' + search_query + '</span>' + content.substr(content.indexOf(search_query)
                            + search_query.length, content.length - (content.indexOf(search_query)
                            + search_query.length)) + '</a></td></tr>');
                }
                // because dataType is json 'data' is guaranteed to be an object
                console.log('done', data);

            }).fail(function(jqXHR, textStatus, errorThrown) {
                // the response is not guaranteed to be json
                if (jqXHR.responseJSON) {
                    // jqXHR.reseponseJSON is an object
                    console.log('failed with json data', data);
                }
                else {
                    // jqXHR.responseText is not JSON data
                    console.log('failed with unknown data', data);
                }
            }).always(function(dataOrjqXHR, textStatus, jqXHRorErrorThrown) {
                console.log('always');
            });
        }
    });
    $('')
});
