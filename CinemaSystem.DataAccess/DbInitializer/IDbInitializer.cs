using System;
using System.Collections.Generic;
using System.Text;

namespace CinemaSystem.DataAccess.DbInitializer
{
    public interface IDbInitializer
    {
        Task InitializeAsync();
    }
}
