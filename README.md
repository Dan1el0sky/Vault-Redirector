# Vault-Redirector
Automated File Redirector & Compression Engine for Windows

**Current Status: Phase 1 (MVP) - Core Mover Logic**

This utility allows you to move a folder from a source location (e.g., SSD) to a target location (e.g., HDD) and automatically replaces the source with a Directory Junction. This makes the operating system and applications believe the files are still in the original location, while they physically reside on the target drive.

## Features (Implemented)
- **Robust Redirection:** safely moves folders and creates junctions.
- **Cross-Volume Support:** Works across different drives (SSD to HDD).
- **Interactive Mode:** Prompts for paths if not provided via command line.
- **Logging:** Detailed crash and operation logs (`crash_log.txt`, `debug_engine.log`).

## Roadmap
- [x] Phase 1: The "Junction" Logic (MVP)
- [ ] Phase 2: The "Sentinel" (Real-time File Watcher for Temp/Cache folders)
- [ ] Phase 3: The Rule Engine (Configuration for specific apps)
- [ ] Phase 4: System Tray UI & Dashboard

## How to Run
1. Ensure you have the .NET 8 Runtime installed.
2. Run `build_and_run.bat` to build and start the application.
3. If no arguments are passed, follow the on-screen prompts to enter Source and Target paths.

## Development
- Run `run_tests.bat` to execute the test suite.
