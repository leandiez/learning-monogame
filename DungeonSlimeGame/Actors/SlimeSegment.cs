using Microsoft.Xna.Framework;

namespace DungeonSlimeGame.Actors;

public struct SlimeSegment
{
    /// <summary>
    /// Posicion desde donde inicial el segmento esta antes del ciclo de movimiento.
    /// </summary>
    public Vector2 At;

    /// <summary>
    /// Posicion final donde se movera durante el ciclo de movimiento.
    /// </summary>
    public Vector2 To;

    /// <summary>
    /// Direccion actual de movimiento del segmento.
    /// </summary>
    public Vector2 Direction;

    /// <summary>
    /// Atajo para obtener direccion opuesta en base a la direccion actual.
    /// </summary>
    public Vector2 ReverseDirection => new Vector2(-Direction.X, -Direction.Y);
}
