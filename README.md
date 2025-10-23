Betty Slot Game Automation Framework (Spinberry - Irish Wilds)

This framework is built using C#, Reqnroll (Cucumber for .NET), NUnit, 
and Playwright (.NET), and follows a clean DI (Dependency Injection) architecture. 
It is designed to be easily scalable and supports execution across different profiles 
(Desktop and Mobile Emulation) via configuration in appsettings.json.

# Prerequisites

1. .NET 8.0 SDK or newer.

2. Visual Studio (or VS Code).

3. Playwright Browsers: Playwright binary files must be installed.

4. Install the extension Reqnroll for Visual Studio 22 

# Installation and Setup

1. Clone or unzip the project.

2. Restore dependencies: Open a command prompt in the project's root directory and run:

dotnet restore


3. Install Playwright Browsers: Playwright requires binary files for Chromium, Firefox, and Webkit.

# Run this once after the project has been built

dotnet build
pwsh bin/Debug/net8.0/playwright.ps1 install


Running the Tests
	Tests are executed using profiles defined in appsettings.json. 
	You can switch between profiles using the TEST_PROFILE environment variable.

1. Running in Desktop Profile (Default)
If TEST_PROFILE is not set, desktopChrome is used.

# Runs tests using Chromium (Desktop)
dotnet test

2. Running in Mobile Profile (Emulation)
Use TEST_PROFILE=mobileWebkit for iPhone 12 emulation.

PowerShell (Windows):

## $env:TEST_PROFILE="mobileWebkit"
dotnet test

*The supported profiles can be viewed in app settings.json.
Additional valid configurations may be added, adhering to the specified format.*

Bash/Zsh (Linux/macOS):

## TEST_PROFILE=mobileWebkit dotnet test ##

3. Parallel execution#
	- The parameters that take care of the parallel execution of the tests are found in:
	/Properties/AssemblyInfo.cs

	*Static parameters*

*[assembly: Parallelizable(ParallelScope.All)] - Sets the maximum number of threads that NUnit should use.
	0 means NUnit should use as many CPU cores as are available (recommended).*
*[assembly: LevelOfParallelism(4)]   Using this parameter here, we can set the number of 
	threads/workers statically by running the tests from 
	the terminal with the command: dotnet test*
* Dinamyc parameters *

	 ## cmd /c "set TEST_PROFILE=mobileWebkit && dotnet test -- NUnit.NumberOfTestWorkers=3"



	The command above will start the tests on mobileWebkit device(iPhone 12) on 3 TestWorkers *

Reporting Test Results
📸 Automatic Screenshots on Failure
When a test fails, the system automatically takes a full-page screenshot and saves it in:
	[ProjectDir]\bin\Debug\net8.0\reports\screenshots

📄 Generating a Detailed Report
Execution results are logged in HTML format. To generate a nice HTML report.

Path to the NUnit XML file:
	[ProjectDir]\bin\Debug\net8.0\reports\TestResults.xml

Architectural Overview
BDD/Test Runner: Reqnroll (using NUnit as the executor) provides the BDD structure (Gherkin) 
and manages the scenario lifecycle.

Dependency Injection (DI): Reqnroll (BoDi Container) automatically 
injects all core objects (IPage, AppConfig, IrishWildsPage) into class constructors.

Profiles: DIHooks.cs loads a configuration profile (desktop/mobile) from appsettings.json.

Mobile Emulation: BrowserManager.cs applies Device options when creating the IBrowserContext, 
ensuring tests run in a mobile environment.

Gameplay Validation: IrishWildsPage.cs encapsulates the logic for spinning, fetching the balance,
and validating that the balance is updated after a series of spins.
