namespace Mar_Azul_API.DTO
{
    public class ArticuloDTO
    {
       public int IdArticulo { get; set; }

     public string Nombre { get; set; }

     public string Descripcion { get; set; }

     public string Contenido { get; set; }

        public string Categoria { get; set; } // Nombre de la categoria 

        public string ImagenUrl { get; set; }

     public DateTime FechaPublicacion { get; set; }

     public string Estado { get; set; }


    }
}
