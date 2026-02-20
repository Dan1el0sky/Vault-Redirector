import React, { useState, useEffect } from 'react';
import './styles/App.css';
const { ipcRenderer } = window.require('electron');

function App() {
  const [rules, setRules] = useState([]);
  const [scanning, setScanning] = useState(false);
  const [scanResults, setScanResults] = useState([]);

  useEffect(() => {
    loadRules();
  }, []);

  const loadRules = async () => {
    const data = await ipcRenderer.invoke('get-rules');
    setRules(data);
  };

  const handleScan = async () => {
    setScanning(true);
    setScanResults([]);
    try {
      const detected = await ipcRenderer.invoke('scan-junk');
      setScanResults(detected);
    } catch (err) {
      console.error(err);
    } finally {
      setScanning(false);
    }
  };

  const handleAddRule = async (preset) => {
    const rule = {
      appName: preset.name,
      sourcePath: preset.path,
      targetPath: `D:\\Vault\\${preset.name.replace(/\s/g, '')}`,
      isEnabled: true
    };
    await ipcRenderer.invoke('add-rule', rule);
    loadRules();
    // Remove from scan results visually
    setScanResults(scanResults.filter(r => r.path !== preset.path));
  };

  const ManualRedirect = async (e) => {
    e.preventDefault();
    const source = e.target.source.value;
    const target = e.target.target.value;
    const result = await ipcRenderer.invoke('manual-redirect', { source, target });
    if (result.success) {
      alert('Success! Folder redirected.');
    } else {
      alert(`Error: ${result.error}`);
    }
  };

  return (
    <div className="container">
      <header className="header">
        <h1>Vault Redirector</h1>
        <div>
          <button className="btn btn-secondary" onClick={handleScan}>
            {scanning ? 'Scanning...' : 'Scan for Junk'}
          </button>
          <button className="btn" onClick={loadRules}>Refresh</button>
        </div>
      </header>

      {/* Scan Results Area */}
      {scanResults.length > 0 && (
        <div className="card">
          <h2 className="card-title">Detected Junk Folders</h2>
          <table>
            <thead>
              <tr>
                <th>App Name</th>
                <th>Path</th>
                <th>Action</th>
              </tr>
            </thead>
            <tbody>
              {scanResults.map((result, idx) => (
                <tr key={idx}>
                  <td>{result.name}</td>
                  <td>{result.path}</td>
                  <td>
                    <button className="btn btn-sm" onClick={() => handleAddRule(result)}>
                      Redirect
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      {/* Manual Redirect Form */}
      <div className="card">
        <h2 className="card-title">Manual Redirect</h2>
        <form onSubmit={ManualRedirect}>
          <div style={{ display: 'flex', gap: '10px' }}>
            <input type="text" name="source" placeholder="Source Path (e.g. C:\Temp)" required />
            <input type="text" name="target" placeholder="Target Path (e.g. D:\Vault\Temp)" required />
            <button type="submit" className="btn">Move & Link</button>
          </div>
        </form>
      </div>

      {/* Active Rules List */}
      <div className="card">
        <h2 className="card-title">Active Rules (Watchers)</h2>
        {rules.length === 0 ? (
          <p>No active rules. Add one or scan for junk.</p>
        ) : (
          <table>
            <thead>
              <tr>
                <th>App Name</th>
                <th>Source</th>
                <th>Target</th>
                <th>Status</th>
              </tr>
            </thead>
            <tbody>
              {rules.map((rule, idx) => (
                <tr key={idx}>
                  <td>{rule.appName}</td>
                  <td>{rule.sourcePath}</td>
                  <td>{rule.targetPath}</td>
                  <td>
                    <span className="status-badge">
                      {rule.isEnabled ? 'Active' : 'Disabled'}
                    </span>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>
    </div>
  );
}

export default App;
