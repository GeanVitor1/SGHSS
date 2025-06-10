$(document).ready(function () {
    select2("#cargoSelect");
});

function gravarInclusao() {
    var dados = $("#formIncluir").serialize();

    var form = document.getElementById("formIncluir");
    if (form.checkValidity()) {
        $.ajax({
            url: '/Selecao/Incluir',
            method: 'POST',
            data: dados,
            beforeSend: function () {
                showOverlay(".wrapper");
            },
            success: function (data) {
                if (data.resultado == "falha") {
                    mensagem.error("Erro ao incluir Selecao", data.mensagem, 10);
                    console.log(data.mensagem);
                    return false;
                }

                window.location.href = "/Selecao";
            },
            complete: function () {
                hideOverlay(".wrapper");
            },
            error: function () {
                mensagem.error("Erro ao incluir selecao", "Ocorreu um erro desconhecido", 10);
            }
        });
    } else {
        form.reportValidity();
    }
}