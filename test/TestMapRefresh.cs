// TARGET:C:\Program Files\ArcGIS\Pro\bin\ArcGISPro.exe
// START_IN:C:\Program Files\ArcGIS\Pro\bin\
using LoginPI.Engine.ScriptBase;

public class ArcGISPro : ScriptBase
{
    void Execute() 
    {
        // Start the application
        START(mainWindowTitle: "ArcGIS Sign In", processName: "ArcGISPro");

        // Wait for the application to be ready
        WAIT(2);

        // Click the control
        STOP();
    }
}