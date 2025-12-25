// Master Technician Management JavaScript

let techniciansTable;

$(document).ready(function () {
    // Initialize DataTable with AJAX
    techniciansTable = $('#techniciansTable').DataTable({
        ajax: {
            url: '/Admin/Technicians/GetData',
            dataSrc: 'data'
        },
        columns: [
            { data: 'technicianId' },
            { data: 'fullName' },
            { data: 'phone' },
            {
                data: 'isActive',
                render: function (data, type, row) {
                    if (data) {
                        return '<span class="badge bg-success">Active</span>';
                    } else {
                        return '<span class="badge bg-danger">Inactive</span>';
                    }
                }
            },
            {
                data: 'avgRating',
                render: function (data, type, row) {
                    return data.toFixed(2);
                }
            },
            { data: 'completedJobs' },
            { data: 'orderCount' },
            {
                data: null,
                orderable: false,
                render: function (data, type, row) {
                    return `
                        <button class="btn btn-info btn-sm me-1" onclick="viewTechnician(${row.technicianId})" title="View Details">
                            <i class="bi bi-eye"></i>
                        </button>
                        <button class="btn btn-warning btn-sm me-1" onclick="editTechnician(${row.technicianId})" title="Edit">
                            <i class="bi bi-pencil"></i>
                        </button>
                        <button class="btn btn-danger btn-sm" onclick="deleteTechnician(${row.technicianId})" title="Delete">
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
function loadAvailableUsersForTechnician() {
    $.ajax({
        url: '/Admin/Technicians/GetAvailableUsers',
        type: 'GET',
        success: function (response) {
            const select = $('#createTechUserId');
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
$(document).on('change', '#createTechUserId', function() {
    const phone = $(this).find(':selected').data('phone');
    $('#createTechPhone').val(phone || '');
});

// Create new technician
function createTechnician() {
    loadAvailableUsersForTechnician();
    $('#createTechnicianForm')[0].reset();
    $('#createTechActive').prop('checked', true);
    $('#createTechnicianModal').modal('show');
}

// Submit create technician
function submitCreateTechnician() {
    const userId = $('#createTechUserId').val();
    
    if (!userId) {
        Swal.fire('Validation Error', 'Please select a user', 'warning');
        return;
    }

    const data = {
        userId: userId,
        phoneNumber: $('#createTechPhone').val(),
        isActive: $('#createTechActive').is(':checked')
    };

    $.ajax({
        url: '/Admin/Technicians/Create',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(data),
        success: function (response) {
            $('#createTechnicianModal').modal('hide');
            Swal.fire({
                icon: 'success',
                title: 'Success!',
                text: response.message,
                timer: 2000
            });
            techniciansTable.ajax.reload();
        },
        error: function (xhr) {
            const error = xhr.responseJSON?.error || 'Failed to create technician';
            Swal.fire('Error', error, 'error');
        }
    });
}

// View technician details
function viewTechnician(technicianId) {
    $.ajax({
        url: `/Admin/Technicians/Get/${technicianId}`,
        type: 'GET',
        success: function (data) {
            $('#viewTechName').text(data.fullName || 'N/A');
            $('#viewTechEmail').text(data.fullName || 'N/A');
            $('#viewTechPhone').text(data.phone || 'N/A');
            $('#viewTechStatus').html(data.isActive ? 
                '<span class="badge bg-success">Active</span>' : 
                '<span class="badge bg-danger">Inactive</span>');
            $('#viewTechRating').text(data.avgRating.toFixed(1) + ' ⭐');
            $('#viewTechJobs').text(data.completedJobs);
            $('#viewTechOrders').text('N/A');
            $('#viewTechnicianModal').modal('show');
        },
        error: function () {
            Swal.fire('Error', 'Failed to load technician details', 'error');
        }
    });
}

// Edit technician
function editTechnician(technicianId) {
    $.ajax({
        url: `/Admin/Technicians/Get/${technicianId}`,
        type: 'GET',
        success: function (tech) {
            $('#editTechId').val(tech.technicianId);
            $('#editTechUserId').val(tech.userId);
            $('#editTechUserEmail').val(tech.fullName);
            $('#editTechPhone').val(tech.phone || '');
            $('#editTechActive').prop('checked', tech.isActive);
            $('#editTechRating').text(tech.avgRating.toFixed(1));
            $('#editTechJobs').text(tech.completedJobs);
            $('#editTechnicianModal').modal('show');
        },
        error: function () {
            Swal.fire('Error', 'Failed to load technician data', 'error');
        }
    });
}

// Submit update technician
function submitUpdateTechnician() {
    const technicianId = $('#editTechId').val();
    
    const data = {
        technicianId: parseInt(technicianId),
        userId: $('#editTechUserId').val(),
        phoneNumber: $('#editTechPhone').val(),
        isActive: $('#editTechActive').is(':checked')
    };

    $.ajax({
        url: `/Admin/Technicians/Update/${technicianId}`,
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(data),
        success: function (response) {
            $('#editTechnicianModal').modal('hide');
            Swal.fire({
                icon: 'success',
                title: 'Success!',
                text: response.message,
                timer: 2000
            });
            techniciansTable.ajax.reload();
        },
        error: function (xhr) {
            const error = xhr.responseJSON?.error || 'Failed to update technician';
            Swal.fire('Error', error, 'error');
        }
    });
}

// Delete technician
function deleteTechnician(technicianId) {
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
                url: `/Admin/Technicians/Delete/${technicianId}`,
                type: 'DELETE',
                success: function (response) {
                    Swal.fire({
                        icon: 'success',
                        title: 'Deleted!',
                        text: 'Technician has been deleted.',
                        timer: 2000,
                        showConfirmButton: false
                    });
                    techniciansTable.ajax.reload();
                },
                error: function (xhr, status, error) {
                    Swal.fire({
                        icon: 'error',
                        title: 'Error',
                        text: 'Failed to delete technician'
                    });
                }
            });
        }
    });
}
