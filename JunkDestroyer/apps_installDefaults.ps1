
# First, get a list of all default microsoft apps and pipe it.
# The second part of the pipe loops through each app and installs it using its appxmanifest for configuration.
Get-AppXPackage -AllUsers | foreach {Add-AppxPackage -Register "$($_.InstallLocation)\appxmanifest.xml” -DisableDevelopmentMode}
#
# -DisableDeveloperMode makes windows think it is reinstalling corrupted packages.