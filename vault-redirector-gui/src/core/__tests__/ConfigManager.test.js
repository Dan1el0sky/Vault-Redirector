const { ConfigManager } = require('../ConfigManager');
const fs = require('fs-extra');
const path = require('path');

jest.mock('fs-extra');

describe('ConfigManager', () => {
    let configManager;
    const mockRules = [{ appName: 'Test', sourcePath: 'C:\\Test', targetPath: 'D:\\Test' }];

    beforeEach(() => {
        configManager = new ConfigManager();
        jest.clearAllMocks();
    });

    test('loadRules returns empty array if file does not exist', async () => {
        fs.existsSync.mockReturnValue(false);
        const rules = await configManager.loadRules();
        expect(rules).toEqual([]);
    });

    test('loadRules returns parsed JSON', async () => {
        fs.existsSync.mockReturnValue(true);
        fs.readJson.mockResolvedValue(mockRules);
        const rules = await configManager.loadRules();
        expect(rules).toEqual(mockRules);
    });

    test('addRule saves new rule', async () => {
        fs.existsSync.mockReturnValue(true);
        fs.readJson.mockResolvedValue([]);
        fs.writeJson.mockResolvedValue();

        const newRule = { appName: 'New', sourcePath: 'C:\\New', targetPath: 'D:\\New' };
        await configManager.addRule(newRule);

        expect(fs.writeJson).toHaveBeenCalledWith(
            expect.any(String),
            [newRule],
            expect.objectContaining({ spaces: 2 })
        );
    });
});
