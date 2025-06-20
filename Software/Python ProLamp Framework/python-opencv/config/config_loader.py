import yaml

class Config:
    """
    Loads configuration values from a YAML file.
    
    All changeable parameters (UDP, camera, network, calibration, ArUco, etc.)
    are read from the configuration file.
    """
    def __init__(self, path="./config/config.yaml"):
        with open(path, "r") as f:
            self.config = yaml.safe_load(f)
    
    def get_udp_config(self):
        """Returns the UDP configuration as a dictionary."""
        return self.config.get("udp", {})
    
    def get_camera_config(self):
        """Returns the camera configuration as a dictionary."""
        return self.config.get("camera", {})
    
    def get_network_config(self):
        """Returns the network configuration as a dictionary."""
        return self.config.get("network", {})
    
    def get_calibration_config(self):
        """Returns the calibration configuration as a dictionary."""
        return self.config.get("calibration", {})
    
    def get_aruco_config(self):
        """Returns the ArUco configuration as a dictionary."""
        return self.config.get("aruco", {})
    
    def get_logging_config(self):
        """Returns the logging configuration as a dictionary."""
        return self.config.get("logging", {})

    def get_smoothing_config(self):
        """Returns the logging configuration as a dictionary."""
        return self.config.get("smoothing", {})
