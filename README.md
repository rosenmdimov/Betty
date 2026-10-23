# Betty – Slot Game Automation Framework (Spinberry – Irish Wilds)

BDD test automation for [spinberry.com](https://www.spinberry.com) and the *Irish Wilds* slot game, built with
**C#**, **Reqnroll** (Cucumber for .NET), **NUnit** and **Playwright for .NET**. It uses dependency injection
throughout and runs the same scenarios on desktop and emulated mobile devices, selected by profile.

## Prerequisites

- .NET 8.0 SDK or newer
- Visual Studio 2022 (with the **Reqnroll for Visual Studio 2022** extension) or VS Code
- PowerShell (`pwsh`) to install the Playwright browsers
- Node.js 20+ and the [Allure 3](https://allurereport.org/) CLI for the test report: `npm install -g allure`

## Installation and setup

```bash
git clone https://github.com/rosenmdimov/Betty.git
cd Betty
dotnet restore
dotnet build
pwsh bin/Debug/net8.0/playwright.ps1 install
```

The last command downloads the Chromium, Firefox and WebKit binaries Playwright needs. Run it again after
upgrading the `Microsoft.Playwright` package.

## Running the tests

Tests run with the profile from `appsettings.json`. Without `TEST_PROFILE`, `DefaultProfile` (`desktopChrome`) is used.

| Profile         | Browser  | Device emulation |
|-----------------|----------|------------------|
| `desktopChrome` | Chromium | –                |
| `mobileWebkit`  | WebKit   | iPhone 12        |
| `androidChrome` | Chromium | Pixel 7          |

```powershell
# PowerShell (Windows)
dotnet test                                  # desktopChrome
$env:TEST_PROFILE = "mobileWebkit"; dotnet test
```

```bash
# Bash/Zsh (Linux/macOS)
TEST_PROFILE=mobileWebkit dotnet test
```

### Overriding profile settings

Any profile value can be overridden with an environment variable named
`TestProfiles__<profile>__<setting>`, for example to run headless:

```powershell
$env:TestProfiles__desktopChrome__Headless = "true"; dotnet test
```

```bash
TestProfiles__desktopChrome__Headless=true dotnet test
```

Profile settings: `BaseUrl`, `Browser` (`chromium`, `firefox`, `webkit`), `Headless`, `Device`
(a [Playwright device name](https://playwright.dev/docs/emulation#devices)), `ElementTimeoutMs`, `NavigationTimeoutMs`.

### Running a subset

```bash
dotnet test --filter "Category=mobile"                        # by Gherkin tag
dotnet test --filter "FullyQualifiedName~IrishWildsGameplay"  # by feature
```

Tags only filter scenarios; the browser and device always come from the profile.

### Parallel execution

Features and scenarios run in parallel (`[assembly: Parallelizable(ParallelScope.All)]` in
`Properties/AssemblyInfo.cs`); every scenario gets its own browser. NUnit uses one worker per CPU core by default.
To change the number of workers for one run:

```bash
dotnet test -- NUnit.NumberOfTestWorkers=3
```

To fix it permanently, uncomment `[assembly: LevelOfParallelism(4)]` in `Properties/AssemblyInfo.cs`.

## Test results (Allure)

Results are recorded with [Allure.Reqnroll](https://allurereport.org/docs/reqnroll/). After every local run, an
Allure 3 report is generated and opened in the default browser.

| Output                        | Location                                        |
|-------------------------------|-------------------------------------------------|
| Allure report (single file)   | `bin/Debug/net8.0/allure-report/index.html`     |
| Raw Allure results            | `bin/Debug/net8.0/allure-results/`              |
| Run history (trends, flaky)   | `bin/Debug/net8.0/allure-history.jsonl`         |
| Screenshots of failed steps   | `bin/Debug/net8.0/reports/screenshots/`         |

The report shows for every scenario:

- each Gherkin step with its status and duration, and the error message and stack trace of the failing step
- **Failed** (assertion not met) vs **Broken** (exception such as a timeout) results
- grouping by epic → feature → story, plus severity, tags and Scenario Outline parameters
- the profile, browser version and device the scenario ran on (labels `profile`, `browser`, `device`)
- on failure: a full-page screenshot and the URL of **every open tab** (main page and game tab)
- trends and flaky tests across runs, from the history file (kept until `bin` is cleaned)

The report is a single self-contained `index.html`, so it can be opened from disk or shared as one file.
Report settings are in `allurerc.mjs`; adapter settings in `allureConfig.json`.

### Generating the report manually

When the `CI` environment variable is set (as on most build agents), the report is not generated or opened
automatically. Generate it from the output folder instead:

```bash
cd bin/Debug/net8.0
allure generate allure-results
```

Or let Allure run the tests and build the report in one go (writes `./allure-report` in the project root):

```bash
allure run -- dotnet test
```

### Report tags

Add these Gherkin tags to features or scenarios to organise the report:

| Tag                                         | Effect                    |
|---------------------------------------------|---------------------------|
| `@allure.epic:<Name>`                       | Epic (top-level group)    |
| `@allure.story:<Name>`                      | Story (group in a feature)|
| `@blocker` `@critical` `@normal` `@minor` `@trivial` | Severity         |
| `@allure.owner:<Name>`                      | Owner                     |
| `@allure.issue:<ID>` / `@allure.link:<URL>` | Links                     |

Tag values can't contain spaces.

## Scenarios

| Feature            | Scenario                                               |
|--------------------|--------------------------------------------------------|
| `IrishWilds.feature` | Can open Irish Wilds game (demo indicator is shown)  |
| `IrishWilds.feature` | Can verify that the initial amount is $2000          |
| `IrishWilds.feature` | Verify that the balance changes after 10 spins       |
| `MainPage.feature`   | Social media icons (Facebook, Instagram, LinkedIn) are displayed |

## Project structure

```
Configuration/  AppConfig – the profile model bound from appsettings.json
Features/       Gherkin .feature files (the .feature.cs code-behind is generated on build)
Hooks/          DIHooks – browser lifecycle, screenshots on failure, Allure report generation
Pages/          Page Objects: BasePage, MainPage, IrishWildsPage
Properties/     AssemblyInfo – NUnit parallel execution settings
Steps/          Step definitions
Support/        ScenarioState – state shared between the steps of one scenario
utils/          ConfigurationLoader (profile selection), BrowserManager (browser + device emulation)
allureConfig.json  Allure.Reqnroll adapter settings
allurerc.mjs       Allure 3 report settings (name, single-file output, history, grouping)
```

## Architecture overview

- **BDD / test runner:** Reqnroll, with NUnit as the executor, provides the Gherkin structure and the scenario lifecycle.
- **Dependency injection:** Reqnroll's container injects `IPage`, `AppConfig`, `ScenarioState` and the page objects into constructors.
- **Profiles:** `ConfigurationLoader` picks the profile (`TEST_PROFILE`, then `DefaultProfile`) once per run; `DIHooks` registers it for every scenario.
- **Mobile emulation:** `BrowserManager` applies the Playwright device settings (viewport, user agent, scale factor, touch) when creating the browser context.
- **Gameplay:** `IrishWildsPage` opens the game in a new tab, starts it, spins and reads the balance. Steps store balances in `ScenarioState` to compare them.
- **Reporting:** the Allure.Reqnroll plugin records steps, tags and parameters automatically; `DIHooks` adds the run labels and failure attachments and runs the Allure CLI after the test run.
