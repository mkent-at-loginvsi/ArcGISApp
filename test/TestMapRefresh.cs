// TARGET:C:\Program Files\ArcGIS\Pro\bin\ArcGISPro.exe
// START_IN:C:\Program Files\ArcGIS\Pro\bin\
using System.Diagnostics.Tracing;
using System.Runtime.CompilerServices;
using LoginPI.Engine.ScriptBase;
using LoginPI.Engine.ScriptBase.Components;

public class ArcGISPro : ScriptBase
{
    void Execute() 
    {
        // Start the application
        string STEP_NAME = "";
        START();
        
        // ===============================================================================
        // Timer: OpenMainWindow
        // ===============================================================================
        STEP_NAME = "OpenMainWindow";
        StartTimer(STEP_NAME);
        var openMapWindow = FindWindow(title: "ArcGIS Pro");
        if (openMapWindow == null)
        {
            CancelTimer(STEP_NAME);
        }
        else 
        {
            StopTimer(STEP_NAME);
            //openMapWindow.Maximize();
        }
        // ===============================================================================
        
        // ===============================================================================
        // Timer: OpenProject
        // ===============================================================================
        STEP_NAME = "OpenProject";
        StartTimer(STEP_NAME);
        var openProjectBtn = openMapWindow.FindControl(className: "Button:Button", title: "Open another project");
        if (openProjectBtn == null)
        {
            CancelTimer(STEP_NAME);
        }
        else 
        {
            StopTimer(STEP_NAME);
            openProjectBtn.Click();
            var MyOrgWindow= FindWindow(className : "Wpf Window:Window", title : "Open Project", processName : "ArcGISPro",timeout: 10);
            MyOrgWindow.FindControl(className : "Text:TextBlock", title : "MyProject").Click();
            MyOrgWindow.FindControl(className : "Edit:TextBox", title : "Name Text Box").Type("MyProject.aprx"+"{Enter}");
        }
        // ===============================================================================
        
        var mapWindow = FindWindow(className : "Wpf Window:Window", title : "MyProject");
        if (mapWindow == null)
        {
            Log("FAIL");
        }
        else 
        {
            Log("SUCCESS");
        }
        
        // ===============================================================================
        // Timer: WaitForRefresh
        // ===============================================================================    
        STEP_NAME = "WaitForRefresh";
        StartTimer(STEP_NAME);
        var refreshwheel = mapWindow.FindControl(className: "Button:Button", title: "Refresh", timeout: 200, continueOnError: true);
        if (refreshwheel == null)
        {
            CancelTimer(STEP_NAME);
            Log("FAIL");
        }
        else 
        {
            StopTimer(STEP_NAME);
            Log("SUCCESS");
        }
        
        // Wait for the application to be ready
        Wait(30);
        
        // ===============================================================================
        // Timer: Refresh_Map
        // ===============================================================================
        MeasureMapRefreshValues(mapWindow: mapWindow, timerName: "Refresh_Map", mapload_timeout: 30);
        // ===============================================================================
        
        Wait(10);
        
        // Click the control
        STOP();
    }
    
    private void MeasureMapRefreshValues(IWindow mapWindow, string timerName, int mapload_timeout) {

        StartTimer(timerName);
        
        var paneBelowMapContainingRefreshWheel = mapWindow.FindControl(className: "Button:Button", title: "Refresh", timeout: mapload_timeout, continueOnError: true);
        
        //var clickRefresh = paneBelowMapContainingRefreshWheel.FindControl(className: "Button:Button", title: "Refresh", timeout: mapload_timeout, continueOnError: true);
        
        if(paneBelowMapContainingRefreshWheel != null)
        {
            Log("Clicking Refresh");
            paneBelowMapContainingRefreshWheel.Click();
            Wait(5);
        }
        
        mapWindow.FindControl(className : "Button:Button", title : "Drawing.  Click to cancel.", timeout: mapload_timeout);
        
        var refreshwheel = paneBelowMapContainingRefreshWheel.FindControl(className: "Button:Button", title: "Refresh", timeout: mapload_timeout, continueOnError: true);
        
        if (refreshwheel == null)
        {
            CancelTimer(timerName);
        }
        else 
        {
            StopTimer(timerName);
        }
    }
    
    private void MeasureMapRefreshValuesXpath(IWindow mapWindow, string timerName, int mapload_timeout) {

        StartTimer(timerName);
        mapWindow.FindControl(className: "Image:Image", timeout: 15, continueOnError: true);

        var paneBelowMapContainingRefreshWheel = mapWindow.FindControlWithXPath(xPath : "Group:FrameworkDockSite/Group:DockHost/Group:SplitContainer/Pane:Workspace/Tab:TabbedMdiContainer/TabItem:DockingWindowContainerTabItem/Pane:DocumentWindow/Custom:MapPaneView/Custom:MapCoordinateReadoutControl", timeout: 200);
        mapWindow.FindControl(className : "Button:Button", title : "Drawing.  Click to cancel.", timeout: mapload_timeout);
        var refreshwheel = paneBelowMapContainingRefreshWheel.FindControl(className: "Button:Button", title: "Refresh", timeout: mapload_timeout, continueOnError: true);
        if (refreshwheel == null)
        {
            CancelTimer(timerName);
        }
        else 
        {
            StopTimer(timerName);
        }
    }
    
}