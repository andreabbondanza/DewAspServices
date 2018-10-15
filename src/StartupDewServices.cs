using DewCore.AspNetCore.Services;

namespace DewCore.AspNetCore.Services
{
    public class StartupDewServices
    {
        public StartupDewServices()
        {
            DewServices.GetServices();
        }
    }
}