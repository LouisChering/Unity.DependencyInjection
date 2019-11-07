using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SomeDependency : MonoBehaviour,ISomeInterface {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public int DoSomething()
    {
        Debug.Log("Did something");
        return -1;
    }
}

public interface ISomeInterface
{
    int DoSomething();
}