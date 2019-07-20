﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// hinput class responsible for handling settings.<br/>
/// You can attach it to a GameObject to expose settings. 
/// If you don’t, it will automatically be instantiated at runtime the first time you call hinput, with default settings.
/// </summary>
public class hSettings : MonoBehaviour {
	// --------------------
	// SINGLETON PATTERN
	// --------------------

	//The instance of hinputSettings. Assigned when first called.
	private static hSettings _instance;
	public static hSettings instance { 
		get {
			if (_instance == null) {
				GameObject go = new GameObject();
				go.name = "hinputSettings";
				_instance = go.AddComponent<hSettings>();
			}
			
			return _instance;
		} 
	}

	private void Awake () {
		if (_instance == null) _instance = this;
		if (_instance != this) Destroy(this);
		DontDestroyOnLoad (this);

		if (_buildAllOnStartUp) {
			hinput.anyGamepad.BuildAll();
			for (int i=0; i<hUtils.maxGamepads; i++) hinput.gamepad[i].BuildAll();
		}
	}


	// --------------------
	// SETTINGS
	// --------------------

	[Header("hinput settings")]
	[Space(10)]

	[SerializeField]
	[Tooltip ("If enabled, hinput will start tracking every control of every gamepad from startup. "
	+"Otherwise, each control will only start being registered the first time you ask for it.")]
	private bool _buildAllOnStartUp = false;

	[SerializeField]
	[Range(0,1)]
	[Tooltip("The distance from the origin beyond which stick inputs start being registered (except for raw inputs).")]
	protected float _stickDeadZone = 0.2f;
	/// <summary>
	/// The distance from the origin beyond which stick inputs start being registered (except for raw inputs).
	/// </summary>
	public static float stickDeadZone { 
		get { return instance._stickDeadZone; } 
		set { instance._stickDeadZone = value; } 
	}

	[SerializeField]
	[Range(0,1)]
	[Tooltip("The distance from the origin beyond which trigger inputs start being registered (except for raw inputs).")]
	protected float _triggerDeadZone = 0.1f;
	/// <summary>
	/// The distance from the origin beyond which trigger inputs start being registered (except for raw inputs).
	/// </summary>
	public static float triggerDeadZone { 
		get { return instance._triggerDeadZone; } 
		set { instance._triggerDeadZone = value; } 
	}

	[SerializeField]
	[Range(0,1)]
	[Tooltip("The distance from the end of the dead zone beyond which stick inputs are considered pushed.")]
	private float _stickPressedZone = 0.5f;
	/// <summary>
	/// The distance from the end of the dead zone beyond which stick inputs are considered pushed.
	/// </summary>
	public static float stickPressedZone { 
		get { return instance._stickPressedZone; } 
		set { instance._stickPressedZone = value; }  
	}

	[SerializeField]
	[Range(0,1)]
	[Tooltip("The distance from the end of the dead zone beyond which trigger inputs are considered pushed.")]
	private float _triggerPressedZone = 0.5f;
	/// <summary>
	/// The distance from the end of the dead zone beyond which trigger inputs are considered pushed.
	/// </summary>
	public static float triggerPressedZone { 
		get { return instance._triggerPressedZone; } 
		set { instance._triggerPressedZone = value; }  
	}

	[SerializeField]
	[Range(45,90)]
	[Tooltip("The size of the angle that defines a stick direction.\n\n"+
	"Note : if it is higher than 45 degrees, directions like (up) and (leftUp) will overlap. " 
	+"Likewise, if it is lower than 90 degrees, there will be a gap between directions like (up) and (left).")]
	private float _directionAngle = 90f;
	/// <summary>
	/// The size of the angle that defines a stick direction. <br/>
	/// Note : if it is higher than 45 degrees, directions like (up) and (leftUp) will overlap.
	/// Likewise, if it is lower than 90 degrees, there will be a gap between directions like (up) and (left).
	/// </summary>
	public static float directionAngle { 
		get { return instance._directionAngle; } 
		set { instance._directionAngle = value; }  
	}

	[SerializeField]
	[Range(0,2)]
	[Tooltip("The maximum duration between the start of two presses for them to be considered a double press.")]
	private float _doublePressDuration = 0.3f;
	/// <summary>
	/// The maximum duration between the start of two presses for them to be considered a double press.
	/// </summary>
	public static float doublePressDuration { 
		get { return instance._doublePressDuration; } 
		set { instance._doublePressDuration = value; }  
	}

	[SerializeField]
	[Range(0,2)]
	[Tooltip("The minimum duration of a press for it to be considered a long press.")]
	private float _longPressDuration = 0.3f;
	/// <summary>
	/// The minimum duration of a press for it to be considered a long press.
	/// </summary>
	public static float longPressDuration { 
		get { return instance._longPressDuration; } 
		set { instance._longPressDuration = value; }  
	}

	[SerializeField]
	[Tooltip("The Camera on which the worldPositionCamera and worldPositionCameraRaw properties of hStick should be calculated. If no Camera is set, hinput will try to find one on your scene.")]
	private Transform _worldCamera = null;
	/// <summary>
	/// The Camera on which the worldPositionCamera and worldPositionCameraRaw properties of hStick should be calculated. If no Camera is set, hinput will try to find one on your scene.
	/// </summary>
	/// <remarks>
	/// hinput will first try to get the gameobject tagged “MainCamera”. If there isn’t one, hinput will get the first gameobject on the game scene that has a Camera component.
	/// If there is no Camera on the scene, hinput will return an error whenever you call a worldPositionCamera or worldPositionCameraRaw property.
	/// </remarks>
	public static Transform worldCamera { 
		get { 
			if (instance._worldCamera != null) return instance._worldCamera;
			else if (Camera.main != null) instance._worldCamera = Camera.main.transform;
			else if (GameObject.FindObjectOfType<Camera>() != null) instance._worldCamera = GameObject.FindObjectOfType<Camera>().transform;
			else { Debug.LogError ("hinput error : No camera found !"); return null; }
			return instance._worldCamera;
		} 
		set { instance._worldCamera = value; } 
	}
}