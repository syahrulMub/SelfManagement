// New function to fetch expense details by IDs
function getExpenseDetailsByIds(expenseIds, categoryName) {
    if (!expenseIds || expenseIds.length === 0) {
        console.log('No expense IDs provided');
        return;
    }

    // Convert array to comma-separated string
    var idsString = expenseIds.join(',');

    $.ajax({
        type: "GET",
        url: "/Expense/ExpenseDetailsByIds",
        data: { expenseIds: idsString },
        success: function (response) {
            renderTransactionDetailTable(response, categoryName);
        },
        error: function (error) {
            console.log('Error fetching expense details:', error);
        }
    });
}

// New function to render transaction details in a table
function renderTransactionDetailTable(data, categoryName) {
    // Update the title with category name
    $('#transactionDetailsTitle').text(`Transaction Details | ${categoryName}`);

    // Clear existing table data
    var tbody = $('#transactionDetailsBody');
    tbody.empty();

    if (!data || data.length === 0) {
        tbody.append('<tr><td colspan="3" class="text-center">No transactions found</td></tr>');
    } else {
        // Populate table rows
        data.forEach(function(expense) {
            var formattedAmount = new Intl.NumberFormat('id-ID').format(expense.amount);
            var formattedDate = new Date(expense.date).toLocaleDateString('id-ID', { 
                year: 'numeric', 
                month: 'short', 
                day: 'numeric' 
            });
            
            var row = `
                <tr>
                    <td>${expense.description || 'N/A'}</td>
                    <td>${formattedAmount}</td>
                    <td>${formattedDate}</td>
                </tr>
            `;
            tbody.append(row);
        });
    }

    // Show the details card
    $('#transactionDetailsCard').fadeIn();
}
