using System;
using Godot;

namespace VoyVivika.OSCRelay.NodeHelpers;

public class NodeInput
{
    public NodeInput(
        string name, 
        TypeHelpers.TYPES type, 
        bool noConnect = false, 
        bool noEdit = false, 
        Variant defaultValue = default, 
        string placeholderTxt = "", 
        bool hideTextWhenEditVisible = false, 
        bool oneLine = false)
    {
        Name = name;
        Type = type;
        DefaultValue = defaultValue;
        NoConnect = noConnect;
        NoEdit = noEdit;
        Value = defaultValue;
        InputBox = null;
        PlaceholderTxt = placeholderTxt;
        HideTextWhenEditVisible = hideTextWhenEditVisible;
        OneLine = oneLine;
    }
    
    public string Name { get; }
    public TypeHelpers.TYPES Type { get; }
    public Variant DefaultValue { get; }
    public bool NoConnect { get; }
    public bool NoEdit { get; }
    public Variant Value { get; set; }
    public Node InputBox { get; set; }
    public int Port { get; set; }
    public string PlaceholderTxt { get; set; }
    public bool HideTextWhenEditVisible { get; set; }
    public bool OneLine  { get; set; }
    public Label label { get; set; }
}

public class NodeOutput
{
    public NodeOutput(string name, TypeHelpers.TYPES type, bool noConnect = false)
    {
        Name = name;
        Type = type;
        NoConnect = noConnect;
    }
    
    public string Name { get; }
    public TypeHelpers.TYPES Type { get; }
    public bool NoConnect { get; }
    public int Port { get; set; }
    public Label label { get; set; }
}