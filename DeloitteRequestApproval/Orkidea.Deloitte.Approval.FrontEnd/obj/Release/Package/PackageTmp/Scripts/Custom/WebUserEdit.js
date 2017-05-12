$(document).ready(function () {
    var cboRoles = $('#idRol');
    var valRol = $('#rol').val();
    cboRoles.empty();

    cboRoles.append(
                $('<option/>', {
                    value: ''
                }).html('')
            );
    cboRoles.append(
                $('<option/>', {
                    value: 'A'
                }).html('Administrador')
            );
    cboRoles.append(
                $('<option/>', {
                    value: 'S'
                }).html('Socio')
            );
    cboRoles.append(
                $('<option/>', {
                    value: 'U'
                }).html('Usuario')
            );

    var opciones = $('option')

    $(opciones).each(function () {
        var option = this;
        if (option.value === valRol) {
            option.setAttribute("selected", true);
        }
    });
});