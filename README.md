# Vault-Redirector
Automated File Redirector & Compression Engine for Windows

**Current Status: Phase 4 (GUI - Electron)**

This project now includes a modern **Electron + React GUI** for managing file redirections.

## Features
- **Modern Dashboard:** View active rules and system status.
- **Auto-Scan:** One-click scan for common junk folders (Temp, Chrome, Spotify).
- **Manual Redirect:** Easily move and link folders.
- **System Tray:** Runs in the background (Windows/Linux).
- **Cross-Platform Core:** Logic written in Node.js (fs-extra, chokidar).

## How to Run (Development)
1. Navigate to `vault-redirector-gui`.
2. Install dependencies:
   ```bash
   npm install
   ```
3. Run the application:
   ```bash
   npm start
   ```
   *Note: In development mode, `main.js` expects the React dev server to be running on port 5173. You may need to run `npm run dev` in a separate terminal.*

## How to Build (Production)
1. Build the React app:
   ```bash
   npm run build
   ```
2. Start Electron (it will load the built files from `dist/`):
   ```bash
   npm start
   ```
