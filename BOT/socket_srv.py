import socket
import subprocess

def executor(command : str) -> str:
    try:
        result = subprocess.run(command, shell=True, check=True, stdout=subprocess.PIPE, stderr=subprocess.PIPE, text=True)
        return result.stdout
    except subprocess.CalledProcessError as e:
        return f"Error: {e.stderr}"

def start_server(host, port):
    server_socket = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
    server_socket.bind((host, port))
    print(f"Server started at {host}:{port}")

    while True:
        message, address = server_socket.recvfrom(1024)
        command = message.decode()
        print(f"Received command: {command}")

        response = executor(command)
        server_socket.sendto(response.encode(), address)
        print(f"Sent response: {response}")

if __name__ == "__main__":
    HOST = "127.0.0.1"
    #HOST = socket.gethostbyname(socket.gethostname())
    PORT = 8888
    start_server(HOST, PORT)
