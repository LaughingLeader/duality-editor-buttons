EditorButtons Plugin for Duality
=======
Customizable Component buttons for [Duality](http://duality.adamslair.net/), a 2D game engine for C#.

This plugin allows specialized properties (on Components) to display as buttons in the Duality Editor.

### Installation
1. Open Duality
2. Click **File**, then **Manage Packages...**
3. Change **View** from **Installed Packages** to **Online Repository**
4. Select **Editor Buttons Plugin** from the list, and click **Install**
5. Once the installation is done, click the **Apply** button.
6. Done!

### Manual Installation
1. Grab the repository, either through cloning or downloading the zip.
2. Open EditorButtons.sln in Visual Studio
3. Restore the nuget packages, or manually add the missing references for both the Core and Editor plugins.
4. Build the solution.
5. Copy the output files to your Duality project's Plugin directory.
###### Output Files
Core Files | Editor Files
------------ | -------------
EditorButtons.core.dll | EditorButtons.editor.dll
EditorButtons.core.pdb | EditorButtons.editor.pdb
EditorButtons.core.xml |

### Usage
See the [Sample plugin](https://github.com/LaughingLeader/duality-editor-buttons/tree/master/Sample/CorePlugin) for an example of how to use this plugin.

Also check out the [Getting Started](https://github.com/LaughingLeader/duality-editorbuttons/wiki/Getting_Started) page for more information.

## Contributing to this project

Anyone and everyone is welcome to contribute. Please take a moment to
review the [guidelines for contributing](CONTRIBUTING.md).

* [Bug reports](CONTRIBUTING.md#bugs)
* [Feature requests](CONTRIBUTING.md#features)
* [Pull requests](CONTRIBUTING.md#pull-requests)


### Attribution
- [Duality](https://github.com/AdamsLair/duality), licensed under the [MIT License](https://github.com/AdamsLair/duality/blob/master/LICENSE)
