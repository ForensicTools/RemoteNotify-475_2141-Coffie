################################################################################
#                               RemoteNotify.ps1                               #
#     Application to notify the user when their system is remotely accessed    #
#                Created by Caleb Coffie - CalebCoffie@gmail.com               #
################################################################################

#Imports
Add-Type -AssemblyName "System.Drawing"
Add-Type -AssemblyName "System.Windows.Forms"

#Declaration of various GUI objects
$NotificationIcon = New-Object Windows.Forms.NotifyIcon
$ContextMenu = New-Object System.Windows.Forms.ContextMenu
$MenuItem_ViewLog = New-Object System.Windows.Forms.MenuItem
$MenuItem_ViewTimeline = New-Object System.Windows.Forms.MenuItem

#Get Directory in which script is executing
$fullPathIncFileName = $MyInvocation.MyCommand.Definition
$currentScriptName = $MyInvocation.MyCommand.Name
$currentExecutingPath = $fullPathIncFileName.Replace($currentScriptName, "")

#Set Resources Directory
$resourcesDirectory = $currentExecutingPath + "Resources\"

#NotificationIcon Attributes setup
#Set NotifyIcon Icon
$Icon = New-Object System.Drawing.Icon($($resourcesDirectory + "icon.ico"))
$NotificationIcon.Icon = $Icon
$NotificationIcon.ContextMenu = $ContextMenu
$NotificationIcon.Visible = $True
$NotificationIcon.contextMenu.MenuItems.AddRange($MenuItem_ViewLog)
$NotificationIcon.contextMenu.MenuItems.AddRange($MenuItem_ViewTimeline)

#NotificationIcon set menu items
$MenuItem_ViewLog.Text = "Show logs"
$MenuItem_ViewLog.add_Click({ Invoke-Expression .\LogViewer.ps1 })
$MenuItem_ViewTimeline.Text = "View Timeline of Events"
$MenuItem_ViewTimeline.add_Click({ Invoke-Expression .\OpenTimeline })

#Start Data Collectors
Invoke-Expression .\DataCollection\StartDataCollectors.ps1

#TODO: Check for updates to log files
#Show notification if there is an update