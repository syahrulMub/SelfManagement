function addTaskCategory() {
    var categoryName = $('#categoryName').val();

    $.ajax({
        url: '/TaskCategory/Create',
        method: 'POST',
        data: { taskCategoryName: categoryName },
        success: function (data) {
            swal({
                title: "Success!",
                text: "Success create Task Category!",
                icon: "success",
                button: "Ok",
              });
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
                var inputElement = $('<input type="text" class="form-control" value="' + category.taskCategoryName + '" id="categoryNameInput' + category.taskCategoryId + '">');
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
            swal({
                title: "Success!",
                text: "Success update Task Category!",
                icon: "success",
                button: "Ok",
              });
        },
        error: function (){
            console.log("error");
        }
    })
}

function deleteTaskCategory(taskCategoryId) {
    swal({
        title: "are you sure?",
        text: "once deleted this task category, the Task Works with this Task category will also be deleted",
        icon: "warning",
        buttons: true,
        dangerMode: true,
    })
    .then((willDelete) => {
        if (willDelete){
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
                    swal({
                        title: "Success!",
                        text: "Success delete Task Category!",
                        icon: "success",
                        button: "Ok",
                    }).then(function() {
                        loadTaskCategories();
                    });
                },
                error: function () {
                    console.log("error");
                }
            });
        } else {
            swal("cancel delete task category");
        }
    });
}
