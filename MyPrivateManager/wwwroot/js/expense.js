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

function detailExpense(expenseId){
    $.ajax({
        url: '/Expense/GetExpense/' + expenseId,
        type: 'GET',
        success : function(data){
            $('#detailExpenseModalLabel').text('Details Expense');
            $('#detailExpenseId').val(data.expenseId);
            $('#detailExpenseAmount').val(data.amount);
            $('#detailExpenseCategory').val(data.category.categoryName);
            var localDate = new Date(data.date);
            var offset = localDate.getTimezoneOffset();
            localDate.setMinutes(localDate.getMinutes() - offset);
            var formattedDate = localDate.toISOString().split('T')[0];
            $('#detailExpenseDate').val(formattedDate);
            $('#detailExpenseDescription').val(data.description);
            $('#detailExpenseModal').modal('show');
        },
        error: function(error){
            console.error('Error prosessing data'+error);
        }
    });
}

function editExpense(expenseId){
    $.ajax({
        url: '/Expense/GetExpense/' + expenseId,
        type: 'GET',
        success: function(data){
            $('#editExpenseModalLabel').text('Edit Expense');
            $('#editExpenseId').val(data.expenseId);
            $('#editExpenseAmount').val(data.amount);
            getCategoryForEdit(data.categoryId);
            var localDate = new Date(data.date);
            var offset = localDate.getTimezoneOffset();
            localDate.setMinutes(localDate.getMinutes() - offset);
            var formattedDate = localDate.toISOString().split('T')[0];
            $('#editExpenseDate').val(formattedDate);
            $('#editExpenseDescription').val(data.description);
            $('#editExpenseModal').modal('show');
        },
        error: function(error){
            console.log('error prosessing data :' + error);
        }
    });
}

function updateExpense(){
    var expenseId = $('#editExpenseId').val();
    var amount = $('#editExpenseAmount').val();
    var categoryId = $('#editCategoryIdForExpense').val();
    var date = $('#editExpenseDate').val();
    var description = $('#editExpenseDescription').val();
    $.ajax({
        url: '/Expense/UpdateExpense/' + expenseId,
        type: 'POST',
        data: {Amount : amount, ExpenseId: expenseId, CategoryId: categoryId, Date: date, Description: description},
        success: function(){
            Swal.fire({
                icon: 'success',
                title: 'Succes!',
                text: "database expense success updated!",
                showConfirmButton: false,
                timer: 2000
              })
            $('#editExpenseModal').modal('hide');
            // location.reload();
        },
        error: function(error){
            console.log('Error: ' + error);
        }
    });
}

function getCategoryForEdit(categoryId) {
    $.ajax({
        url: '/Category/Categories',
        type: 'GET',
        success: function (data) {
            var dropdown = $('#editCategoryIdForExpense');
            data.forEach(function (category) {
                dropdown.append($('<option></option>').val(category.categoryId).text(category.categoryName));
            });
            dropdown.val(categoryId);
        },
        error: function () {
            console.error('Failed to fetch data');
        }
    });
}

function deleteExpense( expenseId){
    Swal.fire({
        title: "are you sure?",
        text: "once deleted, the data will remove from your account",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    })
    .then((willDelete) => {
        if (willDelete.isConfirmed){
            $.ajax({
                url: '/Expense/DeleteExpense/' + expenseId,
                type: 'DELETE', 
                success: function(){
                    Swal.fire({
                        icon: "success",
                        title: "Expense deleted",
                        showConfirmButton: false,
                        timer: 2000
                    });
                },
                error: function(error){
                    console.log('error delete data : ' + error);
                }
            });
        } else {
            Swal.fire("cancel delete expense");
        }
    });
}

function migrateExpense(categoryIdFrom){
    $('#migrateExpenseModalLabel').text('Migrate Expense');
    populateCheckboxForMigrateExpense(categoryIdFrom);
    $('#migrateExpenseModal').data('categoryIdFrom', categoryIdFrom).modal('show');
}

function populateCheckboxForMigrateExpense(categoryIdFrom) {
    $.ajax({
        url: '/Category/Categories',
        type: 'GET',
        success: function(data){
            var container = $('#radioMigrateExpense');
            container.empty();
            function radioButton(category){
                return `
                <div class="form-check">
                    <input class="form-check-input" type="radio" name="categoryOption" id="categoryOption${category.categoryId}" value="${category.categoryId}">
                    <label class="form-check-label" for="categoryOption${category.categoryId}"> ${category.categoryName}</label>
                </div>
                `;
            }

            data.forEach(function(category){
                if (category.categoryId !== categoryIdFrom){
                    container.append(radioButton(category));
                }
            })
        }
    });
}

function submitExpenseMigration(){
    var categoryIdFrom = $('#migrateExpenseModal').data('categoryIdFrom');
    var categoryIdTo = $('input[value]:checked').val();
    $.ajax({
        url: '/Expense/MigrateExpense',
        type: 'POST',
        data: {categoryIdFrom : categoryIdFrom, categoryIdTo: categoryIdTo},
        success: function(){
            console.log('Success migrate expense');
        },
        error: function(){
            console.log('error migrate expense');
        }
    });
    $('#migrateExpenseModal').modal('hide');
    deleteCategory(categoryIdFrom);
}

function deleteCategory(categoryId){
    var isConfirmed = confirm('Are you sure you want to delete this category?');
    if (isConfirmed){
        $.ajax({
            url: '/Category/DeleteCategory/' + categoryId,
            type: 'DELETE',
            success: function(){
                location.reload();
            },
            error: function(error){
                console.log(error);
            }
        })
    }
}

function updateChart(filter) {
    switch (filter) {
        case 'daily':
            if (myChart) {
                myChart.destroy();
            }
            getExpenseChartDaily();
            break;
        case 'weekly':
            if (myChart) {
                myChart.destroy();
            }
            getExpenseChartWeekly();
            break;
        case 'monthly':
            if (myChart) {
                myChart.destroy();
            }
            getExpenseChartMonthly();
            break;
        default:
            getExpenseChartMonthly();
            break;
    }
}

function getExpenseChartMonthly(){
    $.ajax({
        url: '/Expense/MonthlyChart',
        type: 'GET',
        success: function(data){
            myChart = new Chart(document.querySelector('#lineChart'),{
                type: 'line',
                data: {
                    labels: ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'],
                    datasets: [{
                        label: 'Expense Chart Monthly',
                        data: data,
                        fill: true,
                        borderColor: 'rgb(75,192,192)',
                        tension: 0.1
                    }]
                },
                options: {
                    scales: {
                        y: {
                            beginAtZero: true
                        }
                    }
                }
            });
        },
        error: function(error){
            console.log('Error creating expense chart' + error);
        }
    });
}

function getExpenseChartDaily(){
    $.ajax({
        url: '/Expense/ExpenseDailyChart',
        type: 'GET',
        success: function(data){

            myChart = new Chart(document.querySelector('#lineChart'), {
                type: 'line',
                data: {
                    labels: ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday'],
                    datasets: [{
                        label: 'Expense Chart Daily',
                        data: data,
                        fill: true,
                        borderColor: 'rgb(0,206,209)',
                        tension: 0.1
                    }]
                },
                options: {
                    scales: {
                        y: {
                            beginAtZero : true,
                        }
                    }
                }
            });
        },
        error: function(error){
            console.log('error' + error);
        }
    });
}

function getExpenseChartWeekly(){
    $.ajax({
        url: '/Expense/ExpenseWeeklyChart',
        type: 'GET',
        success: function(data){

            myChart = new Chart(document.querySelector('#lineChart'), {
                type: 'line',
                data: {
                    labels: ['Week 1', 'Week 2', 'Week 3', 'Week 4', 'Week 5'],
                    datasets: [{
                        label: 'Expense Chart Weekly',
                        data: data,
                        fill: true,
                        borderColor: 'rgb(0,206,29)',
                        tension: 0.1
                    }]
                },
                options: {
                    scales: {
                        y: {
                            beginAtZero : true,
                        }
                    }
                }
            });
        },
        error: function(error){
            console.log('error' + error);
        }
    });
}

function getExpenseChartByCategory(){
    $.ajax({
        url: '/Expense/ExpenseByCategory',
        type: 'GET',
        success: function(data){
            var indicatorData = [];
            var totalByCategory =[];
            data.forEach(function(item){
                indicatorData.push({
                    name: item.categoryName,
                    max: item.maxSum + (item.maxSum/5)
                });
                totalByCategory.push(item.total);
            });
            echarts.init(document.querySelector("#incomeBySourceChart")).setOption({
                legend: {
                    data: ['Chart by Category']
                },
                radar: {
                    indicator: indicatorData
                },
                series: [{
                    name: 'Chart by Category',
                    type: 'radar',
                    data: [{
                        value: totalByCategory,
                        name: 'Category chart'
                    }]
                }]
            });
        }
    });
}