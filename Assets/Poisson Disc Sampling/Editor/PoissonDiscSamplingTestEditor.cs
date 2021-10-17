using System.Collections;
using System.Collections.Generic;
using UnityEditor; 
using UnityEngine;

[CustomEditor(typeof(PoissonDiscSamplingTest))]
public class PoissonDiscSamplingTestEditor : Editor {


    public override void OnInspectorGUI()

    {
        DrawDefaultInspector();

        PoissonDiscSamplingTest myTarget = (PoissonDiscSamplingTest) target;

        if (GUILayout.Button("Build Object"))
        {
            myTarget.GeneratePoints();
        }
    }

}
