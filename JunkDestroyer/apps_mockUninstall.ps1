# Collect all information from installed-apps.csv into a variable '$installedApps'
$installedApps = import-CSV "$PSScriptRoot\installed.csv"

# Loop through all apps
foreach($app in $installedApps){
    
    #collect the name of the app and pipe it into the remove command
    #***-WhatIf makes this a mock execution. It does not actually run, it only pretends to.
    Get-AppXPackage $app.name | Remove-AppXPackage -WhatIf
}

# wait for 5 seconds
Start-Sleep -Seconds 5