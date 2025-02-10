using System;
using System.Collections.Generic;
using UnityEngine;

namespace InsaneSystems.HealthbarsKit.UI
{
	/// <summary> This class controls all Ui healthbars on scene. </summary>
	public sealed class UIHealthbars : MonoBehaviour
	{
		public delegate void HealthChangedAction(float newHealthValue);

		public static UIHealthbars Instance { get; private set; }
		public Camera CachedCamera { get; private set; }

		public bool SetColorByHealthPecents => setColorByHealthPercents;
		public bool HideHealthbarsIfHealthFull => hideHealthbarsIfHealthFull;

		readonly List<UIHealthbar> allHealthbars = new List<UIHealthbar>();

		[SerializeField] Canvas canvasForHealthbars;
		[SerializeField] GameObject healthbarObjectTemplate;
		[SerializeField] bool setColorByHealthPercents;
		[Tooltip("Healthbar color by health percents.")]
		[SerializeField] Gradient colorByHealth = new Gradient();
		[Tooltip("If selected, healthbars for units with 100% of health will be hidden until them will take some damage.")]
		[SerializeField] bool hideHealthbarsIfHealthFull = false;

		bool isInitialized;

		void Awake()
		{
			if (Instance)
			{
				enabled = false;
				return;
			}

			if (!canvasForHealthbars)
				throw new NullReferenceException($"{typeof(UIHealthbars)} field canvasForHealthbars is not set!");

			if (!healthbarObjectTemplate)
				throw new NullReferenceException($"{typeof(UIHealthbars)} field healthbarObjectTemplate is not set!");
			
			Instance = this;
			CachedCamera = Camera.main; // caching camera because Camera.main is expensive on maps with big objects count.

			isInitialized = true;
		}

		void Update()
		{
			for (int i = 0; i < allHealthbars.Count; i++)
				if (allHealthbars[i])
					allHealthbars[i].OnUpdate();
		}

		void OnDestroy()
		{
			if (Instance == this)
				Instance = null;
		}

		/// <summary> Use this to add healthbar for your characters objects. </summary>
		/// <param name="targetObject">Your character object which healthbar should be attached to.</param>
		/// <param name="targetMaxHealth">Your character max health value. Needed to correct calculations of healthbar percents.</param>
		public static UIHealthbar AddHealthbar(GameObject targetObject, float targetMaxHealth)
		{
			if (!Instance || !Instance.isInitialized)
				throw new Exception($"{typeof(UIHealthbars)} is not initialized due some errors.");
			
			if (!targetObject)
				throw new NullReferenceException("NULL is passed to the AddHealthbar targetObject argument! It is possible, that object was destroyed.");
			
			var spawnedHealthbarObject = Instantiate(Instance.healthbarObjectTemplate, Instance.canvasForHealthbars.transform);
			var healthbar = spawnedHealthbarObject.GetComponent<UIHealthbar>();

			healthbar.SetupWithTarget(targetObject.transform, targetMaxHealth);

			Instance.allHealthbars.Add(healthbar);

			return healthbar;
		}

		public static void ClearHealthbars()
		{
			for (int i = 0; i < Instance.allHealthbars.Count; i++)
				Destroy(Instance.allHealthbars[i].gameObject);

			Instance.allHealthbars.Clear();
		}

		public static void SetHealthbarTemplate(GameObject newTemplate) => Instance.healthbarObjectTemplate = newTemplate;
		public static void SetCustomColors(Gradient colorGradient) => Instance.colorByHealth = colorGradient;
		public static void SetCustomCamera(Camera cam) => Instance.CachedCamera = cam;
		
		public static Color GetColor(float healthPercents) => Instance.colorByHealth.Evaluate(healthPercents);
	}
}