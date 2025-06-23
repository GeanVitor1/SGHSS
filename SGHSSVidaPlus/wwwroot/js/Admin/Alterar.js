$(document).ready(function () {
    // Lógica para habilitar/desabilitar permissões se o usuário for administrador
    $("#administradorCheck").change(function () {
        var isAdmin = $(this).is(":checked");
        // Desabilita/habilita o input e o label, e garante que as permissões não são enviadas se Admin
        $("#line-permissoes input[type='hidden']").prop("disabled", isAdmin); // Desabilita inputs hidden
        $("#line-permissoes input[type='checkbox']").prop("disabled", isAdmin); // Desabilita checkboxes
        $("#line-permissoes label.form-check-label").css("opacity", isAdmin ? "0.5" : "1"); // Estilo visual
        $("#line-permissoes b").css("opacity", isAdmin ? "0.5" : "1"); // Estilo visual para títulos de seção
    }).trigger("change"); // Executa na carga da página para aplicar o estado inicial
});

function gravarAlteracao() {
    var form = $("#formAlterar");
    var formData = form.serializeArray().reduce(function (obj, item) {
        obj[item.name] = item.value;
        return obj;
    }, {});

    // Captura o estado dos checkboxes 'Admin' e 'Bloqueado'
    formData.Admin = $("#administradorCheck").is(":checked");
    formData.Bloqueado = $("#statusBloqueado").is(":checked");

    // Coleta as permissões (Claims) apenas se o usuário não for Admin
    if (!formData.Admin) {
        var permissoes = [];
        form.find('input[name^="Permissoes"]').each(function (index, element) {
            var inputName = $(element).attr('name');
            var nameParts = inputName.match(/Permissoes\[(\d+)\]\.(.+)/);
            if (nameParts) {
                var idx = parseInt(nameParts[1]);
                var prop = nameParts[2];
                if (!permissoes[idx]) {
                    permissoes[idx] = {};
                }
                if (prop === "IsSelected") {
                    permissoes[idx][prop] = $(element).is(":checked");
                } else {
                    permissoes[idx][prop] = $(element).val();
                }
            }
        });
        formData.Permissoes = permissoes.filter(p => p !== null);
    } else {
        formData.Permissoes = []; // Se for admin, envia um array vazio de permissões específicas
    }

    // Verifica a validade do formulário antes de enviar
    if (form[0].checkValidity()) {
        $.ajax({
            url: '/Admin/Alterar',
            method: 'POST',
            contentType: 'application/json', // Mude para JSON
            data: JSON.stringify(formData), // Envie como JSON string
            beforeSend: function () {
                if (typeof showOverlay === 'function') showOverlay(".wrapper");
            },
            success: function (data) {
                if (data.resultado == "falha") {
                    mensagem.error("Erro ao alterar usuário", data.mensagem, 10);
                    return false;
                }
                mensagem.success(data.mensagem || "Usuário alterado com sucesso!", "", 10);
                window.location.href = "/Admin"; // Redirecionamento direto
            },
            complete: function () {
                if (typeof hideOverlay === 'function') hideOverlay(".wrapper");
            },
            error: function () {
                mensagem.error("Erro ao alterar usuário", "Ocorreu um erro desconhecido", 10);
            }
        });
    } else {
        form[0].reportValidity();
    }
}