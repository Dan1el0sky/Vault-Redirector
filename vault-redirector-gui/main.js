const { app, BrowserWindow, ipcMain, Tray, Menu } = require('electron');
const path = require('path');
const fs = require('fs-extra');
// Use require for compatibility with JS files
const { Redirector } = require('./src/core/Redirector');
const { WatcherManager } = require('./src/core/WatcherManager');
const { ConfigManager } = require('./src/core/ConfigManager');

let mainWindow;
let tray;
const configManager = new ConfigManager();
const redirector = new Redirector();
const watcherManager = new WatcherManager(configManager, redirector);

function createWindow() {
  mainWindow = new BrowserWindow({
    width: 900,
    height: 700,
    webPreferences: {
      nodeIntegration: true,
      contextIsolation: false // For simple IPC
    }
  });

  // Load the React app (dev or build)
  if (process.env.NODE_ENV === 'development') {
    mainWindow.loadURL('http://localhost:5173');
  } else {
    mainWindow.loadFile(path.join(__dirname, 'dist', 'index.html'));
  }

  // Create System Tray
  const iconPath = path.join(__dirname, 'public', 'icon.png'); // Placeholder path
  if (fs.existsSync(iconPath)) {
      tray = new Tray(iconPath);
      const contextMenu = Menu.buildFromTemplate([
        { label: 'Show Dashboard', click: () => mainWindow?.show() },
        { label: 'Quit', click: () => app.quit() }
      ]);
      tray.setToolTip('Vault Redirector');
      tray.setContextMenu(contextMenu);
  }

  mainWindow.on('closed', () => {
    mainWindow = null;
  });
}

app.on('ready', () => {
    createWindow();
    watcherManager.startAll(); // Auto-start watchers on launch
});

app.on('window-all-closed', () => {
  if (process.platform !== 'darwin') {
    app.quit();
  }
});

app.on('activate', () => {
  if (mainWindow === null) {
    createWindow();
  }
});

// IPC Handlers for React Frontend
ipcMain.handle('get-rules', async () => {
    return await configManager.loadRules();
});

ipcMain.handle('add-rule', async (event, rule) => {
    await configManager.addRule(rule);
    watcherManager.restartAll(); // Refresh watchers
    return true;
});

ipcMain.handle('scan-junk', async () => {
    const commonPaths = [
        { name: 'Windows Temp', path: process.env.TEMP || '' },
        { name: 'Chrome Cache', path: path.join(process.env.LOCALAPPDATA || '', 'Google/Chrome/User Data/Default/Cache') },
        { name: 'Spotify Cache', path: path.join(process.env.LOCALAPPDATA || '', 'Spotify/Storage') }
    ];

    const detected = [];
    for (const p of commonPaths) {
        if (p.path && await fs.pathExists(p.path)) {
            detected.push(p);
        }
    }
    return detected;
});

ipcMain.handle('manual-redirect', async (event, { source, target }) => {
    try {
        await redirector.redirectFolder(source, target);
        return { success: true };
    } catch (error) {
        return { success: false, error: error.message };
    }
});
