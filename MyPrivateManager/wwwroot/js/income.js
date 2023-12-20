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

function detailIncome(incomeId){
    $.ajax({
        url: '/Income/GetIncome/' + incomeId,
        type: 'GET',
        success : function(data){
            $('#detailIncomeModalLabel').text('Detail Income');
            $('#detailIncomeId').val(data.incomeId);
            $('#detailIncomeAmount').val(data.amount);
            $('#detailIncomeSourceName').val(data.source.sourceName);
            var localDate = new Date(data.date);
            var offset = localDate.getTimezoneOffset();
            localDate.setMinutes(localDate.getMinutes() - offset);
            var formattedDate = localDate.toISOString().split('T')[0];
            $('#detailIncomeDate').val(formattedDate);
            $('#detailIncomeDescription').val(data.description);
            $('#detailIncomeModal').modal('show');
        },
        error: function(){
            console.log('Error proseccing data');
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

            function radioButton(source){
                return `
                <div class="form-check">
                    <input class="form-check-input" type="radio" name="sourceOption" id="sourceOption${source.sourceId}" value="${source.sourceId}">
                    <label class="form-check-label" for="sourceOption${source.sourceId}" >
                        ${source.sourceName}
                    </label>
                </div>
                `;
            }

            data.forEach(function (source){
                if (source.sourceId !== sourceIdFrom){
                    container.append(radioButton(source));
                }
            });

           
        },
        error: function () {
            console.error('Failed to fetch data');
        }
    });
}

function submitMigration(){
    var sourceIdFrom = $('#migrateIncomeModal').data('sourceIdFrom');
    var sourceIdTo = $('input[value]:checked').val();
    console.log(sourceIdFrom);
    console.log('value of source Id :',sourceIdTo);
    $.ajax({
        url : '/Source/MigrateIncome',
        type : 'POST',
        data : {sourceIdFrom : sourceIdFrom, sourceIdTo : sourceIdTo},
        success : function(){
            console.log("success migrate");
            
        },
        error : function(){
            console.error("error during migrate income");
        }
    });
    $('#migrateIncomeModal').modal('hide');
    deleteSource(sourceIdFrom);
}

function deleteSource(sourceId) {
    var isConfirmed = confirm('are you sure you want to delete this source?');
    if (isConfirmed) {
        $.ajax({
            url: '/Source/DeleteSource/' + sourceId,
            type: 'DELETE',
            success: function () {
                location.reload();
            },
            error: function () {
                console.error('Failed to delete source');
            }
        });
    }
}

function getIncomeChartByCategory(){
    $.ajax({
        url: '/Income/IncomeByCategory',
        type: 'GET',
        success: function(data){
            var indicatorData = [];
            var totalByCategory =[];
            data.forEach(function(item){
                indicatorData.push({
                    name: item.sourceName,
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

