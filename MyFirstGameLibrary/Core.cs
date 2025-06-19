using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.InteropServices;
namespace MyFirstGameLibrary;

public class Core : Game
{
    internal static Core s_instance;

    /// <summary>
    /// Gets a reference to the Core instance.
    /// </summary>
    public static Core Instance => s_instance;

    /// <summary>
    /// Gets the graphics device manager to control the presentation of graphics.
    /// </summary>
    public static GraphicsDeviceManager Graphics { get; private set; }

    /// <summary>
    /// Gets the graphics device used to create graphical resources and perform primitive rendering.
    /// </summary>
    public static new GraphicsDevice GraphicsDevice { get; private set; }

    /// <summary>
    /// Gets the sprite batch used for all 2D rendering.
    /// </summary>
    public static SpriteBatch SpriteBatch { get; private set; }

    /// <summary>
    /// Gets the content manager used to load global assets.
    /// </summary>
    public static new ContentManager Content { get; private set; }

    /// <summary>
    /// Creates a new Core instance.
    /// </summary>
    /// <param name="title">The title to display in the title bar of the game window.</param>
    /// <param name="width">The initial width, in pixels, of the game window.</param>
    /// <param name="height">The initial height, in pixels, of the game window.</param>
    /// <param name="fullScreen">Indicates if the game should start in fullscreen mode.</param>
    public Core(string title, int width, int height, bool fullScreen)
    {
        // Ensure that multiple cores are not created.
        if (s_instance != null)
        {
            throw new InvalidOperationException($"Only a single Core instance can be created");
        }

        // Store reference to engine for global member access.
        s_instance = this;

        // Create a new graphics device manager.
        Graphics = new GraphicsDeviceManager(this);

        // Set the graphics defaults
        Graphics.PreferredBackBufferWidth = width;
        Graphics.PreferredBackBufferHeight = height;
        Graphics.IsFullScreen = fullScreen;

        // Apply the graphic presentation changes
        Graphics.ApplyChanges();

        // Set the window title
        Window.Title = title;

        // Set the core's content manager to a reference of hte base Game's
        // content manager.
        Content = base.Content;

        // Set the root directory for content
        Content.RootDirectory = "Content";

        // Mouse is visible by default
        IsMouseVisible = true;
        
        // Agrego GamePad sin registrar en SDL. Ver de mover esto a un InputManager
        SDL_GameControllerAddMapping("03000000790000000600000000000000,G-Shark GS-GP702,a:b2,b:b1,back:b8,dpdown:h0.4,dpleft:h0.8,dpright:h0.2,dpup:h0.1,leftshoulder:b4,leftstick:b10,lefttrigger:b6,leftx:a0,lefty:a1,rightshoulder:b5,rightstick:b11,righttrigger:b7,rightx:a2,righty:a4,start:b9,x:b3,y:b0,platform:Windows,");
    }

    protected override void Initialize()
    {
        base.Initialize();

        // Set the core's graphics device to a reference of the base Game's
        // graphics device.
        GraphicsDevice = base.GraphicsDevice;

        // Create the sprite batch instance.
        SpriteBatch = new SpriteBatch(GraphicsDevice);
    }
    [DllImport("SDL2.dll", CallingConvention=CallingConvention.Cdecl)]
    public static extern int SDL_GameControllerAddMapping(string mappingString);

    /* Extraido desde Source Code de MonoGame. Es para emular la carga de la libreria desde distintas plataformas y considerando todos los escenarios posibles de SDL Nativo
     
    public static IntPtr NativeLibrary = LoadLibraryExt("SDL2.dll");
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int d_sdl_gamecontrolleraddmapping(string mappingString);
    public static d_sdl_gamecontrolleraddmapping AddMapping = LoadFunction<d_sdl_gamecontrolleraddmapping>(NativeLibrary, "SDL_GameControllerAddMapping");
    public static T LoadFunction<T>(IntPtr library, string function, bool throwIfNotFound = false)
    {
        var ret = IntPtr.Zero;
        ret = GetProcAddress(library, function);

        if (ret == IntPtr.Zero)
        {
            if (throwIfNotFound)
                throw new EntryPointNotFoundException(function);

            return default(T);
        }
        #if NETSTANDARD
            return Marshal.GetDelegateForFunctionPointer<T>(ret);
        #else
            return (T)(object)Marshal.GetDelegateForFunctionPointer(ret, typeof(T));
        #endif
    }
    
    [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern IntPtr LoadLibraryW(string lpszLib);
    [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
    public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);
    public static IntPtr LoadLibraryExt(string libname)
    {
        var ret = IntPtr.Zero;
        var assemblyLocation = Path.GetDirectoryName(System.AppContext.BaseDirectory) ?? "./";

        // Try .NET Framework / mono locations
        
        if (Environment.Is64BitProcess) ret = LoadLibraryW(Path.Combine(assemblyLocation, "x64", libname));
        else ret = LoadLibraryW(Path.Combine(assemblyLocation, "x86", libname));

        // Try .NET Core development locations
        if (ret == IntPtr.Zero)
            ret = LoadLibraryW(Path.Combine(assemblyLocation, "runtimes", "win-x64", "native", libname));

        // Try current folder (.NET Core will copy it there after publish)
        if (ret == IntPtr.Zero)
            ret = LoadLibraryW(Path.Combine(assemblyLocation, libname));

        // Try alternate way of checking current folder
        // assemblyLocation is null if we are inside macOS app bundle
        if (ret == IntPtr.Zero)
            ret = LoadLibraryW(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, libname));

        // Try loading system library
        if (ret == IntPtr.Zero)
            ret = LoadLibraryW(libname);

        // Welp, all failed, PANIC!!!
        if (ret == IntPtr.Zero)
            throw new Exception("Failed to load library: " + libname);

        return ret;
    }
    */
    
}
