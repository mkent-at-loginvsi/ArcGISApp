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
using LoginPI.Engine.ScriptBase.Constants;

public class ArcGISPro : ScriptBase
{
    void Execute()
    {
        FindControlImageRefresh("1_10500_2");

    }
    
    private void CheckAppWindowStatus(string timerName){
        Log("===============================================================================");
        Log("= BEGIN: "+timerName);
        Stopwatch stopwatch = Stopwatch.StartNew();
        TakeScreenshot("PRE_"+timerName);
        StartTimer(timerName);        
        var appWindow = FindWindow(className : "Wpf Window:Window", title : "As-Built Editing - NYC - As-Built Pressure View - NYC - ArcGIS Pro",timeout:450,continueOnError:true);
        if(appWindow == null){
            ABORT("Unable to locate App Window.");
        }
        
        var refreshControl = FindAppControl(appWindow, "Button:Button", "Refresh", timerName+"_refreshcontrol", mapload_timeout, true);
        
        if (refreshControl == null){
            ABORT("Unable to locate Refesh Control");
        }
        
        StopTimer(timerName);
        TakeScreenshot("POST_"+timerName);
        stopwatch.Stop();
        long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
        int i = (int)elapsedMilliseconds;
        SetTimer("ELAPSED_MS_"+timerName, i);
        // Log or process the elapsed time as needed
        Console.WriteLine($"Task executed in {elapsedMilliseconds} milliseconds");
        Log("= END: "+timerName);
        Log("===============================================================================");
    }
}