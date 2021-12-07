# UniversalUnityDemosaics
A collection of BepInEx plugins for games made in Unity3D engine that are used to remove mosaic censoring. They are mainly targetted at Japanese 3D eroges (hentai games). They can work with some 2D games as well. Most of the plugins require the latest version of BepInEx 5.

You can see some examples of games demosaiced by these plugins [here](https://www.patreon.com/ManlyMarco?filters[tag]=Modpack).

#### DumbRendererDemosaic
Basic "bread and butter" demosaic. It works with the largest amount of games and should be the first one to try. It works by disabling discrete mosaic objects and removing their textures.
#### DumbRendererDemosaicIl2Cpp
A version of DumbRendererDemosaic made for IL2CPP games. Needs latest version of BepInEx 6 for IL2CPP.
#### CombinedMeshDemosaic
A smarter version of DumbRendererDemosaic that works with games that use combined mesh renderers available in newer Unity versions (DumbRendererDemosaic doesn't work at all on those). Scans individual materials on all renderers for materials that could be mozaics and changes their shaders to be invisible. Useful when meshes are combined and not represented by transforms. Use together with DumbRendererDemosaic if there are any dedicated mosaic renderers.
#### MaterialReplaceDemosaic
A smarter version of DumbRendererDemosaic, uncensors some Live2D games where privates completely disappear with other demosaic plugins.
#### DumbTypeDemosaic
A variation of the DumbRendererDemosaic that check game code for possible mosaic methods and disables them. It rarely works but is required by some games.
#### CubismRendererDisableDemosaic
Demosaic targetted at games using the CubismModel framework. Usually DumbRendererDemosaic is good enough but some games might work better with this one.

## How to use?
1. Get the latest version of BepInEx 5 and install it to your game.
2. Copy one of the demosaic .dll files to the `BepInEx\plugins` folder.
3. See if the plugin load and have effect on the game. If not, remove it and try a different one.
