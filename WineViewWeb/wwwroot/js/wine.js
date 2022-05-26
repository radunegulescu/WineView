var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Admin/Wine/GetAll"
        },
        "columns": [
            { "data": "name", "width": "15%" },
            { "data": "price", "width": "15%" },
            { "data": "color.name", "width": "15%" },
            { "data": "style.name", "width": "15%" },
            { "data": "winery.name", "width": "15%" },
            {
                "data": "grapes",
                "render": function (data) {
                    var grapes = data[0].name;
                    for (var i = 1; i < data.length; i++) {
                        grapes = grapes.concat(", ", data[i].name);
                    }
                    return grapes;
                },
                "width": "15%"
            },
            { "data": "volume", "width": "15%" },
            { "data": "isInClasifier", "width": "15%" },
            { "data": "clasifierId", "width": "15%" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="w-75 btn-group" role="group">
                        <a href="/Admin/Wine/Upsert?id=${data}"
                        class="btn btn-primary mx-2"><i class="bi bi-pencil-square"></i> &nbsp; Edit</a>
                        <a onClick=Delete('/Admin/Wine/Delete/${data}')
                        class="btn btn-danger mx-2"><i class="bi bi-trash3"></i> &nbsp; Delete</a>
                    </div>
                        `
                },
                "width": "15%"
            }
        ]
    });
}

function Delete(url) {
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: "DELETE",
                success: function (data) {
                    if (data.success) {
                        dataTable.ajax.reload();
                        toastr.success(data.message);
                    }
                    else {
                        toastr.error(data.message);
                    }
                }
            })
        }
    })
}