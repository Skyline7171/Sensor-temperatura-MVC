using System.ComponentModel.DataAnnotations;

namespace MonitorSerial.Models
{
    public class LecturaMetadata
    {
        [Required(ErrorMessage = "Debe seleccionar un dispositivo.")]
        public int IdDispositivo { get; set; }

        [Required(ErrorMessage = "La temperatura es obligatoria.")]
        [Range(typeof(decimal), "-40", "125",
            ErrorMessage = "La temperatura debe estar entre -40 °C y 125 °C.")]
        public decimal Temperatura { get; set; }
    }

    [MetadataType(typeof(LecturaMetadata))]
    public partial class Lectura
    {
    }
}