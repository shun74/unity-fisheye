import open3d as o3d
import numpy as np
import matplotlib.pyplot as plt
import OpenEXR
import Imath
import cv2

fx = 512.0  # Focal length x
fy = 512.0  # Focal length y
cx = 512.0  # Optical center x
cy = 512.0  # Optical center y

depth_file = OpenEXR.InputFile("outputs/depth.exr")

dw = depth_file.header()['dataWindow']
size = (dw.max.x - dw.min.x + 1, dw.max.y - dw.min.y + 1)

pt = Imath.PixelType(Imath.PixelType.FLOAT)
depth_str = depth_file.channel("Y", pt)
depth = np.frombuffer(depth_str, dtype=np.float32).reshape((size[1], size[0]))

rgb_image = cv2.imread("outputs/rgb.png")
rgb_image = cv2.cvtColor(rgb_image, cv2.COLOR_BGR2RGB)

points = []
colors = []
for y in range(size[1]):
    for x in range(size[0]):
        z = depth[y, x]
        
        if z > 0 and z < 100:
            X = (x - cx) * z / fx
            Y = (y - cy) * z / fy
            points.append((X, Y, z))
            
            color = rgb_image[y, x]
            colors.append(color)

points = np.array(points)
colors = np.array(colors)

pcd = o3d.geometry.PointCloud()

pcd.points = o3d.utility.Vector3dVector(points)

colors_normalized = colors / 255.0
pcd.colors = o3d.utility.Vector3dVector(colors_normalized)

vis = o3d.visualization.Visualizer()
vis.create_window()

vis.get_render_option().background_color = np.array([0.8, 0.8, 0.8])

vis.add_geometry(pcd)

vis.run()
vis.destroy_window()

