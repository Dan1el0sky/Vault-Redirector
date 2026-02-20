const fs = require('fs-extra');
const path = require('path');

class ConfigManager {
    constructor() {
        this.configPath = path.join(process.cwd(), 'rules.json');
    }

    async loadRules() {
        if (!fs.existsSync(this.configPath)) {
            return [];
        }
        try {
            const rules = await fs.readJson(this.configPath);
            return rules;
        } catch {
            return [];
        }
    }

    async addRule(rule) {
        const rules = await this.loadRules();
        // Check duplicate?
        const index = rules.findIndex(r => r.sourcePath === rule.sourcePath);
        if (index > -1) {
            rules[index] = rule; // Update
        } else {
            rules.push(rule);
        }
        await fs.writeJson(this.configPath, rules, { spaces: 2 });
    }
}

module.exports = { ConfigManager };
