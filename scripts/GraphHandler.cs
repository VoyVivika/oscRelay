using Godot;
using System;
using System.Collections.Generic;
using Godot.Collections;
using VoyVivika.OSCRelay.NodeHelpers;
using VoyVivika.OSCRelay.Nodes;

namespace VoyVivika.OSCRelay
{
	public partial class GraphHandler : GraphEdit
	{
		[Export]
		public Window CreateNodeWindow;
		
		public Vector2 GraphClickedPosition;
		
		private PopupMenu _popupMenu;
		
		public override void _Ready()
		{
			// for some reason this keeps asking for longs, I cannot fathom why.
			ConnectionRequest += OnConnectionRequest;
			DisconnectionRequest += OnDisconnectionRequest;
			
			int[] vals = (int[])Enum.GetValuesAsUnderlyingType<TypeHelpers.TYPES>();

			for (int i = 0; i < vals.Length; i++)
			{
				TypeNames[vals[i]] = TypeHelpers.GetTypeName((TypeHelpers.TYPES)vals[i]);
			}

			GD.Print(TypeNames);
			
			SetupPopupMenu();
		}

		private void SetupPopupMenu()
		{
			_popupMenu = new PopupMenu();
			AddChild( _popupMenu);
			
			_popupMenu.AddItem("Add Node", 0);
			_popupMenu.AddSeparator();
			_popupMenu.AddItem("Delete", 1);

			PopupRequest += OnPopupRequest;
			_popupMenu.IdPressed += OnMenuItemPressed;
		}

		private void OnPopupRequest(Vector2 pos)
		{
			GraphClickedPosition = pos;
			Vector2 screenPos = GetGlobalTransformWithCanvas() * pos;
			
			_popupMenu.Position = (Vector2I)screenPos;
			_popupMenu.Popup();
		}

		public List<NodeConnection> GetConnections(StringName name)
		{
			var result = new List<NodeConnection>();

			foreach (Dictionary connection in GetConnectionList())
			{
				StringName from = (StringName)connection["from_node"];
				StringName to = (StringName)connection["to_node"];

				if (from == name || to == name)
				{
					{
						result.Add(new NodeConnection(from, (int)connection["from_port"], to, (int)connection["to_port"]));
					}
				}
			}

			return result;
		}

		private void OnMenuItemPressed(long id)
		{
			switch (id)
			{
				case 0: // Add Node
					CreateNodeWindow.Show();
					break;
				case 1: // Delete Node
					DeleteSelectedNodes();
					break;
			}
		}

		private void DeleteSelectedNodes()
		{
			GetSelectedGraphNodes(true);
		}
		
		private Array<BaseNode> GetSelectedGraphNodes(bool delete = false)
		{
			Array<BaseNode> selectedNodes = new();

			foreach (Node child in GetChildren())
			{
				if (child is BaseNode baseNode && baseNode.Selected)
				{
					if (!delete)
						selectedNodes.Add(baseNode);
					else
						baseNode.DeleteNode();
				}
			}

			return selectedNodes;
		}
		
		// if two nodes are connected
		public bool IsNodeConnected(
			StringName fromNode,
			int fromPort,
			StringName toNode,
			int toPort)
		{
			foreach (Godot.Collections.Dictionary connection in GetConnectionList())
			{
				if ((StringName)connection["from_node"] == fromNode &&
				    (int)connection["from_port"] == fromPort &&
				    (StringName)connection["to_node"] == toNode &&
				    (int)connection["to_port"] == toPort)
				{
					return true;
				}
			}

			return false;
		}

		public NodeConnection GetInputConnection(StringName toNode, int toPort)
		{
			foreach (Godot.Collections.Dictionary connection in GetConnectionList())
			{
				if ((StringName)connection["to_node"] == toNode &&
				    (int)connection["to_port"] == toPort)
				{
					StringName fromNode = (StringName)connection["from_node"];
					int fromPort = (int)connection["from_port"];
					return new NodeConnection(fromNode, fromPort, toNode, toPort);
				}
			}
			
			return null;
		}
		
		
		public List<NodeConnection> GetOutputConnections(StringName fromNode, int fromPort)
		{
			List<NodeConnection> connections = new List<NodeConnection>();
			
			foreach (Godot.Collections.Dictionary connection in GetConnectionList())
			{
				if ((StringName)connection["from_node"] == fromNode &&
				    (int)connection["from_port"] == fromPort)
				{
					StringName toNode = (StringName)connection["to_node"];
					int toPort = (int)connection["to_port"];
					connections.Add(new NodeConnection(fromNode, fromPort, toNode, toPort));
					GD.Print("Connection Found: " + fromNode + " at " + fromPort + " -> " + toNode + " at " + toPort);
				}
			}
			
			return connections;
		}

		public void RemoveConnections(NodeConnection[] connections)
		{
			foreach (NodeConnection connection in connections)
			{
				DisconnectNode(connection.FromNode, (int)connection.FromPort, connection.ToNode, (int)connection.ToPort);
			}
		}
		
		// if this node has an input connection
		public bool HasInputConnection(StringName nodeName, int portIndex)
		{
			foreach (Godot.Collections.Dictionary connection in GetConnectionList())
			{
				if ((StringName)connection["to_node"] == nodeName &&
				    (int)connection["to_port"] == portIndex)
				{
					return true;
				}
			}

			return false;
		}
		
		public bool HasOutputConnection(StringName nodeName, int portIndex)
		{
			foreach (Godot.Collections.Dictionary connection in GetConnectionList())
			{
				if ((StringName)connection["from_node"] == nodeName &&
				    (int)connection["from_port"] == portIndex)
				{
					return true;
				}
			}

			return false;
		}

		// idk why these have to be longs but it'll complain if I don't do this. These functions use ints
		private void OnConnectionRequest(StringName fromNode, long fromPort, StringName toNode, long toPort)
		{
			BaseNode _fromNode = GetNode<BaseNode>(new NodePath(fromNode));
			BaseNode _toNode = GetNode<BaseNode>(new NodePath(toNode));
			int _fromPort = (int)fromPort;
			int _toPort = (int)toPort;

			if (IsNodeConnected(fromNode, _fromPort, toNode, _toPort))
			{
				GD.Print($"Connection Already Exists: {_fromNode} to {_toNode}");
				return;
			}

			int fNodeType = _fromNode.GetOutputPortType(_fromPort);
			int tNodeType = _toNode.GetInputPortType(_toPort);
			bool equalType = fNodeType == tNodeType;
			if (equalType)
			{
				TypeHelpers.TYPES connectionType = (TypeHelpers.TYPES)fNodeType;
				bool isFlow = (connectionType == TypeHelpers.TYPES.Flow);
				bool isFlowAndIsConnected = isFlow && HasInputConnection(toNode, _toPort);
				if (isFlowAndIsConnected)
				{
					//GD.Print($"Flow Connection Already Exists: {_fromNode} to {_toNode}");
					var connection = GetInputConnection(toNode, _toPort);
					DisconnectNode(connection.FromNode, (int)connection.FromPort, connection.ToNode, (int)connection.ToPort);
					//return;
				}
				if (isFlow)
				{
					RemoveConnections(GetOutputConnections(fromNode, _fromPort).ToArray());
				}
				ConnectNode(fromNode, _fromPort, toNode, _toPort);
				_toNode.UpdateVisibilityOfEditBoxes();
				GD.Print($"Connection Established: {_fromNode} to {_toNode}");
			}
		}
		
		private void OnDisconnectionRequest(
			StringName fromNode,
			long fromPort,
			StringName toNode,
			long toPort)
		{
			BaseNode _fromNode = GetNode<BaseNode>(new NodePath(fromNode));
			BaseNode _toNode = GetNode<BaseNode>(new NodePath(toNode));
			
			DisconnectNode(fromNode, (int)fromPort, toNode, (int)toPort);
			
			_toNode.UpdateVisibilityOfEditBoxes();
		}


		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
		}
	}
}
