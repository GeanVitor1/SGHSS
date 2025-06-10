$(document).ready(function () {
    dataTableNoPaging("#tbCandidatos");
});
function verCurriculo(id) {
    $.ajax({
        url: '/Candidatos/BuscarCurriculo',
        method: 'GET',
        data: id,
        beforeSend: function () {
            showOverlay(".wrapper");
        },
        success: function (data) {
            $("#dadoCurriculo").empty().html(data);
            $("#modalCurriculo").modal("show");
        },
        complete: function () {
            hideOverlay(".wrapper");
        },
        error: function () {
            mensagem.error("Erro ao incluir contato", "Ocorreu um erro desconhecido", 10);
        }
    });
}

function configurarColunas() {

    if ($("#codigoCheckBox").is(':checked')) {
        $(".thCodigo").show();
        $(".tdCodigo").show();
    } else {
        $(".thCodigo").hide();
        $(".tdCodigo").hide();
    }
    if ($("#nomeCheckBox").is(':checked')) {
        $(".thNome").show();
        $(".tdNome").show();
    } else {
        $(".thNome").hide();
        $(".tdNome").hide();
    }
    if ($("#bairroCheckBox").is(':checked')) {
        $(".thBairro").show();
        $(".tdBairro").show();
    } else {
        $(".thBairro").hide();
        $(".tdBairro").hide();
    }

    if ($("#idadeCheckBox").is(':checked')) {
        $(".thIdade").show();
        $(".tdIdade").show();
    } else {
        $(".thIdade").hide();
        $(".tdIdade").hide();
    }
    $("#configurarTabela").modal("hide");
}


function gravarInclusao() {
    var dados = $("#formIncluirCandidatos").serialize();

    $.ajax({
        url: '/Selecao/IncluirCandidatos',
        method: 'POST',
        data: dados,
        beforeSend: function () {
            showOverlay(".wrapper");
        },
        success: function (data) {
            if (data.resultado == "falha") {
                mensagem.error("Erro ao incluir candidatos na seleção", data.mensagem, 10);
                console.log(data.mensagem);
                return false;
            }

            window.location.href = "/Selecao";
        },
        complete: function () {
            hideOverlay(".wrapper");
        },
        error: function () {
            mensagem.error("Erro ao incluir candidatos na seleção", "Ocorreu um erro desconhecido", 10);
        }
    });
}