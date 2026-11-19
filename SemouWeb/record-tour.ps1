# Records a ~1.5 minute tour video of the Semou web app (test copy on port 5090).
# The password is asked here and is only passed to this one recording process; it is never saved.
$ErrorActionPreference = 'Stop'
$secure = Read-Host -AsSecureString 'كلمة مرور المستخدم sa'
$env:SEMOU_PASS = [Runtime.InteropServices.Marshal]::PtrToStringAuto([Runtime.InteropServices.Marshal]::SecureStringToBSTR($secure))
$env:SEMOU_USER = 'sa'
try {
  Push-Location 'C:\Users\ma109\AppData\Local\Temp\claude\26464384d1704772\1248759d-3e92-45f8-8d68-c30931165179\scratchpad\video'
  node tour.mjs
} finally {
  Pop-Location
  Remove-Item Env:SEMOU_PASS -ErrorAction SilentlyContinue
}
Write-Host 'تم حفظ الفيديو على: C:\Users\ma109\Desktop\Semou\جولة-نظام-السمو.mp4' -ForegroundColor Green
