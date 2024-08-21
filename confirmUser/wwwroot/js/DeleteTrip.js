
//$(document).ready(function () {
//    // Delegate the click event for dynamically created buttons
//    $('#trips').on('click', '.js-delete', function () {
//        // Get the ID from the data attribute
//        const memberId = $(this).data('id');

//        const swal = Swal.mixin({
//            customClass: {
//                confirmButton: "btn btn-danger mx-2",
//                cancelButton: "btn btn-light"
//            },
//            buttonsStyling: false
//        });

//        swal.fire({
//            title: "Are you sure you want to delete this Trip?",
//            text: "You won't be able to revert this!",
//            icon: "warning",
//            showCancelButton: true,
//            confirmButtonText: "Yes, delete it!",
//            cancelButtonText: "No, cancel!",
//            reverseButtons: true
//        }).then((result) => {
//            if (result.isConfirmed) {
//                $.ajax({
//                    url: `https://localhost:44372/api/Trip/DeleteTrip/${memberId}`,
//                    method: "POST", // Using POST for deletion
//                    success: function () {
//                        swal.fire({
//                            title: "Deleted!",
//                            text: "Trip has been deleted.",
//                            icon: "success"
//                        });

//                        // Reload the DataTable
//                        $('#trips').DataTable().ajax.reload();
//                    },
//                    error: function (err) {
//                        swal.fire({
//                            title: "Ooooops!",
//                            text: "Something went wrong.",
//                            icon: "error"
//                        });
//                    }
//                });
//            }
//        });
//    });
//});
function DeleteMember(memberId) {
    const swal = Swal.mixin({
        customClass: {
            confirmButton: "btn btn-danger mx-2",
            cancelButton: "btn btn-light"
        },
        buttonsStyling: false
    });

    swal.fire({
        title: "Are you sure you want to delete this trip?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonText: "Yes, delete it!",
        cancelButtonText: "No, cancel!",
        reverseButtons: true
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: `https://localhost:44372/api/Trip/DeleteTrip/${memberId}`, // Adjust URL if needed
                method: "POST", // Use DELETE method if appropriate
                success: function () {
                    swal.fire({
                        title: "Deleted!",
                        text: "Trip has been deleted.",
                        icon: "success"
                    }).then(() => {
                        // Find the row with the corresponding TripId and remove it
                        $(`#tripRow_${memberId}`).remove();
                    });

                    // Reload the table or update the UI as needed
                    //$('#trips').DataTable().ajax.reload(); // Ensure DataTable is properly initialized
                },
                error: function (xhr, status, error) {
                    swal.fire({
                        title: "Ooooops!",
                        text: "Something went wrong: " + error,
                        icon: "error"
                    });
                }
            });
        }
    });
}

