using System.ComponentModel.DataAnnotations;

namespace MonitorSerial.Models
{
    public class DispositivoMetadata
    {
        [Required(ErrorMessage = "El nombre del dispositivo es obligatorio.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El puerto COM es obligatorio.")]
        public string PuertoCOM { get; set; }
    }

    [MetadataType(typeof(DispositivoMetadata))]
    public partial class Dispositivo
    {
    }
}