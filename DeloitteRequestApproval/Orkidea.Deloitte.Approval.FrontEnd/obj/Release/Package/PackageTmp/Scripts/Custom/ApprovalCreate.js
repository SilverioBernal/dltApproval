$(document).ready(function () {
    var distributionList = $('#idDistributionList');

    distributionList.empty();
    distributionList.append(
                $('<option/>', {
                    value: ''
                }).html('')
            );

    $.getJSON('../DistributionList/GetAsyncDistributionList', function (result) {
        $(result).each(function () {
            distributionList.append(
                $('<option/>', {
                    value: this.id
                }).html(this.name)
            );
        });
    });
});