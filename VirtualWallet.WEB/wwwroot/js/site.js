// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


$(document).ready(function () {
    $('#errorModal').modal({
    }).modal('show');
});

function loadContent(element) {
    var url = $(element).data("url");
    $.get(url, function (data) {
        $('#dashboard-content').html(data);
    }).fail(function () {
        $('#dashboard-content').html('<div class="alert alert-danger">Failed to load content.</div>');
    });
}
