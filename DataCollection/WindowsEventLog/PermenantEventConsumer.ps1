$computer = "mred1"
$filterNS = "root\cimv2"
$wmiNS = "root\subscription"
$query = "Select * from __InstanceModificationEvent within 10 where
  targetInstance isa 'Win32_Service' and targetInstance.Name = 'Bits'"
$filterName = "Win32ServiceModification"
$scriptFileName = "C:\fso\ToggleBrowserService.vbs"
$filterPath = Set-WmiInstance -Class __EventFilter `
 -ComputerName $computer -Namespace $filterNS -Arguments `
  @{name=$filterName; EventNameSpace=$filterNS; QueryLanguage="WQL";
    Query=$query}
$consumerPath = Set-WmiInstance -Class ActiveScriptEventConsumer `
 -ComputerName $computer -Namespace $wmiNS `
 -Arguments @{name="ToggleBits"; ScriptFileName=$scriptFileName;
  ScriptingEngine="VBScript"}
Set-WmiInstance -Class __FilterToConsumerBinding -ComputerName $computer `
  -Namespace $wmiNS -arguments @{Filter=$filterPath; Consumer=$consumerPath} |
  out-null