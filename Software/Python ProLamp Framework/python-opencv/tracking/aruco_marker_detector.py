import cv2
import cv2.aruco as aruco
import numpy as np

class ArucoMarkerDetector:
    """
    ArucoMarkerDetector is responsible for detecting ArUco markers in a video frame.
    
    It uses OpenCV's ArUco module and is configured via a dictionary (loaded from YAML)
    that specifies the desired ArUco dictionary.
    """
    def __init__(self, config):
        """
        Initializes the detector with configuration parameters.
        
        :param config: A dictionary containing ArUco configuration.
                       Expected key:
                         - dictionary: A string representing the ArUco dictionary (e.g., "DICT_4X4_50").
        """
        self.config = config
        dictionary_name = self.config.get("dictionary", "DICT_4X4_50")
        self.aruco_dict = aruco.getPredefinedDictionary(
            getattr(aruco, dictionary_name, aruco.DICT_4X4_50)
        )
        self.parameters = aruco.DetectorParameters()

    def detect(self, frame):
        """
        Detects ArUco markers in the provided frame.
        
        :param frame: A BGR image (NumPy array) from which to detect markers.
        :return: A tuple (corners, ids, rejected) as returned by cv2.aruco.detectMarkers.
        """
        gray = cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY)
        corners, ids, rejected = aruco.detectMarkers(gray, self.aruco_dict, parameters=self.parameters)
        return corners, ids, rejected
