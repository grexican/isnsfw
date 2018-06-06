dism.exe /Online /Enable-Feature:Microsoft-Hyper-V /All
dism.exe /Online /Enable-Feature:containers /All
bcdedit /set hypervisorlaunchtype auto 