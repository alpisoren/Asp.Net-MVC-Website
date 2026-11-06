<#
.SYNOPSIS
    Entity Framework 6 migration komutlarını .NET 10 projesinde çalıştırır (Visual Studio olmadan).

.EXAMPLE
    .\tools\Migration.ps1 list
    .\tools\Migration.ps1 add YeniAlanlar

.DESCRIPTION
    Önce projeyi derler, sonra EntityFramework paketinin içindeki ef6.dll aracını uygulamanın bağımlılıklarıyla çalıştırır.
    Bağlantı CorporateWebsite/appsettings.Development.json'dan (ya da ConnectionStrings__KurumsalDB ortam değişkeninden) okunur.
    "add" yeni migration dosyalarını CorporateWebsite/Migrations altına üretir; Up() metodunu mutlaka gözden geçirin.
    Visual Studio'da aynı işi Package Manager Console'dan "Add-Migration <Ad>" ile de yapabilirsiniz.
#>
param(
    [Parameter(Mandatory = $true)][ValidateSet('list', 'add')][string]$Komut,
    [string]$Ad
)
$ErrorActionPreference = 'Stop'
$proje = Join-Path $PSScriptRoot '..\CorporateWebsite' | Resolve-Path
Push-Location $proje
try {
    dotnet build CorporateWebsite.csproj -nologo -v:q | Out-Null
    if ($LASTEXITCODE -ne 0) { throw 'Derleme başarısız.' }

    [xml]$csproj = Get-Content CorporateWebsite.csproj
    $efSurum = ($csproj.Project.ItemGroup.PackageReference | Where-Object { $_.Include -eq 'EntityFramework' }).Version
    $paketler = Join-Path $env:USERPROFILE '.nuget\packages'
    $ef6 = Join-Path $paketler "entityframework\$efSurum\tools\net6.0\any\ef6.dll"
    $cikti = 'bin\Debug\net10.0'

    $ortak = @('--assembly', "$cikti\CorporateWebsite.dll", '--project-dir', "$proje\", '--root-namespace', 'CorporateWebsite',
               '--language', 'C#', '--data-dir', 'App_Data')
    $arac = @('exec', '--depsfile', "$cikti\CorporateWebsite.deps.json", '--runtimeconfig', "$cikti\CorporateWebsite.runtimeconfig.json",
              '--additionalprobingpath', $paketler, $ef6)

    switch ($Komut) {
        'list' { dotnet @arac migrations list @ortak }
        'add' {
            if (-not $Ad) { throw 'Migration adı verin: .\tools\Migration.ps1 add <Ad>' }
            dotnet @arac migrations add $Ad @ortak
            Write-Host "Üretilen dosyaları (Migrations\*_$Ad.cs) gözden geçirin; yeniden adlandırmalarda RenameColumn kullanın." -ForegroundColor Yellow
        }
    }
}
finally {
    Pop-Location
}
