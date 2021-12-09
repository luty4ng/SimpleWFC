using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;

[CreateAssetMenu(menuName = "Wave Function Collapse/Module Data", fileName = "Modules.asset")]
public class ModuleData : ScriptableObject
{
    public static Module[] Current;
    public GameObject Prototypes;
    public Module[] Modules;

#if UNITY_EDITOR
    public void CreateModules(bool respectNeigborExclusions = true)
    {
        int count = 0;
        var modules = new List<Module>();
        Prototype[] prototypes = Prototypes.GetComponentsInChildren<Prototype>();
        var scenePrototype = new Dictionary<Module, Prototype>();
        for (int i = 0; i < prototypes.Length; i++)
        {
            var prototype = prototypes[i];
            for (int rotation = 0; rotation < 4; rotation++)
            {
                if (rotation == 0 || !prototype.CompareRotatedVariants(0, rotation))
                {
                    var module = new Module(prototype.gameObject, rotation, count);
                    modules.Add(module);
                    scenePrototype[module] = prototype;
                    count++;
                }
            }
            EditorUtility.DisplayProgressBar("Creating module prototypes...", prototype.gameObject.name, (float)i / prototypes.Length);
        }

        foreach (var module in modules)
        {
            for (int direction = 0; direction < 6; direction++)
            {
                var face = scenePrototype[module].Faces[direction];
                IEnumerable<Module> neighbors = modules.Where(neighbor => module.Fits(direction, neighbor));
                foreach (var neighbor in neighbors)
                {
                    module.possibleNeighbors[direction].adjacent.Add(neighbor.name);
                }
            }
        }

        ModuleData.Current = modules.ToArray();
        EditorUtility.ClearProgressBar();
        this.Modules = modules.ToArray();
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
    }
#endif
}
