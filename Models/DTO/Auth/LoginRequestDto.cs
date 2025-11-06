namespace Farmacia.UI.Models.DTO.Auth
{
    public class LoginRequestDto
    {
        public string username { get; set; }
        public string password { get; set; }
        public bool remember { get; set; }
    }
}
