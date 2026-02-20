const fs = require('fs-extra');
const path = require('path');
const { exec } = require('child_process');

class Redirector {
    async redirectFolder(source, target) {
        if (!fs.existsSync(source)) {
            throw new Error(`Source path not found: ${source}`);
        }
        if (fs.existsSync(target)) {
            // Check if target is same as source (avoid loop)
            if (path.resolve(source) === path.resolve(target)) {
                 throw new Error("Source and target cannot be the same.");
            }
             // For MVP, fail if target exists
             // Ideally we merge or overwrite
             throw new Error(`Target path already exists: ${target}`);
        }

        const parentTarget = path.dirname(target);
        if (!fs.existsSync(parentTarget)) {
            fs.mkdirpSync(parentTarget);
        }

        // 1. Move
        // Using rename works across same volume, copy+del for cross volume
        try {
            await fs.move(source, target, { overwrite: true });
        } catch (err) {
            throw new Error(`Failed to move folder: ${err.message}`);
        }

        // 2. Junction
        // fs.symlink(target, source, 'junction') works on Windows
        try {
            await fs.symlink(target, source, 'junction');
        } catch (err) {
            // Rollback? Move back?
            // For MVP, just log
            throw new Error(`Failed to create junction: ${err.message}`);
        }
    }
}

module.exports = { Redirector };
