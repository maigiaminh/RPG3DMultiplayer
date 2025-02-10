using System;
using System.Collections.Generic;
using UnityEngine;

namespace InsaneSystems.HealthbarsKit
{
	/// <summary> This class controls all Sprite healthbars on scene. </summary>
	public sealed class SpriteHealthbars : MonoBehaviour
	{
		static SpriteHealthbars instance;
		
		[SerializeField] GameObject healthbarTemplate;
		[Tooltip("Healthbar color by health percents.")]
		[SerializeField] Gradient colorByHealth = new Gradient();
		[Tooltip("Should healthbar be rotated to camera? Used for 3d games.")]
		[SerializeField] bool billboard;
		[Tooltip("Determines healthbar vertical offset scaling. Mostly used for 2d games, for example pixelart ones, which require same PPU for all sprites.")]
		[SerializeField] int pixelsPerUnit = 16;
		
		readonly List<SpriteHealthbar> healthbars = new List<SpriteHealthbar>();
		readonly List<IHealthed> targets = new List<IHealthed>();
		readonly Dictionary<IHealthed, GameObject> targetsLinks = new Dictionary<IHealthed, GameObject>();
		
		Transform cachedCamera;
		
		bool isInitialized;
		
		void Awake()
		{
			if (instance)
			{
				enabled = false;
				return;
			}

			if (!healthbarTemplate)
				throw new NullReferenceException($"{typeof(SpriteHealthbars)} field healthbarTemplate is not set!");
			
			instance = this;
			cachedCamera = Camera.main.transform;
			
			isInitialized = true;
		}

		void Update()
		{
			if (!cachedCamera)
				return;
			
			var camPos = cachedCamera.position + cachedCamera.forward * 1000f;
			
			for (var i = targets.Count - 1; i >= 0; i--)
			{
				var bar = healthbars[i];
				var target = targets[i];
				
				if (targetsLinks[target].gameObject == null)
				{
					Destroy(bar.Transform.gameObject);
					healthbars.RemoveAt(i);
					targets.RemoveAt(i);
					targetsLinks.Remove(target);
					continue;
				}

				var health = target.GetHealth();

				if (!Mathf.Approximately(bar.LastHealth, health))
					bar.UpdateView(target, GetColor(health / target.GetMaxHealth()));
				
				bar.Transform.position = target.GetGameObject().transform.position + bar.VerticalOffset;
				
				if (billboard)
					bar.Transform.LookAt(camPos);
			}
		}
		
		void OnDestroy()
		{
			if (instance == this)
				instance = null;
		}
		
		public static void AddHealthbar(IHealthed target, float verticalOffset = 20)
		{ 
			if (!instance || !instance.isInitialized)
				throw new Exception($"{typeof(SpriteHealthbars)} is not initialized due some errors.");
			
			if (!target.GetGameObject())
				throw new NullReferenceException("NULL is passed to the AddHealthbar target argument! It is possible, that object was destroyed or IHealthed GetGameObject method is incorrect.");

			var spawnedObject = Instantiate(instance.healthbarTemplate);
			var fillSprite = spawnedObject.transform.GetChild(0).GetComponent<SpriteRenderer>();
			
			var bar = new SpriteHealthbar
			{
				Transform = spawnedObject.transform,
				FillSprite = fillSprite,
				MaxSize = fillSprite.size.x,
				VerticalOffset = Vector3.up * verticalOffset / instance.pixelsPerUnit
			};

			fillSprite.drawMode = SpriteDrawMode.Sliced;
			
			var color = instance.GetColor(1);
			
			bar.UpdateView(target, color);
			bar.Transform.position = target.GetGameObject().transform.position + bar.VerticalOffset;
			
			instance.healthbars.Add(bar);
			instance.targets.Add(target);
			instance.targetsLinks[target] = target.GetGameObject();
		}

		public static void SetCustomColors(Gradient colorsGradient) => instance.colorByHealth = colorsGradient;
		public static void SetCustomCamera(Camera cam) => instance.cachedCamera = cam.transform;

		Color GetColor(float percents) => colorByHealth.Evaluate(percents);
	}
}