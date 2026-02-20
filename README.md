# Vault-Redirector
Automated File Redirector & Compression Engine for Windows

**Current Status: Phase 3 Complete (Scanning & Config)**

This utility allows you to move a folder from a source location (e.g., SSD) to a target location (e.g., HDD) and automatically replaces the source with a Directory Junction.

## Features (Implemented)
- **Robust Redirection:** safely moves folders and creates junctions.
- **Auto-Scan:** Detects common junk folders (Windows Temp, Chrome Cache, Spotify) and suggests redirection rules.
- **Watcher Mode:** Monitors parent folders and automatically redirects target folders upon creation.
- **Configurable:** Saves rules to `rules.json`.
- **Interactive Console UI:** Wizard-style menu for adding rules and scanning.

## Roadmap
- [x] Phase 1: The "Junction" Logic (MVP)
- [x] Phase 2: The "Sentinel" (Real-time File Watcher)
- [x] Phase 3: The Rule Engine & Scanner
- [ ] Phase 4: Native GUI (WPF/WinUI) & System Tray

## How to Run
1. Ensure you have the .NET 8 Runtime installed.
2. Run `build_and_run.bat`.
3. Choose an option from the menu:
    - **1. Manual Redirect:** Move a folder once.
    - **2. Add Rule:** Create a custom rule.
    - **3. Run Watcher Mode:** Start monitoring based on saved rules.
    - **4. Scan for Junk Folders:** Automatically detect and add common junk folders.

## Development
- Run `run_tests.bat` to execute the test suite.
- **WPF GUI Note:** A native GUI requires a Windows environment to build. The current Console UI serves as a fully functional dashboard.
