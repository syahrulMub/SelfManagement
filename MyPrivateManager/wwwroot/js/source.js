function openAddSource() {

}

function openEditSource(id) {

}

function openDeleteSource() {

}

function storeSource() {

}

function updateSource() {

}

function destroySource(id) {

}




function addSource() {
    $('#createSourceModalLabel').text('Add Source');
    $('#createSourceName').val('');
    $('#createSourceModal').modal('show');
}

function editSource(sourceId) {
    $.ajax({
        url: '/Source/GetSource/' + sourceId,
        type: 'GET',
        success: function (data) {
            $('#editSourceModalLabel').text('Edit Source');
            $('#editSourceId').val(data.sourceId);
            $('#editSourceName').val(data.sourceName);
            $('#editSourceModal').modal('show');
        },
        error: function () {
            console.error('Failed to fetch source details');
        }
    });
}

function createSource() {
    var sourceData = {
        SourceName: $('#createSourceName').val()
    };
    $.ajax({
        url: '/Source/CreateSource',
        type: 'POST',
        data: sourceData,
        success: function () {
            $('#createSourceModal').modal('hide');
            location.reload();
        },
        error: function () {
            console.error('Failed to save source');
        }
    });
}

function updateSource() {
    var sourceId = $('#editSourceId').val();
    var sourceData = {
        SourceName: $('#editSourceName').val()
    };
    $.ajax({
        url: '/Source/UpdateSource/' + sourceId,
        type: 'POST',
        data: sourceData,
        success: function () {
            $('#editSourceModal').modal('hide');
            location.reload();
        },
        error: function () {
            console.error('Something went wrong');
        }
    });
}