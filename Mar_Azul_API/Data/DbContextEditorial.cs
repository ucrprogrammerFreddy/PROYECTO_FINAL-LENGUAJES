using Mar_Azul_API.Models;
using Microsoft.EntityFrameworkCore;

namespace Mar_Azul_API.Data
{

    public class DbContextEditorial : DbContext
    {
        // Constructor que recibe opciones de configuración para la base de datos.
        // Estas opciones son proporcionadas desde la configuración de la aplicación (en Program.cs).
        // Esto es esencial porque permite flexibilidad al definir el proveedor de base de datos (SQL Server, SQLite, etc.).
        //Es imprescindible, ya que sin esto, DbContext no sabe cómo conectarse a la base de datos.


        public DbContextEditorial(DbContextOptions<DbContextEditorial> options) : base(options)
        {

        }// Cierre del constructor

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Etiquetas> Etiquetas { get; set; }
        public DbSet<Articulos> Articulos { get; set; }
        public DbSet<Categorias> Categorias { get; set; }
        public DbSet<Secciones> Secciones { get; set; }
        public DbSet<ArticuloAutor> ArticuloAutor { get; set; }
        public DbSet<ArticuloAutor> ArticuloEtiqueta { get; set; }




        // Método que se ejecuta al crear el modelo de la base de datos.
        // Se usa para configurar relaciones, restricciones y datos semilla.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {


            modelBuilder.Entity<ArticuloAutor>()
              .HasKey(ae => new { ae.IdArticulo, ae.IdUsuario });

            // Relación entre ArticulosAutor y Articulo
            modelBuilder.Entity<ArticuloAutor>()
                .HasOne(aa => aa.Articulo)
                .WithMany(a => a.ArticuloAutor)
                .HasForeignKey(ae => ae.IdArticulo);

            modelBuilder.Entity<ArticuloAutor>()
                .HasOne(au => au.Usuario)
                .WithMany(a => a.ArticuloAutor)
                .HasForeignKey(ae => ae.IdUsuario);

            // Relacion Articulo Autor - Articulos

            // Relación DetallePedido - Producto
            modelBuilder.Entity<ArticuloAutor>()
                .HasOne(dp => dp.Articulo)
                .WithMany(aa => aa.ArticuloAutor)
                .HasForeignKey(dp => dp.IdArticulo);



            // Categoria -articulo
            modelBuilder.Entity<Articulos>()
                .HasOne(a => a.Categoria)
                .WithMany(c => c.Articulo)
                .HasForeignKey(a => a.IdCategoria);

            // relacion categoria seccion
            modelBuilder.Entity<Categorias>()
              .HasOne(c => c.Seccion)
              .WithMany(s => s.Categoria)
              .HasForeignKey(a => a.IdSeccion);

            // Definir la clave primaria compuesta para ArticuloEtiqueta
            modelBuilder.Entity<ArticuloEtiqueta>()
                .HasKey(ae => new { ae.IdArticulo, ae.IdEtiqueta });

            // Relación entre Articulos y Etiquetas (ArticuloEtiqueta)

            modelBuilder.Entity<ArticuloEtiqueta>()
            .HasOne(ae => ae.Articulo)
            .WithMany(a => a.ArticuloEtiqueta)
            .HasForeignKey(ae => ae.IdArticulo)
            .OnDelete(DeleteBehavior.NoAction); // Prevenir eliminación en cascada

            modelBuilder.Entity<ArticuloEtiqueta>()
                .HasOne(ae => ae.Etiqueta)
                .WithMany(e => e.ArticuloEtiqueta)
                .HasForeignKey(ae => ae.IdEtiqueta)
                .OnDelete(DeleteBehavior.NoAction); // Prevenir eliminación en cascada


















            // Agregamos datos iniciales a la tabla Etiqueetas para que existan al momento de crear la base de datos.
            // Esto es útil para pruebas o configuraciones iniciales.
            // Si la aplicación no necesita datos iniciales, esta parte del código puede omitirse.
            modelBuilder.Entity<Etiquetas>().HasData
            (new Etiquetas()
            {
                IdEtiqueta = 1,
                Nombre = "Etiqueta nueva",
                Estado = 'A'
            }
            );
        }// Cierre del método OnModelCreating
    } // Cierre de la clase
} // Cierre del namespace 
