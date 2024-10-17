$(document).ready(function () {
    console.log("manageReservations.js loaded");

    // Function to show messages
    function showReservationMessage(message, alertClass) {
        var messageDiv = $("#reservationMessage");
        messageDiv.removeClass();
        messageDiv.addClass("alert " + alertClass);
        messageDiv.text(message);
        messageDiv.show();
        setTimeout(function () {
            messageDiv.hide();
        }, 5000);
    }

    // Handle form submission for creating a reservation
    $("#reservationForm").on("submit", function (event) {
        event.preventDefault();
        var form = $(this);
        var formData = form.serialize();

        $.ajax({
            type: "POST",
            url: form.attr("action"),
            data: formData,
            success: function (response) {
                showReservationMessage(response.message, response.success ? "alert-success" : "alert-danger");
                if (response.success) {
                    form[0].reset();
                }
            },
            error: function (xhr, status, error) {
                showReservationMessage("An error occurred while processing your request. Please try again.", "alert-danger");
                console.error("Failed to create reservation:", xhr.responseText);
            }
        });
    });
});