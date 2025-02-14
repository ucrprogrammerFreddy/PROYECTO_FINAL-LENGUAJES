namespace EcoHogar_API.Services
{
    using System.Net;
    using System.IO;
    using Microsoft.Extensions.Configuration;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using System;

    public class FTPService
    {
        private readonly string _server;
        private readonly string _username;
        private readonly string _password;
        private readonly string _baseUrl;

        public FTPService(IConfiguration configuration)
        {
            _server = configuration["FTPSettings:Server"];
            _username = configuration["FTPSettings:Username"];
            _password = configuration["FTPSettings:Password"];
            _baseUrl = configuration["FTPSettings:BaseUrl"];
        }

        private string GenerateUniqueFileName(string originalFileName)
        {
            string extension = Path.GetExtension(originalFileName);
            string uniqueFileName = $"{Guid.NewGuid()}{extension}";
            return uniqueFileName;
        }

        private string GetFolderByFileType(string extension)
        {
            switch (extension.ToLower())
            {
                case ".mp4":
                case ".mkv":
                case ".avi":
                    return "videos/";
                case ".jpg":
                case ".jpeg":
                case ".png":
                case ".gif":
                    return "images/";
                case ".pdf":
                case ".doc":
                case ".docx":
                    return "documents/";
                default:
                    return "others/";
            }
        }

        private async Task<bool> EnsureDirectoryExistsAsync(string folder)
        {
            try
            {
                string directoryUrl = $"{_server}/{folder}".TrimEnd('/');
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(directoryUrl);
                request.Method = WebRequestMethods.Ftp.MakeDirectory;
                request.Credentials = new NetworkCredential(_username, _password);
                request.UsePassive = true;
                request.UseBinary = true;
                request.KeepAlive = false;

                using FtpWebResponse response = (FtpWebResponse)await request.GetResponseAsync();
                return true; // Directorio creado exitosamente
            }
            catch (WebException ex)
            {
                if (ex.Response is FtpWebResponse response)
                {
                    if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                    {
                        // El directorio ya existe
                        return true;
                    }
                    else
                    {
                        // Otro error
                        return false;
                    }
                }
                else
                {
                    // Error no relacionado con FTP
                    return false;
                }
            }
            catch (Exception)
            {
                // Otro tipo de excepción
                return false;
            }
        }

        public async Task<(string imageUrl, string errorMessage)> UploadFileAsync(string localFilePath, string originalFileName)
        {
            // Generar un nombre único para el archivo
            string uniqueFileName = GenerateUniqueFileName(originalFileName);
            string extension = Path.GetExtension(originalFileName);
            string folder = GetFolderByFileType(extension);  // Obtener la carpeta según el tipo de archivo

            // Asegurar que el directorio exista en el servidor FTP
            bool directoryExists = await EnsureDirectoryExistsAsync(folder);
            if (!directoryExists)
            {
                return (null, $"No se pudo crear o acceder al directorio '{folder}' en el servidor FTP.");
            }

            // Ruta completa en el servidor FTP
            string ftpUrl = $"{_server}/{folder}{uniqueFileName}";

            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpUrl);
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.Credentials = new NetworkCredential(_username, _password);
                request.UsePassive = true;
                request.UseBinary = true;
                request.KeepAlive = false;

                byte[] fileContents = await File.ReadAllBytesAsync(localFilePath);
                request.ContentLength = fileContents.Length;

                using (Stream requestStream = request.GetRequestStream())
                {
                    await requestStream.WriteAsync(fileContents, 0, fileContents.Length);
                }

                using FtpWebResponse response = (FtpWebResponse)await request.GetResponseAsync();
                if (response.StatusCode == FtpStatusCode.ClosingData || response.StatusCode == FtpStatusCode.FileActionOK)
                {
                    // Devuelve el enlace completo al archivo subido
                    return ($"{_baseUrl}{folder}{uniqueFileName}", null);
                }

                return (null, $"El servidor FTP devolvió el código de estado: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                // Retornar el mensaje de excepción para diagnosticar el error
                return (null, ex.Message);
            }
        }

        public async Task<bool> DeleteFileAsync(string remoteFileName)
        {
            string ftpUrl = $"{_server}/{remoteFileName}";
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpUrl);
                request.Method = WebRequestMethods.Ftp.DeleteFile;
                request.Credentials = new NetworkCredential(_username, _password);
                request.UsePassive = true;
                request.UseBinary = true;
                request.KeepAlive = false;

                using FtpWebResponse response = (FtpWebResponse)await request.GetResponseAsync();
                return response.StatusCode == FtpStatusCode.FileActionOK;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<string>> ListFilesAsync()
        {
            string ftpUrl = _server;
            List<string> files = new List<string>();

            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpUrl);
                request.Method = WebRequestMethods.Ftp.ListDirectory;
                request.Credentials = new NetworkCredential(_username, _password);
                request.UsePassive = true;
                request.UseBinary = true;
                request.KeepAlive = false;

                using FtpWebResponse response = (FtpWebResponse)await request.GetResponseAsync();
                using StreamReader reader = new StreamReader(response.GetResponseStream());

                string line = await reader.ReadLineAsync();
                while (!string.IsNullOrEmpty(line))
                {
                    files.Add(line);
                    line = await reader.ReadLineAsync();
                }

                return files;
            }
            catch (Exception)
            {
                return files;
            }
        }
    }
}
