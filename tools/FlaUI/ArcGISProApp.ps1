<#
    .SYNOPSIS
#>
Param (
     [Parameter(Mandatory=$true,Position=1)] [string]$ServerUrl,
     [Parameter(Mandatory=$true,Position=1)] [string]$Domain,
     [Parameter(Mandatory=$true,Position=1)] [string]$UserName,
     [Parameter(Mandatory=$true,Position=2)] [string]$Password,
     [Parameter(Mandatory=$true,Position=3)] [string]$DesktopName,
     [Parameter(Mandatory=$false)] [int]$SleepBeforeLogoff = 5,
     [Parameter(Mandatory=$false)] [int]$NumberOfRetries = 30,
     [Parameter(Mandatory=$false)] [string]$LogFilePath = "$env:TEMP",
     [Parameter(Mandatory=$false)] [string]$LogFileName = "ArcGISProApp_$($UserName.Replace('\','_')).log",
     [Parameter(Mandatory=$false)] [switch]$NoLogFile,
     [Parameter(Mandatory=$false)] [switch]$NoConsoleOutput
 )

$debug=$true

# Include Functions
if ($MyInvocation.MyCommand.CommandType -eq "ExternalScript"){
  $ScriptPath = Split-Path -Parent -Path $MyInvocation.MyCommand.Definition
}else{
  $ScriptPath = Split-Path -Parent -Path ([Environment]::GetCommandLineArgs()[0])
  if (!$ScriptPath){ $ScriptPath = "." }
}
Write-Host "Running from $ScriptPath"
#C:\Program Files\Login VSI\ViewDuo\Functions.ps1
. "$ScriptPath\Functions.ps1"

Write-Host "Loading Assemblies"
Load-Assemblies

################
# App Definition
################
$appProcessName = 'ArcGISPro'
$appMainWindowTitle = 'ArcGIS Pro'
$appExecutablePath = 'C:\Program Files\ArcGIS\Pro\bin\ArcGISPro.exe'
$appWorkingDirectory = 'C:\Program Files\ArcGIS\Pro\bin'
$appArgs = ''

###################
# Start Application
###################
#$appProcess = [Diagnostics.Process]::Start($appExecutablePath)
#$appProcess.WaitForInputIdle(5000) | Out-Null

if($debug){Write-Host "[DEBUG]: Launching App"}
$appProcess = Start-Process -FilePath $appExecutablePath -ArgumentList "--serverURL=""$ServerUrl"" --userName=""$Domain\$UserName"" --password=""$Password"" --domainName=""$Domain"" --desktopName=""$DesktopName""" -Passthru

#####################
# Get PID of instance
#####################
$appProcessId = ((Get-Process).where{ $_.id -eq $appProcess.Id })[0].Id
if($debug){Write-Host "[DEBUG]: App Launched PID: $appProcessId"}

###############################
# Set Automation Root (Desktop)
###############################
$uia = [FlaUI.UIA3.UIA3Automation]::new()
$cf = $uia.ConditionFactory
$desktopSession = $uia.GetDesktop()

##########################
# Get Main Window Instance
##########################
Wait-Action { $desktopSession.FindFirstDescendant($cf.ByProcessId($appProcessId)) -ne $null }
$mainWindow = $desktopSession.FindFirstDescendant($cf.ByProcessId($appProcessId))
if($debug){Write-Host "[DEBUG]: Main Window: "$mainWindow.Name}

#####################
# Wait for Connection
#####################
# Statuses:
# ConnectingView
# ConnectFailedView
# SecurIdPasscodeAuthView
# AuthenticatingView
# LaunchingView

$retryConnect = 0
while($retryConnect -le $NumberOfRetries){
start-sleep 3
  $desktopSession = $uia.GetDesktop()
  #$mainWindow = $desktopSession.FindFirstDescendant($cf.ByProcessId($appProcessId))
  Start-Sleep 2
  $statusPane = $mainWindow.FindFirstChild($cf.ByControlType("Custom"))
  if($statusPane.ClassName -eq "ConnectFailedView"){
    Write-Host "Status: "$statusPane.ClassName
    Write-Host "The VMware View Client failed to connect to Horizon Server and will be terminated."
    $appProcess.kill()
    Exit 1
  }elseif($statusPane.ClassName -eq "ConnectingView"){
    Write-Host "Status: "$statusPane.ClassName
    Write-Host "Waiting for the Vmware View Client to connect to the Horizon Server..."
    $retryConnect++
    start-sleep 1
  }elseif($statusPane.ClassName -eq "SecurIdPasscodeAuthView"){
    Write-Host "Status: "$statusPane.ClassName
    Write-Host "Waiting for the Vmware View Client to connect to the Horizon Server..."
    Break
  }elseif($statusPane.ClassName -eq "AuthenticatingView"){
    Write-Host "Status: "$statusPane.ClassName
    Write-Host "Waiting for the Vmware View Client to connect to the Horizon Server..."
    $retryConnect++
    start-sleep 1
  }else{
    Write-Host "Status: "$statusPane.ClassName
  }
}

 ###################
 # Password/Passcode
 ###################
 # SetValue on Edit "Password"
 Write-Host("Set Value of Edit 'Password'")
 $xpath_SetValueEditPassword = "/Custom[@ClassName='SecurIdPasscodeAuthView']/Edit[@ClassName='PasswordBox'][@Name='Password']"

 Wait-Action { $mainWindow.FindFirstByXPath($xpath_SetValueEditPassword) -ne $null }
 $winElem_SetValueEditPassword = $mainWindow.FindFirstByXPath($xpath_SetValueEditPassword)

 if ($winElem_SetValueEditPassword -ne $null){
     $winElem_SetValueEditPassword.Patterns.Value.Pattern.SetValue("$Password")
 }else{
     Write-Host("Failed to set element value  using xpath: $xpath_SetValueEditPassword")
     Exit 1
 }

 ##############
 # Login Button
 ##############
 # LeftClick on Button "Login"
 Write-Host("Click on Button 'Login'")
 $xpath_LeftClickButtonLogin = "/Custom[@ClassName='SecurIdPasscodeAuthView']/Button[@ClassName='Button'][@Name='Login']"
 $winElem_LeftClickButtonLogin = $mainWindow.FindFirstByXPath($xpath_LeftClickButtonLogin)
 if ($winElem_LeftClickButtonLogin -ne $null){
     $winElem_LeftClickButtonLogin.Click()
     $LASTERRORCODE
 }else{
     Write-Host("Failed to find element using xpath: $xpath_LeftClickButtonLogin")
     Exit 1
 }

 ########################
 # Wait for Session Login
 ########################
 # Statuses:
 # ConnectingView
 # ConnectFailedView
 # SecurIdPasscodeAuthView
 # AuthenticatingView
 # LaunchingView

 $retryConnect = 0
 while($retryConnect -le $NumberOfRetries){
   start-sleep 3
   $desktopSession = $uia.GetDesktop()
   $mainWindow = $desktopSession.FindFirstDescendant($cf.ByProcessId($appProcessId))
   $statusPane = $mainWindow.FindFirstChild($cf.ByControlType("Custom"))
   if($statusPane.ClassName -eq "ConnectFailedView"){
     Write-Host "Status: "$statusPane.ClassName
     Write-Host "The VMware View Client failed to connect to Horizon Server and will be terminated."
     $appProcess.kill()
     Exit 1
   }elseif($statusPane.ClassName -eq "ConnectingView"){
     Write-Host "Status: "$statusPane.ClassName
     Write-Host "Waiting for the Vmware View Client to connect to the Horizon Server..."
     $retryConnect++
     start-sleep 1
   }elseif($statusPane.ClassName -eq "SecurIdPasscodeAuthView"){
     Write-Host "Status: "$statusPane.ClassName
     Write-Host "Waiting for the Vmware View Client to connect to the Horizon Server..."
     Break
   }elseif($statusPane.ClassName -eq "AuthenticatingView"){
     Write-Host "Status: "$statusPane.ClassName
     Write-Host "Waiting for the Vmware View Client to connect to the Horizon Server..."
     $retryConnect++
     start-sleep 1
   }elseif($statusPane.ClassName -eq "LaunchingView"){
     Write-Host "Status: "$statusPane.ClassName
     Write-Host "Waiting for the Vmware View Client to connect to the Horizon Server..."
     $retryConnect++
     start-sleep 1
   }else{
     Write-Host "Status: "$statusPane.ClassName
     Break
   }
 }

 ##########
 #Close App
 ##########
 #$appProcess.Kill()
