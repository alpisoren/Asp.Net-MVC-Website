/* İletişim formu: sayfa yenilemeden gönderir, sonucu formun altında (ekran okuyucuya da duyurularak) gösterir.
   JavaScript kapalıysa form normal POST ile çalışmaya devam eder. */
(function () {
    "use strict";

    var form = document.getElementById("iletisimFormu");
    if (!form || !window.fetch) {
        return;
    }

    var durum = document.getElementById("iletisimDurum");
    var buton = form.querySelector("button[type=submit]");
    var butonMetni = buton ? buton.innerHTML : "";

    function goster(basarili, mesaj) {
        durum.className = "kw-alert is-visible " + (basarili ? "kw-alert-success" : "kw-alert-error");
        durum.textContent = mesaj;
    }

    form.addEventListener("submit", function (e) {
        if (!form.checkValidity()) {
            return; // tarayıcı kendi uyarılarını göstersin
        }
        e.preventDefault();

        buton.disabled = true;
        buton.innerHTML = "Gönderiliyor…";
        durum.className = "kw-alert";

        fetch(form.action, {
            method: "POST",
            body: new FormData(form),
            headers: { "X-Requested-With": "XMLHttpRequest" },
            credentials: "same-origin"
        }).then(function (yanit) {
            if (!yanit.ok) { throw new Error(); }
            return yanit.json();
        }).then(function (sonuc) {
            goster(sonuc.basarili, sonuc.mesaj);
            if (sonuc.basarili) { form.reset(); }
        }).catch(function () {
            goster(false, "Mesajınız gönderilemedi. Lütfen sayfayı yenileyip tekrar deneyin.");
        }).finally(function () {
            buton.disabled = false;
            buton.innerHTML = butonMetni;
        });
    });
})();
