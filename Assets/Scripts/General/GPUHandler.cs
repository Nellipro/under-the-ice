using UnityEngine;
using System;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;

public class GPUHandler : MonoBehaviour
{
    // Request high-performance GPU as early as possible.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
    private static void RequestDedicatedGpu()
    {
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
        TryRequestHighPerformanceGpuWindows();
#elif UNITY_STANDALONE_LINUX && !UNITY_EDITOR
        TryRequestHighPerformanceGpuLinux();
#endif
    }
//Windows-specific GPU handling
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
    private enum GpuPreference : uint
    {
        Unspecified = 0,
        MinimumPower = 1,
        HighPerformance = 2
    }

    [DllImport("dxgi.dll", EntryPoint = "SetProcessDefaultGpuPreference", CallingConvention = CallingConvention.Winapi)]
    private static extern int SetProcessDefaultGpuPreference(GpuPreference gpuPreference);

    private static void TryRequestHighPerformanceGpuWindows()
    {
        SetProcessDefaultGpuPreference(GpuPreference.HighPerformance);
    }
#endif
//Linux-specific GPU handling
#if UNITY_STANDALONE_LINUX && !UNITY_EDITOR
    private const string LinuxRelaunchMarker = "UnderTheIce_DGPU_RELAUNCHED";

    private static void TryRequestHighPerformanceGpuLinux()
    {
        try
        {
            bool alreadyRelaunched = Environment.GetEnvironmentVariable(LinuxRelaunchMarker) == "1";
            bool changedEnvironment = false;

            // Cover common Linux hybrid GPU setups (Mesa + NVIDIA PRIME offload).
            changedEnvironment |= EnsureEnvVar("DRI_PRIME", "1");
            changedEnvironment |= EnsureEnvVar("__NV_PRIME_RENDER_OFFLOAD", "1");
            changedEnvironment |= EnsureEnvVar("__GLX_VENDOR_LIBRARY_NAME", "nvidia");
            changedEnvironment |= EnsureEnvVar("__VK_LAYER_NV_optimus", "NVIDIA_only");

            if (changedEnvironment && !alreadyRelaunched)
            {
                RelaunchSelfWithUpdatedEnvironment();
                return;
            }

         
        }
        catch (Exception ex)
        {
          
        }
    }
// Helper methods for Linux GPU handling
    private static bool EnsureEnvVar(string key, string value)
    {
        string current = Environment.GetEnvironmentVariable(key);
        if (!string.IsNullOrEmpty(current))
        {
            return false;
        }

        Environment.SetEnvironmentVariable(key, value);
        return true;
    }
// Relaunch the application with the updated environment variables for Linux GPU handling.
    private static void RelaunchSelfWithUpdatedEnvironment()
    {
        string executablePath = GetExecutablePath();
        if (string.IsNullOrEmpty(executablePath))
        {
          
            return;
        }

        string arguments = BuildCommandLineArguments(Environment.GetCommandLineArgs());
        Environment.SetEnvironmentVariable(LinuxRelaunchMarker, "1");
        // Start the new process with the updated environment variables.
        Process.Start(new ProcessStartInfo
        {
            FileName = executablePath,
            Arguments = arguments,
            UseShellExecute = false,
            WorkingDirectory = Environment.CurrentDirectory
        });

        // Exit the current process to allow the new process to take over.
        Application.Quit();
    }
// Get the path to the current executable for Linux GPU handling.
    private static string GetExecutablePath()
    {
        try
        {
            using (Process process = Process.GetCurrentProcess())
            {
                if (process.MainModule != null && !string.IsNullOrEmpty(process.MainModule.FileName))
                {
                    return process.MainModule.FileName;
                }
            }
        }
        catch
        {
            // Fall back to argv[0] below.
        }

        string[] args = Environment.GetCommandLineArgs();
        return args.Length > 0 ? args[0] : string.Empty;
    }
// Build the command line arguments for relaunching the application on Linux.
    private static string BuildCommandLineArguments(string[] args)
    {
        if (args == null || args.Length <= 1)
        {
            return string.Empty;
        }

        StringBuilder builder = new StringBuilder();
        for (int i = 1; i < args.Length; i++)
        {
            if (i > 1)
            {
                builder.Append(' ');
            }

            builder.Append(QuoteArg(args[i]));
        }

        return builder.ToString();
    }

    private static string QuoteArg(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return "\"\"";
        }

        if (value.IndexOfAny(new[] { ' ', '\t', '"' }) == -1)
        {
            return value;
        }

        return "\"" + value.Replace("\\", "\\\\").Replace("\"", "\\\"") + "\"";
    }
#endif

    void Awake()
    {
        // 1. Get the GPU name and convert it to lowercase
        string gpuName = SystemInfo.graphicsDeviceName.ToLower();
        

        // 2. Decide and apply settings based on the GPU vendor
        if (gpuName.Contains("nvidia"))
        {
            ApplyNvidiaSettings();
        }
        else if (gpuName.Contains("amd") || gpuName.Contains("radeon"))
        {
            ApplyAmdSettings();
        }
        else if (gpuName.Contains("intel"))
        {
            ApplyIntelSettings();
        }
        else
        {
            ApplyDefaultSettings();
        }
        Invoke(nameof(MainSceneLoader), 0.5f);
    }

    void ApplyNvidiaSettings()
    {
      
 
    }

    void ApplyAmdSettings()
    {
      
        // Example: Enable FSR or tweak async compute settings
    }

    void ApplyIntelSettings()
    {
      
        // Example: Lower some demanding features for integrated graphics
        // QualitySettings.globalTextureMipmapLimit = 1; 
    }

    void ApplyDefaultSettings()
    {
    }
    void MainSceneLoader()
    {
        SceneFadeLoader.LoadScene(1);
    }
}

