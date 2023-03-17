

//--------------Click:: Add Roleadmin------------

$('#management').click(function () {
    RoleAdmin();
    $('#management').attr("disabled", true);
})
$('#sales').click(function () {
    Role();
    $('#sales').attr("disabled", true);
})

//-------------Edit::RoleAdmin------------
function RoleAdmin() {
    var idroleadmin = $('#idroleadmin').val().trim();
    var manageMaincategories = $('#manageMaincategories').is(":checked");
    var purchasemanager = $('#purchasemanager').is(":checked");
    var salesmanager = $('#salesmanager').is(":checked");
    var warehousemanagement = $('#warehousemanagement').is(":checked");
    var managepayments = $('#managepayments').is(":checked");
    var accountingtransfer = $('#accountingtransfer').is(":checked");
    $.ajax({
        url: '/roleadmin/EditRoleAdmin',
        type: 'post',
        data: {
            idroleadmin, manageMaincategories, purchasemanager, salesmanager,
            warehousemanagement, managepayments, accountingtransfer
        },
        success: function (data) {
            if (data.code == 200) {
                Swal.fire({
                    title: "Thành Công !!",
                    icon: "success",
                    buttonsStyling: false,
                    confirmButtonText: "Confirm me!",
                    customClass: {
                        confirmButton: "btn btn-primary"
                    }
                });
            }
        }
    })
}

//-------------Add::Role------------
function Role() {
    var idrole = $('#idrole').val().trim();
    var admin = $('#admin').is(":checked");
    var editdiscountgoods = $('#editdiscountgoods').is(":checked");
    var editdiscountbill = $('#editdiscountbill').is(":checked");
    var editpricegoods = $('#editpricegoods').is(":checked");
    var editamountgoods = $('#editamountgoods').is(":checked");
    var deletegoods = $('#deletegoods').is(":checked");
    var printagainbill = $('#printagainbill').is(":checked");
    var hangbill = $('#hangbill').is(":checked");
    var changecategoods = $('#changecategoods').is(":checked");
    var changeunit = $('#changeunit').is(":checked");
    var editdate = $('#editdate').is(":checked");
    var identifyconsultants = $('#identifyconsultants').is(":checked");
    var confirmcusinfor = $('#confirmcusinfor').is(":checked");
    var returngoods = $('#returngoods').is(":checked");
    $.ajax({
        url: '/roleadmin/EditRole',
        type: 'post',
        data: {
            idrole, admin, editdiscountgoods, editdiscountbill, editpricegoods,
            editamountgoods, deletegoods, printagainbill, hangbill, changecategoods,
            changeunit, editdate, identifyconsultants, confirmcusinfor, returngoods
        },
        success: function (data) {
            if (data.code == 200) {
                Swal.fire({
                    title: " Thành Công !!",
                    icon: "success",
                    buttonsStyling: false,
                    confirmButtonText: "Confirm me!",
                    customClass: {
                        confirmButton: "btn btn-primary"
                    }
                });
            }
        }
    })
}


//---------eye::pass---------
$('.icon-xl.far.fa-eye-slash').css("display", "none")
$('.icon-xl.far.fa-eye').click(function () {
    $('.icon-xl.far.fa-eye-slash').css("display", "block")
    $('.icon-xl.far.fa-eye').css("display", "none")
    $('#pass').attr("type", "text");
})
$('.icon-xl.far.fa-eye-slash').click(function () {
    $('.icon-xl.far.fa-eye-slash').css("display", "none")
    $('.icon-xl.far.fa-eye').css("display", "block")
    $('#pass').attr("type", "password");
})
    