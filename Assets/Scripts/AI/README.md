# Underwater enemy AI - minimum setup

## 1. Create the navigation volume

1. Create an empty GameObject named `Waypoint Network`.
2. Add `WaypointNetwork3D` to it.
3. Add child GameObjects throughout the water and put `UnderwaterWaypoint` on each one.
4. Keep neighbouring points within `Maximum Connection Distance`.
5. Put the level geometry that fish must avoid on the `NavigationObstacle` layer.
6. Select `Waypoint Network`. Cyan lines show connections the creatures can use.

Place extra waypoints around corners, above and below the submarine, and at narrow entrances.
Do not put the player, enemies, or sonar on `NavigationObstacle`.

## 2. Set up an enemy

1. Give the creature a Rigidbody and Collider.
2. Add `UnderwaterEnemyAI`.
3. Assign the `Waypoint Network` reference.
4. For the player hunter, assign the player's root Transform to `Player Target`.
5. Choose one of the two `Behaviour` values:
   - `Chases Player And Fears Sonar`
   - `Ignores Player And Attacks Sonar`

The script disables Rigidbody gravity. Freeze Rigidbody rotation if collisions make the model roll.

## 3. Set up the sonar

1. Add `SonarStimulus` to the sonar GameObject.
2. When a sonar pulse or continuous sonar begins, call `SetEmitting(true)`.
3. When it ends, call `SetEmitting(false)`.
4. Connect `On Attacked` to the sonar's damage or disable method when that system exists.

The sonar-hunting creature must have a clear line to an emitting sonar before it notices it. After detection,
it can follow the waypoint path around an obstacle. The player hunter hears an emitting sonar within its fear
distance and chooses a route away from it.

## Fast test

1. Use cubes as obstacles and assign them to `NavigationObstacle`.
2. Make 10-20 waypoints around the cubes, including points at different heights.
3. Use capsules as the two enemies.
4. Enter Play Mode and watch `Current State` in each enemy's Inspector.
5. Enable `Is Emitting` on the sonar during Play Mode to test both reactions.

Yellow gizmos show an enemy's current path. If no yellow path appears, check the cyan network connections,
increase `Maximum Connection Distance`, or add a waypoint around the blocking object.
