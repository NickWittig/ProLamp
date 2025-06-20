import cv2
import numpy as np

class FrameCalibrator:
    """
    FrameCalibrator allows interactive calibration on a single frame.

    The user clicks on 4 corners in the provided frame. The source points are then mapped to the provided
    destination points using a homography. The expected click order is: top-left, top-right, bottom-right, bottom-left.
    """
    def __init__(self, frame, dst_points):
        """
        Initializes the calibrator with a frame and destination points.

        :param frame: A BGR image (NumPy array) for calibration.
        :param dst_points: A 4x2 NumPy array containing destination points.
        """
        self.original_frame = frame.copy()
        self.frame = frame.copy()
        self.dst_points = dst_points
        self.calibration_points = []

    def mouse_callback(self, event, x, y, flags, param):
        """
        Mouse callback that captures calibration points upon a left-button click.
        """
        if event == cv2.EVENT_LBUTTONDOWN:
            print("Clicked at: ({}, {})".format(x, y))
            self.calibration_points.append([x, y])
            cv2.circle(self.frame, (x, y), 5, (0, 255, 0), -1)
            cv2.imshow("Calibration", self.frame)

    def calibrate(self, window_name="Calibration"):
        """
        Runs interactive calibration on the provided frame.

        :param window_name: The name of the calibration window.
        :return: The computed 3x3 homography matrix (NumPy array), or None if calibration fails.
        """
        cv2.namedWindow(window_name, cv2.WINDOW_NORMAL)
        cv2.setMouseCallback(window_name, self.mouse_callback)
        print("Calibration: Please click on 4 corners in the window, then press ESC.")

        while True:
            cv2.imshow(window_name, self.frame)
            key = cv2.waitKey(1) & 0xFF
            if key == 27 or len(self.calibration_points) >= 4:
                break
        cv2.destroyWindow(window_name)
        if len(self.calibration_points) < 4:
            print("Calibration aborted: not enough points captured.")
            return None
        src_points = np.array(self.calibration_points[:4], dtype=np.float32)
        homography, _ = cv2.findHomography(src_points, self.dst_points)
        print("Computed homography matrix:")
        print(homography)
        return homography
