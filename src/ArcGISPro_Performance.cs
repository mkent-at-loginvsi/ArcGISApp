// TARGET:C:\Program Files\ArcGIS\Pro\bin\ArcGISPro.exe
// START_IN:C:\Program Files\ArcGIS\Pro\bin\
using System;
using System.Diagnostics;
using System.Collections.Generic;
using LoginPI.Engine.ScriptBase;
using System.Threading;
using System.Runtime.InteropServices;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

public class ArcGISPro : ScriptBase
{
    void Execute()
    {
        try{
            Log($"===> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            
            //================================================================================
            int mapload_timeout = 180;
            //string projectTitle = "As-Built Editing - NYC - As-Built Pressure View - NYC - ArcGIS Pro";
            string projectTitle = "MyProject";
            int step_wait = 5;
            bool altLogin = true;
            
            Suite_Setup();
            
            // ===============================================================================
            // Step No: 1
            // Transaction Name: app_start_time
            // Description: Launch ArcGISPro application
            // How the metrics are measured:
            //  Timer_start: After launching the application
            //  Timer_end: After the Login window popup (and in ready state) with ArcGIS title
            // ***NOTE: This timer is automatically generated, no additional timer is used.
            // ===============================================================================
            Wait(step_wait);
            Log("===> App_Start_Time");
            START(mainWindowTitle: "ArcGIS Sign In", processName: "ArcGISPro");
            Log($"{IsHungAppWindow(MainWindow.NativeWindowHandle)}");
            
            // ===============================================================================
            // Step No: 2
            // Transaction Name: 001_arcgispro_loginpage
            // Description: Wait for the login popup to display ArcGISPro login window to enter the credentials
            // How the metrics are measured:
            //  Timer_start: Timer_start - after launching the Loginpage (after findwindow is successful). This timer does not account for the time it takes to find the window.
            //  Timer_end: After the Login window is loaded (after the username/password controls are found, and recieve a click event)
            // ***NOTE: This timer is started after the window controls are clickable
            // ===============================================================================
            Wait(step_wait);
            Log("===> 001_ArcGISPro_LoginPage");
            TakeScreenshot("001_ArcGISPro_LoginPage");
            
            Log("===> Selecting the logintype for testuser details");
            var signInWindowCheck = FindWindow(className: "Wpf Window:Window", title: "ArcGIS Sign In", processName: "ArcGISPro",timeout:mapload_timeout);
            
            // ===============================================================================
            // Login VSI Alt Login, this can be removed for customer.
            // ===============================================================================
            if(altLogin){
            Log("altLogin detected.");
                CheckSignInEdgeWindow();
                SignInEdgeWindow();
            }else{

            if(signInWindowCheck != null){
                StartTimer("001_ArcGISPro_LoginPage");
                Log("===> Find Sign in automatically checkbox");
                var signInAutoChk = signInWindowCheck.FindControl(className : "CheckBox:CheckBox", title : "Sign in automatically",timeout:mapload_timeout);
                if(signInAutoChk != null){
                    signInAutoChk.Click();
                }
                else{
                    ABORT("===> Failed: Find Sign in automatically checkbox");
                }
                
                try{
                    Log("===> Find Username Input");
                    var userName = signInWindowCheck.FindControl(className : "Edit:padding-left-2", title : "Username",timeout:mapload_timeout);
                    if(userName != null){
                        userName.Click();
                    }
                    else{
                        ABORT("===> Failed: Find Username Input");
                    }
                }
                catch{
                    Log("==> Find Login Dropdown");
                    var loginDropdown = signInWindowCheck.FindControl(className:"TabItem:accordion-title", title: "ArcGIS login",timeout:mapload_timeout);
                    if(loginDropdown != null){
                        loginDropdown.Click();
                        Type("{TAB}");
                    }
                    else{
                        ABORT("===> Failed: Find Login Dropdown");
                    }
                }
                finally{
                    StopTimer("001_ArcGISPro_LoginPage");      
                    
                    //typing the credentials for logging in and going to mainpage
                    signInWindowCheck.FindControl(className : "Edit:padding-left-2", title : "Username", text : "￼",timeout:mapload_timeout).Type("XXXXXXXXXX",cpm:300);
                    signInWindowCheck.FindControl(className : "Edit:padding-left-2", title : "Password", text : "￼",timeout:mapload_timeout).Type("XXXXXXXXXX", cpm:300);
                    signInWindowCheck.FindControl(className : "Button:btn btn-small btn-fill", title : "Sign In",timeout:mapload_timeout).Click();
                }
                
                
            }
            
            }
            
            // ===============================================================================
            // Step No: 3
            // Transaction Name: 002_arcgispro_mainwindow
            // Description: Entering the credentials and click sign in
            // How the metrics are measured:
            //  Timer_start: Timer_start - after entering the credentials
            //  Timer_end: Timer_End - After the MainWindow pops up (after findwindow is successful)
            // ===============================================================================
            Wait(step_wait);
            Log("===> 002_ArcGISPro_MainWindow");
            TakeScreenshot("002_ArcGISPro_Mainwindow_Start");
           
            StartTimer("002_ArcGISPro_MainWindow");
            Log("===> Find OpenMapWindow");
            var openMapWindow = FindWindow(title: "ArcGIS Pro",timeout:mapload_timeout);
            if(openMapWindow != null){
                TakeScreenshot("002_ArcGISPro_Mainwindow");
                openMapWindow.Maximize();
                StopTimer("002_ArcGISPro_MainWindow");
            }
            else{
                ABORT("===> Failed: Find Main Window");
            }
            
            // ===============================================================================
            // Step No: 4
            // Transaction Name: 003_arcgispro_selectedmap
            // Description: Click the selected map
            // How the metrics are measured:
            //  Timer_start - after Opening the ,myorg window and selecting the map (to load)
            //  Timer_End - After activating the help tab, and finding the help button (after find control is successful). This action is used to align the appropriate stop time both visually and programatically (see note)
            // ***NOTE: The original test case stated "After teh refresh button stops", however, this button is unreliable as an indicator of ApplicationReady.
            // ===============================================================================
            Wait(step_wait);
            Log($"===> 003_ArcGISPro_SelectedMap");
            TakeScreenshot("003_ArcGISPro_SelectedMap_Start");
            
            //opening the project
            // ===============================================================================
            // Login VSI Alt Login, this can be removed for customer.
            // ===============================================================================
            if(altLogin){
                Log("altLogin detected.");
                OpenProjectNoOrg();
            }
            else{
            
            //opening the project
            Log("===> Find Open Project button");
            var openProjectBtn = openMapWindow.FindControl(className: "Button:Button", title: "Open another project",timeout:mapload_timeout);
            if(openProjectBtn != null){
                Wait(1);
                openProjectBtn.Click();
            }
            else{
                ABORT("===> Failed: Find Open Project button");
            }
            
            
            //selecting the folder from the myorganisation window
            var MyOrgWindow= FindWindow(className : "Wpf Window:Window", title : "Open Project", processName : "ArcGISPro",timeout:mapload_timeout);
            Wait(5);
            TakeScreenshot("003_ArcGISPro_MapSelection");
            MyOrgWindow.FindControl(className : "Text:TextBlock", title : "My Organization",timeout:mapload_timeout).Click();
            MyOrgWindow.FindControl(className : "Edit:TextBox", title : "Name Text Box",timeout:mapload_timeout).Type("As-Built Editing - NYC");
            MyOrgWindow.FindControl(className : "Button:Button", title : "OK",timeout:mapload_timeout).Click();
            
            StartTimer("003_ArcGISPro_SelectedMap");
            
            //pointed to the main waindow where the map is loading 
            var mapWindow = FindWindow(className : "Wpf Window:Window", title : "As-Built Editing - NYC - As-Built Pressure View - NYC - ArcGIS Pro",timeout:450,continueOnError:true);
            mapWindow.FindControl(className : "Edit:TextBox", text : "1:*", timeout:mapload_timeout, continueOnError: true);
            
            StopTimer("003_ArcGISPro_SelectedMap");
    
            // Screenshot Map Load
            TakeScreenshot("003_ArcGISPro_SelectedMap_Loaded");
            
            }
            
            
            
            // ===============================================================================
            // Step No: 6
            // Transaction Name: 007_arcgispro_1_1000_map
            // Description: Click the selected map
            // How the metrics are measured:
            //  Timer_start - after Opening the ,myorg window and selecting the map (to load)
            //  Timer_End - After activating the help tab, and finding the help button (after find control is successful). This action is used to align the appropriate stop time both visually and programatically (see note)
            // ***NOTE: The original test case stated "After teh refresh button stops", however, this button is unreliable as an indicator of ApplicationReady.
            // ===============================================================================
            Wait(step_wait);
            Log($"===> 007_ArcGISPro_1_1000_Map");
            TakeScreenshot("007_ArcGISPro_1_1000_Map");
            
            var mapWindow007 = FindWindow(className : "Wpf Window:Window", title : projectTitle,timeout:450,continueOnError:true);
            var paneBelowMapContainingRefreshWheel = mapWindow007.FindControlWithXPath(xPath : "Group:FrameworkDockSite/Group:DockHost/Group:SplitContainer/Pane:Workspace/Tab:TabbedMdiContainer/TabItem:DockingWindowContainerTabItem/Pane:DocumentWindow/Custom:MapPaneView/Custom:MapCoordinateReadoutControl",timeout:200);
        
            
            //1:1000 map_Loading
            var mapScalingTextEditField = mapWindow007.FindControlWithXPath(xPath : "Group:FrameworkDockSite/Group:DockHost/Group:SplitContainer/Pane:Workspace/Tab:TabbedMdiContainer/TabItem:DockingWindowContainerTabItem/Pane:DocumentWindow/Custom:MapPaneView/Custom:MapCoordinateReadoutControl/Custom:ScaleControl/ComboBox:ExtendedComboBox/Edit:TextBox",timeout:mapload_timeout);
        
            mapScalingTextEditField.Type("1:1000"+"{Enter}");
            //StartTimer("007_ArcGISPro_1_1000_Map");

            Log($"===> Is Appwindow Hung: {IsHungAppWindow(mapWindow007.NativeWindowHandle)}");
            mapWindow007.FindControl(className : "Button:Button", title : "Drawing.  Click to cancel.",timeout:20);
            //refreshwheel = paneBelowMapContainingRefreshWheel.FindControl(className: "Button:Button", title: "Refresh", timeout: mapload_timeout, continueOnError: true);
            /*var refreshwheel6 = paneBelowMapContainingRefreshWheel.FindControl(className: "Button:Button", title: "Refresh", timeout: mapload_timeout, continueOnError: true);
            if (refreshwheel6 == null)
            {
                CancelTimer("007_ArcGISPro_1_1000_Map");
            }
            else 
            {
                StopTimer("007_ArcGISPro_1_1000_Map");
            }
            
            TakeScreenshot("011_ArcGISPro_1000_Loaded"); */
                
            // ===============================================================================
            // Step No: 7
            // Transaction Name: 008_ArcGISPro_1_25000_Map
            // Description: Click the selected map
            // How the metrics are measured:
            //  Timer_start - after Opening the ,myorg window and selecting the map (to load)
            //  Timer_End - After activating the help tab, and finding the help button (after find control is successful). This action is used to align the appropriate stop time both visually and programatically (see note)
            // ***NOTE: The original test case stated "After teh refresh button stops", however, this button is unreliable as an indicator of ApplicationReady.
            // ===============================================================================
            Wait(step_wait);
            Log($"===> 008_ArcGISPro_1_25000_Map");
            TakeScreenshot("008_ArcGISPro_1_25000_Map");
            
            mapWindow007 = FindWindow(className : "Wpf Window:Window", title : projectTitle,timeout:450,continueOnError:true);
            paneBelowMapContainingRefreshWheel = mapWindow007.FindControlWithXPath(xPath : "Group:FrameworkDockSite/Group:DockHost/Group:SplitContainer/Pane:Workspace/Tab:TabbedMdiContainer/TabItem:DockingWindowContainerTabItem/Pane:DocumentWindow/Custom:MapPaneView/Custom:MapCoordinateReadoutControl",timeout:200);
        
            
            //1:25000 map_Loading
            mapScalingTextEditField = mapWindow007.FindControlWithXPath(xPath : "Group:FrameworkDockSite/Group:DockHost/Group:SplitContainer/Pane:Workspace/Tab:TabbedMdiContainer/TabItem:DockingWindowContainerTabItem/Pane:DocumentWindow/Custom:MapPaneView/Custom:MapCoordinateReadoutControl/Custom:ScaleControl/ComboBox:ExtendedComboBox/Edit:TextBox",timeout:mapload_timeout);
        
            mapScalingTextEditField.Type("{CTRL+A}"+ "1:25000");
            Type("{Enter}");
            //StartTimer("007_ArcGISPro_1_1000_Map");

            Log($"===> Is Appwindow Hung: {IsHungAppWindow(mapWindow007.NativeWindowHandle)}");
            mapWindow007.FindControl(className : "Button:Button", title : "Drawing.  Click to cancel.",timeout:20);
            var refreshwheel7 = paneBelowMapContainingRefreshWheel.FindControl(className: "Button:Button", title: "Refresh", timeout: mapload_timeout, continueOnError: true);
            
            if (refreshwheel7 == null)
            {
                CancelTimer("008_ArcGISPro_1_1000_Map");
            }
            else 
            {
                StopTimer("008_ArcGISPro_1_1000_Map");
            }
            
            TakeScreenshot("008_ArcGISPro_1000_Loaded"); 
            Wait(step_wait);
            Wait(30);
            STOP();
            
            Wait(step_wait);
            Suite_TearDown();
                
            }
            catch{
                Handler();    
            }
        }
    
    public void Handler(){
        Log($"===> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
        
        Console.WriteLine("Handler Invoked");
    }
    
    public void Suite_Setup(){
        Log($"===> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
        
        // We check for artifacts from previous runs here, incase of a crash that left open windows, etc.
        CloseExistingBrowserWindows();
    }
    
    public void Suite_TearDown(){
        Log($"===> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
        
    }
    
    public void CloseExistingBrowserWindows(){
        Log($"===> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
        
        var windows = FindWindows(className: "Win32 Window:Chrome_WidgetWin_1", title: "Sign In*",processName: "msedge", timeout:5);
        foreach(var window in windows){
            window.Close();
        }
    }
    
    public void CheckSignInEdgeWindow(){
        Log($"===> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
    
        int mapload_timeout = 180;
        
        Log("===> Selecting the logintype for testuser details");
        var signInWindowCheck = FindWindow(className: "Wpf Window:Window", title: "ArcGIS Sign In", processName: "ArcGISPro",timeout:mapload_timeout);
        if(signInWindowCheck != null){
            Log("===> Find Sign in automatically checkbox");
            StartTimer("001_ArcGISPro_LoginPage");
            var signInAutoChk = signInWindowCheck.FindControl(className : "CheckBox:CheckBox", title : "Sign in automatically",timeout:mapload_timeout);
            if(signInAutoChk != null){
                signInAutoChk.Click();
            }
            else{
                ABORT("===> Failed: Find Sign in automatically checkbox");
            }
        }
        
        Log("===> Find Edge Credential Window");
        var edgeSignInWindowCheck = FindWindow(className: "Win32 Window:Chrome_WidgetWin_1", title: "Sign In*",processName: "msedge", timeout:5, continueOnError:true);
        if(edgeSignInWindowCheck != null){
            try
                {  
                    Log("===> Find Username input");
                    var userName = edgeSignInWindowCheck.FindControl(className : "Edit:padding-left-2", title : "Username",timeout:mapload_timeout);
                    if(userName != null){
                        userName.Click();
                    }
                    else{
                        Log("===> Failed: Find Username input");
                    }
                }
                catch 
                {
                    Log("===> Find loginDropdown");
                    var loginDropdown = edgeSignInWindowCheck.FindControl(className:"TabItem:accordion-title", title: "ArcGIS login",timeout:mapload_timeout);
                    loginDropdown.Click();
                    Type("{TAB}");
                }
        }
        StopTimer("001_ArcGISPro_LoginPage");
    }
    
    public void SignInEdgeWindow(){
        Log($"===> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
    
        int mapload_timeout = 180;
    
        Dictionary<String, String> MySettings = File
        .ReadLines($"{Environment.CurrentDirectory}\\config.txt")
        .ToDictionary(line => line.Substring(0, line.IndexOf('=')).Trim(),
                  line => line.Substring(line.IndexOf('=') + 1).Trim().Trim('"'));
        
        Log("===> Find Edge Window");
        var edgeSignInWindow = FindWindow(className: "Win32 Window:Chrome_WidgetWin_1", title: "Sign In*",processName: "msedge", timeout:5, continueOnError:true);
        if(edgeSignInWindow != null){
            Log("===> Find Edge Credential Window");
            var edgeSignInWindowCheck = FindWindow(className: "Win32 Window:Chrome_WidgetWin_1", title: "Sign In*",processName: "msedge", timeout:5, continueOnError:true);
            if(edgeSignInWindowCheck != null){
                try
                {  
                        Log("===> Find UsernameCheck input");
                        var userNameCheck = edgeSignInWindowCheck.FindControl(className : "Edit:padding-left-2", title : "Username",timeout:mapload_timeout);
                        if(userNameCheck != null){
                            userNameCheck.Click();
                        }
                        else{
                            Log("===> Failed: Find Username input");
                        }
                }
                catch 
                {
                    Log("===> Find loginDropdown");
                    var loginDropdown = edgeSignInWindowCheck.FindControl(className:"TabItem:accordion-title", title: "ArcGIS login",timeout:mapload_timeout);
                    loginDropdown.Click();
                    Type("{TAB}");
                }
                //typing the credentials for logging in and going to mainpage
                Log("===> Find Username Input");
                var userName = edgeSignInWindow.FindControl(className : "Edit:padding-left-2", title : "Username");
                if(userName != null){
                    userName.Click();
                    userName.Type(MySettings["username"],cpm:300);
                }
                else{
                    ABORT("===> Failed:Find Username Input");
                }
            
                Log("Find Password Input");
                var password = edgeSignInWindow.FindControl(className : "Edit:padding-left-2", title : "Password", text : "Password");
                if(password != null){
                    password.Click();
                    password.Type(MySettings["password"], cpm:300);
                }else{
                    ABORT("===> Failed: Find Password Input");
                }
            
                edgeSignInWindow.FindControl(className : "Button:btn btn-small btn-fill", title : "Sign In",timeout:mapload_timeout).Click();
            }
        }
        else{
            ABORT("===> Failed: Find Edge Window");
        }
    }
    
    public void OpenProjectNoOrg(){
        Log($"===> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
        
        int mapload_timeout = 180;
        var projectTitle = "MyProject";
        
        Log("===> Find Main Window");
        var openMapWindow = FindWindow(title: "ArcGIS Pro",timeout:mapload_timeout);
        if(openMapWindow == null){
            ABORT("===> Failed: Find Main Window");
        }
             
        //opening the project
        Log("===> Find Open Project button");
        var openProjectBtn = openMapWindow.FindControl(className: "Button:Button", title: "Open another project",timeout:mapload_timeout);
        if(openProjectBtn != null){
            Wait(1);
            openProjectBtn.Click();
        }
        else{
            ABORT("===> Failed: Find Open Project button");
        }
        
        //selecting the folder from the myorganisation window
        var MyProjectWindow = FindWindow(className : "Wpf Window:Window", title : "Open Project", processName : "ArcGISPro",timeout:mapload_timeout);
        Wait(5);
        TakeScreenshot("003_ArcGISPro_MapSelection");
        //MyOrgWindow.FindControl(className : "Text:TextBlock", title : "My Organization",timeout:mapload_timeout).Click();
        MyProjectWindow.FindControl(className : "Edit:TextBox", title : "Name Text Box",timeout:mapload_timeout).Type($"{projectTitle}.aprx");
        MyProjectWindow.FindControl(className : "Button:Button", title : "OK",timeout:mapload_timeout).Click();
        
        //pointed to the main waindow where the map is loading 
        Log("===> Find Map Window");
        
        StartTimer("003_ArcGISPro_SelectedMap");
        var mapWindow = FindWindow(className : "Wpf Window:Window", title : projectTitle ,timeout:450,continueOnError:true);
        if(mapWindow != null){
            mapWindow.FindControl(className : "Edit:TextBox", text : "1:*", timeout:mapload_timeout, continueOnError: true);
            StopTimer("003_ArcGISPro_SelectedMap");
        }
        else{
            ABORT("===> Failed: Find Map Window");
        }

        // Screenshot Map Load
        TakeScreenshot("003_ArcGISPro_SelectedMap_Loaded");
    
    }
    
    [DllImport("user32.dll", CallingConvention = CallingConvention.Winapi, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool IsHungAppWindow(IntPtr hwnd);
}