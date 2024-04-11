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
using LoginPI.Engine.ScriptBase.Constants;

public class ArcGISPro : ScriptBase
{
    void Execute()
    {
        string timestamp = DateTime.Now.ToString("HHmmssssssffff");
        StartTimer("_FindImage_"+timestamp);
        StopTimer("_FindImage_"+timestamp);
        
        timestamp = DateTime.Now.ToString("HHmmssssssffff");
        StartTimer("_FindImage_"+timestamp);
        StopTimer("_FindImage_"+timestamp);     
    
    }
}