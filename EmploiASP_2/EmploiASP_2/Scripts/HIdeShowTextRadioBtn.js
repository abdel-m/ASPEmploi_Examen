$(document).ready(function () {
    $("input[name='ContratTravailleurSoumis.idContrat']").change(function () {
        var test = $(this).val();
        if (test == 1111) {
            $("#divNumDossierMedical").show();
        } else {
            $("#divNumDossierMedical").hide();
        }
    });

    $("input[name='ContratTravailleurSoumis.idContrat']").prop("checked", true).trigger("change");
});
