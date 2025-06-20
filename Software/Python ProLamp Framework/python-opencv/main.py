import cv2
import numpy as np
from config.config_loader import Config
from network.udp_sender import UDPSender
from tracking.aruco_marker_detector import ArucoMarkerDetector
from calibration.homography_transformer import HomographyTransformer
from calibration.calibrator import Calibrator
from calibration.homography_storage import HomographyStorage
from tracking.marker_processor import MarkerProcessor
from util.message_creator import MessageCreator

def main():
    # Load configuration from YAML.
    config = Config()
    udp_config = config.get_udp_config()
    camera_config = config.get_camera_config()
    calib_config = config.get_calibration_config()
    aruco_config = config.get_aruco_config()
    logging_config = config.get_logging_config()

    # Configure and open the camera.
    cam_index = camera_config.get("index", 0)
    resolution = camera_config.get("resolution", {})
    width = resolution.get("width", 1920)
    height = resolution.get("height", 1080)

    cap = cv2.VideoCapture(cam_index)
    cap.set(cv2.CAP_PROP_FRAME_WIDTH, width)
    cap.set(cv2.CAP_PROP_FRAME_HEIGHT, height)
    if not cap.isOpened():
        raise RuntimeError("Could not open camera.")

    # Get calibration destination points from config.
    dst_points = np.array(
        calib_config.get("dst_points", [[0, 0], [1280, 0], [1280, 800], [0, 800]]),
        dtype=np.float32,
    )

    # Set up homography storage.
    homography_filepath = "./data/homography.json"
    homography_storage = HomographyStorage(homography_filepath)
    H = homography_storage.load()
    transformer = None
    if H is not None:
        transformer = HomographyTransformer(H)
    else:
        print("No homography matrix found. Starting interactive calibration...")
        calibrator = Calibrator(cap, dst_points)
        homography = calibrator.calibrate()
        if homography is None:
            print("Calibration failed.")
            cap.release()
            return
        transformer = HomographyTransformer(homography)
        homography_storage.store(homography)

    # Instantiate the ArUco detector.
    detector = ArucoMarkerDetector(aruco_config)

    # Instantiate the UDPSender.
    udp_sender = UDPSender(udp_config)

    print("Starting marker detection. Press ESC to exit.")
    while True:
        ret, frame = cap.read()
        if not ret:
            continue

        # Process all markers in the frame.
        centers, orientations = MarkerProcessor.process_all_marker_data(frame, detector, transformer)
        if centers:
            message = MessageCreator.create_message_from_centers(centers, orientations)
        else:
            message = "0"

        print("Sending UDP message:", message)
        udp_sender.send(message)

        if logging_config.get("level", "DEBUG") == "DEBUG":
            # Optionally, draw detected marker centers for debugging.
            # Note: We call detector.detect(frame) once more for visualization.
            detCorners, detIds, _ = detector.detect(frame)
            if detCorners is not None:
                cv2.aruco.drawDetectedMarkers(frame, detCorners, detIds)
            for center in centers:
                cv2.circle(frame, (int(center[0]), int(center[1])), 5, (0, 0, 255), -1)
            cv2.imshow("Marker Detection", frame)

        if cv2.waitKey(1) & 0xFF == 27:
            break

    cap.release()
    cv2.destroyAllWindows()
    udp_sender.close()

if __name__ == "__main__":
    main()
