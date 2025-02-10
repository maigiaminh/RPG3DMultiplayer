using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace InsaneSystems.HealthbarsKit
{
	public sealed class ExampleSpriteController : MonoBehaviour
	{
		public static bool UseCustomColors { get; set; }
		public static Gradient ColorsGradient { get; set; }

		Camera cachedCamera;

		void Awake()
		{
			if (UseCustomColors)
				SpriteHealthbars.SetCustomColors(ColorsGradient);
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
				UseCustomColors = false;

			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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