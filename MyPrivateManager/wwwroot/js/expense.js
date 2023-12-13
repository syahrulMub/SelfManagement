function addExpense(){
    $('#createExpenseModalLabel').text('Create Expense');
    $('#createExpenseAmount').val('');
    getCategoryData();
    $('#createExpenseDate').val('');
    $('#createExpenseDescription').val('');
    $('#createExpenseModal').modal('show');
}

function getCategoryData(){
    $.ajax({
        url: '/Category/Categories',
        type: 'GET',
        success: function(data){
            var dropdown = $('#createExpenseCategoryId');
            dropdown.empty();

            data.forEach(function(category){
                var option = ($('<option></option>').val(category.categoryId).text(category.categoryName));
                dropdown.append(option);
            });
        },
        error: function(){
            console.log('error prosessing category');
        }
    });
}

function createExpense(){
    var amount = $('#createExpenseAmount').val();
    var categoryId = $('#createExpenseCategoryId').val();
    var date = $('#createExpenseDate').val();
    var description = $('#createExpenseDescription').val();

    $.ajax({
        url: '/Expense/CreateExpense',
        type: 'POST',
        data: {Amount : amount, CategoryId: categoryId, Date: date, Description: description},
        success: function(){
            $('#createExpenseModal').modal('hide');
            location.reload();
        },
        error: function(){
            console.log('Error creating expense');
        }
    })
}