using System;
using ViandasDelSur.Models;
using ViandasDelSur.Models.Responses;
using ViandasDelSur.Repositories.Interfaces;



    namespace ViandasDelSur.Repositories.Implementations
    {
        public class ConfiguracionRepository : IConfiguracionRepository
        {
        private readonly VDSContext _context;

        public ConfiguracionRepository(VDSContext context)
        {
            _context = context;
        }

        public Configuracion GetConfiguracion()
            {
                return _context.Configuracion.FirstOrDefault() ?? new Configuracion();
            }

            public void UpdateMinimoPlatosDescuento(int nuevoValor)
            {
                var config = _context.Configuracion.FirstOrDefault();
                if (config != null)
                {
                    config.MinimoPlatosDescuento = nuevoValor;
                }
                else
                {
                    config = new Configuracion { MinimoPlatosDescuento = nuevoValor };
                    _context.Configuracion.Add(config);
                }
                _context.SaveChanges();
            }
        }
    }

