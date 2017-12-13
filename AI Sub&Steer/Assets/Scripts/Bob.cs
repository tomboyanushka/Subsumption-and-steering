using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bob : MonoBehaviour {

	
	// Update is called once per frame
	void Update () {

        transform.Translate(new Vector3(0, 0, Mathf.Sin(Time.time * 6)) * Time.deltaTime);

	}
}
