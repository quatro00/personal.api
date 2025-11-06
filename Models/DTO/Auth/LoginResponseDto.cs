namespace Farmacia.UI.Models.DTO.Auth
{
    public class LoginResponseDto
    {
        public string Username { get; set; }
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public List<string> Roles { get; set; }
    }
}
