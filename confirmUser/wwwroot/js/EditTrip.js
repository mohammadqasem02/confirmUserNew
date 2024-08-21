//$(document).ready(function () {
//    $('.edit').on('click', function (e) {
//        e.preventDefault();

//        // Validate the form
//        if ($('#editForm')[0].checkValidity()) {
//            // Submit the form using Ajax
//            $.ajax({
//                url: $('#editForm').attr('action'),
//                method: 'POST',
//                data: $('#editForm').serialize(),
//                success: function (response) {
//                    const swal = Swal.mixin({
//                        customClass: {
//                            confirmButton: "btn btn-primary mx-2",
//                            cancelButton: "btn btn-light"
//                        },
//                        buttonsStyling: false,
//                        timer: 2000
//                    });

//                    if (response && response.success) {
//                        swal.fire({
//                            title: "User Updated!",
//                            text: "The user has been updated successfully.",
//                            icon: "success",
//                            showConfirmButton: false,
//                            timerProgressBar: false
//                        }).then(() => {
//                            window.location.href = `https://localhost:44313/`;
//                        });
//                    } else {
//                        handleFormError(response); // Handle error if 'success' is false or response is invalid
//                    }
//                },
//                error: function (xhr, status, error) {
//                    console.error(xhr.responseText);
//                    handleFormError(); // Handle error for Ajax failure
//                }
//            });
//        } else {
//            // If the form is not valid, show validation errors
//            $('#editForm')[0].reportValidity();
//        }
//    });
//});

//function handleFormError(response) {
//    const swal = Swal.mixin({
//        customClass: {
//            confirmButton: "btn btn-primary mx-2",
//            cancelButton: "btn btn-light"
//        },
//        buttonsStyling: false
//    });

//    swal.fire({
//        title: "Oops!",
//        text: response && response.message ? response.message : "Something went wrong.",
//        icon: "error",
//        showCancelButton: false,
//        showConfirmButton: true
//    });
//}
//function EditMember(memberId) {
//    // Example: Redirect to an edit page
//    window.location.href = `/Home/EditTrip/${memberId}`;
//}
function EditMember(memberId) {
    // Show a confirmation dialog before making the request
    Swal.fire({
        title: 'Are you sure?',
        text: "Do you want to edit this member's trip?",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, edit it!',
        cancelButtonText: 'Cancel'
    }).then((result) => {
        if (result.isConfirmed) {
            // Redirect to the edit page if confirmed
            window.location.href = `/Home/EditTrip/${memberId}`;

            // Perform AJAX call to the server after redirecting
            if ($('#editForm')[0].checkValidity()) {
                // Submit the form using Ajax
                $.ajax({
                    url: $('#editForm').attr('action'),
                    method: 'POST', // Change to 'POST' if your action is a POST
                    success: function (response) {
                        if (response && response.success) {
                            Swal.fire({
                                title: 'Success!',
                                text: 'Trip details loaded successfully.',
                                icon: 'success',
                                confirmButtonText: 'OK'
                            });
                        } else
                            handleFormError(response);
                    },
                    error: function (xhr, status, error) {
                        Swal.fire({
                            title: 'Error!',
                            text: 'Failed to load trip details.',
                            icon: 'error',
                            confirmButtonText: 'OK'
                        });
                    }
                });
            }
        };
    }
