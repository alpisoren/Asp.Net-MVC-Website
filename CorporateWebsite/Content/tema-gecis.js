/* Açık / koyu tema geçişi (site ve panel ortak).
   <head> içinde, CSS'ten hemen sonra ve "defer" OLMADAN yüklenir; böylece sayfa çizilmeden
   tema uygulanır ve açık/koyu arasında yanıp sönme olmaz. Inline script kullanmaz (CSP uyumlu).
   Varsayılan tema açıktır; kullanıcının seçimi tarayıcıda (localStorage) saklanır. */
(function () {
    "use strict";

    var ANAHTAR = "kw-tema";
    var kok = document.documentElement;

    function kayitliTema() {
        try {
            var t = window.localStorage.getItem(ANAHTAR);
            return t === "dark" || t === "light" ? t : null;
        } catch (e) {
            return null; // gizli pencere vb.
        }
    }

    function kaydet(tema) {
        try { window.localStorage.setItem(ANAHTAR, tema); } catch (e) { /* yok say */ }
    }

    function uygula(tema) {
        kok.setAttribute("data-theme", tema);
        kok.style.colorScheme = tema;

        var meta = document.querySelector('meta[name="theme-color"]');
        if (meta) {
            meta.setAttribute("content", tema === "dark" ? "#0B1220" : "#F6F8FB");
        }

        // Düğmelerin durumunu ve erişilebilir adını güncelle
        var dugmeler = document.querySelectorAll("[data-tema-degistir]");
        for (var i = 0; i < dugmeler.length; i++) {
            var koyu = tema === "dark";
            dugmeler[i].setAttribute("aria-pressed", koyu ? "true" : "false");
            dugmeler[i].setAttribute("aria-label", koyu ? "Açık temaya geç" : "Koyu temaya geç");
            dugmeler[i].setAttribute("title", koyu ? "Açık temaya geç" : "Koyu temaya geç");
        }

        // Panelde Jodit editörleri de temaya uysun
        var editorler = document.querySelectorAll(".jodit-container, .jodit-popup, .jodit-dialog");
        for (var j = 0; j < editorler.length; j++) {
            editorler[j].classList.toggle("jodit_theme_dark", tema === "dark");
            editorler[j].classList.toggle("jodit_theme_default", tema !== "dark");
        }

        try {
            document.dispatchEvent(new CustomEvent("kw-tema-degisti", { detail: tema }));
        } catch (e) { /* eski tarayıcı */ }
    }

    // 1) Sayfa çizilmeden önce uygula
    uygula(kayitliTema() || "light");

    // 2) Düğmeleri bağla
    function bagla() {
        uygula(kok.getAttribute("data-theme") || "light"); // düğme etiketlerini güncelle
        document.addEventListener("click", function (e) {
            var dugme = e.target.closest ? e.target.closest("[data-tema-degistir]") : null;
            if (!dugme) { return; }
            e.preventDefault();
            var yeni = kok.getAttribute("data-theme") === "dark" ? "light" : "dark";
            kaydet(yeni);
            uygula(yeni);
        });
    }

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", bagla);
    } else {
        bagla();
    }

    // Başka sekmede tema değişirse bu sekme de uysun
    window.addEventListener("storage", function (e) {
        if (e.key === ANAHTAR && (e.newValue === "dark" || e.newValue === "light")) {
            uygula(e.newValue);
        }
    });

    window.KurumsalTema = { uygula: uygula };
})();
