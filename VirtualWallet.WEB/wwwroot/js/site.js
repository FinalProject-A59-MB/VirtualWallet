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

// showing the desctription form for contacts
function showEditForm(element) {
    const container = element.closest('.description-container');
    const form = container.querySelector('.description-form');
    const text = element;

    // Hide the text and show the form
    text.style.display = 'none';
    form.style.display = 'flex';
}


// toggle only 1 form (change email/password)
document.addEventListener("DOMContentLoaded", function () {

    let emailState = 0;
    let passwordState = 0;

    const emailButton = document.querySelector('[data-bs-target="#changeEmailForm"]');
    const passwordButton = document.querySelector('[data-bs-target="#changePasswordForm"]');
    const changeEmailForm = document.querySelector("#changeEmailForm");
    const changePasswordForm = document.querySelector("#changePasswordForm");

    emailButton.addEventListener("click", function () {
        if (emailState === 0) {
            // Turn on the email form
            changeEmailForm.classList.add("show");
            emailState = 1;

            // Turn off the password form if it is on
            if (passwordState === 1) {
                changePasswordForm.classList.remove("show");
                passwordState = 0;
            }
        } else {
            // Turn off the email form
            changeEmailForm.classList.remove("show");
            emailState = 0;
        }
    });

    passwordButton.addEventListener("click", function () {
        if (passwordState === 0) {
            // Turn on the password form
            changePasswordForm.classList.add("show");
            passwordState = 1;

            // Turn off the email form if it is on
            if (emailState === 1) {
                changeEmailForm.classList.remove("show");
                emailState = 0;
            }
        } else {
            // Turn off the password form
            changePasswordForm.classList.remove("show");
            passwordState = 0;
        }
    });
});

// add logo and change background color for cards in user/cards
document.addEventListener("DOMContentLoaded", function () {
    const issuerLogos = {
        "american_express": "/images/cardLogos/american-express.svg",
        "visa": "/images/cardLogos/visa.svg",
        "diners": "/images/cardLogos/diners.svg",
        "discover": "/images/cardLogos/discover.svg",
        "jcb": "/images/cardLogos/jcb.svg",
        "maestro": "/images/cardLogos/maestro.svg",
        "mastercard": "/images/cardLogos/mastercard.svg",
    };

    const issuerBackgrounds = {
        "american_express": "linear-gradient(10deg, #2f1152, #6a27a1)",
        "visa": "linear-gradient(10deg, #1f2857, #5e75d6)",
        "diners": "linear-gradient(10deg, #2b4145, #546d72)",
        "discover": "linear-gradient(10deg, #4a3525, #d69a76)",
        "jcb": "linear-gradient(10deg, #06364d, #6ab8d9)",
        "maestro": "linear-gradient(10deg, #541010, #c98181)",
        "mastercard": "linear-gradient(10deg, #542711, #915b41)",
    };

    document.querySelectorAll(".issuer-logo").forEach(function (img) {
        const issuer = img.getAttribute("data-issuer").toLowerCase();

        if (issuerLogos[issuer]) {
            img.setAttribute("src", issuerLogos[issuer]);
        } else {
            img.setAttribute("alt", "No logo available");
        }

        const cardElement = img.closest('.card');
        if (issuerBackgrounds[issuer]) {
            cardElement.style.setProperty('background', issuerBackgrounds[issuer], 'important');
        }
    });
});

