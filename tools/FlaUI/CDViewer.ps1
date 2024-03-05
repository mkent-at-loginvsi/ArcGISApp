<#
    .SYNOPSIS
#>
Param (
     [Parameter(Mandatory=$true,Position=1)] [string]$UserName,
     [Parameter(Mandatory=$true,Position=2)] [string]$Password,
     [Parameter(Mandatory=$false)] [int]$SleepBeforeLogoff = 5,
     [Parameter(Mandatory=$false)] [int]$NumberOfRetries = 30,
     [Parameter(Mandatory=$false)] [string]$LogFilePath = "$env:TEMP",
     [Parameter(Mandatory=$false)] [string]$LogFileName = "CDViewer_$($UserName.Replace('\','_')).log",
     [Parameter(Mandatory=$false)] [switch]$NoLogFile,
     [Parameter(Mandatory=$false)] [switch]$NoConsoleOutput
 )

# Settings
$debug=$true
$TessdataDirectory = Resolve-Path "$pwd\lib\tessdata"

# Main Function
Function Main {
    #####################
    # Init
    #####################
    if($debug){Write-Host "Loading Assemblies"}
    Load-Assemblies
    
    if($debug){Write-Host "Adding Assembly Types"}
    Add-Types

    ################
    # App Definition
    ################
    $appProcessName = 'CDViewer'
    
    #####################
    # Get PID of instance
    #####################
    #$appProcessId = ((Get-Process).where{ $_.id -eq $appProcess.Id })[0].Id
    Wait-Action { (Get-Process -Name $appProcessName | ?{$_.StartTime -le [datetime]::Now -And $_.StartTime -ge [datetime]::Today.AddSeconds(-10)}) -ne $null } -Timeout 10
    $appProcess = (Get-Process -Name $appProcessName | ?{$_.StartTime -le [datetime]::Now -And $_.StartTime -ge [datetime]::Today.AddSeconds(-10)})
    $appProcessId = $appProcess.Id 
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

    
    ##########################
    # Evaluate Window Contents
    ##########################
    $retryConnect = 0
    while($retryConnect -le $NumberOfRetries){
        $imageName = [guid]::NewGuid().ToString() + '.bmp'
        $mainWindow.CaptureToFile("$pwd\$imageName")
        $tesseract = New-Object Tesseract.TesseractEngine($TessdataDirectory, "eng", [Tesseract.EngineMode]::Default, $null)
        $image = $tesseract.Process([Tesseract.PixConverter]::ToPix([System.Drawing.Bitmap]::FromFile("$pwd\$imageName")))

        $text = $image.GetText()
        $confidence = $image.GetMeanConfidence()
        #$text
        $confidence

        Write-Host "Looking for $UserName"
        #Write-Host "$text"
        <#Switch ($text)
	    {
            ($text -like "*connecting*") { Write-Host "Waiting for Desktop Viewer to Connect"}
            $text.Contains($UserName.Substring(0,$username.Length)) { Write-Host "Username Match"; Send-Keys  }
            $text.Contains($UserName.Substring(0,($username.Length -1))) { Write-Host "Username Match"; Send-Keys }
            $text.Contains($UserName.Substring(1,($username.Length -1))) { Write-Host "Username Match"; Send-Keys }
            Default { Write-Host "Unable to match username"}
        }
        #>

        # This is due to Powershell OCR that are unable to retrurn confidence scores
        # Sensitive passwords can be exposed in the username field e.g.

        #username is the field label before type
        if($text.Contains($UserName)){
            Write-Host "Username Match"
            Send-Keys
            Break
        }
        $retryConnect++
        Start-Sleep 1
        
    }

    
    if($debug){
        #pause
    }

    $image.Dispose()
    $image = $null
    $imageName = $null
    $mainWindow = $null

    
    Start-Sleep 10
    while(Get-ChildItem -Path "$(pwd)" *.bmp){
        Start-Sleep 10
        Get-ChildItem -Path "$(pwd)" *.bmp | foreach { Remove-Item -Path $_.FullName -ErrorAction SilentlyContinue } 
    }
    

}

# Functions
Function Load-Assemblies {
    $libs = @(
        "$(pwd)\lib\FlaUI.UIA3.dll",
        #"$(pwd)\lib\FlaUI.UIA2.dll",
        "$(pwd)\lib\FlaUI.Core.dll",
        "$(pwd)\lib\Interop.UIAutomationClient.dll",
        "$(pwd)\lib\Tesseract.dll"
    )

    foreach($lib in $libs){
        $FileToLoad = $lib

        if (-not (Test-Path "$FileToLoad" -PathType Leaf))
        {
            Throw "Can't find or open file ${FileToLoad}."

        }
        else{
            Write-Host "Loading Library: $FileToLoad"
            [System.Reflection.Assembly]::LoadFrom($FileToLoad) | Out-Null
        }

    }
}

Function Add-Types {
    Add-Type -AssemblyName System.Windows.Forms
    Add-Type -AssemblyName "System.Drawing"
}

function Wait-Action {
    [OutputType([void])]
    [CmdletBinding()]
    param
    (
        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty()]
        [scriptblock]$Condition,
  
        [Parameter()]
        [ValidateNotNullOrEmpty()]
        [int]$Timeout = 30,
  
        [Parameter()]
        [ValidateNotNullOrEmpty()]
        [object[]]$ArgumentList,
  
        [Parameter()]
        [ValidateNotNullOrEmpty()]
        [int]$RetryInterval = 1
    )
  
      try {
          $timer = [Diagnostics.Stopwatch]::StartNew()
          while (($timer.Elapsed.TotalSeconds -lt $Timeout) -and (-not (& $Condition $ArgumentList))) {
              Start-Sleep -Seconds $RetryInterval
              $totalSecs = [math]::Round($timer.Elapsed.TotalSeconds, 0)
              Write-Verbose -Message "Still waiting for action to complete after [$totalSecs] seconds..."
          }
          $timer.Stop()
          if ($timer.Elapsed.TotalSeconds -gt $Timeout) {
              throw 'Action did not complete before timeout period.'
          } else {
              Write-Verbose -Message 'Action completed before timeout period.'
          }
      } catch {
          Write-Error -Message $_.Exception.Message
      }
  }
Function Send-Keys {
	$mainWindow.SetForeground()
	$wshell = New-Object -ComObject wscript.shell;
    $wshell.SendKeys("$Password{ENTER}")
}

Main