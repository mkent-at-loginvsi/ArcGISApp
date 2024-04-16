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
    }
    
    // Main
    void Execute() 
    {
        try{
            step_setup();
            step_app_start_time();
            step_001_arcgispro_loginpage();
            
            /*
            step_move_start_position_wait();
            step_002_arcgispro_mainwindow();
            step_003_arcgispro_selectedmap();
            step_004_arcgispro_selectedlocation();
            step_towards_up();
            step_towards_down();
            step_towards_left();
            step_007_arcgispro_1_1000_map();
            step_009_arcgispro_1_560_map();
            step_010_arcgispro_1_10500_map();
            */
            Wait(5);
            step_teardown();
        }
        catch (System.Runtime.InteropServices.COMException ex)
        {
            // Handle the "script not responding" error
            System.Console.WriteLine("A script not responding error occurred: " + ex.Message);
            // You can add additional logic here to handle the error, such as retrying the operation or displaying a user-friendly message
        }

    }
    
    // Steps
    public void step_setup(){
        // Place any test suite/test case preparatory steps here. These are actions that are not timed as part of the test case.
        string stepName = System.Reflection.MethodBase.GetCurrentMethod().Name;
        Log($"{stepName}");
    }
    
    public void step_app_start_time(){
        // Launch ArcGIS application 
        string stepName = System.Reflection.MethodBase.GetCurrentMethod().Name;
        Log($"{stepName}");

        // Locate ArcGIS Sign In Window
        START(mainWindowTitle : "ArcGIS Sign In", processName: "ArcGISPro");

        TakeScreenshot($"{stepName}");
    }
    
    public void step_teardown(){
        // Place any post test case/ test suite cleanup actions here. These are actions that are not timed as part of the test case.
        string stepName = System.Reflection.MethodBase.GetCurrentMethod().Name;
        Log($"{stepName}");
        
        // Logout for arcGISpro
        // Find window "Wpf Window:Window" (sign out window)
        var mapWindow = FindWindow(className: "Wpf Window:Window", title: "ArcGIS Sign In", processName: "ArcGISPro",timeout:30);
        if(mapWindow != null)
        {
            Log($"Successfully found element: sign out window");
        }
        else
        {
            Log($"Failed to find element: Wpf Window:Window");
        }
        
        // LeftClick on Buttion "PopupButton"
        var btnPopup = mapWindow.FindControlWithXPath(xPath: "MenuBar:Ribbon/Button:PopupButton", timeout: 60, continueOnError: true);
        if(btnPopup != null)
        {
            btnPopup.Click();
        }
        else
        {
            Log($"Failed to find element: PopupButton");
        }
        
        // LeftClick on Button "SignOnStatusView"
        var btnSignOut = mapWindow.FindControlWithXPath(xPath: "Wpf Window:Popup/Custom:SignOnUserControlView/Custom:SignOnStatusView/Button:Button/Text:TextBlock",timeout:60, continueOnError:true);
        if(btnSignOut != null)
        {
            btnSignOut.Click();
        }
        else
        {
            Log($"Failed to find element: SignOnStatusView Button");
        }
        
        //Wait(60);
        try
        {
            var SignInWindowlast1 = FindWindow(className: "Wpf Window:Window", title: "ArcGIS Sign In", processName: "ArcGISPro",timeout:30);
            SignInWindowlast1.FindControl(className : "Button:Button", title : "Close").Click();
            Wait(5);
            var SignInWindowlast2 = FindWindow(className: "Wpf Window:Window", title: "ArcGIS Sign In", processName: "ArcGISPro",timeout:30);
            SignInWindowlast2.FindControl(className : "Button:Button", title : "Close").Click();
            Wait(5);
            var SignInWindowlast3 = FindWindow(className: "Wpf Window:Window", title: "ArcGIS Sign In", processName: "ArcGISPro",timeout:30);
            SignInWindowlast3.FindControl(className : "Button:Button", title : "Close").Click();
            Wait(80);
         }
         catch
         {
            Wait(80);
            mapWindow.FindControl(className : "Image:Image", timeout: 60, continueOnError: true);
            mapWindow.FindControl(className : "Button:Button", title : "Close",timeout:90).Click();
            TakeScreenshot("016_ArcGISPro_CloseProject");
            
        
            //final checkbox for closing the project
            Wait(5);
            var checbox_closing =FindWindow(className : "Wpf Window:Window", title : "ArcGIS Project",timeout:60);
            TakeScreenshot("017_ArcGISPro_SaveProject_Checkbox");
            checbox_closing.FindControl(className : "Button:Button", title : "No",timeout:30).Click();
            
            Wait(5);
          }
      // Close web brower window
      var winBrowserSignIn = FindWindow(title : "Sign In * Microsoft​ Edge", className : "Win32 Window:Chrome_WidgetWin_1", processName : "msedge");
        if(winBrowserSignIn != null){
            winBrowserSignIn.Close();
        }
      
    }
    
    public void step_move_start_position_wait(){
        // 
        string stepName = System.Reflection.MethodBase.GetCurrentMethod().Name;
        Log($"{stepName}");
        StartTimer($"{stepName}");
        
        //var winArcGISProSignIn = MainWindow;
        //Wait(Globals.waitBetweenSteps);

        StopTimer($"{stepName}");
        TakeScreenshot($"{stepName}");
    }
    
    public void step_validate_app_responsive(){
        // The 
        string stepName = System.Reflection.MethodBase.GetCurrentMethod().Name;
        Log($"{stepName}");

        StopTimer($"{stepName}");
        TakeScreenshot($"{stepName}");
    }
    
    public void step_activate_help_TabAndButton(){
        string stepName = System.Reflection.MethodBase.GetCurrentMethod().Name;
        Log($"{stepName}");

        StopTimer($"{stepName}");
        TakeScreenshot($"{stepName}");
    }
    
    public void step_001_arcgispro_loginpage()
    {
        string stepName = System.Reflection.MethodBase.GetCurrentMethod().Name;
        Log($"{stepName}");
        StartTimer($"{stepName}");
        
        // Sign In to ArcGIS
        // Find Window: "Sign In"
        Log($"Assigning MainWindow to new var");
        var winArcGISProSignIn = MainWindow;
        
        var txtWaiting = winArcGISProSignIn.FindControl(className : "Text:TextBlock", title : "Waiting...", continueOnError: true);
        if(txtWaiting == null)
        {
            Log($"Failed to find element 'Waiting...' text of Sign In window.");
            ABORT($"Failed to find element 'Waiting...' text of Sign In window.");
        }
        
        // Uncheck 'Sign In Automatically'
        var chkSignInAutomatically = winArcGISProSignIn.FindControl(className : "CheckBox:CheckBox", title : "Sign in automatically",timeout:Globals.mapload_timeout);
        if(chkSignInAutomatically != null){
            chkSignInAutomatically.Click();
        }
        else
        {
            Log($"Failed to find element 'Waiting...' text of Sign In window.");
            ABORT($"Failed to find element 'Waiting...' text of Sign In window.");
        }
        
        // Find browser window requesting username and password
        var winBrowserSignIn = FindWindow(title : "Sign In * Microsoft​ Edge", className : "Win32 Window:Chrome_WidgetWin_1", processName : "msedge");
        if(winBrowserSignIn == null){
            ABORT("Unable to locate browser credential window");
        }
        winBrowserSignIn.Focus();
        
        // For selecting the option if the window is not pointing for entering the credentials
        try{
            // LeftClick on TextField "Username"
            var UserName = winBrowserSignIn.FindControl(className : "Edit:padding-left-2", title : "Username",timeout:Globals.mapload_timeout);
            if (UserName != null)
            {
                UserName.Click();
            }
            else
            {
                Log($"Failed to find element: Username");    
            }
        }
        catch 
        {
            // LeftClick on TabItem "accordian-title"
            var loginDropdown = winBrowserSignIn.FindControl(className:"TabItem:accordion-title", title: "ArcGIS login",timeout:Globals.mapload_timeout);
            if(loginDropdown != null)
            {
                loginDropdown.Click();
                Type("{TAB}");
            }
            else
            {
                Log($"Failed to find element: accordian-title"); 
            }
        }
        
        // Enter Username / Password and click 'Sign In'
        Log($"Entering Username and Password and clicking Sign In");
        winBrowserSignIn.FindControl(className : "Edit:padding-left-2", title : "Username", text : "Username",timeout:Globals.mapload_timeout).Type(Globals.userName,cpm:300);
        winBrowserSignIn.FindControl(className : "Edit:padding-left-2", title : "Password", text : "Password",timeout:Globals.mapload_timeout).Type(Globals.password, cpm:300);
        winBrowserSignIn.FindControl(className : "Button:btn btn-small btn-fill", title : "Sign In",timeout:Globals.mapload_timeout).Click();
        
        StopTimer($"{stepName}");
        TakeScreenshot($"{stepName}");
        
        Wait(10);
        if(winBrowserSignIn != null)
        {
            winBrowserSignIn.Close();
        }
    }
    
    public void step_002_arcgispro_mainwindow()
    {
        string stepName = System.Reflection.MethodBase.GetCurrentMethod().Name;
        Log($"{stepName}");
        StartTimer($"{stepName}");

        StopTimer($"{stepName}");
        TakeScreenshot($"{stepName}");
    }
    
    public void step_003_arcgispro_selectedmap()
    {
        string stepName = System.Reflection.MethodBase.GetCurrentMethod().Name;
        Log($"{stepName}");
        StartTimer($"{stepName}");
        
        StopTimer($"{stepName}");
        TakeScreenshot($"{stepName}");
    }
    
    public void step_004_arcgispro_selectedlocation()
    {
        string stepName = System.Reflection.MethodBase.GetCurrentMethod().Name;
        Log($"{stepName}");
        StartTimer($"{stepName}");

        StopTimer($"{stepName}");
        TakeScreenshot($"{stepName}");
    }
    
    public void step_towards_up()
    {
        string stepName = System.Reflection.MethodBase.GetCurrentMethod().Name;
        Log($"{stepName}");
        StartTimer($"{stepName}");

        StopTimer($"{stepName}");
        TakeScreenshot($"{stepName}");
    }
    
    public void step_towards_down()
    {
        string stepName = System.Reflection.MethodBase.GetCurrentMethod().Name;
        Log($"{stepName}");
        StartTimer($"{stepName}");

        StopTimer($"{stepName}");
        TakeScreenshot($"{stepName}");
    }
    
    public void step_towards_left()
    {
        string stepName = System.Reflection.MethodBase.GetCurrentMethod().Name;
        Log($"{stepName}");
        StartTimer($"{stepName}");

        StopTimer($"{stepName}");
        TakeScreenshot($"{stepName}");
    }
    
    public void step_007_arcgispro_1_1000_map()
    {
        string stepName = System.Reflection.MethodBase.GetCurrentMethod().Name;
        Log($"{stepName}");
        StartTimer($"{stepName}");

        StopTimer($"{stepName}");
        TakeScreenshot($"{stepName}");
    }
        
    public void step_009_arcgispro_1_560_map()
    {
        string stepName = System.Reflection.MethodBase.GetCurrentMethod().Name;
        Log($"{stepName}");
        StartTimer($"{stepName}");

        StopTimer($"{stepName}");
        TakeScreenshot($"{stepName}");
    }
    
    public void step_010_arcgispro_1_10500_map()
    {
        string stepName = System.Reflection.MethodBase.GetCurrentMethod().Name;
        Log($"{stepName}");
        StartTimer($"{stepName}");

        StopTimer($"{stepName}");
        TakeScreenshot($"{stepName}");
    }
}
