import socket

class UDPSender:
    """
    UDPSender is responsible for sending messages over UDP.
    
    It is configured via a dictionary (typically loaded from the YAML configuration).
    """
    def __init__(self, config):
        """
        Initializes the UDPSender with configuration parameters.
        
        :param config: A dictionary containing UDP configuration.
                       Expected keys:
                         - client_ip: The IP address of the client (PC A) to send data to.
                         - port: The UDP port.
        """
        self.client_ip = config.get("client_ip", "127.0.0.1")
        self.port = config.get("port", 8051)
        self.sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)

    def send(self, message):
        """
        Sends a message via UDP.
        
        :param message: The message string to send.
        """
        self.sock.sendto(message.encode('utf-8'), (self.client_ip, self.port))
        #print("Sent message:", message)

    def close(self):
        """Closes the UDP socket."""
        self.sock.close()
