# ProLamp python-opencv

This project demonstrates an interactive calibration and ArUco marker detection system that maps camera coordinates to a target coordinate system (such as a projector or Unity world). The transformed marker corner coordinates are then sent via UDP.

## Project Structure

- **calibrator.py**  
  Contains the `Calibrator` class, which handles interactive calibration. The user clicks on four corners in the camera feed to compute a homography.

- **udp_sender.py**  
  Contains the `UDPSender` class for sending messages over UDP.

- **aruco_detector.py**  
  Contains the `ArucoDetector` class, which detects ArUco markers, applies the computed homography, and sends the transformed coordinates via UDP.

- **main.py**  
  The main entry point that:
  1. Opens the camera.
  2. Performs interactive calibration.
  3. Initializes the UDP sender.
  4. Runs the ArUco marker detection loop.

## Requirements

- Python 3.x
- OpenCV (with the contrib modules): Install via

  ```bash
  pip install opencv-python opencv-contrib-python
  ```


## Code Formatting with Black

This project uses [Black](https://github.com/psf/black) to automatically format Python code. To install Black, run:

```bash
pip install black
```