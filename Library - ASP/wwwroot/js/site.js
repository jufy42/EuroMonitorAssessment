function SuccessMessage(control, message) {
    $(".alert").remove();
    $(control)
        .before('<div class="alert bg-success">' +
            message +
            '</div>');
    $(".alert").delay(2000).fadeOut("slow", function() { $(this).remove(); });
}

function AlertMessage(control, message) {
    $(".alert").remove();
    $(control)
        .before('<div class="alert bg-danger">' +
            message +
            '</div>');
    $(".alert").delay(2000).fadeOut("slow", function() { $(this).remove(); });
}

$(document).ready(function() {
    $("#overlayLoadGenerate").hide();

    if ($(".alert").length) {
        $(".alert").delay(2000).fadeOut("slow", function() { $(this).remove(); });
    }
});