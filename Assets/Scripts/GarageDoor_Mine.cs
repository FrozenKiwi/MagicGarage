using Apt.Unity.Projection;
using UnityEngine;

public class GarageDoor_Mine : MonoBehaviour
{

    public GameObject[] Panels;
    public float GutterSize = 0.1f;

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitPanelXforms(ProjectionPlane projPlane)
    {
        var numPanels = Panels.Length;
        var projSize = projPlane.Size;
        // Resize panels to fit, and offset by gutter
        var panelHeight = (projSize.y - (GutterSize * (numPanels - 1))) / numPanels;
        var panelWidth = projSize.x;
        for (var i = 0; i < numPanels; i++)
        {
            var panel = Panels[i];
            var posY = -(projSize.y / 2) + (panelHeight / 2) + (i * (panelHeight + GutterSize));
            panel.transform.position = new Vector3(0, posY, 0);
            panel.transform.localScale = new Vector3(panelWidth, panelHeight, 0.1f);
        }
    }
}
