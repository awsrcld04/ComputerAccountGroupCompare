
# ComputerAccountGroupCompare

DESCRIPTION: 
- Compares group membership between 2 computer accounts

> NOTES: "v1.0" was completed in 2011. 

## Requirements:

Operating System Requirements:
- Windows Server 2003 or higher (32-bit)
- Windows Server 2008 or higher (32-bit)

Additional software requirements:
Microsoft .NET Framework v3.5

Additional requirements:
Administrative access is required to perform operations by ComputerAccountGroupCompare


## Operation and Configuration:

Command-line parameters:
- run (Required parameter)
- ref:[ComputerName] (specify reference computer)
- check:[ComputerName] (specify computer to check)

Examples:
ComputerAccountGroupCompare -run -ref:Computer1 -check:Computer2
