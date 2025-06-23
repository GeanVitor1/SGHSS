$(document).ready(function () {
    $("#tbUsuarios").DataTable({
        "language": { "url": "//cdn.datatables.net/plug-ins/1.10.25/i18n/Portuguese-Brasil.json" }, // Atualizado para 1.10.25 (ou a versão que você usa)
        "ordering": false,
        "paging": false,
        "bFilter": true
    });
});

function bloquearUser(userId, userName) { // Renomeado 'nome' para 'userName' para consistência
    Swal.fire({ // Usando Swal.fire
        title: "Bloquear usuário?",
        text: `Tem certeza que deseja bloquear o usuário ${userName}?`,
        icon: "warning", // Usando 'warning' para bloqueio
        showCancelButton: true,
        confirmButtonColor: "#3085d6", // Cores padrão para consistência
        cancelButtonColor: "#d33",
        confirmButtonText: `<i class="fa fa-thumbs-up"></i> Confirmar!`,
        cancelButtonText: `<i class="fa fa-thumbs-down"></i> Cancelar`,
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: '/Admin/BloquearUsuario',
                method: 'POST',
                data: { userId: userId }, // Passando userId como objeto
                beforeSend: function () {
                    if (typeof showOverlay === 'function') showOverlay(".wrapper"); // Verifica se a função existe
                },
                success: function (data) {
                    if (data.resultado == "falha") {
                        mensagem.error("Erro ao bloquear usuário", data.mensagem, 10);
                        return false;
                    }
                    mensagem.success(data.mensagem || "Usuário bloqueado com sucesso!", "", 10); // Mensagem de sucesso
                    window.location.href = "/Admin"; // Redirecionamento direto
                },
                complete: function () {
                    if (typeof hideOverlay === 'function') hideOverlay(".wrapper"); // Verifica se a função existe
                },
                error: function () {
                    mensagem.error("Erro ao bloquear usuário", "Ocorreu um erro desconhecido", 10);
                }
            });
        }
    });
}

function desbloquearUser(userId, userName) { // Renomeado 'nome' para 'userName' para consistência
    Swal.fire({ // Usando Swal.fire
        title: "Desbloquear usuário?",
        text: `Tem certeza que deseja desbloquear o usuário ${userName}?`,
        icon: "question",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Confirmar!"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: '/Admin/DesbloquearUsuario',
                method: 'POST',
                data: { userId: userId }, // Passando userId como objeto
                beforeSend: function () {
                    if (typeof showOverlay === 'function') showOverlay(".wrapper");
                },
                success: function (data) {
                    if (data.resultado == "falha") {
                        mensagem.error("Erro ao desbloquear usuário", data.mensagem, 10);
                        return false;
                    }
                    mensagem.success(data.mensagem || "Usuário desbloqueado com sucesso!", "", 10);
                    window.location.href = "/Admin";
                },
                complete: function () {
                    if (typeof hideOverlay === 'function') hideOverlay(".wrapper");
                },
                error: function () {
                    mensagem.error("Erro ao desbloquear usuário", "Ocorreu um erro desconhecido", 10);
                }
            });
        }
    });
}