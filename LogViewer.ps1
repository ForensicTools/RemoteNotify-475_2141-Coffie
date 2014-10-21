################################################################################
#                                 LogViewer.ps1                                #
#            Shows a GUI with all of the events the program collects           #
#                Created by Caleb Coffie - CalebCoffie@gmail.com               #
################################################################################

#Imports
Add-Type -AssemblyName "System.Drawing"
Add-Type -AssemblyName "System.Windows.Forms"

#Declaration of various GUI objects
$Logs_GUI = New-Object System.Windows.Forms.Form
$TabControl = New-Object System.Windows.Forms.TabControl
$LogonEventLog_Tab = New-Object System.Windows.Forms.TabPage
$LogonEventLog_Table = New-Object Windows.Forms.DataGridView
$RunningProcessesLog_Tab = New-Object System.Windows.Forms.TabPage
$RunningProcessesLog_Table = New-Object Windows.Forms.DataGridView

#Storage ArrayList used to populate the DataGridView
$LogonEventLog_GridData = New-Object System.Collections.ArrayList
$RunningProcessesLog_GridData = New-Object System.Collections.ArrayList

#Get Directory in which script is executing
$fullPathIncFileName = $MyInvocation.MyCommand.Definition
$currentScriptName = $MyInvocation.MyCommand.Name
$currentExecutingPath = $fullPathIncFileName.Replace($currentScriptName, "")

#Set Resources Directory
$resourcesDirectory = $currentExecutingPath + "..\Resources\"

#Main GUI Settings
$Logs_GUI.Text = "RemoteNotify"
$Logs_GUI.NAME = "RemoteNotify"
$Logs_GUI.Icon = New-Object System.Drawing.Icon($($resourcesDirectory + "icon.ico"))
#$GUI.AutoSize = $True
$Logs_GUI.ClientSize = '700,200'
$Logs_GUI.StartPosition = "CenterScreen"

#Set up Tab Pages
$LogonEventLog_Tab.Text = "Logon Events"
$RunningProcessesLog_Tab.Text = "Running Processes Log"

#Set up TabControl
$TabControl.TabPages.Add($LogonEventLog_Tab)
$TabControl.TabPages.Add($RunningProcessesLog_Tab)
$Logs_GUI.Controls.Add($TabControl)
$TabControl.Location = '10,10'
$TabControl.Size = '680,180'
$TabControl.Anchor = 'Top, Bottom, Left, Right'

#Logon Events Main Table View
$LogonEventLog_Tab.Controls.Add($LogonEventLog_Table)
$LogonEventLog_Table.Name = "Logon Events"
$LogonEventLog_Table.Anchor = 'Top, Bottom, Left, Right'
$LogonEventLog_Table.AllowUserToAddRows = $False
$LogonEventLog_Table.AllowUsertoDeleteRows = $False
$LogonEventLog_Table.ReadOnly = $True
$LogonEventLog_Table.ColumnHeadersHeightSizeMode = 'DisableResizing'
$LogonEventLog_Table.RowHeadersVisible = $False
$LogonEventLog_Table.SelectionMode = 'FullRowSelect'
$LogonEventLog_Table.MultiSelect = $False
$LogonEventLog_Table.AllowUserToResizeRows = $False
$LogonEventLog_Table.AllowUserToResizeColumns = $False

#Running Processes Events Main Table View
$RunningProcessesLog_Tab.Controls.Add($RunningProcessesLog_Table)
$RunningProcessesLog_Table.Name = "Logon Events"
$RunningProcessesLog_Table.Anchor = 'Top, Bottom, Left, Right'
$RunningProcessesLog_Table.AllowUserToAddRows = $False
$RunningProcessesLog_Table.AllowUsertoDeleteRows = $False
$RunningProcessesLog_Table.ReadOnly = $True
$RunningProcessesLog_Table.ColumnHeadersHeightSizeMode = 'DisableResizing'
$RunningProcessesLog_Table.RowHeadersVisible = $False
$RunningProcessesLog_Table.SelectionMode = 'FullRowSelect'
$RunningProcessesLog_Table.MultiSelect = $False
$RunningProcessesLog_Table.AllowUserToResizeRows = $False
$RunningProcessesLog_Table.AllowUserToResizeColumns = $False

#TODO: Must read in data from the logs

[void] $Logs_GUI.ShowDialog()