// TARGET:C:\Program Files\ArcGIS\Pro\bin\ArcGISPro.exe
// START_IN:C:\Program Files\ArcGIS\Pro\bin
using LoginPI.Engine.ScriptBase;
using System.Threading.Tasks;
using System;

public class TestMapRefresh3 : ScriptBase
{
    void Execute() 
    {
        START();
        Task.Run(() => 
        {
            Console.WriteLine("Running in a separate thread!");
        });
        
        
        Wait(2); 
        STOP();
    }
    
    //Step
    public class Step1 {
    }
}