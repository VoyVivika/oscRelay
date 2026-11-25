using Godot;
using System.Collections.Generic;
using VoyVivika.OSCRelay.NodeHelpers;
//using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Godot.Collections;
using LucHeart.CoreOSC;
using Array = Godot.Collections.Array;

namespace VoyVivika.OSCRelay.Nodes
{
	[GlobalClass]
	public partial class OscListenerNode : BaseNode
	{
		public override string TypeName { get; } = "OSC Listener";
		public override List<NodeInput> Inputs { get; set; } =
		[
			new NodeInput("Port", TypeHelpers.TYPES.i, defaultValue: 4646)
		];

		public override List<NodeOutput> Outputs { get; set; } =
		[
			new NodeOutput(">", TypeHelpers.TYPES.Flow),
			new NodeOutput("Address", TypeHelpers.TYPES.s),
			new NodeOutput("Value", TypeHelpers.TYPES.Value)
		];
		
		SynchronizationContext _mainContext = SynchronizationContext.Current;
		// Inputs
		//public int Port = 4646;
		
		// Outputs: Osc Address, Osc Value

		
		public async Task StartListening(IPEndPoint endpoint, CancellationToken ct)
		{
			using OscListener listener = new OscListener(endpoint);

			while (!ct.IsCancellationRequested)
			{
				OscBundle bundle = await listener.ReceiveBundleAsync();
				_mainContext.Post(_ =>
				{
					ProcessBundle(bundle);

				}, null);
			}
		}
		

		public void ProcessBundle(OscBundle bundle)
		{
			foreach (var message in bundle.Messages)
			{
				EmitSignal(SignalName.String, message.Address);
				EmitSignal(SignalName.OscMessage, OSCHelpers.ConvertToGodot(message.Arguments));
			}
		}

		[Signal]
		public delegate void StringEventHandler(string value);
		
		[Signal]
		public delegate void OscMessageEventHandler(Array<Variant> message);
	}
}
