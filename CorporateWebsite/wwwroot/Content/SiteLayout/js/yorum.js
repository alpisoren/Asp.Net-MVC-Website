/* Blog detay sayfasındaki yorum formu (AJAX). Sonuç formun altında, ekran okuyucuya da duyurularak gösterilir.
   Inline script yerine ayrı dosyada; CSP ile uyumlu. */
(function ($) {
    "use strict";

    $(function () {
        var $form = $("#yorumFormu");
        if (!$form.length) {
            return;
        }

        var $durum = $("#yorumDurum");
        var $buton = $form.find("button[type=submit]");
        var butonMetni = $buton.html();

        function goster(basarili, mesaj) {
            $durum.attr("class", "kw-alert mb-3 is-visible " + (basarili ? "kw-alert-success" : "kw-alert-error")).text(mesaj);
        }

        $form.on("submit", function (e) {
            if (!this.checkValidity()) {
                return; // tarayıcı kendi alan uyarılarını göstersin
            }
            e.preventDefault();
            $buton.prop("disabled", true).text("Gönderiliyor…");
            $durum.attr("class", "kw-alert mb-3");

            $.ajax({
                url: $form.attr("action"),
                type: "POST",
                dataType: "json",
                data: $form.serialize()
            }).done(function (hata) {
                if (hata) {
                    goster(false, "Yorumunuz gönderilemedi. Lütfen alanları kontrol edin ya da biraz sonra tekrar deneyin.");
                } else {
                    goster(true, "Teşekkürler! Yorumunuz onaylandıktan sonra yayınlanacak.");
                    $form[0].reset();
                }
            }).fail(function () {
                goster(false, "Yorumunuz gönderilemedi. Lütfen sayfayı yenileyip tekrar deneyin.");
            }).always(function () {
                $buton.prop("disabled", false).html(butonMetni);
            });
        });
    });
})(jQuery);
