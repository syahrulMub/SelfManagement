// Master Technician Management JavaScript

let techniciansTable;

$(document).ready(function () {
    // Initialize DataTable with AJAX
    techniciansTable = $('#techniciansTable').DataTable({
        ajax: {
            url: '/MasterData/GetTechniciansData',
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

// Create new technician
function createTechnician() {
    Swal.fire({
        icon: 'info',
        title: 'Feature Coming Soon',
        text: 'Create technician modal will be implemented'
    });
}

// View technician details
function viewTechnician(technicianId) {
    Swal.fire({
        icon: 'info',
        title: 'Feature Coming Soon',
        text: 'View technician details modal will be implemented'
    });
}

// Edit technician
function editTechnician(technicianId) {
    Swal.fire({
        icon: 'info',
        title: 'Feature Coming Soon',
        text: 'Edit technician modal will be implemented'
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
                url: `/Technician/DeleteTechnician/${technicianId}`,
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
