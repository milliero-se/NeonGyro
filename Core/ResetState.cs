using System.Runtime.CompilerServices;

namespace NeonGyro.Core;

class ResetState {
	// Initial angle of the reset which is in progress
	float resetInitAngle = 0f;
	// value in [0,1) indicating how much of the reset is completed
	// 0 indicates no reset in progress
	float resetProgress = 0f;

	public ResetState() {}

	// Accepts the angle of the current camera Y and a deltaTime.
	// Returns an eased angle to be added to the rotation.
	public float Update(float angle, float deltaTime) 
	{
		if (Mod.ControllerManager == null || Mod.Config == null) return 0f; // mod not initialized

		float resetTime = Mod.Config.ResetTime.Value;

		// Reset is not in progress
		if (resetProgress == 0f) 
		{
			if (Mod.ControllerManager.ResetButtonNewlyPressed)
			{
				// If reset time is too short, early return
				if (deltaTime >= resetTime)
					return -angle;
				// Start a camera reset
				resetInitAngle = angle;
			}
			else
			{
				// Not resetting, early return
				return 0f;
			}
		}

		float t0 = EaseOutCubic(resetProgress);

		// Update resetProgress
		resetProgress += deltaTime / resetTime;
		if (resetProgress > 1f)
			resetProgress = 1f; // clamp

		float t1 = EaseOutCubic(resetProgress);

		// Complete the reset
		if (resetProgress >= 1f)
			resetProgress = 0f;

		return -deltaTime / resetTime * resetInitAngle;
	}

	// Copied from GyroHelpers/GyroHelpers/FlickStick.
	// Probably belongs in MathHelper.cs, but I did not want to make two PRs.
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static float EaseOutCubic(float t)
	{
		t -= 1f;
		return 1 + t * t * t;
	}
}
