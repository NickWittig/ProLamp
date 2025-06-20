import cv2
import numpy as np
from .homography_storage import HomographyStorage


class Calibrator:
    """
    Calibrator handles interactive calibration by capturing four source points from
    a video stream (via mouse clicks) and computing a homography that maps these source
    points to predefined destination points.

    The user is expected to click four corners in the following order:
    top-left, top-right, bottom-right, bottom-left.
    """

    def __init__(self, cap, dst_points):
        """
        Initializes the calibrator.

        :param cap: OpenCV VideoCapture object.
        :param dst_points: A 4x2 NumPy array containing destination points in the target coordinate system.
                           The order should be: [top-left, top-right, bottom-right, bottom-left].
        """
        self.cap = cap
        self.dst_points = dst_points
        self.src_points = None
        self.homography = None
        self.calibration_points = []
        self.homography_storage = HomographyStorage("./data/homography.json")

    def mouse_callback(self, event, x, y, flags, param):
        """
        Mouse callback function that captures calibration points on mouse click.
        """
        if event == cv2.EVENT_LBUTTONDOWN:
            print("Clicked at: ({}, {})".format(x, y))
            self.calibration_points.append([x, y])

    def calibrate(self, window_name="Calibration - Click 4 corners"):
        """
        Enters calibration mode by displaying the camera feed and letting the user
        click on four corners. After capturing four points, the homography is computed
        mapping the source points to the predefined destination points.

        :param window_name: The name of the window used for calibration.
        :return: The computed 3x3 homography matrix (NumPy array) or None if calibration fails.
        """
        cv2.namedWindow(window_name, cv2.WINDOW_NORMAL)
        cv2.setMouseCallback(window_name, self.mouse_callback)
        print(
            "Calibration: Please click on the four corners (order: top-left, top-right, bottom-right, bottom-left) of the projection area in the camera feed."
        )

        while True:
            ret, frame = self.cap.read()
            if not ret:
                continue

            # Draw the already captured calibration points.
            for pt in self.calibration_points:
                cv2.circle(frame, (int(pt[0]), int(pt[1])), 5, (0, 255, 0), -1)

            cv2.imshow(window_name, frame)
            key = cv2.waitKey(1) & 0xFF
            if len(self.calibration_points) >= 4 or key == 27:
                break

        cv2.destroyWindow(window_name)
        if len(self.calibration_points) < 4:
            print("Calibration aborted: not enough points captured.")
            return None

        self.src_points = np.array(self.calibration_points[:4], dtype=np.float32)
        print("Captured source points:")
        print(self.src_points)
        self.homography, _ = cv2.findHomography(self.src_points, self.dst_points)
        self.homography_storage.store(self.homography)
        print("Computed homography matrix:")
        print(self.homography)
        return self.homography
