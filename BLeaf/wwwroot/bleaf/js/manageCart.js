$(document).ready(function () {
    console.log("manageCart.js loaded");

    // Add to Cart
    $(document).on("click", ".add-to-cart", function (event) {
        event.preventDefault();

        var itemId = $(this).data("item-id");
        var userId = $(this).data("user-id");
        var quantity = $("#quantity_" + itemId).val() || 1;
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
    $(document).on("change", ".update-cart", function () {
        var cartItemId = $(this).data("cart-item-id");
        var newQuantity = $(this).val();

        console.log("Updating cart:", { cartItemId, newQuantity });

        updateCartItem(cartItemId, newQuantity);
    });

    // Remove from Cart
    $(document).on("click", ".remove-from-cart", function (event) {
        event.preventDefault();

        var cartItemId = $(this).data("cart-item-id");

        console.log("Removing from cart:", { cartItemId });

        removeCartItem(cartItemId);
    });

    function addToCart(itemId, userId, quantity, shopCartUrl) {
        $.ajax({
            url: "/api/item/" + itemId,
            type: 'GET',
            success: function (item) {
                let cart = JSON.parse(localStorage.getItem('cart')) || [];

                // Check if the item already exists in the cart
                console.log('item', item, itemId)
                let existingCartItem = cart.find(cartItem => cartItem.item.itemId === item.itemId);
                console.log("existing", existingCartItem)
                if (existingCartItem) {
                    // Update the quantity of the existing item
                    existingCartItem.quantity += parseInt(quantity, 10);
                } else {
                    // Add the new item to the cart
                    cart.push({ item, quantity: parseInt(quantity, 10) });
                }

                localStorage.setItem('cart', JSON.stringify(cart));
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
            cartItem.quantity = parseInt(newQuantity, 10);
            localStorage.setItem('cart', JSON.stringify(cart));
            loadCart(); // Reload cart after updating item
        }
    }

    function removeCartItem(cartItemId) {
        console.log('Removing cart item ID:', cartItemId);
        let cart = JSON.parse(localStorage.getItem('cart')) || [];
        cart = cart.filter(cartItem => cartItem.item.itemId !== parseInt(cartItemId, 10));
        localStorage.setItem('cart', JSON.stringify(cart));
        loadCart(); // Reload cart after removing item
    }

    function loadCart() {
        let cart = getCartItems();
        console.log('cart', cart);
        var cartItemsContainer = $("#cartItemsContainer");
        cartItemsContainer.empty();
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
                        <h5 class="price text-primary mb-0">$${cartItem.item.price * cartItem.quantity}</h5>
                    </div>
                </div>
            </div>
        `;
            cartItemsContainer.append(cartItemHtml);
        });

        // Update bill details
        let total = calculateCartTotal();
        console.log('total', total);
        let itemCount = calculateTotalItemCount();

        $("#itemTotal").text("$" + total);
        $("#grandTotal").text("$" + total);
        $("#cartItemCount").text(itemCount);
    }

    function showCartMessage(message, alertClass) {
        var messageDiv = $("#cartMessage");
        messageDiv.removeClass();
        messageDiv.addClass("alert " + alertClass);
        messageDiv.text(message);
        messageDiv.show();
        setTimeout(function () {
            messageDiv.hide();
        }, 5000);
    }

    // Function to get all items from the cart
    function getCartItems() {
        return JSON.parse(localStorage.getItem('cart')) || [];
    }

    // Function to clear the cart
    function clearCart() {
        localStorage.removeItem('cart');
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

    // Initial load of the cart
    loadCart();
});