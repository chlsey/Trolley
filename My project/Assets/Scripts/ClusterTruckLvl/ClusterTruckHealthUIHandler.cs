using UnityEngine;
using UnityEngine.UIElements;

public class ClusterTruckHealthUIHandler : MonoBehaviour
{
    static int heartAmount = 4;
    public GameObject healthUI;
    public GameObject player;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStats()
    {
        heartAmount = 4;
    }
    void Start()
    {
        Health healthScript = player.GetComponent<Health>();

        heartAmount--;
        UpdateHearts();
      
    }
    private void UpdateHearts()
    {
        Debug.Log("Health Down");
        var uiDoc = healthUI.GetComponent<UIDocument>();
        var root = uiDoc.rootVisualElement;
        VisualElement heartHolder = root.Q("Hearts_Container");
        for (int i = 0; i < heartHolder.childCount; i++)
        {
            VisualElement heart = heartHolder[i];
            bool visible = i < heartAmount;
            heart.visible = visible;
        }
    }
}
