const customSwal = Swal.mixin({
    allowOutsideClick: false,
    keydownListenerCapture: false,
    allowEscapeKey: false,
    confirmButtonColor: "#1A2035",
    cancelButtonColor: "#F25961",
    iconColor: "#1A2035",
    confirmButtonText: `<i class="fa fa-check"></i> Confirmar!`,
    cancelButtonText: `<i class="fa fa-times"></i> Cancelar`,
    didOpen: () => {
        const actionsContainer = document.querySelector('.swal2-actions');
        if (actionsContainer) {
            const confirmButton = document.querySelector('.swal2-confirm');
            const cancelButton = document.querySelector('.swal2-cancel');
            if (confirmButton && cancelButton) {
                // Mover o botão "Confirmar" após o botão "Cancelar"
                actionsContainer.appendChild(confirmButton);
            }
        }
    }
});

function select2(seletor) {
    $(seletor).select2({
        theme: "bootstrap-5",
        width: $(this).data('width') ? $(this).data('width') : $(this).hasClass('w-100') ? '100%' : 'style',
        placeholder: $(this).data('placeholder'),
    });
}

function dataTableDefaut(tabela) {

}

function dataTableNoPaging(tabela) {
    var table = $(tabela).DataTable({
        columnDefs: [
            { width: '20px', targets: 0 }
        ],
        "language": {
            "url": "/lib/plugin/datatables/language/datatables.language.brasil.json"
        },
        "ordering": false,
        "paging": false,
        "bFilter": true,
        "autoWidth": false
    });
}

