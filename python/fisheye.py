import numpy as np
import math
import cv2
import matplotlib.pyplot as plt

def fisheye_transform(x, y, rad):
    r = math.atan2(math.sqrt(x*x + y*y), 1.0) / rad
    phi = math.atan2(y, x)
    u = r * math.cos(phi)
    v = r * math.sin(phi)
    return u, v

def inverse_fisheye_transform(u, v, rad):
    r = math.hypot(u, v)
    phi = math.atan2(v, u)
    x = math.tan(r * rad) * math.cos(phi)
    y = math.tan(r * rad) * math.sin(phi)
    return x, y


def perspective_to_fisheye(img, K, new_K, rad):
    h, w = img.shape[:2]
    map_x = np.zeros((h, w), dtype=np.float32)
    map_y = np.zeros((h, w), dtype=np.float32)
    for i in range(h):
        for j in range(w):
            x = (j - new_K[0][2])/new_K[0][0]
            y = (i - new_K[1][2])/new_K[1][1]
            u, v = inverse_fisheye_transform(x, y, rad)
            px = u*K[0][0] + K[0][2]
            py = v*K[1][1] + K[1][2]
            map_x[i, j] = px
            map_y[i, j] = py
    fisheye_img = cv2.remap(img, map_x, map_y, cv2.INTER_LINEAR)
    return fisheye_img

if __name__ == "__main__":
    # create checkerboard pattern
    w = 512
    h = 512
    step = 32
    checker_img = np.zeros((h, w), dtype=np.uint8)
    for i in range(0, h, step):
        for j in range(0, w, step):
            if (i // step + j // step) % 2 == 0:
                checker_img[i:i + step, j:j + step] = 255

    # create fisheye image
    angle = 165
    radian = angle/2 * np.pi / 180
    a = np.tan(radian)
    K = np.array([[w/2/a, 0, w/2],
                  [0, h/2/a, h/2],
                  [0, 0, 1]])
    new_K = np.array([[w/2, 0, w/2],
                      [0, h/2, h/2],
                      [0, 0, 1]])
    print(f"rad: {radian}, tan(rad): {a}")
    print(f"fx: {K[0][0]}, fy: {K[1][1]}")
    fisheye_img = perspective_to_fisheye(checker_img, K, new_K, radian)

    # show images
    plt.subplot(1, 2, 1)
    plt.imshow(checker_img, cmap='gray')
    plt.title('checkerboard')
    plt.subplot(1, 2, 2)
    plt.imshow(fisheye_img, cmap='gray')
    plt.title('fisheye')
    plt.show()   

