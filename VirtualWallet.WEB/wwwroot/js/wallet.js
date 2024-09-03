//$(document).ready(function () {
//    $('.delete-wallet').on('click', function () {
//        var walletId = $(this).data('id');

//        if (confirm("Are you sure you want to delete this wallet?")) {
//            $.ajax({
//                url: `/Wallet/Delete/${walletId}`,
//                type: 'DELETE',
//                success: function (result) {
//                    alert("Wallet deleted successfully.");
//                    window.location.href = '/User/Wallets';
//                },
//                error: function (xhr, status, error) {
//                    alert("Failed to delete the wallet. Please try again.");
//                    console.error("Error deleting wallet:", xhr, status, error);
//                }
//            });
//        }
//    });

//    $('#submit-add-user').on('click', function () {

//        var walletId = $('#wallet-id').val();
//        var username = $('#username').val();

//        $.ajax({
//            url: '/Wallet/AddUser',
//            type: 'POST',
//            data: {
//                walletId: walletId,
//                username: username,
//            },
//            success: function (result) {
//                window.location.href = "/Wallet/Index/" + walletId
//            },
//            error: function (xhr, status, error) {
//                alert("Failed to add user. Please try again.");
//            }
//        });

//    });

//    $('#submit-add-user').on('click', function () {

//        var walletId = $('#wallet-id').val();
//        var username = $('#username').val();

//        $.ajax({
//            url: '/Wallet/AddUser',
//            type: 'POST',
//            data: {
//                walletId: walletId,
//                username: username,
//            },
//            success: function (result) {
//                window.location.href = "/Wallet/Index/" + walletId
//            },
//        });

//    });

//});


//function removeUser(walletId, username) {
//    $.ajax({
//        url: '/Wallet/RemoveUser',
//        type: 'POST',
//        data: {
//            walletId: walletId,
//            username: username
//        },
//        success: function (response) {
//            window.location.href = "/Wallet/Index/" + walletId
//        },
//        error: function (xhr, status, error) {
//            alert("Error removing user: " + error);
//        }
//    });
//}

document.addEventListener('DOMContentLoaded', function () {
    const editButton = document.getElementById('edit-wallet-button');
    const saveButton = document.getElementById('save-wallet-button');
    const cancelButton = document.getElementById('cancel-wallet-button');
    const walletNameElement = document.getElementById('wallet-name');
    const walletNameInput = document.getElementById('wallet-name-input');

    editButton.addEventListener('click', function () {
        walletNameElement.style.display = 'none';
        walletNameInput.style.display = 'inline-block';
        editButton.style.display = 'none';
        saveButton.style.display = 'inline-block';
        cancelButton.style.display = 'inline-block';
    });

    saveButton.addEventListener('click', function () {
        // Trigger form submission to save the updated wallet name
        document.getElementById('edit-wallet-form').submit();
    });

    cancelButton.addEventListener('click', function () {
        walletNameElement.style.display = 'inline-block';
        walletNameInput.style.display = 'none';
        editButton.style.display = 'inline-block';
        saveButton.style.display = 'none';
        cancelButton.style.display = 'none';
    });
});
