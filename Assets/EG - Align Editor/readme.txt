Align Editor by Equilibre Games
http://www.equilibregames.com

Twitter: @equilibreGames

Current version is v2.2 - 2013-11-04

_________
What does it do ?

This editor window allows you to easily align game objects on your scene :
- align game objects on all axis or the one you chose
- align on the left or the right (use renderer extents or collider extents)
- set scales from one object to the other selected ones
- set rotations from one object to the other selected ones
- distribute objects in equals distances (for position, rotation and scale)
- duplicate objects in grid from prefab or selected object (on a plane or in 3D)
- make objects fall on the next collider beneath them (respect the collider angle if you want to)
- center objects on screen (last used camera on the scene)
- make objects face the camera (last used camera on the scene)
- switch values for every transform in the selection with the next one

Everything can be made from the editor windows and at runtime through scripts. See Example scenes.

_________
Installation

1. Import the unity package from the asset store or my dev blog into your project
2. Open the windows from the "Window/Equilibre/Align Editor" menu : Classic (alignment tools), Grid (grid distribution and duplication), Buttons only, Camera/Screen and settings popup

_________
Update instructions

IMPORTANT : please remove the previous "EG - Align Editor" folder when updating as there's some files that has been moved

_________
Usage

Classic window :
1. Select at least two objects
2. choose the axis (All, X, Y, Z) or let it decide from the scene view (when using predefined orientations on the axis gizmo)
3. choose the axis on which transforms are sorted, or let it pick the same axis as the align axis
4. Click on the icon : align from the minimum values, align on the center, align from the maximum value, distribute objects, fall on terrain, switch values

Grid window :
1. Select one or more objects (your choice, if you want to duplicate or to align)
2. Fill in the grid size, for each axis you use
3. Hit the grid button ! It will align the selected objects or duplicate the objects while keeping prefabs linked if any

Camera :
1. Select one or more objects
2. Hit one of the two buttons
3. You're done !

Buttons only window :
- all the buttons of other windows without parameters : use little space for equal features

Settings :
- Direct access to the website for documented APIs
- Select the component you want to use for boundaries (collider or renderer)
- Enable/disable the use of the collider angle to rotate falling objects
- Choose the skin you want to use (two sets of icons)
- Select the display : text only, image only, image left, image above

_________
Versions History

v2.2 - 2013-11-04
- Fix : FallOnTerrain API was broken for scale different than (1,1,1)
- Fix : auto align axis is not correct when only the "only buttons" window is displayed
- New : sample scene included : Example 1 - Fall On Terrain (more will come)
- Change : removed the workaround for CapsuleCollider Bounds that has been solved in unity
- Change : toggles are now mini buttons in classic and grid windows

v2.1 - 2012-09-18
- Fix : wrong label for max and medium buttons
- Fix : fix issue when no collider is found on a game object
- Fix : fix the incorrect usage of preferred bound types in ExtentsGetter
- Fix : uses height and radius of CapsuleCollider and CharacterController instead of standard collider bounds that return only bounds from radius
- Fix : wrong usage of calculated grid size when duplicating objects in grid
- New : Switch selected object values
- New : the Fall On feature uses now a SphereCastAll to get hits below the selected objects : it's more accurate !
- Change : only two icon sets are now included : All Blue and Colored (default). If you wish to get old icons back, ask me so, I'll make them public on my website
- Change : a lot of refactoring for the ExtentsGetter class and its usage, it was not always used correctly and not clean
- Change : the GetComponentsInChildren function used in ExtentsGetter can use now the correct returned type

v2.0 - 2012-08-29
- Fix (critical) : error in build process because of a DuplicateManager in the wrong folder (uses UnityEditor)
- New : tooltips for buttons (useful within the only buttons window so you can see used parameters)
- New : auto vertical/horizontal layout for the "only buttons" window
- New : align gameobjects on terrain angle when falling on terrain (deactivable in settings)
- New : Camera window to align with screen/camera features
- New : Orient to camera : orient all selected game objects so they are facing the camera
- New : Center in screen : make all selected objects be centered in the current scene view (lastSceneView camera)
- New : choose between icons, text, or both for the toolbars buttons
- Change : new design for settings popup
- Change : sample scenes are not included in the package anymore (not very useful and save bandwidth)
- Change : icons "Fall On" represents the angle that objects take into account when falling

v1.9 - 2012-06-14
- New : the alignment is now linked to the current tool (view/move, rotate, scale)
- Change : the axis detection from the Scene View use the right axis so it's more natural (see website for details)
- Change : label for the buttons toolbar has been changed to "Toolbar layout" (was "All In Direction")
- Fix : duplicate game objects correctly when children bounds does not contain parent transform position
- Fix : make objects fall on the terrain correctly when children bounds does not contain parent transform position
- Fix : close the settings window before reopening one (do not handle destruction)
- Fix : enforce the grid size minimum values to be 1

v1.8 - 2012-03-08
- New : get rid of the Resources folder, the icons can now be loaded with the AssetDatabase class
- New : three windows to allow any placement of classic, grid and all-in buttons ! The settings are in a utility window
- New : everything you configure is stored in the editor prefs : restore the state the next time you launch Unity
- Fix : object duplication launched an exception by trying to use the prefab name when there is no prefab
- Fix : used obsolete functions from unity 3.4
- Change : default value for SortAxis is "None" (was "X")
- Change : default value for UseAlignAxis is "false" (was "true")
- Change : default value for All In Direction is "Vertical" (was "Horizontal")
- Change : default value for Preferred Bounds is "Collider" (was "Renderer")

v1.7 - 2011-12-05
- Fall On Terrain Feature
- Offset added for grid distribution and duplication
- AllIn Vertical setting to allow a really small toolbar
- Offset added for grid distribution and duplication
- AllIn tabs to show only icons
- Cosmetics changes, GUI elements enabled only when useful
- Protection against misused of some Align APIs
- Internal API SortBy and Landmark enums
- C# inline comments and doc corrections
- Simplified External plugins (less parameters, signatures with even less parameters)
- Now located in Window/Equilibre/Align Editor instead of Window/EGames/Align Editor

v1.6 - 2011-11-23
- Fixed the grid duplication that could create an object at the exact position of the selected object
- Align and Duplicate Functions now externalized also with full inline C# documentation
- Settings tab : choose the icon set, usage of boundaries or mesh extents
- New icons set

v1.5 - 2011-05-24
- Fix : the calculated grid size is now used in grid distribution
- Extents management and Distribution functions has been externalized so you can use them directly from your code !
- Fully inline C# comments to easy use of functions

v1.4 - 2011-05-15
- two modes : classic for alignment and line distribution, grid for grid distribution/duplication
- detect user skin to adapt used icons (updated icons)
- grid distribution with plane choice (All, XY, XZ, YZ)
- take child objects into account for boundaries
- auto detect : duplicate prefab or active object for 1 item selection, make only a distribution for N items
- keep prefab link for duplication

v1.3 - 2011-03-16
- sample scenes
- sort axis can be chosen or linked to the align axis
- distribution enabled for scale
- distribution enabled for rotation
- regions added to help reading the code
- enhanced icon
- screenshots

v1.2 - 2011-02-26
- added the checkbox "auto" to let you decide if you wish to auto detect the alignment axis from the scene view, or not
- the "autodetect" works only in the position alignement type
- all changes in the inspector have been made undo-able to fulfill asset store requirements
- icons loaded via the Resources class instead of EditorGUIUtility (that needs the Assets/Editor Default Resources folder, not included for the Asset Store)
- distribute button is now hidden when position is not selected (as it does nothing for rotation and scale)

v1.1 - 2011-02-24
- Entirely rewritten code for alignment !
- Move the Align Editor Window the the "Window" menu
- Add buttons for (min, mean, max, distribution) alignment
- Auto set the axis from the last active view camera !
- Repaint the window when user changes something (select objects for eg.)
- Remove the massive setter from this editor window, it will be shipped with another editor window

v1.0 - 2011-02-18
- first delivery to the blog and public share !
