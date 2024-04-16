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
        
        int mapload_timeout = 180;
        START(mainWindowTitle: "ArcGIS Sign In", processName: "ArcGISPro");
        
        TakeScreenshot("001_ArcGISPro_LoginPage");
        
        //Selecting the logintype for testuser details
        var SignInWindow = FindWindow(className: "Wpf Window:Window", title: "ArcGIS Sign In", processName: "ArcGISPro",timeout:mapload_timeout);
        StartTimer("001_ArcGISPro_LoginPage");
        SignInWindow.FindControl(className : "CheckBox:CheckBox", title : "Sign in automatically",timeout:mapload_timeout).Click();
        
        
        //try catch for selecting the option if the window is not pointing for entering the credentials
         try
         {  var UserName= SignInWindow.FindControl(className : "Edit:padding-left-2", title : "Username",timeout:mapload_timeout);
            UserName.Click();
         }
        catch 
        {
            var loginDropdown = SignInWindow.FindControl(className:"TabItem:accordion-title", title: "ArcGIS login",timeout:mapload_timeout);
            loginDropdown.Click();
            Type("{TAB}");
        }
        
        StopTimer("001_ArcGISPro_LoginPage");        
        //typing the credentials for logging in and going to mainpage
        SignInWindow.FindControl(className : "Edit:padding-left-2", title : "Username", text : "￼",timeout:mapload_timeout).Type("XXXXXXXXXX",cpm:300);
        SignInWindow.FindControl(className : "Edit:padding-left-2", title : "Password", text : "￼",timeout:mapload_timeout).Type("XXXXXXXXXX", cpm:300);
        SignInWindow.FindControl(className : "Button:btn btn-small btn-fill", title : "Sign In",timeout:mapload_timeout).Click();
        
        StartTimer("002_ArcGISPro_Mainwindow");
        var openMapWindow = FindWindow(title: "ArcGIS Pro",timeout:mapload_timeout);
        
        
        TakeScreenshot("002_ArcGISPro_Mainwindow");S
        openMapWindow.Maximize();
        StopTimer("002_ArcGISPro_Mainwindow");
        
        //opening the project
        var openProjectBtn = openMapWindow.FindControl(className: "Button:Button", title: "Open another project",timeout:mapload_timeout);
        Wait(1);
        openProjectBtn.Click();
        
        Wait(10);
        
        //selecting the folder from the myorganisation window
        var MyOrgWindow= FindWindow(className : "Wpf Window:Window", title : "Open Project", processName : "ArcGISPro",timeout:mapload_timeout);
        Wait(5);
        TakeScreenshot("003_ArcGISPro_MapSelection");
        MyOrgWindow.FindControl(className : "Text:TextBlock", title : "My Organization",timeout:mapload_timeout).Click();
        MyOrgWindow.FindControl(className : "Edit:TextBox", title : "Name Text Box",timeout:mapload_timeout).Type("As-Built Editing - NYC");
        MyOrgWindow.FindControl(className : "Button:Button", title : "OK",timeout:mapload_timeout).Click();
        
        //Wait(180);
        
        /*
        //server window to be closed
        var server_window = FindWindow(className : "Wpf Window:Window", title : "Server Credentials",timeout:250,continueOnError:true);
        TakeScreenshot("004_ArcGISPro_SeverCredentialsBox");
        server_window.FindControl(className : "Text:TextBlock", title : "Cancel").Click();
        */
        StartTimer("003_ArcGISPro_SelectedMap");
        
        //pointed to the main waindow where the map is loading 
        var mapWindow = FindWindow(className : "Wpf Window:Window", title : "As-Built Editing - NYC - As-Built Pressure View - NYC - ArcGIS Pro",timeout:450,continueOnError:true);
        mapWindow.FindControl(className : "Edit:TextBox", text : "1:*", timeout:mapload_timeout, continueOnError: true);
        
        StopTimer("003_ArcGISPro_SelectedMap");
        
        // Screenshot Map Load
        TakeScreenshot("005_ArcGISPro_SelectedMap_Loaded");
        
        
        
        //Locate Mapbutton and search for location
        mapWindow.FindControlWithXPath(xPath : "MenuBar:Ribbon/Tab:TabStrip/TabItem:Tab/Text:TextBlock",timeout:mapload_timeout).Click();
        Wait(5);
        mapWindow.FindControlWithXPath(xPath : "MenuBar:Ribbon/Tab:TabStrip/TabItem:Tab/Group:RibbonGroup[4]/Pane:StackPanel/SplitButton:SplitButton[1]/Image:Image",timeout:mapload_timeout).Click();
        mapWindow.FindControlWithXPath(xPath : "Group:FrameworkDockSite/Group:DockHost/Group:SplitContainer/Tab:ToolWindowContainer[1]/TabItem:DockingWindowContainerTabItem[1]/Pane:ToolWindow/Custom:LocateDockPane/Edit:TextBox",timeout:mapload_timeout).Type("622 Katan Ave, Staten Island, NY 10312"+"{Enter}");
        StartTimer("004_ArcGISPro_SelectedLocation");
        mapWindow.FindControl(className : "Image:Image",timeout:mapload_timeout, continueOnError: true);
        //assigning the refersh wheel
         var paneBelowMapContainingRefreshWheel = mapWindow.FindControlWithXPath(xPath : "Group:FrameworkDockSite/Group:DockHost/Group:SplitContainer/Pane:Workspace/Tab:TabbedMdiContainer/TabItem:DockingWindowContainerTabItem/Pane:DocumentWindow/Custom:MapPaneView/Custom:MapCoordinateReadoutControl",timeout:200);
        mapWindow.FindControl(className : "Button:Button", title : "Drawing.  Click to cancel.",timeout:mapload_timeout);
       var refreshwheel1 = paneBelowMapContainingRefreshWheel.FindControl(className: "Button:Button", title: "Refresh", timeout: mapload_timeout, continueOnError: true);
       if (refreshwheel1 == null)
        {
            CancelTimer("004_ArcGISPro_SelectedLocation");
        }
        else 
        {
            StopTimer("004_ArcGISPro_SelectedLocation");
        }
        
       //Wait(15);
        
        TakeScreenshot("006_ArcGISPro_SelectedLocation_Loaded");
        
        //moving mouse towards top on the map
        mapWindow.FindControlWithXPath(xPath : "Wpf Window:Window",timeout:mapload_timeout).MoveMouseToCenter();
        StartTimer("Towards_Up");
        MouseMoveBy(dx:0,dy:-200);
        MouseDown();
        MouseMoveBy(dx:0,dy:200);
        MouseUp();
        mapWindow.FindControl(className : "Button:Button", title : "Drawing.  Click to cancel.",timeout:mapload_timeout);
        var refreshwheel2 = paneBelowMapContainingRefreshWheel.FindControl(className: "Button:Button", title: "Refresh", timeout: mapload_timeout, continueOnError: true);
        if (refreshwheel2 == null)
        {
            CancelTimer("Towards_Up");
        }
        else 
        {
            StopTimer("Towards_Up");
        }
        TakeScreenshot("007_ArcGISPro_Towardsup_Loaded");
        
        
        //moving Mouse towards down on the map
        mapWindow.FindControlWithXPath(xPath : "Wpf Window:Window",timeout:mapload_timeout).MoveMouseToCenter();
        StartTimer("Towards_Down");
        MouseMoveBy(dx:0,dy:200);
        MouseDown();
        MouseMoveBy(dx:0,dy:-200);
        MouseUp();
        mapWindow.FindControl(className : "Button:Button", title : "Drawing.  Click to cancel.",timeout:mapload_timeout);
        var refreshwheel3 = paneBelowMapContainingRefreshWheel.FindControl(className: "Button:Button", title: "Refresh", timeout: mapload_timeout, continueOnError: true);
        if (refreshwheel3 == null)
        {
            CancelTimer("Towards_Down");
        }
        else 
        {
            StopTimer("Towards_Down");
        }
        TakeScreenshot("008_ArcGISPro_Towardsdown_Loaded");
        
        //Moving Mouse towards left on the map
        mapWindow.FindControlWithXPath(xPath : "Wpf Window:Window",timeout:mapload_timeout).MoveMouseToCenter();
        StartTimer("Towards_Left");
        MouseMoveBy(dx:-200,dy:0);
        MouseDown();
        MouseMoveBy(dx:200,dy:0);
        MouseUp();
        mapWindow.FindControl(className : "Button:Button", title : "Drawing.  Click to cancel.",timeout:mapload_timeout);
        var refreshwheel4 = paneBelowMapContainingRefreshWheel.FindControl(className: "Button:Button", title: "Refresh", timeout: mapload_timeout, continueOnError: true);
        if (refreshwheel4 == null)
        {
            CancelTimer("Towards_Left");
        }
        else 
        {
            StopTimer("Towards_Left");
        }
        TakeScreenshot("009_ArcGISPro_Towardsleft_Loaded");
        
        
        //Moving towards Right on the map
        mapWindow.FindControlWithXPath(xPath : "Wpf Window:Window",timeout:mapload_timeout).MoveMouseToCenter();
        StartTimer("Towards_Right");
        MouseMoveBy(dx:200,dy:0);
        MouseDown();
        MouseMoveBy(dx:-200,dy:0);
        MouseUp();
        mapWindow.FindControl(className : "Button:Button", title : "Drawing.  Click to cancel.",timeout:mapload_timeout);
        var refreshwheel5 = paneBelowMapContainingRefreshWheel.FindControl(className: "Button:Button", title: "Refresh", timeout: mapload_timeout, continueOnError: true);
         if (refreshwheel5 == null)
        {
            CancelTimer("Towards_Right");
        }
        else 
        {
            StopTimer("Towards_Right");
        }
        TakeScreenshot("0010_ArcGISPro_Towardsright_Loaded");
        
        
        
        
        
        //1:1000 map_Loading
        var mapScalingTextEditField = mapWindow.FindControlWithXPath(xPath : "Group:FrameworkDockSite/Group:DockHost/Group:SplitContainer/Pane:Workspace/Tab:TabbedMdiContainer/TabItem:DockingWindowContainerTabItem/Pane:DocumentWindow/Custom:MapPaneView/Custom:MapCoordinateReadoutControl/Custom:ScaleControl/ComboBox:ExtendedComboBox/Edit:TextBox",timeout:mapload_timeout);
        
        mapScalingTextEditField.Type("1:1000"+"{Enter}");
        StartTimer("007_ArcGISPro_1_1000_Map");
        mapWindow.FindControl(className : "Button:Button", title : "Drawing.  Click to cancel.",timeout:20);
        //refreshwheel = paneBelowMapContainingRefreshWheel.FindControl(className: "Button:Button", title: "Refresh", timeout: mapload_timeout, continueOnError: true);
        var refreshwheel6 = paneBelowMapContainingRefreshWheel.FindControl(className: "Button:Button", title: "Refresh", timeout: mapload_timeout, continueOnError: true);
        if (refreshwheel6 == null)
        {
            CancelTimer("007_ArcGISPro_1_1000_Map");
        }
        else 
        {
            StopTimer("007_ArcGISPro_1_1000_Map");
        }
        TakeScreenshot("011_ArcGISPro_1000_Loaded");


        
        
        //1:25000     
        mapScalingTextEditField.Type("{CTRL+A}"+ "1:25000");
        Type("{Enter}");
        StartTimer("008_ArcGISPro_1_25000_Map");
        mapWindow.FindControl(className : "Button:Button", title : "Drawing.  Click to cancel.",timeout:20);
        var refreshwheel7 = paneBelowMapContainingRefreshWheel.FindControl(className: "Button:Button", title: "Refresh", timeout: mapload_timeout, continueOnError: true);
        if (refreshwheel7 == null)
        {
            CancelTimer("008_ArcGISPro_1_25000_Map");
        }
        else 
        {
            StopTimer("008_ArcGISPro_1_25000_Map");
        }
        TakeScreenshot("012_ArcGISPro_25000_Loaded");
        //paneBelowMapContainingRefreshWheel.FindControl(className: "Button:Button", title: "Refresh", timeout: mapload_timeout, continueOnError: true);
        
        
        //1:560     
        mapScalingTextEditField.Type("{CTRL+A}"+ "1:560");
        Type("{Enter}");
        StartTimer("009_ArcGISPro_1_560_Map");
        mapWindow.FindControl(className : "Button:Button", title : "Drawing.  Click to cancel.",timeout:20);
        //paneBelowMapContainingRefreshWheel.FindControl(className: "Button:Button", title: "Refresh", timeout: mapload_timeout, continueOnError: true);
        var refreshwheel8 = paneBelowMapContainingRefreshWheel.FindControl(className: "Button:Button", title: "Refresh", timeout: mapload_timeout, continueOnError: true);
        if (refreshwheel8 == null)
        {
            CancelTimer("009_ArcGISPro_1_560_Map");
        }
        else 
        {
            StopTimer("009_ArcGISPro_1_560_Map");
        }
        TakeScreenshot("013_ArcGISPro_560_Loaded");
        
        
        
        //1:10500   
        mapScalingTextEditField.Type("{CTRL+A}"+ "1:10500");
        StartTimer("010_ArcGISPro_1_10500_Map");
        Type("{Enter}");
        
        mapWindow.FindControl(className : "Button:Button", title : "Drawing.  Click to cancel.",timeout:20);
        //paneBelowMapContainingRefreshWheel.FindControl(className: "Button:Button", title: "Refresh", timeout: mapload_timeout, continueOnError: true);
        var refreshwheel9 = paneBelowMapContainingRefreshWheel.FindControl(className: "Button:Button", title: "Refresh", timeout: mapload_timeout, continueOnError: true);
         if (refreshwheel9 == null)
        {
            CancelTimer("010_ArcGISPro_1_10500_Map");
        }
        else 
        {
            StopTimer("010_ArcGISPro_1_10500_Map");
        }
        TakeScreenshot("014_ArcGISPro_10500_Loaded");
        
        //Logout for arcGISpro
        mapWindow.FindControlWithXPath(xPath : "MenuBar:Ribbon/Button:PopupButton",timeout:60).Click();
        mapWindow.FindControlWithXPath(xPath : "Wpf Window:Popup/Custom:SignOnUserControlView/Custom:SignOnStatusView/Button:Button/Text:TextBlock",timeout:60).Click();
        TakeScreenshot("015_ArcGISPro_Signout");
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
            TakeScreenshot("016_ArcGISPro_CloseProject");
            
        
            //final checkbox for closing the project
            Wait(5);
            var checbox_closing =FindWindow(className : "Wpf Window:Window", title : "ArcGIS Project",timeout:60);
            TakeScreenshot("017_ArcGISPro_SaveProject_Checkbox");
            checbox_closing.FindControl(className : "Button:Button", title : "No",timeout:30).Click();
            
            Wait(5);
          }
        
        
        
        STOP();
    
}
}
