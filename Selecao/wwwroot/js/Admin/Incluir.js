function gravarInclusao() {
    var dados = $("#formIncluir").serialize();

    var form = document.getElementById("formIncluir");
    if (form.checkValidity()) {
        $.ajax({
            url: '/Admin/Incluir',
            method: 'POST',
            data: dados,
            beforeSend: function () {
                showOverlay(".wrapper");
            },
            success: function (data) {
                if (data.resultado == "falha") {
                    mensagem.error("Erro ao incluir usuário", data.mensagem, 10);
                    return false;
                }

                window.location.href = "/Admin";
            },
            complete: function () {
                hideOverlay(".wrapper");
            },
            error: function () {
                mensagem.error("Erro ao incluir contato", "Ocorreu um erro desconhecido", 10);
            }
        });
    } else {
        form.reportValidity();
    }
}