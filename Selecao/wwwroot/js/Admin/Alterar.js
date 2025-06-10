function gravarInclusao() {
    var dados = $("#formAlterar").serialize();

    var form = document.getElementById("formAlterar");
    if (form.checkValidity()) {
        $.ajax({
            url: '/Admin/Alterar',
            method: 'POST',
            data: dados,
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
    } else {
        form.reportValidity();
    }
}