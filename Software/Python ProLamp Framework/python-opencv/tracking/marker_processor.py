import cv2
import numpy as np

class MarkerProcessor:
    """
    MarkerProcessor processes a frame to detect all ArUco markers,
    transform their corners using a given homography, and compute each marker's center
    along with a rough orientation (rounded to the nearest 90 degrees).
    """
    
    @staticmethod
    def compute_rough_orientation(transformed_corners):
        """
        Computes a rough orientation for a marker based on its top edge.
        The orientation is determined by the vector from corner 0 to corner 1 and is rounded
        to the nearest multiple of 90 (0, 90, 180, or 270 degrees).
        
        :param transformed_corners: A NumPy array of shape (4, 2) representing the transformed marker corners.
        :return: An integer representing the rough orientation in degrees.
        """
        # Calculate vector from the first to the second corner.
        v = transformed_corners[1] - transformed_corners[0]
        angle = np.degrees(np.arctan2(v[1], v[0]))
        if angle < 0:
            angle += 360
        rough_angle = round(angle / 90) * 90 % 360
        return int(rough_angle)

    @staticmethod
    def process_all_marker_data(frame, detector, transformer):
        """
        Processes the given frame to detect all ArUco markers, transform their corners,
        and compute each marker's center and rough orientation.
        
        :param frame: BGR image (NumPy array) representing the current frame.
        :param detector: An instance of ArucoMarkerDetector.
        :param transformer: An instance of HomographyTransformer.
        :return: A tuple (centers, orientations) where:
                 - centers is a list of NumPy arrays (each shape (2,)) representing marker centers.
                 - orientations is a list of integers representing the rough orientation of each marker.
                 Returns ([], []) if no markers are detected.
        """
        centers = []
        orientations = []
        corners, ids, _ = detector.detect(frame)
        if ids is None or len(ids) == 0:
            return centers, orientations

        for marker in corners:
            # marker has shape (1,4,2); reshape to (4,2)
            marker_corners = marker.reshape(4, 2)
            transformed_corners = transformer.transform_points(marker_corners)
            center = transformed_corners.mean(axis=0)
            # Multiply orientation by -1 if needed (as in your original code)
            rough_orientation = MarkerProcessor.compute_rough_orientation(transformed_corners) * -1
            centers.append(center)
            orientations.append(rough_orientation)
        return centers, orientations
