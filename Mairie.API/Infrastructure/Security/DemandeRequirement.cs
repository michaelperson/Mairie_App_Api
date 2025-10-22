namespace Mairie.API.Infrastructure.Security
{
    public static class DemandeRequirement
    {
        public static DemandeOwnerRequirement Create = new DemandeOwnerRequirement("Create");
        public static DemandeOwnerRequirement Read = new DemandeOwnerRequirement("Read");

        public static DemandeOwnerRequirement Update = new DemandeOwnerRequirement("Update");

        public static DemandeOwnerRequirement Delete = new DemandeOwnerRequirement("Delete");
    }
}
