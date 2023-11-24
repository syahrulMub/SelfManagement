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

function getSourceDataForEdit(sourceId) {
    $.ajax({
        url: '/Source/GetSources',
        type: 'GET',
        success: function (data) {
            var dropdown = $('#editSourceIdForIncome');
            data.forEach(function (source) {
                dropdown.append($('<option></option>').val(source.sourceId).text(source.sourceName));
            });
            dropdown.val(sourceId);
        },
        error: function () {
            console.error('Failed to fetch data');
        }
    });
}

function editIncome(incomeId) {
    $.ajax({
        url: '/Income/GetIncome/' + incomeId,
        type: 'GET',
        success: function (data) {
            $('#editIncomeModalLabel').text('Edit Income');
            $('#editIncomeId').val(data.incomeId);
            $('#editIncomeAmount').val(data.amount);
            getSourceDataForEdit(data.sourceId);
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

function migrateIncome(sourceIdFrom){
    $('#migrateIncomeModalLabel').text('Migrate Income');
    populateCheckboxForMigration(sourceIdFrom);
    $('#migrateIncomeModal').data('sourceIdFrom', sourceIdFrom).modal('show');
}

function populateCheckboxForMigration(sourceIdFrom){
    $.ajax({
        url: '/Source/GetSources',
        type: 'GET',
        success: function (data) {
            var container = $('#checkboxContainer');
            container.empty();

            data.forEach(function (source){
                if (source.sourceId !== sourceIdFrom){
                    
                    var radioInput = $('<input>')
                        .attr('type','radio')
                        .attr('name', 'sourceOption')
                        .attr('id', 'sourceOption' + source.SourceId)
                        .val(source.sourceId);
    
                    var label = $('<label>').attr('for', 'sourceOption' + source.sourceId).text(source.sourceName);
    
                    container.append(radioInput);
                    container.append(label);
                }
            });
            // data.forEach(function (source){
            //     if (source.sourceId !== sourceIdFrom){
            //         var option = $('<option></option>')
            //             .val(source.sourceId)
            //             .text(source.sourceName);
            
            //         container.append(option);
            //     }
            // });
        },
        error: function () {
            console.error('Failed to fetch data');
        }
    });
}

function submitMigration(){
    var sourceIdFrom = $('#migrateIncomeModal').data('sourceIdFrom');
    var sourceIdTo = $('input[name="sourceOption"]:checked').val();
    $.ajax({
        url : '/Source/MigrateIncome',
        type : 'POST',
        data : {sourceIdFrom : sourceIdFrom, sourceIdTo : sourceIdTo},
        success : function(response){
            if (response === 'success'){
                console.log('success migrate data');
            } else {
                console.error('migrate failed');
            }
        },
        error : function(){
            console.error("error during migrate income");
        }
    });
}