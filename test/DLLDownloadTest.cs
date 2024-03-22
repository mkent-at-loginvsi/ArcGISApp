// TARGET:wordpad.exe
// START_IN:


//// _TARGET:C:\Program Files\ArcGIS\Pro\bin\ArcGISPro.exe
//// _START_IN:C:\Program Files\ArcGIS\Pro\bin
using System;
using System.IO;
using LoginPI.Engine.ScriptBase;
using LoginPI.Engine.ScriptBase.Components;
using LoginPI.Engine.ScriptBase.Constants;
using System.Runtime.InteropServices;

public class FindImages : ScriptBase
{
    private static readonly string SourceDllpath = $"{UrnBaseForFiles.UrnBase}FindImageCpp.dll";
    static readonly string lpPathName = $"{System.IO.Path.GetTempPath()}LoginPI\\Engine\\content_cache";
    
    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    static extern bool SetDllDirectory(string lpPathName);
    
    void Execute() 
    {    
        
        CreateEvent("CurrentDirectory", $"{Environment.CurrentDirectory}");
        CreateEvent("BaseDirectory", $"{AppDomain.CurrentDomain.BaseDirectory}");
        CreateEvent("SourceDllpath", $"{SourceDllpath}");
        CreateEvent("CurrentUserTempDir", $"{Path.GetFullPath(System.IO.Path.GetTempPath())}");
        CreateEvent("FindImageDLLPath", $"{ImageFinder.FindImageDllPath}");
        CreateEvent("FindImageDLL_FullPath", $"{Path.GetFullPath(ImageFinder.FindImageDllPath)}");
        
        string CurrentUserContentCache = $"{System.IO.Path.GetTempPath()}LoginPI\\Engine\\content_cache";
        CreateEvent("CurrentUserContentCache", $"{CurrentUserContentCache}");
        
        if (!File.Exists(CurrentUserContentCache+"\\FindImageCpp.dll"))
        {
            CreateEvent("FindContentCache", $"{CurrentUserContentCache}\\FindImageCpp.dll");
        }
        
        Environment.CurrentDirectory = $"{System.IO.Path.GetTempPath()}LoginPI\\Engine\\content_cache";
        
        if (!File.Exists(ImageFinder.FindImageDllPath))
        {
            Log($"Copying {Path.GetFullPath(ImageFinder.FindImageDllPath)}");
            CopyFile(SourceDllpath, $"{CurrentUserContentCache}\\FindImageCpp.dll" );
        }
        
        START();
        Wait(2);
        int x,y;
        StartTimer("datetime");
        if (!FindImageCenter(MainWindow.ToRectangle(), Images.date_and_time, out x, out y, tolerance:1))
        {
            CancelTimer("datetime");
            Log("The image was not found");
        }
        else
        {
            StopTimer("datetime");
            MouseMove(x,y);
            Log("Moving the mouse to the center of the image");
        }
        Wait(1);
                
        // This command will keep the target app open when we exit
        // Environment.Exit(0);
        
        // Normally, you would exit by STOP();
        STOP();
    }
    

    ///
    /// Convenience method to quickly find the center of an image
    /// and log the result with more detailed information
    ///
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
    public static string date_and_time = "eFY0EiIAAAAhAAAA///////////39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1/769vv+Xl5j/lpaX/5aWlv+VlZb/lJSV/5SUlP+Tk5T/kpKT/5KSkv+RkZL/kJCR/5CQkP+Pj4//jo6P/42Njv+NjY3/jIyM/4uLjP+Li4v/ioqK/4mJiv+IiIn/iIiI/4eHh/+Ghof/hoaG/4WFhf+FhYX/hISE/7Oysv/39vX/9/b1//f29f+Xl5j/9PT0/+fn5//n5+f/5+fn/+fn5//m5ub/5ubm/+bm5v/l5eX/5eXl/+Xl5f/k5OT/5OTk/+Pj4//j4+P/4uLi/+Li4v/h4eH/4eHh/+Dg4P/g4OD/39/f/9/f3//e3t7/3t7e/93d3f/c3Nz/0dHR/8DAwP+Dg4P/9PPy//f29f/39vX/l5eY/+bm5v/Sz8//0s/P/9LPz//Sz8//0s/P/9LPz//Sz8//0s/P/9LPz//Sz8//0s/P/9LPz//Nycn/xsHB/7+4uf+7s7T/u7O0/7uztP+7s7T/u7O0/7uztP+7s7T/u7O0/7uztP+7s7T/u7O0/7uztP/Q0ND/goKC/+7t7P/39vX/9/b1/5qam//n5+f/0s/P/9LPz//Sz8//0s/P/9LPz//Sz8//0s/P/9LPz//Sz8//0s/P/9LPz//Nycn/xsHB/7+4uf+7s7T/u7O0/7uztP+7s7T/u7O0/7uztP+7s7T/u7O0/7uztP+7s7T/u7O0/7uztP+7s7T/2NjY/4KCgv/r6un/9/b1//f29f+ampr/5+fn/6VrO/+eZjn/nWU4/5xkOP+bYzf/mmI2/5hhNf+XYDX/lV80/5ReM/+SXTL/kVwx/5BbMP+OWTD/jVkv/4xYLv+LVy7/ilYt/4lWLf+JVi3/iVYt/4lWLf+JVi3/iVYt/4lWLf+JVi3/iVYt/9jY1/+BgYH/6+rp//f29f/39vX/mZma/+bm5v+lajv/nmY5/51lOP+cZDj/m2M3/5piNv+YYTX/l2A1/5VfNP+UXjP/kl0y/5FcMf+PWjD/jlkw/41YL/+MWC7/i1cu/4pWLf+JVi3/iVYt/4lWLf+JVi3/iVYt/4lWLf+JVi3/iVYt/4lWLf/X19f/gICB/+vq6f/39vX/9/b1/5iYmf/m5ub//bFg//2xYP/9sWD//bFg//2xYP/9sWD//bFg//2xYP/9sWD//bFg//2xYP/9sWD//bFg//2xYP/9sWD//bFg//2xYP/9sWD//bFg//2xYP/9sWD//bFg//2xYP/9sWD//bFg//2xYP/7s2f/19fX/4CAgP/r6un/9/b1//f29f+Xl5j/5eXl//3OnP/+2K///tiv//ywX//9zpz//tiv//7Yr//8sF///du3//7z5//+8+f//LBf//3bt//+8+f//vPn//ywX//927f//vPn//7z5//8sF///du3//7z5//+8+f//LBf//3bt//+8+f//vTo/9bW1v+AgID/6+rp//f29f/39vX/l5eX/+Xl5f/9zZv//deu//3Xrv/7rl3//c2b//3Xrv/9167/+65d//3btv/+8+f//vPn//uuXf/927b//vPn//7z5//7rl3//du2//7z5//+8+f/+65d//3btv/+8+f//vPn//uuXf/927b//vPn//7z6P/W1tb/f39//+vq6f/39vX/9/b1/5aWlv/l5eX/+a1b//mtW//5rVv/+a1b//mtW//5rVv/+a1b//mtW//5rVv/+a1b//mtW//5rVv/+a1b//mtW//5rVv/+a1b//mtW//5rVv/+a1b//mtW//5rVv/+a1b//mtW//5rVv/+a1b//mtW//3sGL/1dXV/39/f//r6un/9/b1//f29f+VlZb/4+Pj//zZtf/+8uf//vLn//irWv/82bX//vLn//7y5//4q1r//Nm1//7y5//+8uf/+Kta//zZtf/+8uf//vLn//irWv/82bX//vLn//7y5//4q1r//Nm1//7y5//+8uf/+Kta//zZtf/+8uf//fPo/9XV1f9/f3//6+rp//f29f/39vX/lJSV/+Pj4//72LT//fLm//3y5v/2qVj/+9i0//3y5v/98ub/9qlY//vYtP/98ub//fLm//apWP/72LT//fLm//3y5v/2q1z/+9q5//3z6f/99Or/+Lh1//zfwf/99Or//fPp//euYv/72bb//fLm//3z5//U1NT/f39//+vq6f/39vX/9/b1/5OTlP/i4uL/9ahW//WoVv/1qFb/9ahW//WoVv/1qFb/9ahW//WoVv/1qFb/9ahW//WoVv/1qFb/9ahW//WqWf/2rmL/97dy//jCif/5zp//+tav//vZtv/72bb/+tWt//nMnP/4v4P/9rNr//WrXP/zq17/1NTU/39/f//r6un/9/b1//f29f+Tk5P/4uLi//nXs//98ub//fLm//OmVf/517P//fLm//3y5v/zplX/+dez//3y5v/98ub/86ZV//nYtf/98+n//vXt//nTqv/Dv7r/ra2o/7KvqP+4ta3/ubWu/6upo/+WlpP/saKU//Pcw//78eb//fLn/9TU1P9/f3//6+rp//f29f/39vX/kpKS/+Hh4f/517L//fLm//3y5v/ypVP/+dey//3y5v/98ub/8qVT//nXsv/98ub//fLm//KnVv/627r//vbu/+Xj3/+pqaL/sayj/+He2v/r6ef/w7+z/+7s6f/r6ef/397Z/8LAuP+alpH/2tPL//bs4v/S0tL/f39//+vq6f/39vX/9/b1/5GRkf/g4OD/8aRS//GkUv/xpFL/8aRS//GkUv/xpFL/8aRS//GkUv/xpFL/8aRS//GlU//yq2D/9b6D/+DLs//CwLv/1tDJ/7OsnP/q5+L/7u3p/7+5rP/v7en/8e/s/+vp5P/Au67/19bR/5+blP/Ci1H/zMzM/39/f//r6un/9/b1//f29f+QkJH/4ODg//jVsf/88eX//PHl//CjUf/41bH//PHl//zx5f/wo1H/+NWx//zx5f/88eb/87Vy//jn1/+tq6P/29fR/+3r6P/s6uX/7u3p/+/t6v+7tqn/8e/s//Lw7f/y8e7/8vHu//Tz8f/a2dT/kpKM/7a2tv98fHz/6+rp//f29f/39vX/j4+Q/9/f3//41bH//PHl//zx5f/wo1H/+NWx//zx5f/88eX/8KNR//jVsf/88eX//PLn//XCjP+yr6v/sauf/9jTyv/u7Oj/7u3p/+/t6v/w7uv/vLep//Lw7f/y8e//8/Lw//Tz8f/19PL/2dbN/7++tv+CgoH/c3Nz/+jo5//39vX/9/b1/46Oj//f39//8aRS//GkUv/xpFL/8aRS//GkUv/xpFL/8aRS//GkUv/xpFL/8aRS//Owaf/40KX/ra2o/+bk4P/Oyb3/7u3p/+/t6v/w7uv/8e/s/724q//z8vD/8/Lw//Tz8f/19PL/9vXz/93Z0P/i4N7/jIyI/2ZmZv/i4eD/9/b1//f29f+Ojo7/3d3d//rYs//98ub//fLm//SoVv/62LP//fLm//3y5v/0qFb/+tiz//3y5v/99Or/+tq3/7azrP/u7Oj/7u3p/+/t6v/w7uv/8vDt//Xz8P+/u6//9vX0//Tz8f/z8u//8/Lv//b18v/5+Pb/9/b2/6enov9bW1v/2NfW//f29f/39vX/jY2N/93d3f/82rb//vPn//7z5//5rFv//Nq2//7z5//+8+f/+axb//zatv/+8+f//vXs//3gwf/Fwbz/7u3p/+/t6v/w7uv/8e/s//Xz8P/z8vH/u7ap/+3r5//u7Oj/8O/q//Py7//39vT/+vr6//v7+//Av7r/VFRU/9LR0P/39vX/9/b1/4eHh//Ozs7/9b6D//2xYP/9sWD//bFg//2xYP/9sWD//bFg//2xYP/9sWD//bFg//3BgP/+4sT/ycbA/7WvnP/w7er/8e/s//Px7v/v7uv/zMi+/21iSP+Sinb/m5OB/5yVgv+dloT/n5iH//b18/+7tKP/xsPA/1FRUf/Pzs7/9/b1//f29f+Ghof/sK+v/83Nzf/f39//3t7e/93d3f/d3d3/3Nzc/9zc3P/b29v/29vb/9ra2v/h4eH/8PDw/767tf/w7ur/8e/s//Lw7f/w7+3/7+7q/+7t6f/LyL3/7u3o//Dv6//08/D/9/f1//r6+v/7+/v//Pz7/7a2sf9RUVH/zs3N//f29f/39vX/tLOz/4WFhf+EhIX/hISE/4ODg/+Dg4P/goKC/4KCgv+BgYH/gYGB/4CAgP+AgID/kpKS/76+vv+npaH/7Oro//Lw7f/x8O7/8O/s/+/u6v/u7ej/7u3o//Du6v/y8e3/9fXz//n5+P/7+/v//Pz8//b29f+UlZH/a2tr/9PS0f/39vX/9/b1//f29f/08/L/7u3s/+vq6f/r6un/6+rp/+vq6f/r6un/6+rp/+vq6f/r6un/6+rp/+3s6//x8PD/lZWS/8/Mxf+9t6n/8O/s//Dv6//v7un/7u3o/+/u6f/x8Oz/9PPx//f39v/7+/r//Pz7/8K8rf/g39v/cXJu/6Sjov/d3Nv/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/+Pf3/9nY2P+urqj/6ejl//Hw7f/w7uv/7+3p/+/u6f/w7+v/8vLu//b18//5+fj//Pz7//z8/P/z8/P/oKCZ/4CAf/+7urn/6Ofm//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/z8vH/sbCv/8PBu//n5uP/zsm8/+zp4//w7ur/8fHt//T08f/4+Pb/9PPx/9vY0P/09PP/w8O9/25ubP+ko6L/1NPS//Hw7//39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9vX0/+vq6f+joqD/sbGr/8XCt//t6+f/8O/s/9DLwv/29fL/+vr5//j39v/U0cn/tLSt/21tbP+bmpr/xsXE/+no6P/29fT/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/19PP/6Ofm/7Oysv+Xl5P/tbSu/9DOyP/LyMD/2tjV/9nY0//GxsH/kZCO/318fP+hoJ//xcTD/+bl5P/08/L/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//X08//q6en/09LR/7i3t/+lpKP/nZyc/5uamv+bmpr/nZyc/6Sjov+2tbX/0M/P/+jo5//08/L/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9vX0//Dv7v/n5uX/3dzb/9nY1//Y19b/2NfW/9nY1//d3Nv/5uXk//Dv7v/29fT/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f+8mg==";
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

