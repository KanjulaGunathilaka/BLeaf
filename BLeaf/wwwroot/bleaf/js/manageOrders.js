$(document).ready(function () {
    console.log("manageOrders.js loaded");

    // Function to fetch and display the count of pending orders
    function updatePendingOrdersBadge() {
        $.ajax({
            type: "GET",
            url: "/api/order/pendingCount",
            success: function (count) {
                if (count > 0) {
                    $("#pendingOrdersBadge").text(count).show();
                } else {
                    $("#pendingOrdersBadge").hide();
                }
            },
            error: function (xhr, status, error) {
                console.error("Failed to fetch pending orders count:", error);
            }
        });
    }

    // Call the function to update the badge on page load
    updatePendingOrdersBadge();

    // Load Orders
    function loadOrders() {
        $.ajax({
            type: "GET",
            url: "/api/order",
            success: function (orders) {
                var table = $("#orderDataTable tbody");
                table.empty();
                $.each(orders, function (i, order) {
                    var row = $("<tr>");
                    row.append($("<td>").text(order.orderId));
                    row.append($("<td>").text(order.user.fullName));
                    row.append($("<td>").text(order.address ? order.address.addressLine1 : ''));
                    row.append($("<td>").text(order.orderTotal));
                    row.append($("<td>").text(order.orderStatus));
                    row.append($("<td>").text(order.paymentStatus));
                    row.append($("<td>").text(order.orderPlacedAt));
                    row.append($("<td>").text(order.estimatedDelivery));
                    row.append($("<td>").text(order.deliveredAt));
                    row.append($("<td>").html('<div class="d-flex flex-wrap"><button class="btn btn-secondary edit-order m-1" data-id="' + order.orderId + '">Edit</button> <button class="btn btn-danger delete-order m-1" data-id="' + order.orderId + '">Delete</button></div>'));
                    table.append(row);
                });
            },
            error: function (xhr, status, error) {
                console.error("Failed to load orders:", error);
            }
        });
    }

    loadOrders();

    // Edit Order
    $(document).on("click", ".edit-order", function () {
        var orderId = $(this).data("id");
        editOrder(orderId);
    });

    // Delete Order
    $(document).on("click", ".delete-order", function () {
        var orderId = $(this).data("id");
        showOrderDeleteConfirmation(orderId);
    });

    function editOrder(orderId) {
        $.ajax({
            type: "GET",
            url: "/api/order/" + orderId,
            success: function (order) {
                populateOrderForm(order);
            },
            error: function (xhr, status, error) {
                console.error("Failed to fetch order:", error);
            }
        });
    }

    function deleteOrder(orderId) {
        $.ajax({
            type: "DELETE",
            url: "/api/order/" + orderId,
            success: function (response) {
                showOrderMessage("Order deleted successfully", "alert-success");
                loadOrders();
                updatePendingOrdersBadge(); // Update the badge after deleting an order
                clearOrderMessage();
            },
            error: function (xhr, status, error) {
                showOrderMessage("Failed to delete order", "alert-danger");
                console.error("Failed to delete order:", xhr.responseText);
                clearOrderMessage();
            }
        });
    }

    function showOrderDeleteConfirmation(orderId) {
        var confirmationModal = `
            <div class="modal fade" tabindex="-1" role="dialog" id="deleteOrderConfirmationModal">
                <div class="modal-dialog" role="document">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title">Confirm Deletion</h5>
                            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                                <span aria-hidden="true">&times;</span>
                            </button>
                        </div>
                        <div class="modal-body">
                            Are you sure you want to delete this order?
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary" id="confirmDeleteOrderCancelButton">Cancel</button>
                            <button type="button" class="btn btn-danger" id="confirmDeleteOrderButton">Delete</button>
                        </div>
                    </div>
                </div>
            </div>
        `;
        $('body').append(confirmationModal);

        $('#deleteOrderConfirmationModal').modal('show');

        $('#confirmDeleteOrderButton').on('click', function () {
            deleteOrder(orderId);
            $('#deleteOrderConfirmationModal').modal('hide');
        });

        $('#confirmDeleteOrderCancelButton').on('click', function () {
            $('#deleteOrderConfirmationModal').modal('hide');
        });

        $('#deleteOrderConfirmationModal').on('hidden.bs.modal', function (e) {
            $(this).remove();
        });
    }

    function populateOrderForm(order) {
        $("#orderId").val(order.orderId);
        $("#orderStatus").val(order.orderStatus);
        $("#paymentStatus").val(order.paymentStatus);

        // Store other required fields in hidden inputs
        $("#orderUserId").val(order.user.userId);
        $("#orderUserFullName").val(order.user.fullName);
        $("#orderUserEmail").val(order.user.email);
        $("#orderAddressId").val(order.addressId);
        $("#orderTotal").val(order.orderTotal);
        $("#orderPlacedAt").val(order.orderPlacedAt);
        $("#orderEstimatedDelivery").val(order.estimatedDelivery);
        $("#orderDeliveredAt").val(order.deliveredAt);

        $("#editOrderForm").show();
        $("#orderFormTitle").text("Edit Order Details");
        $("#submitOrder").text("Update Order");
        clearOrderMessage();
    }

    $("#submitOrder").click(function (event) {
        event.preventDefault();

        var formData = {
            orderId: $("#orderId").val(),
            orderStatus: $("#orderStatus").val(),
            paymentStatus: $("#paymentStatus").val(),
            userId: $("#orderUserId").val(),
            user: {
                userId: $("#orderUserId").val(),
                fullName: $("#orderUserFullName").val(),
                email: $("#orderUserEmail").val()
            },
            addressId: $("#orderAddressId").val(),
            orderTotal: $("#orderTotal").val(),
            orderPlacedAt: $("#orderPlacedAt").val(),
            estimatedDelivery: $("#orderEstimatedDelivery").val(),
            deliveredAt: $("#orderDeliveredAt").val()
        };

        updateOrder(formData);
    });

    function updateOrder(formData) {
        var orderId = formData.orderId;
        $.ajax({
            type: "PUT",
            url: "/api/order/" + orderId,
            data: JSON.stringify(formData),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                showOrderMessage("Order updated successfully", "alert-success");
                loadOrders();
                updatePendingOrdersBadge(); // Update the badge after updating an order
                $("#editOrderForm").hide();
                setTimeout(clearOrderMessage, 5000);
            },
            error: function (xhr, status, error) {
                showOrderMessage("Failed to update order", "alert-danger");
                console.error("Failed to update order:", xhr.responseText);
                setTimeout(clearOrderMessage, 5000);
            }
        });
    }

    $("#cancelOrderEdit").click(function () {
        $("#editOrderForm").hide();
        clearOrderMessage();
    });

    function showOrderMessage(message, alertClass) {
        var messageDiv = $("#orderMessage");
        messageDiv.removeClass();
        messageDiv.addClass("alert " + alertClass);
        messageDiv.text(message);
        messageDiv.show();
    }

    function clearOrderMessage() {
        var messageDiv = $("#orderMessage");
        messageDiv.removeClass();
        messageDiv.text("");
        messageDiv.hide();
    }
});