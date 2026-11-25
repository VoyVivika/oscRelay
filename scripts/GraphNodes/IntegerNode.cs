using System;
using System.Collections.Generic;
using VoyVivika.OSCRelay.NodeHelpers;
using Godot;
using Godot.Collections;

namespace VoyVivika.OSCRelay.Nodes;

public partial class IntegerNode : BaseNode
{
    public override string TypeName { get; } = "Integer";

    public override List<NodeInput> Inputs { get; set; } =
    [
        new NodeInput("Int", TypeHelpers.TYPES.i, noConnect: true, defaultValue: 0, hideTextWhenEditVisible: true)
    ];

    public override List<NodeOutput> Outputs { get; set; } =
    [
        new NodeOutput("Output", TypeHelpers.TYPES.i, false)
    ];
}