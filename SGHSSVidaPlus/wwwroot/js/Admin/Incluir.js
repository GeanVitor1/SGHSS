function gravarInclusao() {
    var form = $("#formIncluir");

    // Coleta todos os dados do formulário
    var formData = form.serializeArray().reduce(function (obj, item) {
        obj[item.name] = item.value;
        return obj;
    }, {});

    // Captura o estado do checkbox 'Admin'
    formData.Admin = $("#administradorCheck").is(":checked");

    // Coleta as permissões (Claims)
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
    // Filtra para remover entradas nulas e adiciona ao formData
    formData.Permissoes = permissoes.filter(p => p !== null);

    // Verifica a validade do formulário antes de enviar
    if (form[0].checkValidity()) { // checkValidity no elemento DOM nativo
        $.ajax({
            url: '/Admin/Incluir',
            method: 'POST',
            contentType: 'application/json', // Mude para JSON
            data: JSON.stringify(formData), // Envie como JSON string
            beforeSend: function () {
                if (typeof showOverlay === 'function') showOverlay(".wrapper");
            },
            success: function (data) {
                if (data.resultado == "falha") {
                    mensagem.error("Erro ao incluir usuário", data.mensagem, 10);
                    return false;
                }
                mensagem.success(data.mensagem || "Usuário incluído com sucesso!", "", 10);
                window.location.href = "/Admin"; // Redirecionamento direto
            },
            complete: function () {
                if (typeof hideOverlay === 'function') hideOverlay(".wrapper");
            },
            error: function () {
                mensagem.error("Erro ao incluir usuário", "Ocorreu um erro desconhecido", 10);
            }
        });
    } else {
        form[0].reportValidity(); // reportValidity no elemento DOM nativo
    }
}