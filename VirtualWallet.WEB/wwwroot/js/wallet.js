$(document).ready(function () {
    $('.delete-wallet').on('click', function () {
        var walletId = $(this).data('id');

        if (confirm("Are you sure you want to delete this wallet?")) {
            $.ajax({
                url: `/Wallet/Delete/${walletId}`,
                type: 'DELETE',
                success: function (result) {
                    alert("Wallet deleted successfully.");
                    window.location.href = '/User/Wallets';
                },
                error: function (xhr, status, error) {
                    alert("Failed to delete the wallet. Please try again.");
                    console.error("Error deleting wallet:", xhr, status, error);
                }
            });
        }
    });

    $('#submit-add-user').on('click', function () {

        var walletId = $('#wallet-id').val();
        var username = $('#username').val();

        $.ajax({
            url: '/Wallet/AddUser',
            type: 'POST',
            data: {
                walletId: walletId,
                username: username,
            },
            success: function (result) {
                window.location.href = "/Wallet/Index/" + walletId
            },
            error: function (xhr, status, error) {
                alert("Failed to add user. Please try again.");
            }
        });

    });

    $('#submit-add-user').on('click', function () {

        var walletId = $('#wallet-id').val();
        var username = $('#username').val();

        $.ajax({
            url: '/Wallet/AddUser',
            type: 'POST',
            data: {
                walletId: walletId,
                username: username,
            },
            success: function (result) {
                window.location.href = "/Wallet/Index/" + walletId
            },
        });

    });

});


function removeUser(walletId, username) {
    $.ajax({
        url: '/Wallet/RemoveUser',
        type: 'POST',
        data: {
            walletId: walletId,
            username: username
        },
        success: function (response) {
            window.location.href = "/Wallet/Index/" + walletId
        },
        error: function (xhr, status, error) {
            alert("Error removing user: " + error);
        }
    });
}