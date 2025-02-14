using System.ComponentModel.DataAnnotations;

namespace Mar_Azul_API.Models
{
    public class Usuario
    {

       

        [Key]
        public int IdUsuario { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")] // Valida que el campo no esté vacío.
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")] // Define la longitud máxima.
        [DataType(DataType.Text)] // Especifica que es un texto.
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El correo es obligatorio")] // Valida que el campo no esté vacío.
        [StringLength(100, ErrorMessage = "El correo no puede exceder los 100 caracteres.")] // Define la longitud máxima.
        [DataType(DataType.Text)] // Especifica que es un texto.
        public string Email { get; set; }



        [Required(ErrorMessage = "La clave es obligatorio")] // Valida que el campo no esté vacío.
        [StringLength(100, ErrorMessage = "la clave no puede exceder los 100 caracteres.")] // Define la longitud máxima.
        [DataType(DataType.Text)] // Especifica que es un texto.
        public string Clave { get; set; }

        [Required(ErrorMessage = "El Rol es obligatorio")] // Valida que el campo no esté vacío.
        [RegularExpression("[A-Z]", ErrorMessage = "El rol debe ser un único carácter en mayúscula.")] // Valida el patrón de entrada.
        [DataType(DataType.Text)] // Especifica que es un texto (aunque es un char, puede tratarse como texto).
        public string Rol { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio.")] // Valida que el campo no esté vacío.
        [RegularExpression("[A-Z]", ErrorMessage = "El estado debe ser un único carácter en mayúscula.")] // Valida el patrón de entrada.
        [DataType(DataType.Text)] // Especifica que es un texto (aunque es un char, puede tratarse como texto).
        public string Estado { get; set; }

        public ICollection<ArticuloAutor> ArticuloAutor { get; set; }
    }
}
