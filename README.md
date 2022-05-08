# Webperf Core Leaderboard
Reads Webperf Core SQLite data onto a dashboard-like .NET 5.0 Blazor web page.

## UPDATE May 8th 2022!
I recommend looking at [my other leaderboard project](https://github.com/krompaco/webperf-omvp-rc-leaderboard) instead, it generates a static site and has examples on how to run the tests more stable and in more steps with GitHub Actions.

## Demo site
Take a look: [Demo site running on Azure](https://webperf-leaderboard-demo.azurewebsites.net)

## Getting started
1. Download or clone and set up [webperf_core](https://github.com/Webperf-se/webperf_core) on your machine.
2. Edit `appsettings.json` and match the path setting to your machine.
3. Run the WebApp application.

## Contribute or modify
1. [Download NSwag](https://github.com/RicoSuter/NSwag) and install so that you get the `nswag` command available on your system.
2. If you make API changes, run the PowerShell script in the repository root to regenerate the ApiClient class.

## Shoutouts
* [Webperf](https://webperf.se/)
* [Tailwind CSS](https://tailwindcss.com/)
* [Markdig](https://github.com/lunet-io/markdig)
* [NSwag](https://github.com/RicoSuter/NSwag)
