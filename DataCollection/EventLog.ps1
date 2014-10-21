################################################################################
#                                 EventLog.ps1                                 #
#      Looks through event log for required events that it will trigger on     #
#                Created by Caleb Coffie - CalebCoffie@gmail.com               #
################################################################################

#For the WinRM Event you must continually querry with get-winevent
#This is because of the log that Winrm is stored in doesn't pass
#through WMI.

#Command for WinRM - Get-WinEvent Microsoft-Windows-WinRM/Operational
#Following command gives events for the past day
#Get-WinEvent -FilterHashTable @{LogName="Microsoft-Windows-WinRM/Operational"; StartTime=(Get-Date).AddDays(-1)}

################################################################################

while ($TRUE)
{
    $WinRMEvents = Get-WinEvent Microsoft-Windows-WinRM/Operational
    $($WinRMEvents.ToString()) | Out-File C:\WinRMEvents.txt -Append
    "################################################################################" | Out-File C:\WinRMEvents.txt -Append
    echo "test"
    Start-Sleep 60
}
