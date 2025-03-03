using ViandasDelSur.Models.Responses;

namespace ViandasDelSur.Repositories.Interfaces
{
    public interface IConfiguracionRepository
    {
        Configuracion GetConfiguracion();
        void UpdateMinimoPlatosDescuento(int nuevoValor);
    }
}

