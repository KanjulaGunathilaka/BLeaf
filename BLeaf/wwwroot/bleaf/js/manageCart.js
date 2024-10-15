$(document).ready(function () {
    console.log("manageCart.js loaded");

    // Ensure single event listener for Add to Cart
    $(document).off("click", ".add-to-cart").on("click", ".add-to-cart", function (event) {
        event.preventDefault();

        console.log("Add to Cart button clicked");

        var itemId = $(this).data("item-id");
        var userId = $(this).data("user-id");
        var quantity = parseInt($("#quantity_" + itemId).val()) || 1;
        var shopCartUrl = $(this).data("shop-cart-url");

        if (userId === "guest") {
            userId = null;
        }

        console.log("Adding to cart:", { itemId, userId, quantity });

        if (!itemId) {
            console.error("Item ID is missing!");
            return;
        }

        addToCart(itemId, userId, quantity, shopCartUrl);
    });

    // Update Cart Item Quantity
    $(document).off("change", ".update-cart").on("change", ".update-cart", function () {
        var cartItemId = $(this).data("cart-item-id");
        var newQuantity = parseInt($(this).val());

        console.log("Updating cart:", { cartItemId, newQuantity });

        updateCartItem(cartItemId, newQuantity);
    });

    // Remove from Cart
    $(document).off("click", ".remove-from-cart").on("click", ".remove-from-cart", function (event) {
        event.preventDefault();

        var cartItemId = $(this).data("cart-item-id");

        console.log("Removing from cart:", { cartItemId });

        removeCartItem(cartItemId);
    });

    // Checkout Button Click
    $(document).off("click", ".btn-checkout").on("click", ".btn-checkout", function (event) {
        event.preventDefault();

        let cart = getCartItems();
        if (cart.length === 0) {
            showMessage("Your cart is empty. Please add items to your cart before proceeding to checkout.", "alert-warning");
            return;
        }

        window.location.href = $(this).attr("href");
    });

    function addToCart(itemId, userId, quantity, shopCartUrl) {
        $.ajax({
            url: "/api/item/" + itemId,
            type: 'GET',
            success: function (item) {
                let cart = JSON.parse(localStorage.getItem('cart')) || [];

                quantity = parseInt(quantity, 10);
                console.log('Parsed quantity:', quantity);

                // Check if the item already exists in the cart
                console.log('item', item, itemId);
                let existingCartItem = cart.find(cartItem => cartItem.item.itemId === item.itemId);
                console.log("existing", existingCartItem);
                if (existingCartItem) {
                    // Update the quantity of the existing item
                    existingCartItem.quantity += quantity;
                } else {
                    // Add the new item to the cart
                    cart.push({ item, quantity: quantity });
                }

                console.log("Cart after adding item:", cart);
                localStorage.setItem('cart', JSON.stringify(cart));
                updateCartItemCount(); // Update the cart item count
                loadCart(); // Reload cart after adding item
            },
            error: function (xhr, status, error) {
                console.error("Failed to add item to cart:", xhr.responseText);
            }
        });
    }

    function updateCartItem(cartItemId, newQuantity) {
        console.log('Updating cart item ID:', cartItemId, 'with new quantity:', newQuantity);
        let cart = JSON.parse(localStorage.getItem('cart')) || [];
        let cartItem = cart.find(cartItem => cartItem.item.itemId === parseInt(cartItemId, 10));
        if (cartItem) {
            cartItem.quantity = newQuantity;
            localStorage.setItem('cart', JSON.stringify(cart));
            updateCartItemCount(); // Update the cart item count
            loadCart(); // Reload cart after updating item
        }
    }

    function removeCartItem(cartItemId) {
        console.log('Removing cart item ID:', cartItemId);
        let cart = JSON.parse(localStorage.getItem('cart')) || [];
        cart = cart.filter(cartItem => cartItem.item.itemId !== parseInt(cartItemId, 10));
        localStorage.setItem('cart', JSON.stringify(cart));
        updateCartItemCount(); // Update the cart item count
        loadCart(); // Reload cart after removing item
    }

    function loadCart() {
        let cart = getCartItems();
        console.log('cart', cart);
        var cartItemsContainer = $("#cartItemsContainer");
        cartItemsContainer.empty();

        if (cart.length === 0) {
            $("#emptyCartMessage").show();
        } else {
            $("#emptyCartMessage").hide();
            $.each(cart, function (index, cartItem) {
                console.log('Setting cart item ID:', cartItem.item.itemId);
                var cartItemHtml = `
                <div class="cart-item style-1">
                    <div class="dz-media">
                        <img src="${cartItem.item.imageUrl}" alt="${cartItem.item.name}">
                    </div>
                    <div class="dz-content">
                        <div class="dz-head">
                            <h6 class="title mb-0">${cartItem.item.name}</h6>
                            <a href="javascript:void(0);" class="remove-from-cart" data-cart-item-id="${cartItem.item.itemId}"><i class="fa-solid fa-xmark text-danger"></i></a>
                        </div>
                        <div class="dz-body">
                            <div class="btn-quantity style-1">
                                <input id="quantity_${cartItem.item.itemId}" type="text" value="${cartItem.quantity}" class="update-cart" data-cart-item-id="${cartItem.item.itemId}">
                            </div>
                            <h5 class="price text-primary mb-0">$${(cartItem.item.price * cartItem.quantity).toFixed(2)}</h5>
                        </div>
                    </div>
                </div>
            `;
                cartItemsContainer.append(cartItemHtml);
            });
        }

        // Update bill details
        let total = calculateCartTotal();
        console.log('total', total);
        $("#itemTotal").text("$" + total);
        $("#grandTotal").text("$" + total);
    }

    function showMessage(message, alertClass) {
        var messageDiv = $("#cartMessage");
        messageDiv.removeClass();
        messageDiv.addClass("alert " + alertClass);
        messageDiv.text(message);
        messageDiv.show();
        setTimeout(function () {
            messageDiv.hide();
        }, 5000);
    }

    function getCartItems() {
        return JSON.parse(localStorage.getItem('cart')) || [];
    }

    function calculateCartTotal() {
        let cart = getCartItems();
        let total = cart.reduce((sum, cartItem) => sum + cartItem.item.price * cartItem.quantity, 0);
        return total.toFixed(2); // Return the total as a string with 2 decimal places
    }

    function calculateTotalItemCount() {
        let cart = getCartItems();
        let count = cart.reduce((sum, cartItem) => sum + parseInt(cartItem.quantity, 10), 0);
        return count;
    }

    // Function to update the cart item count in the header and sidebar
    function updateCartItemCount() {
        let cart = getCartItems();
        let itemCount = cart.reduce((sum, item) => sum + item.quantity, 0);
        $("#cartItemCount").text(itemCount); // Update header cart item count
        $(".widget-title .text-primary").text(`(${itemCount})`); // Update sidebar cart item count
    }

    // Initial load of the cart
    loadCart();

    // Initial update of the cart item count
    updateCartItemCount();
});