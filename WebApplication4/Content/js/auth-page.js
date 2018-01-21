$('.login-tab').on('click', function () {
    $('.login-form').css('display', 'block');
    $('.register-form').css('display', 'none');
    $(this).addClass('active-tab');
    $('.register-tab').removeClass('active-tab');
});

$('.register-tab').on('click', function () {
    
    $('.login-form').css('display', 'none');
    $('.register-form').css('display', 'block');
    $(this).addClass('active-tab');
    $('.login-tab').removeClass('active-tab');
});