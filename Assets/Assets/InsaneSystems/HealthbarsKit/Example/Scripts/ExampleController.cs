using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace InsaneSystems.HealthbarsKit
{
	public class ExampleController : MonoBehaviour
	{
		public static GameObject CustomHealthbarToSet { get; set; }
		public static bool UseCustomColors{ get; set; }
		public static Gradient ColorsGradient { get; set; }

		[SerializeField] List<GameObject> healthbarTemplatesDemo = new List<GameObject>();

		Camera cachedCamera;

		void Awake()
		{
			if (CustomHealthbarToSet || UseCustomColors)
			{
				if (CustomHealthbarToSet)
					UI.UIHealthbars.SetHealthbarTemplate(CustomHealthbarToSet);

				if (UseCustomColors)
					UI.UIHealthbars.SetCustomColors(ColorsGradient);
			}
		}

		void Start()
		{
			cachedCamera = Camera.main;
		}

		void Update()
		{
			if (Input.GetMouseButtonDown(0))
			{
				var ray = cachedCamera.ScreenPointToRay(Input.mousePosition);

				if (Physics.Raycast(ray, out var hit, 1000f))
				{
					var damageable = hit.collider.GetComponent<Damageable>();

					if (damageable)
						damageable.TakeDamage(10f);
				}
			}
		}

		public void OnResetButtonClick(bool fullReset = true)
		{
			if (fullReset)
			{
				CustomHealthbarToSet = null;
				UseCustomColors = false;
			}

			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}

		public void OnHealthbarChangeButtonClick(int healthbarToSelect)
		{
			if (healthbarTemplatesDemo.Count <= healthbarToSelect)
				return;

			CustomHealthbarToSet = healthbarTemplatesDemo[healthbarToSelect];

			OnResetButtonClick(false);
		}

		public void OnSetDefaultColorSchemeClick()
		{
			UseCustomColors = false;

			OnResetButtonClick(false);
		}

		public void OnHealthbarBlueColorButtonClick()
		{
			UseCustomColors = true;

			ColorsGradient = new Gradient
			{
				colorKeys = new[]
				{
					new GradientColorKey(new Color(1f, 0.35f, 0f), 0f),
					new GradientColorKey(new Color(0.17f, 0.35f, 1f), 1f),
				}
			};

			OnResetButtonClick(false);
		}

		public void OnHealthbarSingleColorButtonClick()
		{
			UseCustomColors = true;
			
			ColorsGradient = new Gradient
			{
				colorKeys = new[]
				{
					new GradientColorKey(Color.green, 0f),
					new GradientColorKey(Color.green, 1f),
				}
			};

			OnResetButtonClick(false);
		}
	}
}