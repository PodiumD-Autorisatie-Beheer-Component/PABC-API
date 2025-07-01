using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PABC.Data;
using PABC.Data.Entities;
using PABC.Server.Auth;
using PABC.Server.Features.GetApplicationRolesPerEntityType;
using System.Net.Mime;

namespace PABC.Server.Features.Seed
{


    [ApiController]
    [Route("seed")]
    [Authorize(Policy = ApiKeyAuthentication.Policy)]
    public class SeedController(PabcDbContext db) : ControllerBase
    {

        [HttpGet]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized, MediaTypeNames.Application.ProblemJson)]
        public async Task<ActionResult<GetApplicationRolesResponse>> GetAsync()
        {
            var seedResult = await Seed();
            return Ok(new { seedDataStored = seedResult });

        }



        private async Task<bool> Seed()
        {
            if (await db.EntityTypes.AnyAsync() ||
              await db.Domains.AnyAsync() ||
              await db.ApplicationRoles.AnyAsync() ||
              await db.FunctionalRoles.AnyAsync() ||
              await db.Mappings.AnyAsync()) return false;

            var appRoleZac1 = new ApplicationRole { Id = Guid.NewGuid(), Application = "ZAC", Name = "Raadpleger" };
            var appRoleZac2 = new ApplicationRole { Id = Guid.NewGuid(), Application = "ZAC", Name = "Behandelaar" };
            var appRoleZac3 = new ApplicationRole { Id = Guid.NewGuid(), Application = "ZAC", Name = "Coördinator" };
            var appRoleZac4 = new ApplicationRole { Id = Guid.NewGuid(), Application = "ZAC", Name = "Recordmanager" };
            var appRoleZac5 = new ApplicationRole { Id = Guid.NewGuid(), Application = "ZAC", Name = "Beheerder" };

            var appRoleApp2 = new ApplicationRole { Id = Guid.NewGuid(), Application = "MyApplication2", Name = "MyAppRole2" };

            var funcRole1 = new FunctionalRole { Id = Guid.NewGuid(), Name = "GemeenteRol1" };
            var funcRole2 = new FunctionalRole { Id = Guid.NewGuid(), Name = "GemeenteRol2" };

            var entityType1 = new EntityType { Id = Guid.NewGuid(), EntityTypeId = Guid.NewGuid().ToString(), Type = "zaaktype", Name = "Melding klein kansspel" };
            var entityType2 = new EntityType { Id = Guid.NewGuid(), EntityTypeId = Guid.NewGuid().ToString(), Type = "zaaktype", Name = "Aanvraag ontheffing bedrijfsvoertuigen behandelen" };
            var entityType3 = new EntityType { Id = Guid.NewGuid(), EntityTypeId = Guid.NewGuid().ToString(), Type = "zaaktype", Name = "Aanvraag subsidieregeling Energieneutraal Ten Post behandelen" };
            var entityType4 = new EntityType { Id = Guid.NewGuid(), EntityTypeId = Guid.NewGuid().ToString(), Type = "zaaktype", Name = "Melding flyeren en samplen behandelen" };

            var domain1 = new Domain { Id = Guid.NewGuid(), Description = "Domein bedrijfsvoering gaat over bedrijfsvoering", Name = "Bedrijfsvoering", EntityTypes = [entityType1, entityType2, entityType3] };
            var domain2 = new Domain { Id = Guid.NewGuid(), Description = "Domein leefomgeving gaat over leefomgeving", Name = "Leefomgeving", EntityTypes = [entityType3, entityType4] };
            var domain3 = new Domain { Id = Guid.NewGuid(), Description = "Domein ruimte gaat over ruimte", Name = "Ruimte", EntityTypes = [entityType3, entityType4] };

            var mapping1 = new Mapping { Id = Guid.NewGuid(), ApplicationRole = appRoleZac1, Domain = domain1, FunctionalRole = funcRole1 };
            var mapping2 = new Mapping { Id = Guid.NewGuid(), ApplicationRole = appRoleZac2, Domain = domain2, FunctionalRole = funcRole2 };
            var mapping3 = new Mapping { Id = Guid.NewGuid(), ApplicationRole = appRoleZac3, Domain = domain1, FunctionalRole = funcRole2 };
            var mapping4 = new Mapping { Id = Guid.NewGuid(), ApplicationRole = appRoleZac4, Domain = domain2, FunctionalRole = funcRole1 };
            var mapping5 = new Mapping { Id = Guid.NewGuid(), ApplicationRole = appRoleZac5, Domain = domain3, FunctionalRole = funcRole1 };
            var mapping6 = new Mapping { Id = Guid.NewGuid(), ApplicationRole = appRoleApp2, Domain = domain3, FunctionalRole = funcRole1 };

            await db.AddRangeAsync(mapping1, mapping2, mapping3, mapping4, mapping5, mapping6);
            var result = await db.SaveChangesAsync();
            return result > 0;
        }

    }

}

