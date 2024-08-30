$(document).ready(function () {
    $('.delete-wallet').on('click', function () {
        var walletId = $(this).data('id');

        if (confirm("Are you sure you want to delete this wallet?")) {
            $.ajax({
                url: `/Wallet/Delete/${walletId}`,
                type: 'DELETE',
                success: function (result) {
                    alert("Wallet deleted successfully.");
                },
                error: function (xhr, status, error) {
                    alert("Failed to delete the wallet. Please try again.");
                    console.error("Error deleting wallet:", xhr, status, error);
                }
            });
        }
    });
});
