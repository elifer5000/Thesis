# Thesis (2012)

### Multi-picture and multi-scale 3D reconstruction of fracture surfaces using Stereo-Photogrammetry

Extension of work done by [M. Khokhlov](http://tx.technion.ac.il/~gordoncn/publications/2012/2b_Fisher_Paper.pdf)

This is a .NET project that uses [Emgu (OpenCV in .NET)](http://www.emgu.com/wiki/index.php/Main_Page) for cross-correlation calculations and [OpenTK (C# wrapper for OpenGL)](https://github.com/opentk/opentk) for visualizations.

It takes as input 2 images that are taken from the same microscopic sample, with slightly different angle, and reconstructs a 2.5D map of the surface. One of the images is used as a texture for the resulting surface.

![pic](screenshot.png)
