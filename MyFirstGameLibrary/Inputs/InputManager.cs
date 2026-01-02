namespace MyFirstGameLibrary.Inputs;
using Microsoft.Xna.Framework;
using System.Runtime.InteropServices;
// TODO: Implementar patron Command para agregar capa que separe acciones del juego de los distintos Inputs
public class InputManager
{
    /// <summary>
    /// Gets the state information of keyboard input.
    /// </summary>
    public KeyboardInfo Keyboard { get; private set; }

    /// <summary>
    /// Gets the state information of mouse input.
    /// </summary>
    public MouseInfo Mouse { get; private set; }

    /// <summary>
    /// Gets the state information of a gamepad.
    /// </summary>
    public GamePadInfo[] GamePads { get; private set; }

    /// <summary>
    /// Creates a new InputManager.
    /// </summary>
    /// <param name="game">The game this input manager belongs to.</param>
    public InputManager()
    {
        #if WINDOWS
        // Agrego GamePad sin registrar en SDL.
        SDL_GameControllerAddMapping("03000000790000000600000000000000,G-Shark GS-GP702,a:b2,b:b1,back:b8,dpdown:h0.4,dpleft:h0.8,dpright:h0.2,dpup:h0.1,leftshoulder:b4,leftstick:b10,lefttrigger:b6,leftx:a0,lefty:a1,rightshoulder:b5,rightstick:b11,righttrigger:b7,rightx:a2,righty:a4,start:b9,x:b3,y:b0,platform:Windows,");
        #endif
        Keyboard = new KeyboardInfo();
        Mouse = new MouseInfo();

        GamePads = new GamePadInfo[4];
        for (int i = 0; i < 4; i++)
        {
            GamePads[i] = new GamePadInfo((PlayerIndex)i);
        }
    }
    /// <summary>
    /// Updates the state information for the keyboard, mouse, and gamepad inputs.
    /// </summary>
    /// <param name="gameTime">A snapshot of the timing values for the current frame.</param>
    public void Update(GameTime gameTime)
    {
        Keyboard.Update();
        Mouse.Update();

        for (int i = 0; i < 4; i++)
        {
            GamePads[i].Update(gameTime);
        }
    }
    
    
    
    
    
    // Llamada a libreria SDL para agregar Joysticks que no esten listados como GamePads en la libreria nativa
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