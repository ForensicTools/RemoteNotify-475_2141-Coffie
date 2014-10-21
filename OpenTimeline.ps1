################################################################################
#                               OpenTimeline.ps1                               #
#  Used by RemoteNotify to read the event logs and create them into a timeline #
#                Created by Caleb Coffie - CalebCoffie@gmail.com               #
################################################################################

#Read in Logs from csv files
$LogonEventLog = Import-Csv .\Logs\LogonEventLog.csv
$RunningProcessesLog = Import-Csv .\Logs\RunningProcessesLog.csv

#Must generate the data for the timeline first
#This is done by creating the Timeline\events.js file
$head = Get-Content .\Timeline\events-template-head.txt
$foot = Get-Content .\Timeline\events-template-foot.txt
$blankassets = Get-Content .\Timeline\events-template-blankassets.txt

#Array to hold events
$events = [System.Collections.ArrayList]@()

#Must iterate through each log now and add the them to the $events Array
Foreach ($event in $LogonEventLog)
{
    #TODO: Must still interate through these when we know what each contains
    $events.Add("`t`t`t{")
    $events.Add("`t`t`t`t`"startDate`":`"$STARTDATEVARIABLE`",") #ADD start date for event here
    $events.Add("`t`t`t`t`"endDate`":`"`",")
    $events.Add("`t`t`t`t`"headline`":`"$HEADLINEVARIABLE`",") #ADD headline here for the title of the event
    $events.Add("`t`t`t`t`"text`":`"$DETAILSVARIABLE`",") #ADD details of the event here
    $events += $blankassets
    $events.Add("`t`t`t},")
}
Foreach ($event in $RunningProcessesLog)
{
    #TODO: Must still interate through these when we know what each contains
    $events.Add("`t`t`t{")
    $events.Add("`t`t`t`t`"startDate`":`"$STARTDATEVARIABLE`",") #ADD start date for event here
    $events.Add("`t`t`t`t`"endDate`":`"`",")
    $events.Add("`t`t`t`t`"headline`":`"$HEADLINEVARIABLE`",") #ADD headline here for the title of the event
    $events.Add("`t`t`t`t`"text`":`"$DETAILSVARIABLE`",") #ADD details of the event here
    $events += $blankassets
    $events.Add("`t`t`t},")
}

$eventsOutFile = $head + $events + $foot

$eventsOutFile | Out-File -Force -FilePath .\Timeline\events.js

Invoke-Expression .\Timeline\timeline.html