using Godot;
using System;
using System.Collections.Generic;
using LucHeart.CoreOSC;
using VoyVivika.OSCRelay.Nodes;

namespace VoyVivika.OSCRelay;

public partial class CreateNodeWindowManager : Window
{
	private Dictionary<TreeItem, Type> _types;
	
	[Export]
	public Tree tree;
	[Export]
	public Button AddButton;
	[Export]
	public Button CancelButton;
	[Export]
	public GraphHandler handler;
	
	
	public override void _Ready()
	{
		//TreeItem root = tree.CreateItem(index: -1);
		
		_types = new Dictionary<TreeItem, Type>();
		
		TreeItem root = tree.CreateItem();
		root.DisableFolding = true;
		
		// OSC Nodes
		{
			TreeItem osc_root = tree.CreateItem(root);
			osc_root.Collapsed = true;
			osc_root.SetText(0, "OSC");
			osc_root.SetSelectable(0, false);

			TreeItem osc_listener = tree.CreateItem(osc_root);
			osc_listener.SetText(0, "OSC Listener");
			_types.Add(osc_listener, typeof(OscListenerNode));

			TreeItem osc_sender = tree.CreateItem(osc_root);
			osc_sender.SetText(0, "OSC Sender");
			_types.Add(osc_sender, typeof(OscSenderNode));
		}
		
		// Types
		{
			TreeItem var_root = tree.CreateItem(root);
			var_root.Collapsed = true;
			var_root.SetText(0, "Types & Their Functions");
			var_root.SetSelectable(0, false);
			
			// Integer
			{
				TreeItem var_int_node = tree.CreateItem(var_root);
				var_int_node.Collapsed = true;
				var_int_node.SetText(0, "Integer");
				_types.Add(var_int_node, typeof(IntegerNode));
			}
		}

		CloseRequested += OnCloseRequested;
		AddButton.Pressed += OnAddButtonPressed;
		AddButton.Disabled = true;
		CancelButton.Pressed += OnCloseRequested;
		tree.ItemSelected += OnTreeSelectionChanged;
	}

	private void OnCloseRequested()
	{
		Hide();
	}

	private void OnTreeSelectionChanged()
	{
		Type type;
		AddButton.Disabled = !_types.TryGetValue(tree.GetSelected(), out type);
	}

	private void OnAddButtonPressed()
	{
		var selection = tree.GetSelected();
		if (selection == null) return;
		Type type;
		if (_types.TryGetValue(selection, out type))
		{
			BaseNode node;
			node = (BaseNode)Activator.CreateInstance(type);
			handler.AddChild(node);
			node.PositionOffset = handler.GraphClickedPosition + handler.ScrollOffset;
			Hide();
		}
	}
	
	

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
