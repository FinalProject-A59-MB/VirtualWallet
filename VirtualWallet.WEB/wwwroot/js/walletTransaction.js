$(document).ready(function () {
    $('#submit').on('click', function () {

        var inputAmount = $('#input-amount').val();
        var selectedFrom = $('#input-from').val();
        var selectedTo = $('#input-to').val();



        if (confirm("Are you sure you want to make this payment?")) {
            $.ajax({
                url: '/WalletTransaction/Deposit',
                type: 'POST',
                data: {
                    senderWalletId: selectedFrom,
                    recipientWalletId: selectedTo,
                    amount: inputAmount
                },
                success: function (result) {
                    window.location.href = "/WalletTransaction/VerifyPayment/" + result
                },
                error: function (xhr, status, error) {
                    alert("Failed to send payment. Please try again.");
                }
            });
        }
    });
    $('#verify').on('click', function () {

        var input = $('#input-code').val();
        var transactionId = $('#transaction-id').val();

        $.ajax({
            url: '/WalletTransaction/VerifyPayment',
            type: 'POST',
            data: {
                transactionId: transactionId,
                code: input,
            },
            success: function (result) {
                window.location.href = "/WalletTransaction/Details/" + transactionId
            },
            error: function (xhr, status, error) {
                alert("Failed to verify payment. Please try again.");
            }
        });

    });
});
