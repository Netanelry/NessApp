using Microsoft.AspNetCore.Mvc;
using NessServer.DAL;
using NessServer.Exceptions;
using NessServer.Models;

namespace NessServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public ClientController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost("UploadFile")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest("No file uploaded.");
                }

                using var reader = new StreamReader(file.OpenReadStream());
                var validClients = ParseClientsFromFile(reader);

                await LoadClientsToDatabase(validClients);

                return Ok("File uploaded and data saved successfully.");
            }
            catch (DuplicateIdException duplicateEx)
            {
                return Conflict(duplicateEx.Message);
            }
            catch (ClientAlreadyExistsException clientExistsEx)
            {
                return Conflict(clientExistsEx.Message);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        private List<Client> ParseClientsFromFile(StreamReader reader)
        {
            var validClients = new List<Client>();
            var lineNumber = 0;
            var isFirstLine = true;
            var processedIds = new HashSet<int>();

            while (!reader.EndOfStream)
            {
                lineNumber++;
                var line = reader.ReadLine();

                try
                {
                    if (isFirstLine)
                    {
                        isFirstLine = false;
                        continue;
                    }

                    var clientData = line.Split(',');

                    if (clientData.Length != 4)
                    {
                        throw new Exception("Invalid data format.");
                    }

                    var fullName = clientData[0];
                    var idStr = clientData[1];
                    var ipAddress = clientData[2];
                    var phoneNumber = clientData[3];

                    if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(idStr) ||
                        string.IsNullOrEmpty(ipAddress) || string.IsNullOrEmpty(phoneNumber))
                    {
                        throw new Exception("One or more fields are missing.");
                    }

                    if (!int.TryParse(idStr, out var id) || double.IsNaN(id))
                    {
                        throw new Exception("Invalid ID format.");
                    }

                    phoneNumber = phoneNumber.Replace("\"", "");

                    if (processedIds.Contains(id))
                    {
                        throw new DuplicateIdException(id);
                    }

                    if (_dbContext.Clients.Any(c => c.Id == id))
                    {
                        throw new ClientAlreadyExistsException(id);
                    }

                    processedIds.Add(id);

                    var client = new Client
                    {
                        FullName = fullName,
                        Id = id,
                        IPAddress = ipAddress,
                        PhoneNumber = phoneNumber
                    };

                    validClients.Add(client);
                }
                catch (Exception ex)
                {
                    var errorMessage = $"Error parsing data in line {lineNumber}: {ex.Message}";
                    throw new Exception(errorMessage);
                }
            }

            return validClients;
        }

        private async Task LoadClientsToDatabase(List<Client> clients)
        {
            try
            {
                _dbContext.Clients.AddRange(clients);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error loading data to the database: {ex.Message}");
            }
        }

        [HttpGet]
        public IActionResult GetClients()
        {
            try
            {
                var clients = _dbContext.Clients.ToList();
                return Ok(clients);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving clients: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddClient(Client client)
        {
            try
            {
                if (client == null)
                {
                    return BadRequest("Invalid client data.");
                }

                if (_dbContext.Clients.Any(c => c.Id == client.Id))
                {
                    return BadRequest("A client with the same ID already exists.");
                }

                _dbContext.Clients.Add(client);
                await _dbContext.SaveChangesAsync();

                return Ok("Client added successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error adding client: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveClient(int id)
        {
            try
            {
                var client = await _dbContext.Clients.FindAsync(id);
                if (client == null)
                {
                    return NotFound("Client not found.");
                }

                _dbContext.Clients.Remove(client);
                await _dbContext.SaveChangesAsync();

                return Ok("Client removed successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error removing client: {ex.Message}");
            }
        }

        [HttpGet("GeoInfo")]
        public async Task<IActionResult> GetGeoInfo(string ipAddress)
        {
            try
            {
                var httpClient = new HttpClient();
                var response = await httpClient.GetFromJsonAsync<GeoInfoResponse>($"http://ip-api.com/json/{ipAddress}");

                if (response == null)
                {
                    return BadRequest("Error retrieving geo information.");
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error getting geo information: {ex.Message}");
            }
        }
    }

}