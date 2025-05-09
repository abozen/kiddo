/*
Collectable Prefab Setup Guide

To create a collectable prefab:

1. Create a new GameObject with a simple shape (like a sphere or coin)
2. Add a Collider component and mark it as "Is Trigger"
3. Add the Collectable script to the GameObject
4. Set up the Tag as "Collectable" in Unity's tag system
5. Configure the visual properties in the Collectable script
6. Add a Rigidbody component with "Is Kinematic" checked
7. Adjust the size as needed

Then, drag this GameObject to your Project view to create a prefab.
Assign this prefab to the "collectablePrefab" field in the RoadManager component.

The CollectableSpawner will handle creating instances of this prefab on each road tile,
with varying point values (1, 5, and 10) according to their spawn probabilities.
*/ 