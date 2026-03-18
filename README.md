# AimDeltaFix

Fixes mouse sensitivity feeling slower at higher framerates in R.E.P.O.

## What's the bug?

The camera smoothing in `CameraAim.Update()` uses `Quaternion.Lerp(current, target, num3 * Time.deltaTime)` in a feedback loop. At 60fps that lerp factor is ~0.83, so the camera snaps almost instantly to where you're aiming. At higher framerates like 700fps, it drops to ~0.07 per frame — the camera barely moves each frame and it feels like your sensitivity got turned way down.

## How this fixes it

When camera smoothing is off, the lerp is skipped entirely — the camera just snaps to your aim target regardless of FPS. When smoothing is on, the lerp factor is replaced with `1 - Exp(-num3 * Time.deltaTime)` which gives proper frame-rate independent exponential decay.

## Dev fix

The issue is on line 213 of `CameraAim.Update()`. Easiest fix for smoothing off is to just assign the target directly (`base.transform.localRotation = quaternion`). For smoothed mode, swap `num3 * Time.deltaTime` with `1f - Mathf.Exp(-num3 * Time.deltaTime)`.

## Credit

Bug originally spotted by MiahTRT.
