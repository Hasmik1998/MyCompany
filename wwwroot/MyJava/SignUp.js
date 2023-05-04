$(document).ready(function () {
    $(".btn-login").removeAttr("disabled");
});

$("input#password-field").on({
    keydown: function (e) {
        if (e.which === 32)
            return false;
    },
    change: function () {
        this.value = this.value.replace(/\s/g, "");
    }
});
function MyTrim() {
    $("#FirstName").val($.trim($("#FirstName").val()));
    $("#LastName").val($.trim($("#LastName").val()));
    if ($("#FirstName").val() == null || $("LastName").val() == null) {
        return false;
    }
    return true;
}