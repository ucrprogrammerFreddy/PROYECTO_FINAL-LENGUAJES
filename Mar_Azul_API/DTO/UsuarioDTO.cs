namespace Mar_Azul_API.DTO
{
    public class UsuarioDTO
    {

        public int IdUsuario { get; set; }
        public string? Nombre { get; set; }
        public string? Email { get; set; }

        public string Clave { get; set; }

        public string? Rol { get; set; }
       public string? token { get; set; }   

        public string? Estado { get; set; }

    }
}
