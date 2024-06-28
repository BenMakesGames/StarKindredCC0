# Star Kindred

This is the source code for Star Kindred, a browser-based kingdom-building game programmed in C#/ASP.NET Core and TypeScript/Angular.

I am no longer working on this project, or maintaining the original game.

Feel free to take this thing, and run with it. The code and graphics have been released to the public domain.

**🧚‍♀️ Hey, listen!** You may notice that a few graphics are missing or changed from the original game; those graphics came from another project of mine, and are not in the public domain.

---

If you'd like to support me recreationally giving away thousands of lines of code, consider buying one of my games on Steam, supporting me on Patreon, and/or telling others to do the same :)
* [Buy my games on Steam](https://store.steampowered.com/search/?developer=Ben%20Hendel-Doying)
* [Support me on Patreon](https://www.patreon.com/BenMakesGames)

## Running locally

**🧚‍♀️ Hey, listen!** I've tried to provide as much information as possible for how to run this game locally, however it's been a long time since I've had to run this from scratch, so some info may be missing. If you run into any issues, and find solutions, feel free to open a pull request to update these instructions!

### Pre-reqs

#### C# API Server

1. Install the .NET 7.0 or later SDK - https://dotnet.microsoft.com/en-us/download/dotnet/7.0
2. Install MariaDB or MySQL
   * If you dislike MySQL (fair), and are handy with C# and EF, feel free to use PostgreSQL, or whatever else; it shouldn't be hard to modify the application accordingly. Don't forget to create a new set of migrations!

3. Install the dotnet EF tools - run `dotnet tool install --global dotnet-ef` from anywhere

#### Angular Front-end

1. Install Node.js - https://nodejs.org/en
2. Install the Angular CLI tools - run `npm install -g @angular/cli` from anywhere
2. Run `npm install` from this directory

### Running locally

#### C# API Server

1. Copy `API/StarKindred.API/appsettings.Development.json.sample` as `appsettings.Development.json`, and fill in the blanks
   * Database settings are super-important
   * You can delete the "DiscordLogger" section for local config; that's more useful for when you actually publish to the web
2. From the `API/StarKindred.API` directory, run `dotnet ef database update --project=../StarKindred.Common/StarKindred.Common.csproj`
3. From the same directory, run `dotnet run`

#### Angular Front-end

1. Run the API, if you haven't already
2. Check the values in `src/environments/environment.ts`, and make sure you like their values
3. Run `ng serve`

## Developing

* Open `API/StarKindred.sln` with your C# IDE of choice (I like Rider, personally)
* Open the `WebClient/` directory with your webdev IDE of choice (I like WebStorm, personally)

## Publishing to the web

If you'd like to host a copy for many people to play, I had been running the C# API as an Azure app, and the Angular front-end out of an AWS S3 bucket with CloudFront. An unusual combo! There are a lot of ways to host an application like Star Kindred. Pick your favorite.

## Special thanks

The following people were [supporting me on Patreon](https://www.patreon.com/BenMakesGames) at the time I released StarKindred to the public domain - thanks, y'all! :)

* Andre Burgoyne
* aquaticFeline
* Cywir
* Drone
* Fo Relle
* Holly Mar
* JaneBuzJane
* Key Wheeler
* Rue Lew
* SevenEggs
* Silas Yeem

