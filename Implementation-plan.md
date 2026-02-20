🏗️ Project Title: Vault Redirector
Tagline: Automated File Redirector & Compression Engine for Windows "Digital Rot."

1. High-Level Architecture
To build this without the extreme complexity of writing a Windows Kernel Driver (which requires WHQL signing), we will use a High-Level File System Watcher combined with Symlinks/Junctions.

The Watcher: Monitors known "Junk" directories (AppData/Local/Temp, Chrome Cache, etc.).

The Mover: When a file/folder is created, the app moves it to the "Vault" (HDD) and leaves a Directory Junction behind so the original app thinks the files are still on the SSD.

The Compressor: Uses Windows native NTFS compression or CompactOS to shrink the files on the HDD.

2. The Repository Structure
Organizing your GitHub repo properly will help Jules understand the boundaries of the code.

Plaintext
/vault-redirector
├── /src
│   ├── /Core          # File system watcher & logic
│   ├── /Engine        # Symlink creation & Compression tools
│   ├── /UI            # System tray icon & Configuration dashboard
│   └── /Service       # Background Windows Service code
├── /config            # Default JSON rules for Chrome, Spotify, etc.
├── /docs              # API docs and architecture diagrams
└── README.md
3. Implementation Phases (The Roadmap)
Phase 1: The "Junction" Logic (MVP)
Before automating everything, you need the "Mover" to work.

Task: Create a script that takes a folder (e.g., Spotify Cache), moves it to D:\Vault\Spotify, and creates a Junction link at the original location.

Tech: System.IO (C#) or os module (Python). Using mklink /J via CLI is the easiest start.

Phase 2: The "Sentinel" (Real-time Watcher)
Task: Use FileSystemWatcher to monitor %TEMP% and %LOCALAPPDATA%.

The Challenge: You can't move files that are currently "in use" by an app.

Jules's Task: Help write a "Retry-with-Exponential-Backoff" logic to wait for an app to release a file handle before moving it.

Phase 3: The Rule Engine
Task: Define which folders are "Safe to Redirect." You don't want to move critical OS files to a slow HDD.

JSON Config Example:

JSON
{
  "app": "Chrome",
  "source": "%LOCALAPPDATA%/Google/Chrome/User Data/Default/Cache",
  "action": "Redirect",
  "compress": true
}
Phase 4: Transparency & UI
Task: A System Tray app that shows a "Savings Dashboard."

Metrics: "Moved 4.2GB to HDD," "Compressed 1.5GB into 800MB," "SSD Life Extended by 12%."

4. Technical Stack Recommendations
Since this is a Windows utility, I recommend:

Language: C# / .NET 8 (Best access to Windows APIs) or Rust (if you want it to be incredibly lightweight).

UI: WinUI 3 or WPF for a modern Windows look.

Background Ops: Windows Service (so it runs even if the UI is closed).
