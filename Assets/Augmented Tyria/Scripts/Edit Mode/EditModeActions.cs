using System;
using System.Linq;
using UnityEngine;

public partial class EditMode
{
    private Node GetSelectedNode()
    {
        if (this.onDetached)
        {
            if (this.detachedNodeIndex < 0)
                return null;
            else
                return this.Route.DetachedNodes[this.detachedNodeIndex];
        }
        else
        {
            if (this.nodeIndex < 0)
                return null;
            else
                return this.Route.Nodes[this.nodeIndex];
        }
    }

    private void AddVisualNode(Node node, bool detached = false, int at = -1)
    {
        GameObject gameObject = Instantiate(this.NodePrefab, this.RouteDisplay.transform);
        NodeDisplay display = gameObject.GetComponent<NodeDisplay>();
        display.Node = node;

        display.SetMesh(detached);

        if (detached)
        {
            this.detachedNodes.Add(display);
        }
        else
        {
            if (at == -1)
                this.nodes.Add(display);
            else
                this.nodes.Insert(at, display);

            this.UpdatePath();
        }
    }

    private void RemoveVisualNode(int at, bool detached = false)
    {
        if (detached)
        {
            Destroy(this.detachedNodes[at].gameObject);
            this.detachedNodes.RemoveAt(at);
        }
        else
        {
            Destroy(this.nodes[at].gameObject);
            this.nodes.RemoveAt(at);
            this.UpdatePath();
        }
    }

    private void UpdatePath()
    {
        this.RouteDisplay.positionCount = 0;  // TODO: Find out if this is needed.
        Vector3[] positions = this.Route.Nodes.Select(n => n.Position).ToArray();
        this.RouteDisplay.positionCount = positions.Length;
        this.RouteDisplay.SetPositions(positions);
    }

    private void ScrollNodeType(int direction)
    {
        Node node = this.GetSelectedNode();
        if (node == null)
            return;

        Array values = Enum.GetValues(typeof(NodeType));
        int newValue = ((int)node.Type + direction) % values.Length;

        while (newValue < 0)
            newValue += values.Length;
        while (newValue >= values.Length)
            newValue -= values.Length;

        node.Type = (NodeType)newValue;
        this.UpdateSelectedNodeInfo();
    }

    private string GetComment()
    {
        Node node = this.GetSelectedNode();
        if (node == null)
            return null;
        return node.Comment;
    }

    private void SetComment(string to)
    {
        Node node = this.GetSelectedNode();
        if (node == null)
            return;

        node.Comment = to;
        this.UpdateSelectedNodeInfo();
    }

    private string GetData()
    {
        Node node = this.GetSelectedNode();
        if (node == null)
            return null;

        switch (node.Type)
        {
            case NodeType.Waypoint:
                return node.WaypointCode;

            case NodeType.HeartWall:
                return node.HeartWallValue;
        }
        return null;
    }

    private void SetData(string to)
    {
        Node node = this.GetSelectedNode();
        if (node == null)
            return;

        switch (node.Type)
        {
            case NodeType.Waypoint:
                node.SetWaypointCode(this.RouteHolder.MapId, this.RouteHolder.GameDatabase, to);
                break;

            case NodeType.HeartWall:
                node.HeartWallValue = to;
                break;
        }
        this.UpdateSelectedNodeInfo();
    }

    private void UpdateSelectedNodeInfo()
    {
        if (this.onDetached && this.detachedNodeIndex >= 0)
            this.detachedNodes[this.detachedNodeIndex].Node = this.Route.DetachedNodes[this.detachedNodeIndex];
        else if (!this.onDetached && this.nodeIndex >= 0)
            this.nodes[this.nodeIndex].Node = this.Route.Nodes[this.nodeIndex];
    }

    private void ToggleAttachNode()
    {
        Node node = this.GetSelectedNode();
        if (node == null)
            return;

        // TODO: Review the logic here?
        if (this.onDetached && this.nodeIndex >= 0)
        {
            this.Route.DetachedNodes.RemoveAt(this.detachedNodeIndex);
            this.Route.Nodes.Insert(this.nodeIndex + 1, node);
            this.RemoveVisualNode(this.detachedNodeIndex, true);
            this.AddVisualNode(node, false, this.nodeIndex + 1);

            this.detachedNodeIndex = -1;
            this.nodeIndex++;
            this.onDetached = false;

            this.UpdateSelectedNode();
            this.UpdatePath();
        }
        else if (!this.onDetached)
        {
            this.Route.Nodes.RemoveAt(this.nodeIndex);
            this.Route.DetachedNodes.Add(node);
            this.RemoveVisualNode(this.nodeIndex);
            this.AddVisualNode(node, true);

            this.detachedNodeIndex = this.Route.DetachedNodes.Count - 1;
            if (this.nodeIndex > 0 || this.nodes.Count == 0)
                this.nodeIndex--;
            this.onDetached = true;

            this.UpdateSelectedNode();
            this.UpdatePath();
        }
    }

    private void SelectClosestNode()
    {
        // TODO: Find a way to apply Don't Repeat Yourself here.
        int newNodeIndex = -1;
        if (this.nodes.Count != 0)
        {
            float d = -1;
            newNodeIndex = 0;
            foreach (var item in this.nodes.Select((display, i) => new { transform = display.transform, i = i }))
            {
                float cd = (this.Cursor.position - item.transform.position).sqrMagnitude;
                if (d == -1 || d > cd)
                {
                    d = cd;
                    newNodeIndex = item.i;
                }
            }
            this.onDetached = false;
        }

        int newDetachedNodeIndex = -1;
        if (this.detachedNodes.Count != 0)
        {
            float d = -1;
            newDetachedNodeIndex = 0;
            foreach (var item in this.detachedNodes.Select((display, i) => new { transform = display.transform, i = i }))
            {
                float cd = (this.Cursor.position - item.transform.position).sqrMagnitude;
                if (d == -1 || d > cd)
                {
                    d = cd;
                    newDetachedNodeIndex = item.i;
                }
            }
            this.onDetached = true;
        }

        if (newNodeIndex >= 0 && newDetachedNodeIndex >= 0)
        {
            if ((this.Cursor.position - this.nodes[newNodeIndex].transform.position).sqrMagnitude >
                (this.Cursor.position - this.detachedNodes[newDetachedNodeIndex].transform.position).sqrMagnitude)
            {
                this.onDetached = true;
                this.detachedNodeIndex = newDetachedNodeIndex;
            }
            else
            {
                this.onDetached = false;
                this.nodeIndex = newNodeIndex;
            }
        }
        else if (newNodeIndex >= 0)
        {
            this.nodeIndex = newNodeIndex;
            this.onDetached = false;
        }
        else if (newDetachedNodeIndex >= 0)
        {
            this.detachedNodeIndex = newDetachedNodeIndex;
            this.onDetached = true;
        }

        this.UpdateSelectedNode();
    }

    private void AddNode()
    {
        Node newNode = new Node()
        {
            Type = NodeType.Reach,
            Position = this.Cursor.position
        };

        if (onDetached)
        {
            this.Route.DetachedNodes.Add(newNode);
            this.AddVisualNode(newNode, true);
            this.detachedNodeIndex = this.detachedNodes.Count - 1;
        }
        else
        {
            this.nodeIndex++;
            this.Route.Nodes.Insert(this.nodeIndex, newNode);
            this.AddVisualNode(newNode, false, this.nodeIndex);
        }

        this.UpdateSelectedNode();
    }

    private void RemoveNode()
    {
        Node node = this.GetSelectedNode();
        if (node == null)
            return;

        if (onDetached)
        {
            this.Route.DetachedNodes.RemoveAt(this.detachedNodeIndex);
            this.RemoveVisualNode(this.detachedNodeIndex, true);
            this.detachedNodeIndex = -1;
        }
        else
        {
            this.Route.Nodes.RemoveAt(this.nodeIndex);
            this.RemoveVisualNode(this.nodeIndex);
            if (this.nodeIndex != 0 || this.nodes.Count == 0)
                this.nodeIndex--;
        }
        this.UpdateSelectedNode();
    }

    private void MoveSelectedNode()
    {
        Node node = this.GetSelectedNode();
        if (node == null)
            return;

        node.Position = this.Cursor.position;
        this.UpdateSelectedNodeInfo();
        this.UpdatePath();
    }

    private void UpdateSelectedNode()
    {
        this.RouteHolder.NodeIndex = this.nodeIndex;

        this.nodes.ForEach(n => n.Select(false));
        this.detachedNodes.ForEach(n => n.Select(false));

        if (this.onDetached && this.detachedNodeIndex >= 0)
            this.detachedNodes[this.detachedNodeIndex].Select(true);
        else if (!this.onDetached && this.nodeIndex >= 0) 
            this.nodes[this.nodeIndex].Select(true);
    }
}