function addIncome() {
    // Menampilkan modal tambah data
    $('#createIncomeModalLabel').text('Add Income');
    $('#amount').val('');
    getSourceData();
    $('#date').val('');
    $('#description').val('');
    $('#createIncomeModal').modal('show');
}

function getSourceData() {
    $.ajax({
        url: '/Source/GetSources',
        type: 'GET',
        success: function (data) {
            populateSourceDropdown(data);
        },
        error: function () {
            console.error('failed to fetch data');
        }
    });
}
function populateSourceDropdown(sourceData) {
    var dropdown = $('#sourceId');
    dropdown.empty();

    sourceData.forEach(function (source) {
        var option = ($('<option></option>').val(source.sourceId).text(source.sourceName));
        dropdown.append(option);
    });
}

function getSourceDataForEdit(incomeId) {
    $.ajax({
        url: '/Source/GetSources',
        type: 'GET',
        success: function (data) {
            populateSourceDropdownForEdit(data, incomeId);
        },
        error: function () {
            console.error('Failed to fetch data');
        }
    });
}
function populateSourceDropdownForEdit(sourceData, incomeId) {
    var dropdown = $('#editSourceIdForIncome');

    dropdown.empty();

    sourceData.forEach(function (source) {
        dropdown.append($('<option></option>').val(source.sourceId).text(source.sourceName));
    });

    dropdown.data('incomeId', incomeId);
}


function editIncome(incomeId) {
    $.ajax({
        url: '/Income/GetIncome/' + incomeId,
        type: 'GET',
        success: function (data) {
            $('#editIncomeModalLabel').text('Edit Income');
            $('#editIncomeId').val(data.incomeId);
            $('#editIncomeAmount').val(data.amount);
            getSourceDataForEdit(data.incomeId);
            var localDate = new Date(data.date);
            var offset = localDate.getTimezoneOffset();
            localDate.setMinutes(localDate.getMinutes() - offset);
            var formattedDate = localDate.toISOString().split('T')[0];
            $('#editIncomeDate').val(formattedDate);
            $('#editIncomeDescription').val(data.description);
            $('#editIncomeModal').modal('show');
        },
        error: function () {
            console.error('Failed to fetch income details');
        }
    });
}

function createIncome() {
    var amount = $('#amount').val();
    var sourceId = $('#sourceId').val();
    var date = $('#date').val();
    var description = $('#description').val();

    // validate input
    if (!amount || !sourceId || !date) {
        alert('Please fill in all fields');
        return;
    }

    $.ajax({
        url: '/Income/CreateIncome',
        type: 'POST',
        data: { Amount: amount, SourceId: sourceId, Date: date, Description: description },
        success: function () {
            location.reload();
        },
        error: function () {
            console.error('Failed to save income');
        }
    });

    $('#createIncomeModal').modal('hide');
}

function updateIncome() {
    var incomeId = $('#editIncomeId').val();
    var amount = $('#editIncomeAmount').val();
    var sourceId = $('#editSourceIdForIncome').val();
    var date = $('#editIncomeDate').val();
    var description = $('#editIncomeDescription').val();

    if (!amount || !sourceId || !date) {
        alert('Please fill in all fields');
        return;
    }

    $.ajax({
        url: '/Income/UpdateIncome/' + incomeId,
        type: 'POST',
        data: { Amount: amount, SourceId: sourceId, Date: date, Description: description },
        success: function () {
            location.reload();
        },
        error: function () {
            console.log('failder to update income');
        }
    });
}

function deleteIncome(incomeId) {
    var validation = confirm("Are you sure you want to delete this income?");
    if (validation) {
        $.ajax({
            url: '/Income/DeleteIncome/' + incomeId,
            type: 'DELETE',
            success: function () {
                location.reload();
            },
            error: function () {
                console.error('Failed to delete income');
            }
        });
    }
}

// function truncateDescription(description, maxLength) {
//     if (description.length > maxLength) {
//         return description.substring(0, maxLength) + '...';
//     }
//     return description;
// }