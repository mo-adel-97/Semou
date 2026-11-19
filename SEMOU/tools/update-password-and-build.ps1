# تحديث باسورد السيرفر في برنامج السمو + build جديد على Desktop\SEMOU_Build
# بيغيّر الباسورد بس (ENC_PASSWORD و ENC_OLD_PASSWORD) ويسيب السيرفر واسم الداتا بيز واليوزر زي ما هم.
# الاستخدام: كليك يمين > Run with PowerShell ، أو:
#   powershell -ExecutionPolicy Bypass -File update-password-and-build.ps1
# الملف ده للمبرمج بس - متنزلوش عند العملاء
$ErrorActionPreference = 'Stop'

$root = Split-Path -Parent $PSScriptRoot
$configPath = Join-Path $root 'SSTLI\Moduel\ServerConfig.vb'
$buildBat = Join-Path $root 'build-desktop.bat'

$secure = Read-Host 'اكتب باسورد السيرفر الجديد' -AsSecureString
$ptr = [Runtime.InteropServices.Marshal]::SecureStringToBSTR($secure)
try { $password = [Runtime.InteropServices.Marshal]::PtrToStringBSTR($ptr) }
finally { [Runtime.InteropServices.Marshal]::ZeroFreeBSTR($ptr) }

if ([string]::IsNullOrWhiteSpace($password)) { throw 'الباسورد فاضي' }

$sameForOld = Read-Host 'نفس الباسورد لقاعدة البيانات القديمة (SSTLI2023)؟ (y/n) [y]'
$oldPassword = $password
if ($sameForOld -eq 'n') {
    $secureOld = Read-Host 'اكتب باسورد القاعدة القديمة' -AsSecureString
    $ptr = [Runtime.InteropServices.Marshal]::SecureStringToBSTR($secureOld)
    try { $oldPassword = [Runtime.InteropServices.Marshal]::PtrToStringBSTR($ptr) }
    finally { [Runtime.InteropServices.Marshal]::ZeroFreeBSTR($ptr) }
}

# نفس تشفير ServerConfig.DEC و encrypt-server.ps1
$sha = [System.Security.Cryptography.SHA256]::Create()
$key = $sha.ComputeHash([System.Text.Encoding]::UTF8.GetBytes('SEMOU#Srv@2026!k'))
function Enc([string]$v) {
    $aes = [System.Security.Cryptography.Aes]::Create()
    $aes.Key = $key
    $aes.GenerateIV()
    $bytes = [System.Text.Encoding]::UTF8.GetBytes($v)
    $body = $aes.CreateEncryptor().TransformFinalBlock($bytes, 0, $bytes.Length)
    [Convert]::ToBase64String($aes.IV + $body)
}

$src = [IO.File]::ReadAllText($configPath, [Text.Encoding]::UTF8)
Copy-Item $configPath "$configPath.bak_$(Get-Date -Format yyyyMMdd_HHmmss)"

$src = [regex]::Replace($src, '(Private Const ENC_PASSWORD As String = ")[^"]*(")', { param($m) $m.Groups[1].Value + (Enc $password) + $m.Groups[2].Value })
$src = [regex]::Replace($src, '(Private Const ENC_OLD_PASSWORD As String = ")[^"]*(")', { param($m) $m.Groups[1].Value + (Enc $oldPassword) + $m.Groups[2].Value })

[IO.File]::WriteAllText($configPath, $src, (New-Object Text.UTF8Encoding($true)))
$password = $null; $oldPassword = $null
Write-Host 'تم تحديث الباسورد في ServerConfig.vb' -ForegroundColor Green

Write-Host 'جاري عمل build...' -ForegroundColor Cyan
& cmd /c "`"$buildBat`""
