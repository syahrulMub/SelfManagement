// Master Customer Management JavaScript

let customersTable;

$(document).ready(function () {
    // Initialize DataTable with AJAX
    customersTable = $('#customersTable').DataTable({
        ajax: {
            url: '/MasterData/GetCustomersData',
            dataSrc: 'data'
        },
        columns: [
            { data: 'customerId' },
            { data: 'userEmail' },
            { data: 'address' },
            { data: 'city' },
            {
                data: null,
                orderable: false,
                render: function (data, type, row) {
                    if (row.latitude && row.longitude) {
                        return `${row.latitude}, ${row.longitude}`;
                    }
                    return '<span class="text-muted">N/A</span>';
                }
            },
            { data: 'orderCount' },
            {
                data: null,
                orderable: false,
                render: function (data, type, row) {
                    return `
                        <button class="btn btn-info btn-sm me-1" onclick="viewCustomer(${row.customerId})" title="View Details">
                            <i class="bi bi-eye"></i>
                        </button>
                        <button class="btn btn-warning btn-sm me-1" onclick="editCustomer(${row.customerId})" title="Edit">
                            <i class="bi bi-pencil"></i>
                        </button>
                        <button class="btn btn-danger btn-sm" onclick="deleteCustomer(${row.customerId})" title="Delete">
                            <i class="bi bi-trash"></i>
                        </button>
                    `;
                }
            }
        ],
        order: [[0, 'desc']],
        pageLength: 10
    });
});

// Create new customer
function createCustomer() {
    Swal.fire({
        icon: 'info',
        title: 'Feature Coming Soon',
        text: 'Create customer modal will be implemented'
    });
}

// View customer details
function viewCustomer(customerId) {
    Swal.fire({
        icon: 'info',
        title: 'Feature Coming Soon',
        text: 'View customer details modal will be implemented'
    });
}

// Edit customer
function editCustomer(customerId) {
    Swal.fire({
        icon: 'info',
        title: 'Feature Coming Soon',
        text: 'Edit customer modal will be implemented'
    });
}

// Delete customer
function deleteCustomer(customerId) {
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        cancelButtonColor: '#3085d6',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: `/Customer/DeleteCustomer/${customerId}`,
                type: 'DELETE',
                success: function (response) {
                    Swal.fire({
                        icon: 'success',
                        title: 'Deleted!',
                        text: 'Customer has been deleted.',
                        timer: 2000,
                        showConfirmButton: false
                    });
                    customersTable.ajax.reload();
                },
                error: function (xhr, status, error) {
                    Swal.fire({
                        icon: 'error',
                        title: 'Error',
                        text: 'Failed to delete customer'
                    });
                }
            });
        }
    });
}
