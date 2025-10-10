using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MyFirstGameLibrary;
using MyFirstGameLibrary.Inputs;

namespace DungeonSlimeGame.Utils;

/// <summary>
/// Provee una abstraccion especifica del juego, que mapea inputs fisicos
/// a acciones del juego, sirviendo de bridge entre el sistema de input y funcionalidad.
/// Implementa el patron Command
/// </summary>
public static class GameActions
{
    private static KeyboardInfo s_keyboard => Core.Input.Keyboard;
    private static GamePadInfo s_gamePad => Core.Input.GamePads[(int)PlayerIndex.One];

    public static bool MoveUp()
    {
        return s_keyboard.WasKeyJustPressed(Keys.Up) ||
               s_keyboard.WasKeyJustPressed(Keys.W) ||
               s_gamePad.WasButtonJustPressed(Buttons.DPadUp) ||
               s_gamePad.WasButtonJustPressed(Buttons.LeftThumbstickUp);
    }

    public static bool MoveDown()
    {
        return s_keyboard.WasKeyJustPressed(Keys.Down) ||
               s_keyboard.WasKeyJustPressed(Keys.S) ||
               s_gamePad.WasButtonJustPressed(Buttons.DPadDown) ||
               s_gamePad.WasButtonJustPressed(Buttons.LeftThumbstickDown);
    }

    public static bool MoveLeft()
    {
        return s_keyboard.WasKeyJustPressed(Keys.Left) ||
               s_keyboard.WasKeyJustPressed(Keys.A) ||
               s_gamePad.WasButtonJustPressed(Buttons.DPadLeft) ||
               s_gamePad.WasButtonJustPressed(Buttons.LeftThumbstickLeft);
    }

    public static bool MoveRight()
    {
        return s_keyboard.WasKeyJustPressed(Keys.Right) ||
               s_keyboard.WasKeyJustPressed(Keys.D) ||
               s_gamePad.WasButtonJustPressed(Buttons.DPadRight) ||
               s_gamePad.WasButtonJustPressed(Buttons.LeftThumbstickRight);
    }

    /// <summary>
    /// Verdadero si se presionaron teclas que ejecutan la pausa
    /// </summary>
    public static bool Pause()
    {
        return s_keyboard.WasKeyJustPressed(Keys.Escape) ||
               s_gamePad.WasButtonJustPressed(Buttons.Start);
    }

    /// <summary>
    /// Verdadero si se presiono la tecla que confirma acciones en el juego
    /// </summary>
    public static bool Action()
    {
        return s_keyboard.WasKeyJustPressed(Keys.Enter) ||
               s_gamePad.WasButtonJustPressed(Buttons.A);
    }
}
