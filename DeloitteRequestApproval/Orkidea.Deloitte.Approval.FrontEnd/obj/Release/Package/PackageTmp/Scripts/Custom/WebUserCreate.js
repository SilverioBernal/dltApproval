$(document).ready(function () {
    var cboRoles = $('#idRol');
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
});