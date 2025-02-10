using UnityEngine;

namespace InsaneSystems.HealthbarsKit
{
	public class SpriteHealthbar
	{
		public IHealthed Target;
		public Transform Transform;
		public SpriteRenderer FillSprite;
		public Vector3 VerticalOffset;
		public float LastHealth;
		public float MaxSize;

		public void UpdateView(IHealthed target, Color color)
		{
			Target = target;
			
			var percents = target.GetHealth() / target.GetMaxHealth();
			FillSprite.size = new Vector2(percents * MaxSize, FillSprite.size.y);
			FillSprite.color = color;
			LastHealth = target.GetHealth();
		}
	}
}