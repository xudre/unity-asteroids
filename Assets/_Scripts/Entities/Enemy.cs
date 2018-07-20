using UnityEngine;

namespace Asteroids
{
  public class Enemy : Destructible
  {

    [SerializeField]
    private float _rotationSpeed = 35f;
    [SerializeField]
    private float _moveSpeed = 15f;

    public float RotationSpeed
    {
      get { return _rotationSpeed; }
    }

    public float MoveSpeed
    {
      get { return _moveSpeed; }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
      if (!gameObject.activeSelf || Dead || !other.tag.Equals("PlayerBullet"))
        return;

      Life--;

      other.gameObject.SetActive(false);

      if (!Dead)
        return;

      OnDead();
    }
  }
}
