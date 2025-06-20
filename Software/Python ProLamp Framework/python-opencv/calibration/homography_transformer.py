import cv2
import numpy as np

class HomographyTransformer:
    """
    HomographyTransformer applies a homography transformation to arbitrary 2D points.
    
    This class maps points from the source coordinate system (e.g., camera image coordinates)
    to the destination coordinate system using the provided homography matrix.
    """
    def __init__(self, homography_matrix):
        """
        Initializes the transformer with a given homography matrix.
        
        :param homography_matrix: A 3x3 NumPy array representing the homography transformation.
        """
        self.homography = homography_matrix

    def transform_points(self, points):
        """
        Applies the homography transformation to an arbitrary set of 2D points.
        
        :param points: Either a single 2D point (iterable of two elements) or an array-like
                       collection of 2D points with shape (N, 2).
        :return: The transformed point(s):
                 - If a single point is provided, returns a NumPy array of shape (2,).
                 - If multiple points are provided, returns a NumPy array of shape (N, 2).
        :raises ValueError: If the input is not in the expected shape.
        """
        points = np.array(points, dtype=np.float32)
        
        if points.ndim == 1:
            if points.shape[0] != 2:
                raise ValueError("A single point must have exactly two elements (x, y).")
            points = points.reshape(1, 1, 2)
            transformed = cv2.perspectiveTransform(points, self.homography)
            return transformed.reshape(2,)
        elif points.ndim == 2:
            if points.shape[1] != 2:
                raise ValueError("Each point must have exactly two elements (x, y).")
            points = points.reshape(-1, 1, 2)
            transformed = cv2.perspectiveTransform(points, self.homography)
            return transformed.reshape(-1, 2)
        else:
            raise ValueError("Input points must be a single point (shape (2,)) or an array of points (shape (N, 2)).")
