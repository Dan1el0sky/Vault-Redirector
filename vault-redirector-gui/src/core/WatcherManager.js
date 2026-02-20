const fs = require('fs-extra');
const chokidar = require('chokidar');
const path = require('path');

class WatcherManager {
    constructor(configManager, redirector) {
        this.configManager = configManager;
        this.redirector = redirector;
        this.watchers = []; // { source: string, watcher: chokidar.FSWatcher }
    }

    async startAll() {
        await this.stopAll();
        const rules = await this.configManager.loadRules();

        rules.forEach(rule => {
            if (!rule.isEnabled) return;
            this.watchParent(rule);
        });
    }

    async stopAll() {
        this.watchers.forEach(w => w.watcher.close());
        this.watchers = [];
    }

    async watchParent(rule) {
        const sourcePath = rule.sourcePath;
        const parentPath = path.dirname(sourcePath);
        const folderName = path.basename(sourcePath);

        // We watch parent for creation of target folder
        if (!fs.existsSync(parentPath)) {
            console.log(`Parent path not found: ${parentPath}. Skipping ${rule.appName}`);
            return;
        }

        const watcher = chokidar.watch(parentPath, {
            ignored: (p) => {
                 // Ignore everything except the folder we care about
                 // This is tricky with Chokidar globs.
                 // Better to watch everything and filter event.
                 return false;
            },
            depth: 0,
            ignoreInitial: true,
            persistent: true
        });

        watcher.on('addDir', async (newPath) => {
            if (path.basename(newPath) === folderName) {
                console.log(`Detected creation of ${newPath}! Redirecting...`);
                // Wait a bit?
                await new Promise(r => setTimeout(r, 500));
                try {
                    await this.redirector.redirectFolder(newPath, rule.targetPath);
                    console.log(`Successfully redirected ${rule.appName}`);
                } catch (err) {
                    console.error(`Failed redirect: ${err.message}`);
                }
            }
        });

        this.watchers.push({ source: rule.sourcePath, watcher });
    }

    async restartAll() {
        await this.startAll();
    }
}

module.exports = { WatcherManager };
