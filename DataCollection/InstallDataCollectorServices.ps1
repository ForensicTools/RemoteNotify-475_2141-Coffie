################################################################################
#                       InstallDataCollectorServices.ps1                       #
#               Used once to install the data collection services              #
#                Created by Caleb Coffie - CalebCoffie@gmail.com               #
################################################################################


########################################
# Escalate Privileges to Administrator #
########################################
#Get the ID and security principal of the current user account
$myWindowsID=[System.Security.Principal.WindowsIdentity]::GetCurrent()
$myWindowsPrincipal=new-object System.Security.Principal.WindowsPrincipal($myWindowsID)
 
# Get the security principal for the Administrator role
$adminRole=[System.Security.Principal.WindowsBuiltInRole]::Administrator
 
# Check to see if we are currently running "as Administrator"
if !($myWindowsPrincipal.IsInRole($adminRole))
{
   # We are not running "as Administrator" - so relaunch as administrator
   
   # Create a new process object that starts PowerShell
   $newProcess = new-object System.Diagnostics.ProcessStartInfo "PowerShell";
   
   # Specify the current script path and name as a parameter
   $newProcess.Arguments = "& '" + $script:MyInvocation.MyCommand.Path + "'"
   
   # Indicate that the process should be elevated
   $newProcess.Verb = "runas";
   
   # Start the new process
   [System.Diagnostics.Process]::Start($newProcess);
   
   # Exit from the current, unelevated, process
   exit
}


#TODO: Following code is completely wrong
# Invoke-Expression .\EventLog.ps1
# Invoke-Expression .\NetworkConnections.ps1
