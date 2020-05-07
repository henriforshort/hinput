﻿using UnityEngine;

namespace HinputClasses.Internal {
    // Hinput class responsible for updating gamepads. 
    // It is automatically instantiated at runtime, or added to the gameobject with the HinputSettings component if you
    // created one.
    public class Updater : MonoBehaviour {
    	// --------------------
    	// SINGLETON PATTERN
    	// --------------------
    
    	//The instance of Updater. Assigned when first called.
    	private static Updater _instance;
    	public static Updater instance { 
    		get {
    			CheckInstance();
    			return _instance;
    		} 
    	}
    
    	public static void CheckInstance() {
    		if (_instance != null) return;
    		
    		_instance = Settings.instance.gameObject.AddComponent<Updater>();
            UpdateGamepads();
    	}
    
    	private void Awake () {
    		if (_instance == null) _instance = this;
    		if (_instance != this) Destroy(this);
    		DontDestroyOnLoad (this);
    	}
    
    
    	// --------------------
    	// UPDATE
    	// --------------------
    	
    	private void Update () {
    		UpdateGamepads();
    	}
    
        private static void UpdateGamepads () {
            Hinput.gamepad.ForEach(gamepad => gamepad.Update());
    		Hinput.anyGamepad.Update();
    	}
    
    
    	// --------------------
    	// ON APPLICATION QUIT
    	// --------------------
    
    	public void OnApplicationQuit() {
    		Hinput.anyGamepad.StopVibration();
    	}
    }
}