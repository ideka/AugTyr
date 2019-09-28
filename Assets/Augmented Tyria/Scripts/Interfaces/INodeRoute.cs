using UnityEngine;

public interface INodeRoute
{
    NodeDisplay GetNodePrefab();
    LineRenderer GetRouteDisplay();
    UserConfig UserConfig { get; }
}

public static class INodeRouteExtensions
{
    public static NodeDisplay NewNodeDisplay(this INodeRoute nr, bool detached, Node node)
    {
        NodeDisplay display = Object.Instantiate(nr.GetNodePrefab().gameObject, nr.GetRouteDisplay().transform).GetComponent<NodeDisplay>();
        display.SetUp(nr.UserConfig.NodeSize, detached, node);
        return display;
    }
}