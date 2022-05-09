# This PowerShell script creates two .csv files for storing information.
# We use installed_names.csv for displaying information to the user, and installed_fullNames.csv for the deletion process.

#create a new folder on the C: drive for our app lists. The C: drive allows us to access the lists from the GUI.
New-Item "C:\app-lists" -Type Directory


# 1. Retrieve all information about installed AppXPackages on the system
Get-AppXPackage |`

# 2. grab the names and packageFullNames
Select-Object -Property Name, PackageFullName |`

# 3. Write full names file
Export-Csv "C:\app-lists\\installed_full.csv"

# 4. Repeat steps for names file (only collect the names)
Get-AppXPackage |`

Select-Object -Property Name |`

Export-Csv "C:\app-lists\\installed_names.csv"