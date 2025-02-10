using InsaneSystems.HealthbarsKit.UI;
using UnityEngine;

namespace InsaneSystems.HealthbarsKit
{
	/// <summary>This component allows to make any game object having health and its own healthbar. Use this in your game or you can make your custom class, using its code as template. </summary>
	public sealed class Damageable : MonoBehaviour, IHealthed
	{
		enum HealthbarsMode
		{
			UI,
			Sprite
		}
		
		public event UIHealthbars.HealthChangedAction HealthWasChanged;

		[SerializeField] [Range(0.1f, 1000f)] float maxHealth = 100;
		[SerializeField] HealthbarsMode mode = HealthbarsMode.UI;
		
		float health;

		void Awake() => health = maxHealth;
		void Start() => AddHealthbarToThisObject();

		void AddHealthbarToThisObject()
		{
			if (mode == HealthbarsMode.UI)
			{
				var healthBar = UIHealthbars.AddHealthbar(gameObject, maxHealth);
				
				// setting up event to connect the Healthbar with this Damageable.
				// Now every time when it will take damage, Healthbar will be updated.
				HealthWasChanged += healthBar.OnHealthChanged; 
			}
			else if (mode == HealthbarsMode.Sprite)
			{
				SpriteHealthbars.AddHealthbar(this);
			}
			
			OnHealthChanged();
		}

		void OnHealthChanged() => HealthWasChanged?.Invoke(health);

		public void TakeDamage(float damage)
		{
			health = Mathf.Clamp(health - damage, 0, maxHealth);

			OnHealthChanged();

			if (health == 0)
				Die();
		}

		public void Die() => Destroy(gameObject);

		public GameObject GetGameObject() => gameObject;
		public float GetHealth() => health;
		public float GetMaxHealth() => maxHealth;
	}
}