import os
import json
import numpy as np

class HomographyStorage:
    """
    HomographyStorage handles saving and loading a homography matrix to/from a JSON file.
    
    This class is solely responsible for file I/O related to the homography matrix.
    """
    def __init__(self, filepath):
        """
        Initializes HomographyStorage with a specified file path.
        
        :param filepath: The path where the homography JSON file is stored.
        """
        self.filepath = filepath

    def load(self):
        """
        Loads the homography matrix from the JSON file if it exists.
        
        :return: A 3x3 NumPy array representing the homography matrix, or None if the file does not exist.
        """
        if os.path.exists(self.filepath):
            with open(self.filepath, "r") as f:
                data = json.load(f)
                H = np.array(data.get("homography_matrix"), dtype=np.float32)
                print("Loaded homography matrix from", self.filepath)
                return H
        else:
            return None

    def store(self, homography):
        """
        Saves the provided homography matrix to the JSON file.
        
        :param homography: A 3x3 NumPy array representing the homography matrix.
        """
        os.makedirs(os.path.dirname(self.filepath), exist_ok=True)
        data = {"homography_matrix": homography.tolist()}
        with open(self.filepath, "w") as f:
            json.dump(data, f, indent=4)
        print("Saved homography matrix to", self.filepath)
