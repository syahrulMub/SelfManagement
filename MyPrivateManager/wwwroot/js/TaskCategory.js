function addTaskCategory() {
    var categoryName = $('#categoryName').val();

    $.ajax({
        url: '/TaskCategory/Create',
        method: 'POST',
        data: { taskCategoryName: categoryName },
        success: function (data) {
            swal.fire({
                icon: "success",
                title: "Success!",
                text: "Success add Task Category!",
                showConfirmButton: false,
                timer: 2000
            });
            
            loadTaskCategories();
            loadTaskWorkTable(); // Sync main table
            $('#categoryName').val('');
        },
        error: function (error) {
            console.error('Error adding Task Category: ', error);
        }
    });
}

function loadTaskCategories() {
    $.ajax({
        url: '/TaskCategory/GetCategories',
        method: 'GET',
        success: function (data) {
            $('#categoryList').html(data); // Inject HTML partial
        },
        error: function (error) {
            console.error('Error getting Task Categories: ', error);
        }
    });
}

$('#TaskCategoryModal').on('shown.bs.modal', function () {
    loadTaskCategories();
});

function updateTaskCategory(taskCategoryId){

    var updatedCategoryName = $('#categoryNameInput' + taskCategoryId).val();
    $.ajax({
        url: '/TaskCategory/Update/' + taskCategoryId,
        type: 'POST',
        data: {taskCategoryId:taskCategoryId ,TaskCategoryName: updatedCategoryName},
        success: function () {
            swal.fire({
                icon: "success",
                title: "Success!",
                text: "Success update Task Category!",
                showConfirmButton: false,
                timer: 2000
              });
            loadTaskWorkTable(); // Sync main table
        },
        error: function (){
            console.log("error");
        }
    })
}

function deleteTaskCategory(taskCategoryId) {
    swal.fire({
        title: "are you sure?",
        text: "once deleted this task category, the Task Works with this Task category will also be deleted",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    })
    .then((willDelete) => {
        if (willDelete.isConfirmed){
            $.ajax({
                url: '/TaskWork/bulkDeleteByTaskCategory/' + taskCategoryId,
                type: 'DELETE',
                data: {taskCategoryId: taskCategoryId},
                success: function() {
                    // After bulk delete, delete category
                     $.ajax({
                        url: '/TaskCategory/Delete/' + taskCategoryId,
                        type: 'DELETE',
                        data: {taskCategoryId: taskCategoryId},
                        success: function () {
                            swal.fire({
                                icon: "success",
                                title: "Success!",
                                text: "Success delete Task Category!",
                                showConfirmButton: false,
                                timer: 2000
                            }).then(function() {
                                loadTaskCategories();
                                loadTaskWorkTable(); // Sync main table
                            });
                        },
                        error: function () {
                            console.log("error deleting category");
                        }
                    });
                },
                error: function(){
                    console.log("error occurred during bulkDeleteByTaskCategory");
                }
            })
           
        } else {
            Swal.fire({
                icon: 'info',
                title: 'canceled',
                text: 'you canceled for deleting task category',
                showConfirmButton: false,
                timer: 2000
              })
        }
    });
}
