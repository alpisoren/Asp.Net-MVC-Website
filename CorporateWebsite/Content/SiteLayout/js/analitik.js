/* Google Analytics 4 + çerez onayı (KVKK).
   - Bant yalnızca Web.config'te GoogleAnalyticsId doluysa sayfada vardır (data-ga özniteliği).
   - GA betiği ziyaretçi "Kabul et" demeden YÜKLENMEZ; ret seçilirse hiçbir istek gitmez.
   - Reklam izinleri Consent Mode ile her zaman kapalıdır; yalnızca ziyaret istatistiği toplanır.
   - Tercih tarayıcıda saklanır (localStorage "kw-cerez"); alt bilgideki "Çerez tercihleri" ile değiştirilebilir.
   Inline script yok; CSP ile uyumlu. */
(function () {
    "use strict";

    var ANAHTAR = "kw-cerez";
    var bant = document.getElementById("kw-cerez");
    if (!bant) {
        return;
    }
    var olcumKimligi = bant.getAttribute("data-ga");
    var yuklendi = false;

    function oku() {
        try { return window.localStorage.getItem(ANAHTAR); } catch (e) { return null; }
    }

    function yaz(deger) {
        try { window.localStorage.setItem(ANAHTAR, deger); } catch (e) { /* gizli pencere vb. */ }
    }

    function gaYukle() {
        if (yuklendi || !olcumKimligi) {
            return;
        }
        yuklendi = true;
        window.dataLayer = window.dataLayer || [];
        window.gtag = function () { window.dataLayer.push(arguments); };
        window.gtag("consent", "default", {
            analytics_storage: "granted",
            ad_storage: "denied",
            ad_user_data: "denied",
            ad_personalization: "denied"
        });
        window.gtag("js", new Date());
        window.gtag("config", olcumKimligi);

        var betik = document.createElement("script");
        betik.async = true;
        betik.src = "https://www.googletagmanager.com/gtag/js?id=" + encodeURIComponent(olcumKimligi);
        document.head.appendChild(betik);
    }

    function goster(acik, odakla) {
        bant.hidden = !acik;
        if (acik && odakla) {
            var ilk = bant.querySelector("button");
            if (ilk) {
                ilk.focus({ preventScroll: true });
            }
        }
    }

    bant.addEventListener("click", function (e) {
        var dugme = e.target.closest("[data-cerez]");
        if (!dugme) {
            return;
        }
        var secim = dugme.getAttribute("data-cerez");
        var onceki = oku();
        yaz(secim);
        goster(false);
        if (secim === "kabul") {
            gaYukle();
        } else if (onceki === "kabul" && yuklendi) {
            // Onay geri alındı: GA'nın bu sayfada daha fazla veri göndermemesi için sayfayı yenile.
            window.location.reload();
        }
    });

    // Alt bilgideki "Çerez tercihleri" bağlantısı bandı yeniden açar.
    document.addEventListener("click", function (e) {
        var ayar = e.target.closest("[data-cerez-ayar]");
        if (ayar) {
            e.preventDefault();
            goster(true, true);
        }
    });

    var tercih = oku();
    if (tercih === "kabul") {
        gaYukle();
    } else if (tercih !== "ret") {
        goster(true, false); // ilk ziyarette odak sayfada kalır (bant ekranın altında sabit durur)
    }
})();
