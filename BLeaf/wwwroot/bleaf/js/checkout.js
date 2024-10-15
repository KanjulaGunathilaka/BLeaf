$(document).ready(function () {
    console.log("checkout.js loaded");

    // Function to get all items from the cart
    function getCartItems() {
        return JSON.parse(localStorage.getItem('cart')) || [];
    }

    // Function to calculate the total price of the cart
    function calculateCartTotal() {
        let cart = getCartItems();
        let total = cart.reduce((sum, item) => sum + item.item.price * item.quantity, 0);
        return total.toFixed(2); // Return the total as a string with 2 decimal places
    }

    // Function to load checkout items
    function loadCheckoutItems() {
        let cart = getCartItems();
        let checkoutItemsContainer = $("#checkoutItemsContainer");
        checkoutItemsContainer.empty();

        if (cart.length === 0) {
            checkoutItemsContainer.append('<tr><td colspan="5" class="text-center">No items in the cart.</td></tr>');
        } else {
            $.each(cart, function (index, item) {
                var checkoutItemHtml = `
                    <tr>
                        <td class="product-item-img"><img src="${item.item.imageUrl}" alt="${item.item.name}"></td>
                        <td class="product-item-name">${item.item.name}</td>
                        <td class="product-quantity">${item.quantity}</td>
                        <td class="product-price">$${(item.item.price * item.quantity).toFixed(2)}</td>
                    </tr>
                `;
                checkoutItemsContainer.append(checkoutItemHtml);
            });
        }

        // Update order total
        let total = calculateCartTotal();
        $("#orderSubtotal").text("$" + total);
        $("#orderTotal").text("$" + total);

        // Attach event handler for remove buttons
        $(".remove-item").on("click", function () {
            var cartItemId = $(this).data("cart-item-id");
            removeCartItem(cartItemId);
            loadCheckoutItems(); // Reload checkout items after removal
        });
    }

    // Function to remove an item from the cart
    function removeCartItem(cartItemId) {
        let cart = getCartItems();
        cart = cart.filter(item => item.item.itemId !== parseInt(cartItemId, 10));
        localStorage.setItem('cart', JSON.stringify(cart));
    }

    // Function to gather form data and create order
    function gatherOrderDetails() {
        let cart = getCartItems();
        let orderTotal = calculateCartTotal();

        let orderDetails = {
            user: {
                fullName: $("input[name='dzFirstName']").val() + " " + $("input[name='dzLastName']").val(),
                email: $("input[name='dzEmail']").val(),
                phoneNumber: $("input[name='dzPhoneNumber']").val()
            },
            address: {
                addressLine1: $("input[name='dzOther[Address]']").val(),
                addressLine2: $("input[name='dzOther[Other]']").val(),
                city: $("input[name='dzOther[Town/City]']").val(),
                state: $("input[name='dzOther[State/County]']").val(),
                zipCode: $("input[name='Postcode/Zip']").val(),
                phoneNumber: $("input[name='dzPhoneNumber']").val()
            },
            orderTotal: parseFloat(orderTotal),
            items: cart.map(item => ({
                item: {
                    itemId: item.item.itemId,
                    price: item.item.price
                },
                quantity: item.quantity
            }))
        };

        return orderDetails;
    }

    // Function to place order
    function placeOrder(orderDetails) {
        $.ajax({
            url: '/api/Order',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(orderDetails),
            success: function (response) {
                showMessage("Order placed successfully!", "alert-success");
                // Clear the cart and redirect or update the UI as needed
                localStorage.removeItem('cart');
                window.location.href = '/order-success'; // Redirect to a success page
            },
            error: function (xhr, status, error) {
                showMessage("Failed to place order. Please try again.", "alert-danger");
                console.error("Error placing order:", xhr.responseText);
            }
        });
    }

    // Function to show inline messages
    function showMessage(message, alertClass) {
        var messageDiv = $("#checkoutMessage");
        messageDiv.removeClass();
        messageDiv.addClass("alert " + alertClass);
        messageDiv.text(message);
        messageDiv.show();
        setTimeout(function () {
            messageDiv.hide();
        }, 5000);
    }

    // Event listener for "Place Order Now" button
    $("button[name='submit']").on("click", function (event) {
        event.preventDefault();

        let cart = getCartItems();
        if (cart.length === 0) {
            showMessage("Your cart is empty. Please add items to your cart before placing an order.", "alert-warning");
            return;
        }

        let orderDetails = gatherOrderDetails();
        placeOrder(orderDetails);
    });

    // Initial load of the checkout items
    loadCheckoutItems();
});