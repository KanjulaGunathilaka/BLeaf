$(document).ready(function () {
    console.log("manageUsers.js loaded");

    // Show the Add User Form
    $("#showAddUserForm").click(function () {
        clearUserForm();
        $("#addUserForm").toggle();
        $("#userFormTitle").text("Enter User Details");
        $("#submitUser").text("Submit User");
        $("#userId").val("");
        $("#cancelUserEdit").hide();
        clearUserMessage();
    });

    // Submit User Form
    $("#submitUser").click(function (event) {
        event.preventDefault();

        var formData = {
            fullName: $("#fullName").val(),
            email: $("#userEmail").val(), // Ensure the correct ID is used
            phoneNumber: $("#phoneNumber").val(),
            billingAddress: $("#billingAddress").val(),
            role: $("#role").val(),
            passwordHash: "default_password_hash", // Replace with actual password hashing logic
            addresses: [],
            orders: [],
            shoppingCartItems: [],
            reservations: []
        };

        var userId = $("#userId").val();

        if (userId) {
            formData.userId = userId;
            updateUser(formData);
        } else {
            addUser(formData);
        }
    });

    function addUser(formData) {
        $.ajax({
            type: "POST",
            url: "/api/user",
            data: JSON.stringify(formData),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                showUserMessage("User added successfully", "alert-success");
                loadUsers();
                $("#addUserForm").hide();
                setTimeout(clearUserMessage, 5000);
            },
            error: function (xhr, status, error) {
                showUserMessage("Failed to add user", "alert-danger");
                console.error("Failed to add user:", xhr.responseText);
                setTimeout(clearUserMessage, 5000);
            }
        });
    }

    function updateUser(formData) {
        var userId = formData.userId;
        $.ajax({
            type: "PUT",
            url: "/api/user/" + userId,
            data: JSON.stringify(formData),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                showUserMessage("User updated successfully", "alert-success");
                loadUsers();
                $("#addUserForm").hide();
                setTimeout(clearUserMessage, 5000);
            },
            error: function (xhr, status, error) {
                showUserMessage("Failed to update user", "alert-danger");
                console.error("Failed to update user:", xhr.responseText);
                setTimeout(clearUserMessage, 5000);
            }
        });
    }

    function loadUsers() {
        $.ajax({
            type: "GET",
            url: "/api/user",
            success: function (users) {
                var table = $("#userDataTable tbody");
                table.empty();
                $.each(users, function (i, user) {
                    var row = $("<tr>");
                    row.append($("<td>").text(user.userId));
                    row.append($("<td>").addClass("text-truncate").attr("data-toggle", "tooltip").attr("title", user.fullName).text(user.fullName));
                    row.append($("<td>").addClass("text-truncate").attr("data-toggle", "tooltip").attr("title", user.email).text(user.email));
                    row.append($("<td>").addClass("text-truncate").attr("data-toggle", "tooltip").attr("title", user.phoneNumber).text(user.phoneNumber));
                    row.append($("<td>").addClass("text-truncate").attr("data-toggle", "tooltip").attr("title", user.billingAddress).text(user.billingAddress));
                    row.append($("<td>").addClass("text-truncate").attr("data-toggle", "tooltip").attr("title", user.role).text(user.role));
                    row.append($("<td>").html('<div class="d-flex flex-wrap"><button class="btn btn-secondary edit-user m-1" data-id="' + user.userId + '">Edit</button> <button class="btn btn-danger delete-user m-1" data-id="' + user.userId + '">Delete</button></div>'));
                    table.append(row);
                });
                $('[data-toggle="tooltip"]').tooltip(); // Initialize tooltips after loading users
            },
            error: function (xhr, status, error) {
                console.error("Failed to load users:", error);
            }
        });
    }

    loadUsers();

    $(document).on("click", ".edit-user", function () {
        var userId = $(this).data("id");
        editUser(userId);
    });

    $(document).on("click", ".delete-user", function () {
        var userId = $(this).data("id");
        showUserDeleteConfirmation(userId);
    });

    function editUser(userId) {
        $.ajax({
            type: "GET",
            url: "/api/user/" + userId,
            success: function (user) {
                populateUserForm(user);
            },
            error: function (xhr, status, error) {
                console.error("Failed to fetch user:", error);
            }
        });
    }

    function deleteUser(userId) {
        $.ajax({
            type: "DELETE",
            url: "/api/user/" + userId,
            success: function (response) {
                showUserMessage("User deleted successfully", "alert-success");
                loadUsers();
                clearUserMessage();
            },
            error: function (xhr, status, error) {
                showUserMessage("Failed to delete user", "alert-danger");
                console.error("Failed to delete user:", xhr.responseText);
                clearUserMessage();
            }
        });
    }

    function showUserDeleteConfirmation(userId) {
        var confirmationModal = `
            <div class="modal fade" tabindex="-1" role="dialog" id="deleteUserConfirmationModal">
                <div class="modal-dialog" role="document">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title">Confirm Deletion</h5>
                            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                                <span aria-hidden="true">&times;</span>
                            </button>
                        </div>
                        <div class="modal-body">
                            Are you sure you want to delete this user?
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary" id="confirmDeleteUserCancelButton">Cancel</button>
                            <button type="button" class="btn btn-danger" id="confirmDeleteUserButton">Delete</button>
                        </div>
                    </div>
                </div>
            </div>
        `;
        $('body').append(confirmationModal);

        $('#deleteUserConfirmationModal').modal('show');

        $('#confirmDeleteUserButton').on('click', function () {
            deleteUser(userId);
            $('#deleteUserConfirmationModal').modal('hide');
        });

        $('#confirmDeleteUserCancelButton').on('click', function () {
            $('#deleteUserConfirmationModal').modal('hide');
        });

        $('#deleteUserConfirmationModal').on('hidden.bs.modal', function (e) {
            $(this).remove();
        });
    }

    function populateUserForm(user) {
        $("#fullName").val(user.fullName);
        $("#userEmail").val(user.email); // Ensure the correct ID is used
        $("#phoneNumber").val(user.phoneNumber);
        $("#billingAddress").val(user.billingAddress);
        $("#role").val(user.role);
        $("#userId").val(user.userId);

        $("#addUserForm").show();
        $("#userFormTitle").text("Edit User");
        $("#submitUser").text("Update User");
        $("#cancelUserEdit").show();
        clearUserMessage();
    }

    $("#cancelUserEdit").click(function () {
        $("#addUserForm").hide();
        $("#userFormTitle").text("Enter User Details");
        $("#submitUser").text("Submit User");
        clearUserForm();
        clearUserMessage();
    });

    function showUserMessage(message, alertClass) {
        var messageDiv = $("#userMessage");
        messageDiv.removeClass();
        messageDiv.addClass("alert " + alertClass);
        messageDiv.text(message);
        messageDiv.show();
    }

    function clearUserMessage() {
        var messageDiv = $("#userMessage");
        messageDiv.removeClass();
        messageDiv.text("");
        messageDiv.hide();
    }

    function clearUserForm() {
        $("#fullName").val("");
        $("#userEmail").val(""); // Ensure the correct ID is used
        $("#phoneNumber").val("");
        $("#billingAddress").val("");
        $("#role").val("Customer");
        $("#userId").val("");
    }

    $(function () {
        $('[data-toggle="tooltip"]').tooltip();
    });
});