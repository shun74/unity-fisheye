import numpy as np
import cv2
import matplotlib.pyplot as plt

if __name__ == "__main__":

  h = 512
  w = 512
  s = 32

  # create checkerboard
  checker = np.zeros((h,w), dtype=np.uint8)
  for i in range(0,h,s):
    for j in range(0,w,s):
      if (i/s + j/s)%2 == 0:
        checker[i:i+s,j:j+s] = 255
  
  # fx = 196
  # fy = 196
  fx = 256
  fy = 256
  cy = h//2
  cx = w//2
  strength = 3.0
 
  # create fisheye distortion map
  map_x = np.zeros((h,w), dtype=np.float32)
  map_y = np.zeros((h,w), dtype=np.float32)
  for i in range(0,h):
    for j in range(0,w):
      y = (i-h/2) / fy
      x = (j-w/2) / fx
      theta = np.arctan2(y,x)
      r = np.sqrt(y**2 + x**2)
      r = np.power(r, strength)
      x_d = r * np.cos(theta)
      y_d = r * np.sin(theta)
      # r = np.sqrt(y**2 + x**2)
      # theta = np.arctan(r)
      # y_d = theta/r * y
      # x_d = theta/r * x
      map_y[i,j] = fy*y_d + cy
      map_x[i,j] = fx*x_d + cx

  # apply distortion
  checker = cv2.remap(checker, map_x, map_y, cv2.INTER_LINEAR)

  plt.imshow(checker, cmap='gray')
  plt.show()
