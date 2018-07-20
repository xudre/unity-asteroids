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

    private void OnTriggerEnter2D(Collider2D other)
    {
      if (LevelManager.Instance.PlayerImmortal || !gameObject.activeSelf || Dead || !other.tag.Equals("EnemyBullet"))
        return;

      Life--;

      OnDead();

      if (Life > 0)
        LevelManager.Instance.PlayerRespawn();
    }

  }
}
