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
            userId = null; // Set userId to null for guest users
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
        var quantity = $(this).val();

        console.log("Updating cart:", { cartItemId, quantity });

        updateCart(cartItemId, quantity);
    });

    // Remove from Cart
    $(document).on("click", ".remove-from-cart", function (event) {
        event.preventDefault();

        var cartItemId = $(this).data("cart-item-id");

        console.log("Removing from cart:", { cartItemId });

        removeFromCart(cartItemId);
    });

    function addToCart(itemId, userId, quantity, shopCartUrl) {
        //$.ajax({
        //    url: '/api/ShoppingCartItem',
        //    type: 'POST',
        //    contentType: 'application/json',
        //    data: JSON.stringify({ ItemId: itemId, UserId: userId, Quantity: quantity }),
        //    success: function (result) {
        //        // Update cart UI
        //        showCartMessage("Item added to cart successfully", "alert-success");
        //        // Redirect to Shopping Cart page
        //        window.location.href = shopCartUrl;
        //    },
        //    error: function (xhr, status, error) {
        //        showCartMessage("Failed to add item to cart", "alert-danger");
        //        console.error("Failed to add item to cart:", xhr.responseText);
        //    }
        //});
        $.ajax({
            url: "/api/item/" + itemId,
            type: 'GET',
            success: function (item) {
                let cart = JSON.parse(localStorage.getItem('cart')) || [];

                // Check if the item already exists in the cart
                console.log('item', item, itemId)
                let existingItem = cart.find(cartItem => cartItem.item.itemId === item.itemId);
                console.log("exitsing", existingItem)
                if (existingItem) {
                    // Update the quantity of the existing item
                    existingItem.quantity += parseInt(quantity, 10);
                } else {
                    // Add the new item to the cart
                    cart.push({ item, quantity: parseInt(quantity, 10) });
                }

                localStorage.setItem('cart', JSON.stringify(cart));
                //showCartMessage("Item added to cart successfully", "alert-success");
                // Optionally, you can redirect to the shopping cart page
                // window.location.href = shopCartUrl;
            },
            error: function (xhr, status, error) {
                //showCartMessage("Failed to add item to cart", "alert-danger");
                console.error("Failed to add item to cart:", xhr.responseText);
            }
        });
        
    }

    function updateCart(cartItemId, quantity) {
        $.ajax({
            url: '/api/ShoppingCartItem/' + cartItemId,
            type: 'PUT',
            contentType: 'application/json',
            data: JSON.stringify({ ShoppingCartItemId: cartItemId, Quantity: quantity }),
            success: function (result) {
                // Update cart UI
                showCartMessage("Cart updated successfully", "alert-success");
                loadCart();
            },
            error: function (xhr, status, error) {
                showCartMessage("Failed to update cart", "alert-danger");
                console.error("Failed to update cart:", xhr.responseText);
            }
        });
    }

    function removeFromCart(cartItemId) {
        $.ajax({
            url: '/api/ShoppingCartItem/' + cartItemId,
            type: 'DELETE',
            success: function (result) {
                // Update cart UI
                showCartMessage("Item removed from cart successfully", "alert-success");
                loadCart();
            },
            error: function (xhr, status, error) {
                showCartMessage("Failed to remove item from cart", "alert-danger");
                console.error("Failed to remove item from cart:", xhr.responseText);
            }
        });
    }

    function loadCart() {
        let cart = getCartItems();
        console.log('cart', cart)
        var cartItemsContainer = $("#cartItemsContainer");
        cartItemsContainer.empty();
        $.each(cart, function (index, item) {
            var cartItemHtml = `
                <div class="cart-item style-1">
                    <div class="dz-media">
                        <img src="${item.item.imageUrl}" alt="${item.item.name}">
                    </div>
                    <div class="dz-content">
                        <div class="dz-head">
                            <h6 class="title mb-0">${item.item.name}</h6>
                            <a href="javascript:void(0);" class="remove-from-cart" data-cart-item-id="${item.shoppingCartItemId}"><i class="fa-solid fa-xmark text-danger"></i></a>
                        </div>
                        <div class="dz-body">
                            <div class="btn-quantity style-1">
                                <input id="quantity_${item.shoppingCartItemId}" type="text" value="${item.quantity}" class="update-cart" data-cart-item-id="${item.shoppingCartItemId}">
                            </div>
                            <h5 class="price text-primary mb-0">$${item.item.price * item.quantity}</h5>
                        </div>
                    </div>
                </div>
            `;
            cartItemsContainer.append(cartItemHtml);
        });

        // Update bill details
        let total = calculateCartTotal();
        console.log('total', total)
        let itemCount = calculateTotalItemCount()

        $("#itemTotal").text("$" + total);
        //$("#deliveryCharges").text("$" + cart.deliveryCharges.toFixed(2));
        //$("#taxes").text("$" + cart.taxes.toFixed(2));
        $("#grandTotal").text("$" + total);
        $("#cartItemCount").text(itemCount);


        //$.ajax({
        //    url: '/api/ShoppingCartItem',
        //    type: 'GET',
        //    success: function (cart) {
        //        // Update the cart UI with the new data
        //        var cartItemsContainer = $("#cartItemsContainer");
        //        cartItemsContainer.empty();

        //        $.each(cart.items, function (index, item) {
        //            var cartItemHtml = `
        //                <div class="cart-item style-1">
        //                    <div class="dz-media">
        //                        <img src="${item.item.imageUrl}" alt="${item.item.name}">
        //                    </div>
        //                    <div class="dz-content">
        //                        <div class="dz-head">
        //                            <h6 class="title mb-0">${item.item.name}</h6>
        //                            <a href="javascript:void(0);" class="remove-from-cart" data-cart-item-id="${item.shoppingCartItemId}"><i class="fa-solid fa-xmark text-danger"></i></a>
        //                        </div>
        //                        <div class="dz-body">
        //                            <div class="btn-quantity style-1">
        //                                <input id="quantity_${item.shoppingCartItemId}" type="text" value="${item.quantity}" class="update-cart" data-cart-item-id="${item.shoppingCartItemId}">
        //                            </div>
        //                            <h5 class="price text-primary mb-0">$${item.item.price}</h5>
        //                        </div>
        //                    </div>
        //                </div>
        //            `;
        //            cartItemsContainer.append(cartItemHtml);
        //        });

        //        // Update bill details
        //        $("#itemTotal").text("$" + cart.totalPrice.toFixed(2));
        //        $("#deliveryCharges").text("$" + cart.deliveryCharges.toFixed(2));
        //        $("#taxes").text("$" + cart.taxes.toFixed(2));
        //        $("#grandTotal").text("$" + cart.grandTotal.toFixed(2));
        //        $("#cartItemCount").text(cart.items.length);
        //    },
        //    error: function (xhr, status, error) {
        //        console.error("Failed to load cart:", xhr.responseText);
        //    }
        //});
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
        let total = cart.reduce((sum, item) => sum + item.item.price * item.quantity, 0);
        return total.toFixed(2); // Return the total as a string with 2 decimal places
    }

    function calculateTotalItemCount() {
        let cart = getCartItems();
        let count = cart.reduce((sum, item) => sum + parseInt(item.quantity, 10), 0);
        return count;
    }

    function loadCheckoutItems() {
        let cart = getCartItems();
        let checkoutItemsContainer = $("#checkoutItemsContainer");
        checkoutItemsContainer.empty();
        $.each(cart, function (index, item) {
            var checkoutItemHtml = `
                <tr>
                    <td class="product-item-img"><img src="${item.item.imageUrl}" alt="${item.item.name}"></td>
                    <td class="product-item-name">${item.item.name}</td>
                    <td class="product-price">$${item.item.price * item.quantity}</td>
                </tr>
            `;
            checkoutItemsContainer.append(checkoutItemHtml);
        });

        // Update order total
        let total = calculateCartTotal();
        $("#orderSubtotal").text("$" + total);
        $("#orderTotal").text("$" + total);
    }

    // Load checkout items if on checkout page
    if (window.location.pathname.includes("Checkout")) {
        loadCheckoutItems();
    }

    // Initial load of the cart
    loadCart();
});