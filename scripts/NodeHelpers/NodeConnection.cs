using Godot;

namespace VoyVivika.OSCRelay.NodeHelpers;

public class NodeConnection
{
    public NodeConnection(StringName fromNode, long fromPort, StringName toNode, long toPort)
    {
        FromNode = fromNode;
        FromPort = fromPort;
        ToNode = toNode;
        ToPort = toPort;
    }
    public StringName FromNode;
    public long FromPort;
    public StringName ToNode;
    public long ToPort;
}