# NVG Sight Dimmer

A BepInEx client mod for SPT that automatically dims sight and scope reticle brightness when night vision goggles (NVGs) are active.

## Features

- Automatically detects when NVGs are turned on and dims sight brightness to prevent the reticle from blooming/washing out the view
- Two independent F12 config sliders:
  - **NVG Sight Brightness (Holo / Red Dot)** — controls collimator sights (red dots, holographics)
  - **NVG Sight Brightness (Scopes)** — controls magnified scope reticles
- Toggle to enable/disable auto-dimming entirely
- Brightness automatically restores to normal when NVGs are turned off

## Installation

Extract the `NVGSightDimmer` folder into your `BepInEx/plugins/` directory.

## Configuration

Press **F12** in-game to open the config menu and adjust:
- **Enable Auto Dim** — toggle the mod on/off
- **NVG Sight Brightness (Holo / Red Dot)** — brightness multiplier (0.01–1.0, lower = dimmer)
- **NVG Sight Brightness (Scopes)** — brightness multiplier (0.01–1.0, lower = dimmer)

## Known Limitations

- ACOG-style prism scopes (e.g. Monstrum Tactical 2x32) have a reticle baked into the scope's mesh as static emissive geometry rather than a separate shader-driven element, so their brightness cannot be adjusted by this mod.
- The HHS-1 hybrid sight's flip-up magnifier has no reticle of its own — it magnifies the existing holographic dot, which is dimmed correctly by the holo slider.

## Requirements

- SPT (Single Player Tarkov)
- BepInEx
