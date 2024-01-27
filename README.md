# OnDemandShadowMapUpdate
By default, High Definition Render Pipeline (HDRP) updates the realtime shadow maps every frame. This script adds more options to customize the on-demand shadow rendering to optimize the runtime performance. It relies on `RequestShadowMapRendering` and `RequestSubShadowMapRendering` functions which you can read about in the [HDRP documentation](https://docs.unity3d.com/Packages/com.unity.render-pipelines.high-definition@12.0/manual/Shadows-in-HDRP.html?q=requestshadowmaprendering#shadow-update-mode).

Special thanks to [Remy](https://github.com/RemyUnity) for introducing me to this feature.

## Features
* Update shadow map on camera movement.
  * This is especially useful for Directional Lights, since their shadows are view-dependent.
  * By default, the active camera is automatically selected.
* Three shadow map update modes:
  * Entire Shadow Map. Intended for all light types.
  * Selected Cascade. Intended for Directions Lights.
  * Selected Subshadow. Intended for Point Lights.
* Update shadows on a per-second or per-frame basis.

## Setup

### Cloning and importing the project
1. Clone the repository, or download the latest [release](https://github.com/radishface/OnDemandShadowMapUpdate/releases).
2. Import the project using **Unity 2021.3.33f1** or newer.
3. Open the `IndoorsScene.unity` file to explore the example scene.

### Enabling the script
1. Select an existing light in your scene, or add a new one.
2. Under its **Light** component, find the **Shadows** header.
3. Enable the shadows, and set the **Update Mode** to **On Demand**.
4. Attach the `OnDemandShadowMapUpdate` script to the light either by dragging and dropping it, or by using the **Add Component** button.

If done correctly, you should see the UI shown in the screenshot below:

<img width="640" alt="OnDemandShadpwMapUpadte-MainUI" src="https://github.com/radishface/OnDemandShadowMapUpdate/assets/1553981/7c74d734-98d7-4ad0-9ee0-b4286891d687">

*Please note that any changes made to the script will only be visible in Play mode or in standalone builds.*
