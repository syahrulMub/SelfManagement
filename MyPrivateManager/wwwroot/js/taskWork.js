$(document).ready(function(){
    $('#CreateTaskWorkModal').on('shown.bs.modal', function () {
        getTaskCategoryData()
        loadEnumTaskPriority('createTaskPriority');
        loadEnumTaskStage('createTaskStage');
    });
    $('#createTaskWorkForm').submit(function(e) {
        e.preventDefault();
        createTaskWork();
    });
});

function getTaskCategoryData() {
    $.ajax({
        url: '/TaskCategory/GetCategories',
        type: 'GET',
        success: function (data) {
            var dropdown = $('#createTaskCategoryId');
            dropdown.empty();

            data.forEach(function (category) {
                var option = $('<option></option>').val(category.taskCategoryId).text(category.taskCategoryName);
                dropdown.append(option);
            });
        },
        error: function () {
            console.log('error processing category');
        }
    });
}

function loadEnumTaskPriority(elementId) {
    var dropdown = $('#' + elementId);
    dropdown.empty();

    var optionCritical = $('<option></option>').val('0').text('Critical').addClass('option-critical');
    dropdown.append(optionCritical);

    var optionHigh = $('<option></option>').val('1').text('High').addClass('option-high');
    dropdown.append(optionHigh);

    var optionMedium = $('<option></option>').val('2').text('Medium').addClass('option-medium');
    dropdown.append(optionMedium);

    var optionLow = $('<option></option>').val('3').text('Low').addClass('option-low');
    dropdown.append(optionLow);
}

function loadEnumTaskStage(elementId) {
    var dropdown = $('#' + elementId);
    dropdown.empty();

    var optionNotStarted = $('<option></option>').val('NotStarted').text('Not Started').addClass('option notstarted');
    dropdown.append(optionNotStarted);

    var optionInProgress = $('<option></option>').val('InProgress').text('In Progress').addClass('option inprogress');
    dropdown.append(optionInProgress);

    var optionReviewing = $('<option></option>').val('Reviewing').text('Reviewing').addClass('optionreviewing');
    dropdown.append(optionReviewing);

    var optionCompleted = $('<option></option>').val('Completed').text('Completed').addClass('option completed');
    dropdown.append(optionCompleted);
}

function createTaskWork() {
    var description = $('#createTaskDescription').val();
    var stage = $('#createTaskStage').val();
    var priority = $('#createTaskPriority').val();
    var dueDate = $('#createTaskDueDate').val();
    var taskCategoryId = $('#createTaskCategoryId').val();
    $.ajax({
        url: '/TaskWork/Create',
        type: 'POST',
        data: {
            Description: description,
            TaskStage: stage,
            TaskPriority: priority,
            DueDate: dueDate,
            TaskCategoryId: taskCategoryId,
        },
        success: function () {
            $('#CreateTaskWorkModal').modal('hide');
            location.reload();
        },
        error: function () {
            console.log('Error creating task work');
        }
    });
}

function setFormattedDate(inputElement) {
    var originalDate = inputElement.getAttribute('data-original-date');
    var formattedDate = new Date(originalDate).toLocaleDateString('en-GB', {
        day: 'numeric',
        month: 'short',
        year: 'numeric'
    }).split(' ').join('-');

    inputElement.value = formattedDate;
}

function deleteTaskWork(taskWorkId){
    swal({
        title: "are you sure?",
        text: "delete this task Work?",
        icon: "warning",
        buttons: true,
        dangerMode: true,
    })
    .then((willDelete) => {
        if (willDelete){
            $.ajax({
                url: '/TaskWork/Delete/' + taskWorkId,
                type: 'DELETE',
                data : {taskWorkId: taskWorkId},
                success: function () {
                    swal({
                        title: "Success!",
                        text: "Success delete Task Work!",
                        icon: "success",
                        button: "Ok",
                    }).then(function() {

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

function updateTaskWork(taskWorkId){
    description = $('#description-'+taskWorkId).val();
    dueDate = $('#dueDate-'+taskWorkId).val();
    taskPriority = $('#priority-'+taskWorkId).val();
    taskStage = $('#stage-'+taskWorkId).val();
    console.log( description,dueDate, taskPriority, taskStage );
    $.ajax({
        url: '/TaskWork/Update/' + taskWorkId,
        type: 'POST',
        data: {taskWorkId: taskWorkId, Description: description, DueDate: dueDate, TaskPriority: taskPriority, TaskStage: taskStage},
        success: function (){
            Swal.fire({
                icon: 'success',
                title: 'Succes!',
                text: "Task Work Success Updated!",
                showConfirmButton: false,
                timer: 2000
              })
        },
        error: function(error){
            console.log(error);
        }
    })
}