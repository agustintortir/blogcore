var dataTable;

$(document).ready(function () {
    cargarDatatable();
});

function cargarDatatable() {
    dataTable = $("#tblUsuarios").DataTable({
        ajax: {
            url: "/admin/usuarios/GetAll",
            type: "GET",
            datatype: "json"
        },
        columns: [
            { data: "id", width: "20%" },
            { data: "nombre", width: "30%" },
            { data: "email", width: "30%" },
            {
                data: null,
                className: "text-center",
                render: function (row) {
                    const id = row.id;
                    const bloqueado = row.estaBloqueado === true;

                    const lockBtn = bloqueado
                        ? `<a href="/Admin/Usuarios/Desbloquear/${id}" class="btn btn-warning text-white me-1" title="Desbloquear" style="width:42px">
                               <i class="fas fa-lock"></i>
                           </a>`
                        : `<a href="/Admin/Usuarios/Bloquear/${id}" class="btn btn-success text-white me-1" title="Bloquear" style="width:42px">
                               <i class="fas fa-lock-open"></i>
                           </a>`;

                    const delBtn = `<a onclick="Delete('/Admin/Usuarios/Delete/${id}')" class="btn btn-danger text-white" title="Borrar" style="width:42px">
                                        <i class="fas fa-trash-alt"></i>
                                    </a>`;

                    return `<div class="d-inline-flex">${lockBtn}${delBtn}</div>`;
                },
                width: "20%"
            }
        ],
        language: {
            decimal: "",
            emptyTable: "No hay registros",
            info: "Mostrando _START_ a _END_ de _TOTAL_ entradas",
            infoEmpty: "Mostrando 0 a 0 de 0 entradas",
            infoFiltered: "(filtrado de _MAX_ entradas)",
            lengthMenu: "Mostrar _MENU_ entradas",
            loadingRecords: "Cargando...",
            processing: "Procesando...",
            search: "Buscar:",
            zeroRecords: "Sin resultados",
            paginate: {
                first: "Primero",
                last: "Último",
                next: "Siguiente",
                previous: "Anterior"
            }
        },
        width: "100%"
    });
}

function Delete(url) {
    // Extraigo el id si vino '/Admin/Usuarios/Delete/{id}'
    const id = url.split('/').pop();
    const token = $('input[name="__RequestVerificationToken"]').val();

    swal({
        title: "¿Seguro que querés borrar este usuario?",
        text: "Este cambio no se puede deshacer.",
        type: "warning",
        showCancelButton: true,
        confirmButtonColor: "#DD6B55",
        confirmButtonText: "Sí, borrar",
        cancelButtonText: "Cancelar",
        closeOnConfirm: true  // <-- C mayúscula
    }, function () {
        $.ajax({
            type: 'POST',
            url: '/Admin/Usuarios/Delete',
            data: { id: id, __RequestVerificationToken: token },
            success: function (data) {
                if (data && data.success) {
                    toastr.success(data.message || "Usuario borrado");
                    dataTable.ajax.reload();
                } else {
                    toastr.error((data && data.message) || "No se pudo borrar");
                }
            },
            error: function (xhr) {
                const msg = xhr.responseJSON?.message || xhr.responseText || "Error del servidor";
                toastr.error(msg);
            }
        });
    });
}

