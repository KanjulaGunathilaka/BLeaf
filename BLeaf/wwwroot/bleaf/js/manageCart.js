$(document).ready(function () {

    // Ensure single event listener for Add to Cart
    $(document).off("click", ".add-to-cart").on("click", ".add-to-cart", function (event) {
        event.preventDefault();
        var itemId = $(this).data("item-id");
        var userId = $(this).data("user-id");
        var quantity = parseInt($("#quantity_" + itemId).val()) || 1;
        var shopCartUrl = $(this).data("shop-cart-url");

        if (userId === "guest") {
            userId = null;
        }
        if (!itemId) {
            console.error("Item ID is missing!");
            return;
        }

        if (quantity < 1 || quantity > 100) {
            showMessage("Quantity must be between 1 and 100.", "alert-warning", "#cartMessage");
            return;
        }

        addToCart(itemId, userId, quantity, shopCartUrl);
    });

    // Event listener for Add to Cart from Menu button
    $(document).off("click", ".add-to-cart-menu").on("click", ".add-to-cart-menu", function (event) {
        event.preventDefault();
        var itemId = $(this).data("item-id");
        var userId = $(this).data("user-id");
        var quantity = 1; // Default quantity for menu add-to-cart

        if (userId === "guest") {
            userId = null;
        }
        if (!itemId) {
            console.error("Item ID is missing!");
            return;
        }

        var $button = $(this);
        addToCart(itemId, userId, quantity, function () {
            // Change the icon to a checkmark
            $button.html('<i class="fa fa-check"></i>').addClass('added-to-cart');

            // Revert back to the original icon after 2 seconds
            setTimeout(function () {
                $button.html('<i class="fa fa-plus"></i>').removeClass('added-to-cart');
            }, 2000);
        });
    });

    // Event listener for Buy Now button
    $(document).off("click", ".btn-buy-now").on("click", ".btn-buy-now", function (event) {
        event.preventDefault();
        var itemId = $(this).data("item-id");
        var userId = $(this).data("user-id");
        var quantity = parseInt($("#quantity_" + itemId).val()) || 1;
        var checkoutUrl = $(this).data("checkout-url");

        if (userId === "guest") {
            userId = null;
        }
        if (!itemId) {
            console.error("Item ID is missing!");
            return;
        }

        if (quantity < 1 || quantity > 100) {
            showMessage("Quantity must be between 1 and 100.", "alert-warning", "#cartMessage");
            return;
        }

        addToCartAndCheckout(itemId, userId, quantity, checkoutUrl);
    });

    // Update Cart Item Quantity
    $(document).off("change", ".update-cart").on("change", ".update-cart", function () {
        var cartItemId = $(this).data("cart-item-id");
        var newQuantity = parseInt($(this).val());

        // Handle null or invalid values
        if (!newQuantity || newQuantity < 1 || newQuantity > 100) {
            showMessage("Quantity must be between 1 and 100.", "alert-warning", "#cartMessage");
            $(this).val(1); // Reset to minimum value
            return;
        }

        updateCartItem(cartItemId, newQuantity);
    });

    // Prevent user from completely removing the text from the textbox
    $(document).off("input", ".update-cart").on("input", ".update-cart", function () {
        if ($(this).val() === "") {
            $(this).val(1); // Set to minimum value if empty
        }
    });

    // Remove from Cart
    $(document).off("click", ".remove-from-cart").on("click", ".remove-from-cart", function (event) {
        event.preventDefault();
        var cartItemId = $(this).data("cart-item-id");
        removeCartItem(cartItemId);
    });

    // Checkout Button Click
    $(document).off("click", ".btn-checkout").on("click", ".btn-checkout", function (event) {
        event.preventDefault();

        let cart = getCartItems();
        if (cart.length === 0) {
            showMessage("Your cart is empty. Please add items to your cart before proceeding to checkout.", "alert-warning", "#cartMessage");
            return;
        }

        window.location.href = $(this).attr("href");
    });

    function addToCart(itemId, userId, quantity, callback) {
        $.ajax({
            url: "/api/item/" + itemId,
            type: 'GET',
            success: function (item) {
                let cart = JSON.parse(localStorage.getItem('cart')) || [];
                quantity = parseInt(quantity, 10);

                // Check if the item already exists in the cart
                let existingCartItem = cart.find(cartItem => cartItem.item.itemId === item.itemId);
                if (existingCartItem) {
                    // Update the quantity of the existing item
                    existingCartItem.quantity += quantity;
                } else {
                    // Add the new item to the cart
                    cart.push({ item, quantity: quantity });
                }

                localStorage.setItem('cart', JSON.stringify(cart));
                updateCartItemCount(); // Update the cart item count
                loadCart(); // Reload cart after adding item
                showMessage("Item added to cart successfully!", "alert-success", "#cartMessage");

                if (callback) {
                    callback();
                }
            },
            error: function (xhr, status, error) {
                showMessage("Failed to add item to cart.", "alert-danger", "#cartMessage");
            }
        });
    }

    function addToCartAndCheckout(itemId, userId, quantity, checkoutUrl) {
        $.ajax({
            url: "/api/item/" + itemId,
            type: 'GET',
            success: function (item) {
                let cart = JSON.parse(localStorage.getItem('cart')) || [];
                quantity = parseInt(quantity, 10);

                let existingCartItem = cart.find(cartItem => cartItem.item.itemId === item.itemId);
                if (existingCartItem) {
                    // Update the quantity of the existing item
                    existingCartItem.quantity += quantity;
                } else {
                    // Add the new item to the cart
                    cart.push({ item, quantity: quantity });
                }

                localStorage.setItem('cart', JSON.stringify(cart));
                updateCartItemCount(); // Update the cart item count
                loadCart(); // Reload cart after adding item
                window.location.href = checkoutUrl; // Navigate to checkout page
            },
            error: function (xhr, status, error) {
                showMessage("Failed to add item to cart.", "alert-danger", "#cartMessage");
            }
        });
    }

    function updateCartItem(cartItemId, newQuantity) {
        let cart = JSON.parse(localStorage.getItem('cart')) || [];
        let cartItem = cart.find(cartItem => cartItem.item.itemId === parseInt(cartItemId, 10));
        if (cartItem) {
            cartItem.quantity = newQuantity;
            localStorage.setItem('cart', JSON.stringify(cart));
            updateCartItemCount(); // Update the cart item count
            applyPromoCodeAndLoadCart(); // Reapply promo code and reload cart after updating item
        }
    }

    function removeCartItem(cartItemId) {
        let cart = JSON.parse(localStorage.getItem('cart')) || [];
        cart = cart.filter(cartItem => cartItem.item.itemId !== parseInt(cartItemId, 10));
        localStorage.setItem('cart', JSON.stringify(cart));
        updateCartItemCount(); // Update the cart item count
        applyPromoCodeAndLoadCart(); // Reapply promo code and reload cart after removing item
    }

    function applyPromoCodeAndLoadCart() {
        // Reapply promo code if it exists
        let promoCode = localStorage.getItem('promoCode');
        if (promoCode) {
            applyPromoCode(promoCode, loadCart);
        } else {
            loadCart();
        }
    }

    function loadCart() {
        let cart = getCartItems();
        var cartItemsContainer = $("#cartItemsContainer");
        cartItemsContainer.empty();

        if (cart.length === 0) {
            $("#emptyCartMessage").show();
        } else {
            $("#emptyCartMessage").hide();
            $.each(cart, function (index, cartItem) {
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
        $("#itemTotal").text("$" + total);

        // Retrieve discount information from localStorage
        let discountValue = localStorage.getItem('discountValue') || "0.00";
        let discountedTotal = localStorage.getItem('discountedTotal') || total;

        $("#discountValue").text("-$" + discountValue);
        $("#grandTotal").text("$" + discountedTotal);
    }

    // Event listener for Apply Promo Code button
    $(document).off("click", "#applyPromoCode").on("click", "#applyPromoCode", function (event) {
        event.preventDefault();
        var promoCode = $("#promoCode").val();
        applyPromoCode(promoCode, loadCart);
    });

    function applyPromoCode(promoCode, callback) {
        $.ajax({
            url: "/api/discount/validate/" + promoCode,
            type: 'GET',
            success: function (discount) {
                if (discount) {
                    let total = calculateCartTotal();

                    // Check if the total meets the minimum order amount requirement
                    const minimumOrderAmount = 20;
                    if (total < minimumOrderAmount) {
                        showMessage("Your order must be at least $" + minimumOrderAmount + " to apply this promo code.", "alert-warning", "#cartMessage");
                        $("#discountValue").text("$0.00");
                        $("#grandTotal").text("$" + total.toFixed(2));
                        return;
                    }

                    let discountValue = 0;

                    if (discount.discountAmount) {
                        discountValue = discount.discountAmount;
                    } else if (discount.discountPercentage) {
                        discountValue = total * (discount.discountPercentage / 100);
                    }

                    // Ensure the discount value does not exceed the total
                    if (discountValue > total) {
                        discountValue = total;
                    }

                    let discountedTotal = total - discountValue;

                    // Ensure the discounted total is not less than zero
                    if (discountedTotal < 0) {
                        discountedTotal = 0;
                    }

                    // Store discount information in localStorage
                    localStorage.setItem('discountValue', discountValue.toFixed(2));
                    localStorage.setItem('discountedTotal', discountedTotal.toFixed(2));
                    localStorage.setItem('promoCode', promoCode);

                    $("#discountValue").text("-$" + discountValue.toFixed(2));
                    $("#grandTotal").text("$" + discountedTotal.toFixed(2));
                    showMessage("Promo code applied successfully!", "alert-success", "#cartMessage");

                    if (callback) {
                        callback();
                    }
                } else {
                    showMessage("Invalid promo code.", "alert-danger", "#cartMessage");
                    localStorage.removeItem('discountValue');
                    localStorage.removeItem('discountedTotal');
                    localStorage.removeItem('promoCode');
                    $("#discountValue").text("$0.00");
                    $("#grandTotal").text("$" + calculateCartTotal());

                    if (callback) {
                        callback();
                    }
                }
            },
            error: function (xhr, status, error) {
                showMessage("Failed to apply promo code.", "alert-danger", "#cartMessage");
                localStorage.removeItem('discountValue');
                localStorage.removeItem('discountedTotal');
                localStorage.removeItem('promoCode');
                $("#discountValue").text("$0.00");
                $("#grandTotal").text("$" + calculateCartTotal());

                if (callback) {
                    callback();
                }
            }
        });
    }

    function showMessage(message, alertClass, container) {
        var messageDiv = $(container);
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