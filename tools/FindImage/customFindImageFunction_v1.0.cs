using LoginPI.Engine.ScriptBase;
using System;
using System.Runtime.InteropServices;
public class customFindImageFunction : ScriptBase {
    private void Execute() {
		/*
		-Disclaimer: This workload is provided as-is and might need further configuration and customization to work successfully in each unique environment. For further Professional Services-based customization please consult with the Login VSI Support and Services team. Please refer to the Help Center section "Application Customization" for further self-help information regarding workload crafting and implementation.
		-YouTube video: https://youtu.be/mm542s9Xs34
		-Workload version and changelist:
		--V1.0 | original
		-Leave Application running compatibility: false
		-Ensure to read through the workload and update the Custom.LoadDll path
        This workload will:
        -Find defined images on the screen, if the image is presently "on the screen". The screenshots need to be taken in advance
		*/
		
        // Load native DLL
        Custom.LoadDll(@"%temp%\LoginPI\ImageSearchDLL64.dll");

        // find image 1
        StartTimer("find_image_1");
        var image1 = FindImage(image: @"%temp%\LoginPI\Images\1.png", click: false, log: true);
        if (image1) // if image1 is found or true
        {
            StopTimer("find_image_1");
        }
        else // else cancel timer
        {
            CancelTimer("find_image_1");
        }

        // find image 2 with timer
        StartTimer("find_image_2");
        var image2 = FindImage(image: @"%temp%\LoginPI\Images\2.png", click: true, resetMousePosition: true, log: true);
        if (image2) // if image2 is found or true
        {
            StopTimer("find_image_2");
        }
        else // else cancel timer
        {
            CancelTimer("find_image_2");
        }

        // find image 3 with timer
        StartTimer("find_image_3");
        var image3 = FindImage(@"%temp%\LoginPI\Images\3.png", click: true, resetMousePosition: true, log: true);
        if (image3) // if image3 is found or true
        {
            StopTimer("find_image_3");
        }
        else // else cancel timer
        {
            CancelTimer("find_image_3");
        }
    }

    bool FindImage(string image, byte tolerance = 10, bool click = false, bool resetMousePosition = false, bool log = false, int timeout = 30) {
        bool found = false;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
            var startTime = DateTime.UtcNow;
            while (startTime.AddSeconds(timeout) > DateTime.UtcNow) {
                int[] results = Custom.UseImageSearch(image, tolerance);
                if (results != null) {
                    found = true;
                    if (click) { Click(results[1] + results[3] / 2, results[2] + results[4] / 2); }
                    if (log) { Log("Found image match. Reference image = " + image); }
                    if (resetMousePosition) { Custom.SetCursorPos(0, 0); }
                    break;
                }
                else {
                    if (log) { Log("Could not find image match. Reference image = " + image); }
                    System.Threading.Thread.Sleep(100);
                    //Wait(0.1);
                }
            }
        }
        return found;
    }
    bool FindImage(string image, byte tolerance = 10, bool doubleClick = false, bool resetMousePosition = false, bool log = false, uint timeout = 30) {
        bool found = false;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
            var startTime = DateTime.UtcNow;
            while (startTime.AddSeconds(timeout) > DateTime.UtcNow) {
                int[] results = Custom.UseImageSearch(image, tolerance);
                if (results != null) {
                    found = true;
                    if (doubleClick) { DoubleClick(results[1] + results[3] / 2, results[2] + results[4] / 2); }
                    if (log) { Log("Found image match. Reference image = " + image); }
                    if (resetMousePosition) { Custom.SetCursorPos(0, 0); }
                    break;
                }
                else {
                    if (log) { Log("Could not find image match. Reference image = " + image); }
                    System.Threading.Thread.Sleep(100);
                    //Wait(0.1);
                }
            }
        }
        return found;
    }
    bool FindImage(string image, byte tolerance = 10, bool rightClick = false, bool resetMousePosition = false, bool log = false, double timeout = 30) {
        bool found = false;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
            var startTime = DateTime.UtcNow;
            while (startTime.AddSeconds(timeout) > DateTime.UtcNow) {
                int[] results = Custom.UseImageSearch(image, tolerance);
                if (results != null) {
                    found = true;
                    if (rightClick) { RightClick(results[1] + results[3] / 2, results[2] + results[4] / 2); }
                    if (log) { Log("Found image match. Reference image = " + image); }
                    if (resetMousePosition) { Custom.SetCursorPos(0, 0); }
                    break;
                }
                else {
                    if (log) { Log("Could not find image match. Reference image = " + image); }
                    System.Threading.Thread.Sleep(100);
                    //Wait(0.1);
                }
            }
        }
        return found;
    }
    private static class Custom {
        [DllImport("ImageSearchDLL64")] public static extern IntPtr ImageSearch(int x, int y, int right, int bottom, [MarshalAs(UnmanagedType.LPStr)]string imagePath);
        public static int[] UseImageSearch(string imgPath, int tolerance) {
            imgPath = "*" + tolerance.ToString() + " " + Environment.ExpandEnvironmentVariables(imgPath);
            IntPtr result = ImageSearch(0, 0, 1920, 1080, imgPath);
            string res = Marshal.PtrToStringAnsi(result);
            if (res[0] == '0') return null;
            string[] data = res.Split('|');
            int[] ints = Array.ConvertAll(data, int.Parse);
            return ints;
        }
        [DllImport("user32", SetLastError = true)] public static extern bool SetCursorPos(int x, int y);
        [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)] private extern static IntPtr LoadLibrary(string libraryPath);
        public static void LoadDll(string libraryPath) {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                LoadLibrary(Environment.ExpandEnvironmentVariables(libraryPath));
            }
        }
    }
}