using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mar_Azul_API.Models
{
    public class Secciones
    {
        // Clave primaria de tipo entero.
        [Key]
        public int IdSeccion { get; set; }

        // Nombre de la etiqueta, de tipo cadena (string). Obligatorio con longitud máxima de 100 caracteres.
        [Required(ErrorMessage = "El nombre es obligatorio.")] // Valida que el campo no esté vacío.
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")] // Define la longitud máxima.
        [DataType(DataType.Text)] // Especifica que es un texto.
        public string Nombre { get; set; }


         [Required(ErrorMessage = "El nombre es obligatorio.")] // Valida que el campo no esté vacío.
        [StringLength(100, ErrorMessage = "La descripcion no puede exceder los 100 caracteres.")] // Define la longitud máxima.
        [DataType(DataType.Text)] // Especifica que es un texto.
        public string Descripcion { get; set; }

        //Especifica que la url no es obligatorio : pendiente
        public string? ImagenURL { get; set; }


        // Estado de la seccion debe tener un tipo carácter (char). Obligatorio y debe ser un carácter en mayúscula.
        [Required(ErrorMessage = "El estado es obligatorio.")] // Valida que el campo no esté vacío.
        [RegularExpression("[A-Z]", ErrorMessage = "El estado debe ser un único carácter en mayúscula.")] // Valida el patrón de entrada.
        [DataType(DataType.Text)] // Especifica que es un texto (aunque es un char, puede tratarse como texto).
        public char Estado { get; set; }


     


        public ICollection<Categorias> Categoria { get; set; } = new List<Categorias>();


    }
}
