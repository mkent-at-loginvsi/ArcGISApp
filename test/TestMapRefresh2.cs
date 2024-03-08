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
        
        // Setup
        // ------------
        
        // Open Project
        OpenProject("MyProject", "open_myproject");
        
        // Verify Window / Refresh Status
        CheckAppWindowStatus("SETUP");
        
        // Run Tests
        // ------------------------
        
        // Refresh and Find Control
        //CheckAppWindowStatus("FindControl");
        //Wait(10);
        //FindControlRefresh("FindControlRefresh");
        
        // Refresh and Find Control Xpath
        //CheckAppWindowStatus("FindControlWithXpath");
        //Wait(10);
        //FindControlXpathRefresh("FindControlXpathRefresh");
        
        // Refresh and Find Control Native Automation Element
        CheckAppWindowStatus("FindControlNativeAE");
        Wait(10);
        FindControlNativeAERefresh("FindControlNativeAE");
        
        
        // Refresh and Find Control Find Image
        //CheckAppWindowStatus("FindControlFindImage");
        //Wait(10);
        //FindControlImageRefresh("FindControlFindImage");

        Wait(10);
        STOP();
    }
    
    // STEPS
    
    // Open Project - After app starts, the project needs to be opened before testing
    private void OpenProject(string name, string timerName){
        Log("===============================================================================");
        Log("= BEGIN: "+timerName);
        Stopwatch stopwatch = Stopwatch.StartNew();
        TakeScreenshot("PRE_"+timerName);
        StartTimer(timerName);
        
        var openAppWindow = FindAppWindow("Wpf Window:Window", "ArcGIS Pro", timerName+"_app", mapload_timeout, true);
        
        var openProjectBtn = openAppWindow.FindControl(className: "Button:Button", title: "Open another project");
        if (openProjectBtn == null)
        {
            CancelTimer(timerName);
        }
        else 
        {
            openProjectBtn.Click();
            var MyProjectWindow = FindWindow(className : "Wpf Window:Window", title : "Open Project", processName : "ArcGISPro",timeout: 10);
            MyProjectWindow.FindControl(className : "Text:TextBlock", title : name).Click();
            MyProjectWindow.FindControl(className : "Edit:TextBox", title : "Name Text Box").Type("MyProject.aprx"+"{Enter}");
            var openMyProjectWindow = FindWindow(className : "Wpf Window:Window", title : "MyProject");
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
    
    
    // Check App Window Status - Ensures the UI is settled before runnng a test
    private void CheckAppWindowStatus(string timerName){
        Log("===============================================================================");
        Log("= BEGIN: "+timerName);
        Stopwatch stopwatch = Stopwatch.StartNew();
        TakeScreenshot("PRE_"+timerName);
        StartTimer(timerName);        
        var appWindow = FindWindow(className : "Wpf Window:Window", title : "MyProject");
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
    
    private void FindControlRefresh(string timerName){
        // Find the App Window and prepare to click the refresh button
        var appWindow = FindAppWindow(className : "Wpf Window:Window", title : "MyProject", timerName: timerName+"_AppWindowPreClick", mapload_timeout : 450, continueOnError : true);
        appWindow.MoveMouseToCenter();
        
        // Find the refresh button and prepare to click
        var refreshControl = FindAppControl(appWindow, "Button:Button", "Refresh", timerName+"_RefreshControl", mapload_timeout, true);

        // Start the timer and click the refresh button
        StartTimer(timerName+"_ClickRefresh");
        refreshControl.Click();

        // Find the status button and check if the refresh is complete
        // Refresh the app window and find the status button - Check to see if the button title changed 
        appWindow = FindAppWindow(className : "Wpf Window:Window", title : "MyProject", timerName: timerName+"_AppWindowPostClick", mapload_timeout : 450, continueOnError : true);
        appWindow.MoveMouseToCenter();
        
        // Because the refresh button's state may take a moment to change, we need to wait for the busy indicator to appear (or timeout)
        //var statusButton = FindAppControl(appWindow, className : "Button:Button", title : "Drawing.  Click to cancel.", timerName+"_getStatusButton", mapload_timeout, true);
        var statusButton = appWindow.FindControl(className : "Button:Button", title : "Drawing.  Click to cancel.", searchRecursively : false, timeout: 5,continueOnError :  true);
        if (statusButton == null)
        {
            Log("'Drawing.  Click to cancel.' was never found");
        }else{
            Log($"statusButton Title: {statusButton.GetTitle()}");
        }
        
        // Now that the busy indicator has passed or timed out, Find the refresh button and check if it is back to pre-click state
        // Refresh the app window and find the status button
        var paneBelowMapContainingRefreshWheel = FindAppControl(appWindow, "Custom:MapCoordinateReadoutControl", title: "", timerName+"_CheckStatus", mapload_timeout, true);
        
        // Focus the search scope to children of the pane with map controls and find the refresh button
        var checkStatusButton = paneBelowMapContainingRefreshWheel.FindControl(className: "Button:Button", title: "Refresh", timeout: mapload_timeout, continueOnError: true);
        if (checkStatusButton == null)
        {
            CancelTimer(timerName+"_ClickRefresh");
        }
        else 
        {
            StopTimer(timerName+"_ClickRefresh");
        }
        
        TakeScreenshot(timerName+"_ClickRefresh");
    }
    
    private void FindControlXpathRefresh(string timerName){
        // Find the App Window and prepare to click the refresh button
        var appWindow = FindAppWindow(className : "Wpf Window:Window", title : "MyProject", timerName: timerName+"_AppWindowPreClick", mapload_timeout : 450, continueOnError : true);
        appWindow.MoveMouseToCenter();
        
        // Focus the search scope to children of the pane with map controls and find the refresh button
        var strXpathRefreshPanel = "Group:FrameworkDockSite/Group:DockHost/Group:SplitContainer/Pane:Workspace/Tab:TabbedMdiContainer/TabItem:DockingWindowContainerTabItem/Pane:DocumentWindow/Custom:MapPaneView/Custom:MapCoordinateReadoutControl";
        var refreshControlPanel = FindAppControlWithXpath(appWindow, strXpathRefreshPanel, timerName+"_refreshControlPanel", mapload_timeout, true);
        
        // Find the refresh button and prepare to click
        var strXpathRefreshControl = "Button:Button";
        var refreshControl = FindAppControlWithXpath(refreshControlPanel, strXpathRefreshControl, timerName+"_refreshControl", mapload_timeout, true);
        
        // Start the timer and click the refresh button
        StartTimer(timerName+"_ClickRefresh");
        refreshControl.Click();

        // Find the status button and check if the refresh is complete
        // Refresh the app window and find the status button - Check to see if the button title changed 
        appWindow = FindAppWindow(className : "Wpf Window:Window", title : "MyProject", timerName: timerName+"_AppWindowPostClick", mapload_timeout : 450, continueOnError : true);
        appWindow.MoveMouseToCenter();
        
        // Because the refresh button's state may take a moment to change, we need to wait for the busy indicator to appear (or timeout)
        var statusButton = FindAppControlWithXpath(appWindow, xpath : "Button:Button", timerName+"_getStatusButton", 5, true);
        if (statusButton == null)
        {
            Log("'Drawing.  Click to cancel.' was never found");
        }else{
            Log($"statusButton Title: {statusButton.GetTitle()}");
        }

        // Now that the busy indicator has passed or timed out, Find the refresh button and check if it is back to pre-click state
        // Refresh the app window and find the status button        
        var paneBelowMapContainingRefreshWheel = FindAppControl(appWindow, "Custom:MapCoordinateReadoutControl", null, timerName+"_GetButtonParent", mapload_timeout, true);

        // Focus the search scope to children of the pane with map controls and find the refresh button
        var refreshwheel6 = paneBelowMapContainingRefreshWheel.FindControl(className: "Button:Button", title: "Refresh", timeout: mapload_timeout, continueOnError: true);
        if (refreshwheel6 == null)
        {
            CancelTimer(timerName+"_ClickRefresh");
        }
        else 
        {
            StopTimer(timerName+"_ClickRefresh");
        }
        TakeScreenshot(timerName+"_ClickRefresh");
                
    }
    
    private void FindControlNativeAERefresh(string timerName){
        // Find the App Window and prepare to click the refresh button
        var uiaWindow = FindAppControlNativeUIA(UIA_PropertyIds.UIA_NamePropertyId, "ArcGIS Pro", "ArcGIS Pro", timerName+"_uiaWindow", mapload_timeout);

        // Focus the search scope to children of the pane with map controls and find the refresh button
        // Find the refresh button and prepare to click
        //var uiaRefreshControl = uiaWindow.FindFirst(TreeScope.TreeScope_Children, automation.CreatePropertyCondition(UIA_PropertyIds.UIA_NamePropertyId, "Refresh"));
        //if (uiaRefreshControl == null)
        //{
        //    CancelTimer(timerName+"_uiaRefreshControl");
        //}
        //else 
        //{
        //    StopTimer(timerName+"_uiaRefreshControl");
        //}
        // Start the timer and click the refresh button
        //StartTimer(timerName+"_ClickRefresh");
        //uiaRefreshControl.Click();

        // Find the status button and check if the refresh is complete
        // Refresh the app window and find the status button - Check to see if the button title changed


        // Because the refresh button's state may take a moment to change, we need to wait for the busy indicator to appear (or timeout)

        // Now that the busy indicator has passed or timed out, Find the refresh button and check if it is back to pre-click state
        // Refresh the app window and find the status button

        // Focus the search scope to children of the pane with map controls and find the refresh button




        //var cond = automation.CreatePropertyCondition(propertyId, title);
        //var uiaWindow = automation.GetRootElement().FindFirst(TreeScope.TreeScope_Children,cond);

    }

    private void FindControlImageRefresh(string timerName){
        // Find the App Window and prepare to click the refresh button
        var appWindow = FindAppWindow(className : "Wpf Window:Window", title : "MyProject", timerName: timerName+"_AppWindowPreClick", mapload_timeout : 450, continueOnError : true);
        appWindow.MoveMouseToCenter();

        int x,y;
        StartTimer(timerName+"_FindImage");
        if (!FindImageCenter(appWindow.ToRectangle(), Images.refresh_fullscreen, out x, out y, tolerance:1))
        {
            CancelTimer(timerName+"_FindImage");
            Log("The image was not found");
        }
        else
        {
            StopTimer(timerName+"_FindImage");
            MouseMove(x,y);
            Log("Moving the mouse to the center of the image");
        }

        // StartTimer(timerName+"_FindAppControlImage");
        // if (!FindAppControlWithXpath(appWindow, Images.refresh_fullscreen, out x, out y, tolerance:1)
        // {
        //     CancelTimer(+"_FindAppControlImage");
        //     Log("The image was not found");
        // }
        // else
        // {
        //     StopTimer(+"_FindAppControlImage");
        //     MouseMove(x,y);
        //     Log("Moving the mouse to the center of the image");
        // }
        

    }

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
   
    private IWindow FindAppControl(IWindow window, string className, string title, string timerName, int mapload_timeout, bool continueOnError){
        Log("===============================================================================");
        Log("= BEGIN: "+timerName);
        Stopwatch stopwatch = Stopwatch.StartNew();
        TakeScreenshot("PRE_"+timerName);
        StartTimer(timerName);
        IWindow appControl = window.FindControl(className : className, title: title, timeout: mapload_timeout, continueOnError: continueOnError);
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
        return appControl;
    }
    
    private IWindow FindAppControlWithXpath(IWindow window, string xpath, string timerName, int mapload_timeout, bool continueOnError){
        Log("===============================================================================");
        Log("= BEGIN: "+timerName);
        Stopwatch stopwatch = Stopwatch.StartNew();
        TakeScreenshot("PRE_"+timerName);
        StartTimer(timerName);
        IWindow appControlXpath = window.FindControlWithXPath(xPath: xpath , timeout: mapload_timeout, continueOnError : continueOnError);
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
        return appControlXpath;
    }
    
    private bool FindAppControlImage(IWindow window, Images image, string timerName, int mapload_timeout){
        Log("===============================================================================");
        Log("= BEGIN: "+timerName);
        Stopwatch stopwatch = Stopwatch.StartNew();
        TakeScreenshot("PRE_"+timerName);
        StartTimer(timerName);
        
        int x,y;
        bool imageFound;
        
        if (!FindImageCenter(window.ToRectangle(), image, out x, out y, tolerance:1))
        {
            CancelTimer(timerName);
            imageFound = false;
            Log("FAIL: The image was not found");
        }else{
            StopTimer(timerName);
            imageFound = true;
            Log("SUCCESS: The image was found");
        }
        TakeScreenshot("POST_"+timerName);
        stopwatch.Stop();
        long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
        int i = (int)elapsedMilliseconds;
        SetTimer("ELAPSED_MS_"+timerName, i);
        // Log or process the elapsed time as needed
        Console.WriteLine($"Task executed in {elapsedMilliseconds} milliseconds");
        Log("= END: "+timerName);
        Log("===============================================================================");
        return imageFound;
    }
    
    private IUIAutomationElement FindAppControlNativeUIA(int propertyId, string className, string title, string timerName, int mapload_timeout){
        Log("===============================================================================");
        Log("= BEGIN: "+timerName);
        Stopwatch stopwatch = Stopwatch.StartNew();
        TakeScreenshot("PRE_"+timerName);
        StartTimer(timerName);
        var cond = automation.CreatePropertyCondition(propertyId, title);
        var uiaWindow = automation.GetRootElement().FindFirst(TreeScope.TreeScope_Children,cond);
        Log($"Element Name: {uiaWindow.CurrentName}");
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
        return uiaWindow;
    }
    
    // Sample getting all control properties from a control
    void CheckRulerState()
    {
        Log("##### Checkbox state example");
        MainWindow.FindControl(className : "TabItem", title : "View").Click();
        Wait(1);
        // Move the mouse pointer out of the way, so we can see the checkbox toggles
        MainWindow.MoveMouseToCenter();
        var ruler = MainWindow.FindControl(className : "CheckBox", title : "Ruler");
        LogDetails(ruler);
        var checkBox = ruler.NativeAutomationElement.GetCurrentPattern(UIA_PatternIds.UIA_TogglePatternId) as IUIAutomationTogglePattern;
        if (checkBox == null)        
        {
            Log("This is not a checkbox");
            return;
        }
        Log($"Ruler is {checkBox.CurrentToggleState}");
        checkBox.Toggle();
        Wait(3);
        Log($"Ruler is {checkBox.CurrentToggleState}");
        checkBox.Toggle();
        Wait(3);
        Log($"Ruler is {checkBox.CurrentToggleState}");
        Wait(3);
        MainWindow.FindControl(className : "TabItem", title : "Home").Click();
        Wait(3);
        Log("##### Checkbox state example end");
    }    
    
    void LogDetails(IWindow control)
     {
        Log("Checking patterns");
        // Example of how to inspect a control on a deepter level
        // First we check which patterns are available
        var patterns = EnumerateFields(typeof(UIA_PatternIds));
        foreach(var pattern in patterns)
        {
           var implementation = control.NativeAutomationElement.GetCurrentPattern(pattern.Value);
           if (implementation != null)
           {
             Log($"Pattern {pattern.Key} is supported");
           }
        }
        
        Log("Checking properties");
        // Example of how to inspect a control on a deepter level
        // First we check which patterns are available
        var properties = EnumerateFields(typeof(UIA_PropertyIds));
        foreach(var property in properties)
        {
            var value = control.NativeAutomationElement.GetCurrentPropertyValue(property.Value);
            if (value != null)
            {
                Log($"property {property.Key} has value : {value}");
            }
        }
    }

    private KeyValuePair<string, int>[] EnumerateFields(Type type)
    {
        FieldInfo[] fields = type.GetFields(
            BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

        List<KeyValuePair<string, int>> valuePairs = new List<KeyValuePair<string, int>>();

        foreach (FieldInfo field in fields)
        {
            if (!field.IsLiteral)
            {
               continue;
            }
            var obj = field.GetValue(null);
            if (obj.GetType().Name == "Int32")
            {
                valuePairs.Add(new KeyValuePair<string, int>(field.Name, (int)obj));
            }
            else
            {
                Log($"{field.Name} is {field.GetType().Name}");
            }
        }

        return valuePairs.ToArray();
    }
    
    bool FindImageCenter(Rectangle bounds, string image, out int x, out int y, int tolerance=0, uint pollingDelayMs=100, uint timeoutinSeconds=10)
    {        
        var result = ImageFinder.FindImage(bounds, image, tolerance, pollingDelayMs, timeoutinSeconds);
        switch (result.result)
        {
            case FindImageResultState.InvalidImage:
              Log($"The image could not be decoded: {result.errorCode}");
              break;

            case FindImageResultState.EmptySearchArea:
              Log("The search are is empty");
              break;

            case FindImageResultState.NotFound:
              Log("The image was not found");
              break;
              
            case FindImageResultState.Found:
              Log($"The image was found at {result.left} x {result.top} with a size of {result.width} x {result.height}");
              x = result.left + result.width/2;
              y = result.top + result.height/2;
              return true;
              
             default:
               ABORT($"Invalid result from FindImage: {result.result}");
               break;
        }
        x = y = 0;
        return false;            
    }

    ///
    /// Convenience method to quickly find the center of an image
    /// and log the result with more detailed information
    ///
    bool FindImageCenterFromFile(Rectangle bounds, string image, out int x, out int y, int radioActiveX = -1, int radioActiveY = -1, int tolerance=0, uint pollingDelayMs=100, uint timeoutinSeconds=10)
    {
        var result = ImageFinder.FindImageFromFile(bounds, image,radioActiveX, radioActiveY, tolerance, pollingDelayMs, timeoutinSeconds);
        switch (result.result)
        {
            case FindImageResultState.InvalidImage:
              Log($"The image could not be loaded: {result.errorCode}");
              break;

            case FindImageResultState.EmptySearchArea:
              Log("The search area is empty");
              break;

            case FindImageResultState.NotFound:
              Log("The image was not found");
              break;
              
            case FindImageResultState.Found:
              Log($"The image was found at {result.left} x {result.top} with a size of {result.width} x {result.height}");
              x = result.left + result.width/2;
              y = result.top + result.height/2;
              return true;
              
             default:
               ABORT($"Invalid result from FindImage: {result.result}");
               break;
        }
        x = y = 0;
        return false;            
    }
    
}

///
/// The list of all images in the script
/// Every image is a variable
///
static class Images
{
    public static string refresh_fullscreen = "eFY0EigAAAAoAAAA////////////5rj/bm5u//j4+P/4+Pj/+Pj4//j4+P/4+Pj/bm5u///muP//5rj/bm5u//j4+P/4+Pj/+Pj4/25ubv//5rj//+a4///muP//5rj//+a4///muP//5rj/19TT/+vr6//r6+v/6+vr/+vr6//r6+v/6+vr/9fU0//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf//+a4/5mShP9ubm7/bm5u/25ubv9ubm7/bm5u/4mEfP//5rj//+a4/5mShP9ubm7/bm5u/25ubv+JhHz//+a4///muP//5rj//+a4///muP//5rj//+a4/9fU0//r6+v/6+vr/+vr6//r6+v/6+vr/+vr6//X1NP/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3///muP//5rj//+a4///muP//5rj//+a4///muP//5rj//+a4///muP//5rj//+a4///muP//5rj//+a4///muP//5rj//+a4///muP//5rj//+a4///muP/X1NP/6+vr/+vr6//r6+v/6+vr/+vr6//r6+v/19TT//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49///5rj//+a4///muP//5rj//+a4///muP//5rj//+a4///muP//5rj//+a4///muP//5rj//+a4///muP//5rj//+a4///muP//5rj//+a4///muP//5rj/19TT/+vr6//r6+v/6+vr/+vr6//r6+v/6+vr/9fU0//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf//+a4///muP//5rj//+a4///muP//5rj//+a4///muP//5rj//+a4///muP//5rj//+a4///muP//5rj//+a4///muP//5rj//+a4///muP//5rj//+a4/9fU0//r6+v/6+vr/+vr6//r6+v/6+vr/+vr6//X1NP/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3///muP//5rj//+a4///muP//5rj//+a4///muP//5rj//+a4///muP//5rj//+a4///muP//5rj//+a4///muP//5rj//+a4///muP//5rj//+a4///muP/X1NP/6+vr/+vr6//r6+v/6+vr/+vr6//r6+v/19TT//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49///5rj//+a4///muP//5rj//+Gu///hrv//4a7//+Gu///muP//5rj//+a4///muP//5rj//+a4///muP//5rj//+a4///muP//5rj//+a4///muP//5rj/19TT/+vr6//r6+v/6+vr/+vr6//r6+v/6+vr/9fU0//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf//+a4///muP//5rj//+a4///muP//4a7//+Gu///muP//5rj//+a4///muP//5rj//+a4///muP//5rj//+a4///muP//5rj//+a4///muP//5rj//+a4/9fU0//r6+v/6+vr/+vr6//r6+v/6+vr/+vr6//X1NP/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3///muP//5rj//+a4///muP//5rj//+a4///hrv//5rj//+a4///muP//5rj//+a4///muP//4a7//+Gu///hrv//4a7//+Gu///hrv//4a7//+Gu///hrv/X1NP/6+vr/+vr6//r6+v/6+vr/+vr6//r6+v/19TT//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49///4a7//+Gu///hrv//4a7//+Gu///hrv//4a7//+Gu///muP//4a7//+Gu///hrv//4a7//+Gu///hrv//4a7//+Gu///hrv//4a7//+Gu///hrv//4a7/19TT/+vr6//r6+v/6+vr/+vr6//r6+v/6+vr/9fU0//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3/9fU0//r6+v/6+vr/+vr6//r6+v/6+vr/+vr6//X1NP/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//X1NP/6+vr/+vr6//r6+v/6+vr/+vr6//r6+v/19TT//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/19TT/+vr6//r6+v/6+vr/+vr6//r6+v/6+vr/9fU0//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3/9fU0//r6+v/6+vr/+vr6//r6+v/6+vr/+vr6//X1NP/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/9e/m/+jPqf/guX3/3bBr/92wa//hvIL/6c+n//Xx6P/4+Pf/+Pj3/9mmV//Zplf/+Pj3//j49//4+Pf/+Pj3//j49//X1NP/6+vr/+vr6//r6+v/6+vr/+vr6//r6+v/19TT//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/6dKu/9uqX//Zplf/2aZX/9mmV//Zplf/2aZX/9mmV//aqVz/6c+n//j49//Zplf/2aZX//j49//4+Pf/+Pj3//j49//4+Pf/19TT/+vr6//r6+v/6+vr/+vr6//r6+v/6+vr/9fU0//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/58qd/9mmV//Zplf/2aZX/9qpXP/gt3f/4bt//9uqX//Zplf/2aZX/9mmV//kwov/2aZX/9mmV//4+Pf/+Pj3//j49//4+Pf/+Pj3/9fU0//r6+v/6+vr/+vr6//r6+v/6+vr/+vr6//X1NP/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/6dGs/9mmV//Zplf/26pf/+rUsf/39fL/+Pj3//j49//39fL/6tSx/9qnWv/Zplf/2aZX/9mmV//Zplf/+Pj3//j49//4+Pf/+Pj3//j49//X1NP/6+vr/+vr6//r6+v/6+vr/+vr6//r6+v/19TT//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/9vLt/9uqX//Zplf/2qda/+7exf/4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//v4cn/26pf/9mmV//Zplf/2aZX//j49//4+Pf/+Pj3//j49//4+Pf/19TT/+vr6//r6+v/6+vr/+vr6//r6+v/6+vr/9fU0//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//07uP/6M2i/+vVs//4+Pf/+Pj3//j49//4+Pf/+Pj3/9mmV//Zplf/2aZX/9mmV//Zplf/2aZX/9mmV//4+Pf/+Pj3//j49//4+Pf/+Pj3/9fU0//r6+v/6+vr/+vr6//r6+v/6+vr/+vr6//X1NP/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//Zplf/2aZX/9mmV//Zplf/2aZX/9mmV//Zplf/+Pj3//j49//4+Pf/+Pj3//j49//X1NP/6+vr/+vr6//r6+v/6+vr/+vr6//r6+v/19TT//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/19TT/+vr6//r6+v/6+vr/+vr6//r6+v/6+vr/9fU0//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3/9fU0//r6+v/6+vr/+vr6//r6+v/6+vr/+vr6//X1NP/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//Zplf/2aZX/9mmV//Zplf/2aZX/9mmV//Zplf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//X1NP/6+vr/+vr6//r6+v/6+vr/+vr6//r6+v/19TT//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/2aZX/9mmV//Zplf/2aZX/9mmV//Zplf/2aZX//j49//4+Pf/+Pj3//j49//4+Pf/8ujZ/+nSrv/28uv/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/19TT/+vr6//r6+v/6+vr/+vr6//r6+v/6+vr/9fU0//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3/9mmV//Zplf/2aZX/9usYf/x5dH/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/9vLt/9+2df/Zplf/2qlc//Pr3v/4+Pf/+Pj3//j49//4+Pf/+Pj3/9fU0//r6+v/6+vr/+vr6//r6+v/6+vr/+vr6//X1NP/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//Zplf/2aZX/9mmV//Zplf/26pf/+/hyf/4+Pf/+Pj3//j49//4+Pf/8OTP/96zcP/Zplf/2aZX/+TDjv/4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//X1NP/6+vr/+vr6//r6+v/6+vr/+vr6//r6+v/19TT//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/2aZX/9mmV//jwIn/2aZX/9mmV//Zplf/26xh/+PAif/kw47/37Rz/9mmV//Zplf/2aZX/+C5ff/29O//+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/19TT/+vr6//r6+v/6+vr/+vr6//r6+v/6+vr/9fU0//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3/9mmV//Zplf/9/f0/+jMoP/ap1r/2aZX/9mmV//Zplf/2aZX/9mmV//Zplf/2aZX/+TDjv/29O//+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3/9fU0//r6+v/6+vr/+vr6//r6+v/6+vr/+vr6//X1NP/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//Zplf/2aZX//j49//4+Pf/9Ozg/+bImf/ftHP/2aZX/9mmV//brGP/5MOR//Dkz//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//X1NP/6+vr/+vr6//r6+v/6+vr/+vr6//r6+v/19TT//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/19TT/+vr6//r6+v/6+vr/+vr6//r6+v/6+vr/9fU0//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3/9fU0//r6+v/6+vr/+vr6//r6+v/6+vr/+vr6//X1NP/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//X1NP/6+vr/+vr6//r6+v/6+vr/+vr6//r6+v/19TT//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/19TT/+vr6//r6+v/6+vr/+vr6//r6+v/6+vr/9fU0//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/+Pj3//j49//4+Pf/19TT/9fU0//X1NP/19TT/9fU0//X1NP/19TT/9fU0//X1NP/19TT/9fU0//X1NP/19TT/9fU0//X1NP/19TT/9fU0//X1NP/19TT/9fU0//X1NP/19TT/9fU0//r6+v/6+vr/+vr6//r6+v/6+vr/+vr6//X1NP/19TT/9fU0//X1NP/19TT/9fU0//X1NP/19TT/9fU0//X1NP/19TT/+vr6//r6+v/6+vr/+vr6//r6+v/6+vr/+vr6//r6+v/6+vr/+vr6//r6+v/6+vr/+vr6//r6+v/6+vr/+vr6//r6+v/6+vr/+vr6//r6+v/6+vr/+vr6//r6+v/6+vr/+vr6//r6+v/6+vr/+vr6//r6+v/6+vr/+vr6//r6+v/6+vr/+vr6//r6+v/6+vr/+vr6//r6+v/6+vr/+vr6//r6+v/6+vr/+vr6//r6+v/6+vr/+vr6//r6+v/6+vr/+vr6//r6+v/6+vr/+vr6//r6+v/6+vr/+vr6//r6+v/6+vr/+vr6//r6+v/6+vr/+vr6//r6+v/6+vr/+vr6//r6+v/6+vr/+vr6//r6+v/6+vr/+vr6//r6+v/6+vr/+vr6//r6+v/6+vr/+vr6//r6+v/6+vr/+vr6//r6+v/6+vr/+vr6//r6+v/6+vr/+vr6//r6+v/6+vr/+vr6//r6+v/6+vr/+vr6//r6+v/6+vr/+vr6//r6+v/6+vr/+vr6//r6+v/6+vr/+vr6//r6+v/6+vr/+vr6//r6+v/6+vr/+vr6//r6+v/6+vr/+vr6//r6+v/6+vr/+vr6//r6+v/6+vr/+vr6//r6+v/6+vr/+vr6//r6+v/6+vr/+vr6//r6+v/6+vr/+vr6//r6+v/6+vr/+vr6//r6+v/6+vr/+vr6//r6+v/6+vr/+vr6//r6+v/6+vr/+vr6//r6+v/6+vr/+vr6//r6+v/6+vr/+vr6//r6+v/6+vr/+vr6//r6+v/6+vr/+vr6//r6+v/6+vr/+vr6//r6+v/6+vr/+vr6//r6+v/6+vr/+vr6//r6+v/6+vr/+vr6//r6+v/6+vr/+vr6//r6+v/6+vr/+vr6//r6+v/6+vr/+vr6//r6+v/6+vr/+vr6//r6+v/6+vr/+vr6//r6+v/6+vr/+vr6//r6+v/6+vr/+vr6//r6+v/6+vr/+vr6//r6+v/6+vr/+vr6//r6+v/6+vr/+vr6//r6+v/6+vr/+vr6//r6+v/6+vr/+vr6//r6+v/6+vr/+vr6//r6+v/vJo=";
}

public class Rectangle 
{
    public Rectangle(IWindow window, int xOffset, int yOffset, int width, int height) : this(window)
    {
        Left += xOffset;
        Top += yOffset;
        Right = Left + width;
        Bottom = Top + height;
    }

    public Rectangle(IWindow window)
    {
        var windowBounds = window.GetBounds();
        Left = (int)windowBounds.Left;
        Top = (int)windowBounds.Top;
        Right = (int)windowBounds.Right;
        Bottom = (int)windowBounds.Bottom;
    }
    
    public int Left {get; set;}
    public int Top {get; set;}
    public int Right {get; set;}
    public int Bottom {get; set;}
    
    public int Width => Right-Left;
    public int Height => Bottom-Top;        
}


///
/// Helper classes for find image logic
///
static class ImageFinder
{    
    public const string FindImageDllPath = @".\FindImageCpp.dll";

    public static Rectangle ToRectangle(this IWindow window) => new Rectangle(window);
    
    public static Rectangle ToRectangle(this IWindow window, int xOffset, int yOffset, int width, int height)
                => new Rectangle(window, xOffset, yOffset, width, height);
    
    [DllImport(FindImageDllPath, CallingConvention = CallingConvention.Cdecl)]
    public static extern FindOperationResult WaitUntilImage(ref ReferenceImage referenceImage, int left, int top, int right, int bottom, int tolerance, uint pollingDelayMs, uint timeoutSeconds);
    
    [DllImport(FindImageDllPath, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int LoadBmpImage(ref ReferenceImage referenceImage, string imagePath);

    [DllImport(FindImageDllPath, CallingConvention = CallingConvention.Cdecl)]
    public static extern bool FreeBmpImage(ref ReferenceImage referenceImage);

    public static FindImageResult FindImageFromFile(Rectangle bounds, string filePath, int radioActiveX, int radioActiveY, int tolerance, uint pollingDelayMs, uint timeoutinSeconds)
    {
        var referenceImage = new ReferenceImage();
        var loadResult = LoadBmpImage(ref referenceImage, filePath);
        if (loadResult != 0)
        {
            return new FindImageResult             
            {
                result = FindImageResultState.InvalidImage,
                errorCode = loadResult
            };
        }
        referenceImage.RadioActiveX = radioActiveX;
        referenceImage.RadioActiveY = radioActiveY;
        try
        {
            return FindImage(bounds, referenceImage, tolerance, pollingDelayMs, timeoutinSeconds);            
        }
        finally
        {
            FreeBmpImage(ref referenceImage);
        }
        
    }

    public static FindImageResult FindImage(Rectangle bounds, string encodedImage, int tolerance, uint pollingDelayMs, uint timeoutinSeconds)
    {
        using (var referenceImage = DecodeReferenceImage(encodedImage))
        {
            return FindImage(bounds, referenceImage, tolerance, pollingDelayMs, timeoutinSeconds);
        }
    }

    public static FindImageResult FindImage(Rectangle bounds, ReferenceImage referenceImage, int tolerance, uint pollingDelayMs, uint timeoutinSeconds)
    {
        if (bounds.Width == 0 || bounds.Height == 0)
        {
            return new FindImageResult             
            {
                result = FindImageResultState.EmptySearchArea
            };
        }
        var referenceImageCopy = referenceImage;
        var result = WaitUntilImage(ref referenceImageCopy,
                            bounds.Left,
                            bounds.Top,
                            bounds.Right,
                            bounds.Bottom,
                            tolerance,
                            pollingDelayMs,                                
                            timeoutinSeconds            
                            );
        if (result.result < 0)
        {
            return new FindImageResult             
            {
                result = FindImageResultState.InvalidImage,
                errorCode = result.result
            };
        }
        else if (result.result == 0)
        {
            return new FindImageResult             
            {
                result = FindImageResultState.NotFound,
                errorCode = result.result
            };
        }
        return new FindImageResult             
        {
            result = FindImageResultState.Found,
            left = result.x,
            top = result.y,
            width = referenceImage.Width,
            height = referenceImage.Height
        };
        
    }
    
    public static ReferenceImage DecodeReferenceImage(string encodedImage)
    {
        const int startMarker = 0x12345678; // start marker
        const ushort endMarker = 0x9ABC;    // end marker

        var bytes = Convert.FromBase64String(encodedImage);
        // Prepare out variables
        var result = new ReferenceImage();

        // Create a memory stream and a binary reader
        using (MemoryStream ms = new MemoryStream(bytes))
        using (BinaryReader br = new BinaryReader(ms))
        {
            // Read and validate the start marker
            if (br.ReadInt32() != startMarker)
            {
                result.Width = 0;
                result.Height = 0;
                return result;
            }

            // Read the data size
            result.Width = br.ReadInt32();
            result.Height = br.ReadInt32();
            result.RadioActiveX = br.ReadInt32();
            result.RadioActiveY = br.ReadInt32();

            var pixelDataSize = 4 * result.Width * result.Height;

            var pixels = br.ReadBytes(pixelDataSize);
            if (br.ReadUInt16() != endMarker)
            {
                result.Width = 0;
                result.Height = 0;
                return result;
            }

            result.memoryHandle = GCHandle.Alloc(pixels, GCHandleType.Pinned);
            result.Pixels = result.memoryHandle.AddrOfPinnedObject();
            // Read and validate the end marker

            return result;
        }
    }
}

enum FindImageResultState
{
    NotFound = 1,
    Found = 2,
    InvalidImage = 3,
    EmptySearchArea = 4
}

struct FindImageResult
{
    public int left,top,width,height;
    public FindImageResultState result;
    public int errorCode;
}

[StructLayout(LayoutKind.Sequential)]
public struct ReferenceImage: IDisposable
{
    public IntPtr Pixels;
    public int Width;
    public int Height;
    public int RadioActiveX;
    public int RadioActiveY;
    // Until here the struct is identical to the C++ struct
    public GCHandle memoryHandle;

    public void Dispose()
    {
        memoryHandle.Free();
    }
}

[StructLayout(LayoutKind.Sequential)]
struct FindOperationResult
{
    public int x;
    public int y;
    // Is 1 if found, 0 if not found, Negative if an error occurred
    public int result;
}

