# AINirvanaUltrawideFix
Implements proper ultrawide support and UI fixes for **AI: The Somnium Files - nirvanA Initiative** through *MelonLoader*.

*Credits to @Lyall for initial fix and resolution update code.*

![image](https://user-images.githubusercontent.com/11449167/178424192-2f9f0697-dcf0-4c84-bfb1-5ff1f63fb944.png)

**Fixes:**
* Support for arbitrary resolutions and aspect ratios
* Proper UI fixes, including proper window scaling and scaling of visual filters (like brightness)
* **Fixes in-game MSAA**

## Installation
* Extract the [latest release](https://github.com/jshethbright/AINirvanaUltrawideFix/releases) to the root of the game directory.
    * For Steam, this can be located by right-clicking the game in the library bar -> Manage -> Browse local files.
* Run the game (this will take a while on the first run as MelonLoader needs to install).

## Configuration
* Edit the generated config file at the root of the game directory, in the **Mod** folder.
* **Settings:**
    * `Resolution_Width`: Monitor resolution width (e.g. 3440)
    * `Resolution_Height`: Monitor resolution height (e.g. 1440)
    * `Fullscreen`: (true/false) Choose between windowed (false) or fullscreen (true)
    * `UI_Fixes`: (true/false) Enable UI fixes for ultrawide monitors
    * `CursorVisible`: (true/false) Hide hardware cursor
    * `MSAA`: (0/2/4/8) Enable Multisample anti-aliasing (MSAA)

## Usage
* Run the game!
* Combine this mod with [SpecialK's framerate limiter](https://wiki.special-k.info/) for a smoother experience.

## Known Issues
* When using a controller, the game cursor is bound to a 16:9 rectangle.

## Credits
* @Lyall for his [AISomniumFiles2Fix](https://github.com/Lyall/AISomniumFiles2Fix)
* [MelonLoader](https://github.com/LavaGang/MelonLoader) is licensed under the Apache License, Version 2.0. See [LICENSE](https://github.com/LavaGang/MelonLoader/blob/master/LICENSE.md) for the full License.
