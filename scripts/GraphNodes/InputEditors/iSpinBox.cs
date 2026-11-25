using Godot;
using System;
using System.Numerics;


namespace VoyVivika.OSCRelay.Nodes
{
    [GlobalClass]
    public partial class iSpinBox : SpinBox
    {
        //public BaseNode ParentNode = null;
        public int slot { get; set; } = -1;

        public override void _ValueChanged(double newValue)
        {
            EmitSignal(SignalName.SpinBoxSignal, slot, newValue);
        }
        
        [Signal]
        public delegate void SpinBoxSignalEventHandler(int slot, double value);


    }


}