/* Yönetim paneli ortak betikleri.
   Inline <script> ve onclick/onsubmit kullanılmaz; böylece Content-Security-Policy ile uyumlu kalır. */
(function ($) {
    "use strict";

    var turkce = {
        decimal: ",",
        emptyTable: "Tabloda kayıt yok",
        info: "_TOTAL_ kayıttan _START_ - _END_ arası gösteriliyor",
        infoEmpty: "Kayıt yok",
        infoFiltered: "(_MAX_ kayıt içinden filtrelendi)",
        thousands: ".",
        lengthMenu: "Sayfada _MENU_ kayıt göster",
        loadingRecords: "Yükleniyor...",
        processing: "İşleniyor...",
        search: "Ara:",
        zeroRecords: "Eşleşen kayıt bulunamadı",
        paginate: { first: "İlk", last: "Son", next: "Sonraki", previous: "Önceki" },
        aria: { sortAscending: ": artan sırala", sortDescending: ": azalan sırala" }
    };

    // Mobil kart görünümü için: her hücreye sütun başlığını data-label olarak yaz (bkz. panel.css).
    // Başlığı yalnızca ekran okuyucuya yönelik olan (sr-only) sütun etiketsiz kalır.
    function hucreleriEtiketle(tablo) {
        var basliklar = $(tablo).find("> thead > tr:first > th").map(function () {
            return $(this).find(".sr-only").length ? "" : $.trim($(this).text());
        }).get();
        $(tablo).find("> tbody > tr").each(function () {
            $(this).children("td").each(function (i) {
                if (basliklar[i]) {
                    $(this).attr("data-label", basliklar[i]);
                }
            });
        });
    }

    $(function () {
        $("table.kp-table").each(function () { hucreleriEtiketle(this); });

        // data-tablo özniteliği olan tablolar DataTables ile zenginleştirilir (her sayfa çiziminde etiketler yenilenir).
        if ($.fn.DataTable) {
            $("table[data-tablo]")
                .on("draw.dt", function () { hucreleriEtiketle(this); })
                .DataTable({ language: turkce, order: [] });
        }

        // Zengin metin editörü (Jodit, MIT lisanslı). Kaydedilen HTML sunucuda HtmlTemizleyici'den geçer.
        if (window.Jodit) {
            // Resimler /Medya/Yukle'ye gönderilir (CSRF belirteci sayfadaki herhangi bir formdan alınır).
            var csrf = $("input[name=__RequestVerificationToken]").first().val();
            $("textarea.kp-zengin-editor").each(function () {
                window.Jodit.make(this, {
                    language: "tr",
                    theme: document.documentElement.getAttribute("data-theme") === "dark" ? "dark" : "default",
                    height: 420,
                    minHeight: 260,
                    toolbarAdaptive: false,
                    showCharsCounter: false,
                    showWordsCounter: false,
                    showXPathInStatusbar: false,
                    askBeforePasteHTML: false,
                    askBeforePasteFromWord: false,
                    defaultActionOnPaste: "insert_clear_html",
                    uploader: {
                        url: "/Medya/Yukle",
                        insertImageAsBase64URI: false,
                        imagesExtensions: ["jpg", "jpeg", "png"],
                        filesVariableName: function (i) { return "dosya" + i; },
                        prepareData: function (formData) {
                            if (csrf) {
                                formData.append("__RequestVerificationToken", csrf);
                            }
                            return formData;
                        }
                    },
                    buttons: [
                        "paragraph", "|",
                        "bold", "italic", "underline", "strikethrough", "|",
                        "ul", "ol", "|",
                        "link", "image", "table", "hr", "|",
                        "align", "|",
                        "undo", "redo", "|",
                        "source", "fullsize"
                    ],
                    controls: {
                        paragraph: {
                            list: { p: "Paragraf", h2: "Başlık 2", h3: "Başlık 3", h4: "Başlık 4", blockquote: "Alıntı" }
                        }
                    }
                });
            });
        }

        // data-onay="Mesaj" olan formlar gönderilmeden önce onay ister (silme işlemleri).
        $(document).on("submit", "form[data-onay]", function (e) {
            if (!window.confirm($(this).data("onay"))) {
                e.preventDefault();
            }
        });

        // Mobilde kenar menüsünü aç/kapat (dışına tıklayınca ya da Esc ile kapanır)
        var $govde = $("body");
        var $dugme = $("#kpMenuToggle");
        function menu(acik) {
            $govde.toggleClass("kp-menu-acik", acik);
            $dugme.attr("aria-expanded", acik ? "true" : "false")
                  .attr("aria-label", acik ? "Menüyü kapat" : "Menüyü aç");
        }
        $dugme.on("click", function (e) {
            e.stopPropagation();
            menu(!$govde.hasClass("kp-menu-acik"));
        });
        $(document).on("click", function (e) {
            if ($govde.hasClass("kp-menu-acik") && !$(e.target).closest("#kpSidebar").length) {
                menu(false);
            }
        }).on("keydown", function (e) {
            if (e.key === "Escape" && $govde.hasClass("kp-menu-acik")) {
                menu(false);
                $dugme.trigger("focus");
            }
        });

        // Rozetlerde 0 ise gizle (CSS data-sayi ile)
        $(".kp-badge").each(function () {
            var sayi = $.trim($(this).text());
            $(this).attr("data-sayi", sayi).text(sayi === "0" ? "" : sayi);
        });
    });
})(jQuery);
