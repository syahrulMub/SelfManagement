function addCategory() {
    $('#createCategoryModalLabel').text('Add Category');
    $('#createCategoryName').val('');
    $('#createCategoryModal').modal('show');
}

function createCategory(){
    var CategoryData = {
        CategoryName : $('#createCategoryName').val()
    };
    $.ajax({
        url: '/Category/CreateCategory',
        type: 'POST',
        data: CategoryData,
        success: function(){
            $('#createCategoryModal').modal('hide');
            location.reload();
        },
        error: function(){
            console.log('Error create category');
        }
    });
}

function editCategory(categoryId){
    $.ajax({
        url: '/Category/GetCategory/' + categoryId,
        type: 'GET',
        success: function(data){
            $('#editCategoryModalLabel').text('Edit Category');
            $('#editCategoryId').val(data.categoryId);
            $('#editCategoryName').val(data.categoryName);
            $('#editCategoryModal').modal('show');
        },
        error: function(){
            console.log('Error loading category');
        }
    });
}

function updateCategory(){
    var categoryId = $('#editCategoryId').val();
    var categoryData = {
        CategoryName: $('#editCategoryName').val()
    };
    $.ajax({
        url: '/Category/UpdateCategory/' + categoryId,
        type: 'POST',
        data: categoryData,
        success: function(){
            $('#editCategoryModal').modal('hide');
            location.reload();
        },
        error: function(){
            console.log('Error updating data');
        }
    });
}