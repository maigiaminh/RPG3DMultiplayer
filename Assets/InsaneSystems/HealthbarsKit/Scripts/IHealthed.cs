using UnityEngine;

namespace InsaneSystems.HealthbarsKit
{
	public interface IHealthed
	{
		public GameObject GetGameObject();
		public float GetHealth();
		public float GetMaxHealth();
	}
}