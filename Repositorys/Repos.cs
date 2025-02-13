using Microsoft.EntityFrameworkCore;
using Repositorys.Repositorys;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;    

namespace Repositorys
{
    public class Repos
    {
        public class AddressRepository : BaseRepository<Addresses>
        {
            public AddressRepository(DbContext context) : base(context) { }
        }

        public class CityRepository : BaseRepository<Cities>
        {
            public CityRepository(DbContext context) : base(context) { }
        }

        public class ClientRepository : BaseRepository<Clients>
        {
            public ClientRepository(DbContext context) : base(context) { }
        }

        public class ContractRepository : BaseRepository<Contracts>
        {
            public ContractRepository(DbContext context) : base(context) { }
        }

        public class ContractStatusRepository : BaseRepository<ContractStatuses>
        {
            public ContractStatusRepository(DbContext context) : base(context) { }
        }

        public class CountryRepository : BaseRepository<Countries>
        {
            public CountryRepository(DbContext context) : base(context) { }
        }

        public class LocationRepository : BaseRepository<Locations>
        {
            public LocationRepository(DbContext context) : base(context) { }
        }

        public class OrderRepository : BaseRepository<Orders>
        {
            public OrderRepository(DbContext context) : base(context) { }
        }

        public class PaimentMethodRepository : BaseRepository<PaimentMethods>
        {
            public PaimentMethodRepository(DbContext context) : base(context) { }
        }

        public class RoleRepository : BaseRepository<Roles>
        {
            public RoleRepository(DbContext context) : base(context) { }
        }

        public class StatusRepository : BaseRepository<Statuses>
        {
            public StatusRepository(DbContext context) : base(context) { }
        }

        public class TourRepository : BaseRepository<Tours>
        {
            public TourRepository(DbContext context) : base(context) { }
        }

        public class TransactionRepository : BaseRepository<Transactions>
        {
            public TransactionRepository(DbContext context) : base(context) { }
        }

        public class UserRepository : BaseRepository<Users>
        {
            public UserRepository(DbContext context) : base(context) { }
        }
    }
}
