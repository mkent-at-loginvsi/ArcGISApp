// TARGET:Wordpad.exe
// START_IN:
using System;
using System.IO;
using LoginPI.Engine.ScriptBase;
using LoginPI.Engine.ScriptBase.Components;
using System.Runtime.InteropServices;

public class FindImages : ScriptBase
{
    const string sourceDllPath = @"E:\Repos\Tools\ImageFinder\x64\Release\FindImageCpp.dll";
    void Execute() 
    {    
        if (!File.Exists(ImageFinder.FindImageDllPath) ||
           File.GetLastWriteTime(sourceDllPath) > File.GetLastWriteTime(ImageFinder.FindImageDllPath))
        {
            Log($"Copying {ImageFinder.FindImageDllPath}");
            CopyFile(sourceDllPath, ImageFinder.FindImageDllPath);
        }
        START();
        Wait(2);
        int x,y;
        StartTimer("tableft");
        if (!FindImageCenter(MainWindow.ToRectangle(), Images.date_and_time, out x, out y, tolerance:1))
        {
            CancelTimer("tableft");
            Log("The image was not found");
        }
        else
        {
            StopTimer("tableft");
            MouseMove(x,y);
            Log("Moving the mouse to the center of the image");
        }
        Wait(1);
        
        StartTimer("file_rx");
        if (!FindImageCenterFromFile(MainWindow.ToRectangle(), @"E:\Repos\Tools\ImageFinder\ExampleImages\zoom-in.bmp", out x, out y, radioActiveX: 8, radioActiveY: 8, tolerance:1))
        {
            CancelTimer("file_rx");
            Log("The image was not found");
        }
        else
        {
            StopTimer("file_rx");
        }        

        StartTimer("file_no_rx");
        if (!FindImageCenterFromFile(MainWindow.ToRectangle(), @"E:\Repos\Tools\ImageFinder\ExampleImages\zoom-in.bmp", out x, out y, radioActiveX: -1, radioActiveY: -1, tolerance:1))
        {
            CancelTimer("file_no_rx");
            Log("The image was not found");
        }
        else
        {
            StopTimer("file_no_rx");
            MouseMove(x,y);
            Log("Moving the mouse to the center of the image");
        }
        Log("Done");
        
        // This command will keep the target app open when we exit
        Environment.Exit(0);
        
        // Normally, you would exit by STOP();
        //STOP();
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
    public static string insert_picture = "eFY0EiEAAAAdAAAA///////////39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/zMvL/66urv+urq7/rq6u/62trf+tra3/rKys/6ysrP+rq6v/q6ur/6qqqv+qqqr/qamp/6mpqf+oqKj/p6en/6enp/+mpqb/paWl/6SkpP+kpKT/o6Oj/6Kiov+hoaH/oaGh/6CgoP+fn5//np6e/52dnf/CwcH/9/b1//f29f/39vX/rq6u//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////+cnJz/7+7t//f29f/39vX/rq6u///////uvIb/67Z//+u2gP/rtoD/67aA/+u2gP/rtoD/67aA/+u1f//rtn//67Z//+u2f//rtYD/67V//+u0fP/rtnv/672C/+vHkv/r1rD/69et/+vNmP/q06T/6uTI/+rt3P/q7eH/7vDd//////+bm5v/4N/e//f29f/39vX/rq6u///////jkj3/8ZU2//KXOf/ylzn/8pc5//KXOf/ylzn/8pc5//KXO//ymj7/8ps///KYO//ylzr/8pk+//KmVP/yu3P/8s6N//LYmP/y5LH/8uaz//Lcm//y2pn/8uOq//LlrP/u3qj/5NGc//////+ampr/2NfW//f29f/39vX/ra2t///////lnlb//qlU//+sVv//rFX//6xV//+sV///rVj//65Y//+vW///s2L//7Zl//+xX///sFv//7Jf///Okf//8sf///fJ///3xP///9z////v///91f//77H//+Sd///ejf/83ZL/5sqP//////+ampr/2NfW//f29f/39vX/ra2t///////lpmX//LBl//+2a///tWn//7Ro//+zZv//s2f//7Rq//+2bP//unD//712//+7c///um///792///bo///7Lb//+Wm///rsP//+9n///7n///0xv//5KL//9iL///Wh//65Kj/5ter//////+ZmZn/2NfW//f29f/39vX/rKys///////lsHX//MSC///Wnf//1Z3//9Wd///Pk///xIL//798///Bgf//xob//8eI///GhP//xYX//8yO///fpf//4J///+Om///vvP///uD///7f///1xv//777//+Wr///cmP/66LT/5tiw//////+YmJj/2NfW//f29f/39vX/rKys///////lx5j//OWx///2zf//9c3///rW///30v//5bT//9ee///Wof//2af//9ej///Smv//0Jj//9KY///boP//6rf///LG///40f//++D///rd///40////Nr///XL///iov/63qD/5s+d//////+Xl5f/2NfW//f29f/39vX/q6ur///////l4cX//PjV///yxP//773///fN///93f///dz///fV///wy///68P//+nA///fsP//2qf//9qk///fpP//67n///XJ///+3///+dL///HB///0zP//+dT///HG///quP/64qr/5tGi//////+Xl5f/2NfW//f29f/39vX/q6ur///////m48b//ffP///0wf//9MH///XI///82f///+b////t////6P//+tX///XK///yyv//7MH//+e2///st///7bX//+21///60////t////zb///72P///uL////x////7//6++D/5uTI//////+Wlpb/2NfW//f29f/39vX/qqqq///////i3rb/+fTF//vyxP/79cn//vzT///+2P///97////g///91P//98f///PD///3zP//+9P///vX///92f//+9L///7b///81P///NH///7d////3f///+T////v////5f/6+uX/5ubY//////+VlZX/2NfW//f29f/39vX/qqqq//////+urYX/xsGT/6Wpe/+wu4z/7O/E////2v///9T///zO///6yP//+sn///rL///6zP///Nb////l////7f///+j////h///+0////c////3T////1////9n///7W///+1v/6+9b/5ubS//////+UlJT/2NfW//f29f/39vX/qamp//////+KjGL/kZRj/4yWaP+Hl2r/qLyO/+vwwf///9n////b////2P///9P////T////0////9j////n////5////97////c////3f///9z////d////3v///9/////g////4v/6+uD/5ubY//////+UlJT/2NfW//f29f/39vX/qamp//////+HhV3/kpFi/5KYav+QnG//lKZ4/664iv/JzqH/4eS4//r61P///+n////o////4v///+n////w////7P///+T////m////6////+v////m////5P///+T////m////6v/6+uf/5ubb//////+Tk5P/2NfW//f29f/39vX/qKio//////9zflT/Z3pJ/115SP9fg03/bZdd/4ejbf+dpnb/qKl6/7S1iP/T1bH////n/////P///+j///7f///62f//+NL///vX///00P//+Nf////q////9f////H////s////8P/6+u3/5ubd//////+SkpL/2NfW//f29f/39vX/p6en//////9ceET/Vnk+/0ptOf9Ibjb/T3k+/2KOT/97m2D/jpto/5iaav+hpHb/vMKb/+Tlxv/f1aT/2c6Z/+/Vov//2qz//9yv///Upf//0qP//9mx///w1/////n////////////6+vj/5ubh//////+SkpL/2NfW//f29f/39vX/p6en//////9NYDH/UWsv/1FqMf9MZC7/S2Qu/09vNP9egkP/bIxP/3OQV/93k1n/eJla/4WoZ/+HpWH/f6Jd/5GsaP+gsHH/qbR2/8i9hP/RuoH/w7V5/8LBif/Y0aX/79y////05P//////7uvx//////+RkZH/2NfW//f29f/39vX/pqam//////8+TCj/P1Ei/0NXJf9BVCb/QlYo/0JYKP9LZS7/V3c4/11/QP9fgkP/XYJA/1uEQP9gjUj/YI9J/2KSTP9dj0n/VohH/2+WV/97nl3/dJxX/3GaU/+Anlj/matn/6m/g/+xz6L/t8mw//////+RkZH/2NfW//f29f/39vX/paWl//////90Vif/eVge/3xaIf99WyL/f18m/4BiKP+EaCv/jnEx/5N4M/+ReTT/kno2/5iCPP+Zhj7/no1F/56PRf+ZjkP/kIdA/4+PRf+UmUz/mZ5O/5uiUf+dqlf/o7Jf/6W0Zf+ltWr/sriA//////+QkJD/2NfW//f29f/39vX/pKSk//////+9bCr/xnEi/8p2J//LeCn/zHor/819L//NgDH/zoIz/9GGNv/TjDn/1ZA+/9aUQv/XlUH/2p1I/92jTP/fpk3/4qtU/+OvWP/lslv/6Llk/+zCdP/uyX//8M2I//PXm//y2Kb/6M6i//////+QkJD/2NfW//f29f/39vX/pKSk///////DdCr/yXok/8yCKv/Nhiz/zYgu/8+MMv/TkDT/1ZU5/9eaPv/bn0T/3KRM/92nUP/fqFD/36tS/+KwV//isFf/47Vd/+a3YP/oumT/6cB0/+3Kif/uzJX/7s2Y//PYq//0377/6tKz//////+Pj4//2NfW//f29f/39vX/o6Oj///////FeSn/zIAf/8+GJv/Qiij/0Ywr/9SSMv/Xlzb/15o6/9mcQP/aoEX/2qJI/9ylTP/dpk//3aZP/9yoU//cp1H/3qtW/+CxX//jtWb/5Lpv/+fBff/px4f/6ceK/+zKkf/rzpr/5MeX//////+Pj4//2NfW//f29f/39vX/oqKi///////Igiv/zIQf/8+IJv/Riyv/0Y4s/9OQMf/UlDf/1ZY6/9WXOf/UmDr/1ptA/9eeQ//XnEP/155G/9edRf/Xn0T/2aFI/9ypU//fsF7/4rhv/+XAfv/mwX7/5b57/+S7df/fsmT/3bZ4//////+Ojo7/2NfW//f29f/39vX/oaGh///////Pk0f/zI08/8yPPf/OkUP/0JZJ/8+URv/Plkn/0ZlP/9GZTv/Rmk7/0p1T/9SeVv/Un1f/1KBY/9SfVv/Uoln/1KNd/9enYf/XqWT/2a5u/9qxb//asG3/3LZ4/9u1dv/asW//4sKT//////+Ojo7/2NfW//f29f/39vX/oaGh//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////+NjY3/2NfW//f29f/39vX/xMPD/5+fn/+enp7/nZ2d/52dnf+cnJz/m5ub/5qamv+ampr/mZmZ/5iYmP+Xl5f/l5eX/5aWlv+VlZX/lJSU/5SUlP+Tk5P/kpKS/5KSkv+RkZH/kZGR/5CQkP+QkJD/j4+P/4+Pj/+Ojo7/jo6O/42Njf+cm5v/4N/e//f29f/39vX/9/b1/+/u7f/g397/2NfW/9jX1v/Y19b/2NfW/9jX1v/Y19b/2NfW/9jX1v/Y19b/2NfW/9jX1v/Y19b/2NfW/9jX1v/Y19b/2NfW/9jX1v/Y19b/2NfW/9jX1v/Y19b/2NfW/9jX1v/Y19b/2NfW/9jX1v/g397/7+7t//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f+8mg==";
    public static string insert = "eFY0EisAAAAtAAAA////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////3Nva/9zb2v/c29r/3Nva/9zb2v/c29r/3Nva/9zb2v/c29r/3Nva/9zb2v/c29r/3Nva/9zb2v/c29r/3Nva/9zb2v/c29r/3Nva/9zb2v/c29r/3Nva/9zb2v/c29r/3Nva/9zb2v/c29r/3Nva/9zb2v/c29r/3Nva/9zb2v/c29r/3Nva/9zb2v/c29r/3Nva/9zb2v/c29r/3Nva/9zb2v/c29r/3Nva//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1/46Ojv+Ojo7/8vHw//X08//39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1/46Ojv+Ojo7/8fDv//X08//39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f+Ojo7/jo6O//Hw7//29fT/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f+Ojo7/jo6O/46Ojv+Ojo7/jo6O/46Ojv+Ojo7/jo6O/46Ojv+Ojo7/jo6O/46Ojv+Ojo7/jo6O/46Ojv+Ojo7/jo6O/46Ojv+Ojo7/jo6O/46Ojv+Ojo7/jo6O/46Ojv+Ojo7/jo6O/46Ojv+Ojo7/jo6O/46Ojv/m5eT/8vHw//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/8vHw/46Ojv///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////46Ojv/Nzcz/3t3c//Hw7//39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//X08/+Ojo7//////////////////////////////////v7+//7+/v/+/v7//v7+//7+/v/+/v7//f39//39/f/9/f3//f39//39/f/8/Pz//Pz8//z8/P/8/Pz/+/v7//v7+//7+/v/+/v7//////+Ojo7/zMzL/+bl5P/19PP/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/jo6O/////////////////////////////v7+//7+/v/+/v7//v7+//7+/v/+/v7//f39//39/f/9/f3/ut+4/3/FfP9fuVr/TrFJ/121Wf95vXb/u9i6//X19f/5+fn/+/v7//v7+///////jo6O/9DPz//t7Ov/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1/46Ojv///////////////////////v7+//7+/v/+/v7//v7+//7+/v/+/v7//f39//39/f/m8uX/br5r/1a4Uv9dvVj/X8Bb/129WP9WuFL/TK9I/0CmPP9grFz/2uPZ//b29v/5+fn//////46Ojv/Qz8//7ezr//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f+Ojo7//////////////////v7+//7+/v/+/v7//v7+//7+/v/+/v7//f39//39/f/l8eX/WLRV/1a4Uv9iwl3/aslm/23Laf9qyWb/YsJd/1a4Uv9IrUT/OqE2/0KdPv/T3NP/9PT0//////+Ojo7/0M/P/+3s6//39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/jo6O/////////////v7+//7+/v/+/v7//v7+//7+/v/+/v7//f39//39/f/9/f3/aLdk/06xSf9dvVj/aslm/3TRcP951XX/dNFw/2rJZv9dvVj/TrFJ/z+kOv8wmCz/UZ5N/+Tk5P/7+/v/jo6O/9DPz//t7Ov/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1/46Ojv///////v7+//7+/v/+/v7//v7+//7+/v/+/v7//f39//39/f/9/f3/u9y6/0CmPP9Qs0v/X8Bb/23Laf951XX/gdx9/3nVdf9ty2n/X8Bb/1CzS/9Apjz/Mpkt/yWOIP+cupv/8PDw/42Njf/Qz8//7ezr//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f+Ojo7///////7+/v/+/v7//v7+//7+/v/+/v7//f39//39/f/9/f3//f39/3a6dP8/pDr/TrFJ/129WP9qyWb/dNFw/3nVdf900XD/aslm/129WP9OsUn/P6Q6/zCYLP8kjh//TpdK/+Hh4f+Li4v/0M/P/+3s6//39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/jo6O///////+/v7//v7+//7+/v/+/v7//f39//39/f/9/f3//f39//39/f9CoD3/OqE2/0itRP9WuFL/YsJd/2rJZv9ty2n/aslm/2LCXf9WuFL/SK1E/zqhNv8tlij/IYwd/yuKJ//U1NT/h4eH/9DPz//t7Ov/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1/46Ojv///////v7+//7+/v/+/v7//f39//3ixP//x4v//8eL///Hi///x4v//8eL///Hi///x4v//8eL///Hi///x4v/y5pZ/1m0VP9VtVH/TK9I/0CmPP80my//KJEj/x6JGf8bhBb/y8vL/4WFhf/Qz8//7ezr//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f+Ojo7///////7+/v/+/v7//f39//3ixP//x4v//8eL///Hi///x4v//8eL///Hi///x4v//8eL///Hi///x4v/y5pZ/6psK/9DmT//RaVB/0CmPP82nTL/LJUn/yKMHf8ahhX/JYch/8jIyP+EhIT/0M/P/+3s6//39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/jo6O///////+/v7//f39//3ixP//x4v//8eL///Hi///x4v//8eL///Hi///x4v//8eL///Hi///x4v/y5pZ/6psK/+qbCv/M4Yv/zaXM/80my//LJUn/ySOH/8chxf/F4MS/z2OOf/Kysr/hYWF/9DPz//t7Ov/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1/46Ojv///////f39//3ixP//x4v//8eL///Hi///x4v//8eL///Hi///x4v//8eL///Hi///x4v/yphX/6psK/+qbCv/qmwr/yd7JP8qjSX/KJEj/yKMHf8chxf/F4MS/xeDEv92oHT/0dHR/4aGhv/Qz8//7ezr//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1/46Ojv+Ojo7///////39/f/ypVP/8qVT//KlU//ypVP/8qVT//KlU//ypVP/8qVT//KlU//ypVP/7qJR/6psK/+qbCv/qmwr/6psK/8dcxn/H4Mb/x6JGf8ahhX/F4MS/xeDEv8wiiz/t7i3/97e3v+Kior/jo6O/+bl5P/19PP/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f+Ojo7/jo6O///////9/f3/8qVT//KlU//ypVP/8qVT//KlU//ypVP/8qVT//KlU//ypVP/8qVT//GkU/+qbCv/qmwr/6psK/+qbCv/FW0R/xd8E/8XgxL/F4MS/xeDEv8jhh//nayc/8nJyf/t7e3/jY2N/46Ojv/Y19b/7+7t//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/8O/u/46Ojv///////f39//KlU//ypVP/8qVT//KlU//ypVP/8qVT//KlU//ypVP/8qVT//KlU//ypVP/qmwr/6psK/+qbCv/qmwr/xNqD/8WexH/F4MS/xeDEv8ziy//nayc/8XFxf/f39//+vr6/46Ojv+8u7r/2NfW/+/u7f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//X08/+Ojo7///////39/f/ypVP/8qVT//KlU//ypVP/8qVT//KlU//ypVP/8qVT//KlU//ypVP/8qVT/6psK/+qbCv/qmwr/6psK/8Tag//JX8h/0WQQv9+o33/urq6/8nJyf/f39//7+/v//7+/v+Ojo7/ycnI/+bl5P/19PP/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/jo6O///////8/Pz/8qVT//KlU//ypVP/8qVT//KlU//ypVP/8qVT//KlU//ypVP/8qVT//KlU/+qbCv/qmwr/6psK/+qbCv/np6e/7a2tv/ExMT/zMzM/9jY2P/n5+f/8fHx//X19f//////jo6O/9DPz//t7Ov/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1/46Ojv///////Pz8//KlU//ypVP/8qVT//KlU//ypVP/8qVT//KlU//ypVP/8qVT//KlU//ypVP/qmwr/6psK/+qbCv/qmwr/7y8vP/Y2Nj/5+fn/+rq6v/w8PD/9PT0//b29v/29vb//////46Ojv/Qz8//7ezr//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f+Ojo7///////z8/P/ypVP/8qVT//KlU//ypVP/8qVT//KlU//ypVP/8qVT//KlU//ypVP/8qVT/6psK/+qbCv/qmwr/6psK//Jycn/6Ojo//f39//39/f/9vb2//b29v/29vb/9fX1//////+Ojo7/0M/P/+3s6//39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/jo6O///////8/Pz/8qVT//KlU//ypVP/8qVT//KlU//ypVP/8qVT//KlU//ypVP/8qVT//KlU/+qbCv/qmwr/6psK//CooH/zMzM/+np6f/39/f/9vb2//b29v/29vb/9fX1//X19f//////jo6O/9DPz//t7Ov/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1/46Ojv//////+/v7//KlU//ypVP/8qVT//KlU//ypVP/8qVT//KlU//ypVP/8qVT//KlU//ypVP/qmwr/6psK//CooH/xcXF/9ra2v/v7+//9vb2//b29v/29vb/9fX1//X19f/19fX//////46Ojv/Qz8//7ezr//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f+Ojo7///////v7+//ypVP/8qVT//KlU//ypVP/8qVT//KlU//ypVP/8qVT//KlU//ypVP/8qVT/6psK//CooH/xcXF/9jY2P/r6+v/9PT0//b29v/29vb/9fX1//X19f/19fX/9fX1//////+Ojo7/0M/P/+3s6//39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/jo6O///////7+/v/8qVT//KlU//ypVP/8qVT//KlU//ypVP/8qVT//KlU//ypVP/8qVT//KlU//CooH/xcXF/9jY2P/r6+v/9PT0//b29v/29vb/9fX1//X19f/19fX/9fX1//X19f//////jo6O/9DPz//t7Ov/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1/46Ojv//////+/v7/+/v7//X19f/y8vL/8vLy//Ly8v/ysrK/8rKyv/Kysr/ycnJ/8nJyf/Jycn/zMzM/9ra2v/r6+v/9PT0//b29v/29vb/9fX1//X19f/19fX/9fX1//X19f/09PT//////46Ojv/Qz8//7ezr//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f+Ojo7////////////7+/v/8/Pz/+/v7//v7+//7+/v/+/v7//v7+//7+/v/+/v7//v7+//7+/v//Hx8f/39/f//f39//////////////////////////////////////////////////////+Ojo7/0M/P/+3s6//39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f+Ojo7/jo6O/46Ojv+Ojo7/jo6O/46Ojv+Ojo7/jo6O/46Ojv+Ojo7/jo6O/46Ojv+Ojo7/jo6O/46Ojv+Ojo7/jo6O/46Ojv+Ojo7/jo6O/46Ojv+Ojo7/jo6O/46Ojv+Ojo7/jo6O/46Ojv+Ojo7/jo6O/46Ojv/m5eT/9fTz//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/jo6O/46Ojv/Kysn/ycnI/87Nzf/Ozc3/zs3N/87Nzf/Ozc3/zs3N/87Nzf/Ozc3/zs3N/87Nzf/Ozc3/jo6O/46Ojv+6ubj/yMfG/87Nzf/Ozc3/zs3N/87Nzf/Ozc3/zs3N/87Nzf/Ozc3/zs3N/46Ojv+Ojo7/3Nva//Dv7v/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//Lx8P/j4uH/29rZ/+Xk4//s6+r/7Ovq/+zr6v/s6+r/7Ovq/+zr6v/s6+r/7Ovq/+zr6v/s6+r/7Ovq/+Xk4//V1NP/1dTT/+Xk4//s6+r/7Ovq/+zr6v/s6+r/7Ovq/+zr6v/s6+r/7Ovq/+zr6v/l5OP/2tnY/+Lh4P/x8O//9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/19PP/8fDv//Dv7v/08/L/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/08/L/7u3s/+7t7P/08/L/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9PPy//Dv7v/x8O//9fTz//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f/39vX/9/b1//f29f+8mg==";
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

