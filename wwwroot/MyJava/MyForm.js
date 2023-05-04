function SubmitForm() {
    $.ajax({
        type: "post",
        url: "/RequestForm/MyForm",
        data: $('#contact-form').serialize()
    });
}
function Submit() {
    $('#myModal2').modal('show');
    SubmitForm();
}