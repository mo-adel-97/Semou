# بيطلع القيم المشفرة اللي بتتحط في SSTLI\Moduel\ServerConfig.vb
# الاستخدام:  powershell -ExecutionPolicy Bypass -File encrypt-server.ps1 -Server "IP\INSTANCE" -Database Semou -User semou_app -Password "xxx"
# الملف ده للمبرمج بس - متنزلوش عند العملاء
param(
    [Parameter(Mandatory)] [string]$Server,
    [Parameter(Mandatory)] [string]$Database,
    [Parameter(Mandatory)] [string]$User,
    [Parameter(Mandatory)] [string]$Password,
    [string]$OldDatabase = "",
    [string]$OldUser = "",
    [string]$OldPassword = ""
)
$sha = [System.Security.Cryptography.SHA256]::Create()
$key = $sha.ComputeHash([System.Text.Encoding]::UTF8.GetBytes("SEMOU#Srv@2026!k"))
function Enc([string]$v) {
    $aes = [System.Security.Cryptography.Aes]::Create()
    $aes.Key = $key
    $aes.GenerateIV()
    $bytes = [System.Text.Encoding]::UTF8.GetBytes($v)
    $body = $aes.CreateEncryptor().TransformFinalBlock($bytes, 0, $bytes.Length)
    [Convert]::ToBase64String($aes.IV + $body)
}
"    Private Const ENC_SERVER As String = `"$(Enc $Server)`""
"    Private Const ENC_DATABASE As String = `"$(Enc $Database)`""
"    Private Const ENC_USER As String = `"$(Enc $User)`""
"    Private Const ENC_PASSWORD As String = `"$(Enc $Password)`""
if ($OldDatabase) {
"    Private Const ENC_OLD_DATABASE As String = `"$(Enc $OldDatabase)`""
"    Private Const ENC_OLD_USER As String = `"$(Enc $OldUser)`""
"    Private Const ENC_OLD_PASSWORD As String = `"$(Enc $OldPassword)`""
}
