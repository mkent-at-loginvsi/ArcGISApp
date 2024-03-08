// TARGET:C:\Program Files\ArcGIS\Pro\bin\ArcGISPro.exe
// START_IN:C:\Program Files\ArcGIS\Pro\bin\
using LoginPI.Engine.ScriptBase;

public class ArcGISPro : ScriptBase
{
    void Execute() 
    {
        //var projectTitle = "";
        //var locationToSearch = "";
        
        //Launching the ArcGIS Pro login window 
        
        int mapload_timeout = 60;
        double waitTime = 0.1;

        START(mainWindowTitle: "ArcGIS Sign In", processName: "ArcGISPro");

        TakeScreenshot("001_ArcGISPro_LoginPage");
        

        // ===============================================================================
        // Timer: LoginPage
        // ===============================================================================

        //Selecting the logintype for testuser details
        var SignInWindow = FindWindow(className: "Wpf Window:Window", title: "ArcGIS Sign In", processName: "ArcGISPro",timeout:10);
        
        
        StartTimer("001_ArcGISPro_LoginPage");
        SignInWindow.FindControl(className : "CheckBox:CheckBox", title : "Sign in automatically").Click();
        
        
        //try catch for selecting the option if the window is not pointing for entering the credentials
         try
         {  var UserName= SignInWindow.FindControl(className : "Edit:padding-left-2", title : "Username");
            UserName.Click();
         }
        catch 
        {
            var loginDropdown = SignInWindow.FindControl(className:"TabItem:accordion-title", title: "ArcGIS login",timeout: 5);
            loginDropdown.Click();
            Type("{TAB}");
        }
        
        StopTimer("001_ArcGISPro_LoginPage");
        // ===============================================================================

        //typing the credentials for logging in and going to mainpage
        SignInWindow.FindControl(className : "Edit:padding-left-2", title : "Username", text : "￼").Type("QESTesting",cpm:300);
        SignInWindow.FindControl(className : "Edit:padding-left-2", title : "Password", text : "￼").Type("GIStesting22!", cpm:300);
        SignInWindow.FindControl(className : "Button:btn btn-small btn-fill", title : "Sign In").Click();
        
        // ===============================================================================
        // Timer: MainWindow
        // ===============================================================================
        StartTimer("002_ArcGISPro_Mainwindow");
        var openMapWindow = FindWindow(title: "ArcGIS Pro",timeout: 40);
        TakeScreenshot("002_ArcGISPro_Mainwindow");
        openMapWindow.Maximize();
        StopTimer("002_ArcGISPro_Mainwindow");
        // ===============================================================================
        
        //opening the project
        var openProjectBtn = openMapWindow.FindControl(className: "Button:Button", title: "Open another project");
        Wait(1);
        openProjectBtn.Click();
        
        Wait(10);
        
        //selecting the folder from the myorganisation window
        var MyOrgWindow= FindWindow(className : "Wpf Window:Window", title : "Open Project", processName : "ArcGISPro",timeout: 10);
        Wait(5);
        TakeScreenshot("003_ArcGISPro_MapSelection");
        MyOrgWindow.FindControl(className : "Text:TextBlock", title : "My Organization").Click();
        MyOrgWindow.FindControl(className : "Edit:TextBox", title : "Name Text Box").Type("As-Built Editing - NYC");
        MyOrgWindow.FindControl(className : "Button:Button", title : "OK").Click();
        
        //Wait(180);
        
        /*
        //server window to be closed
        var server_window = FindWindow(className : "Wpf Window:Window", title : "Server Credentials",timeout:250,continueOnError:true);
        TakeScreenshot("004_ArcGISPro_SeverCredentialsBox");
        server_window.FindControl(className : "Text:TextBlock", title : "Cancel").Click();
        */
        
        var mapWindow = FindWindow(className : "Wpf Window:Window", title : "As-Built Editing - NYC - As-Built Pressure View - NYC - ArcGIS Pro",timeout:350);

        // ===============================================================================
        // Timer: SelectedMap
        // ===============================================================================
        StartTimer("003_ArcGISPro_SelectedMap");
        //pointed to the main waindow where the map is loading 
        mapWindow.FindControl(className : "Edit:TextBox", text : "1:*", timeout: 10, continueOnError: true);
        StopTimer("003_ArcGISPro_SelectedMap");
        // ===============================================================================
        
        // Screenshot Map Load
        TakeScreenshot("005_ArcGISPro_SelectedMap_Loaded");
        
        //Locate Mapbutton and search for location
        mapWindow.FindControlWithXPath(xPath : "MenuBar:Ribbon/Tab:TabStrip/TabItem:Tab/Text:TextBlock").Click();
        Wait(5);
        mapWindow.FindControlWithXPath(xPath : "MenuBar:Ribbon/Tab:TabStrip/TabItem:Tab/Group:RibbonGroup[4]/Pane:StackPanel/SplitButton:SplitButton[1]/Image:Image").Click();
        mapWindow.FindControlWithXPath(xPath : "Group:FrameworkDockSite/Group:DockHost/Group:SplitContainer/Tab:ToolWindowContainer[1]/TabItem:DockingWindowContainerTabItem[1]/Pane:ToolWindow/Custom:LocateDockPane/Edit:TextBox").Type("622 Katan Ave, Staten Island, NY 10312"+"{Enter}");
        
        // ===============================================================================
        // Timer: SelectedLocation
        // ===============================================================================
        MeasureMapRefreshValues(mapWindow: mapWindow, timerName: "004_ArcGISPro_SelectedLocation", mapload_timeout: mapload_timeout, waitTime: waitTime);
        TakeScreenshot("006_ArcGISPro_SelectedLocation_Loaded");
        // ===============================================================================

        //1:1000 map_Loading
        var mapScalingTextEditField = mapWindow.FindControlWithXPath(xPath : "Group:FrameworkDockSite/Group:DockHost/Group:SplitContainer/Pane:Workspace/Tab:TabbedMdiContainer/TabItem:DockingWindowContainerTabItem/Pane:DocumentWindow/Custom:MapPaneView/Custom:MapCoordinateReadoutControl/Custom:ScaleControl/ComboBox:ExtendedComboBox/Edit:TextBox");
        
        // ===============================================================================
        // Timer: 1_1000_Map
        // ===============================================================================
        mapScalingTextEditField.Type("1:1000"+"{Enter}");
        MeasureMapRefreshValues(mapWindow: mapWindow, timerName: "007_ArcGISPro_1_1000_Map", mapload_timeout: mapload_timeout, waitTime: waitTime);
        TakeScreenshot("007_ArcGISPro_1_1000_Map");
        // ===============================================================================
        
        // ===============================================================================
        // Timer: 1_25000_Map
        // ===============================================================================
        mapScalingTextEditField.Type("{CTRL+A}" + "1:25000" + "{ENTER}");
        MeasureMapRefreshValues(mapWindow: mapWindow, timerName: "008_ArcGISPro_1_25000_Map", mapload_timeout: mapload_timeout, waitTime: waitTime)
        TakeScreenshot("008_ArcGISPro_1_25000_Map_Loaded");
        // ===============================================================================

        // ===============================================================================
        // Timer: 1_560_Map
        // ===============================================================================
        mapScalingTextEditField.Type("{CTRL+A}"+ "1:560" + "{ENTER}");
        MeasureMapRefreshValues(mapWindow: mapWindow, timerName: "009_ArcGISPro_1_560_Map", mapload_timeout: mapload_timeout, waitTime: waitTime)
        TakeScreenshot("009_ArcGISPro_1_560_Map_Loaded");
        // ===============================================================================

        // ===============================================================================
        // Timer: 1_10500_Map
        // ===============================================================================
        mapScalingTextEditField.Type("{CTRL+A}"+ "1:10500" + "{Enter}");
        MeasureMapRefreshValues(mapWindow: mapWindow, timerName: "010_ArcGISPro_1_10500_Map", mapload_timeout: mapload_timeout, waitTime: waitTime);
        TakeScreenshot("010_ArcGISPro_1_10500_Map_Loaded")
        // ===============================================================================
        
        //Logout for arcGISpro
        mapWindow.FindControlWithXPath(xPath : "MenuBar:Ribbon/Button:PopupButton").Click();
        mapWindow.FindControlWithXPath(xPath : "Wpf Window:Popup/Custom:SignOnUserControlView/Custom:SignOnStatusView/Button:Button/Text:TextBlock").Click();
        TakeScreenshot("014_ArcGISPro_Signout");
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
            mapWindow.FindControl(className : "Image:Image", timeout: 10, continueOnError: true);
            mapWindow.FindControl(className : "Button:Button", title : "Close").Click();
            TakeScreenshot("015_ArcGISPro_CloseProject");
            
        
            //final checkbox for closing the project
            Wait(5);
            var checbox_closing =FindWindow(className : "Wpf Window:Window", title : "ArcGIS Project",timeout:40);
            TakeScreenshot("016_ArcGISPro_SaveProject_Checkbox");
            checbox_closing.FindControl(className : "Button:Button", title : "No").Click();
            
            Wait(5);
          }
        
        STOP();
    }

    // First, the function finds the pane containing the refresh wheel. This is the pane that spans the length of the map window. 
    // Then, it finds the refresh wheel located inside the paneBelowMapContainingRefreshWheel object found in the line above.
    // It starts the timer, and every 0.1, polls the application for the current title. 
    // Until the title matches the target "Refresh" value, we continue to loop and return the refresh wheel's text. 
    // As we've seen, looping while the map is refreshing has performance costs that may impact the applications performance.
    // In this implementation, we are not performing any Find operations, so there is a smaller performance impact. 
    private void MeasureMapRefreshValues(IWindow mapWindow, string timerName, int mapload_timeout, double waitTime) {
        // find refresh wheel container
        var paneBelowMapContainingRefreshWheel = mapWindow.FindControlWithXPath(xPath : "Group:FrameworkDockSite/Group:DockHost/Group:SplitContainer/Pane:Workspace/Tab:TabbedMdiContainer/TabItem:DockingWindowContainerTabItem/Pane:DocumentWindow/Custom:MapPaneView/Custom:MapCoordinateReadoutControl", timeout: 200);
        
        // preemptively find the refreshwheel
        var refreshwheel = paneBelowMapContainingRefreshWheel.FindControl(className: "Button:Button", title: "Refresh", timeout: mapload_timeout, continueOnError: true);

        StartTimer(timerName);
        
        // retrieve the text of the refresh wheel. If it's Drawing, Click to Cancel, we wait (default 0.1) seconds before checking again. 
        while (refreshwheel.GetText() != "Refresh") 
        {
            Wait(waitTime);
        }
        // once the refresh wheel's text is Refresh, we know the map is ready to refresh and has completed loading.
        StopTimer(timerName);
    }

    // This is the most recent version of the function I provided to NG. This was creating using their code to create a function, rather than having identical lines of code with different settings.
    // We suspected that having repetitive lines of code was causing different behaviors-- so we created a function to perform the tasks-- which became a single point of failure. 
    private void LegacyMeasureMapRefreshValues(IWindow mapWindow, string timerName, int mapload_timeout) {

        StartTimer(timerName);
        mapWindow.FindControl(className: "Image:Image", timeout: 15, continueOnError: true)

        var paneBelowMapContainingRefreshWheel = mapWindow.FindControlWithXPath(xPath : "Group:FrameworkDockSite/Group:DockHost/Group:SplitContainer/Pane:Workspace/Tab:TabbedMdiContainer/TabItem:DockingWindowContainerTabItem/Pane:DocumentWindow/Custom:MapPaneView/Custom:MapCoordinateReadoutControl", timeout: 200);
        // mapWindow.FindControl(className : "Button:Button", title : "Drawing.  Click to cancel.", timeout: 10);
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