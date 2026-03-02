# URP Energy Shield Shader (HLSL)

A lightweight, high-performance energy shield shader for Unity **Universal Render Pipeline (URP)**, written in pure **HLSL**.

## Features
* **Intersection Highlighting:** Glow effect where the shield intersects with world geometry (Depth-based).
* **Fresnel (Rim) Effect:** Smooth edge glow for a spherical or organic look.
* **Animated Textures:** Support for scrolling textures/noise to simulate energy flow.
* **Highly Customizable:** Easily adjust colors, thickness, and distortion via the Material Inspector.
* **Optimized:** Written in HLSL for better performance compared to Shader Graph.

## Requirements
* Unity 2021.3 LTS or newer.
* Universal Render Pipeline (URP).

## Installation
1. Download or clone this repository.
2. Move the `Shaders` folder into your Unity `Assets` directory.
3. Create a new Material and select `Shader -> Custom/EnergyShield`.

## License
This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
