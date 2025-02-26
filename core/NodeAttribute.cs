using Godot;
using System.Linq;
using System;
using System.Reflection;

[System.AttributeUsage(System.AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
sealed class NodeAttribute : System.Attribute
{

	private static MethodInfo methodGetNode = typeof(Node2D)
				.GetMethod(nameof(Node2D.GetNode), 1, new Type[] {typeof(NodePath)});

	public static void AssignNodes(Node node) {
		var fields =
			from field in node.GetType().GetFields()
			from attr in field.GetCustomAttributes(true)
			where attr is NodeAttribute
			select (field, (NodeAttribute)attr);
		GD.Print($"{node.GetType().Name} has {fields.Count()} attributes on fields");
		foreach ((var field, var attr) in fields) {
			GD.Print(field.Name, " ", attr.GetType().Name);
		}

		foreach ((var field, var attr) in fields) {
			GD.Print($"{node.GetType().Name} has field {field.Name} to be assigned to node {attr.path}");
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
