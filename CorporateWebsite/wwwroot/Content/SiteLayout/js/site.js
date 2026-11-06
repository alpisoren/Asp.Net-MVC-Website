/* Ziyaretçi sitesi ortak davranışları (şablonun main.js dosyasının sadeleştirilmiş hali).
   Kullanılmayan Swiper/GLightbox/Isotope bağımlılıkları yok; CSP ile uyumlu (inline script yok). */
(function () {
    "use strict";

    var header = document.getElementById("header");
    var navbar = document.getElementById("navbar");
    var toggle = document.querySelector(".mobile-nav-toggle");
    var backToTop = document.querySelector(".back-to-top");

    // Kaydırınca başlığa gölge, "yukarı çık" düğmesini göster
    function kaydirmaDurumu() {
        var asagida = window.scrollY > 100;
        if (header) { header.classList.toggle("header-scrolled", asagida); }
        if (backToTop) { backToTop.classList.toggle("active", asagida); }
    }
    window.addEventListener("load", kaydirmaDurumu);
    document.addEventListener("scroll", kaydirmaDurumu, { passive: true });

    // Mobil menü
    function menuyuKapat() {
        if (!navbar.classList.contains("navbar-mobile")) { return; }
        navbar.classList.remove("navbar-mobile");
        toggle.classList.add("bi-list");
        toggle.classList.remove("bi-x");
        toggle.setAttribute("aria-expanded", "false");
    }

    if (toggle && navbar) {
        toggle.setAttribute("aria-expanded", "false");
        toggle.addEventListener("click", function () {
            var acik = navbar.classList.toggle("navbar-mobile");
            toggle.classList.toggle("bi-list", !acik);
            toggle.classList.toggle("bi-x", acik);
            toggle.setAttribute("aria-expanded", acik ? "true" : "false");
        });

        document.addEventListener("keydown", function (e) {
            if (e.key === "Escape") { menuyuKapat(); toggle.focus(); }
        });

        navbar.querySelectorAll("ul a").forEach(function (a) {
            a.addEventListener("click", menuyuKapat);
        });
    }

    // Yukarı çık
    if (backToTop) {
        backToTop.addEventListener("click", function (e) {
            e.preventDefault();
            window.scrollTo({ top: 0, behavior: "smooth" });
            var main = document.getElementById("main");
            if (main) { main.focus({ preventScroll: true }); }
        });
    }

    // Kaydırma animasyonları (hareket azaltma tercihine saygı)
    var azHareket = window.matchMedia && window.matchMedia("(prefers-reduced-motion: reduce)").matches;
    if (window.AOS) {
        window.AOS.init({ duration: 500, easing: "ease-out", once: true, disable: azHareket });
    }
})();
