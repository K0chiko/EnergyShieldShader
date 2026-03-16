# URP Energy Shield Shader (HLSL)

A professional, high-performance energy shield shader for Unity **Universal Render Pipeline (URP)**, written in pure **HLSL**. Optimized for Unity 6.
<img width="1253" height="1075" alt="Cover" src="https://github.com/user-attachments/assets/1eb304be-8835-49ed-96c5-f64c10a64c0a" />

https://github.com/user-attachments/assets/b3e1c9c3-24e5-45df-b34c-e31e1533a5ec


https://github.com/user-attachments/assets/2867e74c-ec37-4908-8548-b3718e05b4e4


## Features
* **Intersection Highlighting:** Glow effect where the shield intersects with world geometry (Depth-based).
* **Two-Sided Rendering:** Unique color properties for front and back faces (`_FrontColor` / `_BackColor`) using `SV_IsFrontFace`.
* **Vertex Displacement:** Dynamic "In/Out" breathing animation driven by sine waves and vertex normals.
* **Procedural Hexagon Grid:** A mathematical hex-grid overlay with adjustable wire-width and a rhythmic pulse effect.
* **Screen-Space Refraction:** Realistic background distortion using `_CameraOpaqueTexture` and a distortion normal map.
* **Dynamic Hit System:** Support for impact effects via `_HitPos` and `_HitStrength`, allowing the shield to react to collisions.
* **Fresnel (Rim) Effect:** Smooth edge glow for a spherical or organic look.
* **Animated Textures:** Support for scrolling textures/noise to simulate energy flow.
* **Highly Customizable:** Easily adjust colors, thickness, and distortion via the Material Inspector.
* **Optimized:** Hand-written HLSL for maximum performance and a clean Material Inspector with organized headers.

## Requirements
* Unity 6 (Untested on Unity 2022, but likely to work without issues.).
* Universal Render Pipeline (URP).
* **Note:** Ensure "Opaque Texture" is enabled in your URP Asset settings for the refraction effect to work.

## Installation
1. Download or clone this repository.
2. Move file `EnergyShield.shader` from the `Shield` folder into your Unity `Assets` directory.
3. Create a new Material and select `Shader -> Custom/EnergyShield`.
4. (Optional) Assign a hexagon or noise texture to the `Shield Texture` slot for best results. You can use my textures from the `Shield` folder for example

## License
This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.








