using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using simplePayloadWebApi.Models;

namespace simplePayloadWebApi.Data
{
    public class simplePayloadContext : DbContext
    {
        public simplePayloadContext (DbContextOptions<simplePayloadContext> options)
            : base(options)
        {
        }

        public DbSet<simplePayloadWebApi.Models.UserInteraction> UserInteraction { get; set; } = default!;
        public DbSet<simplePayloadWebApi.Models.Car> Cars { get; set; } = default!;

        public DbSet<simplePayloadWebApi.Models.CarUpload> CarUploads { get; set; } = default!;



    }
}
