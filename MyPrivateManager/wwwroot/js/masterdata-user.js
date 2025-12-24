// Master User Management JavaScript

let usersTable;
let availableRoles = [];

$(document).ready(function () {
    // Initialize DataTable with AJAX
    usersTable = $('#usersTable').DataTable({
        ajax: {
            url: '/MasterData/GetUsersData',
            dataSrc: 'data'
        },
        columns: [
            { data: 'email' },
            { data: 'userName' },
            {
                data: 'roles',
                render: function (data, type, row) {
                    if (!data || data.length === 0) {
                        return '<span class="badge bg-secondary">No Roles</span>';
                    }
                    return data.map(role => 
                        `<span class="badge bg-primary me-1">${role}</span>`
                    ).join('');
                }
            },
            {
                data: null,
                orderable: false,
                render: function (data, type, row) {
                    return `<button class="btn btn-warning btn-sm" onclick="editUserRole('${row.id}')">
                                <i class="bi bi-pencil"></i> Edit Roles
                            </button>`;
                }
            }
        ],
        order: [[0, 'asc']],
        pageLength: 10
    });

    // Load available roles
    loadAvailableRoles();
});

// Load available roles from database
function loadAvailableRoles() {
    $.ajax({
        url: '/MasterData/GetAvailableRoles',
        type: 'GET',
        success: function (response) {
            availableRoles = response.data;
        },
        error: function (xhr, status, error) {
            console.error('Error loading roles:', error);
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: 'Failed to load available roles'
            });
        }
    });
}

// Open edit user role modal
function editUserRole(userId) {
    $.ajax({
        url: `/MasterData/GetUser?userId=${userId}`,
        type: 'GET',
        success: function (user) {
            $('#editUserId').val(user.id);
            $('#editUserEmail').text(user.email);

            // Generate role checkboxes
            const container = $('#rolesCheckboxContainer');
            container.empty();
            
            availableRoles.forEach(role => {
                const isChecked = user.roles && user.roles.includes(role);
                const checkboxHtml = `
                    <div class="form-check">
                        <input class="form-check-input" type="checkbox" 
                               value="${role}" id="role_${role}" 
                               name="roles" ${isChecked ? 'checked' : ''}>
                        <label class="form-check-label" for="role_${role}">
                            ${role}
                        </label>
                    </div>
                `;
                container.append(checkboxHtml);
            });

            // Show modal
            const modal = new bootstrap.Modal(document.getElementById('editUserRoleModal'));
            modal.show();
        },
        error: function (xhr, status, error) {
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: 'Failed to load user data'
            });
        }
    });
}

// Submit update user role
function submitUpdateUserRole() {
    const userId = $('#editUserId').val();
    const selectedRoles = [];
    
    $('input[name="roles"]:checked').each(function () {
        selectedRoles.push($(this).val());
    });

    const data = {
        userId: userId,
        roles: selectedRoles
    };

    $.ajax({
        url: '/MasterData/UpdateUserRole',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(data),
        success: function (response) {
            Swal.fire({
                icon: 'success',
                title: 'Success!',
                text: response.message || 'User roles updated successfully',
                timer: 2000,
                showConfirmButton: false
            });

            // Close modal
            const modal = bootstrap.Modal.getInstance(document.getElementById('editUserRoleModal'));
            modal.hide();

            // Reload DataTable
            usersTable.ajax.reload();
        },
        error: function (xhr, status, error) {
            let errorMessage = 'Failed to update user roles';
            if (xhr.responseJSON && xhr.responseJSON.error) {
                errorMessage = xhr.responseJSON.error;
            }
            
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: errorMessage
            });
        }
    });
}
