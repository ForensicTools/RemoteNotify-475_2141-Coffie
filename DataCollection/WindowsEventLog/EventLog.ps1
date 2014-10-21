#Logon Event listeners
$LogonEventLog = "Select * From __InstanceCreationEvent Where
                TargetInstance ISA 'Win32_NTLogEvent' AND
                TargetInstance.LogFile='Security' AND
                (targetInstance.EventCode=4648 OR 
                targetInstance.EventCode=4648)"

#Register-WmiEvent -Query $LogonEventLog -Action { LogonEventTrigger }
#TODO: Must delete event on exit of powershell script

function LogonEventTrigger {

}
