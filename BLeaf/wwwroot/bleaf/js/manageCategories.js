$(document).ready(function () {
    console.log("manageCategories.js loaded");

    // Show the Add Category Form
    $("#showAddCategoryForm").click(function () {
        clearCategoryForm();
        $("#addCategoryForm").toggle();
        $("#categoryFormTitle").text("Enter Category Details");
        $("#submitCategory").text("Submit Category");
        $("#categoryId").val("");
        $("#cancelCategoryEdit").hide();
        clearCategoryMessage();
    });

    // Submit Category Form
    $("#submitCategory").click(function (event) {
        event.preventDefault();

        var formData = {
            name: $("#categoryName").val(),
            description: $("#categoryDescription").val(),
            createdAt: new Date().toISOString(),
            updatedAt: new Date().toISOString()
        };

        var categoryId = $("#categoryId").val();

        if (categoryId) {
            formData.categoryId = categoryId;
            updateCategory(formData);
        } else {
            addCategory(formData);
        }
    });

    function addCategory(formData) {
        $.ajax({
            type: "POST",
            url: "/api/category",
            data: JSON.stringify(formData),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                showCategoryMessage("Category added successfully", "alert-success");
                loadCategories();
                $("#addCategoryForm").hide();
                setTimeout(clearCategoryMessage, 5000);
            },
            error: function (xhr, status, error) {
                showCategoryMessage("Failed to add category", "alert-danger");
                console.error("Failed to add category:", xhr.responseText);
                setTimeout(clearCategoryMessage, 5000);
            }
        });
    }

    function updateCategory(formData) {
        var categoryId = formData.categoryId;
        $.ajax({
            type: "PUT",
            url: "/api/category/" + categoryId,
            data: JSON.stringify(formData),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                showCategoryMessage("Category updated successfully", "alert-success");
                loadCategories();
                $("#addCategoryForm").hide();
                setTimeout(clearCategoryMessage, 5000);
            },
            error: function (xhr, status, error) {
                showCategoryMessage("Failed to update category", "alert-danger");
                console.error("Failed to update category:", xhr.responseText);
                setTimeout(clearCategoryMessage, 5000);
            }
        });
    }

    function loadCategories() {
        $.ajax({
            type: "GET",
            url: "/api/category",
            success: function (categories) {
                var table = $("#categoryDataTable tbody");
                table.empty();
                $.each(categories, function (i, category) {
                    var row = $("<tr>");
                    row.append($("<td>").text(category.categoryId));
                    row.append($("<td>").addClass("text-truncate").attr("data-toggle", "tooltip").attr("title", category.name).text(category.name));
                    row.append($("<td>").addClass("text-truncate").attr("data-toggle", "tooltip").attr("title", category.description).text(category.description));
                    row.append($("<td>").html('<div class="d-flex flex-wrap"><button class="btn btn-secondary edit-category m-1" data-id="' + category.categoryId + '">Edit</button> <button class="btn btn-danger delete-category m-1" data-id="' + category.categoryId + '">Delete</button></div>'));
                    table.append(row);
                });
                $('[data-toggle="tooltip"]').tooltip(); // Initialize tooltips after loading categories
            },
            error: function (xhr, status, error) {
                console.error("Failed to load categories:", error);
            }
        });
    }

    loadCategories();

    $(document).on("click", ".edit-category", function () {
        var categoryId = $(this).data("id");
        editCategory(categoryId);
    });

    $(document).on("click", ".delete-category", function () {
        var categoryId = $(this).data("id");
        showCategoryDeleteConfirmation(categoryId);
    });

    function editCategory(categoryId) {
        $.ajax({
            type: "GET",
            url: "/api/category/" + categoryId,
            success: function (category) {
                populateCategoryForm(category);
            },
            error: function (xhr, status, error) {
                console.error("Failed to fetch category:", error);
            }
        });
    }

    function deleteCategory(categoryId) {
        $.ajax({
            type: "DELETE",
            url: "/api/category/" + categoryId,
            success: function (response) {
                showCategoryMessage("Category deleted successfully", "alert-success");
                loadCategories();
                clearCategoryMessage();
            },
            error: function (xhr, status, error) {
                showCategoryMessage("Failed to delete category", "alert-danger");
                console.error("Failed to delete category:", xhr.responseText);
                clearCategoryMessage();
            }
        });
    }

    function showCategoryDeleteConfirmation(categoryId) {
        var confirmationModal = `
            <div class="modal fade" tabindex="-1" role="dialog" id="deleteCategoryConfirmationModal">
                <div class="modal-dialog" role="document">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title">Confirm Deletion</h5>
                            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                                <span aria-hidden="true">&times;</span>
                            </button>
                        </div>
                        <div class="modal-body">
                            Are you sure you want to delete this category?
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary" id="confirmDeleteCategoryCancelButton">Cancel</button>
                            <button type="button" class="btn btn-danger" id="confirmDeleteCategoryButton">Delete</button>
                        </div>
                    </div>
                </div>
            </div>
        `;
        $('body').append(confirmationModal);

        $('#deleteCategoryConfirmationModal').modal('show');

        $('#confirmDeleteCategoryButton').on('click', function () {
            deleteCategory(categoryId);
            $('#deleteCategoryConfirmationModal').modal('hide');
        });

        $('#confirmDeleteCategoryCancelButton').on('click', function () {
            $('#deleteCategoryConfirmationModal').modal('hide');
        });

        $('#deleteCategoryConfirmationModal').on('hidden.bs.modal', function (e) {
            $(this).remove();
        });
    }

    function populateCategoryForm(category) {
        $("#categoryName").val(category.name);
        $("#categoryDescription").val(category.description);
        $("#categoryId").val(category.categoryId);

        $("#addCategoryForm").show();
        $("#categoryFormTitle").text("Edit Category");
        $("#submitCategory").text("Update Category");
        $("#cancelCategoryEdit").show();
        clearCategoryMessage();
    }

    $("#cancelCategoryEdit").click(function () {
        $("#addCategoryForm").hide();
        $("#categoryFormTitle").text("Enter Category Details");
        $("#submitCategory").text("Submit Category");
        clearCategoryForm();
        clearCategoryMessage();
    });

    function showCategoryMessage(message, alertClass) {
        var messageDiv = $("#categoryMessage");
        messageDiv.removeClass();
        messageDiv.addClass("alert " + alertClass);
        messageDiv.text(message);
        messageDiv.show();
    }

    function clearCategoryMessage() {
        var messageDiv = $("#categoryMessage");
        messageDiv.removeClass();
        messageDiv.text("");
        messageDiv.hide();
    }

    function clearCategoryForm() {
        $("#categoryName").val("");
        $("#categoryDescription").val("");
        $("#categoryId").val("");
    }

    $(function () {
        $('[data-toggle="tooltip"]').tooltip();
    });
});