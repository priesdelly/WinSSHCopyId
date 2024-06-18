# WinSSHCopyId

WinSSHCopyId is a Windows application developed in C# that provides an easy way to copy SSH public keys to a remote server. This tool is particularly useful for Windows users who lack the `ssh-copy-id` utility.

## Features

- **WinForm Interface**: User-friendly graphical interface for easy SSH key management.
- **C# Language**: Developed using C# for seamless integration with Windows.
- **SSH Key Management**: Automatically adds SSH public keys to the `authorized_keys` file on the remote server.

## Motivation

The motivation behind creating WinSSHCopyId is to provide Windows users with a simple and effective tool to manage SSH keys, similar to the `ssh-copy-id` utility available on Unix-based systems.

## Installation

### Prerequisites

- .NET Framework 4.8 or later
- Visual Studio 2019 or later

### Steps for Developers

1. Clone the repository:
    ```bash
    git clone https://github.com/priesdelly/WinSSHCopyId.git
    ```
2. Open the solution file `WinSSHCopyId.sln` in Visual Studio.
3. Build the solution to restore the NuGet packages and compile the project.

### Steps for Normal Users

1. Go to the [Releases](https://github.com/priesdelly/WinSSHCopyId/releases) section of the GitHub repository.
2. Download the latest release of the application.
3. Extract the downloaded files and run `WinSSHCopyId.exe`.

## Usage

1. Run the application by executing the `WinSSHCopyId.exe` file.
2. Enter the following details in the respective fields:
    - **Host**: The hostname or IP address of the remote server.
    - **Username**: The username to log in to the remote server.
    - **Password**: The password for the username (if required).
    - **Public Key**: The SSH public key to be added to the remote server.
3. Click the `Copy` button to add the public key to the `authorized_keys` file on the remote server.

## Example

Here is an example of how to use WinSSHCopyId:

1. Enter the remote server details:
    - Host: `example.com`
    - Username: `user`
    - Public Key: `ssh-rsa AAAAB3Nza...`
2. Click `Copy`.
3. The application will log the process in the console window, indicating whether the key was successfully added or if it already exists.

## Contributing

Contributions are welcome! Please follow these steps to contribute:

1. Fork the repository.
2. Create a new branch (`git checkout -b feature-branch`).
3. Make and commit your changes (`git commit -m 'Add new feature'`).
4. Push to the branch (`git push origin feature-branch`).
5. Open a Pull Request.

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details.

## Acknowledgments

- [Renci.SshNet](https://github.com/sshnet/SSH.NET) for the SSH.NET library used in this project.
- Inspiration from the `ssh-copy-id` utility available on Unix-based systems.

## Contact

For any questions or feedback, please open an issue on the [GitHub repository](https://github.com/priesdelly/WinSSHCopyId).
