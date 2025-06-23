// Função para gravar a inativação/ativação (se a view de Excluir tiver um botão de confirmação)
function gravarInativacao(profissionalId) {
    Swal.fire({
        title: 'Confirmar Inativação',
        text: 'Deseja realmente inativar este profissional de saúde? Ele não poderá mais ser usado em novos agendamentos.',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Sim, inativar!',
        cancelButtonText: 'Cancelar'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: '/ProfissionaisSaude/AlterarStatus', // Reutiliza a ação AlterarStatus
                method: 'POST',
                data: { id: profissionalId, ativo: false }, // Passa o ID e o status desejado
                beforeSend: function () {
                    if (typeof showOverlay === 'function') showOverlay(".wrapper");
                },
                success: function (data) {
                    if (data.resultado === "sucesso") {
                        mensagem.success(data.mensagem || "Profissional de saúde inativado com sucesso!", "", 10);
                        setTimeout(function () { location.href = "/ProfissionaisSaude/Index"; }, 1500);
                    } else {
                        mensagem.error(data.mensagem || "Erro ao inativar profissional de saúde.", "", 10);
                    }
                },
                complete: function () {
                    if (typeof hideOverlay === 'function') hideOverlay(".wrapper");
                },
                error: function () {
                    mensagem.error("Erro na comunicação com o servidor.", "", 10);
                }
            });
        }
    });
}