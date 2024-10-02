$(document).ready(function () {
    console.log("manageItems.js loaded");

    $("#showAddItemForm").click(function () {
        clearItemForm();
        $("#addItemForm").toggle();
        $("#itemFormTitle").text("Enter Item Details");
        $("#submitItem").text("Submit Item");
        $("#itemId").val("");
        $("#cancelItemEdit").hide();
        clearItemMessage();
    });

    $("#submitItem").click(function (event) {
        event.preventDefault();

        var formData = {
            name: $("#itemName").val(),
            description: $("#itemDescription").val(),
            ingredients: $("#itemIngredients").val(),
            specialInformation: $("#itemSpecialInformation").val(),
            price: $("#itemPrice").val(),
            imageUrl: $("#itemImageUrl").val(),
            imageThumbnailUrl: $("#itemImageThumbnailUrl").val(),
            inStock: $("#itemInStock").is(":checked"),
            categoryId: parseInt($("#itemCategoryId").val()),
            isSpecial: false, // Or set this based on your form
            createdAt: new Date().toISOString(),
            updatedAt: new Date().toISOString()
        };

        var itemId = $("#itemId").val();

        if (itemId) {
            formData.itemId = itemId;
            updateItem(formData);
        } else {
            addItem(formData);
        }
    });

    function addItem(formData) {
        $.ajax({
            type: "POST",
            url: "/api/item",
            data: JSON.stringify(formData),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                showItemMessage("Item added successfully", "alert-success");
                loadItems();
                $("#addItemForm").hide();
                setTimeout(clearItemMessage, 5000);
            },
            error: function (xhr, status, error) {
                showItemMessage("Failed to add item", "alert-danger");
                console.error("Failed to add item:", xhr.responseText);
                setTimeout(clearItemMessage, 5000);
            }
        });
    }

    function updateItem(formData) {
        var itemId = formData.itemId;
        $.ajax({
            type: "PUT",
            url: "/api/item/" + itemId,
            data: JSON.stringify(formData),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                showItemMessage("Item updated successfully", "alert-success");
                loadItems();
                $("#addItemForm").hide();
                setTimeout(clearItemMessage, 5000);
            },
            error: function (xhr, status, error) {
                showItemMessage("Failed to update item", "alert-danger");
                console.error("Failed to update item:", xhr.responseText);
                setTimeout(clearItemMessage, 5000);
            }
        });
    }

    function loadItems() {
        $.ajax({
            type: "GET",
            url: "/api/item",
            success: function (items) {
                var table = $("#itemDataTable tbody");
                table.empty();
                $.each(items, function (i, item) {
                    var row = $("<tr>");
                    row.append($("<td>").text(item.itemId));
                    row.append($("<td>").addClass("text-truncate").attr("data-toggle", "tooltip").attr("title", item.name).text(item.name));
                    row.append($("<td>").addClass("text-truncate").attr("data-toggle", "tooltip").attr("title", item.description).text(item.description));
                    row.append($("<td>").addClass("text-truncate").attr("data-toggle", "tooltip").attr("title", item.ingredients).text(item.ingredients));
                    row.append($("<td>").addClass("text-truncate").attr("data-toggle", "tooltip").attr("title", item.specialInformation).text(item.specialInformation));
                    row.append($("<td>").text(item.price));
                    row.append($("<td>").addClass("text-truncate").attr("data-toggle", "tooltip").attr("title", item.imageUrl).text(item.imageUrl));
                    row.append($("<td>").addClass("text-truncate").attr("data-toggle", "tooltip").attr("title", item.imageThumbnailUrl).text(item.imageThumbnailUrl));
                    row.append($("<td>").text(item.inStock));
                    row.append($("<td>").addClass("text-truncate").attr("data-toggle", "tooltip").attr("title", item.category.name).text(item.category.name));
                    row.append($("<td>").html('<div class="d-flex"><button class="btn btn-secondary edit-item m-1" data-id="' + item.itemId + '">Edit</button> <button class="btn btn-danger delete-item m-1" data-id="' + item.itemId + '">Delete</button></div>'));
                    table.append(row);
                });
                $('[data-toggle="tooltip"]').tooltip(); // Initialize tooltips after loading items
            },
            error: function (xhr, status, error) {
                console.error("Failed to load items:", error);
            }
        });
    }

    loadItems();

    $(document).on("click", ".edit-item", function () {
        var itemId = $(this).data("id");
        editItem(itemId);
    });

    $(document).on("click", ".delete-item", function () {
        var itemId = $(this).data("id");
        showItemDeleteConfirmation(itemId);
    });

    function editItem(itemId) {
        $.ajax({
            type: "GET",
            url: "/api/item/" + itemId,
            success: function (item) {
                populateItemForm(item);
            },
            error: function (xhr, status, error) {
                console.error("Failed to fetch item:", error);
            }
        });
    }

    function deleteItem(itemId) {
        $.ajax({
            type: "DELETE",
            url: "/api/item/" + itemId,
            success: function (response) {
                showItemMessage("Item deleted successfully", "alert-success");
                loadItems();
                clearItemMessage();
            },
            error: function (xhr, status, error) {
                showItemMessage("Failed to delete item", "alert-danger");
                console.error("Failed to delete item:", xhr.responseText);
                clearItemMessage();
            }
        });
    }

    function showItemDeleteConfirmation(itemId) {
        var confirmationModal = `
            <div class="modal fade" tabindex="-1" role="dialog" id="deleteItemConfirmationModal">
                <div class="modal-dialog" role="document">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title">Confirm Deletion</h5>
                            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                                <span aria-hidden="true">&times;</span>
                            </button>
                        </div>
                        <div class="modal-body">
                            Are you sure you want to delete this item?
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary" id="confirmDeleteItemCancelButton">Cancel</button>
                            <button type="button" class="btn btn-danger" id="confirmDeleteItemButton">Delete</button>
                        </div>
                    </div>
                </div>
            </div>
        `;
        $('body').append(confirmationModal);

        $('#deleteItemConfirmationModal').modal('show');

        $('#confirmDeleteItemButton').on('click', function () {
            deleteItem(itemId);
            $('#deleteItemConfirmationModal').modal('hide');
        });

        $('#confirmDeleteItemCancelButton').on('click', function () {
            $('#deleteItemConfirmationModal').modal('hide');
        });

        $('#deleteItemConfirmationModal').on('hidden.bs.modal', function (e) {
            $(this).remove();
        });
    }

    function populateItemForm(item) {
        $("#itemName").val(item.name);
        $("#itemDescription").val(item.description);
        $("#itemIngredients").val(item.ingredients);
        $("#itemSpecialInformation").val(item.specialInformation);
        $("#itemPrice").val(item.price);
        $("#itemImageUrl").val(item.imageUrl);
        $("#itemImageThumbnailUrl").val(item.imageThumbnailUrl);
        $("#itemInStock").prop("checked", item.inStock);
        $("#itemCategoryId").val(item.categoryId);
        $("#itemId").val(item.itemId);

        $("#addItemForm").show();
        $("#itemFormTitle").text("Edit Item");
        $("#submitItem").text("Update Item");
        $("#cancelItemEdit").show();
        clearItemMessage();
    }

    $("#cancelItemEdit").click(function () {
        $("#addItemForm").hide();
        $("#itemFormTitle").text("Enter Item Details");
        $("#submitItem").text("Submit Item");
        clearItemForm();
        clearItemMessage();
    });

    function showItemMessage(message, alertClass) {
        var messageDiv = $("#itemMessage");
        messageDiv.removeClass();
        messageDiv.addClass("alert " + alertClass);
        messageDiv.text(message);
        messageDiv.show();
    }

    function clearItemMessage() {
        var messageDiv = $("#itemMessage");
        messageDiv.removeClass();
        messageDiv.text("");
        messageDiv.hide();
    }

    function clearItemForm() {
        $("#itemName").val("");
        $("#itemDescription").val("");
        $("#itemIngredients").val("");
        $("#itemSpecialInformation").val("");
        $("#itemPrice").val("");
        $("#itemImageUrl").val("");
        $("#itemImageThumbnailUrl").val("");
        $("#itemInStock").prop("checked", false);
        $("#itemCategoryId").val("");
        $("#itemId").val("");
    }

    $(function () {
        $('[data-toggle="tooltip"]').tooltip({
            trigger: 'hover',
            placement: 'top',
            container: 'body'
        });
    });
});