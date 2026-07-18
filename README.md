# NVG Sight Dimmer

Red dots bloom like a flashbang through NVGs. This dims sight and scope reticles automatically the moment your goggles come down, and puts everything back when they come up.

## Sliders (F12)

- **Enable Auto Dim** — the whole mod on/off
- **NVG Sight Brightness (Holo / Red Dot)** — multiplier for collimator sights (0.01–1.0, lower = dimmer)
- **NVG Sight Brightness (Scopes)** — multiplier for magnified scope reticles, same range

## Known limitations

- ACOG-style prism scopes (e.g. Monstrum Tactical 2x32) bake their reticle into the scope mesh as static emissive geometry — there's no shader value to turn down, so they can't be dimmed yet. On the list, along with some LPVOs.
- The HHS-1's flip-up magnifier has no reticle of its own — it magnifies the holo dot, which the holo slider already handles.

## Install

`NVGSightDimmer` folder into `BepInEx/plugins/`. SPT 4.0.x / BepInEx 5.x.
