
//--------------------Session::Name--------------
function RefreshEPC() {
    Swal.fire({
        title: window.parent.confirmVar,
        icon: "warning",
        showDenyButton: true,
        showCancelButton: true,
        confirmButtonText: window.parent.confirmVar,
        denyButtonText: window.parent.closeVar,
        cancelButtonText: window.parent.cancelVar
    }).then((result) => {
        /* Read more about isConfirmed, isDenied below */
        if (result.isConfirmed) {
            $.ajax({
                url: '/FunctionOrder/DeleteEPC',
                type: 'post',
                success: function (data) {
                    if (data.code == 200) {
                        Swal.fire(data.msg, window.parent.pressButtonVar, "success");
                    } else {
                        Swal.fire(data.msg, window.parent.pressButtonVar, "error");
                    }
                }
            })
        } else if (result.isDenied) {
           
        }
    })

}
function SignOut() {
        $.ajax({
            url: '/Login/SignOut',
            type: 'get',
            success: function (data) {
                if (data.code == 200) {
                    window.location.href = data.Url
                } else {
                    Swal.fire(data.msg, window.parent.pressButtonVar, "error");
                }
            }
        })
}


$(function () {
     var li = ``
    $.ajax({
        url: '/Base/language',
        type: 'get',
        success: function (data) {
            if (data.code == 200) {
                if (data.language == "vi-vn") {
                    li = `<img class="h-20px w-20px rounded-sm" src="assets/media/svg/flags/220-vietnam.svg" alt="">`
                } else {
                    li = `<img class="h-20px w-20px rounded-sm" src="assets/media/svg/flags/226-united-states.svg" alt="">`
                }
                $('#sslanguage').append(li)
            }
        }
    })
})

$('li[name="changeLanguage"]').click(function () {
    var ddlCulture = $(this).attr('id')
    var returnUrl = window.location.href
    $.ajax({
        url: '/Base/ChangCulture',
        type: 'get',
        data: { ddlCulture, returnUrl },
        success: function (data) {
            if (data.code == 200) {
                window.location.href = data.url
            }
        }
    })
})