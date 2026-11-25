using Godot;
using System;

namespace VoyVivika.OSCRelay.Nodes
{

    [GlobalClass]
    public partial class iLineEdit : LineEdit
    {
        public int slot { get; set; } = -1;

        public override void _Ready()
        {
            TextChanged += OnTextChanged;
        }

        public void OnTextChanged(string txt)
        {
            EmitSignal(SignalName.LineEditSignal, slot, txt);
        }

        [Signal]
        public delegate void LineEditSignalEventHandler(int slot, string text);

    }
}