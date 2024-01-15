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
                text: "Success delete Task Category!",
                showConfirmButton: false,
                timer: 2000
            })
            
            loadTaskCategories();
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
            $('#categoryList').empty();
            data.forEach(function (category) {
                var listItemElement = $('<div class="category-item"></div>');
        
                var formDiv = $('<div class="form-div"></div>');
                var inputElement = $('<input type="text" class="transparant form-control" value="' + category.taskCategoryName + '" id="categoryNameInput' + category.taskCategoryId + '">');
                formDiv.append(inputElement);
        
                var buttonsDiv = $('<div class="buttons-div"></div>');
        
                var saveButton = $('<button class="btn btn-success ri-save-3-line btn-sm"></button>');
                saveButton.click(function () {
                    updateTaskCategory(category.taskCategoryId);
                });
                buttonsDiv.append(saveButton);
        
                var deleteButton = $('<button class="btn btn-danger ri-delete-bin-line btn-sm"></button>');
                deleteButton.click(function () {
                    deleteTaskCategory(category.taskCategoryId);
                });
                buttonsDiv.append(deleteButton);
        
                listItemElement.append(formDiv);
                listItemElement.append(buttonsDiv);
        
                $('#categoryList').append(listItemElement);
            });
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
                    
                },
                error: function(){
                    console.log("error occurred during bulkDeleteByTaskCategory");
                }
            })
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
                    });
                },
                error: function () {
                    console.log("error");
                }
            });
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
