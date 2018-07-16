using UnityEngine;

namespace Asteroids
{
  public class Player : Destructible
  {

    [SerializeField]
    private float _rotationSpeed;
    [SerializeField]
    private float _thrustSpeed;
    [SerializeField]
    private SpriteRenderer[] _flames;

    public float RotationSpeed
    {
      get { return _rotationSpeed; }
    }

    public float ThrustSpeed
    {
      get { return _thrustSpeed; }
    }

    public SpriteRenderer[] Flames
    {
      get { return _flames; }
    }

  }
}
