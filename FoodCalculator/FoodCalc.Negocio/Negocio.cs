using System;
using System.Collections.Generic;

namespace FoodCalc.Negocio
{

    public enum TipoDonador
    {
        Individual,
        Empresa,
        Organizacion
    }

    public enum TipoDonacion
    {
        Comida,
        Dinero,
        Insumos,
        Otro
    }

    // El documento original solo da 2 estados pero yo agregué mas 
    public enum EstadoDonacion
    {
        Pendiente,
        EnProceso,
        Completada,
        Cancelada
    }

    // NUEVO: no estaba contemplado en el doc, hace falta para NivelNecesidad de Comedor.
    public enum NivelNecesidad
    {
        Baja,
        Media,
        Alta,
        Critica
    }

    public abstract class Usuario
    {
        public int ID { get; set; }
        public string Nombre { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public string NombreUsuario { get; set; }
        public string PasswordHash { get; set; }

        public Usuario(int id, string nombre, string telefono, string email, string nombreUsuario, string passwordHash)
        {
            ID = id;
            Nombre = nombre;
            Telefono = telefono;
            Email = email;
            NombreUsuario = nombreUsuario;
            PasswordHash = passwordHash;
        }

        public bool IniciarSesion(string password)
        {
            // validacion
            return true;
        }

        public abstract void Registrar();
    }

    public class Donador : Usuario
    {
        public string Direccion { get; set; }
        public TipoDonador Tipo { get; set; }

        public Donador(
            int id, string nombre, string telefono, string email, string nombreUsuario, string passwordHash,
            string direccion, TipoDonador tipo)
          : base(id, nombre, telefono, email, nombreUsuario, passwordHash)
        {
            Direccion = direccion;
            Tipo = tipo;
        }

        public override void Registrar()
        {
            // Implementación específica para registrar un donador
        }

        public void IngresarDonacion()
        {

        }

        public void EnviarDonacion()
        {

        }
    }

    public class Administrador : Usuario
    {
        public Administrador(
            int id, string nombre, string telefono, string email, string nombreUsuario, string passwordHash)
          : base(id, nombre, telefono, email, nombreUsuario, passwordHash)
        {

        }

        public override void Registrar()
        {
            // Implementación específica para registrar un administrador
        }

        public void IngresarComedor()
        {

        }

        public void EjecutarDistribucion()
        {

        }

        public void GestionarUsuarios()
        {

        }

        public void GestionarDonaciones()
        {

        }
    }

    public class Donacion
    {
        public int ID { get; set; }
        public TipoDonacion Tipo { get; set; }
        public string Nombre { get; set; }
        public decimal Cantidad { get; set; }
        public DateTime Fecha { get; set; }
        public EstadoDonacion Estado { get; set; }

        // navegaciones (reemplazan los FK_ID_Donador / FK_ID_Pool del DER)
        public Donador Donador { get; set; }
        public Pool Pool { get; set; }

        public Donacion(int id, TipoDonacion tipo, string nombre, decimal cantidad, DateTime fecha,
            EstadoDonacion estado, Donador donador, Pool pool)
        {
            ID = id;
            Tipo = tipo;
            Nombre = nombre;
            Cantidad = cantidad;
            Fecha = fecha;
            Estado = estado;
            Donador = donador;
            Pool = pool;
        }
    }

    public class Pool
    {
        public int ID { get; set; }
        public TipoDonacion TipoDonacion { get; set; }
        public decimal CantidadDisponible { get; set; }
        public DateTime FechaActualizacion { get; set; }

        // navegaciones:
        public List<Donacion> Donaciones { get; set; } = new List<Donacion>();
        public List<Distribucion> Distribuciones { get; set; } = new List<Distribucion>();

        // NUEVO: faltaba la lógica de descuento de stock al ejecutar una distribución.
        public void DescontarCantidad(decimal monto)
        {
            if (monto < 0)
                throw new ArgumentException("El monto a descontar no puede ser negativo.");

            if (monto > CantidadDisponible)
                throw new InvalidOperationException(
                    $"No se puede descontar {monto} porque el Pool solo tiene {CantidadDisponible} disponible.");

            CantidadDisponible -= monto;
            FechaActualizacion = DateTime.Now;
        }

        public string CalcularEstadoPool()
        {
            if (CantidadDisponible <= 0)
                return "Vacío";

            if (CantidadDisponible < 10) // umbral a criterio, ajustar según lo que tenga sentido para tu escala 
                return "Bajo";

            return "Disponible";
        }

        public void AgregarDonacion(Donacion donacion)
        {

        }
    }

    // NUEVO: no existía en el doc.
    public class Distribucion
    {
        public int ID { get; set; }
        public decimal Cantidad { get; set; }
        public DateTime Fecha { get; set; }
        public string CriterioAplicado { get; set; }

        public Pool Pool { get; set; }
        public Comedor Comedor { get; set; }
        public Administrador Administrador { get; set; }

        public Distribucion(int id, decimal cantidad, DateTime fecha, string criterioAplicado,
            Pool pool, Comedor comedor, Administrador administrador)
        {
            ID = id;
            Cantidad = cantidad;
            Fecha = fecha;
            CriterioAplicado = criterioAplicado;
            Pool = pool;
            Comedor = comedor;
            Administrador = administrador;
        }

        public List<Comedor> RecomendarCentros()
        {
            return new List<Comedor>();
        }
    }

    // NUEVO: no existía en el doc.
    public class HistorialDistribucion
    {
        public int ID { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string Detalle { get; set; }

        // nullable: un registro es de una Donacion O de una Distribucion, no necesariamente ambas
        public Donacion Donacion { get; set; }
        public Distribucion Distribucion { get; set; }

        public void RegistrarEvento()
        {

        }
    }

    // NUEVO: no existía en el doc.
    public class Comedor
    {
        private const decimal UnidadPorPersona = 1;
        public int ID { get; set; }
        public string Nombre { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public decimal CantidadComidaRestante { get; set; }
        public decimal CantidadComidaTotal { get; set; }
        public decimal CantidadComidaNecesaria
        {
            get { return CantidadNecesitados * UnidadPorPersona; }
        }
        public int CantidadNecesitados { get; set; }
        public decimal PorcentajeCobertura         
        {
            get
            {
                if (CantidadComidaNecesaria == 0) return 0;
                return (CantidadComidaRestante / CantidadComidaNecesaria) * 100;
            }
        }
        public NivelNecesidad NivelNecesidad { get; set; }

        public Administrador Administrador { get; set; }
        public List<Distribucion> DistribucionesRecibidas { get; set; } = new List<Distribucion>();

        public NivelNecesidad CalcularNivelNecesidad()
        {
            decimal Necesidad = CantidadComidaNecesaria / CantidadNecesitados;
            if (CantidadComidaNecesaria == CantidadNecesitados) return NivelNecesidad.Baja;
            return NivelNecesidad.Baja;
        }
    }

    // NUEVO: recomendado para cumplir RNF5 (polimorfismo real en el algoritmo) y
    // RNF4 (agregar criterios sin tocar el motor). Ver checklist anterior.
    public interface ICriterioPriorizacion
    {
        decimal CalcularPrioridad(Comedor comedor);
    }

    // Prioriza según qué tan grave es la falta de comida
    public class CriterioPorUrgencia : ICriterioPriorizacion
    {
        public decimal CalcularPrioridad(Comedor comedor)
        {
            // cuanto menos comida le queda respecto a lo que necesita, más prioridad
            if (comedor.CantidadComidaNecesaria == 0) return 0;

            decimal faltante = comedor.CantidadComidaNecesaria - comedor.CantidadComidaRestante;
            return faltante / comedor.CantidadComidaNecesaria;
        }
    }

    //Se pueden agregar mas criterios de priorización implementando la interfaz ICriterioPriorizacion

    // NUEVO: no existía en el doc.
    public class MotorDistribucion
    {
        public List<Distribucion> Ejecutar(Pool pool, List<Comedor> comedores,
            List<ICriterioPriorizacion> criterios, Administrador administrador)
        {
            var resultado = new List<Distribucion>();
            decimal disponible = pool.CantidadDisponible;

            // equilibrio: búsqueda binaria del nivel de cobertura sostenible
            decimal minimo = 0, maximo = 1;
            for (int i = 0; i < 30; i++)
            {
                decimal mitad = (minimo + maximo) / 2;
                decimal costoTotal = 0;
                foreach (var comedor in comedores)
                {
                    decimal faltaParaNivel = (mitad * comedor.CantidadComidaNecesaria) - comedor.CantidadComidaRestante;
                    if (faltaParaNivel > 0) costoTotal += faltaParaNivel;
                }
                if (costoTotal > disponible)
                    maximo = mitad;
                else
                    minimo = mitad;
            }

            // reparto: un solo loop, calcula y usa "falta" para el mismo comedor
            foreach (var comedor in comedores)
            {
                decimal falta = Math.Max(0, (minimo * comedor.CantidadComidaNecesaria) - comedor.CantidadComidaRestante);

                if (falta <= 0) continue; // ya está en el nivel de equilibrio, no le toca nada

                decimal aAsignar = Math.Min(falta, disponible);

                var distribucion = new Distribucion(
                    id: resultado.Count + 1,
                    cantidad: aAsignar,
                    fecha: DateTime.Now,
                    criterioAplicado: string.Join(", ", criterios.Select(c => c.GetType().Name)),
                    pool: pool,
                    comedor: comedor,
                    administrador: administrador
                );

                resultado.Add(distribucion);
                disponible -= aAsignar;
                comedor.CantidadComidaRestante += aAsignar;
            }

            pool.DescontarCantidad(pool.CantidadDisponible - disponible);
            pool.Distribuciones.AddRange(resultado);

            return resultado;
        }
    }
}