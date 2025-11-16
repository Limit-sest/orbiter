<img width="800" height="500" alt="header" src="https://github.com/user-attachments/assets/84c052ea-7695-46ac-a1a0-94774008de98" />

# orbiter
A simple real-time solar system simulator that runs in your terminal. It renders planets, their orbits, and their moons using unicode characters.

## Showcase video
https://github.com/user-attachments/assets/2e079610-25fc-4591-b9d5-1e152bb650c1

## Features
- **Solar system**: simulates all 8 planets including their moons
- **Time control**: allows changing the speed of the simulation and pausing it
- **Selectable rendering**: you can choose what features you want to be rendered

## How to run
### a) Download binary
1. Go to [the releases section](https://github.com/Limit-sest/orbiter/releases/latest) and download the executable file for your operating system
2. Open your downloads location in a modern terminal (Windows terminal, iTerm2, or any other)
3. Run the executable from the terminal, usually with `./orbiter-<platform>-x64`
### b) Build it yourself
Note: **requires to have .NET 8.0 installed**

1. Clone this repository
```bash
git clone https://github.com/Limit-sest/orbiter.git
```
2. Go into the repo
```bash
cd orbiter
```
3. Run the code
```bash
dotnet run
```

## Controls
|Key|Action|
|---|------|
|<kbd>q</kbd>/<kbd>esc</kbd>|Quit the app|
|<kbd>←</kbd>|Decrease speed|
|<kbd>→</kbd>|Increase speed|
|<kbd>space</kbd>|Pause|
|<kbd>l</kbd>|Show/hide labels|
|<kbd>m</kbd>|Show/hide moons|
