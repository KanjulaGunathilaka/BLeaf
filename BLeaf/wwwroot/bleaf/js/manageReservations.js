$(document).ready(function () {
    console.log("manageReservations.js loaded");

    // Store the current reservation details
    var currentReservation = {};

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

    // Handle form submission for creating a reservation (ContactUs page)
    $("#reservationForm").on("submit", function (event) {
        event.preventDefault();
        var form = $(this);
        var formData = form.serializeArray();
        var reservationData = {};

        $.each(formData, function (index, field) {
            // Convert field names to match the backend model casing
            var fieldName = field.name.charAt(0).toUpperCase() + field.name.slice(1);
            reservationData[fieldName] = field.value;
        });

        console.log("Creating reservation:", reservationData); // Debug log

        $.ajax({
            type: "POST",
            url: form.attr("action"),
            data: JSON.stringify(reservationData),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
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

    // Handle form submission for updating a reservation (Admin Panel)
    $("#updateReservation").on("click", function (event) {
        event.preventDefault();
        var reservationId = $("#reservationId").val();
        var reservationStatus = $("#reservationStatus").val();

        // Ensure reservationId is correctly set
        if (!reservationId) {
            console.error("Reservation ID is missing!");
            return;
        }

        // Update the status in the current reservation object
        currentReservation.reservationStatus = reservationStatus;

        console.log("Updating reservation:", currentReservation); // Debug log

        $.ajax({
            type: "PUT",
            url: "/api/reservation/" + reservationId,
            data: JSON.stringify(currentReservation),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                console.log("Update response:", response); // Debug log
                showReservationMessage(response.message, response.success ? "alert-success" : "alert-danger");
                if (response.success) {
                    $("#editReservationForm").hide();
                    loadReservations();
                }
            },
            error: function (xhr, status, error) {
                showReservationMessage("An error occurred while processing your request. Please try again.", "alert-danger");
                console.error("Failed to update reservation:", xhr.responseText);
            }
        });
    });

    // Load reservations (Admin Panel)
    function loadReservations() {
        $.ajax({
            type: "GET",
            url: "/api/reservation",
            success: function (reservations) {
                var table = $("#reservationDataTable tbody");
                table.empty();
                $.each(reservations, function (i, reservation) {
                    var row = $("<tr>");
                    row.append($("<td>").text(reservation.reservationId));
                    row.append($("<td>").text(reservation.name));
                    row.append($("<td>").text(reservation.email));
                    row.append($("<td>").text(reservation.phoneNumber));
                    row.append($("<td>").text(reservation.numberOfPeople));
                    row.append($("<td>").text(reservation.specialRequests));
                    row.append($("<td>").text(reservation.reservationStatus));
                    row.append($("<td>").html('<div class="d-flex flex-wrap justify-content-between"><button class="btn btn-secondary edit-reservation m-1" data-id="' + reservation.reservationId + '">Edit</button> <button class="btn btn-danger delete-reservation m-1" data-id="' + reservation.reservationId + '">Delete</button></div>'));
                    table.append(row);
                });
            },
            error: function (xhr, status, error) {
                console.error("Failed to load reservations:", error);
            }
        });
    }

    // Only load reservations if the table exists (admin panel)
    if ($("#reservationDataTable").length) {
        loadReservations();
    }

    $(document).on("click", ".edit-reservation", function () {
        var reservationId = $(this).data("id");
        editReservation(reservationId);
    });

    $(document).on("click", ".delete-reservation", function () {
        var reservationId = $(this).data("id");
        showReservationDeleteConfirmation(reservationId);
    });

    function editReservation(reservationId) {
        $.ajax({
            type: "GET",
            url: "/api/reservation/" + reservationId,
            success: function (reservation) {
                currentReservation = reservation; // Store the current reservation details
                populateReservationForm(reservation);
            },
            error: function (xhr, status, error) {
                console.error("Failed to fetch reservation:", error);
            }
        });
    }

    function deleteReservation(reservationId) {
        $.ajax({
            type: "DELETE",
            url: "/api/reservation/" + reservationId,
            success: function (response) {
                showReservationMessage("Reservation deleted successfully", "alert-success");
                loadReservations();
            },
            error: function (xhr, status, error) {
                showReservationMessage("Failed to delete reservation", "alert-danger");
                console.error("Failed to delete reservation:", xhr.responseText);
            }
        });
    }

    function showReservationDeleteConfirmation(reservationId) {
        var confirmationModal = `
            <div class="modal fade" tabindex="-1" role="dialog" id="deleteReservationConfirmationModal">
                <div class="modal-dialog" role="document">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title">Confirm Deletion</h5>
                            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                                <span aria-hidden="true">&times;</span>
                            </button>
                        </div>
                        <div class="modal-body">
                            Are you sure you want to delete this reservation?
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary" id="confirmDeleteReservationCancelButton">Cancel</button>
                            <button type="button" class="btn btn-danger" id="confirmDeleteReservationButton">Delete</button>
                        </div>
                    </div>
                </div>
            </div>
        `;
        $('body').append(confirmationModal);

        $('#deleteReservationConfirmationModal').modal('show');

        $('#confirmDeleteReservationButton').on('click', function () {
            deleteReservation(reservationId);
            $('#deleteReservationConfirmationModal').modal('hide');
        });

        $('#confirmDeleteReservationCancelButton').on('click', function () {
            $('#deleteReservationConfirmationModal').modal('hide');
        });

        $('#deleteReservationConfirmationModal').on('hidden.bs.modal', function (e) {
            $(this).remove();
        });
    }

    function populateReservationForm(reservation) {
        $("#reservationId").val(reservation.reservationId);
        $("#reservationStatus").val(reservation.reservationStatus);

        $("#editReservationForm").show();
        $("#reservationFormTitle").text("Edit Reservation Status");
        $("#submitReservation").text("Update Reservation");
        $("#cancelReservationEdit").show();
        clearReservationMessage();
    }

    $("#cancelReservationEdit").click(function () {
        $("#editReservationForm").hide();
        $("#reservationFormTitle").text("Edit Reservation Status");
        $("#submitReservation").text("Update Reservation");
        clearReservationForm();
        clearReservationMessage();
    });

    function clearReservationMessage() {
        var messageDiv = $("#reservationMessage");
        messageDiv.removeClass();
        messageDiv.text("");
        messageDiv.hide();
    }

    function clearReservationForm() {
        $("#reservationId").val("");
        $("#reservationStatus").val("Pending");
    }

    $(function () {
        $('[data-toggle="tooltip"]').tooltip();
    });
});