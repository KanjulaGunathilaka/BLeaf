$(document).ready(function () {
    console.log("manageDiscounts.js loaded");

    // Show the Add Discount Form
    $("#showAddDiscountForm").click(function () {
        clearDiscountForm();
        $("#addDiscountForm").toggle();
        $("#discountFormTitle").text("Enter Discount Details");
        $("#submitDiscount").text("Submit Discount");
        $("#discountId").val("");
        $("#cancelDiscountEdit").hide();
        clearDiscountMessage();
    });

    $("#submitDiscount").click(function (event) {
        event.preventDefault();

        var formData = {
            code: $("#discountCode").val(),
            description: $("#discountDescription").val(),
            discountAmount: $("#discountAmount").val(),
            discountPercentage: $("#discountPercentage").val(),
            validFrom: $("#validFrom").val(),
            validTo: $("#validTo").val(),
            isActive: $("#isActive").is(":checked"), // Ensure boolean value is correctly captured
            createdAt: new Date().toISOString(),
            updatedAt: new Date().toISOString()
        };

        var discountId = $("#discountId").val();

        if (discountId) {
            formData.discountId = discountId;
            updateDiscount(formData);
        } else {
            addDiscount(formData);
        }
    });

    function addDiscount(formData) {
        $.ajax({
            type: "POST",
            url: "/api/discount",
            data: JSON.stringify(formData),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                showDiscountMessage("Discount added successfully", "alert-success");
                loadDiscounts();
                $("#addDiscountForm").hide();
                setTimeout(clearDiscountMessage, 5000);
            },
            error: function (xhr, status, error) {
                showDiscountMessage("Failed to add discount", "alert-danger");
                console.error("Failed to add discount:", xhr.responseText);
                setTimeout(clearDiscountMessage, 5000);
            }
        });
    }

    function updateDiscount(formData) {
        var discountId = formData.discountId;
        $.ajax({
            type: "PUT",
            url: "/api/discount/" + discountId,
            data: JSON.stringify(formData),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                showDiscountMessage("Discount updated successfully", "alert-success");
                loadDiscounts();
                $("#addDiscountForm").hide();
                setTimeout(clearDiscountMessage, 5000);
            },
            error: function (xhr, status, error) {
                showDiscountMessage("Failed to update discount", "alert-danger");
                console.error("Failed to update discount:", xhr.responseText);
                setTimeout(clearDiscountMessage, 5000);
            }
        });
    }

    function loadDiscounts() {
        $.ajax({
            type: "GET",
            url: "/api/discount",
            success: function (discounts) {
                var table = $("#discountDataTable tbody");
                table.empty();
                $.each(discounts, function (i, discount) {
                    var row = $("<tr>");
                    row.append($("<td>").text(discount.discountId));
                    row.append($("<td>").text(discount.code));
                    row.append($("<td>").text(discount.description));
                    row.append($("<td>").text(discount.discountAmount));
                    row.append($("<td>").text(discount.discountPercentage));
                    row.append($("<td>").text(discount.validFrom ? new Date(discount.validFrom).toLocaleDateString() : ""));
                    row.append($("<td>").text(discount.validTo ? new Date(discount.validTo).toLocaleDateString() : ""));
                    row.append($("<td>").text(discount.isActive));
                    row.append($("<td>").html('<div class="d-flex flex-wrap"><button class="btn btn-secondary edit-discount m-1" data-id="' + discount.discountId + '">Edit</button> <button class="btn btn-danger delete-discount m-1" data-id="' + discount.discountId + '">Delete</button></div>'));
                    table.append(row);
                });
            },
            error: function (xhr, status, error) {
                console.error("Failed to load discounts:", error);
            }
        });
    }

    loadDiscounts();

    $(document).on("click", ".edit-discount", function () {
        var discountId = $(this).data("id");
        editDiscount(discountId);
    });

    $(document).on("click", ".delete-discount", function () {
        var discountId = $(this).data("id");
        showDiscountDeleteConfirmation(discountId);
    });

    function editDiscount(discountId) {
        $.ajax({
            type: "GET",
            url: "/api/discount/" + discountId,
            success: function (discount) {
                populateDiscountForm(discount);
            },
            error: function (xhr, status, error) {
                console.error("Failed to fetch discount:", error);
            }
        });
    }

    function deleteDiscount(discountId) {
        $.ajax({
            type: "DELETE",
            url: "/api/discount/" + discountId,
            success: function (response) {
                showDiscountMessage("Discount deleted successfully", "alert-success");
                loadDiscounts();
                clearDiscountMessage();
            },
            error: function (xhr, status, error) {
                showDiscountMessage("Failed to delete discount", "alert-danger");
                console.error("Failed to delete discount:", xhr.responseText);
                clearDiscountMessage();
            }
        });
    }

    function showDiscountDeleteConfirmation(discountId) {
        var confirmationModal = `
            <div class="modal fade" tabindex="-1" role="dialog" id="deleteDiscountConfirmationModal">
                <div class="modal-dialog" role="document">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title">Confirm Deletion</h5>
                            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                                <span aria-hidden="true">&times;</span>
                            </button>
                        </div>
                        <div class="modal-body">
                            Are you sure you want to delete this discount?
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary" id="confirmDeleteDiscountCancelButton">Cancel</button>
                            <button type="button" class="btn btn-danger" id="confirmDeleteDiscountButton">Delete</button>
                        </div>
                    </div>
                </div>
            </div>
        `;
        $('body').append(confirmationModal);

        $('#deleteDiscountConfirmationModal').modal('show');

        $('#confirmDeleteDiscountButton').on('click', function () {
            deleteDiscount(discountId);
            $('#deleteDiscountConfirmationModal').modal('hide');
        });

        $('#confirmDeleteDiscountCancelButton').on('click', function () {
            $('#deleteDiscountConfirmationModal').modal('hide');
        });

        $('#deleteDiscountConfirmationModal').on('hidden.bs.modal', function (e) {
            $(this).remove();
        });
    }

    function populateDiscountForm(discount) {
        $("#discountCode").val(discount.code);
        $("#discountDescription").val(discount.description);
        $("#discountAmount").val(discount.discountAmount);
        $("#discountPercentage").val(discount.discountPercentage);
        $("#validFrom").val(discount.validFrom ? new Date(discount.validFrom).toISOString().substring(0, 10) : "");
        $("#validTo").val(discount.validTo ? new Date(discount.validTo).toISOString().substring(0, 10) : "");
        $("#isActive").prop("checked", discount.isActive); // Ensure checkbox is set correctly
        $("#discountId").val(discount.discountId);

        $("#addDiscountForm").show();
        $("#discountFormTitle").text("Edit Discount");
        $("#submitDiscount").text("Update Discount");
        $("#cancelDiscountEdit").show();
        clearDiscountMessage();
    }

    $("#cancelDiscountEdit").click(function () {
        $("#addDiscountForm").hide();
        $("#discountFormTitle").text("Enter Discount Details");
        $("#submitDiscount").text("Submit Discount");
        clearDiscountForm();
        clearDiscountMessage();
    });

    function showDiscountMessage(message, alertClass) {
        var messageDiv = $("#discountMessage");
        messageDiv.removeClass();
        messageDiv.addClass("alert " + alertClass);
        messageDiv.text(message);
        messageDiv.show();
    }

    function clearDiscountMessage() {
        var messageDiv = $("#discountMessage");
        messageDiv.removeClass();
        messageDiv.text("");
        messageDiv.hide();
    }

    function clearDiscountForm() {
        $("#discountCode").val("");
        $("#discountDescription").val("");
        $("#discountAmount").val("");
        $("#discountPercentage").val("");
        $("#validFrom").val("");
        $("#validTo").val("");
        $("#isActive").prop("checked", false);
        $("#discountId").val("");
    }

    $(function () {
        $('[data-toggle="tooltip"]').tooltip();
    });
});