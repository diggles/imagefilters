Performance Characteristics
============
5 Iteration Benchmarks

Initial: 41 seconds
Optimised: 4 seconds

Apparently Bitmap.Width and Bitmap.Height are very expensive operations
The filter loop has been updates to refer to a pre-calculated value for a factor of 10 performance increase 


Convolution Filtering
============
An example implementation of convolution filtering in C#
http://en.wikipedia.org/wiki/Kernel_%28image_processing%29

Samples
============
Original
===
![Original](/samples/original.jpg?raw=true "Original")
Blur
===
![Blur](/samples/blur.PNG?raw=true "Blur")
Edge Detect
===
![Edge Detect](/samples/edge.PNG?raw=true "Edge Detect")
Sharpen
===
![Sharpen](/samples/sharp.PNG?raw=true "Sharpen")
