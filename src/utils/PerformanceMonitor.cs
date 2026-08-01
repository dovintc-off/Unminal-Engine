// Unminal Engine - Copyright (C) 2026 Dov1ntc
// Licensed under GNU AGPLv3 with No-Misattribution Addendum
// See LICENSE file for details.
using System.Diagnostics;

namespace Unminal.Utils.Diagnostics;

[SupportedOSPlatform("windows")]
public class PerformanceMonitor : IDisposable {
    private const float UpdateIntervalSeconds = 0.5f;
    private readonly Stopwatch _updateTimer = new();
    private TimeSpan _lastCpuTime = TimeSpan.Zero;
    private DateTime _lastTime = DateTime.MinValue;
    private List<PerformanceCounter>? _gpuCounters;
    private Thread? _gpuThread;
    private bool _isRunning = true;
    private double _cachedCpuUsage;
    private float _cachedGpuUsage;
    private float _cachedMemoryUsage;
    private bool _isMemoryInGb;

    public PerformanceMonitor() {
        _updateTimer.Start();
        InitGpuCounters();

        if (_gpuCounters != null && _gpuCounters.Count > 0) {
            _gpuThread = new Thread(BackgroundGpuLoop) {
                IsBackground = true, 
                Name = "Unminal_GPU_Profiler"
            };
            _gpuThread.Start();
        }
    }

    public string GetCPU() {
        UpdateMetricsIfTimePassed();

        string tag = "";
        if (_cachedCpuUsage >= 85.0) tag = "[#red]";
        else if (_cachedCpuUsage >= 60.0) tag = "[#orange]";

        return $"{tag}CPU: {_cachedCpuUsage:F1} %";
    }

    public string GetGPU() {
        string tag = "";
        if (_cachedGpuUsage >= 85f) tag = "[#red]";
        else if (_cachedGpuUsage >= 60f) tag = "[#orange]";

        return $"{tag}GPU: {_cachedGpuUsage:F1} %";
    }

    public string GetMemory() {
        UpdateMetricsIfTimePassed();
        if (_isMemoryInGb) return $"RAM: {_cachedMemoryUsage:F2} GB";
        return $"RAM: {_cachedMemoryUsage:F1} MB";
    }

    private void UpdateMetricsIfTimePassed() {
        float elapsedSeconds = _updateTimer.ElapsedMilliseconds / 1000f;
        if (elapsedSeconds < UpdateIntervalSeconds && _lastTime != DateTime.MinValue)  return;
        _updateTimer.Restart();
        CalculateCpu();
        CalculateRam();
    }

    private void CalculateCpu() {
        DateTime now = DateTime.UtcNow;
        using Process process = Process.GetCurrentProcess();
        TimeSpan cpuTime = process.TotalProcessorTime;

        if (_lastTime != DateTime.MinValue) {
            double systemTimePassed = (now - _lastTime).TotalMilliseconds;
            double cpuTimePassed = (cpuTime - _lastCpuTime).TotalMilliseconds;

            if (systemTimePassed > 0) {
                _cachedCpuUsage = (cpuTimePassed / systemTimePassed) * 100.0 / Environment.ProcessorCount;
                if (_cachedCpuUsage > 100.0) _cachedCpuUsage = 100.0;
            }
        }

        _lastTime = now;
        _lastCpuTime = cpuTime;
    }

    private void BackgroundGpuLoop() {
        while (_isRunning) {
            if (_gpuCounters != null) {
                try {
                    float totalUsage = 0f;
                    foreach (var counter in _gpuCounters)
                        totalUsage += counter.NextValue();
                    
                    _cachedGpuUsage = totalUsage > 100f ? 100f : totalUsage;
                } catch {
                    _cachedGpuUsage = 0f;
                }
            }

            Thread.Sleep(500);
        }
    }

    private void CalculateRam() {
        using Process currentProcess = Process.GetCurrentProcess();
        float memoryInMb = currentProcess.WorkingSet64 / (1024f * 1024f);

        if (memoryInMb >= 1024f) {
            _cachedMemoryUsage = memoryInMb / 1024f;
            _isMemoryInGb = true;
        } else {
            _cachedMemoryUsage = memoryInMb;
            _isMemoryInGb = false;
        }
    }

    private void InitGpuCounters() {
        try {
            var category = new PerformanceCounterCategory("GPU Engine");
            var instanceNames = category.GetInstanceNames();

            _gpuCounters = instanceNames
                .Where(name => name.EndsWith("engtype_3D"))
                .SelectMany(name => category.GetCounters(name))
                .Where(counter => counter.CounterName == "Utilization Percentage")
                .ToList();

            _gpuCounters.ForEach(c => c.NextValue());
        } catch {
            _gpuCounters = null;
        }
    }

    public void Dispose() {
        _isRunning = false;
        _gpuThread?.Join(500);
    }
}

