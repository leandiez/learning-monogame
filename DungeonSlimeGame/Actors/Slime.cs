using System;
using System.Collections.Generic;
using DungeonSlimeGame.Utils;
using Microsoft.Xna.Framework;
using MyFirstGameLibrary;
using MyFirstGameLibrary.Graphics;
using MyFirstGameLibrary.Primitives;
namespace DungeonSlimeGame.Actors;

public class Slime(Vector2 position, Vector2 velocity, AnimatedSprite animation) {
    // Propiedades basicas
    public Vector2 Position { get; set; } = position;
    public Vector2  Velocity { get; set; } = velocity;
    public AnimatedSprite Animation { get; set; }
    private AnimatedSprite _animation = animation;
    public Circle Collider { get; private set; }
    // ------------- Propiedades de mecanica Snake -------------------------
    // Tiempo a esperar entre cambios de movimiento. Simula un poco los ticks, de un juego de Snake
    private static readonly TimeSpan s_movementTime = TimeSpan.FromMilliseconds(200);
    // Acumulador del tiempo transcurrido hasta el s_movementTime
    private TimeSpan _movementTimer;
    // Valor normalizado del tick, para calcular que posicion interpolar entre el origen y destino del movimiento
    private float _movementProgress;
    // Direccion a aplicar el proximo ciclo de movimiento
    private Vector2 _nextDirection;

    // Numero de pixeles a mover la cabeza durante el ciclo de movimiento
    private float _stride;
    // Segmentos de todo el Slime
    private List<SlimeSegment> _segments;

    /// <summary>
    /// Evento disparado cuando se choca el Slime
    /// </summary>
    public event EventHandler BodyCollision;

    /// <summary>
    /// Inicializa el Slime y puede ser usado para reiniciar su estado.
    /// </summary>
    /// <param name="startingPosition">Posicion inicial de la cabeza</param>
    /// <param name="stride">Numero de pixeles a mover la cabeza durante el ciclo de movimiento</param>
    public void Initialize(Vector2 startingPosition, float stride)
    {
        _segments = new List<SlimeSegment>();
        _stride = stride;

        // Creacion de la cabeza.
        SlimeSegment head = new SlimeSegment();
        head.At = startingPosition;
        head.To = startingPosition + new Vector2(_stride, 0);
        head.Direction = Vector2.UnitX; // Su direccion inicial es hacia la derecha
        _segments.Add(head);

        // Seteo siguiente direccion
        _nextDirection = head.Direction;

        // Inicializo el contador de "ticks"
        _movementTimer = TimeSpan.Zero;
    }
    // Actualiza la direccion del Slime dependiendo que accion este siendo ejecutada desde el jugador
    private void HandleInput() {
        Vector2 potentialNextDirection = _nextDirection;

        if (GameActions.MoveUp()) {
            potentialNextDirection = -Vector2.UnitY;
        }
        else if (GameActions.MoveDown()) {
            potentialNextDirection = Vector2.UnitY;
        }
        else if (GameActions.MoveLeft()) {
            potentialNextDirection = -Vector2.UnitX;
        }
        else if (GameActions.MoveRight()) {
            potentialNextDirection = Vector2.UnitX;
        }
        // Permite cambiar de direccion solo si no es la opuesta.
        float dot = Vector2.Dot(potentialNextDirection, _segments[0].Direction);
        if (dot >= 0) {
            _nextDirection = potentialNextDirection;
        }
    }

    private void Move()
    {
        SlimeSegment head = _segments[0];

        // Actualizo la direccion de la cabeza en funcion de lo que HandleInput haya actualizado
        head.Direction = _nextDirection;

        // Actualizo la posicion actual a la destino ya que es un nuevo ciclo de movimiento
        head.At = head.To;

        // Actualizo la posicion destino usando la direccion y los pixeles de distancia
        head.To = head.At + head.Direction * _stride;

        // Insert the new adjusted value for the head at the front of the
        // segments and remove the tail segment. This effectively moves
        // the entire chain forward without needing to loop through every
        // segment and update its "at" and "to" positions.
        _segments.Insert(0, head);
        _segments.RemoveAt(_segments.Count - 1);

        // Iterate through all of the segments except the head and check
        // if they are at the same position as the head. If they are, then
        // the head is colliding with a body segment and a body collision
        // has occurred.
        for (int i = 1; i < _segments.Count; i++)
        {
            SlimeSegment segment = _segments[i];

            if (head.At == segment.At)
            {
                if(BodyCollision != null)
                {
                    BodyCollision.Invoke(this, EventArgs.Empty);
                }

                return;
            }
        }
    }

    /// <summary>
    /// Informs the slime to grow by one segment.
    /// </summary>
    public void Grow()
    {
        // Capture the value of the tail segment
        SlimeSegment tail = _segments[_segments.Count - 1];

        // Create a new tail segment that is positioned a grid cell in the
        // reverse direction from the tail moving to the tail.
        SlimeSegment newTail = new SlimeSegment();
        newTail.At = tail.To + tail.ReverseDirection * _stride;
        newTail.To = tail.At;
        newTail.Direction = Vector2.Normalize(tail.At - newTail.At);

        // Add the new tail segment
        _segments.Add(newTail);
    }


/// <summary>
/// Updates the slime.
/// </summary>
/// <param name="gameTime">A snapshot of the timing values for the current update cycle.</param>
public void Update(GameTime gameTime)
{
    // Update the animated sprite.
    _animation.Update(gameTime);

    // Handle any player input
    HandleInput();

    // Increment the movement timer by the frame elapsed time.
    _movementTimer += gameTime.ElapsedGameTime;

    // If the movement timer has accumulated enough time to be greater than
    // the movement time threshold, then perform a full movement.
    if (_movementTimer >= s_movementTime)
    {
        _movementTimer -= s_movementTime;
        Move();
    }

    // Update the movement lerp offset amount
    _movementProgress = (float)(_movementTimer.TotalSeconds / s_movementTime.TotalSeconds);
}

    /// <summary>
    /// Draws the slime.
    /// </summary>
    public void Draw() {
        // Iterate through each segment and draw it
        foreach (SlimeSegment segment in _segments) {
            // Calculate the visual position of the segment at the moment by
            // lerping between its "at" and "to" position by the movement
            // offset lerp amount
            Vector2 pos = Vector2.Lerp(segment.At, segment.To, _movementProgress);

            // Draw the slime sprite at the calculated visual position of this
            // segment
            _animation.Draw(Core.SpriteBatch, pos);
        }
    }
/// <summary>
/// Returns a Circle value that represents collision bounds of the slime.
/// </summary>
/// <returns>A Circle value.</returns>
public Circle GetBounds()
{
    SlimeSegment head = _segments[0];

    // Calculate the visual position of the head at the moment of this
    // method call by lerping between the "at" and "to" position by the
    // movement offset lerp amount
    Vector2 pos = Vector2.Lerp(head.At, head.To, _movementProgress);

    // Create the bounds using the calculated visual position of the head.
    Circle bounds = new Circle(
        (int)(pos.X + (_animation.Width * 0.5f)),
        (int)(pos.Y + (_animation.Height * 0.5f)),
        (int)(_animation.Width * 0.5f)
    );

    return bounds;
}

}