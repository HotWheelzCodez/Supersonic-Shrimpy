using Godot;
using System.Linq;
using System;
using System.Reflection;

[System.AttributeUsage(System.AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
sealed class NodeAttribute : System.Attribute
{

	private static MethodInfo methodGetNode = typeof(Node2D)
				.GetMethod(nameof(Node2D.GetNode), 1, new Type[] {typeof(NodePath)});

	public static void StartTreeListener(Node root) {
		root.ChildEnteredTree += _OnTreeChildEnter;
	}

	private static void _OnTreeChildEnter(Node node) {
		NodeAttribute.AssignNodes(node);
		node.ChildEnteredTree += _OnTreeChildEnter;
	}

	private static void AssignNodes(Node node) {
		var fields =
			from field in node.GetType().GetFields()
			from attr in field.GetCustomAttributes(true)
			where attr is NodeAttribute
			select (field, (NodeAttribute)attr);

		foreach ((var field, var attr) in fields) {
			var method = methodGetNode.MakeGenericMethod(field.FieldType);
			var v = method.Invoke(node, new object[]{attr.path});
			field.SetValue(node, v);
		}
	}

	private readonly NodePath path;

	// This is a positional argument
	public NodeAttribute(NodePath path) {
		this.path = path;
	}
	public NodeAttribute(string path) {
		this.path = path;
	}
}
