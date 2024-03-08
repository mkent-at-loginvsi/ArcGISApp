// TARGET:C:\Program Files\ArcGIS\Pro\bin\ArcGISPro.exe
// START_IN:C:\Program Files\ArcGIS\Pro\bin

using System;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;
using Interop.UIAutomationClient;
using System.Runtime.InteropServices;
using LoginPI.Engine.ScriptBase;
using LoginPI.Engine.ScriptBase.Components;

public class TestMapRefresh2 : ScriptBase
{
    int mapload_timeout = 180;
    
    private readonly CUIAutomation8 automation = new CUIAutomation8();
    const string sourceDllPath = @"C:\Users\vsiadmin\Documents\GitHub\ArcGISApp\tools\FindImage\FindImageCpp.dll";
    
    void Execute() 
    {
        if (!File.Exists(ImageFinder.FindImageDllPath) ||
           File.GetLastWriteTime(sourceDllPath) > File.GetLastWriteTime(ImageFinder.FindImageDllPath))
        {
            Log($"Copying {ImageFinder.FindImageDllPath}");
            CopyFile(sourceDllPath, ImageFinder.FindImageDllPath);
        }
        START();

        STOP();
    }

    private void FindControlNativeAERefresh(string timerName){}
        // Find the App Window and prepare to click the refresh button
        
    private IWindow FindAppWindow(string className, string title, string timerName, int mapload_timeout, bool continueOnError){
    Log("===============================================================================");
    Log("= BEGIN: "+timerName);
    Stopwatch stopwatch = Stopwatch.StartNew();
    TakeScreenshot("PRE_"+timerName);
    StartTimer(timerName);
    IWindow appWindow = FindWindow(className : className, title: title, timeout: mapload_timeout, continueOnError: continueOnError);
    StopTimer(timerName);
    TakeScreenshot("POST_"+timerName);
    stopwatch.Stop();
    long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
    int i = (int)elapsedMilliseconds;
    SetTimer("ELAPSED_MS_"+timerName, i);
    // Log or process the elapsed time as needed
    Console.WriteLine($"Task executed in {elapsedMilliseconds} milliseconds");
    //return window;
    Log("= END: "+timerName);
    Log("===============================================================================");
    return appWindow;
    }

}

    