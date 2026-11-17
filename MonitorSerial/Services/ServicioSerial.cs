using System;
using System.IO.Ports;
using System.Globalization;

namespace MonitorSerial.Services
{
    public class ServicioSerial : IDisposable
    {
        private SerialPort puerto;

        public string[] ObtenerPuertosDisponibles()
        {
            return SerialPort.GetPortNames();
        }

        public void Conectar(string nombrePuerto)
        {
            if (puerto != null)
            {
                puerto.Dispose();
                puerto = null;
            }

            puerto = new SerialPort(
                nombrePuerto,
                9600,
                Parity.None,
                8,
                StopBits.One
            );

            puerto.ReadTimeout = 3000;
            puerto.WriteTimeout = 3000;

            puerto.NewLine = "\n";

            puerto.Open();
        }

        public void Dispose()
        {
            if (puerto != null)
            {
                if (puerto.IsOpen)
                {
                    puerto.Close();
                }

                puerto.Dispose();
                puerto = null;
            }
        }

        public decimal? LeerTemperatura()
        {
            if (puerto == null || !puerto.IsOpen)
            {
                throw new InvalidOperationException(
                    "El puerto serial no está conectado."
                );
            }

            try
            {
                // Evitamos procesar respuestas anteriores.
                puerto.DiscardInBuffer();

                // Solicitamos la temperatura al sensor.
                puerto.WriteLine("T?");

                // Esperamos la respuesta.
                string respuesta = puerto.ReadLine().Trim();

                // Convertimos el texto recibido a decimal.
                if (decimal.TryParse(
                    respuesta,
                    NumberStyles.AllowLeadingSign |
                    NumberStyles.AllowDecimalPoint,
                    CultureInfo.InvariantCulture,
                    out decimal temperatura))
                {
                    return temperatura;
                }

                return null;
            }
            catch (TimeoutException)
            {
                return null;
            }
        }
    }
}