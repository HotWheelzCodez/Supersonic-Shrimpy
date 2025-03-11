using Godot;

public partial class HealthIndicator : Control {
	public AnimationNodeStateMachinePlayback anim;

	public override void _Ready() {
		anim = (AnimationNodeStateMachinePlayback)GetNode<AnimationTree>("AnimationTree").Get("parameters/playback");
	}

	public void SetState(string state) {
		if (anim.GetCurrentNode() != state) {
			anim.Travel(state);
		}
	}
}
