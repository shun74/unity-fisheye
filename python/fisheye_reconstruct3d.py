import open3d as o3d
import numpy as np
import matplotlib.pyplot as plt
import OpenEXR
import Imath
import cv2

def get_rgb(file_path):
    rgb_image = cv2.imread(file_path)
    rgb_image = cv2.cvtColor(rgb_image, cv2.COLOR_BGR2RGB)
    return rgb_image

def get_depth(file_path):
    depth_file = OpenEXR.InputFile(file_path)

    dw = depth_file.header()['dataWindow']
    size = (dw.max.x - dw.min.x + 1, dw.max.y - dw.min.y + 1)

    pt = Imath.PixelType(Imath.PixelType.FLOAT)
    depth_str = depth_file.channel("Y", pt)
    depth = np.frombuffer(depth_str, dtype=np.float32).reshape((size[1], size[0]))
    depth = np.where(depth > 100, 0, depth)
    return depth

def calc_pcd(rgb, depth, w, h, fx, fy, cx, cy):
    points = []
    colors = []
    for y in range(h):
        for x in range(w):
            z = depth[y, x]
            if z<0 or 100<z:
                continue
            X = (x - cx) * z / fx
            Y = (y - cy) * z / fy
            points.append((X, Y, z))
            color = rgb[y, x]
            colors.append(color)

    pcd = o3d.geometry.PointCloud()
    points = np.array(points)
    pcd.points = o3d.utility.Vector3dVector(points)
    colors = np.array(colors)
    colors_normalized = colors / 255.0
    pcd.colors = o3d.utility.Vector3dVector(colors_normalized)
    return pcd

def calculate_focal_length(w, h, angle_deg):
    angle_rad = np.deg2rad(angle_deg)
    f = (w / 2) / np.tan(angle_rad / 2)
    return f

def undistort_fisheye_image(img, w, h, fx, fy, cx, cy, k1, k2, k3, k4):
    K = np.array([[fx, 0, cx],
                  [0, fy, cy],
                  [0, 0, 1]])
    new_K = np.array([[cx, 0, cx],
                      [0, cy, cy],
                      [0, 0, 1]])
    D = np.array([k1, k2, k3, k4])

    map1, map2 = cv2.fisheye.initUndistortRectifyMap(K, D, np.eye(3), new_K, (w, h), cv2.CV_16SC2)
    undistorted_img = cv2.remap(img, map1, map2, interpolation=cv2.INTER_LINEAR, borderMode=cv2.BORDER_CONSTANT)

    return undistorted_img


if __name__ == "__main__":
    w = 1280
    h = 1280
    fx = 390.0
    fy = 390.0
    cx = 640.0
    cy = 640.0
    k1 = -0.028
    k2 = 0.00027
    k3 = -0.0076
    k4 = 0.0024
    fov = 136.0

    rgb = get_rgb("outputs/fisheye_rgb.png")
    depth = get_depth("outputs/fisheye_depth.exr")
    undistorted_rgb = undistort_fisheye_image(rgb, w, h, fx, fy, cx, cy, k1, k2, k3, k4)
    plt.imshow(undistorted_rgb)
    plt.show()
