namespace Mar_Azul_API.Models
{
    public class ArticuloEtiqueta
    {

        public Etiquetas Etiqueta { get; set; }
        public int IdEtiqueta { get; set; }

        public Articulos Articulo { get; set; }
        public int IdArticulo { get; set; }


    }
}
