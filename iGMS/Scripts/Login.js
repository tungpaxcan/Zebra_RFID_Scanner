

//------------------------Login-----------------

function Login() {
    var user = $('#user').val().trim();
    var passs = $('#pass').val().trim();
    var pass = md5(passs)
    $.ajax({
        url: '/login/LoginiGMS',
        type: 'get',
        data: {
            user, pass
        },
        success: function (data) {
            if (data.code == 200) {              
                window.location.href = data.Url;            
            } else if (data.code == 300) {
                alert(data.msg)
                $('#pass').val('')
                
            }
            else {
                alert(data.msg)
            }
        }
    })
}

$(document).on('keypress', '#pass,#user', function (e) {
    if (e.which == 13) {
        e.preventDefault()
        Login()
    }
})
