using Godot;
using System;
using System.Collections.Generic;
using VoyVivika.OSCRelay.NodeHelpers;
using Godot.Collections;
using Array = Godot.Collections.Array;
using GType = Godot.Variant.Type;

namespace VoyVivika.OSCRelay.Nodes
{

    public abstract partial class BaseNode : GraphNode
    {
        public long savedId = -1;
        public bool initalized { get; private set; } = false;
        public virtual string TypeName { get; } = "Base Relay Graph Node";
        public virtual List<NodeInput> Inputs { get; set; } = [];
        public virtual List<NodeOutput> Outputs { get; set; } = [];
        //public virtual Array<Variant> InputValues { get; set; }
        public GraphHandler ParentNode  { get; set; }
        
        public override void _Ready()
        {
            Resizable = true;
            Title = TypeName;
            CreateSlots();
            ParentNode = GetParent() as GraphHandler;
            UpdateVisibilityOfEditBoxes();
            initalized = true;
        }
        
        public virtual void CreateSlots()
        {
            int amount = Math.Max(Inputs.Count, Outputs.Count);
            if (amount == 0) return;

            for (int i = 0; i < amount; i++)
            {
                HBoxContainer container = new HBoxContainer();
                AddChild(container);
				
                bool hasInput = i < Inputs.Count;
                bool displayInput = hasInput; // for enabling the input port
                bool hasOutput = i < Outputs.Count;

                int inputType = -1;
                int outputType = -1;
				
                Color inputColor = Colors.Black;
                Color outputColor = Colors.Black;

                //InputValues = new Array<Variant>();
                //InputValues.Resize(Inputs.Count);
                //InputValues.Fill(0);

                //InputBoxes = new Array<Node>();
                //InputBoxes.Resize(Inputs.Count);
                //InputBoxes.Fill(null);
				
                if (hasInput)
                {
                    // INPUT LABEL
                    Label label = new Label();
                    container.AddChild(label);
                    label.CustomMinimumSize = new Vector2(0f, 32f);
                    label.VerticalAlignment =  VerticalAlignment.Center;
                    label.Text = (string)Inputs[i].Name;
                    inputType = (int)Inputs[i].Type;
                    inputColor = TypeHelpers.GetColor((TypeHelpers.TYPES)inputType);

                    Inputs[i].Port = GetInputPortIndexFromSlot(i);
                    
                    bool hasDefault = Inputs[i].DefaultValue.VariantType != Variant.Type.Nil;
                    if (hasDefault)
                        Inputs[i].Value = Inputs[i].DefaultValue;
                    
                    if (!Inputs[i].NoEdit)
                    {
                        Node inputControl = AddInputControl((TypeHelpers.TYPES)inputType, container, i, Inputs[i].Value);
                        if (inputControl != null)
                            Inputs[i].InputBox = inputControl;
                    }
                    
                    Inputs[i].label = label;
                    displayInput = !Inputs[i].NoConnect;
                }

                if (hasOutput)
                {
                    // OUTPUT LABEL
                    Label label = new Label();
                    container.AddChild(label);
                    label.SizeFlagsHorizontal = SizeFlags.ExpandFill;
                    label.HorizontalAlignment = HorizontalAlignment.Right;
                    label.CustomMinimumSize = new Vector2(0f, 32f);
                    label.VerticalAlignment = VerticalAlignment.Center;
                    label.Text = (string)Outputs[i].Name;
                    outputType = (int)Outputs[i].Type;
                    outputColor = TypeHelpers.GetColor((TypeHelpers.TYPES)outputType);
                    
                    Outputs[i].label = label;
                    Outputs[i].Port = GetOutputPortIndexFromSlot(i);
                }

                if (hasOutput || hasInput)
                {
                    SetSlot(i, displayInput, inputType, inputColor, hasOutput, outputType, outputColor);
                }

            }
        }

        public virtual void UpdateInput(int slot, Variant value)
        {
            Inputs[slot].Value = value;
            GD.Print("updated slot " + slot + " with value: " + value);
        }

        public virtual void UpdateDoubleInput(int slot, double value)
        {
            UpdateInput(slot, (Variant)value);
        }

        public virtual void UpdateStringInput(int slot, string value)
        {
            UpdateInput(slot, value);
        }

        public virtual Node AddInputControl(TypeHelpers.TYPES type, Container container, int slot, Variant defvalue)
        {
            GD.Print("adding control of type " + type.ToString() + " in slot " + slot);
            bool defNull = (defvalue.VariantType == Variant.Type.Nil);

            bool typeIsFloat = type == TypeHelpers.TYPES.f || type == TypeHelpers.TYPES.d;
            bool typeIsInt = type == TypeHelpers.TYPES.i || type == TypeHelpers.TYPES.h;
            
            // Number Edit
            if ( typeIsInt || typeIsFloat )
            {
                iSpinBox input = new iSpinBox();
                input.slot = slot;
                input.AllowGreater = true;
                input.AllowLesser = true;
                input.SizeFlagsHorizontal = SizeFlags.ExpandFill;
                input.CustomMinimumSize = new Vector2(0f, 32f);
                container.AddChild(input);
                input.SpinBoxSignal += UpdateDoubleInput;
                if (!defNull)
                {
                    input.Value = (double)defvalue;
                }
                
                if (typeIsFloat)
                    input.Step = 0.0f;
                else
                    input.Step = 1.0f;
                return input;
            }
            else if (type == TypeHelpers.TYPES.s)
            {
                
                if (Inputs[slot].OneLine)
                {
                    iLineEdit input = new iLineEdit();
                    input.slot = slot;
                    input.SizeFlagsHorizontal = SizeFlags.ExpandFill;
                    input.CustomMinimumSize = new Vector2(32f, 32f);
                    container.AddChild(input);
                    input.LineEditSignal += UpdateStringInput;
                    input.PlaceholderText = Inputs[slot].PlaceholderTxt;
                    if (!defNull)
                    {
                        input.Text = (string)defvalue;
                    }

                    return input;
                }
                else
                {
                    iTextEdit input = new iTextEdit();
                    input.slot = slot;
                    input.SizeFlagsHorizontal = SizeFlags.ExpandFill;
                    input.CustomMinimumSize = new Vector2(256f, 32f);
                    container.AddChild(input);
                    input.TextEditSignal += UpdateStringInput;
                    input.PlaceholderText = Inputs[slot].PlaceholderTxt;
                    if (!defNull)
                    {
                        input.Text = (string)defvalue;
                    }

                    return input;
                }
            }
            
            return null;
        }

        public virtual void UpdateVisibilityOfEditBoxes()
        {
            for (int i = 0; i < Inputs.Count; i++)
            {
                if (Inputs[i].InputBox == null) continue;
                
                Type type = Inputs[i].InputBox.GetType();
                
                if (!typeof(CanvasItem).IsAssignableFrom(type)) continue;
                
                // check if connection exists
                bool connection = ParentNode.HasInputConnection(this.Name, GetInputPortIndexFromSlot(i));
                CanvasItem inputBox = (CanvasItem)Inputs[i].InputBox;
                
                inputBox.Visible = !connection;

                if (Inputs[i].HideTextWhenEditVisible)
                    if (Inputs[i].label != null)
                    {
                        Inputs[i].label.Visible = connection;
                    }
            }
        }
        
        public int GetInputPortIndexFromSlot(int slotIndex)
        {
            for (int i = 0; i < GetInputPortCount(); i++)
            {
                if (GetInputPortSlot(i) == slotIndex)
                    return i;
            }

            return -1;
        }

        public int GetOutputPortIndexFromSlot(int slotIndex)
        {
            for (int i = 0; i < GetOutputPortCount(); i++)
            {
                if (GetOutputPortSlot(i) == slotIndex)
                    return i;
            }

            return -1;
        }

        public void DeleteNode()
        {
            ParentNode.RemoveConnections(ParentNode.GetConnections(Name).ToArray());
            QueueFree();
        }


    }

}
