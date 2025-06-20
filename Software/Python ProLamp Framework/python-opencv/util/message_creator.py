class MessageCreator:
    """
    MessageCreator formats processed marker data into a UDP message.
    
    The expected message format is:
      "N, center1_x, center1_y, orientation1, center2_x, center2_y, orientation2, ..."
    where N is the number of markers detected.
    """
    @staticmethod
    def create_message_from_centers(centers, orientations):
        """
        Creates a comma-separated string from a list of marker centers and their orientations.
        
        :param centers: A list of NumPy arrays (each shape (2,)) representing marker centers.
        :param orientations: A list of integers representing each marker's rough orientation.
        :return: A string formatted as described above.
        """
        N = len(centers)
        message_parts = [str(N)]
        for i in range(N):
            message_parts.append("{:.2f}".format(centers[i][0]))
            message_parts.append("{:.2f}".format(centers[i][1]))
            message_parts.append(str(orientations[i]))
        return ",".join(message_parts)
