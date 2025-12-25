// Master Customer Management JavaScript

let customersTable;

$(document).ready(function () {
    // Initialize DataTable with AJAX
    customersTable = $('#customersTable').DataTable({
        ajax: {
            url: '/Admin/Customers/GetData',
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

// Load available users for create modal
function loadAvailableUsersForCustomer() {
    $.ajax({
        url: '/Admin/Customers/GetAvailableUsers',
        type: 'GET',
        success: function (response) {
            const select = $('#createCustUserId');
            select.empty().append('<option value="">-- Select User --</option>');
            response.data.forEach(user => {
                select.append(`<option value="${user.id}" data-phone="${user.phoneNumber || ''}">${user.email} (${user.userName})</option>`);
            });
        },
        error: function () {
            Swal.fire('Error', 'Failed to load available users', 'error');
        }
    });
}

// Auto-fill phone when user is selected
$(document).on('change', '#createCustUserId', function() {
    const phone = $(this).find(':selected').data('phone');
    $('#createCustPhone').val(phone || '');
});

// Create new customer
function createCustomer() {
    loadAvailableUsersForCustomer();
    $('#createCustomerForm')[0].reset();
    $('#createCustomerModal').modal('show');
}

// Submit create customer
function submitCreateCustomer() {
    const userId = $('#createCustUserId').val();
    const address = $('#createCustAddress').val().trim();
    const city = $('#createCustCity').val().trim();
    
    if (!userId || !address || !city) {
        Swal.fire('Validation Error', 'Please fill in all required fields', 'warning');
        return;
    }

    const data = {
        userId: userId,
        phoneNumber: $('#createCustPhone').val(),
        address: address,
        city: city,
        latitude: $('#createCustLat').val() || null,
        longitude: $('#createCustLng').val() || null
    };

    $.ajax({
        url: '/Admin/Customers/Create',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(data),
        success: function (response) {
            $('#createCustomerModal').modal('hide');
            Swal.fire({
                icon: 'success',
                title: 'Success!',
                text: response.message,
                timer: 2000
            });
            customersTable.ajax.reload();
        },
        error: function (xhr) {
            const error = xhr.responseJSON?.error || 'Failed to create customer';
            Swal.fire('Error', error, 'error');
        }
    });
}

// View customer details
function viewCustomer(customerId) {
    $.ajax({
        url: `/Admin/Customers/Get/${customerId}`,
        type: 'GET',
        success: function (data) {
            $('#viewCustEmail').text(data.userEmail || 'N/A');
            $('#viewCustPhone').text('N/A');
            $('#viewCustAddress').text(data.address || 'N/A');
            $('#viewCustCity').text(data.city || 'N/A');
            
            if (data.latitude && data.longitude) {
                $('#viewCustLocation').text(`${data.latitude}, ${data.longitude}`);
            } else {
                $('#viewCustLocation').text('Not set');
            }
            
            $('#viewCustOrders').text('N/A');
            $('#viewCustomerModal').modal('show');
        },
        error: function () {
            Swal.fire('Error', 'Failed to load customer details', 'error');
        }
    });
}

// Edit customer
function editCustomer(customerId) {
    $.ajax({
        url: `/Admin/Customers/Get/${customerId}`,
        type: 'GET',
        success: function (customer) {
            $('#editCustId').val(customer.customerId);
            $('#editCustUserId').val(customer.userId);
            $('#editCustUserEmail').val(customer.userEmail);
            $('#editCustPhone').val('');
            $('#editCustAddress').val(customer.address);
            $('#editCustCity').val(customer.city);
            $('#editCustLat').val(customer.latitude || '');
            $('#editCustLng').val(customer.longitude || '');
            $('#editCustomerModal').modal('show');
        },
        error: function () {
            Swal.fire('Error', 'Failed to load customer data', 'error');
        }
    });
}

// Submit update customer
function submitUpdateCustomer() {
    const customerId = $('#editCustId').val();
    const address = $('#editCustAddress').val().trim();
    const city = $('#editCustCity').val().trim();
    
    if (!address || !city) {
        Swal.fire('Validation Error', 'Please fill in all required fields', 'warning');
        return;
    }

    const data = {
        customerId: parseInt(customerId),
        userId: $('#editCustUserId').val(),
        phoneNumber: $('#editCustPhone').val(),
        address: address,
        city: city,
        latitude: $('#editCustLat').val() || null,
        longitude: $('#editCustLng').val() || null
    };

    $.ajax({
        url: `/Admin/Customers/Update/${customerId}`,
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(data),
        success: function (response) {
            $('#editCustomerModal').modal('hide');
            Swal.fire({
                icon: 'success',
                title: 'Success!',
                text: response.message,
                timer: 2000
            });
            customersTable.ajax.reload();
        },
        error: function (xhr) {
            const error = xhr.responseJSON?.error || 'Failed to update customer';
            Swal.fire('Error', error, 'error');
        }
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
                url: `/Admin/Customers/Delete/${customerId}`,
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
