$ErrorActionPreference = "Stop"

function CheckLastExitCode {
  param ([int[]]$SuccessCodes = @(0), [scriptblock]$CleanupScript=$null)

  if ($SuccessCodes -notcontains $LastExitCode) {
      if ($CleanupScript) {
          "Executing cleanup script: $CleanupScript"
          &$CleanupScript
      }
      $msg = @"
EXE RETURNED EXIT CODE $LastExitCode
CALLSTACK:$(Get-PSCallStack | Out-String)
"@
      throw $msg
  }
}

$env:STOP_AFTER_FAIL="true"
$env:embedded="true"
$env:NODE_ENV="development"
$doteNetRelease = "net8.0"
