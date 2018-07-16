using UnityEngine;

namespace Asteroids
{
  public class Player : Destructible
  {

    [SerializeField]
    private float _rotationSpeed = 5f;
    [SerializeField]
    private float _thrustSpeed = 5f;

    public float RotationSpeed
    {
      get { return _rotationSpeed; }
    }

    public float ThrustSpeed
    {
      get { return _thrustSpeed; }
    }

  }
}
