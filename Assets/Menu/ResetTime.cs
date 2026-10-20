using UnityEngine;public class ResetTime : MonoBehaviour{void Start(){ResetTimeScale();}void Awake(){ResetTimeScale();}void ResetTimeScale(){Time.timeScale = 1f;}}
