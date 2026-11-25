using Godot;
using System;

namespace VoyVivika.OSCRelay.Nodes
{

	[GlobalClass]
	public partial class iTextEdit : TextEdit
	{
		public int slot { get; set; } = -1;

		public override void _Ready()
		{
			ScrollFitContentHeight = true;
			TextChanged += OnTextChanged;
		}

		public void OnTextChanged()
		{
			EmitSignal(SignalName.TextEditSignal, slot, Text);
		}

		[Signal]
		public delegate void TextEditSignalEventHandler(int slot, string text);

	}
}
