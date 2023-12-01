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