using System;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

namespace Unminal.Utils.Diagnostics;

[SupportedOSPlatform("windows")]
public class PerformanceMonitor : IDisposable
{
    private const float UpdateIntervalSeconds = 0.5f;
    private readonly Stopwatch _updateTimer = new();

    // Системные объекты для CPU
    private TimeSpan _lastCpuTime = TimeSpan.Zero;
    private DateTime _lastTime = DateTime.MinValue;

    // Асинхронный поток для GPU
    private List<PerformanceCounter>? _gpuCounters;
    private Thread? _gpuThread;
    private bool _isRunning = true;

    // Кэшированные значения
    private double _cachedCpuUsage;
    private float _cachedGpuUsage;
    private float _cachedMemoryUsage;
    private bool _isMemoryInGb;

    public PerformanceMonitor()
    {
        _updateTimer.Start();
        InitGpuCounters();

        // ЗАПУСКАЕМ ТЯЖЕЛЫЙ ОПРОС GPU В ОТДЕЛЬНОМ ФОНОВОМ ПОТОКЕ
        if (_gpuCounters != null && _gpuCounters.Count > 0)
        {
            _gpuThread = new Thread(BackgroundGpuLoop)
            {
                IsBackground = true, // Поток закроется сам при выходе из игры
                Name = "Unminal_GPU_Profiler"
            };
            _gpuThread.Start();
        }
    }

    public string GetCPU()
    {
        UpdateMetricsIfTimePassed();

        string tag = "";
        if (_cachedCpuUsage >= 85.0) tag = "[#red]";
        else if (_cachedCpuUsage >= 60.0) tag = "[#orange]";

        return $"{tag}CPU: {_cachedCpuUsage:F1} %";
    }

    public string GetGPU()
    {
        // Метод больше ничего не считает, а просто мгновенно забирает 
        // данные, которые для него посчитал фоновый поток
        string tag = "";
        if (_cachedGpuUsage >= 85f) tag = "[#red]";
        else if (_cachedGpuUsage >= 60f) tag = "[#orange]";

        return $"{tag}GPU: {_cachedGpuUsage:F1} %";
    }

    public string GetMemory()
    {
        UpdateMetricsIfTimePassed();

        if (_isMemoryInGb)
        {
            return $"RAM: {_cachedMemoryUsage:F2} GB";
        }
        return $"RAM: {_cachedMemoryUsage:F1} MB";
    }

    private void UpdateMetricsIfTimePassed()
    {
        float elapsedSeconds = _updateTimer.ElapsedMilliseconds / 1000f;
        if (elapsedSeconds < UpdateIntervalSeconds && _lastTime != DateTime.MinValue) 
            return;

        _updateTimer.Restart();

        // CPU и RAM работают очень быстро, их можно оставить в основном потоке
        CalculateCpu();
        CalculateRam();
    }

    private void CalculateCpu()
    {
        DateTime now = DateTime.UtcNow;
        using Process process = Process.GetCurrentProcess();
        TimeSpan cpuTime = process.TotalProcessorTime;

        if (_lastTime != DateTime.MinValue)
        {
            double systemTimePassed = (now - _lastTime).TotalMilliseconds;
            double cpuTimePassed = (cpuTime - _lastCpuTime).TotalMilliseconds;

            if (systemTimePassed > 0)
            {
                _cachedCpuUsage = (cpuTimePassed / systemTimePassed) * 100.0 / Environment.ProcessorCount;
                if (_cachedCpuUsage > 100.0) _cachedCpuUsage = 100.0;
            }
        }

        _lastTime = now;
        _lastCpuTime = cpuTime;
    }

    /// <summary>
    /// БЕСКОНЕЧНЫЙ ЦИКЛ ФОНОВОГО ПОТОКА ДЛЯ GPU
    /// </summary>
    private void BackgroundGpuLoop()
    {
        while (_isRunning)
        {
            if (_gpuCounters != null)
            {
                try
                {
                    float totalUsage = 0f;
                    foreach (var counter in _gpuCounters)
                    {
                        totalUsage += counter.NextValue();
                    }
                    
                    // Записываем результат (операция атомарна для float)
                    _cachedGpuUsage = totalUsage > 100f ? 100f : totalUsage;
                }
                catch
                {
                    _cachedGpuUsage = 0f;
                }
            }

            // Усыпляем фоновый поток на 500 мс, чтобы он не грузил ядро процессора
            Thread.Sleep(500);
        }
    }

    private void CalculateRam()
    {
        using Process currentProcess = Process.GetCurrentProcess();
        float memoryInMb = currentProcess.WorkingSet64 / (1024f * 1024f);

        if (memoryInMb >= 1024f)
        {
            _cachedMemoryUsage = memoryInMb / 1024f;
            _isMemoryInGb = true;
        }
        else
        {
            _cachedMemoryUsage = memoryInMb;
            _isMemoryInGb = false;
        }
    }

    private void InitGpuCounters()
    {
        try
        {
            var category = new PerformanceCounterCategory("GPU Engine");
            var instanceNames = category.GetInstanceNames();

            _gpuCounters = instanceNames
                .Where(name => name.EndsWith("engtype_3D"))
                .SelectMany(name => category.GetCounters(name))
                .Where(counter => counter.CounterName == "Utilization Percentage")
                .ToList();

            _gpuCounters.ForEach(c => c.NextValue());
        }
        catch
        {
            _gpuCounters = null;
        }
    }

    public void Dispose()
    {
        // Останавливаем фоновый поток при закрытии игры
        _isRunning = false;
        _gpuThread?.Join(500); // Ждем завершения потока не более 500мс
    }
}
