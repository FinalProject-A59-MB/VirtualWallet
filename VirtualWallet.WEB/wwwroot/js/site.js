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
        // Check if the response contains the dashboard content
        if ($(data).find('#dashboard-content').length > 0) {
            var newContent = $(data).find('#dashboard-content').html();
            $('#dashboard-content').html(newContent);
        } else {
            $('#dashboard-content').html(data);
        }
    }).fail(function () {
        $('#dashboard-content').html('<div class="alert alert-danger">Failed to load content.</div>');
    });
}

function saveDescription(event, contactId) {
    event.preventDefault();

    const form = $('#descriptionForm-' + contactId);
    const url = form.attr('action');
    const data = form.serialize();

    $.ajax({
        type: "POST",
        url: url,
        data: data,
        success: function (response) {
            // Update the UI with the new description
            const container = form.closest('.description-container');
            const textElement = container.find('.description-text');
            const formElement = container.find('.description-form');

            // Update the text with the new description
            textElement.text(form.find('input[name="description"]').val());

            // Hide the form and show the updated description
            formElement.hide();
            textElement.show();

            // Set additional styles if needed
            formElement.css('display', 'none');  // Ensure form is hidden
            textElement.css('display', 'flex');  // Ensure text is displayed with flex
        },
        error: function (xhr, status, error) {
            alert("There was an error saving the description: " + error);
        }
    });
}


function showEditForm(element) {
    const container = element.closest('.description-container');
    const form = container.querySelector('.description-form');
    const text = element;

    // Hide the text and show the form
    text.style.display = 'none';
    form.style.display = 'flex';
}