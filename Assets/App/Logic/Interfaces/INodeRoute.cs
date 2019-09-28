using UnityEngine;

public interface INodeRoute
{
    NodeDisplay GetNodePrefab();
    LineRenderer GetRouteDisplay();
    UserConfig UserConfig { get; }
}

public static class INodeRouteExtensions
{
    public static NodeDisplay NewNodeDisplay(this INodeRoute nodeRoute, bool detached, Node node)
    {
        NodeDisplay display = nodeRoute.GetRouteDisplay().transform.Instantiate(nodeRoute.GetNodePrefab());
        display.SetUp(nodeRoute.UserConfig.NodeSize, detached, node);
        return display;
    }
}