using Godot;
using System;
using System.Collections.Generic;
using VoyVivika.OSCRelay;
using VoyVivika.OSCRelay.NodeHelpers;
using VoyVivika.OSCRelay.Nodes;

public partial class OscSenderNode : BaseNode
{
	public override string TypeName { get; } = "OSC Sender";

	public override List<NodeInput> Inputs { get; set; } =
	[
		new NodeInput(">", TypeHelpers.TYPES.Flow),
		new NodeInput("Address", TypeHelpers.TYPES.s, placeholderTxt: "Address", oneLine: true, hideTextWhenEditVisible: true),
		new NodeInput("Port", TypeHelpers.TYPES.i, defaultValue: 4848),
		new NodeInput("Value", TypeHelpers.TYPES.Value)
	];
}
