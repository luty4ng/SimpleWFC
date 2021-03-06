using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
public enum CollapseMode
{
    LowestEntropy,
    HighestEntropy,
    Random
}
public class WFCGenerator : MonoBehaviour
{
    public static WFCGenerator instance;
    public Vector3Int mapSize;
    public ModuleData moduleData;
    public CollapseMode collapseMode;
    public bool isGenerating = false;
    public bool showProcess = true;
    private BlockMap blockMap;
    private Queue<Slot> propaQueue;
    private List<Slot> renderedSlot;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void Start()
    {
        Module borderModule = moduleData.Modules.ToList().Find(m => m.name == "OuterEmpty R0");
        blockMap = new BlockMap(mapSize, borderModule);
        propaQueue = new Queue<Slot>();
        renderedSlot = new List<Slot>();
        GenerateBlock();
    }
    public void GenerateBlock()
    {
        if (showProcess)
            StartCoroutine(IEGenerate());
        else
            Generate();
    }

    IEnumerator IEGenerate()
    {
        isGenerating = true;
        while (!blockMap.AllCollapse)
        {
            Slot targetSlot;
            if (collapseMode == CollapseMode.LowestEntropy)
                targetSlot = blockMap.GetLowestEntropySlot();
            else if (collapseMode == CollapseMode.HighestEntropy)
                targetSlot = blockMap.GetHighestEntropySlot();
            else
                targetSlot = blockMap.GetSlotRandomSlot();

            CollapseAt(targetSlot);
            PropagateFrom(targetSlot);
            Render();
            yield return new WaitForSeconds(.1f);
        }
        Debug.LogWarning("Generate Finished.");
        isGenerating = false;
    }
    public void Generate()
    {
        while (!blockMap.AllCollapse)
        {
            Slot targetSlot;
            if (collapseMode == CollapseMode.LowestEntropy)
                targetSlot = blockMap.GetLowestEntropySlot();
            else if (collapseMode == CollapseMode.HighestEntropy)
                targetSlot = blockMap.GetHighestEntropySlot();
            else
                targetSlot = blockMap.GetSlotRandomSlot();
            CollapseAt(targetSlot);
            PropagateFrom(targetSlot);
        }
        Render();
    }

    public void PropagateFrom(Slot slot)
    {
        propaQueue.Enqueue(slot);
        while (propaQueue.Count > 0)
        {
            var curSlot = propaQueue.Peek();
            List<Slot> visitedSlots = new List<Slot>();
            visitedSlots.Add(curSlot);
            propaQueue.Dequeue();
            for (int dir = 0; dir < 6; dir++)
            {
                List<Module> removeList = new List<Module>();
                List<string> curPossibleModule = curSlot.GetPossibleNeighborAt(dir);
                Slot neighborSlot = curSlot.GetNeighbor(dir);

                if (visitedSlots.Contains(neighborSlot) || neighborSlot.Collapsed)
                    continue;

                foreach (var possibleModule in neighborSlot.modules)
                {
                    if (!curPossibleModule.Contains(possibleModule.name))
                    {
                        removeList.Add(possibleModule);
                        if (!propaQueue.Contains(neighborSlot))
                            propaQueue.Enqueue(neighborSlot);
                    }
                }
                Constrain(neighborSlot, removeList);
            }
        }
    }

    public void CollapseAt(Slot curSlot)
    {
        for (int dir = 0; dir < 6; dir++)
        {
            Slot neighborSlot = curSlot.GetNeighbor(dir);
            ApplyBorderConstrain(curSlot, neighborSlot, dir);
        }
        curSlot.CollapseRandom();
    }

    private void Constrain(Slot neighborSlot, List<Module> possibleModules)
    {
        foreach (var possibleModule in possibleModules)
        {
            if (neighborSlot.modules.Count > 1)
                neighborSlot.RemoveModule(possibleModule);
            else if (neighborSlot.modules.Count == 1)
            {
                neighborSlot.Collapse(neighborSlot.modules[0]);
                break;
            }
        }
    }

    public void Render()
    {
        for (int i = 0; i < blockMap.slotList.Count; i++)
        {
            if (blockMap.slotList[i].Collapsed)
            {
                if (!renderedSlot.Contains(blockMap.slotList[i]))
                {
                    GameObject prefab = blockMap.slotList[i].outputModule.prefab;
                    Vector3 position = blockMap.slotList[i].position;
                    Vector3 eulerAngel = Orientations.GetRotateAngle(blockMap.slotList[i].outputModule.rotation);
                    Quaternion rotation = Quaternion.Euler(eulerAngel);
                    GameObject.Instantiate(prefab, position, rotation, this.transform);
                    renderedSlot.Add(blockMap.slotList[i]);
                }

            }
        }
    }

    private void ApplyBorderConstrain(Slot curSlot, Slot neighborSlot, int direction)
    {
        if (Slot.CheckBorderSlot(neighborSlot))
        {
            int opositeDir = (direction + 3) % 6;
            if (opositeDir == Orientations.UP)  // Down Border has no Constrain 
                return;

            List<Module> removeList = new List<Module>();
            List<string> possibleModules = neighborSlot.GetPossibleNeighborAt(opositeDir);

            foreach (var module in curSlot.modules)
                if (!possibleModules.Contains(module.name))
                    removeList.Add(module);
            Constrain(curSlot, removeList);
        }
    }

    public void ClearSlot()
    {
        propaQueue.Clear();
        renderedSlot.Clear();
        blockMap.ResetSlot(mapSize);
        int childCount = transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }

}