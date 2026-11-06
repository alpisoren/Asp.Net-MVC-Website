<#
.SYNOPSIS
    Birden fazla sunucuda (web farm / yük dengeleyici) çalışırken gereken sabit <machineKey> öğesini üretir.

.DESCRIPTION
    Yönetim paneli oturum çerezi (.KurumsalPanel) ve CSRF belirteçleri makine anahtarıyla şifrelenir/imzalanır.
    Varsayılan olarak her sunucu kendi anahtarını üretir; bu durumda bir sunucuda açılan oturum diğerinde geçersizdir
    ve formlar "sahtecilik önleme belirteci" hatası verir. Tek sunucuda bu ayar GEREKMEZ.

    Üretilen öğeyi TÜM sunuculardaki Web.config'in <system.web> bölümüne aynı şekilde ekleyin.
    Anahtar gizlidir: depoya (git) koymayın; sunucuda elle ya da dağıtım aracınızın gizli değişkenleriyle ekleyin.

.EXAMPLE
    powershell -ExecutionPolicy Bypass -File tools\MachineKeyOlustur.ps1
#>
function Get-RastgeleHex([int]$bayt) {
    $veri = New-Object byte[] $bayt
    [System.Security.Cryptography.RandomNumberGenerator]::Create().GetBytes($veri)
    return ([System.BitConverter]::ToString($veri)).Replace('-', '')
}

$dogrulama = Get-RastgeleHex 64   # HMACSHA256 için 64 bayt
$sifreleme = Get-RastgeleHex 32   # AES-256 için 32 bayt

Write-Output "<machineKey validation=`"HMACSHA256`" validationKey=`"$dogrulama`" decryption=`"AES`" decryptionKey=`"$sifreleme`" />"
