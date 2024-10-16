// login.js

$(document).ready(function () {
    $('#loginForm').on('submit', function (e) {
        e.preventDefault(); // Prevent the default form submission

        var formData = $(this).serialize(); // Serialize the form data

        $.ajax({
            url: $(this).attr('action'), // The URL to send the request to
            type: 'POST', // The HTTP method to use for the request
            data: formData, // The data to send in the request
            success: function (response) {
                // Handle the success response
                if (response.success) {
                    // Redirect to the desired page or show a success message
                    window.location.href = response.redirectUrl;
                } else {
                    // Show validation errors or other messages
                    $('#loginValidation').text(response.message);
                }
            },
            error: function (xhr, status, error) {
                // Handle the error response
                console.error('Error:', error);
                $('#loginValidation').text('An error occurred while processing your request. Please try again.');
            }
        });
    });

    // Toggle password visibility
    $('.show-pass').on('click', function () {
        var passwordField = $('#loginPassword');
        var passwordFieldType = passwordField.attr('type');
        if (passwordFieldType === 'password') {
            passwordField.attr('type', 'text');
            $('.eye-open').show();
            $('.eye-close').hide();
        } else {
            passwordField.attr('type', 'password');
            $('.eye-open').hide();
            $('.eye-close').show();
        }
    });
});