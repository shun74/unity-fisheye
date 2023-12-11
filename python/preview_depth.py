import matplotlib.pyplot as plt
import OpenEXR
import Imath
import numpy as np

depth_file = OpenEXR.InputFile("outputs/fisheye_depth.exr")

dw = depth_file.header()['dataWindow']
size = (dw.max.x - dw.min.x + 1, dw.max.y - dw.min.y + 1)

pt = Imath.PixelType(Imath.PixelType.FLOAT)
depth_str = depth_file.channel("Y", pt)
depth = np.frombuffer(depth_str, dtype=np.float32).reshape((size[1], size[0]))
depth = np.where(depth > 100, 0, depth)

plt.imshow(depth, cmap='gray')
plt.show()
