using Microsoft.EntityFrameworkCore;

namespace P5AspEntity;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Etudiant> Etudiants => Set<Etudiant>();
}