$(document).ready(function () {
    $("#tbUsuarios").DataTable({
        "language": { "url": "//cdn.datatables.net/plug-ins/1.10.11/i18n/Portuguese-Brasil.json" },
        "ordering": false,
        "paging": false,
        "bFilter": true
    });
});

function bloquearUser(userId, nome) {
    customSwal.fire({
        title: "Bloquear usuário?",
        text: `Tem certeza que deseja bloquear o usuário ${nome}?`,
        icon: "question",
        showCancelButton: true,
        confirmButtonColor: "#1A2035",
        cancelButtonColor: "#F25961",
        confirmButtonText: `<i class="fa fa-thumbs-up"></i> Confirmar!`,
        cancelButtonText: `<i class="fa fa-thumbs-down"></i> Cancelar`,
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: '/Admin/BloquearUsuario',
                method: 'POST',
                data: { userId },
                beforeSend: function () {
                    showOverlay(".wrapper");
                },
                success: function (data) {
                    if (data.resultado == "falha") {
                        mensagem.error("Erro ao alterar usuário", data.mensagem, 10);
                        return false;
                    }

                    window.location.href = "/Admin";
                },
                complete: function () {
                    hideOverlay(".wrapper");
                },
                error: function () {
                    mensagem.error("Erro ao alterar contato", "Ocorreu um erro desconhecido", 10);
                }
            });
        }
    });
}

function desbloquearUser(userId, nome) {
    Swal.fire({
        title: "Desbloquear usuário?",
        text: `Tem certeza que deseja desbloquear o usuário ${nome}?`,
        icon: "question",
        showCancelButton: true,
        confirmButtonColor: "#1A2035",
        cancelButtonColor: "#F25961",
        confirmButtonText: "Confirmar!"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: '/Admin/DesbloquearUsuario',
                method: 'POST',
                data: { userId },
                beforeSend: function () {
                    showOverlay(".wrapper");
                },
                success: function (data) {
                    if (data.resultado == "falha") {
                        mensagem.error("Erro ao alterar usuário", data.mensagem, 10);
                        return false;
                    }

                    window.location.href = "/Admin";
                },
                complete: function () {
                    hideOverlay(".wrapper");
                },
                error: function () {
                    mensagem.error("Erro ao alterar contato", "Ocorreu um erro desconhecido", 10);
                }
            });
        }
    });
}