# YAFE: Yet Another Fractal Explorer

This is my Final Degree Project made at Universitat de Barcelona for the Double degree in Mathematics and Computer Science!

This is an application that allows you to explore the fractals from Mandelbrot (and Multibrot) sets and Julia sets as well as some 3D fractals such as the Mandelbulb or some IFS fractals directly on a [website!](https://0xAdriaTorralba.github.io/YetAnotherFractalExplorer/)

In addition, you can find on [releases](https://github.com/0xAdriaTorralba/YetAnotherFractalExplorer/releases/latest) the binaries for MacOS (both compiled for Intel and Apple Silicon), Windows and Linux.

Might consider building and adapting this app to a mobile version (iOS and Android) if there are interest on something like that.

Find the Final Degree Project memory [here](http://diposit.ub.edu/dspace/handle/2445/178855)


### Differences between WebGL (Website) version and Desktop version

When I built this project (back in 2019), there was no support for threading on the CPU using WebGL, so the website version have the multithreading rendering on the CPU tab disabled, whereas the compiled desktop binaries have this feature enabled by default.

Unity claimed that they would be adding support for multithreading on WebGL compilations. Further research is needed.



## Screenshots of the application

### Main page
![Imgur](https://i.imgur.com/ijVMPEv.png "Main Page")

### 2D Fractals rendered by CPU
![Imgur](https://i.imgur.com/tLp4bv7.png "2D CPU Page")

#### More options

![Imgur](https://i.imgur.com/rsTvUpa.png "More options")

![Imgur](https://i.imgur.com/xHV539g.png "Finding periodic orbits")

![Imgur](https://i.imgur.com/jnhjqG8.png "Henrisken Algorithm")


![Imgur](https://i.imgur.com/QvQONdu.png "Different Colormaps")

### 2D Fractals rendered by GPU
![Imgur](https://i.imgur.com/OW7UJKj.png "2D GPU Scene")

![Imgur](https://i.imgur.com/66Jp1xR.png "Multibrot sets")


### 3D Fractals rendered by GPU
![Imgur](https://i.imgur.com/SF5M0uC.png "3D GPU Scene")


![Imgur](https://i.imgur.com/j77vjOF.png "Mandelbulb")



![Imgur](https://i.imgur.com/ceEp2xM.png "Sierpiński Tetrahedron")



![Imgur](https://i.imgur.com/BFbSLAR.png "Menger Sponge")

## Development setup

The simplest way to setup the project is to download `YAFE.unitypackage`, create an empty 3D project on Unity and import the downloaded file there.

Additionally, it should work aswell if you clone this repository and recompile and reimport the assets on your local editor.
