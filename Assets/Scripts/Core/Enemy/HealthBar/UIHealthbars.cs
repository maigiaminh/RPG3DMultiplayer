using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIHealthbars : Singleton<UIHealthbars>
{
	public float rangeDetectHealthBar = 20f;
	public LayerMask playerLayer;
	public float detectAngle = 90;
	public delegate void HealthChangedAction(float newHealthValue);

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
	private Transform player;


	protected override void Awake()
	{
		player = FindAnyObjectByType<TankPlayer>().transform;

		if (!canvasForHealthbars)
			throw new NullReferenceException($"{typeof(UIHealthbars)} field canvasForHealthbars is not set!");

		if (!healthbarObjectTemplate)
			throw new NullReferenceException($"{typeof(UIHealthbars)} field healthbarObjectTemplate is not set!");

		CachedCamera = Camera.main; // caching camera because Camera.main is expensive on maps with big objects count.
		Debug.Log("UIHealthbars Created");
		isInitialized = true;
	}

	private void FixedUpdate()
	{
		if (PlayerSpawnManager.Instance && SceneManager.GetActiveScene().name.Equals(PlayerSpawnManager.Instance.DUNGEON_SPECIAL_SCENE_NAME)) return;
		foreach (var healthbar in allHealthbars)
		{
			if (healthbar)
			{
				if (healthbar.target == null)
				{
					Destroy(healthbar.gameObject);
					allHealthbars.Remove(healthbar);
					continue;
				}
				bool shouldShow = ShouldShowHealthbar(healthbar.target);
				healthbar.gameObject.SetActive(shouldShow);

				if (shouldShow)
					healthbar.OnUpdate();
			}
		}
	}


	bool ShouldShowHealthbar(Transform enemyTransform)
	{
		return IsWithinDistance(enemyTransform)
				&& IsWithinAngle(enemyTransform)
				&& !IsObstructed(enemyTransform);
	}

	private bool IsObstructed(Transform enemyTransform)
	{
		Vector3 directionToPlayer = player.position - enemyTransform.position;
		RaycastHit hit;
		if (Physics.Raycast(enemyTransform.position, directionToPlayer.normalized, out hit, rangeDetectHealthBar, playerLayer))
		{
			// Kiểm tra xem vật cản có phải enemy không
			return !hit.transform.CompareTag("Player");
		}

		return false; // Không có vật cản
	}

	private bool IsWithinAngle(Transform enemyTransform)
	{
		Vector3 directionToEnemy = enemyTransform.position - player.position;
		float angle = Vector3.Angle(player.forward, directionToEnemy);
		return angle <= detectAngle;
	}

	private bool IsWithinDistance(Transform enemyTransform)
	{
		return Vector3.Distance(player.position, enemyTransform.position) <= rangeDetectHealthBar;
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