// TARGET:C:\Program Files\ArcGIS\Pro\bin\ArcGISPro.exe
// START_IN:C:\Program Files\ArcGIS\Pro\bin\
using System.Diagnostics;
using LoginPI.Engine.ScriptBase;

public class ArcGISPro : ScriptBase
{
    // Global variables
    static class Globals
    {
        // global int
        public static int waitBetweenSteps = 10;
        public static int mapload_timeout = 180;
        public static string userName = "bmartynowicz1"; //XXXXXXXXXX
        public static string password = "Tw3ntyf0ur!";
        public static string projectTitle = "MyProject";
        public static string searchLocation = "";
    }
    
    // Main
    void Execute() 
    {
        try{
            Log($"Executing Script");
            Step_App_Start_Time();
            Step_001_ArcGISPro_LoginPage();
            Step_002_ArcGISPro_MainWindow();
            Step_003_ArcGISPro_SelectedMap();
            Step_004_ArcGISPro_SelectedLocation();
            Step_Towards_Up();
            Step_Towards_Down();
            Step_Towards_Left();
            Step_Towards_Right();
            Step_007_ArcGISPro_1_1000_Map();
            Step_008_ArcGISPro_1_25000_Map();
            Step_009_ArcGISPro_1_560_Map();
            Step_010_ArcGISPro_1_10500_Map();
            Step_ArcGISPro_Logout();
        }
        //catch (System.Runtime.InteropServices.COMException ex)
        catch (System.Exception ex)
        {
            // Handle the "script not responding" error
            System.Console.WriteLine("A script not responding error occurred: " + ex.Message);
            // You can add additional logic here to handle the error, such as retrying the operation or displaying a user-friendly message
            Handler();
        }
    }
    public void Handler(){
        System.Console.WriteLine("Oppsies... somethign when wrong");
    }
    
    // Steps
    public void Step_App_Start_Time(){
        // Launch ArcGIS application
        // Timer_start - after launching the application 
        // Timer_End - After the Login window popup with ArcGIS title

        string stepName = System.Reflection.MethodBase.GetCurrentMethod().Name;
        Log($"{stepName}");

        //Launching the ArcGIS Pro login window 
        Log("Launching ArcGIS Pro Login Window");
        //int mapload_timeout = 180;
        START(mainWindowTitle: "ArcGIS Sign In", processName: "ArcGISPro");
        TakeScreenshot("001_Arcgispro_Loginpage");
    }

    public void Step_001_ArcGISPro_LoginPage(){
        // Wait for the login popup to display ArcGIS login window to enter the credentials
        // Timer_start - after launching the Loginpage
        // Timer_End - After the Login window is loaded

        string stepName = System.Reflection.MethodBase.GetCurrentMethod().Name;
        Log($"{stepName}");
        
        //Selecting the logintype for testuser details
        Log("Selecting the logintype for testuser details");
        var SignInWindow = FindWindow(className: "Wpf Window:Window", title: "ArcGIS Sign In", processName: "ArcGISPro",timeout:Globals.mapload_timeout);

        StartTimer($"{stepName}");
        SignInWindow.FindControl(className : "CheckBox:CheckBox", title : "Sign in automatically",timeout:Globals.mapload_timeout).Click();
        
        
        //try catch for selecting the option if the window is not pointing for entering the credentials
        Log("Locate Edge Window");
        var EdgeSignInWindow = FindWindow(className: "Win32 Window:Chrome_WidgetWin_1", title: "Sign In*",processName: "msedge", timeout:30, continueOnError:true);
        
        if(EdgeSignInWindow != null){
            //EdgeSignInWindow.FindControl(className : "Pane:BrowserRootView", title : "Sign In - Microsoft Edge");
            try
            {  
                var UserName= EdgeSignInWindow.FindControl(className : "Edit:padding-left-2", title : "Username",timeout:Globals.mapload_timeout);
                UserName.Click();
            }
            catch 
            {
                var loginDropdown = EdgeSignInWindow.FindControl(className:"TabItem:accordion-title", title: "ArcGIS login",timeout:Globals.mapload_timeout);
                loginDropdown.Click();
                Type("{TAB}");
            }
        }
        else
        {
            
            try
            {  
                var UserName= SignInWindow.FindControl(className : "Edit:padding-left-2", title : "Username",timeout:Globals.mapload_timeout);
                UserName.Click();
            }
            catch 
            {
                var loginDropdown = SignInWindow.FindControl(className:"TabItem:accordion-title", title: "ArcGIS login",timeout:Globals.mapload_timeout);
                loginDropdown.Click();
                Type("{TAB}");
            }
        }        
        StopTimer($"{stepName}");
    }

    public void Step_002_ArcGISPro_MainWindow(){
        //  Entering the credentials and click sign in
        // Timer_start - after entering the credentials
        // Timer_End - After the MainWindow pops up

        string stepName = System.Reflection.MethodBase.GetCurrentMethod().Name;
        Log($"{stepName}");
        
        //Selecting the logintype for testuser details
        Log($"Find Signin Window");
        var SignInWindow = FindWindow(className: "Wpf Window:Window", title: "ArcGIS Sign In", processName: "ArcGISPro",timeout:Globals.mapload_timeout, continueOnError: true);
        if(SignInWindow != null){
            Log("Success");
        }
        else{
            Log("Sign In Window not found");
        }
        
        Log($"Find Edge Signin Window");
        var EdgeSignInWindow = FindWindow(className: "Win32 Window:Chrome_WidgetWin_1", title: "Sign In*",processName: "msedge", timeout:30, continueOnError:true);
                if(EdgeSignInWindow != null){
            Log("Success");
        }
        else{
            Log("Sign In Window not found");
        }
        
        if(EdgeSignInWindow != null){
            //typing the credentials for logging in and going to mainpage
            Log("Locating Input Controls");
            EdgeSignInWindow.FindControl(className : "Edit:padding-left-2", title : "Username", text : "Username",timeout:Globals.mapload_timeout).Type(Globals.userName,cpm:300);
            EdgeSignInWindow.FindControl(className : "Edit:padding-left-2", title : "Password", text : "Password",timeout:Globals.mapload_timeout).Type(Globals.password, cpm:300);
            EdgeSignInWindow.FindControl(className : "Button:btn btn-small btn-fill", title : "Sign In",timeout:Globals.mapload_timeout).Click();
        }
        else{
            SignInWindow.FindControl(className : "Edit:padding-left-2", title : "Username", text : "￼",timeout:Globals.mapload_timeout).Type(Globals.userName,cpm:300);
            SignInWindow.FindControl(className : "Edit:padding-left-2", title : "Password", text : "￼",timeout:Globals.mapload_timeout).Type(Globals.password, cpm:300);
            SignInWindow.FindControl(className : "Button:btn btn-small btn-fill", title : "Sign In",timeout:Globals.mapload_timeout).Click();
        }
        StartTimer("002_Arcgispro_Mainwindow");
        Log("Open Map Window");
        var openMapWindow = FindWindow(title: "ArcGIS Pro",timeout:Globals.mapload_timeout);
        
        
        TakeScreenshot("002_Arcgispro_Mainwindow");
        Log("Maximize Map Window");
        openMapWindow.Maximize();
        StopTimer("002_Arcgispro_Mainwindow");
    }

    public void Step_003_ArcGISPro_SelectedMap(){
        // Click the selected map
        // Timer_start - after Opening the myorg window and selecting the map
        // Timer_End - After the refresh button stops

        string stepName = System.Reflection.MethodBase.GetCurrentMethod().Name;
        Log($"{stepName}");
        
        var openMapWindow = FindWindow(title: "ArcGIS Pro",timeout:Globals.mapload_timeout);

        //opening the project
        var openProjectBtn = openMapWindow.FindControl(className: "Button:Button", title: "Open another project",timeout:Globals.mapload_timeout);
        Wait(1);
        openProjectBtn.Click();
        
        Wait(10);
        
        if(Globals.projectTitle == "*NYC*"){
        
            //selecting the folder from the myorganisation window
            var MyOrgWindow= FindWindow(className : "Wpf Window:Window", title : "Open Project", processName : "ArcGISPro",timeout:Globals.mapload_timeout);
            Wait(5);
            TakeScreenshot($"{stepName}");
            MyOrgWindow.FindControl(className : "Text:TextBlock", title : "My Organization",timeout:Globals.mapload_timeout).Click();
            MyOrgWindow.FindControl(className : "Edit:TextBox", title : "Name Text Box",timeout:Globals.mapload_timeout).Type(Globals.projectTitle);
            MyOrgWindow.FindControl(className : "Button:Button", title : "OK",timeout:Globals.mapload_timeout).Click();
        }
        else{
            var MyProjectWindow= FindWindow(className : "Wpf Window:Window", title : "Open Project", processName : "ArcGISPro",timeout:Globals.mapload_timeout);
            Wait(5);
            TakeScreenshot($"{stepName}");
            //MyProjectWindow.FindControl(className : "Text:TextBlock", title : "My Organization",timeout:Globals.mapload_timeout).Click();
            MyProjectWindow.FindControl(className : "Edit:TextBox", title : "Name Text Box",timeout:Globals.mapload_timeout).Type($"{Globals.projectTitle}.aprx");
            MyProjectWindow.FindControl(className : "Button:Button", title : "OK",timeout:Globals.mapload_timeout).Click();
        }
        //Wait(180);
        
        /*
        //server window to be closed
        var server_window = FindWindow(className : "Wpf Window:Window", title : "Server Credentials",timeout:250,continueOnError:true);
        TakeScreenshot("Arcgispro_SeverCredentialsBox");
        server_window.FindControl(className : "Text:TextBlock", title : "Cancel").Click();
        */
        StartTimer($"{stepName}");
        
        //pointed to the main waindow where the map is loading 
        var mapWindow = FindWindow(className : "Wpf Window:Window", title : $"*{Globals.projectTitle}*",timeout:450,continueOnError:true);
        mapWindow.FindControl(className : "Edit:TextBox", text : "1:*", timeout:Globals.mapload_timeout, continueOnError: true);
        
        StopTimer($"{stepName}");

        // Screenshot Map Load
        TakeScreenshot($"{stepName}_Loaded");
    }

    public void Step_004_ArcGISPro_SelectedLocation(){
        // Search the location in the map
        // Timer_start - after entering the Location and hit enter
        // Timer_End - After the refresh button stops

        string stepName = System.Reflection.MethodBase.GetCurrentMethod().Name;
        Log($"{stepName}");
        
        //pointed to the main waindow where the map is loading
        Log("pointed to the main waindow where the map is loading");
        var mapWindow = FindWindow(className : "Wpf Window:Window", title : $"*{Globals.projectTitle}*",timeout:450,continueOnError:true);

        //Locate Mapbutton and search for location
        Log("Locate Mapbutton and search for location");
        
        mapWindow.FindControlWithXPath(xPath : "MenuBar:Ribbon/Tab:TabStrip/TabItem:Tab/Text:TextBlock",timeout:Globals.mapload_timeout).Click();
        Wait(5);
        mapWindow.FindControlWithXPath(xPath : "MenuBar:Ribbon/Tab:TabStrip/TabItem:Tab/Group:RibbonGroup[4]/Pane:StackPanel/SplitButton:SplitButton[1]/Image:Image",timeout:Globals.mapload_timeout).Click();
        //mapWindow.FindControlWithXPath(xPath : "Group:FrameworkDockSite/Group:DockHost/Group:SplitContainer/Tab:ToolWindowContainer[1]/TabItem:DockingWindowContainerTabItem[1]/Pane:ToolWindow/Custom:LocateDockPane/Edit:TextBox",timeout:Globals.mapload_timeout).Type("622 Katan Ave, Staten Island, NY 10312"+"{Enter}");
        StartTimer($"{stepName}");
        mapWindow.FindControl(className : "Image:Image",timeout:Globals.mapload_timeout, continueOnError: true);
        //assigning the refersh wheel
        var paneBelowMapContainingRefreshWheel = mapWindow.FindControlWithXPath(xPath : "Group:FrameworkDockSite/Group:DockHost/Group:SplitContainer/Pane:Workspace/Tab:TabbedMdiContainer/TabItem:DockingWindowContainerTabItem/Pane:DocumentWindow/Custom:MapPaneView/Custom:MapCoordinateReadoutControl",timeout:200);
        mapWindow.FindControl(className : "Button:Button", title : "Drawing.  Click to cancel.",timeout:Globals.mapload_timeout);
        var refreshwheel1 = paneBelowMapContainingRefreshWheel.FindControl(className: "Button:Button", title: "Refresh", timeout:Globals.mapload_timeout, continueOnError: true);
        if (refreshwheel1 == null)
        {
            CancelTimer($"{stepName}");
        }
        else 
        {
            StopTimer($"{stepName}");
        }
        
        //Wait(15);
        TakeScreenshot($"{stepName}_Loaded");
    }

    public void Step_Towards_Up(){
        // Dragging the map towards Up
        // Timer_start - after clicking and dragging towards up
        // Timer_End - After the refresh button stops

        string stepName = System.Reflection.MethodBase.GetCurrentMethod().Name;
        Log($"{stepName}");
        
        //pointed to the main waindow where the map is loading
        var mapWindow = FindWindow(className : "Wpf Window:Window", title : $"{Globals.projectTitle}",timeout:450,continueOnError:true);
        
        //assigning the refersh wheel
        var paneBelowMapContainingRefreshWheel = mapWindow.FindControlWithXPath(xPath : "Group:FrameworkDockSite/Group:DockHost/Group:SplitContainer/Pane:Workspace/Tab:TabbedMdiContainer/TabItem:DockingWindowContainerTabItem/Pane:DocumentWindow/Custom:MapPaneView/Custom:MapCoordinateReadoutControl",timeout:200);        
        
        //moving mouse towards top on the map
        mapWindow.FindControlWithXPath(xPath : "Wpf Window:Window",timeout:Globals.mapload_timeout).MoveMouseToCenter();
        StartTimer($"{stepName}");
        MouseMoveBy(dx:0,dy:-200);
        MouseDown();
        MouseMoveBy(dx:0,dy:200);
        MouseUp();
        mapWindow.FindControl(className : "Button:Button", title : "Drawing.  Click to cancel.",timeout:Globals.mapload_timeout);
        var refreshwheel2 = paneBelowMapContainingRefreshWheel.FindControl(className: "Button:Button", title: "Refresh", timeout:Globals.mapload_timeout, continueOnError: true);
        if (refreshwheel2 == null)
        {
            CancelTimer($"{stepName}");
        }
        else 
        {
            StopTimer($"{stepName}");
        }
        TakeScreenshot($"{stepName}_Loaded");
    }

    public void Step_Towards_Down(){
        // Dragging the map towards down
        // Timer_start - after clicking and dragging towards down
        // Timer_End - After the refresh button stops

        string stepName = System.Reflection.MethodBase.GetCurrentMethod().Name;
        Log($"{stepName}");

        //pointed to the main waindow where the map is loading
        var mapWindow = FindWindow(className : "Wpf Window:Window", title : $"{Globals.projectTitle}",timeout:450,continueOnError:true);
        
        //assigning the refersh wheel
        var paneBelowMapContainingRefreshWheel = mapWindow.FindControlWithXPath(xPath : "Group:FrameworkDockSite/Group:DockHost/Group:SplitContainer/Pane:Workspace/Tab:TabbedMdiContainer/TabItem:DockingWindowContainerTabItem/Pane:DocumentWindow/Custom:MapPaneView/Custom:MapCoordinateReadoutControl",timeout:200);        
        
        //moving Mouse towards down on the map
        mapWindow.FindControlWithXPath(xPath : "Wpf Window:Window",timeout:Globals.mapload_timeout).MoveMouseToCenter();
        StartTimer($"{stepName}");
        MouseMoveBy(dx:0,dy:200);
        MouseDown();
        MouseMoveBy(dx:0,dy:-200);
        MouseUp();
        mapWindow.FindControl(className : "Button:Button", title : "Drawing.  Click to cancel.",timeout:Globals.mapload_timeout);
        var refreshwheel3 = paneBelowMapContainingRefreshWheel.FindControl(className: "Button:Button", title: "Refresh", timeout:Globals.mapload_timeout, continueOnError: true);
        if (refreshwheel3 == null)
        {
            CancelTimer($"{stepName}");
        }
        else 
        {
            StopTimer($"{stepName}");
        }
        TakeScreenshot($"{stepName}_Loaded");   
    }
    public void Step_Towards_Left(){
        // Dragging the map towards Left
        // Timer_start - after clicking and dragging towards left
        // Timer_End - After the refresh button stops

        string stepName = System.Reflection.MethodBase.GetCurrentMethod().Name;
        Log($"{stepName}");

        //pointed to the main waindow where the map is loading
        var mapWindow = FindWindow(className : "Wpf Window:Window", title : $"{Globals.projectTitle}",timeout:450,continueOnError:true);
        
        //Moving Mouse towards left on the map
        mapWindow.FindControlWithXPath(xPath : "Wpf Window:Window",timeout:Globals.mapload_timeout).MoveMouseToCenter();
        StartTimer($"{stepName}");
        MouseMoveBy(dx:-200,dy:0);
        MouseDown();
        MouseMoveBy(dx:200,dy:0);
        MouseUp();
        mapWindow.FindControl(className : "Button:Button", title : "Drawing.  Click to cancel.",timeout:Globals.mapload_timeout);
        
        //assigning the refersh wheel
        var paneBelowMapContainingRefreshWheel = mapWindow.FindControlWithXPath(xPath : "Group:FrameworkDockSite/Group:DockHost/Group:SplitContainer/Pane:Workspace/Tab:TabbedMdiContainer/TabItem:DockingWindowContainerTabItem/Pane:DocumentWindow/Custom:MapPaneView/Custom:MapCoordinateReadoutControl",timeout:200);        
        
        var refreshwheel4 = paneBelowMapContainingRefreshWheel.FindControl(className: "Button:Button", title: "Refresh", timeout:Globals.mapload_timeout, continueOnError: true);
        if (refreshwheel4 == null)
        {
            CancelTimer($"{stepName}");
        }
        else 
        {
            StopTimer($"{stepName}");
        }
        TakeScreenshot($"{stepName}_Loaded");
    }

    public void Step_Towards_Right(){
        // Dragging the map towards Right
        // Timer_start - after clicking and dragging towards right
        // Timer_End - After the refresh button stops

        string stepName = System.Reflection.MethodBase.GetCurrentMethod().Name;
        Log($"{stepName}");

        //pointed to the main waindow where the map is loading
        var mapWindow = FindWindow(className : "Wpf Window:Window", title : $"{Globals.projectTitle}",timeout:450,continueOnError:true);
        
        //assigning the refersh wheel
        var paneBelowMapContainingRefreshWheel = mapWindow.FindControlWithXPath(xPath : "Group:FrameworkDockSite/Group:DockHost/Group:SplitContainer/Pane:Workspace/Tab:TabbedMdiContainer/TabItem:DockingWindowContainerTabItem/Pane:DocumentWindow/Custom:MapPaneView/Custom:MapCoordinateReadoutControl",timeout:200);        
        
        //Moving towards Right on the map
        mapWindow.FindControlWithXPath(xPath : "Wpf Window:Window",timeout:Globals.mapload_timeout).MoveMouseToCenter();
        StartTimer($"{stepName}");
        MouseMoveBy(dx:200,dy:0);
        MouseDown();
        MouseMoveBy(dx:-200,dy:0);
        MouseUp();
        mapWindow.FindControl(className : "Button:Button", title : "Drawing.  Click to cancel.",timeout:Globals.mapload_timeout);
        var refreshwheel5 = paneBelowMapContainingRefreshWheel.FindControl(className: "Button:Button", title: "Refresh", timeout:Globals.mapload_timeout, continueOnError: true);
         if (refreshwheel5 == null)
        {
            CancelTimer($"{stepName}");
        }
        else 
        {
            StopTimer($"{stepName}");
        }
        TakeScreenshot($"{stepName}_Loaded");
        
    }

    public void Step_007_ArcGISPro_1_1000_Map(){
        // Enter 1:1000 in the bottom tool bar of the map
        // Timer_start - after entering 1:1000 in the map and hit enter
        // Timer_End - After the refresh button stops

        string stepName = System.Reflection.MethodBase.GetCurrentMethod().Name;
        Log($"{stepName}");

        //pointed to the main waindow where the map is loading
        var mapWindow = FindWindow(className : "Wpf Window:Window", title : $"{Globals.projectTitle}",timeout:450,continueOnError:true);
        
        //1:1000 map_Loading
        var mapScalingTextEditField = mapWindow.FindControlWithXPath(xPath : "Group:FrameworkDockSite/Group:DockHost/Group:SplitContainer/Pane:Workspace/Tab:TabbedMdiContainer/TabItem:DockingWindowContainerTabItem/Pane:DocumentWindow/Custom:MapPaneView/Custom:MapCoordinateReadoutControl/Custom:ScaleControl/ComboBox:ExtendedComboBox/Edit:TextBox",timeout:Globals.mapload_timeout);
        
        mapScalingTextEditField.Type("1:1000"+"{Enter}");
        StartTimer($"{stepName}");
        mapWindow.FindControl(className : "Button:Button", title : "Drawing.  Click to cancel.",timeout:20);
        
        //assigning the refersh wheel
        var paneBelowMapContainingRefreshWheel = mapWindow.FindControlWithXPath(xPath : "Group:FrameworkDockSite/Group:DockHost/Group:SplitContainer/Pane:Workspace/Tab:TabbedMdiContainer/TabItem:DockingWindowContainerTabItem/Pane:DocumentWindow/Custom:MapPaneView/Custom:MapCoordinateReadoutControl",timeout:200);        
        
        //refreshwheel = paneBelowMapContainingRefreshWheel.FindControl(className: "Button:Button", title: "Refresh", timeout: mapload_timeout, continueOnError: true);
        var refreshwheel6 = paneBelowMapContainingRefreshWheel.FindControl(className: "Button:Button", title: "Refresh", timeout:Globals.mapload_timeout, continueOnError: true);
        if (refreshwheel6 == null)
        {
            CancelTimer($"{stepName}");
        }
        else 
        {
            StopTimer($"{stepName}");
        }
        TakeScreenshot($"{stepName}_Loaded");
    }

    public void Step_008_ArcGISPro_1_25000_Map(){
        // Enter 1:1000 in the bottom tool bar of the map
        // Timer_start - after entering 1:1000 in the map and hit enter
        // Timer_End - After the refresh button stops

        string stepName = System.Reflection.MethodBase.GetCurrentMethod().Name;
        Log($"{stepName}");

        //pointed to the main waindow where the map is loading
        var mapWindow = FindWindow(className : "Wpf Window:Window", title : $"{Globals.projectTitle}",timeout:450,continueOnError:true);
        
        //1:25000
        var mapScalingTextEditField = mapWindow.FindControlWithXPath(xPath : "Group:FrameworkDockSite/Group:DockHost/Group:SplitContainer/Pane:Workspace/Tab:TabbedMdiContainer/TabItem:DockingWindowContainerTabItem/Pane:DocumentWindow/Custom:MapPaneView/Custom:MapCoordinateReadoutControl/Custom:ScaleControl/ComboBox:ExtendedComboBox/Edit:TextBox",timeout:Globals.mapload_timeout);

        mapScalingTextEditField.Type("{CTRL+A}"+ "1:25000");
        Type("{Enter}");
        StartTimer($"{stepName}");
        mapWindow.FindControl(className : "Button:Button", title : "Drawing.  Click to cancel.",timeout:20);
        
        //assigning the refersh wheel
        var paneBelowMapContainingRefreshWheel = mapWindow.FindControlWithXPath(xPath : "Group:FrameworkDockSite/Group:DockHost/Group:SplitContainer/Pane:Workspace/Tab:TabbedMdiContainer/TabItem:DockingWindowContainerTabItem/Pane:DocumentWindow/Custom:MapPaneView/Custom:MapCoordinateReadoutControl",timeout:200);        
        
        var refreshwheel7 = paneBelowMapContainingRefreshWheel.FindControl(className: "Button:Button", title: "Refresh", timeout:Globals.mapload_timeout, continueOnError: true);
        if (refreshwheel7 == null)
        {
            CancelTimer($"{stepName}");
        }
        else 
        {
            StopTimer($"{stepName}");
        }
        TakeScreenshot($"{stepName}_Loaded");
        //paneBelowMapContainingRefreshWheel.FindControl(className: "Button:Button", title: "Refresh", timeout: mapload_timeout, continueOnError: true);
    }

    public void Step_009_ArcGISPro_1_560_Map(){
        // Enter 1:1000 in the bottom tool bar of the map
        // Timer_start - after entering 1:1000 in the map and hit enter
        // Timer_End - After the refresh button stops

        //pointed to the main waindow where the map is loading
        var mapWindow = FindWindow(className : "Wpf Window:Window", title : $"{Globals.projectTitle}",timeout:450,continueOnError:true);
        
        string stepName = System.Reflection.MethodBase.GetCurrentMethod().Name;
        Log($"{stepName}");

        //1:560 
        var mapScalingTextEditField = mapWindow.FindControlWithXPath(xPath : "Group:FrameworkDockSite/Group:DockHost/Group:SplitContainer/Pane:Workspace/Tab:TabbedMdiContainer/TabItem:DockingWindowContainerTabItem/Pane:DocumentWindow/Custom:MapPaneView/Custom:MapCoordinateReadoutControl/Custom:ScaleControl/ComboBox:ExtendedComboBox/Edit:TextBox",timeout:Globals.mapload_timeout);

        mapScalingTextEditField.Type("{CTRL+A}"+ "1:560");
        Type("{Enter}");
        StartTimer($"{stepName}");
        mapWindow.FindControl(className : "Button:Button", title : "Drawing.  Click to cancel.",timeout:20);
        
        //assigning the refersh wheel
        var paneBelowMapContainingRefreshWheel = mapWindow.FindControlWithXPath(xPath : "Group:FrameworkDockSite/Group:DockHost/Group:SplitContainer/Pane:Workspace/Tab:TabbedMdiContainer/TabItem:DockingWindowContainerTabItem/Pane:DocumentWindow/Custom:MapPaneView/Custom:MapCoordinateReadoutControl",timeout:200);        
        
        //paneBelowMapContainingRefreshWheel.FindControl(className: "Button:Button", title: "Refresh", timeout: mapload_timeout, continueOnError: true);
        var refreshwheel8 = paneBelowMapContainingRefreshWheel.FindControl(className: "Button:Button", title: "Refresh", timeout:Globals.mapload_timeout, continueOnError: true);
        if (refreshwheel8 == null)
        {
            CancelTimer($"{stepName}");
        }
        else 
        {
            StopTimer($"{stepName}");
        }
        TakeScreenshot($"{stepName}_Loaded");   
    }
    
    public void Step_010_ArcGISPro_1_10500_Map(){
        // Enter 1:1000 in the bottom tool bar of the map
        // Timer_start - after entering 1:1000 in the map and hit enter
        // Timer_End - After the refresh button stops

        string stepName = System.Reflection.MethodBase.GetCurrentMethod().Name;
        Log($"{stepName}");

        //pointed to the main waindow where the map is loading
        var mapWindow = FindWindow(className : "Wpf Window:Window", title : $"{Globals.projectTitle}",timeout:450,continueOnError:true);
        
        //1:10500   
        var mapScalingTextEditField = mapWindow.FindControlWithXPath(xPath : "Group:FrameworkDockSite/Group:DockHost/Group:SplitContainer/Pane:Workspace/Tab:TabbedMdiContainer/TabItem:DockingWindowContainerTabItem/Pane:DocumentWindow/Custom:MapPaneView/Custom:MapCoordinateReadoutControl/Custom:ScaleControl/ComboBox:ExtendedComboBox/Edit:TextBox",timeout:Globals.mapload_timeout);

        mapScalingTextEditField.Type("{CTRL+A}"+ "1:10500");
        StartTimer($"{stepName}");
        Type("{Enter}");
        
        mapWindow.FindControl(className : "Button:Button", title : "Drawing.  Click to cancel.",timeout:20);
        
        //assigning the refersh wheel
        var paneBelowMapContainingRefreshWheel = mapWindow.FindControlWithXPath(xPath : "Group:FrameworkDockSite/Group:DockHost/Group:SplitContainer/Pane:Workspace/Tab:TabbedMdiContainer/TabItem:DockingWindowContainerTabItem/Pane:DocumentWindow/Custom:MapPaneView/Custom:MapCoordinateReadoutControl",timeout:200);        
        
        //paneBelowMapContainingRefreshWheel.FindControl(className: "Button:Button", title: "Refresh", timeout: mapload_timeout, continueOnError: true);
        var refreshwheel9 = paneBelowMapContainingRefreshWheel.FindControl(className: "Button:Button", title: "Refresh", timeout:Globals.mapload_timeout, continueOnError: true);
         if (refreshwheel9 == null)
        {
            CancelTimer($"{stepName}");
        }
        else 
        {
            StopTimer($"{stepName}");
        }
        TakeScreenshot($"{stepName}_Loaded");
    }

    public void Step_ArcGISPro_Logout(){
        string stepName = System.Reflection.MethodBase.GetCurrentMethod().Name;
        Log($"{stepName}");
        
        //pointed to the main waindow where the map is loading
        var mapWindow = FindWindow(className : "Wpf Window:Window", title : $"{Globals.projectTitle}",timeout:450,continueOnError:true);
        
        //Logout for arcGISpro
        mapWindow.FindControlWithXPath(xPath : "MenuBar:Ribbon/Button:PopupButton",timeout:60).Click();
        mapWindow.FindControlWithXPath(xPath : "Wpf Window:Popup/Custom:SignOnUserControlView/Custom:SignOnStatusView/Button:Button/Text:TextBlock",timeout:60).Click();
        TakeScreenshot($"{stepName}");
        //Wait(60);
        try
        {
            var SignInWindowlast1 =FindWindow(className: "Wpf Window:Window", title: "ArcGIS Sign In", processName: "ArcGISPro",timeout:30);
            SignInWindowlast1.FindControl(className : "Button:Button", title : "Close").Click();
            Wait(5);
            var SignInWindowlast2 =FindWindow(className: "Wpf Window:Window", title: "ArcGIS Sign In", processName: "ArcGISPro",timeout:30);
            SignInWindowlast2.FindControl(className : "Button:Button", title : "Close").Click();
            Wait(5);
            var SignInWindowlast3 =FindWindow(className: "Wpf Window:Window", title: "ArcGIS Sign In", processName: "ArcGISPro",timeout:30);
            SignInWindowlast3.FindControl(className : "Button:Button", title : "Close").Click();
            Wait(80);
            
            
         }
         catch
         {
            Wait(80);
            mapWindow.FindControl(className : "Image:Image", timeout: 60, continueOnError: true);
            mapWindow.FindControl(className : "Button:Button", title : "Close",timeout:90).Click();
            TakeScreenshot($"{stepName}_CloseProject");
            
        
            //final checkbox for closing the project
            Wait(5);
            var checbox_closing =FindWindow(className : "Wpf Window:Window", title : "ArcGIS Project",timeout:60);
            TakeScreenshot($"{stepName}_Checkbox");
            checbox_closing.FindControl(className : "Button:Button", title : "No",timeout:30).Click();
            
            Wait(5);

            STOP();
          }
    }
    

    // Validators    
    public void Step_Move_Start_Position_Wait(){
        // 
        string stepName = System.Reflection.MethodBase.GetCurrentMethod().Name;
        Log($"{stepName}");
        StartTimer($"{stepName}");

        MainWindow.FindControl(className : "SplitButton:SplitButton", title : "Explore").Click();
        
        StopTimer($"{stepName}");
        TakeScreenshot($"{stepName}");
    }
    
    public void Step_Validate_App_Responsive(){
        // The 
        string stepName = System.Reflection.MethodBase.GetCurrentMethod().Name;
        Log($"{stepName}");

        Step_Activate_Help_TabAndButton();

        StopTimer($"{stepName}");
        TakeScreenshot($"{stepName}");
    }
    
    public void Step_Activate_Help_TabAndButton(){
        string stepName = System.Reflection.MethodBase.GetCurrentMethod().Name;
        Log($"{stepName}");

        MainWindow.FindControl(className : "Button:ProButton", title : "Diagnostic Monitor").Click();

        StopTimer($"{stepName}");
        TakeScreenshot($"{stepName}");
    }
}
