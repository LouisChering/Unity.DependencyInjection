using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ResolutionFramework;

public class Tester : MonoBehaviour {

    [ResolveLocal]
    private SomeDependency Local;

    [ResolveInChildren]
    public SomeDependency InChildren;

    [ResolveByTag("MyTag")]
    public SomeDependency ByTag;

    [ResolveByName("NamedObject")]
    public SomeDependency ByName;

    [ResolveByName("GlobalObject")]
    public SomeDependency GlobalDependency;

    [@Resolve]
    public ISomeInterface KernelDependency;

    // Use this for initialization
    void Start () {
        this.Resolve();
        Debug.Log(KernelDependency);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
